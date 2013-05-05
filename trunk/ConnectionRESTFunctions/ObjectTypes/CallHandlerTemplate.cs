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
    /// The call handler template class is used only to provide an interface for user to select which template to use when creating new call 
    /// handlers.  
    /// </summary>
    public class CallHandlerTemplate
    {

        #region Constructors and Destructors

        /// <summary>
        /// default constructor used by JSON parser
        /// </summary>
        public CallHandlerTemplate()
        {
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Constructor requires ConnectionServer instance for where the template is homed on and will take the ObjectId or 
        /// the display name of the template to load.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the template is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectID of the template to load - can be passed as blank if using name to lookup template.
        /// </param>
        /// <param name="pDisplayName">
        /// display name to look for - it's possible to have multiple display names that conflict so the first match 
        /// is returned in this case.
        /// </param>
        public CallHandlerTemplate(ConnectionServer pConnectionServer, string pObjectId, string pDisplayName = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to CallHandlerTemplate construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                return;
            }

            WebCallResult res = GetCallHandlerTemplate(pObjectId, pDisplayName);
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,"Failed to fetch handler template by alias or objectId:" + res);
            }

        }

        #endregion


        #region Fields and Properties

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        public ConnectionServer HomeServer { get; private set; }

        #endregion


        #region Call Handler Template Properties

        private int _afterMessageAction;
        /// <summary>
        /// 0 =Ignore , 1=Hangup,2=Goto, 3=Error, 5=SkipGreeting, 4=TakeMsg, 6=RestartGreeting, 7=TransferAltContact, 8=RouteFromNextRule
        /// </summary>
        public int AfterMessageAction
        {
            get { return _afterMessageAction; }
            set
            {
                _changedPropList.Add("AfterMessageAction", value);
                _afterMessageAction = value;
            }
        }

        private string _afterMessageTargetConversation;
        public string AfterMessageTargetConversation
        {
            set
            {
                _afterMessageTargetConversation = value;
                _changedPropList.Add("AfterMessageTargetConversation", value);
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

        [JsonProperty]
        public DateTime CreationTime { get; private set; }

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

        private int _playAfterMessage;
        public int PlayAfterMessage
        {
            get { return _playAfterMessage; }
            set
            {
                _playAfterMessage = value;
                _changedPropList.Add("PlayAfterMessage", value);
            }
        }

        private int _playPostGreetingRecording;
        public int PlayPostGreetingRecording
        {
            get { return _playPostGreetingRecording; }
            set
            {
                _playPostGreetingRecording = value;
                _changedPropList.Add("PlayPostGreetingRecording", value);
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

        private int _sendPrivateMsg;
        /// <summary>
        /// A flag indicating whether an unidentified caller can mark a message as secure.
        /// </summary>
        public int SendPrivateMsg
        {
            get { return _sendPrivateMsg; }
            set
            {
                _sendPrivateMsg = value;
                _changedPropList.Add("SendPrivateMsg", value);
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

        private int _sendUrgentMsg;
        /// <summary>
        /// A flag indicating whether an unidentified caller can mark a message as urgent.
        /// 1=Always, 2=Ask, 0=Never
        /// </summary>
        public int SendUrgentMsg
        {
            get { return _sendUrgentMsg; }
            set
            {
                _sendUrgentMsg = value;
                _changedPropList.Add("SendUrgentMsg", value);
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

    

        #endregion

        
        #region Static Methods

        /// <summary>
        /// Gets the list of all call handler templates and resturns them as a generic list of CallHandlerTemplate objects.  This
        /// list can be used for providing drop down list selection for handler creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the templates should be pulled from
        /// </param>
        /// <param name="pCallHandlerTemplates">
        /// Out parameter that is used to return the list of CallHandlerTemplate objects defined on Connection - there must be at least one.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetCallHandlerTemplates(ConnectionServer pConnectionServer, out List<CallHandlerTemplate> pCallHandlerTemplates
            , int pPageNumber = 1, int pRowsPerPage = 20, params string[] pClauses)
        {
            WebCallResult res;
            pCallHandlerTemplates = new List<CallHandlerTemplate>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetCallHandlerTemplates";
                return res;
            }

            var temp = pClauses.ToList();
            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "callhandlertemplates", temp.ToArray());

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one template
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount==0)
            {
                res.Success = false;
                return res;
            }

            pCallHandlerTemplates = HTTPFunctions.GetObjectsFromJson<CallHandlerTemplate>(res.ResponseText);

            if (pCallHandlerTemplates == null)
            {
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pCallHandlerTemplates)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// Fetch a single instance of a CallHandlerTemplate using the objectId or name of the template
        /// </summary>
        /// <param name="pCallHandlerTemplate">
        /// Pass back the instance of the handler template on this parameter
        /// </param>
        /// <param name="pConnectionServer">
        /// Connection server being searched
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the template to fetch
        /// </param>
        /// <param name="pDisplayName">
        /// Display name of template to search for
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult GetCallHandlerTemplate(out CallHandlerTemplate pCallHandlerTemplate, ConnectionServer pConnectionServer, 
            string pObjectId = "",string pDisplayName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCallHandlerTemplate = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetCallHandlerTemplate";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((pObjectId.Length == 0) & (pDisplayName.Length == 0))
            {
                res.ErrorText = "Empty objectId and DisplayName passed to GetCallHandlerTemplate";
                return res;
            }

            //create a new CallHandlerTemplate instance passing the ObjectId (or alias) which fills out the data automatically
            try
            {
                pCallHandlerTemplate = new CallHandlerTemplate(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch handler template in GetCallHandlerTemplate:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchUserTemplateData()
        {
            return GetCallHandlerTemplate(this.ObjectId,"");
        }


        /// <summary>
        /// Create a new call handler template in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// The ObjectId of a the media switch this call handler should be associated with
        /// </param>
        /// <param name="pDisplayName">
        /// Display Name of the new call handler template - must be unique.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a property name and a new value for that property to apply to the template being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddCallHandlerTemplate(ConnectionServer pConnectionServer, string pDisplayName,
            string pMediaSwitchObjectId, string pRecipientDistributionListId, string pRecipientUserId,
            ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddCallHandlerTemplate";
                return res;
            }

            if (String.IsNullOrEmpty(pDisplayName) || string.IsNullOrEmpty(pMediaSwitchObjectId))
            {
                res.ErrorText = "Empty value passed for display name or template objectId in AddCallHandlerTemplate";
                return res;
            }

            if (string.IsNullOrEmpty(pRecipientDistributionListId) & string.IsNullOrEmpty(pRecipientUserId))
            {
                res.ErrorText ="Both user and dl recipientIds passed as blank - a handler template requires a recipient";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            if (string.IsNullOrEmpty(pRecipientUserId))
            {
                pPropList.Add("RecipientDistributionListObjectId",pRecipientDistributionListId);
            }
            else
            {
                pPropList.Add("RecipientSubscriberObjectId", pRecipientUserId);
            }

            pPropList.Add("DisplayName", pDisplayName);
            pPropList.Add("MediaSwitchObjectId", pMediaSwitchObjectId);

            string strBody = "<CallhandlerTemplate>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</CallhandlerTemplate>";
            
            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "callhandlertemplates", 
                MethodType.POST, pConnectionServer, strBody, false);

            //fetch the objectId of the newly created object off the return
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/callhandlertemplates/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/callhandlertemplates/", "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Create a new call handler template in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pTemplateObjectId">
        /// Unique identifier for template to base new template off of
        /// </param>
        /// <param name="pDisplayName">
        /// Display Name of the new call handler template - must be unique.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a property name and a new value for that property to apply to the template being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <param name="pCallHandlerTemplate">
        /// Newly created call handler template instance is passed back in this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddCallHandlerTemplate(ConnectionServer pConnectionServer,
                                                        string pDisplayName,
                                                        string pMediaSwitchObjectId, 
                                                        string pRecipientDistributionListId, 
                                                        string pRecipientUserId,
                                                        ConnectionPropertyList pPropList,
                                                        out CallHandlerTemplate pCallHandlerTemplate)
        {
            pCallHandlerTemplate = null;

            WebCallResult res = AddCallHandlerTemplate(pConnectionServer, pDisplayName,pMediaSwitchObjectId,pRecipientDistributionListId,
                pRecipientUserId, pPropList);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                //fetc the instance of the directory handler just created.
                try
                {
                    pCallHandlerTemplate = new CallHandlerTemplate(pConnectionServer, res.ReturnedObjectId);
                }
                catch (Exception)
                {
                    res.Success = false;
                    res.ErrorText = "Could not find newly created handler template by objectId:" + res;
                }
            }

            return res;
        }


        /// <summary>
        /// Allows one or more properties on a template to be udpated (for instance display name etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handler template is homed.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the handler template to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a handler property name and a new value for that property to apply to the object being updated.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one property
        /// pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateCallHandlerTemplate(ConnectionServer pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateCallHandlerTemplate";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateCallHandlerTemplate";
                return res;
            }

            string strBody = "<CallHandlerTemplate>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</CallHandlerTemplate>";

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "callhandlertemplates/" + pObjectId,
                                            MethodType.PUT, pConnectionServer, strBody, false);

        }


        /// <summary>
        /// DELETE a handler template from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the template is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the handler template in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteCallHandlerTemplate(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteCallHandlerTemplate";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "callhandlertemplates/" + pObjectId,
                                            MethodType.DELETE, pConnectionServer, "");
        }


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
        /// Fills the current instance with details of a call handler template fetched using the ObjectID or the name.
        /// </summary>
        /// <param name="pObjectId">
        ///     ObjectId to search for - can be empty if name provided.
        /// </param>
        /// <param name="pDisplayName">
        ///     display name to search for.
        /// </param>
        /// <returns>
        /// Instance of the webCallSearchResult class.
        /// </returns>
        private WebCallResult GetCallHandlerTemplate(string pObjectId, string pDisplayName)

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
                            ErrorText = "Empty ObjectId passed to GetHandlerTemplate"
                        };
                }
            }

            string strUrl = string.Format("{0}callhandlertemplates/{1}", HomeServer.BaseUrl, strObjectId);

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

            this.ClearPendingChanges();
            return res;
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
            string strUrl = string.Format("{0}callhandlertemplates/?query=(DisplayName is {1})", HomeServer.BaseUrl, pName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false || res.TotalObjectCount ==0)
            {
                return "";
            }

            List<CallHandlerTemplate> oTemplates = HTTPFunctions.GetObjectsFromJson<CallHandlerTemplate>(res.ResponseText);

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
        /// If the object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }


        /// <summary>
        /// Allows one or more properties on a template to be udpated (for instance display name etc...).  
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
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for handler template {0}", this);
                return res;
            }

            res= UpdateCallHandlerTemplate(HomeServer, ObjectId, _changedPropList);
            if (res.Success)
            {
                ClearPendingChanges();
            }
            return res;
        }

        /// <summary>
        /// DELETE a handler template from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            return DeleteCallHandlerTemplate(HomeServer, ObjectId);
        }

        #endregion

    }
}
