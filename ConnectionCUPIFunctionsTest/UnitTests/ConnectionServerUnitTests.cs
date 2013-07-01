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

        /// <summary>
        /// Used for making sure json parsing errors raise events properly
        /// </summary>
        private static int _errorsRaised = 0;

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
        public void ClassCreationFailure()
        {
            //invalid login value - empty server name
            ConnectionServerRest oTestServer = new ConnectionServerRest(new RestTransportFunctions(), "", "login", "Pw");
            Console.WriteLine(oTestServer);
        }

        /// <summary>
        /// Make sure an Exception is thrown if 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailureBadLogin()
        {
            ConnectionServerRest oTestServer = new ConnectionServerRest(new RestTransportFunctions(),  "badservername", "badloginname", "badpassword");
            Console.WriteLine(oTestServer);
        }

        #endregion


        [TestMethod]
        public void ConstructorsWithDifferentTransports()
        {
            ConnectionServerRest oServer;
            
            oServer= new ConnectionServerRest(null);
            Assert.IsFalse(string.IsNullOrEmpty(oServer.Version.ToString()),"No version string for default constructor");

            oServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsFalse(string.IsNullOrEmpty(oServer.Version.ToString()), "No version string for REstTransportFunctions constructor");
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
        public void ParseVersionString_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());

            Assert.IsFalse(oTempServer.ParseVersionString(""), "Empty string");
            Assert.IsFalse(oTempServer.ParseVersionString("1"), "Invalid number of version parts");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2"), "Invalid number of version parts");
            Assert.IsFalse(oTempServer.ParseVersionString("a.2.3"), "Invalid major");
            Assert.IsFalse(oTempServer.ParseVersionString("1.a.3"), "Invalid minor");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.a"), "Invalid rev");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.3.a"), "Invalid build");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.3.4ES4ES"), "Invalid ES");

            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3"), "Failed to parse legal version string with 3 elements");

            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3.4ES5"),"Failed to parse legal version string");
            Assert.AreEqual(oTempServer.Version.Major, 1, "Major is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Minor, 2, "Minor is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Rev, 3, "Rev is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Build,4,"Build is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Es, 5, "ES is not parsed out correctly");

        }

        [TestMethod]
        public void HarnessTest_Constructor()
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


        public class TestClass
        {
            public string URI { get; set; }
            public System.Int16 Int16 { get; set; }
            public System.Int32 Int32 { get; set; }
            public System.Int64 Int64 { get; set; }
            public string String { get; set; }
            public bool Boolean { get; set; }
            public DateTime DateTime { get; set; }
            public Cisco.UnityConnection.RestFunctions.SubscriberConversationTui SubscriberConversationTui { get; set; }
            public Cisco.UnityConnection.RestFunctions.TransferOptionTypes TransferOptionType { get; set; }
            public Cisco.UnityConnection.RestFunctions.GreetingTypes GreetingType { get; set; }
            public Cisco.UnityConnection.RestFunctions.ConversationNames ConversationName { get; set; }
            public Cisco.UnityConnection.RestFunctions.MessageType MessageType { get; set; }
            public Cisco.UnityConnection.RestFunctions.SensitivityType SensitivityType { get; set; }
            public Cisco.UnityConnection.RestFunctions.PriorityType PriorityType { get; set; }
            public Cisco.UnityConnection.RestFunctions.SipTransportEnum Bogus { get; set; }
        }
    }
}
