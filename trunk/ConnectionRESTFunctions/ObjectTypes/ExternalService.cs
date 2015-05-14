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

    public enum AuthenticationMode { None, Basic, Digest, NTLM}

    public enum SecurityTransportType { None, SSL}

    public enum LdapSecurityTransportType { None, SSL }

    public enum ServerType { MeetingPlace20, MeetingPlace70, Exchange=4, HostedExchange=5}

    public enum ExchServerType { Exchange2003, Exchange2007}

    public enum MailboxSyncFaxAction {  None, Deliver, Relay, AcceptRelay}

    public enum MailboxSyncEmailAction { None, Deliver, Relay, AcceptRelay }

    /// <summary>
    /// The External Service class provides methods for fetching, creating, updtaing and deleting external service accounts associated with external services for
    /// Exchange and Meeting Place interfaces
    /// </summary>
    [Serializable]
    public class ExternalService :IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// Generic constructor for JSON parsing library
        /// </summary>
        public ExternalService()
        {
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Constructor for the ExternalService class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        /// <param name="pObjectId">
        /// Optional - if passed in the specifics of the switch identified by this GUID is fetched and the properties are filled in.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name to search for a service by
        /// </param>
        public ExternalService(ConnectionServerRest pConnectionServer, string pObjectId = "", string pDisplayName = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to ExternalService construtor");
            }

            HomeServer = pConnectionServer;

            //if no objectId is passed in just create an empty version of the class - used for constructing lists from data fetches.
            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetExternalService(pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res, string.Format("External service not found in ExternalService constructor using ObjectId={0} " +
                                                  "or displayName={1}\n\r{2}", pObjectId, pDisplayName, res.ErrorText));
            }
        }

        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DisplayName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        //reference to the ConnectionServer object used to create this object instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        //for checking on pending changes
        public ConnectionPropertyList ChangeList { get { return _changedPropList; } }

        #endregion


        #region ExternalService Properties

        private AuthenticationMode _authenticationMode;
        public AuthenticationMode AuthenticationMode 
        {
            get { return _authenticationMode; } 
            set
            {
                _changedPropList.Add("AuthenticationMode",(int)value);
                _authenticationMode = value;
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

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _changedPropList.Add("IsEnabled", value);
                _isEnabled = value;
            }
        }


        private string _server;
        public string Server
        {
            get { return _server; }
            set
            {
                _changedPropList.Add("Server", value);
                _server = value;
            }
        }

        private string _serviceAlias;
        public string ServiceAlias
        {
            get { return _serviceAlias; }
            set
            {
                _changedPropList.Add("ServiceAlias", value);
                _serviceAlias = value;
            }
        }

        //[JsonProperty]
        //public string ServicePassword { get; private set; }

        private string _servicePassword;
        public string ServicePassword
        {
            get { return _servicePassword; }
            set
            {
                _changedPropList.Add("ServicePassword", value);
                _servicePassword = value;
            }
        }

        private SecurityTransportType _securityTransportType;
        public SecurityTransportType SecurityTransportType
        {
            get { return _securityTransportType; }
            set
            {
                _changedPropList.Add("SecurityTransportType", (int)value);
                _securityTransportType = value;
            }
        }

        private LdapSecurityTransportType _ldapSecurityTransportType;
        public LdapSecurityTransportType LdapSecurityTransportType
        {
            get { return _ldapSecurityTransportType; }
            set
            {
                _changedPropList.Add("LdapSecurityTransportType", (int)value);
                _ldapSecurityTransportType = value;
            }
        }

        private ServerType _serverType;
        public ServerType ServerType
        {
            get { return _serverType; }
            set
            {
                _changedPropList.Add("ServerType", (int)value);
                _serverType = value;
            }
        }


        private bool _supportsCalendarCapability;
        public bool SupportsCalendarCapability
        {
            get { return _supportsCalendarCapability; }
            set
            {
                _changedPropList.Add("SupportsCalendarCapability", value);
                _supportsCalendarCapability = value;
            }
        }

        private bool _supportsMeetingCapability;
        public bool SupportsMeetingCapability
        {
            get { return _supportsMeetingCapability; }
            set
            {
                _changedPropList.Add("SupportsMeetingCapability", value);
                _supportsMeetingCapability = value;
            }
        }

        private bool _supportsTtsOfEmailCapability;
        public bool SupportsTtsOfEmailCapability
        {
            get { return _supportsTtsOfEmailCapability; }
            set
            {
                _changedPropList.Add("SupportsTtsOfEmailCapability", value);
                _supportsTtsOfEmailCapability = value;
            }
        }

        private bool _validateServerCertificate;
        public bool ValidateServerCertificate
        {
            get { return _validateServerCertificate; }
            set
            {
                _changedPropList.Add("ValidateServerCertificate", value);
                _validateServerCertificate = value;
            }
        }

        private bool _ldapValidateServerCertificate;
        public bool LdapValidateServerCertificate
        {
            get { return _ldapValidateServerCertificate; }
            set
            {
                _changedPropList.Add("LdapValidateServerCertificate", value);
                _ldapValidateServerCertificate = value;
            }
        }

        private bool _useServiceCredentials;
        public bool UseServiceCredentials
        {
            get { return _useServiceCredentials; }
            set
            {
                _changedPropList.Add("UseServiceCredentials", value);
                _useServiceCredentials = value;
            }
        }

        private bool _supportsMailboxSynchCapability;
        public bool SupportsMailboxSynchCapability
        {
            get { return _supportsMailboxSynchCapability; }
            set
            {
                _changedPropList.Add("SupportsMailboxSynchCapability", value);
                _supportsMailboxSynchCapability = value;
            }
        }

        private bool _exchDoAutodiscover;
        public bool ExchDoAutodiscover
        {
            get { return _exchDoAutodiscover; }
            set
            {
                _changedPropList.Add("ExchDoAutodiscover", value);
                _exchDoAutodiscover = value;
            }
        }

        private bool _exchDoAutodiscover2003;
        public bool ExchDoAutodiscover2003
        {
            get { return _exchDoAutodiscover2003; }
            set
            {
                _changedPropList.Add("ExchDoAutodiscover2003", value);
                _exchDoAutodiscover2003 = value;
            }
        }

        private ExchServerType _exchServerType;
        public ExchServerType ExchServerType
        {
            get { return _exchServerType; }
            set
            {
                _changedPropList.Add("ExchServerType", (int)value);
                _exchServerType = value;
            }
        }

        private string _exchOrgDomain;
        public string ExchOrgDomain
        {
            get { return _exchOrgDomain; }
            set
            {
                _changedPropList.Add("ExchOrgDomain", value);
                _exchOrgDomain = value;
            }
        }

        private string _exchSite;
        public string ExchSite
        {
            get { return _exchSite; }
            set
            {
                _changedPropList.Add("ExchSite", value);
                _exchSite = value;
            }
        }

        private MailboxSyncFaxAction _mailboxSyncFaxAction;
        public MailboxSyncFaxAction MailboxSyncFaxAction
        {
            get { return _mailboxSyncFaxAction; }
            set
            {
                _changedPropList.Add("MailboxSyncFaxAction", (int)value);
                _mailboxSyncFaxAction = value;
            }
        }

        private MailboxSyncEmailAction _mailboxSyncEmailAction;
        public MailboxSyncEmailAction MailboxSyncEmailAction
        {
            get { return _mailboxSyncEmailAction; }
            set
            {
                _changedPropList.Add("MailboxSyncEmailAction", (int)value);
                _mailboxSyncEmailAction = value;
            }
        }

        private string _transferExtensionDialString;
        public string TransferExtensionDialString
        {
            get { return _transferExtensionDialString; }
            set
            {
                _changedPropList.Add("TransferExtensionDialString", value);
                _transferExtensionDialString = value;
            }
        }

        private string _proxyServer;
        public string ProxyServer
        {
            get { return _proxyServer; }
            set
            {
                _changedPropList.Add("ProxyServer", value);
                _proxyServer = value;
            }
        }

        [JsonProperty]
        public string ObjectId { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the external service
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
        /// string containing all the name value pairs defined in the ExternalService object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            StringBuilder strBuilder = new StringBuilder();

            PropertyInfo[] oProps = GetType().GetProperties();

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
        public WebCallResult RefetchExternalServiceData()
        {
            return GetExternalService(ObjectId, "");
        }

        /// <summary>
        /// Fetch details for a single external service by ObjectId/name and populate the local instance's properties with it
        /// </summary>
        /// <param name="pObjectId">
        /// Unique identifier for service to fetch
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name to search for a service system for
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        private WebCallResult GetExternalService(string pObjectId, string pDisplayName)
        {
            string strObjectId;

            if (!string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = pObjectId;
            }
            else if (!string.IsNullOrEmpty(pDisplayName))
            {
                //fetch the ObjectId for the name if possible
                strObjectId = GetObjectIdByServiceName(pDisplayName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "No external service found for display name passed into GetExternalService:" + pDisplayName
                    };
                }
            }
            else
            {
                return new WebCallResult
                {
                    Success = false,
                    ErrorText = "No value for ObjectId or display name passed to GetObjectIdByServiceName."
                };
            }

            string strUrl = string.Format("{0}externalservices/{1}", HomeServer.BaseUrl, strObjectId);

            var res= HomeServer.FillObjectWithRestGetResults(strUrl,this);        
            ClearPendingChanges();
            return res;
        }


        /// <summary>
        /// Fetch an external service ObjectId by it's name - 
        /// </summary>
        /// <param name="pServiceName">
        /// Display name of the service to search for
        /// </param>
        /// <returns>
        /// ObjectId of the external service with the name if found, or blank string if not.
        /// </returns>
        private string GetObjectIdByServiceName(string pServiceName)
        {

            string strUrl = string.Format("{0}externalservices/?query=(DisplayName is {1})", HomeServer.BaseUrl, pServiceName.UriSafe());

            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false || res.TotalObjectCount==0)
            {
                return "";
            }

            List<ExternalService> oServices = HomeServer.GetObjectsFromJson<ExternalService>(res.ResponseText);

            foreach (var oService in oServices)
            {
                if (oService.DisplayName.Equals(pServiceName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oService.ObjectId;
                }
            }

            return "";
        }


        /// <summary>
        /// Allows one or more properties on a service to be udpated.  
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update(bool pRefetchDataAfterSuccessfulUpdate = false)
        {
            WebCallResult res;

            if (!_changedPropList.Any())
            {
                res = new WebCallResult
                {
                    Success = false,
                    ErrorText =string.Format("Update called but there are no pending changes for ExternalService:{0}, objectid=[{1}]",this, ObjectId)
                };
                return res;
            }

            res = UpdateExternalService(HomeServer, ObjectId, _changedPropList);

            if (res.Success)
            {
                _changedPropList.Clear();
                if (pRefetchDataAfterSuccessfulUpdate)
                {
                    return RefetchExternalServiceData();
                }
            }

            return res;
        }

        /// <summary>
        /// DELETE an external service from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            return DeleteExternalService(HomeServer, ObjectId);
        }

        /// <summary>
        /// If the external service has any pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all external services and resturns them as a generic list of ExternalService objects.  This
        /// list can be used for providing drop down list selection for user creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the external service should be pulled from
        /// </param>
        /// <param name="pExternalServices">
        /// Out parameter that is used to return the list of ExternalService objects defined on Connection - there must be at least one.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>        
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter 
        /// at a time  are currently supported by CUPI - in other words you can't have "query=(displayname startswith ab)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>        
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetExternalServices(ConnectionServerRest pConnectionServer, out List<ExternalService> pExternalServices,
            int pPageNumber = 1, int pRowsPerPage = 20, params string[] pClauses)
        {
            WebCallResult res;
            pExternalServices = new List<ExternalService>();

            if (pConnectionServer==null)
            {
                res = new WebCallResult {ErrorText = "Null ConnectionServer referenced passed to GetExternalServices"};
                return res;
            }

            List<String> oParams;
            if (pClauses == null)
            {
                oParams=new List<string>();
            }
            else
            {
                oParams = pClauses.ToList();
            }

            oParams.Add("pageNumber=" + pPageNumber);
            oParams.Add("rowsPerPage=" + pRowsPerPage);

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "externalservices", oParams.ToArray());

            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                res.ErrorText = "Empty response received";
                return res;
            }

            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                return res;
            }

            pExternalServices = pConnectionServer.GetObjectsFromJson<ExternalService>(res.ResponseText);

            if (pExternalServices == null)
            {
                pExternalServices = new List<ExternalService>();
                res.ErrorText = "Could not parse JSON into ExternalService:" + res.ResponseText;
                res.Success = false;
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pExternalServices)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// returns a single ExternalService object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the service is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the service to load
        /// </param>
        /// <param name="pExternalService">
        /// The out param that the filled out instance of the ExternalService class is returned on.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name 
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetExternalService(out ExternalService pExternalService, ConnectionServerRest pConnectionServer, string pObjectId, 
            string pDisplayName="")
        {
            WebCallResult res = new WebCallResult {Success = false};

            pExternalService = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetExternalService";
                return res;
            }

            if ( string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty objectId and DisplayName passed to GetExternalService";
                return res;
            }

            //create a new ExternalService instance passing the ObjectId (or display name) which fills out the data automatically
            try
            {
                pExternalService = new ExternalService(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch external service in GetExternalService:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Allows one or more properties on an external service to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the service is homed.
        /// </param>
        /// <param name="pObjectId">
        /// Unique identifier for the external service to update.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a property name and a new value for that property to apply to the object
        /// being updated. 
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateExternalService(ConnectionServerRest pConnectionServer,string pObjectId,ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateExternalService";
                return res;
            }

            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateExternalService";
                return res;
            }

            string strBody = "<ExternalService>";

            foreach (var oPair in pPropList)
            {
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</ExternalService>";

            return pConnectionServer.GetCupiResponse(string.Format("{0}externalservices/{1}", pConnectionServer.BaseUrl, pObjectId), 
                MethodType.PUT, strBody, false);

        }

        /// <summary>
        /// Adds a new Exchange service account  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the service to
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the service to add.  Display name should be unique among external services.
        /// </param>
        /// <param name="pServiceAlias"></param>
        /// <param name="pServicePassword"></param>
        /// <param name="pServerName"></param>
        /// <param name="pAuthenticationMode"></param>
        /// <param name="pExchangeServerType"></param>
        /// <param name="pSecurityTransportType"></param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddExchangeExternalService(ConnectionServerRest pConnectionServer,string pDisplayName, string pServiceAlias,
            string pServicePassword, string pServerName, AuthenticationMode pAuthenticationMode, ExchServerType pExchangeServerType, 
            SecurityTransportType pSecurityTransportType)
        {
            WebCallResult res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddExchangeExternalService";
                return res;
            }

            //make sure that something is passed in for the required param
            if (String.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for display name in AddExchangeExternalService";
                return res;
            }

            string strBody = "<ExternalService>";
            strBody += string.Format("<{0}>{1}</{0}>", "DisplayName", pDisplayName.HtmlBodySafe());
            strBody += string.Format("<{0}>{1}</{0}>", "ServiceAlias", pServiceAlias.HtmlBodySafe());
            strBody += string.Format("<{0}>{1}</{0}>", "ServicePassword", pServicePassword.HtmlBodySafe());
            strBody += string.Format("<{0}>{1}</{0}>", "ServerType", (int)ServerType.Exchange);
            strBody += string.Format("<{0}>{1}</{0}>", "Server", pServerName);
            strBody += string.Format("<{0}>{1}</{0}>", "AuthenticationMode", (int)pAuthenticationMode);
            strBody += string.Format("<{0}>{1}</{0}>", "ExchServerType", (int) pExchangeServerType);
            strBody += string.Format("<{0}>{1}</{0}>", "SecurityTransportType",(int) pSecurityTransportType);
            strBody += "</ExternalService>";

            res = pConnectionServer.GetCupiResponse(string.Format("{0}externalservices", pConnectionServer.BaseUrl),
                    MethodType.POST, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                const string strPrefix = @"/vmrest/externalservices/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Adds a new Exchange service account  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the service to
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the service to add.  Display name should be unique among external services.
        /// </param>
        /// <param name="pServiceAlias"></param>
        /// <param name="pServicePassword"></param>
        /// <param name="pServerName"></param>
        /// <param name="pAuthenticationMode"></param>
        /// <param name="pExchangeServerType"></param>
        /// <param name="pSecurityTransportType"></param>
        /// <param name="oNewSerive"></param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddExchangeExternalService(ConnectionServerRest pConnectionServer, string pDisplayName, string pServiceAlias,
            string pServicePassword, string pServerName, AuthenticationMode pAuthenticationMode, ExchServerType pExchangeServerType,
            SecurityTransportType pSecurityTransportType, out ExternalService oNewSerive)
        {
            oNewSerive = null;
            var res = AddExchangeExternalService(pConnectionServer, pDisplayName, pServiceAlias, pServicePassword, pServerName,
                pAuthenticationMode,pExchangeServerType,pSecurityTransportType);
            if (res.Success)
            {
                return GetExternalService(out oNewSerive, pConnectionServer, res.ReturnedObjectId);
            }
            return res;
        }

        /// <summary>
        /// Adds a new Office 365 service account  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the service to
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the service to add.  Display name should be unique among external services.
        /// </param>
        /// <param name="pServiceAlias"></param>
        /// <param name="pServicePassword"></param>
        /// <param name="pServerName"></param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddOffice365ExternalService(ConnectionServerRest pConnectionServer, string pDisplayName, string pServiceAlias,
            string pServicePassword, string pServerName)
        {
            WebCallResult res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddExchangeExternalService";
                return res;
            }

            //make sure that something is passed in for the required param
            if (String.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for display name in AddExchangeExternalService";
                return res;
            }

            string strBody = "<ExternalService>";
            strBody += string.Format("<{0}>{1}</{0}>", "DisplayName", pDisplayName.HtmlBodySafe());
            strBody += string.Format("<{0}>{1}</{0}>", "ServiceAlias", pServiceAlias.HtmlBodySafe());
            strBody += string.Format("<{0}>{1}</{0}>", "ServicePassword", pServicePassword.HtmlBodySafe());
            strBody += string.Format("<{0}>{1}</{0}>", "ServerType", (int)ServerType.HostedExchange);
            strBody += string.Format("<{0}>{1}</{0}>", "Server", pServerName);
            strBody += "</ExternalService>";

            res = pConnectionServer.GetCupiResponse(string.Format("{0}externalservices", pConnectionServer.BaseUrl),
                    MethodType.POST, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                const string strPrefix = @"/vmrest/externalservices/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Adds a new Office 365 service account  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the service to
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the service to add.  Display name should be unique among external services.
        /// </param>
        /// <param name="pServiceAlias"></param>
        /// <param name="pServicePassword"></param>
        /// <param name="pServerName"></param>
        /// <param name="oNewSerive"></param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddOffice365ExternalService(ConnectionServerRest pConnectionServer,
            string pDisplayName, string pServiceAlias,string pServicePassword, string pServerName, out ExternalService oNewSerive)
        {
            oNewSerive = null;
            var res = AddOffice365ExternalService(pConnectionServer, pDisplayName, pServiceAlias, pServicePassword,pServerName);
            if (res.Success)
            {
                return GetExternalService(out oNewSerive, pConnectionServer, res.ReturnedObjectId);
            }
            return res;
        }

        /// <summary>
        /// Adds a new Meeting Place service account  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the service to
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the service to add.  Display name should be unique among external services.
        /// </param>
        /// <param name="pServiceAlias"></param>
        /// <param name="pServicePassword"></param>
        /// <param name="pServerName"></param>
        /// <param name="pTransferExtensionDialString"></param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddMeetingPlaceExternalService(ConnectionServerRest pConnectionServer, string pDisplayName, string pServiceAlias,
            string pServicePassword, string pServerName, string pTransferExtensionDialString)
        {
            WebCallResult res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddExchangeExternalService";
                return res;
            }

            //make sure that something is passed in for the required param
            if (String.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for display name in AddExchangeExternalService";
                return res;
            }

            string strBody = "<ExternalService>";
            strBody += string.Format("<{0}>{1}</{0}>", "DisplayName", pDisplayName.HtmlBodySafe());
            strBody += string.Format("<{0}>{1}</{0}>", "ServiceAlias", pServiceAlias.HtmlBodySafe());
            strBody += string.Format("<{0}>{1}</{0}>", "ServicePassword", pServicePassword.HtmlBodySafe());
            strBody += string.Format("<{0}>{1}</{0}>", "ServerType", (int)ServerType.MeetingPlace70);
            strBody += string.Format("<{0}>{1}</{0}>", "Server", pServerName);
            strBody += string.Format("<{0}>{1}</{0}>", "TransferExtensionDialString", pTransferExtensionDialString);
            strBody += "</ExternalService>";

            res = pConnectionServer.GetCupiResponse(string.Format("{0}externalservices", pConnectionServer.BaseUrl),
                    MethodType.POST, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                const string strPrefix = @"/vmrest/externalservices/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Adds a new Meeting Place service account  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the service to
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the service to add.  Display name should be unique among external services.
        /// </param>
        /// <param name="pServiceAlias"></param>
        /// <param name="pServicePassword"></param>
        /// <param name="pServerName"></param>
        /// <param name="pTransferExtensionDialString"></param>
        /// <param name="oNewSerive"></param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddMeetingPlaceExternalService(ConnectionServerRest pConnectionServer,
            string pDisplayName, string pServiceAlias,
            string pServicePassword, string pServerName, string pTransferExtensionDialString, out ExternalService oNewSerive)
        {
            oNewSerive = null;
            var res = AddMeetingPlaceExternalService(pConnectionServer, pDisplayName, pServiceAlias, pServicePassword,
                pServerName, pTransferExtensionDialString);
            if (res.Success)
            {
                return GetExternalService(out oNewSerive, pConnectionServer, res.ReturnedObjectId);
            }
            return res;
        }


        /// <summary>
        /// DELETE an external service from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the service is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the service in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteExternalService(ConnectionServerRest pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult
                    {
                        ErrorText = "Null ConnectionServer referenced passed to DeleteExternalService"
                    };
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                return new WebCallResult
                {
                    ErrorText = "Blank ObjectId passed to DeleteExternalService"
                };
            }

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "externalservices/" + pObjectId,MethodType.DELETE, "");
        }

        #endregion

    }
}
