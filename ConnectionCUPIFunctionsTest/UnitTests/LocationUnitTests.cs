using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class LocationUnitTests : BaseUnitTests
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
            Location oTestLocation = new Location(null);
            Console.WriteLine(oTestLocation);
        }

        #endregion


        [TestMethod]
        public void GetLocations_NullConnectionServer_Failure()
        {
            List<Location> oLocations;
            WebCallResult res = Location.GetLocations(null, out oLocations);
            Assert.IsFalse(res.Success, "Call to GetLocations did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void GetLocation_NullConnectionServer_Failure()
        {
            Location oNewLocation;

            var res = Location.GetLocation(out oNewLocation, null, "", "displayname");
            Assert.IsFalse(res.Success, "Call to GetLocation did not fail with null ConnectionServer");

        }

        [TestMethod]
        public void GetLocation_EmptyObjectId_Failure()
         {
             Location oNewLocation;

            var res = Location.GetLocation(out oNewLocation, _mockServer, "");
             Assert.IsFalse(res.Success, "Call to GetLocation did not fail with empty name and objectId parameters");
         }
    }
}
