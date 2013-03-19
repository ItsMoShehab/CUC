using System;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Test for the credential class - just construction failure tests in here - the User tests contain most of the coverage
    /// for fetching/listing PIN and Password credential details.
    /// </summary>
    [TestClass]
    public class CredentialTest
    {

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
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
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start Credential test:" + ex.Message);
            }

        }

        #endregion

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure_nullServer()
        {
            Credential oTest = new Credential(null,"blah",CredentialType.Pin);
        }

        /// <summary>
        /// Make sure an exception is thrown if an invalid ObjectId is presented
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure_InvalidObjectId()
        {
            Credential oTest = new Credential(_connectionServer, "blah", CredentialType.Pin);
        }

        /// <summary>
        /// Make sure an exception is thrown if an empty ObjectId is presented
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure_EmptyObjectId()
        {
            Credential oTest = new Credential(_connectionServer, "", CredentialType.Pin);
        }



    }
}
