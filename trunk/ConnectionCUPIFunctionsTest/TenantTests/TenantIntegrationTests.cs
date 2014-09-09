using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class TenantIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static Tenant _tempTenant;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            //create new handler with GUID in the name to ensure uniqueness
            String strName = "Tenant"+ Guid.NewGuid().ToString().Replace("-", "").Substring(0,8);

            WebCallResult res = Tenant.AddTenant(_connectionServer, strName, strName+".org", strName,"","", out _tempTenant);
            Assert.IsTrue(res.Success, "Failed creating temporary tenant:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempTenant != null)
            {
                WebCallResult res = _tempTenant.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary tenant on cleanup.");
            }
        }

        #endregion


        #region Class Creation Failures

        /// <summary>
        /// UnityConnectionRestException on invalid ObjectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            Tenant oTest = new Tenant(_connectionServer,"ObjectId");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// throw UnityConnectionRestException on invalid alias
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidAlias_Failure()
        {
            Tenant oTest = new Tenant(_connectionServer, "","bogus alias");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Test Failures

        [TestMethod]
        public void DeleteTenant_InvalidObjectId_Failure()
        {
            var res = Tenant.DeleteTenant(_connectionServer, "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteTenant with null ConnectionServer should fail");
        }

        [TestMethod]
        public void GetTenant_InvalidObjectId_Failure()
        {
            Tenant oTenant;
            var res = Tenant.GetTenant(out oTenant, _connectionServer, "objectId");
            Assert.IsFalse(res.Success, "Calling GetTenant with invalid objectID should fail");
        }

        [TestMethod]
        public void GetTenant_InvalidAlias_Failure()
        {
            Tenant oTenant;
            var res = Tenant.GetTenant(out oTenant, _connectionServer, "", "invalid alias");
            Assert.IsFalse(res.Success, "Calling GetTenant with invalid alias did not fail");
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void AddClassOfServiceToTenant_BlankId_Failure()
        {
            var res = _tempTenant.AddClassOfServiceToTenant("");
            Assert.IsFalse(res.Success, "Adding COS to tenant with blank id did not fail");
        }

        [TestMethod]
        public void AddClassOfServiceToTenant_Invalid_Failure()
        {
            var res = _tempTenant.AddClassOfServiceToTenant("invalid id");
            Assert.IsFalse(res.Success, "Adding COS to tenant with invalid id did not fail");
        }

        [TestMethod]
        public void AddClassOfServiceToTenant_Success()
        {
            ClassOfService oCos;
            var res = ClassOfService.AddClassOfService(_connectionServer, Guid.NewGuid().ToString(), null,out oCos);
            Assert.IsTrue(res.Success,"Failed to create new temporary COS");
            Assert.IsNotNull(oCos,"Null COS returned from create");

            res = _tempTenant.AddClassOfServiceToTenant(oCos.ObjectId);
            Assert.IsTrue(res.Success,"Adding COS to tenant failed:"+res);
        }

        [TestMethod]
        public void AddScheduleSetToTenant_BlankId_Failure()
        {
            var res = _tempTenant.AddScheduleSetToTenant("");
            Assert.IsFalse(res.Success, "Adding schedule to tenant with blank id did not fail");

            }

        [TestMethod]
        public void AddScheduleSetToTenant_InvalidId_Failure()
        {
            var res = _tempTenant.AddScheduleSetToTenant("bogus");
            Assert.IsFalse(res.Success, "Adding schedule to tenant with invalid id did not fail");

            }

        [TestMethod]
        public void AddScheduleSetToTenant_Success()
        {
            ScheduleSet oSchedule;
            var res = ScheduleSet.AddQuickSchedule(_connectionServer, Guid.NewGuid().ToString(),
                                               _connectionServer.PrimaryLocationObjectId, "", 0, 1020,
                                               true, true, true, true, true, false, false, DateTime.Now, null,
                                               out oSchedule);
            Assert.IsTrue(res.Success,"Failed to create schedule for test:"+res);

            res = _tempTenant.AddScheduleSetToTenant(oSchedule.ObjectId);
            Assert.IsTrue(res.Success,"Failed to add schedule to tenant");
        }

        [TestMethod]
        public void GetCallHandlerTemplates_Success()
        {
            List<CallHandlerTemplate> oHandlerTemplates;
            WebCallResult res = _tempTenant.GetCallHandlerTemplates(out oHandlerTemplates, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch call handler templates:" + res);
            Assert.IsTrue(oHandlerTemplates.Count == 1, "One call handler template not returned");
        }

        [TestMethod]
        public void GetCallHandlers_Success()
         {
            List<CallHandler> oHandlers;
            var res = _tempTenant.GetCallHandlers(out oHandlers, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch call handlers:" + res);
            Assert.IsTrue(oHandlers.Count > 0, "No call handlers returned");
        }

        [TestMethod]
        public void GetClassesOfService_Success()
        {
            List<ClassOfService> oCoses;
            var res = _tempTenant.GetClassesOfService(out oCoses);
            Assert.IsTrue(res.Success, "Failed to fetch coses:" + res);
            Assert.IsTrue(oCoses.Count > 0, "No coses returned");
        }

        [TestMethod]
        public void GetDirectoryHandlers_Success()
        {
            List<DirectoryHandler> oDirHandlers;
            var res = _tempTenant.GetDirectoryHandlers(out oDirHandlers);
            Assert.IsTrue(res.Success, "Failed to fetch directory handlers:" + res);
            Assert.IsTrue(oDirHandlers.Count > 0, "No directory handlers returned");
        }

        [TestMethod]
        public void GetDistributionLists_Success()
        {
            List<DistributionList> oLists;
            var res = _tempTenant.GetDistributionLists(out oLists);
            Assert.IsTrue(res.Success, "Failed to fetch distribution lists:" + res);
            Assert.IsTrue(oLists.Count > 0, "No distributionlists returned");
        }

        [TestMethod]
        public void GetInterviewHandlers_Success()
        {
            List<InterviewHandler> oInterviewers;
            var res = _tempTenant.GetInterviewHandlers(out oInterviewers);
            Assert.IsTrue(res.Success, "Failed to fetch interview handlers:" + res);
            Assert.IsTrue(oInterviewers.Count > 0, "No interview handlers returned");
        }

        [TestMethod]
        public void GetPhoneSystems_Success()
        {
            List<PhoneSystem> oPhones;
           var res = _tempTenant.GetPhoneSystems(out oPhones);
            Assert.IsTrue(res.Success, "Failed to fetch phone systems:" + res);
            Assert.IsTrue(oPhones.Count > 0, "No phone systems returned");
        }

        [TestMethod]
        public void GetScheduleSets_Success()
        {
            List<ScheduleSet> oSchedules;
            var res = _tempTenant.GetScheduleSets(out oSchedules);
            Assert.IsTrue(res.Success, "Failed to fetch schedule sets:" + res);
            Assert.IsTrue(oSchedules.Count > 0, "No schedule sets returned");
        }

        [TestMethod]
        public void GetUserTemplates_Success()
        {
            List<UserTemplate> oUserTemplates;
            var res = _tempTenant.GetUserTemplates(out oUserTemplates);
            Assert.IsTrue(res.Success, "Failed to fetch user templates:" + res);
            Assert.IsTrue(oUserTemplates.Count > 0, "No user templates returned");
        }

        [TestMethod]
        public void GetUsers_Success()
        {
            List<UserBase> oUsers;
            var res = _tempTenant.GetUsers(out oUsers);
            Assert.IsTrue(res.Success, "Failed to fetch users :" + res);
            Assert.IsTrue(oUsers.Count > 0, "No users returned");
        }

        [TestMethod]
        public void GetTenants_Success()
        {
            List<Tenant> oTenants;
            var res = Tenant.GetTenants(_connectionServer, out oTenants,null);
            Assert.IsTrue(res.Success, "Failed to fetch tenants:" + res);
            Assert.IsTrue(oTenants.Count > 0, "No tenants returned from fetch");

            }

        [TestMethod]
        public void GetTenant_ObjectId_Success()
        {
            Tenant oTenant;
            var res = Tenant.GetTenant(out oTenant, _connectionServer, _tempTenant.ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch tenant from valid objectid:" + res);

            }

        [TestMethod]
        public void GetTenant_Alias_Success()
        {
            Tenant oTenant;
            var res = Tenant.GetTenant(out oTenant, _connectionServer, "", _tempTenant.Alias);
            Assert.IsTrue(res.Success, "Failed to fetch tenant from valid alias:" + res);

            Console.WriteLine(oTenant.ToString());
            oTenant.DumpAllProps();
        }

        [TestMethod]
        public void GetTenants_InvalidQuery_Success()
        {
            List<Tenant> oTenants;
            var res = Tenant.GetTenants(_connectionServer, out oTenants,1,2,"query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching tenants with invalid query should not fail:" + res);
            Assert.IsTrue(oTenants.Count == 0, "Invalid query string should return an empty tenant list:" + oTenants.Count);
        }

        #endregion

    }

}

