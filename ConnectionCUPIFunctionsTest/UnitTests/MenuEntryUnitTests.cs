using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

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
            MenuEntry oTest = new MenuEntry(null, "ParentObjectId");
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


        [TestMethod]
        public void Constructor_EmptyKeyName_Success()
        {
            MenuEntry oTest = new MenuEntry(_mockServer, "ParentObjectId");
            Console.WriteLine(oTest.SelectionDisplayString);
            Console.WriteLine(oTest.UniqueIdentifier);
        }

        [TestMethod]
        public void Constructor_Success()
        {
            MenuEntry oTest = new MenuEntry(_mockServer, "ParentObjectId","1");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                    });
            MenuEntry oTest = new MenuEntry(_mockServer, "ParentObjectId", "1");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            MenuEntry oTest = new MenuEntry();
            Console.WriteLine(oTest.ToString());
            Console.WriteLine(oTest.DumpAllProps());
        }

        #endregion


        #region Static Method Tests

        [TestMethod]
        public void UpdateMenuEntry_NullConnectionServer_Failure()
        {
            WebCallResult res = MenuEntry.UpdateMenuEntry(null, "objectid", "1", null);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest parameter should fail");
        }

        [TestMethod]
        public void UpdateMenuEntry_NullPropertyList_Failure()
        {
            var res = MenuEntry.UpdateMenuEntry(_mockServer, "ObjectId", "1", null);
            Assert.IsFalse(res.Success, "Null property list value should fail");
        }

        [TestMethod]
        public void UpdateMenuEntry_EmptyPropertyList_Failure()
        {
            var res = MenuEntry.UpdateMenuEntry(_mockServer, "ObjectId", "1", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Empty property list value should fail");
        }

        [TestMethod]
        public void UpdateMenuEntry_EmptyObjectId_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test","test");
            var res = MenuEntry.UpdateMenuEntry(_mockServer, "", "1", oProps);
            Assert.IsFalse(res.Success, "Empty CallHandlerObjectId property should fail");
        }

        [TestMethod]
        public void UpdateMenuEntry_EmptyKeyName_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test", "test");
            var res = MenuEntry.UpdateMenuEntry(_mockServer, "ObjectId", "", oProps);
            Assert.IsFalse(res.Success, "Empty KeyName should fail");
        }

        [TestMethod]
        public void GetMenuEntry_NullConnectionServer_Failure()
        {
            MenuEntry oMenu;

            WebCallResult res = MenuEntry.GetMenuEntry(null, "objectid", "1", out oMenu);
            Assert.IsFalse(res.Success, "Calling GetMenuEntry with null Connection server should fail");
        }

        [TestMethod]
        public void GetMenuEntry_EmptyHandlerObjectId_Failure()
        {
            MenuEntry oMenu;

            WebCallResult res = MenuEntry.GetMenuEntry(_mockServer, "", "1", out oMenu);
            Assert.IsFalse(res.Success, "Calling GetMenuEntry with empty call handler ObjectId should fail");
        }

        [TestMethod]
        public void GetMenuEntries_NullConnectionServer_Failure()
        {
            List<MenuEntry> oMenuEntries;
            var res = MenuEntry.GetMenuEntries(null, "objectid", out oMenuEntries);
            Assert.IsFalse(res.Success, "Calling GetMenuEntries with Null Connection server should fail");
        }

        #endregion


        #region Property Tests

        [TestMethod]
        public void PropertyGetFetch_AfterMessageAction()
        {
            MenuEntry oMenuEntry = new MenuEntry();
            const ActionTypes expectedValue = ActionTypes.Error;
            oMenuEntry.Action = expectedValue;
            Assert.IsTrue(oMenuEntry.ChangeList.ValueExists("Action", (int)expectedValue),"Action value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Locked()
        {
            MenuEntry oMenuEntry = new MenuEntry();
            const bool expectedValue = false;
            oMenuEntry.Locked = expectedValue;
            Assert.IsTrue(oMenuEntry.ChangeList.ValueExists("Locked", expectedValue), "Locked value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TargetConversation()
        {
            MenuEntry oMenuEntry = new MenuEntry();
            const ConversationNames expectedValue = ConversationNames.SubSignIn;
            oMenuEntry.TargetConversation = expectedValue;
            Assert.IsTrue(oMenuEntry.ChangeList.ValueExists("TargetConversation", expectedValue.ToString()), "TargetConversation value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TargetHandlerObjectId()
        {
            MenuEntry oMenuEntry = new MenuEntry();
            const string expectedValue = "Test string";
            oMenuEntry.TargetHandlerObjectId = expectedValue;
            Assert.IsTrue(oMenuEntry.ChangeList.ValueExists("TargetHandlerObjectId", expectedValue), "TargetHandlerObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferNumber()
        {
            MenuEntry oMenuEntry = new MenuEntry();
            const string expectedValue = "Test string";
            oMenuEntry.TransferNumber = expectedValue;
            Assert.IsTrue(oMenuEntry.ChangeList.ValueExists("TransferNumber", expectedValue), "TransferNumber value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferRings()
        {
            MenuEntry oMenuEntry = new MenuEntry();
            const int expectedValue = 13;
            oMenuEntry.TransferRings = expectedValue;
            Assert.IsTrue(oMenuEntry.ChangeList.ValueExists("TransferRings", expectedValue), "TransferRings value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferType()
        {
            MenuEntry oMenuEntry = new MenuEntry();
            const TransferTypes expectedValue = TransferTypes.Supervised;
            oMenuEntry.TransferType = expectedValue;
            Assert.IsTrue(oMenuEntry.ChangeList.ValueExists("TransferType", (int)expectedValue), "TransferType value get fetch failed");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetMenuEntries_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<MenuEntry> oMenuEntries;
            var res = MenuEntry.GetMenuEntries(_mockServer,"CallHandlerObjectId", out oMenuEntries);
            Assert.IsFalse(res.Success, "Calling GetMenuEntries with EmptyResultText did not fail");
        }


        [TestMethod]
        public void GetMenuEntries_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<MenuEntry> oMenuEntries;
            var res = MenuEntry.GetMenuEntries(_mockServer, "CallHandlerObjectId", out oMenuEntries);
            Assert.IsFalse(res.Success, "Calling GetMenuEntries with garbage results should fail");
            Assert.IsTrue(oMenuEntries.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetMenuEntries_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<MenuEntry> oMenuEntries;
            var res = MenuEntry.GetMenuEntries(_mockServer, "CallHandlerObjectId", out oMenuEntries);
            Assert.IsFalse(res.Success, "Calling GetMenuEntries with ErrorResponse did not fail");
        }

        /// <summary>
        /// Zero count in this case should fail - ALL call handlers should ALWAYS have 12 menu entries no matter what.
        /// </summary>
        [TestMethod]
        public void GetMenuEntries_ZeroCount_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<MenuEntry> oMenuEntries;
            var res = MenuEntry.GetMenuEntries(_mockServer, "CallHandlerObjectId", out oMenuEntries);
            Assert.IsFalse(res.Success, "Calling GetMenuEntries with ZeroCount should fail");
        }

        [TestMethod]
        public void GetMenuEnty_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            MenuEntry oMenuEntry;
            var res = MenuEntry.GetMenuEntry(_mockServer, "CallHandlerObjectId", "1", out oMenuEntry);
            Assert.IsFalse(res.Success, "Calling GetMenuEntry with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdateMenuEntry_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test","Test");
            var res = MenuEntry.UpdateMenuEntry(_mockServer, "CallHandlerObjectId", "1", oProps);
            Assert.IsFalse(res.Success, "Calling UpdateMenuEntry with ErrorResponse did not fail");
        }

        [TestMethod]
        public void Update_EmptyChangeList_Failure()
        {
            var oMenuEntry = new MenuEntry(_mockServer, "ObjectId");
            var res = oMenuEntry.Update();
            Assert.IsFalse(res.Success,"Calling Update with no pending changes should fail");
        }

        [TestMethod]
        public void Update_ErrorResponse_Failure()
        {
            var oMenuEntry = new MenuEntry(_mockServer, "ObjectId");
            oMenuEntry.Locked = false;
            
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            var res = oMenuEntry.Update();
            Assert.IsFalse(res.Success, "Calling Update with error response should fail");
        }

        [TestMethod]
        public void GetMenuEntry_EmptyKeyName_Failure()
        {
            var oMenuEntry = new MenuEntry(_mockServer, "ObjectId");
            var res = oMenuEntry.GetMenuEntry("");
            Assert.IsFalse(res.Success, "Calling GetMenuEntry with no key name should fail");
        }

        [TestMethod]
        public void GetMenuEntry_GarbageResponse_Failure()
        {
            var oMenuEntry = new MenuEntry(_mockServer, "ObjectId");
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            var res = oMenuEntry.GetMenuEntry("1");
            Assert.IsFalse(res.Success, "Calling GetMenuEntry with garbage results should fail");
        }


        [TestMethod]
        public void RefetchMenuEntryData_ErrorResponse_Failure()
        {
            var oMenuEntry = new MenuEntry(_mockServer, "ObjectId");
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            var res = oMenuEntry.RefetchMenuEntryData();
            Assert.IsFalse(res.Success,"Calling RefetchMenuEntry with an error response should fail");
        }

        #endregion
    }
}
