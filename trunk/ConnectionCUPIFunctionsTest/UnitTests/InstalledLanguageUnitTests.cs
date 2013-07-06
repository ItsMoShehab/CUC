using System;
using Cisco.UnityConnection.RestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class InstalledLanguageUnitTests
    {
        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        #endregion


        [TestMethod]
        public void Constructor_NullConnectionServer_Failure()
        {
            InstalledLanguage oLanguages = new InstalledLanguage(null);
            Assert.IsFalse(oLanguages.IsLanguageInstalled(1033),"Checking language install without loading languages from server should return false");
        }


        [TestMethod]
        public void InstalledLanguage_Constructor_InvalidConnectionServer_Failure()
        {
            try
            {
                InstalledLanguage oLanguages = new InstalledLanguage(new ConnectionServerRest(new RestTransportFunctions()));
                Assert.Fail("Fetching languages for invalid ConnectionServerRest should fail.");
            }
            catch (UnityConnectionRestException ex)
            {
                Console.WriteLine("Expected failure:"+ex);
            }
        }
    }
}
