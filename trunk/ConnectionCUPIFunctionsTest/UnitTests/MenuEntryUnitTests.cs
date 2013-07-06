using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for MenuEntryUnitTests and is intended
    ///to contain all MenuEntryUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class MenuEntryUnitTests : BaseUnitTests
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
            MenuEntry oTest = new MenuEntry(null, "aaaa");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure class creation failes if an empty objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyObjectId_Failure()
        {
            MenuEntry oTest = new MenuEntry(_mockServer, "");
            Console.WriteLine(oTest);
        }

        #endregion


        /// <summary>
        /// exercise menu entry failure points
        /// </summary>
        [TestMethod]
        public void UpdateMenuEntry_NullConnectionServer_Failure()
        {
            //manual update failure calls
            WebCallResult res = MenuEntry.UpdateMenuEntry(null, "objectid", "1", null);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest parameter should fail");
        }

        [TestMethod]
        public void UpdateMenuEntry_EmptyObjectId_Failure()
        {
            var res = MenuEntry.UpdateMenuEntry(_mockServer, "", "1", null);
            Assert.IsFalse(res.Success, "Empty CallHandlerObjectID value should fail");
        }


        [TestMethod]
        public void GetMenuEntry_NullConnectionServer_Failure()
        {
            //update menu entry
            MenuEntry oMenu;

            WebCallResult res = MenuEntry.GetMenuEntry(null, "objectid", "1", out oMenu);
            Assert.IsFalse(res.Success, "Calling GetMenuEntry with null Connection server should fail");
        }

        [TestMethod]
        public void GetMenuEntries_NullConnectionServer_Failure()
        {
            List<MenuEntry> oMenuEntries;
            var res = MenuEntry.GetMenuEntries(null, "objectid", out oMenuEntries);
            Assert.IsFalse(res.Success, "Calling GetMenuEntries with Null Connection server should fail");
        }


    }
}
