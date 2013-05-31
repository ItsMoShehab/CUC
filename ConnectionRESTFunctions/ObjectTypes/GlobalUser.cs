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
    /// The GlobalUser class holds data about a global user - these are users that are limited to data replicated around the network (i.e. very limited).
    /// You cannot add/delete or edit global users - only search them (alias/extension/first/last/display name etc...) to find their home server which 
    /// will then let you connection to that server for more robust API access to that user account. 
    /// </summary>
    public class GlobalUser :IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// Creates a new instance of the GlobalUser class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this user.  The GlobalUser class contains much less data than the UserFull class and is, as a result, quicker to fetch and 
        /// load and is used for all list presentations from searches.  
        /// If you pass the pObjectID parameter the user is automatically filled with data for that user from the server.  If no pObjectID is passed an
        /// empty instance of the GlobalUser class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the user being created.
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the user on the home server provided.  If no ObjectId is passed then an empty instance of the GlobalUser
        /// class is returned instead.
        /// </param>
        /// <param name="pAlias">
        /// Optional parameter for fetching a user's data based on alias.  If both the ObjectId and the Alias are passed, the ObjectId will be used 
        /// for the search.
        /// </param>
        public GlobalUser(ConnectionServerRest pConnectionServer, string pObjectId = "", string pAlias = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null Connection Server passed to the GlobalUser constructor");
            }

            HomeServer = pConnectionServer;

            if (pObjectId.Length == 0 & pAlias.Length == 0) return;

            //if the ObjectId or Alias are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetGlobalUser(pObjectId, pAlias);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("User not found in GlobalUser constructor using Alias={0} " +
                                                                         "and/or ObjectId={1}\n\rError={2}", pAlias, pObjectId, res.ErrorText));
            }
        }

        /// <summary>
        /// Generic constructor for Json parsing libaries
        /// </summary>
        public GlobalUser()
        {

        }

        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DisplayName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        //reference to the ConnectionServer object used to create this object instance.
        internal ConnectionServerRest HomeServer { get; private set; }

        #endregion


        #region GlobalUser Properties

        //The names of the properties must match exactly the tags in XML for them including case - the routine that deserializes data from XML into the 
        //objects requires this to match them up.
        [JsonProperty]
        public string Alias { get; private set; }

        [JsonProperty]
        public string DisplayName { get; private set; }

        [JsonProperty]
        public string AltFirstName { get; private set; }

        [JsonProperty]
        public string AltLastName { get; private set; }

        [JsonProperty]
        public string City { get; private set; }

        [JsonProperty]
        public string Department { get; private set; }

        [JsonProperty]
        public string DtmfAccessId { get; private set; }

        [JsonProperty]
        public string DtmfNameFirstLast { get; private set; }

        [JsonProperty]
        public string DtmfNameLastFirst { get; private set; }

        [JsonProperty]
        public string FirstName { get; private set; }

        [JsonProperty]
        public string LastName { get; private set; }

        [JsonProperty]
        public bool ListInDirectory { get; private set; }

        [JsonProperty]
        public string LocationObjectId { get; private set; }

        [JsonProperty]
        public bool IsTemplate { get; private set; }

        /// <summary>
        /// Unique identifier for the user 
        /// </summary>
        [JsonProperty]
        public string ObjectId { get; private set; }

        /// <summary>
        /// The unique identifier of the Partition to which the DtmfAccessId is assigned
        /// </summary>
        [JsonProperty]
        public string PartitionObjectId { get; private set; }

        [JsonProperty]
        public string XferString { get; private set; }

        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of globalusers from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(alias startswith ab)"
        /// sort: "sort=(alias asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <remarks>
        /// While this method name does have the plural in it, you can use it for fetching single users as well.  If searching by
        /// ObjectId just construct a query in the form "query=(ObjectId is {ObjectId})".  This is just as fast as using the URI format of 
        /// "{server name}\vmrest\users\{ObjectId}" but returns consistently formatted XML code as multiple users does so the parsing of 
        /// the data to deserialize it into User objects is consistent.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users are being fetched from.
        /// </param>
        /// <param name="pUsers">
        /// The list of users returned from the CUPI call (if any) is returned as a generic list of GlobalUser class instances via this out param.  
        /// If no users are  found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetUsers(ConnectionServerRest pConnectionServer, out List<GlobalUser> pUsers, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pUsers = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetUsers";
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "globalusers", pClauses);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's an error
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                pUsers = new List<GlobalUser>();
                return res;
            }

            //not an error, just return an empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                pUsers=new List<GlobalUser>();
                return res;
            }

            pUsers = pConnectionServer.GetObjectsFromJson<GlobalUser>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pUsers)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// This function allows for a GET of globalusers from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(alias startswith ab)"
        /// sort: "sort=(alias asc)"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <remarks>
        /// While this method name does have the plural in it, you can use it for fetching single users as well.  If searching by
        /// ObjectId just construct a query in the form "query=(ObjectId is {ObjectId})".  This is just as fast as using the URI format of 
        /// "{server name}\vmrest\users\{ObjectId}" but returns consistently formatted XML code as multiple users does so the parsing of 
        /// the data to deserialize it into User objects is consistent.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users are being fetched from.
        /// </param>
        /// <param name="pUsers">
        /// The list of users returned from the CUPI call (if any) is returned as a generic list of GlobalUser class instances via this out param.  
        /// If no users are  found NULL is returned for this parameter.
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

        public static WebCallResult GetUsers(ConnectionServerRest pConnectionServer, out List<GlobalUser> pUsers,int pPageNumber=1, 
            int pRowsPerPage=20,params string[] pClauses)
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

            return GetUsers(pConnectionServer, out pUsers, temp.ToArray());
        }


        /// <summary>
        /// returns a single GlobalUser object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the user is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the user to load
        /// </param>
        /// <param name="pUser">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <param name="pAlias">
        /// Optional parameter - since alias is unique for users you may pass in an empty ObjectId and use an alias instead.  If both the alias and
        /// ObjectId are passed, the ObjectId will be used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetUser(out GlobalUser pUser, ConnectionServerRest pConnectionServer, string pObjectId, string pAlias = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pUser = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetUser";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId)&& string.IsNullOrEmpty(pAlias))
            {
                res.ErrorText = "Emtpy ObjectId and Alias passed to GetUser";
                return res;
            }

            //create a new GlobalUser instance passing the ObjectId which fills out the data automatically
            try
            {
                pUser = new GlobalUser(pConnectionServer, pObjectId, pAlias);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch user in GetUser:" + ex.Message;
                return res;
            }

            return res;
        }

        #endregion


        #region Instance Methods

        //Fills the current instance of GlobalUser in with properties fetched from the server.  The fetch uses a query construction instead of the full ObjectId
        //construction which returns less data and is quicker.

        /// <summary>
        /// Diplays the alias, display name and extension of the user by default.
        /// </summary>
        public override string ToString()
        {
            return String.Format("Global user:{0} [{1}] x{2}", this.Alias, this.DisplayName, this.DtmfAccessId);
        }

        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchGlobalUserData()
        {
            return GetGlobalUser(this.ObjectId);
        }


        /// <summary>
        /// Helper function to fill in the user instance with data from a user by their objectID string or their alias string.
        /// </summary>
        /// <param name="pObjectId"></param>
        /// <param name="pAlias"></param>
        /// <returns></returns>
        private WebCallResult GetGlobalUser(string pObjectId, string pAlias = "")
        {
            //when fetching a base user use the query construct (which returns less data and is quicker) than the users/(objectid) format for 
            //UserFull object.
            string strObjectId = pObjectId;
            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdFromAlias(pAlias);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult {Success = false, ErrorText = "No global user found with alias = " + pAlias};
                }
            }

            string strUrl = string.Format("{0}globalusers/{1}", HomeServer.BaseUrl, strObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
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

            return res;
        }

        /// <summary>
        /// Fetch the ObjectId of a global user by it's alias.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pAlias">
        /// Alias of the global user to find
        /// </param>
        /// <returns>
        /// ObjectId of global user if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromAlias(string pAlias)
        {
           string strUrl = string.Format("{0}globalusers?query=(Alias is {1})", HomeServer.BaseUrl, pAlias.UriSafe());

            //issue the command to the CUPI interface
           WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false || res.TotalObjectCount == 0)
            {
                return "";
            }

            List<GlobalUser> oTemplates = HomeServer.GetObjectsFromJson<GlobalUser>(res.ResponseText);

            foreach (var oTemplate in oTemplates)
            {
                if (oTemplate.Alias.Equals(pAlias, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oTemplate.ObjectId;
                }
            }

            return "";
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the user object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the user object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix="")
        {
            StringBuilder strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
        }

     
        #endregion


    }
}
