using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for ConnectionServerUnitTests and is intended
    ///to contain all ConnectionServerUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class ConnectionServerUnitTests : BaseUnitTests
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


        #region Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a blank server name is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_BlankServerName_Failure()
        {
            //invalid login value - empty server name
            ConnectionServerRest oTestServer = new ConnectionServerRest(new RestTransportFunctions(), "", "login", "Pw");
            Console.WriteLine(oTestServer);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidLogin_Failure()
        {
            ConnectionServerRest oTestServer = new ConnectionServerRest(new RestTransportFunctions(),  "badservername", "badloginname", "badpassword");
            Console.WriteLine(oTestServer);
        }

        #endregion


        [TestMethod]
        public void ConnectionServerRestConstructor_NullTransport_NoEmptyVersionString()
        {
            ConnectionServerRest oServer = new ConnectionServerRest(null);
            Assert.IsFalse(string.IsNullOrEmpty(oServer.Version.ToString()), "No version string for default constructor");
        }

        [TestMethod]
        public void ConnectionServerRestConstructor_DefaultConstructor_NoEmptyVersionString()
        {
            ConnectionServerRest oServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsFalse(string.IsNullOrEmpty(oServer.Version.ToString()), "No version string for RestTransportFunctions constructor");
        }


        [TestMethod]
        public void ConnectionVersion()
        {
            ConnectionVersion oVersion=null;
            try
            {
                oVersion = new ConnectionVersion(1,2,3,4,5);
            }
            catch (Exception ex)
            {
                Assert.Fail("Construction of version failed:"+ex);
            }
                
            string strVersion = oVersion.ToString();
            Assert.IsTrue(strVersion.Contains("ES"),"Version with ES value does not produce ES in string:"+oVersion);

            try
            {
                oVersion = new ConnectionVersion(1, 2, 3, 4,0);
            }
            catch (Exception ex)
            {
                Assert.Fail("Construction of version failed:" + ex);
            }

            strVersion = oVersion.ToString();
            Assert.IsFalse(strVersion.Contains("ES"), "Version without ES value produce ES in string:" + oVersion);
        }

        /// <summary>
        /// Create a new instance of a Connection server object without logging in
        /// </summary>
        [TestMethod]
        public void ConstructorPlain()
        {
            ConnectionServerRest oTestServer = new ConnectionServerRest(new RestTransportFunctions());
            Console.WriteLine(oTestServer);
        }


        /// <summary>
        ///A test for ParseVersionString - use a seperate instance of the ConnectionServer object for this so as not to 
        /// corrupt other tests.
        ///</summary>
        [TestMethod]
        public void ParseVersionString_EmptyString_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());

            Assert.IsFalse(oTempServer.ParseVersionString(""), "Empty string");
        }

        [TestMethod]
        public void ParseVersionString_InvalidParts1_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());

            Assert.IsFalse(oTempServer.ParseVersionString("1"), "Invalid number of version parts");
            }

        [TestMethod]
        public void ParseVersionString_InvalidParts2_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsFalse(oTempServer.ParseVersionString("1.2"), "Invalid number of version parts");
            }

        [TestMethod]
        public void ParseVersionString_InvalidMajorVersionValue_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsFalse(oTempServer.ParseVersionString("a.2.3"), "Invalid major");
            }

        [TestMethod]
        public void ParseVersionString_InvalidMinorVersionValue_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsFalse(oTempServer.ParseVersionString("1.a.3"), "Invalid minor");
            }

        [TestMethod]
        public void ParseVersionString_InvalidRevValue_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.a"), "Invalid rev");
            }

        [TestMethod]
        public void ParseVersionString_InvaliudBuildValue_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.3.a"), "Invalid build");
            }

        [TestMethod]
        public void ParseVersionString_InvalidEsValue_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.3.4ES4ES"), "Invalid ES");
            }

        [TestMethod]
        public void ParseVersionString_Legal3PartValue_Success()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3"), "Failed to parse legal version string with 3 elements");
        }

        [TestMethod]
        public void ParseVersionString_LegalValueWithEs_Success()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3.4ES5"), "Failed to parse legal version string");
        }

        [TestMethod]
        public void ParseVersionString_CheckMajorValue_Success()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3.4ES5"), "Failed to parse legal version string");
            Assert.AreEqual(oTempServer.Version.Major, 1, "Major is not parsed out correctly");
        }

        [TestMethod]
        public void ParseVersionString_CheckMinorValue_Success()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3.4ES5"), "Failed to parse legal version string");
            Assert.AreEqual(oTempServer.Version.Minor, 2, "Minor is not parsed out correctly");
        }

        [TestMethod]
        public void ParseVersionString_CheckRevValue_Success()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3.4ES5"), "Failed to parse legal version string");
            Assert.AreEqual(oTempServer.Version.Rev, 3, "Rev is not parsed out correctly");
        }

        [TestMethod]
        public void ParseVersionString_CheckBuildValue_Success()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3.4ES5"), "Failed to parse legal version string");
            Assert.AreEqual(oTempServer.Version.Build, 4, "Build is not parsed out correctly");
        }

        [TestMethod]
        public void ParseVersionString_CheckEsValue_Success()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3.4ES5"), "Failed to parse legal version string");
            Assert.AreEqual(oTempServer.Version.Es, 5, "ES is not parsed out correctly");
        }

        [TestMethod]
        public void ConnectionServerRest_Constructor_EmptyResults_Failure()
        {
           //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            try
            {
                ConnectionServerRest oServer = new ConnectionServerRest(_mockTransport.Object, "EmptyResultText", "login","password", false);
                Assert.Fail("Creating server with empty results text for version check did not fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected creation failure:"+ex);
            }

            }

        [TestMethod]
        public void ConnectionServerRest_Constructor_ErrorResponse_Failure()
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
                ConnectionServerRest oServer = new ConnectionServerRest(_mockTransport.Object, "ErrorResponse", "login", "password", false);
                Assert.Fail("Creating server with error response for version check did not fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected creation failure:" + ex);
            }

            }

        [TestMethod]
        public void ConnectionServerRest_Constructor_GarbageResponse_Failure()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result",
                                      TotalObjectCount = 1
                                  });
            try
            {
                ConnectionServerRest oServer = new ConnectionServerRest(_mockTransport.Object, "InvalidResultText", "login", "password", false);
                Assert.Fail("Creating server with invalid results text for version check did not fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected creation failure:" + ex);
            }
        }

    }
}
