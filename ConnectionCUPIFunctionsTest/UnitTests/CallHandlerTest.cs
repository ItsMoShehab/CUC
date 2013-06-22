using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for CallHandlerTest and is intended
    ///to contain all CallHandlerTest Unit Tests
    ///</summary>
    [TestClass]
    public class CallHandlerTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

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


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            CallHandler oTestHandler = new CallHandler(null);
            Console.WriteLine(oTestHandler);
        }


        #endregion


        #region Static Method Call Failure Tests

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_GetCallHandlers()
        {
            List<CallHandler> oHandlerList;

            WebCallResult res = CallHandler.GetCallHandlers(null, out oHandlerList, null);
            Assert.IsFalse(res.Success, "GetHandler should fail with null ConnectionServerRest passed to it");

        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_GetCallHandler()
        {
            CallHandler oHandler;

            WebCallResult res = CallHandler.GetCallHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetCallHandler should fail if the ConnectionServerRest is null");

            res = CallHandler.GetCallHandler(out oHandler, _mockServer);
            Assert.IsFalse(res.Success, "GetCallHandler should fail if the ObjectId and display name are both blank");
        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_AddCallHandler()
        {
            WebCallResult res = CallHandler.AddCallHandler(null, "", "", "", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the ConnectionServerRest parameter is null");

            res = CallHandler.AddCallHandler(_mockServer, "", "aaa", "123", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the template parameter is empty");

            res = CallHandler.AddCallHandler(_mockServer, "voicemailtemplate", "aaa", "", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the extension parameter is empty");

            res = CallHandler.AddCallHandler(_mockServer, "voicemailtemplate", "", "1234", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the DisplayName parameter is empty");
        }



        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_DeleteCallHandler()
        {
            WebCallResult res = CallHandler.DeleteCallHandler(null, "");
            Assert.IsFalse(res.Success, "DeleteCallHandler should fail if the ConnectionServerRest parameter is null");

            res = CallHandler.DeleteCallHandler(_mockServer, "");
            Assert.IsFalse(res.Success, "DeleteCallHandler should fail if the ObjectId parameter is blank");
        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_UpdateCallHandler()
        {
            ConnectionPropertyList oPropList = new ConnectionPropertyList();

            WebCallResult res = CallHandler.UpdateCallHandler(null, "", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the ConnectionServerRest parameter is null");

            res = CallHandler.UpdateCallHandler(_mockServer, "", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the ObjectId parameter is blank");

            res = CallHandler.UpdateCallHandler(_mockServer, "aaa", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the property list is empty");
        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_GetCallHandlerVoiceName()
        {
            //use the same string for the alias and display name here
            const string strWavName = @"c:\";

            //invalid local WAV file name
            WebCallResult res = CallHandler.GetCallHandlerVoiceName(null, "aaa", "");
            Assert.IsFalse(res.Success, "GetCallHandlerVoiceName did not fail for null Conneciton server");

            //empty target file path
            res = CallHandler.GetCallHandlerVoiceName(_mockServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "GetCallHandlerVoiceName did not fail with invalid target path passed");

            //invalid objectId 
            res = CallHandler.GetCallHandlerVoiceName(_mockServer, "", strWavName);
            Assert.IsFalse(res.Success, "GetCallHandlerVoiceName did not fail with invalid ObjectId passed");


        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_SetCallHandlerVoiceName()
        {
            //use the same string for the alias and display name here
            const string strWavName = @"c:\";

            //invalid Connection server
            WebCallResult res = CallHandler.SetCallHandlerVoiceName(null, "", "");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with null Connection server passed.");

            //invalid target path
            res = CallHandler.SetCallHandlerVoiceName(_mockServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with invalid target path");

            //invalid ObjectId
            res = CallHandler.SetCallHandlerVoiceName(_mockServer, strWavName, "");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with invalid obejctID");

        }

        [TestMethod]
        public void StaticCallFailure_SetCallHandlerVoiceNameToStreamFile()
        {
            var res = CallHandler.SetCallHandlerVoiceNameToStreamFile(null, "objectid", "StreamId");
            Assert.IsFalse(res.Success,"Calling SetCallHandlerVoiceNameToStreamFile with null ConnectionServerRest did not fail");

            res = CallHandler.SetCallHandlerVoiceNameToStreamFile(_mockServer, "objectid", "StreamId");
            Assert.IsFalse(res.Success, "Calling SetCallHandlerVoiceNameToStreamFile with invalid ObjectId did not fail");

            res = CallHandler.SetCallHandlerVoiceNameToStreamFile(_mockServer, "", "StreamId");
            Assert.IsFalse(res.Success, "Calling SetCallHandlerVoiceNameToStreamFile with empty objectId did not fail");

            res = CallHandler.SetCallHandlerVoiceNameToStreamFile(_mockServer, "ObjectId", "");
            Assert.IsFalse(res.Success, "Calling SetCallHandlerVoiceNameToStreamFile with empty stream ID did not fail");

        }


        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetCallHandlers_Harness_EmptyResult()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            List<CallHandler> oHandlers;
            var res = CallHandler.GetCallHandlers(_mockServer, out oHandlers, 1, 5, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetCallHandlers with EmptyResultText did not fail");

        }
        
        [TestMethod]
        public void GetCallHandlers_Harness_GarbageResponse()
        {
            List<CallHandler> oHandlers;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(),true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result"
                                  });

            var res = CallHandler.GetCallHandlers(_mockServer, out oHandlers, 1, 5, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetCallHandlers with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oHandlers.Count == 0, "Invalid result text should produce an empty list");
        }

        
        [TestMethod]
        public void GetCallHandlers_Harness_ErrorResponse()
        {
            List<CallHandler> oHandlers;
        
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(),true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = CallHandler.GetCallHandlers(_mockServer, out oHandlers, 1, 5, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetCallHandlers with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetCallHandlers_Harness_ZeroCount()
        {
            List<CallHandler> oHandlers;

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            var res = CallHandler.GetCallHandlers(_mockServer, out oHandlers, 1, 5, "objectid");
            Assert.IsTrue(res.Success, "Calling GetCallHandlers with ZeroCount failed:"+res);
        }

        [TestMethod]
        public void GetCallHandler_Harness_EmptyResponse()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            CallHandler oHandler;
            var res = CallHandler.GetCallHandler(out oHandler, _mockServer, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetCallHandler with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetCallHandler_Harness_GarbageResponse()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result"
                                  });

            CallHandler oHandler;
            var res = CallHandler.GetCallHandler(out oHandler, _mockServer, "InvalidResultText");
            Assert.IsFalse(res.Success, "Calling GetCallHandler with InvalidResultText should fail");

            }

        [TestMethod]
        public void GetCallHandler_Harness_ErrorResponse()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            CallHandler oHandler;
            var res = CallHandler.GetCallHandler(out oHandler, _mockServer, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetCallHandler with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetCallHandler_Harness_ZeroCount()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "garbage",
                                        TotalObjectCount = 0
                                    });

            CallHandler oHandler;
            var res = CallHandler.GetCallHandler(out oHandler, _mockServer, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetCallHandler with zero count result did not fail");
        }
        #endregion
    }
}
