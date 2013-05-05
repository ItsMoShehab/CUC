using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class LocationTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start Location test:" + ex.Message);
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
            Location oTestLocation = new Location(null);
            Console.WriteLine(oTestLocation);
        }

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


        [TestMethod]
        public void LocationFetchTests()
        {
            List<Location> oLocations;
            WebCallResult res = Location.GetLocations(_connectionServer, out oLocations,1,10,null);
            Assert.IsTrue(res.Success,"Failed to fetch list of Locations:"+res);

            Assert.IsTrue(oLocations.Count>0,"No locations found in fetch");

            string strObjectId = "";
            string strDisplayName = "";

            Console.WriteLine(oLocations[0].ToString());
            Console.WriteLine(oLocations[0].DumpAllProps());
            strObjectId = oLocations[0].ObjectId;
            strDisplayName = oLocations[0].DisplayName;

            res = oLocations[0].RefetchLocationData();
            Assert.IsTrue(res.Success,"Refetching location data failed:"+res);

            Location oNewLocation;

            res = Location.GetLocation(out oNewLocation, _connectionServer, strObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch location with valid ObjectId:" + res);

            res = Location.GetLocation(out oNewLocation, _connectionServer, "", strDisplayName);
            Assert.IsTrue(res.Success, "Failed to fetch location with valid display name:" + res);
        }

        [TestMethod]
        public void StaticCallFailure_GetLocations()
        {
            List<Location> oLocations;
            WebCallResult res = Location.GetLocations(null, out oLocations);
            Assert.IsFalse(res.Success, "Call to GetLocations did not fail with null ConnectionServer");

            res = Location.GetLocations(_connectionServer, out oLocations, 1, 10, "query=(BogusQuery)");
            Assert.IsFalse(res.Success, "Call to GetLocations did not fail with bogus query string");

            res = Location.GetLocations(_connectionServer, out oLocations, "", "query=(ObjectID is bogus)");
            Assert.IsTrue(res.Success, "Call to GetLocations with valid query failed:"+res);
            Assert.IsTrue(oLocations.Count==0,"Query that should return no matches returned non zero");


        }

        [TestMethod]
         public void StaticCallFailure_GetLocation()
         {
             Location oNewLocation;

             var res = Location.GetLocation(out oNewLocation, null, "", "displayname");
             Assert.IsFalse(res.Success, "Call to GetLocation did not fail with null ConnectionServer");

             res = Location.GetLocation(out oNewLocation, _connectionServer, "");
             Assert.IsFalse(res.Success, "Call to GetLocation did not fail with empty name and objectId parameters");

             res = Location.GetLocation(out oNewLocation, _connectionServer, "","__bogus___");
             Assert.IsFalse(res.Success, "Call to GetLocation did not fail with invalid name");
         }
    }
}
