using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Test for the credential class - just construction failure tests in here - the User tests contain most of the coverage
    /// for fetching/listing PIN and Password credential details.
    /// </summary>
    [TestClass]
    public class CredentialUnitTests : BaseUnitTests
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
        /// Make sure an exception is thrown if an empty ObjectId is presented
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure_EmptyObjectId()
        {
            Credential oTest = new Credential(_mockServer, "", CredentialType.Pin);
            Console.WriteLine(oTest);
        }

        #endregion

    }
}
