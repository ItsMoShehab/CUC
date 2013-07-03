using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for GreetingUnitTests and is intended
    ///to contain all GreetingUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class GreetingUnitTests : BaseUnitTests
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


        #region Static Call Failures

        [TestMethod]
        public void StaticCallFailures_GetGreetingStreamFile()
        {
            GreetingStreamFile oStream;
            //static method tests
            var res = GreetingStreamFile.GetGreetingStreamFile(null, "ObjectId", GreetingTypes.Standard, 1033, out oStream);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_mockServer, "", GreetingTypes.Standard, 1033, out oStream);
            Assert.IsFalse(res.Success, "Empty call handler ID param should fail");

            res = GreetingStreamFile.GetGreetingStreamFile(_mockServer, "ObjectId", GreetingTypes.Invalid, 1033, out oStream);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

        }


        [TestMethod]
        public void StaticCallFailures_GetGreetingWavFile()
        {
            //static calls to GetGreetingWAVFile
            var res = GreetingStreamFile.GetGreetingWavFile(null, "test.wav", "streamfilename");
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

            res = GreetingStreamFile.GetGreetingWavFile(_mockServer, "", "StreamFileName");
            Assert.IsFalse(res.Success, "Empty target WAV file path should fail");

            res = GreetingStreamFile.GetGreetingWavFile(_mockServer, "test.wav", "");
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
            WebCallResult res = Greeting.GetGreetings(null, "objectid", out oGreetings);
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.GetGreetings(_mockServer, "", out oGreetings);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

        }


        /// <summary>
        /// exercise greeting rules failure conditions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateGreeting()
        {
            //static calls for updateGreeting
            WebCallResult res = Greeting.UpdateGreeting(_mockServer, "objectid", GreetingTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Empty parameter list param should fail");

            res = Greeting.UpdateGreeting(null, "objectid", GreetingTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.UpdateGreeting(_mockServer, "", GreetingTypes.Alternate, null);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

            res = Greeting.UpdateGreeting(_mockServer, "objectid", GreetingTypes.Invalid, null);
            Assert.IsFalse(res.Success, "Invalid Greeting type name should fail");

        }


        /// <summary>
        /// exercise greeting rules failure conditions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_SetGreetingWavFile()
        {
            //SetGreetingWavFile
            WebCallResult res = Greeting.SetGreetingWavFile(null, "Dummy.wav", "objectid", GreetingTypes.Alternate, 1033, true);
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.SetGreetingWavFile(_mockServer, "bogus.wav", "objectid", GreetingTypes.Alternate, 1033, true);
            Assert.IsFalse(res.Success, "Invalid WAV file target should fail");

            res = Greeting.SetGreetingWavFile(_mockServer, "Dummy.wav", "", GreetingTypes.Alternate, 1033, true);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

            res = Greeting.SetGreetingWavFile(_mockServer, "Dummy.wav", "objectid", GreetingTypes.Invalid, 1033, true);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

            //static calls to SetGreetingWAVFiles with invalid params
            res = GreetingStreamFile.SetGreetingWavFile(null, "aaa", GreetingTypes.Alternate, 1033, "Dummy.wav");
            Assert.IsFalse(res.Success, "Null ConnectionServerRest param should fail");

            res = GreetingStreamFile.SetGreetingWavFile(_mockServer, "", GreetingTypes.Invalid, 1033, "Dummy.wav");
            Assert.IsFalse(res.Success, "Empty CallHandler ObjectId or greeting type should fail");

            List<GreetingStreamFile> oStreams;
            res = GreetingStreamFile.GetGreetingStreamFiles(null, "objectid", GreetingTypes.Alternate, out oStreams);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

        }

        /// <summary>
        /// exercise greeting rules failure conditions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateGreetingEnabledStatus()
        {
            //greeting enabled status
            WebCallResult res = Greeting.UpdateGreetingEnabledStatus(null, "objectid", GreetingTypes.Alternate, true, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_mockServer, "", GreetingTypes.Alternate, true, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_mockServer, "objectid", GreetingTypes.Invalid, true, DateTime.Now.AddDays(1));
            Assert.IsFalse(res.Success, "Invalid greeting rule name should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_mockServer, "objectid", GreetingTypes.Alternate, true, DateTime.Now.AddDays(-1));
            Assert.IsFalse(res.Success, "Enabling greeting to TRUE with a date in the past should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_mockServer, "objectid", GreetingTypes.Standard, false);
            Assert.IsFalse(res.Success, "Disabling the Standard greeting should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_mockServer, "objectid", GreetingTypes.Error, false);
            Assert.IsFalse(res.Success, "Disabling the ErrorGreeting should fail");

            res = Greeting.UpdateGreetingEnabledStatus(_mockServer, "objectid", GreetingTypes.Alternate, false, DateTime.Now.AddDays(1));
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
            WebCallResult res = Greeting.GetGreeting(null, "objectid", GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Null Connection server object param should fail");

            res = Greeting.GetGreeting(_mockServer, "", GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId string should fail");

            res = Greeting.GetGreeting(_mockServer, "objectid", GreetingTypes.Invalid, out oGreeting);
            Assert.IsFalse(res.Success, "Invalid greeting type name should fail");

            res = Greeting.GetGreeting(null, "objectid", GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Null ConnecitonObject param should fail");

            res = Greeting.GetGreeting(_mockServer, "", GreetingTypes.Alternate, out oGreeting);
            Assert.IsFalse(res.Success, "Empty call handler ObjectId param should fail");

            res = Greeting.GetGreeting(_mockServer, "objectid", GreetingTypes.Invalid, out oGreeting);
            Assert.IsFalse(res.Success, "Invalid Greeting type name should fail");
        }

        #endregion

    }
}
