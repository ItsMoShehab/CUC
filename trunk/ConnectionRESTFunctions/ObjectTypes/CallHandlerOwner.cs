using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Static class for fetching the owner(s) of a system call handler.  As of 10.5 a call handler can have users or 
    /// public distribution lists as owners.  Prior releases are limited to users only.
    /// A system call handler doesn't need an owner, it's optional.  Any number of owners may be added.
    /// </summary>
    [Serializable]
    public class CallHandlerOwner
    {
        #region  CallHandlerOwner Properties

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string UserObjectId { get; private set; }

        [JsonProperty]
        public string DistributionListObjectId { get; private set; }

        [JsonProperty]
        public string TargetHandlerObjectId { get; private set; }

        #endregion


        public override string ToString()
        {
            if (string.IsNullOrEmpty(DistributionListObjectId))
            {
                return string.Format("Call handler owner for call handler id=[{0}], user id=[{1}]",ObjectId,UserObjectId);
            }
            return string.Format("Call handler owner for call handler id=[{0}], distribution list id=[{1}]", ObjectId, DistributionListObjectId);
        }

        /// <summary>
        /// Returns a list of CallHandlerOwner objects representing the owners (users or public distribution lists) for a particular 
        /// system call handler. 
        /// A call handler may have no owners in which case an empty list is returned.
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pCallHandlerObjectId">
        /// Call handler to fetch owner information for.
        /// </param>
        /// <param name="pCallHandlerOwners">
        /// List of CallHandlerOwner objects.  Can represent users or public distribution lists.
        /// </param>
        /// <param name="pPageNumber"></param>
        /// <param name="pRowsPerPage"></param>
        /// <returns>
        /// Instance of the WebCallResult object with details of the call and results.
        /// </returns>
        public static WebCallResult GetCallHandlerOwners(ConnectionServerRest pConnectionServer, string pCallHandlerObjectId,
            out List<CallHandlerOwner> pCallHandlerOwners,int pPageNumber = 1, int pRowsPerPage = 20)
        {
            List<string> temp = new List<string>();
            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            return GetCallHandlerOwners(pConnectionServer, pCallHandlerObjectId, out pCallHandlerOwners, temp.ToArray());
        }


        /// <summary>
        /// Returns a list of CallHandlerOwner objects representing the owners (users or public distribution lists) for a particular 
        /// system call handler. 
        /// A call handler may have no owners in which case an empty list is returned.
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pCallHandlerObjectId">
        /// Call handler to fetch owner information for.
        /// </param>
        /// <param name="pCallHandlerOwners">
        /// List of CallHandlerOwner objects.  Can represent users or public distribution lists.
        /// </param>
        /// <param name="pClauses">
        /// Optional search clauses such as "query=(name is testname)"
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult object with details of the call and results.
        /// </returns>
        public static WebCallResult GetCallHandlerOwners(ConnectionServerRest pConnectionServer, string pCallHandlerObjectId,
            out List<CallHandlerOwner> pCallHandlerOwners, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCallHandlerOwners = new List<CallHandlerOwner>();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetCallHandlerOwners";
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "handlers/callhandlers/"+pCallHandlerObjectId+"/callhandlerowners", 
                pClauses);

            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                res.ErrorText = "Empty response recieved";

                return res;
            }

            //not a failure, just return an empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                return res;
            }

            pCallHandlerOwners = pConnectionServer.GetObjectsFromJson<CallHandlerOwner>(res.ResponseText, "CallhandlerOwner");

            if (pCallHandlerOwners == null)
            {
                pCallHandlerOwners = new List<CallHandlerOwner>();
                res.ErrorText = "Could not parse JSON into call handler owner objects:" + res.ResponseText;
                res.Success = false;
            }
            return res;
        }
    }


}
