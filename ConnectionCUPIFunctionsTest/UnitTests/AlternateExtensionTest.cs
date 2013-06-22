using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for AlternateExtensionTest and is intended
    ///to contain all AlternateExtensionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AlternateExtensionTest
    {

        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //Mock transport interface - 
        private static Mock<IConnectionRestCalls> _mockTransport = new Mock<IConnectionRestCalls>();

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
            //setup mock server interface 
            _mockTransport = new Mock<IConnectionRestCalls>();

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                });

            try
            {
                _mockServer = new ConnectionServerRest(_mockTransport.Object, "test", "test", "test", false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed creating mock server instance:"+ex);
            }
        }

        #endregion

        
        #region Class Construction Errors

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            AlternateExtension oTemp = new AlternateExtension(null, "aaa");
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ObjectId is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            AlternateExtension oTemp = new AlternateExtension(new ConnectionServerRest(new RestTransportFunctions()), "");
        }

        #endregion


        #region Static Call Failures

        /// <summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_DeleteAlternateExtension()
        {
            //static delete failure calls
            var res = AlternateExtension.DeleteAlternateExtension(_mockServer, "objectid", "");
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(_mockServer, "", "aaa");
            Assert.IsFalse(res.Success, "Empty user objectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(null, "objectid", "aaa");
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

        }

        ///<summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetAlternateExtensions()
        {
            List<AlternateExtension> oAltExts;

            //static GetAlternateExtensions calls
            var res = AlternateExtension.GetAlternateExtensions(null, "objectid", out oAltExts);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

        }

        ///<summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetAlternateExtension()
        {
            AlternateExtension oAltExt;

            var res = AlternateExtension.GetAlternateExtension(null, "objectid", "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Null ConnectonServer object should fail");

            res = AlternateExtension.GetAlternateExtension(_mockServer, "", "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Empty UServerObjectID should fail");
        }


        ///<summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateAlternateExtension()
        {
            //static UpdateAlternateExtension calls
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("name", "value");

            var res = AlternateExtension.UpdateAlternateExtension(null, "objectid", "aaa", oProps);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

            res = AlternateExtension.UpdateAlternateExtension(_mockServer, "", "aaa", oProps);
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail");

            res = AlternateExtension.UpdateAlternateExtension(_mockServer, "objectid", "aaa", null);
            Assert.IsFalse(res.Success, "Empty property list should fail");
        }

        /// <summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_AddAlternateExtension()
        {
            var res = AlternateExtension.AddAlternateExtension(null,"objectid", 1, "1234");
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = AlternateExtension.AddAlternateExtension(_mockServer, "", 1, "1234");
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail");

            res = AlternateExtension.AddAlternateExtension(_mockServer, "objectid", 1, "");
            Assert.IsFalse(res.Success, "Empty extension string should fail");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetAlternateExtension_Harness_ErrorResponse()
        {
            AlternateExtension oAltExt;

            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = false,
                                               ResponseText = "error text",
                                               StatusCode = 404
                                           });

            var res = AlternateExtension.GetAlternateExtension(_mockServer, "userObjectID", "ErrorResponse", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with server error response should fail");

        }
        [TestMethod]
        public void GetAlternateExtension_Harness_GarbageResponse()
        {
            AlternateExtension oAltExt;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            var res = AlternateExtension.GetAlternateExtension(_mockServer, "userObjectID", "InvalidResultText", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with InvalidResultText should fail");
         }

        [TestMethod]
        public void GetAlternateExtension_Harness_EmptyResult()
        {
            AlternateExtension oAltExt;
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

           var res = AlternateExtension.GetAlternateExtension(_mockServer, "userObjectID", "EmptyResultText", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with EmptyResultText should fail");
        }

        [TestMethod]
        public void GetAlternateExtension_Harness_ZeroCount()
        {
            AlternateExtension oAltExt;
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "Garbage",
                    TotalObjectCount = 0
                });

            var res = AlternateExtension.GetAlternateExtension(_mockServer, "userObjectID", "EmptyResultText", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with zero object count should fail");
        }



        [TestMethod]
        public void GetAlternateExtensions_Harness_ErrorResponse()
        {
            List<AlternateExtension> oAltExts;

            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = false,
                                               ResponseText = "error text",
                                               StatusCode = 404
                                           });

            var res = AlternateExtension.GetAlternateExtensions(_mockServer, "ErrorResponse", out oAltExts);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtensions with server error response should fail");

        }

        [TestMethod]
        public void GetAlternateExtensions_Harness_GarbageResponse()
        {
            List<AlternateExtension> oAltExts;
            //garbage response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               TotalObjectCount = 1,
                                               ResponseText = "garbage result"
                                           });

            var res = AlternateExtension.GetAlternateExtensions(_mockServer, "InvalidResultText", out oAltExts);
            Assert.IsTrue(res.Success, "Calling GetAlternateExtensions with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oAltExts.Count == 0,
                          "Invalid text should result in empty list of alternate extensions being returned");

        }

        [TestMethod]
        public void GetAlternateExtensions_Harness_EmptyResponse()
        {
            List<AlternateExtension> oAltExts;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = AlternateExtension.GetAlternateExtensions(_mockServer, "EmptyResultText", out oAltExts);
            Assert.IsTrue(res.Success, "Calling GetAlternateExtensions with EmptyResultText should not fail:"+res);
            Assert.IsTrue(oAltExts.Count == 0, "Empty text should result in empty list of alternate extensions being returned");
        }

        [TestMethod]
        public void GetAlternateExtensions_Harness_ZeroCountResponse()
        {
            List<AlternateExtension> oAltExts;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "garbage result",
                    TotalObjectCount = 0
                });

            var res = AlternateExtension.GetAlternateExtensions(_mockServer, "EmptyResultText", out oAltExts);
            Assert.IsTrue(res.Success, "Calling GetAlternateExtensions with zero count should not fail:" + res);
            Assert.IsTrue(oAltExts.Count == 0, "Zero count response should result in empty list of alternate extensions being returned");
        }

        #endregion
    }
}
