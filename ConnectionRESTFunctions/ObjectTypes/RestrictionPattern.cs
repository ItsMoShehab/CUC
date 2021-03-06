﻿#region Legal Disclaimer

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
    /// Read only class that lets you fetch restriction table patternss
    /// </summary>
    [Serializable]
    public class RestrictionPattern
    {

        #region Constructors and Destructors


        /// <summary>
        /// Pass in the objectId of the restriction table to load, it's display name (which should be unique) or neither and the constructor
        /// will create an uninitalized instance - this is used when constructing lists of restriction tables via the static method.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of the Connection Server class we are doing queries against.
        /// </param>
        /// <param name="pRestrictionTableObjectId">
        /// ObjectId of the restriction table that owns the pattern
        /// </param>
        /// <param name="pObjectId">
        /// Optional ObjecTId of the restriction table to load.
        /// </param>
        public RestrictionPattern(ConnectionServerRest pConnectionServer, string pRestrictionTableObjectId, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to RestrictionPattern constructor.");
            }

            if (string.IsNullOrEmpty(pRestrictionTableObjectId))
            {
                throw new ArgumentException("Empty restriction table ObjectId passed to RestrictionPattern constructor.");
            }

            HomeServer = pConnectionServer;
            RestrictionTableObjectId = pRestrictionTableObjectId;

            //if the user passed in a specific ObjectId then go load that pattern up, otherwise just return an empty instance.
            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            ObjectId = pObjectId;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetRestrictionTablePattern(pObjectId);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("RestrictionPattern not found in RestrictionPattern constructor " +
                                                                         "using ObjectId={0}\n\r{1}", pObjectId, res.ErrorText));
            }
        }

        /// <summary>
        /// General constructor for Json parsing library
        /// </summary>
        public RestrictionPattern()
        {
        }


        #endregion


        #region Fields and Properties

        public ConnectionServerRest HomeServer { get; private set; }

        #endregion


        #region RestrictionPattern Properties


        [JsonProperty]
        public bool Blocked { get; private set; }

        [JsonProperty]
        public string NumberPattern { get; private set; }

        [JsonProperty]
        public string RestrictionTableObjectId { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public int SequenceNumber { get; private set; }

        #endregion


        #region Instance Methods

        public override string ToString()
        {
            return string.Format("Restriction pattern: {0}, blocked={1}", NumberPattern, Blocked);
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

            PropertyInfo[] oProps = GetType().GetProperties();

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
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetRestrictionTablePattern(string pObjectId)
        {
            //when fetching a RT use the query construct in both cases so the XML parsing is identical
            string strUrl = string.Format("{0}restrictiontables/{1}/restrictionpatterns/{2}", HomeServer.BaseUrl,RestrictionTableObjectId, pObjectId);

            //issue the command to the CUPI interface
            return HomeServer.FillObjectWithRestGetResults(strUrl,this);
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all restriction patterns and resturns them as a generic list of RestrictionPattern objects.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the templates should be pulled from
        /// </param>
        /// <param name="pRestrictionTableObjectId">
        /// The objectId of the restriction table to fetch patterns for.
        /// </param>
        /// <param name="pRestrictionPatterns">
        /// Out parameter that is used to return the list of RestrictionTable objects defined on Connection - there must be at least one.
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
        public static WebCallResult GetRestrictionPatterns(ConnectionServerRest pConnectionServer, string pRestrictionTableObjectId,
            out List<RestrictionPattern> pRestrictionPatterns, int pPageNumber = 1, int pRowsPerPage = 20)
        {
            WebCallResult res;
            pRestrictionPatterns = new List<RestrictionPattern>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetRestrictionPatterns";
                return res;
            }

            if (string.IsNullOrEmpty(pRestrictionTableObjectId))
            {
                res = new WebCallResult();
                res.ErrorText = "Empty restriction table objectId passed to GetRestrictionPatterns";
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(string.Format("{0}restrictiontables/{1}/restrictionpatterns", pConnectionServer.BaseUrl, 
                pRestrictionTableObjectId), "pageNumber=" + pPageNumber, "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
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

            //no error, just return an empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                return res;
            }

            pRestrictionPatterns = pConnectionServer.GetObjectsFromJson<RestrictionPattern>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pRestrictionPatterns)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.RestrictionTableObjectId = pRestrictionTableObjectId;
            }

            return res;
        }

        #endregion

    }
}
