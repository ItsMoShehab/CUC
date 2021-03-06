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
    /// <summary>
    /// Class that contains methods for finding, fetching, creating and deleting tenant objects in Connection 10.0 
    /// and later versions of Unity Connection.  Also includes helper methods for fetching sub object collections such 
    /// as all handlers associated with a tenant and setting additional COS and schedule objects to an existing 
    /// tenant.
    /// </summary>
    [Serializable]
    public class Tenant : IUnityDisplayInterface
    {

        #region Constructors and Destructors

        /// <summary>
        /// Creates a new instance of the Tenant class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this tenant.  
        /// If you pass the pObjectID or pAlias parameter the tenant is automatically filled with data for that tenant from the server.  
        /// If neither are passed an empty instance of the Tenant class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the tenant being created.
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the tenant on the home server provided.  If no ObjectId is passed then an empty instance 
        /// of the Tenant class is returned instead.
        /// </param>
        /// <param name="pAlias">
        /// Optional alias to search on - if both ObjectId and alais are passed, ObjectId is used.  The alias search is not case sensitive.
        /// </param>
        public Tenant(ConnectionServerRest pConnectionServer, string pObjectId = "", string pAlias = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to Tenant construtor");
            }

            //keep track of the home Connection server this tenant is created on.
            HomeServer = pConnectionServer;

            _changedPropList = new ConnectionPropertyList();

            //if the user passed in a specific ObjectId or alias then go load that tenant up, otherwise just return an empty instance.
            if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pAlias))) return;

            //if the ObjectId or alias are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetTenant(pObjectId, pAlias);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,
                    string.Format("Tenant not found in constructor using ObjectId={0} and Alias={1}\n\r{2}", pObjectId, pAlias, res.ErrorText));
            }
        }

        /// <summary>
        /// Generic constructor for Json parsing libraries
        /// </summary>
        public Tenant()
        {
        }

        #endregion


        #region Fields and Properties

        public ConnectionServerRest HomeServer { get; private set; }
        
        public string SelectionDisplayString { get { return Description; }}
        
        public string UniqueIdentifier { get { return ObjectId; } }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        /// <summary>
        /// for checking on pending changes
        /// </summary>
        public ConnectionPropertyList ChangeList { get { return _changedPropList; } }


        #endregion


        #region Tenant Properties

        [JsonProperty]
        public string Alias { get; private set; }

        [JsonProperty]
        public string AttributeObjectId { get; private set; }

        [JsonProperty]
        public TenantAttributeType AttributeType { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }

        private string _description;
        [JsonProperty]
        public string Description 
        {
            get { return _description; } 
            set
            {
                _changedPropList.Add("Description", value);
                _description = value;
            } 
        }

        [JsonProperty]
        public int Language { get; private set; }

        [JsonProperty]
        public string MailboxStoreObjectId { get; private set; }

        [JsonProperty]
        public string MediaSwitchObjectId { get; private set; }

        private string _name;
        [JsonProperty]
        public string Name
        {
            get { return _name; }
            set
            {
                _changedPropList.Add("Name", value);
                _name = value;
            }
        }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string PartitionObjectId { get; private set; }

        [JsonProperty]
        public string PhoneSystemObjectId { get; private set; }

        private string _pilotNumber;
        [JsonProperty]
        public string PilotNumber
        {
            get { return _pilotNumber; }
            set
            {
                _changedPropList.Add("PilotNumber", value);
                _pilotNumber = value;
            }
        }

        private string _smtpDomain;
        [JsonProperty]
        public string SmtpDomain
        {
            get { return _smtpDomain; }
            set
            {
                _changedPropList.Add("SmtpDomain", value);
                _smtpDomain = value;
            }
        }

        [JsonProperty]
        public string TimeZone { get; private set; }

        #endregion


        #region Static Methods

        /// <summary>
        /// This method allows for a GET of tenants from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(alias startswith ab)"
        /// sort: "sort=(alias asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the tenants are being fetched from.
        /// </param>
        /// <param name="pTenants">
        /// The list of tenants returned from the CUPI call (if any) is returned as a generic list of Tenant class instances via this out param.  
        /// If no tenants are found an empty list is returned.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetTenants(ConnectionServerRest pConnectionServer, out List<Tenant> pTenants,params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pTenants = new List<Tenant>();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetTenants";
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "tenants", pClauses);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.ErrorText = "Empty response received";
                res.Success = false;
                return res;
            }

            //not an error, just return empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                return res;
            }

            pTenants = pConnectionServer.GetObjectsFromJson<Tenant>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pTenants)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// This method allows for a GET of tenants from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(alias startswith ab)"
        /// sort: "sort=(alias asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the tenants are being fetched from.
        /// </param>
        /// <param name="pTenants">
        /// The list of tenants returned from the CUPI call (if any) is returned as a generic list of Tenant class instances via this out param.  
        /// If no tenants are found an empty list is returned.
        /// </param>
        /// <param name="pRowsPerPage">
        /// How many rows to return with the fetch, defaults to 20
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetTenants(ConnectionServerRest pConnectionServer, out List<Tenant> pTenants,
            int pPageNumber = 1, int pRowsPerPage = 20, params string[] pClauses)
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

            return GetTenants(pConnectionServer, out pTenants, temp.ToArray());
        }


        /// <summary>
        /// returns a single Tenant object from an ObjectId or alias string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the tenant is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the tenant to load
        /// </param>
        /// <param name="pTenant">
        /// The out param that the filled out instance of the tenant class is returned on.
        /// </param>
        /// <param name="pAlias">
        /// Optional alias to search for an  on.  If both the ObjectId and alias are passed, the ObjectId is used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetTenant(out Tenant pTenant, ConnectionServerRest pConnectionServer, string pObjectId = "", string pAlias = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pTenant = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetTenant";
                return res;
            }

            //you need an ObjectId and/or a display name - both being blank is not acceptable
            if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pAlias)))
            {
                res.ErrorText = "Empty ObjectId and alias passed to GetTenant";
                return res;
            }

            try
            {
                pTenant = new Tenant(pConnectionServer, pObjectId, pAlias);
                pTenant.ClearPendingChanges();
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch tenant in GetTenant:" + ex.Message;
                res.Success = false;
            }

            return res;
        }


        /// <summary>
        /// Create a new tenant
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to add the tenant to
        /// </param>
        /// <param name="pAlias">
        /// alias of the new tenant
        /// </param>
        /// <param name="pSmtpDomain">
        /// SMTP Domain of the new tenant
        /// </param>
        /// <param name="pDescription">
        /// Description of new tenant
        /// </param>
        /// <param name="pPilotNumber">
        /// if passed as blank string a seperate phone sytem will be created for the tenant, otherwise it will "share" the 
        /// same base phone system as all other tenants and route based on pilot number matching - only for 10.5.2 and later.
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// When a pilot number is provided, a corresponding media switch ObjectId string must also be provided - this indicates
        /// which switch is being "shared" with the pilot number - currently it's an "either or" setup, you cannot mix trunk
        /// and pilot based routing between tenants on a single installation.
        /// </param>
        /// <param name="pObjectId">
        /// UUID will be used for the tenant if passed in and unique - if not passed in (default) Connection will create it's own
        /// UUID for the new tenant.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddTenant(ConnectionServerRest pConnectionServer,
                                                    string pAlias,
                                                    string pSmtpDomain,
                                                    string pDescription,
                                                    string pPilotNumber,
                                                    string pMediaSwitchObjectId,
                                                    string pObjectId="")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddTenant";
                return res;
            }

            if (String.IsNullOrEmpty(pAlias) | string.IsNullOrEmpty(pSmtpDomain))
            {
                res.ErrorText = "Empty value passed for alias or SMTPDomain in AddTenant";
                return res;
            }

            if (!string.IsNullOrEmpty(pPilotNumber) & string.IsNullOrEmpty(pMediaSwitchObjectId))
            {
                res.ErrorText ="Empty media switch ObjectId value passed with pilot number - both must be provided for pilot routing.";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            var oPropList = new ConnectionPropertyList();

            oPropList.Add("Alias", pAlias);
            oPropList.Add("SmtpDomain", pSmtpDomain);
            oPropList.Add("Description", pDescription);
            if (!string.IsNullOrEmpty(pPilotNumber))
            {
                oPropList.Add("PilotNumber",pPilotNumber);
                oPropList.Add("MediaSwitchObjectId",pMediaSwitchObjectId);
            }
            if (!string.IsNullOrEmpty(pObjectId))
            {
                oPropList.Add("ObjectId",pObjectId);
            }

            string strBody = "<Tenant>";

            foreach (var oPair in oPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Tenant>";

            res = pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "tenants", MethodType.POST, strBody, false);

            //fetch the objectId of the newly created object off the return
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/tenants/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/tenants/", "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Create a new tenant
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to add the tenant to
        /// </param>
        /// <param name="pAlias">
        /// alias of the new tenant
        /// </param>
        /// <param name="pSmtpDomain">
        /// SMTP Domain of the new tenant
        /// </param>
        /// <param name="pDescription">
        /// Description of new tenant
        /// </param>
        /// <param name="pPilotNumber">
        /// if passed as blank string a seperate phone sytem will be created for the tenant, otherwise it will "share" the 
        /// same base phone system as all other tenants and route based on pilot number matching - only for 10.5.2 and later.
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// When a pilot number is provided, a corresponding media switch ObjectId string must also be provided - this indicates
        /// which switch is being "shared" with the pilot number - currently it's an "either or" setup, you cannot mix trunk
        /// and pilot based routing between tenants on a single installation.
        /// </param>
        /// <param name="pTenant">
        /// Instance of the newly created tenant is passed back on this out param
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddTenant(ConnectionServerRest pConnectionServer,
                                              string pAlias,
                                              string pSmtpDomain,
                                              string pDescription,
                                              string pPilotNumber,
                                              string pMediaSwitchObjectId,
                                              out Tenant pTenant)
        {
            pTenant = null;
            var res = AddTenant(pConnectionServer, pAlias, pSmtpDomain, pDescription,pPilotNumber,pMediaSwitchObjectId);

            if (res.Success)
            {
                //fetc the instance of the tenant just created.
                try
                {
                    pTenant = new Tenant(pConnectionServer, res.ReturnedObjectId);
                }
                catch (Exception)
                {
                    res.Success = false;
                    res.ErrorText = "Could not find newly created tenant by objectId:" + res;
                }
            }

            return res;
        }

        /// <summary>
        /// Create a new tenant
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to add the tenant to
        /// </param>
        /// <param name="pAlias">
        /// alias of the new tenant
        /// </param>
        /// <param name="pSmtpDomain">
        /// SMTP Domain of the new tenant
        /// </param>
        /// <param name="pDescription">
        /// Description of new tenant
        /// </param>
        /// <param name="pPilotNumber">
        /// if passed as blank string a seperate phone sytem will be created for the tenant, otherwise it will "share" the 
        /// same base phone system as all other tenants and route based on pilot number matching - only for 10.5.2 and later.
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// When a pilot number is provided, a corresponding media switch ObjectId string must also be provided - this indicates
        /// which switch is being "shared" with the pilot number - currently it's an "either or" setup, you cannot mix trunk
        /// and pilot based routing between tenants on a single installation.
        /// </param>
        /// <param name="pObjectId">
        /// UUID will be used for the tenant if passed in and unique - if not passed in (default) Connection will create it's own
        /// UUID for the new tenant.
        /// </param>        /// <param name="pTenant">
        /// Instance of the newly created tenant is passed back on this out param
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddTenant(ConnectionServerRest pConnectionServer,
                                              string pAlias,
                                              string pSmtpDomain,
                                              string pDescription,
                                              string pPilotNumber,
                                              string pMediaSwitchObjectId,
                                              string pObjectId,
                                              out Tenant pTenant)
        {
            pTenant = null;
            var res = AddTenant(pConnectionServer, pAlias, pSmtpDomain, pDescription, pPilotNumber, pMediaSwitchObjectId, pObjectId);

            if (res.Success)
            {
                try
                {
                    pTenant = new Tenant(pConnectionServer, res.ReturnedObjectId);
                }
                catch (Exception)
                {
                    res.Success = false;
                    res.ErrorText = "Could not find newly created tenant by objectId:" + res;
                }
            }

            return res;
        }

        /// <summary>
        /// DELETE a tenant from the Connection directory.
        /// WARNING! This removes all objects related to the tenant!
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the tenant is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the tenant in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteTenant(ConnectionServerRest pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult
                {
                    ErrorText = "Null ConnectionServer referenced passed to DeleteTenant"
                };
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                return new WebCallResult
                {
                    ErrorText = "Empty pObjectId passed to DeleteTenant"
                };
            }
            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "tenants/" + pObjectId,
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
        public static WebCallResult UpdateTenant(ConnectionServerRest pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateTenant";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateTenant";
                return res;
            }

            string strBody = "<Tenant>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Tenant>";

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "tenants/" + pObjectId,MethodType.PUT, strBody, false);

        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays alias and description of the tenant
        /// </summary>
        public override string ToString()
        {
            return String.Format("Tenant:{0}: {1}", this.Alias, this.Description);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the tenant object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the tenant object instance.
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
        /// Gets a tenant by ObjectId or alias and fills in the current instance with its properties
        /// </summary>
        /// <param name="pObjectId">
        /// ObjectId - if passed this is used.  Must pass either ObjectId or Alias, both cannot be blank
        /// </param>
        /// <param name="pAlias">
        /// Alias of tenant - will get used if ObjectId is blank
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of request and response from server.
        /// </returns>
        private WebCallResult GetTenant(string pObjectId, string pAlias)
        {
            string strObjectId = pObjectId;
            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdFromAlias(pAlias);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "No tenant found for alias=" + pAlias
                    };
                }
            }

            string strUrl = string.Format("{0}tenants/{1}", HomeServer.BaseUrl, strObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                res.ErrorText = "Failed to find tenant by objectid=" + pObjectId + " or alias=" + pAlias;
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, RestTransportFunctions.JsonSerializerSettings);
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
        /// Fetch the ObjectId of a tenant by it's alias.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pAlias">
        /// alias of the tenant to find
        /// </param>
        /// <returns>
        /// ObjectId of tenant if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromAlias(string pAlias)
        {
            string strUrl = string.Format("{0}tenants/?query=(Alias is {1})", HomeServer.BaseUrl, pAlias.UriSafe());

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false || res.TotalObjectCount == 0)
            {
                return "";
            }

            List<Tenant> oTenants = HomeServer.GetObjectsFromJson<Tenant>(res.ResponseText);

            foreach (var oTenant in oTenants)
            {
                if (oTenant.Alias.Equals(pAlias, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oTenant.ObjectId;
                }
            }

            return "";
        }


        /// <summary>
        /// If the call handler object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            if (_changedPropList == null) return;
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
        public WebCallResult Update(bool pRefetchDataAfterSuccessfulUpdate = false)
        {
            WebCallResult res;

            //check if the handler intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for tenant {0}", this);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateTenant(HomeServer, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
                if (pRefetchDataAfterSuccessfulUpdate)
                {
                    return RefetchTenantData();
                }
            }

            return res;
        }

        /// <summary>
        /// Remove a tenant from the directory.
        /// WARNING! This removes all objects related to the tenant!
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            return DeleteTenant(HomeServer, ObjectId);
        }

        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made externaly this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchTenantData()
        {
            return GetTenant(this.ObjectId, this.Alias);
        }
        #region Related Tenant Object Methods


        #region Class of Service

        //used for fetching list of COS IDs off tenant
        private class TenantCos
        {
            public string CosUri { get; set; }
        }

        /// <summary>
        /// Returns the list of Class of Services associated with this tenant.  Since COS objects do not have a partition it's necessary to 
        /// fish this list off the Tenant interface in the API directly instead of a simple partition filter.
        /// </summary>
        /// <param name="pClassesOfService">
        /// The associated classes of service are returned on this generic list of ClassOfService objects.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <param name="pClauses">
        /// list of 0 or more query clauses that can be added to the search call such as "query=(displayname is mydisplayname)"
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetClassesOfService(out List<ClassOfService> pClassesOfService, int pPageNumber=1, int pRowsPerPage=20,
            params string[] pClauses)
        {
            pClassesOfService = new List<ClassOfService>();

           List<string> oParams;
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

            string strUrl = ConnectionServerRest.AddClausesToUri(string.Format("{0}tenants/{1}/coses", HomeServer.BaseUrl, ObjectId),oParams.ToArray());

            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //a tenant must always have at least one COS
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount==0)
            {
                res.ErrorText = "Failed to find COS for tenant by objectid=" + ObjectId;
                return res;
            }

            int iHoldTotalCount = res.TotalObjectCount;

            List<TenantCos> oCoses = HomeServer.GetObjectsFromJson<TenantCos>(res.ResponseText, "TenantCos");

            if (oCoses == null)
            {
                res.Success = false;
                res.ErrorText = "Could not parse response into COSes:" + res.ResponseText;
                return res;
            }

            foreach (var oCos in oCoses)
            {
                ClassOfService oNewCos;
                res = ClassOfService.GetClassOfService(out oNewCos, HomeServer,oCos.CosUri.Replace("/vmrest/coses/", ""));

                if (res.Success == false)
                {
                    return res;
                }
                pClassesOfService.Add(oNewCos);
            }

            res.TotalObjectCount = iHoldTotalCount;

            return res;
        }

        /// <summary>
        /// Adds an existing class of service to the list associated with a tenant.  Be aware that when you delete the tenant all
        /// objects associated with that tenant are removed, including all COS objects you "tie" to the tenant - so don't do this unless
        /// the COS is ONLY for this tenant.
        /// </summary>
        /// <param name="pCosObjectId">
        /// Existing class of service to add to the tenant definition
        /// </param>
        /// <returns>
        /// Instance of the WebCallResultClass with details of the request and response from the server.
        /// </returns>
        public WebCallResult AddClassOfServiceToTenant(string pCosObjectId)
        {
            string strBody = "<TenantCos>";

            //tack on the property value pair with appropriate tags
            strBody += string.Format("<{0}>{1}</{0}>", "CosObjectId", pCosObjectId);

            strBody += "</TenantCos>";

            string strUrl = string.Format("{0}tenants/{1}/coses", HomeServer.BaseUrl, ObjectId);
            return HomeServer.GetCupiResponse(strUrl, MethodType.POST, strBody, false);
        }

        #endregion


        #region Phone Systems

        //used for getting list of phone systems off tenant.
        private class TenantPhoneSystem
        {
            public string PhoneSystemUri { get; set; }
        }

        /// <summary>
        /// Returns the list of phone systems associated with this tenant.  Since phone systems do not have a partition it's 
        /// necessary to fish this list off the Tenant interface in the API directly instead of a simple partition filter.
        /// </summary>
        /// <param name="pPhoneSystems">
        /// The associated phone systems are returned on this generic list of PhoneSystem objects.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetPhoneSystems(out List<PhoneSystem> pPhoneSystems, int pPageNumber = 1,int pRowsPerPage = 20,
            params string[] pClauses)
        {
            pPhoneSystems = new List<PhoneSystem>();

            List<string> oParams;
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

            string strUrl = ConnectionServerRest.AddClausesToUri(string.Format("{0}tenants/{1}/phonesystems", HomeServer.BaseUrl, ObjectId), oParams.ToArray());

            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //a tenant must always have at least one phone system
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                res.Success = false;
                res.ErrorText = "Failed to find phone system for tenant by objectid=" + ObjectId;
                return res;
            }

            int iHoldTotalCount = res.TotalObjectCount;

            List<TenantPhoneSystem> pPhones = HomeServer.GetObjectsFromJson<TenantPhoneSystem>(res.ResponseText, "TenantPhoneSystem");

            foreach (var oPhone in pPhones)
            {
                PhoneSystem oNewPhoneSystem;
                res = PhoneSystem.GetPhoneSystem(out oNewPhoneSystem, HomeServer, oPhone.PhoneSystemUri.Replace("/vmrest/phonesystems/", ""));

                if (res.Success == false)
                {
                    return res;
                }
                pPhoneSystems.Add(oNewPhoneSystem);
            }

            res.TotalObjectCount = iHoldTotalCount;
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPhoneSystemObjectId"></param>
        /// <returns></returns>
        public WebCallResult AddPhoneSystemToTenant(string pPhoneSystemObjectId)
        {
            string strBody = "<TenantPhoneSystem>";

            //tack on the property value pair with appropriate tags
            strBody += string.Format("<{0}>{1}</{0}>", "PhoneSystemObjectId", pPhoneSystemObjectId);

            strBody += "</TenantPhoneSystem>";

            string strUrl = string.Format("{0}tenants/{1}/phonesystems", HomeServer.BaseUrl, ObjectId);
            return HomeServer.GetCupiResponse(strUrl, MethodType.POST, strBody, false);
        }

        #endregion


        #region Schedule Sets

        //used for fetching list of schedules off tenant
        private class TenantScheduleSet
        {
            public string ScheduleSetUri { get; set; }
        }

        /// <summary>
        /// Returns the list of schedule sets associated with this tenant.  Since schedules do not have a partition it's necessary to 
        /// fish this list off the Tenant interface in the API directly instead of a simple partition filter.
        /// </summary>
        /// <param name="pScheduleSets">
        /// The associated schedule sets are returned on this generic list of ScheduleSet objects.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <param name="pClauses">
        /// list of 0 or more query clauses that can be added to the search call such as "query=(displayname is mydisplayname)"
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetScheduleSets(out List<ScheduleSet> pScheduleSets, int pPageNumber = 1,int pRowsPerPage = 20,
            params string[] pClauses)
        {
            pScheduleSets = new List<ScheduleSet>();

            List<string> oParams;
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

            string strUrl = ConnectionServerRest.AddClausesToUri(string.Format("{0}tenants/{1}/schedulesets", HomeServer.BaseUrl, ObjectId), oParams.ToArray());

            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //a tenant must always have at least one schedule set, but a search string can still return zero entries
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                res.ErrorText = "Failed to find schedules for tenant by objectid=" + ObjectId;
                return res;
            }

            int iHoldTotalCount = res.TotalObjectCount;

            List<TenantScheduleSet> pSchedules = HomeServer.GetObjectsFromJson<TenantScheduleSet>(res.ResponseText, "TenantScheduleSet");

            if (pSchedules == null)
            {
                res.Success = false;
                res.ErrorText = "Could not parse results text into schedules:" + res.ResponseText;
                return res;
            }

            foreach (var oSchedule in pSchedules)
            {
                ScheduleSet oNewSchedule;
                res = ScheduleSet.GetScheduleSet(out oNewSchedule, HomeServer, oSchedule.ScheduleSetUri.Replace("/vmrest/schedulesets/", ""));

                if (res.Success == false)
                {
                    return res;
                }
                pScheduleSets.Add(oNewSchedule);
            }
            res.TotalObjectCount = iHoldTotalCount;
            return res;
        }

        /// <summary>
        /// Adds an existing schedule to the list associated with a tenant.  Be aware that when you delete the tenant all
        /// objects associated with that tenant are removed, including all schedule objects you "tie" to the tenant - 
        /// so don't do this unless the schedule is ONLY for this tenant.
        /// </summary>
        /// <param name="pScheduleSetObjectId">
        /// Existing schedule to add to the tenant definition
        /// </param>
        /// <returns>
        /// Instance of the WebCallResultClass with details of the request and response from the server.
        /// </returns>
        public WebCallResult AddScheduleSetToTenant(string pScheduleSetObjectId)
        {
            string strBody = "<TenantScheduleSet>";

            //tack on the property value pair with appropriate tags
            strBody += string.Format("<{0}>{1}</{0}>", "ScheduleSetObjectId", pScheduleSetObjectId);

            strBody += "</TenantScheduleSet>";

            string strUrl = string.Format("{0}tenants/{1}/schedulesets", HomeServer.BaseUrl, ObjectId);
            return HomeServer.GetCupiResponse(strUrl, MethodType.POST, strBody, false);
        }


        #endregion


        /// <summary>
        /// Fetches all user templates associated with this tenant.
        /// </summary>
        /// <param name="pTemplates">
        /// List of user templates is returned as a generic list of UserTemplate objects on this out parameter.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetUserTemplates(out List<UserTemplate> pTemplates, int pPageNumber = 1, int pRowsPerPage = 20)
        {
            string strQuery = "query=(PartitionObjectId is " + this.PartitionObjectId + ")";
            return UserTemplate.GetUserTemplates(HomeServer, out pTemplates, pPageNumber, pRowsPerPage, strQuery);
        }


        /// <summary>
        /// Fetches all call handler templates associated with this tenant.
        /// </summary>
        /// <param name="pTemplates">
        /// List of handler tempaltes is returned as a generic list of CallHandlerTemplate objects on this out parameter.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetCallHandlerTemplates(out List<CallHandlerTemplate> pTemplates, int pPageNumber = 1, int pRowsPerPage = 20)
        {
            string strQuery = "query=(PartitionObjectId is " + this.PartitionObjectId + ")";
            return CallHandlerTemplate.GetCallHandlerTemplates(HomeServer, out pTemplates, pPageNumber, pRowsPerPage, strQuery);
        }

        /// <summary>
        /// Fetches all users associated with this tenant.
        /// </summary>
        /// <param name="pUsers">
        /// List of users is returned as a generic list of UserBase objects on this out parameter.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetUsers(out List<UserBase> pUsers, int pPageNumber = 1,int pRowsPerPage = 20)
        {
            string strQuery = "query=(PartitionObjectId is " + this.PartitionObjectId + ")";
            return UserBase.GetUsers(HomeServer, out pUsers, pPageNumber, pRowsPerPage, strQuery);
        }

        /// <summary>
        /// Returns all system handlers sssociated with this tenant.  Primary call handlers are not included as of build
        /// 107.
        /// </summary>
        /// <param name="pHandlers">
        /// List of associated call handlers is passed back as a generic list of CallHandler objects on this 
        /// out parameter
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetCallHandlers(out List<CallHandler> pHandlers, int pPageNumber = 1, int pRowsPerPage = 20)
        {
            string strQuery = "query=(PartitionObjectId is " + this.PartitionObjectId + " & IsPrimary is 0)";
            return CallHandler.GetCallHandlers(HomeServer, out pHandlers, pPageNumber, pRowsPerPage, strQuery);
        }


        /// <summary>
        /// Returns all directory handlers associated with this tenant.
        /// </summary>
        /// <param name="pHandlers">
        /// List of associated directory handlers is passed back as a generic list of DirectoryHandler objects on this 
        /// out parameter
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetDirectoryHandlers(out List<DirectoryHandler> pHandlers, int pPageNumber = 1, int pRowsPerPage = 20)
        {
            string strQuery = "query=(PartitionObjectId is " + this.PartitionObjectId + ")";
            return DirectoryHandler.GetDirectoryHandlers(HomeServer, out pHandlers, pPageNumber, pRowsPerPage, strQuery, "");
        }


        /// <summary>
        /// Returns all interview handlers associated with this tenant.
        /// </summary>
        /// <param name="pHandlers">
        /// List of associated interview handlers is passed back as a generic list of InterviewHandler objects on this 
        /// out parameter
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetInterviewHandlers(out List<InterviewHandler> pHandlers, int pPageNumber = 1, int pRowsPerPage = 20)
        {
            string strQuery = "query=(PartitionObjectId is " + this.PartitionObjectId + ")";
            return InterviewHandler.GetInterviewHandlers(HomeServer, out pHandlers, pPageNumber, pRowsPerPage, strQuery);
        }


        /// <summary>
        /// Returns all public distribution lists associated with this tenant.
        /// </summary>
        /// <param name="pLists">
        /// List of associated public distribution lists is passed back as a generic list of DistributionList objects on this 
        /// out parameter
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch, defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, default to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and response from the server.
        /// </returns>
        public WebCallResult GetDistributionLists(out List<DistributionList> pLists, int pPageNumber = 1, int pRowsPerPage = 20)
        {
            string strQuery = "query=(PartitionObjectId is " + this.PartitionObjectId + ")";
            return DistributionList.GetDistributionLists(HomeServer, out pLists, pPageNumber, pRowsPerPage, strQuery);
        }

        #endregion //related object methods

        #endregion //instance objects
      
    }
}
