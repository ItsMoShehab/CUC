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
    [TestClass]
    public class MenuEntryTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        //call handler to use for testing
        private static CallHandler _callHandler;

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
                throw new Exception("Unable to attach to Connection server to start MenuEntryTests test:" + ex.Message);
            }

            //grab the first template - should always be one and it doesn't matter which
            List<CallHandlerTemplate> oTemplates;
            WebCallResult res = CallHandlerTemplate.GetCallHandlerTemplates(_connectionServer, out oTemplates);
            if (res.Success == false || oTemplates == null || oTemplates.Count == 0)
            {
                Assert.Fail("Could not fetch call handler templates:" + res);
            }

            //create new handler with GUID in the name to ensure uniqueness
            String strName = "TempHandler_" + Guid.NewGuid().ToString().Replace("-", "");

            res = CallHandler.AddCallHandler(_connectionServer, oTemplates[0].ObjectId, strName, "", null, out _callHandler);
            Assert.IsTrue(res.Success, "Failed creating temporary callhandler:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_callHandler != null)
            {
                WebCallResult res = _callHandler.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary call handler on cleanup.");
            }
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CallHandlerMenuEntry_ClassCreationFailure()
        {
            MenuEntry oTest = new MenuEntry(null, "aaaa");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure class creation failes if an empty objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CallHandlerMenuEntry_ClassCreationFailure2()
        {
            MenuEntry oTest = new MenuEntry(_connectionServer, "");
            Console.WriteLine(oTest);
        }

        #endregion


        /// <summary>
        /// exercise menu entry functions
        /// </summary>
        [TestMethod]
        public void MenuEntry_FetchTest()
        {
            //update menu entry
            MenuEntry oMenu;

            //first test getting a bogus menu entry
            WebCallResult res = _callHandler.GetMenuEntry("a", out oMenu);
            Assert.IsFalse(res.Success, "GetMenuEntry should fail with an invalid key name");

            res = _callHandler.GetMenuEntry("1", out oMenu);
            Assert.IsTrue(res.Success, "Failed fetching the '1' menu key");

            var oEntries = _callHandler.GetMenuEntries();
            Assert.IsNotNull(oEntries,"Null menu entries list returned");
            Assert.IsTrue(oEntries.Count==12,"12 menu entries not returned from fetch");

            Console.WriteLine(oEntries[0].ToString());
            Console.WriteLine(oEntries[0].DumpAllProps());
        }

        [TestMethod]
        public void MenuEntry_UpdateTests()
        {
            MenuEntry oMenu;
            WebCallResult res = _callHandler.GetMenuEntry("1", out oMenu);
            Assert.IsTrue(res.Success, "Failed fetching the '1' menu key");

            //an update with an empty change list should fail
            res = oMenu.Update();
            Assert.IsFalse(res.Success, "Update of a menu entry with no pending changes should fail");

            oMenu.Locked = true;
            res = oMenu.Update();
            Assert.IsTrue(res.Success, "Failed updating menu entry");

            oMenu.TargetConversation = "Bogus";
            res = oMenu.Update();
            Assert.IsFalse(res.Success, "Update of a menu entry with invalid conversation should fail");

            oMenu.TargetConversation = ConversationNames.PHTransfer.ToString();
            oMenu.TargetHandlerObjectId = _callHandler.ObjectId;
            oMenu.Action = (int) ActionTypes.GoTo;
            res = oMenu.Update();
            Assert.IsTrue(res.Success,"Failed to update menu entry to point back to host call handler");
        }


        /// <summary>
        /// exercise menu entry failure points
        /// </summary>
        [TestMethod]
        public void UpdateMenuEntry_Failure()
        {
            //manual update failure calls
            WebCallResult res = MenuEntry.UpdateMenuEntry(null, _callHandler.ObjectId, "1", null);
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
        [TestMethod]
        public void GetMenuEntry_Failure()
        {
            //update menu entry
            MenuEntry oMenu;
            List<MenuEntry> oMenuEntries;

            WebCallResult res = MenuEntry.GetMenuEntry(null, _callHandler.ObjectId, "1", out oMenu);
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
