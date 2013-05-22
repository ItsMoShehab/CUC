using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class TenantTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static Tenant _tempTenant;

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
                _connectionServer = new ConnectionServer(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start Tenant test:" + ex.Message);
            }

            //create new handler with GUID in the name to ensure uniqueness
            //String strName = "Tenant_"+ Guid.NewGuid().ToString().Replace("-", "").Substring(0,13);
            string strName = "too long to keep test from running" + Guid.NewGuid().ToString();
            WebCallResult res = Tenant.AddTenant(_connectionServer, strName, strName+".org", strName, out _tempTenant);
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
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure_nullServer()
        {
            Tenant oTest = new Tenant(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid ObjectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure_InvalidObjectId()
        {
            Tenant oTest = new Tenant(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// throw UnityConnectionRestException on invalid alias
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure_InvalidAlias()
        {
            Tenant oTest = new Tenant(_connectionServer, "","bogus");
            Console.WriteLine(oTest);
        }


        #endregion


        #region Static Test Failures

        [TestMethod]
        public void StaticCallFailures_DeleteTenant()
        {
            //DeleteTenant
            var res = Tenant.DeleteTenant(null, "objectid");
            Assert.IsFalse(res.Success, "");

            res = Tenant.DeleteTenant(_connectionServer, "objectid");
            Assert.IsFalse(res.Success, "");

            res = Tenant.DeleteTenant(_connectionServer, "");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void StaticCallFailures_GetTenant()
        {
            //GetTenant
            Tenant oTenant;
            var res = Tenant.GetTenant(out oTenant, null, "objectId");
            Assert.IsFalse(res.Success, "");

            res = Tenant.GetTenant(out oTenant, _connectionServer, "objectId");
            Assert.IsFalse(res.Success, "");

            res = Tenant.GetTenant(out oTenant, _connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "");

            res = Tenant.GetTenant(out oTenant, _connectionServer);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void StaticCallFailures_GetTenants()
        {
            //GetTenants
            List<Tenant> oTenants;
            var res = Tenant.GetTenants(null, out oTenants, 1, 1);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void StaticCallFailures_AddTenant()
        {
            //AddTenant
            WebCallResult res = Tenant.AddTenant(null, "alias", "domain", "description");
            Assert.IsFalse(res.Success,"");

            res = Tenant.AddTenant(_connectionServer, "", "domain", "description");
            Assert.IsFalse(res.Success, "");

            res = Tenant.AddTenant(_connectionServer, "alias", "", "description");
            Assert.IsFalse(res.Success, "");

        }

        #endregion


        [TestMethod]
        public void Tenant_SetTests()
        {
            //COS
            WebCallResult res = _tempTenant.AddClassOfServiceToTenant("");
            Assert.IsFalse(res.Success,"Adding COS to tenant with blank id did not fail");

            res = _tempTenant.AddClassOfServiceToTenant("bogus");
            Assert.IsFalse(res.Success, "Adding COS to tenant with invalid id did not fail");

            ClassOfService oCos;
            res = ClassOfService.AddClassOfService(_connectionServer, Guid.NewGuid().ToString(), null,out oCos);
            Assert.IsTrue(res.Success,"Failed to create new temporary COS");
            Assert.IsNotNull(oCos,"Null COS returned from create");

            res = _tempTenant.AddClassOfServiceToTenant(oCos.ObjectId);
            Assert.IsTrue(res.Success,"Adding COS to tenant failed:"+res);

            //schedule
            res = _tempTenant.AddScheduleSetToTenant("");
            Assert.IsFalse(res.Success, "Adding schedule to tenant with blank id did not fail");

            res = _tempTenant.AddScheduleSetToTenant("bogus");
            Assert.IsFalse(res.Success, "Adding schedule to tenant with invalid id did not fail");

            ScheduleSet oSchedule;
            res = ScheduleSet.AddQuickSchedule(_connectionServer, Guid.NewGuid().ToString(),
                                               _connectionServer.PrimaryLocationObjectId, "", 0, 1020,
                                               true, true, true, true, true, false, false, DateTime.Now, null,
                                               out oSchedule);
            Assert.IsTrue(res.Success,"Failed to create schedule for test:"+res);

            res = _tempTenant.AddScheduleSetToTenant(oSchedule.ObjectId);
            Assert.IsTrue(res.Success,"Failed to add schedule to tenant");
        }

        [TestMethod]
        public void Tenant_FetchTests()
        {
            List<CallHandlerTemplate> oHandlerTemplates;
            WebCallResult res = _tempTenant.GetCallHandlerTemplates(out oHandlerTemplates, 1, 1);
            Assert.IsTrue(res.Success,"Failed to fetch call handler templates:"+res);
            Assert.IsTrue(oHandlerTemplates.Count==1,"One call handler template not returned");

            List<CallHandler> oHandlers;
            res = _tempTenant.GetCallHandlers(out oHandlers, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch call handlers:" + res);
            Assert.IsTrue(oHandlers.Count > 0, "No call handlers returned");

            List<ClassOfService> oCoses;
            res = _tempTenant.GetClassesOfService(out oCoses);
            Assert.IsTrue(res.Success, "Failed to fetch coses:" + res);
            Assert.IsTrue(oCoses.Count > 0, "No coses returned");

            List<DirectoryHandler> oDirHandlers;
            res = _tempTenant.GetDirectoryHandlers(out oDirHandlers);
            Assert.IsTrue(res.Success, "Failed to fetch directory handlers:" + res);
            Assert.IsTrue(oDirHandlers.Count > 0, "No directory handlers returned");

            List<DistributionList> oLists;
            res = _tempTenant.GetDistributionLists(out oLists);
            Assert.IsTrue(res.Success, "Failed to fetch distribution lists:" + res);
            Assert.IsTrue(oLists.Count > 0, "No distributionlists returned");

            List<InterviewHandler> oInterviewers;
            res = _tempTenant.GetInterviewHandlers(out oInterviewers);
            Assert.IsTrue(res.Success, "Failed to fetch interview handlers:" + res);
            Assert.IsTrue(oInterviewers.Count > 0, "No interview handlers returned");

            List<PhoneSystem> oPhones;
            res = _tempTenant.GetPhoneSystems(out oPhones);
            Assert.IsTrue(res.Success, "Failed to fetch phone systems:" + res);
            Assert.IsTrue(oPhones.Count > 0, "No phone systems returned");

            List<ScheduleSet> oSchedules;
            res = _tempTenant.GetScheduleSets(out oSchedules);
            Assert.IsTrue(res.Success, "Failed to fetch schedule sets:" + res);
            Assert.IsTrue(oSchedules.Count > 0, "No schedule sets returned");

            List<UserTemplate> oUserTemplates;
            res = _tempTenant.GetUserTemplates(out oUserTemplates);
            Assert.IsTrue(res.Success, "Failed to fetch user templates:" + res);
            Assert.IsTrue(oUserTemplates.Count > 0, "No user templates returned");

            List<UserBase> oUsers;
            res = _tempTenant.GetUsers(out oUsers);
            Assert.IsTrue(res.Success, "Failed to fetch users :" + res);
            Assert.IsTrue(oUsers.Count > 0, "No users returned");


            List<Tenant> oTenants;
            res = Tenant.GetTenants(_connectionServer, out oTenants);
            Assert.IsTrue(res.Success,"Failed to fetch tenants:"+res);
            Assert.IsTrue(oTenants.Count>0,"No tenants returned from fetch");

            Tenant oTenant;
            res = Tenant.GetTenant(out oTenant, _connectionServer, oTenants[0].ObjectId);
            Assert.IsTrue(res.Success,"Failed to fetch tenant from valid objectid:"+res);

            res = Tenant.GetTenant(out oTenant, _connectionServer,"", oTenants[0].Alias);
            Assert.IsTrue(res.Success, "Failed to fetch tenant from valid alias:" + res);


            Console.WriteLine(oTenant.ToString());
            oTenant.DumpAllProps();
        }
    }
}
