using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ScheduleUnitTests : BaseUnitTests
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


        #region Class Creation Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Schedule_Constructor_NullConnectionServer_Failure()
        {
            Schedule oTest = new Schedule(null);
            Console.WriteLine(oTest);
        }
        
        
        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Schedule_Constructor_InvalidConnectionServer_Failure()
        {
            Schedule oTest = new Schedule(new ConnectionServerRest(new RestTransportFunctions()),"ObjectId");
            Console.WriteLine(oTest);
        }


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ScheduleDetail_Constructor_NullConnectionServer_Failure()
        {
            ScheduleDetail oTest = new ScheduleDetail(null);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failures 

        [TestMethod]
        public void GetSchedules_NullConnectionServer_Failure()
        {
            //get schedules
            List<Schedule> oSchedules;

            var res = Schedule.GetSchedules(null, out oSchedules);
            Assert.IsFalse(res.Success, "Static call to getSchedules with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void AddSchedule_NullConnectionServer_Failure()
        {
            var res = Schedule.AddSchedule(null, "bogus", "bogus", "", false);
            Assert.IsFalse(res.Success, "Static call to create new schedule with null Connection server did not fail");
        }

        [TestMethod]
        public void AddSchedule_EmptyName_Failure()
        {
            var res = Schedule.AddSchedule(_mockServer, "", "OwnerLocationObjectId", "", false);
            Assert.IsFalse(res.Success, "Static call to create new schedule with empty name did not fail");
        }

        [TestMethod]
        public void AddSchedule_EmptyOwnerFields_Failure()
        {
            var res = Schedule.AddSchedule(_mockServer, "DisplayName", "", "", false);
            Assert.IsFalse(res.Success, "Static call to create new schedule with empty location and user owner fields did not fail");
        }

        [TestMethod]
        public void DeleteSchedule_NullConnectionServer_Failure()
        {
            var res = Schedule.DeleteSchedule(null, "");
            Assert.IsFalse(res.Success, "Static call to delete schedule with null connection server did not fail");
        }

        [TestMethod]
        public void DeleteSchedule_BlankObjectId_Failure()
        {
            var res = Schedule.DeleteSchedule(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to delete schedule with empty objectId did not fail");
        }

        [TestMethod]
        public void AddScheduleDetail_NullConnectionServer_Failure()
        {
            var res = Schedule.AddScheduleDetail(null, "ObjectId", "subject", 0, 200, true, true, true,
                                                 true, true, false, false);
            Assert.IsFalse(res.Success, "Static call to addScheduleDetail with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void AddScheduleDetail_EmptyObjectId_Failure()
        {
            var res = Schedule.AddScheduleDetail(_mockServer, "", "subject", 0, 200, true, true, true,
                                 true, true, false, false);
            Assert.IsFalse(res.Success, "Static call to addScheduleDetail with empty ScheduleObjectId did not fail");
        }

        [TestMethod]
        public void DeleteScheduleDetail_NullConnectionServer_Failure()
        {
            var res = ScheduleDetail.DeleteScheduleDetail(null, "scheduleobjectId", "detailobjectid");
            Assert.IsFalse(res.Success, "Static call to DeleteScheduleDetail did not fail with null Connection server");
        }

        [TestMethod]
        public void DeleteScheduleDetail_EmptyScheduleObjectId_Failure()
        {
            var res = ScheduleDetail.DeleteScheduleDetail(_mockServer, "", "detailobjectid");
            Assert.IsFalse(res.Success, "Static call to DeleteScheduleDetail did not fail with blank schedule ObjectId");
        }

        [TestMethod]
        public void DeleteScheduleDetail_EmptyDetailObjectId_Failure()
        {
            var res = ScheduleDetail.DeleteScheduleDetail(_mockServer, "scheduleId", "");
            Assert.IsFalse(res.Success, "Static call to DeleteScheduleDetail did not fail with blank objectId ");
        }


        [TestMethod]
        public void GetSchedule_NullConnectionServer_Failure()
        {
            Schedule oSchedule;

            var res = Schedule.GetSchedule(out oSchedule, null);
            Assert.IsFalse(res.Success, "Static call to get schedule with null ConnectionServerRest did not fail");
        }


        [TestMethod]
        public void GetSchedule_NullConnectionServerWithObjectId_Failure()
        {
            Schedule oSchedule; 
            var res = Schedule.GetSchedule(out oSchedule, null, "ObjectId");
            Assert.IsFalse(res.Success, "Static call to get schedule with null ConnectionServerRest did not fail");
        }


        [TestMethod]
        public void GetSchedule_EmptyObjectIdAndName_Failure()
        {
            Schedule oSchedule; 
            var res = Schedule.GetSchedule(out oSchedule, _mockServer);
            Assert.IsFalse(res.Success, "Static call to get schedule with empty objectId and name did not fail");
        }


        [TestMethod]
        public void GetSchedule_NullConnectionServerWithDisplayName_Failure()
        {
            Schedule oSchedule; 
            var res = Schedule.GetSchedule(out oSchedule, null, "", "bogus display name");
            Assert.IsFalse(res.Success, "Static call to get schedule with null ConnectionServerRest did not fail");
        }


        #endregion

    }
}
