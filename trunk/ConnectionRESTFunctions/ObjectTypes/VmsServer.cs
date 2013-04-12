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

        #region Fields and Properties

        public string ServerName { get; set; }
        public string VmsServerObjectId { get; set; }
        public string IpAddress { get; set; }
        public int ClusterMemberId { get; set; }
        public int ServerState { get; set; }
        public int ServerDisplayState { get; set; }
        public bool SubToPerformReplicationRole { get; set; }

        public ConnectionServer HomeServer;

        #endregion


        #region Constructors

        public VmsServer(ConnectionServer pConnectionServer, string pObjectId="")
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
                throw new Exception(string.Format("Failed to fetch VmsServer by ObjectId={0}", pObjectId));
            }
        }

        /// <summary>
        /// Generic constructor for JSON library
        /// </summary>
        public VmsServer()
        {
            
        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the description of the objectId
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("VmsServer:{0}, ObjectId:{1}", ServerName, VmsServerObjectId);
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
        /// string containing all the name value pairs defined in the call handler object instance.
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
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this);
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

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "vmsservers");

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
                pServers = new List<VmsServer>();
                return res;
            }

            pServers = HTTPFunctions.GetObjectsFromJson<VmsServer>(res.ResponseText);

            if (pServers == null)
            {
                pServers = new List<VmsServer>();
                return res;
            }

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
