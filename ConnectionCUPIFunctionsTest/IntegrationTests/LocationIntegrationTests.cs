using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class LocationIntegrationTests : BaseIntegrationTests
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


        #region Constructor Tests

        /// <summary>
        /// Throw an UnityConnectionRestException if an invalid objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            Location oTestLocation = new Location(_connectionServer, "bogus");
            Console.WriteLine(oTestLocation);
        }

        /// <summary>
        /// Throw an UnityConnectionRestException if an invalid name address is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            Location oTestLocation = new Location(_connectionServer, "", "bogus");
            Console.WriteLine(oTestLocation);
        }

        #endregion


        private Location GetLocationHelper()
        {
            List<Location> oLocations;
            WebCallResult res = Location.GetLocations(_connectionServer, out oLocations, 1, 10, null);
            Assert.IsTrue(res.Success, "Failed to fetch list of Locations:" + res);
            Assert.IsTrue(oLocations.Count > 0, "No locations found in fetch");
            return oLocations[0];
        }
        
        [TestMethod]
        public void RefetchLocationData_Success()
        {
            Location oLocation = GetLocationHelper();

            Console.WriteLine(oLocation.ToString());
            Console.WriteLine(oLocation.DumpAllProps());

            var res = oLocation.RefetchLocationData();
            Assert.IsTrue(res.Success, "Refetching location data failed:" + res);
        }

        [TestMethod]
        public void GetLocation_ByObjectId_Success()
        {
            Location oLocation = GetLocationHelper();
            Location oNewLocation;

            var res = Location.GetLocation(out oNewLocation, _connectionServer, oLocation.ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch location with valid ObjectId:" + res);
        }

        [TestMethod]
        public void GetLocation_ByName_Success()
        {
            Location oLocation = GetLocationHelper();
            Location oNewLocation;

            var res = Location.GetLocation(out oNewLocation, _connectionServer, "", oLocation.DisplayName);
            Assert.IsTrue(res.Success, "Failed to fetch location with valid display name:" + res);
        }

        [TestMethod]
        public void GetLocations_ByObjectIdQuery_Success()
        {
            List<Location> oLocations;

            var res = Location.GetLocations(_connectionServer, out oLocations, 1, 10, "query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching locations with invalid query should not fail:" + res);
            Assert.IsTrue(oLocations.Count == 0, "Invalid query string should return an empty location list:" + oLocations.Count);
        }

        [TestMethod]
        public void GetLocations_InvalidQuery_Failure()
        {
            List<Location> oLocations;

            var res = Location.GetLocations(_connectionServer, out oLocations, 1, 10, "query=(BogusQuery)");
            Assert.IsFalse(res.Success, "Call to GetLocations did not fail with bogus query string");

        }

        [TestMethod]
        public void GetLocations_QueryWithNoMatches_Success()
        {
            List<Location> oLocations;

            var res = Location.GetLocations(_connectionServer, out oLocations, "", "query=(ObjectID is bogus)");
            Assert.IsTrue(res.Success, "Call to GetLocations with valid query failed:"+res);
            Assert.IsTrue(oLocations.Count==0,"Query that should return no matches returned non zero");
        }

        [TestMethod]
        public void GetLocation_InvalidObjectId_Failure()
         {
             Location oNewLocation;

             var res = Location.GetLocation(out oNewLocation, _connectionServer, "bogus");
             Assert.IsFalse(res.Success, "Call to GetLocation did not fail with empty name and objectId parameters");
        }

        [TestMethod]
        public void GetLocation_InvalidName_Failure()
        {
            Location oNewLocation;

             var res = Location.GetLocation(out oNewLocation, _connectionServer, "","__bogus___");
             Assert.IsFalse(res.Success, "Call to GetLocation did not fail with invalid name");
        }
    }
}
