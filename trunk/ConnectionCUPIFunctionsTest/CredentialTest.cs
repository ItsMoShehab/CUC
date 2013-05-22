using System;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
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
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

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
                throw new Exception("Unable to attach to Connection server to start Credential test:" + ex.Message);
            }

        }

        #endregion


        #region Class Construction Failures 

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure_NullServer()
        {
            Credential oTest = new Credential(null,"blah",CredentialType.Pin);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ObjectId is presented
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure_InvalidObjectId()
        {
            Credential oTest = new Credential(_connectionServer, "blah", CredentialType.Pin);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an exception is thrown if an empty ObjectId is presented
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure_EmptyObjectId()
        {
            Credential oTest = new Credential(_connectionServer, "", CredentialType.Pin);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void StaticCallFailures_GetCredential()
        {
            Credential oCredential;
            var res= Credential.GetCredential(null, "userid", CredentialType.Password, out oCredential);
            Assert.IsFalse(res.Success,"Calling GetCredential with null Connection server did not fail");
            
            res = Credential.GetCredential(_connectionServer, "userid", CredentialType.Password, out oCredential);
            Assert.IsFalse(res.Success, "Calling GetCredential with invalid user id did not fail");

            res = Credential.GetCredential(_connectionServer, "", CredentialType.Password, out oCredential);
            Assert.IsFalse(res.Success, "Calling GetCredential with empty user id did not fail");
        }

        #endregion
    }
}
