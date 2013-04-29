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

    /// <summary>
    /// Class that provides methods for finding, fetching, adding and deleting schedule sets.  Also contains the very handy 
    /// AddQuickSchedule static method that can add a schedule set associated schedule and associated schedule details to make
    /// a fully formed schedule set object and sub objects in one step for common scheduling needs.
    /// </summary>
    public class ScheduleSet
    {

        #region Constructors and Destructors

        /// <summary>
        /// Schedule set constructor - optionally takes an ObjectId to fetch - if not provided just a blank instance of the class is produced - used for 
        /// generating lists of objects.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to query against.
        /// </param>
        /// <param name="pObjectId">
        /// Optional unique identifier for a schedule set to fetch.
        /// </param>
        /// <param name="pDisplayName">
        /// Name of schedule set to find
        /// </param>
        public ScheduleSet(ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to ScheduleSet constructor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                return;
            }

            ObjectId = pObjectId;
            WebCallResult res = GetScheduleSet(pObjectId, pDisplayName);
            if (res.Success == false)
            {
                throw new Exception(string.Format("Failed to find ScheduleSet using Objectid={0}, or displayname={1}, error={2}",
                    pObjectId, pDisplayName, res));
            }

        }

        /// <summary>
        /// General constructor for Json libraries
        /// </summary>
        public ScheduleSet()
        {
        }

        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object used to create this Alternate Extension instance.
        public ConnectionServer HomeServer { get; private set; }

        //list of schedules associated with this schedule set.  Typically a schedule set will contain two schedules - one
        //for regular schedule details and one for holidays - but it may not contain a holiday schedule and can technically
        //contain more than one schedule (although the GUI admin does not allow for this).
        private List<Schedule> _schedules;

        #endregion


        #region ScheduleSet Properties

        public string ObjectId { get; set; }
        public string DisplayName { get; set; }
        public string OwnerLocationObjectId { get; set; }
        public bool Undeletable { get; set; }

        #endregion


        #region Instance Methods


        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
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
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchScheduleSetData()
        {
            return GetScheduleSet(this.ObjectId,"");
        }


        /// <summary>
        /// Fills the current instance of GetSchedule in with properties fetched from the server.  If both the display name and ObjectId
        /// parameters are provided, the ObjectId is used for the search.
        /// </summary>
        /// <param name="pObjectId">
        /// Unique GUID of the schedule to fetch - can be blank if the display name is passed in.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name of schedule set to fetch
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetScheduleSet(string pObjectId, string pDisplayName)
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
                            ErrorText = "Could not find schedule set by DisplayName=" + pDisplayName
                        };
                }
            }

            string strUrl = string.Format("{0}schedulesets/{1}", HomeServer.BaseUrl, strObjectId);

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


        /// <summary>
        /// Fetch the ObjectId of a schedule set by it's name.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pName">
        /// Name of the schedule set to find
        /// </param>
        /// <returns>
        /// ObjectId of schedule set if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pName)
        {
            string strUrl = HomeServer.BaseUrl + string.Format("schedulesets/?query=(DisplayName is {0})", pName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false || res.TotalObjectCount == 0)
            {
                return "";
            }

            List<ScheduleSet> oTemplates = HTTPFunctions.GetObjectsFromJson<ScheduleSet>(res.ResponseText);

            foreach (var oTemplate in oTemplates)
            {
                if (oTemplate.DisplayName.Equals(pName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oTemplate.ObjectId;
                }
            }

            return "";
        }


        /// <summary>
        /// Lazy fetch method for getting the list of schedules associated with this schedule set.
        /// </summary>
        /// <returns>
        /// Generic list of schedule objects - can be empty.
        /// </returns>
        public List<Schedule> Schedules()
        {
            if (_schedules != null)
            {
                return _schedules;
            }
            _schedules = new List<Schedule>();

            List<ScheduleSetMember> oScheduleSetMembers;

            //get the list of memebers (schedules) associated with this schedule set
            GetSchedulesSetsMembers(HomeServer, ObjectId, out oScheduleSetMembers);

            //for each one, build a new schedule object and add it to the list
            foreach (var oMember in oScheduleSetMembers)
            {
                var oSchedule = new Schedule(HomeServer, oMember.ScheduleObjectId);
                _schedules.Add(oSchedule);
            }

            return _schedules;
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
            var oState = ScheduleState.INACTIVE;

            foreach (Schedule oSchedule in this.Schedules())
            {
                var oStateTemp = oSchedule.GetScheduleState(pDateTime);
                
                //holidays overrule other schedules - if any schedule returns holiday state we can exit.
                if (oStateTemp == ScheduleState.HOLIDAY)
                {
                    return oStateTemp;
                }

                //if the schedule is active, that's our return UNLESS we process a holiday schedule later - that overrides
                //the active state
                if (oStateTemp == ScheduleState.ACTIVE)
                {
                    oState = oStateTemp;
                }
            }

            return oState;
        }


        /// <summary>
        /// Adds a schedule as a member of the schedule set
        /// </summary>
        /// <param name="pScheduleObjectId">
        /// Schedule to add as a member
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public WebCallResult AddScheduleSetMember(string pScheduleObjectId)
        {
            return AddScheduleSetMember(this.HomeServer, this.ObjectId, pScheduleObjectId);
        }

        /// <summary>
        /// Gets the list of all schedule sets and resturns them as a generic list of ScheduleSet objects. 
        /// </summary>
        /// <param name="pScheduleSetsMembers">
        /// Out parameter that is used to return the list of Schedule ObjectIds associated with the schedule set
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetSchedulesSetsMembers(out List<ScheduleSetMember> pScheduleSetsMembers)
        {
            return GetSchedulesSetsMembers(HomeServer, ObjectId, out pScheduleSetsMembers);
        }

        /// <summary>
        /// Delete a schedule set
        /// </summary>
        public WebCallResult Delete()
        {
            return DeleteScheduleSet(HomeServer, this.ObjectId);
        }

        #endregion


        #region Static Methods


        /// <summary>
        /// returns a single ScheduleSet object from an ObjectId string passed in or optionally a displayname string.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the ScheduleSet is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the ScheduleSet to load
        /// </param>
        /// <param name="pScheduleSet">
        /// The out param that the filled out instance of the ScheduleSet class is returned on.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional name to search for ScheduleSet list on.  If both the ObjectId and name are passed, the objectID is used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetScheduleSet(out ScheduleSet pScheduleSet, ConnectionServer pConnectionServer, string pObjectId = "",
            string pDisplayName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pScheduleSet = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetSchedule";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((pObjectId.Length == 0) & (pDisplayName.Length == 0))
            {
                res.ErrorText = "Empty objectId and DisplayName passed to GetSchedule";
                return res;
            }

            //create a new DistributionList instance passing the ObjectId (or alias) which fills out the data automatically
            try
            {
                pScheduleSet = new ScheduleSet(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch scheduleset in GetScheduleSet:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Gets the list of all schedule sets and resturns them as a generic list of ScheduleSet objects. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the schedulesets should be pulled from
        /// </param>
        /// <param name="pScheduleSets">
        /// Out parameter that is used to return the list of ScheduleSet objects defined on Connection - there must be at least one.
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
        public static WebCallResult GetSchedulesSets(ConnectionServer pConnectionServer, out List<ScheduleSet> pScheduleSets, int pPageNumber = 1, 
            int pRowsPerPage = 20)
        {
            WebCallResult res;
            pScheduleSets = new List<ScheduleSet>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetSchedulesSets";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "schedulesets", "pageNumber=" + pPageNumber, 
                "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one template
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                res.Success = false;
                return res;
            }

            pScheduleSets = HTTPFunctions.GetObjectsFromJson<ScheduleSet>(res.ResponseText);

            if (pScheduleSets == null)
            {
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pScheduleSets)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// Gets the list of all schedule sets and resturns them as a generic list of ScheduleSet objects. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the schedulesets should be pulled from
        /// </param>
        /// <param name="pScheduleSetObjectId">
        /// The schedule set to look for schedules for.
        ///  </param>
        /// <param name="pScheduleSetsMembers">
        /// Out parameter that is used to return the list of Schedule ObjectIds associated with the schedule set
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetSchedulesSetsMembers(ConnectionServer pConnectionServer, string pScheduleSetObjectId,
            out List<ScheduleSetMember> pScheduleSetsMembers)
        {
            WebCallResult res;
            pScheduleSetsMembers = new List<ScheduleSetMember>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetSchedulesSetsMembers";
                return res;
            }

            string strUrl = string.Format("{0}schedulesets/{1}/schedulesetmembers", pConnectionServer.BaseUrl, pScheduleSetObjectId);


            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one template
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                res.Success = false;
                return res;
            }

            pScheduleSetsMembers = HTTPFunctions.GetObjectsFromJson<ScheduleSetMember>(res.ResponseText);

            if (pScheduleSetsMembers == null)
            {
                return res;
            }

            return res;
        }


        /// <summary>
        /// Create a new schedule set in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the new schedule set - must be unique.
        /// </param>
        /// <param name="pOwnerLocationObjectId">
        /// location ObjectId to assign as the owner of a schedule.  If the primary location of the local server is used then the schedule
        /// set will be visible in the direcotry on the CUCA web page.
        /// Must pass either this or the OwerUserObjectId value, both cannot be blank.
        /// </param>
        /// <param name="pOwnerSubscriberObjectId">
        /// User to act as owner of schedule set.  A user owns a schedule set when it's associated with one of their notification devices
        /// and it doesn't show up in the CUCA web page.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddScheduleSet(ConnectionServer pConnectionServer,
                                                    string pDisplayName,
                                                    string pOwnerLocationObjectId,
                                                    string pOwnerSubscriberObjectId)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddScheduleSet";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for partition name in AddScheduleSet";
                return res;
            }

            if (string.IsNullOrEmpty(pOwnerLocationObjectId) & string.IsNullOrEmpty(pOwnerSubscriberObjectId))
            {
                res.ErrorText = "Empty pOwnerLocationObjectId and pOwnerSubscriberObjectId passed to AddScheduleSet";
                return res;
            }

            if (!string.IsNullOrEmpty(pOwnerLocationObjectId) & !string.IsNullOrEmpty(pOwnerSubscriberObjectId))
            {
                res.ErrorText = "Both pOwnerLocationObjectId and pOwnerSubscriberObjectId passed to AddScheduleSet";
                return res;
            }

            string strBody = "<ScheduleSet>";

            strBody += string.Format("<{0}>{1}</{0}>", "DisplayName", pDisplayName);
            
            if (!string.IsNullOrEmpty(pOwnerLocationObjectId))
            {
                strBody += string.Format("<{0}>{1}</{0}>", "OwnerLocationObjectId", pOwnerLocationObjectId);
            }

            if (!string.IsNullOrEmpty(pOwnerSubscriberObjectId))
            {
                strBody += string.Format("<{0}>{1}</{0}>", "OwnerSubscriberObjectId", pOwnerSubscriberObjectId);
            }

            strBody += "</ScheduleSet>";

            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "schedulesets", MethodType.POST, pConnectionServer, strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/schedulesets/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/schedulesets/", "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Create a new schedule set in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the new schedule set - must be unique.
        /// </param>
        /// <param name="pOwnerLocationObjectId">
        /// location ObjectId to assign as the owner of a schedule.  If the primary location of the local server is used then the schedule
        /// set will be visible in the direcotry on the CUCA web page.
        /// Must pass either this or the OwerUserObjectId value, both cannot be blank.
        /// </param>
        /// <param name="pOwnerSubscriberObjectId">
        /// User to act as owner of schedule set.  A user owns a schedule set when it's associated with one of their notification devices
        /// and it doesn't show up in the CUCA web page.
        /// </param>
        /// <param name="pScheduleSet">
        /// If the schedule set is created, an instance of the ScheduleSet class is passed back on this parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddScheduleSet(ConnectionServer pConnectionServer,
                                                    string pDisplayName,
                                                    string pOwnerLocationObjectId,
                                                    string pOwnerSubscriberObjectId,
                                                    out ScheduleSet pScheduleSet)
        {
            pScheduleSet = null;

            WebCallResult res = AddScheduleSet(pConnectionServer, pDisplayName, pOwnerLocationObjectId, pOwnerSubscriberObjectId);

            //if the create goes through, fetch the schedule as an object and return it all filled in.
            if (res.Success)
            {
                res = GetScheduleSet(out pScheduleSet, pConnectionServer, res.ReturnedObjectId);
            }

            return res;
        }


        /// <summary>
        /// Remove a schedule set from the Connection directory.  If this schedule set is being referenced the removal will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pScheduleSetObjectId">
        /// ObjectId of the schedule set to delete
        /// </param>
        /// <param name="pRemoveAllSchedules">
        /// By default when removing a schedule set all the schedules associated with it are removed as well - if passed as false then 
        /// the schedules are left alone and you need to clean them up (or reuse them in another schedule set) or they will possibly 
        /// show up the CUCA web admin interface as "stranded" schedule elements
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult DeleteScheduleSet(ConnectionServer pConnectionServer, string pScheduleSetObjectId, bool pRemoveAllSchedules=true)
        {
            WebCallResult res =new WebCallResult {Success = false};
            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer reference passed to DeleteScheduleSet";
                return res;
            }

            if (string.IsNullOrEmpty(pScheduleSetObjectId))
            {
                res.ErrorText = "Empty objectId passed to DeleteScheduleSet";
                return res;
            }

            //delete all schedules associated with set if requested
            if (pRemoveAllSchedules)
            {
                //don't proceed if there's an error removing one of the schedules in this case
                res = DeleteAllSchedulesAssociatedWithScheduleSet(pConnectionServer, pScheduleSetObjectId);
                if (res.Success == false)
                {
                    return res;
                }
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "schedulesets/" + pScheduleSetObjectId,
                                            MethodType.DELETE, pConnectionServer, "");
        }


        /// <summary>
        /// When deleting a schedule set, we need to get rid of all schedules associated with it as well.
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer that the schedule set resides on
        /// </param>
        /// <param name="pScheduleSetObjectId">
        /// Schedule set to clear out
        /// </param>
        /// <returns>
        /// instance of the WebCallResults class with details on the call and response if there's an error.
        /// </returns>
        private static WebCallResult DeleteAllSchedulesAssociatedWithScheduleSet(ConnectionServer pConnectionServer, string pScheduleSetObjectId)
        {
            List<ScheduleSetMember> oMembers;
            WebCallResult res = GetSchedulesSetsMembers(pConnectionServer, pScheduleSetObjectId,out oMembers);

            if (res.Success == false)
            {
                return res;
            }

            foreach (var oMember in oMembers)
            {
                res = Schedule.DeleteSchedule(pConnectionServer, oMember.ScheduleObjectId);
                if (res.Success == false)
                {
                    return res;
                }
            }

            //if we're here, all is well
            return new WebCallResult {Success = true};
        }


        /// <summary>
        /// Adds a schedule as a member of the schedule set
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that's being edited
        /// </param>
        /// <param name="pScheduleSetObjectId">
        /// Schedule set to add the member to
        /// </param>
        /// <param name="pScheduleObjectId">
        /// Schedule to add as a member
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult AddScheduleSetMember(ConnectionServer pConnectionServer,
                                                 string pScheduleSetObjectId,
                                                 string pScheduleObjectId)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddScheduleSetMember";
                return res;
            }

            if (string.IsNullOrEmpty(pScheduleSetObjectId) | string.IsNullOrEmpty(pScheduleObjectId))
            {
                res.ErrorText = "Empty pScheduleSetObjectId or pScheduleObjectId passed to AddScheduleSetMember";
                return res;
            }

            string strBody = "<ScheduleSetMember>";

            strBody += string.Format("<{0}>{1}</{0}>", "ScheduleSetObjectId", pScheduleSetObjectId);
            strBody += string.Format("<{0}>{1}</{0}>", "ScheduleObjectId", pScheduleObjectId);

            strBody += "</ScheduleSetMember>";

            string strPath = string.Format("schedulesets/{0}/schedulesetmembers", pScheduleSetObjectId);
            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + strPath, MethodType.POST, pConnectionServer, strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            strPath += "/";
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/"+strPath))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/"+strPath, "").Trim();
                }
            }

            return res;
        }



        /// <summary>
        /// Helper function that creates a scheduleset, schedule, adds a detail item to the schedule and then adds the 
        /// schedule as a member of the schedule set all in one step.
        /// The ObjectId of the newly created ScheduleSet is returned on the WebCallResults structure. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to create the schedule items on.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to assign to the schedule
        /// </param>
        /// <param name="pOwnerLocationObjectId">
        /// Owner of the schedule - primary location object of a server means it's a "system schedule" and is visible in 
        /// CUCA on the schedules page.
        /// </param>
        /// <param name="pOwnerSubscriberObjectId">
        /// If the owner is a subscriber it's generally used as a notification device item.
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
        /// <param name="pActiveMonday"></param>
        /// <param name="pActiveTuesday"></param>
        /// <param name="pActiveWednesday"></param>
        /// <param name="pActiveThursday"></param>
        /// <param name="pActiveFriday"></param>
        /// <param name="pActiveSaturday"></param>
        /// <param name="pActiveSunday"></param>
        /// <param name="pStartDate">
        /// the date when this schedule detail becomes active.  a value of null means the schedule is active immediately.
        /// </param>
        /// <param name="pEndDate">
        /// the date when this schedule detail ends.  a value of null indicates the scheule is active indefinitely.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class - if all goes through the ObjectId of the newly created ScheduleSet will
        /// be returned in the ReturnedObjectId property.
        /// </returns>
        public static WebCallResult AddQuickSchedule(ConnectionServer pConnectionServer, string pDisplayName,
                                              string pOwnerLocationObjectId,string pOwnerSubscriberObjectId,
                                              int pStartTime, int pEndTime,bool pActiveMonday, bool pActiveTuesday, bool pActiveWednesday,
                                              bool pActiveThursday,bool pActiveFriday, bool pActiveSaturday, bool pActiveSunday,
                                              DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            WebCallResult res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddScheduleSet";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for partition name in AddScheduleSet";
                return res;
            }

            if (string.IsNullOrEmpty(pOwnerLocationObjectId) & string.IsNullOrEmpty(pOwnerSubscriberObjectId))
            {
                res.ErrorText = "Empty pOwnerLocationObjectId and pOwnerSubscriberObjectId passed to AddScheduleSet";
                return res;
            }

            if (!string.IsNullOrEmpty(pOwnerLocationObjectId) & !string.IsNullOrEmpty(pOwnerSubscriberObjectId))
            {
                res.ErrorText = "Both pOwnerLocationObjectId and pOwnerSubscriberObjectId passed to AddScheduleSet";
                return res;
            }

            //first, create the schedule set
            res = AddScheduleSet(pConnectionServer, pDisplayName, pOwnerLocationObjectId, pOwnerSubscriberObjectId);
            if (res.Success == false)
            {
                return res;
            }

            string strScheduleSetObjectId = res.ReturnedObjectId;

            res = Schedule.AddSchedule(pConnectionServer, pDisplayName, pOwnerLocationObjectId, pOwnerSubscriberObjectId,false);

            if (res.Success == false)
            {
                return res;
            }

            //now add the schedule
            string strScheduleObjectId = res.ReturnedObjectId;

            res = Schedule.AddScheduleDetail(pConnectionServer, strScheduleObjectId, "", pStartTime, pEndTime,pActiveMonday, 
                pActiveTuesday,pActiveWednesday, pActiveThursday, pActiveFriday, pActiveSaturday,pActiveSunday, pStartDate, pEndDate);
             
            if (res.Success == false)
            {
                return res;
            }

            //now map the schedule to the schedule set
            res = AddScheduleSetMember(pConnectionServer, strScheduleSetObjectId, strScheduleObjectId);
            if (res.Success == false)
            {
                return res;
            }

            //if we're here all is well - stuff the schedule set ObjectId into the res structure for return since that's what the 
            //calling client will need to use.
            res.ReturnedObjectId = strScheduleSetObjectId;
            return res;
        }


        /// <summary>
        /// Overloaded version of method that returns a copy of the schedule set as an out parameter
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to create the schedule items on.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to assign to the schedule
        /// </param>
        /// <param name="pOwnerLocationObjectId">
        /// Owner of the schedule - primary location object of a server means it's a "system schedule" and is visible in 
        /// CUCA on the schedules page.
        /// </param>
        /// <param name="pOwnerSubscriberObjectId">
        /// If the owner is a subscriber it's generally used as a notification device item.
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
        /// <param name="pActiveMonday"></param>
        /// <param name="pActiveTuesday"></param>
        /// <param name="pActiveWednesday"></param>
        /// <param name="pActiveThursday"></param>
        /// <param name="pActiveFriday"></param>
        /// <param name="pActiveSaturday"></param>
        /// <param name="pActiveSunday"></param>
        /// <param name="pStartDate"></param>
        /// <param name="pEndDate"></param>
        /// <param name="pScheduleSet">
        /// Resulting schedule set created is returned on this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class - if all goes through the ObjectId of the newly created ScheduleSet will
        /// be returned in the ReturnedObjectId property.
        /// </returns>
        public static WebCallResult AddQuickSchedule(ConnectionServer pConnectionServer, string pDisplayName,
                                                     string pOwnerLocationObjectId, string pOwnerSubscriberObjectId,
                                                     int pStartTime, int pEndTime, bool pActiveMonday,
                                                     bool pActiveTuesday, bool pActiveWednesday,bool pActiveThursday, 
                                                     bool pActiveFriday, bool pActiveSaturday,bool pActiveSunday,
                                                     DateTime? pStartDate, DateTime? pEndDate, out ScheduleSet pScheduleSet)
        {
            pScheduleSet = null;
            WebCallResult res = AddQuickSchedule(pConnectionServer, pDisplayName, pOwnerLocationObjectId,
                                                 pOwnerSubscriberObjectId, pStartTime, pEndTime,
                                                 pActiveMonday, pActiveThursday, pActiveWednesday, pActiveThursday,
                                                 pActiveFriday, pActiveSaturday, pActiveSunday, pStartDate,pEndDate);

            //if the create goes through, fetch the ScheduleSet as an object and return it.
            if (res.Success)
            {
                res = GetScheduleSet(out pScheduleSet, pConnectionServer, res.ReturnedObjectId);
            }
            
            return res;
        }


        #endregion
    }
}
