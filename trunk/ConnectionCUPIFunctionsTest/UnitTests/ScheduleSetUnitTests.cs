using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ScheduleSetUnitTests : BaseUnitTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

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
            ScheduleSet oTest = new ScheduleSet(null);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void GetScheduleSet_NullConnectionServer_Failure()
        {
            ScheduleSet oTempSet;
            var res = ScheduleSet.GetScheduleSet(out oTempSet, null, "", "bogus");
            Assert.IsFalse(res.Success, "Calling GetScheduleSet with null ConnecitonServer did not fail.");
        }

        [TestMethod]
        public void GetScheduleSet_EmptyNameAndOBjectId_Failure()
        {
            ScheduleSet oTempSet;

            var res = ScheduleSet.GetScheduleSet(out oTempSet, _mockServer);
            Assert.IsFalse(res.Success, "Calling GetScheduleSet with empty name and ObjectId did not fail.");
        }

        [TestMethod]
        public void GetSchedulesSetsMembers_NullConnectionServer_Failure()
        {
            List<ScheduleSetMember> oMembers;
            var res = ScheduleSet.GetSchedulesSetsMembers(null, "bogus", out oMembers);
            Assert.IsFalse(res.Success, "Getting schedule set members with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void GetSchedulesSetsMembers_EmptyObjectId_Failure()
        {
            List<ScheduleSetMember> oMembers;
            var res = ScheduleSet.GetSchedulesSetsMembers(_mockServer, "", out oMembers);
            Assert.IsFalse(res.Success, "Getting schedule set members with empty objectId did not fail");
        }

        [TestMethod]
        public void AddScheduleSet_NullConnectionServer_Failure()
        {
            ScheduleSet oSet;
            var res = ScheduleSet.AddScheduleSet(null, "DisplayName", "locationObjectId", "",out oSet);
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with null ConnectionServerRest did not fail");

         }

        [TestMethod]
        public void AddScheduleSet_EmptyDisplayName_Failure()
        {
            var res = ScheduleSet.AddScheduleSet(_mockServer, "", "locationObjectId", "");
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with empty display name did not fail");

            }

        [TestMethod]
        public void AddScheduleSet_EmptyLocationIdAndSubscriberId_Failure()
        {
            var res = ScheduleSet.AddScheduleSet(_mockServer, "DisplayName", "", "");
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with empty locaitonId and subscriberId did not fail");

            }

        [TestMethod]
        public void AddScheduleSet_BothLocationAndSubscriberIds_Failure()
        {
            var res = ScheduleSet.AddScheduleSet(_mockServer, "DisplayName", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling AddScheduleSet with both a location and owner objectId did not fail");
        }

        [TestMethod]
        public void DeleteScheduleSet_NullConnectionServer_Failure()
        {
            var res = ScheduleSet.DeleteScheduleSet(null, "bogus");
            Assert.IsFalse(res.Success, "Calling DeleteScheduleSet with null ConnectionServerRest did not fail");

            }

        [TestMethod]
        public void DeleteScheduleSet_EmptyObjectId_Failure()
        {
            var res = ScheduleSet.DeleteScheduleSet(_mockServer, "");
            Assert.IsFalse(res.Success, "Calling DeleteScheduleSet with empty objectId did not fail");
        }

        [TestMethod]
        public void AddScheduleSetMember_NullConnectionServer_Failure()
        {
            var res = ScheduleSet.AddScheduleSetMember(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling AddScheduleSetMember with null ConnectionServerRest did not fail");

            }

        [TestMethod]
        public void AddScheduleSetMember_EmptySchedulesetAndScheduleIds_Failure()
        {
            var res = ScheduleSet.AddScheduleSetMember(_mockServer, "", "");
            Assert.IsFalse(res.Success, "Calling AddScheduleSetMember with empty scheduleset and schedule Ids did not fail");
        }

        [TestMethod]
        public void AddQuickSchedule_NullConnectionServer_Failure()
        {
            var res = ScheduleSet.AddQuickSchedule(null, "display name","LocationObjectId", "",
                                               0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with null Connection server did not fail");

}

        [TestMethod]
        public void AddQuickSchedule_EmptyDisplayName_Failure()
        {
            var res = ScheduleSet.AddQuickSchedule(_mockServer, "", "LocationObjectId", "",
                                    0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with empty display name did not fail");

            }

        [TestMethod]
        public void AddQuickSchedule_BothLocationAndSubscriberObjectIds_Failure()
        {
            var res = ScheduleSet.AddQuickSchedule(_mockServer, "display name", "LocationObjectId", "bogus",
                                    0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with both a location and subscriberObjectId did not fail");

            }

        [TestMethod]
        public void AddQuickSchedule_EmptyLocationAndSubscriberObjectId_Failure()
        {
            var res = ScheduleSet.AddQuickSchedule(_mockServer, "display name", "", "",
                                    0, 100, true, true, true, true, true, false, false);
            Assert.IsFalse(res.Success, "Calling AddQuickSchedule with empty locaiton and subscriber ObjectID did not fail");

        }

        [TestMethod]
        public void GetSchedulesSets_NullConnectionServer_Failure()
        {
            //get schedule sets
            List<ScheduleSet> oSets;

            WebCallResult res = ScheduleSet.GetSchedulesSets(null, out oSets);
            Assert.IsFalse(res.Success,"Getting schedule sets with null ConnectionServerRest did not fail");
        }

        #endregion
    }
}
