using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for PhoneSystemUnitTests and is intended
    ///to contain all PhoneSystemUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class PhoneSystemUnitTests : BaseUnitTests
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


        #region Class Creation Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            PhoneSystem oTest = new PhoneSystem(null, "aaa");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        public void Constructor_EmptyObjectIdAndDisplayName_Success()
        {
            PhoneSystem oTest = new PhoneSystem(_mockServer);
            Console.WriteLine(oTest.ToString());
            Console.WriteLine(oTest.DumpAllProps());
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            PhoneSystem oTest = new PhoneSystem();
            Console.WriteLine(oTest.SelectionDisplayString);
            Console.WriteLine(oTest.UniqueIdentifier);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_DisplayName_ErrorResponse_Failure()
        {
            _mockTransport.Setup(m => m.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(),
                                                        It.IsAny<ConnectionServerRest>(), It.IsAny<string>(),
                                                        It.IsAny<bool>())).Returns(new WebCallResult
                                                            {
                                                                ResponseText = "Error response text",
                                                                Success = false,
                                                            });
            PhoneSystem oTest = new PhoneSystem(_mockServer, "","Display Name");
            Console.WriteLine(oTest);
        }


        #endregion


        #region Static Call Tests 

        [TestMethod]
        public void GetPhoneSystems_NullConnectionServer()
        {
            List<PhoneSystem> oPhoneSystems;
            var res = PhoneSystem.GetPhoneSystems(null, out oPhoneSystems, 1, 10, "");
            Assert.IsFalse(res.Success,"Calling GetPhoneSystems with null ConnectionServer should fail");
        }

        [TestMethod]
        public void AddPhoneSystem_NullConnectionServer_Failure()
        {
            PhoneSystem oPhoneSystem;
            WebCallResult res = PhoneSystem.AddPhoneSystem(null, "bogus",out oPhoneSystem);
            Assert.IsFalse(res.Success, "Static call to AddPhoneSystem with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void AddPhoneSystem_EmptyName_Failure()
        {
            PhoneSystem oPhoneSystem;
            var res = PhoneSystem.AddPhoneSystem(_mockServer, "",out oPhoneSystem);
            Assert.IsFalse(res.Success, "Static call to AddPhoneSystem with empty name did not fail");
        }

        [TestMethod]
        public void DeletePhoneSystem_NullConnectionServer_Failure()
        {
            var res = PhoneSystem.DeletePhoneSystem(null, "objectid");
            Assert.IsFalse(res.Success, "Static call to DeletePhoneSystem with null ConnectionServerRest did not fail");

        }

        [TestMethod]
        public void DeletePhoneSystem_BlankObjectId_Failure()
        {
            var res = PhoneSystem.DeletePhoneSystem(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to DeletePhoneSystem with blank ObjectId did not fail");
        }


        [TestMethod]
        public void UpdatePhoneSystem_NullConnectionServer_Failure()
        {
            var res = PhoneSystem.UpdatePhoneSystem(null, "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePhoneSystem with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void UpdatePhoneSystem_BlankObjectId_Failure()
        {
            var res = PhoneSystem.UpdatePhoneSystem(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePhoneSystem with blank ObjectId did not fail");
        }

        [TestMethod]
        public void GetPhoneSystemAssociations_NullConnectionServer_Failure()
        {
            List<PhoneSystemAssociation> oList;
            var res = PhoneSystem.GetPhoneSystemAssociations(null, "objectid", out oList);
            Assert.IsFalse(res.Success,
                           "Static call to GetPhoneSystemAssociations with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void GetPhoneSystemAssociations_BlankObjectId_Failure()
        {
            List<PhoneSystemAssociation> oList;
            var res = PhoneSystem.GetPhoneSystemAssociations(_mockServer, "", out oList);
            Assert.IsFalse(res.Success, "Fetching phone system associations with blank objectid should fail");
        }


        [TestMethod]
        public void GetPhoneSystem_NullConnectionServer_Failure()
        {
            PhoneSystem oPhoneSystem;
            var res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, null, "objectid");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void GetPhoneSystem_BlankObjectIdAndName_Failure()
        {
            PhoneSystem oPhoneSystem;
            var res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, _mockServer, "");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with invalid empty objectId and name did not fail");
        }

        [TestMethod]
        public void PhoneSystemAssociation_ToString()
        {
            PhoneSystemAssociation oPhoneSystemAssociation = new PhoneSystemAssociation();
            Console.WriteLine(oPhoneSystemAssociation.ToString());
        }


        #endregion


        #region Property Tests

        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const string expectedValue = "Test string";
            oPhoneSystem.DisplayName = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("DisplayName", expectedValue),"DisplayName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CallLoopDTMF()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const string expectedValue = "Test string";
            oPhoneSystem.CallLoopDTMF = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("CallLoopDTMF", expectedValue), "CallLoopDTMF value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CallLoopExtensionDetect()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = true;
            oPhoneSystem.CallLoopExtensionDetect = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("CallLoopExtensionDetect", expectedValue), "CallLoopExtensionDetect value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CallLoopForwardNotificationDetect()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = false;
            oPhoneSystem.CallLoopForwardNotificationDetect = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("CallLoopForwardNotificationDetect", expectedValue), 
                "CallLoopForwardNotificationDetect value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CallLoopGuardTimeMs()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const int expectedValue = 321;
            oPhoneSystem.CallLoopGuardTimeMs = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("CallLoopGuardTimeMs", expectedValue), "CallLoopGuardTimeMs value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CallLoopSupervisedTransferDetect()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = false;
            oPhoneSystem.CallLoopSupervisedTransferDetect = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("CallLoopSupervisedTransferDetect", expectedValue), 
                "CallLoopSupervisedTransferDetect value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DefaultTrapSwitch()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = false;
            oPhoneSystem.DefaultTrapSwitch = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("DefaultTrapSwitch", expectedValue),"DefaultTrapSwitch value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_EnablePhoneApplications()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = false;
            oPhoneSystem.EnablePhoneApplications = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("EnablePhoneApplications", expectedValue),
                "EnablePhoneApplications value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MwiAlwaysUpdate()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = false;
            oPhoneSystem.MwiAlwaysUpdate = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("MwiAlwaysUpdate", expectedValue),"MwiAlwaysUpdate value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MwiPortMemory()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = false;
            oPhoneSystem.MwiPortMemory = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("MwiPortMemory", expectedValue),"MwiPortMemory value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MwiForceOff()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = false;
            oPhoneSystem.MwiForceOff = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("MwiForceOff", expectedValue),"MwiForceOff value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RestrictDialUnconditional()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = false;
            oPhoneSystem.RestrictDialUnconditional = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("RestrictDialUnconditional", expectedValue), 
                "RestrictDialUnconditional value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RestrictDialScheduled()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const bool expectedValue = false;
            oPhoneSystem.RestrictDialScheduled = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("RestrictDialScheduled", expectedValue), 
                "RestrictDialScheduled value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RestrictDialStartTime()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const int expectedValue = 1234;
            oPhoneSystem.RestrictDialStartTime = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("RestrictDialStartTime", expectedValue), 
                "RestrictDialStartTime value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RestrictDialEndTimef()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem();
            const int expectedValue = 345;
            oPhoneSystem.RestrictDialEndTime = expectedValue;
            Assert.IsTrue(oPhoneSystem.ChangeList.ValueExists("RestrictDialEndTime", expectedValue), "RestrictDialEndTime value get fetch failed");
        }


        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetPhoneSystems_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<PhoneSystem> oPhoneSystems;
            var res = PhoneSystem.GetPhoneSystems(_mockServer, out oPhoneSystems, 1, 5, "");
            Assert.IsFalse(res.Success, "Calling GetPhoneSystems with EmptyResultText did not fail");

        }

        [TestMethod]
        public void GetPhoneSystems_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<PhoneSystem> oPhoneSystems;
            var res = PhoneSystem.GetPhoneSystems(_mockServer, out oPhoneSystems, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetPhoneSystems with garbage results should fail");
            Assert.IsTrue(oPhoneSystems.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetPhoneSystems_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<PhoneSystem> oPhoneSystems;
            var res = PhoneSystem.GetPhoneSystems(_mockServer, out oPhoneSystems, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetPhoneSystems with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetPhoneSystems_ZeroCount_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<PhoneSystem> oPhoneSystems;
            var res = PhoneSystem.GetPhoneSystems(_mockServer, out oPhoneSystems, 1, 5, null);
            Assert.IsTrue(res.Success, "Calling GetPhoneSystems with ZeroCount failed:" + res);
        }


        [TestMethod]
        public void GetPhoneSystem_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            PhoneSystem oPhoneSystem;
            var res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, _mockServer,"ObjectId");
            Assert.IsFalse(res.Success, "Calling GetPhoneSystem with ErrorResponse did not fail");
        }


        [TestMethod]
        public void UpdatePhoneSystem_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test","Test");
            var res = PhoneSystem.UpdatePhoneSystem(_mockServer, "MediaSwitchObjectId",oProps);
            Assert.IsFalse(res.Success, "Calling UpdatePhoneSystem with ErrorResponse did not fail");
        }

        [TestMethod]
        public void AddPhoneSystem_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            PhoneSystem oPhoneSystem;
            var res = PhoneSystem.AddPhoneSystem(_mockServer, "MediaSwitchObjectId", out oPhoneSystem);
            Assert.IsFalse(res.Success, "Calling AddPhoneSystem with ErrorResponse did not fail");
        }

        [TestMethod]
        public void DeletePhoneSystem_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = PhoneSystem.DeletePhoneSystem(_mockServer, "ObjectId");
            Assert.IsFalse(res.Success, "Calling DeletePhoneSystem with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetPhoneSystemAssociations_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<PhoneSystemAssociation> oPhoneSystemAssociations;
            var res = PhoneSystem.GetPhoneSystemAssociations(_mockServer, "ObjectId", out oPhoneSystemAssociations, 1, 5);
            Assert.IsFalse(res.Success, "Calling GetPhoneSystemAssociations with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetPhoneSystemAssociations_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<PhoneSystemAssociation> oPhoneSystemAssociations;
            var res = PhoneSystem.GetPhoneSystemAssociations(_mockServer, "ObjectId", out oPhoneSystemAssociations, 1, 5);
            Assert.IsFalse(res.Success, "Calling GetPhoneSystemAssociations with garbage results should fail");
            Assert.IsTrue(oPhoneSystemAssociations.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetPhoneSystemAssociations_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<PhoneSystemAssociation> oPhoneSystemAssociations;
            var res = PhoneSystem.GetPhoneSystemAssociations(_mockServer, "ObjectId", out oPhoneSystemAssociations, 1, 5);
            Assert.IsFalse(res.Success, "Calling GetPhoneSystemAssociations with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetPhoneSystemAssociations_ZeroCount_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<PhoneSystemAssociation> oPhoneSystemAssociations;
            var res = PhoneSystem.GetPhoneSystemAssociations(_mockServer, "ObjectId", out oPhoneSystemAssociations, 1, 5);
            Assert.IsTrue(res.Success, "Calling GetPhoneSystemAssociations with ZeroCount failed:" + res);
        }


        [TestMethod]
        public void RefetchPhoneSystemData_EmptyClass_Failure()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem(_mockServer);
            var res = oPhoneSystem.RefetchPhoneSystemData();
            Assert.IsFalse(res.Success,"Calling RefetchPhoneSystemData with an empty class instance should fail");
        }


        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem(_mockServer);
            var res = oPhoneSystem.Update();
            Assert.IsFalse(res.Success, "Calling Update with no pending changes should fail");
        }

        [TestMethod]
        public void Update_EmptyClass_Failure()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem(_mockServer);
            oPhoneSystem.DisplayName = "Updated display name";
            var res = oPhoneSystem.Update();
            Assert.IsFalse(res.Success, "Calling Update on empty class should fail");
        }

        [TestMethod]
        public void Delete_EmptyClass_Failure()
        {
            PhoneSystem oPhoneSystem = new PhoneSystem(_mockServer);
            var res = oPhoneSystem.Delete();
            Assert.IsFalse(res.Success, "Calling Delete on empty class should fail");
        }

        [TestMethod]
        public void GetPhoneSystemAssociations_EmptyClass_Failure()
        {
            List<PhoneSystemAssociation> oPhoneSystemAssociations;
            
            PhoneSystem oPhoneSystem = new PhoneSystem(_mockServer);
            var res = oPhoneSystem.GetPhoneSystemAssociations(out oPhoneSystemAssociations);
            Assert.IsFalse(res.Success, "Calling GetPhoneSystemAssociations on empty class should fail");
        }

        #endregion
    }
}

