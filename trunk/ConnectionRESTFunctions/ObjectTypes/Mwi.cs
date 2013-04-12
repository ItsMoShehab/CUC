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
    /// The Mwi class contains all the properties associated with a MWI Devices in Unity Connection that can be fetched 
    /// via the CUPI interface.  This class also contains a number of static and instance methods for finding, deleting, editing and listing
    /// MWIs.
    /// </summary>
    public class Mwi
    {
        
        #region Properties
        
        //reference to the ConnectionServer object used to create this notificationd evice instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of whic properties have been updated
        private readonly ConnectionPropertyList _changedPropList;


        private bool _active;
        /// <summary>
        /// A flag indicating whether the device is active or inactive (enabled/disabled).
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set
            {
                _changedPropList.Add("Active", value);
                _active = value;
            }
        }

        private bool _mwiOn;
        /// <summary>
        /// A flag indicating whether the device is on or not (lit or dark).
        /// </summary>
        public bool MwiOn
        {
            get { return _mwiOn; }
            set
            {
                _changedPropList.Add("MwiOn", value);
                _mwiOn = value;
            }
        }

        private bool _includeTextMessages;
        /// <summary>
        /// A flag indicating whether the device triggers on text messages
        /// </summary>
        public bool IncludeTextMessages
        {
            get { return _includeTextMessages; }
            set
            {
                _changedPropList.Add("IncludeTextMessages", value);
                _includeTextMessages = value;
            }
        }


        private bool _includeVoiceMessages;
        /// <summary>
        /// A flag indicating whether the device triggers on voice messages
        /// </summary>
        public bool IncludeVoiceMessages
        {
            get { return _includeVoiceMessages; }
            set
            {
                _changedPropList.Add("IncludeVoiceMessages", value);
                _includeVoiceMessages = value;
            }
        }

        private bool _includeFaxMessages;
        /// <summary>
        /// A flag indicating whether the device triggers on faxes
        /// </summary>
        public bool IncludeFaxMessages
        {
            get { return _includeFaxMessages; }
            set
            {
                _changedPropList.Add("IncludeFaxMessages", value);
                _includeFaxMessages = value;
            }
        }

        private bool _usePrimaryExtension;
        /// <summary>
        /// A flag indicating whether the device is tied to the user primary extension (DTMFAccessId)
        /// </summary>
        public bool UsePrimaryExtension
        {
            get { return _usePrimaryExtension; }
            set
            {
                _changedPropList.Add("UsePrimaryExtension", value);
                _usePrimaryExtension = value;
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

        private string _mediaSwitchObjectId;
        /// <summary>
        /// The unique identifier of the MediaSwitch object to use for notification.
        /// Applies only to phone and pager notificationd evices.
        /// </summary>
        public string MediaSwitchObjectId
        {
            get { return _mediaSwitchObjectId; }
            set
            {
                _changedPropList.Add("MediaSwitchObjectId", value);
                _mediaSwitchObjectId = value;
            }
        }

        /// <summary>
        /// The text name of the Media Switch. The unique text name (e.g., "Unified Communications Manager Cluster - Seattle") 
        /// of the media switch to be used when displaying entries in the administrative console, e.g. Cisco Unity Connection Administration.
        /// This is a read only field for display purposes.
        /// </summary>
        [JsonProperty]
        public string MediaSwitchDisplayName { get; private set; }

        //you can't change the ObjectId of a standing object
        [JsonProperty]
        public string ObjectId { get; private set; }

        private string _mwiExtension;
        public string MwiExtension
        {
            get { return _mwiExtension; }
            set
            {
                _changedPropList.Add("MwiExtension", value);
                _mwiExtension = value;
            }
        }

        //you can't change the Subscriber owner of a notification device.
        [JsonProperty]
        public string SubscriberObjectId { get; private set; }
        
        //not retrieved from Connection and cannot be changed once created
        public string UserObjectId { get; private set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Generic constructor for JSON parser
        /// </summary>
        public Mwi()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Creates a new instance of the Mwi class.  You must provide the ConnectionServer reference that the device lives on and an ObjectId
        /// of the user that owns it.  You can optionally pass in the ObjectId of the device itself and it will load the data for that device, otherwise an
        /// empty instance is returned.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that the device is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID that identifies the user that owns the device
        /// </param>
        /// <param name="pObjectId">
        /// Optionally the ObjectId of the device itself - if passed in this will load the NotificationDevice object with data for that device from Connection.
        /// </param>
        public Mwi(ConnectionServer pConnectionServer, string pUserObjectId, string pObjectId=""):this()
        {
          	if (pConnectionServer==null)
            {
                throw new ArgumentException("Null ConnectionServer reference passed to Mwi constructor");
            }

            if (string.IsNullOrEmpty(pUserObjectId))
            {
                throw new ArgumentException("Emtpy UserObjectID passed to Mwi constructor");
            }

            HomeServer = pConnectionServer;

            UserObjectId = pUserObjectId;

            //if the user passed in a specific ObjectId then go load that handler up, otherwise just return an empty instance.
            if (pObjectId.Length == 0) return;

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetMwi(pUserObjectId,pObjectId);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Mwi Device not found in NotificationDevice constructor using ObjectId={0}\n\r{1}"
                                                 ,pObjectId,res.ErrorText));
            }

        }


        #endregion


        #region Static Methods

        /// <summary>
        /// returns a single Mwi object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the device is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID of the user that owns the Mwi device to be fetched.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the device to load
        /// </param>
        /// <param name="pMwiDevice">
        /// The out param that the filled out instance of the Mwi class is returned on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMwiDevice(ConnectionServer pConnectionServer, string pUserObjectId, string pObjectId, out Mwi pMwiDevice)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pMwiDevice = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetMwiDevice";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to GetMwiDevice";
                return res;
            }

            //create a new Mwi instance passing the ObjectId which fills out the data automatically
            try
            {
                pMwiDevice = new Mwi(pConnectionServer,pUserObjectId, pObjectId);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch device in GetMwiDevice:" + ex.Message;
            }

            return res;
        }


        /// <summary>
        /// Returns all the mwi devices for a user.  There should always be at least 1 device but may be more.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the devices are being fetched from.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID identifying the user that owns the devices to be fetched.
        /// </param>
        /// <param name="pMwiDevices">
        /// The list of mwi devices is returned on this out param
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMwiDevices(ConnectionServer pConnectionServer,
                                                            string pUserObjectId,
                                                           out List<Mwi> pMwiDevices)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pMwiDevices = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetMwiDevices";
                return res;
            }

            string strUrl = string.Format("{0}users/{1}/mwis", pConnectionServer.BaseUrl, pUserObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty in this case that does mean an error - all users should have at least one MWI device.
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount==0)
            {
                pMwiDevices = new List<Mwi>();
                res.Success = false;
                return res;
            }

            pMwiDevices = HTTPFunctions.GetObjectsFromJson<Mwi>(res.ResponseText);

            if (pMwiDevices == null)
            {
                pMwiDevices = new List<Mwi>();
                res.Success = false;
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pMwiDevices)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.UserObjectId = pUserObjectId;
                oObject.ClearPendingChanges();
            }

            return res;
        }
        

        /// <summary>
        /// Add a new MWI device for a user
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pUserObjectId"></param>
        /// <param name="pDeviceDisplayName"></param>
        /// <param name="pMediaSwitchObjectId"></param>
        /// <param name="pMwiExtension"></param>
        /// <param name="pActivated"></param>
        /// <returns></returns>
        public static WebCallResult AddMwi(ConnectionServer pConnectionServer,
                                  string pUserObjectId,
                                  string pDeviceDisplayName,
                                  string pMediaSwitchObjectId,
                                  string pMwiExtension,
                                  bool pActivated)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddMwi";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pDeviceDisplayName) |
                (String.IsNullOrEmpty(pUserObjectId)) |
                (String.IsNullOrEmpty(pMediaSwitchObjectId)) |
                (String.IsNullOrEmpty(pMwiExtension)))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddMwi";
                return res;
            }

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("DisplayName", pDeviceDisplayName);
            oProps.Add("MwiExtension", pMwiExtension);
            oProps.Add("MediaSwitchObjectId", pMediaSwitchObjectId);
            oProps.Add("Active", pActivated);

            string strBody = "<Mwi>";

            //tack on the property value pair with appropriate tags
            foreach (var oProp in oProps)
            {
                strBody += string.Format("<{0}>{1}</{0}>", oProp.PropertyName, oProp.PropertyValue);
            }

            strBody += "</Mwi>";

            res = HTTPFunctions.GetCupiResponse(string.Format("{0}users/{1}/mwis", pConnectionServer.BaseUrl, pUserObjectId),
                MethodType.POST,pConnectionServer,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = @"/vmrest/users/" + pUserObjectId + "/mwis/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// DELETE an MWI associated with a user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that houses the user that owns the MWI to be removed.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID of the user that owns the MWI to be removed.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the MWI to remove
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteMwiDevice(ConnectionServer pConnectionServer, 
                                                            string pUserObjectId, 
                                                            string pObjectId)
        {
            WebCallResult res;

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteMwiDevice";
                return res;
            }

            string strUrl = string.Format("{0}users/{1}/mwis/{2}", pConnectionServer.BaseUrl, pUserObjectId,pObjectId);

            //if empty comes back it's because it didn't recognize the device type
            if (String.IsNullOrEmpty(strUrl))
            {
                res = new WebCallResult();
                res.ErrorText = "Invalid device type passed to DeleteMwiDevice:" + pObjectId;
                return res;
            }

            return HTTPFunctions.GetCupiResponse(strUrl,MethodType.DELETE,pConnectionServer, "");
        }


        /// <summary>
        /// Allows one or more properties on a MWI.  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the alternate extension is homed.
        /// </param>
        /// <param name="pUserObjectId">
        /// Unique identifier for user that owns the alternate extension being edited.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the alternate extension owned by the user to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a alternate extension property name and a new value for that property to apply to the extension 
        /// being updated. This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one
        /// property pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateMwi(ConnectionServer pConnectionServer, 
                                                            string pUserObjectId, 
                                                            string pObjectId, 
                                                            ConnectionPropertyList pPropList)
        {
            string strBody = "";
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateMwi";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateMwi";
                return res;
            }

            strBody += "<Mwi>";

            //construct the full path to the device type off this user
            string strUrl = string.Format("{0}users/{1}/mwis/{2}", pConnectionServer.BaseUrl, pUserObjectId, pObjectId);

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Mwi>";

            return HTTPFunctions.GetCupiResponse(strUrl,MethodType.PUT,pConnectionServer,strBody,false);
        }


        #endregion


        #region Instance Methods
        
        /// <summary>
        /// Diplays the display name, it's type and if it's active or not
        /// </summary>
        public override string ToString()
        {
            return String.Format("MWI device [{0}], x{3}, active={1}, on={2}", this.DisplayName, this.Active,this.MwiOn,this.MwiExtension);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the MWI object in "name=value" format - each pair is on its
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
        /// Allows one or more properties on a device to be udpated (for instance display name).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the extension intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for MWI Device:{0}, objectid=[{1}]",
                                              this, this.ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateMwi(HomeServer, UserObjectId, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }


        /// <summary>
        /// DELETE an MWI device from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeleteMwiDevice(HomeServer, UserObjectId, ObjectId);
        }


        /// <summary>
        /// Fills the current instance of Mwi in with properties fetched from the server.
        /// </summary>
        /// <param name="pObjectId">
        /// GUID that identifies the user that owns the device
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID that identifies the device itself.
        /// </param>
        /// <returns></returns>
        private WebCallResult GetMwi(string pUserObjectId, string pObjectId)
        {
            string strUrl = string.Format("{0}users/{1}/mwis/{2}", HomeServer.BaseUrl,pUserObjectId, pObjectId);

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

            //all the updates above will flip pending changes into the queue - clear that here.
            this.ClearPendingChanges();

            return res;
        }



        /// <summary>
        /// If the notificationdevice object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }

        #endregion


    }
}
