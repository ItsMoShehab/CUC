#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The ClassOfService class contains all the properties associated with a COS in Unity Connection that can be fetched 
    /// via the CUPI interface.  This class also contains a number of static and instance methods for finding, editing, adding, deleting
    /// and listing COS isntances.  
    /// </summary>
    public class ClassOfService
    {
        #region Fields and Properties

        //reference to the ConnectionServer object used to create this TransferOption instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of whic properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        //COS properties from the REST interface

        private bool _accessFaxMail;
        public bool AccessFaxMail
        {
            get { return _accessFaxMail; }
            set
            {
                _changedPropList.Add("AccessFaxMail", value);
                _accessFaxMail = value;
            }
        }

        private bool _accessTts;
        public bool AccessTts
        {
            get { return _accessTts; }
            set
            {
                _changedPropList.Add("AccessTts", value);
                _accessTts = value;
            }
        }

        private bool _callHoldAvailable;
        public bool CallHoldAvailable
        {
            get { return _callHoldAvailable; }
            set
            {
                _changedPropList.Add("CallHoldAvailable", value);
                _callHoldAvailable = value;
            }
        }

        private bool _callScreenAvailable;
        public bool CallScreenAvailable
        {
            get { return _callScreenAvailable; }
            set
            {
                _changedPropList.Add("CallScreenAvailable", value);
                _callScreenAvailable = value;
            }
        }

        private bool _canRecordName;
        public bool CanRecordName
        {
            get { return _canRecordName; }
            set
            {
                _changedPropList.Add("CanRecordName", value);
                _canRecordName = value;
            }
        }

        private string _faxRestrictionObjectId;
        public string FaxRestrictionObjectId
        {
            get { return _faxRestrictionObjectId; }
            set
            {
                _changedPropList.Add("FaxRestrictionObjectId", value);
                _faxRestrictionObjectId = value;
            }
        }

        private bool _listInDirectoryStatus;
        public bool ListInDirectoryStatus
        {
            get { return _listInDirectoryStatus; }
            set
            {
                _changedPropList.Add("ListInDirectoryStatus", value);
                _listInDirectoryStatus = value;
            }
        }

        private int _maxGreetingLength;
        public int MaxGreetingLength
        {
            get { return _maxGreetingLength; }
            set
            {
                _changedPropList.Add("MaxGreetingLength", value);
                _maxGreetingLength = value;
            }
        }

        private int _maxMsgLength;
        public int MaxMsgLength
        {
            get { return _maxMsgLength; }
            set
            {
                _changedPropList.Add("MaxMsgLength", value);
                _maxMsgLength = value;
            }
        }

        private int _maxNameLength;
        public int MaxNameLength
        {
            get { return _maxNameLength; }
            set
            {
                _changedPropList.Add("MaxNameLength", value);
                _maxNameLength = value;
            }
        }

        private int _maxPrivateDlists;
        public int MaxPrivateDlists
        {
            get { return _maxPrivateDlists; }
            set
            {
                _changedPropList.Add("MaxPrivateDlists", value);
                _maxPrivateDlists = value;
            }
        }

        private bool _movetoDeleteFolder;
        public bool MovetoDeleteFolder
        {
            get { return _movetoDeleteFolder; }
            set
            {
                _changedPropList.Add("MovetoDeleteFolder", value);
                _movetoDeleteFolder = value;
            }
        }

        private string _outcallRestrictionObjectId;
        public string OutcallRestrictionObjectId
        {
            get { return _outcallRestrictionObjectId; }
            set
            {
                _changedPropList.Add("OutcallRestrictionObjectId", value);
                _outcallRestrictionObjectId = value;
            }
        }

        private bool _personalAdministrator;
        public bool PersonalAdministrator
        {
            get { return _personalAdministrator; }
            set
            {
                _changedPropList.Add("PersonalAdministrator", value);
                _personalAdministrator = value;
            }
        }

        private string _xferRestrictionObjectId;
        public string XferRestrictionObjectId
        {
            get { return _xferRestrictionObjectId; }
            set
            {
                _changedPropList.Add("XferRestrictionObjectId", value);
                _xferRestrictionObjectId = value;
            }
        }

        private int _warnIntervalMsgEnd;
        public int WarnIntervalMsgEnd
        {
            get { return _warnIntervalMsgEnd; }
            set
            {
                _changedPropList.Add("WarnIntervalMsgEnd", value);
                _warnIntervalMsgEnd = value;
            }
        }

        private bool _canSendToPublicDl;
        public bool CanSendToPublicDl
        {
            get { return _canSendToPublicDl; }
            set
            {
                _changedPropList.Add("CanSendToPublicDl", value);
                _canSendToPublicDl = value;
            }
        }

        private bool _enableEnhancedSecurity;
        public bool EnableEnhancedSecurity
        {
            get { return _enableEnhancedSecurity; }
            set
            {
                _changedPropList.Add("EnableEnhancedSecurity", value);
                _enableEnhancedSecurity = value;
            }
        }

        private bool _accessVmi;
        public bool AccessVmi
        {
            get { return _accessVmi; }
            set
            {
                _changedPropList.Add("AccessVmi", value);
                _accessVmi = value;
            }
        }


        private bool _accessLiveReply;
        public bool AccessLiveReply
        {
            get { return _accessLiveReply; }
            set
            {
                _changedPropList.Add("AccessLiveReply", value);
                _accessLiveReply = value;
            }
        }

        private int _uaAlternateExtensionAccess;
        public int UaAlternateExtensionAccess
        {
            get { return _uaAlternateExtensionAccess; }
            set
            {
                _changedPropList.Add("UaAlternateExtensionAccess", value);
                _uaAlternateExtensionAccess = value;
            }
        }

        private bool _accessCallRoutingRules;
        public bool AccessCallRoutingRules
        {
            get { return _accessCallRoutingRules; }
            set
            {
                _changedPropList.Add("AccessCallRoutingRules", value);
                _accessCallRoutingRules = value;
            }
        }

        private int _warnMinMsgLength;
        public int WarnMinMsgLength
        {
            get { return _warnMinMsgLength; }
            set
            {
                _changedPropList.Add("WarnMinMsgLength", value);
                _warnMinMsgLength = value;
            }
        }

        private bool _sendBroadcastMessage;
        public bool SendBroadcastMessage
        {
            get { return _sendBroadcastMessage; }
            set
            {
                _changedPropList.Add("SendBroadcastMessage", value);
                _sendBroadcastMessage = value;
            }
        }

        private bool _updateBroadcastMessage;
        public bool UpdateBroadcastMessage
        {
            get { return _updateBroadcastMessage; }
            set
            {
                _changedPropList.Add("UpdateBroadcastMessage", value);
                _updateBroadcastMessage = value;
            }
        }

        private bool _accessVui;
        public bool AccessVui
        {
            get { return _accessVui; }
            set
            {
                _changedPropList.Add("AccessVui", value);
                _accessVui = value;
            }
        }

        private bool _imapCanFetchMessageBody;
        public bool ImapCanFetchMessageBody
        {
            get { return _imapCanFetchMessageBody; }
            set
            {
                _changedPropList.Add("ImapCanFetchMessageBody", value);
                _imapCanFetchMessageBody = value;
            }
        }

        private bool _imapCanFetchPrivateMessageBody;
        public bool ImapCanFetchPrivateMessageBody
        {
            get { return _imapCanFetchPrivateMessageBody; }
            set
            {
                _changedPropList.Add("ImapCanFetchPrivateMessageBody", value);
                _imapCanFetchPrivateMessageBody = value;
            }
        }

        private int _maxMembersPvl;
        public int MaxMembersPVL
        {
            get { return _maxMembersPvl; }
            set
            {
                _changedPropList.Add("MaxMembersPVL", value);
                _maxMembersPvl = value;
            }
        }

        private bool _accessIMAP;
        public bool AccessIMAP
        {
            get { return _accessIMAP; }
            set
            {
                _changedPropList.Add("AccessIMAP", value);
                _accessIMAP = value;
            }
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _changedPropList.Add("ReadOnly", value);
                _readOnly = value;
            }
        }

        private bool _accessAdvancedUserFeatures;
        public bool AccessAdvancedUserFeatures
        {
            get { return _accessAdvancedUserFeatures; }
            set
            {
                _changedPropList.Add("AccessAdvancedUserFeatures", value);
                _accessAdvancedUserFeatures = value;
            }
        }

        private bool _accessUnifiedClient;
        public bool AccessUnifiedClient
        {
            get { return _accessUnifiedClient; }
            set
            {
                _changedPropList.Add("AccessUnifiedClient", value);
                _accessUnifiedClient = value;
            }
        }

        private int _requireSecureMessages;
        public int RequireSecureMessages
        {
            get { return _requireSecureMessages; }
            set
            {
                _changedPropList.Add("RequireSecureMessages", value);
                _requireSecureMessages = value;
            }
        }

        private bool _accessOutsideLiveReply;
        public bool AccessOutsideLiveReply
        {
            get { return _accessOutsideLiveReply; }
            set
            {
                _changedPropList.Add("AccessOutsideLiveReply", value);
                _accessOutsideLiveReply = value;
            }
        }

        private bool _accessStt;
        public bool AccessSTT
        {
            get { return _accessStt; }
            set
            {
                _changedPropList.Add("AccessSTT", value);
                _accessStt = value;
            }
        }

        private int _enableSttSecureMessage;
        public int EnableSTTSecureMessage
        {
            get { return _enableSttSecureMessage; }
            set
            {
                _changedPropList.Add("EnableSTTSecureMessage", value);
                _enableSttSecureMessage = value;
            }
        }

        private int _messagePlaybackRestriction;
        public int MessagePlaybackRestriction
        {
            get { return _messagePlaybackRestriction; }
            set
            {
                _changedPropList.Add("MessagePlaybackRestriction", value);
                _messagePlaybackRestriction = value;
            }
        }

        private int _sttType;
        public int SttType
        {
            get { return _sttType; }
            set
            {
                _changedPropList.Add("SttType", value);
                _sttType = value;
            }
        }

        private string _displayName;
        public String DisplayName
        {
            get { return _displayName; }
            set
            {
                _changedPropList.Add("DisplayName", value);
                _displayName = value;
            }
        }

        /// <summary>
        /// The unique identifier of the LocationVMS object to which this system distribution list belongs.
        /// </summary>
        [JsonProperty]
        public String LocationObjectId { get; private set; }


        /// <summary>
        /// Unique identifier for this transfer option.
        /// You cannot change the objectID of a standing object.
        /// </summary>
        [JsonProperty]
        public string ObjectId { get; private set; }


        /// <summary>
        /// A flag indicating whether this distribution list can be deleted via an administrative application such as Cisco Unity Connection 
        /// Administration. It is used to prevent deletion of factory defaults
        /// </summary>
        private bool _undeletable;
        public bool Undeletable
        {
            get { return _undeletable; }
            set
            {
                _changedPropList.Add("Undeletable", value);
                _undeletable = value;
            }
        }

        private RestrictionTable _restrictionTableTransfer;
        /// <summary>
        /// Lazy fetch for restriction table associated with COS - this needs to be implemented as a method instead of a 
        /// property so that if a grid is bound to the generic list of objects it doesn't "lazy fetch" it for display purposes resulting
        /// in needless data fetching
        /// </summary>
        /// <param name="pForceRefetchOfData">
        /// Pass as true to force the restriction table to be refetched even if its already be populated earlier.
        /// </param>
        /// <returns>
        /// Instance of the RestrictionTable class 
        /// </returns>
        public RestrictionTable TransferRestrictionTable(bool pForceRefetchOfData = false)
        {
            if (pForceRefetchOfData)
            {
                _restrictionTableTransfer = null;
            }

            if (_restrictionTableTransfer == null)
            {
                try
                {
                    _restrictionTableTransfer = new RestrictionTable(HomeServer, this.XferRestrictionObjectId);
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Console.WriteLine("Failed fetching TransferRestrictionTable:"+ex);
                }
            }

            return _restrictionTableTransfer;
        }


        private RestrictionTable _restrictionTableFax;
        /// <summary>
        /// Lazy fetch for restriction table associated with COS - this needs to be implemented as a method instead of a 
        /// property so that if a grid is bound to the generic list of objects it doesn't "lazy fetch" it for display purposes resulting
        /// in needless data fetching
        /// </summary>
        /// <param name="pForceRefetchOfData">
        /// Pass as true to force the restriction table to be refetched even if its already be populated earlier.
        /// </param>
        /// <returns>
        /// Instance of the RestrictionTable class 
        /// </returns>
        public RestrictionTable FaxRestrictionTable(bool pForceRefetchOfData = false)
        {
            if (pForceRefetchOfData)
            {
                _restrictionTableFax = null;
            }

            if (_restrictionTableFax == null)
            {
                try
                {
                    _restrictionTableFax = new RestrictionTable(HomeServer, this.FaxRestrictionObjectId);
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Console.WriteLine("Failed fetching FaxRestrictionTable:"+ex);
                }
            }

            return _restrictionTableFax;
        }

        private RestrictionTable _restrictionTableOutcall;
        /// <summary>
        /// Lazy fetch for restriction table associated with COS - this needs to be implemented as a method instead of a 
        /// property so that if a grid is bound to the generic list of objects it doesn't "lazy fetch" it for display purposes resulting
        /// in needless data fetching
        /// </summary>
        /// <param name="pForceRefetchOfData">
        /// Pass as true to force the restriction table to be refetched even if its already be populated earlier.
        /// </param>
        /// <returns>
        /// Instance of the RestrictionTable class 
        /// </returns>
        public RestrictionTable OutcallRestrictionTable(bool pForceRefetchOfData = false)
        {
            if (pForceRefetchOfData)
            {
                _restrictionTableOutcall = null;
            }

            if (_restrictionTableOutcall == null)
            {
                try
                {
                    _restrictionTableOutcall = new RestrictionTable(HomeServer, this.OutcallRestrictionObjectId);
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Console.WriteLine("Failed fetching OutcallRestrictionTable:"+ex);
                }
            }

            return _restrictionTableOutcall;
        }


        #endregion

        
        #region Constructor

        /// <summary>
        /// Generic constructor for JSON parsing library
        /// </summary>
        public ClassOfService()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Creates a new instance of the TransferOption class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this entry.  
        /// If you pass the pTransferOptionType parameter the transfer option is automatically filled with data for that entry from the server.  
        /// If no pTransferOptionType is passed an empty instance of the TransferOption class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the transfer option being created.
        /// </param>
        /// <param name="pObjectId"></param>
        /// <param name="pDisplayName"></param>
        public ClassOfService(ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = ""):this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to TransferOption constructor.");
            }

            HomeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or display name then go load that COS up, otherwise just return an empty instance.
            if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pDisplayName))) return;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetClassOfService(pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new Exception(string.Format("COS not found in ClassOfService constructor using ObjectId={0} and DisplayName={1}\n\r{2}"
                                 , pObjectId, pDisplayName, res.ErrorText));
            }
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of coses from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(displayname startswith ab)"
        /// sort: "sort=(displayname asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the lists are being fetched from.
        /// </param>
        /// <param name="pClassOfServices">
        /// The list of coses returned from the CUPI call (if any) is returned as a generic list of ClassOfService class 
        /// instances via this out param.  If no COSes are found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetClassesOfService(ConnectionServer pConnectionServer, out List<ClassOfService> pClassOfServices, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pClassOfServices = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetClassOfService";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "coses", pClauses);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that doesn't mean an error - just return the empty list.
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pClassOfServices = new List<ClassOfService>();
                return res;
            }

            pClassOfServices = HTTPFunctions.GetObjectsFromJson<ClassOfService>(res.ResponseText,"cos");

            if (pClassOfServices == null)
            {
                pClassOfServices = new List<ClassOfService>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pClassOfServices)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// This function allows for a GET of coses from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(displayname startswith ab)"
        /// sort: "sort=(displayname asc)"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the lists are being fetched from.
        /// </param>
        /// <param name="pClassOfServices">
        /// The list of coses returned from the CUPI call (if any) is returned as a generic list of ClassOfService class 
        /// instances via this out param.  If no COSes are found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
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
        public static WebCallResult GetClassesOfService(ConnectionServer pConnectionServer,out List<ClassOfService> pClassOfServices,
            int pPageNumber=1, int pRowsPerPage=20,params string[] pClauses)
        {
            //tack on the paging items to the parameters list
            var temp = pClauses.ToList();
            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            return GetClassesOfService(pConnectionServer, out pClassOfServices, temp.ToArray());
        }

        /// <summary>
        /// returns a single DistributionList object from an ObjectId string passed in or optionally an alias string.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the list is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the list to load
        /// </param>
        /// <param name="pClassOfService">
        /// The out param that the filled out instance of the DistributionList class is returned on.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional alias to search for distribution list on.  If both the ObjectId and alias are passed, the objectID is used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetClassOfService(out ClassOfService pClassOfService, ConnectionServer pConnectionServer, string pObjectId = "", 
            string pDisplayName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pClassOfService = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetClassOfService";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((pObjectId.Length == 0) & (pDisplayName.Length == 0))
            {
                res.ErrorText = "Empty objectId and name passed to GetClassOfServic";
                return res;
            }

            //create a new DistributionList instance passing the ObjectId (or alias) which fills out the data automatically
            try
            {
                pClassOfService = new ClassOfService(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch list in GetClassOfService:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Allows for the creation of a new distribution list on the Connection server directory.  The display name and alias must be provided 
        /// but the extension can be blank.  Other distribution list properties and their values may be passed in via the ConnectonPropertyList 
        /// structure.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the call handler is being added.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new list.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a handlers property name and a new value for that property to apply to the list being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddClassOfService(ConnectionServer pConnectionServer,
                                                    string pDisplayName,
                                                    ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddClassOfService";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for display name in AddClassOfService";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            //cheat here a bit and simply add the alias and display name values to the proplist where it can be tossed into the body later.
            pPropList.Add("DisplayName", pDisplayName);

            string strBody = "<Cos>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Cos>";

            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "coses", MethodType.POST,pConnectionServer,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/coses/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/coses/", "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new distribution list on the Connection server directory.  The alias must be provided but the 
        /// extension can be blank.  
        /// </summary>
        /// <remarks>
        /// This is an alternateive AddDistributionList that passes back a DistributionList object with the newly created list filled 
        /// out in it if the add goes through.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the list is being added.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new distribution list.  
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a list's property name and a new value for that property to apply to the list being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <param name="oClassOfService">
        /// Out parameter that instance of Class of Service class is returned on if a match is found - null if no match is found.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddClassOfService(ConnectionServer pConnectionServer,
                                                    string pDisplayName,
                                                    ConnectionPropertyList pPropList,
                                                    out ClassOfService oClassOfService)
        {
            oClassOfService = null;

            WebCallResult res = AddClassOfService(pConnectionServer, pDisplayName,pPropList);

            //if the create goes through, fetch the list as an object and return it all filled in.
            if (res.Success)
            {
                res = GetClassOfService(out oClassOfService, pConnectionServer, res.ReturnedObjectId);
            }

            return res;
        }

        /// <summary>
        /// Allows one or more properties on a list to be udpated (for instance display name/DTMFAccessID etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the list is homed.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the list to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a list property name and a new value for that property to apply to the list being updated.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one property
        /// pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateClassOfService(ConnectionServer pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateClassOfService";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateClassOfService";
                return res;
            }

            string strBody = "<Cos>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Cos>";

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "coses/" + pObjectId,
                                            MethodType.PUT,pConnectionServer,strBody,false);

        }


        /// <summary>
        /// DELETE a COS from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the COS is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the COS in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteClassOfService(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteClassOfService";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "coses/" + pObjectId,MethodType.DELETE,pConnectionServer, "");
        }



        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays the display name of the COS
        /// </summary>
        public override string ToString()
        {
            return String.Format("Class of service [{0}]", this.DisplayName);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the Class of Service object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the call handler object instance.
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
        /// Pull the data from the Connection server for this object again - if changes have been made externaly this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchClassOfServiceData()
        {
            return GetClassOfService(this.ObjectId);
        }

        /// <summary>
        /// Fills the current instance of DirectoryHandler in with properties fetched from the server.  If both the display name and ObjectId
        /// parameters are provided, the ObjectId is used for the search.
        /// </summary>
        /// <param name="pObjectId">
        /// Unique GUID of the interview handler to fetch - can be blank if the display name is passed in.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name (required to be unique for all interview handlers) to search on an interview handler by.  Can be blank if the ObjectId 
        /// parameter is provided.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetClassOfService(string pObjectId, string pDisplayName = "")
        {
            string strObjectId;

            //when fetching a COS prefer the ObjectId if provided
            if (pObjectId.Length > 0)
            {
                strObjectId = pObjectId;
            }
            else if (pDisplayName.Length > 0)
            {
                //fetch the ObjectId for the name if possible
                strObjectId= GetObjectIdByCosName(pDisplayName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult
                        {
                            Success = false,
                            ErrorText = "No COS found for display name passed into GetClassofService:" + pDisplayName
                        };
                }
            }
            else
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "No value for ObjectId or display name passed to GetClassOfService."
                    };
            }

            string strUrl = string.Format("{0}coses/{1}", HomeServer.BaseUrl, strObjectId);

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
        /// Fetch a COSes ObjectId by it's name - fetching a COS needs to happen via ObjectId so we get all the properties - searching by name returns only the display
        /// name and ObjectId but not all the rest of the properties.
        /// </summary>
        /// <param name="pCosName">
        /// Display name of the COS to search for
        /// </param>
        /// <returns>
        /// ObjectId of the COS with the name if found, or blank string if not.
        /// </returns>
        private string GetObjectIdByCosName(string pCosName)
        {

            string strUrl = string.Format("{0}coses/?query=(DisplayName is {1})", HomeServer.BaseUrl, pCosName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return "";
            }

            List<ClassOfService> oCoses = HTTPFunctions.GetObjectsFromJson<ClassOfService>(res.ResponseText,"Cos");

            foreach (var oCos in oCoses)
            {
                if (oCos.DisplayName.Equals(pCosName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oCos.ObjectId;
                }
            }

            return "";
        }
           


        /// <summary>
        /// Allows one or more properties on a list to be udpated (for instance display name, DTMFAccessID etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the list intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for class of service {0}", this);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateClassOfService(HomeServer, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }

        /// <summary>
        /// DELETE a COS from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeleteClassOfService(HomeServer, ObjectId);
        }


        /// <summary>
        /// If the call handler object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }

        #endregion

    }
}
