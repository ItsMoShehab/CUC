using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ScheduleDetail_Constructor_NullConnectionServer_Failure()
        {
            ScheduleDetail oTest = new ScheduleDetail(null);
            Console.WriteLine(oTest);
        }

        
        [TestMethod]
        public void ScheduleDetail_Constructor_EmptyObjectIds_Success()
        {
            ScheduleDetail oTest = new ScheduleDetail(_mockServer);
            Console.WriteLine(oTest.DumpAllProps());
        }

        [TestMethod]
        public void ScheduleDetail_Constructor_Default_Success()
        {
            ScheduleDetail oTest = new ScheduleDetail();
            Console.WriteLine(oTest.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ScheduleDetail_Constructor_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = false,
                                           ResponseText = "error text",
                                           StatusCode = 404
                                       });
            ScheduleDetail oTest = new ScheduleDetail(_mockServer,"ScheduleObjectId","ScheduleDetailObjectId");
            Console.WriteLine(oTest);
        }




        [TestMethod]
        public void ScheduleSetMember_Constructor_Empty_Success()
        {
            var oMember = new ScheduleSetMember();
            Console.WriteLine(oMember.ToString());
        }

        [TestMethod]
        public void ScheduleSetMember_Constructor_ObjectId_Success()
        {
            var oMember = new ScheduleSetMember("ObjectId");
            Console.WriteLine(oMember);
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


        [TestMethod]
        public void GetScheduleDetail_NullConnectionServerWithDisplayName_Failure()
        {
            List<ScheduleDetail> oScheduleDetails;
            var res = ScheduleDetail.GetScheduleDetails(null,"ScheduleObjectId", out oScheduleDetails);
            Assert.IsFalse(res.Success, "Static call to GetScheduleDetails with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void GetScheduleDetail_EmptyObjectId_Failure()
        {
            List<ScheduleDetail> oScheduleDetails;
            var res = ScheduleDetail.GetScheduleDetails(_mockServer, "", out oScheduleDetails);
            Assert.IsFalse(res.Success, "Static call to GetScheduleDetails with empty ObjectId did not fail");
        }


        [TestMethod]
        public void GetDetails_EmptyObjectId_Failure()
        {
            List<ScheduleDetail> oScheduleDetails;
            oScheduleDetails= ScheduleDetail.GetDetails(_mockServer, "");
            Assert.IsTrue(oScheduleDetails.Count == 0, "Static call to GetDetails with empty ObjectId should return empty list");
        }


        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetScheduleDetails_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<ScheduleDetail> oScheduleDetails;
            var res = ScheduleDetail.GetScheduleDetails(_mockServer, "ScheduleObjectId", out oScheduleDetails);
            Assert.IsFalse(res.Success, "Calling GetScheduleDetails with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetScheduleDetails_GarbageResponse_Failure()
        {
            List<CallHandler> oHandlers;
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<ScheduleDetail> oScheduleDetails;
            var res = ScheduleDetail.GetScheduleDetails(_mockServer, "ScheduleObjectId", out oScheduleDetails);
            Assert.IsFalse(res.Success, "Calling GetScheduleDetails with garbage results should fail");
            Assert.IsTrue(oScheduleDetails.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetScheduleDetails_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<ScheduleDetail> oScheduleDetails;
            var res = ScheduleDetail.GetScheduleDetails(_mockServer, "ScheduleObjectId", out oScheduleDetails);
            Assert.IsFalse(res.Success, "Calling GetScheduleDetails with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetScheduleDetails_ZeroCount_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<ScheduleDetail> oScheduleDetails;
            var res = ScheduleDetail.GetScheduleDetails(_mockServer, "ScheduleObjectId", out oScheduleDetails);
            Assert.IsTrue(res.Success, "Calling GetScheduleDetails with ZeroCount failed:" + res);
        }


        [TestMethod]
        public void DeleteScheduleDetail_Instance_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ScheduleDetail oDetail = new ScheduleDetail(_mockServer);
            var res = oDetail.Delete();
            Assert.IsFalse(res.Success, "Calling DeleteScheduleDetail with ErrorResponse did not fail");
        }

        [TestMethod]
        public void DeleteScheduleDetail_Static_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = ScheduleDetail.DeleteScheduleDetail(_mockServer, "ScheduleObjectId", "ScheduleDetailObjectId");
            Assert.IsFalse(res.Success, "Calling DeleteScheduleDetail with ErrorResponse did not fail");
        }


        /// <summary>
        /// populate an instance of ScheduleDetail with JSON generated for the options passed in.
        /// </summary>
        private ScheduleDetail CreateScheduleDetailHelper(bool pActiveMonday, bool pActiveTuesday, bool pActiveWednesday, bool pActiveThursday, 
            bool pActiveFriday, bool pActiveSaturday, bool pActiveSunday, int pStartMinute, int pEndMinute, bool pTillEndOfDay, 
            DateTime? pStartDate, DateTime? pEndDate)
        {
            string strJson =string.Format("\"ObjectId\":\"objectId\"," +
                            "\"ScheduleObjectId\":\"ScheduleObjectId\"," +
                            "\"IsActiveMonday\":\"{0}\"," +
                            "\"IsActiveTuesday\":\"{1}\"," +
                            "\"IsActiveWednesday\":\"{2}\"," +
                            "\"IsActiveThursday\":\"{3}\"," +
                            "\"IsActiveFriday\":\"{4}\"," +
                            "\"IsActiveSaturday\":\"{5}\"," +
                            "\"IsActiveSunday\":\"{6}\"," +
                            "\"StartTime\":\"{7}\"," +
                            "\"EndTime\":\"{8}\"," +
                            "\"EndOfDay\":\"{9}\"," +
                            "\"StartDate\":\"{10}\"," +
                            "\"EndDate\":\"{11}\","
                            ,pActiveMonday, pActiveTuesday,pActiveWednesday,pActiveThursday,pActiveFriday,pActiveSaturday,
                            pActiveSunday, pStartMinute,pEndMinute,pTillEndOfDay,pStartDate,pEndDate);

            strJson = "{" + strJson + "}";
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                   {
                                       Success = true,
                                       ResponseText = strJson,
                                   });
            
            ScheduleDetail oDetail=null;
            try
            {
                oDetail = new ScheduleDetail(_mockServer, "ScheduleObjectId", "ObjectId");
                
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed creating schedule detail from raw JSON:"+ex);
            }

            return oDetail;
        }

        [TestMethod]
        public void IsScheduleDetailActive_StartStopTimeTest()
        {
            DateTime oTestTime = DateTime.Parse("1/1/2013 5:20:00 pm");
            
            //active every day from noon to 2pm
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, true, true, 
                Schedule.GetMinutesFromTimeParts(12, 0), Schedule.GetMinutesFromTimeParts(16, 0),false,
                oTestTime, oTestTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTestTime, false);
            Assert.IsFalse(oResult,"Schedule should not be active at 5:20 pm");

            oTestTime = DateTime.Parse("1/1/2013 1:20:00 pm");

            oResult = oDetail.IsScheduleDetailActive(oTestTime, false);
            Assert.IsTrue(oResult, "Schedule should be active at 1:20 pm");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Monday_Active()
        {
            //Monday
            DateTime oTime = DateTime.Parse("7/8/2013 4:20:00 pm");

            //active during m-f only
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, false, false,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsTrue(oResult, "Schedule should be active on Monday");
        }
        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Tuesday_Active()
        {
            //Tuesday
            DateTime oTime = DateTime.Parse("7/9/2013 4:20:00 pm");

            //active during m-f only
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, false, false,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsTrue(oResult, "Schedule should be active on Tuesday");
        }
        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Wed_Active()
        {
            //Wed
            DateTime oTime = DateTime.Parse("7/10/2013 4:20:00 pm");

            //active during m-f only
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, false, false,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsTrue(oResult, "Schedule should not be active on Wed");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Thursday_Active()
        {
            //Thursday
            DateTime oTime = DateTime.Parse("7/11/2013 4:20:00 pm");

            //active during m-f only
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, false, false,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsTrue(oResult, "Schedule should be active on Thursday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Friday_Active()
        {
            //friday
            DateTime oTime = DateTime.Parse("7/12/2013 4:20:00 pm");

            //active during m-f only
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, false, false,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsTrue(oResult, "Schedule should be active on Friday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Saturday_Incactive()
        {
            //saturday
            DateTime oTime = DateTime.Parse("7/13/2013 4:20:00 pm");

            //active during m-f only
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, false, false,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsFalse(oResult, "Schedule should not be active on Saturday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Sunday_Inactive()
        {
            //sunday
            DateTime oTime = DateTime.Parse("7/14/2013 4:20:00 pm");

            //active during m-f only
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, false, false,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsFalse(oResult, "Schedule should not be active on Sunday");
        }


        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Monday_Inactive()
        {
            //Monday
            DateTime oTime = DateTime.Parse("7/8/2013 4:20:00 pm");

            //active during sat,sun only
            var oDetail = CreateScheduleDetailHelper(false, false, false, false, false, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsFalse(oResult, "Schedule should not be active on Monday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Tuesday_Inactive()
        {
            //Tuesday
            DateTime oTime = DateTime.Parse("7/9/2013 4:20:00 pm");

            //active during sat,sun only
            var oDetail = CreateScheduleDetailHelper(false, false, false, false, false, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsFalse(oResult, "Schedule should not be active on Tuesday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Wed_Inactive()
        {
            //Wed
            DateTime oTime = DateTime.Parse("7/10/2013 4:20:00 pm");

            //active during sat,sun only
            var oDetail = CreateScheduleDetailHelper(false, false, false, false, false, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsFalse(oResult, "Schedule should not be active on Wed");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Thursday_Inactive()
        {
            //Thursday
            DateTime oTime = DateTime.Parse("7/11/2013 4:20:00 pm");

            //active during sat,sun only
            var oDetail = CreateScheduleDetailHelper(false, false, false, false, false, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsFalse(oResult, "Schedule should not be active on Thursday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Friday_Inactive()
        {
            //friday
            DateTime oTime = DateTime.Parse("7/12/2013 4:20:00 pm");

            //active during sat,sun only
            var oDetail = CreateScheduleDetailHelper(false, false, false, false, false, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsFalse(oResult, "Schedule should not be active on Friday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Saturday_Active()
        {
            //saturday
            DateTime oTime = DateTime.Parse("7/13/2013 4:20:00 pm");

            //active during sat,sun only
            var oDetail = CreateScheduleDetailHelper(false, false, false, false, false, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsTrue(oResult, "Schedule should be active on Saturday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_ActiveDayTest_Sunday_Active()
        {
            //sunday
            DateTime oTime = DateTime.Parse("7/14/2013 4:20:00 pm");

            //active during sat,sun only
            var oDetail = CreateScheduleDetailHelper(false, false, false, false, false, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(23, 0), false, oTime, oTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTime, false);
            Assert.IsTrue(oResult, "Schedule should be active on Sunday");
        }


        [TestMethod]
        public void IsScheduleDetailActive_EndOfDayTest_NotActive()
        {
            DateTime oTestTime = DateTime.Parse("1/1/2013 5:20:00 pm");

            //active every day from noon to 2pm
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(2, 0), false, oTestTime, oTestTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTestTime, false);
            Assert.IsFalse(oResult, "Schedule should not be active at 5:20 pm");
        }

        [TestMethod]
        public void IsScheduleDetailActive_EndOfDayTest_Active()
        {
            DateTime oTestTime = DateTime.Parse("1/1/2013 5:20:00 pm");

            //active every day from noon to 2pm
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(2, 0), true, oTestTime, oTestTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTestTime, false);
            Assert.IsTrue(oResult, "Schedule should be active at 5:20 pm");
        }


        [TestMethod]
        public void IsScheduleDetailActive_Holiday_InActive()
        {
            DateTime oHolidayStartTime = DateTime.Parse("1/1/2013 5:20:00 pm");
            DateTime oHolidayEndTime = DateTime.Parse("1/2/2013 5:20:00 pm");
            DateTime oTestTime = DateTime.Parse("1/1/2013 2:20:00 pm");
            //active every day from noon to 2pm
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(2, 0), true, oHolidayStartTime, oHolidayEndTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTestTime, true);
            Assert.IsTrue(oResult, "Schedule should be active due to holiday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_Holiday_Active()
        {
            DateTime oHolidayStartTime = DateTime.Parse("1/1/2013 5:20:00 pm");
            DateTime oHolidayEndTime = DateTime.Parse("1/2/2013 5:20:00 pm");
            DateTime oTestTime = DateTime.Parse("1/3/2013 2:20:00 pm");
            
            //active every day from noon to 2pm
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(2, 0), true, oHolidayStartTime, oHolidayEndTime);

            bool oResult = oDetail.IsScheduleDetailActive(oTestTime, true);
            Assert.IsFalse(oResult, "Schedule should not be active due to holiday");
        }

        [TestMethod]
        public void IsScheduleDetailActive_Holiday_ActiveAllTimes()
        {
            DateTime oTestTime = DateTime.Parse("1/3/2013 2:20:00 pm");

            //active every day from noon to 2pm
            var oDetail = CreateScheduleDetailHelper(true, true, true, true, true, true, true,
                Schedule.GetMinutesFromTimeParts(1, 0), Schedule.GetMinutesFromTimeParts(2, 0), true, null, null);

            bool oResult = oDetail.IsScheduleDetailActive(oTestTime, true);
            Assert.IsTrue(oResult, "Schedule should be active due to Holiday");
        }
        #endregion

    }
}
