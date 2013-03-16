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
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{
    /// <summary>
    /// Class that provides methods for creating, featching and deleting policies in Connection.
    /// </summary>
    public class Policy
    {
        #region Fields and Properties 

        public string ObjectId { get; set; }
        public string UserObjectId { get; set; }
        public string VmsObjectId { get; set; }
        public string RoleObjectId { get; set; }
        public DateTime DateCreated { get; set; }
        public string TargetVmsObjectId { get; set; }
        public string TargetHandlerObjectId { get; set; }

        //reference to the ConnectionServer object used to create this instance.
        private readonly ConnectionServer _homeServer;

        #endregion


        #region Constructors

        //constructor
        public Policy(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to Policy construtor");
            }

            _homeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }
            
            WebCallResult res= GetPolicy(pObjectId);
            if (res.Success == false)
            {
                throw new Exception(string.Format("Failed to fetch policy by ObjectId={0}",pObjectId));
            }
        }

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
        /// Dumps out all the properties associated with the instance of the call handler object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the call handler object instance.
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
            WebCallResult res = new WebCallResult();

            string strUrl = _homeServer.BaseUrl + "policies/" + pObjectId;

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, _homeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XMLElement == null || res.XMLElement.HasElements == false)
            {
                res.Success = false;
                res.ErrorText = "No XML elements returned from policy fetch";
                return res;
            }

            foreach (var oElement in res.XMLElement.Elements())
            {
                _homeServer.SafeXMLFetch(this, oElement);
            }

            res.Success = true;
            return res;
        }


        /// DELETE not yet supported
        /// <summary>
        /// Remove policy definition from Connection
        /// </summary>
        /// <returns></returns>
        //public WebCallResult Delete()
        //{
        //    return DeletePolicy(this._homeServer, this.ObjectId);
        //}

        #endregion


        #region Static Methods


        ///NOTE: ADD and DELETE are not yet supported for policies
        /// <summary>
        /// Allows for the creation of a new policy on the Connection server directory.  the UserObjectId and RoleObjectId are required for 
        /// all policies, the targetVMSObjectId is required only for the Greetings Administrator role to identify the call handler the user 
        /// is being added as an owner for.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the policy is being added.
        /// </param>
        /// <param name="pRoleObjectId">
        /// The role to assign as part of the policy - required.
        /// </param>
        /// <param name="pUserObjectId">
        /// The user the policy is being created for - required.
        /// </param>
        /// <param name="pTargetVmsObjectId">
        /// Optional target VMS object id - the Greetings Adminsistrator role requires a call handler ObjectId here or an error will be thrown.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        //public static WebCallResult AddPolicy(ConnectionServer pConnectionServer,
        //                                            string pUserObjectId,
        //                                            string pRoleObjectId,
        //                                            string pTargetVmsObjectId = "")
        //{
        //    WebCallResult res = new WebCallResult();
        //    res.Success = false;

        //    if (pConnectionServer == null)
        //    {
        //        res.ErrorText = "Null ConnectionServer referenced passed to AddPolicy";
        //        return res;
        //    }

        //    //make sure that something is passed in for the 2 required params - the extension is optional.
        //    if (String.IsNullOrEmpty(pUserObjectId) || string.IsNullOrEmpty(pRoleObjectId))
        //    {
        //        res.ErrorText = "Empty value passed for one or more required parameters in AddPolicy on ConnectionServer.cs";
        //        return res;
        //    }

        //    string strBody = "<Policy>";
        //    strBody += string.Format("<{0}>{1}</{0}>", "UserObjectId", pUserObjectId);
        //    strBody += string.Format("<{0}>{1}</{0}>", "RoleObjectId", pRoleObjectId);

        //    if (!string.IsNullOrEmpty(pTargetVmsObjectId))
        //    {
        //        strBody += string.Format("<{0}>{1}</{0}>", "TargetVmsObjectId", pTargetVmsObjectId);
        //    }

        //    strBody += "</Policy>";

        //    res = HTTPFunctions.GetCUPIResponse(pConnectionServer.BaseUrl + "policies", MethodType.POST,pConnectionServer,strBody);

        //    //if the call went through then the ObjectId will be returned in the URI form.
        //    if (res.Success)
        //    {
        //        if (res.ResponseText.Contains(@"/vmrest/policies/"))
        //        {
        //            res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/policies/", "").Trim();
        //        }
        //    }

        //    return res;
        //}


        /// NOT SUPPORTED YET
        /// <summary>
        /// Delete a handler from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handler is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the handler in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        //public static WebCallResult DeletePolicy(ConnectionServer pConnectionServer, string pObjectId)
        //{
        //    if (pConnectionServer == null)
        //    {
        //        WebCallResult res = new WebCallResult();
        //        res.ErrorText = "Null ConnectionServer referenced passed to DeletePolicy";
        //        return res;
        //    }

        //    return HTTPFunctions.GetCUPIResponse(pConnectionServer.BaseUrl + "policies/" + pObjectId,MethodType.DELETE,pConnectionServer, "");
        //}


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
        /// The list of handlers returned from the CUPI call (if any) is returned as a generic list of CallHAndler class instances via this out param.  
        /// If no handlers are  found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(UserObjectId is 0d84fee3-8680-4bd2-aa81-49e32921299b)"
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

            string strUrl = pConnectionServer.BaseUrl + "policies";

            //the spaces get "escaped out" in the HTTPFunctions class call at a lower level, don't worry about it here.
            //Tack on all the search/query/page clauses here if any are passed in.  If an empty string is passed in account
            //for it here.
            for (int iCounter = 0; iCounter < pClauses.Length; iCounter++)
            {
                if (pClauses[iCounter].Length == 0)
                {
                    continue;
                }

                //if it's the first param seperate the clause from the URL with a ?, otherwise append compound clauses 
                //seperated by &
                if (iCounter == 0)
                {
                    strUrl += "?";
                }
                else
                {
                    strUrl += "&";
                }
                strUrl += pClauses[iCounter];
            }

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty, that's legal
            if (res.XMLElement == null || res.XMLElement.HasElements == false)
            {
                pPolicies= new List<Policy>();
                return res;
            }

            pPolicies = GetPoliciesFromXElements(pConnectionServer, res.XMLElement);
            return res;

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
        /// The list of handlers returned from the CUPI call (if any) is returned as a generic list of CallHAndler class instances via this out param.  
        /// If no handlers are  found NULL is returned for this parameter.
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
        /// The list of handlers returned from the CUPI call (if any) is returned as a generic list of CallHAndler class instances via this out param.  
        /// If no handlers are  found NULL is returned for this parameter.
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



        //Helper function to take an XML blob returned from the REST interface for a policy (or policies) return and convert it into an generic
        //list of Policy class objects. 
        private static List<Policy> GetPoliciesFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {
            List<Policy> oPolicyList = new List<Policy>();

            //pull out a set of XMLElements for each CallHandler object returned using the power of LINQ
            var policies = from e in pXElement.Elements()
                           where e.Name.LocalName == "Policy"
                           select e;

            //for each policy returned in the list of policies from the XML, construct a Policy object using the elements associated with that 
            //policy.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlPolicy in policies)
            {
                Policy oPolicy = new Policy(pConnectionServer);
                foreach (XElement oElement in oXmlPolicy.Elements())
                {
                    //adds the XML property to the Policy object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXMLFetch(oPolicy, oElement);
                }

                //add the fully populated Policy object to the list that will be returned to the calling routine.
                oPolicyList.Add(oPolicy);
            }

            return oPolicyList;
        }
        #endregion
    }
}
