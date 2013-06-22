﻿using System;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Cluster is a simple read only class that has very little logic other than it's cosntrctor
    /// </summary>
    [TestClass]
    public class ClusterIntegrationTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion


        #region Additional test attributes
        // 
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
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
                throw new Exception("Unable to attach to Connection server to start Cluster test:" + ex.Message);
            }

        }

        #endregion


        #region Class Construction Errors

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            Cluster oTest = new Cluster(null);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an empty Connection server is passed in
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            Cluster oTest = new Cluster(new ConnectionServerRest(new RestTransportFunctions()));
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void FetchTests()
        {
            Cluster oCluster = new Cluster(_connectionServer);

            Console.WriteLine(oCluster.ToString());

            Assert.IsTrue(oCluster.Servers.Count>0,"There should always be at least 1 server in a cluster");

            foreach (var oServer in oCluster.Servers)
            {
                Console.WriteLine(oServer.ToString());
                Console.WriteLine(oServer.DumpAllProps());
                
            }
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void HarnessTest_GetServers()
        {
            var oTestTransport = new Mock<IConnectionRestCalls>();

            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                });

            ConnectionServerRest oServer = new ConnectionServerRest(oTestTransport.Object, "test", "test", "test", false);

            Cluster oCluster=null;

            //empty results
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            try
            {
                oCluster = new Cluster(oServer);
                Assert.Fail("Creating cluster with empty response text should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("expected error:"+ex);
            }

            try
            {
                Console.WriteLine(oCluster.Servers.Count);
                Assert.Fail("Getting server count with invalid cluster fetch should fail");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Expected Failure getting servers count:"+ex);
            }

            //error response
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            try
            {
                oCluster = new Cluster(oServer);
                Assert.Fail("Creating cluster with error response should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("expected error:" + ex);
            }
        }

        #endregion
    }
}