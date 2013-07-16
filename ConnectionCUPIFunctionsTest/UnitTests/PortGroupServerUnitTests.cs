using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortGroupServerUnitTests : BaseUnitTests
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


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            PortGroupServer oTemp = new PortGroupServer(null,"PortGroupId","objectID");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// throw ArgumentException on empty objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyPortGroupObjectIdObjectId_Failure()
        {
            PortGroupServer oTemp = new PortGroupServer(_mockServer, "");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void Constructor_EmptyObjectId_Success()
        {
            BaseUnitTests.Reset();
            PortGroupServer oTemp = new PortGroupServer(_mockServer, "PortGroupId", "");
            Console.WriteLine(oTemp.DumpAllProps());
            Console.WriteLine(oTemp.ToString());
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            BaseUnitTests.Reset();
            PortGroupServer oTemp = new PortGroupServer();
        }

        [TestMethod]
        public void Constructor_ObjectId_Success()
        {
            BaseUnitTests.Reset();
            PortGroupServer oTemp = new PortGroupServer(_mockServer, "PortGroupId", "ObjectId");
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_ObjectId_ErrorResponse_Success()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            PortGroupServer oTemp = new PortGroupServer(_mockServer, "PortGroupId", "ObjectId");
        }

        #endregion


        #region Static Call Tests

        [TestMethod]
        public void AddPortGroupServer_NullConnectionServer_Failure()
        {
            var res = PortGroupServer.AddPortGroupServer(null, "", 100, "10.20.30.40");
            Assert.IsFalse(res.Success, "Static call to AddPortGroupServer did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void AddPortGroupServer_EmptyObjectId_Failure()
        {
            PortGroupServer oServer;
            var res = PortGroupServer.AddPortGroupServer(_mockServer   , "", 100, "10.20.30.40","", out oServer);
            Assert.IsFalse(res.Success, "Static call to AddPortGroupServer did not fail with empty objectId");
        }

        [TestMethod]
        public void DeletePortGroupServer_NullConnectionServer_Failure()
        {
            var res = PortGroupServer.DeletePortGroupServer(null, "objectid", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupServer did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void DeletePortGroupServer_EmptyObjectId_Failure()
        {
            var res = PortGroupServer.DeletePortGroupServer(_mockServer, "", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupServer did not fail with empty ObjectId");
        }

        [TestMethod]
        public void DeletePortGroupServer_EmptyPortGroupObjectId_Failure()
        {
            var res = PortGroupServer.DeletePortGroupServer(_mockServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupServer did not fail with empty PortGroupObjectId");
        }

        [TestMethod]
        public void GetPortGroupServer_NullConnectionServer_Failure()
        {
            PortGroupServer oPortGroupServer;
            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, null, "objectid", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to GetPortGroupServer did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void GetPortGroupServer_EmptyObjectId_Failure()
        {
            PortGroupServer oPortGroupServer;
            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _mockServer, "", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to GetPortGroupServer did not fail with empty ObjectId");
        }

        [TestMethod]
        public void GetPortGroupServer_EmptyPortGroupObjectId_Failure()
        {
            PortGroupServer oPortGroupServer;
            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _mockServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to GetPortGroupServer did not fail with empty portgroupObjectId");
        }

        [TestMethod]
        public void UpdatePortGroupServer_NullConnectionServer_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(null, "portgroupobjectid", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void UpdatePortGroupServer_EmptyPortGroupObjectId_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_mockServer, "", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with empty PortGroupObjectId");
        }

        [TestMethod]
        public void UpdatePortGroupServer_EmptyObjectId_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_mockServer, "portgroupobjectid", "", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with empty ObjectId");
        }

        [TestMethod]
        public void UpdatePortGroupServer_NullPropertyList_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_mockServer, "portgroupobjectid", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with null property list");
        }

        [TestMethod]
        public void UpdatePortGroupServer_EmptyPropertyList_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_mockServer, "portgroupobjectid", "objectid", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with empty property list");
        }


        [TestMethod]
        public void GetPortGroupServers_NullConnectionServer_Failure()
        {
            List<PortGroupServer> oList;
            WebCallResult res = PortGroupServer.GetPortGroupServers(null, "", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void GetPortGroupServers_EmptyPortGroupObjectId_Failure()
        {
            List<PortGroupServer> oList;

            var res = PortGroupServer.GetPortGroupServers(_mockServer, "", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: empty objectId");
        }

        #endregion


        #region Property Tests


        [TestMethod]
        public void PropertyGetFetch_HostOrIpAddress()
        {
            PortGroupServer oServer = new PortGroupServer();
            const string expectedValue = "Test string";
            oServer.HostOrIpAddress = expectedValue;
            Assert.IsTrue(oServer.ChangeList.ValueExists("HostOrIpAddress", expectedValue),"HostOrIpAddress value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_HostOrIpAddressV6()
        {
            PortGroupServer oServer = new PortGroupServer();
            const string expectedValue = "Test string";
            oServer.HostOrIpAddressV6 = expectedValue;
            Assert.IsTrue(oServer.ChangeList.ValueExists("HostOrIpAddressV6", expectedValue), "HostOrIpAddressV6 value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MediaRemoteServiceEnum()
        {
            PortGroupServer oServer = new PortGroupServer();
            const MediaRemoteServiceEnum expectedValue = MediaRemoteServiceEnum.CCM;
            oServer.MediaRemoteServiceEnum = expectedValue;
            Assert.IsTrue(oServer.ChangeList.ValueExists("MediaRemoteServiceEnum", (int)expectedValue), 
                "MediaRemoteServiceEnum value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MediaPortGroupObjectId()
        {
            PortGroupServer oServer = new PortGroupServer();
            const string expectedValue = "Test string";
            oServer.MediaPortGroupObjectId = expectedValue;
            Assert.IsTrue(oServer.ChangeList.ValueExists("MediaPortGroupObjectId", expectedValue), "MediaPortGroupObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Port()
        {
            PortGroupServer oServer = new PortGroupServer();
            const int expectedValue = 123;
            oServer.Port = expectedValue;
            Assert.IsTrue(oServer.ChangeList.ValueExists("Port", expectedValue), "Port value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Precedence()
        {
            PortGroupServer oServer = new PortGroupServer();
            const string expectedValue = "Test string";
            oServer.Precedence = expectedValue;
            Assert.IsTrue(oServer.ChangeList.ValueExists("Precedence", expectedValue), "Precedence value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SkinnyStateMachineEnum()
        {
            PortGroupServer oServer = new PortGroupServer();
            const SkinnyStateMachineEnum expectedValue = SkinnyStateMachineEnum.Ccm;
            oServer.SkinnyStateMachineEnum = expectedValue;
            Assert.IsTrue(oServer.ChangeList.ValueExists("SkinnyStateMachineEnum", (int)expectedValue), 
                "SkinnyStateMachineEnum value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TlsPort()
        {
            PortGroupServer oServer = new PortGroupServer();
            const string expectedValue = "Test string";
            oServer.TlsPort = expectedValue;
            Assert.IsTrue(oServer.ChangeList.ValueExists("TlsPort", expectedValue), "TlsPort value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            PortGroupServer oServer = new PortGroupServer();
            const string expectedValue = "Test string";
            oServer.DisplayName = expectedValue;
            Assert.IsTrue(oServer.ChangeList.ValueExists("DisplayName", expectedValue), "DisplayName value get fetch failed");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetPortGroupServers_EmptyResult_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<PortGroupServer> oServers;
            var res = PortGroupServer.GetPortGroupServers(_mockServer, "PortGroupObjectId", out oServers);
            Assert.IsFalse(res.Success, "Calling GetPortGroupServers with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetPortGroupServers_GarbageResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<PortGroupServer> oServers;
            var res = PortGroupServer.GetPortGroupServers(_mockServer, "PortGroupObjectId", out oServers);
            Assert.IsFalse(res.Success, "Calling GetPortGroupServers with garbage results should fail");
            Assert.IsTrue(oServers.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetPortGroupServers_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<PortGroupServer> oServers;
            var res = PortGroupServer.GetPortGroupServers(_mockServer, "PortGroupObjectId", out oServers);
            Assert.IsFalse(res.Success, "Calling GetPortGroupServers with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetPortGroupServers_ZeroCount_Success()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<PortGroupServer> oServers;
            var res = PortGroupServer.GetPortGroupServers(_mockServer, "PortGroupObjectId", out oServers);
            Assert.IsTrue(res.Success, "Calling GetPortGroupServers with ZeroCount failed:" + res);
        }

        [TestMethod]
        public void GetPortGroupServer_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            PortGroupServer oServer;
            var res = PortGroupServer.GetPortGroupServer(out oServer, _mockServer, "ObjectId", "PortGroupObjectId");
            Assert.IsFalse(res.Success, "Calling GetPortGroupServer with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdatePortGroupServer_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.PUT, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test", "Test");
            var res = PortGroupServer.UpdatePortGroupServer(_mockServer, "PortGroupObjectId","ObjectId",oProps);
            Assert.IsFalse(res.Success, "Calling UpdatePortGroupServer with ErrorResponse did not fail");
        }


        [TestMethod]
        public void AddPortGroupServer_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.POST, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test", "Test");
            var res = PortGroupServer.AddPortGroupServer(_mockServer, "PortGroupObjectId", 1,"10.20.30.40","IPV6Address");
            Assert.IsFalse(res.Success, "Calling AddPortGroupServer with ErrorResponse did not fail");
        }

        [TestMethod]
        public void DeletePortGroupServer_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.DELETE, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test", "Test");
            var res = PortGroupServer.DeletePortGroupServer(_mockServer, "ObjectId", "PortGroupObjectId");
            Assert.IsFalse(res.Success, "Calling DeletePortGroupServer with ErrorResponse did not fail");
        }

        [TestMethod]
        public void RefetchPortGroupServerData_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            PortGroupServer oServer = new PortGroupServer(_mockServer,"PortGroupObjectId");
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), true)).Returns(new WebCallResult
                                   {
                                       Success = false,
                                       ResponseText = "error text",
                                       StatusCode = 404
                                   });
            var res = oServer.RefetchPortGroupServerData();
            Assert.IsFalse(res.Success, "Calling RefetchPortGroupServerData with an error response should fail");
        }

        [TestMethod]
        public void Delete_ErrorResponse_Failure()
        {
            PortGroupServer oServer = new PortGroupServer(_mockServer, "PortGroupObjectId");
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.DELETE, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), true)).Returns(new WebCallResult
                                   {
                                       Success = false,
                                       ResponseText = "error text",
                                       StatusCode = 404
                                   });
            var res = oServer.Delete();
            Assert.IsFalse(res.Success, "Calling Delete with an error response should fail");
        }

        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            BaseUnitTests.Reset();
            PortGroupServer oServer = new PortGroupServer(_mockServer, "PortGroupObjectId");
            var res = oServer.Update();
            Assert.IsFalse(res.Success, "Calling Update with no pending chnages should fail");
        }

        [TestMethod]
        public void Update_ErrorRespnse_Failure()
        {
            PortGroupServer oServer = new PortGroupServer(_mockServer, "PortGroupObjectId");
            oServer.DisplayName = "New display name";
            
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            
            var res = oServer.Update();
            Assert.IsFalse(res.Success, "Calling Update with error response should fail");
        }

        #endregion
    }
}
