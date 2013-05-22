using System;
using System.Collections.Generic;
using System.Linq;
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
            strRet.AppendLine(String.Format("    Status returned {0}:{1}", StatusCode, Enum.ToObject(typeof(HTTPStatusCode), StatusCode)));
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
    public enum MethodType { PUT, POST, GET, DELETE }

    /// <summary>
    /// List of common HTTP status codes used in providing diagnoostic/log output and such.
    /// </summary>
    public enum HTTPStatusCode
    {
        OK = 200,
        Created = 201,
        Change_Accepted = 204,
        Moved_Permanently = 301,
        Moved_Temporarily = 302,
        Bad_Request = 400,
        Unauthorized_User = 401,
        Forbidden = 403,
        Page_Not_Found = 404,
        Method_Not_Allowed = 405,
        Not_Acceptable = 406,
        Gone = 410,
        Unsupported_Media_Type = 415,
        Server_Error = 500
    }

    #endregion


    interface IConnectionRestCalls
    {
        WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServer pConnectionServer,
                                                    Dictionary<string, string> pRequestDictionary);


        WebCallResult GetHttpResponse(string pUrl, MethodType pMethod, ConnectionServer pConnectionServer,
                                      string pRequestBody, bool pIsJson = false);
    }
}
