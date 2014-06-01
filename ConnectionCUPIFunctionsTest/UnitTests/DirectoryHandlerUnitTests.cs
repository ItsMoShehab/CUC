using System;
using System.Collections.Generic;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cisco.UnityConnection.RestFunctions;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for DirectoryHandlerUnitTests and is intended
    ///to contain all DirectoryHandler Unit Tests
    ///</summary>
    [TestClass]
    public class DirectoryHandlerUnitTests : BaseUnitTests
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
            DirectoryHandler oTestHandler = new DirectoryHandler(null);
            Console.WriteLine(oTestHandler);
        }

        [TestMethod]
        public void Constructor_ObjectId_Success()
        {
           BaseUnitTests.ClassInitialize(null);
            DirectoryHandler oTestHandler = new DirectoryHandler(_mockServer,"ObjectId");
            Console.WriteLine(oTestHandler);
        }

        [TestMethod]
        public void Constructor_EmptyObjectIdAndName_Success()
        {
            DirectoryHandler oTestHandler = new DirectoryHandler(_mockServer, "");
            Console.WriteLine(oTestHandler.ToString());
            Console.WriteLine(oTestHandler.DumpAllProps());
            Console.WriteLine(oTestHandler.SelectionDisplayString);
            Console.WriteLine(oTestHandler.UniqueIdentifier);
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            DirectoryHandler oTestHandler = new DirectoryHandler();
            Console.WriteLine(oTestHandler);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_ObjectId_EmptyResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), true)).Returns(new WebCallResult
                                   {
                                       Success = true,
                                       ResponseText = "",
                                       TotalObjectCount = 1
                                   });
            DirectoryHandler oTestHandler = new DirectoryHandler(_mockServer, "ObjectId");
            Console.WriteLine(oTestHandler);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_ObjectId_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), true)).Returns(new WebCallResult
                                   {
                                       Success = true,
                                       ResponseText = "garbage response body that will not be parsed out as JSON for directory handlers",
                                       TotalObjectCount = 1
                                   });
            DirectoryHandler oTestHandler = new DirectoryHandler(_mockServer, "ObjectId");
            Console.WriteLine(oTestHandler);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_DisplayNameNotFound_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "junk response",
                                      TotalObjectCount = 0
                                  });
            DirectoryHandler oTestHandler = new DirectoryHandler(_mockServer, "","Display Name");
            Console.WriteLine(oTestHandler);
        }
        #endregion


        #region Property Tests

        [TestMethod]
        public void PropertyGetFetch_GetGreetingStreamFiles()
        {
            List<DirectoryHandlerGreetingStreamFile> oStreams;
            DirectoryHandler oHandler = new DirectoryHandler();
            oStreams= oHandler.GetGreetingStreamFiles(true);
            Assert.IsNull(oStreams,"Fetching greeting streas from an empty class should return null");
        }


        [TestMethod]
        public void PropertyGetFetch_AutoRoute()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.AutoRoute = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("AutoRoute", expectedValue),"AutoRoute value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "String test";
            oHandler.DisplayName = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("DisplayName", expectedValue), "DisplayName value get fetch failed");
        }
        
        [TestMethod]
        public void PropertyGetFetch_DtmfAccessId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "String test";
            oHandler.DtmfAccessId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("DtmfAccessId", expectedValue), "DtmfAccessId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_EndDialDelay()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const int expectedValue = 123;
            oHandler.EndDialDelay = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("EndDialDelay", expectedValue), "EndDialDelay value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Language()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const int expectedValue = 456;
            oHandler.Language = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("Language", expectedValue), "Language value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MaxMatches()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const int expectedValue = 99;
            oHandler.MaxMatches = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("MaxMatches", expectedValue), "MaxMatches value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MenuStyle()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.MenuStyle = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("MenuStyle", expectedValue), "MenuStyle value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PartitionObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.PartitionObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("PartitionObjectId", expectedValue), "PartitionObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PlayAllNames()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.PlayAllNames = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("PlayAllNames", expectedValue), "PlayAllNames value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SayCity()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.SayCity = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SayCity", expectedValue), "SayCity value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SayDepartment()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.SayDepartment = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SayDepartment", expectedValue), "SayDepartment value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SayExtension()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.SayExtension = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SayExtension", expectedValue), "SayExtension value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SearchByFirstName()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.SearchByFirstName = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SearchByFirstName", expectedValue), "SearchByFirstName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SearchScope()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const DirectoryHandlerSearchScope expectedValue = DirectoryHandlerSearchScope.DialingDomain;
            oHandler.SearchScope = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SearchScope", (int)expectedValue), "SearchScope value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SearchScopeObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.SearchScopeObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SearchScopeObjectId", expectedValue), "SearchScopeObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SpeechConfidenceThreshold()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const int expectedValue = 222;
            oHandler.SpeechConfidenceThreshold = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("SpeechConfidenceThreshold", expectedValue), "SpeechConfidenceThreshold value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_StartDialDelay()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const int expectedValue = 123;
            oHandler.StartDialDelay = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("StartDialDelay", expectedValue), "StartDialDelay value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ScopeObjectCosObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.ScopeObjectCosObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ScopeObjectCosObjectId", expectedValue), "ScopeObjectCosObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ScopeObjectLocationObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.ScopeObjectLocationObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ScopeObjectLocationObjectId", expectedValue), 
                "ScopeObjectLocationObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ScopeObjectDistributionListObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.ScopeObjectDistributionListObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ScopeObjectDistributionListObjectId", expectedValue), 
                "ScopeObjectDistributionListObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ScopeObjectSearchSpaceObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.ScopeObjectSearchSpaceObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ScopeObjectSearchSpaceObjectId", expectedValue), 
                "ScopeObjectSearchSpaceObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Tries()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const int expectedValue = 22;
            oHandler.Tries = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("Tries", expectedValue), "Tries value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_UseCustomGreeting()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.UseCustomGreeting = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("UseCustomGreeting", expectedValue), "UseCustomGreeting value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Undeletable()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.Undeletable = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("Undeletable", expectedValue), "Undeletable value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_UseDefaultLanguage()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.UseDefaultLanguage = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("UseDefaultLanguage", expectedValue), "UseDefaultLanguage value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_UseCallLanguage()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.UseCallLanguage = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("UseCallLanguage", expectedValue), "UseCallLanguage value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_UseStarToExit()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const bool expectedValue = true;
            oHandler.UseStarToExit = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("UseStarToExit", expectedValue), "UseStarToExit value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ExitAction()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const ActionTypes expectedValue = ActionTypes.Hangup ;
            oHandler.ExitAction = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ExitAction", (int)expectedValue), "ExitAction value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ExitTargetConversatione()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const ConversationNames expectedValue = ConversationNames.SubSignIn;
            oHandler.ExitTargetConversation = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ExitTargetConversation", expectedValue.ToString()), 
                "ExitTargetConversation value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ExitTargetHandlerObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.ExitTargetHandlerObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ExitTargetHandlerObjectId", expectedValue),
                "ExitTargetHandlerObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_NoInputAction()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const ActionTypes expectedValue = ActionTypes.RestartGreeting;
            oHandler.NoInputAction = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("NoInputAction", (int)expectedValue),"NoInputAction value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_NoInputTargetConversation()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const ConversationNames expectedValue = ConversationNames.PHTransfer;
            oHandler.NoInputTargetConversation = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("NoInputTargetConversation", expectedValue.ToString()),
                "NoInputTargetConversation value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_NoInputTargetHandlerObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.NoInputTargetHandlerObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("NoInputTargetHandlerObjectId", expectedValue),
                "NoInputTargetHandlerObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_NoSelectionAction()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const ActionTypes expectedValue = ActionTypes.TransferToAlternateContactNumber;
            oHandler.NoSelectionAction = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("NoSelectionAction", (int)expectedValue),
                "NoSelectionAction value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_NoSelectionTargetConversation()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const ConversationNames expectedValue = ConversationNames.ConvUtilsLiveRecord;
            oHandler.NoSelectionTargetConversation = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("NoSelectionTargetConversation", expectedValue.ToString()),
                "NoSelectionTargetConversation value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_NoSelectionTargetHandlerObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.NoSelectionTargetHandlerObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("NoSelectionTargetHandlerObjectId", expectedValue),
                "NoSelectionTargetHandlerObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ZeroAction()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const ActionTypes expectedValue = ActionTypes.Ignore;
            oHandler.ZeroAction = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ZeroAction", (int)expectedValue),
                "ZeroAction value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ZeroTargetConversation()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const ConversationNames expectedValue = ConversationNames.ConvUtilsLiveRecord;
            oHandler.ZeroTargetConversation = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ZeroTargetConversation", expectedValue.ToString()),
                "ZeroTargetConversation value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ZeroTargetHandlerObjectId()
        {
            DirectoryHandler oHandler = new DirectoryHandler();
            const string expectedValue = "Test string";
            oHandler.ZeroTargetHandlerObjectId = expectedValue;
            Assert.IsTrue(oHandler.ChangeList.ValueExists("ZeroTargetHandlerObjectId", expectedValue),
                "ZeroTargetHandlerObjectId value get fetch failed");
        }
        
        #endregion


        #region Static Call Tests

        [TestMethod]
        public void GetDirectoryHandlers_NullConnectionServer_Failure()
        {
            List<DirectoryHandler> oHandlerList;

            WebCallResult res = DirectoryHandler.GetDirectoryHandlers(null, out oHandlerList,1,10,null);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail with null ConnectionServerRest passed to it");

        }

        [TestMethod]
        public void AddDirectoryHandler_NullConnectionServer_Failure()
        {
            DirectoryHandler oHandler;
            var res = DirectoryHandler.AddDirectoryHandler(null, "display name", true, null, out oHandler);
            Assert.IsFalse(res.Success, "Calling AddHandler with null ConnectionServerRest did not fail");

            }

        [TestMethod]
        public void AddDirectoryHandler_EmptyDisplayName_Failure()
        {
            DirectoryHandler oHandler;
            var res = DirectoryHandler.AddDirectoryHandler(_mockServer, "", true, null, out oHandler);
            Assert.IsFalse(res.Success, "Calling AddHandler with empty display name did not fail");
        }


        [TestMethod]
        public void DeleteDirectoryHandler_NullConnectionServer_Failure()
        {
            var res = DirectoryHandler.DeleteDirectoryHandler(null, "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with null ConnectionServerRest did not fail");
        }


        [TestMethod]
        public void DeleteDirectoryHandler_EmptyObjectId_Failure()
        {
            var res = DirectoryHandler.DeleteDirectoryHandler(_mockServer, "");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with empty objectId did not fail");
        }

        [TestMethod]
        public void UpdateDirectoryHandler_NullConnectionServer_Failure()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(null, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with null ConnectionServerRest did not fail");

            }

        [TestMethod]
        public void UpdateDirectoryHandler_NullPropertyList_Failure()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with null property list did not fail");

            }

        [TestMethod]
        public void UpdateDirectoryHandler_EmptyObjectId_Failure()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with empty objectid did not fail");

            }

        [TestMethod]
        public void UpdateDirectoryHandler_EmptyPropertyList_Failure()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "objectid", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with empty property list did not fail");
        }


        [TestMethod]
        public void GetDirectoryHandler_NullConnectionServer_Failure()
        {
            DirectoryHandler oHandler;

            WebCallResult res = DirectoryHandler.GetDirectoryHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ConnectionServerRest is null");
        }

        [TestMethod]
        public void GetDirectoryHandler_BlankNameAndObjectId_Failure()
        {
            DirectoryHandler oHandler;
            var res = DirectoryHandler.GetDirectoryHandler(out oHandler, _mockServer);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ObjectId and display name are both blank");
        }


        [TestMethod]
        public void GetGreetingStreamFiles_NullConnectionServer_Failure()
        {
            List<DirectoryHandlerGreetingStreamFile> oStreams;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(null, "objectid", out oStreams);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFiles with null ConnectionServer should fail");
        }


        [TestMethod]
        public void GetGreetingStreamFiles_EmptyObjectId_Failure()
        {
            List<DirectoryHandlerGreetingStreamFile> oStreams;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(_mockServer, "", out oStreams);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFiles with empty objectId should fail");
        }

        [TestMethod]
        public void GetGreetingWavFile_NullConnectionServer_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "streamname.wav");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with null ConnectionServer should fail");

         }

        [TestMethod]
        public void GetGreetingWavFile_EmptyWavPath_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "", "streamname.wav");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty local wav path should fail");
        }

        [TestMethod]
        public void GetGreetingWavFile_EmptyStreamName_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "c:\\temp.wav", "");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty stream name should fail");
        }

        [TestMethod]
        public void GetGreetingWavFile2_NullConnectionServer_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with null ConnectionServer should fail");
        }

        [TestMethod]
        public void GetGreetingWavFile2_EmptyWavPath_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty local wav file path should fail");

            }

        [TestMethod]
        public void GetGreetingWavFile2_EmptyObjectId_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "c:\\temp.wav", "", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty objectId should fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_NullConnectionServer_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(null, "objectid", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with null ConnectionServer should fail");
         }

        [TestMethod]
        public void SetGreetingWavFile_InvalidLocalWavFilePath_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_mockServer, "objectid", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with invalid local wav file should fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_EmptyObjectId_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_mockServer, "", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with Empty ObjectId should fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_EmptyWavFilePath_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_mockServer, "objectid", 1033, "", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with empty local wav file path should fail");
        }


        [TestMethod]
        public void SetGreetingRecordingToStreamFile_NullConnectionServer_Failure()
        {
            var res = DirectoryHandler.SetGreetingRecordingToStreamFile(null, "streamname", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with null ConnectionServerRest did not fail");
         }


        [TestMethod]
        public void SetGreetingRecordingToStreamFile_EmptyStreamId_Failure()
        {
            var res = DirectoryHandler.SetGreetingRecordingToStreamFile(_mockServer, "", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with empty streamID did not fail");
        }


        [TestMethod]
        public void SetGreetingRecordingToStreamFile_EmptyObjectId_Failure()
        {
            var res = DirectoryHandler.SetGreetingRecordingToStreamFile(_mockServer, "streamname", "", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with empty objecTId did not fail");
        }


        [TestMethod]
        public void GetGreetingStreamFile_NullConnectionServer_Failure()
        {
            DirectoryHandlerGreetingStreamFile oStream;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(null, "objectid", 1033, out oStream);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFile with null ConnectionServer should fail");
        }

        [TestMethod]
        public void GetGreetingStreamFile_EmptyObjectId_Failure()
        {
            DirectoryHandlerGreetingStreamFile oStream;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(_mockServer, "", 1033, out oStream);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFile with empty objectId should fail");
        }


        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetDirectoryHandlers_EmptyResult_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<DirectoryHandler> oHandlers;
            var res = DirectoryHandler.GetDirectoryHandlers(_mockServer, out oHandlers, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetDirectoryHandlers with EmptyResultText did not fail");

        }

        [TestMethod]
        public void GetDirectoryHandlers_GarbageResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<DirectoryHandler> oHandlers;
            var res = DirectoryHandler.GetDirectoryHandlers(_mockServer, out oHandlers, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetDirectoryHandlers with garbage results should fail");
            Assert.IsTrue(oHandlers.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetDirectoryHandlers_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<DirectoryHandler> oHandlers;
            var res = DirectoryHandler.GetDirectoryHandlers(_mockServer, out oHandlers, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetDirectoryHandlers with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetDirectoryHandlers_ZeroCount_Success()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<DirectoryHandler> oHandlers;
            var res = DirectoryHandler.GetDirectoryHandlers(_mockServer, out oHandlers, 1, 5, null);
            Assert.IsTrue(res.Success, "Calling GetDirectoryHandlers with ZeroCount failed:" + res);
        }

        [TestMethod]
        public void GetDirectoryHandler_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            DirectoryHandler oHandler;
            var res = DirectoryHandler.GetDirectoryHandler(out oHandler, _mockServer,"ObjectId");
            Assert.IsFalse(res.Success, "Calling GetDirectoryHandler with ErrorResponse did not fail");
        }

        [TestMethod]
        public void AddDirectoryHandler_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = DirectoryHandler.AddDirectoryHandler(_mockServer,"Display Name",false,null);
            Assert.IsFalse(res.Success, "Calling AddDirectoryHandler with ErrorResponse did not fail");
        }

        [TestMethod]
        public void DeleteDirectoryHandler_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.DELETE, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = DirectoryHandler.DeleteDirectoryHandler(_mockServer,"ObjectId");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with ErrorResponse did not fail");
        }


        [TestMethod]
        public void UpdateDirectoryHandler_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.DELETE, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test", "Test");
            var res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "ObjectId",oProps);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with ErrorResponse did not fail");
        }


        [TestMethod]
        public void SetGreetingWavFile_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = DirectoryHandler.SetGreetingWavFile(_mockServer, "Dummy.wav", "ObjectId",1033, true);
            Assert.IsFalse(res.Success, "Calling SetGreetingWavFile with ErrorResponse did not fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_InvalidWavFile_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "dummy text",
                                    });

            var res = DirectoryHandler.SetGreetingWavFile(_mockServer, "moq.dll", "ObjectId",1033, true);
            Assert.IsFalse(res.Success, "Calling SetGreetingWavFile with invalid wav file did not fail");
        }

        [TestMethod]
        public void RefetchDirectoryHandlerData_InvalidWavFile_Failure()
        {
            DirectoryHandler oHandler = new DirectoryHandler(_mockServer,"");
            var res = oHandler.RefetchDirectoryHandlerData();
            Assert.IsFalse(res.Success,"Calling RefetchDirectoryHandlerData on an empty class instance should fail");
        }

        #endregion

    }
}
