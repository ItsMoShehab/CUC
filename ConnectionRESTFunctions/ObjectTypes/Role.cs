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
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{
    /// <summary>
    /// Read only class for fetching all the roles defined on the Connection server and returning them as a generic list of 
    /// class objects.
    /// </summary>
    public class Role
    {

        #region Fields and Properties 

        public string RoleDescription { get; set; }
        public string RoleName { get; set; }
        public string ObjectId { get; set; }
        public bool IsEnabled { get; set; }
        public bool ReadOnly { get; set; }

        //reference to the ConnectionServer object used to create this instance.
        private readonly ConnectionServer _homeServer;

        #endregion


        #region Constructors

        //constructor
        public Role(ConnectionServer pConnectionServer, string pObjectId="", string pRoleName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to Role construtor");
            }

            _homeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId) && string.IsNullOrEmpty(pRoleName))
            {
                return;
            }
            
            WebCallResult res= GetRole(pObjectId,pRoleName);
            if (res.Success == false)
            {
                throw new Exception(string.Format("Failed to fetch role by ObjectId={0} or Name={1}",pObjectId,pRoleName));
            }
        }

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
                strObjectId = GetObjectIdFromName(_homeServer, pName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    res.Success = false;
                    res.ErrorText = "Could not find role name:" + pName;
                    return res;
                }
            }

            string strUrl = _homeServer.BaseUrl + "roles/" + strObjectId;

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, _homeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                res.Success = false;
                res.ErrorText = "No XML elements returned from role fetch";
                return res;
            }

            foreach (var oElement in res.XmlElement.Elements())
            {
                _homeServer.SafeXmlFetch(this,oElement);
            }

            res.Success = true;
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
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return "";
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                return "";
            }

            foreach (var oElement in res.XmlElement.Elements().Elements())
            {
                if (oElement.Name.ToString().Equals("ObjectId"))
                {
                    return oElement.Value;
                }
            }

            return "";
        }

        /// <summary>
        /// Gets the list of all roles and resturns them as a generic list of Role objects.  This
        /// list can be used for providing drop down list selection or the like.
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
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                pRoles =new List<Role>();
                return res;
            }

            pRoles = GetRolesFromXElements(pConnectionServer, res.XmlElement);
            return res;
        }


        //Helper function to take an XML blob returned from the REST interface for Roles returned and convert it into an generic
        //list of Role class objects.  
        private static List<Role> GetRolesFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {

            List<Role> oRoleList = new List<Role>();

            //Use LINQ to XML to create a list of Role objects in a single statement.
            var roles = from e in pXElement.Elements()
                                      where e.Name.LocalName == "Role"
                                      select e;

            //for each object returned in the list from the XML, construct a class object using the elements associated with that 
            //object.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlRole in roles)
            {
                Role oRole = new Role(pConnectionServer);
                foreach (XElement oElement in oXmlRole.Elements())
                {
                    //adds the XML property to the object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXmlFetch(oRole, oElement);
                }

                //add the fully populated object to the list that will be returned to the calling routine.
                oRoleList.Add(oRole);
            }

            return oRoleList;
        }

        #endregion


    }
}
