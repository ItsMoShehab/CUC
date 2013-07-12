using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class MwiUnitTests : BaseUnitTests
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
        [ExpectedException(typeof (ArgumentException))]
        public void Constuctor_NullConnectionServer_Failure()
        {
            var oTemp = new Mwi(null,"UserObjectid","objectid");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void Constructor_NonEmptyOBjectId_Success()
        {
            var oTemp = new Mwi(_mockServer, "ObjectId");
            Console.WriteLine(oTemp.ToString());
            Console.WriteLine(oTemp.DumpAllProps());
            Console.WriteLine(oTemp.SelectionDisplayString);
            Console.WriteLine(oTemp.UniqueIdentifier);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyUserObjectId_Failure()
        {
            var oTemp = new Mwi(_mockServer, "");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Tests

        [TestMethod]
        public void GetMwiDevice_NullConnectionServer_Failure()
        {
            Mwi oMwi;
            var res = Mwi.GetMwiDevice(null, "UserObjectId", "ObjectId", out oMwi);
            Assert.IsFalse(res.Success,"Calling GetMwiDevice with null connection server should fail");
        }

        [TestMethod]
        public void GetMwiDevice_EmptyUserObjectId_Failure()
        {
            Mwi oMwi;
            var res = Mwi.GetMwiDevice(_mockServer, "", "ObjectId", out oMwi);
            Assert.IsFalse(res.Success, "Calling GetMwiDevice with empty user objectId should fail");
        }

        [TestMethod]
        public void GetMwiDevice_EmptyObjectId_Failure()
        {
            Mwi oMwi;
            var res = Mwi.GetMwiDevice(_mockServer, "UserObjectId", "", out oMwi);
            Assert.IsFalse(res.Success, "Calling GetMwiDevice with empty objectId should fail");
        }


        [TestMethod]
        public void GetMwiDevices_NullConnectionServer_Failure()
        {
            List<Mwi> oMwis;
            var res = Mwi.GetMwiDevices(null, "UserObjectId", out oMwis);
            Assert.IsFalse(res.Success, "Calling GetMwiDevices with null connection server should fail");
        }

        [TestMethod]
        public void GetMwiDevices_EmptyUserObjectId_Failure()
        {
            List<Mwi> oMwis;
            var res = Mwi.GetMwiDevices(_mockServer, "", out oMwis);
            Assert.IsFalse(res.Success, "Calling GetMwiDevices with empty UserObjectId should fail");
        }


        [TestMethod]
        public void AddMwi_NullConnectionServer_Failure()
        {
            var res = Mwi.AddMwi(null, "UserOBjectId","Device Name","SwitchObjectId","1234", false);
            Assert.IsFalse(res.Success, "Calling AddMwi with null connectionServer should fail");
        }

        [TestMethod]
        public void AddMwi_EmptyUserObjectId_Failure()
        {
            var res = Mwi.AddMwi(_mockServer, "", "Device Name", "SwitchObjectId", "1234", false);
            Assert.IsFalse(res.Success, "Calling AddMwi with empty user objectId should fail");
        }

        [TestMethod]
        public void AddMwi_EmptyDeviceName_Failure()
        {
            var res = Mwi.AddMwi(_mockServer, "UserOBjectId", "", "SwitchObjectId", "1234", false);
            Assert.IsFalse(res.Success, "Calling AddMwi with empty objectId should fail");
        }

        [TestMethod]
        public void AddMwi_SwitchObjectId_Failure()
        {
            var res = Mwi.AddMwi(_mockServer, "UserOBjectId", "Device Name", "", "1234", false);
            Assert.IsFalse(res.Success, "Calling AddMwi with empty switchObjectId should fail");
        }

        [TestMethod]
        public void AddMwi_EmptyExtension_Failure()
        {
            var res = Mwi.AddMwi(_mockServer, "UserOBjectId", "Device Name", "SwitchObjectId", "", false);
            Assert.IsFalse(res.Success, "Calling AddMwi with empty extension should fail");
        }

        [TestMethod]
        public void DeleteMwiDevice_NullConnectionServer_Failure()
        {
            var res = Mwi.DeleteMwiDevice(null, "UserOBjectId", "ObjectId");
            Assert.IsFalse(res.Success, "Calling DeleteMwiDevice with null connection server should fail");
        }

        [TestMethod]
        public void DeleteMwiDevice_EmptyUserObjectId_Failure()
        {
            var res = Mwi.DeleteMwiDevice(_mockServer, "", "ObjectId");
            Assert.IsFalse(res.Success, "Calling DeleteMwiDevice with empty UserObjectId should fail");
        }

        [TestMethod]
        public void DeleteMwiDevice_EmptyObjectId_Failure()
        {
            var res = Mwi.DeleteMwiDevice(_mockServer, "UserOBjectId", "");
            Assert.IsFalse(res.Success, "Calling DeleteMwiDevice with empty objectId should fail");
        }

        [TestMethod]
        public void UpdateMwi_NullConnectionServer_Failure()
        {
            var res = Mwi.UpdateMwi(null, "UserOBjectId", "ObjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateMwi with null ConnectionServer should fail");
        }

        [TestMethod]
        public void UpdateMwi_EmptyUserObjectId_Failure()
        {
            var res = Mwi.UpdateMwi(_mockServer, "", "ObjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateMwi with empty UserobjectId should fail");
        }
        
        [TestMethod]
        public void UpdateMwi_EmptyObjectId_Failure()
        {
            var res = Mwi.UpdateMwi(_mockServer, "UserOBjectId", "", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateMwi with empty objectId should fail");
        }
        
        [TestMethod]
        public void UpdateMwi_NullPropertyList_Failure()
        {
            var res = Mwi.UpdateMwi(_mockServer, "UserOBjectId", "ObjectId", null);
            Assert.IsFalse(res.Success, "Calling UpdateMwi with null property list should fail");
        }

        [TestMethod]
        public void UpdateMwi_EmptyPropertyList_Failure()
        {
            var res = Mwi.UpdateMwi(_mockServer, "UserOBjectId", "ObjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateMwi with empty property list should fail");
        }

        #endregion

        #region Harness Tests

        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            var oTemp = new Mwi(_mockServer, "ObjectId");
            oTemp.ClearPendingChanges();
            var res = oTemp.Update();
            Assert.IsFalse(res.Success,"Update should fail with no pending updates");
        }
        
        [TestMethod]
        public void Update_InvalidHomeServer_Failure()
        {
            var oTemp = new Mwi(_mockServer, "ObjectId");
            oTemp.UsePrimaryExtension = false;
            var res = oTemp.Update();
            Assert.IsFalse(res.Success, "Update should fail with invalid HomeServer property updates");
        }

        [TestMethod]
        public void Delete_InvalidHomeServer_Failure()
        {
            var oTemp = new Mwi(_mockServer, "ObjectId");
            var res = oTemp.Delete();
            Assert.IsFalse(res.Success, "Delete should fail with invalid HomeServer property updates");
        }


        [TestMethod]
        public void GetMwiDevice_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            Mwi oMwi;
            var res = Mwi.GetMwiDevice(_mockServer, "userObjectId", "ObjectId", out oMwi);
            Assert.IsFalse(res.Success,"Calling GetMwiDevice with an error response should fail");
        }


        [TestMethod]
        public void GetMwiDevices_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
             List<Mwi> oMwis;
            var res = Mwi.GetMwiDevices(_mockServer, "userObjectId", out oMwis);
            Assert.IsFalse(res.Success, "Calling GetMwiDevices with an error response should fail");
        }

        [TestMethod]
        public void GetMwiDevices_GarbageResponse_Failure()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result in the response body that will not parse properly to mwi device"
                                  });
            List<Mwi> oMwis;
            var res = Mwi.GetMwiDevices(_mockServer, "userObjectId", out oMwis);
            Assert.IsFalse(res.Success, "Calling GetMwiDevices with a garbage response should fail");
        }

        [TestMethod]
        public void GetMwiDevices_EmptyResponse_Failure()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });
            List<Mwi> oMwis;
            var res = Mwi.GetMwiDevices(_mockServer, "userObjectId", out oMwis);
            Assert.IsFalse(res.Success, "Calling GetMwiDevices with an empty response should fail");
        }

        [TestMethod]
        public void GetMwiDevices_ZeroResult_Success()
        {
            //zero results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           TotalObjectCount = 0,
                                           ResponseText = "junk"
                                       });
            List<Mwi> oMwis;
            var res = Mwi.GetMwiDevices(_mockServer, "userObjectId", out oMwis);
            Assert.IsFalse(res.Success, "Calling GetMwiDevices with zero result failed:"+res);
            Assert.IsTrue(oMwis.Count==0,"Calling GetMwiDevices with zero result should produce an empty list");
        }



        [TestMethod]
        public void AddMwi_GarbageResponse_Failure()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result in the response body that will not parse properly to mwi device"
                                  });
            var res = Mwi.AddMwi(_mockServer, "userObjectId", "Device Name","SwitchId","1234",true);
            Assert.IsFalse(res.Success, "Calling AddMwi with a garbage response should fail");
        }

        [TestMethod]
        public void AddMwi_EmptyResponse_Failure()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });
            var res = Mwi.AddMwi(_mockServer, "userObjectId", "Device Name", "SwitchId", "1234", true);
            Assert.IsFalse(res.Success, "Calling AddMwi with an empty response should fail");
        }

        [TestMethod]
        public void AddMwi_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            var res = Mwi.AddMwi(_mockServer, "userObjectId", "Device Name", "SwitchId", "1234", true);
            Assert.IsFalse(res.Success, "Calling AddMwi with an error response should fail");
        }

        [TestMethod]
        public void DeleteMwiDevice_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            var res = Mwi.DeleteMwiDevice(_mockServer, "userObjectId", "Device Name");
            Assert.IsFalse(res.Success, "Calling DeleteMwiDevice with an error response should fail");
        }

        #endregion
    }
}
