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
        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
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
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start InterviewHandler test:" + ex.Message);
            }

        }

        #endregion


        #region Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure_nullServer()
        {
            InterviewHandler oTestInterviewer = new InterviewHandler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure_InvalidObjectId()
        {
            InterviewHandler oTestInterviewer = new InterviewHandler(_connectionServer,"blah");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure_Name()
        {
            InterviewHandler oTestInterviewer = new InterviewHandler(_connectionServer, "","blah");
        }

        #endregion

        [TestMethod]
        public void StaticMethodFailures()
        {
            WebCallResult res;
            InterviewHandler oInterviewer;
            List<InterviewHandler> oHandlers;
            res = InterviewHandler.GetInterviewHandlers(null, out oHandlers);
            Assert.IsFalse(res.Success,"Calling static method GetInterviewHandlers did not fail with: null ConnectionServer");

            res = InterviewHandler.AddInterviewHandler(null, "display name", null,out oInterviewer);
            Assert.IsFalse(res.Success, "Calling static method AddInterviewHandler did not fail with: null ConnectionServer");

            res = InterviewHandler.AddInterviewHandler(_connectionServer, "", null,out oInterviewer);
            Assert.IsFalse(res.Success, "Calling static method AddInterviewHandler did not fail with: empty objectid ");

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

            res = InterviewHandler.GetInterviewHandler(out oInterviewer, _connectionServer, "", "");
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
            WebCallResult res;
            List<InterviewHandler> oHandlerList;
            string strObjectId="";

            InterviewHandler oInterviewHandler;

            //limit the fetch to the first 1 handler 
            string[] pClauses = { "rowsPerPage=1" };

            res = InterviewHandler.GetInterviewHandlers(_connectionServer, out oHandlerList, pClauses);

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



        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void GetInterviewHandlers_Failure()
        {
            WebCallResult res;
            List<InterviewHandler> oHandlerList;

            res = InterviewHandler.GetInterviewHandlers(null, out oHandlerList, null);
            Assert.IsFalse(res.Success, "GetInterviewHandler should fail with null ConnectionServer passed to it");

        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void GetInterviewHandler_Failure()
        {
            WebCallResult res;
            InterviewHandler oHandler;

            res = InterviewHandler.GetInterviewHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetInterviewHandler should fail if the ConnectionServer is null");

            res = InterviewHandler.GetInterviewHandler(out oHandler, _connectionServer, "", "");
            Assert.IsFalse(res.Success, "GetInterviewHandler should fail if the ObjectId and display name are both blank");
        }


    }
}
