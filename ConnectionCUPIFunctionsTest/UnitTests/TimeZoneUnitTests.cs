using System;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class TimeZoneUnitTests : BaseUnitTests 
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


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            TimeZones oTest = new TimeZones(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid ConnectionServer
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_EmptyConnectionServer_Failure()
        {
            TimeZones oTest = new TimeZones(new ConnectionServerRest(new RestTransportFunctions()));
            Console.WriteLine(oTest);
        }

        [TestMethod]
        public void Constructor_ConnectionTimeZone_Success()
        {
            var CxnTimeZone = new ConnectionTimeZone();
            Console.WriteLine(CxnTimeZone.ToString());
        }

        #endregion


    }
}
