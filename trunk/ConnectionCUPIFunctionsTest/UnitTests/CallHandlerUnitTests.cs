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


        #region Class Creation 

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

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        public void Constructor_EmptyConstructor_Success()
        {
            CallHandler oTestHandler = new CallHandler();
            Console.WriteLine(oTestHandler.ToString());
            Console.WriteLine(oTestHandler.DumpAllProps());
            Console.WriteLine(oTestHandler.UniqueIdentifier);
            Console.WriteLine(oTestHandler.SelectionDisplayString);
        }

        #endregion


        #region Static Method Call Tests

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
        public void GetCallHandlers_EmptyResult_Failure()
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
            var res = CallHandler.GetCallHandlers(_mockServer, out oHandlers, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetCallHandlers with EmptyResultText did not fail");

        }
        
        [TestMethod]
        public void GetCallHandlers_GarbageResponse_Failure()
        {
            List<CallHandler> oHandlers;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(),true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            var res = CallHandler.GetCallHandlers(_mockServer, out oHandlers, 1, 5, "InvalidResultText");
            Assert.IsFalse(res.Success, "Calling GetCallHandlers with garbage results should fail");
            Assert.IsTrue(oHandlers.Count == 0, "Invalid result text should produce an empty list");
        }

        
        [TestMethod]
        public void GetCallHandlers_ErrorResponse_Failure()
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
        public void GetCallHandlers_ZeroCount_Success()
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
        public void GetCallHandler_EmptyResponse_Failure()
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
        public void GetCallHandler_GarbageResponse_Failure()
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
        public void GetCallHandler_ErrorResponse_Failure()
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
        public void GetCallHandler_ZeroCount_Success()
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

        [TestMethod]
        public void AddCallHandler_JunkObjectIdReturn_Failure()
        {
            //invalid objectId response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                 It.IsAny<string>(),It.IsAny<bool>() )).Returns(new WebCallResult
                                 {
                                     Success = true,
                                     ResponseText = "/vmrest/handlers/callhandlers/junk"
                                 });

            CallHandler oHandler;
            var res = CallHandler.AddCallHandler(_mockServer, "templateid", "displayname", "1234", null, out oHandler);
            Assert.IsFalse(res.Success, "AddCallHandler that produces invalid new ObjectId did not fail");
        }

        [TestMethod]
        public void DeleteCallHandler_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                           {
                                               Success = false,
                                               ResponseText = "error text",
                                               StatusCode = 404
                                           });

            var res = CallHandler.DeleteCallHandler(_mockServer, "ObjectID");
            Assert.IsFalse(res.Success, "Calling DeleteCallHandler with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdateCallHandler_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var oProps = new ConnectionPropertyList();
            oProps.Add("Test","test");
            var res = CallHandler.UpdateCallHandler(_mockServer,"ObjectID",oProps);
            Assert.IsFalse(res.Success, "Calling UpdateCallHandler with ErrorResponse did not fail");
        }


        [TestMethod]
        public void GetCallHandlerVoiceName_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = CallHandler.GetCallHandlerVoiceName(_mockServer, "temp.wav","ObjectId","WavFileName");
            Assert.IsFalse(res.Success, "Calling GetCallHandlerVoiceName with ErrorResponse did not fail");
        }


        [TestMethod]
        public void SetCallHandlerVoiceName_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = CallHandler.SetCallHandlerVoiceName(_mockServer, "Dummy.wav", "ObjectId", true);
            Assert.IsFalse(res.Success, "Calling SetCallHandlerVoiceName with ErrorResponse did not fail");
        }

        [TestMethod]
        public void SetCallHandlerVoiceName_InvalidWavFile_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "dummy text",
                                    });

            var res = CallHandler.SetCallHandlerVoiceName(_mockServer, "moq.dll", "ObjectId", true);
            Assert.IsFalse(res.Success, "Calling SetCallHandlerVoiceName with invalid wav file did not fail");
        }

        [TestMethod]
        public void SetCallHandlerVoiceNameToStreamFile_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ErrorText = "Error text"
                                    });

            var res = CallHandler.SetCallHandlerVoiceNameToStreamFile(_mockServer,"ObjectId","StreamResourceName");
            Assert.IsFalse(res.Success, "Calling SetCallHandlerVoiceNameToStreamFile with error respone did not fail");
        }

        [TestMethod]
        public void RefetchCallHandlerData_EmptyHandler_Failure()
        {
            CallHandler oHandler = new CallHandler();
            var res = oHandler.RefetchCallHandlerData();
            Assert.IsFalse(res.Success,"Refetching data for an empty call handler should fail");
        }

        #endregion


        #region Property Tests

        [TestMethod]
        public void PropertyGetFetch_AfterMessageAction()
        {
            CallHandler oHandler = new CallHandler();
            const ActionTypes expectedValue = ActionTypes.Error;
            oHandler.AfterMessageAction = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("AfterMessageAction", (int)expectedValue),
                "AfterMessageAction value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_AfterMessageTargetConversation()
        {
            CallHandler oHandler = new CallHandler();
            const ConversationNames expectedValue = ConversationNames.GreetingsAdministrator;
            oHandler.AfterMessageTargetConversation = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("AfterMessageTargetConversation", expectedValue.ToString()),
                "AfterMessageTargetConversation value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_AfterMessageTargetHandlerObjectId()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "test string value";
            oHandler.AfterMessageTargetHandlerObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("AfterMessageTargetHandlerObjectId", expectedValue),
                "AfterMessageTargetConversation value get fetch failed");
        }

       

        [TestMethod]
        public void PropertyGetFetch_CallSearchSpaceObjectId()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "test string value";
            oHandler.CallSearchSpaceObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("CallSearchSpaceObjectId", expectedValue.ToString()),
                "CallSearchSpaceObjectId value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "test string value";
            oHandler.DisplayName = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("DisplayName", expectedValue.ToString()),
                "DisplayName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DispatchDelivery()
        {
            CallHandler oHandler = new CallHandler();
            const bool expectedValue = true;
            oHandler.DispatchDelivery = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("DispatchDelivery", expectedValue),
                "DispatchDelivery value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DtmfAccessId()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "12345";
            oHandler.DtmfAccessId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("DtmfAccessId", expectedValue.ToString()),
                "DtmfAccessId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_EditMsg()
        {
            CallHandler oHandler = new CallHandler();
            const bool expectedValue = true;
            oHandler.EditMsg = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("EditMsg", expectedValue),
                "EditMsg value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_EnablePrependDigits()
        {
            CallHandler oHandler = new CallHandler();
            const bool expectedValue = true;
            oHandler.EnablePrependDigits = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("EnablePrependDigits", expectedValue),
                "EnablePrependDigits value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_InheritSearchSpaceFromCall()
        {
            CallHandler oHandler = new CallHandler();
            const bool expectedValue = true;
            oHandler.InheritSearchSpaceFromCall = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("InheritSearchSpaceFromCall", expectedValue),
                "InheritSearchSpaceFromCall value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_Language()
        {
            CallHandler oHandler = new CallHandler();
            const int expectedValue = 1036;
            oHandler.Language = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("Language", expectedValue),
                "Language value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MaxMsgLen()
        {
            CallHandler oHandler = new CallHandler();
            const int expectedValue = 555;
            oHandler.MaxMsgLen = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("MaxMsgLen", expectedValue),
                "MaxMsgLen value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_MediaSwitchObjectId()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "Test string value";
            oHandler.MediaSwitchObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("MediaSwitchObjectId", expectedValue.ToString()),
                "MediaSwitchObjectId value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_OneKeyDelay()
        {
            CallHandler oHandler = new CallHandler();
            const int expectedValue = 123;
            oHandler.OneKeyDelay = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("OneKeyDelay", expectedValue),
                          "OneKeyDelay value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PartitionObjectId()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "Test string value";
            oHandler.PartitionObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("PartitionObjectId", expectedValue.ToString()),
                "PartitionObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PlayAfterMessage()
        {
            CallHandler oHandler = new CallHandler();
            const PlayAfterMessageTypes expectedValue = PlayAfterMessageTypes.Recorded;
            oHandler.PlayAfterMessage = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("PlayAfterMessage", (int)expectedValue),
                "PlayAfterMessage value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PostGreetingRecordingObjectId()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "Test string value";
            oHandler.PostGreetingRecordingObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("PostGreetingRecordingObjectId", expectedValue.ToString()),
                "PostGreetingRecordingObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PrependDigits()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "544332";
            oHandler.PrependDigits = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("PrependDigits", expectedValue.ToString()),
                "PrependDigits value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RecipientDistributionListObjectId()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "Test string value";
            oHandler.RecipientDistributionListObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("RecipientDistributionListObjectId", expectedValue.ToString()),
                "RecipientDistributionListObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RecipientSubscriberObjectId()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "Test string value";
            oHandler.RecipientSubscriberObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("RecipientSubscriberObjectId", expectedValue.ToString()),
                "RecipientSubscriberObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ScheduleSetObjectId()
        {
            CallHandler oHandler = new CallHandler();
            const string expectedValue = "Test string value";
            oHandler.ScheduleSetObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ScheduleSetObjectId", expectedValue.ToString()),
                "ScheduleSetObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SendPrivateMsg()
        {
            CallHandler oHandler = new CallHandler();
            const ModeYesNoAsk expectedValue = ModeYesNoAsk.Ask; 
            oHandler.SendPrivateMsg = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SendPrivateMsg", (int)expectedValue),
                "SendPrivateMsg value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SendSecureMsg()
        {
            CallHandler oHandler = new CallHandler();
            const bool expectedValue = true;
            oHandler.SendSecureMsg = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SendSecureMsg", expectedValue),
                "SendSecureMsg value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SendUrgentMsg()
        {
            CallHandler oHandler = new CallHandler();
            const ModeYesNoAsk expectedValue = ModeYesNoAsk.Ask;
            oHandler.SendUrgentMsg = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SendUrgentMsg", (int)expectedValue),
                "SendUrgentMsg value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TimeZone()
        {
            CallHandler oHandler = new CallHandler();
            const int expectedValue = 77;
            oHandler.TimeZone = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("TimeZone", expectedValue),
                "TimeZone value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Undeletable()
        {
            CallHandler oHandler = new CallHandler();
            const bool expectedValue = true;
            oHandler.Undeletable = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("Undeletable", expectedValue),
                "Undeletable value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_UseCallLanguage()
        {
            CallHandler oHandler = new CallHandler();
            const bool expectedValue = true;
            oHandler.UseCallLanguage = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("UseCallLanguage", expectedValue),
                "UseCallLanguage value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_UseDefaultLanguage()
        {
            CallHandler oHandler = new CallHandler();
            const bool expectedValue = true;
            oHandler.UseDefaultLanguage = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("UseDefaultLanguage", expectedValue),
                "UseDefaultLanguage value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_UseDefaultTimeZone()
        {
            CallHandler oHandler = new CallHandler();
            const bool expectedValue = true;
            oHandler.UseDefaultTimeZone = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("UseDefaultTimeZone", expectedValue),
                "UseDefaultTimeZone value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_GetTransferOptions_NullReturn()
        {
            CallHandler oHandler = new CallHandler();

            var oTransferOptions = oHandler.GetTransferOptions(true);
            Assert.IsNull(oTransferOptions,"Fetching transfer options from empty call handler should return null list");
        }

        [TestMethod]
        public void PropertyGetFetch_GetGreetings_NullReturn()
        {
            CallHandler oHandler = new CallHandler();

            var oGreetings = oHandler.GetGreetings(true);
            Assert.IsNull(oGreetings, "Fetching greetings from empty call handler should return null list");
        }

        [TestMethod]
        public void PropertyGetFetch_GetMenuEntries_NullReturn()
        {
            CallHandler oHandler = new CallHandler();

            var oMenuEntries = oHandler.GetMenuEntries(true);
            Assert.IsNull(oMenuEntries, "Fetching menu entries from empty call handler should return null list");
        }

        [TestMethod]
        public void PropertyGetFetch_GetScheduleSet_NullReturn()
        {
            CallHandler oHandler = new CallHandler();

            var oSchedules = oHandler.GetScheduleSet(true);
            Assert.IsNull(oSchedules, "Fetching schedules from empty call handler should return null list");
        }


        #endregion
    }
}
