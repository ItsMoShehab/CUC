using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for MenuEntryIntegrationTests and is intended
    ///to contain all MenuEntryIntegrationTests Unit Tests
    ///</summary>
    [TestClass]
    public class MenuEntryIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //call handler to use for testing
        private static CallHandler _callHandler;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

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
        /// Make sure class creation fails if an invalid ObjectId and key name are passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void CallHandlerMenuEntry_ClassCreationFailure2()
        {
            MenuEntry oTest = new MenuEntry(_connectionServer, "bogus","bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void GetMenuEntry_Instance_InvalidKeyName_Failure()
        {
            //update menu entry
            MenuEntry oMenu;

            //first test getting a bogus menu entry
            WebCallResult res = _callHandler.GetMenuEntry("a", out oMenu);
            Assert.IsFalse(res.Success, "GetMenuEntry should fail with an invalid key name");
        }

        [TestMethod]
        public void GetMenuEntry_FetchKey1_Success()
        {
            //update menu entry
            MenuEntry oMenu;
            var res = _callHandler.GetMenuEntry("1", out oMenu);
            Assert.IsTrue(res.Success, "Failed fetching the '1' menu key");

        }

        [TestMethod]
        public void GetMenuEntries_Success()
        {
            var oEntries = _callHandler.GetMenuEntries();
            Assert.IsNotNull(oEntries,"Null menu entries list returned");
            Assert.IsTrue(oEntries.Count==12,"12 menu entries not returned from fetch");

            //exercise tostring and dumpallprops interfaces
            Console.WriteLine(oEntries[0].ToString());
            Console.WriteLine(oEntries[0].DumpAllProps());
        }


        [TestMethod]
        public void MenuEntry_UpdateWithNoPendingChanges_Failure()
        {
            MenuEntry oMenu;
            WebCallResult res = _callHandler.GetMenuEntry("1", out oMenu);
            Assert.IsTrue(res.Success, "Failed fetching the '1' menu key");

            //an update with an empty change list should fail
            res = oMenu.Update();
            Assert.IsFalse(res.Success, "Update of a menu entry with no pending changes should fail");
        }
    
        [TestMethod]
        public void MenuEntry_UpdateLockedFlag_Success()
        {
            MenuEntry oMenu;
            WebCallResult res = _callHandler.GetMenuEntry("1", out oMenu);
            Assert.IsTrue(res.Success, "Failed fetching the '1' menu key");

            oMenu.Locked = true;
            res = oMenu.Update();
            Assert.IsTrue(res.Success, "Failed updating menu entry");

        }

        [TestMethod]
        public void MenuEntry_UpdateConversationToInvalid_Failure()
        {
            MenuEntry oMenu;
            WebCallResult res = _callHandler.GetMenuEntry("1", out oMenu);
            Assert.IsTrue(res.Success, "Failed fetching the '1' menu key");

            oMenu.TargetConversation = ConversationNames.Invalid;
            res = oMenu.Update();
            Assert.IsFalse(res.Success, "Update of a menu entry with invalid conversation should fail");

        }

        [TestMethod]
        public void MenuEntry_UpdateConversation_Success()
        {
            MenuEntry oMenu;
            WebCallResult res = _callHandler.GetMenuEntry("1", out oMenu);
            Assert.IsTrue(res.Success, "Failed fetching the '1' menu key");

            oMenu.TargetConversation = ConversationNames.PHTransfer;
            oMenu.TargetHandlerObjectId = _callHandler.ObjectId;
            oMenu.Action = ActionTypes.GoTo;
            res = oMenu.Update();
            Assert.IsTrue(res.Success,"Failed to update menu entry to point back to host call handler");
        }


        /// <summary>
        /// exercise menu entry failure points
        /// </summary>
        [TestMethod]
        public void UpdateMenuEntry_InvalidObjectId_Failure()
        {
            var res = MenuEntry.UpdateMenuEntry(_connectionServer, "aaa", "1", null);
            Assert.IsFalse(res.Success, "Invalid CallHandlerObjectId should fail");
        }

        [TestMethod]
        public void UpdateMenuEntry_InvalidKeyName_Failure()
        {
            var res = MenuEntry.UpdateMenuEntry(_connectionServer, _callHandler.ObjectId, "a", null);
            Assert.IsFalse(res.Success, "Invalid menu entry key name should fail");
        }

        [TestMethod]
        public void GetMenuEntry_InvalidObjectId_Failure()
        {
            //update menu entry
            MenuEntry oMenu;
            
            var res = MenuEntry.GetMenuEntry(_connectionServer, "aaa", "1", out oMenu);
            Assert.IsFalse(res.Success, "Calling GetMenuEntry with invalid ObjectId should fail");
        }

        [TestMethod]
        public void GetMenuEntry_InvalidKeyName_Failure()
        {
            //update menu entry
            MenuEntry oMenu;

            var res = MenuEntry.GetMenuEntry(_connectionServer, _callHandler.ObjectId, "a", out oMenu);
            Assert.IsFalse(res.Success, "Calling GetMenuEntry with invalid key name should fail");

       }

        [TestMethod]
        public void GetMenuEntries_InvalidObjectId_Failure()
        {
            //update menu entry
            List<MenuEntry> oMenuEntries;

            var res = MenuEntry.GetMenuEntries(_connectionServer, "aaa", out oMenuEntries);
            Assert.IsFalse(res.Success, "Calling GetMenuEntries with invalid ObjectId should fail");

            }

        [TestMethod]
        public void NewMenuEntry_Success()
        {
            //create a menu entry manually and then have it fill in
            var oMenu = new MenuEntry(_connectionServer, _callHandler.ObjectId);
            Assert.IsNotNull(oMenu, "Failed creating new MenuEntry instance");

            }

        [TestMethod]
        public void NewMenuEntry_GetMenuEntry_Instance_EmptyKeyName_Failure()
        {
            //create a menu entry manually and then have it fill in
            var oMenu = new MenuEntry(_connectionServer, _callHandler.ObjectId);
            Assert.IsNotNull(oMenu, "Failed creating new MenuEntry instance");

            var res = oMenu.GetMenuEntry("");
            Assert.IsFalse(res.Success, "Emtpy menu entry key should cause failure");

            }

        [TestMethod]
        public void NewMenuEntry_GetMenuEntry_Instance_InvalidKeyName_Failure()
        {
            //create a menu entry manually and then have it fill in
            var oMenu = new MenuEntry(_connectionServer, _callHandler.ObjectId);
            Assert.IsNotNull(oMenu, "Failed creating new MenuEntry instance");

            var res = oMenu.GetMenuEntry("a");
            Assert.IsFalse(res.Success, "Invalid menu entry key should cause failure");
        }

        [TestMethod]
        public void NewMenuEntry_GetMenuEntry_Instance_Success()
        {
            //create a menu entry manually and then have it fill in
            var oMenu = new MenuEntry(_connectionServer, _callHandler.ObjectId);
            Assert.IsNotNull(oMenu, "Failed creating new MenuEntry instance");

            var res = oMenu.GetMenuEntry("1");
            Assert.IsTrue(res.Success, "Failed fetching menu entry key");
        }

        [TestMethod]
        public void NewMenuEntry_Update_Instance_Success()
        {
            //create a menu entry manually and then have it fill in
            var oMenu = new MenuEntry(_connectionServer, _callHandler.ObjectId);
            Assert.IsNotNull(oMenu, "Failed creating new MenuEntry instance");

            var res = oMenu.GetMenuEntry("1");
            Assert.IsTrue(res.Success, "Failed fetching menu entry key");

            //add the transfer type to the menu and update it
            oMenu.TransferType = TransferTypes.Supervised;
            oMenu.TransferNumber = "123";
            oMenu.TransferRings = 3;
            oMenu.Action = ActionTypes.TransferToAlternateContactNumber;
            res = oMenu.Update();
            Assert.IsTrue(res.Success, "Failed updating menu entry:" + res);
        }

        #endregion
    }
}
