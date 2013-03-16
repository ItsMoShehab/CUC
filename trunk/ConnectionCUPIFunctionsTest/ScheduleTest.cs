using System;
using System.Collections.Generic;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ScheduleTest
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


        #region Class Creation Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            Schedule oTest = new Schedule(null);
        }
        
        
        /// <summary>
        /// Make sure an Exception is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            Schedule oTest = new Schedule(new ConnectionServer(),"blah");
        }


        /// <summary>
        /// Make sure an Exception is thrown if an invalid ObjectId is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            Schedule oTest = new Schedule(_connectionServer,"blah");
        }


        /// <summary>
        /// Make sure an Exception is thrown if an invalid ObjectId is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure4()
        {
            ScheduleDetail oDetail = new ScheduleDetail(_connectionServer,"blah","blah");
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure5()
        {
            ScheduleDetail oTest = new ScheduleDetail(null);
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
                Console.WriteLine(oSchedules.ToString());
                Console.WriteLine(oSchedule.DumpAllProps());
                strObjectId = oSchedule.ObjectId;
            }

            //create a new schedule instance from objectId
            Schedule oNewSchedule;
            try
            {
                oNewSchedule = new Schedule(_connectionServer, strObjectId);
                ScheduleState oState = oNewSchedule.GetScheduleState(DateTime.Now);
                Assert.IsNotNull(oState, "Failed to fetch schedule state from schedule objectId:" + strObjectId);

                Console.WriteLine(oNewSchedule.ToString());

                foreach (var oDetail in oNewSchedule.ScheduleDetails())
                {
                    Console.WriteLine(oDetail.ToString());
                    Console.WriteLine(oDetail.DumpAllProps());
                }
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false,"Failed to create new schedule instance from ObjectId:"+ex);
            }

            try
            {
                oNewSchedule = new Schedule(_connectionServer,"","blah");
                Assert.IsTrue(false,"Creating schedule class with invalid schedule name should throw an exception");
            }
            catch{}
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
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false,"Failed to create a ScheduleDetial instance with valid objectIds:"+ex);
            }
        }
    }
}
