﻿using System;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PhoneRecordingTest
    {

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private static string _extensionToDial;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

        }

        #endregion


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            PhoneRecording oTemp = new PhoneRecording(null,"1234");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            PhoneRecording oTemp = new PhoneRecording(_connectionServer, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            PhoneRecording oTemp = new PhoneRecording(_connectionServer, "xyz");
        }


        //By default this is not included in the automated run of tests.  Uncomment the "TestMethod()" line and it will be 
        //included - you need to provide an extension of a phone to dial in the properties of the ConnectionCUPIFunctionsTest
        //project.
        //The phone will ring, answer it - you should hear a beep, record a brief message and then press # - it should be played
        //back to you and the call then terminates.
        [TestMethod()]
        public void TestMethod1()
        {
            PhoneRecording oRecording=null;

            try
            {
                oRecording = new PhoneRecording(_connectionServer, _extensionToDial,4);
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Phone connection failed to extension:{0}, error={1}",_extensionToDial,ex));
            }

            WebCallResult res = oRecording.PlayMessageFile("");
            Assert.IsFalse(res.Success, "Playing a message back with no message ID and no stream recorded did not fail");

            res = oRecording.PlayMessageFile("bogus");
            Assert.IsFalse(res.Success, "Playing a message back with invalid message ID did not fail");

            res = oRecording.PlayStreamFile("");
            Assert.IsFalse(res.Success, "Call to play stream file back before something is recorded did not fail.");

            res = oRecording.PlayStreamFile("bogus");
            Assert.IsFalse(res.Success, "Call to play stream file with invalid ID did not fail");

            Assert.IsTrue(oRecording.IsCallConnected(),"Call not connected after class creation");

            res = oRecording.RecordStreamFile();
            Assert.IsTrue(res.Success,"Recording of stream failed:"+res);

            res = oRecording.PlayStreamFile("");
            Assert.IsTrue(res.Success,"Failed to play recording stream back:"+res);

            res = oRecording.PlayMessageFile("");
            Assert.IsFalse(res.Success,"Playing a message back with no message ID did not fail");

            Console.WriteLine(oRecording.ToString());

            oRecording.HangUp();
            Assert.IsFalse(oRecording.IsCallConnected(),"Call not disconnected after hangup");

            oRecording.Dispose();
        }
    }
}