using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PhoneRecordingTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //Mock transport interface - 
        private static Mock<IConnectionRestCalls> _mockTransport;

        //Mock REST server
        private static ConnectionServerRest _mockServer;

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        private static PhoneRecording _mockPhoneRecording;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private static string _extensionToDial;

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
            _extensionToDial = mySettings.ExtensionToDial;
            Thread.Sleep(300);
            try
            {
                _connectionServer = new ConnectionServerRest(new RestTransportFunctions(), mySettings.ConnectionServer,
                                                             mySettings.ConnectionLogin,
                                                             mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start PhoneRecording test:" + ex.Message);
            }

            //setup mock server interface 
            _mockTransport = new Mock<IConnectionRestCalls>();

            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                                           });

            _mockServer = new ConnectionServerRest(_mockTransport.Object, "test", "test", "test", false);

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.POST, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = "vmrest/calls/123"
                                       });

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           JsonDictionary = new Dictionary<string, object>
                                                       {
                                                           {"connected", "true"},
                                                       },
                                       });

            
            try
            {
                _mockPhoneRecording = new PhoneRecording(_mockServer, "1234");
            }
            catch (Exception ex)
            {
                Assert.Fail("Phone connection with valid call id should not fail:" + ex);
            }
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void ClassCreationFailure()
        {
            PhoneRecording oTemp = new PhoneRecording(null, "1234");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// ArgumentException if blank phone number passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void ClassCreationFailure2()
        {
            PhoneRecording oTemp = new PhoneRecording(_connectionServer, "");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Throw UnityConnectionRestException if an invalid phone number passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof (UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            PhoneRecording oTemp = new PhoneRecording(_connectionServer, "xyz");
            Console.WriteLine(oTemp);
        }

        #endregion


        //By default this is not included in the automated run of tests.  Uncomment the "TestMethod()" line and it will be 
        //included - you need to provide an extension of a phone to dial in the properties of the ConnectionCUPIFunctionsTest
        //project.
        //The phone will ring, answer it - you should hear a beep, record a brief message and then press # - it should be played
        //back to you and the call then terminates.
        [TestMethod]
        public void RecordingTest_Live()
        {
            PhoneRecording oRecording = null;
            try
            {
                oRecording = new PhoneRecording(_connectionServer, _extensionToDial, 6);
            }
            catch (UnityConnectionRestException ex)
            {
                   Assert.Fail("Phone connection failed to extension:{0}, error={1}", _extensionToDial, ex.WebCallResult);
            }

            WebCallResult res = oRecording.PlayMessageFile();
            Assert.IsFalse(res.Success, "Playing a message back with no message ID and no stream recorded did not fail");

            res = oRecording.PlayMessageFile("bogus");
            Assert.IsFalse(res.Success, "Playing a message back with invalid message ID did not fail");

            res = oRecording.PlayStreamFile();
            Assert.IsFalse(res.Success, "Call to play stream file back before something is recorded did not fail.");

            res = oRecording.PlayStreamFile("bogus");
            Assert.IsFalse(res.Success, "Call to play stream file with invalid ID did not fail");

            Assert.IsTrue(oRecording.IsCallConnected(), "Call not connected after class creation");

            res = oRecording.RecordStreamFile();
            Assert.IsTrue(res.Success, "Recording of stream failed:" + res);

            res = oRecording.PlayStreamFile();
            Assert.IsTrue(res.Success, "Failed to play recording stream back:" + res);

            res = oRecording.PlayMessageFile();
            Assert.IsFalse(res.Success, "Playing a message back with no message ID did not fail");

            Console.WriteLine(oRecording.ToString());

            oRecording.HangUp();
            Assert.IsFalse(oRecording.IsCallConnected(), "Call not disconnected after hangup");

            oRecording.Dispose();
        }

        [TestMethod]
        public void RecordingDialFail()
        {
            PhoneRecording oRecording = null;
            try
            {
                oRecording = new PhoneRecording(_connectionServer, "abcd");
                Assert.Fail("Phone connection to invalid extension should fail");
            }
            catch (UnityConnectionRestException ex)
            {
                Console.WriteLine("Expected failure:" + ex);
            }
        }

        [TestMethod]
        public void RecordingConstructor_Harness_Empty()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            try
            {
                PhoneRecording oPhoneRecording = new PhoneRecording(_mockServer, "1234");
                Assert.Fail("Phone connection with invalid ConnectionServerRest should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure:" + ex);
            }
        }

        [TestMethod]
        public void RecordingConstructor_Harness_NoCallNumber()
        {

            //result with no call number
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = "vmrest/calls/"
                                           });

            try
            {
                PhoneRecording oPhoneRecording = new PhoneRecording(_mockServer, "1234");
                Assert.Fail("Phone connection with invalid ConnectionServerRest should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure:" + ex);
            }

        }

        [TestMethod]
        public void RecordingConstructor_Harness_InvalidCallNumber()
        {
            //result with invalid call
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = "vmrest/calls/xxx"
                                           });

            try
            {
                PhoneRecording oPhoneRecording = new PhoneRecording(_mockServer, "1234");
                Assert.Fail("Phone connection with invalid ConnectionServerRest should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure:" + ex);
            }
        }

        [TestMethod]
        public void IsCallConnected_Harness_ErrorResponse()
        {
            

            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                           {
                                               Success = false,
                                           });

            Assert.IsFalse(_mockPhoneRecording.IsCallConnected(),
                           "Phone is connected should return false with error response");
        }


        [TestMethod]
        public void RecordStreamFile_Harness_ErrorResponse()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<Dictionary<string,string>>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = _mockPhoneRecording.RecordStreamFile();
            Assert.IsFalse(res.Success,"PhoneRecording with error response from server should fail");
        }

        [TestMethod]
        public void RecordStreamFile_Harness_EmptyJsonDictionary()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                    });

            var res = _mockPhoneRecording.RecordStreamFile();
            Assert.IsFalse(res.Success, "PhoneRecording with null Json dictionary response from server should fail");
        }


        [TestMethod]
        public void RecordStreamFile_Harness_InvalidJsonDictionary()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        JsonDictionary = new Dictionary<string, object>()
                                    });

            var res = _mockPhoneRecording.RecordStreamFile();
            Assert.IsFalse(res.Success, "PhoneRecording with invalid JsonDictionary response from server should fail");
        }

        [TestMethod]
        public void RecordStreamFile_Harness_InvalidDictionaryValues()
        {
            //invalid last result (0 means ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        JsonDictionary = new Dictionary<string, object>{{"lastResult","11"},{"resourceId","123"}}
                                    });

            var res = _mockPhoneRecording.RecordStreamFile();
            Assert.IsFalse(res.Success, "PhoneRecording with invalid lastResult value from server should fail");

            //invalid resourceid (blank is not ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                        It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                        {
                            Success = true,
                            JsonDictionary = new Dictionary<string, object> { { "lastResult", "0" }, { "resourceId", "" } }
                        });

            res = _mockPhoneRecording.RecordStreamFile();
            Assert.IsFalse(res.Success, "PhoneRecording with invalid resourceId value from server should fail");
        }


        [TestMethod]
        public void PlayStreamFile_Harness_ErrorResponse()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                    });

            var res = _mockPhoneRecording.PlayStreamFile("streamid");
            Assert.IsFalse(res.Success, "PlayStreamFile with error response from server should fail");
        }


        [TestMethod]
        public void PlayStreamFile_Harness_InvalidDictionaryValues()
        {
            //invalid last result (0 means ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        JsonDictionary = new Dictionary<string, object> { { "lastResult", "11" }}
                                    });

            var res = _mockPhoneRecording.PlayStreamFile("streamid");
            Assert.IsFalse(res.Success, "PlayStreamFile with invalid lastResult value from server should fail");

            //invalid resourceid (blank is not ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                        It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                        {
                            Success = true,
                            JsonDictionary = new Dictionary<string, object> { { "lastResult", null } }
                        });

            res = _mockPhoneRecording.PlayStreamFile("streamid");
            Assert.IsFalse(res.Success, "PlayStreamFile with null lastResult value from server should fail");
        }


        [TestMethod]
        public void PlayMessageFile_Harness_InvalidDictionaryValues()
        {
            //invalid last result (0 means ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        JsonDictionary = new Dictionary<string, object> { { "lastResult", "11" } }
                                    });

            var res = _mockPhoneRecording.PlayMessageFile("messageid");
            Assert.IsFalse(res.Success, "PlayMessageFile with invalid lastResult value from server should fail");

            //invalid resourceid (blank is not ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                        It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                        {
                            Success = true,
                            JsonDictionary = new Dictionary<string, object> { { "lastResult", null } }
                        });

            res = _mockPhoneRecording.PlayMessageFile("messageid");
            Assert.IsFalse(res.Success, "PlayMessageFile with null lastResult value from server should fail");
        }

        [TestMethod]
        public void PlayMessageFile_Harness_Success()
        {
            //invalid last result (0 means ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        JsonDictionary = new Dictionary<string, object> { { "lastResult", "0" } }
                                    });

            var res = _mockPhoneRecording.PlayMessageFile("messageid");
            Assert.IsTrue(res.Success, "PlayMessageFile with valid lastResult value from server failed:"+res);
        }
    }



}