#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Class that provides methods for fetching, adding, updating and deleting port group servers in the Unity Connection
    /// directory
    /// </summary>
    public class PortGroupServer
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor for the PortGroup class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group that owns the port group server.
        /// </param>
        /// <param name="pObjectId">
        /// Optional - if passed in the specifics of the switch identified by this GUID is fetched and the properties are filled in.
        /// </param>
        public PortGroupServer(ConnectionServerRest pConnectionServer, string pPortGroupObjectId, string pObjectId = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to PortGroupServer construtor");
            }

            if (string.IsNullOrEmpty(pPortGroupObjectId))
            {
                throw new ArgumentException("Empty port group ObjectId passed to PortGroupServer constructor");
            }

            HomeServer = pConnectionServer;
            ObjectId = pObjectId;
            PortGroupObjectId = pPortGroupObjectId;

            //if no objectId is passed in just create an empty version of the class - used for constructing lists from XML fetches.
            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetPortGroupServer(pObjectId, pPortGroupObjectId);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Port group server not found in PortGroup constructor using ObjectId={0}\n\r{1}"
                                                 , pObjectId, res.ErrorText));
            }
        }

        /// <summary>
        /// Generic constructor for Json libraries
        /// </summary>
        public PortGroupServer()
        {
            _changedPropList = new ConnectionPropertyList();
        }

        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object used to create this object instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        //used to get to the pending change list if necessary
        public ConnectionPropertyList ChangeList { get { return _changedPropList; } }

        #endregion


        #region PortGroupServer Properties

        private string _hostOrIpAddress;
        public string HostOrIpAddress
        {
            get { return _hostOrIpAddress; }
            set
            {
                _changedPropList.Add("HostOrIpAddress", value);
                _hostOrIpAddress = value;
            }
        }

        private string _hostOrIpAddressV6;
        public string HostOrIpAddressV6
        {
            get { return _hostOrIpAddressV6; }
            set
            {
                _changedPropList.Add("HostOrIpAddressV6", value);
                _hostOrIpAddressV6 = value;
            }
        }

        private MediaRemoteServiceEnum _mediaRemoteServiceEnum;
        public MediaRemoteServiceEnum MediaRemoteServiceEnum
        {
            get { return _mediaRemoteServiceEnum; }
            set
            {
                _changedPropList.Add("MediaRemoteServiceEnum",(int) value);
                _mediaRemoteServiceEnum = value;
            }
        }

        private string _mediaPortGroupObjectId;
        public string MediaPortGroupObjectId
        {
            get { return _mediaPortGroupObjectId; }
            set
            {
                _changedPropList.Add("MediaPortGroupObjectId", value);
                _mediaPortGroupObjectId = value;
            }
        }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string PortGroupObjectId { get; private set; }


        private int _port;
        public int Port
        {
            get { return _port; }
            set
            {
                _changedPropList.Add("Port", value);
                _port = value;
            }
        }

        private string _precedence;
        public string Precedence
        {
            get { return _precedence; }
            set
            {
                _changedPropList.Add("Precedence", value);
                _precedence = value;
            }
        }

        private SkinnyStateMachineEnum _skinnyStateMachineEnum;
        public SkinnyStateMachineEnum SkinnyStateMachineEnum
        {
            get { return _skinnyStateMachineEnum; }
            set
            {
                _changedPropList.Add("SkinnyStateMachineEnum",(int) value);
                _skinnyStateMachineEnum = value;
            }
        }

        private string _tlsPort;
        public string TlsPort
        {
            get { return _tlsPort; }
            set
            {
                _changedPropList.Add("TlsPort", value);
                _tlsPort = value;
            }
        }

        private string _displayName;
        public string DisplayName 
        {
            get { return _displayName; }
            set
            {
                _changedPropList.Add("DisplayName", value);
                _displayName = value;
            }
        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the phone system
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
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
        /// string containing all the name value pairs defined in the PortGroupServer object instance.
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
        /// If the transfer option object has any pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }

        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchPortGroupServerData()
        {
            return GetPortGroupServer(this.ObjectId, this.PortGroupObjectId);
        }

        /// <summary>
        /// Fetch details for a single port group server by ObjectId and populate the local instance's properties with it
        /// </summary>
        /// <param name="pObjectId">
        /// Unique identifier for port group server to fetch
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group that owns the port group server to fetch
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        private WebCallResult GetPortGroupServer(string pObjectId, string pPortGroupObjectId)
        {
            string strUrl = string.Format("{0}portgroups/{1}/portgroupservers/{2}", HomeServer.BaseUrl,pPortGroupObjectId, pObjectId);

            //issue the command to the CUPI interface
            var res = HomeServer.FillObjectWithRestGetResults(strUrl,this);
            ClearPendingChanges();
            return res;
        }

        /// <summary>
        /// Allows one or more properties on a port group server to be udpated.  
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update(bool pRefetchDataAfterSuccessfulUpdate = false)
        {
            //check if the transfer option intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = string.Format("Update called but there are no pending changes for PortGroupServer:{0}, " +
                                                  "objectid=[{1}]",this,this.ObjectId),
                    };
            }

            //just call the static method with the info from the instance 
            WebCallResult res = UpdatePortGroupServer(HomeServer, PortGroupObjectId, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
                if (pRefetchDataAfterSuccessfulUpdate)
                {
                    return RefetchPortGroupServerData();
                }
            }

            return res;
        }

        /// <summary>
        /// DELETE a port group server from a port group
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeletePortGroupServer(HomeServer, ObjectId,PortGroupObjectId);
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all port group servers associated with a port group. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the phone systems should be pulled from
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group to fetch portgroupservers from.
        /// </param>
        /// <param name="pPortGroupServers">
        /// Out parameter that is used to return the list of PortGroupServer objects defined for port group
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
        public static WebCallResult GetPortGroupServers(ConnectionServerRest pConnectionServer, string pPortGroupObjectId,
            out List<PortGroupServer> pPortGroupServers,int pPageNumber = 1, int pRowsPerPage = 20)
        {
            WebCallResult res = new WebCallResult {Success = false};
            pPortGroupServers = new List<PortGroupServer>();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetPortGroupServers";
                return res;
            }

            if (string.IsNullOrEmpty(pPortGroupObjectId))
            {
                res.ErrorText = "Empty PortGroupObjectId passed to GetPortGroupServers";
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(string.Format("{0}portgroups/{1}/portgroupservers", pConnectionServer.BaseUrl,pPortGroupObjectId), 
                "pageNumber=" + pPageNumber,"rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.ErrorText = "Empty response received";
                res.Success = false;
                return res;
            }

            //no error, just return the empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                return res;
            }

            pPortGroupServers = pConnectionServer.GetObjectsFromJson<PortGroupServer>(res.ResponseText);

            if (pPortGroupServers == null)
            {
                pPortGroupServers = new List<PortGroupServer>();
                res.ErrorText = "Could not parse JSON into PortGroupServer objects:" + res.ResponseText;
                res.Success = false;
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pPortGroupServers)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// Fetches a port group server by ObjectId and passes it back as an out parameter as an instance of the PortGroupServer class.
        /// </summary>
        /// <param name="pPortGroupServer">
        /// Out param that the information is passed back on.
        /// </param>
        /// <param name="pConnectionServer">
        /// Connection server that the portgroup lives on.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the portgroup server to load.
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// PortGroup that the port group server is tied to.
        /// </param>
        /// <returns>
        ///  Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPortGroupServer(out PortGroupServer pPortGroupServer, ConnectionServerRest pConnectionServer, string pObjectId, 
            string pPortGroupObjectId)
        {
            WebCallResult res = new WebCallResult { Success = false };

            pPortGroupServer = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPortGroupServer";
                return res;
            }

            //you need an objectID and a PortGroupObjectId 
            if (string.IsNullOrEmpty(pObjectId) | string.IsNullOrEmpty(pPortGroupObjectId))
            {
                res.ErrorText = "Empty objectId or PortGroupObjectId passed to GetPortGroupServer";
                return res;
            }

            //create a new PortGroupServer instance passing the ObjectId (or display name) which fills out the data automatically
            try
            {
                pPortGroupServer = new PortGroupServer(pConnectionServer, pPortGroupObjectId, pObjectId);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch port group server in GetPortGroupServer:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Update one or more proprties on a PortGroupServer object
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the media port server lives on.
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// The port group that owns the media group server.
        /// </param>
        /// <param name="pObjectId">
        /// OBjectId (GUID) of the Media Port server.
        /// </param>
        /// <param name="pPropList">
        /// Name value pair of property names that have changed and their corresponding values.
        /// </param>
        /// <returns>
        ///  Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdatePortGroupServer(ConnectionServerRest pConnectionServer,
                                                        string pPortGroupObjectId,
                                                        string pObjectId,
                                                        ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdatePortGroupServer";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdatePortGroupServer";
                return res;
            }

            string strBody = "<PortGroupServer>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</PortGroupServer>";

            return pConnectionServer.GetCupiResponse(string.Format("{0}portgroups/{1}/portgroupservers/{2}", pConnectionServer.BaseUrl, 
                pPortGroupObjectId, pObjectId),MethodType.PUT, strBody, false);

        }

        /// <summary>
        /// Adds a new port group server to a port group.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the port group server to.
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group to add the port group server to.
        /// </param>
        /// <param name="pMediaPortGroupServiceEnum">
        /// </param>
        /// <param name="pHostOrIpAddress">
        /// Host address or IP address of the server to add.
        /// </param>
        /// <param name="pHostOrIpAddressV6">
        /// IPV6 host or ip address
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddPortGroupServer(ConnectionServerRest pConnectionServer, string pPortGroupObjectId, int pMediaPortGroupServiceEnum,
            string pHostOrIpAddress, string pHostOrIpAddressV6="")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddPortGroupServer";
                return res;
            }

            //make sure that something is passed in for the required param
            if (string.IsNullOrEmpty(pPortGroupObjectId) | string.IsNullOrEmpty(pHostOrIpAddress))
            {
                res.ErrorText = "Empty value passed for host address or PortGroupObjectId in AddPortGroupServer";
                return res;
            }

            string strBody = "<PortGroupServer>";

            strBody += string.Format("<MediaPortGroupObjectId >{0}</MediaPortGroupObjectId >", pPortGroupObjectId);
            strBody += string.Format("<MediaRemoteServiceEnum >{0}</MediaRemoteServiceEnum >", pMediaPortGroupServiceEnum);

            //tack on the property value pair with appropriate tags
            if (!string.IsNullOrEmpty(pHostOrIpAddress))
            {
                strBody += string.Format("<HostOrIPAddress>{0}</HostOrIPAddress>", pHostOrIpAddress);
            }
            
            if (!string.IsNullOrEmpty(pHostOrIpAddressV6))
            {
                strBody += string.Format("<HostOrIPAddressV6>{0}</HostOrIPAddressV6>", pHostOrIpAddressV6);
            }
            strBody += "</PortGroupServer>";

            res = pConnectionServer.GetCupiResponse(string.Format("{0}portgroups/{1}/portgroupservers", pConnectionServer.BaseUrl, 
                    pPortGroupObjectId),MethodType.POST, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = string.Format(@"/vmrest/portgroups/{0}/portgroupservers/", pPortGroupObjectId);
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Adds a new port group server to a port group.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the port group server to.
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Port group to add the port group server to.
        /// </param>
        /// <param name="pMediaPortGroupServiceEnum"></param>
        /// <param name="pHostOrIpAddress">
        /// Host address or IP address of the server to add.
        /// </param>
        /// <param name="pHostOrIpAddressV6">
        /// IPV6 host or ip address
        /// </param>
        /// <param name="pPortGroupServer">
        /// Instance of the new PortGroupServer is passed back on this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddPortGroupServer(ConnectionServerRest pConnectionServer, string pPortGroupObjectId,int pMediaPortGroupServiceEnum,
            string pHostOrIpAddress, string pHostOrIpAddressV6,out PortGroupServer pPortGroupServer)
        {
            pPortGroupServer = null;

            WebCallResult res = AddPortGroupServer(pConnectionServer, pPortGroupObjectId,pMediaPortGroupServiceEnum, pHostOrIpAddress,pHostOrIpAddressV6);

            //if the create goes through, fetch the phone system as an object and return it.
            if (res.Success)
            {
                res = GetPortGroupServer(out pPortGroupServer, pConnectionServer, res.ReturnedObjectId,pPortGroupObjectId);
            }

            return res;
        }


        /// <summary>
        /// Delete a port group server from a port group
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that houses the port group server to be deleted
        /// </param>
        /// <param name="pObjectId">
        /// Unique identifier for port group server
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// Unique identifier for port group that owns the port group server.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeletePortGroupServer(ConnectionServerRest pConnectionServer, string pObjectId, string pPortGroupObjectId)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult { ErrorText = "Null ConnectionServer referenced passed to DeletePortGroupServer" };
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                return new WebCallResult { ErrorText = "Empty ObjectId passed to DeletePortGroupServer" };
            }

            if (string.IsNullOrEmpty(pPortGroupObjectId))
            {
                return new WebCallResult { ErrorText = "Empty PortGroupServerObjectId passed to DeletePortGroupServer" };
            }

            return pConnectionServer.GetCupiResponse(string.Format("{0}portgroups/{1}/portgroupservers/{2}", pConnectionServer.BaseUrl, 
                    pPortGroupObjectId, pObjectId),MethodType.DELETE, "");
        }

        #endregion

    }
}
