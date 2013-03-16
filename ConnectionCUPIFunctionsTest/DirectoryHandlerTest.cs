using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for DirectoryHandlerTest and is intended
    ///to contain all DirectoryHandler Unit Tests
    ///</summary>
    [TestClass]
    public class DirectoryHandlerTest
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
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start DirectoryHandler test:" + ex.Message);
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
            DirectoryHandler oTestHandler = new DirectoryHandler(null);
        }


        /// <summary>
        /// Get first handler in directory using static method call, iterate over it and use the ToString and DumpAllProps
        /// methods on it.
        /// For Directory handlers there should always be one in a valid Connection installation
        /// </summary>
        [TestMethod()]
        public void GetDirectoryHandlers_Test()
        {
            WebCallResult res;
            List<DirectoryHandler> oHandlerList;
            DirectoryHandler oNewHandler;

            //limit the fetch to the first 1 handler 
            string[] pClauses = { "rowsPerPage=1" };

            res = DirectoryHandler.GetDirectoryHandlers(_connectionServer, out oHandlerList, pClauses);

            Assert.IsTrue(res.Success, "Fetching of first directory handler failed: " + res.ToString());
            Assert.AreEqual(oHandlerList.Count, 1, "Fetching of the first directory handler returned a different number of handlers: " + res.ToString());

            //exercise the ToString and DumpAllProperties as part of this test as well
            foreach (DirectoryHandler oHandler in oHandlerList)
            {
                Console.WriteLine(oHandler.ToString());
                Console.WriteLine(oHandler.DumpAllProps());

                //fetch a new directory handler using the objectId 
                res = DirectoryHandler.GetDirectoryHandler(out oNewHandler, _connectionServer, oHandler.ObjectId);
                Assert.IsTrue(res.Success, "Fetching directory handler by ObjectId: " + res.ToString());
            }

            //hit failed searches
            res = DirectoryHandler.GetDirectoryHandler(out oNewHandler, _connectionServer,"","bogus name that shouldnt match");
            Assert.IsFalse(res.Success, "Fetching directory handler by bogus name did not fail");

        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void GetDirectoryHandlers_Failure()
        {
            WebCallResult res;
            List<DirectoryHandler> oHandlerList;

            res = DirectoryHandler.GetDirectoryHandlers(null, out oHandlerList, null);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail with null ConnectionServer passed to it");

        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void GetDirectoryHandler_Failure()
        {
            WebCallResult res;
            DirectoryHandler oHandler;

            res = DirectoryHandler.GetDirectoryHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ConnectionServer is null");

            res = DirectoryHandler.GetDirectoryHandler(out oHandler, _connectionServer, "", "");
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ObjectId and display name are both blank");
        }
    }
}
