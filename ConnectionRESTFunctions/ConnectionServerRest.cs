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
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace Cisco.UnityConnection.RestFunctions
{
    // Class to hold the Connection version information - this is gathered when the server is attached to along with a few other
    // commonly needed items such as the primary location object ID.
    #region Version Class
    [Serializable]
    public class ConnectionVersion
    {
        public int Major;
        public int Minor;
        public int Build;
        public int Rev;
        public int Es;

        //constructor requires all values for version number - pass 0 for values that are not present.
        public ConnectionVersion(int pMajor, int pMinor, int pBuild, int pRev, int pEs) 
        {
            Major = pMajor;
            Minor = pMinor;
            Build = pBuild;
            Rev = pRev;
            Es = pEs;
        }

        //displays version in a readable format for logging and display purposes.
        public override string ToString()
        {
            if (Es>0)
            {
                return string.Format("{0}.{1}({2}.{3}) ES {4}",Major,Minor,Rev, Build,Es);
            }
            return string.Format("{0}.{1}({2}.{3})",Major,Minor,Rev,Build);
        }


        /// <summary>
        /// Returns true if the version of Connection installed on the server currently attached is the same as or newer (greater) than
        /// the version passed in.  This is useful for situations where data schemas vary across versions of Connection.
        /// </summary>
        /// <param name="pMajor">
        /// Major version.
        /// </param>
        /// <param name="pMinor">
        /// Minor version
        /// </param>
        /// <param name="pBuild">
        /// Build version
        /// </param>
        /// <param name="pRev">
        /// Rev version
        /// </param>
        /// <param name="pEs">
        /// Engineering special number
        /// </param>
        /// <returns>
        /// TRUE is returned if the version passed in is the same as or less than the version of Connection.
        /// </returns>
        public bool IsVersionAtLeast(int pMajor, int pMinor, int pRev, int pBuild, int pEs = 0)
        {
            if (Major > pMajor
                | Major == pMajor & Minor > pMinor
                | Major == pMajor & Minor == pMinor & Rev > pRev
                | Major == pMajor & Minor == pMinor & Rev == pRev & Build > pBuild
                | Major == pMajor & Minor == pMinor & Rev == pRev & Build == pBuild & Es >= pEs)
            {
                return true;
            }
            return false;
        }
    }

    #endregion

    /// <summary>
    /// Primary class for teh ConnectionRestFunctions library.  This is used to connect to and get/set data to and from a remote Connection
    /// server via the CUPI REST based web interface.
    /// </summary>
    [Serializable]
    public class ConnectionServerRest
    {

        #region Fields and Properties

        private readonly IConnectionRestCalls _transportFunctions;

        /// <summary>
        /// When in debug mode missing member flags are output to the command window, otherwise 
        /// </summary>
        public bool DebugMode { 
            get { return _transportFunctions.DebugMode; } 
            set{_transportFunctions.DebugMode = value;} 
        }

        /// <summary>
        /// By default HTTP timeout is 15 seconds.
        /// </summary>
        public int HttpTimeoutSeconds
        {
            get { return _transportFunctions.HttpTimeoutSeconds; }
            set { _transportFunctions.HttpTimeoutSeconds = value; }
        }


        //values set at class creation time that are read only externally
        public ConnectionVersion Version { get; private set; }
        public  string ServerName { get; private set; }
        public  string LoginName { get; private set; }
        public  string LoginPw { get; private set; }

        //the start of the URL construction to get to the VMRest calls for the server this instance is pointing to.
        public string BaseUrl { get; private set; }

        //keeps track of the last session cookie we got from this server via HTTPS
        public string LastSessionCookie { get; set; }

        //keeps track of the last time we sent anything or got anything to/from this server via HTTPS
        public DateTime TimeSessionCookieIssued { get; set; }

        //information about cluster servers (if any) filled in at login time.  If there is no cluster the list
        //will contain only the same server being attached to by this instance of ConnectionServer.
        public List<VmsServer> VmsServers { get; private set; }

        //lazy fetch implementation for fetching the primary location objectId for the Connection server this instance is 
        //pointing to.
        private Location _primaryLocation;
        public string PrimaryLocationObjectId { 
            get
            {
                if (_primaryLocation == null)
                {
                    _primaryLocation = GetPrimaryLocation();
                }
                
                if (_primaryLocation == null)
                {
                    return "";
                }

                return _primaryLocation.ObjectId;
            }  
        }

        //Lazy fetch for getting the VMSserver ObjectID
        private VmsServer _vmsServer;
        public string VmsServerObjectId
        {
            get
            {
                if (_vmsServer == null)
                {
                    _vmsServer = GetVmsServer();
                }

                if (_vmsServer == null) return "";
                return _vmsServer.VmsServerObjectId;
            }
        }

        //Laxy fetch for getting VMSServer name.
        public string VmsServerName
        {
            get
            {
                if (_vmsServer == null)
                {
                    _vmsServer = GetVmsServer();
                }
                if (_vmsServer == null) return "";

                return _vmsServer.ServerName;
            }
        }

        #endregion 


        #region Constructors and Destructors

        /// <summary>
        /// default constructor - initalize everything to blank/0s and create a RestTransportFunctions instance
        /// if one is not provided.
        /// </summary>
        public ConnectionServerRest(IConnectionRestCalls pTransportFunctions, bool pAllowSelfSignedCertificates=true)
        {
            ServerName = "";
            LoginName = "";
            LoginPw = "";
            BaseUrl = "";
            Version = new ConnectionVersion(0, 0, 0, 0, 0);

            if (pTransportFunctions == null)
            {
                _transportFunctions=new RestTransportFunctions(pAllowSelfSignedCertificates);
                return;
            }

            _transportFunctions = pTransportFunctions;
        }

        /// <summary>
        /// Constructor for the ConnectionServer class that allows the caller to provide the server name, login name and login password used to 
        /// authenticate to the CUPI interface on a Connection server.
        /// </summary>
        /// <param name="pTransportFunctions">
        /// Handle to a class that implements the IConnectionRestCalls interface that this instance of ConnectionServer will use to communicate 
        /// with Unity Connection via REST.
        /// </param>
        /// <param name="pServerName">
        /// Name or IP address of the remote Connection server to attach to.
        /// </param>
        /// <param name="pLoginName">
        /// Login name used to authenticate to the CUPI interface on the Connection server provided.
        /// </param>
        /// <param name="pLoginPw">
        /// Login password used to authenticate to the CUPI interface on the Connection server provided.
        /// </param>
        /// <param name="pLoginAsAdministrator">
        /// When validating the login credentials of a user (as opposed to an admin) pass this as false instead
        /// </param>
        /// <param name="pAllowSelfSignedCertificates">
        /// If passed as true the errors about self signed certificates will be suppressed - this is the default.  Passed as false 
        /// only valid signed certificates will be accepted when authenticating with a server.
        /// </param>
        /// <returns>
        /// Instance of the ConnectionServer class
        /// </returns>
        public ConnectionServerRest (IConnectionRestCalls pTransportFunctions, string pServerName, string pLoginName, string pLoginPw,
            bool pLoginAsAdministrator = true, bool pAllowSelfSignedCertificates = true)
            : this(pTransportFunctions,pAllowSelfSignedCertificates)
        {
            BaseUrl = string.Format("https://{0}:8443/vmrest/", pServerName);
            Version = new ConnectionVersion(0,0,0,0,0);
            
            if (string.IsNullOrEmpty(pServerName) | string.IsNullOrEmpty(pLoginName) | string.IsNullOrEmpty(pLoginPw))
            {
                throw new ArgumentException("Empty server name, login name or password provided on constructor");
            }

            //starting with 10.0 we need to be careful managing our basic authentication requests to Connection and keep our cookies and 
            //tokens around - these values are part of that system
            LastSessionCookie = "";
            TimeSessionCookieIssued = DateTime.MinValue;

            //validate login.  This fills in the version and primary location object ID details.
            var res = LoginToConnectionServer(pServerName, pLoginName, pLoginPw, pLoginAsAdministrator);
            if (res.Success == false)
            {
                ServerName = "";
                LoginName = "";
                LoginPw = "";
                BaseUrl = "";
                throw new UnityConnectionRestException(res, "Login failed to Connection server:"+pServerName+". Details="+res);
            }

            //register for error and logging events to the transport mechanism. 
            _transportFunctions.DebugEvents += TransportFunctionsOnDebugEvents;
            _transportFunctions.ErrorEvents += TransportFunctionsOnErrorEvents;
            
            if (RestTransportFunctions.JsonSerializerSettings != null)
            {
                RestTransportFunctions.JsonSerializerSettings.Error += JsonParseError;
            }

            ServerName = pServerName;
            LoginName = pLoginName;
            LoginPw = pLoginPw;
        }

        

        /// <summary>
        /// Constructor for the ConnectionServer class that allows the caller to provide the server name, login name and login password 
        /// used to authenticate to the CUPI interface on a Connection server.  Defaults to using the RestTransportFunctions version 
        /// of the IConnectionRestCalls interface.
        /// </summary>
        /// <param name="pServerName">
        /// Name or IP address of the remote Connection server to attach to.
        /// </param>
        /// <param name="pLoginName">
        /// Login name used to authenticate to the CUPI interface on the Connection server provided.
        /// </param>
        /// <param name="pLoginPw">
        /// Login password used to authenticate to the CUPI interface on the Connection server provided.
        /// </param>
        /// <param name="pLoginAsAdministrator">
        /// When validating the login credentials of a user (as opposed to an admin) pass this as false instead
        /// </param>
        /// <param name="pAllowSelfSignedCertificates">
        /// If passed as true the errors about self signed certificates will be suppressed - this is the default.  Passed as false 
        /// only valid signed certificates will be accepted when authenticating with a server.
        /// </param>
        /// <returns>
        /// Instance of the ConnectionServer class
        /// </returns>
        public ConnectionServerRest(string pServerName, string pLoginName, string pLoginPw,
            bool pLoginAsAdministrator = true, bool pAllowSelfSignedCertificates=true)
            : this(null, pServerName, pLoginName, pLoginPw, pLoginAsAdministrator, pAllowSelfSignedCertificates)
        {
        }

        #endregion


        #region Wrapped REST Calls

        /// <summary>
        /// Primary method for sending/fetching data to and from the Connection server via CUPI - tries to parse results returned 
        /// into XML format if XML response is used (pass pJsonResponse = true to skip that).  Results are contained in the WebCallResult 
        /// class returned.
        /// </summary>
        /// <param name="pUrl">
        /// Full URL to send to Connection - format should look like:
        /// https://{Connection Server Name}:8443/vmrest/users
        /// </param>
        /// <param name="pMethod">
        /// GET, PUT, POST, DELETE method type
        /// </param>
        /// <param name="pRequestBody">
        /// If the command (for instance a POST) include the need for a post body for additional data, include it here.  Not all commands
        /// require this (GET calls for instance).
        /// </param>
        /// <param name="pJsonResponse">
        /// Defaults to getting JSON body as a response, if passed as false XML will be requested instead.
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... 
        /// associated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        public WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, string pRequestBody, bool pJsonResponse = true)
        {
            return _transportFunctions.GetCupiResponse(pUrl, pMethod, this, pRequestBody, pJsonResponse);
        }


        /// <summary>
        /// Overload for the GetCupiResponse that takes a simple string/string dictionary that is assumed to be the body of a 
        /// JSON based CUPI request - the dictionary is constructed into a simple json string and inserted into the body of the 
        /// request.
        /// </summary>
        /// <param name="pUrl">
        /// Full URL to send to Connection - format should look like:
        /// https://{Connection Server Name}:8443/vmrest/users
        /// </param>
        /// <param name="pMethod">
        /// GET, PUT, POST, DELETE method type
        /// </param>
        /// <param name="pRequestDictionary">
        /// a string/string dictionary containing the parameters to send in the body of the request formatted for JSON
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... 
        /// associated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        public WebCallResult GetCupiResponse(string pUrl, MethodType pMethod,Dictionary<string, string> pRequestDictionary)
        {
            return _transportFunctions.GetCupiResponse(pUrl, pMethod, this, pRequestDictionary);
        }


        /// <summary>
        /// This routine is used for download a WAV file from a remote Connection server for a voice name or greeting.  Note that this cannot be
        /// used for downloading messages (voice mail or broadcast messages) or prompts - this is only used for voice names and greetings at 
        /// present.
        /// </summary>
        /// <remarks>
        /// This is a general purpose WAV download routine that can be used for greetings, voice names or interview handlers - this does
        /// not leverage the URI style media specific formats of the REST interface into CUPI, but is the existing CUALS web interface that's
        /// been in place since 7.0(2) and will work across all versions of Connection (COBRAS uses this).  I'm using it here to simplify
        /// fetching media files through a single method here - all that's needed is the actual WAV file name (GUID followed by a .wav).
        /// </remarks>
        /// <param name="pLocalWavFilePath" type="string">
        /// The full path to stored the downloaded WAV file locally.
        /// </param>
        /// <param name="pConnectionFileName" type="string">
        /// The file name on the remote Connection server to download.
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        /// associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>    
        public WebCallResult DownloadWavFile(string pLocalWavFilePath,string pConnectionFileName)
        {
            return _transportFunctions.DownloadWavFile(this, pLocalWavFilePath, pConnectionFileName);
        }


        /// <summary>
        /// This routine is used for download a message attachment from a remote Connection server for a voice name or greeting.  
        /// </summary>
        /// <remarks>
        /// This is a general purpose WAV download routine that can be used for all binary attachments for messages (i.e. WAV files).
        /// </remarks>
        /// <param name="pBaseUrl">
        /// Base URL for VMRest - usually something like "https://(server name):8442/vmrest/".  This is created and stored off the 
        /// ConnserverServer object when it's created.
        /// </param>
        /// <param name="pLocalWavFilePath" type="string">
        /// The full path to stored the downloaded WAV file locally.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID identifying the user that owns the message being fetched.
        /// </param>
        /// <param name="pMessageObjectId">
        /// The GUID identifying the specific message to fetch an attachment for.
        /// </param>
        /// <param name="pAttachmentNumber">
        /// Zero based number indicating which attachment for a message to fetch.
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        /// associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>    
        public WebCallResult DownloadMessageAttachment(string pBaseUrl, string pLocalWavFilePath,
                                                       string pUserObjectId, string pMessageObjectId,
                                                       int pAttachmentNumber)
        {
            return _transportFunctions.DownloadMessageAttachment(pBaseUrl, this, pLocalWavFilePath, pUserObjectId, pMessageObjectId,
                                       pAttachmentNumber);
        }

        /// <summary>
        /// This routine is used for upload a WAV file to a remote Connection server for a voice name or greeting.  Note that this cannot be used
        /// for uploading messages (voice mail or broadcast messages) or prompts - this is only used for voice names and greetings at present.
        /// You must first allocate a stream file name on the Connection server which is passed into this routine along with the server name, 
        /// login and password.
        /// </summary>
        /// <param name="pFullResourcePath" type="string">
        /// Path to the resource stream.  For instance a user's voice name resource path look something like:
        /// https://ConnectionServer1.MyCompany.com:8443/vmrest/users/51e94483-2dec-43b1-974e-2b9320b86d78/voicename
        /// </param>
        /// <param name="pLocalWavFilePath" type="string">
        /// The full path to the local WAV file to upload to the remote Connection server.
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        /// associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        public WebCallResult UploadWavFile(string pFullResourcePath, string pLocalWavFilePath)
        {
            return _transportFunctions.UploadWavFile(pFullResourcePath, this, pLocalWavFilePath);
        }


        /// <summary>
        /// Upload a new message to the Connection server using a local WAV file as the voice mail attachment.
        /// </summary>
        /// <param name="pPathToLocalWav">
        /// The path to the local WAV file on the hard drive to include as the message attachment. 
        /// </param>
        /// <param name="pMessageDetailsJsonString">
        /// Message details in JSON form - should look something like this:
        /// {\"Subject\":\""+pSubject+" \",\"ArrivalTime\":\"0\",\"FromSub\":\"false\",\"FromVmIntSub\":\"false\"}"
        /// </param>
        /// <param name="pSenderUserObjectId">
        /// ObjectId of the subscriber (user with a mailbox) that the message will be sent on behalf of.
        /// </param>
        /// <param name="pRecipientJsonString">
        /// Json format string for the list of recipients to address the message to - any number of TO, CC or BCC address types can be included.
        /// </param>
        /// <param name="pUriConstruction">
        /// Defaults to blank - if passed it is used for the URI instead of assuming a construction for new message upload.  Used for 
        /// forwards and replies.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details about the call and results recieved back from the server.
        /// </returns>
        public WebCallResult UploadVoiceMessageWav(string pPathToLocalWav,string pMessageDetailsJsonString, 
                                                   string pSenderUserObjectId,string pRecipientJsonString,
                                                   string pUriConstruction = "")
        {
            return _transportFunctions.UploadVoiceMessageWav(this, pPathToLocalWav, pMessageDetailsJsonString,
                                                             pSenderUserObjectId, pRecipientJsonString, pUriConstruction);
        }


        /// <summary>
        /// Create a new message using a resourceID for a stream already recorded on the Connection server - used when leveraging 
        /// CUTI to create new messages.
        /// </summary>
        /// <param name="pResourceId">
        /// The resourceId of the stream file to use for the voice message attachment
        /// </param>
        /// <param name="pMessageDetailsJsonString">
        /// Message details in JSON form - should look something like this:
        /// {\"Subject\":\""+pSubject+" \",\"ArrivalTime\":\"0\",\"FromSub\":\"false\",\"FromVmIntSub\":\"false\"}"
        /// </param>
        /// <param name="pSenderUserObjectId">
        /// ObjectId of the subscriber (user with a mailbox) that the message will be sent on behalf of.
        /// </param>
        /// <param name="pRecipientJsonString">
        /// Json format string for the list of recipients to address the message to - any number of TO, CC or BCC address types can be included.
        /// </param>
        /// <param name="pUriConstruction">
        /// Defaults to blank - if passed it is used for the URI instead of assuming a construction for new message upload.  Used for 
        /// forwards and replies.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details about the call and results recieved back from the server.
        /// </returns>
        public WebCallResult UploadVoiceMessageResourceId(string pResourceId,string pMessageDetailsJsonString, 
                                                          string pSenderUserObjectId,string pRecipientJsonString,
                                                          string pUriConstruction = "")
        {
            return _transportFunctions.UploadVoiceMessageResourceId(this, pResourceId, pMessageDetailsJsonString,
                                                                    pSenderUserObjectId, pRecipientJsonString,
                                                                    pUriConstruction);
        }

        /// <summary>
        /// This routine will generate a temporary WAV file name on Connecton and uplaod the local WAV file to that location.  If it completes the 
        /// Connection stream file name will be returned and can be assigned as the stream file property for a voice name, greeting or interview 
        /// handler question.  
        /// Note that these wav files can NOT be used for messages (regular or dispatch).
        /// This is an older construction and is only needed for interview handler questions currently.
        /// </summary>
        /// <param name="pLocalWavFilePath">
        /// Full path on the local file system for the WAV file to uplaod.
        /// </param>
        /// <param name="pConnectionStreamFileName">
        /// Returns the stream file name (GUID.wav) from the Connection server that the WAV file was uploaded to - you can use this value to update
        /// the StreamFile property for greetings for instance.
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        /// associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        public WebCallResult UploadWavFileToStreamLibrary(string pLocalWavFilePath,out string pConnectionStreamFileName)
        {
            return _transportFunctions.UploadWavFileToStreamLibrary(this, pLocalWavFilePath,out pConnectionStreamFileName);
        }

        #endregion


        #region Logging and Error Events

        /// <summary>
        /// Event handle for external clients to register with so they can get logging events on errors and warnings that happen
        /// within this class.
        /// </summary>
        public event LoggingEventHandler ErrorEvents;

        /// <summary>
        /// Debug events can be registered for and recieved to view raw send/response text
        /// </summary>
        public event LoggingEventHandler DebugEvents;

        /// <summary>
        /// The RestTransportFunctions class sends errors and warnings encountered in the class as an event that's raised which 
        /// clients can subscribe to for logging events if they wish.  A custom eventArg is used for this that contains
        /// just a simple string "Line" property.
        /// </summary>
        public class LogEventArgs : EventArgs
        {
            public string Line { get; set; }

            public LogEventArgs(string pLine)
            {
                Line = pLine;
            }

            public override string ToString()
            {
                return Line;
            }
        }

        /// <summary>
        /// Alternative event handler for logging events that includes the LogEventArgs that include the log string in the 
        /// argument
        /// </summary>
        public delegate void LoggingEventHandler(object sender, LogEventArgs e);

        /// <summary>
        /// If there's one or more clients registered for the ErrorEvent event then issue it here.
        /// </summary>
        /// <param name="pLine">
        /// String to pass back to the receiving method
        /// </param>
        internal void RaiseErrorEvent(string pLine)
        {
            //notify registered clients 
            LoggingEventHandler handler = ErrorEvents;

            if (handler != null)
            {
                LogEventArgs oArgs = new LogEventArgs(pLine);
                handler(null, oArgs);
            }
        }

        /// <summary>
        /// If there's one or more clients registerd for the DebugEvents event then issue it here.
        /// </summary>
        /// <param name="pLine">
        /// String to pass back to the receiving method
        /// </param>
        private void RaiseDebugEvent(string pLine)
        {
            if (DebugMode == false) return;

            //notify registered clients
            LoggingEventHandler handler = DebugEvents;

            if (handler != null)
            {
                LogEventArgs oArgs = new LogEventArgs(pLine);
                handler(null, oArgs);
            }
        }

        /// <summary>
        /// register for events off the TranspfortFunctions interface and "pinwheel" the events up to clients 
        /// who have registered for error events off the server class
        /// </summary>
        private void TransportFunctionsOnErrorEvents(object sender, RestTransportFunctions.LogEventArgs logEventArgs)
        {
            RaiseErrorEvent(logEventArgs.Line);
        }

        /// <summary>
        /// register for events off the TranspfortFunctions interface and "pinwheel" the events up to clients 
        /// who have registered for debug events off the server class
        /// </summary>
        private void TransportFunctionsOnDebugEvents(object sender, RestTransportFunctions.LogEventArgs logEventArgs)
        {
            RaiseDebugEvent(logEventArgs.Line);
        }



        #endregion


        #region Server Methods

        public override string ToString()
        {
            return string.Format("{0} [{1}]", ServerName, Version);
        }

        /// <summary>
        /// When creating an instance of the ConnectionServer class normally the login function here is called to establish that the server name, login name and
        /// login password provided are all valid and allow attaching to the CUPI interface on the remote Connection server.  As part of the login process the 
        /// full version information is fetched and parsed out and stored as a property on the class instance for easy reference later.
        /// </summary>
        /// <param name="pServerName">
        /// Connection server name or IP address to attach to. 
        /// </param>
        /// <param name="pLoginName"></param>
        /// The Login name to use when attaching to the Connection server's CUPI interface.  Needs to have the admin role in Connection.
        /// <param name="pLoginPw">
        /// The login password to use when attachign to the Connection server's CUPI interface.
        /// </param>
        /// <param name="pIncludeServers">
        /// When authenticating as a user instead of an admin you wont have rights to get server (cluster) details - so this is passed as false.  When 
        /// authenticating as an admin it'll be passed as true so we have cluster details.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.  If the login fails the Success property
        /// on the return class will be FALSE, otherwise TRUE is returned.
        /// </returns>
        private WebCallResult LoginToConnectionServer(string pServerName, string pLoginName, string pLoginPw, bool pIncludeServers)
        {
            this.LoginName = pLoginName;
            this.LoginPw = pLoginPw;

            WebCallResult ret = GetVersionInfo(pServerName);
            if (ret.Success == false)
            {
                return ret;
            }

            //the servers method was not in prior to 9.0
            if (!this.Version.IsVersionAtLeast(9, 0, 1, 0))
            {
                return ret;
            }

            //if we don't need to include servers, just return now.
            if (pIncludeServers == false)
            {
                return ret;
            }

            return GetClusterInfo();
        }


        /// <summary>
        /// Helper method to fetch the Conneciton version number off the server we're attaching to
        /// </summary>
        /// <returns></returns>
        private WebCallResult GetVersionInfo(string pServerName)
        {
            WebCallResult ret = _transportFunctions.GetCupiResponse(BaseUrl + "version", MethodType.GET, this, "");

            if (ret.Success == false)
            {
                return ret;
            }

            string strVersion;
            Dictionary<string, string> oResponse;
            if (string.IsNullOrEmpty(ret.ResponseText))
            {
                //invalid JSON returned
                ret.ErrorText = string.Format("Invalid version XML returned logging into Connection server: {0}, return text={1}", pServerName, ret.ResponseText);
                ret.Success = false;
                return ret;
            }

            try
            {
                oResponse = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(ret.ResponseText);
            }
            catch (Exception ex)
            {
                ret.ErrorText = string.Format("Invalid version XML returned logging into Connection server: {0}, return text={1}, error={2}",
                    pServerName, ret.ResponseText, ex);
                ret.Success = false;
                return ret;
            }

            oResponse.TryGetValue("version", out strVersion);

            if (ParseVersionString(strVersion) == false)
            {
                //Invalid version string encountered (or no version string returned).
                ret.ErrorText = String.Format("No version version returned logging into Connection server: {0}, return text={1}", pServerName, ret.ResponseText);
                ret.Success = false;
                return ret;
            }

            return ret;
        }

        /// <summary>
        /// fetch the servers in the cluster - if the call succeeds the list (which may contain only the local server) will be stored
        /// on the VmsServers property off the instance of this class
        /// </summary>
        public WebCallResult GetClusterInfo()
        {
            List<VmsServer> oServers;
            var ret= VmsServer.GetVmsServers(this, out oServers);
            if (ret.Success == false)
            {
                return ret;
            }

            VmsServers = oServers;

            return ret;
        }


        /// <summary>
        ///Parse out the version string passed back from the version directive and break it into its individual parts so we can do 
        ///basic version checking easily.
        /// </summary>
        /// <param name="pVersionString">
        /// A valid version string must contain at least 3 parts like "8.5.1" - but will almost always contain 4 like "8.5.1.0ES22".   
        /// </param>
        /// <returns>
        /// TRUE if a valid version string was passed in to pad, false otherwise.
        /// </returns>
        internal bool ParseVersionString(string pVersionString)
        {
            int iTemp;

            string[] strVersionChunks = pVersionString.Split('.');

            if (strVersionChunks.Count()<3)
            {
                return false;
            }

            //major is first up
            if (int.TryParse(strVersionChunks[0], out iTemp) == false)
            {
                return false;
            }
            Version.Major = iTemp;

            //minor is up next
            if (int.TryParse(strVersionChunks[1], out iTemp) == false)
            {
                return false;
            }
            Version.Minor = iTemp;

            //build is the 3rd eleement
            if (int.TryParse(strVersionChunks[2], out iTemp) == false)
            {
                return false;
            }
            Version.Rev = iTemp;

            //The build element may or may not be there in some versions and it may contain both a build AND and ES number tacked 
            //onto the string so we have to handle this special.
            if (strVersionChunks.Count()<4)
            {
                Version.Build = 0;
                Version.Es = 0;
                return true;
            }

            //0ES22
            if (strVersionChunks[3].Contains("ES"))
            {
              	//there will be a leading digit or digits, followed by an "ES" followed by trailing digit/digits
                string[] esChunks = strVersionChunks[3].Replace("ES", ".").Split('.');
                if (esChunks.Count()!=2)
                {
                    return false;
                }

                //In rare cases the build can be blank - don't treat that as a failur although you shouldn't run into this in production builds.
                if (int.TryParse(esChunks[0], out iTemp))
                {
                    Version.Build = iTemp;
                }

                //get ES - in rare cases the ES will not be a number - this is technically a problem in the versioning scheme but don't 
                //treat it as a failure here
                if (int.TryParse(esChunks[1], out iTemp))
                {
                    Version.Es = iTemp;    
                }
            }
            else
            {
                //just a digit or digits for the build or test throttle number - don't worry if this doesn't parse to a number
                if (int.TryParse(strVersionChunks[3], out iTemp) == false)
                {
                    return true;
                }
    
                Version.Build = iTemp;
            }

            return true;
        }


        /// <summary>
        ///General purpose routine that will take an instance of an object (forinstance a User instance) and an XMLElement (i.e. looks like
        /// "<FirstName>Jeff</FirstName>" and populates the appropriate field in the class instance with the value from the element.
        ///This requires that the name of the property on the class matches the element name (including case).  Proper type conversion is 
        ///handled and logic to make sure the property is represented on the class and the value is valid (exists) is included.
        ///NOTE: The User, CallHandler etc... classes all define the object id as "ObjectId" and in XML its preresented as "ObjectId" - this 
        ///is deliberate as I want that value skipped since it's the unique identifier used in the class constructors and I don't want them being
        ///updated during a sweep of properties.
        /// </summary>
        /// <param name="pObject">
        /// Object to fill in properties for.
        /// </param>
        /// <param name="pElement">
        /// XML Element that holds a value/name corresponding to a value in the object
        /// </param>
        public void SafeXmlFetch(Object pObject, XElement pElement)
        {
            //if this is a "complex" type, process the sub elements - this comes into play mostly for messages which got a little cute with 
            //complex types for numerous properties
            if (pElement.HasElements)
            {
                //grab the handle to the sub object and then process the sub elements onto it.  If this is a generic list of instances
                //of a type then this scenario needs to be handled by hand.
                object pSubObject;
                PropertyInfo propInfo = pObject.GetType().GetProperty(pElement.Name.LocalName);
                if (propInfo != null)
                {
                    pSubObject = propInfo.GetValue(pObject, null);
                }
                else
                {
                    return;
                }

                if (pSubObject == null)
                {
                    return;
                }
                


                foreach (XElement subElement in pElement.Elements())
                {
                    GetXmlProperty(pSubObject, subElement, subElement.Name.LocalName);
                }
            }
            else
            {
                GetXmlProperty(pObject, pElement, pElement.Name.LocalName);
            }
        }

        /// <summary>
        ///General purpose routine that will take an instance of an object (forinstance a User instance) and an XMLElement (i.e. looks like
        /// "<FirstName>Jeff</FirstName>" and populates the appropriate field in the class instance with the value from the element.
        ///This requires that the name of the property on the class matches the element name (including case).  Proper type conversion is 
        ///handled and logic to make sure the property is represented on the class and the value is valid (exists) is included.
        ///NOTE: The User, CallHandler etc... classes all define the object id as "ObjectId" and in XML its preresented as "ObjectId" - this 
        ///is deliberate as I want that value skipped since it's the unique identifier used in the class constructors and I don't want them being
        ///updated during a sweep of properties.
        /// </summary>
        /// <param name="pObject">
        /// Object to fill in properties for.
        /// </param>
        /// <param name="pElement">
        /// XML Element that holds a value/name corresponding to a value in the object
        /// </param>
        /// <param name="pName">
        /// Name of the property to look for - if it's not on the object, return.  
        /// </param>
        private void GetXmlProperty(object pObject, XElement pElement, string pName)
        {
            //four value types to handle the four values we can pull from XML via the CUPI interface.

            //if the property is not defined on our class (some of the URI properties are redundant) return.
            if (pObject.GetType().GetProperty(pName) == null)
            {
                if (pName.Contains("URI"))
                {
                    return;
                }
                RaiseErrorEvent("Missing property value:" + pName);
                return;
            }

            //we need to know to target type of the element so we can cast is properly - for each type parse out the value field 
            //into the appropraite type and then add that type to the object property.
            switch (pObject.GetType().GetProperty(pName).PropertyType.FullName.ToLower())
            {
                case "system.int32":
                    int intValue = int.Parse(pElement.Value);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, intValue, null);
                    break;
                case "system.int64":
                    long longValue = long.Parse(pElement.Value);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, longValue, null);
                    break;
                case "system.string":
                    string strValue = pElement.Value;
                    pObject.GetType().GetProperty(pName).SetValue(pObject, strValue, null);
                    break;
                case "system.boolean":
                    bool? boolValue = bool.Parse(pElement.Value);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, boolValue, null);
                    break;
                case "system.datetime":
                    DateTime dateValue = DateTime.Parse(pElement.Value);

                    pObject.GetType().GetProperty(pName).SetValue(pObject, dateValue, null);
                    break;
                case "cisco.unityconnection.restfunctions.subscriberconversationtui":
                    strValue = pElement.Value;
                    SubscriberConversationTui oConv;
                    Enum.TryParse(strValue, true, out oConv);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, oConv, null);
                    break;
                case "cisco.unityconnection.restfunctions.transferoptiontypes":
                    strValue = pElement.Value;
                    TransferOptionTypes oTran;
                    Enum.TryParse(strValue, true, out oTran);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, oTran, null);
                    break;
                case "cisco.unityconnection.restfunctions.greetingtypes":
                    strValue = pElement.Value;
                    GreetingTypes oGreet;
                    Enum.TryParse(strValue, true, out oGreet);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, oGreet, null);
                    break;
                case "cisco.unityconnection.restfunctions.conversationnames":
                    strValue = pElement.Value;
                    ConversationNames oConvName;
                    Enum.TryParse(strValue, true, out oConvName);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, oConvName, null);
                    break;
                case "cisco.unityconnection.restfunctions.messagetype":
                    strValue = pElement.Value;
                    MessageType oMsgType;
                    Enum.TryParse(strValue, true, out oMsgType);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, oMsgType, null);
                    break;
                case "cisco.unityconnection.restfunctions.sensitivitytype":
                    strValue = pElement.Value;
                    SensitivityType oSensitivityType;
                    Enum.TryParse(strValue, true, out oSensitivityType);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, oSensitivityType, null);
                    break;
                case "cisco.unityconnection.restfunctions.prioritytype":
                    strValue = pElement.Value;
                    PriorityType oPriorityType;
                    Enum.TryParse(strValue, true, out oPriorityType);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, oPriorityType, null);
                    break;
                default:
                    RaiseErrorEvent("Unknown type encountered in GetXMLProperty on ConnectionServer.cs:"
                                   + pObject.GetType().GetProperty(pName).PropertyType.FullName.ToLower());
                    break;
            }
        }


        /// <summary>
        /// Use a simple set of command line tools tools to convert just about any WAV format into raw PCM format that Connection will be 
        /// happy with.  This will handle GSM6.10, mp3, G729a, G726 and many other WAV formats I've run into in the field - the same library
        /// is used in COBRAS when importing Windows based backups (which may have numerous WAV formats for greetings and voice names) into 
        /// Connection.
        /// </summary>
        /// <param name="pPathToWavFile">
        /// Wav file to convert
        /// </param>
        /// <returns>
        /// Path to a temporary file that contains the converted WAV - File path will never be empty but the file will not exist on th e
        /// hard drive if the conversion fails.
        /// </returns>
        public string ConvertWavFileToPcm(string pPathToWavFile)
        {
            //create a temporary file with a GUID file name in the temporary folder for the local OS install.
            string strConvertedWavFilePath = Path.GetTempFileName() + ".wav";


            // Use ProcessStartInfo class - this lets us set properties, hide the "box" window and wait for the process to complete before 
            //continuing.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;

            //When running in Unit Test mode MSTest struggles to get additional files copied over to the on-the-fly context folder properly so
            //sub folders are not recreated - check to see if the wavcopy.exe is in the folder we're currently operating in (unit test mode) or
            //a sub folder called WavConvert (production mode).  This is the only concession we have to make in production code to get unit tests
            //to run smoothly so it's not the end of the world.
            if (File.Exists(@"WAVConvert\wavcopy.exe"))
            {
                startInfo.FileName = @"WAVConvert\wavcopy.exe";
            }
            else
            {
                startInfo.FileName = @"wavcopy.exe";
            }
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            //Connection is happier with PCM at 8Khz, 16 bit and mono - to rip the wav file into that format if requested.
            startInfo.Arguments = string.Format("\"{0}\" \"{1}\" -pcm:8000,16,1", pPathToWavFile, strConvertedWavFilePath);
            try
            {
                // Start the process with the info we specified.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    //30 seconds is much more than enough, even if the file is enormous
                    exeProcess.WaitForExit(30000);
                }
            }
            catch (Exception ex)
            {
                //log the error here in a production application - for our purposes we just pass back blank for the file 
                //name indicating there was an issue.
                RaiseErrorEvent("Error converting WAV file to PCM:"+ex);
                strConvertedWavFilePath = "";
            }

            return strConvertedWavFilePath;

        }


        /// <summary>
        /// Helper function to convert an set of action data to human readable string for display.  The action data includes an action ID (0 is ignore, 
        /// 1 is hangup, 2 is goto etc...), conversation name (if any) and a target handler objectID (if any).  These three items are used repeatedly in
        /// Connection's database to indicate an action to take for, say, user input keys, after greeting, exiting the subscriber conversation, leaving
        /// a name lookup handler etc... 
        /// This does a database lookup of the target objectID to construct a simple human readable description for items such as "Ring phone for handler 
        /// [operator]"
        /// </summary>
        /// <param name="pAction">
        /// Action value (0 is ignore, 1 is hangup, 2 is goto, etc...)
        /// </param>
        /// <param name="pConversationName">
        /// Conversation name such as PHTransfer, PHGreeting, AD etc...  this can be blank
        /// </param>
        /// <param name="pTargetHandlerObjectId">
        /// The destination ObjectId of a handler (interviewer, name lookupg handler or call handler including a primary call handler for a user). This can 
        /// be blank.
        /// </param>
        /// <returns>
        /// Human readable string describing the action sequence for display/logging purposes.
        /// </returns>
        public string GetActionDescription(ActionTypes pAction, ConversationNames pConversationName, string pTargetHandlerObjectId)
        {
            switch (pAction)
            {
                    //Take care of the action types that do not reference any target or conversation name first.
                case ActionTypes.Ignore:
                    return "Ignore";
                case ActionTypes.Hangup:
                    return "Hang up.";
                case ActionTypes.Error:
                    return "Play error greeting.";
                case ActionTypes.TakeMessage:
                    return "Take message.";
                case ActionTypes.SkipGreeting:
                    return "Skip Greeting";
                case ActionTypes.RestartGreeting:
                    return "Repeat Greeting";
                case ActionTypes.RouteFromNextCallRoutingRule:
                    return "Route from next call routing rule";
                case ActionTypes.TransferToAlternateContactNumber:
                    return "Transfer to alternate contact number";
                case ActionTypes.GoTo:
                    {

                        WebCallResult ret;
                        CallHandler oHandler;
                        switch (pConversationName)
                        {
                            case ConversationNames.SubSignIn:
                                return "Route to user sign in";
                            case ConversationNames.GreetingsAdministrator:
                                return "Route to Greetings administrator";
                            case ConversationNames.ConvHotelCheckedOut:
                                return "Route to checked out hotel guest conversation";
                            case ConversationNames.ConvCvmMboxReset:
                                return "Route to Community Voice Mail box reset";
                            //a few of these had different names in Unity versions - I leave the overloads in here
                            case ConversationNames.SubSysTransfer: 
                                return "Route to user system transfer";
                            case ConversationNames.SystemTransfer:
                                return "Route to system transfer";
                            case ConversationNames.EasySignIn:
                                return "Route to easy subscriber sign in";
                            case ConversationNames.TransferAltContactNumber:
                                return "Alternate contact number";
                            case ConversationNames.BroadcastMessageAdministrator:
                                return "Broadcast message administrator";
                            case ConversationNames.Ad:
                                {
                                    //Alpha Directory (what we used to call Name Lookup Handlers) - this requires a name lookup handler target 
                                    //to load - fetch that handler here so we can showt he display name here.
                                    DirectoryHandler oDirHandler;
                                    ret = DirectoryHandler.GetDirectoryHandler(out oDirHandler, this, pTargetHandlerObjectId);
                                    if (ret.Success==false)
                                    {
                                        return "Invalid link!";
                                    }

                                    return "Route to name lookup handler: "+oDirHandler.DisplayName;
                                }
                            case ConversationNames.PHTransfer:
                                {
                                    //PHTransfer is the transfer conversation entry point for a call handler (subscriber's primary call handler as well
                                    //of course).  It requires a valid target handler Object which we'll fetch here so we can include it's display name
                                    //in the output.
                                    ret = CallHandler.GetCallHandler(out oHandler, this, pTargetHandlerObjectId);
                                    if (ret.Success==false)
                                    {
                                        return "Invalid link!";
                                    }

                                    if (oHandler.IsPrimary)
                                    {
                                        return "Ring phone for subscriber:" + oHandler.DisplayName;
                                    }
                                    return "Ring phone for call handler:" + oHandler.DisplayName;
                                }
                            case ConversationNames.PHGreeting: 
                                {
                                    //PHGreeting is the greeting conversation entry point for a call handler (subscriber's primary call handler as well
                                    //of course).  It requires a valid target handler Object which we'll fetch here so we can include it's display name
                                    //in the output.
                                    ret = CallHandler.GetCallHandler(out oHandler, this, pTargetHandlerObjectId);
                                    if (ret.Success == false)
                                    {
                                        return "Invalid link!";
                                    }

                                    if (oHandler.IsPrimary)
                                    {
                                        return "Send to greeting for subscriber:" + oHandler.DisplayName;
                                    }
                                    return "Send to greeting for call handler:" + oHandler.DisplayName;
                                }
                            case ConversationNames.PHInterview:
                                {
                                    //PHInterview or CHInterview (older style) is the conversation for an interview handler that requires a valid 
                                    //interview handler target to load.  Fetch it here so we can include it's display name in the output.
                                    InterviewHandler oInterviewHandler;
                                    ret = InterviewHandler.GetInterviewHandler(out oInterviewHandler, this,pTargetHandlerObjectId);
                                    if (ret.Success == false)
                                    {
                                        return "Invalid Link!";
                                    }
                                    return "Send to interview handler: "+oInterviewHandler.DisplayName;
                                }

                            default:
                                //Not a conversation name we recognize - there are clients that customize this so be sure to handle this cleanly.
                                return "(error) invalid conversation name:" + pConversationName;
                        }
                    }
                default:
                    //Unknown action type - this should never be possible due to FK constraints in the DB, just just in case.
                    return "(error) invalid action type:" + pAction.ToString();
            }
        }


        #endregion


        #region Helper Methods

        /// <summary>
        /// Hook the JSON.NET serialization error event and "pinwheel" it up as an error event off the server object.
        /// Ignore all errors about URIs being missing - those are bogus since they're unnecessary.
        /// </summary>
        private void JsonParseError(object sender, ErrorEventArgs errorEventArgs)
        {
            if (errorEventArgs.ErrorContext.Member.ToString().ToLower().Contains("uri"))
            {
                return;
            }
            RaiseErrorEvent(string.Format("[ERROR] JSON serialization error: [{0}]:{1}", errorEventArgs.CurrentObject.GetType().Name,
                    errorEventArgs.ErrorContext.Error.Message));
        }


        /// <summary>
        /// Generic method for fetching a list of objects from an Json - works for all class types
        /// </summary>
        /// <typeparam name="T">
        /// Type of object to return a list of
        /// </typeparam>
        /// <param name="pJson">
        /// Raw Json returned from the HTTP request
        /// </param>
        /// <param name="pTypeNameOverride">
        /// If you need to use a differnt type name for the JSON parsing, pass it in as a string here - defaults to 
        /// using the name of the type off the class.
        /// Used for classes like User and UserFull that need to be parsed using just "User"
        /// </param>
        /// <returns>
        /// List of instances of the object type passed in.
        /// </returns>
        public List<T> GetObjectsFromJson<T>(string pJson, string pTypeNameOverride = "")
        {
            return _transportFunctions.GetObjectsFromJson<T>(pJson, pTypeNameOverride);
        }

        /// <summary>
        /// Generic method for fetching a list of objects from an Json - works for all class types
        /// </summary>
        /// <typeparam name="T">
        /// Type of class to fill in.
        /// </typeparam>
        /// <param name="pJson">
        /// Json text to parse
        /// </param>
        /// <param name="pTypeNameOverride">
        /// If what's returned via the JSON text does not match the class name itself, you can override it 
        /// with this string.  Needed when parsing UserBase, UserFull and other classes that don't match.
        /// </param>
        /// <returns>
        /// Instance of the class passed in filled out (hopefully) with the data from the JSON text.
        /// </returns>
        public T GetObjectFromJson<T>(string pJson, string pTypeNameOverride = "") where T : new()
        {
            return _transportFunctions.GetObjectFromJson<T>(pJson, pTypeNameOverride);
        }

        /// <summary>
        /// Helper method to do a fetch and fill the target object with values from the resulting response body.  Commmon operation 
        /// needed for all object types.
        /// The response itself and the full GET URI used to fetch it are included in the WebCallResult class returned.
        /// </summary>
        /// <param name="pUrl">
        /// GET URI for fetchind the data for a single object of the type being filled in
        /// </param>
        /// <param name="pObject">
        /// Instance of the object class to be filled in
        /// </param>
        /// <returns>
        /// WebCallResult instance with the details of the request and response and failure if there is one.
        /// </returns>
        public WebCallResult FillObjectWithRestGetResults<T>(string pUrl, T pObject)
        {
            WebCallResult res = GetCupiResponse(pUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, pObject, RestTransportFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance from JSON response:" + ex;
                res.Success = false;
            }
            return res;
        }

        /// <summary>
        /// Simple function to take an alias of a user and their password and indicate if they are capable of authenticating against
        /// the server or not
        /// </summary>
        /// <param name="pLoginName">
        /// The login name (alias) of the user you want to validate
        /// </param>
        /// <param name="pPassword">
        /// Password (GUI password - not the PIN for the phone password) for the user to verify.
        /// </param>
        /// <param name="pUser">
        /// If a match is found and the password is valid an instance of User is created and filled in with the target user's 
        /// details and passed back on this out parameter
        /// </param>
        /// <returns>
        /// True if the user is found and the password hash matches.  False if not.
        /// </returns>
        public bool ValidateUser(string pLoginName, string pPassword, out UserBase pUser)
        {
            pUser = null;
            try
            {
                new ConnectionServerRest(_transportFunctions, this.ServerName, pLoginName, pPassword, false);
            }
            catch 
            {
                return false;
            }
            
            //fetch the objectId of the user - unfortunately if the user is not an admin then the ObjectId is not 
            //returned from a basic properties fetch so we have to do this as a 2nd call via the admin interface
            //instead.
            WebCallResult res= UserBase.GetUser(out pUser, this, "", pLoginName);
            return res.Success;
        }

        /// <summary>
        /// Version of the validate user method that does not return an instance of User - this eliminates the need for a 2nd
        /// fetch request and is quicker as a result - if you only need to verify that a user's password is valid but don't need 
        /// to do anything with that user, this is the version to use.
        /// </summary>
        /// <param name="pLoginName">
        /// The login name (alias) of the user you want to validate
        /// </param>
        /// <param name="pPassword">
        /// Password (GUI password - not the PIN for the phone password) for the user to verify.
        /// </param>
        /// <returns>
        /// True if the user is found and the password hash matches.  False if not.
        /// </returns>
        public bool ValidateUser(string pLoginName, string pPassword)
        {
            try
            {
                new ConnectionServerRest(_transportFunctions,this.ServerName, pLoginName, pPassword, false);
            }
            catch 
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Simple method to fetch the primary location ObjectId of the Connection server the instance of this class is pointing to.
        /// </summary>
        /// <returns>
        /// Primary location objectId for the Connection server.
        /// </returns>
        private Location GetPrimaryLocation()
        {
            List<Location> oList;

            //there should only ever be one location defined that has the "isprimary" set to true.
            WebCallResult res= Location.GetLocations(this, out oList,"query=(IsPrimary is 1)");

            if (res.Success == false || oList.Count !=1)
            {
                return null;
            }

            return oList[0];
        }

        /// <summary>
        /// Simple method to fetch the VMSServer ObjectId for the Connection server attached to this instance of the ConnectionServer class.
        /// If this is a cluster the ObjectId of the VMSServer that corresponds to the IP address of the Connection server this instance is 
        /// attached to is returned.  
        /// </summary>
        /// <returns>
        /// ObjectId of the VMSServer currently attached.
        /// </returns>
        private VmsServer GetVmsServer()
        {
            List<VmsServer> oList;
            //there should only ever be one location defined that has the "isprimary" set to true.
            WebCallResult res = VmsServer.GetVmsServers(this, out oList);

            if (res.Success == false)
            {
                return null;
            }

            if (oList.Count == 0)
            {
                return null;
            }

            //select the one that matches the Connection server's IP address we're connected to
            foreach (var oServer in oList)
            {
                if (oServer.HomeServer.ServerName == this.ServerName )
                {
                    return oServer;
                }
            }

            return null;
        }


        /// <summary>
        ///armed with the object type, it's object ID and the server name we can construct a URL to open the SA web page to that 
        ///object directly.  Only the frame showing the object details itself are shown, the navigation at the top and tree to the 
        /// left in CUCA are hidden - this makes it easy to "embed" an admin interface and leverage the CUCA for your applications.
        /// It's not lazy, it's efficent.
        /// </summary>
        /// <param name="pObjectType">
        /// The type of object (call handler, subscriber, distribution list etc...) you want to launch the CUCA page for.
        /// </param>
        /// <param name="strObjectId">
        /// GUID identifying the object to be shown in CUCA.
        /// </param>
        /// <returns>
        /// full URI to get to the CUCA for the object in question - blank if there was an error processing the request.
        /// </returns>
        public string GetCucaUrlForObject(ConnectionObjectType pObjectType, string strObjectId)
        {
            string strUrl = "https://" + this.ServerName + ":8443/cuadmin/";

            switch (pObjectType)
            {
                case ConnectionObjectType.InterviewHandler:
                    strUrl += "interview-handler.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.NameLookupHandler:
                    strUrl += "directory-handler.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.User:
                case ConnectionObjectType.Subscriber:
                case ConnectionObjectType.GlobalUser:
                    strUrl += "user.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.SystemCallHandler:
                case ConnectionObjectType.Handler:
                    strUrl += "callhandler.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Location:
                    strUrl += "location-vms.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.RestrictionTable:
                    strUrl += "restriction-table.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Role:
                    strUrl += "edit-role.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.RoutingRuleDirect:
                    strUrl += "routing-rule.do?op=readDirectRule&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.RoutingRuleForwarded:
                    strUrl += "routing-rule.do?op=readForwardedRule&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.SmppProvider:
                    strUrl += "smpp-provider.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.SubscriberTemplate:
                    strUrl += "user.do?op=readTemplate&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.CallHandlerTemplate:
                    strUrl += "callhandler.do?op=readTemplate&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Schedule:
                case ConnectionObjectType.ScheduleSet:
                    strUrl += "schedule.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Partition:
                    strUrl += "partition.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.SearchSpace:
                    strUrl += "search-space.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Switch:
                    strUrl += "media-switch.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.DistributionList:
                    strUrl += "edit-distribution-list.do?objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.PersonalCallTransferRule:
                    //PCA "hop" link is constructed a bit differently
                    strUrl = "https://" + this.ServerName + ":8443/ciscopca/runas.do?userid=" + strObjectId + "&startat=unitycallroutingrules/rulesets.do";
                    break;
                case ConnectionObjectType.SystemContact:
                case ConnectionObjectType.VpimContact:
                    strUrl += "contact.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Cos:
                    //https://fmstest3.cisco.com:8443/cuadmin/cos.do?op=read&objectId=85ba8e9b-1f46-491c-b9e1-8e03e3c89124
                    strUrl += "cos.do?op=read&objectId=" + strObjectId;
                    break;
                default:
                    strUrl = "";
                    break;
            }

            return strUrl;
        }


        #endregion


        #region Static Helper Methods

        /// <summary>
        /// Helper method that tacks on zero, one or many clauses onto the end of a URI before sending it to Connection.
        /// </summary>
        /// <param name="pUri">
        /// Uri to build on.  Gets returned unchanged if there are no clauses to add
        /// </param>
        /// <param name="pClauses">
        /// Array of strings representing seperate clauses such as "rowsPerPage=10" for instance.
        /// </param>
        /// <returns>
        /// Update URI with clauses tacked on with ? and & seperators as appropriate - the same URI is returned that is passed in
        /// if there are no clauses in the params list.
        /// </returns>
        public static string AddClausesToUri(string pUri, params string[] pClauses)
        {
            string retUri = pUri;

            if (pClauses == null)
            {
                return retUri;
            }

            bool bFirstClause = true;

            //Tack on all the search/query/page clauses here if any are passed in.  If an empty string is passed in account
            //for it here.
            for (int iCounter = 0; iCounter < pClauses.Length; iCounter++)
            {
                if (pClauses[iCounter].Length == 0)
                {
                    continue;
                }

                //if it's the first param seperate the clause from the URL with a ?, otherwise append compound clauses 
                //seperated by &
                if (bFirstClause)
                {
                    retUri += "?";
                    bFirstClause = false;
                }
                else
                {
                    retUri += "&";
                }
                
                //special case the query and sort parameters
                retUri += pClauses[iCounter].UriSafe();
            }

            return retUri;
        }


        /// <summary>
        /// Connection wants booleans passed in as "0" or "1", not true or false - the .ToString method on booleans returns "true" or "false" which doesn't
        /// work - this is just a helper function to make the conversion cleanly.
        /// </summary>
        /// <param name="pBool">
        /// Boolean to be certed into a "0" or "1" string.
        /// </param>
        /// <returns>
        /// string of "0" or "1" is returned in all cases.
        /// </returns>
        public static string BoolToString(bool pBool)
        {
            int iTemp = 0;
            if (pBool)
                iTemp = 1;
            return iTemp.ToString();
        }

        /// <summary>
        /// strips out the preamble when a search is done that can return more than one item - the items count and class
        /// name leading up to the actual result is stripped off along with the accompanying trailing curly brace - if the 
        /// preamble is not there then it does not strip either off.
        /// </summary>
        /// <param name="pJson"></param>
        /// <param name="pClassName"></param>
        /// <param name="pAddBrackets"></param>
        /// <returns></returns>
        public static string StripJsonOfObjectWrapper(string pJson, string pClassName, bool pAddBrackets = false)
        {
            string pTokenName = string.Format("\"{0}\":", pClassName);
            string strCleanJson;
            if (pJson.ToLower().Contains(pTokenName.ToLower()))
            {
                strCleanJson = pJson.TrimToEndOfToken(pTokenName).TrimTokenFromEnd("}");
            }
            else
            {
                strCleanJson = pJson;
            }

            if (pAddBrackets)
            {
                //only add the brackets if they're not there already
                if (!strCleanJson.StartsWith("["))
                {
                    strCleanJson = "[" + strCleanJson + "]";
                }
            }

            return strCleanJson;
        }


        #endregion

    }
}

