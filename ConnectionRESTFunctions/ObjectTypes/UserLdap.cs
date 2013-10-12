using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// When importing users via LDAP this class is used for fetching available users and actually doing the 
    /// user imports
    /// </summary>
    [Serializable]
    public class UserLdap : IUnityDisplayInterface
    {

        #region Constructors and Destructors

         /// <summary>
         /// Empty constructor is all that's needed here - you don't fetch a specific LdapUser, they are used only
         /// for importing as UnityConnection users via LDAP
         /// </summary>

        #endregion


        #region Fileds and Properties

        public ConnectionServerRest HomeServer;

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return Alias; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return Pkid; } }

        #endregion


        #region UserLdap Fields and Properties

        [JsonProperty]
        public string Alias { get; private set; }

        [JsonProperty]
        public string FirstName { get; private set; }
        
        [JsonProperty]
        public string LastName { get; private set; }
        
        [JsonProperty]
        public string Pkid { get; private set; }

        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of Ldapusers from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(firstname startswith ab)"
        /// sort: "sort=(alias asc)"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <remarks>
        /// While this method name does have the plural in it, you'll want to use it for fetching single users as well.  If searching by
        /// pKid just construct a query in the form "query=(pkid is {pkid})".
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users are being fetched from.
        /// </param>
        /// <param name="pUsers">
        /// The list of users returned from the CUPI call (if any) is returned as a generic list of UserLdap class instances via this out param.  
        /// If no users are found this is returned as en empty 
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetLdapUsers(ConnectionServerRest pConnectionServer, out List<UserLdap> pUsers, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pUsers = new List<UserLdap>();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetLdapUsers";
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "import/users/ldap", pClauses);

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

            pUsers = pConnectionServer.GetObjectsFromJson<UserLdap>(res.ResponseText, "ImportUser");

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oUser in pUsers)
            {
                oUser.HomeServer = pConnectionServer;
            }

            return res;
        }

        /// <summary>
        /// This function allows for a GET of Ldapusers from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(firstname startswith ab)"
        /// sort: "sort=(alias asc)"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <remarks>
        /// While this method name does have the plural in it, you'll want to use it for fetching single users as well.  If searching by
        /// pKid just construct a query in the form "query=(pkid is {pkid})".
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users are being fetched from.
        /// </param>
        /// <param name="pUsers">
        /// The list of users returned from the CUPI call (if any) is returned as a generic list of UserLdap class instances via this out param.  
        /// If no users are found this is returned as en empty 
        /// </param>
        /// <param name="pRowsPerPage">
        /// How many rows to fetch at a time (defaults to 20)
        /// </param>
        /// <param name="pPageNumber">
        /// Page number to fetch (Starting with 1) when paging through large numbers of results.  Defaults to 1.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetLdapUsers(ConnectionServerRest pConnectionServer, out List<UserLdap> pUsers,
                                                 int pPageNumber = 0, int pRowsPerPage = 20, params string[] pClauses)
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

            //the LDAP user list uses different naming conventions for page/rows than the resst of of the CUPI interface
            temp.Add("offset=" + pPageNumber);
            temp.Add("limit=" + pRowsPerPage);

            return GetLdapUsers(pConnectionServer, out pUsers, temp.ToArray());
        }


        /// <summary>
        /// Import an LDAP user as a local Unity Connection user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to import the user on.
        /// </param>
        /// <param name="pTemplateAlias">
        /// Alias of the user template to use when importing the user, required.
        /// </param>
        /// <param name="pPkid">
        /// Unique ID from the Call Manager database for the LDAP synchronized user.
        /// </param>
        /// <param name="pAlias">
        /// Alias of the user in LDAP to import
        /// </param>
        /// <param name="pFirstName">
        /// First name of the user to import
        /// </param>
        /// <param name="pLastName">
        /// Last name of the user to import
        /// </param>
        /// <param name="pExtension">
        /// Extension number to assign the user in Connection's diretory
        /// </param>
        /// <param name="pPropList">
        /// Name value pair list of optional values to include in the import.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the call and results from the server.
        /// </returns>
        public static WebCallResult ImportLdapUser(ConnectionServerRest pConnectionServer, string pTemplateAlias, 
                                                   string pPkid, string pAlias, string pFirstName, string pLastName, 
                                                    string pExtension,ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to ImportLdapUser";
                return res;
            }

            //make sure that something is passed in for the 3 required params 
            if (String.IsNullOrEmpty(pTemplateAlias) || string.IsNullOrEmpty(pPkid) || string.IsNullOrEmpty(pExtension))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in ImportLdapUser on ConnectionServer.cs";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            //cheat here a bit and simply add the alias and extension values to the proplist where it can be tossed into the body later.
            pPropList.Add("pkid", pPkid);
            pPropList.Add("alias", pAlias);
            pPropList.Add("firstName", pFirstName);
            pPropList.Add("lastName", pLastName);
            pPropList.Add("dtmfAccessId", pExtension);

            //use JSON style body payload
            string strBody = "{";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("\"{0}\":\"{1}\",", oPair.PropertyName, oPair.PropertyValue);
            }
            strBody = strBody.TrimEnd(',') + "}";

            res = pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "import/users/ldap?templateAlias=" + pTemplateAlias,
                MethodType.POST, strBody);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (!string.IsNullOrEmpty(res.ResponseText) && res.ResponseText.Contains(@"/vmrest/users/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/users/", "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Import an LDAP user as a local Unity Connection user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to import the user on.
        /// </param>
        /// <param name="pTemplateAlias">
        /// Alias of the user template to use when importing the user, required.
        /// </param>
        /// <param name="pPkid">
        /// Unique ID from the Call Manager database for the LDAP synchronized user.
        /// </param>
        /// <param name="pAlias">
        /// Alias of the user in LDAP to import
        /// </param>
        /// <param name="pFirstName">
        /// First name of the user to import
        /// </param>
        /// <param name="pLastName">
        /// Last name of the user to import
        /// </param>
        /// <param name="pExtension">
        /// Extension number to assign the user in Connection's diretory
        /// </param>
        /// <param name="pPropList">
        /// Name value pair list of optional values to include in the import.
        /// </param>
        /// <param name="pUser">
        /// Instance of the UserFull class is passed back filled in with the details of the newly import user if the 
        /// import succeeds.  Null if the import fails.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the call and results from the server.
        /// </returns>
        public static WebCallResult ImportLdapUser(ConnectionServerRest pConnectionServer, string pTemplateAlias,
                                                   string pPkid, string pAlias, string pFirstName, string pLastName,
                                                   string pExtension, ConnectionPropertyList pPropList,out UserFull pUser)
        {
            pUser = null;

            var res = ImportLdapUser(pConnectionServer, pTemplateAlias, pPkid, pAlias, pFirstName, pLastName, 
                                    pExtension,pPropList);
            if (res.Success)
            {
                return UserFull.GetUser(pConnectionServer, res.ReturnedObjectId, out pUser);
            }
            return res;
        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays the alias, display name and extension of the user by default.
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0}, {1} {2} [{3}", this.Alias, this.FirstName, this.LastName,this.Pkid);
        }


        /// <summary>
        /// Import an LDAP user as a full Unity Connection user
        /// </summary>
        /// <param name="pExtension">
        /// Extension to assign to newly imported LDAP user- must be unique in the partition being imported to
        /// </param>
        /// <param name="pTemplateAlias">
        /// Alias of the user template to use when importing the user.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details on the call and results to the server.
        /// </returns>
        public WebCallResult Import(string pExtension, string pTemplateAlias)
        {
            return ImportLdapUser(HomeServer, pTemplateAlias, Pkid, Alias, FirstName, LastName, pExtension, null);
        }


        /// <summary>
        /// Import an LDAP user as a full Unity Connection user
        /// </summary>
        /// <param name="pExtension">
        /// Extension to assign to newly imported LDAP user- must be unique in the partition being imported to
        /// </param>
        /// <param name="pTemplateAlias">
        /// Alias of the user template to use when importing the user.
        /// </param>
        /// <param name="pUser">
        /// Instance of the UserFull class filled in with the details of the newly import user are returned on thi s
        /// out param if the import is successful - this is null otherwise.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details on the call and results to the server.
        /// </returns>
        public WebCallResult Import(string pExtension, string pTemplateAlias, out UserFull pUser)
        {
            return ImportLdapUser(HomeServer, pTemplateAlias, Pkid, Alias, FirstName, LastName, pExtension, null,out pUser);
        }

        #endregion

    }
}
