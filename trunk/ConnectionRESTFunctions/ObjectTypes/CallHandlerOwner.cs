using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions.ObjectTypes
{
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
        public string RoleObjectId { get; private set; }

        [JsonProperty]
        public DateTime DateCreated { get; private set; }

        [JsonProperty]
        public string TargetHandlerObjectId { get; private set; }

        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DistributionListObjectId))
            {
                return string.Format("Call handler owner for call handler id=[{0}], user id=[{1}]",ObjectId,UserObjectId);
            }
            else
            {
                return string.Format("Call handler owner for call handler id=[{0}], distribution list id=[{1}]", ObjectId, DistributionListObjectId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pCallHandlerObjectId"></param>
        /// <param name="pCallHandlerOwners"></param>
        /// <param name="pPageNumber"></param>
        /// <param name="pRowsPerPage"></param>
        /// <returns></returns>
        public static WebCallResult GetCallHandlerOwners(ConnectionServerRest pConnectionServer, string pCallHandlerObjectId,
            out List<CallHandlerOwner> pCallHandlerOwners,int pPageNumber = 1, int pRowsPerPage = 20)
        {
            List<string> temp = new List<string>();
            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            return GetCallHandlerOwners(pConnectionServer, pCallHandlerObjectId, out pCallHandlerOwners, temp.ToArray());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pCallHandlerObjectId"></param>
        /// <param name="pCallHandlerOwners"></param>
        /// <param name="pClauses"></param>
        /// <returns></returns>
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
                return res;
            }

            return res;
        }
    }


}
