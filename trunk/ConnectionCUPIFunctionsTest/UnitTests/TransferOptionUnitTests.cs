using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
            TransferOption oTest = new TransferOption(null,"objectid",TransferOptionTypes.Alternate);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyObjectId_Failure()
        {
            TransferOption oTest = new TransferOption(_mockServer, "", TransferOptionTypes.Alternate);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_InvalidTranserOptionType_Failure()
        {
            TransferOption oTest = new TransferOption(_mockServer, "objectid", TransferOptionTypes.Invalid);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void UpdateTransferOptionEnabledStatus_NullConnectionServer_Failure()
        {
            WebCallResult res = TransferOption.UpdateTransferOptionEnabledStatus(null, "objectid",TransferOptionTypes.Alternate, true);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest parameter should fail");

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

    }
}
