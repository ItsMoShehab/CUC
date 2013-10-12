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
    /// The restriction table class is used only to provide an interface for user to select restriction tables for assignment to COS 
    /// instances.  
    /// </summary>
    [Serializable]
    public class RestrictionTable : IUnityDisplayInterface
    {

        #region Constructors and Destructors


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
        public RestrictionTable(ConnectionServerRest pConnectionServer, string pObjectId = "", string pDisplayName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to RestrictionTable constructor.");
            }

            HomeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or display name then go load that restriction table up, 
            //otherwise just return an empty instance.
            if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pDisplayName))) return;

            ObjectId = pObjectId;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetRestrictionTable(pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("RestrictionTable not found in RestrictionTable constructor using ObjectId={0} " +
                                                                         "and DisplayName={1}\n\r{2}", pObjectId, pDisplayName, res.ErrorText));
            }
        }

        /// <summary>
        /// General constructor for Json parsing library
        /// </summary>
        public RestrictionTable()
        {

        }

        #endregion

       
        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DisplayName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }


        public ConnectionServerRest HomeServer { get; private set; }

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
        public List<RestrictionPattern> RestrictionPatterns(bool pForceRefetchOfData = false)
        {
            if (pForceRefetchOfData)
            {
                _restrictionPatterns = null;
            }

            if (_restrictionPatterns == null)
            {
                RestrictionPattern.GetRestrictionPatterns(this.HomeServer, this.ObjectId, out _restrictionPatterns);
            }

            return _restrictionPatterns;
        }

        #endregion


        #region RestrictionTable Properties

        [JsonProperty]
        public DateTime CreationTime { get; private set; }

        [JsonProperty]
        public bool DefaultBlocked { get; private set; }

        [JsonProperty]
        public string DisplayName { get; private set; }

        [JsonProperty]
        public string LocationObjectId { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public int MaxDigits { get; private set; }

        [JsonProperty]
        public int MinDigits { get; private set; }

        [JsonProperty]
        public bool Undeletable { get; private set; }

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

            string strUrl = string.Format("{0}restrictiontables/{1}", HomeServer.BaseUrl, strObjectId);

            return HomeServer.FillObjectWithRestGetResults(strUrl,this);
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
            string strUrl = string.Format("{0}restrictiontables/?query=(DisplayName is {1})", HomeServer.BaseUrl, pName.UriSafe());

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false || res.TotalObjectCount ==0)
            {
                return "";
            }

            List<RestrictionTable> oTables = HomeServer.GetObjectsFromJson<RestrictionTable>(res.ResponseText);

            foreach (var oTable in oTables)
            {
                if (oTable.DisplayName.Equals(pName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oTable.ObjectId;
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
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>        
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter 
        /// at a time  are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>  
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRestrictionTables(ConnectionServerRest pConnectionServer, out List<RestrictionTable> pRestrictionTables,
            int pPageNumber = 1, int pRowsPerPage = 20, params string[] pClauses)
        {
            WebCallResult res;
            pRestrictionTables = new List<RestrictionTable>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetRestrictionTables";
                return res;
            }

            List<String> oParams;
            if (pClauses == null)
            {
                oParams = new List<string>();
            }
            else
            {
                oParams = pClauses.ToList();
            }

            oParams.Add("pageNumber=" + pPageNumber);
            oParams.Add("rowsPerPage=" + pRowsPerPage);

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "restrictiontables", oParams.ToArray());

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET,  "");

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

            pRestrictionTables = pConnectionServer.GetObjectsFromJson<RestrictionTable>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pRestrictionTables)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }

        #endregion

    }
}
