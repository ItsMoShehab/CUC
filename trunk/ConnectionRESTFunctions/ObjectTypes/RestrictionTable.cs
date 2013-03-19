#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{
    /// <summary>
    /// The restriction table class is used only to provide an interface for user to select restriction tables for assignment to COS 
    /// instances handlers.  
    /// </summary>
    public class RestrictionTable
    {
       
        #region Fields and Properties

        private readonly ConnectionServer _homeServer;

        public DateTime CreationTime { get; set; }
        public bool DefaultBlocked { get; set; }
        public string DisplayName { get; set; }
        public string LocationObjectId { get; set; }
        public string ObjectId { get; set; }
        public int MaxDigits { get; set; }
        public int MinDigits { get; set; }
        public bool Undeletable { get; set; }


        private List<RestrictionPattern> _restrictionPatterns;
        /// <summary>
        /// Lazy fetch for restriction patterns associated with a table - this needs to be implemented as a method instead of a 
        /// property so that if a grid is bound to the generic list of objects it doesn't "lazy fetch" it for display purposes resulting
        /// in needless data fetching
        /// </summary>
        /// <param name="pForceRefetchOfData">
        /// Pass as true to force the restriction patterns to be refetched even if they've already be populated earlier.
        /// </param>
        /// <returns>
        /// Generic list of RestrictionPattern objects associated with the restriction table.
        /// </returns>
        public List<RestrictionPattern> RestrictionPatterns(bool pForceRefetchOfData=false)
        {
            if (pForceRefetchOfData)
            {
                _restrictionPatterns = null;
            }

            if (_restrictionPatterns == null)
            {
                RestrictionPattern.GetRestrictionPatterns(this._homeServer, this.ObjectId, out _restrictionPatterns);
            }

            return _restrictionPatterns;
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Pass in the objectId of the restriction table to load, it's display name (which should be unique) or neither and the constructor
        /// will create an uninitalized instance - this is used when constructing lists of restriction tables via the static method.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of the Connection Server class we are doing queries against.
        /// </param>
        /// <param name="pObjectId">
        /// Optional ObjecTId of the restriction table to load.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name for the restriction table to load. 
        /// </param>
        public RestrictionTable(ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to RestrictionTable constructor.");
            }
            
            _homeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or display name then go load that handler up, otherwise just return an empty instance.
            if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pDisplayName))) return;

            ObjectId = pObjectId;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetRestrictionTable(pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new Exception(string.Format("RestrictionTable not found in RestrictionTable constructor using ObjectId={0} and DisplayName={1}\n\r{2}"
                                 , pObjectId, pDisplayName, res.ErrorText));
            }
            
        }


        #endregion


        #region Instance Methods

        public override string ToString()
        {
            return string.Format("Restriction table: {0} [{1}]", DisplayName, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the schedule object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the schedule object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            var strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Fills the current instance of RestrictionTable in with properties fetched from the server.  If both the display name and ObjectId
        /// parameters are provided, the ObjectId is used for the search.
        /// </summary>
        /// <param name="pObjectId">
        /// Unique GUID of the RT to fetch - can be blank if the display name is passed in.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to search on a RT by.  Can be blank if the ObjectId parameter is provided.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetRestrictionTable(string pObjectId, string pDisplayName = "")
        {
            string strObjectId = pObjectId;

            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdFromName(pDisplayName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult
                        {
                            Success = false,
                            ErrorText = "No restriction table found by name=" + pDisplayName
                        };
                }
            }

            string strUrl = string.Format("{0}restrictiontables/{1}", _homeServer.BaseUrl, strObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, _homeServer, "");

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

            //populate this call handler instance with data from the XML fetch
            foreach (XElement oElement in res.XmlElement.Elements())
            {
                _homeServer.SafeXmlFetch(this, oElement);
            }

            return res;
        }

        /// <summary>
        /// Fetch the ObjectId of a RT by it's name.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pName">
        /// Name of the RT to find
        /// </param>
        /// <returns>
        /// ObjectId of RT if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pName)
        {
            string strUrl = string.Format("{0}restrictiontables/?query=(DisplayName is {1})", _homeServer.BaseUrl, pName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, _homeServer, "");

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

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all restriction tables and resturns them as a generic list of RestrictionTable objects.  This
        /// list can be used for providing drop down list selection for COS assignment
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the templates should be pulled from
        /// </param>
        /// <param name="pRestrictionTables">
        /// Out parameter that is used to return the list of RestrictionTable objects defined on Connection - there must be at least one.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRestrictionTables(ConnectionServer pConnectionServer, out List<RestrictionTable> pRestrictionTables)
        {
            WebCallResult res;
            pRestrictionTables = null;

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetRestrictionTables";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + "restrictiontables";

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty, that's legal
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                pRestrictionTables = new List<RestrictionTable>();
                return res;
            }

            pRestrictionTables = GetRestrictionTablesFromXElements(pConnectionServer, res.XmlElement);
            return res;
        }


        //Helper function to take an XML blob returned from the REST interface for user RT returned and convert it into an generic
        //list of RestrictionTable class objects.  
        private static List<RestrictionTable> GetRestrictionTablesFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {

            List<RestrictionTable> oRtList = new List<RestrictionTable>();

            //Use LINQ to XML to create a list of RT objects in a single statement.
            var restrictionTable = from e in pXElement.Elements()
                                where e.Name.LocalName == "RestrictionTable"
                                select e;

            //for each object returned in the list from the XML, construct a class object using the elements associated with that 
            //object.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlRt in restrictionTable)
            {
                RestrictionTable oRt = new RestrictionTable(pConnectionServer);
                foreach (XElement oElement in oXmlRt.Elements())
                {
                    //adds the XML property to the object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXmlFetch(oRt, oElement);
                }

                //add the fully populated object to the list that will be returned to the calling routine.
                oRtList.Add(oRt);
            }
            
            return oRtList;
        }


        #endregion

    }
}
