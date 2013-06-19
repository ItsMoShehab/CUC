using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for SmppProviderTest and is intended
    ///to contain all SmppProviderTest Unit Tests
    ///</summary>
    [TestClass]
    public class SmppProviderTest
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
                throw new Exception("Unable to attach to Connection server to start SmppProvider test:" + ex.Message);
            }

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

        #endregion
        

        #region Constructor Tests

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if the objectId for an SMPP provider is not found
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreation_Failure()
        {
            SmppProvider oTest = new SmppProvider(_connectionServer, "aaa");
            Console.WriteLine(oTest);
        }


        /// <summary>
        /// Make sure an ArgumentException is thrown if an empty objectID is passed in
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreation_Failure2()
        {
            SmppProvider oTest = new SmppProvider(null, "");
            Console.WriteLine(oTest);
        }

        #endregion


        /// <summary>
        ///A test for getting and listing PhoneSystems
        ///</summary>
        [TestMethod]
        public void SmppProvider_FetchTest()
        {
            List<SmppProvider> oProviders;

            WebCallResult res = SmppProvider.GetSmppProviders(null, out oProviders,1,5);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

            res = SmppProvider.GetSmppProviders(_connectionServer, out oProviders,1,5,null);
            Assert.IsTrue(res.Success, "Failed to fetch SmppProviders");
            Assert.IsTrue(oProviders.Count>0,"No SMPP providers returned");

            string strObjectId="";
            foreach (SmppProvider oTemp in oProviders)
            {
                Console.WriteLine(oTemp.ToString());

                //check IUnityDisplayInterface properties
                Console.WriteLine(oTemp.SelectionDisplayString);
                Console.WriteLine(oTemp.UniqueIdentifier);
                strObjectId = oTemp.ObjectId;
            }

            try
            {
                SmppProvider oNewProvider = new SmppProvider(_connectionServer, strObjectId);
                Console.WriteLine(oNewProvider);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unable to fetch SMPP provider by valid ObjectId:"+ex);
            }

            

        }

        [TestMethod]
        public void SmppProvider_CreateEmpty()
        {
            try
            {
                SmppProvider oProvider = new SmppProvider(_connectionServer);
                Console.WriteLine(oProvider);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create an empty ObjectInstance with no ObjectId:"+ex);
            }
        }

        [TestMethod]
        public void HarnessTest_GetSmppProvider()
        {

            //error response
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
        public void HarnessTest_GetSmppProviders()
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


            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            res = SmppProvider.GetSmppProviders(_mockServer, out oProviders, 1, 10);
            Assert.IsTrue(res.Success, "Forcing empty result text from server should not fail:"+res);
            Assert.IsTrue(oProviders.Count==0,"Empty response from server should result in 0 elements returned:"+oProviders.Count);


            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result",
                                      TotalObjectCount = 1
                                  });

            res = SmppProvider.GetSmppProviders(_mockServer, out oProviders, 1, 10);
            Assert.IsFalse(res.Success, "Forcing invalid result text from server should fail:");
            Assert.IsTrue(oProviders.Count == 0, "Invalid response text from server should result in 0 elements returned:" + oProviders.Count);

            //0 count response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result",
                                      TotalObjectCount = 0
                                  });
            res = SmppProvider.GetSmppProviders(_mockServer, out oProviders, 1, 10);
            Assert.IsTrue (res.Success, "Forcing zero count response from server should not fail:"+res);
            Assert.IsTrue(oProviders.Count == 0, "Invalid response text from server should result in 0 elements returned:" + oProviders.Count);
        }


    }
}
