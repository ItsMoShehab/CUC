using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RoleIntegrationTests : BaseIntegrationTests
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


        #region Constructor Tests

        
        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown for an invalid ObjectID
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            Role oTest = new Role(_connectionServer,"ObjectId");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown on an invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidDisplayName_Failure()
        {
            Role oTest = new Role(_connectionServer,"","bogus Display Name");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void GetRoles_Success()
        {
            List<Role> oRoles;
            WebCallResult res = Role.GetRoles(_connectionServer, out oRoles);
            Assert.IsTrue(res.Success, "Failed to fetch roles list:" + res);
            Assert.IsTrue(oRoles.Count > 0, "No roles returned from server");

            Console.WriteLine(oRoles[0].ToString());
            Console.WriteLine(oRoles[0].UniqueIdentifier);
            Console.WriteLine(oRoles[0].SelectionDisplayString);

            oRoles.Sort(new UnityDisplayObjectCompare());
        }

        [TestMethod]
        public void RoleConstructor_DisplayName_Success()
        {

            string strObjectId = "";
            try
            {
                Role oNewRole = new Role(_connectionServer, "", "Audio Text Administrator");
                Assert.IsTrue(oNewRole.RoleName.Equals("Audio Text Administrator"));
                strObjectId = oNewRole.ObjectId;
            }
            catch (Exception ex)
            {
                Assert.Fail("Class construction with valid role name failed:"+ex);
            }

            //fetch it again using the objectId and compare
            try
            {
                Role oNewRole = new Role(_connectionServer, strObjectId);
                Assert.IsTrue(oNewRole.ObjectId.Equals(strObjectId));

            }
            catch (Exception ex)
            {
                Assert.Fail("Class construction with valid objectId failed:" + ex);
            }
        }

        #endregion

    }
}
