using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class VmsServerUnitTests : BaseUnitTests 
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            VmsServer oTemp = new VmsServer(null);
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void Constructor_EmptyObjectId_Success()
        {
            VmsServer oTemp = new VmsServer(_mockServer);
            Console.WriteLine(oTemp.ToString());
            Console.WriteLine(oTemp.DumpAllProps());
        }

        [TestMethod]
        public void Constructor_ObjectId_Success()
        {
            VmsServer oTemp = new VmsServer(_mockServer,"ObjectId");
            Console.WriteLine(oTemp.ToString());
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            VmsServer oTemp = new VmsServer();
            Console.WriteLine(oTemp.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_ObjectId_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), true)).Returns(new WebCallResult
                                   {
                                       Success = false,
                                       ResponseText = "error text",
                                       StatusCode = 404
                                   });

            VmsServer oTemp = new VmsServer(_mockServer, "ObjectId");
            Console.WriteLine(oTemp.ToString());
        }


        #endregion


        #region Static Method Tests

        [TestMethod]
        public void GetVmsServers_NullConnectionServer_Failure()
        {
            List<VmsServer> oList;
            WebCallResult res = VmsServer.GetVmsServers(null, out oList);
            Assert.IsFalse(res.Success, "Static call to GetVmsServer did not fail with: null ConnectionServer");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetVmsServers_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<VmsServer> oServers;
            var res = VmsServer.GetVmsServers(_mockServer, out oServers);
            Assert.IsFalse(res.Success, "Calling GetVmsServers with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetVmsServers_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<VmsServer> oServers;
            var res = VmsServer.GetVmsServers(_mockServer, out oServers);
            Assert.IsFalse(res.Success, "Calling GetVmsServers with garbage results should fail");
            Assert.IsTrue(oServers.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetVmsServers_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<VmsServer> oServers;
            var res = VmsServer.GetVmsServers(_mockServer, out oServers);
            Assert.IsFalse(res.Success, "Calling GetVmsServers with ErrorResponse did not fail");
        }

        /// <summary>
        /// There should always be at least one server - so a zero count return is considered an error here.
        /// </summary>
        [TestMethod]
        public void GetVmsServers_ZeroCount_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<VmsServer> oServers;
            var res = VmsServer.GetVmsServers(_mockServer, out oServers);
            Assert.IsFalse(res.Success, "Calling GetVmsServers with ZeroCount should fail");
        }

        #endregion

    }
}
