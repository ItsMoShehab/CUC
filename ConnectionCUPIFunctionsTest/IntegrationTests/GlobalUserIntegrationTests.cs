using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class GlobalUserIntegrationTests : BaseIntegrationTests
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


        #region Construction tests

        /// <summary>
        /// Make sure a UnityConnectionRestException is thrown for an invalid objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            GlobalUser oTest = new GlobalUser(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Throw a UnityConnectionRestException if the alias is invalid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            GlobalUser oTest = new GlobalUser(_connectionServer,"","bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Live Tests 

        [TestMethod]
        public void GlobalUser_GetUser()
        {
            GlobalUser oNewUser;
            List<GlobalUser> oUsers;

            WebCallResult res = GlobalUser.GetUsers(_connectionServer, out oUsers,1,10);
            
            Assert.IsTrue(res.Success, "Failed to fetch global users");
            Assert.IsTrue(oUsers.Count > 0, "No global users returned on fetch");

            //GetUser
            res = GlobalUser.GetUser(out oNewUser, _connectionServer, "");
            Assert.IsFalse(res.Success, "Fetching user with static method with empty objectId did not fail");

            res = GlobalUser.GetUser(out oNewUser, null, "bogus");
            Assert.IsFalse(res.Success, "Fetching user with static method with null Connection server did not fail");

            res = GlobalUser.GetUser(out oNewUser, _connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Fetching user with static method with invalid objectId did not fail");

            res = GlobalUser.GetUser(out oNewUser, _connectionServer, oUsers[0].ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch a user by valid ObjectId:" + res);

            res = GlobalUser.GetUser(out oNewUser, _connectionServer, "", oUsers[0].Alias);
            Assert.IsTrue(res.Success, "Failed to fetch a user by valid Alias:" + res);
        }

        [TestMethod]
        public void GlobalUser_GetUsers()
        {
            //static method calls
            List<GlobalUser> oNewUsers;

            List<GlobalUser> oUsers;
            WebCallResult res = GlobalUser.GetUsers(_connectionServer, out oUsers,1,10);
            Assert.IsTrue(res.Success, "Failed to fetch global users");
            Assert.IsTrue(oUsers.Count > 0, "No global users returned on fetch");

            //exercise dump calls
            Console.WriteLine(oUsers[0].ToString());
            Console.WriteLine(oUsers[0].DumpAllProps());

            //GetUsers
            res = GlobalUser.GetUsers(null, out oNewUsers);
            Assert.IsFalse(res.Success, "Fetching users via static method with null ConnectionServerRest did not fail");

            res = GlobalUser.GetUsers(_connectionServer, out oNewUsers, "query=(bogus)", "", "sort=(bogus)");
            Assert.IsFalse(res.Success, "Fetching users via static method with invalid query construction did not fail");

            string strQuery = string.Format("query=(Alias is {0})", oUsers[0].Alias);
            res = GlobalUser.GetUsers(_connectionServer, out oNewUsers, strQuery);
            Assert.IsTrue(res.Success, "Fetching users via static method with valid alias query construction failed");
            Assert.IsTrue(oNewUsers.Count == 1, "Fetching users by alias construction did not return single match");

            res = GlobalUser.GetUsers(_connectionServer, out oUsers, 1, 20, "query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching users with invalid query should not fail:" + res);
            Assert.IsTrue(oUsers.Count == 0, "Invalid query string should return an empty user list:" + oUsers.Count);

        }

        #endregion
    }
}
