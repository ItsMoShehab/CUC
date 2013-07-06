using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for CallHandlerUnitTests and is intended
    ///to contain all CallHandlerUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class CallHandlerUnitTests : BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            CallHandler oTestHandler = new CallHandler(null);
            Console.WriteLine(oTestHandler);
        }


        #endregion


        #region Static Method Call Failure Tests

        [TestMethod]
        public void GetCallHandlers_NullConnectionServer_Failure()
        {
            List<CallHandler> oHandlerList;

            WebCallResult res = CallHandler.GetCallHandlers(null, out oHandlerList, null);
            Assert.IsFalse(res.Success, "GetHandler should fail with null ConnectionServerRest passed to it");

        }


        [TestMethod]
        public void GetCallHandler_NullConnectionServer_Failure()
        {
            CallHandler oHandler;

            WebCallResult res = CallHandler.GetCallHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetCallHandler should fail if the ConnectionServerRest is null");
        }


        [TestMethod]
        public void GetCallHandler_EmptyObjectIdAndDisplayName_Failure()
        {
            CallHandler oHandler;

            var res = CallHandler.GetCallHandler(out oHandler, _mockServer);
            Assert.IsFalse(res.Success, "GetCallHandler should fail if the ObjectId and display name are both blank");
        }


        [TestMethod]
        public void AddCallHandler_NullConnectionServer_Failure()
        {
            WebCallResult res = CallHandler.AddCallHandler(null, "", "", "", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the ConnectionServerRest parameter is null");

         }


        [TestMethod]
        public void AddCallHandler_EmptyTemplateId_Failure()
        {
            var res = CallHandler.AddCallHandler(_mockServer, "", "aaa", "123", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the template parameter is empty");

            }


        [TestMethod]
        public void AddCallHandler_EmptyExtension_Failure()
        {
            var res = CallHandler.AddCallHandler(_mockServer, "voicemailtemplate", "aaa", "", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the extension parameter is empty");

            }


        [TestMethod]
        public void AddCallHandler_EmptyDisplayName_Failure()
        {
            var res = CallHandler.AddCallHandler(_mockServer, "voicemailtemplate", "", "1234", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the DisplayName parameter is empty");
        }


        [TestMethod]
        public void DeleteCallHandler_NullConnectionServer_Failure()
        {
            WebCallResult res = CallHandler.DeleteCallHandler(null, "");
            Assert.IsFalse(res.Success, "DeleteCallHandler should fail if the ConnectionServerRest parameter is null");

         }

        [TestMethod]
        public void DeleteCallHandler_EmptyObjectId_Failure()
        {
            var res = CallHandler.DeleteCallHandler(_mockServer, "");
            Assert.IsFalse(res.Success, "DeleteCallHandler should fail if the ObjectId parameter is blank");
        }


        [TestMethod]
        public void UpdateCallHandler_NullConnectionServer_Failure()
        {
            ConnectionPropertyList oPropList = new ConnectionPropertyList();

            WebCallResult res = CallHandler.UpdateCallHandler(null, "", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the ConnectionServerRest parameter is null");
         }


        [TestMethod]
        public void UpdateCallHandler_EmptyObjectId_Failure()
        {
            ConnectionPropertyList oPropList = new ConnectionPropertyList();

            var res = CallHandler.UpdateCallHandler(_mockServer, "", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the ObjectId parameter is blank");
         }


        [TestMethod]
        public void UpdateCallHandler_EmptyPropertyList_Failure()
        {
            ConnectionPropertyList oPropList = new ConnectionPropertyList();
            var res = CallHandler.UpdateCallHandler(_mockServer, "aaa", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the property list is empty");
        }

        [TestMethod]
        public void GetCallHandlerVoiceName_NullConnectionServer_Failure()
        {
            var res = CallHandler.GetCallHandlerVoiceName(null, "aaa", "");
            Assert.IsFalse(res.Success, "GetCallHandlerVoiceName did not fail for null Conneciton server");
        }

        [TestMethod]
        public void GetCallHandlerVoiceName_InvalidTargetPath_Failure()
        {
            var res = CallHandler.GetCallHandlerVoiceName(_mockServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "GetCallHandlerVoiceName did not fail with invalid target path passed");
        }

        [TestMethod]
        public void GetCallHandlerVoiceName_EmptyObjectId_Failure()
        {
            const string strWavName = @"c:\";

            var res = CallHandler.GetCallHandlerVoiceName(_mockServer, "", strWavName);
            Assert.IsFalse(res.Success, "GetCallHandlerVoiceName did not fail with empty ObjectId passed");
        }

        [TestMethod]
        public void SetCallHandlerVoiceName_NullConnectionServer_Failure()
        {
            //invalid Connection server
            WebCallResult res = CallHandler.SetCallHandlerVoiceName(null, "", "");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with null Connection server passed.");
         }

        [TestMethod]
        public void SetCallHandlerVoiceName_InvalidTargetPath_Failure()
        {
            //invalid target path
            var res = CallHandler.SetCallHandlerVoiceName(_mockServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with invalid target path");
        }

        [TestMethod]
        public void SetCallHandlerVoiceName_EmptyObjectId_Failure()
        {
            const string strWavName = @"c:\";
            var res = CallHandler.SetCallHandlerVoiceName(_mockServer, strWavName, "");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with empty obejctID");
        }


        [TestMethod]
        public void SetCallHandlerVoiceNameToStreamFile_NullConnectionServer_Failure()
        {
            var res = CallHandler.SetCallHandlerVoiceNameToStreamFile(null, "objectid", "StreamId");
            Assert.IsFalse(res.Success,"Calling SetCallHandlerVoiceNameToStreamFile with null ConnectionServerRest did not fail");
         }


        [TestMethod]
        public void SetCallHandlerVoiceNameToStreamFile_EmptyObjectId_Failure()
        {
            var res = CallHandler.SetCallHandlerVoiceNameToStreamFile(_mockServer, "", "StreamId");
            Assert.IsFalse(res.Success, "Calling SetCallHandlerVoiceNameToStreamFile with empty objectId did not fail");
        }


        [TestMethod]
        public void SetCallHandlerVoiceNameToStreamFile_EmptyStreamId_Failure()
        {
            var res = CallHandler.SetCallHandlerVoiceNameToStreamFile(_mockServer, "ObjectId", "");
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
