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
        [ClassInitialize()]
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
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

        }

        #endregion


        #region Class Construction Error Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void RestrictionTableClassCreationFailure()
        {
            RestrictionTable oTest = new RestrictionTable(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void RestrictionTableClassCreationFailureClassCreationFailure2()
        {
            RestrictionTable oTest = new RestrictionTable(_connectionServer,"bogus");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void RestrictionTableClassCreationFailureClassCreationFailure3()
        {
            RestrictionTable oTest = new RestrictionTable(_connectionServer, "", "bogus");
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void RestrictionPatternClassCreationFailure()
        {
            RestrictionPattern oTest = new RestrictionPattern(null,"bogus","bogus");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void RestrictionPatternClassCreationFailureClassCreationFailure2()
        {
            RestrictionPattern oTest = new RestrictionPattern(_connectionServer, "", "bogus");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void RestrictionPatternClassCreationFailureClassCreationFailure3()
        {
            RestrictionPattern oTest = new RestrictionPattern(_connectionServer, "bogus", "bogus");
        }

        #endregion


        [TestMethod]
        public void TestMethod1()
        {

            List<RestrictionTable> oTables;
            WebCallResult res = RestrictionTable.GetRestrictionTables(_connectionServer, out oTables);
            Assert.IsTrue(res.Success,"Fetching restriction tables failed:"+res);

            string strTableName = "";
            string strTableObjectId = "";
            string strPatternObjectId = "";

            foreach (var oTable in oTables)
            {
                strTableName = oTable.DisplayName;
                strTableObjectId = oTable.ObjectId;

                Console.WriteLine(oTable.ToString());
                Console.WriteLine(oTable.DumpAllProps());
                foreach (var oPattern in oTable.RestrictionPatterns())
                {
                    strPatternObjectId = oPattern.ObjectId;
                    Console.WriteLine(oPattern.ToString());
                    Console.WriteLine(oPattern.DumpAllProps());
                }
                oTable.RestrictionPatterns(true);

            }

            Assert.IsFalse(string.IsNullOrEmpty(strTableObjectId),"No valid restriction table found in fetch");
            Assert.IsFalse(string.IsNullOrEmpty(strTableName), "No valid restriction table found in fetch");
            Assert.IsFalse(string.IsNullOrEmpty(strPatternObjectId), "No valid restriction pattern found in fetch");

            RestrictionTable oNewTable;
            try
            {
                oNewTable = new RestrictionTable(_connectionServer, strTableObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("RestrictionTable class creation failed with valid objectId:"+ex);
            }

            try
            {
                oNewTable = new RestrictionTable(_connectionServer, "", strTableName);
            }
            catch (Exception ex)
            {
                Assert.Fail("RestrictionTable class creation failed with valid name:" + ex);
            }

            try
            {
                RestrictionPattern oNewPattern = new RestrictionPattern(_connectionServer,strTableObjectId, strPatternObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("RestrictionPattern class creation failed with valid objectId:" + ex);
            }


            //static fetch failures
            res = RestrictionTable.GetRestrictionTables(null, out oTables);
            Assert.IsFalse(res.Success,"Static restriction table creation did not fail with null ConnectionServer");

            List<RestrictionPattern> oPatterns;
            res = RestrictionPattern.GetRestrictionPatterns(null, "bogus", out oPatterns);
            Assert.IsFalse(res.Success,"Static call to GetRestrictionPattners did not fail with null Connection server");

            res = RestrictionPattern.GetRestrictionPatterns(_connectionServer, "", out oPatterns);
            Assert.IsFalse(res.Success, "Static call to GetRestrictionPattners did not fail with empty objectId");

        }
    }
}
