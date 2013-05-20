using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static CallHandler _tempHandler;

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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

            //grab the first template - should always be one and it doesn't matter which
            List<CallHandlerTemplate> oTemplates;
            WebCallResult res =CallHandlerTemplate.GetCallHandlerTemplates(_connectionServer, out oTemplates);
            if (res.Success == false || oTemplates==null || oTemplates.Count == 0)
            {
                Assert.Fail("Could not fetch call handler templates:"+res);    
            }
            
            //create new handler with GUID in the name to ensure uniqueness
            String strName = "TempHandler_" + Guid.NewGuid().ToString().Replace("-", "");

            res = CallHandler.AddCallHandler(_connectionServer, oTemplates[0].ObjectId, strName, "", null, out _tempHandler);
            Assert.IsTrue(res.Success, "Failed creating temporary callhandler:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempHandler != null)
            {
                WebCallResult res = _tempHandler.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary call handler on cleanup.");
            }
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


        #region Live Test Methods

        /// <summary>
        /// GET first 3 handler in directory using static method call, iterate over them and use the ToString and DumpAllProps
        /// methods on them each.
        /// </summary>
        [TestMethod]
        public void GetCallHandlers_Test()
        {
            List<CallHandler> oHandlerList;
            CallHandler oHoldHandler=null;

            //limit the fetch to the first 3 handlers to be sure this passes even on a default install
            string[] pClauses = { "rowsPerPage=3" };

            WebCallResult res = CallHandler.GetCallHandlers(_connectionServer, out oHandlerList, pClauses);

            Assert.IsTrue(res.Success, "Fetching of top three call handlers failed: " + res.ToString());
            Assert.AreEqual(oHandlerList.Count, 3, "Fetching of the top three call handlers returned a different number of handlers: " + res.ToString());

            //exercise the ToString and DumpAllProperties as part of this test as well
            foreach (CallHandler oHandler in oHandlerList)
            {
                Console.WriteLine(oHandler.ToString());
                Console.WriteLine(oHandler.DumpAllProps());
                oHoldHandler = oHandler;
            }

            //exercise the menu entry action descriptions
            MenuEntry oMenu;
             res = oHoldHandler.GetMenuEntry("0", out oMenu);
             Assert.IsTrue(res.Success, "Fetching of 0 menu entry key failed: " + res.ToString());

            Console.WriteLine(_connectionServer.GetActionDescription(oMenu.Action, oMenu.TargetConversation, oMenu.TargetHandlerObjectId));

            res = oHoldHandler.GetMenuEntry("*", out oMenu);
            Assert.IsTrue(res.Success, "Fetching of * menu entry key failed: " + res.ToString());

            Console.WriteLine(_connectionServer.GetActionDescription(oMenu.Action, oMenu.TargetConversation, oMenu.TargetHandlerObjectId));

        }


        /// <summary>
        /// Add a new handler, change it's name, save it and delete it.  This covers a lot of ground but since we're working with a real, live 
        /// Connection server we need to consolidate edits like this into a routine where we're working with a temporary handler that we clean
        /// up afterwards.
        /// </summary>
        [TestMethod]
        public void AddEditDeleteCallHandler_TopLevelTest()
        {
            //call update with no edits - this should fail
            WebCallResult res = _tempHandler.Update();
            Assert.IsFalse(res.Success, "Call to update with no pending changes should fail");

            //Edit the handler's name
            _tempHandler.DisplayName = _tempHandler.DisplayName + "x";
            _tempHandler.PrependDigits = "123";

            res = _tempHandler.Update();
            Assert.IsTrue(res.Success, "Call to update call handler failed:" + res.ToString());

            _tempHandler.DtmfAccessId = "____";
            res = _tempHandler.Update();
            Assert.IsFalse(res.Success,"Setting call handler to illegal primary extension did not fail:"+res);

            _tempHandler.ClearPendingChanges();

            _tempHandler.ClearPendingChanges();

            _tempHandler.AfterMessageAction = ActionTypes.GoTo;
            _tempHandler.AfterMessageTargetConversation = ConversationNames.PHTransfer;
            _tempHandler.AfterMessageTargetHandlerObjectId = _tempHandler.ObjectId;
            res = _tempHandler.Update();

            Assert.IsTrue(res.Success,"Failed setting after message action to loop back to self:"+res);
        }

        [TestMethod]
        public void AddEditDeleteCallHandler_VoiceNameTests()
        {
            //try to download voice name- this should fail
            WebCallResult res = _tempHandler.GetVoiceName(@"c:\temp.wav");
            Assert.IsFalse(res.Success, "Empty voice name fetch should return false for newly created handler");

            //try and upload a bogus WAV file which should fail - the exe will be in the output folder when running unit
            //tests and makes a handy file that will fail to convert or upload
            res = _tempHandler.SetVoiceName("wavcopy.exe", true);
            Assert.IsFalse(res.Success, "Invalid WAV file should fail to convert");

            //upload a voice name to the handler
            res = _tempHandler.SetVoiceName("Dummy.wav", true);
            Assert.IsTrue(res.Success, "Updating voice name on new call handler failed: " + res.ToString());

            //download the wav file we just uploaded
            res = _tempHandler.GetVoiceName("DummyDownload.wav");
            Assert.IsTrue(res.Success, "Downloading voice name for call handler failed:" + res.ToString());
        }


        [TestMethod]
        public void AddEditDeleteCallHandler_TransferOptionTests()
        {
            List<TransferOption> oTransferOptions = _tempHandler.GetTransferOptions();
            Assert.IsTrue(oTransferOptions.Count == 3, "Transfer option collection not returned from call handler properly.");

            TransferOption oTransferOption = oTransferOptions[0];
            
            WebCallResult res = oTransferOption.Update();
            Assert.IsFalse(res.Success,"Update for transfer rule with no pending changes should fail");

            oTransferOption.TransferAnnounce = true;
            oTransferOption.TransferConfirm = true;
            oTransferOption.Extension = "123";
            res = oTransferOption.Update();
            Assert.IsTrue(res.Success,"Failed to update transfer rule");

            oTransferOption.UsePrimaryExtension = true;
            oTransferOption.Action = ActionTypes.Invalid;
            res = oTransferOption.Update();
            Assert.IsFalse(res.Success,"Setting transfer option to invalid action did not fail");
        }

        [TestMethod]
        public void AddEditDeleteCallHandler_MenuOptionTests()
        {
            List<MenuEntry> oMenuEntries = _tempHandler.GetMenuEntries();
            Assert.IsTrue(oMenuEntries.Count == 12, "Menu entries not returned from call handler properly.");

            MenuEntry oMenuEntry = oMenuEntries[2];

            WebCallResult res = oMenuEntry.Update();
            Assert.IsFalse(res.Success,"Calling update of menu entry with no pending changes did not fail");

            oMenuEntry.Action = ActionTypes.Invalid;
            res = oMenuEntry.Update();
            Assert.IsFalse(res.Success, "Setting menu entry to invalid action value did not fail");

            oMenuEntry.Action = ActionTypes.GoTo;
            oMenuEntry.Locked = true;
            oMenuEntry.TargetConversation = ConversationNames.PHGreeting;
            oMenuEntry.TargetHandlerObjectId = oMenuEntry.CallHandlerObjectId;

            res = oMenuEntry.Update();
            Assert.IsTrue(res.Success,"Failed to update menu entry to PHGreeting back to self");
        }

        [TestMethod]
        public void AddEditDeleteCallHandler_GreetingOptionTests()
        {
            Greeting oGreeting;
            GreetingStreamFile oStream;

            WebCallResult res = _tempHandler.GetGreeting(GreetingTypes.Alternate , out oGreeting);
            Assert.IsTrue(res.Success, "Failed to get alternate greeting" + res);

            //update the greeting propert and upload a wav file to it
            oGreeting.PlayWhat = PlayWhatTypes.RecordedGreeting;
            oGreeting.TimeExpiresSetNull();

            res = oGreeting.Update();
            Assert.IsTrue(res.Success, "Failed updating 'playWhat' for alternate greeting rule:" + res.ToString());

            res = oGreeting.SetGreetingWavFile(1033, "wavcopy.exe", true);
            Assert.IsFalse(res.Success, "Uploading invalid WAV file should fail");

            res = oGreeting.SetGreetingWavFile(1033, "Dummy.wav", true);
            Assert.IsTrue(res.Success, "Failed updating the greeting wav file for the alternate greeting:" + res);

            //use static greeting stream to set wav file instead
            res = GreetingStreamFile.SetGreetingWavFile(_connectionServer, _tempHandler.ObjectId,GreetingTypes.Alternate, 1033, "Dummy.wav", true);
            Assert.IsTrue(res.Success, "Updating voice name on new call handler failed: " + res);

            //upload the wav file again, this time using an instance of the GreetingStreamFile object
            res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, _tempHandler.ObjectId, GreetingTypes.Alternate, 1033, out oStream);
            Assert.IsTrue(res.Success, "Failed to create GreetingStreamFile object" + res);

            res = oStream.SetGreetingWavFile("Dummy.wav", true);
            Assert.IsTrue(res.Success, "Failed to upload WAV file via GreetingStreamFile instance" + res);

            //check some failure resuls for GreetingStreamFile static calls while we're here since we know this greeting exists.
            res = GreetingStreamFile.GetGreetingWavFile(null, "temp.wav", _tempHandler.ObjectId, GreetingTypes.Alternate, 1033);
            Assert.IsFalse(res.Success, "Null connection server param should fail" + res);

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "temp.wav", "", GreetingTypes.Alternate, 1033);
            Assert.IsFalse(res.Success, "Empty call handler object ID param should fail" + res);

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "temp.wav", _tempHandler.ObjectId, GreetingTypes.Invalid, 1033);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail" + res);

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "temp.wav", _tempHandler.ObjectId, GreetingTypes.Alternate, 10);
            Assert.IsFalse(res.Success, "Invalid language code should fail" + res);

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "temp.wav", _tempHandler.ObjectId, GreetingTypes.Alternate, 1033);
            Assert.IsTrue(res.Success, "Uploading WAV file to greeting via static GreetingStreamFile call failed:"+res);

            //get list of all greeting stream files
            List<GreetingStreamFile> oStreams = oGreeting.GetGreetingStreamFiles();
            Assert.IsNotNull(oStreams, "Null list of greeting streams returned from greeting streams fetch");
            Assert.IsTrue(oStreams.Count > 0, "Empty list of greeting streams returned");

            //create a new greeting and fetch the stream files we just uploaded for it
            oGreeting = new Greeting(_connectionServer, _tempHandler.ObjectId, GreetingTypes.Alternate);
            Assert.IsNotNull(oGreeting, "Failed to create new greeting object");

            //fetch the stream back out
            res = oGreeting.GetGreetingStreamFile(1033, out oStream);
            Assert.IsTrue(res.Success, "Failed to fetch greeting stream file:"+res);

            res = oGreeting.UpdateGreetingEnabledStatus(true, DateTime.Now.AddDays(1));
            Assert.IsTrue(res.Success, "Failed updating greeting eneabled status for one day:"+res);

            //exercise the "auto fill" greeting, menu entry and transfer option interfaces
            List<Greeting> oGreetings = _tempHandler.GetGreetings();
            Assert.IsTrue(oGreetings.Count > 5, "Greetings collection not returned from call handler properly.");
        }


        /// <summary>
        /// exercise property list adds and dumps - property list does not need it's own test class, everything it can do 
        /// is done right here.  Add each of the types its supports and dump the list out using the ToString override.
        /// </summary>
        [TestMethod]
        public void ExercisePropertyList()
         {
            ConnectionPropertyList oList = new ConnectionPropertyList("propname","propvalue");

            oList.Add("integer",1);
            oList.Add("string","stringvalue");
            oList.Add("date",DateTime.Now);
            oList.Add("boolean",false);

            Console.WriteLine(oList.ToString());

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
            Assert.IsFalse(res.Success, "GetHandler should fail with null ConnectionServer passed to it");

        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_GetCallHandler()
        {
            CallHandler oHandler;

            WebCallResult res = CallHandler.GetCallHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetCallHandler should fail if the ConnectionServer is null");

            res = CallHandler.GetCallHandler(out oHandler, _connectionServer);
            Assert.IsFalse(res.Success, "GetCallHandler should fail if the ObjectId and display name are both blank");
        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_AddCallHandler()
        {
            WebCallResult res = CallHandler.AddCallHandler(null, "", "", "", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the ConnectionServer parameter is null");

            res = CallHandler.AddCallHandler(_connectionServer, "", "aaa", "123", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the template parameter is empty");

            res = CallHandler.AddCallHandler(_connectionServer, "voicemailtemplate", "aaa", "", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the extension parameter is empty");

            res = CallHandler.AddCallHandler(_connectionServer, "voicemailtemplate", "", "1234", null);
            Assert.IsFalse(res.Success, "AddCallHandler should fail if the DisplayName parameter is empty");
        }



        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_DeleteCallHandler()
        {
            WebCallResult res = CallHandler.DeleteCallHandler(null, "");
            Assert.IsFalse(res.Success, "DeleteCallHandler should fail if the ConnectionServer parameter is null");

            res = CallHandler.DeleteCallHandler(_connectionServer, "");
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
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the ConnectionServer parameter is null");

            res = CallHandler.UpdateCallHandler(_connectionServer, "", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the ObjectId parameter is blank");

            res = CallHandler.UpdateCallHandler(_connectionServer, "aaa", oPropList);
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
            res = CallHandler.GetCallHandlerVoiceName(_connectionServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "GetCallHandlerVoiceName did not fail with invalid target path passed");

            //invalid objectId 
            res = CallHandler.GetCallHandlerVoiceName(_connectionServer, "", strWavName);
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
            res = CallHandler.SetCallHandlerVoiceName(_connectionServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with invalid target path");

            //invalid ObjectId
            res = CallHandler.SetCallHandlerVoiceName(_connectionServer, strWavName, "");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with invalid obejctID");

        }


        #endregion
    }
}
