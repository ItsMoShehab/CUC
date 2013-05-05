#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Read only class for fetching all the roles defined on the Connection server and returning them as a generic list of 
    /// class objects.
    /// </summary>
    public class Role
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires ConnectionServer object where the role is defined.  Optionally you can pass the ObjectId or
        /// name of the role to fetch data for that role specifically.
        /// </summary>
        public Role(ConnectionServer pConnectionServer, string pObjectId = "", string pRoleName = "")
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
        /// General constructor for Json parsing libraries
        /// </summary>
        public Role()
        {
        }

        #endregion


        #region Fields and Properties 

        //reference to the ConnectionServer object used to create this instance.
        public ConnectionServer HomeServer { get; private set; }

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


        #region Methods

        /// <summary>
        /// Returns a string with the name and description of the role
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", RoleName, RoleDescription);
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
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false || res.TotalObjectCount == 0)
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

            return res;
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
        public static string GetObjectIdFromName(ConnectionServer pConnectionServer, string pRoleName)
        {
            string strUrl = pConnectionServer.BaseUrl + string.Format("roles/?query=(RoleName is {0})", pRoleName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false || res.TotalObjectCount == 0)
            {
                return "";
            }

            List<Role> oTemplates = HTTPFunctions.GetObjectsFromJson<Role>(res.ResponseText);

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
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRoles(ConnectionServer pConnectionServer, out List<Role> pRoles)
        {
            WebCallResult res;
            pRoles = null;

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetRoles";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + "roles";

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one role
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pRoles = new List<Role>();
                res.Success = false;
                return res;
            }

            pRoles = HTTPFunctions.GetObjectsFromJson<Role>(res.ResponseText);

            if (pRoles == null)
            {
                pRoles = new List<Role>();
                return res;
            }

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
