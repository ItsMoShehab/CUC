using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PolicyIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure UnityConnectionRestException is thrown if a bogus ObjectId is passed into the creator
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            Policy oTest = new Policy(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failures 

        [TestMethod]
        public void GetPoliciesForRole_InvalidObjectId_Success()
        {
            List<Policy> oNewPolicies;

            var res = Policy.GetPoliciesForRole(_connectionServer, "blah", out oNewPolicies);
            Assert.IsTrue(res.Success, "Fetching policies by role with invalid ID should not fail:" + res);
            Assert.IsTrue(oNewPolicies.Count == 0, "Fetching policies with invalid Id should return empty policies list");
        }

        [TestMethod]
        public void GetPoliciesForUser_InvalidObjectId_Success()
        {
            List<Policy> oNewPolicies;

            var res = Policy.GetPoliciesForUser(_connectionServer, "blah", out oNewPolicies);
            Assert.IsTrue(res.Success, "Fetching policies by user with invalid objectID should not fail:" + res);
            Assert.IsTrue(oNewPolicies.Count == 0, "Fetching policies by user with invalid ObjectId should return empty policies list");
        }

        [TestMethod]
        public void GetRoleNamesForUser_InvalidObjectId_Success()
        {
            List<string> oRoles;

            var res = Policy.GetRoleNamesForUser(_connectionServer, "objectid", out oRoles);
            Assert.IsTrue(res.Success, "Fetching role names by user with invalid Id should not fail:" + res);
            Assert.IsTrue(oRoles.Count == 0, "Fetching roles names by user with invalid Id should return empty roles list");
        }

        [TestMethod]
        public void GetPolicies_InvalidQuery_Failure()
        {
            List<Policy> oNewPolicies;

            var res = Policy.GetPolicies(_connectionServer, out oNewPolicies, "query=(bogus)", "", "sort=(bogus)");
            Assert.IsFalse(res.Success, "Fetching policies with invalid query did not fail");
        }

        #endregion


        #region Live Tests

        private Policy HelperFetchPolicyFromServer()
        {
            List<Policy> oPolicies;
            WebCallResult res = Policy.GetPolicies(_connectionServer, out oPolicies, 1, 10, null);
            Assert.IsTrue(res.Success, "Failed to fetch list of policies from server");
            Assert.IsTrue(oPolicies.Count > 0, "No policies returned from fetch");
            return oPolicies[0];
        }

        [TestMethod]
        public void GetPolicies_NullClauses_Success()
        {
            List<Policy> oPolicies;
            WebCallResult res = Policy.GetPolicies(_connectionServer, out oPolicies, 1, 10, null);
            Assert.IsTrue(res.Success, "Failed to fetch list of policies from server");
            Assert.IsTrue(oPolicies.Count > 0, "No policies returned from fetch");
        }

        [TestMethod]
        public void GetPolicies_BlankClauses_Success()
        {
            List<Policy> oPolicies;
            WebCallResult res = Policy.GetPolicies(_connectionServer, out oPolicies, 1, 10, "");
            Assert.IsTrue(res.Success, "Failed to fetch list of policies from server");
            Assert.IsTrue(oPolicies.Count > 0, "No policies returned from fetch");
        }

        [TestMethod]
        public void Policy_ExericesToStringAndDumpAllProps()
        {
            var oPolicy = HelperFetchPolicyFromServer();

            Console.WriteLine(oPolicy.ToString());
            Console.WriteLine(oPolicy.DumpAllProps());
        }

        [TestMethod]
        public void GetPoliciesForRole_Success()
        {
            List<Policy> oNewPolicies;
            var oPolicy = HelperFetchPolicyFromServer();
            var res = Policy.GetPoliciesForRole(_connectionServer, oPolicy.RoleObjectId, out oNewPolicies);
            Assert.IsTrue(res.Success,"Failed to fetch policies by role ObjectId");
            Assert.IsTrue(oNewPolicies.Count>0,"Failed to find policies by role ObjectId");

            }

        [TestMethod]
        public void GetPoliciesForUser_Success()
        {
            List<Policy> oNewPolicies;
            var oPolicy = HelperFetchPolicyFromServer();
            var res = Policy.GetPoliciesForUser(_connectionServer, oPolicy.UserObjectId, out oNewPolicies);
            Assert.IsTrue(res.Success, "Failed to fetch policies by user ObjectId");
            Assert.IsTrue(oNewPolicies.Count > 0, "Failed to find policies by user ObjectId");

            }

        [TestMethod]
        public void GetRoleNamesForUser_Success()
        {
            var oPolicy = HelperFetchPolicyFromServer();
            List<string> oRoles;
            var res = Policy.GetRoleNamesForUser(_connectionServer, oPolicy.UserObjectId, out oRoles);
            Assert.IsTrue(res.Success,"Failed to fetch role names by user ObjectId:"+res);
}

        [TestMethod]
        public void Policy_Constructor_ObjectId_Success()
        {
            var oPolicy = HelperFetchPolicyFromServer();
            try
            {
                new Policy(_connectionServer, oPolicy.ObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new Policy object using valid ObjectId:" + ex);
            }
        }
        
        [TestMethod]
        public void Policy_Constructor_InvalidObjectId_Failure()
        {
            try
            {
                new Policy(_connectionServer, "bogus");
                Assert.Fail("Creating a new policy object with an invalid objectId did not throw an exception");
            }
            catch (Exception ex)
            {
                Console.WriteLine("expected failure creating new policy object:"+ex);
            }
        }

        #endregion
    }
}
