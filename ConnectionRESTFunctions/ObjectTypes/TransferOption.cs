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
    /// The TransferOption class contains all the properties associated with a transfer rule in Unity Connection that can be fetched 
    /// via the CUPI interface.  This class also contains a number of static and instance methods for finding, editing and listing
    /// transfer rules.  You cannot add or remove transfer rules.
    /// </summary>
    public class TransferOption
    {

        #region Constructor

        /// <summary>
        /// Generic constructor for Json parsing
        /// </summary>
        public TransferOption()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
            
            //little bit of a hack - CUPI does not return TimeExpires values for null TimeExpires fields so we can only assume that it's abscence
            //means the greeting is active
            TimeExpires = DateTime.Parse("2200/1/1");
        }

    /// <summary>
    /// Creates a new instance of the TransferOption class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
    /// updating data for this entry.  
    /// If you pass the pTransferOptionType parameter the transfer option is automatically filled with data for that entry from the server.  
    /// If no pTransferOptionType is passed an empty instance of the TransferOption class is returned (so you can fill it out on your own).
    /// </summary>
    /// <param name="pConnectionServer">
    /// Instance of a ConnectonServer object which points to the home server for the transfer option being created.
    /// </param>
    /// <param name="pCallHandlerObjectId">
    /// GUID identifying the Call Handler that owns the transfer option
    /// </param>
    /// <param name="pTransferOptionType">
    /// The transfer rule to fetch (Standard, Alternate, OffHours)
    /// </param>
    public TransferOption(ConnectionServer pConnectionServer, string pCallHandlerObjectId, string pTransferOptionType = ""):this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to TransferOption constructor.");
            }

            //we must know what call handler we're associated with.
            if (String.IsNullOrEmpty(pCallHandlerObjectId))
            {
                throw new ArgumentException("Invalid CallHandlerObjectID passed to TransferOption constructor.");
            }

            HomeServer = pConnectionServer;

            //remember the objectID of the owner of the menu entry as the CUPI interface requires this in the URL construction
            //for operations editing them.
            CallHandlerObjectId = pCallHandlerObjectId;

           //if the user passed in a specific ObjectId then go load that transfer option up, otherwise just return an empty instance.
            if (pTransferOptionType.Length == 0) return;

            //if the TransferRule is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetTransferOption(pTransferOptionType);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Transfer Option not found in TransferOption constructor using CallHandlerObjectID={0} and rule={1}\n\rError={2}",
                    pCallHandlerObjectId, pTransferOptionType, res.ErrorText));
            }
        }


        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object used to create this TransferOption instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of whic properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        private int _action;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection transfers the call to the call handler greeting or attempts to transfer the call to an extension
        /// 0=PlayGreeting, 1=Transfer
        /// </summary>
        public int Action
        {
            get { return _action; }
            set
            {
                _changedPropList.Add("Action", value);
                _action = value;
            }
        }

        
        /// <summary>
        /// Call handler owner of this transfer option.
        /// This cannot be set or changed after creation.
        /// </summary>
        [JsonProperty]
        public string CallHandlerObjectId { get; private set; }

        /// <summary>
        /// Indicates if the transfer rule is enabled (active) or not.  This is read only for reporting purposes - set the TimeExpires date 
        /// to a time in the past to disable it or a time in the future to enable it.
        /// </summary>
        [JsonProperty]
        public bool Enabled { get; private set; }

        private string _extension;
        /// <summary>
        /// The extension (phone number) to transfer to
        /// </summary>
        public string Extension
        {
            get { return _extension; }
            set
            {
                _changedPropList.Add("Extension", value);
                _extension = value;
            }
        }

        private string _mediaSwitchObjectId;
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
        /// Unique identifier for this transfer option.
        /// You cannot change the objectID of a standing object.
        /// </summary>
        [JsonProperty]
        public string ObjectId { get; private set; }

        private bool _personalCallTransfer;
        /// <summary>
        /// A flag indicating whether or not Personal Call Transfer Rules are used for the specific Transfer Option.  This overrides the system transfer
        /// rule if it's active.
        /// </summary>
        public bool PersonalCallTransfer
        {
            get { return _personalCallTransfer; }
            set
            {
                _changedPropList.Add("PersonalCallTransfer", value);
                _personalCallTransfer = value;
            }
        }

        private bool _playTransferPrompt;
        /// <summary>
        /// A flag indicating whether the "Wait while I transfer your call" prompt should be played prior to transferring a call
        /// </summary>
        public bool PlayTransferPrompt
        {
            get { return _playTransferPrompt; }
            set
            {
                _changedPropList.Add("PlayTransferPrompt", value);
                _playTransferPrompt = value;
            }
        }

        private int _rnaAction;
        /// <summary>
        /// The action Cisco Unity Connection takes for a "Ring-No-Answer" (RNA) condition. Cisco Unity Connection will either transfer the 
        /// call to the appropriate greeting or releases the call to the phone system.
        /// 0 is release, 1 is play greeting
        /// </summary>
        public int RnaAction
        {
            get { return _rnaAction; }
            set
            {
                _changedPropList.Add("RnaAction", value);
                _rnaAction = value;
            }
        }

        private DateTime _timeExpires;
        /// <summary>
        /// The date and time when this transfer option expires. If the transfer rule is enabled, the value is NULL or a date in the future. 
        /// If the transfer rule is disable, the value is a past date.
        /// </summary>
        public DateTime TimeExpires
        {
            get { return _timeExpires; }
            set
            {
                _changedPropList.Add("TimeExpires", value);
                _timeExpires = value;
            }
        }

        /// <summary>
        /// Method to set the TimeExpires value to null - this will enable the transfer option forever as opposed to setting a date in the fugure 
        /// at which time it would disable itself.
        /// </summary>
        public void TimeExpiresSetNull()
        {
            _changedPropList.Add("TimeExpires", "");
        }
        
        private bool _transferAnnounce;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection plays "transferring call" when the subscriber answers the phone
        /// </summary>
        public bool TransferAnnounce
        {
            get { return _transferAnnounce; }
            set
            {
                _changedPropList.Add("TransferAnnounce", value);
                _transferAnnounce = value;
            }
        }

        private bool _transferConfirm;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection prompts the subscriber to accept or refuse a call ("Press 1 to take the call or 2 
        /// and I'll take a message"). If the call is accepted, it is transferred to the subscriber phone. If the call is refused, Cisco Unity 
        /// Connection plays the applicable subscriber greeting
        /// </summary>
        public bool TransferConfirm
        {
            get { return _transferConfirm; }
            set
            {
                _changedPropList.Add("TransferConfirm", value);
                _transferConfirm = value;
            }
        }

        private bool _transferDtDetect;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will check for dialtone before attempting to transfer the call
        /// </summary>
        public bool TransferDtDetect
        {
            get { return _transferDtDetect; }
            set
            {
                _changedPropList.Add("TransferDtDetect", value);
                _transferDtDetect = value;
            }
        }

        private int _transferHoldingMode;
        /// <summary>
        /// The action Cisco Unity Connection will take when the extension is busy
        /// 2=Ask, 0=No, 1=Yes
        /// </summary>
        public int TransferHoldingMode
        {
            get { return _transferHoldingMode; }
            set
            {
                _changedPropList.Add("TransferHoldingMode", value);
                _transferHoldingMode = value;
            }
        }

        private bool _transferIntroduce;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will say "call for {recorded name of the call handler}" when the subscriber answers the phone.
        /// </summary>
        public bool TransferIntroduce
        {
            get { return _transferIntroduce; }
            set
            {
                _changedPropList.Add("TransferIntroduce", value);
                _transferIntroduce = value;
            }
        }

        private int _transferRings;
        public int TransferRings
        {
            get { return _transferRings; }
            set
            {
                _changedPropList.Add("TransferRings", value);
                _transferRings = value;
            }
        }

        private bool _transferScreening;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will prompt callers to say their names. When the phone is answered, the subscriber 
        /// hears "Call from..." before Cisco Unity Connection transfers the call
        /// </summary>
        public bool TransferScreening
        {
            get { return _transferScreening; }
            set
            {
                _changedPropList.Add("TransferScreening", value);
                _transferScreening = value;
            }
        }

        
        private int _transferType;
        /// <summary>
        /// The type of call transfer Cisco Unity Connection will perform - supervised or unsupervised (also referred to as "Release to Switch" transfer)
        /// 1=Supervised, 0=Unsupervised
        /// </summary>
        public int TransferType
        {
            get { return _transferType; }
            set
            {
                _changedPropList.Add("TransferType", value);
                _transferType = value;
            }
        }

        /// <summary>
        /// OffHours, Standard, Alternate - cannot be changed.
        /// </summary>
        [JsonProperty]
        public string TransferOptionType { get; private set; }

        private bool _usePrimaryExtension;
        /// <summary>
        /// If extension is null this will be set to true to indicate we are using instead the DtmfAccessId for the owning handler or subscriber.
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

        #endregion


        #region Static Methods



        /// <summary>
        /// Fetches a transfer option object filled with all the properties for a specific entry identified with the ObjectId
        /// of the call handler that owns it and the transfer rule name (Standard, Alternate, OffHours) to fetch.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that the transfer option is homed on.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// The objectID of the call handler that owns the transfer option to be fetched.
        /// </param>
        /// <param name="pTransferOptionType">
        /// The name of the transfer option to fetch (Standard, Alternate, OffHours)
        /// </param>
        /// <param name="pTransferOption">
        /// The out parameter that the instance of the TransferOption class filled in with the details of the fetched entry is
        /// passed back on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetTransferOption(ConnectionServer pConnectionServer,
                                                        string pCallHandlerObjectId,
                                                        string pTransferOptionType,
                                                        out  TransferOption pTransferOption)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pTransferOption = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to TransferOption";
                return res;
            }

            if (string.IsNullOrEmpty(pCallHandlerObjectId) | string.IsNullOrEmpty(pTransferOptionType))
            {
                res.ErrorText = "Empty CallHandlerObjectID or TransferOptionType passed to GetTransferOption";
                return res;
            }

            //create a new transfer option instance passing the transfer rule name which fills out the data automatically
            try
            {
                pTransferOption = new TransferOption(pConnectionServer, pCallHandlerObjectId, pTransferOptionType);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch transfer option in TransferOption:" + ex.Message;
            }



            return res;
        }

        /// <summary>
        /// Returns all the Transfer Options for a call handler. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the transfer options are being fetched from.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// GUID identifying the call handler that owns the transfer options being fetched
        /// </param>
        /// <param name="pTransferOptions">
        /// The list of TransferOption objects are returned using this out parameter.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetTransferOptions(ConnectionServer pConnectionServer,
                                                            string pCallHandlerObjectId,
                                                           out List<TransferOption> pTransferOptions)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pTransferOptions = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetTransferOptions";
                return res;
            }

            string strUrl = string.Format("{0}handlers/callhandlers/{1}/transferoptions", pConnectionServer.BaseUrl, pCallHandlerObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty thats an error - there should always be transfer options.
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pTransferOptions = new List<TransferOption>();
                res.Success = false;
                return res;
            }

            pTransferOptions = HTTPFunctions.GetObjectsFromJson<TransferOption>(res.ResponseText);

            if (pTransferOptions == null)
            {
                pTransferOptions = new List<TransferOption>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pTransferOptions)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.CallHandlerObjectId = pCallHandlerObjectId;
                oObject.ClearPendingChanges();
            }

            return res;

        }


        /// <summary>
        /// Allows one or more properties on a transfer option to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the transfer option is homed.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// Unique identifier for call handler that owns the transfer option being updated
        /// </param>
        /// <param name="pTransferOptionType">
        /// Name of the transfer rule to update (Alternate, Standard, OffHours)
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a transfer option property name and a new value for that property to apply to the option 
        /// being updated. This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one
        /// property pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateTransferOption(ConnectionServer pConnectionServer, 
                                                        string pCallHandlerObjectId, 
                                                        string pTransferOptionType, 
                                                        ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateTransferOption";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList==null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateTransferOption";
                return res;
            }

            string strBody = "<TransferOption>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</TransferOption>";

            return HTTPFunctions.GetCupiResponse(string.Format("{0}handlers/callhandlers/{1}/transferoptions/{2}", pConnectionServer.BaseUrl, pCallHandlerObjectId, 
                pTransferOptionType),MethodType.PUT,pConnectionServer,strBody,false);

        }


        /// <summary>
        /// Special helper function for dealing with the enabled/disabled status of transfer options.  This can be done directly using the timeExpiresSetNull
        /// option found in the propertis seciton above but it's less than intuative and most users do not get how the timeExpires functionality works for 
        /// transfer options and greeting rules - as such this routine wraps it up in a simple single call option.
        /// If TRUE is passed for the enabled then the greeting TimeExpires is set to null (meaning it's always enabled) unless a pTillDate value is passed.  
        /// If this value is passed then the TimeExpires value is set to that.  
        /// If a pTillDate is a time in the passed  and it's a time in the past then nothing is  done and and error is returned - a TimeExpires in the future 
        /// means to enable it till that time, there is no reason to pass a time in the past.
        /// If the pEnabled is passed as FALSE then the TimeExpires is set to "10/11/1999" which disables it.  This date is used for all TimeExpires disabling
        /// routines in the ConnectionCUPIFunctions libarary.
        /// </summary>
        /// <remarks>
        /// The change queue will be cleared when this is called - if you wish to make other changes do it ahead of time or after making this call.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Connection server that the transfer option being edited lives on.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// The GUID that identifies the call handler which owns the transfer option being edited.
        /// </param>
        /// <param name="pTransferOptionType">
        /// The transfer option type being edited (Standard, Off Hours, Alternate).  You cannot edit the Standard transfer option in this routine, it is always
        /// set to enabled on the server and that cannot be changed.
        /// </param>
        /// <param name="pEnabled">
        /// Pass TRUE to enable the transfer option, FALSE to disable it.  If you pass TRUE you may optionally pass a pTillDate DateTime property to indicate the
        /// date the rule will disable itself automatically.
        /// </param>
        /// <param name="pTillDate">
        /// Optional parameter that can be passed when pEnabled is passed as TRUE.  This must be a date/time in the future, a date/time in the past will result in 
        /// the method returning a failure.
        /// </param>
        /// <returns></returns>
        public static WebCallResult UpdateTransferOptionEnabledStatus(ConnectionServer pConnectionServer,
                                                        string pCallHandlerObjectId,
                                                        string pTransferOptionType,
                                                        bool pEnabled,
                                                        DateTime? pTillDate = null)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateTransferOptionEnabledStatus";
                return res;
            }

            if (string.IsNullOrEmpty(pCallHandlerObjectId) | string.IsNullOrEmpty(pTransferOptionType))
            {
                res.ErrorText = "Empty handler ObjectId or TransferType passed to UpdateTransferOptionEnabledStatus";
                return res;
            }

            //first make sure the user isn't trying to change the enabled status on the Standard transfer option - this will fail since that option needs to 
            //always be enabled on the server - fail it up front and pass back the WebCallResult with this information.
            if (pTransferOptionType.Equals("Standard",StringComparison.InvariantCultureIgnoreCase))
            {
                res.ErrorText = "Attempt made to modify Standard transfer option in UpdateTransferOptionEnabledStatus.";
                return res;
            }

            //invalid 
            if (pEnabled==false & (pTillDate != null))
            {
                res.ErrorText = "A date ending time was passed along with enabled=false in UpdateTransferOptionEnabledStatus.";
                return res;
            }

            //finally - no date in the past is valid
            if ((pTillDate != null) && (pTillDate<DateTime.Now))
            {
                res.ErrorText = "A  pTillDate in the past was passed in UpdateTransferOptionEnabledStatus.";
                return res;
            }

            //ok, everthing looks valid, make the change.  The enabled/disaled status of a transfer option rides on the TimeExpires field.
            ConnectionPropertyList oProp = new ConnectionPropertyList();

            if (pEnabled==false)
            {
                //use the 10/11/1999 date to disable the transfer rule.
                oProp.Add("TimeExpires", DateTime.Parse("10/11/1999"));
            }
            else
            {
                if (pTillDate != null)
                {
                    //use the date passed in for the expiration time.
                    oProp.Add("TimeExpires", pTillDate.Value);
                }
                else
                {
                    //set it to null to make it never expire - you also have to pass the enabled flag in this case, it's only valid
                    //for update when the date is cleared.
                    oProp.Add("TimeExpires", "");
                    oProp.Add("Enabled", true);
                }
            }

            string strBody = "<TransferOption>";

            foreach (var oPair in oProp)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</TransferOption>";

            return HTTPFunctions.GetCupiResponse(string.Format("{0}handlers/callhandlers/{1}/transferoptions/{2}", pConnectionServer.BaseUrl, pCallHandlerObjectId, 
                pTransferOptionType),MethodType.PUT,pConnectionServer,strBody,false);
        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// TransferOption display function - outputs the name, action and if it's enabled
        /// </summary>
        /// <returns>
        /// String describing the menu entry
        /// </returns>
        public override string ToString()
        {
            return string.Format("Rule name={0}, Action={1} ({2}), enabled={3}", TransferOptionType, Action, (TransferActionTypes)Action, Enabled);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the transfer option object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the alternate extension object instance.
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
        /// Fetches a transfer option object filled with all the properties for a specific entry identified with the ObjectId
        /// of the call handler that owns it and the name of the transfer rule (Standard, Alternate, Off Hours)
        /// </summary>
        /// <param name="pTransferOptionType">
        /// The name of the transfer option to fetch (Standard, Alternate, Off Hours)
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetTransferOption(string pTransferOptionType)
        {
            WebCallResult res;
            
            if (string.IsNullOrEmpty(pTransferOptionType))
            {
                res=new WebCallResult();
                res.ErrorText = "Empty TransferOptionType passed to GetTransferOption";
                return res;

            }
            
            string strUrl = string.Format("{0}handlers/callhandlers/{1}/transferoptions/{2}", HomeServer.BaseUrl, CallHandlerObjectId, pTransferOptionType);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

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
        /// Special helper function for dealing with the enabled/disabled status of transfer options.  This can be done directly using the timeExpiresSetNull
        /// option found in the propertis seciton above but it's less than intuative and most users do not get how the timeExpires functionality works for 
        /// transfer options and greeting rules - as such this routine wraps it up in a simple single call option.
        /// If TRUE is passed for the enabled then the greeting TimeExpires is set to null (meaning it's always enabled) unless a pTillDate value is passed.  
        /// If this value is passed then the TimeExpires value is set to that.  
        /// If a pTillDate is a time in the passed  and it's a time in the past then nothing is  done and and error is returned - a TimeExpires in the future 
        /// means to enable it till that time, there is no reason to pass a time in the past.
        /// If the pEnabled is passed as FALSE then the TimeExpires is set to "10/11/1999" which disables it.  This date is used for all TimeExpires disabling
        /// routines in the ConnectionCUPIFunctions libarary.
        /// </summary>
        /// <remarks>
        /// The change queue will be cleared when this is called - if you wish to make other changes do it ahead of time or after making this call.
        /// </remarks>
        /// <param name="pEnabled">
        /// Pass TRUE to enable the transfer option, FALSE to disable it.  If you pass TRUE you may optionally pass a pTillDate DateTime property to indicate the
        /// date the rule will disable itself automatically.
        /// </param>
        /// <param name="pTillDate">
        /// Optional parameter that can be passed when pEnabled is passed as TRUE.  This must be a date/time in the future, a date/time in the past will result in 
        /// the method returning a failure.
        /// </param>
        /// <returns></returns>
        public WebCallResult UpdateTransferOptionEnabledStatus(bool pEnabled,
                                                        DateTime? pTillDate = null)
        {
            //the attempt clear the change queue
            ClearPendingChanges();
            return UpdateTransferOptionEnabledStatus(HomeServer, CallHandlerObjectId, TransferOptionType, pEnabled, pTillDate);
        }


        /// <summary>
        /// If the transfer option object has any pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }


        /// <summary>
        /// Allows one or more properties on a transfer option to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the transfer option intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for transferoption:{0}, objectid=[{1}]",
                                              this, this.ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateTransferOption(HomeServer, CallHandlerObjectId, TransferOptionType, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }


        #endregion

    }
}
