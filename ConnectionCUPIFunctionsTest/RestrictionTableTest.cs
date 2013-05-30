using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RestrictionTableTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

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
                throw new Exception("Unable to attach to Connection server to start RestrictionTable test:" + ex.Message);
            }

        }

        #endregion


        #region Class Construction Error Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestrictionTableClassCreationFailure()
        {
            RestrictionTable oTest = new RestrictionTable(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException if invalid ObjectId passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RestrictionTableClassCreationFailureClassCreationFailure2()
        {
            RestrictionTable oTest = new RestrictionTable(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException if invalid name passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RestrictionTableClassCreationFailureClassCreationFailure3()
        {
            RestrictionTable oTest = new RestrictionTable(_connectionServer, "", "bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestrictionPatternClassCreationFailure()
        {
            RestrictionPattern oTest = new RestrictionPattern(null,"bogus","bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// ArgumentException passed if empty objectId passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestrictionPatternClassCreationFailureClassCreationFailure2()
        {
            RestrictionPattern oTest = new RestrictionPattern(_connectionServer, "", "bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException if invalid ObjectId passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RestrictionPatternClassCreationFailureClassCreationFailure3()
        {
            RestrictionPattern oTest = new RestrictionPattern(_connectionServer, "bogus", "bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticCallFailure_GetRestrictionTables()
        {
            //static fetch failures
            List<RestrictionTable> oTables;
            var res = RestrictionTable.GetRestrictionTables(null, out oTables);
            Assert.IsFalse(res.Success, "Static restriction table creation did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void StaticCallFailure_GetRestrictionPatterns()
        {
            List<RestrictionPattern> oPatterns;
            var res = RestrictionPattern.GetRestrictionPatterns(null, "bogus", out oPatterns);
            Assert.IsFalse(res.Success, "Static call to GetRestrictionPattners did not fail with null Connection server");

            res = RestrictionPattern.GetRestrictionPatterns(_connectionServer, "", out oPatterns);
            Assert.IsFalse(res.Success, "Static call to GetRestrictionPattners did not fail with empty objectId");

        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void FetchTests()
        {
            List<RestrictionTable> oTables;
            WebCallResult res = RestrictionTable.GetRestrictionTables(_connectionServer, out oTables,1,2);
            Assert.IsTrue(res.Success,"Fetching restriction tables failed:"+res);
            Assert.IsTrue(oTables.Count>0,"No restriction tables fetched");

            string strTableName = "";
            string strTableObjectId = "";
            string strPatternObjectId = "";

            strTableName = oTables[0].DisplayName;
            strTableObjectId = oTables[0].ObjectId;

            Console.WriteLine(oTables[0].ToString());
            Console.WriteLine(oTables[0].DumpAllProps());
            foreach (var oPattern in oTables[0].RestrictionPatterns())
            {
                strPatternObjectId = oPattern.ObjectId;
                Console.WriteLine(oPattern.ToString());
                Console.WriteLine(oPattern.DumpAllProps());
            }
            oTables[0].RestrictionPatterns(true);


            Assert.IsFalse(string.IsNullOrEmpty(strTableObjectId),"No valid restriction table found in fetch");
            Assert.IsFalse(string.IsNullOrEmpty(strTableName), "No valid restriction table found in fetch");
            Assert.IsFalse(string.IsNullOrEmpty(strPatternObjectId), "No valid restriction pattern found in fetch");

            RestrictionTable oNewTable;
            try
            {
                oNewTable = new RestrictionTable(_connectionServer, strTableObjectId);
                Console.WriteLine(oNewTable);
            }
            catch (Exception ex)
            {
                Assert.Fail("RestrictionTable class creation failed with valid objectId:"+ex);
            }

            try
            {
                oNewTable = new RestrictionTable(_connectionServer, "", strTableName);
                Console.WriteLine(oNewTable);
            }
            catch (Exception ex)
            {
                Assert.Fail("RestrictionTable class creation failed with valid name:" + ex);
            }

            try
            {
                RestrictionPattern oNewPattern = new RestrictionPattern(_connectionServer,strTableObjectId, strPatternObjectId);
                Console.WriteLine(oNewPattern);
            }
            catch (Exception ex)
            {
                Assert.Fail("RestrictionPattern class creation failed with valid objectId:" + ex);
            }


            res = RestrictionTable.GetRestrictionTables(_connectionServer, out oTables,1,2,"query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching RTs with invalid query should not fail:" + res);
            Assert.IsTrue(oTables.Count == 0, "Invalid query string should return an empty RT list:" + oTables.Count);

        }

        [TestMethod]
        public void ConstructWithEmptyObjectId()
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

        #region Test Harness

        // EmptyResultText, InvalidResultText, ErrorResponse
        [TestMethod]
        public void RestrctionPattern_Construct_TestHarnessFailures()
        {
            ConnectionServerRest oServer = new ConnectionServerRest(new TestTransportFunctions(),"test","test","test");

            try
            {
                RestrictionPattern oPattern = new RestrictionPattern(oServer, "EmptyResultText", "objectid");
                Assert.Fail("Creating restriction pattern with empty response text should fail");
            }
            catch {}

            try
            {
                RestrictionPattern oPattern = new RestrictionPattern(oServer, "InvalidResultText", "objectid");
                Assert.Fail("Creating restriction pattern with empty response text should fail");
            }
            catch {}

            try
            {
                RestrictionPattern oPattern = new RestrictionPattern(oServer, "ErrorResponse", "objectid");
                Assert.Fail("Creating restriction pattern with empty response text should fail");
            }
            catch { }

        }

        [TestMethod]
        public void GetRestrictionPatterns_TestHarnessFailures()
        {
            ConnectionServerRest oServer = new ConnectionServerRest(new TestTransportFunctions(), "test", "test", "test");

            List<RestrictionPattern> oPatterns;
            var res = RestrictionPattern.GetRestrictionPatterns(oServer, "EmptyResultText", out oPatterns, 1, 2);
            Assert.IsFalse(res.Success, "Calling GetRestrictionPatterns with empty result text should fail");
            Assert.IsTrue(oPatterns.Count==0,"Empty response text should result in an empty list");

            res = RestrictionPattern.GetRestrictionPatterns(oServer, "InvalidResultText", out oPatterns, 1, 2);
            Assert.IsTrue(res.Success, "Calling GetRestrictionPatterns with InvalidResultText should not fail:"+res);
            Assert.IsTrue(oPatterns.Count == 0, "Invalid response text should result in an empty list");

            res = RestrictionPattern.GetRestrictionPatterns(oServer, "ErrorResponse", out oPatterns, 1, 2);
            Assert.IsFalse(res.Success, "Calling GetRestrictionPatterns with ErrorResponse should fail");
            Assert.IsTrue(oPatterns.Count == 0, "Error response should result in an empty list");
        }

        [TestMethod]
        public void GetRestrictionTables_TestHarnessFailure()
        {
            ConnectionServerRest oServer = new ConnectionServerRest(new TestTransportFunctions(), "test", "test", "test");

            List<RestrictionTable> oTables;
            var res = RestrictionTable.GetRestrictionTables(oServer, out oTables, 1, 2, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetRestrictionTables with empty result text should fail");
            Assert.IsTrue(oTables.Count == 0, "Empty response text should result in an empty list");

            res = RestrictionTable.GetRestrictionTables(oServer, out oTables, 1, 2, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetRestrictionTables with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oTables.Count == 0, "Invalid response text should result in an empty list");

            res = RestrictionTable.GetRestrictionTables(oServer, out oTables, 1, 2, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetRestrictionTables with ErrorResponse should fail");
            Assert.IsTrue(oTables.Count == 0, "Error response should result in an empty list");
            
        }

        [TestMethod]
        public void RestrictionTable_Construct_TestHarnessFailures()
        {
            ConnectionServerRest oServer = new ConnectionServerRest(new TestTransportFunctions(), "test", "test", "test");

            try
            {
                RestrictionTable oTable = new RestrictionTable(oServer, "EmptyResultText");
                Assert.Fail("Creating restriction table with empty response text should fail");
            }
            catch { }

            try
            {
                RestrictionTable oTable = new RestrictionTable(oServer, "InvalidResultText");
                Assert.Fail("Creating restriction table with empty response text should fail");
            }
            catch { }

            try
            {
                RestrictionTable oTable = new RestrictionTable(oServer, "ErrorResponse");
                Assert.Fail("Creating restriction table with empty response text should fail");
            }
            catch { }

        }

        #endregion
    }
}
