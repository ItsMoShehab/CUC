using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class GlobalUserTest
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
                throw new Exception("Unable to attach to Connection server to start GlobalUser test:" + ex.Message);
            }

        }

        #endregion


        #region Construction tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            GlobalUser oTest = new GlobalUser(null);
            Console.WriteLine(oTest);
        }

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


        [TestMethod]
        public void GlobalUserFetchTests()
        {
            List<GlobalUser> oUsers;
            WebCallResult res = GlobalUser.GetUsers(_connectionServer, out oUsers);
            Assert.IsTrue(res.Success,"Failed to fetch global users");
            Assert.IsTrue(oUsers.Count>0,"No global users returned on fetch");

            foreach (var oUser in oUsers)
            {
                Console.WriteLine(oUser.ToString());
                Console.WriteLine(oUser.DumpAllProps());
            }

            //static method calls
            GlobalUser oNewUser;
            List<GlobalUser> oNewUsers;
            
            //GetUser
            res = GlobalUser.GetUser(out oNewUser, _connectionServer, "");
            Assert.IsFalse(res.Success,"Fetching user with static method with empty objectId did not fail");

            res = GlobalUser.GetUser(out oNewUser, null, "bogus");
            Assert.IsFalse(res.Success, "Fetching user with static method with null Connection server did not fail");

            res = GlobalUser.GetUser(out oNewUser, _connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Fetching user with static method with invalid objectId did not fail");

            res = GlobalUser.GetUser(out oNewUser, _connectionServer, oUsers[0].ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch a user by valid ObjectId:"+res);

            res = GlobalUser.GetUser(out oNewUser, _connectionServer, "", oUsers[0].Alias);
            Assert.IsTrue(res.Success, "Failed to fetch a user by valid Alias:" + res);

            //GetUsers
            res = GlobalUser.GetUsers(null, out oNewUsers);
            Assert.IsFalse(res.Success,"Fetching users via static method with null ConnectionServer did not fail");

            res = GlobalUser.GetUsers(_connectionServer, out oNewUsers, "query=(bogus)", "", "sort=(bogus)");
            Assert.IsFalse(res.Success, "Fetching users via static method with invalid query construction did not fail");

            string strQuery = string.Format("query=(Alias is {0})", oUsers[0].Alias);
            res = GlobalUser.GetUsers(_connectionServer, out oNewUsers, strQuery);
            Assert.IsTrue(res.Success, "Fetching users via static method with valid alias query construction failed");
            Assert.IsTrue(oNewUsers.Count==1,"Fetching users by alias construction did not return single match");

        }

    }
}
