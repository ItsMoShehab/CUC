using System;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class TimeZoneTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static ConnectionServerRest _connectionServer;
        
        private TimeZones _timeZones;

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
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start TimeZone test:" + ex.Message);
            }

        }
        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            TimeZones oTest = new TimeZones(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid ConnectionServer
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            TimeZones oTest = new TimeZones(new ConnectionServerRest(new RestTransportFunctions()));
            Console.WriteLine(oTest);
        }

        #endregion


        [TestMethod]
        public void TimeZoneFailures()
        {
            if (_timeZones == null)
            {
                _timeZones = new TimeZones(_connectionServer);
            }

            ConnectionTimeZone oTemp;

            WebCallResult res = _timeZones.GetTimeZone(-1, out oTemp);

            Assert.IsFalse(res.Success, "Fetching time zone of -1 should fail.");
        }



        [TestMethod]
        public void FetchTimeZone()
        {
            if (_timeZones == null)
            {
                _timeZones = new TimeZones(_connectionServer);
            }

            ConnectionTimeZone oTemp;

            WebCallResult res = _timeZones.GetTimeZone(227, out oTemp);

            Assert.IsTrue(res.Success, "Failed to fetch 227 time zone:"+res.ToString());

            Console.WriteLine(oTemp.ToString());

        }
    }
}
