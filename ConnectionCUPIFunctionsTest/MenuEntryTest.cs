using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{


    /// <summary>
    ///This is a test class for MenuEntryTest and is intended
    ///to contain all MenuEntryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MenuEntryTest
    {
        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        //call handler to use for testing
        private static CallHandler _callHandler;

        private TestContext testContextInstance;


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
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

            //get the opening greeting call handler so it can be used for various menu entry tests.
            WebCallResult res = CallHandler.GetCallHandler(out _callHandler, _connectionServer, "", "Opening Greeting");
            if (res.Success == false | _callHandler == null)
            {
                throw new Exception("Unable to get opening greeting call handler for use in testing");
            }


        }

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        /// <summary>
        /// exercise menu entry functions
        /// </summary>
        [TestMethod()]
        public void MenuEntry_Test()
        {
            WebCallResult res;

            //update menu entry
            MenuEntry oMenu;

            //first test getting a bogus menu entry
            res = _callHandler.GetMenuEntry("a", out oMenu);
            Assert.IsFalse(res.Success, "GetMenuEntry should fail with an invalid key name");

            res = _callHandler.GetMenuEntry("1", out oMenu);
            Assert.IsTrue(res.Success, "Failed fetching the '1' menu key");

            oMenu.ClearPendingChanges();

            //an update with an empty change list should fail
            res = oMenu.Update();
            Assert.IsFalse(res.Success, "Update of a menu entry with no pending changes should fail");

            oMenu.Locked = true;
            res = oMenu.Update();
            Assert.IsTrue(res.Success, "Failed updating menu entry");

            //Iterate over all the menu entries and dump their contents
            foreach (MenuEntry oMenus in _callHandler.GetMenuEntries())
            {
                Console.WriteLine(oMenus.ToString());
                Console.WriteLine(oMenus.DumpAllProps());
            }
        }


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CallHandlerMenuEntry_ClassCreationFailure()
        {
            MenuEntry oTestTemplate = new MenuEntry(null, "aaaa");
        }

        /// <summary>
        /// Make sure class creation failes if an empty objectId is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CallHandlerMenuEntry_ClassCreationFailure2()
        {
            MenuEntry oTestTemplate = new MenuEntry(_connectionServer, "");
        }


        /// <summary>
        /// exercise menu entry failure points
        /// </summary>
        [TestMethod()]
        public void UpdateMenuEntry_Failure()
        {
            WebCallResult res;

            //manual update failure calls
            res = MenuEntry.UpdateMenuEntry(null, _callHandler.ObjectId, "1", null);
            Assert.IsFalse(res.Success, "Null ConnectionServer parameter should fail");

            res = MenuEntry.UpdateMenuEntry(_connectionServer, "", "1", null);
            Assert.IsFalse(res.Success, "Empty CallHandlerObjectID value should fail");

            res = MenuEntry.UpdateMenuEntry(_connectionServer, "aaa", "1", null);
            Assert.IsFalse(res.Success, "Invalid CallHandlerObjectId should fail");

            res = MenuEntry.UpdateMenuEntry(_connectionServer, _callHandler.ObjectId, "a", null);
            Assert.IsFalse(res.Success, "Invalid menu entry key name should fail");

        }

        /// <summary>
        /// exercise menu entry failure points
        /// </summary>
        [TestMethod()]
        public void GetMenuEntry_Failure()
        {
            WebCallResult res;

            //update menu entry
            MenuEntry oMenu;
            List<MenuEntry> oMenuEntries;

            res = MenuEntry.GetMenuEntry(null, _callHandler.ObjectId, "1", out oMenu);
            Assert.IsFalse(res.Success, "");

            res = MenuEntry.GetMenuEntry(_connectionServer, "aaa", "1", out oMenu);
            Assert.IsFalse(res.Success, "");

            res = MenuEntry.GetMenuEntry(_connectionServer, _callHandler.ObjectId, "a", out oMenu);
            Assert.IsFalse(res.Success, "");

            res = MenuEntry.GetMenuEntries(null, _callHandler.ObjectId, out oMenuEntries);
            Assert.IsFalse(res.Success, "");

            res = MenuEntry.GetMenuEntries(_connectionServer, "aaa", out oMenuEntries);
            Assert.IsFalse(res.Success, "");

            //greate a menu entry manually and then have it fill in
            oMenu = new MenuEntry(_connectionServer, _callHandler.ObjectId);
            Assert.IsNotNull(oMenu, "Failed creating new MenuEntry instance");

            res = oMenu.GetMenuEntry("");
            Assert.IsFalse(res.Success, "Emtpy menu entry key should cause failure");

            res = oMenu.GetMenuEntry("a");
            Assert.IsFalse(res.Success, "Invalid menu entry key should cause failure");

            res = oMenu.GetMenuEntry("1");
            Assert.IsTrue(res.Success, "Failed fetching menu entry key");

            //add the transfer type to the menu and update it
            oMenu.TransferType = (int)TransferTypes.Supervised;
            oMenu.TransferNumber = "123";
            oMenu.TransferRings = 3;
            oMenu.Action = (int)ActionTypes.TransferToAlternateContactNumber;
            res = oMenu.Update();
            Assert.IsTrue(res.Success, "Failed updating menu entry:"+res);

        }


    }
}
