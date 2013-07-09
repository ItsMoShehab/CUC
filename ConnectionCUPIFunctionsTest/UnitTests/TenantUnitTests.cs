using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class TenantUnitTests : BaseUnitTests
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


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            Tenant oTest = new Tenant(null);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        public void Constructor_NoObjectIdOrAlias_Success()
        {
            //should not throw an exception
            Tenant oTest=new Tenant(_mockServer);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Test Failures

        [TestMethod]
        public void DeleteTenant_NullConnectionServer_Failure()
        {
            var res = Tenant.DeleteTenant(null, "objectid");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void DeleteTenant_EmptyObjectId_Failure()
        {
            var res = Tenant.DeleteTenant(_mockServer, "");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetTenant_NullConnectionServer_Failure()
        {
            Tenant oTenant;
            var res = Tenant.GetTenant(out oTenant, null, "objectId");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetTenant_NoObjectIdOrAlias_Failure()
        {
            Tenant oTenant;
            var res = Tenant.GetTenant(out oTenant, _mockServer);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetTenants_NullConnectionServer_Failure()
        {
            List<Tenant> oTenants;
            var res = Tenant.GetTenants(null, out oTenants, 1, 1,null);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void AddTenant_NullConnectionServer_Failure()
        {
            WebCallResult res = Tenant.AddTenant(null, "alias", "domain", "description");
            Assert.IsFalse(res.Success,"");

           }

        [TestMethod]
        public void AddTenant_EmptyAlias_Failure()
        {
            var res = Tenant.AddTenant(_mockServer, "", "domain", "description");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void AddTenant_EmptyDomain_Failure()
        {
            var res = Tenant.AddTenant(_mockServer, "alias", "", "description");
            Assert.IsFalse(res.Success, "");
        }

        #endregion

      
        #region Harness Tesst

        [TestMethod]
        public void GetClassesOfService_ZeroCount_Success()
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
        public void GetClassesOfService_GarbageResult_Failure()
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
        public void GetClassesOfService_ErrorResult_Failure()
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
        public void GetClassesOfService_EmptyResult_Success()
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
        public void GetPhoneSystems_EmptyServer_Failure()
        {
            Tenant oTenant = new Tenant(_mockServer);

            List<PhoneSystem> oPhoneSystems;

            var res = oTenant.GetPhoneSystems(out oPhoneSystems, 1, 10, null);
            Assert.IsFalse(res.Success, "Fetching phone systems should fail:");
            Assert.IsTrue(oPhoneSystems.Count == 0, "No phone systems should be returned returned");
        }

        [TestMethod]
        public void GetPhoneSystems_EmptyResults_Failure()
        {
            Tenant oTenant = new Tenant(_mockServer);

            List<PhoneSystem> oPhoneSystems;
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = oTenant.GetPhoneSystems(out oPhoneSystems, 1, 10, "EmptyResultText");
            Assert.IsFalse(res.Success, "Fetching phone systems should fail:");
            Assert.IsTrue(oPhoneSystems.Count == 0, "No phone systems should be returned returned");

            }

        [TestMethod]
        public void GetPhoneSystems_GarbageResults_Failure()
        {
            Tenant oTenant = new Tenant(_mockServer);

            List<PhoneSystem> oPhoneSystems;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            var res = oTenant.GetPhoneSystems(out oPhoneSystems, 1, 10, "InvalidResultText");
            Assert.IsFalse(res.Success, "Fetching phone systems with invalid text should fail");
            Assert.IsTrue(oPhoneSystems.Count == 0, "No coses should be returned returned for invalid text");

            }

        [TestMethod]
        public void GetPhoneSystems_ErrorResponse_Failure()
        {
            Tenant oTenant = new Tenant(_mockServer);

            List<PhoneSystem> oPhoneSystems;
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = oTenant.GetPhoneSystems(out oPhoneSystems, 1, 10, "ErrorResponse");
            Assert.IsFalse(res.Success, "Fetching phone systems should fail:");
            Assert.IsTrue(oPhoneSystems.Count == 0, "No phone systems should be returned returned");
        }


        [TestMethod]
        public void GetScheduleSets_ErrorResult_Failure()
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
        public void GetScheduleSets_GarbageResult_Success()
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
        public void GetScheduleSets_ZeroCount_Success()
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
        public void GetScheduleSets_EmptyResult_Success()
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
        public void GetTenant_EmptyResults_Failure()
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

            }


        [TestMethod]
        public void GetTenant_GarbageResults_Failure()
        {
            Tenant oTenant;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            var res = Tenant.GetTenant(out oTenant, _mockServer, "InvalidResultText");
            Assert.IsFalse(res.Success, "Creating tenant with InvalidResultText should fail");

            }


        [TestMethod]
        public void GetTenant_ErrorResults_Failure()
        {
            Tenant oTenant;
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = Tenant.GetTenant(out oTenant, _mockServer, "ErrorResponse");
            Assert.IsFalse(res.Success, "Creating tenant with eErrorResponse should fail");

        }

        [TestMethod]
        public void GetTenants_EmptyResults_Failure()
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

            }

        [TestMethod]
        public void GetTenants_GarbageResults_Success()
        {
            List<Tenant> oTenants;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            var res = Tenant.GetTenants(_mockServer, out oTenants, 1, 10, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetTenants with InvalidResultText should not fail:"+res);
            Assert.IsTrue(oTenants.Count == 0, "Zero tenants should be returned");

            }

        [TestMethod]
        public void GetTenants_ErrorResults_Failure()
        {
            List<Tenant> oTenants;
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = Tenant.GetTenants(_mockServer, out oTenants, 1, 10, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetTenants with ErrorResponse should fail");
        }


        #endregion
    
    }

        

    }

