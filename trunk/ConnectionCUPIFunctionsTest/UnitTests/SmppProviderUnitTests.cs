using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for SmppProviderIntegrationTests and is intended
    ///to contain all SmppProviderIntegrationTests Unit Tests
    ///</summary>
    [TestClass]
    public class SmppProviderUnitTests : BaseUnitTests
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
        /// Make sure an ArgumentException is thrown if an empty objectID is passed in
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            SmppProvider oTest = new SmppProvider(null, "");
            Console.WriteLine(oTest);
        }

        #endregion


        [TestMethod]
        public void SmppProvider_Constructor_ErrorResponse_Faiure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            try
            {
                SmppProvider oProvider = new SmppProvider(_mockServer);
                Assert.Fail("Getting invalid response text back from server did not result in construciton failure");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure on class creation:"+ex);
            }
        }

        [TestMethod]
        public void GetSmppProviders_ErrorResponse_Failure()
        {

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            List<SmppProvider> oProviders;
            var res = SmppProvider.GetSmppProviders(_mockServer, out oProviders, 1, 10);
            Assert.IsFalse(res.Success, "Forcing error response from server did not result in call failure");

            }

        [TestMethod]
        public void GetSmppProviders_EmptyResults_Success()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });
            List<SmppProvider> oProviders;
            var res = SmppProvider.GetSmppProviders(_mockServer, out oProviders, 1, 10);
            Assert.IsTrue(res.Success, "Forcing empty result text from server should not fail:"+res);
            Assert.IsTrue(oProviders.Count==0,"Empty response from server should result in 0 elements returned:"+oProviders.Count);

            }

        [TestMethod]
        public void GetSmppProviders_GarbageResults_Failure()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result",
                                      TotalObjectCount = 1
                                  });
            List<SmppProvider> oProviders;
            var res = SmppProvider.GetSmppProviders(_mockServer, out oProviders, 1, 10);
            Assert.IsFalse(res.Success, "Forcing invalid result text from server should fail:");
            Assert.IsTrue(oProviders.Count == 0, "Invalid response text from server should result in 0 elements returned:" + oProviders.Count);
}

        [TestMethod]
        public void GetSmppProviders_ZeroCountResponse_Success()
        {
            //0 count response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result",
                                      TotalObjectCount = 0
                                  });
            List<SmppProvider> oProviders;
            var res = SmppProvider.GetSmppProviders(_mockServer, out oProviders, 1, 10);
            Assert.IsTrue (res.Success, "Forcing zero count response from server should not fail:"+res);
            Assert.IsTrue(oProviders.Count == 0, "Invalid response text from server should result in 0 elements returned:" + oProviders.Count);
        }


    }
}
