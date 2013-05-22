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
    /// Class that provides methods for featching policies in Connection.  No adding/updating/deleting of policies
    /// is supported yet.
    /// </summary>
    public class Policy 
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires the ConnectionServer that the Policy object lives on and can optionally take an ObjectId for 
        /// a policy to load up data for.
        /// </summary>
        public Policy(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to Policy construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            WebCallResult res = GetPolicy(pObjectId);
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Failed to fetch policy by ObjectId={0}", pObjectId));
            }
        }

        /// <summary>
        /// general constructor for Json parsing libararies
        /// </summary>
        public Policy()
        {

        }

        #endregion


        #region Fields and Properties 

        //reference to the ConnectionServer object used to create this instance.
        public ConnectionServer HomeServer { get; private set; }

        #endregion


        #region Policy Properties

        [JsonProperty]
        public DateTime DateCreated { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string RoleObjectId { get; private set; }

        [JsonProperty]
        public string TargetVmsObjectId { get; private set; }

        [JsonProperty]
        public string TargetHandlerObjectId { get; private set; }

        [JsonProperty]
        public string UserObjectId { get; private set; }

        [JsonProperty]
        public string VmsObjectId { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the name and description of the role
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("User:{0}, Role:{1}", UserObjectId, RoleObjectId);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the policy object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the Policy object instance.
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
        /// Fetch a role by objectId or name and fill the properties (if found) of the current class instance with what's found
        /// </summary>
        /// <param name="pObjectId">
        /// GUID of the role to find.  
        /// </param>
        /// <returns>
        /// WebCallResults instance.
        /// </returns>
        private WebCallResult GetPolicy(string pObjectId)
        {
            string strUrl = HomeServer.BaseUrl + "policies/" + pObjectId;

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

        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of policies from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(UserObjectId is 0d84fee3-8680-4bd2-aa81-49e32921299b)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the policies are being fetched from.
        /// </param>
        /// <param name="pPolicies">
        /// The list of policies returned from the CUPI call (if any) is returned as a generic list of Policy class instances via this out param.  
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and 
        /// "query=(UserObjectId is 0d84fee3-8680-4bd2-aa81-49e32921299b)"
        /// in the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPolicies(ConnectionServer pConnectionServer, out List<Policy> pPolicies, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pPolicies = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPolicies";
                return res;
            }

            string strUrl = ConnectionServer.AddClausesToUri(pConnectionServer.BaseUrl + "policies", pClauses);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's not an error, just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pPolicies = new List<Policy>();
                return res;
            }

            pPolicies = pConnectionServer.GetObjectsFromJson<Policy>(res.ResponseText);

            //special case - Json.Net always creates an object even when there's no data for it.
            if (pPolicies == null || (pPolicies.Count == 1 && string.IsNullOrEmpty(pPolicies[0].ObjectId)))
            {
                pPolicies = new List<Policy>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pPolicies)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// This function allows for a GET of policies from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(UserObjectId is 0d84fee3-8680-4bd2-aa81-49e32921299b)"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the policies are being fetched from.
        /// </param>
        /// <param name="pPolicies">
        /// The list of Policies returned from the CUPI call (if any) is returned as a generic list of Policy class instances via this out param.  
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and 
        /// "query=(UserObjectId is 0d84fee3-8680-4bd2-aa81-49e32921299b)"
        /// in the same call.  Also if you have a sort and a query clause they must both reference the same column.
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
        public static WebCallResult GetPolicies(ConnectionServer pConnectionServer, out List<Policy> pPolicies,int pPageNumber=1, 
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

            return GetPolicies(pConnectionServer, out pPolicies, temp.ToArray());
        }


        /// <summary>
        /// Helper function for fetching all policies targeted at a particular user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the policies are being fetched from.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID of the user to fetch for.
        /// </param>
        /// <param name="pPolicies">
        /// The list of policies returned from the CUPI call (if any) is returned as a generic list of Policy class instances via this out param.  
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPoliciesForUser(ConnectionServer pConnectionServer,string pUserObjectId,
                                                       out List<Policy> pPolicies)
        {
            string strClause = string.Format("query=(UserObjectId is {0})",pUserObjectId);

            return GetPolicies(pConnectionServer, out pPolicies, strClause);
        }


        /// <summary>
        /// Helper function for fetching all role names the user has assigned to them - this is useful for quickly checking
        /// if a user is authorized for a particular task.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the policies are being fetched from.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID of the user to fetch for.
        /// </param>
        /// <param name="pRoleNames">
        /// The list of role names the user has assigned to them.  This can be empty (users can have no roles).
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRoleNamesForUser(ConnectionServer pConnectionServer, string pUserObjectId,
                                                        out List<string> pRoleNames)
        {
            pRoleNames=new List<string>();
            
            if (pConnectionServer == null)
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "Null Connecton server passed to GetRoleNamesForUser"
                    };
            }

            //fetch the list of policies the user has assigned to them (if any)
            List<Policy> oPolicies;
            WebCallResult res = GetPoliciesForUser(pConnectionServer, pUserObjectId, out oPolicies);
            if (res.Success == false)
            {
                return res;
            }

            if (oPolicies.Count == 0)
            {
                //no need to continue here
                return new WebCallResult { Success = true };
            }

            //get the list of roles - this is a static list of 9 items currently but may expand later.
            List<Role> oRoles;
            res = Role.GetRoles(pConnectionServer, out oRoles);

            if (res.Success == false)
            {
                return res;
            }

            //there can be duplicates so only add each role name once.
            foreach (var oPolicy in oPolicies)
            {
                string strRoleName = oRoles.First(oRole => oRole.ObjectId == oPolicy.RoleObjectId).RoleName;
                if (!pRoleNames.Contains(strRoleName))
                {
                    pRoleNames.Add(strRoleName);
                }
            }

            return new WebCallResult{Success = true};
        }

        /// <summary>
        /// Helper function for fetching all policies for a particular role
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the policies are being fetched from.
        /// </param>
        /// <param name="pRoleObjectId">
        /// GUID of the role to fetch for.
        /// </param>
        /// <param name="pPolicies">
        /// The list of policies returned from the CUPI call (if any) is returned as a generic list of Policy instances via this out param.  
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPoliciesForRole(ConnectionServer pConnectionServer, string pRoleObjectId,
                                                       out List<Policy> pPolicies)
        {
            string strClause = string.Format("query=(RoleObjectId is {0})", pRoleObjectId);

            return GetPolicies(pConnectionServer, out pPolicies, strClause);
        }

        #endregion
    }
}
