using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RtpCodecDefUnitTests : BaseUnitTests
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
            BaseUnitTests.Reset();
            RtpCodecDef oTemp = new RtpCodecDef(null);
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void Constructor_EmptyObjectId_Success()
        {
            BaseUnitTests.Reset();
            RtpCodecDef oTemp = new RtpCodecDef(_mockServer);
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void Constructor_ObjectId_Success()
        {
            BaseUnitTests.Reset();
            RtpCodecDef oTemp = new RtpCodecDef(_mockServer,"ObjectId");
            Console.WriteLine(oTemp);
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
            RtpCodecDef oTemp = new RtpCodecDef(_mockServer, "ObjectId");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            BaseUnitTests.Reset();
            RtpCodecDef oTemp = new RtpCodecDef();
            Console.WriteLine(oTemp.ToString());
        }

        #endregion


        #region Static Method Tests

        [TestMethod]
        public void GetRtpCodecDefs_NullConnectionServer_Failure()
        {
            List<RtpCodecDef> oList;
            WebCallResult res = RtpCodecDef.GetRtpCodecDefs(null, out oList);
            Assert.IsFalse(res.Success, "Static call to GetRtpCodecDefs did not fail with: null ConnectionServer");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetRtpCodecDefs_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<RtpCodecDef> oCodecs;
            var res = RtpCodecDef.GetRtpCodecDefs(_mockServer, out oCodecs);
            Assert.IsFalse(res.Success, "Calling GetRtpCodecDefs with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetRtpCodecDefs_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<RtpCodecDef> oCodecs;
            var res = RtpCodecDef.GetRtpCodecDefs(_mockServer, out oCodecs);
            Assert.IsFalse(res.Success, "Calling GetRtpCodecDefs with garbage results should fail");
            Assert.IsTrue(oCodecs.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetRtpCodecDefs_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<RtpCodecDef> oCodecs;
            var res = RtpCodecDef.GetRtpCodecDefs(_mockServer, out oCodecs);
            Assert.IsFalse(res.Success, "Calling GetRtpCodecDefs with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetRtpCodecDefs_ZeroCount_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<RtpCodecDef> oCodecs;
            var res = RtpCodecDef.GetRtpCodecDefs(_mockServer, out oCodecs);
            Assert.IsTrue(res.Success, "Calling GetRtpCodecDefs with ZeroCount failed:" + res);
        }

        #endregion
    }
}
