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
    /// The GlobalUser class holds data about a global user - these are users that are limited to data replicated around the network (i.e. very limited).
    /// You cannot add/delete or edit global users - only search them (alias/extension/first/last/display name etc...) to find their home server which 
    /// will then let you connection to that server for more robust API access to that user account. 
    /// </summary>
    public class GlobalUser
    {
        #region Fields and Properties

        //reference to the ConnectionServer object used to create this user instance.
        internal ConnectionServer HomeServer;

        #endregion


        #region Constructors

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
        public GlobalUser(ConnectionServer pConnectionServer, string pObjectId = "", string pAlias = "")
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
                throw new Exception(string.Format("User not found in GlobalUser constructor using Alias={0} and/or ObjectId={1}\n\rError={2}"
                                                , pAlias, pObjectId,res.ErrorText));
            }
        }

       
        #endregion


        #region GlobalUser Properties

        //The names of the properties must match exactly the tags in XML for them including case - the routine that deserializes data from XML into the 
        //objects requires this to match them up.

        public string Alias { get; private set; }

        public string DisplayName { get; private set; }

        public string AltFirstName { get; private set; }

        public string AltLastName { get; private set; }

        public string City { get; private set; }

        public string Department { get; private set; }

        /// <summary>
        /// Primary extension of the user.
        /// </summary>
        public string DtmfAccessId { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        /// <summary>
        /// A flag indicating whether Cisco Unity Connection should list the subscriber in the phone directory for outside callers.
        /// This does not affect the ability of other users from finding them when addressing messages.
        /// </summary>
        public bool ListInDirectory { get; private set; }

        public string LocationObjectId { get; private set; }

        public bool IsTemplate { get; private set; }

        /// <summary>
        /// Unique identifier for the user 
        /// </summary>
        public string ObjectId { get; private set; }

        /// <summary>
        /// The unique identifier of the Partition to which the DtmfAccessId is assigned
        /// </summary>
        public string PartitionObjectId { get; private set; }

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
        public static WebCallResult GetUsers(ConnectionServer pConnectionServer, out List<GlobalUser> pUsers, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pUsers = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetUsers";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + "globalusers";

            //the spaces get "escaped out" in the HTTPFunctions class call at a lower level, don't worry about it here.
            //Tack on all the search/query/page clauses here if any are passed in.  If an empty string is passed in account
            //for it here.
            if (pClauses != null)
            {
                for (int iCounter = 0; iCounter < pClauses.Length; iCounter++)
                {
                    if (string.IsNullOrEmpty(pClauses[iCounter]))
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
            }
            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty, that's legal
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                pUsers= new List<GlobalUser>();
                return res;
            }

            pUsers = GetUsersFromXElements(pConnectionServer, res.XmlElement);
            return res;

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
        public static WebCallResult GetUser(out GlobalUser pUser, ConnectionServer pConnectionServer, string pObjectId, string pAlias = "")
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
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch user in GetUser:" + ex.Message;
                return res;
            }

            return res;
        }


    
        //Helper function to take an XML blob returned from the REST interface for a user (or users) return and convert it into an generic
        //list of GlobalUser class objects. 
        private static List<GlobalUser> GetUsersFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {
            List<GlobalUser> oUserList = new List<GlobalUser>();

            //pulls all the users returned in the XML as set of elements using the power of LINQ
            var users = from e in pXElement.Elements()
                        where e.Name.LocalName == "GlobalUser"
                        select e;

            //for each user returned in the list of users from the XML, construct a GlobalUser object using the elements associated with that 
            //user.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlUser in users)
            {
                GlobalUser oUser = new GlobalUser(pConnectionServer);
                foreach (XElement oElement in oXmlUser.Elements())
                {
                    //adds the XML property to the GlobalUser object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXmlFetch(oUser, oElement);
                }

                //add the fully populated GlobalUser object to the list that will be returned to the calling routine.
                oUserList.Add(oUser);
            }

            return oUserList;
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
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                res.Success = false;
                return res;
            }

            //in the case of a single base user fetch construct, the list of elements is the full list of properties for the user, but it's nexted in 
            //a "uers" sub element, not at the top level as a full user fetch is - so we have to go another level deep here.
            //Call the same SafeXMLFetch routine for each to let the full user class instance "drive" the fetching of data
            //from the XML elements.
            foreach (XElement oElement in res.XmlElement.Elements())
            {
                HomeServer.SafeXmlFetch(this, oElement);
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
           string strUrl = string.Format("{0}globalusers?query=(Alias is {1})", HomeServer.BaseUrl, pAlias);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, HomeServer, "");

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