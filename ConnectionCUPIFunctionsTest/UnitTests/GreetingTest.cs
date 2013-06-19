using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for GreetingTest and is intended
    ///to contain all GreetingTest Unit Tests
    ///</summary>
    [TestClass]
    public class GreetingTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;
        
        //class wide call handler to use for various greeting tests - this will get filled with the opening greeting call handler.
        private static CallHandler _callHandler;

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
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start Greeting test:" + ex.Message);
            }

            //get a call handler to use for various tests in this class - the opening greeting should always be there
            WebCallResult res = CallHandler.GetCallHandler(out _callHandler, _connectionServer, "", "opening greeting");
            if (res.Success == false | _callHandler == null)
            {
                throw new Exception("Unable to get opening greeting call handler for test functions in GreetingTest");
            }
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticCallFailures_GetGreetingStreamFile()
        {
            GreetingStreamFile oStream;
            //static method tests
            var res = GreetingStreamFile.GetGreetingStreamFile(null, _callHandler.ObjectId, GreetingTypes.Standard, 1033, out oStream);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, "", GreetingTypes.Standard, 1033, out oStream);
            Assert.IsFalse(res.Success, "Empty call handler ID param should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, "aaa", GreetingTypes.Standard, 1033, out oStream);
            Assert.IsFalse(res.Success, "Invalid Call handler Id should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, _callHandler.ObjectId, GreetingTypes.Invalid, 1033, out oStream);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, _callHandler.ObjectId, GreetingTypes.Standard, 10, out oStream);
            Assert.IsFalse(res.Success, "Invalid language code should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, _callHandler.ObjectId, GreetingTypes.Standard, 1033, out oStream);
            Assert.IsTrue(res.Success, "Failed fetching stream file from GreetingStreamFile static call");
        }


        [TestMethod]
        public void StaticCallFailures_GetGreetingWavFile()
        {
            //static calls to GetGreetingWAVFile
            var res = GreetingStreamFile.GetGreetingWavFile(null, "test.wav", "streamfilename");
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "", "StreamFileName");
            Assert.IsFalse(res.Success, "Empty target WAV file path should fail");

            res = GreetingStreamFile.GetGreetingWavFile(_connectionServer, "test.wav", "");
            Assert.IsFalse(res.Success, "Empty stream file name should fail");
        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetGreetings()
        {
            List<Greeting> oGreetings;

            //Static calls for GetGreetings
            WebCallResult res = Greeting.GetGreetings(null, _callHandler.ObjectId, out oGreetings);
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.GetGreetings(_connectionServer, "", out oGreetings);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

            res = Greeting.GetGreetings(_connectionServer, "aaa", out oGreetings);
            Assert.IsFalse(res.Success, "Invalid call handler ObjectId should fail");

            res = Greeting.GetGreetings(_connectionServer, _callHandler.ObjectId, out oGreetings);
            Assert.IsTrue(res.Success, "Failed to fetch greetings in GetGreetings_Failure:" + res);
            Assert.IsNotNull(oGreetings, "Null greetings colleciton returned");
            Assert.IsTrue(oGreetings.Count > 1, "Empty list of greetings returned");

        }


        /// <summary>
        /// exercise greeting rules failure conditions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateGreeting()
        {
            //static calls for updateGreeting
            WebCallResult res = Greeting.UpdateGreeting(_connectionServer, _callHandler.ObjectId, GreetingTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Empty parameter list param should fail");

            res = Greeting.UpdateGreeting(null, _callHandler.ObjectId, GreetingTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.UpdateGreeting(_connectionServer, "", GreetingTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

            res = Greeting.UpdateGreeting(_connectionServer, _callHandler.ObjectId, GreetingTypes.Invalid, null);
            Assert.IsFalse(res.Success, "Invalid Greeting type name should fail");


        }


        /// <summary>
        /// exercise greeting rules failure conditions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_SetGreetingWavFile()
        {
            //SetGreetingWavFile
            WebCallResult res = Greeting.SetGreetingWavFile(null, "Dummy.wav", _callHandler.ObjectId, GreetingTypes.Alternate, 1033, true);
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.SetGreetingWavFile(_connectionServer, "bogus.wav", _callHandler.ObjectId, GreetingTypes.Alternate, 1033, true);
            Assert.IsFalse(res.Success, "Invalid WAV file target should fail");

            res = Greeting.SetGreetingWavFile(_connectionServer, "Dummy.wav", "", GreetingTypes.Alternate, 1033, true);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

            res = Greeting.SetGreetingWavFile(_connectionServer, "Dummy.wav", "aaa", GreetingTypes.Alternate, 1033, true);
            Assert.IsFalse(res.Success, "Invalid call handler ObjectId param should fail");

            res = Greeting.SetGreetingWavFile(_connectionServer, "Dummy.wav", _callHandler.ObjectId, GreetingTypes.Invalid, 1033, true);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

            res = Greeting.SetGreetingWavFile(_connectionServer, "Dummy.wav", _callHandler.ObjectId, GreetingTypes.Alternate, 10, true);
            Assert.IsFalse(res.Success, "Invalid language code should fail");


            //static calls to SetGreetingWAVFiles with invalid params
            res = GreetingStreamFile.SetGreetingWavFile(null, "aaa", GreetingTypes.Alternate, 1033, "Dummy.wav");
            Assert.IsFalse(res.Success, "Null ConnectionServerRest param should fail");

            res = GreetingStreamFile.SetGreetingWavFile(_connectionServer, "", GreetingTypes.Invalid, 1033, "Dummy.wav");
            Assert.IsFalse(res.Success, "Empty CallHandler ObjectId or greeting type should fail");

            List<GreetingStreamFile> oStreams;
            res = GreetingStreamFile.GetGreetingStreamFiles(null, _callHandler.ObjectId, GreetingTypes.Alternate, out oStreams);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

        }

        /// <summary>
        /// exercise greeting rules failure conditions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateGreetingEnabledStatus()
        {
            //greeting enabled status
            WebCallResult res = Greeting.UpdateGreetingEnabledStatus(null, _callHandler.ObjectId, GreetingTypes.Alternate, true, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, "", GreetingTypes.Alternate, true, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, "aaa", GreetingTypes.Alternate, true, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Invalid call handler ObjectId should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, _callHandler.ObjectId, GreetingTypes.Invalid, true, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Invalid greeting rule name should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, _callHandler.ObjectId, GreetingTypes.Alternate, true, DateTime.Now.AddDays(-1));
            Assert.IsFalse(res.Success, "Enabling greeting to TRUE with a date in the past should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, _callHandler.ObjectId, GreetingTypes.Standard, false);
            Assert.IsFalse(res.Success, "Disabling the Standard greeting should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, _callHandler.ObjectId, GreetingTypes.Error, false);
            Assert.IsFalse(res.Success, "Disabling the ErrorGreeting should faile");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, _callHandler.ObjectId, GreetingTypes.Alternate, false, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Disabling a greeting and passing a date in the future should fail");


        }

        /// <summary>
        /// exercise greeting rules failure conditions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetGreeting()
        {
            Greeting oGreeting;

            //static function call for GetGreeting
            WebCallResult res = Greeting.GetGreeting(null, _callHandler.ObjectId, GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Null Connection server object param should fail");

            res = Greeting.GetGreeting(_connectionServer, "", GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId string should fail");

            res = Greeting.GetGreeting(_connectionServer, "aaaa", GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Invalid call handler ObjectId should fail");

            res = Greeting.GetGreeting(_connectionServer, _callHandler.ObjectId, GreetingTypes.Invalid, out oGreeting);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

            res = Greeting.GetGreeting(null, _callHandler.ObjectId, GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.GetGreeting(_connectionServer, "", GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

            res = Greeting.GetGreeting(_connectionServer, "aaa", GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Invalid call handler ObjectId should fail");

            res = Greeting.GetGreeting(_connectionServer, _callHandler.ObjectId, GreetingTypes.Invalid, out oGreeting);
            Assert.IsFalse(res.Success, "Invalid Greeting type name should fail");

            //create an instance Greeting object and fill it with a failure case
            oGreeting = new Greeting(_connectionServer, _callHandler.ObjectId);
            Assert.IsNotNull(oGreeting, "Failed to create new Greeting object");

            res = oGreeting.GetGreeting(_callHandler.ObjectId, GreetingTypes.Invalid);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

            res = oGreeting.GetGreeting(_callHandler.ObjectId, GreetingTypes.Standard);
            Assert.IsTrue(res.Success, "Failed to fill greeting object with Standard greeting rule details" + res);
        }

        #endregion


        #region Live Tests

        /// <summary>
        /// exercise greeting rules
        /// </summary>
        [TestMethod]
        public void Greeting_Test()
        {
            Greeting oGreeting;

            WebCallResult res = CallHandler.GetCallHandler(out _callHandler, _connectionServer, "", "opening greeting");
            Assert.IsTrue(res.Success, "Failed to get opening greeting call handler");
            Assert.IsNotNull(_callHandler, "Null handler returned from search");

            //first, test getting a bogus greeting
            res = _callHandler.GetGreeting(GreetingTypes.Invalid, out oGreeting);
            Assert.IsFalse(res.Success, "GetGreeting should fail with an invalid greeting name");

            //get the alternate greeting rule and set it to play a new greeting we upload
            res = _callHandler.GetGreeting(GreetingTypes.Standard, out oGreeting);
            Assert.IsTrue(res.Success, "Failed fetching greeting rule:" + res.ToString());
            Assert.IsNotNull(oGreeting, "Null greeting returned from greeting fetch");

            oGreeting.ClearPendingChanges();

            //update a greeting with no pending changes should fail
            res = oGreeting.Update();
            Assert.IsFalse(res.Success, "Updating a greeting with no pending changes should fail");

            //iterate over all the greetings and dump their properties
            foreach (Greeting oGreetings in _callHandler.GetGreetings())
            {
                Console.WriteLine(oGreetings.ToString());
                Console.WriteLine(oGreetings.DumpAllProps());
            }

        }


        [TestMethod]
        public void GreetingStream_Test()
        {
            Greeting oGreeting;

            //get the alternate greeting rule and set it to play a new greeting we upload
            WebCallResult res = _callHandler.GetGreeting(GreetingTypes.Standard, out oGreeting);
            Assert.IsTrue(res.Success, "Failed fetching greeting rule:" + res.ToString());
            Assert.IsNotNull(oGreeting, "Null greeting returned from greeting fetch");


            GreetingStreamFile oStream;

            //get specific greeting stream file back out
            res = oGreeting.GetGreetingStreamFile(11, out oStream);
            Assert.IsFalse(res.Success, "Invalid language code should fail");

            List<GreetingStreamFile> oStreams = oGreeting.GetGreetingStreamFiles();
            Assert.IsNotNull(oStreams, "Null list of stream files returned");
            Assert.IsTrue(oStreams.Count > 0, "Empty list of stream files returned.");

            //dump all greeting stream file props
            foreach (GreetingStreamFile oTemp in oStreams)
            {
                oStream = oTemp;
                Console.WriteLine(oTemp.ToString());
                Console.WriteLine(oTemp.DumpAllProps());
            }

            res = oStream.GetGreetingWavFile("tempout.wav");
            Assert.IsTrue(res.Success, "Failed fetching stream file from GreetingStreamFile object");
        }

        #endregion
    }
}
