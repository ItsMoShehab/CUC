using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;


namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for InterviewHandlerTest and is intended
    ///to contain all InterviewHandler Unit Tests
    ///</summary>
    [TestClass]
    public class InterviewHandlerTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static InterviewHandler _tempHandler;

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
                throw new Exception("Unable to attach to Connection server to start InterviewHandler test:" + ex.Message);
            }

            //grab the first user in the directory to use as a recipient
            List<UserBase> oUsers;
            WebCallResult res = UserBase.GetUsers(_connectionServer, out oUsers, 1, 1);
            Assert.IsTrue(res.Success, "Failed fetching user as recipient for creating temporary interview handler:" + res);

            //create new handler with GUID in the name to ensure uniqueness
            //String strName = "TempHandler_" + Guid.NewGuid().ToString().Replace("-", "");
            List<InterviewHandler> oHandlers;
            res = InterviewHandler.GetInterviewHandlers(_connectionServer, out oHandlers, 1, 1);
            Assert.IsTrue(res.Success,"Failed to fetch interview handlers at test init");
            Assert.IsTrue(oHandlers.Count>0,"No interview handlers found in system");
            _tempHandler = oHandlers[0];

            //res = InterviewHandler.AddInterviewHandler(_connectionServer, strName,oUsers[0].ObjectId,"",  null, out _tempHandler);
            //Assert.IsTrue(res.Success, "Failed creating temporary interview handler:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            //if (_tempHandler != null)
            //{
            //    WebCallResult res = _tempHandler.Delete();
            //    Assert.IsTrue(res.Success, "Failed to delete temporary interview handler on cleanup.");
            //}
        }

        #endregion


        #region Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure_NullServer()
        {
            InterviewHandler oTestInterviewer = new InterviewHandler(null);
            Console.WriteLine(oTestInterviewer);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure_InvalidObjectId()
        {
            InterviewHandler oTestInterviewer = new InterviewHandler(_connectionServer,"blah");
            Console.WriteLine(oTestInterviewer);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure_Name()
        {
            InterviewHandler oTestInterviewer = new InterviewHandler(_connectionServer, "","blah");
            Console.WriteLine(oTestInterviewer);
        }

        #endregion


        [TestMethod]
        public void StaticMethodFailures()
        {
            InterviewHandler oInterviewer;
            List<InterviewHandler> oHandlers;
            WebCallResult res = InterviewHandler.GetInterviewHandlers(null, out oHandlers);
            Assert.IsFalse(res.Success,"Calling static method GetInterviewHandlers did not fail with: null ConnectionServer");

            res = InterviewHandler.AddInterviewHandler(null, "display name","","", null,out oInterviewer);
            Assert.IsFalse(res.Success, "Calling static method AddInterviewHandler did not fail with: null ConnectionServer");

            res = InterviewHandler.AddInterviewHandler(_connectionServer, "","bogus","", null,out oInterviewer);
            Assert.IsFalse(res.Success, "Calling static method AddInterviewHandler did not fail with: empty objectid ");

            res = InterviewHandler.AddInterviewHandler(_connectionServer, "bogus", "", "", null, out oInterviewer);
            Assert.IsFalse(res.Success, "Calling static method AddInterviewHandler did not fail with: empty recipient objectIds");

            res = InterviewHandler.UpdateInterviewHandler(null, "objectId", null);
            Assert.IsFalse(res.Success, "Calling static method UpdateInterviewHandler did not fail with: null ConnectionServer");

            res = InterviewHandler.UpdateInterviewHandler(_connectionServer, "", null);
            Assert.IsFalse(res.Success, "Calling static method UpdateInterviewHandler did not fail with: empty object id");

            res = InterviewHandler.UpdateInterviewHandler(_connectionServer, "ObjectId", null);
            Assert.IsFalse(res.Success, "Calling static method UpdateInterviewHandler did not fail with: empty property list");

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("bogus","bogusvalue");

            res = InterviewHandler.UpdateInterviewHandler(_connectionServer, "ObjectId", oProps);
            Assert.IsFalse(res.Success, "Calling static method UpdateInterviewHandler did not fail with: invalid objectId");

            res = InterviewHandler.DeleteInterviewHandler(null, "objectid");
            Assert.IsFalse(res.Success, "Calling static method DeleteInterviewHandler did not fail with: null ConnectionServer");

            res = InterviewHandler.DeleteInterviewHandler(_connectionServer, "ObjectId");
            Assert.IsFalse(res.Success, "Calling static method DeleteInterviewHandler did not fail with: invalid objectid");

            res = InterviewHandler.GetInterviewHandler(out oInterviewer, null, "objectId", "DisplayName");
            Assert.IsFalse(res.Success, "Calling static method GetInterviewHandler did not fail with: null ConnectionServer");

            res = InterviewHandler.GetInterviewHandler(out oInterviewer, _connectionServer);
            Assert.IsFalse(res.Success, "Calling static method GetInterviewHandler did not fail with: empty objectID and display name");

            res = InterviewHandler.GetInterviewHandler(out oInterviewer, _connectionServer, "objectId", "DisplayName");
            Assert.IsFalse(res.Success, "Calling static method GetInterviewHandler did not fail with: invalid objectId and display name");
        }

        /// <summary>
        /// GET first handler in directory using static method call, iterate over it and use the ToString and DumpAllProps
        /// methods on it.
        /// For Interview handlers it's possible there are none here - so for this test to be valid it needs to be run after at least
        /// one interviewer is created.
        /// </summary>
        [TestMethod]
        public void GetInterviewHandlers_Test()
        {
            List<InterviewHandler> oHandlerList;
            string strObjectId="";

            InterviewHandler oInterviewHandler;

            //limit the fetch to the first 1 handler 
            string[] pClauses = { "rowsPerPage=1" };

            WebCallResult res = InterviewHandler.GetInterviewHandlers(_connectionServer, out oHandlerList, pClauses);

            Assert.IsTrue(res.Success, "Fetching of first interview handler failed: " + res.ToString());
            Assert.AreEqual(oHandlerList.Count, 1, "Fetching of the first interview handler returned a different number of handlers: " + res.ToString());

            //exercise the ToString and DumpAllProperties as part of this test as well
            foreach (InterviewHandler oHandler in oHandlerList)
            {
                Console.WriteLine(oHandler.ToString());
                Console.WriteLine(oHandler.DumpAllProps());
                strObjectId = oHandler.ObjectId; //save for test below
            }

            //fetch interviewer by ObjectId
            res = InterviewHandler.GetInterviewHandler(out oInterviewHandler, _connectionServer, strObjectId);
            Assert.IsTrue(res.Success, "Fetching of interview handler by objectId failed: " + res.ToString());

            res = oInterviewHandler.RefetchInterviewHandlerData();
            Assert.IsTrue(res.Success,"Failed refetching interviewer data:"+res);

            Console.WriteLine(oInterviewHandler.DumpAllProps());

            res = oInterviewHandler.Update();
            Assert.IsFalse(res.Success,"Updating interview handler with no pending changes did not fail");

            //failed fetch using bogus name
            res = InterviewHandler.GetInterviewHandler(out oInterviewHandler, _connectionServer, "", "blah");
            Assert.IsFalse(res.Success, "Fetching of interview handler by bogus name did not fail");

        }



        [TestMethod]
        public void GetInterviewHandlers_Failure()
        {
            List<InterviewHandler> oHandlerList;

            WebCallResult res = InterviewHandler.GetInterviewHandlers(null, out oHandlerList, null);
            Assert.IsFalse(res.Success, "GetInterviewHandler should fail with null ConnectionServer passed to it");

        }


        [TestMethod]
        public void InterviewHandlers_VoiceName()
        {
            return;
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
        public void InterviewHandlers_Questions()
        {
            var oQuestions = _tempHandler.GetInterviewQuestions();
            Assert.IsNotNull(oQuestions,"Null returned for questions fetch");
            Assert.IsTrue(oQuestions.Count>1,"No questions returned from fetch");

            InterviewQuestion oQuestion = oQuestions[0];
            Console.WriteLine(oQuestions.ToString());
            Console.WriteLine(oQuestion.DumpAllProps());

            //try and fetch stream

            //set stream

            //fetch it again

            //change description text

            //set invalid property

        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void GetInterviewHandler_Failure()
        {
            InterviewHandler oHandler;

            WebCallResult res = InterviewHandler.GetInterviewHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetInterviewHandler should fail if the ConnectionServer is null");

            res = InterviewHandler.GetInterviewHandler(out oHandler, _connectionServer);
            Assert.IsFalse(res.Success, "GetInterviewHandler should fail if the ObjectId and display name are both blank");
        }

        [TestMethod]
        public void InterviewHandler_UpdateTests()
        {
            return;
            WebCallResult res = _tempHandler.Update();
            Assert.IsFalse(res.Success,"Updating interview handler with no pending changes did not fail");

            _tempHandler.DisplayName = "Updated Display Name";
            


        }
    }
}
