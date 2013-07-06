using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PolicyUnitTests : BaseUnitTests
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


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            Policy oTest = new Policy(null);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failures 

        [TestMethod]
        public void GetPoliciesForRole_NullConnectionServer_Failure()
        {
            List<Policy> oNewPolicies;
           var res = Policy.GetPoliciesForRole(null, "blah", out oNewPolicies);
            Assert.IsFalse(res.Success, "Fetching policies by role with null Connection server did not fail");
        }

        [TestMethod]
        public void GetPoliciesForRole_BlankObjectId_Failure()
        {
            List<Policy> oNewPolicies;
            var res = Policy.GetPoliciesForRole(_mockServer, "", out oNewPolicies);
            Assert.IsFalse(res.Success, "Fetching policies by role with blank objectId did not fail");
        }

        [TestMethod]
        public void GetPoliciesForUser_NullConnectionServer_Failure()
        {
            List<Policy> oNewPolicies;

            var res = Policy.GetPoliciesForUser(null, "blah", out oNewPolicies);
            Assert.IsFalse(res.Success, "Fetching policies by user with null Connection server did not fail");
        }

        [TestMethod]
        public void GetPoliciesForUser_BlankObjectId_Failure()
        {
            List<Policy> oNewPolicies;

            var res = Policy.GetPoliciesForUser(_mockServer, "", out oNewPolicies);
            Assert.IsFalse(res.Success, "Fetching policies by user with blank objectId did not fail");
        }


        [TestMethod]
        public void GetRoleNamesForUser_NullConnectionServer_Failure()
        {
            List<string> oRoles;
            var res = Policy.GetRoleNamesForUser(null, "objectid", out oRoles);
            Assert.IsFalse(res.Success, "Fetching role names by user with null Connection server did not fail");
        }

        [TestMethod]
        public void GetRoleNamesForUser_BlankObjectId_Failure()
        {
            List<string> oRoles;
            var res = Policy.GetRoleNamesForUser(_mockServer, "", out oRoles);
            Assert.IsFalse(res.Success, "Fetching role names by user with blank ObjectId did not fail");
        }

        [TestMethod]
        public void GetPolicies_NullConnectionServer_Failure()
        {
            List<Policy> oNewPolicies;

            WebCallResult res = Policy.GetPolicies(null, out oNewPolicies);
            Assert.IsFalse(res.Success, "Fetching policies with null connection server did not fail");
        }


        #endregion

    }
}
