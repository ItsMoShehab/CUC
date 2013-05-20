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
    /// Class that provides methods for fetching details about ports, creating, updating and deleting them
    /// </summary>
    public class Port
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor for the Port class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        /// <param name="pObjectId">
        /// Optional - if passed in the specifics of the switch identified by this GUID is fetched and the properties are filled in.
        /// </param>
        public Port(ConnectionServer pConnectionServer, string pObjectId = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to Port construtor");
            }

            HomeServer = pConnectionServer;
            ObjectId = pObjectId;

            //if no objectId is passed in just create an empty version of the class - used for constructing lists from XML fetches.
            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetPort(pObjectId);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Port not found in Port constructor using ObjectId={0}\n\r{1}"
                                                 , pObjectId, res.ErrorText));
            }
        }

        /// <summary>
        /// Generic constructor for Json parsing libraries
        /// </summary>
        public Port()
        {
            _changedPropList = new ConnectionPropertyList();
        }

        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object used to create this object instance.
        public ConnectionServer HomeServer { get; private set; }
        
        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region Port Properties

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
        [JsonProperty]
        public string ObjectId { get; private set; }
        
        [JsonProperty]
        public string MediaPortGroupObjectId { get; private set; }

        [JsonProperty]
        public TelephonyIntegrationMethodEnum TelephonyIntegrationMethodEnum { get; private set; }

        private SkinnySecurityModes _skinnySecurityModeEnum;
        public SkinnySecurityModes SkinnySecurityModeEnum
        {
            get { return _skinnySecurityModeEnum; }
            set
            {
                _changedPropList.Add("SkinnySecurityModeEnum",(int) value);
                _skinnySecurityModeEnum = value;
            }
        }

        [JsonProperty]
        public string VmsServerObjectId { get; private set; }

        private int _huntOrder;
        public int HuntOrder
        {
            get { return _huntOrder; }
            set
            {
                _changedPropList.Add("HuntOrder", value);
                _huntOrder = value;
            }
        }

        private bool _capAnswer;
        public bool CapAnswer
        {
            get { return _capAnswer; }
            set
            {
                _changedPropList.Add("CapAnswer", value);
                _capAnswer = value;
            }
        }

        private bool _capNotification;
        public bool CapNotification
        {
            get { return _capNotification; }
            set
            {
                _changedPropList.Add("CapNotification", value);
                _capNotification = value;
            }
        }

        private bool _capMWI;
        public bool CapMWI
        {
            get { return _capMWI; }
            set
            {
                _changedPropList.Add("CapMWI", value);
                _capMWI = value;
            }
        }

        private bool _capEnabled;
        public bool CapEnabled
        {
            get { return _capEnabled; }
            set
            {
                _changedPropList.Add("CapEnabled", value);
                _capEnabled = value;
            }
        }

        private bool _capTrapConnection;
        public bool CapTrapConnection
        {
            get { return _capTrapConnection; }
            set
            {
                _changedPropList.Add("CapTrapConnection", value);
                _capTrapConnection = value;
            }
        }

        [JsonProperty]
        public string MediaSwitchDisplayName { get; private set; }

        [JsonProperty]
        public string MediaSwitchObjectId { get; private set; }

        [JsonProperty]
        public string MediaPortGroupDisplayName { get; private set; }

        [JsonProperty]
        public string VmsServerName { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the port
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the port object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the Port object instance.
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
        /// Fetch details for a single port by ObjectId and populate the local instance's properties with it
        /// </summary>
        /// <param name="pObjectId">
        /// Unique identifier for port to fetch
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        private WebCallResult GetPort(string pObjectId)
        {
            string strUrl = string.Format("{0}ports/{1}", HomeServer.BaseUrl, pObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, HTTPFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }

            ClearPendingChanges();
            return res;
        }

        /// <summary>
        /// If the port object has any pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }

        /// <summary>
        /// Allows one or more properties on a port to be udpated.  
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the object intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for Port:{0}, objectid=[{1}]",
                                              this, ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdatePort(HomeServer, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }

        /// <summary>
        /// DELETE a Port from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeletePort(HomeServer, ObjectId);
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all ports and resturns them as a generic list of Port objects.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that is being queried
        /// </param>
        /// <param name="pPorts">
        /// Out parameter that is used to return the list of Port objects defined on Connection - there may be none - this list can be 
        /// returned empty.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPorts(ConnectionServer pConnectionServer, out List<Port> pPorts, int pPageNumber = 1, int pRowsPerPage = 20,
            params string[] pClauses)
        {
            WebCallResult res;
            pPorts = new List<Port>();

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetPorts";
                return res;
            }

            //add on the paging directive to existing clauses
            List<string> temp;
            if (pClauses == null)
            {
                temp = new List<string>();
            }
            else
            {
                temp = pClauses.ToList();
            }

            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "ports", temp.ToArray());

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error here
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pPorts = new List<Port>();
                return res;
            }

            pPorts = HTTPFunctions.GetObjectsFromJson<Port>(res.ResponseText);

            if (pPorts == null)
            {
                pPorts = new List<Port>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pPorts)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }

        /// <summary>
        /// Get all the ports associated with a port group (if any).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the ports are homed on.
        /// </param>
        /// <param name="pPorts">
        /// List of ports (if any) are passed back on this out parameter
        /// </param>
        /// <param name="pPortGroupObjectId">
        /// ObjectId of the port group to fetch ports for.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPorts(ConnectionServer pConnectionServer, out List<Port> pPorts,
                                             string pPortGroupObjectId)
        {

            return GetPorts(pConnectionServer, out pPorts, 1, 512,
                            string.Format("query=(MediaPortGroupObjectId is {0})",pPortGroupObjectId));
        }


        /// <summary>
        /// returns a single Port object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pPort">
        /// Port instance is returned on this out param
        /// </param>
        /// <param name="pConnectionServer">
        /// Connection server that the port is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the port to load
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPort(out Port pPort, ConnectionServer pConnectionServer, string pObjectId)
        {
            WebCallResult res = new WebCallResult { Success = false };

            pPort = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPort";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty objectId passed to GetPort";
                return res;
            }

            //create a new PhoneSystem instance passing the ObjectId (or display name) which fills out the data automatically
            try
            {
                pPort = new Port(pConnectionServer, pObjectId);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch port in GetPort:" + ex.Message;
            }

            return res;
        }


        /// <summary>
        /// Allows one or more properties on a port to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the port is homed.
        /// </param>
        /// <param name="pPortObjectId">
        /// Unique identifier for media port to update.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a property name and a new value for that property to apply to the object
        /// being updated. 
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdatePort(ConnectionServer pConnectionServer,string pPortObjectId,ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdatePort";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdatePort";
                return res;
            }

            string strBody = "<Port>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Port>";

            return HTTPFunctions.GetCupiResponse(string.Format("{0}ports/{1}", pConnectionServer.BaseUrl, pPortObjectId),
                MethodType.PUT, pConnectionServer, strBody, false);

        }

        /// <summary>
        /// Adds a new port to the system
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to add the port to
        /// </param>
        /// <param name="pMediaPortGroupObjectId">
        /// Media port group to associate the port with
        /// </param>
        /// <param name="pNumberOfPorts">
        /// Number of ports to add to the port group - should be an even number but that's not enforced.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a property name and a new value for that property to apply to the port being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <param name="pPimgPort">
        /// If adding ports for PIMG/TIMG pass this as true - the VMSServer value needs to be empty in that case.  For SIP and SCCP this needs to 
        /// be passed.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddPort(ConnectionServer pConnectionServer, string pMediaPortGroupObjectId, int pNumberOfPorts,
            ConnectionPropertyList pPropList, bool pPimgPort=false)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddPort";
                return res;
            }

            //make sure that something is passed in for the required param
            if (String.IsNullOrEmpty(pMediaPortGroupObjectId))
            {
                res.ErrorText = "Empty value passed for portgroupObjecTId in AddPort";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            pPropList.Add("MediaPortGroupObjectId",pMediaPortGroupObjectId);
            pPropList.Add("NumberOfPorts",pNumberOfPorts);
            
            //for SIP and SCCP the VMSserverObjectId is needed, for TIMG/PIMG it needs to be left out
            if (!pPimgPort)
            {
                pPropList.Add("VmsServerObjectId",pConnectionServer.VmsServerObjectId);
            }

            string strBody = "<Port>";

            //tack on the property value pair with appropriate tags
            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }
            
            strBody += "</Port>";

            res = HTTPFunctions.GetCupiResponse(string.Format("{0}ports", pConnectionServer.BaseUrl),
                    MethodType.POST, pConnectionServer, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                const string strPrefix = @"/vmrest/ports/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// DELETE a port from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the port is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the port in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeletePort(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeletePort";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "ports/" + pObjectId,
                                            MethodType.DELETE, pConnectionServer, "");
        }
        #endregion

    }
}
