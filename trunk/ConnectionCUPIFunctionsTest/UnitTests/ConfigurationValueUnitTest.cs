using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Test fetching configuration values from the large name/value pair table via the ConfigurationValue class.
    /// </summary>
    [TestClass]
    public class ConfigurationValueUnitTest : BaseUnitTests
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
            ConfigurationValue oTest = new ConfigurationValue(new ConnectionServerRest(new RestTransportFunctions()),"blah");
            Console.WriteLine(oTest);
        }

       
        #endregion


        #region Static Call Failures

        [TestMethod]
        public void GetConfigurationValues_ValidQuery_Success()
        {
            List<ConfigurationValue> oValues;

            var res = ConfigurationValue.GetConfigurationValues(_mockServer, out oValues, 1, 20,
                                                                "query=(FullName startswith System)");
            Assert.IsTrue(res.Success,"Static method to fetch configuration values failed with query construction:" + res);

        }

        [TestMethod]
        public void GetConfigurationValues_EmptyQuery_Success()
        {
            List<ConfigurationValue> oValues;

           var res = ConfigurationValue.GetConfigurationValues(_mockServer, out oValues);
            Assert.IsTrue(res.Success, "Static method to fetch configuration values failed with empty query construction:" + res);
        }

        [TestMethod]
        public void GetConfigurationValues_NullConnectionServer_Failure()
        {
            List<ConfigurationValue> oValues; 
            var res = ConfigurationValue.GetConfigurationValues(null, out oValues, 1, 20, "query=(FullName startswith System)");
            Assert.IsFalse(res.Success, "Static method to fetch configuration values did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void GetConfigurationValue_NullConnectionServer_Failure()
        {
            ConfigurationValue oNewValue;

            var res = ConfigurationValue.GetConfigurationValue(out oNewValue, null, "dummy");
            Assert.IsFalse(res.Success, "Call to GetConfigurationValue static method did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void GetConfigurationValue_EmptyKeyName_Failure()
        {
            ConfigurationValue oNewValue;
            var res = ConfigurationValue.GetConfigurationValue(out oNewValue, _mockServer, "");
            Assert.IsFalse(res.Success, "Call to GetConfigurationValue static method did not fail with empty key name");
        }

        #endregion

    }
}
