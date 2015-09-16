#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Read only class for fetching all the roles defined on the Connection server and returning them as a generic list of 
    /// class objects.
    /// </summary>
    [Serializable]
    public class Role : IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires ConnectionServer object where the role is defined.  Optionally you can pass the ObjectId or
        /// name of the role to fetch data for that role specifically.
        /// </summary>
        public Role(ConnectionServerRest pConnectionServer, string pObjectId = "", string pRoleName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to Role construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId) && string.IsNullOrEmpty(pRoleName))
            {
                return;
            }

            WebCallResult res = GetRole(pObjectId, pRoleName);
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Failed to fetch role by ObjectId={0} or Name={1}", pObjectId, pRoleName));
            }
        }

        /// <summary>
        /// Constructor requires ConnectionServer object where the role is defined.  Optionally you can pass the enum of the role you want to
        /// populate the class instance with
        /// </summary>
        public Role(ConnectionServerRest pConnectionServer, RoleName pRoleName)
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to Role construtor");
            }

            HomeServer = pConnectionServer;

            WebCallResult res = GetRole(pRoleName);
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res, string.Format("Failed to fetch role by enum={0}", pRoleName));
            }
        }

        /// <summary>
        /// General constructor for Json parsing libraries
        /// </summary>
        public Role()
        {
        }

        #endregion


        #region Fields and Properties 

        //reference to the ConnectionServer object used to create this instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return RoleName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }


        #endregion


        #region Role Properties

        [JsonProperty]
        public string RoleDescription { get; private set; }

        [JsonProperty]
        public string RoleName { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public bool IsEnabled { get; private set; }

        [JsonProperty]
        public bool ReadOnly { get; private set; }
        
        #endregion


        #region Instance Methods


        /// <summary>
        /// Returns a string with the name and description of the role
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", RoleName, RoleDescription);

        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the greeting object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the alternate extension object instance.
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
        /// Fetch a role by RoleName enum value and fill current object with the values of that role
        /// </summary>
        /// <param name="pName">
        /// RoleName enum value for the role to find
        /// </param>
        /// <returns>
        /// WebCallResults instance.
        /// </returns>
        private WebCallResult GetRole(RoleName pName)
        {
            string strRoleName = GetRoleName(pName);
            if (string.IsNullOrEmpty(strRoleName))
            {
                WebCallResult oRes = new WebCallResult
                    {
                        Success = false,
                        ErrorText = "Invalid role name value passed to GetRole on Role.cs:" + pName
                    };
                return oRes;
            }
            return GetRole("", strRoleName);
        }

        /// <summary>
        /// Fetch a role by objectId or name and fill the properties (if found) of the current class instance with what's found
        /// </summary>
        /// <param name="pObjectId">
        /// GUID of the role to find.  
        /// </param>
        /// <param name="pName">
        /// Name of role to find
        /// </param>
        /// <returns>
        /// WebCallResults instance.
        /// </returns>
        private WebCallResult GetRole(string pObjectId, string pName)
        {
            WebCallResult res = new WebCallResult();

            string strObjectId = pObjectId;
            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdFromName(HomeServer, pName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    res.Success = false;
                    res.ErrorText = "Could not find role name:" + pName;
                    return res;
                }
            }

            string strUrl = HomeServer.BaseUrl + "roles/" + strObjectId;

            //issue the command to the CUPI interface
            res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

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
        /// Fetch a role by RoleName enum value and fill current object with the values of that role
        /// </summary>
        /// <param name="pName">
        /// RoleName enum value for the role to find
        /// </param>
        /// <returns>
        /// WebCallResults instance.
        /// </returns>
        private static string GetRoleName(RoleName pName)
        {
            switch (pName)
            {
                case RestFunctions.RoleName.AudioTextAdministrator:
                    return "Audio Text Administrator";
                case RestFunctions.RoleName.AuditAdministrator:
                    return "Audit Administrator";
                case RestFunctions.RoleName.GreetingAdministrator:
                    return "Greeting Administrator";
                case RestFunctions.RoleName.HelpDeskAdministrator:
                    return "Help Desk Administrator";
                case RestFunctions.RoleName.MailboxAccessDelegateAccount:
                    return "Mailbox Access Delegate Account";
                case RestFunctions.RoleName.RemoteAdministrator:
                    return "Remote Administrator";
                case RestFunctions.RoleName.SystemAdministrator:
                    return "System Administrator";
                case RestFunctions.RoleName.Technician:
                    return "Technician";
                case RestFunctions.RoleName.TenantAdministrator:
                    return "Tenant Administrator";
                case RestFunctions.RoleName.UserAdministrator:
                    return "User Administrator";
                default:
                    return "";
            }
        }

        /// <summary>
        /// searches for a role by the name passed in - if one is found the ObjectId of that role is returned, otherwise blank is returned. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to query
        /// </param>
        /// <param name="pRoleName">
        /// Name of the role to search for - this is not case sensitive
        /// </param>
        /// <returns>
        /// ObjectId of the role if found, blank string if not.
        /// </returns>
        public static string GetObjectIdFromName(ConnectionServerRest pConnectionServer, string pRoleName)
        {
            string strUrl = pConnectionServer.BaseUrl + string.Format("roles/?query=(RoleName is {0})", pRoleName.UriSafe());

            //issue the command to the CUPI interface
            WebCallResult res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false || res.TotalObjectCount == 0)
            {
                return "";
            }

            List<Role> oTemplates = pConnectionServer.GetObjectsFromJson<Role>(res.ResponseText);

            foreach (var oTemplate in oTemplates)
            {
                if (oTemplate.RoleName.Equals(pRoleName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oTemplate.ObjectId;
                }
            }

            return "";
        }

        /// <summary>
        /// Gets the list of all roles and resturns them as a generic list of Role objects.  This
        /// list can be used for providing drop down list selection or the like.
        /// The roles in Connection are static and cannot be added to.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that the roles should be pulled from
        /// </param>
        /// <param name="pRoles">
        /// Out parameter that is used to return the list of Role objects defined on Connection
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter 
        /// at a time are currently supported by CUPI - in other words you can't have "query=(rolename startswith ab)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>        
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRolesForSystem(ConnectionServerRest pConnectionServer, out List<Role> pRoles, params string[] pClauses)
        {
            WebCallResult res;
            pRoles = new List<Role>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult {ErrorText = "Null ConnectionServer referenced passed to GetRolesForSystem",Success = false};
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "roles",pClauses);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one role
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                res.Success = false;
                return res;
            }

            pRoles = pConnectionServer.GetObjectsFromJson<Role>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pRoles)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        ///  Adds a role assignment to a user.  Returns a failure response if that user already has the role assigned to them.
        /// </summary>
        /// <param name="pConnectionServer" type="Cisco.UnityConnection.RestFunctions.ConnectionServerRest">
        /// Connection server that the user being edited lives on.
        /// </param>
        /// <param name="pUserObjectId" type="string">
        /// Unique ID of the user to add the role to    
        /// </param>
        /// <param name="pRoleObjectId" type="string">
        /// Unique ID of the role to add to the user
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddRoleToUser(ConnectionServerRest pConnectionServer,string pUserObjectId, string pRoleObjectId)
        {
            WebCallResult res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddRoleToUser";
                return res;
            }

            if (string.IsNullOrEmpty(pUserObjectId))
            {
                res.ErrorText = "Empty user ObjectId passed to AddRoleToUser";
                return res;
            }

            if (string.IsNullOrEmpty(pRoleObjectId))
            {
                res.ErrorText = "Empty role ObjectId passed to AddRoleToUser";
                return res;
            }

            string strBody = "<UserRole>";
            strBody += string.Format("<RoleObjectId>{0}</RoleObjectId>", pRoleObjectId);
            strBody += "</UserRole>";

            return pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/userroles", pConnectionServer.BaseUrl,pUserObjectId), 
                MethodType.POST, strBody, false);
        }

        /// <summary>
        ///  Adds a role assignment to a user.  Returns a failure response if that user already has the role assigned to them.
        /// </summary>
        /// <param name="pConnectionServer" type="Cisco.UnityConnection.RestFunctions.ConnectionServerRest">
        /// Connection server that the user being edited lives on.
        /// </param>
        /// <param name="pUserObjectId" type="string">
        /// Unique ID of the user to add the role to    
        /// </param>
        /// <param name="pRoleName" type="string">
        /// value from the RoleName enum representing the role to add to the user
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddRoleToUser(ConnectionServerRest pConnectionServer, string pUserObjectId,RoleName pRoleName)
        {
            string strRoleName = GetRoleName(pRoleName);
            if (string.IsNullOrEmpty(strRoleName))
            {
                WebCallResult res = new WebCallResult
                {
                    Success = false,
                    ErrorText = "Invalid role enum value passed to AddRoleToUser:" + pRoleName
                };
                return res;
            }

            string strRoleObjectId = GetObjectIdFromName(pConnectionServer, strRoleName);
            if (string.IsNullOrEmpty(strRoleObjectId))
            {
                WebCallResult res = new WebCallResult
                {
                    Success = false,
                    ErrorText = "Could not find role ObjectId by name in AddRoleToUser:" + strRoleName
                };
                return res;
            }
            return AddRoleToUser(pConnectionServer, pUserObjectId, strRoleObjectId);
        }


        /// <summary>
        ///  Removes a role assignment from a user.  Returns a failure response if that user does not have the role assigned to them.
        /// </summary>
        /// <param name="pConnectionServer" type="Cisco.UnityConnection.RestFunctions.ConnectionServerRest">
        /// Connection server that the user being edited lives on.
        /// </param>
        /// <param name="pUserObjectId" type="string">
        /// Unique ID of the user to remove the role from
        /// </param>
        /// <param name="pRoleObjectId" type="string">
        /// unique Id of the role to remove from the user - this is the unique id of the user's role mapping, not the id of the role in the system.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult RemoveRoleFromUser(ConnectionServerRest pConnectionServer, string pUserObjectId, string pRoleObjectId)
        {
            WebCallResult res = new WebCallResult { Success = false };

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to RemoveRoleFromUser";
                return res;
            }

            if (string.IsNullOrEmpty(pUserObjectId))
            {
                res.ErrorText = "Empty user ObjectId passed to RemoveRoleFromUser";
                return res;
            }

            if (string.IsNullOrEmpty(pRoleObjectId))
            {
                res.ErrorText = "Empty role ObjectId passed to RemoveRoleFromUser";
                return res;
            }

            return pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/userroles/{2}", pConnectionServer.BaseUrl, pUserObjectId,pRoleObjectId),
                MethodType.DELETE, "", false);
        }

        /// <summary>
        ///  Removes a role assignment from a user.  Returns a failure response if that user does not have the role assigned to them.
        /// </summary>
        /// <param name="pConnectionServer" type="Cisco.UnityConnection.RestFunctions.ConnectionServerRest">
        /// Connection server that the user being edited lives on.
        /// </param>
        /// <param name="pUserObjectId" type="string">
        /// Unique ID of the user to remove the role from
        /// </param>
        /// <param name="pRoleName" type="string">
        /// value from the RoleName enum representing the role to add to the user
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>

        public static WebCallResult RemoveRoleFromUser(ConnectionServerRest pConnectionServer, string pUserObjectId,RoleName pRoleName)
        {
            WebCallResult res = new WebCallResult
            {
                Success = false
            };
            string strRoleName = GetRoleName(pRoleName);
            if (string.IsNullOrEmpty(strRoleName))
            {
                res.ErrorText = "Invalid role enum value passed to RemoveRoleFromUser:" + pRoleName;
                return res;
            }

            List<Role> oRoles;
            res = Role.GetRolesForUser(pConnectionServer, pUserObjectId, out oRoles);
            if (!res.Success) return res;
            foreach (var oRole in oRoles)
            {
                if (oRole.RoleName == strRoleName)
                {
                    return RemoveRoleFromUser(pConnectionServer, pUserObjectId, oRole.ObjectId);
                }
            }
            res.ErrorText = "Role not assigned to user";
            return res;
        }

        /// <summary>
        /// Returns a list of role objects that are assigned to the user - an empty list is returned if the user has no roles    
        /// </summary>
        /// <param name="pConnectionServer" type="Cisco.UnityConnection.RestFunctions.ConnectionServerRest">
        ///  ConnectionServer the user is hosted on
        /// </param>
        /// <param name="pUserObjectId" type="string">
        ///  Unique Id of the user to get the roles for
        /// </param>
        /// <param name="pRoles">
        ///  List of Role objects corresponding to the roles assigned to the user.  Can return empty list.
        /// </param>
        /// <returns>
        ///  Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRolesForUser(ConnectionServerRest pConnectionServer, string pUserObjectId, out List<Role> pRoles)
        {
            WebCallResult res;
            pRoles = new List<Role>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult { ErrorText = "Null ConnectionServer referenced passed to GetRolesForUser", Success = false };
                return res;
            }

            res= pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/userroles", pConnectionServer.BaseUrl, pUserObjectId),MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - a 0 count list is ok, though
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                return res;
            }
            if (res.TotalObjectCount == 0)
            {
                res.Success = true;
                return res;
            }

            pRoles = pConnectionServer.GetObjectsFromJson<Role>(res.ResponseText,"UserRole");

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pRoles)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        #endregion

    }
}
