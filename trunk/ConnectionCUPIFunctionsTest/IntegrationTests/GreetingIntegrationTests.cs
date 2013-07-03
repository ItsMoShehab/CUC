using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for GreetingIntegrationTests and is intended
    ///to contain all GreetingIntegrationTests Unit Tests
    ///</summary>
    [TestClass]
    public class GreetingIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties
        
        //class wide call handler to use for various greeting tests - this will get filled with the opening greeting call handler.
        private static CallHandler _callHandler;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            //get a call handler to use for various tests in this class - the opening greeting should always be there
            WebCallResult res = CallHandler.GetCallHandler(out _callHandler, _connectionServer, "", "opening greeting");
            if (res.Success == false | _callHandler == null)
            {
                throw new Exception("Unable to get opening greeting call handler for test functions in GreetingIntegrationTests");
            }
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticCallFailures_GetGreetingStreamFile()
        {
            GreetingStreamFile oStream;
            var res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, "aaa", GreetingTypes.Standard, 1033, out oStream);
            Assert.IsFalse(res.Success, "Invalid Call handler Id should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, _callHandler.ObjectId, GreetingTypes.Invalid, 1033, out oStream);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, _callHandler.ObjectId, GreetingTypes.Standard, 10, out oStream);
            Assert.IsFalse(res.Success, "Invalid language code should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_connectionServer, _callHandler.ObjectId, GreetingTypes.Standard, 1033, out oStream);
            Assert.IsTrue(res.Success, "Failed fetching stream file from GreetingStreamFile static call");
        }


        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetGreetings()
        {
            List<Greeting> oGreetings;

            var res = Greeting.GetGreetings(_connectionServer, "aaa", out oGreetings);
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
        public void StaticCallFailures_SetGreetingWavFile()
        {
            var res = Greeting.SetGreetingWavFile(_connectionServer, "Dummy.wav", "aaa", GreetingTypes.Alternate, 1033, true);
            Assert.IsFalse(res.Success, "Invalid call handler ObjectId param should fail");

            res = Greeting.SetGreetingWavFile(_connectionServer, "Dummy.wav", _callHandler.ObjectId, GreetingTypes.Invalid, 1033, true);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

            res = Greeting.SetGreetingWavFile(_connectionServer, "Dummy.wav", _callHandler.ObjectId, GreetingTypes.Alternate, 10, true);
            Assert.IsFalse(res.Success, "Invalid language code should fail");
        }

        /// <summary>
        /// exercise greeting rules failure conditions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateGreetingEnabledStatus()
        {
            var res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, "aaa", GreetingTypes.Alternate, true, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Invalid call handler ObjectId should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, _callHandler.ObjectId, GreetingTypes.Invalid, true, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Invalid greeting rule name should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, _callHandler.ObjectId, GreetingTypes.Alternate, true, DateTime.Now.AddDays(-1));
            Assert.IsFalse(res.Success, "Enabling greeting to TRUE with a date in the past should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, _callHandler.ObjectId, GreetingTypes.Standard, false);
            Assert.IsFalse(res.Success, "Disabling the Standard greeting should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_connectionServer, _callHandler.ObjectId, GreetingTypes.Error, false);
            Assert.IsFalse(res.Success, "Disabling the ErrorGreeting should fail");

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

            var res = Greeting.GetGreeting(_connectionServer, "aaaa", GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Invalid call handler ObjectId should fail");

            res = Greeting.GetGreeting(_connectionServer, _callHandler.ObjectId, GreetingTypes.Invalid, out oGreeting);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

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
