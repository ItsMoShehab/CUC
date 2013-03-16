using System;
using System.Collections.Generic;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ScheduleSetTest
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


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            ScheduleSet oTest = new ScheduleSet(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            ScheduleSet oTest = new ScheduleSet(_connectionServer,"bogus");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            ScheduleSet oTest = new ScheduleSet(_connectionServer,"","bogus");
        }


        [TestMethod]
        public void TestMethod1()
        {
            List<ScheduleSet> oSets;
            WebCallResult res = ScheduleSet.GetSchedulesSets(_connectionServer, out oSets);
            Assert.IsTrue(res.Success,"Failed to fetch schedule sets:"+res);
            Assert.IsTrue(oSets.Count>0,"No schedule sets returned in fetch");

            string strObjectId="";
            string strName="";

            foreach (var oScheduleSet in oSets)
            {
                strObjectId = oScheduleSet.ObjectId;
                strName = oScheduleSet.DisplayName;
                Console.WriteLine(oScheduleSet.ToString());
                Console.WriteLine(oScheduleSet.DumpAllProps());
                Console.WriteLine(oScheduleSet.GetScheduleState(DateTime.Now));

                Console.WriteLine(oScheduleSet.Schedules().Count);
                
                res = oScheduleSet.RefetchScheduleSetData();
                Assert.IsTrue(res.Success,"Failed to re-fetch schedule set data:"+res);
            }

            ScheduleSet oNewSet;
            try
            {
                oNewSet = new ScheduleSet(_connectionServer, strObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to fetch schedule set by valid ObjectId:"+ex);
            }

            try
            {
                oNewSet = new ScheduleSet(_connectionServer, "", strName);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to fetch schedule set by valid name:"+ex);
            }

        }


        [TestMethod]
        public void StatidMethodFailures()
        {
            ScheduleSet oSet;
            List<ScheduleSet> oSets;

            WebCallResult res = ScheduleSet.GetSchedulesSets(null, out oSets);
            Assert.IsFalse(res.Success,"Getting schedule sets with null ConnectionServer did not fail");

            List<ScheduleSetMember> oMembers;
            res = ScheduleSet.GetSchedulesSetsMembers(null, "bogus", out oMembers);
            Assert.IsFalse(res.Success, "Getting schedule set members with null ConnectionServer did not fail");

            res = ScheduleSet.GetSchedulesSetsMembers(_connectionServer, "", out oMembers);
            Assert.IsFalse(res.Success, "Getting schedule set members with empty objectId did not fail");
        }

    }
}
