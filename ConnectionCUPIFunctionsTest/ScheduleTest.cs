using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ScheduleTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static Schedule _tempSchedule;
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
                throw new Exception("Unable to attach to Connection server to start Schedule test:" + ex.Message);
            }

            

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

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            Schedule oTest = new Schedule(null);
            Console.WriteLine(oTest);
        }
        
        
        /// <summary>
        /// Make sure an Exception is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            Schedule oTest = new Schedule(new ConnectionServer(),"blah");
            Console.WriteLine(oTest);
        }


        /// <summary>
        /// Make sure an Exception is thrown if an invalid ObjectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            Schedule oTest = new Schedule(_connectionServer,"blah");
            Console.WriteLine(oTest);
        }


        /// <summary>
        /// Make sure an Exception is thrown if an invalid ObjectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure4()
        {
            ScheduleDetail oDetail = new ScheduleDetail(_connectionServer,"blah","blah");
            Console.WriteLine(oDetail);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure5()
        {
            ScheduleDetail oTest = new ScheduleDetail(null);
            Console.WriteLine(oTest);
        }

        #endregion


        [TestMethod]
        public void ScheduleTests()
        {
            //cover all lists on the server - there must be at least one
            List<Schedule> oSchedules;
            WebCallResult res = Schedule.GetSchedules(_connectionServer, out oSchedules);
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
        }

        [TestMethod]
        public void StaticScheduleTests()
        {
            Schedule oSchedule;

            //getSchedule
            WebCallResult res = Schedule.GetSchedule(out oSchedule, null);
            Assert.IsFalse(res.Success,"Static call to get schedule with null ConnectionServer did not fail");

            res = Schedule.GetSchedule(out oSchedule, null, "bogus");
            Assert.IsFalse(res.Success, "Static call to get schedule with null ConnectionServer did not fail");

            res = Schedule.GetSchedule(out oSchedule, _connectionServer);
            Assert.IsFalse(res.Success, "Static call to get schedule with empty objectId and name did not fail");

            res = Schedule.GetSchedule(out oSchedule, null, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to get schedule with null ConnectionServer did not fail");

            //get schedules
            List<Schedule> oSchedules;
            
            res = Schedule.GetSchedules(null, out oSchedules);
            Assert.IsFalse(res.Success, "Static call to getSchedules with null ConnectionServer did not fail");

            res = Schedule.GetSchedules(_connectionServer, out oSchedules);
            Assert.IsTrue(res.Success, "Static call to getSchedules with null ConnectionServer did not fail");
            Assert.IsTrue(oSchedules.Count>0,"No schedules returned in fetch:"+res);

            //Add Schedule
            res = Schedule.AddSchedule(null, "bogus", "bogus", "", false);
            Assert.IsFalse(res.Success,"Static call to create new schedule with null Connection server did not fail");

            res = Schedule.AddSchedule(_connectionServer, "", "bogus", "", false);
            Assert.IsFalse(res.Success, "Static call to create new schedule with empty name did not fail");

            res = Schedule.AddSchedule(_connectionServer, _connectionServer.PrimaryLocationObjectId, "", "", false);
            Assert.IsFalse(res.Success, "Static call to create new schedule with empty location did not fail");

            res = Schedule.AddSchedule(_connectionServer, _connectionServer.PrimaryLocationObjectId, "bogus","bogus", false);
            Assert.IsFalse(res.Success, "Static call to create new schedule with invalid user objectId owner did not fail");

            //Delete Schedule
            res = Schedule.DeleteSchedule(null, "");
            Assert.IsFalse(res.Success, "Static call to delete schedule with null connection server did not fail");

            res = Schedule.DeleteSchedule(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static call to delete schedule with empty objectId did not fail");

            //add schedule details
            res = Schedule.AddScheduleDetail(null,oSchedules[0].ObjectId,"subject", 0, 200, true, true, true,
                                             true, true, false, false);
            Assert.IsFalse(res.Success, "Static call to addScheduleDetail with null ConnectionServer did not fail");

            res = Schedule.AddScheduleDetail(_connectionServer, "", "subject", 0, 200, true, true, true,
                                 true, true, false, false);
            Assert.IsFalse(res.Success, "Static call to addScheduleDetail with empty ScheduleObjectId did not fail");


            //schedule detail
            res = ScheduleDetail.DeleteScheduleDetail(null, "scheduleobjectId", "detailobjectid");
            Assert.IsFalse(res.Success,"Static call to DeleteScheduleDetail did not fail with null Connection server");

            res = ScheduleDetail.DeleteScheduleDetail(_connectionServer, "", "detailobjectid");
            Assert.IsFalse(res.Success, "Static call to DeleteScheduleDetail did not fail with blank schedule ObjectId");

            res = ScheduleDetail.DeleteScheduleDetail(_connectionServer, "scheduleId", "");
            Assert.IsFalse(res.Success, "Static call to DeleteScheduleDetail did not fail with blank objectId ");

            //get time parts
            int iMinutes= Schedule.GetMinutesFromTimeParts(2, 10);
            Assert.IsTrue(iMinutes==130,"GetMinutesFromTimeParts did not return 130 for 2 hours and 10 minutes");

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
    }
}
