using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RestrictionTableIntegrationTests : BaseIntegrationTests
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


        #region Class Construction Error Tests

        /// <summary>
        /// UnityConnectionRestException if invalid ObjectId passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RestritionTable_Constructor_InvalidObjectId_Failure()
        {
            RestrictionTable oTest = new RestrictionTable(_connectionServer,"ObjectId");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException if invalid name passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RestrictionTable_Constructor_InvalidDisplayName_Failure()
        {
            RestrictionTable oTest = new RestrictionTable(_connectionServer, "", "bogusDisplayName");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException if invalid ObjectId passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RestrictionPattern_Constructor_InvalidObjectIds_Failure()
        {
            RestrictionPattern oTest = new RestrictionPattern(_connectionServer, "bogusRestrictionTableObjectId", "bogusObjectId");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Calls

        [TestMethod]
        public void GetRestrictionPatterns_InvalidObjectId_Success()
        {
            List<RestrictionPattern> oPatterns;
            var res = RestrictionPattern.GetRestrictionPatterns(_connectionServer, "InvalidObjectId", out oPatterns);
            Assert.IsTrue(res.Success, "Static call to GetRestrictionPattners should not fail with invalid objectId:"+res);
            Assert.IsTrue(oPatterns.Count==0,"Call to GetRestrictionPatterns with invalid objectId should return empty list");
        }

        #endregion


        #region Live Tests

        private RestrictionTable HelperGetRestrictionTable()
        {
            List<RestrictionTable> oTables;
            WebCallResult res = RestrictionTable.GetRestrictionTables(_connectionServer, out oTables, 1, 2);
            Assert.IsTrue(res.Success, "Fetching restriction tables failed:" + res);
            Assert.IsTrue(oTables.Count > 0, "No restriction tables fetched");
            return oTables[0];
        }

        [TestMethod]
        public void GetRestrictionTables_Success()
        {
            var oTable = HelperGetRestrictionTable();

            Console.WriteLine(oTable.ToString());
            Console.WriteLine(oTable.DumpAllProps());
        }

        [TestMethod]
        public void GetRestrictionTables_IteratePatterns_Success()
        {
            var oTable = HelperGetRestrictionTable();

            Assert.IsTrue(oTable.RestrictionPatterns().Count > 0,
                          "No restriction patterns found in restriction table fetch");

            foreach (var oPattern in oTable.RestrictionPatterns())
            {
                Console.WriteLine(oPattern.ToString());
                Console.WriteLine(oPattern.DumpAllProps());
            }

            //force refetch of data
            oTable.RestrictionPatterns(true);
        }

        [TestMethod]
        public void RestrictionTable_ConstructorWithObjectId_Success()
        {
            var oTable = HelperGetRestrictionTable();

            try
            {
                RestrictionTable oNewTable = new RestrictionTable(_connectionServer, oTable.ObjectId);
                Console.WriteLine(oNewTable);
            }
            catch (Exception ex)
            {
                Assert.Fail("RestrictionTable class creation failed with valid objectId:" + ex);
            }
        }

        [TestMethod]
        public void RestrictionTable_ConstructorWithDisplayName_Success()
        {
            var oTable = HelperGetRestrictionTable();
            try
            {
                RestrictionTable oNewTable = new RestrictionTable(_connectionServer, "", oTable.DisplayName);
                Console.WriteLine(oNewTable);
            }
            catch (Exception ex)
            {
                Assert.Fail("RestrictionTable class creation failed with valid name:" + ex);
            }
        }

        [TestMethod]
        public void RestrictionPattern_ConstructorWithObjectId_Success()
        {
            var oTable = HelperGetRestrictionTable();

            try
            {
                RestrictionPattern oNewPattern = new RestrictionPattern(_connectionServer, oTable.ObjectId,
                                                                        oTable.RestrictionPatterns()[0].ObjectId);
                Console.WriteLine(oNewPattern);
            }
            catch (Exception ex)
            {
                Assert.Fail("RestrictionPattern class creation failed with valid objectId:" + ex);
            }
        }


        [TestMethod]
        public void GetRestrictionTables_WithQueryThatReturnsNoResults_Success()
        {
            List<RestrictionTable> oTables;

            var res = RestrictionTable.GetRestrictionTables(_connectionServer, out oTables,1,2,"query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching RTs with invalid query should not fail:" + res);
            Assert.IsTrue(oTables.Count == 0, "Invalid query string should return an empty RT list:" + oTables.Count);
        }

        [TestMethod]
        public void GetRestrictionTables_NullQuery_Success()
        {
            List<RestrictionTable> oTables;
            WebCallResult res = RestrictionTable.GetRestrictionTables(_connectionServer, out oTables, 1, 2,null);
            Assert.IsTrue(res.Success, "Fetching restriction tables failed:" + res);
            Assert.IsTrue(oTables.Count > 0, "No restriction tables fetched");
            
            try
            {
                RestrictionPattern oPattern = new RestrictionPattern(_connectionServer, oTables[0].ObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Creating new pattern object without an objectId should not fail:"+ex);
            }
        }

        #endregion

    }
}
