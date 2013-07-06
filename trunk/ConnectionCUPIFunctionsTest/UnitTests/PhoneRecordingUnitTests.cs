using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PhoneRecordingUnitTests : BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static PhoneRecording _mockPhoneRecording;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);

            //setup mock recording construct - need a post and a get to return specific items for that to happen
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
        public void Constructor_NullConnectionServer_Failure()
        {
            PhoneRecording oTemp = new PhoneRecording(null, "1234");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// ArgumentException if blank phone number passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void Constructor_EmptyPhoneNumber_Failure()
        {
            PhoneRecording oTemp = new PhoneRecording(_mockServer, "");
            Console.WriteLine(oTemp);
        }

        #endregion

        [TestMethod]
        public void PhoneRecording_ConstructorWithEmptyResponse_Failure()
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
                Assert.Fail("Phone connection with empty response text should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure:" + ex);
            }
        }

        [TestMethod]
        public void PhoneRecording_ConstructorWithEmptyCallbackId_Failure()
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
                Assert.Fail("Phone connection with empty callback ID should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure:" + ex);
            }

        }

        [TestMethod]
        public void PhoneRecording_ConstructorWithInvalidCallbackId_Failure()
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
                Assert.Fail("Phone connection with invalid callback Id should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected failure:" + ex);
            }
        }

        [TestMethod]
        public void IsCallConnected_ErrorResponse_Failure()
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
        public void RecordStreamFile_ErrorResponse_Failure()
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
        public void RecordStreamFile_NullJsonResponse_Failure()
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
        public void RecordStreamFile_InvalidJsonDictionary_Failure()
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
        public void RecordStreamFile_InvalidLastResultValue_Failure()
        {
            //invalid last result (0 means ok)
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               JsonDictionary =
                                                   new Dictionary<string, object>
                                                       {
                                                           {"lastResult", "11"},
                                                           {"resourceId", "123"}
                                                       }
                                           });

            var res = _mockPhoneRecording.RecordStreamFile();
            Assert.IsFalse(res.Success, "PhoneRecording with invalid lastResult value from server should fail");
        }

        [TestMethod]
        public void RecordStreamFile_BlankResultValue_Failure()
        {
            //invalid resourceid (blank is not ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                        It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                        {
                            Success = true,
                            JsonDictionary = new Dictionary<string, object> { { "lastResult", "0" }, { "resourceId", "" } }
                        });

            var res = _mockPhoneRecording.RecordStreamFile();
            Assert.IsFalse(res.Success, "PhoneRecording with invalid resourceId value from server should fail");
        }


        [TestMethod]
        public void PlayStreamFile_ErrorResult_Failure()
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
        public void PlayStreamFile_InvalidLastResult_Failure()
        {
            //invalid last result (0 means ok)
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               JsonDictionary = new Dictionary<string, object> {{"lastResult", "11"}}
                                           });

            var res = _mockPhoneRecording.PlayStreamFile("streamid");
            Assert.IsFalse(res.Success, "PlayStreamFile with invalid lastResult value from server should fail");
        }

        [TestMethod]
        public void PlayStreamFile_BlankLastResult_Failure()
        {
            //invalid resourceid (blank is not ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                        It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                        {
                            Success = true,
                            JsonDictionary = new Dictionary<string, object> { { "lastResult", null } }
                        });

            var res = _mockPhoneRecording.PlayStreamFile("streamid");
            Assert.IsFalse(res.Success, "PlayStreamFile with null lastResult value from server should fail");
        }


        [TestMethod]
        public void PlayMessageFile_InvalidLastResultValue_Failure()
        {
            //invalid last result (0 means ok)
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               JsonDictionary = new Dictionary<string, object> {{"lastResult", "11"}}
                                           });

            var res = _mockPhoneRecording.PlayMessageFile("messageid");
            Assert.IsFalse(res.Success, "PlayMessageFile with invalid lastResult value from server should fail");
        }
        
        [TestMethod]
        public void PlayMessageFile_BlankLastResultValue_Failure()
        {
        //invalid resourceid (blank is not ok)
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                        It.IsAny<Dictionary<string, string>>())).Returns(new WebCallResult
                        {
                            Success = true,
                            JsonDictionary = new Dictionary<string, object> { { "lastResult", null } }
                        });

            var res = _mockPhoneRecording.PlayMessageFile("messageid");
            Assert.IsFalse(res.Success, "PlayMessageFile with null lastResult value from server should fail");
        }

        [TestMethod]
        public void PlayMessageFile_Success()
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