using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RoleTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        //Mock transport interface - 
        private static Mock<IConnectionRestCalls> _mockTransport;

        //Mock REST server
        private static ConnectionServerRest _mockServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            //create a connection server instance used for all tests - rather than using a mockup 
            //for fetching data I prefer this "real" testing approach using a public server I keep up
            //and available for the purpose - the conneciton information is stored in the test project's 
            //settings and can be changed to a local instance easily.
            Settings mySettings = new Settings();
            Thread.Sleep(300);
            try
            {
                _connectionServer = new ConnectionServerRest(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                    mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start Role test:" + ex.Message);
            }

            //setup mock server interface 
            _mockTransport = new Mock<IConnectionRestCalls>();

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                });

            _mockServer = new ConnectionServerRest(_mockTransport.Object, "test", "test", "test", false);

        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            Role oTest = new Role(null);
            Console.WriteLine(oTest);
        }

        
        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown for an invalid ObjectID
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            Role oTest = new Role(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown on an invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            Role oTest = new Role(_connectionServer,"","bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void FetchTests()
        {
            List<Role> oRoles;
            WebCallResult res= Role.GetRoles(_connectionServer, out oRoles);
            Assert.IsTrue(res.Success,"Failed to fetch roles list:"+res);
            Assert.IsTrue(oRoles.Count>0,"No roles returned from server");

            Console.WriteLine(oRoles[0].ToString());
            Console.WriteLine(oRoles[0].UniqueIdentifier);
            Console.WriteLine(oRoles[0].SelectionDisplayString);
            
            oRoles.Sort(new UnityDisplayObjectCompare());

            res = Role.GetRoles(null, out oRoles);
            Assert.IsFalse(res.Success,"Static fetch of rules did not fail with null ConnectionServer");
            
            string strObjectId = "";
            
            try
            {
                Role oNewRole = new Role(_connectionServer, "", "Audio Text Administrator");
                strObjectId = oNewRole.ObjectId;
                Assert.IsTrue(oNewRole.RoleName.Equals("Audio Text Administrator"));
            }
            catch (Exception ex)
            {
                Assert.Fail("Class construction with valid role name failed:"+ex);
            }

            try
            {
                Role oNewRole = new Role(_connectionServer, strObjectId);
                Assert.IsTrue(oNewRole.ObjectId.Equals(strObjectId));

            }
            catch (Exception ex)
            {
                Assert.Fail("Class construction with valid objectId failed:" + ex);
            }
        }

        #endregion


        #region Harness Tess
        
        // EmptyResultText, InvalidResultText, ErrorResponse, ReturnSpecificText[

        [TestMethod]
        public void GetRole_HarnessFailures()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            Role oRole;
            try
            {
                oRole = new Role(_mockServer, "EmptyResultText");
                Assert.Fail("Getting role with empty result text should fail");
            }
            catch {}

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            try
            {
                oRole = new Role(_mockServer, "InvalidResultText");
                Assert.Fail("Getting role with InvalidResultText should fail");
            }
            catch { }

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            try
            {
                oRole = new Role(_mockServer, "ErrorResponse");
                Assert.Fail("Getting role with ErrorResponse should fail");
            }
            catch { }
        }

        [TestMethod]
        public void GetRoles_HarnessFailures()
        {
            List<Role> oRoles;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = Role.GetRoles(_mockServer, out oRoles, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetRoles with EmptyResultText should fail");
            Assert.IsTrue(oRoles.Count == 0, "Empty result text shoudl produce empty list of roles");

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            res = Role.GetRoles(_mockServer, out oRoles, "InvalidResultText");
            Assert.IsFalse(res.Success, "Calling GetRoles with InvalidResultText should fail");
            Assert.IsTrue(oRoles.Count==0,"Invalid result text should produce empty list of roles");

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            res = Role.GetRoles(_mockServer, out oRoles, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetRoles with ErrorResponse should fail");
        }

        #endregion
    }
}
