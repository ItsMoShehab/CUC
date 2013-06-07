#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The NotificaitonDevice class contains all the properties associated with a Notification Device in Unity Connection that can be fetched 
    /// via the CUPI interface.  This class also contains a number of static and instance methods for finding, deleting, editing and listing
    /// Notification Devices.
    /// </summary>
    public class NotificationDevice : IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// Generic constructor for JSON parser
        /// </summary>
        public NotificationDevice()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Creates a new instance of the NotificationDevice class.  You must provide the ConnectionServer reference that the device lives on and an ObjectId
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
        /// <param name="pDisplayName">
        /// Optional display name of the device - names must be unique across all devices so this can be used for fetching a specific notificaiton device
        /// of any type.
        /// </param>
        public NotificationDevice(ConnectionServerRest pConnectionServer, string pUserObjectId, string pObjectId = "", string pDisplayName = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer reference passed to NotificationDevice constructor");
            }

            if (string.IsNullOrEmpty(pUserObjectId))
            {
                throw new ArgumentException("Empty user objectId passed to NotificationDevice constructor");
            }

            HomeServer = pConnectionServer;
            UserObjectId = pUserObjectId;

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetNotificationDevice(pUserObjectId, pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Notification Device not found in NotificationDevice constructor using ObjectId={0} " +
                                                                         "or Display name={1}\n\r{2}", pObjectId, pDisplayName, res.ErrorText));
            }

        }


        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DisplayName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        //reference to the ConnectionServer object used to create this notificationd evice instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region NotificationDevice Properties

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

        private string _afterDialDigits;
        /// <summary>
        /// The extra digits (if any) that Cisco Unity Connection will dial after the phone number
        /// </summary>
        public string AfterDialDigits
        {
            get { return _afterDialDigits; }
            set
            {
                _changedPropList.Add("AfterDialDigits", value);
                _afterDialDigits = value;
            }
        }

        private int _busyRetryInterval;
        /// <summary>
        /// The extra digits (if any) that Cisco Unity Connection will dial after the phone number
        /// </summary>
        public int BusyRetryInterval
        {
            get { return _busyRetryInterval; }
            set
            {
                _changedPropList.Add("BusyRetryInterval", value);
                _busyRetryInterval = value;
            }
        }

        private string _conversation;
        /// <summary>
        /// The name of the Conversation Cisco Unity Connection will use when calling the subscriber to notify of new messages. 
        /// This normally does not need to be edited - phone notificaiton types use SubNoitify and you do not have to set this, the 
        /// back end stored procedure does it for you.
        /// </summary>
        public string Conversation
        {
            get { return _conversation; }
            set
            {
                _changedPropList.Add("Conversation", value);
                _conversation = value;
            }
        }

        private bool _detectTransferLoop;
        public bool DetectTransferLoop
        {
            get { return _detectTransferLoop; }
            set
            {
                _changedPropList.Add("DetectTransferLoop", value);
                _detectTransferLoop = value;
            }
        }

        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set 
            {
                _changedPropList.Add("DeviceName", value);
                _deviceName = value; 
            }
        }

        private int _dialDelay;
        /// <summary>
        /// The amount of time (in seconds) Cisco Unity Connection will wait after detecting a successful call before dialing specified additional 
        /// digits (if any). Additional digits are contained in AfterDialDigits
        /// </summary>
        public int DialDelay
        {
            get { return _dialDelay; }
            set
            {
                _changedPropList.Add("DialDelay", value);
                _dialDelay = value;
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

        private string _eventList;
        /// <summary>
        /// Comma separated list of events that trigger notification rules to be evaluate.
        /// AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage,
        ///  </summary>
        public string EventList
        {
            get { return _eventList; }
            set
            {
                _changedPropList.Add("EventList", value);
                _eventList = value;
            }
        }

        private string _failDeviceObjectId;
        /// <summary>
        /// ObjectId of the NotificationDevice that Cisco Unity Connection will use if attempted notification to this device fails.
        /// </summary>
        public string FailDeviceObjectId
        {
            get { return _failDeviceObjectId; }
            set
            {
                _changedPropList.Add("FailDeviceObjectId", value);
                _failDeviceObjectId = value;
            }
        }

        private int _initialDelay;
        /// <summary>
        /// The amount of time (in minutes) from the time when a message is received until message notification triggers
        /// </summary>
        public int InitialDelay
        {
            get { return _initialDelay; }
            set
            {
                _changedPropList.Add("InitialDelay", value);
                _initialDelay = value;
            }
        }

        private int _maxBody;
        /// <summary>
        /// The maximum number of characters allowed in the 'body' of a notification message
        /// </summary>
        public int MaxBody
        {
            get { return _maxBody; }
            set
            {
                _changedPropList.Add("MaxBody", value);
                _maxBody = value;
            }
        }

        private int _maxSubject;
        /// <summary>
        /// The maximum number of characters allowed in the 'subject' of a notification message
        /// </summary>
        public int MaxSubject
        {
            get { return _maxSubject; }
            set
            {
                _changedPropList.Add("MaxSubject", value);
                _maxSubject = value;
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
        
        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _changedPropList.Add("PhoneNumber", value);
                _phoneNumber = value;
            }
        }

        private bool _promptForId;
        /// <summary>
        /// A flag indicating whether to prompt a subscriber for their Cisco Unity Connection ID, or just their password
        /// </summary>
        public bool PromptForId
        {
            get { return _promptForId; }
            set
            {
                _changedPropList.Add("PromptForId", value);
                _promptForId = value;
            }
        }

        private int _repeatInterval;
        /// <summary>
        /// The amount of time (in minutes) Cisco Unity Connection will wait before re-notifying a subscriber of new messages
        /// </summary>
        public int RepeatInterval
        {
            get { return _repeatInterval; }
            set
            {
                _changedPropList.Add("RepeatInterval", value);
                _repeatInterval = value;
            }
        }

        private bool _repeatNotify;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection begins a notification process immediately upon the arrival of each message that
        /// matches the specified criteria.
        /// </summary>
        public bool RepeatNotify
        {
            get { return _repeatNotify; }
            set
            {
                _changedPropList.Add("RepeatNotify", value);
                _repeatNotify = value;
            }
        }

        private int _retriesOnBusy;
        /// <summary>
        /// The number of times Cisco Unity Connection will retry the notification device if it is busy
        /// </summary>
        public int RetriesOnBusy
        {
            get { return _retriesOnBusy; }
            set
            {
                _changedPropList.Add("RetriesOnBusy", value);
                _retriesOnBusy = value;
            }
        }

        private int _retriesOnRna;
        /// <summary>
        /// The number of times Cisco Unity Connection will retry the notification device if it is ring-no-answer
        /// </summary>
        public int RetriesOnRna
        {
            get { return _retriesOnRna; }
            set
            {
                _changedPropList.Add("RetriesOnRna", value);
                _retriesOnRna = value;
            }
        }

        private int _retriesOnSuccess;
        /// <summary>
        /// The number of times Cisco Unity Connection will retry the notification device if it is successful.  This means the target phone was answered
        /// but apparently the person who answered it did not log in and check the message(s) that matched the trigger criteria.
        /// </summary>
        public int RetriesOnSuccess
        {
            get { return _retriesOnSuccess; }
            set
            {
                _changedPropList.Add("RetriesOnSuccess", value);
                _retriesOnSuccess = value;
            }
        }

        private int _ringsToWait;
        public int RingsToWait
        {
            get { return _ringsToWait; }
            set
            {
                _changedPropList.Add("RingsToWait", value);
                _ringsToWait = value;
            }
        }

        private int _rnaRetryInterval;
        /// <summary>
        /// The amount of time (in minutes) Cisco Unity Connection will wait between tries if the device does not answer
        /// </summary>
        public int RnaRetryInterval
        {
            get { return _rnaRetryInterval; }
            set
            {
                _changedPropList.Add("RnaRetryInterval", value);
                _rnaRetryInterval = value;
            }
        }

        private string _scheduleSetObjectId;
        public string ScheduleSetObjectId
        {
            get { return _scheduleSetObjectId; }
            set
            {
                _changedPropList.Add("ScheduleSetObjectId", value);
                _scheduleSetObjectId = value;
            }
        }

        private bool _sendCallerId;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will include the caller id (if available) with the notification message. 
        /// </summary>
        public bool SendCallerId
        {
            get { return _sendCallerId; }
            set
            {
                _changedPropList.Add("SendCallerId", value);
                _sendCallerId = value;
            }
        }

        private bool _sendPcaLink;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will include a link to the PCA with the notification message
        /// </summary>
        public bool SendPcaLink
        {
            get { return _sendPcaLink; }
            set
            {
                _changedPropList.Add("SendPcaLink", value);
                _sendPcaLink = value;
            }
        }

        private bool _sendCount;
        /// <summary>
        /// A flag indicting whether Cisco Unity Connection will include a count of each voice mail, fax, and e-mail message with the 
        /// notification message. When the subscriber receives the notification, the message count appears as a line for each type of message. 
        /// </summary>
        public bool SendCount
        {
            get { return _sendCount; }
            set
            {
                _changedPropList.Add("SendCount", value);
                _sendCount = value;
            }
        }

        private string _smtpAddress;
        public string SmtpAddress
        {
            get { return _smtpAddress; }
            set
            {
                _changedPropList.Add("SmtpAddress", value);
                _smtpAddress = value;
            }
        }

        private string _staticText;
        /// <summary>
        /// The actual text message that the subscriber wants to receive in a Text Pager notification. The SA refers to this as the "Send:" field
        /// </summary>
        public string StaticText
        {
            get { return _staticText; }
            set
            {
                _changedPropList.Add("StaticText", value);
                _staticText = value;
            }
        }

        private int _successRetryInterval;
        /// <summary>
        /// The amount of time (in minutes) Cisco Unity Connection will wait between tries if the device is successful
        /// </summary>
        public int SuccessRetryInterval
        {
            get { return _successRetryInterval; }
            set
            {
                _changedPropList.Add("SuccessRetryInterval", value);
                _successRetryInterval = value;
            }
        }

        //you can't change the Subscriber owner of a notification device.
        [JsonProperty]
        public string SubscriberObjectId { get; private set; }
        
        private bool _transmitForcedAuthorizationCode;
        /// <summary>
        /// A flag indicating whether an authorization code should be transmitted to Cisco Call Manager after this number is dialed during an outbound call
        /// </summary>
        public bool TransmitForcedAuthorizationCode
        {
            get { return _transmitForcedAuthorizationCode; }
            set
            {
                _changedPropList.Add("TransmitForcedAuthorizationCode", value);
                _transmitForcedAuthorizationCode = value;
            }
        }

        
        /// <summary>
        /// The device type (Dial, Numeric Pager, SMTP, Fax).  This cannot be changed after creation.
        /// //3=Fax, 7=MP3, 2=Pager, 1=Phone, 5=SMS, 4=SMTP - 
        /// </summary>
        [JsonProperty]
        public int Type { get; private set; }

        private bool _undeletable;
        public bool Undeletable
        {
            get { return _undeletable; }
            set
            {
                _changedPropList.Add("Undeletable", value);
                _undeletable = value;
            }
        }

        //not retrieved from Connection and cannot be changed once created
        public string UserObjectId { get; private set; }

        private bool _waitConnect;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection waits until it detects a connection before dialing the digits in AfterDialDigits
        /// </summary>
        public bool WaitConnect
        {
            get { return _waitConnect; }
            set
            {
                _changedPropList.Add("WaitConnect", value);
                _waitConnect = value;
            }
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// returns a single NotificationDevice object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the device is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID of the user that owns the notification device to be fetched.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the device to load
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name of device - can be passed if objectId is blank
        /// </param>
        /// <param name="pNotificationDevice">
        /// The out param that the filled out instance of the NotificationDevice class is returned on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetNotificationDeivce(ConnectionServerRest pConnectionServer, string pUserObjectId, string pObjectId,string pDisplayName, 
            out NotificationDevice pNotificationDevice)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pNotificationDevice = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetNotificationDeivce";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty ObjectId and display name passed to GetNotificationDevice";
                return res;
            }

            //create a new NotificationDevice instance passing the ObjectId which fills out the data automatically
            try
            {
                pNotificationDevice = new NotificationDevice(pConnectionServer,pUserObjectId, pObjectId,pDisplayName);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch device in GetNotificationDeivce:" + ex.Message;
            }

            return res;
        }


        /// <summary>
        /// Returns all the notification devices for a user.  There should always be at least 4 notification devices (those created by setup) but
        /// may be more.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the notification devices are being fetched from.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID identifying the user that owns the notification devices to be fetched.
        /// </param>
        /// <param name="pNotificationDevices">
        /// The list of notification devices is returned on this out param
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetNotificationDevices(ConnectionServerRest pConnectionServer,
                                                            string pUserObjectId,
                                                           out List<NotificationDevice> pNotificationDevices)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pNotificationDevices = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetNotificationDevices";
                return res;
            }

            if (string.IsNullOrEmpty(pUserObjectId))
            {
                res.ErrorText = "Empty UserObjectId passed to GetNotificationDevices";
                return res;
            }

            string strUrl = string.Format("{0}users/{1}/notificationdevices", pConnectionServer.BaseUrl, pUserObjectId);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - return true here along with an empty list.
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount==0)
            {
                pNotificationDevices = new List<NotificationDevice>();
                return res;
            }

            pNotificationDevices = pConnectionServer.GetObjectsFromJson<NotificationDevice>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pNotificationDevices)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.UserObjectId = pUserObjectId;
                oObject.ClearPendingChanges();
            }

            return res;
        }

        
        /// <summary>
        /// Adds a new custom SMTP notification device for a user identified via ObjectId.  The device display name must be unique for devices
        /// assigned to that useror the add will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server the user to add the device to lives on.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID identifying the user to add the device to.
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name to assign the notification device
        /// </param>
        /// <param name="pSmtpAddress">
        /// SMTP address notification will be sent to.
        /// </param>
        /// <param name="pEventList">
        /// One or more event types that this device will trigger on:
        ///  AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// True or false to indicate the device is active or not.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddSmtpDevice(ConnectionServerRest pConnectionServer,
                                                  string pUserObjectId,
                                                  string pDeviceDisplayName,
                                                  string pSmtpAddress,
                                                  string pEventList,
                                                  bool pActivated)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddSMTPDevice";
                return res;
            }

            //make sure that something is passed in for the required params.
            if (String.IsNullOrEmpty(pDeviceDisplayName) |
                (String.IsNullOrEmpty(pUserObjectId)) |
                (String.IsNullOrEmpty(pEventList)) |
                (String.IsNullOrEmpty(pSmtpAddress)))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddSMTPDevice";
                return res;
            }

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("DisplayName", pDeviceDisplayName);
            oProps.Add("SmtpAddress",pSmtpAddress);
            oProps.Add("EventList", pEventList);
            oProps.Add("Active", pActivated);

            string strBody = "<SmtpDevice>";

            //tack on the property value pair with appropriate tags
            foreach (var oProp in oProps)
            {
                strBody += string.Format("<{0}>{1}</{0}>", oProp.PropertyName, oProp.PropertyValue);
            }

            strBody += "</SmtpDevice>";

            res =pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/notificationdevices/smtpdevices", pConnectionServer.BaseUrl, 
                pUserObjectId),MethodType.POST,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = @"/vmrest/users/" + pUserObjectId + "/notificationdevices/smtpdevices/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId =res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Adds a new custom SMTP notification device for a user identified via ObjectId.  The device display name must be unique for devices
        /// assigned to that useror the add will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server the user to add the device to lives on.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID identifying the user to add the device to.
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name to assign the notification device
        /// </param>
        /// <param name="pSmtpAddress">
        /// SMTP address notification will be sent to.
        /// </param>
        /// <param name="pEventList">
        /// One or more event types that this device will trigger on:
        ///  AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// True or false to indicate the device is active or not.
        /// </param>
        /// <param name="pDevice">
        /// Instance of the notification device created is passed back on this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddSmtpDevice(ConnectionServerRest pConnectionServer,
                                                   string pUserObjectId,
                                                   string pDeviceDisplayName,
                                                   string pSmtpAddress,
                                                   string pEventList,
                                                   bool pActivated, out NotificationDevice pDevice)
         {
             pDevice = null;
             var res = AddSmtpDevice(pConnectionServer, pUserObjectId, pDeviceDisplayName, pSmtpAddress, pEventList, pActivated);
             if (res.Success)
             {
                 res = GetNotificationDeivce(pConnectionServer, pUserObjectId, res.ReturnedObjectId,"",out pDevice);
             }

             return res;
         }
        

        /// <summary>
        /// Adds a new custom HTML notification device for a user identified via ObjectId.  The device display name must be unique for devices
        /// assigned to that user or the add will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server the user to add the device to lives on.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID identifying the user to add the device to.
        /// </param>
        /// <param name="pTemplateObjectId">
        /// ObjectId of the HTTP template to use when creating this device
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name to assign the notification device
        /// </param>
        /// <param name="pSmtpAddress">
        /// HTTP address notification will be sent to.
        /// </param>
        /// <param name="pEventList">
        /// One or more event types that this device will trigger on:
        ///  AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// True or false to indicate the device is active or not.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddHtmlDevice(ConnectionServerRest pConnectionServer,
                                                  string pUserObjectId,
                                                  string pTemplateObjectId,
                                                  string pDeviceDisplayName,
                                                  string pSmtpAddress,
                                                  string pEventList,
                                                  bool pActivated)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddHtmlDevice";
                return res;
            }

            //make sure that something is passed in for the required params
            if (String.IsNullOrEmpty(pDeviceDisplayName) |
                (String.IsNullOrEmpty(pUserObjectId)) |
                (String.IsNullOrEmpty(pEventList)) |
                (String.IsNullOrEmpty(pSmtpAddress)))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddHtmlDevice";
                return res;
            }

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("DisplayName", pDeviceDisplayName);
            oProps.Add("NotificationTemplateID",pTemplateObjectId);
            oProps.Add("SmtpAddress", pSmtpAddress);
            oProps.Add("EventList", pEventList);
            oProps.Add("Active", pActivated);

            string strBody = "<HtmlDevice>";

            //tack on the property value pair with appropriate tags
            foreach (var oProp in oProps)
            {
                strBody += string.Format("<{0}>{1}</{0}>", oProp.PropertyName, oProp.PropertyValue);
            }

            strBody += "</HtmlDevice>";

            res =
                pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/notificationdevices/htmldevices", pConnectionServer.BaseUrl, 
                pUserObjectId),MethodType.POST, strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = @"/vmrest/users/" + pUserObjectId + "/notificationdevices/htmldevices/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Adds a new custom HTML notification device for a user identified via ObjectId.  The device display name must be unique for devices
        /// assigned to that user or the add will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server the user to add the device to lives on.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID identifying the user to add the device to.
        /// </param>
        /// <param name="pTemplateObjectId">
        /// ObjectId of the HTTP template to use when creating this device
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name to assign the notification device
        /// </param>
        /// <param name="pSmtpAddress">
        /// HTTP address notification will be sent to.
        /// </param>
        /// <param name="pEventList">
        /// One or more event types that this device will trigger on:
        ///  AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// True or false to indicate the device is active or not.
        /// </param>
        /// <param name="pDevice">
        /// Instance of the notification device just created will be returned on this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddHtmlDevice(ConnectionServerRest pConnectionServer,
                                                  string pUserObjectId,
                                                  string pTemplateObjectId,
                                                  string pDeviceDisplayName,
                                                  string pSmtpAddress,
                                                  string pEventList,
                                                  bool pActivated,
                                                  out NotificationDevice pDevice)
        {
            pDevice = null;

            var res = AddHtmlDevice(pConnectionServer, pUserObjectId, pTemplateObjectId, pDeviceDisplayName, pSmtpAddress, pEventList, pActivated);
            if (res.Success)
            {
                res = GetNotificationDeivce(pConnectionServer, pUserObjectId, res.ReturnedObjectId, "", out pDevice);
            }
            return res;
        }

        /// <summary>
        /// Adds a new custom SMTP notification device for a user identified via ObjectId.  The device display name must be unique for devices
        /// assigned to that useror the add will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server the user to add the device to lives on.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID identifying the user to add the device to.
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name to assign the notification device
        /// </param>
        /// <param name="pSmppProviderObjectId">
        /// The GUID identifying the SMS provider this device will use.
        /// </param>
        /// <param name="pRecipientAddress">
        /// SMTP address notification will be sent to.
        /// </param>
        /// <param name="pSenderAddress">
        /// SMTP address notification will be sent from.
        /// </param>
        /// <param name="pEventList">
        /// One or more event types that this device will trigger on:
        ///  AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// True or false to indicate the device is active or not.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddSmsDevice(ConnectionServerRest pConnectionServer,
                                                  string pUserObjectId,
                                                  string pDeviceDisplayName,
                                                  string pSmppProviderObjectId,
                                                  string pRecipientAddress,
                                                  string pSenderAddress,
                                                  string pEventList,
                                                  bool pActivated)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddSMTPDevice";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pDeviceDisplayName) |
                (String.IsNullOrEmpty(pUserObjectId)) |
                (String.IsNullOrEmpty(pEventList)) |
                (String.IsNullOrEmpty(pRecipientAddress)))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddSMTPDevice";
                return res;
            }

            if (string.IsNullOrEmpty(pSmppProviderObjectId))
            {
                res.ErrorText = "Emtpy SMPP provider id passed to AddSMSDevice";
                return res;
            }

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("DisplayName", pDeviceDisplayName);
            oProps.Add("SmppProviderObjectId", pSmppProviderObjectId);
            oProps.Add("RecipientAddress", pRecipientAddress);
            oProps.Add("SenderAddress", pSenderAddress);
            oProps.Add("EventList", pEventList);
            oProps.Add("Active", pActivated);

            string strBody = "<SmsDevice>";

            //tack on the property value pair with appropriate tags
            foreach (var oProp in oProps)
            {
                strBody += string.Format("<{0}>{1}</{0}>", oProp.PropertyName, oProp.PropertyValue);
            }

            strBody += "</SmsDevice>";

            res = pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/notificationdevices/smsdevices", pConnectionServer.BaseUrl, 
                pUserObjectId),MethodType.POST,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = @"/vmrest/users/" + pUserObjectId + "/notificationdevices/smsdevices/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId =res.ResponseText.Replace( strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Adds a new custom SMTP notification device for a user identified via ObjectId.  The device display name must be unique for devices
        /// assigned to that useror the add will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server the user to add the device to lives on.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID identifying the user to add the device to.
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name to assign the notification device
        /// </param>
        /// <param name="pSmppProviderObjectId">
        /// The GUID identifying the SMS provider this device will use.
        /// </param>
        /// <param name="pRecipientAddress">
        /// SMTP address notification will be sent to.
        /// </param>
        /// <param name="pSenderAddress">
        /// SMTP address notification will be sent from.
        /// </param>
        /// <param name="pEventList">
        /// One or more event types that this device will trigger on:
        ///  AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// True or false to indicate the device is active or not.
        /// </param>
        /// <param name="pDevice">
        /// Instance of the notification device just created will be passed back on this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddSmsDevice(ConnectionServerRest pConnectionServer,
                                                 string pUserObjectId,
                                                 string pDeviceDisplayName,
                                                 string pSmppProviderObjectId,
                                                 string pRecipientAddress,
                                                 string pSenderAddress,
                                                 string pEventList,
                                                 bool pActivated,
                                                 out NotificationDevice pDevice)
        {
            pDevice = null;

            var res = AddSmsDevice(pConnectionServer, pUserObjectId, pDeviceDisplayName,pSmppProviderObjectId,
                pRecipientAddress,pSenderAddress,pEventList,pActivated);
            if (res.Success)
            {
                res = GetNotificationDeivce(pConnectionServer, pUserObjectId, res.ReturnedObjectId, "", out pDevice);
            }
            return res;
        }


        /// <summary>
        /// Add a phone notification device for a user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the user to add the device for is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// Identifier for the user to add the device for
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name for the device
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// Switch that the device is associated with - indicates which ports will be used when dialing out to this device.
        /// </param>
        /// <param name="pPhoneNumber">
        /// Phone number to dial 
        /// </param>
        /// <param name="pEventList">
        /// Comma separated list of events that trigger notification rules for this device:
        /// AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// Is the device enabled (active) or not.  Will fail if there is no phone number on the device
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class
        /// </returns>
        public static WebCallResult AddPhoneDevice(ConnectionServerRest pConnectionServer,
                                          string pUserObjectId,
                                          string pDeviceDisplayName,
                                          string pMediaSwitchObjectId,
                                          string pPhoneNumber,
                                          string pEventList,
                                          bool pActivated)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddPhoneDevice";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pDeviceDisplayName) |
                (String.IsNullOrEmpty(pUserObjectId)) |
                (String.IsNullOrEmpty(pMediaSwitchObjectId)) |
                (String.IsNullOrEmpty(pPhoneNumber)))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddPhoneDevice";
                return res;
            }

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("DisplayName", pDeviceDisplayName);
            oProps.Add("MediaSwitchObjectId", pMediaSwitchObjectId);
            oProps.Add("PhoneNumber", pPhoneNumber);
            oProps.Add("EventList", pEventList);
            oProps.Add("Active", pActivated);

            string strBody = "<PhoneDevice>";

            //tack on the property value pair with appropriate tags
            foreach (var oProp in oProps)
            {
                strBody += string.Format("<{0}>{1}</{0}>", oProp.PropertyName, oProp.PropertyValue);
            }

            strBody += "</PhoneDevice>";

            res = pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/notificationdevices/phonedevices", pConnectionServer.BaseUrl, 
                pUserObjectId),MethodType.POST,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = @"/vmrest/users/" + pUserObjectId + "/notificationdevices/phonedevices/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Add a phone notification device for a user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the user to add the device for is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// Identifier for the user to add the device for
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name for the device
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// Switch that the device is associated with - indicates which ports will be used when dialing out to this device.
        /// </param>
        /// <param name="pPhoneNumber">
        /// Phone number to dial 
        /// </param>
        /// <param name="pEventList">
        /// Comma separated list of events that trigger notification rules for this device:
        /// AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// Is the device enabled (active) or not.  Will fail if there is no phone number on the device
        /// </param>
        /// <param name="pDevice">
        /// The newly created notification device will be passed back on this out parameter.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class
        /// </returns>
        public static WebCallResult AddPhoneDevice(ConnectionServerRest pConnectionServer,
                                                   string pUserObjectId,
                                                   string pDeviceDisplayName,
                                                   string pMediaSwitchObjectId,
                                                   string pPhoneNumber,
                                                   string pEventList,
                                                   bool pActivated,
                                                   out NotificationDevice pDevice)
        {
            pDevice = null;

            var res = AddPhoneDevice(pConnectionServer, pUserObjectId, pDeviceDisplayName,pMediaSwitchObjectId,pPhoneNumber,pEventList,pActivated);
            if (res.Success)
            {
                res = GetNotificationDeivce(pConnectionServer, pUserObjectId, res.ReturnedObjectId, "", out pDevice);
            }
            return res;
        }

        /// <summary>
        /// Add a pager notification device for a user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the user to add the device for is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// Identifier for the user to add the device for
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name for the device
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// Switch that the device is associated with - indicates which ports will be used when dialing out to this device.
        /// </param>
        /// <param name="pPhoneNumber">
        /// Phone number to dial 
        /// </param>
        /// <param name="pEventList">
        /// Comma separated list of events that trigger notification rules for this device:
        /// AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// Is the device enabled (active) or not.  Will fail if there is no phone number on the device
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class
        /// </returns>
        public static WebCallResult AddPagerDevice(ConnectionServerRest pConnectionServer,
                                  string pUserObjectId,
                                  string pDeviceDisplayName,
                                  string pMediaSwitchObjectId,
                                  string pPhoneNumber,
                                  string pEventList,
                                  bool pActivated)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddPagerDevice";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pDeviceDisplayName) |
                (String.IsNullOrEmpty(pUserObjectId)) |
                (String.IsNullOrEmpty(pMediaSwitchObjectId)) |
                (String.IsNullOrEmpty(pPhoneNumber)))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddPagerDevice";
                return res;
            }

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("DisplayName", pDeviceDisplayName);
            oProps.Add("MediaSwitchObjectId", pMediaSwitchObjectId);
            oProps.Add("PhoneNumber", pPhoneNumber);
            oProps.Add("EventList", pEventList);
            oProps.Add("Active", pActivated);

            string strBody = "<PagerDevice>";

            //tack on the property value pair with appropriate tags
            foreach (var oProp in oProps)
            {
                strBody += string.Format("<{0}>{1}</{0}>", oProp.PropertyName, oProp.PropertyValue);
            }

            strBody += "</PagerDevice>";

            res = pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/notificationdevices/pagerdevices", pConnectionServer.BaseUrl, 
                pUserObjectId),MethodType.POST,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = @"/vmrest/users/" + pUserObjectId + "/notificationdevices/pagerdevices/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Add a pager notification device for a user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the user to add the device for is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// Identifier for the user to add the device for
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Unique display name for the device
        /// </param>
        /// <param name="pMediaSwitchObjectId">
        /// Switch that the device is associated with - indicates which ports will be used when dialing out to this device.
        /// </param>
        /// <param name="pPhoneNumber">
        /// Phone number to dial 
        /// </param>
        /// <param name="pEventList">
        /// Comma separated list of events that trigger notification rules for this device:
        /// AllMessage,NewFax,NewUrgentFax,NewVoiceMail,NewUrgentVoiceMail,DispatchMessage,UrgentDispatchMessage
        /// </param>
        /// <param name="pActivated">
        /// Is the device enabled (active) or not.  Will fail if there is no phone number on the device
        /// </param>
        /// <param name="pDevice">
        /// An instance of the newly created notificaiton device is passed back on this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class
        /// </returns>
        public static WebCallResult AddPagerDevice(ConnectionServerRest pConnectionServer,
                                                   string pUserObjectId,
                                                   string pDeviceDisplayName,
                                                   string pMediaSwitchObjectId,
                                                   string pPhoneNumber,
                                                   string pEventList,
                                                   bool pActivated,
                                                   out NotificationDevice pDevice)
        {
            pDevice = null;

            var res = AddPagerDevice(pConnectionServer, pUserObjectId, pDeviceDisplayName, pMediaSwitchObjectId,pPhoneNumber,pEventList,pActivated);
            if (res.Success)
            {
                res = GetNotificationDeivce(pConnectionServer, pUserObjectId, res.ReturnedObjectId, "", out pDevice);
            }
            return res;
        }


        /// <summary>
        /// DELETE a notification device associated with a user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that houses the user that owns the device to be removed.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID of the user that owns the device to be removed.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the notification device to remove
        /// </param>
        /// <param name="pDeviceType">
        /// Notificaiton device type defined in the NotificationDeviceTypes enum.  Fax and MP3 types are not supported although they are defined.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteNotificationDevice(ConnectionServerRest pConnectionServer, 
                                                            string pUserObjectId, 
                                                            string pObjectId, 
                                                            NotificationDeviceTypes pDeviceType)
        {
            WebCallResult res;

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteNotificationDevice";
                return res;
            }

            string strUrl = GetUrlPathForDeviceType(pDeviceType, pConnectionServer, pUserObjectId, pObjectId);

            //if empty comes back it's because it didn't recognize the device type
            if (String.IsNullOrEmpty(strUrl))
            {
                res = new WebCallResult();
                res.ErrorText = "Invalid device type passed to DeleteNotificationDevice:" + pDeviceType.ToString();
                return res;
            }

            return pConnectionServer.GetCupiResponse(strUrl, MethodType.DELETE, "");
        }


        /// <summary>
        /// Allows one or more properties on a extension to be udpated (for instance DTMFAccessID).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the device is homed.
        /// </param>
        /// <param name="pUserObjectId">
        /// Unique identifier for user that owns the notification device being edited.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the notificaton device owned by the user to be updated.
        /// </param>
        /// <param name="pDeviceType">
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a notification device property name and a new value for that property to apply to the extension 
        /// being updated. This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one
        /// property pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateNotificationDevice(ConnectionServerRest pConnectionServer, 
                                                            string pUserObjectId, 
                                                            string pObjectId, 
                                                            NotificationDeviceTypes pDeviceType,
                                                            ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateNotificationDevice";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateNotificationDevice";
                return res;
            }

            //construct the full path to the device type off this user
            string strUrl = GetUrlPathForDeviceType(pDeviceType, pConnectionServer, pUserObjectId, pObjectId);

            //if empty comes back it's because it didn't recognize the device type
            if (String.IsNullOrEmpty(strUrl))
            {
                res = new WebCallResult();
                res.ErrorText = "Invalid device type passed to UpdateNotificationDevice:" + pDeviceType.ToString();
                return res;
            }

            string strBody = string.Format("<{0}>",pDeviceType.ToString());

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += string.Format("</{0}>", pDeviceType.ToString());

            return pConnectionServer.GetCupiResponse(strUrl, MethodType.PUT, strBody, false);
        }



        //Helper function to construct the full path to the notificaiton device based on its type which is used by both the edit and
        //delete functions.  There's no provision for deleting a notification devie via the generic "notificationdevices" path - you have
        //to reference the type in the both for the ObjectId to be resolved.
        private static string GetUrlPathForDeviceType(NotificationDeviceTypes pDeviceType, ConnectionServerRest pConnectionServer, string pUserObjectId, 
            string pObjectId)
        {
            string strUrl;

            switch (pDeviceType)
            {
                case NotificationDeviceTypes.Pager:
                    strUrl = string.Format("{0}users/{1}/notificationdevices/pagerdevices/{2}", pConnectionServer.BaseUrl, pUserObjectId, pObjectId);
                    break;
                case NotificationDeviceTypes.Phone:
                    strUrl = string.Format("{0}users/{1}/notificationdevices/phonedevices/{2}", pConnectionServer.BaseUrl, pUserObjectId, pObjectId);
                    break;
                case NotificationDeviceTypes.Sms:
                    strUrl = string.Format("{0}users/{1}/notificationdevices/smsdevices/{2}", pConnectionServer.BaseUrl, pUserObjectId, pObjectId);
                    break;
                case NotificationDeviceTypes.Smtp:
                    strUrl = string.Format("{0}users/{1}/notificationdevices/smtpdevices/{2}", pConnectionServer.BaseUrl, pUserObjectId, pObjectId);
                    break;
                default:
                    pConnectionServer.RaiseErrorEvent("Invalid device type encountered in GetUrlPathForDeviceType:"+pDeviceType.ToString());
                    return "";
            }

            return strUrl;

        }


        #endregion


        #region Instance Methods
        
        /// <summary>
        /// Diplays the device name, display name, it's type and if it's active or not
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} [{1}], type={2}, active={3}", this.DeviceName, this.DisplayName,(NotificationDeviceTypes)this.Type, this.Active);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the notification object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the NotificationDevice object instance.
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
                res.ErrorText = string.Format("Update called but there are no pending changes for Notification Device:{0}, objectid=[{1}]",
                                              this, this.ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateNotificationDevice(HomeServer, UserObjectId, ObjectId,(NotificationDeviceTypes)Type, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }


        /// <summary>
        /// DELETE a notification device from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeleteNotificationDevice(HomeServer, UserObjectId, ObjectId,(NotificationDeviceTypes)Type);
        }


        /// <summary>
        /// Fills the current instance of NotificationDevice in with properties fetched from the server.
        /// </summary>
        /// <param name="pObjectId">
        /// GUID that identifies the user that owns the device
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID that identifies the device itself.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name of device to find - can be blank if objectId is passed
        /// </param>
        /// <returns></returns>
        private WebCallResult GetNotificationDevice(string pUserObjectId, string pObjectId, string pDisplayName)
        {
            string strObjectId = pObjectId;
            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdFromName(pUserObjectId, pDisplayName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult
                        {
                            Success = false,
                            ErrorText = "No notification device found with name=" + pDisplayName
                        };
                }
            }

            string strUrl = string.Format("{0}users/{1}/notificationdevices/{2}", HomeServer.BaseUrl, pUserObjectId, strObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(ConnectionServerRest.StripJsonOfObjectWrapper(res.ResponseText, "NotificationDevice"), this,
                    RestTransportFunctions.JsonSerializerSettings);
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
        /// Fetch the ObjectId of a notification device by it's name.  Empty string returned if no match is found.
        /// </summary>
        /// <param name="pUserObjectId">
        /// ObjectID of the user that owns the device
        /// </param>
        /// <param name="pDeviceDisplayName">
        /// Name of the device to find
        /// </param>
        /// <returns>
        /// ObjectId of device if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pUserObjectId, string pDeviceDisplayName)
        {
            //ugly hack
            //currently the option to query on display name for notification devices is not supported since the field is not indexed.
            //to provide the simple display name fetching model this gets the list of all devices for a user and fetches the one that 
            //has a matching name - not pretty or efficent - this will get replaced with a proper query construct once the API has 
            //support for it.
            List<NotificationDevice> oList;
            WebCallResult res = GetNotificationDevices(HomeServer, pUserObjectId, out oList);
            if (res.Success == false)
            {
                return "";
            }

            foreach (var oDevice in oList)
            {
                if (oDevice.DisplayName.Equals(pDeviceDisplayName,StringComparison.InvariantCultureIgnoreCase))
                    return oDevice.ObjectId;
            }

            return "";
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
