#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The Greeting class contains all the properties associated with a greeting rule in Unity Connection that can be fetched 
    /// via the CUPI interface.  This class also contains a number of static and instance methods for finding, editing and listing
    /// greeting rules.  You cannot add or remove greeting rules.
    /// </summary>
    public class Greeting :IUnityDisplayInterface
    {

        #region Constructor

        /// <summary>
        /// Generic constructor for JSON parsing
        /// </summary>
        public Greeting()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Creates a new instance of the Greeting class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this entry.  
        /// If you pass the pGreetingType parameter the greeting is automatically filled with data for that entry from the server.  If not then an
        /// empty instance of the Greeting class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the greeting being created.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// GUID identifying the Call Handler that owns the greeting
        /// </param>
        /// <param name="pGreetingType">
        /// The greeting rule to fetch (Standard, Alternate, OffHours, Busy, Internal, Error, Holdiay)
        /// </param>
        public Greeting(ConnectionServer pConnectionServer, string pCallHandlerObjectId, GreetingTypes pGreetingType = GreetingTypes.Invalid ):this()
            {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to Greeting constructor.");
            }

            //we must know what call handler we're associated with.
            if (String.IsNullOrEmpty(pCallHandlerObjectId))
            {
                throw new ArgumentException("Invalid CallHandlerObjectID passed to Greeting constructor.");
            }

            HomeServer = pConnectionServer;

            //remember the objectID of the owner of the menu entry as the CUPI interface requires this in the URL construction
            //for operations editing them.
            CallHandlerObjectId = pCallHandlerObjectId;

            //little bit of a hack - CUPI does not return TimeExpires values for null TimeExpires fields so we can only assume that it's abscence
            //means the greeting is active
            TimeExpires = DateTime.Parse("2200/1/1");

            //if the user passed in a specific ObjectId then go load that greeting up, otherwise just return an empty instance.
            if (pGreetingType == GreetingTypes.Invalid) return;

            //if the GreetingType is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetGreeting(pCallHandlerObjectId, pGreetingType);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Greeting not found in Greeting constructor using CallHandlerObjectID={0} " +
                                                                         "and greeting type={1}\n\rError={2}",pCallHandlerObjectId, pGreetingType, res.ErrorText));
            }
        }


        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return GreetingType.Description(); } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        //reference to the ConnectionServer object used to create this Greeting instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region Greeting Properties

        private ActionTypes _afterGreetingAction;

        /// <summary>
        /// type of call action to take, e.g., hang-up, goto another object, etc
        /// 3=Error, 2=Goto, 1=Hangup, 0=Ignore, 5=SkipGreeting, 4=TakeMsg, 6=RestartGreeting, 7=TransferAltContact, 8=RouteFromNextRule
        /// </summary>
        public ActionTypes AfterGreetingAction
        {
            get { return _afterGreetingAction; }
            set
            {
                _changedPropList.Add("AfterGreetingAction", (int)value);
                _afterGreetingAction = value;
            }
        }

        private ConversationNames _afterGreetingTargetConversation;
        public ConversationNames AfterGreetingTargetConversation
        {
            get { return _afterGreetingTargetConversation; }
            set
            {
                _changedPropList.Add("AfterGreetingTargetConversation", value.Description());
                _afterGreetingTargetConversation = value;
            }
        }

        private string _afterGreetingTargetHandlerObjectId;
        public string AfterGreetingTargetHandlerObjectId
        {
            get { return _afterGreetingTargetHandlerObjectId; }
            set
            {
                _changedPropList.Add("AfterGreetingTargetHandlerObjectId", value);
                _afterGreetingTargetHandlerObjectId = value;
            }
        }


        /// <summary>
        /// Reference to the call handler that owns this greeting.
        /// You cannot set or change this value after creation.
        /// </summary>
        [JsonProperty]
        public string CallHandlerObjectId { get; private set; }

        private bool _enableTransfer;

        /// <summary>
        /// A flag indicating when an extension is dialed at the greeting and the extension is not available whether to transfer to another extension.
        /// This is seperate from enabling/disabling the greeting rule itself which is done with the TimeExpires value.
        /// </summary>
        public bool EnableTransfer
        {
            get { return _enableTransfer; }
            set
            {
                _changedPropList.Add("EnableTransfer", value);
                _enableTransfer = value;
            }
        }

        /// <summary>
        /// The type of greeting, e.g. "Standard," "Off Hours," "Busy," etc.
        /// Alternate, Busy, Error, Internal, Off Hours, Standard, Holiday
        /// This value cannot be changed after greeting creation.
        /// </summary>
        [JsonProperty]
        public GreetingTypes GreetingType { get; private set; }

        private bool _ignoreDigits;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection takes action in response to touchtone keys pressed by callers during the greeting
        /// </summary>
        public bool IgnoreDigits
        {
            get { return _ignoreDigits; }
            set
            {
                _changedPropList.Add("IgnoreDigits", value);
                _ignoreDigits = value;
            }
        }

        /// <summary>
        /// Unique identifier for this greeting.
        /// You cannot change the objectID of a standing object.
        /// </summary>
        [JsonProperty]
        public string ObjectId { get; private set; }

        private bool _playRecordMessagePrompt;

        /// <summary>
        /// A flag indicating whether the 'Record your message at the tone' prompt prior to recording a message
        /// </summary>
        public bool PlayRecordMessagePrompt
        {
            get { return _playRecordMessagePrompt; }
            set
            {
                _changedPropList.Add("PlayRecordMessagePrompt", value);
                _playRecordMessagePrompt = value;
            }
        }

        private PlayWhatTypes _playWhat;

        /// <summary>
        /// The source for the greeting when this greeting is active.
        /// 2=NoGreeting, 1=RecordedGreeting, 0=SystemGreeting
        /// </summary>
        public PlayWhatTypes PlayWhat
        {
            get { return _playWhat; }
            set
            {
                _changedPropList.Add("PlayWhat", (int)value);
                _playWhat = value;
            }
        }

        private int _repromptDelay;

        /// <summary>
        /// The amount of time (in seconds) that Cisco Unity Connection waits without receiving any input from a caller before Cisco Unity 
        /// Connection prompts the caller again
        /// </summary>
        public int RepromptDelay
        {
            get { return _repromptDelay; }
            set
            {
                _changedPropList.Add("TimeERepromptDelayxpires", value);
                _repromptDelay = value;
            }
        }

        private int _reprompts;

        /// <summary>
        /// The number of times to reprompt a caller. After the number of times indicated here, Cisco Unity Connection performs the after-greeting action. 
        /// </summary>
        public int Reprompts
        {
            get { return _reprompts; }
            set
            {
                _changedPropList.Add("Reprompts", value);
                _reprompts = value;
            }
        }

        private DateTime _timeExpires;
        /// <summary>
        /// The date and time when this greeting expires. If the greeting rule is enabled, the value is NULL or a date in the future. 
        /// If the greeting rule is disable, the value is a past date.
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
        /// Method to set the TimeExpires value to null - this will enable the greeting forever as opposed to setting a date in the fugure 
        /// at which time it would disable itself.
        /// </summary>
        public void TimeExpiresSetNull()
        {
            _changedPropList.Add("TimeExpires", "");
        }

        //greeting stream files are fetched on the fly if referenced
        private List<GreetingStreamFile> _greetingStreamFiles;
        public List<GreetingStreamFile> GetGreetingStreamFiles()
        {
            //fetch greeting options only if they are referenced
            if (_greetingStreamFiles == null)
            {
                GetGreetingStreamFiles(out _greetingStreamFiles);
            }

            return _greetingStreamFiles;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Fetches a greeting object filled with all the properties for a specific entry identified with the ObjectId
        /// of the call handler that owns it and the greeting type name (Standard, Alternate, Off Hours...) to fetch.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that the greeting is homed on.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// The objectID of the call handler that owns the greeting to be fetched.
        /// </param>
        /// <param name="pGreetingType">
        /// The name of the greeting to fetch (Standard, Alternate, Off Hours...)
        /// </param>
        /// <param name="pGreeting">
        /// The out parameter that the instance of the Greeting class filled in with the details of the fetched entry is
        /// passed back on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetGreeting(ConnectionServer pConnectionServer,
                                                        string pCallHandlerObjectId,
                                                        GreetingTypes pGreetingType,
                                                        out  Greeting pGreeting)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pGreeting = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetGreeting";
                return res;
            }

            if (pGreetingType == GreetingTypes.Invalid)
            {
                res.ErrorText = "Invalid greeting type passed to GetGreeting";
                return res;
            }

            //create a new greeting instance passing the greeting type name which fills out the data automatically
            try
            {
                pGreeting = new Greeting(pConnectionServer, pCallHandlerObjectId, pGreetingType);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch greeting in GetGreeting:" + ex.Message;
            }

            return res;

        }


        /// <summary>
        /// Returns all the greetings for a call handler. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the greetings are being fetched from.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// GUID identifying the call handler that owns the greetings being fetched
        /// </param>
        /// <param name="pGreetings">
        /// The list of Greeting objects are returned using this out parameter.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetGreetings(ConnectionServer pConnectionServer,
                                                            string pCallHandlerObjectId,
                                                           out List<Greeting> pGreetings)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pGreetings = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetGreetings";
                return res;
            }

            string strUrl = string.Format("{0}handlers/callhandlers/{1}/greetings", pConnectionServer.BaseUrl, pCallHandlerObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's an error - there should always been greetings returned.
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount ==0)
            {
                pGreetings = new List<Greeting>();
                res.Success = false;
                return res;
            }

            pGreetings = HTTPFunctions.GetObjectsFromJson<Greeting>(res.ResponseText);

            if (pGreetings == null)
            {
                pGreetings = new List<Greeting>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pGreetings)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.CallHandlerObjectId = pCallHandlerObjectId;
                oObject.ClearPendingChanges();
            }

            return res;
        }



        /// <summary>
        /// Allows one or more properties on a greeting to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be
        /// passed in but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the greeting is homed.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// Unique identifier for call handler that owns the greeting being updated
        /// </param>
        /// <param name="pGreetingType">
        /// Name of the greeting rule to update (Alternate, Standard, Off Hours, Internal, Holiday, Error, Busy)
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a greeting property name and a new value for that property to apply to the option 
        /// being updated. This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one
        /// property pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateGreeting(ConnectionServer pConnectionServer,
                                                        string pCallHandlerObjectId,
                                                        GreetingTypes pGreetingType,
                                                        ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateGreeting";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList==null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateGreeting";
                return res;
            }

            string strBody = "<Greeting>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Greeting>";

            return HTTPFunctions.GetCupiResponse(string.Format("{0}handlers/callhandlers/{1}/greetings/{2}", pConnectionServer.BaseUrl, pCallHandlerObjectId, 
                pGreetingType.Description()),MethodType.PUT,pConnectionServer,strBody,false);

        }


        /// <summary>
        /// Special helper function for dealing with the enabled/disabled status of greeting.  This can be done directly using the timeExpiresSetNull
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
        /// Connection server that the greeting option being edited lives on.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// The GUID that identifies the call handler which owns the greeting option being edited.
        /// </param>
        /// <param name="pGreetingType">
        /// The greeting type being edited (Standard, Off Hours, Alternate etc...).  You cannot edit the Standard greeting option in this routine, it is always
        /// set to enabled on the server and that cannot be changed.
        /// </param>
        /// <param name="pEnabled">
        /// Pass TRUE to enable the greeting option, FALSE to disable it.  If you pass TRUE you may optionally pass a pTillDate DateTime property to indicate the
        /// date the rule will disable itself automatically.
        /// </param>
        /// <param name="pTillDate">
        /// Optional parameter that can be passed when pEnabled is passed as TRUE.  This must be a date/time in the future, a date/time in the past will result in 
        /// the method returning a failure.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateGreetingEnabledStatus(ConnectionServer pConnectionServer,
                                                        string pCallHandlerObjectId,
                                                        GreetingTypes pGreetingType,
                                                        bool pEnabled,
                                                        DateTime? pTillDate = null)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to ChangeGreetingEnabledStatus";
                return res;
            }

            if (string.IsNullOrEmpty(pCallHandlerObjectId))
            {
                res.ErrorText = "Empty handler ObjectId or GreetingType passed to ChangeGreetingEnabledStatus";
                return res;
            }

            //first make sure the user isn't trying to change the enabled status on the Standard greeting - this will fail since that option needs to 
            //always be enabled on the server - fail it up front and pass back the WebCallResult with this information.
            if (pGreetingType == GreetingTypes.Standard)
            {
                res.ErrorText = "Attempt made to modify Standard greeting in ChangeGreetingEnabledStatus.";
                return res;
            }

            //error greetings cannot be turned off either
            if (pGreetingType == GreetingTypes.Error)
            {
                res.ErrorText = "Attempt made to modify Error greeting in ChangeGreetingEnabledStatus.";
                return res;
            }

            
            //invalid 
            if (pEnabled == false & (pTillDate != null))
            {
                res.ErrorText = "A date ending time was passed along with enabled=false in ChangeGreetingEnabledStatus.";
                return res;
            }

            //finally - no date in the past is valid
            if ((pTillDate != null) && (pTillDate < DateTime.Now))
            {
                res.ErrorText = "A  pTillDate in the past was passed in ChangeGreetingEnabledStatus.";
                return res;
            }

            //ok, everthing looks valid, make the change.  The enabled/disaled status of a greeting rides on the TimeExpires field.
            ConnectionPropertyList oProp = new ConnectionPropertyList();

            if (pEnabled == false)
            {
                //use the 10/11/1999 date to disable the greeting.
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

            string strBody = "<Greeting>";

            foreach (var oPair in oProp)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Greeting>";

            return HTTPFunctions.GetCupiResponse(string.Format("{0}handlers/callhandlers/{1}/greetings/{2}", pConnectionServer.BaseUrl, pCallHandlerObjectId, 
                pGreetingType.Description()),MethodType.PUT,pConnectionServer,strBody,false);
        }



        /// <summary>
        /// Sets the greeting recording for a particular language on a selected greeting.  The WAV file is uploaded (after optionally being
        /// converted to a format Conneciton will accept), however the "play what" field is not automatically set to play the custom recorded
        /// greeting, that needs to be done on the greeting itself.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that houses the greeting being edited.
        /// </param>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system to the WAV file to be uploaded as the greeting.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// The GUID identifying the call handler that owns the greeting being edited.
        /// </param>
        /// <param name="pGreetingType">
        /// The greeting type being edited (Standard, Off Hours, Alternate, Busy, Internal, Holiday, Error).
        /// </param>
        /// <param name="pLanguageId">
        /// The language ID of the WAV file being uploaded (for US English this is 1033).  The LanguageCodes enum defined in the ConnectionTypes
        /// class can be helpful here.  The language must be installed and active on the Connection server for this to be allowed.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// Defaults to false, but if passed as true this has the target WAV file first converted PCM, 16 Khz, 8 bit mono before uploading.  This 
        /// helps ensure Connection will not complain about the media file format.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetGreetingWavFile(ConnectionServer pConnectionServer,
                                                        string pSourceLocalFilePath,
                                                        string pCallHandlerObjectId,
                                                        GreetingTypes pGreetingType,
                                                        int pLanguageId,
                                                        bool pConvertToPcmFirst = false)
        {
            WebCallResult res = new WebCallResult();
            
            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetGreetingWavFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pSourceLocalFilePath) || (File.Exists(pSourceLocalFilePath) == false))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid local file path passed to SetGreetingWavFile: " + pSourceLocalFilePath;
                return res;
            }

            //if the user wants to try and rip the WAV file into G711 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                string strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pSourceLocalFilePath);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into G711 format in SetGreetingWavFile.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted G711 WAV file path not found in SetGreetingWavFile: " +
                                    strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pSourceLocalFilePath = strConvertedWavFilePath;

            }

            //new construction - requires 8.5 or later and is done in one step to send the greeting to the server.
            string strGreetingStreamUriPath= string.Format("https://{0}:8443/vmrest/handlers/callhandlers/{1}/greetings/{2}/greetingstreamfiles/{3}/audio",
                                         pConnectionServer.ServerName, pCallHandlerObjectId, pGreetingType.Description(), pLanguageId);

            return HTTPFunctions.UploadWavFile(strGreetingStreamUriPath, pConnectionServer, pSourceLocalFilePath);
        }


        /// <summary>
        /// If you have a recording stream already recorded and in the stream files table on the Connection server (for instance
        /// you are using the telephone as a media device) you can assign a greeting stream file directly to a greeting using this 
        /// method instead of uploading a WAV file from the local hard drive.
        /// </summary>
        /// <param name="pConnectionServer" type="ConnectionServer">
        ///   The Connection server that houses the greeting to be updated      
        /// </param>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the greeting stream to be assigned.
        /// </param>
        /// <param name="pCallHandlerObjectId"> 
        /// The GUID identifying the call handler that owns the greeting being edited.
        /// </param>
        /// <param name="pGreetingType">
        /// The greeting type being edited (Standard, Off Hours, Alternate, Busy, Internal, Holiday, Error).
        /// </param>
        /// <param name="pLanguageId">
        /// The language ID of the WAV file being uploaded (for US English this is 1033).  The LanguageCodes enum defined in the ConnectionTypes
        /// class can be helpful here.  The language must be installed and active on the Connection server for this to be allowed.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetGreetingRecordingToStreamFile(ConnectionServer pConnectionServer,
                                                     string pStreamFileResourceName,
                                                     string pCallHandlerObjectId,                                         
                                                     string pGreetingType,
                                                     int pLanguageId)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetGreetingRecordingToStreamFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pStreamFileResourceName))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid stream file resource id passed to SetGreetingRecordingToStreamFile";
                return res;
            }

            //construct the full URL to call for updating the greeting to a stream file id
            string strUrl = string.Format(@"{0}handlers/callhandlers/{1}/treetings/{2}/greetingstreamfiles/{3}", 
                    pConnectionServer.BaseUrl,pCallHandlerObjectId, pGreetingType, pLanguageId);

            Dictionary<string, string> oParams = new Dictionary<string, string>();
            Dictionary<string, object> oOutput;

            oParams.Add("op", "RECORD");
            oParams.Add("ResourceType", "STREAM");
            oParams.Add("resourceId", pStreamFileResourceName);
            oParams.Add("lastResult", "0");
            oParams.Add("speed", "100");
            oParams.Add("volume", "100");
            oParams.Add("startPosition", "0");

            return HTTPFunctions.GetJsonResponse(strUrl, MethodType.PUT, pConnectionServer, oParams, out oOutput);
        }



        /// <summary>
        /// When updating a greeting wav file you need to do a POST if it's not there already and a PUT if there is - so we need a quick way to indicate if 
        /// a streamfile for a particular greeting exists or not.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that owns the greeting being checked.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// GUID that identifies call handler that owns the greeting being checked.
        /// </param>
        /// <param name="pGreetingType">
        /// Greeting type (Standard, Off Hours, Busy, Internal, Alternate, Holiday, Error).
        /// </param>
        /// <param name="pLanguageId">
        /// Language ID of the stream file to check for (i.e. English US = 1033)
        /// </param>
        /// <returns>
        /// True if the greeting stream already exists, false if it does not.
        /// </returns>
        private static bool DoesGreetingStreamExist(ConnectionServer pConnectionServer,
                                                        string pCallHandlerObjectId,
                                                        GreetingTypes pGreetingType,
                                                        int pLanguageId)
        {
            string strUrl = string.Format("{0}handlers/callhandlers/{1}/greetings/{2}/greetingstreamfiles/{3}",
                                          pConnectionServer.BaseUrl, 
                                          pCallHandlerObjectId, 
                                          pGreetingType.Description(), 
                                          pLanguageId);
            
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            //the only reason this will fail is if it doesn't exists - return that here
            return res.Success;
        }


        #endregion


        #region Instance Methods


        /// <summary>
        /// Greeting display function - outputs the type, playWhat and if it's enabled
        /// </summary>
        /// <returns>
        /// String describing the greeting option
        /// </returns>
        public override string ToString()
        {
            //there's no "Active" item included in the greeting information from the server which is unfortunate - need to pull the TimeExpires property and
            //determine if we're active based on that.
            bool bActive = this.TimeExpires>DateTime.Now;

            return string.Format("Greeting type={0}, Play What={1} ({2}), enabled={3}", GreetingType, PlayWhat, (PlayWhatTypes)PlayWhat,bActive);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the greeting object in "name=value" format - each pair is on its
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
        /// Fetches a greeting object filled with all the properties for a specific entry identified with the ObjectId
        /// of the call handler that owns it and the name of the greeting (Standard, Alternate, Off Hours etc...)
        /// </summary>
        /// <param name="pCallHandlerObjectId">
        /// The objectID of the call handler that owns the greeting to be fetched.
        /// </param>
        /// <param name="pGreetingType">
        /// The name of the greeting option to fetch (Standard, Alternate, Off Hours, Busy, Internal, Error, Holiday)
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetGreeting(string pCallHandlerObjectId, GreetingTypes pGreetingType)
        {
            string strUrl = string.Format("{0}handlers/callhandlers/{1}/greetings/{2}", HomeServer.BaseUrl, pCallHandlerObjectId, pGreetingType.Description());

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(HTTPFunctions.StripJsonOfObjectWrapper(res.ResponseText, "Greeting"), this,
                    HTTPFunctions.JsonSerializerSettings);
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
      
        //helper function to fetch all greeting stream files associated with this greeting (if any).
        //If there are no custom recorded greetings the pGreetingStreamFiles out param is returned as null.
        private WebCallResult GetGreetingStreamFiles(out List<GreetingStreamFile> pGreetingStreamFiles)
        {
            return GreetingStreamFile.GetGreetingStreamFiles(HomeServer, CallHandlerObjectId, GreetingType, out pGreetingStreamFiles);
        }

        /// <summary>
        /// Pass in the greeting option type (Standard, Off Hours, Alternate) and this will return an instance of the GreetingStreamFile class for that
        /// greeting rule (if found) for the language code provided.  
        /// </summary>
        /// <remarks>
        /// This routine will fetch the full list of greeting options if they have not yet been fetched for this handler and return the one of interest.
        /// If the greeting options have already been fetched it simply returns the appropriate instance.
        /// </remarks>
        /// <param name="pLanguageCode">
        /// The language code to fetch (1033=US English for instance).  The LanguageCodes enum defined in the ConnectionTypes class can be helpful here.
        /// </param>
        /// <param name="pGreetingStreamFile">
        /// Out param on which the greeting option is passed.  If there is an error finding the option then null is returned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetGreetingStreamFile(int pLanguageCode, out GreetingStreamFile pGreetingStreamFile)
        {
            WebCallResult res;
            
            pGreetingStreamFile = null;

            //fetch the full greeting stream files list if it hasn't been fetched yet.
            if (_greetingStreamFiles == null)
            {
                res = GetGreetingStreamFiles( out _greetingStreamFiles);

                //if there's some sort of error getting the list, pass it back and bail.
                if (res.Success == false)
                {
                    return res;
                }
            }

            //get the correct stream off the list
            res = new WebCallResult();

            foreach (GreetingStreamFile oStream in _greetingStreamFiles)
            {
                if (oStream.LanguageCode == pLanguageCode)
                {
                    pGreetingStreamFile = oStream;
                    res.Success = true;
                    return res;
                }
            }

            //if we're here then there was a probllem
            res.Success = false;
            res.ErrorText = "Could not find greeting stream file using language code=" + pLanguageCode.ToString();
            return res;
        }



        /// <summary>
        /// Helper function for uploading a local WAV file as a greeting stream file for a particular greeting on a call handler for a 
        /// specific language.
        /// It's highly recommended you use the ConvertToPCMFirst flag, although it defaults to false.  This not only converts the target
        /// WAV file into PCM but also sets the bitrate and channels to settings that Connection is happy with.  The WAV file format checking
        /// in the CUPI interface can be very picky about format.
        /// </summary>
        /// <param name="pLanguageId">
        /// The language ID of the WAV file you are uploading - this language must be installed and active on the Connection server.  For US 
        /// English the code is 1033.  The LanguageCodes enum defined in the ConnectionTypes class has a full list you can leverage here.
        /// </param>
        /// <param name="pSourceLocalFilePath">
        /// Full path to the WAV file to upload on the local file system.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// If set to true the file is converted into PCM and forced into 16 Khz, 8 bit mono format which Connection will be happy with.  It handles
        /// numerous wav file formats including MP3, GSM 6.10 and G729a among others.  
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface. 
        /// </returns>
        public WebCallResult SetGreetingWavFile(int pLanguageId, string pSourceLocalFilePath, bool pConvertToPcmFirst=false)
        {
            return SetGreetingWavFile(HomeServer, pSourceLocalFilePath, CallHandlerObjectId, GreetingType, pLanguageId,pConvertToPcmFirst);
            
        }

        /// <summary>
        /// If you have a recording stream already recorded and in the stream files table on the Connection server (for instance
        /// you are using the telephone as a media device) you can assign a greeting stream file directly to a greeting using this 
        /// method instead of uploading a WAV file from the local hard drive.
        /// </summary>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the greeting stream to be assigned.
        /// </param>
        /// <param name="pCallHandlerObjectId"> 
        /// The GUID identifying the call handler that owns the greeting being edited.
        /// </param>
        /// <param name="pGreetingType">
        /// The greeting type being edited (Standard, Off Hours, Alternate, Busy, Internal, Holiday, Error).
        /// </param>
        /// <param name="pLanguageId">
        /// The language ID of the WAV file being uploaded (for US English this is 1033).  The LanguageCodes enum defined in the ConnectionTypes
        /// class can be helpful here.  The language must be installed and active on the Connection server for this to be allowed.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetGreetingRecordingToStreamFile(string pStreamFileResourceName,
                                                     string pCallHandlerObjectId,                                         
                                                     string pGreetingType,
                                                     int pLanguageId)
        {
            return SetGreetingRecordingToStreamFile(HomeServer, pStreamFileResourceName, pCallHandlerObjectId,pGreetingType, pLanguageId);
        }


        /// <summary>
        /// If the greeting object has any pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }


        /// <summary>
        /// Allows one or more properties on a greeting option to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the greeting intance has any pending changes, if not return false with an appropriate error message
            if (_changedPropList.Count == 0)
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for greeting:{0}, objectid=[{1}]",
                                              this, this.ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateGreeting(HomeServer, CallHandlerObjectId, GreetingType, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }


         /// <summary>
        /// Special helper function for dealing with the enabled/disabled status of greeting.  This can be done directly using the timeExpiresSetNull
        /// option found in the propertis seciton above but it's less than intuative and most users do not get how the timeExpires functionality works for 
        /// greeting options and greeting rules - as such this routine wraps it up in a simple single call option.
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
        /// Pass TRUE to enable the greeting option, FALSE to disable it.  If you pass TRUE you may optionally pass a pTillDate DateTime property to indicate the
        /// date the rule will disable itself automatically.
        /// </param>
        /// <param name="pTillDate">
        /// Optional parameter that can be passed when pEnabled is passed as TRUE.  This must be a date/time in the future, a date/time in the past will result in 
        /// the method returning a failure.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult UpdateGreetingEnabledStatus(bool pEnabled,DateTime? pTillDate = null)
         {
             ClearPendingChanges();
             return UpdateGreetingEnabledStatus( HomeServer, CallHandlerObjectId, GreetingType, pEnabled,pTillDate);
         }

        #endregion

    }
}
