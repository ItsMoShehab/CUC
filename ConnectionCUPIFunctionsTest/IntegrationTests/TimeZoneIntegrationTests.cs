using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class TimeZoneIntegrationTests : BaseIntegrationTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
        }
        #endregion


        #region Live Tests 

        [TestMethod]
        public void GetTimeZone_InvalidZone_Failure()
        {
          
            var timeZones = new TimeZones(_connectionServer);

            ConnectionTimeZone oTemp;

            WebCallResult res = timeZones.GetTimeZone(-1, out oTemp);

            Assert.IsFalse(res.Success, "Fetching time zone of -1 should fail.");
        }



        [TestMethod]
        public void GetTimeZone_Success()
        {
            var timeZones = new TimeZones(_connectionServer);

            ConnectionTimeZone oTemp;

            WebCallResult res = timeZones.GetTimeZone(227, out oTemp);

            Assert.IsTrue(res.Success, "Failed to fetch 227 time zone:"+res.ToString());

            Console.WriteLine(oTemp.ToString());
        }

        #endregion
    }
}
