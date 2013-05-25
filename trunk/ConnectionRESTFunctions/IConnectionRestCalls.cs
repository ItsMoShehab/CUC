using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Cisco.UnityConnection.RestFunctions
{

    #region Related Classes and Enums

    /// <summary>
    /// Structure used as a return code for all calls fetching or setting data via the REST interface.  The ResponseText is is what's 
    /// returned from the HTTP call and the ErrorText is if there's an execution error (for instance the server name does not resolve). 
    /// One or the other or both can be empty depending on 
    /// </summary>
    public struct WebCallResult
    {
        public bool Success;
        public string ResponseText; //raw text returned from the server's HTTP interface.
        public string ErrorText;    //human readable error reason (if any)
        public int StatusCode;      //HTTP status code = 200 (OK), 204 (accepted) etc...
        public string StatusDescription;  //Additional status info sent by server (if any)
        public XElement XmlElement;  //raw text result parsed into XML elements for easy processing.
        public Dictionary<string, object> JsonDictionary; //raw text result parsed into dictionary if the response is JSON.
        public int TotalObjectCount; //for GET operations, even if returing only some users via paging, the total number is always returned.
        public string Url;           //Full URL that was sent to the server.
        public string Method;        //Method used (POST, PUT, GET...)
        public string RequestBody;   //Request body that was sent to the server.
        public string Misc;         //string to hold other data the calling/caller may wish to log such as full paths to file names processed and such.
        public string ReturnedObjectId; //for all new object creation the objectID of the new object is returned here.

        /// <summary>
        /// dumps the entire contents of the WebCallREsult excpept for the XElement object (which is just a parsed version of the ResponseText) and
        /// returns it as a formatted string for logging and display purposes.
        /// </summary>
        public override string ToString()
        {
            StringBuilder strRet = new StringBuilder();

            strRet.AppendLine("    WebCallResults contents:");
            strRet.AppendLine("    URL Sent: " + Url);
            strRet.AppendLine("    Method Sent: " + Method);
            strRet.AppendLine("    Body Sent: " + RequestBody);
            strRet.AppendLine("    Success returned: " + Success);
            strRet.AppendLine(String.Format("    Status returned {0}:{1}", StatusCode, Enum.ToObject(typeof(HttpStatusCode), StatusCode)));
            strRet.AppendLine("    Error Text: " + ErrorText);
            strRet.AppendLine("    Raw Response Text: " + ResponseText);
            strRet.AppendLine("    Total object count: " + TotalObjectCount);
            strRet.AppendLine("    Status description: " + StatusDescription);

            if (!string.IsNullOrEmpty(ReturnedObjectId))
                strRet.AppendLine("    Returned ObjectId: " + ReturnedObjectId);

            if (!string.IsNullOrEmpty(Misc))
                strRet.AppendLine("    Misc data:" + Misc);

            return strRet.ToString();
        }
    }

    /// <summary>
    /// wrap an exception type so we can pass back the WebCallResult class in an exception when necessary
    /// </summary>
    public class UnityConnectionRestException : Exception
    {
        public UnityConnectionRestException(WebCallResult pWebCallResult, string pDescription = "")
            : base(pDescription)
        {
            WebCallResult = pWebCallResult;
            Description = pDescription;
        }

        public WebCallResult WebCallResult { get; private set; }
        public string Description { get; private set; }
    }

    /// <summary>
    /// list of possible methods supported by CUPI for getting/setting properties. 
    /// </summary>
// ReSharper disable InconsistentNaming
    public enum MethodType { PUT, POST, GET, DELETE }
// ReSharper restore InconsistentNaming

    /// <summary>
    /// List of common HTTP status codes used in providing diagnoostic/log output and such.
    /// </summary>
    public enum HttpStatusCode
    {
        Ok = 200,
        Created = 201,
        ChangeAccepted = 204,
        MovedPermanently = 301,
        MovedTemporarily = 302,
        BadRequest = 400,
        UnauthorizedUser = 401,
        Forbidden = 403,
        PageNotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        Gone = 410,
        UnsupportedMediaType = 415,
        ServerError = 500
    }

    #endregion

    /// <summary>
    /// All REST based methods that need to be supported by the SDK are defined here - the GetCupiResponse is the primary interface for 
    /// all calls to all interfaces (CUPI, CUMI, CUTI) along with helper methods for easily uploading and downloading media files to 
    /// and from Connection servers using local WAV files and resourceIDs (using CUTI phone interface)
    /// </summary>
    public interface IConnectionRestCalls
    {
        #region Properties

        /// <summary>
        /// Enables debug options at the transport layer for raising debug events to clients that have registered for them.
        /// </summary>
        bool DebugMode { get; set; }

        /// <summary>
        /// Defaults to 15 seconds, allows clients so change it for testing
        /// </summary>
        int HttpTimeoutSeconds { get; set; }

        #endregion


        #region Events

        event RestTransportFunctions.LoggingEventHandler ErrorEvents;

        event RestTransportFunctions.LoggingEventHandler DebugEvents;

        #endregion


        #region Helper Methods

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
        List<T> GetObjectsFromJson<T>(string pJson, string pTypeNameOverride = "");

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
        T GetObjectFromJson<T>(string pJson, string pTypeNameOverride = "") where T : new();

        #endregion


        #region Http Call Methods

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
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer object
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
        WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServer pConnectionServer,
                                                    string pRequestBody,bool pJsonResponse = true);


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
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer object
        /// </param>
        /// <param name="pRequestDictionary">
        /// a string/string dictionary containing the parameters to send in the body of the request formatted for JSON
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... 
        /// associated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServer pConnectionServer,
                                                    Dictionary<string, string> pRequestDictionary);

        #endregion


        #region Media Download Methods

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
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer class
        /// </param>
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
        WebCallResult DownloadWavFile(ConnectionServer pConnectionServer, string pLocalWavFilePath,
                                      string pConnectionFileName);


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
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer class
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
        WebCallResult DownloadMessageAttachment(string pBaseUrl, ConnectionServer pConnectionServer,string pLocalWavFilePath,
                                                string pUserObjectId, string pMessageObjectId, int pAttachmentNumber);

        #endregion


        #region Media Upload Methods

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
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer class
        /// </param>
        /// <param name="pLocalWavFilePath" type="string">
        /// The full path to the local WAV file to upload to the remote Connection server.
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        /// associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        WebCallResult UploadWavFile(string pFullResourcePath, ConnectionServer pConnectionServer,string pLocalWavFilePath);


       /// <summary>
        /// Upload a new message to the Connection server using a local WAV file as the voice mail attachment.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer class
        /// </param>
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
        WebCallResult UploadVoiceMessageWav(ConnectionServer pConnectionServer, string pPathToLocalWav,
                                            string pMessageDetailsJsonString, string pSenderUserObjectId,
                                            string pRecipientJsonString,
                                            string pUriConstruction = "");


        /// <summary>
        /// Create a new message using a resourceID for a stream already recorded on the Connection server - used when leveraging 
        /// CUTI to create new messages.
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer class instance.
        /// </param>
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
        WebCallResult UploadVoiceMessageResourceId(ConnectionServer pConnectionServer, string pResourceId,
                                                   string pMessageDetailsJsonString, string pSenderUserObjectId,
                                                   string pRecipientJsonString,
                                                   string pUriConstruction = "");

        /// <summary>
        /// This routine will generate a temporary WAV file name on Connecton and uplaod the local WAV file to that location.  If it completes the 
        /// Connection stream file name will be returned and can be assigned as the stream file property for a voice name, greeting or interview 
        /// handler question.  
        /// Note that these wav files can NOT be used for messages (regular or dispatch).
        /// This is an older construction and is only needed for interview handler questions currently.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer class
        /// </param>
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
        WebCallResult UploadWavFileToStreamLibrary(ConnectionServer pConnectionServer, string pLocalWavFilePath,
                                                   out string pConnectionStreamFileName);

        #endregion
    }
}
