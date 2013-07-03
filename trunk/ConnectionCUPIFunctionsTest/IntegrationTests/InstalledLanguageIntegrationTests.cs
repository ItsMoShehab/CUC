using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class InstalledLanguageIntegrationTests : BaseIntegrationTests
    {
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


        [TestMethod]
        public void FetchTests()
        {
            //List<InstalledLanguage> oLanguages;
            InstalledLanguage oLanguages=null;
            try
            {
                 oLanguages= new InstalledLanguage(_connectionServer);
            }
            catch (Exception ex)
            {
                Assert.Fail("Could not create installed language class:"+ex);
            }

            foreach (var oLanguage in oLanguages.InstalledLanguages)
            {
                Console.WriteLine(oLanguage.ToString());
            }

            Assert.IsTrue(oLanguages.IsLanguageInstalled(1033, ConnectionLanguageTypes.GUI), "1033 not installed for GUI");

            Assert.IsTrue(oLanguages.IsLanguageInstalled(1033, ConnectionLanguageTypes.TUI), "1033 not installed for TUI");

            Assert.IsTrue(oLanguages.IsLanguageInstalled(1033, ConnectionLanguageTypes.TTS), "1033 not installed for TTS");

            Assert.IsFalse(oLanguages.IsLanguageInstalled(8888, ConnectionLanguageTypes.TTS), "8888 listed as installed for TTS");
        }
    }
}
