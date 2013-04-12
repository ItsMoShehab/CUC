﻿#region Legal Disclaimer

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

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// struct for values in a server instance that's returned as a member of a cluster
    /// </summary>
    public class Server
    {
        public string HostName { get; set; }
        public int DatabaseReplication { get; set; }
        public string Key { get; set; }
        public string Ipv6Name { get; set; }
        public string MacAddress { get; set; }
        public string Description { get; set; }


        /// <summary>
        /// Returns a string with the text host name and key of the server
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", HostName, Key);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the server object in "name=value" format - each pair is on its
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
    }

    /// <summary>
    /// Read only class for fetching cluster (server) details - little different than most classes in that it includes references to server instances
    /// as opposed to instances of itself - one cluster instance needs to be created (no static methods) and 1 or 2 server(s) will be available in the 
    /// public Servers property (generic list) based on the cluster configuration.
    /// </summary>
    public class Cluster
    {
      
        #region Fields and Properties

        //reference to the ConnectionServer object used to create this user instance.
        public ConnectionServer HomeServer { get; private set; }

        public List<Server> Servers;

        #endregion

           
        #region Constructors

        /// <summary>
        /// Constructor for the Server class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        public Cluster(ConnectionServer pConnectionServer)
        {
            if (pConnectionServer==null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to Cluster construtor");
            }

            HomeServer = pConnectionServer;

            //fetch the servers in the cluster (always 1 but can be 2)
            WebCallResult res = GetServers(HomeServer);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Cluster details not found in Cluster constructor\n\r{0}", res.ErrorText));
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
                strTemp += oServer.ToString() + Environment.NewLine;
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
        private WebCallResult GetServers(ConnectionServer pConnectionServer)
        {
            WebCallResult res;
            Servers = new List<Server>();

            string strUrl = pConnectionServer.BaseUrl + "cluster";

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }


            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one template
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                Servers = new List<Server>();
                res.Success = false;
                return res;
            }

            Servers = HTTPFunctions.GetObjectsFromJson<Server>(res.ResponseText);

            if (Servers == null)
            {
                Servers = new List<Server>();
                return res;
            }

            return res;
        }

        #endregion

    }
}
