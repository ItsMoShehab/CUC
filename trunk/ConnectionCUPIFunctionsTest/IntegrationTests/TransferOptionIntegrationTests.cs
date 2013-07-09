using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for TransferOptionUnitTests and is intended
    ///to contain all TransferOptionUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class TransferOptionIntegrationTests : BaseIntegrationTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties 

        //class level call handler for use with testing - gets filled in with the opening greeting call handler data
        private static CallHandler _callHandler;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            //get call handler to do real tests
            WebCallResult res = CallHandler.GetCallHandler(out _callHandler, _connectionServer, "", "Opening Greeting");

            if (res.Success == false | _callHandler == null)
            {
                throw new Exception("Uanble to fetch Opening Greeting call handler for use in testing scenarios");
            }
        }

        #endregion


        #region Static Calls 

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_InvalidObjectId_Failure()
        {
            var res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, "objectid",TransferOptionTypes.Alternate, true);
            Assert.IsFalse(res.Success,"Calling UpdateTransferOptionEnabledStatus with Invalid ObjectId for call handler should fail");
        }
        
        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_DisableStandardRule_Failure()
        {
            var res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Standard, false);
            Assert.IsFalse(res.Success, "Disabling the standard transfer option should fail");
           }

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_DisableRuleWithDateInTheFuture_Failure()
        {
            var res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Alternate, 
                false, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Disabing a transfer option with a date in the future should fail");
         }

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_EnableWithDateInThePast_Failure()
        {
            var res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Alternate, 
                true, DateTime.Now.AddDays(-1));
            Assert.IsFalse(res.Success, "Enabling rule with date in the past should fail");
        }

        [TestMethod]
        public void UpdateTransferOption_InvalidObjectId_Failure()
        {
            var oProps = new ConnectionPropertyList();
            oProps.Add("test","test");
            var res = TransferOption.UpdateTransferOption(_connectionServer, "objectid", TransferOptionTypes.Alternate,oProps);
            Assert.IsFalse(res.Success, "Calling update for transfer options with no parameters should fail");
        }

        [TestMethod]
        public void UpdateTransferOption_MissingParameters_Failure()
        {
            var res = TransferOption.UpdateTransferOption(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Calling update for transfer options with no parameters should fail");
        }

        [TestMethod]
        public void GetTransferOptions_InvalidObjectId_Failure()
        {
            List<TransferOption> oTransferOptions;

            var res = TransferOption.GetTransferOptions(_connectionServer, "ObjectId", out oTransferOptions);
            Assert.IsFalse(res.Success, "Invalid CallHandlerObjectID should fail");
        }

        [TestMethod]
        public void GetTransferOption_InvalidTransferOptionType_Failure()
        {
            TransferOption oTransfer;
            
            var res = TransferOption.GetTransferOption(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Invalid , out oTransfer);
            Assert.IsFalse(res.Success, "Invalid transfer option type should fail");
        }

        [TestMethod]
        public void GetTransferOption_InvalidObjectId_Failure()
        {
            TransferOption oTransfer;

            var res = TransferOption.GetTransferOption(_connectionServer, "ObjectId", TransferOptionTypes.OffHours, out oTransfer);
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void TransferOption_Update_NoPendingChanges_Failure()
        {
            TransferOption oTransfer;
            var res = _callHandler.GetTransferOption(TransferOptionTypes.OffHours, out oTransfer);
            Assert.IsTrue(res.Success, "Failed to get off hours transfer option");

            oTransfer.ClearPendingChanges();

            //call to update with an empty change list should fail
            res = oTransfer.Update();
            Assert.IsFalse(res.Success, "Call to update transfer option with no pending changes should fail");
        }

        [TestMethod]
        public void TransferOption_Update_Success()
        {
            TransferOption oTransfer;
            var res = _callHandler.GetTransferOption(TransferOptionTypes.OffHours, out oTransfer);
            Assert.IsTrue(res.Success, "Failed to get off hours transfer option");

            oTransfer.Extension = "1234";
            oTransfer.TimeExpiresSetNull();
            res = oTransfer.Update();
            Assert.IsTrue(res.Success, "Failed updating transfer extension number for alternate rule:" + res.ToString());
        }

         [TestMethod]
         public void TransferOption_ToStringDumpAllPropsCall()
         {
             //iterate over all the transfer options and dump their contents out.
             foreach (TransferOption oTransferOption in _callHandler.GetTransferOptions())
             {
                 Console.WriteLine(oTransferOption.ToString());
                 Console.WriteLine(oTransferOption.DumpAllProps());
             }  
         }
        

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_Future_Success()
        {
            TransferOption oTransfer;
            var res = _callHandler.GetTransferOption(TransferOptionTypes.OffHours, out oTransfer);
            Assert.IsTrue(res.Success, "Failed to get off hours transfer option");
            
            res = oTransfer.UpdateTransferOptionEnabledStatus(true, DateTime.Now.AddMonths(1));
            Assert.IsTrue(res.Success, "Failed to activate Off Hourse transfer option:" + res.ToString());
        }


        [TestMethod]
        public void GetTransferOption_InvalidOption_Failure()
        {
            TransferOption oTransfer;
            var res = _callHandler.GetTransferOption(TransferOptionTypes.OffHours, out oTransfer);
            Assert.IsTrue(res.Success, "Failed to get off hours transfer option");
            
            res = oTransfer.GetTransferOption(TransferOptionTypes.Invalid);
            Assert.IsFalse(res.Success, "Fetching invalid transfer option should fail");

            }


        [TestMethod]
        public void GetTransferOption_Alternate_Success()
        {
            TransferOption oTransfer;
            var res = _callHandler.GetTransferOption(TransferOptionTypes.OffHours, out oTransfer);
            Assert.IsTrue(res.Success, "Failed to get off hours transfer option");

            res = oTransfer.GetTransferOption(TransferOptionTypes.Alternate);
            Assert.IsTrue(res.Success, "Fetching alternate transfer option failed");

            res = oTransfer.GetTransferOption(TransferOptionTypes.OffHours);
            Assert.IsTrue(res.Success, "Fetching off hours transfer option failed");
        }


        [TestMethod]
        public void GetTransferOption_ManualConstruction_OnvalidOptionType_Failure()
        {
            try
            {
                var oTransfer = new TransferOption(_connectionServer, _callHandler.ObjectId);
                Assert.Fail("Invalid transfer option type should fail.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure on construction with invalid transfer option:" + ex);
            }

        }

        [TestMethod]
        public void GetTransferOption_ManualConstruction_Success()
        {
            try
            {
                var oTransfer = new TransferOption(_connectionServer, _callHandler.ObjectId,TransferOptionTypes.Alternate);
                Assert.Fail("Invalid transfer option type should fail.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure on construction with invalid transfer option:" + ex);
            }

        }
        #endregion

    }
}
