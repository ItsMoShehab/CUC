using System.Collections.Generic;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for CallHandlerTest and is intended
    ///to contain all CallHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CallHandlerTest
    {
        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
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
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

        }

        #endregion


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            CallHandler oTestHandler = new CallHandler(null);
        }


        /// <summary>
        /// Get first 3 handler in directory using static method call, iterate over them and use the ToString and DumpAllProps
        /// methods on them each.
        /// </summary>
        [TestMethod()]
        public void GetCallHandlers_Test()
        {
            WebCallResult res;
            List<CallHandler> oHandlerList;
            CallHandler oHoldHandler=null;

            //limit the fetch to the first 3 handlers to be sure this passes even on a default install
            string[] pClauses = { "rowsPerPage=3" };

            res = CallHandler.GetCallHandlers(_connectionServer, out oHandlerList, pClauses);

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
        [TestMethod()]
        public void AddEditDeleteCallHandler_Test()
        {
            WebCallResult res;

            //create new list with GUID in the name to ensure uniqueness
            String strHandlerName = "TempHandler_" + Guid.NewGuid().ToString();

            CallHandler oHandler;
            Greeting oGreeting;
            GreetingStreamFile oStream;
            List<GreetingStreamFile> oStreams;

            //get first call handler template
            List<CallHandlerTemplate> oTemplates;
            res = CallHandlerTemplate.GetCallHandlerTemplates(_connectionServer, out oTemplates);
            Assert.IsTrue(res.Success, "Failed to fetch call handler templates:" + res.ToString());
            Assert.IsNotNull(oTemplates, "NULL instance of template list returned");
            Assert.IsTrue(oTemplates.Count > 0, "Zero templates returned");

            //Just grab the first template - doesn't really matter here.
            res = CallHandler.AddCallHandler(_connectionServer, oTemplates[0].ObjectId, strHandlerName, "", null, out oHandler);

            Assert.AreEqual(res.Success, true);
            Assert.AreNotEqual(oHandler, null);

            //call update with no edits - this should fail
            res = oHandler.Update();
            Assert.IsFalse(res.Success, "Call to update with no pending changes should fail");

            //Edit the handler's name
            oHandler.DisplayName = strHandlerName + "x";
            oHandler.PrependDigits = "123";
            
            res = oHandler.Update();
            Assert.IsTrue(res.Success, "Call to update call handler failed:" + res.ToString());

            //try to download voice name- this should fail
            res = oHandler.GetVoiceName(@"c:\temp.wav");
            Assert.IsFalse(res.Success, "Empty voice name fetch should return false for newly created handler");

            //try and upload a bogus WAV file which should fail - the exe will be in the output folder when running unit
            //tests and makes a handy file that will fail to convert or upload
            res = oHandler.SetVoiceName("wavcopy.exe", true);
            Assert.IsFalse(res.Success, "Invalid WAV file should fail to convert");

            //upload a voice name to the handler
            res = oHandler.SetVoiceName("Dummy.wav", true);
            Assert.IsTrue(res.Success, "Updating voice name on new call handler failed: " + res.ToString());

            //download the wav file we just uploaded
            res = oHandler.GetVoiceName("DummyDownload.wav");
            Assert.IsTrue(res.Success, "Downloading voice name for call handler failed:" + res.ToString());

            res=oHandler.GetGreeting("Alternate",out oGreeting);
            Assert.IsTrue(res.Success,"Failed to get alternate greeting");

            //update the greeting propert and upload a wav file to it
            oGreeting.PlayWhat = (int)PlayWhatTypes.RecordedGreeting;
            oGreeting.TimeExpiresSetNull();
            
            res = oGreeting.Update();
            Assert.IsTrue(res.Success, "Failed updating 'playWhat' for alternate greeting rule:" + res.ToString());

            res = oGreeting.SetGreetingWavFile(1033, "wavcopy.exe", true);
            Assert.IsFalse(res.Success, "Uploading invalid WAV file should fail");

            res = oGreeting.SetGreetingWavFile(1033, "Dummy.wav", true);
            Assert.IsTrue(res.Success, "Failed updating the greeting wav file for the alternate greeting:" + res.ToString());

            //use static greeting stream to set wav file instead
            res = GreetingStreamFile.SetGreetingWavFile(_connectionServer, oHandler.ObjectId, "Alternate", 1033, "Dummy.wav", true);
            Assert.IsTrue(res.Success, "Updating voice name on new call handler failed: " + res.ToString());

            //upload the wav file again, this time using an instance of the GreetingStreamFile object
            res=GreetingStreamFile.GetGreetingStreamFile(_connectionServer, oHandler.ObjectId, "Alternate", 1033,out oStream);
            Assert.IsTrue(res.Success,"Failed to create GreetingStreamFile object");

            res=oStream.SetGreetingWavFile("Dummy.wav", true);
            Assert.IsTrue(res.Success,"Failed to upload WAV file via GreetingStreamFile instance");

            //check some failure resuls for GreetingStreamFile static calls while we're here since we know this greeting exists.
            res = GreetingStreamFile.GetGreetingWavFile(null, "temp.wav", oHandler.ObjectId, "Alternate", 1033);
            Assert.IsFalse(res.Success, "Null connection server param should fail");

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "temp.wav", "", "Alternate", 1033);
            Assert.IsFalse(res.Success, "Empty call handler object ID param should fail");

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "temp.wav", oHandler.ObjectId, "Bogus", 1033);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "temp.wav", oHandler.ObjectId, "Alternate", 10);
            Assert.IsFalse(res.Success, "Invalid language code should fail");

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "temp.wav", oHandler.ObjectId, "Alternate", 1033);
            Assert.IsTrue(res.Success, "Uploading WAV file to greeting via static GreetingStreamFile call failed");

            //get list of all greeting stream files
            oStreams = oGreeting.GetGreetingStreamFiles();
            Assert.IsNotNull(oStreams, "Null list of greeting streams returned from greeting streams fetch");
            Assert.IsTrue(oStreams.Count > 0, "Empty list of greeting streams returned");

            //create a new greeting and fetch the stream files we just uploaded for it
            oGreeting = new Greeting(_connectionServer, oHandler.ObjectId,"Alternate");
            Assert.IsNotNull(oGreeting, "Failed to create new greeting object");

            //fetch the stream back out
            res = oGreeting.GetGreetingStreamFile(1033, out oStream);
            Assert.IsTrue(res.Success, "Failed to fetch greeting stream file");

            res = oGreeting.UpdateGreetingEnabledStatus(true, DateTime.Now.AddDays(1));
            Assert.IsTrue(res.Success, "Failed updating greeting eneabled status for one day");

            //exercise the "auto fill" greeting, menu entry and transfer option interfaces
            List<Greeting> oGreetings = oHandler.GetGreetings();
            Assert.IsTrue(oGreetings.Count > 5, "Greetings collection not returned from call handler properly.");

            List<MenuEntry> oMenuEntries = oHandler.GetMenuEntries();
            Assert.IsTrue(oMenuEntries.Count == 12, "Menu entries not returned from call handler properly.");

            List<TransferOption> oTransferOption = oHandler.GetTransferOptions();
            Assert.IsTrue(oTransferOption.Count == 3, "Transfer option collection not returned from call handler properly.");

            //delete the handler
            res = oHandler.Delete();
            Assert.IsTrue(res.Success, "Removal of new handler at end of test failed: " + res.ToString());

        }


        /// <summary>
        /// exercise property list adds and dumps - property list does not need it's own test class, everything it can do 
        /// is done right here.  Add each of the types its supports and dump the list out using the ToString override.
        /// </summary>
        [TestMethod()]
        public void ExercisePropertyList()
         {
             ConnectionPropertyList oList;
             oList=new ConnectionPropertyList("propname","propvalue");

            oList.Add("integer",1);
            oList.Add("string","stringvalue");
            oList.Add("date",DateTime.Now);
            oList.Add("boolean",false);

            Console.WriteLine(oList.ToString());

         }

    
        /// <summary>
        /// exercise call handler templates - call handler templates is a pretty simple class and it only gets used by handlers so 
        /// test it here - make sure it handles an invalid conneciton server passed in and can get the list back out and hit the 
        /// ToString override for it - that about covers it.
        /// </summary>
        [TestMethod()]
        public void CallHandlerTemplate_Test()
         {
             WebCallResult res;

             List<CallHandlerTemplate> oTemplates;

            res = CallHandlerTemplate.GetCallHandlerTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success,"Null ConnectionServer parameter should fail");

            res = CallHandlerTemplate.GetCallHandlerTemplates(_connectionServer, out oTemplates);
            Assert.IsTrue(res.Success,"Failed to get call handler templates");
            Assert.IsNotNull(oTemplates,"Null call handler template returned");
            Assert.IsTrue(oTemplates.Count>0,"Empty list of templates returned");

            //exercise the toString method
            foreach (CallHandlerTemplate oTemplate in oTemplates)
            {
                Console.WriteLine(oTemplate.ToString());
            }

         }


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CallHandlerTemplate_ClassCreationFailure()
        {
            CallHandlerTemplate oTestTemplate = new CallHandlerTemplate(null,"aaa");
        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void GetCallHandlers_Failure()
        {
            WebCallResult res;
            List<CallHandler> oHandlerList;

            res = CallHandler.GetCallHandlers(null, out oHandlerList, null);
            Assert.IsFalse(res.Success, "GetHandler should fail with null ConnectionServer passed to it");

        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void GetCallHandler_Failure()
        {
            WebCallResult res;
            CallHandler oHandler;

            res = CallHandler.GetCallHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetCallHandler should fail if the ConnectionServer is null");

            res = CallHandler.GetCallHandler(out oHandler, _connectionServer, "", "");
            Assert.IsFalse(res.Success, "GetCallHandler should fail if the ObjectId and display name are both blank");
        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void AddCallHandler_Failure()
        {
            WebCallResult res;

            res = CallHandler.AddCallHandler(null, "", "", "", null);
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
        [TestMethod()]
        public void DeleteCallHandler_Failure()
        {
            WebCallResult res;

            res = CallHandler.DeleteCallHandler(null, "");
            Assert.IsFalse(res.Success, "DeleteCallHandler should fail if the ConnectionServer parameter is null");

            res = CallHandler.DeleteCallHandler(_connectionServer, "");
            Assert.IsFalse(res.Success, "DeleteCallHandler should fail if the ObjectId parameter is blank");
        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void UpdateCallHandler_Failure()
        {
            WebCallResult res;
            ConnectionPropertyList oPropList = new ConnectionPropertyList();

            res = CallHandler.UpdateCallHandler(null, "", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the ConnectionServer parameter is null");

            res = CallHandler.UpdateCallHandler(_connectionServer, "", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the ObjectId parameter is blank");

            res = CallHandler.UpdateCallHandler(_connectionServer, "aaa", oPropList);
            Assert.IsFalse(res.Success, "UpdateCallHandler should fail if the property list is empty");
        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void GetCallHandlerVoiceName_Failure()
        {
            WebCallResult res;

            //use the same string for the alias and display name here
            String strWavName = @"c:\";

            //invalid local WAV file name
            res = CallHandler.GetCallHandlerVoiceName(null, "aaa", "");
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
        [TestMethod()]
        public void SetCallHandlerVoiceName_Failure()
        {
            WebCallResult res;

            //use the same string for the alias and display name here
            String strWavName = @"c:\";

            //invalid Connection server
            res = CallHandler.SetCallHandlerVoiceName(null, "", "");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with null Connection server passed.");

            //invalid target path
            res = CallHandler.SetCallHandlerVoiceName(_connectionServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with invalid target path");

            //invalid ObjectId
            res = CallHandler.SetCallHandlerVoiceName(_connectionServer, strWavName, "");
            Assert.IsFalse(res.Success, "SetCallHandlerVoiceName did not fail with invalid obejctID");

        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void GetCallHandlerTemplate_Failure()
        {
            WebCallResult res;
            List<CallHandlerTemplate> oTemplates;
            res = CallHandlerTemplate.GetCallHandlerTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success, "Passing null connection server should fail.");
        }

    }
}
