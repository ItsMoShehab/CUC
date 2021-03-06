﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for CallHandlerTest and is intended
    ///to contain all CallHandlerTest Unit Tests
    ///</summary>
    [TestClass]
    public class CallHandlerIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties
       
        private static CallHandler _tempHandler;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

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


        #region Live Test Methods

        /// <summary>
        /// GET first 3 handler in directory using static method call, iterate over them and use the ToString and DumpAllProps
        /// methods on them each.
        /// </summary>
        [TestMethod]
        public void GetCallHandlers_FetchTest()
        {
            List<CallHandler> oHandlerList;
            CallHandler oHoldHandler=null;

            //limit the fetch to the first 3 handlers to be sure this passes even on a default install
            string[] pClauses = { "rowsPerPage=3" };

            WebCallResult res = CallHandler.GetCallHandlers(_connectionServer, out oHandlerList,1,2, pClauses);

            Assert.IsTrue(res.Success, "Fetching of top three call handlers failed: " + res);
            Assert.AreEqual(oHandlerList.Count, 3, "Fetching of the top three call handlers returned a different number of handlers: " + res.ToString());

            //exercise the ToString and DumpAllProperties as part of this test as well
            foreach (CallHandler oHandler in oHandlerList)
            {
                Console.WriteLine(oHandler.ToString());
                Console.WriteLine(oHandler.DumpAllProps());
                oHoldHandler = oHandler;
            }

            res = CallHandler.GetCallHandlers(_connectionServer, out oHandlerList, 1, 1, null);
            Assert.IsTrue(res.Success,"Failed to fetch call handlers:"+res);
            Assert.IsTrue(oHandlerList.Count==1,"Failed to return single call handler");

            res = CallHandler.GetCallHandlers(_connectionServer, out oHandlerList, 1, 1, "query=(ObjectId is bogus)");
            Assert.IsTrue(res.Success, "fetching handlers with invalid query should not fail:" + res);
            Assert.IsTrue(oHandlerList.Count == 0, "Invalid query string should return an empty handler list:" + oHandlerList.Count);
            
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

            res = _tempHandler.RefetchCallHandlerData();
            Assert.IsTrue(res.Success, "Refetching handler data failed:" + res);
            Assert.IsTrue(_tempHandler.PrependDigits=="123","Pepend digits value did not match after refetch:"+res);

            _tempHandler.DtmfAccessId = "____";
            res = _tempHandler.Update();
            Assert.IsFalse(res.Success,"Setting call handler to illegal primary extension did not fail:"+res);

            _tempHandler.ClearPendingChanges();

            _tempHandler.AfterMessageAction = ActionTypes.GoTo;
            _tempHandler.AfterMessageTargetConversation = ConversationNames.PHTransfer;
            _tempHandler.AfterMessageTargetHandlerObjectId = _tempHandler.ObjectId;
            res = _tempHandler.Update();

            Assert.IsTrue(res.Success,"Failed setting after message action to loop back to self:"+res);
        }

        [TestMethod]
        public void AddEditDeleteCallHandler_OwnerTests()
        {
            List<CallHandlerOwner> oOwners;
            var res=_tempHandler.GetOwners(out oOwners);
            Assert.IsTrue(res.Success, "Call to fetch owners on new call handler should not fail:"+res);
            Assert.IsTrue(oOwners.Count==0,"New call handler should report zero owners");

            UserBase oUser;
            res = UserBase.GetUser(out oUser, _connectionServer, "", "operator");
            Assert.IsTrue(res.Success,"Fetching operator user should not fail:"+res);

            res = _tempHandler.AddOwner(oUser.ObjectId, "");
            Assert.IsTrue(res.Success,"Adding user as owner for temp call handler should not fail:"+res);

            res = _tempHandler.GetOwners(out oOwners);
            Assert.IsTrue(res.Success, "Call to fetch owners on new call handler should not fail:" + res);
            Assert.IsTrue(oOwners.Count == 1, "Owners count should be 1 after adding user to owner list");
            Console.WriteLine(oOwners.First().ToString());

            res = _tempHandler.DeleteOwner(oOwners.First().ObjectId);
            Assert.IsTrue(res.Success, "Call to delete owner on new call handler should not fail:" + res);
            res = _tempHandler.GetOwners(out oOwners);
            Assert.IsTrue(res.Success, "Call to fetch owners on new call handler should not fail:" + res);
            Assert.IsTrue(oOwners.Count == 0, "Owners count should be 0 after removing owner from list");
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
        public void AddEditDeleteCallHandler_RecipientContact()
        {
            List<Contact> oContacts;
            var res = Contact.GetContacts(_connectionServer, out oContacts, 1, 1);
            Assert.IsTrue(res.Success,"Failed to fetch contacts:"+res);
            Assert.IsTrue(oContacts.Count==1,"Failed to fetch single contacts");

            _tempHandler.ClearPendingChanges();
            _tempHandler.RecipientContactObjectId = oContacts[0].ObjectId;
            res = _tempHandler.Update();
            Assert.IsTrue(res.Success,"Failed to update handler to contact recipient:"+res);
        }

        [TestMethod]
        public void AddEditDeleteCallHandler_SetPostGreetingRecording()
        {
            List<PostGreetingRecording> oRecordings;
            var res = PostGreetingRecording.GetPostGreetingRecordings(_connectionServer, out oRecordings, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch recordings:" + res);
            Assert.IsTrue(oRecordings.Count == 1, "Failed to fetch single recording");

            _tempHandler.ClearPendingChanges();
            _tempHandler.PostGreetingRecordingObjectId = oRecordings[0].ObjectId;
            res = _tempHandler.Update();
            Assert.IsTrue(res.Success, "Failed to update handler to postgreeting recording:" + res);
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
            List<GreetingStreamFile> oStreams = oGreeting.GetGreetingStreamFiles(true);
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


        [TestMethod]
        public void AddCallHandlerFailure_InvalidExtension()
        {
            //grab the first template - should always be one and it doesn't matter which
            List<CallHandlerTemplate> oTemplates;
            WebCallResult res =CallHandlerTemplate.GetCallHandlerTemplates(_connectionServer, out oTemplates,1,1);
            if (res.Success == false || oTemplates==null || oTemplates.Count == 0)
            {
                Assert.Fail("Could not fetch call handler templates:"+res);    
            }

            string strExtension = Guid.NewGuid().ToString();
            res = CallHandler.AddCallHandler(_connectionServer, oTemplates[0].ObjectId, strExtension, strExtension, null);
            Assert.IsFalse(res.Success,"Creating new call handler with invalid extension should fail");
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

        [TestMethod]
        public void CollectionFetchTests_TransferOptions()
        {
            List<TransferOption> oTransferOptions = _tempHandler.GetTransferOptions(true);
            Assert.IsTrue(oTransferOptions.Count==3,"Transfer option did not return 3");
        }

        [TestMethod]
        public void CollectionFetchTests_Greetings()
        {
            List<Greeting> oGreetings;
            oGreetings = _tempHandler.GetGreetings(true);
            Assert.IsTrue(oGreetings.Count == 7, "Greeting fetch did not return 7 greetings");
        }

        [TestMethod]
        public void CollectionFetchTests_MenuEntries()
        {
            List<MenuEntry> oMenuEntries;
            oMenuEntries = _tempHandler.GetMenuEntries(true);
            Assert.IsTrue(oMenuEntries.Count == 12, "Menu entry fetch did not return 12 greetings");
        }

        [TestMethod]
        public void CollectionFetchTests_ScheduleSet()
        {
            ScheduleSet oSchedule;
            oSchedule = _tempHandler.GetScheduleSet(true);
            Assert.IsTrue(!string.IsNullOrEmpty(oSchedule.ObjectId), "Schedule set fetch did not return a valid schedule");
        }

        [TestMethod]
        public void ConstructurWithEmptyId()
        {
            CallHandler oHandler = new CallHandler(_connectionServer);
            Assert.IsNotNull(oHandler,"Call handler object not returned from constructor with no ObjectId");
            Assert.IsTrue(string.IsNullOrEmpty(oHandler.ObjectId),"Empty constructor should return call handler not filled in with data");
        }


        #endregion

    }
}
