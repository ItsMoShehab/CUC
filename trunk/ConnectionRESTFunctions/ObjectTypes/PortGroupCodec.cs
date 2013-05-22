#region Legal Disclaimer

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
    /// Class that allows for the fetching, creating and deleting of codecs associated with port groups.
    /// </summary>
    public class PortGroupCodec
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires ConnectionServer where the port group is homed - the PortGroup ID that the codec is homed on.
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer that the port group is homed on.
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group that the portgroup codec is associated with
        /// </param>
        /// <param name="pObjectId">
        /// Unique identifier for the port group codec - if passed the class will be loaded with that PortGroupCodec
        /// object.
        /// </param>
        public PortGroupCodec(ConnectionServer pConnectionServer, string pPortGroupObjectId, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to PortGroupCodec construtor");
            }

            if (string.IsNullOrEmpty(pPortGroupObjectId))
            {
                throw new ArgumentException("Empty port group objectId passed to PortGroupCodec construtor");
            }

            MediaPortGroupObjectId = pPortGroupObjectId;
            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            WebCallResult res = GetPortGroupCodec(pObjectId);
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Failed to fetch PortGroupCodec by ObjectId={0}", pObjectId));
            }
        }

        /// <summary>
        /// general constructor for Json parsing libararies
        /// </summary>
        public PortGroupCodec()
        {

        }

        #endregion

        
        #region Fields and Properties
        
        public ConnectionServer HomeServer;

        #endregion


        #region PortGroupCodec Properties

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string MediaPortGroupObjectId { get; private set; }

        [JsonProperty]
        public string RtpCodecDefObjectId { get; private set; }

        [JsonProperty]
        public string RtpCodecDefDisplayName { get; private set; }

        [JsonProperty]
        public int PreferredPacketSizeMs { get; private set; }

        [JsonProperty]
        public int Preference { get; private set; }

        #endregion

     
        #region Instance Methods

        /// <summary>
        /// Returns a string with the description of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("PortGroupCodec:{0}, ObjectId:{1}", this.RtpCodecDefDisplayName, ObjectId);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the PortGroupCodec object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            StringBuilder strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
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
        private WebCallResult GetPortGroupCodec(string pObjectId)
        {
            string strUrl = HomeServer.BaseUrl + "portgroups/"+MediaPortGroupObjectId+"/portgroupcodecs/" + pObjectId;

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

        /// <summary>
        /// DELETE a port group codec from a port group
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeletePortGroupCodec(HomeServer, MediaPortGroupObjectId, ObjectId);
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of port group codecs from Connection via HTTP
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the templates are being fetched from.
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group to fetch codecs for
        /// </param>
        /// <param name="pCodecs">
        /// The list of port group codecs is returned via this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPortGroupCodecs(ConnectionServer pConnectionServer, string pPortGroupObjectId, out List<PortGroupCodec> pCodecs)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCodecs = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPortGroupCodecs";
                return res;
            }

            string strUrl = ConnectionServer.AddClausesToUri(pConnectionServer.BaseUrl + "portgroups/" + pPortGroupObjectId+"/portgroupcodecs");

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
                pCodecs = new List<PortGroupCodec>();
                return res;
            }

            pCodecs = pConnectionServer.GetObjectsFromJson<PortGroupCodec>(res.ResponseText);

            if (pCodecs == null)
            {
                pCodecs = new List<PortGroupCodec>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pCodecs)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// Adds a new port group codec to a port group
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the port group codec to
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group to add the codec to
        /// </param>
        /// <param name="pRtpCodecDefObjectId">
        /// RtpCodec to add
        /// </param>
        /// <param name="pPreferredPacketSizeMs">
        /// Packet size to use - 10, 20 or 30.
        /// </param>
        /// <param name="pPreference">
        /// Lower values mean this codec is more preferred than higher values for other codecs - typically start with 0 then go to 1, then 
        /// 2 etc... only has meaning if the port group is SIP.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddPortGroupCodec(ConnectionServer pConnectionServer, string pPortGroupObjectId, string pRtpCodecDefObjectId, 
            int pPreferredPacketSizeMs, int pPreference)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddPortGroupCodec";
                return res;
            }

            //make sure that something is passed in for the required param
            if (String.IsNullOrEmpty(pPortGroupObjectId) | string.IsNullOrEmpty(pRtpCodecDefObjectId))
            {
                res.ErrorText = "Empty value passed for port group ObjectId or CodecDefObjectId in AddPortGroupCodec";
                return res;
            }

            string strBody = "<PortGroupCodec>";

            //tack on the property value pair with appropriate tags
            strBody += string.Format("<MediaPortGroupObjectId>{0}</MediaPortGroupObjectId>", pPortGroupObjectId);
            strBody += string.Format("<PreferredPacketSizeMs>{0}</PreferredPacketSizeMs>", pPreferredPacketSizeMs);
            strBody += string.Format("<RtpCodecDefObjectId>{0}</RtpCodecDefObjectId>", pRtpCodecDefObjectId);
            strBody += string.Format("<Preference>{0}</Preference>", pPreference);

            strBody += "</PortGroupCodec>";

            res = pConnectionServer.GetCupiResponse(string.Format("{0}portgroups/{1}/portgroupcodecs", pConnectionServer.BaseUrl, 
                    pPortGroupObjectId),MethodType.POST, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = string.Format("/vmrest/portgroups/{0}/portgroupcodecs/", pPortGroupObjectId);
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Adds a new port group codec to a port group
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the port group codec to
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group to add the codec to
        /// </param>
        /// <param name="pRtpCodecDefObjectId">
        /// RtpCodec to add
        /// </param>
        /// <param name="pPreferredPacketSizeMs">
        /// Packet size to use - 10, 20 or 30.
        /// </param>
        /// <param name="pPreference">
        /// Lower values mean this codec is more preferred than higher values for other codecs - typically start with 0 then go to 1, then 
        /// 2 etc... only has meaning if the port group is SIP.
        /// </param>
        /// <param name="pPortGroupCodec">
        /// Instance of the portGroupCodec class filled in with the newly created codec definition is passed back on this out param.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddPortGroupCodec(ConnectionServer pConnectionServer, string pPortGroupObjectId,
                                                      string pRtpCodecDefObjectId,
                                                      int pPreferredPacketSizeMs,
                                                      int pPreference,
                                                      out PortGroupCodec pPortGroupCodec)
        {
            pPortGroupCodec = null;
            WebCallResult res = AddPortGroupCodec(pConnectionServer, pPortGroupObjectId, pRtpCodecDefObjectId, pPreferredPacketSizeMs, pPreference);

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                pPortGroupCodec = new PortGroupCodec(pConnectionServer, pPortGroupObjectId, res.ReturnedObjectId);
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.ErrorText = ex.ToString();
                return res;
            }

            return res;
        }


        /// <summary>
        /// DELETE a codec from a port group.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the port group is homed.
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group that owns the codec to be deleted
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the port group codec in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeletePortGroupCodec(ConnectionServer pConnectionServer, string pPortGroupObjectId, string pObjectId)
        {
            WebCallResult res = new WebCallResult {Success = false};
            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to DeletePhoneSystem";
                return res;
            }

            if (string.IsNullOrEmpty(pPortGroupObjectId) || string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "PortGroupObjectId or ObjectId values passed in are blank";
                return res;
            }

            return pConnectionServer.GetCupiResponse(string.Format("{0}portgroups/{1}/portgroupcodecs/{2}", pConnectionServer.BaseUrl, 
                    pPortGroupObjectId, pObjectId),MethodType.DELETE,  "");
        }


        #endregion

    }
}
