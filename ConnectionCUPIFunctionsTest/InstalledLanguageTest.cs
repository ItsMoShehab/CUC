using System;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class InstalledLanguageTest
    {
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
                _connectionServer = new ConnectionServer(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin, 
                    mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start InstalledLanguage test:" + ex.Message);
            }

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

        [TestMethod]
        public void ConstructorTest()
        {
            InstalledLanguage oLanguages = new InstalledLanguage(null);

            Assert.IsFalse(oLanguages.IsLanguageInstalled(1033),"Checking language install without loading languages from server should return false");
        }

        [TestMethod]
        public void FailedLanguagesFetchTEst()
        {
            try
            {
                InstalledLanguage oLanguages = new InstalledLanguage(new ConnectionServer(new RestTransportFunctions()));
                Assert.Fail("Fetching languages for invalid ConnectionServer should fail.");
            }
            catch (UnityConnectionRestException ex)
            {
                Console.WriteLine("Expected failure:"+ex);
            }
        }
    }
}
