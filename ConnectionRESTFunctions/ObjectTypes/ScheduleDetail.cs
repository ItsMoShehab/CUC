#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    public class ScheduleDetail
    {

        #region Constructors and Destructors

        /// <summary>
        /// Creates a new instance of the ScheduleDetail class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this schedule detail.  
        /// If you pass the pScheduleDetailObjectId and pScheduleDetailObjectId parameter the schedule detail is automatically filled with data for that schedule 
        /// detail from the server.  
        /// If either are passed an empty instance of the ScheduleDetail class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the schedule 
        /// </param>
        /// <param name="pScheduleObjectId">
        /// Optional parameter for the unique ID of the schedule on the home server provided.  
        /// </param>
        /// <param name="pScheduleDetailObjectId">
        /// Optional schedule detail identifier 
        /// </param>
        public ScheduleDetail(ConnectionServer pConnectionServer, string pScheduleObjectId = "", string pScheduleDetailObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null Connection server passed to ScheduleDetail constructor");
            }

            //if the user passed in a specific ObjectId then go load that schedule up, otherwise just return an empty instance.
            if (string.IsNullOrEmpty(pScheduleObjectId) | string.IsNullOrEmpty(pScheduleDetailObjectId)) return;

            ObjectId = pScheduleDetailObjectId;
            ScheduleObjectId = pScheduleObjectId;
            HomeServer = pConnectionServer;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetScheduleDetail(pScheduleObjectId, pScheduleDetailObjectId);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res, string.Format("Schedule detail not found in ScheduleDetail constructor using " +
                                                                          "ScheduleObjectId={0} and ScheduleDetailObjectId={1}\n\r{2}", 
                                                                          pScheduleObjectId, pScheduleDetailObjectId, res.ErrorText));
            }
        }

        /// <summary>
        /// General constructor for Json libraries
        /// </summary>
        public ScheduleDetail()
        {
        }

        #endregion


        #region Fields and Properties 
        
        //reference to the ConnectionServer object used to create this Alternate Extension instance.
        public ConnectionServer HomeServer { get; private set; }

        #endregion


        #region ScheduleDetail Properties

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string ScheduleObjectId { get; private set; }

        [JsonProperty]
        public bool IsActiveMonday { get; private set; }

        [JsonProperty]
        public bool IsActiveTuesday { get; private set; }

        [JsonProperty]
        public bool IsActiveWednesday { get; private set; }

        [JsonProperty]
        public bool IsActiveThursday { get; private set; }

        [JsonProperty]
        public bool IsActiveFriday { get; private set; }

        [JsonProperty]
        public bool IsActiveSaturday { get; private set; }

        [JsonProperty]
        public bool IsActiveSunday { get; private set; }

        [JsonProperty]
        public int StartTime { get; private set; }

        [JsonProperty]
        public int EndTime { get; private set; }

        [JsonProperty]
        public string StartDate { get; private set; }

        [JsonProperty]
        public string EndDate { get; private set; }

        [JsonProperty]
        public string Subject { get; private set; }

        #endregion


        #region Instance Methods


        public override string ToString()
        {
            return string.Format("{0} [{1}]", Subject, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the schedule detail object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the schedule object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            var strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
        }


        /// <summary>
        /// Returns true or false if the schedule detail segment is active or inactive based on the date/time passed into the method and 
        /// an indication if the detail is part of a holiday schedule or not.  True means the schedule is active, false means it's inactive.
        /// </summary>
        /// <param name="pDateTime">
        /// Date and time to check against
        /// </param>
        /// <param name="pIsHoliday">
        /// Boolean indicating if the detail is part of a holiday schedule or not - this is not stored on the detail so it must be passed
        /// in from the schedule where the flag is kept.
        /// </param>
        /// <returns>
        /// True if the detail segment is active, false if it is not.
        /// </returns>
        public bool IsScheduleDetailActive(DateTime pDateTime, bool pIsHoliday)
        {
            //start/stop time is stored as minutes from midnight.
            int iMinutesFromMidnight = (int)pDateTime.TimeOfDay.TotalMinutes;

            //end time (minutes from midnight) is stored as 0 if it means till end of day - adjust here.
            if (this.EndTime == 0)
            {
                EndTime = 24 * 60; //end of day - 1440 minutes
            }

            //if this is a holiday schedule then there will be a start date and end date that we need to check - otherwise it 
            //will be a series of flags for which day(s) of the week the detail applies to and the start/end minutes.
            //the detail does not know if it's for a holiday, that's on the schedule - so it must be passed in as a flag here.
            if (pIsHoliday)
            {
                DateTime oStartDate;
                DateTime oEndDate;

                //the holiday start/end date is just the day to match - the start time/end time is stored in minutes from midnight and
                //needs to be calculated seperately - if this is a non holiday detail the start/end dates will be empty strings.
                
                //first check if we're on the right date
                if (DateTime.TryParse(this.StartDate, out oStartDate) == false)
                {
                    //null value means it's active all dates
                    oStartDate = DateTime.MinValue;
                }

                if (DateTime.TryParse(this.EndDate, out oEndDate) == false)
                {
                    //null means it's active all dates
                    oEndDate = DateTime.MaxValue;
                }

                //restrict check to just date segment of date/time
                if (pDateTime.Date < oStartDate.Date | pDateTime.Date > oEndDate.Date)
                {
                    return false;
                }

                //now check the time of day
                return (iMinutesFromMidnight >= StartTime & iMinutesFromMidnight <= EndTime);
            }

            //we're not a holiday

            //if the schedule is not active for the day of the week in question, we can return false without
            //further processing.
            switch (pDateTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (IsActiveMonday == false) return false;
                    break;
                case DayOfWeek.Tuesday:
                    if (IsActiveTuesday == false) return false;
                    break;
                case DayOfWeek.Wednesday:
                    if (IsActiveWednesday == false) return false;
                    break;
                case DayOfWeek.Thursday:
                    if (IsActiveThursday == false) return false;
                    break;
                case DayOfWeek.Friday:
                    if (IsActiveFriday == false) return false;
                    break;
                case DayOfWeek.Saturday:
                    if (IsActiveSaturday == false) return false;
                    break;
                case DayOfWeek.Sunday:
                    if (IsActiveSunday == false) return false;
                    break;
            }
            
            //return true if we are between the start/stop minutes from midnight
            return (iMinutesFromMidnight >= StartTime & iMinutesFromMidnight <= EndTime);
        }

        /// <summary>
        /// DELETE the schedule detail from the schedule that owns it.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public WebCallResult Delete()
        {
            return DeleteScheduleDetail(this.HomeServer, this.ScheduleObjectId, this.ObjectId);
        }


        /// <summary>
        /// Fills the current instance of ScheduleDetail in with properties fetched from the server.  Requires both the Id for the schedule and 
        /// the ID for the dtail be passed in
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetScheduleDetail(string pScheduleObjectId, string pScheduleDetailObjectId)
        {
            string strUrl = string.Format("{0}schedules/{1}/scheduledetails/{2}", HomeServer.BaseUrl, pScheduleObjectId,pScheduleDetailObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, HTTPFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }
            return res;
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all schedule details and resturns them as a generic list of ScheduleDetails objects. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the schedules should be pulled from
        /// </param>
        /// <param name="pScheduleObjectId">
        /// Schedule that the schedule detail item(s) are associated with.
        ///  </param>
        /// <param name="pScheduleDetails">
        /// Out parameter that is used to return the list of ScheduleDetails objects defined on Connection - there may be zero in which 
        /// case an empty list is returned.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetScheduleDetails(ConnectionServer pConnectionServer, string pScheduleObjectId, out List<ScheduleDetail> pScheduleDetails, 
            int pPageNumber = 1, int pRowsPerPage = 20)
        {
            WebCallResult res;
            pScheduleDetails = new List<ScheduleDetail>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetScheduleDetails";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(string.Format("{0}schedules/{1}/scheduledetails", pConnectionServer.BaseUrl, pScheduleObjectId),
                "pageNumber=" + pPageNumber, "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - schedules can be empty of details
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pScheduleDetails = new List<ScheduleDetail>();
                return res;
            }

            pScheduleDetails = HTTPFunctions.GetObjectsFromJson<ScheduleDetail>(res.ResponseText);

            if (pScheduleDetails == null)
            {
                pScheduleDetails = new List<ScheduleDetail>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pScheduleDetails)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ScheduleObjectId = pScheduleObjectId;
            }

            return res;
        }

        /// <summary>
        /// Remove a schedule detail from a schedule 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pScheduleObjectId">
        /// Schedule that owns the schedule detail to be removed
        /// </param>
        /// <param name="pScheduleDetailObjectId">
        /// ObjectId of the schedule detail to delete
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult DeleteScheduleDetail(ConnectionServer pConnectionServer, string pScheduleObjectId, string pScheduleDetailObjectId)
        {
            WebCallResult res;
            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Null ConnectionServer reference passed to DeleteScheduleDetail";
                return res;
            }

            if (string.IsNullOrEmpty(pScheduleDetailObjectId))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Empty objectId passed to DeleteScheduleDetail";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + string.Format("schedules/{0}/scheduledetails/{1}",
                pScheduleObjectId, pScheduleDetailObjectId),MethodType.DELETE, pConnectionServer, "");
        }


        /// <summary>
        /// Return a list of ScheduleDetail items for a particular schedule - there may be zero detail items for a schedule, that's
        /// legal - in that case an empty list is returned.
        /// </summary>
        /// <param name="pConnetionServer">
        /// Reference to the Connection server that we're fetching details from.
        /// </param>
        /// <param name="pScheduleObjectId">
        /// Schedule identifier that has the details we are fetching (if any)
        /// </param>
        /// <returns>
        /// List of ScheduleDetail objects - may be an empty list if there are no details.
        /// </returns>
        public static List<ScheduleDetail> GetDetails(ConnectionServer pConnetionServer,string pScheduleObjectId)
        {
            List<ScheduleDetail> oList;

            GetScheduleDetails(pConnetionServer, pScheduleObjectId, out oList);

            return oList;
        }

        #endregion
    }
}
