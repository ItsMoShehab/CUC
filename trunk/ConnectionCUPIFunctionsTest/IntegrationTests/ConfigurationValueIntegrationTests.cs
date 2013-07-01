using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Test fetching configuration values from the large name/value pair table via the ConfigurationValue class.
    /// </summary>
    [TestClass]
    public class ConfigurationValueIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
        }

        #endregion


        #region Class Construction Errors

       /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid configuration item name is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            ConfigurationValue oTest = new ConfigurationValue(_connectionServer, "blah");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticCallFailure_GetConfigurationValues()
        {
            List<ConfigurationValue> oValues;

            var res = ConfigurationValue.GetConfigurationValues(_connectionServer, out oValues, 1, 20, "query=(FullName startswith System)");
            Assert.IsTrue(res.Success, "Static method to fetch configuration values failed with query construction:" + res);

            res = ConfigurationValue.GetConfigurationValues(_connectionServer, out oValues, 1, 20, "", "query=(bogus)");
            Assert.IsFalse(res.Success, "Static method to fetch configuration values did not fail with invalid query construction");

        }

        [TestMethod]
        public void StaticCallFailure_GetConfigurationValue()
        {
            ConfigurationValue oNewValue;

            List<ConfigurationValue> oValues;
            WebCallResult res = ConfigurationValue.GetConfigurationValues(_connectionServer, out oValues);
            Assert.IsTrue(res.Success, "Failed to fetch list of all configuraiton values");
            Assert.IsTrue(oValues.Count > 0, "No configuration values returned from system fetch");

            string strFullName = oValues[0].FullName;

            res = ConfigurationValue.GetConfigurationValue(out oNewValue, _connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Call to GetConfigurationValue static method did not fail with bogus key name");

            res = ConfigurationValue.GetConfigurationValue(out oNewValue, _connectionServer, strFullName);
            Assert.IsTrue(res.Success, "Static method to fetch configuration value failed for valid key name:" + res);
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void FetchTests()
        {

            List<ConfigurationValue> oValues;
            WebCallResult res = ConfigurationValue.GetConfigurationValues(_connectionServer, out oValues);
            Assert.IsTrue(res.Success,"Failed to fetch list of all configuraiton values");

            Assert.IsTrue(oValues.Count>0,"No configuration values returned from system fetch");

            string strFullName= oValues[0].FullName;

            try
            {
                ConfigurationValue oValue = new ConfigurationValue(_connectionServer, strFullName);
                Console.WriteLine(oValue.ToString());
                Console.WriteLine(oValue.DumpAllProps());
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false,"Failed to construct Configuraiton value from full name:"+ex);
            }
        }

        #endregion
    }
}
