﻿#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    [Serializable]
    public class Schedule :IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// Creates a new instance of the Schedule class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this schedule.  
        /// If you pass the pObjectID or pDisplayName parameter the schedule is automatically filled with data for that schedule from the server.  
        /// If neither are passed an empty instance of the Schedule class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the schedule being created.
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the schedule on the home server provided.  If no ObjectId is passed then an empty instance of the schedule
        /// class is returned instead.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name search critiera - if both ObjectId and DisplayName are passed, ObjectId is used.  The display name search is not case
        /// sensitive.
        /// </param>
        public Schedule(ConnectionServerRest pConnectionServer, string pObjectId = "", string pDisplayName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to schedule constructor.");
            }

            HomeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or display name then go load that schedule up, otherwise just return an empty instance.
            if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pDisplayName))) return;

            ObjectId = pObjectId;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetSchedule(pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Schedule not found in Schedule constructor using ObjectId={0} " +
                                                                         "and DisplayName={1}\n\r{2}", pObjectId, pDisplayName, res.ErrorText));
            }
        }


        /// <summary>
        /// General constructor for Json parsing library
        /// </summary>
        public Schedule()
        {
        }

        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DisplayName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }


        //reference to the ConnectionServer object used to create this Alternate Extension instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //Every schedule has a list of schedule details which dictate the on/off time and day(s) the schedule is active.
        //there may be 0 there may be many - it's not required that a schedule have a detail item.
        private List<ScheduleDetail> _scheduleDetails; 

        #endregion


        #region Schedule Properties

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string OwnerSubscriberObjectId { get; private set; }

        [JsonProperty]
        public string DisplayName { get; private set; }

        [JsonProperty]
        public bool IsHoliday { get; private set; }

        [JsonProperty]
        public bool Undeletable { get; private set; }

        [JsonProperty]
        public string OwnerLocationObjectId { get; private set; }

        #endregion


        #region Instance Methods


        public override string ToString()
        {
            return string.Format("{0} [{1}], holiday={2}", DisplayName, ObjectId,IsHoliday);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the schedule object in "name=value" format - each pair is on its
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
        /// Pass in a date/time and this will return the schedule state for that date time - either active (on), inactive 
        /// (off) or holiday.
        /// </summary>
        /// <param name="pDateTime">
        /// Date/time to evaluate for - typicall DateTime.Now is passed but any date is valid.
        /// </param>
        /// <returns>
        /// ScheduleState enum value (INACTIVE, ACTIVE or HOLIDAY)
        /// </returns>
        public ScheduleState GetScheduleState(DateTime pDateTime)
         {
            foreach (ScheduleDetail oScheduleDetail in this.ScheduleDetails())
            {
                if (oScheduleDetail.IsScheduleDetailActive(pDateTime,IsHoliday))
                {
                    if (IsHoliday)
                    {
                        return ScheduleState.HOLIDAY;
                    }
                    return ScheduleState.ACTIVE;
                }
            }
            
            //if all details evaluate to inactive, return inactive.
            return ScheduleState.INACTIVE;
         }


        /// <summary>
        /// Fetch the list of schedule details associated with this schedule (if any)
        /// </summary>
        /// <param name="pScheduleDetails">
        /// Schedule details associated with the schedule are returned on this out paramter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult.
        /// </returns>
        public WebCallResult GetScheduleDetails(out List<ScheduleDetail> pScheduleDetails)
        {
            return ScheduleDetail.GetScheduleDetails(this.HomeServer, this.ObjectId, out pScheduleDetails);
        }

        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchScheduleData()
        {
            return GetSchedule(this.ObjectId);
        }
        
        /// <summary>
        /// Fills the current instance of Schedule in with properties fetched from the server.  If both the display name and ObjectId
        /// parameters are provided, the ObjectId is used for the search.
        /// </summary>
        /// <param name="pObjectId">
        /// Unique GUID of the schedule to fetch - can be blank if the display name is passed in.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to search on a schedule by.  Can be blank if the ObjectId parameter is provided.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetSchedule(string pObjectId, string pDisplayName = "")
        {
            string strObjectId = pObjectId;

            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdFromName(pDisplayName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "No schedule found by name="+pDisplayName
                    };
                }
            }

            string strUrl = string.Format("{0}schedules/{1}", HomeServer.BaseUrl, strObjectId);

            return HomeServer.FillObjectWithRestGetResults(strUrl,this);
        }


        /// <summary>
        /// Fetch the ObjectId of a schedule by it's name.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pName">
        /// Name of the schedule to find
        /// </param>
        /// <returns>
        /// ObjectId of schedule if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pName)
        {
            string strUrl = string.Format("{0}schedules/?query=(DisplayName is {1})", HomeServer.BaseUrl, pName.UriSafe());

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false || res.TotalObjectCount == 0)
            {
                return "";
            }

            List<Schedule> oSchedules = HomeServer.GetObjectsFromJson<Schedule>(res.ResponseText);

            foreach (var oSchedule in oSchedules)
            {
                if (oSchedule.DisplayName.Equals(pName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oSchedule.ObjectId;
                }
            }

            return "";
        }


        /// <summary>
        /// Lazy fetch method for getting the list of schedule details associated with this schedule.
        /// </summary>
        /// <returns>
        /// Generic list of ScheduleDetail objects - can be empty.
        /// </returns>
        public List<ScheduleDetail> ScheduleDetails ()
        {
            if (_scheduleDetails != null)
            {
                return _scheduleDetails;
            }
            _scheduleDetails = ScheduleDetail.GetDetails(HomeServer, ObjectId);
            return _scheduleDetails;
        }


        /// <summary>
        /// Add a new schedule detail to the schedule
        /// </summary>
        /// <param name="pSubject">
        /// optional description string for the schedule item
        /// </param>
        /// <param name="pStartTime">
        /// the start time (in minutes) for the active day or days.  the start time is stored as the number of minutes from 
        /// midnight.  so a value of 480 would mean 8:00 am and 1020 would mean 5:00 pm.  in addition, a value of 0 for the 
        /// start time indicates 12:00 am.
        /// </param>
        /// <param name="pEndTime">
        /// the end time (in minutes) for the active day or days.  the end time is stored as the number of minutes from 
        /// midnight. so a value of 480 would mean 8:00 am and 1020 would mean 5:00 pm. in addition, a value of 0 means 
        /// "till the end of the day" (e.g.  11:59:59 pm in linux land).
        /// </param>
        /// <param name="pStartDate">
        /// the date when this schedule detail becomes active.  a value of null means the schedule is active immediately.
        /// </param>
        /// <param name="pEndDate">
        /// the date when this schedule detail ends.  a value of null indicates the scheule is active indefinitely.
        /// </param>
        /// <param name="pActiveMonday"></param>
        /// <param name="pActiveTuesday"></param>
        /// <param name="pActiveWednesday"></param>
        /// <param name="pActiveThursday"></param>
        /// <param name="pActiveFriday"></param>
        /// <param name="pActiveSaturday"></param>
        /// <param name="pActiveSunday"></param>
        /// <returns>
        /// Instance of the WebCallResult
        /// </returns>
        public WebCallResult AddScheduleDetail(string pSubject,int pStartTime, int pEndTime,
                                               bool pActiveMonday, bool pActiveTuesday, bool pActiveWednesday,
                                               bool pActiveThursday,
                                               bool pActiveFriday, bool pActiveSaturday, bool pActiveSunday,
                                               DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            return AddScheduleDetail(this.HomeServer, this.ObjectId, pSubject, pStartTime, pEndTime, pActiveMonday,
                                     pActiveTuesday,pActiveWednesday, pActiveThursday, pActiveFriday, pActiveSaturday, 
                                     pActiveSunday,pStartDate, pEndDate);
        }

        /// <summary>
        /// Remove a schedule from the Connection directory.  
        /// </summary>
        public WebCallResult Delete()
        {
            return DeleteSchedule(HomeServer, ObjectId);
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all Schedules and resturns them as a generic list of Schedule objects.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the templates should be pulled from
        /// </param>
        /// <param name="pSchedules">
        /// Out parameter that is used to return the list of Schedule objects defined on Connection - there must be at least one.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter 
        /// at a time  are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>       
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetSchedules(ConnectionServerRest pConnectionServer, out List<Schedule> pSchedules, int pPageNumber = 1,
            int pRowsPerPage = 20, params string[] pClauses)
        {
            pSchedules = new List<Schedule>();

            if (pConnectionServer == null)
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "Null ConnectionServer referenced passed to GetSchedules"
                    };
            }

            List<String> oParams;
            if (pClauses == null)
            {
                oParams = new List<string>();
            }
            else
            {
                oParams = pClauses.ToList();
            }

            oParams.Add("pageNumber=" + pPageNumber);
            oParams.Add("rowsPerPage=" + pRowsPerPage);

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "schedules", oParams.ToArray());

            //issue the command to the CUPI interface
            WebCallResult res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET,  "");
            
            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.ErrorText = "Empty response received";
                res.Success = false;
                return res;
            }

            //no error, just return an empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                return res;
            }

            pSchedules = pConnectionServer.GetObjectsFromJson<Schedule>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pSchedules)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// Create a new schedule in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the new schedule - must be unique.
        /// </param>
        /// <param name="pOwnerLocationObjectId">
        /// location ObjectId to assign as the owner of a schedule.  If the primary location of the local server is used then the schedule
        /// set will be visible in the direcotry on the CUCA web page.
        /// Must pass either this or the OwerUserObjectId value, both cannot be blank.
        /// </param>
        /// <param name="pOwnerSubscriberObjectId">
        /// User to act as owner of schedule.  A user owns a schedule set when it's associated with one of their notification devices
        /// and it doesn't show up in the CUCA web page.
        /// </param>
        /// <param name="pIsHoliday">
        /// Pass as true if this schedule is to act as a holiday
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddSchedule(ConnectionServerRest pConnectionServer,
                                                    string pDisplayName,
                                                    string pOwnerLocationObjectId,
                                                    string pOwnerSubscriberObjectId,
                                                    bool pIsHoliday)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddSchedule";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for partition name in AddSchedule";
                return res;
            }

            if (string.IsNullOrEmpty(pOwnerLocationObjectId) & string.IsNullOrEmpty(pOwnerSubscriberObjectId))
            {
                res.ErrorText = "Empty pOwnerLocationObjectId and pOwnerSubscriberObjectId passed to AddSchedule";
                return res;
            }

            if (!string.IsNullOrEmpty(pOwnerLocationObjectId) & !string.IsNullOrEmpty(pOwnerSubscriberObjectId))
            {
                res.ErrorText = "Both pOwnerLocationObjectId and pOwnerSubscriberObjectId passed to AddSchedule";
                return res;
            }

            string strBody = "<Schedule>";

            strBody += string.Format("<{0}>{1}</{0}>", "DisplayName", pDisplayName);

            if (pIsHoliday)
            {
                strBody += string.Format("<{0}>{1}</{0}>", "IsHoliday",1);
            }

            if (!string.IsNullOrEmpty(pOwnerLocationObjectId))
            {
                strBody += string.Format("<{0}>{1}</{0}>", "OwnerLocationObjectId", pOwnerLocationObjectId);
            }

            if (!string.IsNullOrEmpty(pOwnerSubscriberObjectId))
            {
                strBody += string.Format("<{0}>{1}</{0}>", "OwnerSubscriberObjectId", pOwnerSubscriberObjectId);
            }

            strBody += "</Schedule>";

            res = pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "schedules", MethodType.POST, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/schedules/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/schedules/", "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Create a new schedule in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the new schedule - must be unique.
        /// </param>
        /// <param name="pOwnerLocationObjectId">
        /// location ObjectId to assign as the owner of a schedule.  If the primary location of the local server is used then the schedule
        /// set will be visible in the direcotry on the CUCA web page.
        /// Must pass either this or the OwerUserObjectId value, both cannot be blank.
        /// </param>
        /// <param name="pOwnerSubscriberObjectId">
        /// User to act as owner of schedule set.  A user owns a schedule when it's associated with one of their notification devices
        /// and it doesn't show up in the CUCA web page.
        /// </param>
        /// <param name="pIsHoliday">
        /// Pass as true if this schedule is to act as a holiday
        /// </param>
        /// <param name="pSchedule">
        /// If the schedule is found, an instance of the Schedule class is passed back on this parameter - otherwise this is null.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddSchedule(ConnectionServerRest pConnectionServer,
                                                    string pDisplayName,
                                                    string pOwnerLocationObjectId,
                                                    string pOwnerSubscriberObjectId,
                                                    bool pIsHoliday,
                                                    out Schedule pSchedule)
        {
            pSchedule = null;

            WebCallResult res = AddSchedule(pConnectionServer, pDisplayName,pOwnerLocationObjectId,pOwnerSubscriberObjectId,pIsHoliday);

            //if the create goes through, fetch the schedule as an object and return it all filled in.
            if (res.Success)
            {
                res = GetSchedule(out pSchedule, pConnectionServer, res.ReturnedObjectId);
            }

            return res;
        }



        /// <summary>
        /// returns a single Schedule object from an ObjectId string passed in or optionally a dispaly name string.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the schedule is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the schedule to load
        /// </param>
        /// <param name="pSchedule">
        /// The out param that the filled out instance of the Schedule class is returned on.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional name to search for schedule on.  If both the ObjectId and name are passed, the objectID is used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetSchedule(out Schedule pSchedule, ConnectionServerRest pConnectionServer, string pObjectId = "",
            string pDisplayName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pSchedule = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetSchedule";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pDisplayName)))
            {
                res.ErrorText = "Empty objectId and DisplayName passed to GetSchedule";
                return res;
            }

            //create a new Schedule instance passing the ObjectId (or alias) which fills out the data automatically
            try
            {
                pSchedule = new Schedule(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch schedule in GetSchedule:" + ex.Message;
            }

            return res;
        }


        /// <summary>
        /// Add a new schedule detail to the schedule
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pSubject">
        /// optional description string for the schedule item
        /// </param>
        /// <param name="pScheduleObjectId">
        /// Schedule to add the detail item to
        /// </param>
        /// <param name="pStartTime">
        /// the start time (in minutes) for the active day or days.  the start time is stored as the number of minutes from 
        /// midnight.  so a value of 480 would mean 8:00 am and 1020 would mean 5:00 pm.  in addition, a value of 0 for the 
        /// start time indicates 12:00 am.
        /// </param>
        /// <param name="pEndTime">
        /// the end time (in minutes) for the active day or days.  the end time is stored as the number of minutes from 
        /// midnight. so a value of 480 would mean 8:00 am and 1020 would mean 5:00 pm. in addition, a value of 0 means 
        /// "till the end of the day" (e.g.  11:59:59 pm in linux land).
        /// </param>
        /// <param name="pStartDate">
        /// the date when this schedule detail becomes active.  a value of null means the schedule is active immediately.
        /// </param>
        /// <param name="pEndDate">
        /// the date when this schedule detail ends.  a value of null indicates the scheule is active indefinitely.
        /// </param>
        /// <param name="pActiveMonday"></param>
        /// <param name="pActiveTuesday"></param>
        /// <param name="pActiveWednesday"></param>
        /// <param name="pActiveThursday"></param>
        /// <param name="pActiveFriday"></param>
        /// <param name="pActiveSaturday"></param>
        /// <param name="pActiveSunday"></param>
        /// <returns>
        /// Instance of the WebCallResult
        /// </returns>
        public static WebCallResult AddScheduleDetail(ConnectionServerRest pConnectionServer, string pScheduleObjectId, string pSubject,
                                                int pStartTime, int pEndTime,
                                                bool pActiveMonday, bool pActiveTuesday, bool pActiveWednesday, bool pActiveThursday,
                                                bool pActiveFriday, bool pActiveSaturday, bool pActiveSunday,
                                                DateTime? pStartDate=null, DateTime? pEndDate=null)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddScheduleSetMember";
                return res;
            }

            if (string.IsNullOrEmpty(pScheduleObjectId))
            {
                res.ErrorText = "Empty pScheduleObjectId passed to AddScheduleDetail";
                return res;
            }

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("ScheduleObjectId", pScheduleObjectId);
            oProps.Add("StartDate", pStartDate);
            oProps.Add("StartTime", pStartTime);
            oProps.Add("EndTime", pEndTime);
            oProps.Add("EndDate",pEndDate);
            oProps.Add("IsActiveMonday", pActiveMonday);
            oProps.Add("IsActiveTuesday", pActiveTuesday);
            oProps.Add("IsActiveWednesday", pActiveWednesday);
            oProps.Add("IsActiveThursday", pActiveThursday);
            oProps.Add("IsActiveFriday", pActiveFriday);
            oProps.Add("IsActiveSaturday", pActiveSaturday);
            oProps.Add("IsActiveSunday", pActiveSunday);

            string strBody = "<ScheduleDetail>";

            //tack on the property value pair with appropriate tags
            foreach (var oProp in oProps)
            {
                strBody += string.Format("<{0}>{1}</{0}>", oProp.PropertyName, oProp.PropertyValue);
            }

            strBody += "</ScheduleDetail>";

            string strPath = string.Format("schedules/{0}/scheduledetails", pScheduleObjectId);
            res = pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + strPath, MethodType.POST, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            strPath += "/";
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/" + strPath))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/" + strPath, "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Helps convert a time of day into a "minutes from midnight" formate
        /// </summary>
        /// <param name="iHour">
        /// hour in 24 hour format
        /// </param>
        /// <param name="iMinute">minute</param>
        /// <returns>
        /// Minutes from midnight
        /// </returns>
        public static int GetMinutesFromTimeParts(int iHour, int iMinute)
        {
            return (iHour*60) + iMinute;
        }


        /// <summary>
        /// Remove a schedule from the Connection directory.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pScheduleObjectId">
        /// ObjectId of the schedule to delete
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult DeleteSchedule(ConnectionServerRest pConnectionServer, string pScheduleObjectId)
        {
            WebCallResult res;
            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Null ConnectionServer reference passed to DeleteSchedule";
                return res;
            }

            if (string.IsNullOrEmpty(pScheduleObjectId))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Empty objectId passed to DeleteSchedule";
                return res;
            }

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "schedules/" + pScheduleObjectId,
                                            MethodType.DELETE, "");
        }

        #endregion
    }
}
