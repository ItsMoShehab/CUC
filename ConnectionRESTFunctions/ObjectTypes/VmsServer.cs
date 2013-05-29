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
    /// Read only class for fetching VMSServer(s) from a Connection server.  There will only ever be 1 or 2 of these in the 
    /// Unity Connection directory depending on if it's a cluster or a single server installation
    /// </summary>
    public class VmsServer
    {

        #region Constructors and Destructors

        /// <summary>
        /// Constructor requires a ConnectionServer object for where the VMSServer is hosted.  You can optionally pass in an ObjectID 
        /// to load a specific VMSserver object data directly.
        /// </summary>
        public VmsServer(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to VmsServer construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            WebCallResult res = GetVmsServer(pObjectId);
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Failed to fetch VmsServer by ObjectId={0}", pObjectId));
            }
        }

        /// <summary>
        /// Generic constructor for JSON library
        /// </summary>
        public VmsServer()
        {

        }

        #endregion


        #region Fields and Properties

        public ConnectionServer HomeServer;

        #endregion


        #region VmsServer Properties

        /// <summary>
        /// 0  - Primary, 1 – First secondary
        /// </summary>
        [JsonProperty]
        public ClusterMemberId ClusterMemberId { get; private set; }

        [JsonProperty]
        public string HostName { get; private set; }

        [JsonProperty]
        public string IpAddress { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string ServerName { get; private set; }

        /// <summary>
        ///Value: 0 Name: Pri_Init
        ///Value: 1 Name: Pri_Active
        ///Value: 2 Name: Pri_Act_Secondary
        ///Value: 3 Name: Pri_Idle
        ///Value: 4 Name: Pri_Failover
        ///Value: 5 Name: Pri_Takeover
        ///Value: 6 Name: Pri_SBR
        ///Value: 7 Name: Sec_Init
        ///Value: 8 Name: Sec_Active
        ///Value: 9 Name: Sec_Act_Primary
        ///Value: 10 Name: Sec_Idle
        ///Value: 11 Name: Sec_Takeover
        ///Value: 12 Name: Sec_Failover
        ///Value: 13 Name: Sec_SBR
        ///Value: 14 Name: Db_Sync
        ///Value: 15 Name: Set_Peer_Idle
        ///Value: 16 Name: Undefined
        ///Value: 17 Name: Pri_Active_Disconnected
        ///Value: 18 Name: Pri_Connecting
        ///Value: 19 Name: Pri_Choose_Role
        ///Value: 20 Name: Pri_Single_Server
        ///Value: 21 Name: Sec_Act_Primary_Disconnected
        ///Value: 22 Name: Sec_Connecting
        ///Value: 23 Name: Sec_Choose_Role
        ///Value: 24 Name: Shutdown
        /// </summary>
        [JsonProperty]
        public ServerState ServerState { get; private set; }

        /// <summary>
        ///Value: 1 Name: DOWN
        ///Value: 2 Name: INITIALIZING
        ///Value: 3 Name: PRIMARY
        ///Value: 4 Name: SECONDARY
        ///Value: 5 Name: IDLE
        ///Value: 6 Name: IN_DB_SYNC
        ///Value: 7 Name: IN_SBR
        /// </summary>
        [JsonProperty]
        public ServerDisplayState ServerDisplayState { get; private set; }

        [JsonProperty]
        public bool SubToPerformReplicationRole { get; private set; }

        [JsonProperty]
        public string VmsServerObjectId { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the description of the objectId
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("VmsServer:{0}, ObjectId:{1}, IP={2}", ServerName, VmsServerObjectId,IpAddress);
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
        /// string containing all the name value pairs defined in the object instance.
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
        private WebCallResult GetVmsServer(string pObjectId)
        {
            string strUrl = HomeServer.BaseUrl + "vmsservers/" + pObjectId;

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
        /// This function allows for a GET of VMS Servers from Connection via HTTP - typically there are only one defined
        /// on the server so there's no filter clauses or the like supported to keep it simple.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the servers are being fetched from.
        /// </param>
        /// <param name="pServers">
        /// The list of VMSServers is returned via this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetVmsServers(ConnectionServer pConnectionServer, out List<VmsServer> pServers)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pServers = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetVmsServers";
                return res;
            }

            string strUrl = ConnectionServer.AddClausesToUri(pConnectionServer.BaseUrl + "vmsservers");

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's an error - a zero count is also not valid since there must always be one.
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pServers = new List<VmsServer>();
                res.Success = false;
                return res;
            }

            pServers = pConnectionServer.GetObjectsFromJson<VmsServer>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pServers)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }

        #endregion
    }
}
