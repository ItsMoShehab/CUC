using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class UserLdapIntegrationTests : BaseIntegrationTests
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


        #region Static Calls

        [TestMethod]
        public void GetLdapUsers_InvalidPageNumber_Failure()
        {
            List<UserLdap> oUsers;
            var res = UserLdap.GetLdapUsers(_connectionServer, out oUsers, 99999, 100, null);
            Assert.IsFalse(res.Success, "GetLdapUsers with invalid page number should fail");
        }

        [TestMethod]
        public void GetLdapUsers_InvalidQuery()
        {
            List<UserLdap> oUsers;

            var res = UserLdap.GetLdapUsers(_connectionServer, out oUsers, 0, 20, "query=(alias is _bogus_)");

            if (res.ResponseText.Contains("LDAP_DIRECTORY_SYNCH_NOT_ACTIVATED"))
            {
                Console.WriteLine("LDAP not enabled on box -skipping invalid query test");
                return;
            }

            Assert.IsTrue(res.Success, "GetLdapUsers with invalid query server should not fail:"+res);
            Assert.IsNotNull(oUsers,"Bogus query should not return a null list");
            Assert.IsTrue(oUsers.Count==0,"Bogus query should return empty list");
        }


        [TestMethod]
        public void ImportLdapUser_InvalidPkid_Failure()
        {
            UserFull oUser;
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test","test");

            var res = UserLdap.ImportLdapUser(_connectionServer, "templatealias", "pkid", "alias", "firstname", "lastname", "extension", oProps, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with invalid pkid should fail");
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void GetLdapUsers_Success()
        {
            List<UserLdap> oUsers;
            var res = UserLdap.GetLdapUsers(_connectionServer, out oUsers, 0, 5);
            
            if (res.ResponseText.Contains("LDAP_DIRECTORY_SYNCH_NOT_ACTIVATED"))
            {
                Console.WriteLine("LDAP not enabled on box -skipping live tests of LDAP import");
                return;
            }
            
            Assert.IsTrue(res.Success,"Failed fetching LDAP users:"+res);
            Assert.IsNotNull(oUsers,"Fetch of LDAP users returned null list");
            
            foreach (var oUser in oUsers)
            {
                Console.WriteLine(oUser.ToString());
                Console.WriteLine(oUser.SelectionDisplayString);
                Console.WriteLine(oUser.UniqueIdentifier);
            }
        }

        [TestMethod]
        public void EmptyInstance_Import_OutParam_Failure()
        {
            UserLdap oUserLdap = new UserLdap();
            UserFull oUser;
            var res = oUserLdap.Import("1234234", "alias",out oUser);
            Assert.IsFalse(res.Success,"Importing an empty instance should fail");

            }

        [TestMethod]
        public void EmptyInstance_Import_Failure()
        {
            UserLdap oUserLdap = new UserLdap();
            var res = oUserLdap.Import("1234234", "alias");
            Assert.IsFalse(res.Success, "Importing an empty instance should fail");
        }


        #endregion

    }
}
