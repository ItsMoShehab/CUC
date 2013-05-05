using Cisco.UnityConnection.RestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for LanguageHelperTest and is intended
    ///to contain all LanguageHelperTest Unit Tests
    ///</summary>
    [TestClass]
    public class LanguageHelperTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion


        #region Additional test attributes
       

        #endregion


        /// <summary>
        ///A test for GetLanguageIdFromLanguageEnum
        ///</summary>
        [TestMethod]
        public void GetLanguageIdFromLanguageEnumTest()
        {
            int iRet = LanguageHelper.GetLanguageIdFromLanguageEnum(LanguageCodes.EnglishUnitedStates);

            Assert.AreEqual(iRet, 1033, "US English ID of 1033 not returned for language code");

            iRet = LanguageHelper.GetLanguageIdFromLanguageEnum((LanguageCodes) 10);
            Assert.AreEqual(iRet,-1,"Invalid enum reference should return -1");

        }

        /// <summary>
        ///A test for GetLanguageNameFromLanguageID
        ///</summary>
        [TestMethod]
        public void GetLanguageNameFromLanguageIdTest()
        {
            string strRet = LanguageHelper.GetLanguageNameFromLanguageId(1033);
            Assert.AreEqual(strRet,"EnglishUnitedStates","US English name not returned for 1033");

            strRet = LanguageHelper.GetLanguageNameFromLanguageId(10);
            Assert.AreEqual(strRet, "Undefined","Invalid language ID should return 'Invalid' string for name");

        }
    }
}
