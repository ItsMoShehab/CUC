using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ScheduleSetIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static ScheduleSet _tempScheduleSet;

        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

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
        /// UnityConnectionRestException on invalid ObjectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            ScheduleSet oTest = new ScheduleSet(_connectionServer,"ObjectId");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidDisplayName_Failure()
        {
            ScheduleSet oTest = new ScheduleSet(_connectionServer,"","bogus display name");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Live Tests 

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

            res = ScheduleSet.GetSchedulesSets(_connectionServer, out oSets,1,2,"query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching schedule sets with invalid query should not fail:" + res);
            Assert.IsTrue(oSets.Count == 0, "Invalid query string should return an empty schedule list:" + oSets.Count);
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

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void GetScheduleSet_InvalidObjectId_Failure()
        {
            ScheduleSet oTempSet;
            var res = ScheduleSet.GetScheduleSet(out oTempSet, _connectionServer, "objectId");
            Assert.IsFalse(res.Success, "Calling GetScheduleSet with invalid objectID did not fail.");
        }
        
        [TestMethod]
        public void GetScheduleSet_InvalidDisplayName_Failure()
        {
            ScheduleSet oTempSet;
            var res = ScheduleSet.GetScheduleSet(out oTempSet, _connectionServer, "", "bogus display name");
            Assert.IsFalse(res.Success, "Calling GetScheduleSet with invalid name did not fail.");
        }

        [TestMethod]
        public void GetSchedulesSetsMembers_InvalidObjectId_Failure()
        {
            List<ScheduleSetMember> oMembers;

            var res = ScheduleSet.GetSchedulesSetsMembers(_connectionServer, "ObjectId", out oMembers);
            Assert.IsFalse(res.Success, "Getting schedule set members with invalid objectId did not fail");
        }

        [TestMethod]
        public void AddScheduleSet_InvalidOwnerOBjectId_Failure()
        {
            var res = ScheduleSet.AddScheduleSet(_connectionServer, "DisplayName", "", "ownerobjectId");
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with invalid owner ObjectId did not fail");
        }

        [TestMethod]
        public void DeleteScheduleSet_InvalidObjectId_Failure()
        {
            var res = ScheduleSet.DeleteScheduleSet(_connectionServer, "objectId");
            Assert.IsFalse(res.Success, "Calling DeleteScheduleSet with invalid ObjectID did not fail");
        }

        [TestMethod]
        public void AddScheduleSetMember_InvalidScheduleAndScheduleSetObjectIds_Failure()
        {
            var res = ScheduleSet.AddScheduleSetMember(_connectionServer, "ScheduleSetObjectId", "ScheduleObjectId");
            Assert.IsFalse(res.Success, "Calling AddScheduleSetMember with invalid schedule and scheduleset IDs did not fail");
        }

        [TestMethod]
        public void AddQuickSchedule_InvalidSubscriberObjectId_Failure()
        {
            var res = ScheduleSet.AddQuickSchedule(_connectionServer, "display name", "", "bogus",
                        0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with invalid subscriberObjectId did not fail");
        }

        #endregion
    }
}
