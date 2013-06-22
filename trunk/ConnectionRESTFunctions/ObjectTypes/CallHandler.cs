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
using System.Text;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The CallHandler class contains all the properties associated with a Call Handler in Unity Connection that can be fetched via the 
    /// CUPI interface.  This class also contains a number of static and instance methods for finding, deleting, editing and listing 
    /// call handlers. 
    /// </summary>
    public class CallHandler :IUnityDisplayInterface
    {
  
        #region Constructors and Destructors


        /// <summary>
        /// paramaterless constructor for JSON deseralizing path
        /// </summary>
        public CallHandler()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Creates a new instance of the CallHandler class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this handler.  
        /// If you pass the pObjectID or pDisplayName parameter the handler is automatically filled with data for that handler from the server.  
        /// If neither are passed an empty instance of the CallHandler class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the handler being created.
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the handler on the home server provided.  If no ObjectId is passed then an empty instance of the CallHander
        /// class is returned instead.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name search critiera - if both ObjectId and DisplayName are passed, ObjectId is used.  The display name search is not case
        /// sensitive.
        /// </param>
        /// <param name="pIsUserTemplateHandler">
        /// Primary call handlers for user templates are accessed via a seperate URI
        /// </param>
        public CallHandler(ConnectionServerRest pConnectionServer, string pObjectId="",string pDisplayName="", bool pIsUserTemplateHandler=false):this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to CallHandler constructor.");
            }

            HomeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or display name then go load that handler up, otherwise just return an empty instance.
            if ((pObjectId.Length == 0) & (pDisplayName.Length==0)) return;

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetCallHandler(pObjectId,pDisplayName,pIsUserTemplateHandler);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Call Handler not found in CallHandler constructor using ObjectId={0} or " +
                                                                         "DisplayName={1}\n\r{2}", pObjectId,pDisplayName, res.ErrorText));
            }
        }

        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DisplayName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        //reference to the ConnectionServer object used to create this call handler instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;


        #endregion


        #region Call Handler Properties

        private ActionTypes _afterMessageAction;
        /// <summary>
        /// 0 =Ignore , 1=Hangup,2=Goto, 3=Error, 5=SkipGreeting, 4=TakeMsg, 6=RestartGreeting, 7=TransferAltContact, 8=RouteFromNextRule
        /// </summary>
        public ActionTypes AfterMessageAction { 
            get { return _afterMessageAction; } 
            set
            {
                _changedPropList.Add("AfterMessageAction", (int)value);
                _afterMessageAction = value;
            } 
        }

        private ConversationNames _afterMessageTargetConversation;
        public ConversationNames AfterMessageTargetConversation
        {
            set 
            {
                _afterMessageTargetConversation = value;
                _changedPropList.Add("AfterMessageTargetConversation", value.Description());
            }
            get { return _afterMessageTargetConversation; }
        }


        private string _afterMessageTargetHandlerObjectId;
        public string AfterMessageTargetHandlerObjectId
        {
            get { return _afterMessageTargetHandlerObjectId; }
            set
            {
                _afterMessageTargetHandlerObjectId = value;
                _changedPropList.Add("AfterMessageTargetHandlerObjectId", value);
            }
        }


        private string _callSearchSpaceObjectId;
        public string CallSearchSpaceObjectId
        {
            get { return _callSearchSpaceObjectId; }
            set
            {
                _callSearchSpaceObjectId = value;
                _changedPropList.Add("CallSearchSpaceHandlerObjectId", value);
            }
        }


        //you cannot edit the creation time.
         [JsonProperty]
        public DateTime CreationTime { get; private set; }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                _changedPropList.Add("DisplayName", value);
            }
        }

        private bool _dispatchDelivery;
        /// <summary>
        /// A flag indicating that all messages left for the call handler is for dispatch delivery. 
        /// </summary>
        public bool DispatchDelivery
        {
            get { return _dispatchDelivery; }
            set
            {
                _dispatchDelivery = value;
                _changedPropList.Add("DispatchDelivery", value);
            }
        }

        private string _dtmfAccessId;
        public string DtmfAccessId
        {
            get { return _dtmfAccessId; }
            set
            {
                _dtmfAccessId = value;
                _changedPropList.Add("DtmfAccessId", value);
            }
        }

        private bool _editMsg;
        public bool EditMsg
        {
            get { return _editMsg; }
            set
            {
                _editMsg = value;
                _changedPropList.Add("EditMsg", value);
            }
        }

        private bool _enablePrependDigits;
        /// <summary>
        /// Touch-Tone digits to prepended to extension when dialing transfer number
        /// </summary>
        public bool EnablePrependDigits
        {
            get { return _enablePrependDigits; }
            set
            {
                _enablePrependDigits = value;
                _changedPropList.Add("EnablePrependDigits", value);
            }
        }

        private bool _inheritSearchSpaceFromCall;
        /// <summary>
        /// A flag indicating whether the call handler inherits the search space from the call or uses the call handler CallSearchSpaceObject. 
        /// </summary>
        public bool InheritSearchSpaceFromCall
        {
            get { return _inheritSearchSpaceFromCall; }
            set
            {
                _inheritSearchSpaceFromCall = value;
                _changedPropList.Add("InheritSearchSpaceFromCall", value);
            }
        }

        //you cannot change the IsPrimary flag for a handler 
        [JsonProperty]
        public bool IsPrimary { get; private set; }

        //you cannot change the is template setting via CUPI
        public bool IsTemplate { get; private set; }

        private int _language;
        public int Language
        {
            get { return _language; }
            set
            {
                _language = value;
                _changedPropList.Add("Language", value);
            }
        }

        //you cannot change the location objectID
        [JsonProperty]
        public string LocationObjectId { get; private set; }

        private int _maxMsgLen;
        /// <summary>
        /// The maximum recording length (in seconds) for messages left by unidentified callers
        /// </summary>
        public int MaxMsgLen
        {
            get { return _maxMsgLen; }
            set
            {
                _maxMsgLen = value;
                _changedPropList.Add("MaxMsgLen", value);
            }
        }

        private string _mediaSwitchObjectId;
        public string MediaSwitchObjectId
        {
            get { return _mediaSwitchObjectId; }
            set
            {
                _mediaSwitchObjectId = value;
                _changedPropList.Add("MediaSwitchObjectId", value);
            }
        }

        //you cannot edit the ObjectId value
        [JsonProperty]
        public string ObjectId { get; private set; }

        private int _oneKeyDelay;
        /// <summary>
        /// The amount of time (in milliseconds) that Cisco Unity Connection waits for additional input after callers press a single key that is not locked. 
        /// If there is no input within this time, Cisco Unity Connection performs the action assigned to the single key.
        /// </summary>
        public int OneKeyDelay
        {
            get { return _oneKeyDelay; }
            set
            {
                _oneKeyDelay = value;
                _changedPropList.Add("OneKeyDelay", value);
            }
        }

        private string _partitionObjectId;
        public string PartitionObjectId
        {
            get { return _partitionObjectId; }
            set
            {
                _partitionObjectId = value;
                _changedPropList.Add("PartitionObjectId", value);
            }
        }

        private PlayAfterMessageTypes _playAfterMessage;
        public PlayAfterMessageTypes PlayAfterMessage
        {
            get { return _playAfterMessage; }
            set
            {
                _playAfterMessage = value;
                _changedPropList.Add("PlayAfterMessage", (int)value);
            }
        }


        private PlayPostGreetingRecordingTypes _playPostGreetingRecording;
        public PlayPostGreetingRecordingTypes PlayPostGreetingRecording
        {
            get { return _playPostGreetingRecording; }
            set
            {
                _playPostGreetingRecording = value;
                _changedPropList.Add("PlayPostGreetingRecording", (int)value);
            }
        }

        private string _postGreetingRecordingObjectId;
        public string PostGreetingRecordingObjectId
        {
            get { return _postGreetingRecordingObjectId; }
            set
            {
                _postGreetingRecordingObjectId = value;
                _changedPropList.Add("PostGreetingRecordingObjectId", value);
            }
        }


        private string _prependDigits;
        /// <summary>
        /// Touch-Tone digits to prepended to extension when dialing transfer number 
        /// </summary>
        public string PrependDigits
        {
            get { return _prependDigits; }
            set
            {
                _prependDigits = value;
                _changedPropList.Add("PrependDigits", value);
            }
        }

        private string _recipientContactObjectId;
        public string RecipientContactObjectId
        {
            get { return _recipientContactObjectId; }
            set
            {
                _recipientContactObjectId = value;
                _changedPropList.Add("RecipientContactObjectId", value);
            }
        }

        private string _recipientDistributionListObjectId;
        public string RecipientDistributionListObjectId
        {
            get { return _recipientDistributionListObjectId; }
            set
            {
                _recipientDistributionListObjectId = value;
                _changedPropList.Add("RecipientDistributionListObjectId", value);
            }
        }

        private string _recipientSubscriberObjectId;
        public string RecipientSubscriberObjectId
        {
            get { return _recipientSubscriberObjectId; }
            set
            {
                _recipientSubscriberObjectId = value;
                _changedPropList.Add("RecipientSubscriberObjectId", value);
            }
        }

        private string _scheduleSetObjectId;
        public string ScheduleSetObjectId
        {
            get { return _scheduleSetObjectId; }
            set
            {
                _scheduleSetObjectId = value;
                _changedPropList.Add("ScheduleSetObjectId", value);
            }
        }


        private ModeYesNoAsk _sendPrivateMsg;
        /// <summary>
        /// A flag indicating whether an unidentified caller can mark a message as secure.
        /// </summary>
        public ModeYesNoAsk SendPrivateMsg
        {
            get { return _sendPrivateMsg; }
            set
            {
                _sendPrivateMsg = value;
                _changedPropList.Add("SendPrivateMsg",(int) value);
            }
        }


        private bool _sendSecureMsg;
        /// <summary>
        /// A flag indicating whether an unidentified caller can mark a message as secure.
        /// </summary>
        public bool SendSecureMsg
        {
            get { return _sendSecureMsg; }
            set
            {
                _sendSecureMsg = value;
                _changedPropList.Add("SendSecureMsg", value);
            }
        }

        private ModeYesNoAsk _sendUrgentMsg;
        /// <summary>
        /// A flag indicating whether an unidentified caller can mark a message as urgent.
        /// 1=Always, 2=Ask, 0=Never
        /// </summary>
        public ModeYesNoAsk SendUrgentMsg
        {
            get { return _sendUrgentMsg; }
            set
            {
                _sendUrgentMsg = value;
                _changedPropList.Add("SendUrgentMsg",(int) value);
            }
        }

        private int _timeZone;
        public int TimeZone
        {
            get { return _timeZone; }
            set
            {
                _timeZone = value;
                _changedPropList.Add("TimeZone", value);
            }
        }

        private bool _undeletable;
        public bool Undeletable
        {
            get { return _undeletable; }
            set
            {
                _undeletable = value;
                _changedPropList.Add("Undeletable", value);
            }
        }

        private bool _useCallLanguage;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will use the language assigned to the call
        /// </summary>
        public bool UseCallLanguage
        {
            get { return _useCallLanguage; }
            set
            {
                _useCallLanguage = value;
                _changedPropList.Add("UseCallLanguage", value);
            }
        }

        private bool _useDefaultLanguage;
        public bool UseDefaultLanguage
        {
            get { return _useDefaultLanguage; }
            set
            {
                _useDefaultLanguage = value;
                _changedPropList.Add("UseDefaultLanguage", value);
            }
        }

        private bool _useDefaultTimeZone;
        public bool UseDefaultTimeZone
        {
            get { return _useDefaultTimeZone; }
            set
            {
                _useDefaultTimeZone = value;
                _changedPropList.Add("UseDefaultTimeZone", value);
            }
        }
        
        //voice name needs to be edited via a special interface
        [JsonProperty]
        public string VoiceName { get;  private set; }

        //items that are NULL by default but get fetched on the fly when referenced

        private List<TransferOption> _transferOptions;
        public List<TransferOption> GetTransferOptions(bool pForceDataRefetch = false)
        {
            if (pForceDataRefetch)
            {
                _transferOptions = null;
            }
            //fetch transfer options only if they are referenced
            if (_transferOptions == null)
            {
                GetTransferOptions(out _transferOptions);
            }

            return _transferOptions;
        }

        private List<Greeting> _greetings;
        public List<Greeting> GetGreetings(bool pForceDataRefetch=false)
        {
            if (pForceDataRefetch)
            {
                _greetings = null;
            }

            //fetch greetings only if they are referenced
            if (_greetings == null)
            {
                GetGreetings(out _greetings);
            }

            return _greetings;
        }

        private List<MenuEntry> _menuEntries;
        public List<MenuEntry> GetMenuEntries(bool pForceDataRefetch = false)
        {
            if (pForceDataRefetch)
            {
                _menuEntries = null;
            }
            //fetch menu entries only if they are referenced
            if (_menuEntries == null)
            {
                GetMenuEntries(out _menuEntries);
            }

            return _menuEntries;
        }

        //call handlers are associated with schedule sets which can contain one or more schedule definitions 
        //under that - this the scheduleset instance from which the other items can be derived.
        private ScheduleSet _scheduleSet;
        public ScheduleSet GetScheduleSet(bool pForceDataRefetch = false)
        {
            if (pForceDataRefetch)
            {
                _scheduleSet = null;
            }

            if (_scheduleSet == null)
            {
                _scheduleSet = new ScheduleSet(HomeServer,ScheduleSetObjectId);
            }
            return _scheduleSet;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of handlers from Connection via HTTP using a simple paging scheme.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handlers are being fetched from.
        /// </param>
        /// <param name="pCallHandlers">
        /// The list of handlers returned from the CUPI call (if any) is returned as a generic list of CallHAndler class instances via this out param.  
        /// If no handlers are  found an empty list is returned.
        /// </param>
        /// <param name="pPageNumber">
        /// page number to fetch (0 means just get count, first page is 1)
        /// </param>
        /// <param name="pRowsPerPage">
        /// How many handlers to fetch at one time - defaults to 10
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetCallHandlers(ConnectionServerRest pConnectionServer,out List<CallHandler> pCallHandlers,
            int pPageNumber = 1, int pRowsPerPage = 10, params string[] pClauses)
        {
            //tack on the paging items to the parameters list
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

            return GetCallHandlers(pConnectionServer, out pCallHandlers, temp.ToArray());
        }

        /// <summary>
        /// This function allows for a GET of handlers from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(displayname startswith ab)"
        /// sort: "sort=(displayname asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handlers are being fetched from.
        /// </param>
        /// <param name="pCallHandlers">
        /// The list of handlers returned from the CUPI call (if any) is returned as a generic list of CallHAndler class instances via this out param.  
        /// If no handlers are  found an empty list is returned.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetCallHandlers(ConnectionServerRest pConnectionServer, out List<CallHandler> pCallHandlers, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCallHandlers = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetCallHandlers";
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "handlers/callhandlers", pClauses);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                pCallHandlers = new List<CallHandler>();
                return res;
            }

            //not an error, just no handlers returned in query - return empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                pCallHandlers = new List<CallHandler>();
                return res;
            }

            pCallHandlers = pConnectionServer.GetObjectsFromJson<CallHandler>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oHandler in pCallHandlers)
            {
                oHandler.HomeServer = pConnectionServer;
                oHandler.ClearPendingChanges();
            }

            return res;
        }

        /// <summary>
        /// returns a single CallHandler object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the handler is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the handler to load
        /// </param>
        /// <param name="pCallHandler">
        /// The out param that the filled out instance of the CallHandler class is returned on.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name to search for call handler on.  If both the ObjectId and display name are passed, the objectID is used.
        /// The display name search is not case sensitive.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetCallHandler(out CallHandler pCallHandler,ConnectionServerRest pConnectionServer, string pObjectId="", string pDisplayName="")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCallHandler = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetCallHandler";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((pObjectId.Length==0) & (pDisplayName.Length==0))
            {
                res.ErrorText = "Empty objectId and display name passed to GetCallHandler";
                return res;
            }

            //create a new CallHandler instance passing the ObjectId (or display name) which fills out the data automatically
            try
            {
                pCallHandler = new CallHandler(pConnectionServer, pObjectId,pDisplayName);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new call handler on the Connection server directory.  The display name must be provided but the 
        /// extension can be blank.  The ObjectId a template to use when creating the new handler is required, however other handler 
        /// properties and their values may be passed in via the ConnectonPropertyList structure.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the call handler is being added.
        /// </param>
        /// <param name="pTemplateObjectId">
        /// The ObjectId of a call handler template on Connection - this provides important details such as dial partition assignment.  It's
        /// required and must exist on the server or the user creation will fail.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new call handler.  This must be unique against all handlers on the local Connection server.
        /// </param>
        /// <param name="pExtension">
        /// The extension number to be assigned to the new handler.  This may be blank. The partition is determined by the call handler
        /// template provided in the pTempalteObjectID parameter.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a handlers property name and a new value for that property to apply to the handler being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddCallHandler(ConnectionServerRest pConnectionServer, 
                                                    string pTemplateObjectId, 
                                                    string pDisplayName, 
                                                    string pExtension, 
                                                    ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddCallHandler";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pTemplateObjectId) || string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddCallHandler";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            //cheat here a bit and simply add the alias and extension values to the proplist where it can be tossed into the body later.
            pPropList.Add("DisplayName", pDisplayName);
            
            if (pExtension.Length>0)
            {
                pPropList.Add("DtmfAccessId", pExtension);
            }

            string strBody = "<Callhandler>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Callhandler>";

            res = pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "handlers/callhandlers?templateObjectId=" + pTemplateObjectId, 
                            MethodType.POST,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/handlers/callhandlers/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/handlers/callhandlers/", "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new call handler on the Connection server directory.  The display name must be provided but the 
        /// extension can be blank.  The ObjectId a template to use when creating the new handler is required, however other handler 
        /// properties and their values may be passed in via the ConnectonPropertyList structure.
        /// </summary>
        /// <remarks>
        /// This is an alternateive AddCallHandler that passes back a CallHandler object with the newly created handler filled out in it if the 
        /// add goes through.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the call handler is being added.
        /// </param>
        /// <param name="pTemplateObjectId">
        /// The ObjectId of a call handler template on Connection - this provides important details such as dial partition assignment.  It's
        /// required and must exist on the server or the user creation will fail.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new call handler.  This must be unique against all handlers on the local Connection server.
        /// </param>
        /// <param name="pExtension">
        /// The extension number to be assigned to the new handler.  This may be blank. The partition is determined by the call handler
        /// template provided in the pTempalteObjectID parameter.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a handlers property name and a new value for that property to apply to the handler being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <param name="oCallHandler">
        /// Out parameter that returns an instance of a CallHandler object if the creation completes ok.  If the creation fails then this is 
        /// returned as NULL.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddCallHandler(ConnectionServerRest pConnectionServer, 
                                                    string pTemplateObjectId, 
                                                    string pDisplayName, 
                                                    string pExtension, 
                                                    ConnectionPropertyList pPropList,
                                                    out CallHandler oCallHandler)
           {
            oCallHandler = null;

               WebCallResult res = AddCallHandler(pConnectionServer, pTemplateObjectId, pDisplayName, pExtension, pPropList);

               //if the create goes through, fetch the handler as an object and return it.
               if (res.Success)
               {
                   res = GetCallHandler(out oCallHandler,pConnectionServer, res.ReturnedObjectId);
               }

               return res;

           }



        /// <summary>
        /// DELETE a handler from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handler is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the handler in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteCallHandler(ConnectionServerRest pConnectionServer, string pObjectId)
        {
            WebCallResult res = new WebCallResult{Success = false};
            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteCallHandler";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to DeleteCallHandler";
                return res;
            }

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "handlers/callhandlers/" + pObjectId,
                                            MethodType.DELETE, "");
        }


        /// <summary>
        /// Allows one or more properties on a handler to be udpated (for instance display name/DTMFAccessID etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handler is homed.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the handler to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a handler property name and a new value for that property to apply to the handler being updated.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one property
        /// pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateCallHandler(ConnectionServerRest pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateCallHandler";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateCallHandler";
                return res;
            }

            string strBody = "<CallHandler>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</CallHandler>";

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "handlers/callhandlers/" + pObjectId,
                                            MethodType.PUT,strBody,false);

        }


        /// <summary>
        /// Fetches the WAV file for a handler's voice name and stores it on the Windows file system at the file location specified.  If the handler does 
        /// not have a voice name recorded, the WebcallResult structure returns false in the success proeprty and notes the handler has no voice name in 
        /// the error text.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handler is homed.
        /// </param>
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the handler's voice name at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the handler.  
        /// </param>
        /// <param name="pConnectionWavFileName">
        /// The the Connection stream file name is already known it can be passed in here and the handler lookup does not need to take place.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetCallHandlerVoiceName(ConnectionServerRest pConnectionServer, string pTargetLocalFilePath, string pObjectId, 
            string pConnectionWavFileName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetCallHandlerVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pTargetLocalFilePath) || (Directory.GetParent(pTargetLocalFilePath).Exists == false))
            {
                res.ErrorText = "Invalid local file path passed to GetCallHandlerVoiceName: " + pTargetLocalFilePath;
                return res;
            }

            //if the WAV file name itself is passed in that's all we need, otherwise we need to go do a CallHandler fetch with the ObjectId 
            //and pull the VoiceName wav file name from there (if it's present).
            //fetch the call handler info which has the VoiceName property on it
            if (string.IsNullOrEmpty(pConnectionWavFileName))
            {
                CallHandler oCallHandler;

                try
                {
                    oCallHandler = new CallHandler(pConnectionServer, pObjectId);
                }
                catch (UnityConnectionRestException ex)
                {
                    return ex.WebCallResult;
                }

                //the property will be null if no voice name is recorded for the handler.
                if (string.IsNullOrEmpty(oCallHandler.VoiceName))
                {
                    res = new WebCallResult();
                    res.Success = false;
                    res.ErrorText = "No voice named recorded for call handler.";
                    return res;
                }

                pConnectionWavFileName = oCallHandler.VoiceName;
            }
            //fetch the WAV file
            return pConnectionServer.DownloadWavFile(pTargetLocalFilePath,pConnectionWavFileName);
        }


        /// <summary>
        /// Uploads a WAV file indicated as a voice name for the target call handler referenced by the pObjectID value.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the call handler is homed.
        /// </param>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as a voice name for the handler referenced.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the call handler to upload the voice name WAV file for.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// If passed as TRUE the routine will attempt to convert the target WAV file into raw PCM first before uploading it to the Connection
        /// server.  A failure to convert will be considered a failed upload attempt and false is returned.  This value defaults to FALSE meaning
        /// the file will attempt to be uploaded as is.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetCallHandlerVoiceName(ConnectionServerRest pConnectionServer, string pSourceLocalFilePath, string pObjectId, 
            bool pConvertToPcmFirst = false)
        {
            string strConvertedWavFilePath = "";
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetCallHandlerVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pSourceLocalFilePath) || (File.Exists(pSourceLocalFilePath) == false))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid local file path passed to SetCallHandlerVoiceName: " + pSourceLocalFilePath;
                return res;
            }

            //if the user wants to try and rip the WAV file into PCM 16/8/1 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pSourceLocalFilePath);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into PCM format in SetCallHandlerVoiceName.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted PCM WAV file path not found in SetCallHandlerVoiceName: " + strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pSourceLocalFilePath = strConvertedWavFilePath;

            }

            //use the 8.5 and later voice name formatting here which simplifies things a great deal.
            string strResourcePath = string.Format(@"{0}handlers/callhandlers/{1}/voicename", pConnectionServer.BaseUrl, pObjectId);

            //upload the WAV file to the server.
            res = pConnectionServer.UploadWavFile(strResourcePath, pSourceLocalFilePath);

            //if we converted a file to G711 in the process clean up after ourselves here. Only delete it if the upload was good - otherwise
            //keep it around as it may be useful for diagnostic purposes.
            if (res.Success && !string.IsNullOrEmpty(strConvertedWavFilePath) && File.Exists(strConvertedWavFilePath))
            {
                try
                {
                    File.Delete(strConvertedWavFilePath);
                }
                catch (Exception ex)
                {
                    //this is not a show stopper error - just report it back but still return success if that's what we got back from the 
                    //wav upload routine.
                    res.ErrorText = "(warning) failed to delete temporary PCM wav file in SetCallHandlerVoiceName:" + ex.Message;
                }
            }
            return res;
        }


        /// <summary>
        /// If you have a recording stream already recorded and in the stream files table on the Connection server (for instance
        /// you are using the telephone as a media device) you can assign a recording stream file directly to a voice name using this 
        /// method instead of uploading a WAV file from the local hard drive.
        /// </summary>
        /// <param name="pConnectionServer" type="ConnectionServer">
        ///   The Connection server that houses the voice name to be updated      
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the handler to apply the stream file to the voice name for.
        /// </param>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the recording stream to be assigned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetCallHandlerVoiceNameToStreamFile(ConnectionServerRest pConnectionServer, string pObjectId,
                                                     string pStreamFileResourceName)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetCallHandlerVoiceNameToStreamFile";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to SetCallHandlerVoiceNameToStreamFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pStreamFileResourceName))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid stream file resource id passed to SetCallHandlerVoiceNameToStreamFile";
                return res;
            }

            //construct the full URL to call for uploading the voice name file
            string strUrl = string.Format(@"{0}handlers/callhandlers/{1}/voicename", pConnectionServer.BaseUrl, pObjectId);

            Dictionary<string, string> oParams = new Dictionary<string, string>();

            oParams.Add("op", "RECORD");
            oParams.Add("ResourceType", "STREAM");
            oParams.Add("resourceId", pStreamFileResourceName);
            oParams.Add("lastResult", "0");
            oParams.Add("speed", "100");
            oParams.Add("volume", "100");
            oParams.Add("startPosition", "0");

            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.PUT, oParams);

            return res;
        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays the display name and extension of the handler by default.
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} x{1}", this.DisplayName, this.DtmfAccessId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the call handler object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the call handler object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix="")
        {
            StringBuilder strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n",pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
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
        public WebCallResult RefetchCallHandlerData()
        {
            return GetCallHandler(this.ObjectId);
        }



        //Fills the current instance of CallHandler in with properties fetched from the server.
        private WebCallResult GetCallHandler(string pObjectId, string pDisplayName="", bool pIsUserTemplateHandler=false)
        {
            string strUrl;
            WebCallResult res;

            //when fetching a handler use the query construct in both cases so the XML parsing is identical
            if (pObjectId.Length > 0)
            {
                //primary handlers of user templates are stored down a seperate URI even though they're in the same table for whatever reason - 
                //have to special case it here.
                if (pIsUserTemplateHandler)
                {
                    strUrl = string.Format("{0}callhandlerprimarytemplates/?query=(ObjectId is {1})", HomeServer.BaseUrl, pObjectId);
                }
                else
                {
                    strUrl = string.Format("{0}handlers/callhandlers/?query=(ObjectId is {1})", HomeServer.BaseUrl, pObjectId);    
                }
                
            }
            else if (pDisplayName.Length > 0)
            {
                strUrl = string.Format("{0}handlers/callhandlers/?query=(DisplayName is {1})", HomeServer.BaseUrl, pDisplayName.UriSafe());
            }
            else
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "No value for ObjectId or Alias passed to GetCallHandler."
                    };

            }
            
            //issue the command to the CUPI interface
            res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            if (res.TotalObjectCount == 0)
            {
                res.ErrorText = "Call handler not found";
                res.Success = false;
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(ConnectionServerRest.StripJsonOfObjectWrapper(res.ResponseText, "Callhandler"), this,
                    RestTransportFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
                return res;
            }

            //all the updates above will flip pending changes into the queue - clear that here.
            this.ClearPendingChanges();
            return res;
        }


        /// <summary>
        /// If the call handler object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
          	_changedPropList.Clear();
        }


        /// <summary>
        /// Allows one or more properties on a handler to be udpated (for instance display name, DTMFAccessID etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;
            
            //check if the handler intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
              	res=new WebCallResult();
                res.Success = false;
                res.ErrorText =string.Format("Update called but there are no pending changes for call handler {0}",this);
                return res;
            }

            //just call the static method with the info from the instance 
            res=UpdateCallHandler(HomeServer, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
              	_changedPropList.Clear();
            }

            return res;
        }

        /// <summary>
        /// DELETE a call handler from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeleteCallHandler(HomeServer, ObjectId);
        }


        /// <summary>
        /// Uploads a WAV file indicated as a voice name for the target handler
        /// </summary>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as a voice name for the handler referenced.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// If passed as TRUE the routine will attempt to convert the target WAV file into raw PCM first before uploading it to the Connection
        /// server.  A failure to convert will be considered a failed upload attempt and false is returned.  This value defaults to FALSE meaning
        /// the file will attempt to be uploaded as is.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetVoiceName(string pSourceLocalFilePath, bool pConvertToPcmFirst = false)
        {
            //just call the static method with the information from the instance
            return SetCallHandlerVoiceName(HomeServer, pSourceLocalFilePath, ObjectId, pConvertToPcmFirst);
        }

        
        /// <summary>
        /// If you have a recording stream already recorded and in the stream files table on the Connection server (for instance
        /// you are using the telephone as a media device) you can assign a recording stream file directly to a voice name using this 
        /// method instead of uploading a WAV file from the local hard drive.
        /// </summary>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the recording stream to be assigned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetVoiceNameToStreamFile(string pStreamFileResourceName)
        {
            return SetCallHandlerVoiceNameToStreamFile(HomeServer, ObjectId, pStreamFileResourceName);
        }

        /// <summary>
        /// Fetches the WAV file for a handler's voice name and stores it on the Windows file system at the file location specified.  If the handler does 
        /// not have a voice name recorded, the WebcallResult structure returns false in the success proeprty and notes the handler has no voice name in 
        /// the error text.
        /// </summary>
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the handler's voice name at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetVoiceName(string pTargetLocalFilePath)
        {
            //just call the static method with the info from the instance of this object
            return GetCallHandlerVoiceName(HomeServer, pTargetLocalFilePath, ObjectId, VoiceName);
        }


        //helper function used when a call is made to get at the list of transfer options for the handler instance - the public interface is up in the 
        //properties section and keeps the list of transfer options around once they've been fetched.
        private WebCallResult GetTransferOptions(out List<TransferOption> pTransferOptions)
        {
            return TransferOption.GetTransferOptions(HomeServer, ObjectId, out pTransferOptions);
        }

        /// <summary>
        /// Pass in the transfer option type (Standard, Off Hours, Alternate) and this will return an instance of the TransferOption class for that
        /// transfer rule (if found).  
        /// </summary>
        /// <remarks>
        /// This routine will fetch the full list of transfer options if they have not yet been fetched for this handler and return the one of interest.
        /// If the transfer options have already been fetched it simply returns the appropriate instance.
        /// </remarks>
        /// <param name="pTransferOptionType">
        /// The transfer option to fetch associated with the call handler (Standard, Off Hours, Alternate).
        /// </param>
        /// <param name="pTransferOption">
        /// Out param on which the transfer option is passed.  If there is an error finding the option then null is returned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetTransferOption(TransferOptionTypes pTransferOptionType, out TransferOption pTransferOption)
        {
            WebCallResult res;

            pTransferOption = null;

            //fetch the full transfer list if it hasn't been fetched yet.
            if (_transferOptions==null)
            {
                res = GetTransferOptions(out _transferOptions);
                
                //if there's some sort of error getting the list, pass it back and bail.
                if (res.Success==false)
                {
                    return res;
                }
            }

            //get the correct rule off the list
            res = new WebCallResult();
            
            foreach (TransferOption oOption in _transferOptions)
            {
                if (oOption.TransferOptionType == pTransferOptionType)
                {
                    pTransferOption = oOption;
                    res.Success = true;
                    return res;
                }
            }

            //if we're here then there was a probllem
            res.Success = false;
            res.ErrorText = "Could not find Transfer option=" + pTransferOptionType;
            return res;
        }


        //helper function used when a call is made to get at the list of menu options for the handler instance
        private WebCallResult GetMenuEntries(out List<MenuEntry> pMenuEntries)
        {
            return MenuEntry.GetMenuEntries(HomeServer, ObjectId, out pMenuEntries);

        }

        /// <summary>
        /// Pass in the menu entry key (0-9, * or #) and this will return an instance of the MenuEntry class for that key (if found).
        /// </summary>
        /// <remarks>
        /// This routine will fetch the full list of menu entries if they have not yet been fetched for this handler and return the one of interest.
        /// If the menu entries have already been fetched it simply returns the appropriate instance.
        /// </remarks>
        /// <param name="pMenuKey">
        /// The menu key to fetch associated with the call handler (0-9, * or #).
        /// </param>
        /// <param name="pMenuEntry">
        /// Out param on which the menu entry is passed back.  If there is an error finding the entry then null is returned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetMenuEntry(string pMenuKey, out MenuEntry pMenuEntry)
        {
            WebCallResult res;

            pMenuEntry = null;

            //fetch the full menu entry list if it hasn't been fetched yet.
            if (_menuEntries == null)
            {
                res = GetMenuEntries(out _menuEntries);

                //if there's some sort of error getting the list, pass it back and bail.
                if (res.Success == false)
                {
                    return res;
                }
            }

            //get the correct rule off the list
            res = new WebCallResult();

            foreach (MenuEntry oMenuEntry in _menuEntries)
            {
                //no need for case insensitivity here - just numbers and #,* are accepted
                if (oMenuEntry.TouchtoneKey.Equals(pMenuKey))
                {
                    pMenuEntry = oMenuEntry;
                    res.Success = true;
                    return res;
                }
            }

            //if we're here then there was a probllem
            res.Success = false;
            res.ErrorText = "Could not find menu entry for key=" + pMenuKey;
            return res;
        }



        //helper function used when a call is made to get at the list of menu options for the handler instance
        private WebCallResult GetGreetings(out List<Greeting> pGreetings)
        {
            return Greeting.GetGreetings(HomeServer, ObjectId, out pGreetings);
        }

        /// <summary>
        /// Pass in the greeting type (Standard, Off Hours, Alternate etc...) and this will return an instance of the Greeting class for that
        /// rule (if found).  
        /// </summary>
        /// <remarks>
        /// This routine will fetch the full list of greetings if they have not yet been fetched for this handler and return the one of interest.
        /// If the greetings have already been fetched it simply returns the appropriate instance.
        /// </remarks>
        /// <param name="pGreetingType">
        /// The greeting type to fetch associated with the call handler (Standard, Off Hours, Alternate, Busy, Internal, Error, Holiday).
        /// </param>
        /// <param name="pGreeting">
        /// Out param on which the greeting is passed back.  If there is an error finding the greeting then null is returned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetGreeting(GreetingTypes pGreetingType, out Greeting pGreeting)
        {
            WebCallResult res;

            pGreeting = null;

            //fetch the full greeting list if it hasn't been fetched yet.
            if (_greetings == null)
            {
                res = GetGreetings(out _greetings);

                //if there's some sort of error getting the list, pass it back and bail.
                if (res.Success == false)
                {
                    return res;
                }
            }

            //get the correct rule off the list
            res = new WebCallResult();

            foreach (Greeting oGreeting in _greetings)
            {
                //do a case insensitive search just for grins
                if (oGreeting.GreetingType == pGreetingType)
                {
                    pGreeting = oGreeting;
                    res.Success = true;
                    return res;
                }
            }

            //if we're here then there was a probllem
            res.Success = false;
            res.ErrorText = "Could not find Greeting rule=" + pGreetingType;
            return res;
        }

        #endregion

    }
}
