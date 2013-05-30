using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for TransferOptionTest and is intended
    ///to contain all TransferOptionTest Unit Tests
    ///</summary>
    [TestClass]
    public class TransferOptionTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties 

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        //class level call handler for use with testing - gets filled in with the opening greeting call handler data
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
                 _connectionServer = new ConnectionServerRest(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start TransferOption test:" + ex.Message);
            }

            //get call handler to do real tests
            WebCallResult res = CallHandler.GetCallHandler(out _callHandler, _connectionServer, "", "Opening Greeting");

            if (res.Success == false | _callHandler == null)
            {
                throw new Exception("Uanble to fetch Opening Greeting call handler for use in testing scenarios");
            }

        }

        #endregion


        #region Static Call Failures

        /// <summary>
        /// exercise transfer options failure cases 
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateTransferOptionEnabledStatus()
        {
            //hit some invalid calls for updating the enabled status for transfer options
            WebCallResult res = TransferOption.UpdateTransferOptionEnabledStatus(null, _callHandler.ObjectId, TransferOptionTypes.Alternate, true);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest parameter should fail");

            res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, "aaa", TransferOptionTypes.Alternate, true);
            Assert.IsFalse(res.Success, "Invalid ObjectId for call handler should fail");

            res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Standard, false);
            Assert.IsFalse(res.Success, "Disabling the standard transfer option should fail");

            res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Alternate, false, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Disabing a transfer option with a date in the past should fail");

            res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Alternate, true, DateTime.Now.AddDays(-1));
            Assert.IsFalse(res.Success, "Enabling rule with date in the past should fail");

        }

        /// <summary>
        /// exercise transfer options failure cases 
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateTransferOption()
        {
            //check manually editing properties on transfer options failure cases
            WebCallResult res = TransferOption.UpdateTransferOption(null, _callHandler.ObjectId, TransferOptionTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Updating transfer options with null ConnectionServerRest param should fail");

            res = TransferOption.UpdateTransferOption(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Calling update for transfer options with no parameters should fail");

        }

        /// <summary>
        /// exercise transfer options failure cases 
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetTransferOptions()
        {
            List<TransferOption> oTransferOptions;

            WebCallResult res = TransferOption.GetTransferOptions(null, _callHandler.ObjectId, out oTransferOptions);
            Assert.IsFalse(res.Success, "Null Connection server parameter should fail");

            res = TransferOption.GetTransferOptions(_connectionServer, "aaa", out oTransferOptions);
            Assert.IsFalse(res.Success, "Invalid CallHandlerObjectID should fail");

        }

        /// <summary>
        /// exercise transfer options failure cases 
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetTransferOption()
        {
            TransferOption oTransfer;
            
            var res = TransferOption.GetTransferOption(_connectionServer, _callHandler.ObjectId, TransferOptionTypes.Invalid , out oTransfer);
            Assert.IsFalse(res.Success, "Invalid transfer option type should fail");

            res = TransferOption.GetTransferOption(null, "", TransferOptionTypes.Alternate , out oTransfer);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest parameter should fail");

            res = TransferOption.GetTransferOption(_connectionServer, "", TransferOptionTypes.Standard , out oTransfer);
            Assert.IsFalse(res.Success, "Empty ObjectId should should fail");

            //make sure invalid Connection server param is caught
            res = TransferOption.GetTransferOption(null, _callHandler.ObjectId, TransferOptionTypes.Alternate , out oTransfer);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest parameter should fail");

        }

        #endregion

        /// <summary>
        /// exercise transfer options
        /// </summary>
        [TestMethod]
        public void TransferOption_Test()
        {
            TransferOption oTransfer;

            //now get the off hours transfer rule and enable it for a month.
           var res = _callHandler.GetTransferOption(TransferOptionTypes.OffHours , out oTransfer);
            Assert.IsTrue(res.Success, "Failed to get off hours transfer option");

            oTransfer.ClearPendingChanges();

            //call to update with an empty change list should fail
            res = oTransfer.Update();
            Assert.IsFalse(res.Success, "Call to update transfer option with no pending changes should fail");

            oTransfer.Extension = "1234";
            oTransfer.TimeExpiresSetNull();
            res = oTransfer.Update();
            Assert.IsTrue(res.Success, "Failed updating transfer extension number for alternate rule:" + res.ToString());

            res = oTransfer.UpdateTransferOptionEnabledStatus(true, DateTime.Now.AddMonths(1));
            Assert.IsTrue(res.Success, "Failed to activate Off Hourse transfer option:" + res.ToString());

            //iterate over all the transfer options and dump their contents out.
            foreach (TransferOption oTransferOption in _callHandler.GetTransferOptions())
            {
                Console.WriteLine(oTransferOption.ToString());
                Console.WriteLine(oTransferOption.DumpAllProps());
            }

            res = oTransfer.GetTransferOption(TransferOptionTypes.Invalid);
            Assert.IsFalse(res.Success, "Fetching invalid transfer option should fail");

            //now fetch it properly
            res = oTransfer.GetTransferOption(TransferOptionTypes.Alternate);
            Assert.IsTrue(res.Success, "Fetching alternate transfer option failed");

            res = oTransfer.GetTransferOption(TransferOptionTypes.OffHours);
            Assert.IsTrue(res.Success, "Fetching off hours transfer option failed");

            //construct the transfer rule object class and manually fetch option with failure cases
            try
            {
                oTransfer = new TransferOption(_connectionServer, _callHandler.ObjectId);
                Assert.Fail("Invalid transfer option type should fail.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure on construction with invalid transfer option:" + ex);
            }

        }

      
    }
}
