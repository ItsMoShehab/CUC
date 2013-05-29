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
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The SMPP Provider class is used only to provide an interface for user to select which provider to use when creating new SMS notification 
    /// devices. 
    /// </summary>
    public class SmppProvider :IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires ConnectionServer where SNPP provider is homed.  Can optionally pass in an ObjectId to load that
        /// SMPP provider data.
        /// </summary>
        public SmppProvider(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to SmppProvider constructor");
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            WebCallResult res = GetSmppProvider(pConnectionServer, pObjectId);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res, "Failed to find SmppProvider in SmppConstructor:" + res.ToString());
            }
        }

        /// <summary>
        /// General constructor for Json parsing libary
        /// </summary>
        public SmppProvider()
        {
        }

        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return TextName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }


        #endregion


        #region SmppProvider Properties

        [JsonProperty]
        public string TextName { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public bool IsEnabled { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the SMPP provider
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", TextName, ObjectId);
        }

        /// <summary>
        /// Fills current instance of class with details of smpp provider for objectId passed in if found.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to query
        /// </param>
        /// <param name="pObjectId">
        /// Unique Id for search space to load
        /// </param>
        /// <returns>
        /// Instance of WebCallResult class
        /// </returns>
        private WebCallResult GetSmppProvider(ConnectionServer pConnectionServer, string pObjectId)
        {
            string strUrl = pConnectionServer.BaseUrl + "smppproviders/" + pObjectId;

            //issue the command to the CUPI interface
            WebCallResult res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false )
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
        /// Gets the list of all SmppProviders and resturns them as a generic list of SmppProvider objects.  This
        /// list can be used for providing drop down list selection for notification device creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the SmppProviders should be pulled from
        /// </param>
        /// <param name="pSmppProviders">
        /// Out parameter that is used to return the list of SmppProvider objects defined on Connection - the list may be empty
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetSmppProviders(ConnectionServer pConnectionServer, out List<SmppProvider> pSmppProviders,
            params string[] pClauses)
        {
            WebCallResult res;
            pSmppProviders = null;

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetSmppProviders";
                return res;
            }

            string strUrl = ConnectionServer.AddClausesToUri(pConnectionServer.BaseUrl + "smppproviders", pClauses);

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
                pSmppProviders = new List<SmppProvider>();
                return res;
            }

            pSmppProviders = pConnectionServer.GetObjectsFromJson<SmppProvider>(res.ResponseText);

            return res;
        }

        /// <summary>
        /// Gets the list of all SmppProviders and resturns them as a generic list of SmppProvider objects.  This
        /// list can be used for providing drop down list selection for notification device creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the SmppProviders should be pulled from
        /// </param>
        /// <param name="pSmppProviders">
        /// Out parameter that is used to return the list of SmppProvider objects defined on Connection - the list may be empty
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>        
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetSmppProviders(ConnectionServer pConnectionServer,
                                                     out List<SmppProvider> pSmppProviders,
                                                     int pPageNumber = 1, int pRowsPerPage = 20,
                                                     params string[] pClauses)
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

            return GetSmppProviders(pConnectionServer, out pSmppProviders, temp.ToArray());
        }



        #endregion

    }
}
