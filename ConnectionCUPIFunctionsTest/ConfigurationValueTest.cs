using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Test fetching configuration values from the large name/value pair table via the ConfigurationValue class.
    /// </summary>
    [TestClass]
    public class ConfigurationValueTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

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
                throw new Exception("Unable to attach to Connection server to start ConfigurationValue test:" + ex.Message);
            }

        }

        #endregion


        #region Class Construction Errors

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            ConfigurationValue oTest = new ConfigurationValue(null);
            Console.WriteLine(oTest);
        }


        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            ConfigurationValue oTest = new ConfigurationValue(new ConnectionServer(),"blah");
            Console.WriteLine(oTest);
        }

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


            res = ConfigurationValue.GetConfigurationValues(_connectionServer, out oValues);
            Assert.IsTrue(res.Success, "Static method to fetch configuration values failed with empty query construction:" + res);

            res = ConfigurationValue.GetConfigurationValues(null, out oValues, 1, 20, "query=(FullName startswith System)");
            Assert.IsFalse(res.Success, "Static method to fetch configuration values did not fail with null ConnectionServer");
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

            res = ConfigurationValue.GetConfigurationValue(out oNewValue, null, strFullName);
            Assert.IsFalse(res.Success, "Call to GetConfigurationValue static method did not fail with null ConnectionServer");

            res = ConfigurationValue.GetConfigurationValue(out oNewValue, _connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Call to GetConfigurationValue static method did not fail with bogus key name");

            res = ConfigurationValue.GetConfigurationValue(out oNewValue, _connectionServer, "");
            Assert.IsFalse(res.Success, "Call to GetConfigurationValue static method did not fail with empty key name");

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
