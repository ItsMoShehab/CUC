using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// This is a test class for TransferOptionIntegrationTests and is intended
    /// to contain all TransferOptionIntegrationTests Unit Tests
    ///</summary>
    [TestClass]
    public class TransferOptionUnitTests : BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties 

        #endregion


        #region Additional test attributes

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
            Reset();
            TransferOption oTest = new TransferOption(null,"objectid",TransferOptionTypes.Alternate);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyObjectId_Failure()
        {
            Reset();
            TransferOption oTest = new TransferOption(_mockServer, "", TransferOptionTypes.Alternate);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_InvalidTranserOptionType_Failure()
        {
            Reset();
            TransferOption oTest = new TransferOption(_mockServer, "objectid", TransferOptionTypes.Invalid);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        public void Constructor_Base_Success()
        {
            Reset();
            TransferOption oTest = new TransferOption();
            Console.WriteLine(oTest);
        }

        [TestMethod]
        public void Constructor_ObjectId_Success()
        {
            Reset();
            TransferOption oTest = new TransferOption(_mockServer,"ObjectId",TransferOptionTypes.OffHours);
            Console.WriteLine(oTest.ToString());
            Console.WriteLine(oTest.DumpAllProps());
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_ObjectId_ErrorResponse_Failure()
        {
            Reset();

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), true)).Returns(new WebCallResult
                                   {
                                       Success = false,
                                       ResponseText = "error text",
                                       StatusCode = 404
                                   });

            TransferOption oTest = new TransferOption(_mockServer, "ObjectId", TransferOptionTypes.Alternate);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Tests

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_NullConnectionServer_Failure()
        {
            WebCallResult res = TransferOption.UpdateTransferOptionEnabledStatus(null, "objectid",TransferOptionTypes.Alternate, true);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest parameter should fail");

        }

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_EmptyCallHandlerObjectId_Failure()
        {
            WebCallResult res = TransferOption.UpdateTransferOptionEnabledStatus(_mockServer, "", TransferOptionTypes.Alternate, true);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId parameter should fail");

        }

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_DisableStandardTransferRule_Failure()
        {
            var res = TransferOption.UpdateTransferOptionEnabledStatus(_mockServer, "objectid", TransferOptionTypes.Standard, false);
            Assert.IsFalse(res.Success, "Disabling the standard transfer option should fail");

        }
        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_DisableTransferOptionWithDateInTheFuture_Failure()
        {
            var res = TransferOption.UpdateTransferOptionEnabledStatus(_mockServer, "objectid", TransferOptionTypes.Alternate, false, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Disabing a transfer option with a date in the future should fail");
        }

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_EnableRuleWithDateInThePast_Failure()
        {
           var res = TransferOption.UpdateTransferOptionEnabledStatus(_mockServer, "objectid", TransferOptionTypes.Alternate, true, DateTime.Now.AddDays(-1));
            Assert.IsFalse(res.Success, "Enabling rule with date in the past should fail");
        }

        [TestMethod]
        public void UpdateTransferOption_NullConnectionServer_Failure()
        {
            WebCallResult res = TransferOption.UpdateTransferOption(null, "objectid", TransferOptionTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Updating transfer options with null ConnectionServerRest param should fail");

            }

        [TestMethod]
        public void UpdateTransferOption_NullParameters_Failure()
        {
            var res = TransferOption.UpdateTransferOption(_mockServer, "objectid", TransferOptionTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Calling update for transfer options with no parameters should fail");
        }

        [TestMethod]
        public void UpdateTransferOption_EmptyParameters_Failure()
        {
            var res = TransferOption.UpdateTransferOption(_mockServer, "objectid", TransferOptionTypes.Alternate, new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling update for transfer options with no parameters should fail");
        }

        [TestMethod]
        public void GetTransferOptions_NullConnectionServer_Failure()
        {
            List<TransferOption> oTransferOptions;

            WebCallResult res = TransferOption.GetTransferOptions(null, "objectid", out oTransferOptions);
            Assert.IsFalse(res.Success, "Null Connection server parameter should fail");
        }

        [TestMethod]
        public void GetTransferOption_InvalidTransferType_Failure()
        {
            TransferOption oTransfer;
            var res = TransferOption.GetTransferOption(_mockServer, "objectid", TransferOptionTypes.Invalid, out oTransfer);
            Assert.IsFalse(res.Success, "Invalid transfer option type should fail");
        }

        [TestMethod]
        public void GetTransferOption_NullConnectionServer_Failure()
        {
            TransferOption oTransfer; 
            var res = TransferOption.GetTransferOption(null, "", TransferOptionTypes.Alternate, out oTransfer);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest parameter should fail");
        }

        [TestMethod]
        public void GetTransferOption_EmptyObjectId_Failure()
        {
            TransferOption oTransfer; 
            var res = TransferOption.GetTransferOption(_mockServer, "", TransferOptionTypes.Standard, out oTransfer);
            Assert.IsFalse(res.Success, "Empty ObjectId should should fail");

            }

        #endregion


        #region Property Tests

        [TestMethod]
        public void PropertyGetFetch_Action()
        {
            TransferOption oOptions = new TransferOption();
            const ActionTypes expectedValue = ActionTypes.Error;
            oOptions.Action = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("Action", (int)expectedValue),"Action value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Extension()
        {
            TransferOption oOptions = new TransferOption();
            const string expectedValue = "String value";
            oOptions.Extension = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("Extension", expectedValue), "Extension value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MediaSwitchObjectId()
        {
            TransferOption oOptions = new TransferOption();
            const string expectedValue = "String value";
            oOptions.MediaSwitchObjectId = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("MediaSwitchObjectId", expectedValue), "MediaSwitchObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PersonalCallTransfer()
        {
            TransferOption oOptions = new TransferOption();
            const bool expectedValue = true;
            oOptions.PersonalCallTransfer = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("PersonalCallTransfer", expectedValue), "PersonalCallTransfer value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PlayTransferPrompt()
        {
            TransferOption oOptions = new TransferOption();
            const bool expectedValue = false;
            oOptions.PlayTransferPrompt = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("PlayTransferPrompt", expectedValue), "PlayTransferPrompt value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RnaAction()
        {
            TransferOption oOptions = new TransferOption();
            const TransferActionTypes expectedValue = TransferActionTypes.Transfer;
            oOptions.RnaAction = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("RnaAction", (int)expectedValue), "RnaAction value get fetch failed");
        }

        /// <summary>
        /// special case = operates as a method that puts an empty string value on the change queue to the timeExpires
        /// property is nulled out (which means activate the rule forever)
        /// </summary>
        [TestMethod]
        public void PropertyGetFetch_TimeExpiresSetNull()
        {
            TransferOption oOptions = new TransferOption();
            const string expectedValue = "";
            oOptions.TimeExpiresSetNull();
            Assert.IsTrue(oOptions.ChangeList.ValueExists("TimeExpires", expectedValue), "TimeExpires value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferAnnounce()
        {
            TransferOption oOptions = new TransferOption();
            const bool expectedValue = false;
            oOptions.TransferAnnounce = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("TransferAnnounce", expectedValue), "TransferAnnounce value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferConfirm()
        {
            TransferOption oOptions = new TransferOption();
            const bool expectedValue = false;
            oOptions.TransferConfirm = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("TransferConfirm", expectedValue), "TransferConfirm value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferDtDetect()
        {
            TransferOption oOptions = new TransferOption();
            const bool expectedValue = false;
            oOptions.TransferDtDetect = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("TransferDtDetect", expectedValue), "TransferDtDetect value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferHoldingMode()
        {
            TransferOption oOptions = new TransferOption();
            const ModeYesNoAsk expectedValue = ModeYesNoAsk.Ask;
            oOptions.TransferHoldingMode = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("TransferHoldingMode", (int)expectedValue), "TransferHoldingMode value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferIntroduce()
        {
            TransferOption oOptions = new TransferOption();
            const bool expectedValue = false;
            oOptions.TransferIntroduce = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("TransferIntroduce", expectedValue), "TransferIntroduce value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferRings()
        {
            TransferOption oOptions = new TransferOption();
            const int expectedValue = 7;
            oOptions.TransferRings = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("TransferRings", expectedValue), "TransferRings value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferScreening()
        {
            TransferOption oOptions = new TransferOption();
            const bool expectedValue = false;
            oOptions.TransferScreening = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("TransferScreening", expectedValue), "TransferScreening value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferType()
        {
            TransferOption oOptions = new TransferOption();
            const TransferTypes expectedValue = TransferTypes.Unsupervised;
            oOptions.TransferType = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("TransferType", (int)expectedValue), "TransferType value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TUsePrimaryExtension()
        {
            TransferOption oOptions = new TransferOption();
            const bool expectedValue = false;
            oOptions.UsePrimaryExtension = expectedValue;
            Assert.IsTrue(oOptions.ChangeList.ValueExists("UsePrimaryExtension", expectedValue), "UsePrimaryExtension value get fetch failed");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetTransferOptions_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<TransferOption> oOptions;
            var res = TransferOption.GetTransferOptions(_mockServer, "CallHandlerObjectId", out oOptions);
            Assert.IsFalse(res.Success, "Calling GetTransferOptions with EmptyResultText did not fail");

        }

        [TestMethod]
        public void GetTransferOptions_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<TransferOption> oOptions;
            var res = TransferOption.GetTransferOptions(_mockServer, "CallHandlerObjectId", out oOptions);
            Assert.IsFalse(res.Success, "Calling GetTransferOptions with garbage results should fail");
            Assert.IsTrue(oOptions.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetTransferOptions_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<TransferOption> oOptions;
            var res = TransferOption.GetTransferOptions(_mockServer, "CallHandlerObjectId", out oOptions);
            Assert.IsFalse(res.Success, "Calling GetTransferOptions with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GeGetTransferOptions_ZeroCount_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<TransferOption> oOptions;
            var res = TransferOption.GetTransferOptions(_mockServer, "CallHandlerObjectId", out oOptions);
            Assert.IsFalse(res.Success, "Calling GetTransferOptions with ZeroCount should fail");
        }

        [TestMethod]
        public void UpdateTransferOption_ErrorResponse_Failure()
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
            var res = TransferOption.UpdateTransferOption(_mockServer, "CallHandlerObjectId", TransferOptionTypes.Alternate, oProps );
            Assert.IsFalse(res.Success, "Calling UpdateTransferOption with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_Disable_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = TransferOption.UpdateTransferOptionEnabledStatus(_mockServer, "CallHandlerObjectId", TransferOptionTypes.Alternate,false);
            Assert.IsFalse(res.Success, "Calling UpdateTransferOptionEnabledStatus to disable with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_EnableTillDate_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = TransferOption.UpdateTransferOptionEnabledStatus(_mockServer, "CallHandlerObjectId", TransferOptionTypes.Alternate, true,
                DateTime.Now.AddDays(2));
            Assert.IsFalse(res.Success, "Calling UpdateTransferOptionEnabledStatus enable with future date with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_EnableForever_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = TransferOption.UpdateTransferOptionEnabledStatus(_mockServer, "CallHandlerObjectId", TransferOptionTypes.Alternate, true,null);
            Assert.IsFalse(res.Success, "Calling UpdateTransferOptionEnabledStatus enable forever with ErrorResponse did not fail");
        }


        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            Reset();
            TransferOption oOption = new TransferOption(_mockServer,"HandlerObjectId", TransferOptionTypes.Alternate);
            var res = oOption.Update();
            Assert.IsFalse(res.Success,"Calling update with no pending changes should fail");
        }

        [TestMethod]
        public void Update_ErrorResponse_Failure()
        {
            Reset();
            TransferOption oOption = new TransferOption(_mockServer, "HandlerObjectId", TransferOptionTypes.Alternate);
            oOption.Extension = "1234";

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            
            var res = oOption.Update();
            Assert.IsFalse(res.Success, "Calling update with error response should fail");
        }

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_ErrorResponse_Failure()
        {
            Reset();
            TransferOption oOption = new TransferOption(_mockServer, "HandlerObjectId", TransferOptionTypes.Alternate);

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = oOption.UpdateTransferOptionEnabledStatus(false);
            Assert.IsFalse(res.Success, "Calling UpdateTransferOptionEnabledStatus with error response should fail");
        }

        [TestMethod]
        public void RefetchTransferOptionData_ErrorResponse_Failure()
        {
            Reset();
            TransferOption oOption = new TransferOption(_mockServer, "HandlerObjectId", TransferOptionTypes.Alternate);

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = oOption.RefetchTransferOptionData();
            Assert.IsFalse(res.Success, "Calling UpdateTransferOptionEnabledStatus with error response should fail");
        }

        #endregion
    }
}
