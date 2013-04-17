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
    /// The SMPP Provider class is used only to provide an interface for user to select which provider to use when creating new SMS notification 
    /// devices. 
    /// </summary>
    public class SmppProvider
    {
        #region Fields and Properties 

        public string TextName { get; set; }
        public string ObjectId { get; set; }
        public bool IsEnabled { get; set; }

        #endregion


        #region Constructors

        //constructor
        public SmppProvider(ConnectionServer pConnectionServer, string pObjectId="")
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
                throw new Exception("Failed to find SmppProvider in SmppConstructor:"+res.ToString());
            }
        }

        /// <summary>
        /// General constructor for Json parsing libary
        /// </summary>
        public SmppProvider()
        {
        }

        #endregion


        #region Methods

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
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false )
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
        /// Gets the list of all SmppProviders and resturns them as a generic list of SmppProvider objects.  This
        /// list can be used for providing drop down list selection for notification device creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the SmppProviders should be pulled from
        /// </param>
        /// <param name="pSMppProviders">
        /// Out parameter that is used to return the list of SmppProvider objects defined on Connection - the list may be empty
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
        public static WebCallResult GetSmppProviders(ConnectionServer pConnectionServer, out List<SmppProvider> pSMppProviders, 
            int pPageNumber = 1, int pRowsPerPage = 20)
        {
            WebCallResult res;
            pSMppProviders = null;

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetSmppProviders";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "smppproviders", "pageNumber=" + pPageNumber, 
                "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }
            
            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's not an error, just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pSMppProviders = new List<SmppProvider>();
                return res;
            }

            pSMppProviders = HTTPFunctions.GetObjectsFromJson<SmppProvider>(res.ResponseText);

            if (pSMppProviders == null)
            {
                pSMppProviders = new List<SmppProvider>();
                return res;
            }

            return res;
        }


        #endregion

    }
}
