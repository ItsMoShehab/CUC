#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    public class Tenant
    {
        #region Fields and Properties

        public string ObjectId { get; set; }
        public string Alias { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public string SmtpDomain { get; set; }
        public string AttributeObjectId { get; set; }
        public int AttributeType { get; set; }
        public string MailboxStoreObjectId { get; set; }
        public string PhoneSystemObjectId { get; set; }
        public string PartitionObjectId { get; set; }
        
        public string CosesURI { get; set; }
        public string ScheduleSetsURI { get; set; }


        public ConnectionServer HomeServer { get; private set; }

        #endregion


        #region Constructors

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
        public Tenant(ConnectionServer pConnectionServer, string pObjectId = "", string pAlias = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to Tenant construtor");
            }

            //keep track of the home Connection server this tenant is created on.
            HomeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or alias then go load that tenant up, otherwise just return an empty instance.
            if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pAlias))) return;

            //if the ObjectId or alias are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetTenant(pObjectId, pAlias);

            if (res.Success == false)
            {
                throw new Exception(
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
        /// If no tenants are found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetTenants(ConnectionServer pConnectionServer, out List<Tenant> pTenants,params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pTenants = new List<Tenant>();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetTenants";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "tenants", pClauses);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }


            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty this isn't an error - just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount==0)
            {
                return res;
            }

            pTenants = HTTPFunctions.GetObjectsFromJson<Tenant>(res.ResponseText);

            if (pTenants == null)
            {
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pTenants)
            {
                oObject.HomeServer = pConnectionServer;
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
        /// If no tenants are found NULL is returned for this parameter.
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
        public static WebCallResult GetTenants(ConnectionServer pConnectionServer, out List<Tenant> pTenants,
            int pPageNumber = 1, int pRowsPerPage = 20, params string[] pClauses)
        {
            //tack on the paging items to the parameters list
            var temp = pClauses.ToList();
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
        public static WebCallResult GetTenant(out Tenant pTenant, ConnectionServer pConnectionServer, string pObjectId = "", string pAlias = "")
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
                res.Success = true;
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
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddTenant(ConnectionServer pConnectionServer,
                                                    string pAlias,
                                                    string pSmtpDomain,
                                                    string pDescription)
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

            //create an empty property list if it's passed as null since we use it below
            var oPropList = new ConnectionPropertyList();

            oPropList.Add("Alias", pAlias);
            oPropList.Add("SmtpDomain", pSmtpDomain);
            oPropList.Add("Description", pDescription);

            string strBody = "<Tenant>";

            foreach (var oPair in oPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Tenant>";

            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "tenants", MethodType.POST, pConnectionServer, strBody, false);

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
        /// <param name="pTenant">
        /// Instance of the newly created tenant is passed back on this out param
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddTenant(ConnectionServer pConnectionServer,
                                              string pAlias,
                                              string pSmtpDomain,
                                              string pDescription,
                                              out Tenant pTenant)
        {
            pTenant = null;
            var res = AddTenant(pConnectionServer, pAlias, pSmtpDomain, pDescription);

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
        public static WebCallResult DeleteTenant(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteTenant";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "tenants/" + pObjectId,
                                            MethodType.DELETE, pConnectionServer, "");
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
        /// 
        /// </summary>
        /// <param name="pObjectId"></param>
        /// <param name="pAlias"></param>
        /// <returns></returns>
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
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

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
            string strUrl = string.Format("{0}tenants/?query=(Alias is {1})", HomeServer.BaseUrl, pAlias);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false || res.TotalObjectCount == 0)
            {
                return "";
            }

            List<Tenant> oTenants = HTTPFunctions.GetObjectsFromJson<Tenant>(res.ResponseText);

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

        #endregion


    }
}
