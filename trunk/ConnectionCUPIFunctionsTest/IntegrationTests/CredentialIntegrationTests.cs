using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Test for the credential class - just construction failure tests in here - the User tests contain most of the coverage
    /// for fetching/listing PIN and Password credential details.
    /// </summary>
    [TestClass]
    public class CredentialIntegrationTests : BaseIntegrationTests
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


        #region Class Construction Failures 

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ObjectId is presented
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            Credential oTest = new Credential(_connectionServer, "blah", CredentialType.Pin);
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
