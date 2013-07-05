using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PhoneRecordingIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static string _extensionToDial;
        private static PhoneRecording _recording = null;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
            Settings mySettings = new Settings();
            _extensionToDial = mySettings.ExtensionToDial;

            if (string.IsNullOrEmpty(_extensionToDial))
            {
                Assert.Fail("No extension defined to run phone recording tests");
                return;
            }

            try
            {
                _recording = new PhoneRecording(_connectionServer, _extensionToDial, 6);
            }
            catch (UnityConnectionRestException ex)
            {
                Assert.Fail("Phone connection failed to extension:{0}, error={1}", _extensionToDial, ex.WebCallResult);
            }
            Assert.IsTrue(_recording.IsCallConnected(), "Call not connected after class creation");
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_recording != null)
            {
                _recording.HangUp();
                Assert.IsFalse(_recording.IsCallConnected(), "Call not disconnected after hangup");

                _recording.Dispose();
            }
        }

        #endregion


        //The phone will ring, answer it - you should hear a beep, record a brief message and then press # - it should be played
        //back to you and the call then terminates.
        [TestMethod]
        public void PlayMessageFile_NoMessageId_Failure()
        {
            WebCallResult res = _recording.PlayMessageFile();
            Assert.IsFalse(res.Success, "Playing a message back with no message ID and no stream recorded did not fail");
        }

        [TestMethod]
        public void PlayMessageFile_InvalidMessageId_Failure()
        {

            var res = _recording.PlayMessageFile("bogus");
            Assert.IsFalse(res.Success, "Playing a message back with invalid message ID did not fail");

         }

        [TestMethod]
        public void PlayStreamFile_NoRecording_Failure()
        {
            var res = _recording.PlayStreamFile();
            Assert.IsFalse(res.Success, "Call to play stream file back before something is recorded did not fail.");

            }

        [TestMethod]
        public void PlayMessageFile_InvalidStreamId_Failure()
        {
            var res = _recording.PlayStreamFile("bogus");
            Assert.IsFalse(res.Success, "Call to play stream file with invalid ID did not fail");

            }

        [TestMethod]
        public void PlayMessageFile_RecordAndPlaybackStream_Success()
        {
            var res = _recording.RecordStreamFile();
            Assert.IsTrue(res.Success, "Recording of stream failed:" + res);

            res = _recording.PlayStreamFile();
            Assert.IsTrue(res.Success, "Failed to play recording stream back:" + res);

            Console.WriteLine(_recording.ToString());
        }

        [TestMethod]
        public void PhoneRecording_ConstructorWithInvalidPhoneNumber_Failure()
        {
            try
            {
                new PhoneRecording(_connectionServer, "abcd");
                Assert.Fail("Phone connection to invalid extension should fail");
            }
            catch (UnityConnectionRestException ex)
            {
                Console.WriteLine("Expected failure:" + ex);
            }
        }

    }
}