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
    /// Class that provides methods for fetching, pdating, deleting and adding port groups in the Unity Connection
    /// directory.
    /// </summary>
    public class PortGroup :IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor for the PortGroup class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        /// <param name="pObjectId">
        /// Optional - if passed in the specifics of the switch identified by this GUID is fetched and the properties are filled in.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name to search for a port group by
        /// </param>
        public PortGroup(ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to PortGroup construtor");
            }

            HomeServer = pConnectionServer;
            ObjectId = pObjectId;

            //if no objectId is passed in just create an empty version of the class - used for constructing lists from XML fetches.
            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetPortGroup(pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Port group not found in PortGroup constructor using ObjectId={0}\n\r{1}"
                                                 , pObjectId, res.ErrorText));
            }
        }

        /// <summary>
        /// Generic constructor for Json libraries
        /// </summary>
        public PortGroup()
        {
            _changedPropList = new ConnectionPropertyList();
        }

        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DisplayName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        //reference to the ConnectionServer object used to create this object instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region PortGroup Properties

        private bool _ccmDoAutoFailback;
        public bool CcmDoAutoFailback
        {
            get { return _ccmDoAutoFailback; }
            set
            {
                _changedPropList.Add("CcmDoAutoFailback", value);
                _ccmDoAutoFailback = value;
            }
        }

        private int _delayBeforeOpeningMs;
        public int DelayBeforeOpeningMs
        {
            get { return _delayBeforeOpeningMs; }
            set
            {
                _changedPropList.Add("DelayBeforeOpeningMs", value);
                _delayBeforeOpeningMs = value;
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _changedPropList.Add("DisplayName", value);
                _displayName = value;
            }
        }

        private int _dtmfDialInterDigitDelayMs;
        public int DtmfDialInterDigitDelayMs
        {
            get { return _dtmfDialInterDigitDelayMs; }
            set
            {
                _changedPropList.Add("DtmfDialInterDigitDelayMs", value);
                _dtmfDialInterDigitDelayMs = value;
            }
        }

        private bool _enableMwi;
        public bool EnableMWI
        {
            get { return _enableMwi; }
            set
            {
                _changedPropList.Add("EnableMWI", value);
                _enableMwi = value;
            }
        }

        private bool _enableAGC;
        public bool EnableAGC
        {
            get { return _enableAGC; }
            set
            {
                _changedPropList.Add("EnableAGC", value);
                _enableAGC = value;
            }
        }
        

        [JsonProperty]
        public string MediaPortGroupTemplateObjectId { get; private set; }


        private string _mediaSipSecurityProfileObjectId;
        public string MediaSipSecurityProfileObjectId
        {
            get { return _mediaSipSecurityProfileObjectId; }
            set
            {
                _changedPropList.Add("MediaSipSecurityProfileObjectId", value);
                _mediaSipSecurityProfileObjectId = value;
            }
        }


        [JsonProperty]
        public string MediaSwitchObjectId { get; private set; }

        [JsonProperty]
        public string MediaSwitchDisplayName { get; private set; }


        private string _mwiOnCode;
        public string MwiOnCode
        {
            get { return _mwiOnCode; }
            set
            {
                _changedPropList.Add("MwiOnCode", value);
                _mwiOnCode = value;
            }
        }

        private string _mwiOffCode;
        public string MwiOffCode
        {
            get { return _mwiOffCode; }
            set
            {
                _changedPropList.Add("MwiOffCode", value);
                _mwiOffCode = value;
            }
        }

        private int _mwiRetryCountOnSuccess;
        public int MwiRetryCountOnSuccess
        {
            get { return _mwiRetryCountOnSuccess; }
            set
            {
                _changedPropList.Add("MwiRetryCountOnSuccess", value);
                _mwiRetryCountOnSuccess = value;
            }
        }

        private int _mwiRetryIntervalOnSuccessMs;
        public int MwiRetryIntervalOnSuccessMs
        {
            get { return _mwiRetryIntervalOnSuccessMs; }
            set
            {
                _changedPropList.Add("MwiRetryIntervalOnSuccessMs", value);
                _mwiRetryIntervalOnSuccessMs = value;
            }
        }

        private int _mwiMinRequestIntervalMs;
        public int MwiMinRequestIntervalMs
        {
            get { return _mwiMinRequestIntervalMs; }
            set
            {
                _changedPropList.Add("MwiMinRequestIntervalMs", value);
                _mwiMinRequestIntervalMs = value;
            }
        }

        private int _mwiMaxConcurrentRequests;
        public int MwiMaxConcurrentRequests
        {
            get { return _mwiMaxConcurrentRequests; }
            set
            {
                _changedPropList.Add("MwiMaxConcurrentRequests", value);
                _mwiMaxConcurrentRequests = value;
            }
        }

        private bool _noiseFreeEnable;
        public bool NoiseFreeEnable
        {
            get { return _noiseFreeEnable; }
            set
            {
                _changedPropList.Add("NoiseFreeEnable", value);
                _noiseFreeEnable = value;
            }
        }


        [JsonProperty]
        public string ObjectId { get; private set; }

        private int _outgoingGuardTimeMs;
        public int OutgoingGuardTimeMs
        {
            get { return _outgoingGuardTimeMs; }
            set
            {
                _changedPropList.Add("OutgoingGuardTimeMs", value);
                _outgoingGuardTimeMs = value;
            }
        }

        private int _outgoingPostDialDelayMs;
        public int OutgoingPostDialDelayMs
        {
            get { return _outgoingPostDialDelayMs; }
            set
            {
                _changedPropList.Add("OutgoingPostDialDelayMs", value);
                _outgoingPostDialDelayMs = value;
            }
        }

        private int _outgoingPreDialDelayMs;
        public int OutgoingPreDialDelayMs
        {
            get { return _outgoingPreDialDelayMs; }
            set
            {
                _changedPropList.Add("OutgoingPreDialDelayMs", value);
                _outgoingPreDialDelayMs = value;
            }
        }

        [JsonProperty]
        public int PortCount { get; private set; }


        public int _preferredCallControl;
        public int PreferredCallControl
        {
            get { return _preferredCallControl; }
            set
            {
                _changedPropList.Add("PreferredCallControl", value);
                _preferredCallControl = value;
            }
        }


        private int _recordingDTMFClipMs;
        public int RecordingDTMFClipMs
        {
            get { return _recordingDTMFClipMs; }
            set
            {
                _changedPropList.Add("RecordingDTMFClipMs", value);
                _recordingDTMFClipMs = value;
            }
        }

        private int _recordingToneExtraClipMs;
        public int RecordingToneExtraClipMs
        {
            get { return _recordingToneExtraClipMs; }
            set
            {
                _changedPropList.Add("RecordingToneExtraClipMs", value);
                _recordingToneExtraClipMs = value;
            }
        }

        private int _resetStatusEnum;
        public int ResetStatusEnum
        {
            get { return _resetStatusEnum; }
            set
            {
                _changedPropList.Add("ResetStatusEnum", value);
                _resetStatusEnum = value;
            }
        }

        private bool _sipDoSRTP;
        public bool SipDoSRTP
        {
            get { return _sipDoSRTP; }
            set
            {
                _changedPropList.Add("SipDoSRTP", value);
                _sipDoSRTP = value;
            }
        }

        private string _sipContactLineName;
        public string SipContactLineName
        {
            get { return _sipContactLineName; }
            set
            {
                _changedPropList.Add("SipContactLineName", value);
                _sipContactLineName = value;
            }
        }


        private bool _sipDoAuthenticate;
        public bool SipDoAuthenticate
        {
            get { return _sipDoAuthenticate; }
            set
            {
                _changedPropList.Add("SipDoAuthenticate", value);
                _sipDoAuthenticate = value;
            }
        }

        private bool _sipDoDtmfRfc2833;
        public bool SipDoDtmfRfc2833
        {
            get { return _sipDoDtmfRfc2833; }
            set
            {
                _changedPropList.Add("SipDoDtmfRfc2833", value);
                _sipDoDtmfRfc2833 = value;
            }
        }

        private bool _sipDoDtmfKPML;
        public bool SipDoDtmfKPML
        {
            get { return _sipDoDtmfKPML; }
            set
            {
                _changedPropList.Add("SipDoDtmfKPML", value);
                _sipDoDtmfKPML = value;
            }
        }


        private int _sipPreferredMedia;
        public int SipPreferredMedia
        {
            get { return _sipPreferredMedia; }
            set
            {
                _changedPropList.Add("SipPreferredMedia", value);
                _sipPreferredMedia = value;
            }
        }


        private bool _sipRegisterWithProxyServer;
        public bool SipRegisterWithProxyServer
        {
            get { return _sipRegisterWithProxyServer; }
            set
            {
                _changedPropList.Add("SipRegisterWithProxyServer", value);
                _sipRegisterWithProxyServer = value;
            }
        }


        private int _sipTLSModeEnum;
        public int SipTLSModeEnum
        {
            get { return _sipTLSModeEnum; }
            set
            {
                _changedPropList.Add("SipTLSModeEnum", value);
                _sipTLSModeEnum = value;
            }
        }

        private int _sipTransportProtocolEnum;
        public int SipTransportProtocolEnum
        {
            get { return _sipTransportProtocolEnum; }
            set
            {
                _changedPropList.Add("SipTransportProtocolEnum", value);
                _sipTransportProtocolEnum = value;
            }
        }


        private string _skinnyDevicePrefix;
        public string SkinnyDevicePrefix
        {
            get { return _skinnyDevicePrefix; }
            set
            {
                _changedPropList.Add("SkinnyDevicePrefix", value);
                _skinnyDevicePrefix = value;
            }
        }

        private int _waitForCallInfoMs;
        public int WaitForCallInfoMs
        {
            get { return _waitForCallInfoMs; }
            set
            {
                _changedPropList.Add("WaitForCallInfoMs", value);
                _waitForCallInfoMs = value;
            }
        }



        [JsonProperty]
        public int TelephonyIntegrationMethodEnum { get; private set; }


        #endregion

   
        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the port group
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the port group object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the PortGroup object instance.
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
        /// Fetch details for a single port group by ObjectId/name and populate the local instance's properties with it
        /// </summary>
        /// <param name="pObjectId">
        /// Unique identifier for port group to fetch
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name to search for a port group by
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        private WebCallResult GetPortGroup(string pObjectId, string pDisplayName)
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
                strObjectId = GetObjectIdByPortGroupName(pDisplayName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "No port group found for display name passed into GetPortGroup:" + pDisplayName
                    };
                }
            }
            else
            {
                return new WebCallResult
                {
                    Success = false,
                    ErrorText = "No value for ObjectId or display name passed to GetPortGroup."
                };
            }


            string strUrl = string.Format("{0}portgroups/{1}", HomeServer.BaseUrl, strObjectId);

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
        /// Fetch a port group ObjectId by it's name - 
        /// </summary>
        /// <param name="pPhoneSystemName">
        /// Display name of the port group to search for
        /// </param>
        /// <returns>
        /// ObjectId of the port group with the name if found, or blank string if not.
        /// </returns>
        private string GetObjectIdByPortGroupName(string pPhoneSystemName)
        {

            string strUrl = string.Format("{0}portgroups/?query=(DisplayName is {1})", HomeServer.BaseUrl, pPhoneSystemName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false || res.TotalObjectCount==0)
            {
                return "";
            }

            List<PortGroup> oPortGroups = HTTPFunctions.GetObjectsFromJson<PortGroup>(res.ResponseText);

            foreach (var oPortGroup in oPortGroups)
            {
                if (oPortGroup.DisplayName.Equals(pPhoneSystemName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oPortGroup.ObjectId;
                }
            }

            return "";
        }

        /// <summary>
        /// If the port group object has any pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }

        /// <summary>
        /// Allows one or more properties on a port group to be udpated.  
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the transfer option intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for PortGroup:{0}, objectid=[{1}]",
                                              this, this.ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdatePortGroup(HomeServer, this.ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }

        /// <summary>
        /// DELETE a port group from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeletePortGroup(HomeServer, ObjectId);
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all port groups and resturns them as a generic list of PortGroup objects.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that is being queried
        /// </param>
        /// <param name="pPortGroups">
        /// Out parameter that is used to return the list of PortGroup objects defined on Connection - there may be none - this list can be 
        /// returned empty.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>        
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPortGroups(ConnectionServer pConnectionServer, out List<PortGroup> pPortGroups, int pPageNumber = 1,
            int pRowsPerPage = 20, params string[] pClauses)
        {
            WebCallResult res;
            pPortGroups = new List<PortGroup>();

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetPortGroups";
                return res;
            }

            //add on the paging directive to existing clauses
            List<string> temp;
            if (pClauses == null)
            {
                temp = new List<string>();
            }
            else
            {
                temp = pClauses.ToList();
            }

            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "portgroups",temp.ToArray());

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error here - port groups can be empty
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pPortGroups = new List<PortGroup>();
                return res;
            }

            pPortGroups = HTTPFunctions.GetObjectsFromJson<PortGroup>(res.ResponseText);

            if (pPortGroups == null)
            {
                pPortGroups = new List<PortGroup>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pPortGroups)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }

        /// <summary>
        /// Gets all the port groups defined for a particular media switch (if any)
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the port groups are homedon
        /// </param>
        /// <param name="pPortGroups">
        /// List of port groups associated with phone system (if any) are returned on this out parameter
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// Media switch (phone system) to fetch port groups for
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPortGroups(ConnectionServer pConnectionServer, out List<PortGroup> pPortGroups,
                                                  string pMediaSwitchObjectId)
        {
            return GetPortGroups(pConnectionServer, out pPortGroups, 1, 512,
                                 string.Format("query=(MediaSwitchObjectId is {0})", pMediaSwitchObjectId));
        }

        /// <summary>
        /// returns a single PortGroup object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the PortGroup is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the PortGroup to load
        /// </param>
        /// <param name="pPortGroup">
        /// The out param that the filled out instance of the PortGroup class is returned on.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name to search for a port group by
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPortGroup(out PortGroup pPortGroup, ConnectionServer pConnectionServer, string pObjectId, 
            string pDisplayName="")
        {
            WebCallResult res = new WebCallResult { Success = false };

            pPortGroup = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPortGroup";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty objectId and display name passed to GetPortGroup";
                return res;
            }

            //create a new PhoneSystem instance passing the ObjectId (or display name) which fills out the data automatically
            try
            {
                pPortGroup = new PortGroup(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch phone system in GetPortGroup:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Allows one or more properties on a port group to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the port group is homed.
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Unique identifier for the port group to update.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a transfer option property name and a new value for that property to apply to the option 
        /// being updated. This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one
        /// property pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdatePortGroup(ConnectionServer pConnectionServer,
                                                        string pPortGroupObjectId,
                                                        ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdatePortGroup";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdatePortGroup";
                return res;
            }

            string strBody = "<PortGroup>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</PortGroup>";

            return HTTPFunctions.GetCupiResponse(string.Format("{0}portgroups/{1}", pConnectionServer.BaseUrl, pPortGroupObjectId),
                MethodType.PUT, pConnectionServer, strBody, false);

        }


        /// <summary>
        /// Create a new port group - port groups get ports assigned to them and are, in turn, assigned to phone systems.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to create the new port gorup on.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name of the new port gorup
        /// </param>
        /// <param name="pPhoneSystemObjectId">
        /// Phone system to associate the port group to.
        /// </param>
        /// <param name="pHostOrIpAddress">
        /// Host name or IP address of the phone system (i.e. the Call Manager server)
        /// </param>
        /// <param name="pPhoneIntegrationMethod">
        /// SIP, SCCP or PIMG/TIMG
        /// </param>
        /// <param name="pSccpDevicePrefix">
        /// When setting up a port group for SCCP you need to provide a device prefix such as "UnityUM1-VI".  For other integration types this can 
        /// be left blank
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the method call and the results from the server.
        /// </returns>
        public static WebCallResult AddPortGroup(ConnectionServer pConnectionServer, string pDisplayName, string pPhoneSystemObjectId, 
            string pHostOrIpAddress, TelephonyIntegrationMethodEnum pPhoneIntegrationMethod, string pSccpDevicePrefix = "")
        {
            WebCallResult res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddPortGroup";
                return res;
            }

            //get template object Id
            string strMediaPortGroupTemplateObjectId;
            res = PortGroupTemplate.GetPortGroupTemplateObjectId(pConnectionServer, pPhoneIntegrationMethod, out strMediaPortGroupTemplateObjectId);
            if (res.Success == false)
            {
                return res;
            }

            //make sure that something is passed in for the required param
            if (String.IsNullOrEmpty(pDisplayName)
                | string.IsNullOrEmpty(pPhoneSystemObjectId) 
                | string.IsNullOrEmpty(pHostOrIpAddress))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddPortGroup";
                return res;
            }

            string strBody = "<PortGroup>";

            //tack on the property value pair with appropriate tags
            strBody += string.Format("<DisplayName>{0}</DisplayName>", pDisplayName);
            strBody += string.Format("<MediaPortGroupTemplateObjectId>{0}</MediaPortGroupTemplateObjectId>", strMediaPortGroupTemplateObjectId);
            strBody += string.Format("<MediaSwitchObjectId>{0}</MediaSwitchObjectId>", pPhoneSystemObjectId);
            strBody += string.Format("<HostOrIPAddress>{0}</HostOrIPAddress>", pHostOrIpAddress);
            strBody += string.Format("<TelephonyIntegrationMethodEnum>{0}</TelephonyIntegrationMethodEnum>", (int)pPhoneIntegrationMethod);

            if (pPhoneIntegrationMethod == RestFunctions.TelephonyIntegrationMethodEnum.SCCP)
            {
                strBody += string.Format("<SkinnyDevicePrefix>{0}</SkinnyDevicePrefix>", pSccpDevicePrefix);
            }

            strBody += "</PortGroup>";

            res = HTTPFunctions.GetCupiResponse(string.Format("{0}portgroups", pConnectionServer.BaseUrl),
                    MethodType.POST, pConnectionServer, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                const string strPrefix = @"/vmrest/portgroups/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Create a new port group - port groups get ports assigned to them and are, in turn, assigned to phone systems.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to create the new port gorup on.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name of the new port gorup
        /// </param>
        /// <param name="pPhoneSystemObjectId">
        /// Phone system to associate the port group to.
        /// </param>
        /// <param name="pHostOrIpAddress">
        /// Host name or IP address of the phone system (i.e. the Call Manager server)
        /// </param>
        /// <param name="pPhoneIntegrationMethod">
        /// SIP, SCCP or PIMG/TIMG
        /// </param>
        /// <param name="pSccpDevicePrefix">
        /// When setting up a port group for SCCP you need to provide a device prefix such as "UnityUM1-VI".  For other integration types this can 
        /// be left blank
        /// </param>
        /// <param name="pPortGroup">
        /// instance of the newly created PortGroup is passed back on this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the method call and the results from the server.
        /// </returns>
        public static WebCallResult AddPortGroup(ConnectionServer pConnectionServer, string pDisplayName, string pPhoneSystemObjectId, 
                    string pHostOrIpAddress, TelephonyIntegrationMethodEnum pPhoneIntegrationMethod, string pSccpDevicePrefix,out PortGroup pPortGroup)
        {
            pPortGroup = null;
            WebCallResult res = AddPortGroup(pConnectionServer, pDisplayName,
                                             pPhoneSystemObjectId, pHostOrIpAddress,
                                             pPhoneIntegrationMethod,pSccpDevicePrefix);

            if (res.Success == false)
            {
                return res;
            }

            return GetPortGroup(out pPortGroup,pConnectionServer,res.ReturnedObjectId);
        }

        /// <summary>
        /// DELETE a port group from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the port group is homed
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the port group in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeletePortGroup(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeletePortGroup";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "portgroups/" + pObjectId,
                                            MethodType.DELETE, pConnectionServer, "");
        }

        #endregion

    }
}
