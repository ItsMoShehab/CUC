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
    /// Read only class for fetching the list of supported RTP codecs from a Connection server.
    /// </summary>
    public class RtpCodecDef
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires the ConnectionServer object where the RtpCodec is defined.  Optionally can pass the ObjectId string
        /// and have that specific codec definition loaded.
        /// </summary>
        public RtpCodecDef(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to RtpCodecDef construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            WebCallResult res = GetRtpCodecDef(pObjectId);
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Failed to fetch PortGroupTemplate by ObjectId={0}", pObjectId));
            }
        }

        /// <summary>
        /// general constructor for Json parsing libararies
        /// </summary>
        public RtpCodecDef()
        {

        }

        #endregion


        #region Fields and Properties 

        public ConnectionServer HomeServer;

        #endregion


        #region RtpCodecDef Properties

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string DisplayName { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the description of the objectId
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("RtpCodecDef:{0}, ObjectId:{1}", DisplayName, ObjectId);
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
        private WebCallResult GetRtpCodecDef(string pObjectId)
        {
            string strUrl = HomeServer.BaseUrl + "rtpcodecdefs/" + pObjectId;

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
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

        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of RtpCodec definitions from Connection via HTTP
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the templates are being fetched from.
        /// </param>
        /// <param name="pCodecDefs">
        /// The list of rtp codecs defined on the server (typically only 5)
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRtpCodecDefs(ConnectionServer pConnectionServer, out List<RtpCodecDef> pCodecDefs)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCodecDefs = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetRtpCodecDefs";
                return res;
            }

            string strUrl = ConnectionServer.AddClausesToUri(pConnectionServer.BaseUrl + "rtpcodecdefs");

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's not an error, just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pCodecDefs = new List<RtpCodecDef>();
                return res;
            }

            pCodecDefs = pConnectionServer.GetObjectsFromJson<RtpCodecDef>(res.ResponseText);

            //special case - Json.Net always creates an object even when there's no data for it.
            if (pCodecDefs == null || (pCodecDefs.Count == 1 && string.IsNullOrEmpty(pCodecDefs[0].ObjectId)))
            {
                pCodecDefs = new List<RtpCodecDef>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pCodecDefs)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }

        #endregion
    }
}
