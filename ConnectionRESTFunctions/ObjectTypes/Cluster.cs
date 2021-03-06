﻿#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Read only class for fetching cluster (server) details - little different than most classes in that it includes references to server instances
    /// as opposed to instances of itself - one cluster instance needs to be created (no static methods) and 1 or 2 server(s) will be available in the 
    /// public Servers property (generic list) based on the cluster configuration.
    /// </summary>
    [Serializable]
    public class Cluster
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor for the Server class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        public Cluster(ConnectionServerRest pConnectionServer)
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to Cluster construtor");
            }

            HomeServer = pConnectionServer;

            //fetch the servers in the cluster (always 1 but can be 2)
            WebCallResult res = GetServers(HomeServer);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Cluster details not found in Cluster constructor\n\r{0}", res.ErrorText));
            }
        }

        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object used to create this object instance.
        public ConnectionServerRest HomeServer { get; private set; }

        private List<Server> _servers;
        public List<Server> Servers
        {
            get
            {
                var res = GetServers(HomeServer);
                if (res.Success == false)
                {
                    HomeServer.RaiseErrorEvent("Error fetching servers:"+res);
                    return null;
                }
                return _servers;
            }
        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and key of the server for each server in the cluster (1 or 2 servers)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strTemp="";
            foreach (var oServer in Servers)
            {
                strTemp += oServer + Environment.NewLine;
            }
            return strTemp;
        }


        /// <summary>
        /// Gets the list of all servers and resturns them as a generic list of Server objects.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that is being queried
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetServers(ConnectionServerRest pConnectionServer)
        {
            _servers = new List<Server>();
            
            string strUrl = pConnectionServer.BaseUrl + "cluster";

            //issue the command to the CUPI interface
            WebCallResult res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET,  "");

            if (res.Success == false)
            {
                return res;
            }

            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                res.ErrorText = "Empty response received";
                return res;
            }

            _servers = pConnectionServer.GetObjectsFromJson<Server>(res.ResponseText);

            return res;
        }

        #endregion

    }
}
