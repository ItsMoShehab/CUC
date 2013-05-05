using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ScheduleSetTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static ScheduleSet _tempScheduleSet;
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
                throw new Exception("Unable to attach to Connection server to start ScheduleSetTest test:" + ex.Message);
            }

            WebCallResult res = ScheduleSet.AddQuickSchedule(_connectionServer, "temp_" + Guid.NewGuid().ToString(),
                                                             _connectionServer.PrimaryLocationObjectId, "", 0, 200, true,
                                                             true, true, true, true, false, false);
            Assert.IsTrue(res.Success, "Failed to create new quick schedule:" + res);
            res = ScheduleSet.GetScheduleSet(out _tempScheduleSet, _connectionServer, res.ReturnedObjectId);
            Assert.IsTrue(res.Success, "Failed to find new quick schedule:" + res);
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempScheduleSet != null)
            {
                WebCallResult res = _tempScheduleSet.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary schedule set on cleanup.");
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
            ScheduleSet oTest = new ScheduleSet(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid ObjectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            ScheduleSet oTest = new ScheduleSet(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            ScheduleSet oTest = new ScheduleSet(_connectionServer,"","bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        [TestMethod]
        public void ScheduleSetFetchTests()
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
                Console.WriteLine(oNewSet);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to fetch schedule set by valid ObjectId:"+ex);
            }

            try
            {
                oNewSet = new ScheduleSet(_connectionServer, "", strName);
                Console.WriteLine(oNewSet);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to fetch schedule set by valid name:"+ex);
            }

            //fetch the schedule set members
            

        }

        [TestMethod]
        public void AddRemoveMemberTests()
        {
            WebCallResult res = _tempScheduleSet.AddScheduleSetMember("bogus");
            Assert.IsFalse(res.Success,"Adding scheduleSetMemeber with invalid ObjectId did not fail");

            res = _tempScheduleSet.AddScheduleSetMember("");
            Assert.IsFalse(res.Success, "Adding scheduleSetMemeber with empty ObjectId did not fail");

            List<ScheduleSetMember> oMembers;
            res = _tempScheduleSet.GetSchedulesSetsMembers(out oMembers);
            Assert.IsTrue(res.Success,"Failed fetching schedule set members:"+res);
            Assert.IsTrue(oMembers.Count>0,"No members returned from fetch");

            ScheduleSetMember oMember;
            try
            {
                oMember = new ScheduleSetMember(_tempScheduleSet.ObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Could not create new schedule set member from Id:"+ex);
            }

        }

        [TestMethod]
        public void StaticMethodFailures()
        {
            //get schedule sets
            List<ScheduleSet> oSets;

            WebCallResult res = ScheduleSet.GetSchedulesSets(null, out oSets);
            Assert.IsFalse(res.Success,"Getting schedule sets with null ConnectionServer did not fail");

            //get ScheduleSet
            ScheduleSet oTempSet;
            res = ScheduleSet.GetScheduleSet(out oTempSet, null, "", "bogus");
            Assert.IsFalse(res.Success,"Calling GetScheduleSet with null ConnecitonServer did not fail.");

            res = ScheduleSet.GetScheduleSet(out oTempSet, _connectionServer);
            Assert.IsFalse(res.Success, "Calling GetScheduleSet with empty name and ObjectId did not fail.");

            res = ScheduleSet.GetScheduleSet(out oTempSet, _connectionServer, "bougs");
            Assert.IsFalse(res.Success, "Calling GetScheduleSet with invalid objectID did not fail.");

            res = ScheduleSet.GetScheduleSet(out oTempSet, _connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Calling GetScheduleSet with invalid name did not fail.");

            //get scheduleset memebers
            List<ScheduleSetMember> oMembers;
            res = ScheduleSet.GetSchedulesSetsMembers(null, "bogus", out oMembers);
            Assert.IsFalse(res.Success, "Getting schedule set members with null ConnectionServer did not fail");

            res = ScheduleSet.GetSchedulesSetsMembers(_connectionServer, "", out oMembers);
            Assert.IsFalse(res.Success, "Getting schedule set members with empty objectId did not fail");

            //AddScheduleSet
            res = ScheduleSet.AddScheduleSet(null, "DisplayName", _connectionServer.PrimaryLocationObjectId,"");
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with null ConnectionServer did not fail");

            res = ScheduleSet.AddScheduleSet(_connectionServer, "", _connectionServer.PrimaryLocationObjectId, "");
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with empty display name did not fail");

            res = ScheduleSet.AddScheduleSet(_connectionServer, "DisplayName", "", "");
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with empty locaitonId did not fail");

            res = ScheduleSet.AddScheduleSet(_connectionServer, "DisplayName", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with both a location and owner objectId did not fail");

            res = ScheduleSet.AddScheduleSet(_connectionServer, "DisplayName", "", "bogus");
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with invalid owner ObjectId did not fail");

            ScheduleSet oSet;
            res = ScheduleSet.AddScheduleSet(null, "DisplayName", _connectionServer.PrimaryLocationObjectId, "",out oSet);
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with null ConnectionServer did not fail");

            //deleteScheduleSet
            res = ScheduleSet.DeleteScheduleSet(null, "bogus");
            Assert.IsFalse(res.Success, "Calling DeleteScheduleSet with null ConnectionServer did not fail");

            res = ScheduleSet.DeleteScheduleSet(_connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Calling DeleteScheduleSet with invalid ObjectID did not fail");

            res = ScheduleSet.DeleteScheduleSet(_connectionServer, "");
            Assert.IsFalse(res.Success, "Calling DeleteScheduleSet with empty objectId did not fail");

            //AddScheduleSetMember
            res = ScheduleSet.AddScheduleSetMember(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling AddScheduleSetMember with null ConnectionServer did not fail");

            res = ScheduleSet.AddScheduleSetMember(_connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling AddScheduleSetMember with invalid schedule and scheduleset IDs did not fail");

            res = ScheduleSet.AddScheduleSetMember(_connectionServer, "", "");
            Assert.IsFalse(res.Success, "Calling AddScheduleSetMember with empty scheduleset and schedule Ids did not fail");

            //AddQuickSchedule
            res = ScheduleSet.AddQuickSchedule(null, "display name",
                                               _connectionServer.PrimaryLocationObjectId, "",
                                               0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with null Connection server did not fail");

            res = ScheduleSet.AddQuickSchedule(_connectionServer, "",
                                    _connectionServer.PrimaryLocationObjectId, "",
                                    0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with empty display name did not fail");

            res = ScheduleSet.AddQuickSchedule(_connectionServer, "display name",
                                    _connectionServer.PrimaryLocationObjectId, "bogus",
                                    0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with both a location and subscriberObjectId did not fail");

            res = ScheduleSet.AddQuickSchedule(_connectionServer, "display name","", "",
                                    0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with empty locaiton and subscriber ObjectID did not fail");

            res = ScheduleSet.AddQuickSchedule(_connectionServer, "display name","", "bogus",
                        0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with invalid subscriberObjectId did not fail");

        
        }


    }
}
