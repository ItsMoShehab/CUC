using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortUnitTests : BaseUnitTests 
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
            Port oPort = new Port(null);
            Console.WriteLine(oPort);
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            BaseUnitTests.Reset();
            Port oPort = new Port();
            Console.WriteLine(oPort);
        }

        [TestMethod]
        public void Constructor_EmptyObjectId_Success()
        {
            BaseUnitTests.Reset();
            Port oPort = new Port(_mockServer);
            Console.WriteLine(oPort);
        }

        [TestMethod]
        public void Constructor_ObjectId_Success()
        {
            BaseUnitTests.Reset();
            Port oPort = new Port(_mockServer,"ObjectId");
            Console.WriteLine(oPort.ToString());
            Console.WriteLine(oPort.DumpAllProps());
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_ObjectId_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            Port oPort = new Port(_mockServer, "ObjectId");
            Console.WriteLine(oPort);
        }


        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidConnectionServer_Failure()
        {
            Port oPort = new Port(new ConnectionServerRest(new RestTransportFunctions()),"objectid");
            Console.WriteLine(oPort);
        }

        #endregion


        #region Static Call Tests 

        [TestMethod]
        public void UpdatePort_NullConnectionServer_Failure()
        {
            var res = Port.UpdatePort(null, "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePort did not fail with null Connection server");
        }

        [TestMethod]
        public void UpdatePort_EmptyObjectId_Failure()
        {
            var res = Port.UpdatePort(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePort did not fail with empty objectid");
        }

        [TestMethod]
        public void AddPort_NullConnectionServer_Failure()
        {
            var res = Port.AddPort(null, "portgroupid", 4, null);
            Assert.IsFalse(res.Success, "Static call to AddPort did not fail with null connection server");

                    }

        [TestMethod]
        public void AddPort_EmptyObjectId_Failure()
        {
            var res = Port.AddPort(_mockServer, "", 4, null);
            Assert.IsFalse(res.Success, "Static call to AddPort did not fail with empty port group objectId");
        }

        [TestMethod]
        public void DeletePort_NullConnectionServer_Failure()
        {
            var res = Port.DeletePort(null, "objectId");
            Assert.IsFalse(res.Success, "Static call to DeletePort did not fail with null Connection server");

        }

        [TestMethod]
        public void DeletePort_EmptyObjectId_Failure()
        {
            var res = Port.DeletePort(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to DeletePort did not fail with empty objectId");
        }

        [TestMethod]
        public void GetPort_NullConnectionServer_Failure()
        {
            Port oPort;
            WebCallResult res = Port.GetPort(out oPort, null, "objectId");
            Assert.IsFalse(res.Success,"Static call to GetPort did not fail with null Connection server");

            }

        [TestMethod]
        public void GetPort_EmptyObjectId_Failure()
        {
            Port oPort;
            var res = Port.GetPort(out oPort, _mockServer, "");
            Assert.IsFalse(res.Success, "Static call to GetPort did not fail with empty objectId");
        }

        [TestMethod]
        public void GetPorts_NullConnectionServer_Failure()
        {
            List<Port> oPorts;
            var res = Port.GetPorts(null, out oPorts, 1,2, "");
            Assert.IsFalse(res.Success, "Static call to GetPorts did not fail with null ConnectionServer");
        }

        #endregion


        #region Property Tests

        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            Port oPort = new Port();
            const string expectedValue = "String test";
            oPort.DisplayName = expectedValue;
            Assert.IsTrue(oPort.ChangeList.ValueExists("DisplayName", expectedValue),"DisplayName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SkinnySecurityModeEnum()
        {
            Port oPort = new Port();
            const SkinnySecurityModes expectedValue = SkinnySecurityModes.Authenticated;
            oPort.SkinnySecurityModeEnum = expectedValue;
            Assert.IsTrue(oPort.ChangeList.ValueExists("SkinnySecurityModeEnum", (int)expectedValue), "SkinnySecurityModeEnum value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_HuntOrder()
        {
            Port oPort = new Port();
            const int expectedValue = 2;
            oPort.HuntOrder = expectedValue;
            Assert.IsTrue(oPort.ChangeList.ValueExists("HuntOrder", expectedValue), "HuntOrder value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CapAnswer()
        {
            Port oPort = new Port();
            const bool expectedValue = false;
            oPort.CapAnswer = expectedValue;
            Assert.IsTrue(oPort.ChangeList.ValueExists("CapAnswer", expectedValue), "CapAnswer value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CapNotification()
        {
            Port oPort = new Port();
            const bool expectedValue = true;
            oPort.CapNotification = expectedValue;
            Assert.IsTrue(oPort.ChangeList.ValueExists("CapNotification", expectedValue), "CapNotification value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CapMWI()
        {
            Port oPort = new Port();
            const bool expectedValue = true;
            oPort.CapMWI = expectedValue;
            Assert.IsTrue(oPort.ChangeList.ValueExists("CapMWI", expectedValue), "CapMWI value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CapEnabled()
        {
            Port oPort = new Port();
            const bool expectedValue = true;
            oPort.CapEnabled = expectedValue;
            Assert.IsTrue(oPort.ChangeList.ValueExists("CapEnabled", expectedValue), "CapEnabled value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CapTrapConnection()
        {
            Port oPort = new Port();
            const bool expectedValue = true;
            oPort.CapTrapConnection = expectedValue;
            Assert.IsTrue(oPort.ChangeList.ValueExists("CapTrapConnection", expectedValue), "CapTrapConnection value get fetch failed");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetPorts_EmptyResult_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<Port> oPorts;
            var res = Port.GetPorts(_mockServer, out oPorts, 1, 5, "");
            Assert.IsFalse(res.Success, "Calling GetPorts with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetPorts_GarbageResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as JSON data"
                                  });

            List<Port> oPorts;
            var res = Port.GetPorts(_mockServer, out oPorts, 1, 5,null);
            Assert.IsFalse(res.Success, "Calling GetPorts with garbage results should fail");
            Assert.IsTrue(oPorts.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetPorts_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<Port> oPorts;
            var res = Port.GetPorts(_mockServer, out oPorts,"PortGroupObjectId");
            Assert.IsFalse(res.Success, "Calling GetPorts with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetPorts_ZeroCount_Success()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<Port> oPorts;
            var res = Port.GetPorts(_mockServer, out oPorts, 1, 5, "");
            Assert.IsTrue(res.Success, "Calling GetPorts with ZeroCount failed:" + res);
        }

        [TestMethod]
        public void GetPort_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            Port oPort;
            var res = Port.GetPort(out oPort, _mockServer,"ObjectId");
            Assert.IsFalse(res.Success, "Calling GetPort with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdatePort_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ConnectionPropertyList oProps=new ConnectionPropertyList();
            oProps.Add("Test","Test");
            var res = Port.UpdatePort(_mockServer, "ObjectId",oProps);
            Assert.IsFalse(res.Success, "Calling UpdatePort with ErrorResponse did not fail");
        }

        [TestMethod]
        public void AddPort_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = Port.AddPort(_mockServer, "ObjectId", 2,null);
            Assert.IsFalse(res.Success, "Calling AddPort with ErrorResponse did not fail");
        }

        [TestMethod]
        public void DeletePort_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.DELETE, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = Port.DeletePort(_mockServer, "ObjectId");
            Assert.IsFalse(res.Success, "Calling DeletePort with ErrorResponse did not fail");
        }


        [TestMethod]
        public void RefetchPortData_EmptyClassInstance_Failure()
        {
            var oPort = new Port(_mockServer);
            var res = oPort.RefetchPortData();
            Assert.IsFalse(res.Success,"Calling RefetchPortData on an empty class instance should fail");
        }

        [TestMethod]
        public void Delete_EmptyClassInstance_Failure()
        {
            var oPort = new Port(_mockServer);
            var res = oPort.Delete();
            Assert.IsFalse(res.Success, "Calling Delete on an empty class instance should fail");
        }

        [TestMethod]
        public void Update_EmptyClassInstance_Failure()
        {
            var oPort = new Port(_mockServer);
            oPort.DisplayName = "New Display Name";
            var res = oPort.Update();
            Assert.IsFalse(res.Success, "Calling Update on an empty class instance should fail");
        }

        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            var oPort = new Port(_mockServer);
            var res = oPort.Update();
            Assert.IsFalse(res.Success, "Calling Update with no pending changes should fail");
        }

        #endregion
    }
}
