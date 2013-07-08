using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RoleUnitTests : BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
           BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            Role oTest = new Role(null);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Harness Tess
        
        // EmptyResultText, InvalidResultText, ErrorResponse, ReturnSpecificText[

        [TestMethod]
        public void Role_Constructor_EmptyResponse_Failure()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            try
            {
                Role oRole = new Role(_mockServer, "EmptyResultText");
                Assert.Fail("Getting role with empty result text should fail");
            }
            catch
            {
            }
        }

        [TestMethod]
        public void Role_Constructor_GarbageResponse_Failure()
        {

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            try
            {
                Role oRole = new Role(_mockServer, "InvalidResultText");
                Assert.Fail("Getting role with InvalidResultText should fail");
            }
            catch { }

            }

        [TestMethod]
        public void Role_Constructor_ErrorResponse_Failure()
        {
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
                Role oRole = new Role(_mockServer, "ErrorResponse");
                Assert.Fail("Getting role with ErrorResponse should fail");
            }
            catch { }
        }

        [TestMethod]
        public void GetRoles_EmptyResponse_Failure()
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

            }

        [TestMethod]
        public void GetRoles_GarbageResponse_Failure()
        {
            List<Role> oRoles;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            var res = Role.GetRoles(_mockServer, out oRoles, "InvalidResultText");
            Assert.IsFalse(res.Success, "Calling GetRoles with InvalidResultText should fail");
            Assert.IsTrue(oRoles.Count==0,"Invalid result text should produce empty list of roles");

            }

        [TestMethod]
        public void GetRoles_ErrorResponse_Failure()
        {
            List<Role> oRoles;
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = Role.GetRoles(_mockServer, out oRoles, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetRoles with ErrorResponse should fail");
        }

        #endregion
    }
}
