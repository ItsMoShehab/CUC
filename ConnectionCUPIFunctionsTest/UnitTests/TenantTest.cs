using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

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
        private static ConnectionServerRest _connectionServer;

        //Mock transport interface - 
        private static Mock<IConnectionRestCalls> _mockTransport;

        //Mock REST server
        private static ConnectionServerRest _mockServer;

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
                _connectionServer = new ConnectionServerRest(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start Tenant test:" + ex.Message);
            }

            //create new handler with GUID in the name to ensure uniqueness
            String strName = "Tenant_"+ Guid.NewGuid().ToString().Replace("-", "").Substring(0,13);
            //string strName = "too long to keep test from running" + Guid.NewGuid().ToString();
            WebCallResult res = Tenant.AddTenant(_connectionServer, strName, strName+".org", strName, out _tempTenant);
            Assert.IsTrue(res.Success, "Failed creating temporary tenant:" + res.ToString());

           //setup mock server interface 
           _mockTransport = new Mock<IConnectionRestCalls>();

           _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
               It.IsAny<string>(), true)).Returns(new WebCallResult
               {
                   Success = true,
                   ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
               });

           _mockServer = new ConnectionServerRest(_mockTransport.Object, "test", "test", "test", false);
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


        [TestMethod]
        public void ClassCreationNoObjectId()
        {
            //should not throw an exception
            Tenant oTest=new Tenant(_connectionServer);
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
            var res = Tenant.GetTenants(null, out oTenants, 1, 1,null);
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


        #region Live Tests

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
        public void Tenant_FetchTemplates()
        {
            List<CallHandlerTemplate> oHandlerTemplates;
            WebCallResult res = _tempTenant.GetCallHandlerTemplates(out oHandlerTemplates, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch call handler templates:" + res);
            Assert.IsTrue(oHandlerTemplates.Count == 1, "One call handler template not returned");
        }

        [TestMethod]
         public void Tenant_FetchHandlers()
         {
            List<CallHandler> oHandlers;
            var res = _tempTenant.GetCallHandlers(out oHandlers, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch call handlers:" + res);
            Assert.IsTrue(oHandlers.Count > 0, "No call handlers returned");
        }

        [TestMethod]
        public void Tenant_FetchCoses()
        {
            List<ClassOfService> oCoses;
            var res = _tempTenant.GetClassesOfService(out oCoses);
            Assert.IsTrue(res.Success, "Failed to fetch coses:" + res);
            Assert.IsTrue(oCoses.Count > 0, "No coses returned");
        }

        [TestMethod]
        public void Tenant_FetchDirHandlers()
        {
            List<DirectoryHandler> oDirHandlers;
            var res = _tempTenant.GetDirectoryHandlers(out oDirHandlers);
            Assert.IsTrue(res.Success, "Failed to fetch directory handlers:" + res);
            Assert.IsTrue(oDirHandlers.Count > 0, "No directory handlers returned");
        }

        [TestMethod]
        public void Tenant_FetchLists()
        {
            List<DistributionList> oLists;
            var res = _tempTenant.GetDistributionLists(out oLists);
            Assert.IsTrue(res.Success, "Failed to fetch distribution lists:" + res);
            Assert.IsTrue(oLists.Count > 0, "No distributionlists returned");
        }

        [TestMethod]
        public void Tenant_FetchInterviewers()
        {
            List<InterviewHandler> oInterviewers;
            var res = _tempTenant.GetInterviewHandlers(out oInterviewers);
            Assert.IsTrue(res.Success, "Failed to fetch interview handlers:" + res);
            Assert.IsTrue(oInterviewers.Count > 0, "No interview handlers returned");
        }

        [TestMethod]
        public void Tenant_FetchPhoneSystems()
        {
            List<PhoneSystem> oPhones;
           var res = _tempTenant.GetPhoneSystems(out oPhones);
            Assert.IsTrue(res.Success, "Failed to fetch phone systems:" + res);
            Assert.IsTrue(oPhones.Count > 0, "No phone systems returned");
        }

        [TestMethod]
        public void Tenant_Schedules()
        {
            List<ScheduleSet> oSchedules;
            var res = _tempTenant.GetScheduleSets(out oSchedules);
            Assert.IsTrue(res.Success, "Failed to fetch schedule sets:" + res);
            Assert.IsTrue(oSchedules.Count > 0, "No schedule sets returned");
        }

        [TestMethod]
        public void Tenant_FetchUserTemplates()
        {
            List<UserTemplate> oUserTemplates;
            var res = _tempTenant.GetUserTemplates(out oUserTemplates);
            Assert.IsTrue(res.Success, "Failed to fetch user templates:" + res);
            Assert.IsTrue(oUserTemplates.Count > 0, "No user templates returned");
        }

        [TestMethod]
        public void Tenant_FetchUsers()
        {
            List<UserBase> oUsers;
            var res = _tempTenant.GetUsers(out oUsers);
            Assert.IsTrue(res.Success, "Failed to fetch users :" + res);
            Assert.IsTrue(oUsers.Count > 0, "No users returned");
        }

        [TestMethod]
        public void Tenant_FetchTenants()
        {
            List<Tenant> oTenants;
            var res = Tenant.GetTenants(_connectionServer, out oTenants,null);
            Assert.IsTrue(res.Success, "Failed to fetch tenants:" + res);
            Assert.IsTrue(oTenants.Count > 0, "No tenants returned from fetch");

            Tenant oTenant;
            res = Tenant.GetTenant(out oTenant, _connectionServer, oTenants[0].ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch tenant from valid objectid:" + res);

            res = Tenant.GetTenant(out oTenant, _connectionServer, "", oTenants[0].Alias);
            Assert.IsTrue(res.Success, "Failed to fetch tenant from valid alias:" + res);

            Console.WriteLine(oTenant.ToString());
            oTenant.DumpAllProps();

            res = Tenant.GetTenants(_connectionServer, out oTenants,1,2,"query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching tenants with invalid query should not fail:" + res);
            Assert.IsTrue(oTenants.Count == 0, "Invalid query string should return an empty tenant list:" + oTenants.Count);
        }

        #endregion

        
        #region Harness Tesst

        [TestMethod]
        public void Tenant_FetchCoses_Harness_ZeroCount()
        {
            Tenant oTenant = new Tenant(_mockServer);
            List<ClassOfService> oCoses;
            //0 count response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result",
                                      TotalObjectCount = 0
                                  });

            var res = oTenant.GetClassesOfService(out oCoses, 1, 10, "InvalidResultText");
            Assert.IsTrue(res.Success, "Fetching COS with zero count should not fail:" + res);
            Assert.IsTrue(oCoses.Count == 0, "No coses should be returned returned for invalid text");

        }

        [TestMethod]
        public void Tenant_FetchCoses_Harness_GarbageResult()
        {
            Tenant oTenant = new Tenant(_mockServer);
            List<ClassOfService> oCoses;
            
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result",
                                      TotalObjectCount = 1

                                  });

            var res = oTenant.GetClassesOfService(out oCoses, 1, 10, "InvalidResultText");
            Assert.IsFalse(res.Success, "Fetching COS with invalid text should fail:");
            Assert.IsTrue(oCoses.Count == 0, "No coses should be returned returned for invalid text");
        }

        [TestMethod]
        public void Tenant_FetchCoses_Harness_ErrorResult()
        {
            Tenant oTenant = new Tenant(_mockServer);
            List<ClassOfService> oCoses;
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = oTenant.GetClassesOfService(out oCoses, 1, 10, "ErrorResponse");
            Assert.IsFalse(res.Success, "Fetching COS should fail:");
            Assert.IsTrue(oCoses.Count == 0, "No coses should be returned returned");

        }

        [TestMethod]
        public void Tenant_FetchCoses_Harness_EmptyResult()
        {
            Tenant oTenant = new Tenant(_mockServer);

            List<ClassOfService> oCoses;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = oTenant.GetClassesOfService(out oCoses,1,10, "EmptyResultText");
            Assert.IsTrue(res.Success, "Fetching COS with empty results should not fail");
            Assert.IsTrue(oCoses.Count == 0, "No coses should be returned returned");
        }

        [TestMethod]
        public void Tenant_FetchPhoneSystems_Harness()
        {
            Tenant oTenant = new Tenant(_mockServer);

            List<PhoneSystem> oPhoneSystems;

            var res = oTenant.GetPhoneSystems(out oPhoneSystems, 1, 10, null);
            Assert.IsFalse(res.Success, "Fetching phone systems should fail:");
            Assert.IsTrue(oPhoneSystems.Count == 0, "No phone systems should be returned returned");

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            res = oTenant.GetPhoneSystems(out oPhoneSystems, 1, 10, "EmptyResultText");
            Assert.IsFalse(res.Success, "Fetching phone systems should fail:");
            Assert.IsTrue(oPhoneSystems.Count == 0, "No phone systems should be returned returned");

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            //tenants must have at least one phone system
            res = oTenant.GetPhoneSystems(out oPhoneSystems, 1, 10, "InvalidResultText");
            Assert.IsFalse(res.Success, "Fetching phone systems with invalid text should fail");
            Assert.IsTrue(oPhoneSystems.Count == 0, "No coses should be returned returned for invalid text");

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            res = oTenant.GetPhoneSystems(out oPhoneSystems, 1, 10, "ErrorResponse");
            Assert.IsFalse(res.Success, "Fetching phone systems should fail:");
            Assert.IsTrue(oPhoneSystems.Count == 0, "No phone systems should be returned returned");
        }


        [TestMethod]
        public void Tenant_Schedules_Harness_ErrorResult()
        {
            Tenant oTenant = new Tenant(_mockServer);
            List<ScheduleSet> oSchedules;

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = oTenant.GetScheduleSets(out oSchedules, 1, 10, "ErrorResponse");
            Assert.IsFalse(res.Success, "Fetching schedules should fail:");
            Assert.IsTrue(oSchedules.Count == 0, "No schedules should be returned returned");
        }

        [TestMethod]
        public void Tenant_Schedules_Harness_GarbageResult()
        {
            Tenant oTenant = new Tenant(_mockServer);
            List<ScheduleSet> oSchedules;
            
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            var res = oTenant.GetScheduleSets(out oSchedules, 1, 10, "InvalidResultText");
            Assert.IsTrue(res.Success, "Fetching schedules with invalid text should not fail:" + res);
            Assert.IsTrue(oSchedules.Count == 0, "No schedules should be returned returned for invalid text");
        }

        [TestMethod]
        public void Tenant_Schedules_Harness_ZeroCount()
        {
            Tenant oTenant = new Tenant(_mockServer);
            List<ScheduleSet> oSchedules;
            //0 count response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result",
                                      TotalObjectCount = 0

                                  });

            var res = oTenant.GetScheduleSets(out oSchedules, 1, 10, "zerocountresponse");
            Assert.IsTrue(res.Success, "Fetching schedules with zero count should not fail:");
            Assert.IsTrue(oSchedules.Count == 0, "No schedules should be returned");
        }

        
        [TestMethod]
        public void Tenant_Schedules_Harness_EmptyResult()
        {
            Tenant oTenant = new Tenant(_mockServer);
            List<ScheduleSet> oSchedules;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = oTenant.GetScheduleSets(out oSchedules, 1, 10, "EmptyResultText");
            Assert.IsTrue(res.Success, "Fetching schedules with empty result set should not fail:"+res);
            Assert.IsTrue(oSchedules.Count == 0, "No schedules should be returned");
        }


        [TestMethod]
        public void Tenant_CreationErrors_Harness()
        {
            Tenant oTenant;
            
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = Tenant.GetTenant(out oTenant, _mockServer, "EmptyResultText");
            Assert.IsFalse(res.Success,"Creating tenant with empty result text should fail");

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            res = Tenant.GetTenant(out oTenant, _mockServer, "InvalidResultText");
            Assert.IsFalse(res.Success, "Creating tenant with InvalidResultText should fail");

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            res = Tenant.GetTenant(out oTenant, _mockServer, "ErrorResponse");
            Assert.IsFalse(res.Success, "Creating tenant with eErrorResponse should fail");

        }

        [TestMethod]
        public void Tenant_GetTenants_Errors_Harness()
        {
            List<Tenant> oTenants;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = Tenant.GetTenants(_mockServer, out oTenants, 1, 10, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetTenants with empty result text should fail");
            Assert.IsTrue(oTenants.Count==0, "Zero tenants should be returned");

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            res = Tenant.GetTenants(_mockServer, out oTenants, 1, 10, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetTenants with InvalidResultText should not fail:"+res);
            Assert.IsTrue(oTenants.Count == 0, "Zero tenants should be returned");

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            res = Tenant.GetTenants(_mockServer, out oTenants, 1, 10, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetTenants with ErrorResponse should fail");
        }


        #endregion
    
    }

        

    }

