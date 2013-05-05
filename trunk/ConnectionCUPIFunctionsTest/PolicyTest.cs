﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PolicyTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start Policy test:" + ex.Message);
            }
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            Policy oTest = new Policy(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure UnityConnectionRestException is thrown if a bogus ObjectId is passed into the creator
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            Policy oTest = new Policy(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        [TestMethod]
        public void Policy_StaticFailures()
        {
            //Static methods
            List<Policy> oNewPolicies;

            WebCallResult res = Policy.GetPolicies(null, out oNewPolicies);
            Assert.IsFalse(res.Success, "Fetching policies with null connection server did not fail");

            res = Policy.GetPolicies(_connectionServer, out oNewPolicies, "query=(bogus)", "", "sort=(bogus)");
            Assert.IsFalse(res.Success, "Fetching policies with invalid query did not fail");


            res = Policy.GetPoliciesForRole(null, "blah", out oNewPolicies);
            Assert.IsFalse(res.Success, "Fetching policies by role with null Connection server did not fail");

            res = Policy.GetPoliciesForRole(_connectionServer, "blah", out oNewPolicies);
            Assert.IsTrue(res.Success, "Fetching policies by role with invalid ID should not fail:"+res);
            Assert.IsTrue(oNewPolicies.Count==0,"Fetching policies with invalid Id should return empty policies list");

            res = Policy.GetPoliciesForUser(null, "blah", out oNewPolicies);
            Assert.IsFalse(res.Success, "Fetching policies by user with null Connection server did not fail");

            res = Policy.GetPoliciesForUser(_connectionServer, "blah", out oNewPolicies);
            Assert.IsTrue(res.Success, "Fetching policies by user with invalid objectID should not fail:"+res);
            Assert.IsTrue(oNewPolicies.Count==0,"Fetching policies by user with invalid ObjectId should return empty policies list");

            List<string> oRoles; 
            res = Policy.GetRoleNamesForUser(null, "objectid", out oRoles);
            Assert.IsFalse(res.Success, "Fetching role names by user with null Connection server did not fail");

            res = Policy.GetRoleNamesForUser(_connectionServer, "", out oRoles);
            Assert.IsFalse(res.Success, "Fetching role names by user with empty Id did not fail");

            res = Policy.GetRoleNamesForUser(_connectionServer, "objectid", out oRoles);
            Assert.IsTrue(res.Success,"Fetching role names by user with invalid Id should not fail:"+res);
            Assert.IsTrue(oRoles.Count==0,"Fetching roles names by user with invalid Id should return empty roles list");
        }

        [TestMethod]
        public void Policy_FetchTest()
        {
            //fetch all policies and iterate them
            List<Policy> oPolicies;
            WebCallResult res = Policy.GetPolicies(_connectionServer, out oPolicies,1,10);
            Assert.IsTrue(res.Success,"Failed to fetch list of policies from server");

            Assert.IsTrue(oPolicies.Count>0,"No policies returned from fetch");

            foreach (var oPolicy in oPolicies)
            {
                Console.WriteLine(oPolicy.ToString());
                Console.WriteLine(oPolicy.DumpAllProps());
            }

            List<Policy> oNewPolicies;

            res = Policy.GetPoliciesForRole(_connectionServer, oPolicies[0].RoleObjectId, out oNewPolicies);
            Assert.IsTrue(res.Success,"Failed to fetch policies by role ObjectId");
            Assert.IsTrue(oNewPolicies.Count>0,"Failed to find policies by role ObjectId");

            res = Policy.GetPoliciesForUser(_connectionServer, oPolicies[0].UserObjectId, out oNewPolicies);
            Assert.IsTrue(res.Success, "Failed to fetch policies by user ObjectId");
            Assert.IsTrue(oNewPolicies.Count > 0, "Failed to find policies by user ObjectId");

            List<string> oRoles;
            res = Policy.GetRoleNamesForUser(_connectionServer, oPolicies[0].UserObjectId, out oRoles);
            Assert.IsTrue(res.Success,"Failed to fetch role names by user ObjectId:"+res);
            

            try
            {
                new Policy(_connectionServer, oPolicies[0].ObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new Policy object using valid ObjectId:"+ex);
            }

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
    }
}
