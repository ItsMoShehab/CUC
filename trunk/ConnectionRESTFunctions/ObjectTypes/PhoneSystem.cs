#region Legal Disclaimer

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

    /// <summary>
    /// The phone system class provides methods for fetching, creating, updtaing and deleting phone system objects in the Unity 
    /// Connection directory
    /// </summary>
    public class PhoneSystem
    {

        #region Constructors and Destructors


        /// <summary>
        /// Generic constructor for JSON parsing library
        /// </summary>
        public PhoneSystem()
        {
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Constructor for the PhoneSystem class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        /// <param name="pObjectId">
        /// Optional - if passed in the specifics of the switch identified by this GUID is fetched and the properties are filled in.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name to search for a switch definition by
        /// </param>
        public PhoneSystem(ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to MediaSwitch construtor");
            }

            HomeServer = pConnectionServer;

            //if no objectId is passed in just create an empty version of the class - used for constructing lists from data fetches.
            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetPhoneSystem(pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Phone system not found in PhoneSystem constructor using ObjectId={0} " +
                                                  "or displayName={1}\n\r{2}", pObjectId, pDisplayName, res.ErrorText));
            }
        }

        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object used to create this object instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region PhoneSystem Properties

        private string _displayName;
        public string DisplayName 
        { 
            get { return _displayName; } 
            set
            {
                _changedPropList.Add("DisplayName",value);
                _displayName = value;
            } 
        }

        [JsonProperty]
        public string ObjectId { get; private set; }

        private string _callLoopDtmf;
        public string CallLoopDTMF
        {
            get { return _callLoopDtmf; }
            set
            {
                _changedPropList.Add("CallLoopDTMF", value);
                _callLoopDtmf = value;
            }
        }

        private bool _callLoopExtensionDetection;
        public bool CallLoopExtensionDetect
        {
            get { return _callLoopExtensionDetection; }
            set
            {
                _changedPropList.Add("CallLoopExtensionDetect", value);
                _callLoopExtensionDetection = value;
            }
        }

        private bool _callLoopForwardNotificationDetect;
        public bool CallLoopForwardNotificationDetect
        {
            get { return _callLoopForwardNotificationDetect; }
            set
            {
                _changedPropList.Add("CallLoopForwardNotificationDetect", value);
                _callLoopForwardNotificationDetect = value;
            }
        }

        private int _callLoopGuardTimeMs;
        public int CallLoopGuardTimeMs
        {
            get { return _callLoopGuardTimeMs; }
            set
            {
                _changedPropList.Add("CallLoopGuardTimeMs", value);
                _callLoopGuardTimeMs = value;
            }
        }

        private bool _callLoopSupervisedTransferDetect;
        public bool CallLoopSupervisedTransferDetect
        {
            get { return _callLoopSupervisedTransferDetect; }
            set
            {
                _changedPropList.Add("DisplayName", value);
                _callLoopSupervisedTransferDetect = value;
            }
        }

        [JsonProperty]
        public string CcmAXLPassword { get; private set; }
        
        [JsonProperty]
        public string CcmCtiPassword { get; private set; }

        private bool _defaultTrapSwitch;
        public bool DefaultTrapSwitch
        {
            get { return _defaultTrapSwitch; }
            set
            {
                _changedPropList.Add("DefaultTrapSwitch", value);
                _defaultTrapSwitch = value;
            }
        }

        private bool _enablePhoneApplications;
        public bool EnablePhoneApplications
        {
            get { return _enablePhoneApplications; }
            set
            {
                _changedPropList.Add("EnablePhoneApplications", value);
                _enablePhoneApplications = value;
            }
        }

        private bool _mwiAlwaysUpdate;
        public bool MwiAlwaysUpdate
        {
            get { return _mwiAlwaysUpdate; } 
            set
            {
                _changedPropList.Add("MwiAlwaysUpdate", value);
                _mwiAlwaysUpdate = value;
            } 
        }


        private bool _mwiPortMemory;
        public bool MwiPortMemory
        {
            get { return _mwiPortMemory; }
            set
            {
                _changedPropList.Add("MwiPortMemory", value);
                _mwiPortMemory = value;
            }
        }

        private bool _mwiForceOff;
        public bool MwiForceOff
        {
            get { return _mwiForceOff; }
            set
            {
                _changedPropList.Add("MwiForceOff", value);
                _mwiForceOff = value;
            }
        }

        [JsonProperty]
        public int PortCount { get; private set; }

        private bool _restrictDialUnconditional;
        public bool RestrictDialUnconditional
        {
            get { return _restrictDialUnconditional; }
            set
            {
                _changedPropList.Add("RestrictDialUnconditional", value);
                _restrictDialUnconditional = value;
            }
        }

        private bool _restrictDialScheduled;
        public bool RestrictDialScheduled
        {
            get { return _restrictDialScheduled; }
            set
            {
                _changedPropList.Add("RestrictDialScheduled", value);
                _restrictDialScheduled = value;
            }
        }

        private int _restrictDialStartTime;
        public int RestrictDialStartTime
        {
            get { return _restrictDialStartTime; }
            set
            {
                _changedPropList.Add("RestrictDialStartTime", value);
                _restrictDialStartTime = value;
            }
        }

        private int _restrictDialEndTime;
        public int RestrictDialEndTime
        {
            get { return _restrictDialEndTime; }
            set
            {
                _changedPropList.Add("RestrictDialEndTime", value);
                _restrictDialEndTime = value;
            }
        }
        


        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the phone system
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the PhoneSystem object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            StringBuilder strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
        }


        /// <summary>
        /// Fetch details for a single phone system by ObjectId/name and populate the local instance's properties with it
        /// </summary>
        /// <param name="pObjectId">
        /// Unique identifier for phone system to fetch
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name to search for a phone system for
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        private WebCallResult GetPhoneSystem(string pObjectId, string pDisplayName)
        {
            string strObjectId;

            //when fetching a phone system prefer the ObjectId if provided
            if (pObjectId.Length > 0)
            {
                strObjectId = pObjectId;
            }
            else if (pDisplayName.Length > 0)
            {
                //fetch the ObjectId for the name if possible
                strObjectId = GetObjectIdByPhoneSystemName(pDisplayName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "No phone system found for display name passed into GetPhoneSystem:" + pDisplayName
                    };
                }
            }
            else
            {
                return new WebCallResult
                {
                    Success = false,
                    ErrorText = "No value for ObjectId or display name passed to GetPhoneSystem."
                };
            }

            string strUrl = string.Format("{0}phonesystems/{1}", HomeServer.BaseUrl, strObjectId);

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

            ClearPendingChanges();

            return res;
        }


        /// <summary>
        /// Fetch a phone system ObjectId by it's name - 
        /// </summary>
        /// <param name="pPhoneSystemName">
        /// Display name of the phone system to search for
        /// </param>
        /// <returns>
        /// ObjectId of the phone system with the name if found, or blank string if not.
        /// </returns>
        private string GetObjectIdByPhoneSystemName(string pPhoneSystemName)
        {

            string strUrl = string.Format("{0}phonesystems/?query=(DisplayName is {1})", HomeServer.BaseUrl, pPhoneSystemName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return "";
            }

            List<PhoneSystem> oPhoneSystems = HTTPFunctions.GetObjectsFromJson<PhoneSystem>(res.ResponseText);

            foreach (var oPhoneSystem in oPhoneSystems)
            {
                if (oPhoneSystem.DisplayName.Equals(pPhoneSystemName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oPhoneSystem.ObjectId;
                }
            }

            return "";
        }



        /// <summary>
        /// Return a list of all users associated with the current phone system instance.
        /// </summary>
        /// <param name="pAssociations">
        /// List of associated users is returned on this out parameter
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>        
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        public WebCallResult GetPhoneSystemAssociations(out List<PhoneSystemAssociation> pAssociations, int pPageNumber = 1, 
            int pRowsPerPage = 20)
        {
            return GetPhoneSystemAssociations(HomeServer, ObjectId, out pAssociations,pPageNumber,pRowsPerPage);
        }

        /// <summary>
        /// Allows one or more properties on a phone system to be udpated.  
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the object intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for PhoneSystem:{0}, objectid=[{1}]",
                                              this, this.ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdatePhoneSystem(HomeServer, this.ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }

        /// <summary>
        /// DELETE a phone system from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeletePhoneSystem(HomeServer, ObjectId);
        }

        /// <summary>
        /// If the phone system object has any pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all phone systems and resturns them as a generic list of PhoneSystem objects.  This
        /// list can be used for providing drop down list selection for user creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the phone systems should be pulled from
        /// </param>
        /// <param name="pPhoneSystems">
        /// Out parameter that is used to return the list of PhoneSystem objects defined on Connection - there must be at least one.
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
        public static WebCallResult GetPhoneSystems(ConnectionServer pConnectionServer, out List<PhoneSystem> pPhoneSystems, 
            int pPageNumber = 1, int pRowsPerPage = 20)
        {
            WebCallResult res;
            pPhoneSystems = null;

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetPhoneSystems";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "phonesystems", "pageNumber=" + pPageNumber, 
                "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - no phone systems is legal
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pPhoneSystems = new List<PhoneSystem>();
                return res;
            }

            pPhoneSystems = HTTPFunctions.GetObjectsFromJson<PhoneSystem>(res.ResponseText);

            if (pPhoneSystems == null)
            {
                pPhoneSystems = new List<PhoneSystem>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pPhoneSystems)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// returns a single PhoneSystem object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the phone system is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the phone system to load
        /// </param>
        /// <param name="pPhoneSystem">
        /// The out param that the filled out instance of the PhoneSystem class is returned on.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name 
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPhoneSystem(out PhoneSystem pPhoneSystem, ConnectionServer pConnectionServer, string pObjectId, 
            string pDisplayName="")
        {
            WebCallResult res = new WebCallResult {Success = false};

            pPhoneSystem = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPhoneSystem";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ( string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty objectId and DisplayName passed to GetPhonesystem";
                return res;
            }

            //create a new PhoneSystem instance passing the ObjectId (or display name) which fills out the data automatically
            try
            {
                pPhoneSystem = new PhoneSystem(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch phone system in GetPhonesystem:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Allows one or more properties on a phone system to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the phone system is homed.
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// Unique identifier for media switch (phone system) to update.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a property name and a new value for that property to apply to the object
        /// being updated. 
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdatePhoneSystem(ConnectionServer pConnectionServer,string pMediaSwitchObjectId,ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdatePhoneSystem";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdatePhoneSystem";
                return res;
            }

            string strBody = "<PhoneSystem>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</PhoneSystem>";

            return HTTPFunctions.GetCupiResponse(string.Format("{0}phonesystems/{1}", pConnectionServer.BaseUrl, pMediaSwitchObjectId), 
                MethodType.PUT, pConnectionServer, strBody, false);

        }

        /// <summary>
        /// Adds a new phone system with the display name provided 
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the phone system to
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the phone system to add.  Display name should be unique among phone systems.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddPhoneSystem(ConnectionServer pConnectionServer,string pDisplayName)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddPhoneSystem";
                return res;
            }

            //make sure that something is passed in for the required param
            if (String.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for display name in AddPhoneSystem";
                return res;
            }

            string strBody = "<PhoneSystem>";

            //tack on the property value pair with appropriate tags
            strBody += string.Format("<DisplayName>{0}</DisplayName>", pDisplayName);

            strBody += "</PhoneSystem>";

            res =HTTPFunctions.GetCupiResponse(string.Format("{0}phonesystems", pConnectionServer.BaseUrl),
                    MethodType.POST, pConnectionServer, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                const string strPrefix = @"/vmrest/phonesystems/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Adds a new phone system with the display name provided 
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the phone system to
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the phone system to add.  Display name should be unique among phone systems.
        /// </param>
        /// <param name="pPhoneSystem">
        /// If the phone system is added, an instance of it is created and passed back on this out parameter.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddPhoneSystem(ConnectionServer pConnectionServer,string pDisplayName,out PhoneSystem pPhoneSystem)
        {
            pPhoneSystem = null;

            WebCallResult res = AddPhoneSystem(pConnectionServer, pDisplayName);

            //if the create goes through, fetch the phone system as an object and return it.
            if (res.Success)
            {
                res = GetPhoneSystem(out pPhoneSystem, pConnectionServer, res.ReturnedObjectId);
            }

            return res;
        }

        /// <summary>
        /// DELETE a phone system from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the phone system is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the phone system in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeletePhoneSystem(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeletePhoneSystem";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "phonesystems/" + pObjectId,
                                            MethodType.DELETE, pConnectionServer, "");
        }


        /// <summary>
        /// Get a list of all users associated with a phone system 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to do the query against
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the phone system to get associations for
        /// </param>
        /// <param name="pAssociations">
        /// List of associated users is returned on this parameter
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        public static WebCallResult GetPhoneSystemAssociations(ConnectionServer pConnectionServer, string pObjectId,
            out List<PhoneSystemAssociation> pAssociations, int pPageNumber=1, int pRowsPerPage=20)
        {
            WebCallResult res = new WebCallResult {Success = false};
            pAssociations = new List<PhoneSystemAssociation>();
            
            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetPhoneSystemAssociation";
                return res;
            }

            string strUrl =HTTPFunctions.AddClausesToUri(string.Format("{0}phonesystems/{1}/phonesystemassociations", pConnectionServer.BaseUrl, pObjectId),
                    "pageNumber=" + pPageNumber, "rowsPerPage=" + pRowsPerPage);

            res= HTTPFunctions.GetCupiResponse(strUrl,MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - no phone systems is legal
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                return res;
            }

            pAssociations = HTTPFunctions.GetObjectsFromJson<PhoneSystemAssociation>(res.ResponseText);

            if (pAssociations == null)
            {
                pAssociations = new List<PhoneSystemAssociation>();
            }
            return res;
        }

        #endregion

    }
}
