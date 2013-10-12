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
    [Serializable]
    public class RtpCodecDef
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires the ConnectionServer object where the RtpCodec is defined.  Optionally can pass the ObjectId string
        /// and have that specific codec definition loaded.
        /// </summary>
        public RtpCodecDef(ConnectionServerRest pConnectionServer, string pObjectId = "")
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

        public ConnectionServerRest HomeServer;

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

            return HomeServer.FillObjectWithRestGetResults(strUrl,this);
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
        public static WebCallResult GetRtpCodecDefs(ConnectionServerRest pConnectionServer, out List<RtpCodecDef> pCodecDefs)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCodecDefs = new List<RtpCodecDef>();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetRtpCodecDefs";
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "rtpcodecdefs");

            //issue the command to the CUPI interface
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
            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's not an error, just return an empty list
            if (res.ResponseText.Length<20 || res.TotalObjectCount == 0)
            {
                return res;
            }

            pCodecDefs = pConnectionServer.GetObjectsFromJson<RtpCodecDef>(res.ResponseText);

            if (pCodecDefs == null)
            {
                pCodecDefs = new List<RtpCodecDef>();
                res.ErrorText = "Could not parse JSON into RtpCodecDef objects:" + res.ResponseText;
                res.Success = false;
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
