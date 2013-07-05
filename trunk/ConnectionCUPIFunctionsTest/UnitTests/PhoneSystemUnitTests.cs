using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for PhoneSystemUnitTests and is intended
    ///to contain all PhoneSystemUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class PhoneSystemUnitTests : BaseUnitTests
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


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreation_Failure()
        {
            PhoneSystem oTest = new PhoneSystem(null, "aaa");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failures 

        [TestMethod]
        public void AddPhoneSystem_NullConnectionServer_Failure()
        {
            WebCallResult res = PhoneSystem.AddPhoneSystem(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to AddPhoneSystem with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void AddPhoneSystem_EmptyName_Failure()
        {
            var res = PhoneSystem.AddPhoneSystem(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to AddPhoneSystem with empty name did not fail");
        }

        [TestMethod]
        public void DeletePhoneSystem_NullConnectionServer_Failure()
        {
            var res = PhoneSystem.DeletePhoneSystem(null, "objectid");
            Assert.IsFalse(res.Success, "Static call to DeletePhoneSystem with null ConnectionServerRest did not fail");

        }

        [TestMethod]
        public void DeletePhoneSystem_BlankObjectId_Failure()
        {
            var res = PhoneSystem.DeletePhoneSystem(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to DeletePhoneSystem with blank ObjectId did not fail");
        }


        [TestMethod]
        public void UpdatePhoneSystem_NullConnectionServer_Failure()
        {
            var res = PhoneSystem.UpdatePhoneSystem(null, "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePhoneSystem with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void UpdatePhoneSystem_BlankObjectId_Failure()
        {
            var res = PhoneSystem.UpdatePhoneSystem(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePhoneSystem with blank ObjectId did not fail");
        }

        [TestMethod]
        public void GetPhoneSystemAssociations_NullConnectionServer_Failure()
        {
            List<PhoneSystemAssociation> oList;
            var res = PhoneSystem.GetPhoneSystemAssociations(null, "objectid", out oList);
            Assert.IsFalse(res.Success,
                           "Static call to GetPhoneSystemAssociations with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void GetPhoneSystemAssociations_BlankObjectId_Failure()
        {
            List<PhoneSystemAssociation> oList;
            var res = PhoneSystem.GetPhoneSystemAssociations(_mockServer, "", out oList);
            Assert.IsFalse(res.Success, "Fetching phone system associations with blank objectid should fail");
        }


        [TestMethod]
        public void GetPhoneSystem_NullConnectionServer_Failure()
        {
            PhoneSystem oPhoneSystem;
            var res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, null, "objectid");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void GetPhoneSystem_BlankObjectIdAndName_Failure()
        {
            PhoneSystem oPhoneSystem;
            var res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, _mockServer, "");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with invalid empty objectId and name did not fail");
        }

        #endregion

    }
}

