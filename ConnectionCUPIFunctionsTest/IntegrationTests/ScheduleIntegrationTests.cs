using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ScheduleIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static Schedule _tempSchedule;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            WebCallResult res = Schedule.AddSchedule(_connectionServer, "Temp_" + Guid.NewGuid().ToString(),
                                                     _connectionServer.PrimaryLocationObjectId, "", false, out _tempSchedule);
            Assert.IsTrue(res.Success, "Failed to create new system schedule:" + res);
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempSchedule != null)
            {
                WebCallResult res = _tempSchedule.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary schedule on cleanup.");
            }
        }

        #endregion


        #region Class Creation Tests

       
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Schedule_Constructor_InvalidObjectId_Failure()
        {
            Schedule oTest = new Schedule(_connectionServer,"ObjectId");
            Console.WriteLine(oTest);
        }


        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Schedule_Constructor_InvalidDisplayName_Failure()
        {
            Schedule oTest = new Schedule(_connectionServer, "","bogus Display Name");
            Console.WriteLine(oTest);
        }


        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ObjectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ScheduleDetail_Constructor_InvalidObjectIds_Failure()
        {
            ScheduleDetail oDetail = new ScheduleDetail(_connectionServer,"ScheduleObjectId","ScheduleDetailObjectId");
            Console.WriteLine(oDetail);
        }

        #endregion


        #region Static Call Failures 

        [TestMethod]
        public void AddSchedule_InvalidOwnerLocationObjectId_Failure()
        {
            var res = Schedule.AddSchedule(_connectionServer, _connectionServer.PrimaryLocationObjectId, "OwnerLocationObjectID", "", false);
            Assert.IsFalse(res.Success, "Static call to create new schedule with invalid user objectId owner did not fail");
        }

        [TestMethod]
        public void AddSchedule_InvalidOwnerSubscriberObjectId_Failure()
        {
            var res = Schedule.AddSchedule(_connectionServer, _connectionServer.PrimaryLocationObjectId, "", "OwnerSubscriberObjectId", false);
            Assert.IsFalse(res.Success, "Static call to create new schedule with invalid user objectId owner did not fail");
        }
        

        [TestMethod]
        public void DeleteSchedule_InvalidObjectId_Failure()
        {
            var res = Schedule.DeleteSchedule(_connectionServer, "ScheduleObjectId");
            Assert.IsFalse(res.Success, "Static call to delete schedule with empty objectId did not fail");
        }

        [TestMethod]
        public void GetSchedules_Success()
        {
            List<Schedule> oSchedules;
            var res = Schedule.GetSchedules(_connectionServer, out oSchedules);
            Assert.IsTrue(res.Success, "Static call to getSchedules with null ConnectionServerRest did not fail");
            Assert.IsTrue(oSchedules.Count > 0, "No schedules returned in fetch:" + res);
        }

        [TestMethod]
        public void DeleteScheduleDetail_InvalidObjectIds_Failure()
        {
            var res = ScheduleDetail.DeleteScheduleDetail(_connectionServer, "scheduleId", "ScheduleDetailObjectId");
            Assert.IsFalse(res.Success, "Static call to DeleteScheduleDetail did not fail with blank objectId ");
        }


        [TestMethod]
        public void GetSchedule_InvalidObjectId_Failure()
        {
            Schedule oSchedule;
            var res = Schedule.GetSchedule(out oSchedule, _connectionServer, "ObjectId");
            Assert.IsFalse(res.Success, "Static call to get schedule with invalid ObjectId did not fail");
        }

        [TestMethod]
        public void GetSchedule_InvalidDisplayName_Failure()
        {
            Schedule oSchedule;
            var res = Schedule.GetSchedule(out oSchedule, _connectionServer, "","Bogus Display Name");
            Assert.IsFalse(res.Success, "Static call to get schedule with invalid display name did not fail");
        }

        #endregion


        #region Live Test 

        [TestMethod]
        public void TimePartsTest()
        {
            //get time parts
            int iMinutes = Schedule.GetMinutesFromTimeParts(2, 10);
            Assert.IsTrue(iMinutes == 130, "GetMinutesFromTimeParts did not return 130 for 2 hours and 10 minutes");
        }

        [TestMethod]
        public void ScheduleFetchAndCreateTests()
        {
            //cover all lists on the server - there must be at least one
            List<Schedule> oSchedules;
            WebCallResult res = Schedule.GetSchedules(_connectionServer, out oSchedules,1,2,null);
            Assert.IsTrue(res.Success,"Failed to fetch schedule list:"+res.ToString());

            string strObjectId="";
            foreach (var oSchedule in oSchedules)
            {
                Console.WriteLine(oSchedule.ToString());
                Console.WriteLine(oSchedule.DumpAllProps());
                
                List<ScheduleDetail> oDetails;
                oSchedule.GetScheduleDetails(out oDetails);
                
                Console.WriteLine(oSchedule.GetScheduleState(DateTime.Now));

                strObjectId = oSchedule.ObjectId;
            }

            //create a new schedule instance from objectId
            Schedule oNewSchedule;
            try
            {
                oNewSchedule = new Schedule(_connectionServer, strObjectId);
                ScheduleState oState = oNewSchedule.GetScheduleState(DateTime.Now);
                Assert.IsNotNull(oState, "Failed to fetch schedule state from schedule objectId:" + strObjectId);

               
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false,"Failed to create new schedule instance from ObjectId:"+ex);
            }

            Schedule oTempSchedule;
            res = Schedule.GetSchedule(out oTempSchedule, _connectionServer, "", oSchedules[0].DisplayName);
            Assert.IsTrue(res.Success, "Failed to fetch schedule by valid name:" + res);

            try
            {
                oNewSchedule = new Schedule(_connectionServer, "", "blah");
                Assert.IsTrue(false, "Creating schedule class with invalid schedule name should throw an exception");
                Console.WriteLine(oNewSchedule);
            }
            catch (Exception)
            {
                Console.WriteLine("Expected error for creation failure");
            }

            res = Schedule.GetSchedule(out oNewSchedule, _connectionServer, "", "bogus");
            Assert.IsFalse(res.Success,"Fetching schedule by invalid name did not fail");


            res = Schedule.GetSchedules(_connectionServer, out oSchedules,1,2,"query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching schedules with invalid query should not fail:" + res);
            Assert.IsTrue(oSchedules.Count == 0, "Invalid query string should return an empty schedules list:" + oSchedules.Count);

        }


        [TestMethod]
        public void AddRemoveScheduleTest()
        {
            WebCallResult res = _tempSchedule.AddScheduleDetail("test subject", 0, 100, true, true, true, true, true, false, false,
                                                     DateTime.Now, DateTime.Now.AddDays(1));
            Assert.IsTrue(res.Success, "Failed to create new schedule detail and add it to the new schedule:" + res);

            Console.WriteLine(_tempSchedule.ToString());
            _tempSchedule.RefetchScheduleData();


            foreach (var oDetail in _tempSchedule.ScheduleDetails())
            {
                Console.WriteLine(oDetail.ToString());
                Console.WriteLine(oDetail.DumpAllProps());
            }

            //create a holiday
            Schedule oSchedule;
            res = Schedule.AddSchedule(_connectionServer, "temp_" + Guid.NewGuid().ToString(),
                                       _connectionServer.PrimaryLocationObjectId, "", true, out oSchedule);
            Assert.IsTrue(res.Success, "Failed to create new holiday system schedule:" + res);

            res = oSchedule.Delete();
            Assert.IsTrue(res.Success, "Failed to delete delete system schedule:" + res);

        }


        [TestMethod]
        public void ScheduleDetailTests()
        {
            //cover all lists on the server - there must be at least one
            List<Schedule> oSchedules;
            WebCallResult res = Schedule.GetSchedules(_connectionServer, out oSchedules);
            Assert.IsTrue(res.Success, "Failed to fetch schedule list:" + res.ToString());

            string strScheduleObjectId="";

            foreach (var oSchedule in oSchedules)
            {
                if (oSchedule.ScheduleDetails().Count > 0)
                {
                    strScheduleObjectId = oSchedule.ObjectId;
                    break;
                }
            }
            
            Assert.IsFalse(string.IsNullOrEmpty(strScheduleObjectId),"Failed to find a schedule with at least one detail element");
            
            List<ScheduleDetail> oScheduleDetails;
            string strScheduleDetailObjectId = "";

            res =ScheduleDetail.GetScheduleDetails(_connectionServer, strScheduleObjectId, out oScheduleDetails);
            Assert.IsTrue(res.Success, "Failed to fetch schedule details from schedule:"+res);

            foreach (var oDetail in oScheduleDetails)
            {
                Console.WriteLine(oDetail.ToString());
                Console.WriteLine(oDetail.DumpAllProps());

                oDetail.IsScheduleDetailActive(DateTime.Now, true);
                oDetail.IsScheduleDetailActive(DateTime.Now, false);

                oDetail.IsScheduleDetailActive(DateTime.Now+TimeSpan.FromDays(1), false);
                oDetail.IsScheduleDetailActive(DateTime.Now + TimeSpan.FromDays(2), false);
                oDetail.IsScheduleDetailActive(DateTime.Now + TimeSpan.FromDays(3), false);
                oDetail.IsScheduleDetailActive(DateTime.Now + TimeSpan.FromDays(4), false);
                oDetail.IsScheduleDetailActive(DateTime.Now + TimeSpan.FromDays(5), false);
                oDetail.IsScheduleDetailActive(DateTime.Now + TimeSpan.FromDays(6), false);
                strScheduleDetailObjectId = oDetail.ObjectId;
            }

            res = ScheduleDetail.GetScheduleDetails(null, strScheduleObjectId, out oScheduleDetails);
            Assert.IsFalse(res.Success,"Schedule detail fetch with null Connection server should fail");

            try
            {
                ScheduleDetail oScheduleDetail = new ScheduleDetail(_connectionServer, strScheduleObjectId,
                                                                    strScheduleDetailObjectId);
                Console.WriteLine(oScheduleDetail);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false,"Failed to create a ScheduleDetial instance with valid objectIds:"+ex);
            }
        }

        #endregion
    }
}
