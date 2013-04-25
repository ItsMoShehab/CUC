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
        private static ConnectionServer _connectionServer;

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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
                HTTPFunctions.DebugMode = mySettings.DebugOn;
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

       
        /// <summary>
        /// exercise transfer options
        /// </summary>
        [TestMethod]
        public void TransferOption_Test()
        {
            TransferOption oTransfer;

            //first, test getting a bogus transfer option
            WebCallResult res = _callHandler.GetTransferOption("bogus", out oTransfer);
            Assert.IsFalse(res.Success, "GetTransferOption should fail with invalid transfer option name");

            //now get the off hours transfer rule and enable it for a month.
            res = _callHandler.GetTransferOption("Off Hours", out oTransfer);
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

            //construct the transfer rule object class and manually fetch option with failure cases
            oTransfer = new TransferOption(_connectionServer, _callHandler.ObjectId);
            res = oTransfer.GetTransferOption("");
            Assert.IsFalse(res.Success, "Empty transfer option type should fail");

            res = oTransfer.GetTransferOption("Bogus");
            Assert.IsFalse(res.Success, "Invalid transfer option type should fail.");

            //now fetch it properly
            res = oTransfer.GetTransferOption("Alternate");
            Assert.IsTrue(res.Success, "Fetching alternate transfer option failed");

        }


        /// <summary>
        /// exercise transfer options failure cases 
        /// </summary>
        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_Failure()
        {
            //hit some invalid calls for updating the enabled status for transfer options
            WebCallResult res = TransferOption.UpdateTransferOptionEnabledStatus(null, _callHandler.ObjectId, "Alternate", true);
            Assert.IsFalse(res.Success, "Null ConnectionServer parameter should fail");

            res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, "aaa", "Alternate", true);
            Assert.IsFalse(res.Success, "Invalid ObjectId for call handler should fail");

            res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, "Standard", false);
            Assert.IsFalse(res.Success, "Disabling the standard transfer option should fail");

            res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, "Alternate", false, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Disabing a transfer option with a date in the past should fail");

            res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, "aaa", true);
            Assert.IsFalse(res.Success, "Invalid TransferOption type name should fail");

            res = TransferOption.UpdateTransferOptionEnabledStatus(_connectionServer, _callHandler.ObjectId, "Alternate", true, DateTime.Now.AddDays(-1));
            Assert.IsFalse(res.Success, "Enabling rule with date in the past should fail");


        }

        /// <summary>
        /// exercise transfer options failure cases 
        /// </summary>
        [TestMethod]
        public void UpdateTransferOption_Failure()
        {
            //check manually editing properties on transfer options failure cases
            WebCallResult res = TransferOption.UpdateTransferOption(null, _callHandler.ObjectId, "Alternate", null);
            Assert.IsFalse(res.Success, "Updating transfer options with null ConnectionServer param should fail");

            res = TransferOption.UpdateTransferOption(_connectionServer, _callHandler.ObjectId, "Alternate", null);
            Assert.IsFalse(res.Success, "Calling update for transfer options with no parameters should fail");

        }

        /// <summary>
        /// exercise transfer options failure cases 
        /// </summary>
        [TestMethod]
        public void GetTransferOptions_Failure()
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
        public void GetTransferOption_Failure()
        {
            TransferOption oTransfer;

            WebCallResult res = TransferOption.GetTransferOption(_connectionServer, _callHandler.ObjectId, "", out oTransfer);
            Assert.IsFalse(res.Success, "Empty transfer option type should fail");

            res = TransferOption.GetTransferOption(_connectionServer, _callHandler.ObjectId, "Bogus", out oTransfer);
            Assert.IsFalse(res.Success, "Invalid transfer option type should fail");

            res = TransferOption.GetTransferOption(null, "", "", out oTransfer);
            Assert.IsFalse(res.Success, "Null ConnectionServer parameter should fail");

            res = TransferOption.GetTransferOption(_connectionServer, "", "", out oTransfer);
            Assert.IsFalse(res.Success, "Empty ObjectId should should fail");

            //make sure invalid Connection server param is caught
            res = TransferOption.GetTransferOption(null, _callHandler.ObjectId, "Alterante", out oTransfer);
            Assert.IsFalse(res.Success, "Null ConnectionServer parameter should fail");

        }

    }
}
