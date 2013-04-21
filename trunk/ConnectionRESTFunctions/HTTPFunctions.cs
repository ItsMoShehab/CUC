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
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

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
        public int TotalObjectCount; //for GET operations, even if returing only some users via paging, the total number is always returned.
        public string Url;           //Full URL that was sent to the server.
        public string Method;        //Method used (POST, PUT, GET...)
        public string RequestBody;   //Request body that was sent to the server.
        public string Misc;         //string to hold other data the calling/caller may wish to log such as full paths to file names processed and such.
        public string ReturnedObjectId; //for all new object creation the objectID of the new object is returned here.
        public string SessionCookie;  //entire cookie string (can contain both jsession and jsessionsso
        /// <summary>
        /// dumps the entire contents of the WebCallREsult excpept for the XElement object (which is just a parsed version of the ResponseText) and
        /// returns it as a formatted string for logging and display purposes.
        /// </summary>
        public override string ToString()
        {
            StringBuilder strRet=new StringBuilder();

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
    /// list of possible methods supported by CUPI for getting/setting properties. 
    /// </summary>
    public enum MethodType {PUT, POST, GET, DELETE}

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

    /// <summary>
    /// Internal class used by the ConnectionServer class for communicating via HTTP to target Connection servers.  This single static class
    /// can be used to talk to multiple Connection servers via multiple instances of the ConnectionServer class, however this demonostration
    /// application is NOT designed to be thread safe so it's assumed you are communicating to one server at a time.  That said there is basic
    /// thread locking functionality put into the GetResponse method to prevent messy accidents.
    /// </summary>
    public static class HTTPFunctions
    {

        #region Fields and Properties

        //used for basic thread locking when communicating via HTTP.  If you wanted to do proper multi threading support via HTTP you'd want 
        //do implement a much more sophisticated approach that is well beyond the scope of this example solution.
        private static readonly Object ThisLock = new Object();

        /// <summary>
        /// Used to customize JSON serialization and deserialization behavior - used internally only to hook the error handling event so we 
        /// can easily log out missing properties in classes or properties not coming from REST call that should be.
        /// </summary>
        public static JsonSerializerSettings JsonSerializerSettings { get; private set; }

        /// <summary>
        /// When in debug mode missing member flags are output to the command window, otherwise 
        /// </summary>
        public static bool DebugMode { get; set; }

        #endregion


        #region Constructors

        // Default construtor - attach the VAlidateRemoteCertificate to the validation check so we don't get errors on self signed certificates 
        //when attaching to Connection servers.
        static HTTPFunctions()
        {
            // allows for validation of SSL conversations
            DebugMode = false;
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            ServicePointManager.Expect100Continue = false;
            JsonSerializerSettings = new JsonSerializerSettings();
            JsonSerializerSettings.Error += JsonSerializerErrorEvent;
            JsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
        }

        #endregion

        #region Events 

        /// <summary>
        /// Event handle for external clients to register with so they can get logging events on errors and warnings that happen
        /// within this class.
        /// </summary>
        public static event LoggingEventHandler ErrorEvents;

        /// <summary>
        /// Debug events can be registered for and recieved to view raw send/response text
        /// </summary>
        public static event LoggingEventHandler DebugEvents;

        /// <summary>
        /// The HTTPFunctions class sends errors and warnings encountered in the class as an event that's raised which 
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
        private static void RaiseErrorEvent(string pLine)
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
        private static void RaiseDebugEvent(string pLine)
        {
            //notify registered clients
            LoggingEventHandler handler = DebugEvents;

            if (handler != null)
            {
                LogEventArgs oArgs = new LogEventArgs(pLine);
                handler(null, oArgs);
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Method that fires on the JSON serialization or deserialization errors.  Raises an error event only if debug mode is turned on.
        /// Ignore any error about missing properties when it ends with "URI'" - we don't store the URIs for allt he sub objects since it's
        /// not necessary as all those can easily be derived.
        /// </summary>
        private static void JsonSerializerErrorEvent(object sender, ErrorEventArgs e)
        {
            //if debug mode is on and it's not about URI properties then raise a debug event 
            if (!e.ErrorContext.Error.Message.Contains("URI'"))
            {
                RaiseErrorEvent("JSON:" + e.ErrorContext.Error.Message);
            }
            e.ErrorContext.Handled = true;
        }



        //used to ignore self signed certificate errors when using HTTP to move wav files on and off Conneciton servers - nearly every production
        //system you run into out there is using the self signed certificates so you have to override the check here and return true no matter
        //what or this just doesn't fly.
        private static bool ValidateRemoteCertificate(object pSender, X509Certificate pCertificate, X509Chain pChain, SslPolicyErrors pPolicyErrors)
        {
            return true;
        }

        //pass in a string and this will return that string with characters that need to be "Escaped" out of it.
        //For instance the string "this is a test" would look like "this%20is%20a%20test".  Currently the space character
        //is all it handles but the RFC for URI construction does indicate there are other possibilities here so I leave it
        //in its own routine just in case.
        private static string EscapeCharactersInUrl(string pUri)
        {
            return pUri.Replace(" ", "%20");
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
        internal static string BoolToString(bool pBool)
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
        public static List<T> GetObjectsFromJson<T>(string pJson, string pTypeNameOverride = "")
        {
            //chop out the "wrapper" JSON for easier parsing

            string strCleanJson;

            if (string.IsNullOrEmpty(pTypeNameOverride))
            {
                strCleanJson = StripJsonOfObjectWrapper(pJson, typeof(T).Name, true);
            }
            else
            {
                strCleanJson = StripJsonOfObjectWrapper(pJson, pTypeNameOverride, true);
            }

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(strCleanJson, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                RaiseErrorEvent("Failed in GetObjectsFromJson:" + ex);
                return new List<T>();
            }
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
        public static T GetObjectFromJson<T>(string pJson, string pTypeNameOverride = "") where T : new()
        {
            //chop out the "wrapper" JSON for easier parsing
            string strCleanJson;
            if (string.IsNullOrEmpty(pTypeNameOverride))
            {
                strCleanJson = StripJsonOfObjectWrapper(pJson, typeof(T).Name);
            }
            else
            {
                strCleanJson = StripJsonOfObjectWrapper(pJson, pTypeNameOverride);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(strCleanJson, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                RaiseErrorEvent("Failed in GetObjectFromJson:" + ex);
                return new T();
            }
        }

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
                if (iCounter == 0)
                {
                    retUri += "?";
                }
                else
                {
                    retUri += "&";
                }
                retUri += pClauses[iCounter];
            }

            return retUri;
        }

        #endregion


        #region HTTP Request and Response Methods

        /// <summary>
        /// Primary method for sending/fetching data to and from the Connection server via CUPI.  
        /// </summary>
        /// <param name="pUrl">
        /// Full URL to send to Connection - format should look like:
        /// https://{Connection Server Name}:8443/vmrest/users
        /// </param>
        /// <param name="pMethod">
        /// GET, PUT, POST, DELETE method type
        /// </param>
        /// <param name="pLoginName">
        /// The login name used to authenticate to the CUPI interface on Connection.
        /// </param>
        /// <param name="pLoginPw">
        /// The login password used to authenticate to the CUPI interface on Connection
        /// </param>
        /// <param name="pRequestBody">
        /// If the command (for instance a POST) include the need for a post body for additional data, include it here.  Not all commands
        /// require this (GET calls for instance).
        /// </param>
        /// <param name="pIsJson">
        /// If passed as true the resquest is formed as a JSON request and the results are assumed to be in the same format.  If the default of 
        /// false is passed it's sent as XML and the response is assumed to be the same.
        ///  </param>
        /// <param name="pJsessionId"></param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... associated
        /// with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        private static WebCallResult GetHttpResponse(string pUrl, MethodType pMethod, string pLoginName, string pLoginPw,
                        string pRequestBody, bool pIsJson = false, string pJsessionId = "")
        {
            WebCallResult res = new WebCallResult();
            StreamReader reader = null;
            HttpWebResponse response = null;

            //store the request parts in the resonse structure for ease of reference for error logging and such.
            res.Url = EscapeCharactersInUrl(pUrl);
            res.Method = pMethod.ToString();
            res.RequestBody = pRequestBody;
            res.Success = false;

            //ensure that only one thread at a time is in the web request/response section at a time
            lock (ThisLock)
            {
                try
                {
                    HttpWebRequest request = WebRequest.Create(pUrl) as HttpWebRequest;

                    if (request == null)
                    {
                        res.ErrorText ="Error - null returned for WebRequest create in GetResponse on HTTPFunctions.cs using URL=" +pUrl;
                        RaiseErrorEvent(res.ErrorText);
                        return res;
                    }

                    RaiseDebugEvent("**** Sending to server ****");
                    RaiseDebugEvent("    URI:" + request.RequestUri);
                    RaiseDebugEvent("    Method:" + pMethod);
                    RaiseDebugEvent("    Body:" + pRequestBody);
                    
                    request.Method = pMethod.ToString();
                    request.Credentials = new NetworkCredential(pLoginName, pLoginPw);
                    request.KeepAlive = true;
                    request.Timeout = 15 * 1000;
                    //request.PreAuthenticate = true;
                    request.Headers.Add("Cache-Control", "no-cache");

                    //always stick the authorization item in the header - the .NET library only does this on a challange
                    //which wastes a trip
                    string authInfo = pLoginName + ":" + pLoginPw;
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                    request.Headers["Authorization"] = "Basic " + authInfo;

                    //if a session ID is passed in, include it in the header
                    if (!string.IsNullOrEmpty(pJsessionId))
                    {
                        request.Headers["Cookie"] = pJsessionId;
                    }

                    if (pIsJson)
                    {
                        request.ContentType = @"application/json";
                        request.Accept = @"application/json";
                    }
                    else
                    {
                        request.ContentType = @"application/xml";
                        request.Accept = @"application/xml";
                    }

                    //not all requests have a body - add it only if it's passed in as non empty
                    if (!String.IsNullOrEmpty(pRequestBody))
                    {
                        using (Stream requestStream = request.GetRequestStream())
                        using (StreamWriter writer = new StreamWriter(requestStream))
                        {
                            writer.Write(pRequestBody);
                        }
                    }

                    //issue the request to the server.
                    try
                    {
                        response = request.GetResponse() as HttpWebResponse;
                        
                        if (response != null)
                        {
                            //if the server set a session cookie in the header, return it here
                            var cookie = response.Headers["Set-Cookie"];
                            if (cookie != null)
                            {
                                res.SessionCookie = cookie;
                            }
                        }
                         

                    }
                    catch (WebException ex)
                    {
                        //CUPI will return additional information about the error reason in teh ResponseText tucked into the exception's Resonse object
                        //here - this only applies if the WebException thrown is a protocol error which in most cases it will be.
                        if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response !=null)
                        {
                            //fill out the return structure with as much detail as we call for the calling client to use to figure out what went wrong.
                            res.ErrorText = ((HttpWebResponse)ex.Response).StatusDescription;
                            res.ResponseText = GetResponseText(ex.Response as HttpWebResponse);
                            res.XmlElement = GetXElementFromString(res.ResponseText);
                            res.StatusDescription = ((HttpWebResponse)ex.Response).StatusDescription;
                            res.StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                            
                            RaiseErrorEvent(res.ToString());
                            return res;
                        }
                        else
                        {
                            //there's not a lot of good reasons the WebException will be thrown without it being a protocol error, but just in case.
                            res.ErrorText = "Web exception error in GetResponse on HTTPFunctions.cs:" + ex.Message;
                            RaiseErrorEvent(res.ErrorText);
                        }
                    }
                    catch (Exception ex)
                    {
                        //some other error (connection lost, server down etc...) happened, just return it as a general error.
                        res.ErrorText = "Error fetching request in GetResponse on HTTPFunctions.cs:" + ex.Message;
                        RaiseErrorEvent(ex.Message);
                        return res;
                    }

                    if (request.HaveResponse && response != null)
                    {
                        //store the resonse text into the return structure 
                        res.ResponseText = GetResponseText(response);

                        //store the response code details from the server such as 200 (OK), 400 (Bad Request), 401 (Unauthorized user) etc... 
                        res.StatusCode = (int)response.StatusCode;
                        res.StatusDescription = response.StatusDescription;

                        //at this point the send is considered good - parse out the response as XML if any was recieved (not all requests get them).
                        res.Success = true;

                        RaiseDebugEvent("**** Response from server ****");
                        RaiseDebugEvent(string.Format("Status={0}", res.StatusCode));
                        RaiseDebugEvent(res.ResponseText);

                        return res;
                    }
                }
                catch (UriFormatException ex)
                {
                    //URI errors almost always mean an invalid parameter (i.e. an alias or ObjectId that is not found) was 
                    //passed
                    res.ErrorText = string.Format("URI Error encountered in GetResponse on HTTPFunctions.cs{0}\n" 
                                                + "This usually means an invalid property name or ID that could not be found was passed on the URL."
                                                ,ex.Message);

                    RaiseErrorEvent(res.ErrorText);
                    
                    res.Success = false;
                }
                catch (Exception ex)
                {
                    res.ErrorText = "Error encountered in GetResponse on HTTPFunctions.cs:" + ex.Message;
                    res.Success = false;
                    RaiseErrorEvent(res.ErrorText);
                }
                finally
                {
                    //clean up on the way out of town.
                    if (reader != null) reader.Dispose();
                    if (response != null) response.Close();
                }
            }
            
            return res;
        }


        /// <summary>
        /// Primary method for sending/fetching data to and from the Connection server via CUPI - tries to parse results returned into XML format.
        /// Use the GetJsonResponse if you're sending/recieving data in JSON format instead.
        /// </summary>
        /// <param name="pUrl">
        /// Full URL to send to Connection - format should look like:
        /// https://{Connection Server Name}:8443/vmrest/users
        /// </param>
        /// <param name="pMethod">
        /// GET, PUT, POST, DELETE method type
        /// </param>
        /// <param name="pLoginName">
        /// The login name used to authenticate to the CUPI interface on Connection.
        /// </param>
        /// <param name="pLoginPw">
        /// The login password used to authenticate to the CUPI interface on Connection
        /// </param>
        /// <param name="pRequestBody">
        /// If the command (for instance a POST) include the need for a post body for additional data, include it here.  Not all commands
        /// require this (GET calls for instance).
        /// </param>
        /// <param name="pSessionCookie">
        /// Optional session cookie string to include in the request header
        /// </param>
        /// <param name="pJsonResponse">
        /// Defaults to getting JSON body as a response, if passed as false XML will be requested instead.
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... associated
        /// with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        private static WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, string pLoginName, string pLoginPw, string pRequestBody,
            string pSessionCookie = "", bool pJsonResponse = true)
        {

            WebCallResult res = GetHttpResponse(pUrl, pMethod, pLoginName, pLoginPw, pRequestBody, pJsonResponse, pSessionCookie);
            
            res.TotalObjectCount = 0;
            //if we get a result text blob back, try and parse it out and check for a "total" item in there.  This gets used for 
            //paging scenarios and such.
            if (res.Success == false || string.IsNullOrEmpty(res.ResponseText))
            {
                return res;
            }

            //parse out the response body into a dictionary or an XMLElement structure depending on what the format of the response
            //body is.
            if (pJsonResponse)
            {
                //if the query was a list the total number of telements will be given at the front of the response text like this:
                //{"@total":"2",
                //just manually parse it for a total attribute rather than spending the cycles putting the entire response into a 
                //dictionary first
                if (!string.IsNullOrEmpty(res.ResponseText))
                {
                    //unfortunately the different interfaces send counts back differently - check for both
                    int iPos = res.ResponseText.IndexOf("{\"@total\":\"");
                    if (iPos < 0 | iPos > 10)
                    {
                        iPos = res.ResponseText.IndexOf("{\"@total\"=\"");
                        if (iPos < 0 | iPos > 10)
                        {
                            //not a valid position or missing
                            return res;
                        }
                    }

                    //account for length of "total" token
                    iPos += 11;

                    int iPos2 = res.ResponseText.IndexOf(",", iPos, StringComparison.InvariantCulture);
                    if (iPos2 <= iPos | iPos2 > 20)
                    {
                        //check for 2nd construction
                        iPos2 = res.ResponseText.IndexOf("}", iPos, StringComparison.InvariantCulture);
                        if (iPos2 <= iPos | iPos2 > 20)
                        {
                            //invalid
                            return res;
                        }
                    }

                    string strCount = res.ResponseText.Substring(iPos, iPos2-iPos).TrimEnd('\"').TrimStart('\"');
                    int iTemp;
                    if (int.TryParse(strCount, out iTemp))
                    {
                        res.TotalObjectCount = iTemp;
                    }
                    else
                    {
                        if (Debugger.IsAttached) Debugger.Break();
                        RaiseErrorEvent("Failed converting count to integer in GetCupiResponse:" + strCount);
                    }
                    return res;
                }
            }
            else
            {
                //return the results as an XML set if there's anything provided.
                res.XmlElement = GetXElementFromString(res.ResponseText);
                //if we're doing a GET query there will be a "total" attribute on the returned XML indicating how many objects matched on the server
                //side which may be more than the result set being returned if we're paging results (which we must on large systems).  Fetch this 
                //value off and include it in the result set if its there.
                res.TotalObjectCount = 0;

                if ((res.XmlElement != null) && res.XmlElement.Attribute("total") != null)
                {
                    var xAttribute = res.XmlElement.Attribute("total");
                    if (xAttribute != null)
                    {
                        res.TotalObjectCount = int.Parse(xAttribute.Value);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Primary method for sending/fetching data to and from the Connection server via CUPI - tries to parse results returned into XML format.
        /// Use the GetJsonResponse if you're sending/recieving data in JSON format instead.
        /// </summary>
        /// <param name="pUrl">
        /// Full URL to send to Connection - format should look like:
        /// https://{Connection Server Name}:8443/vmrest/users
        /// </param>
        /// <param name="pMethod">
        /// GET, PUT, POST, DELETE method type
        /// </param>
        /// <param name="pConnectionServer">
        /// instance of the ConnectionServer object 
        /// </param>
        /// <param name="pRequestBody">
        /// If the command (for instance a POST) include the need for a post body for additional data, include it here.  Not all commands
        /// require this (GET calls for instance).
        /// </param>
        /// <param name="pJson"></param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... associated
        /// with the call so the calling party can easily log details in the event of a failure.
        /// </returns>        
        public static WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServer pConnectionServer,string pRequestBody,
            bool pJson = true)
        {
            //invalidate the cookie if it's been more than a minute - more aggressive than necessary but safe.
            if ((DateTime.Now - pConnectionServer.LastSessionActivity).TotalSeconds > 60)
            {
                pConnectionServer.LastSessionCookie = "";
            }

            WebCallResult res = GetCupiResponse(pUrl, pMethod, pConnectionServer.LoginName, pConnectionServer.LoginPw,pRequestBody,
                pConnectionServer.LastSessionCookie, pJson);

            //update the details of the session cookie and last connect time.  If the session cookie contains an instance
            //of JSESSIONID in it then store the entire cookie in the LastSessionCookie property and roll the date/time 
            //note of the last time it was updated.
            if (res.Success)
            {
                //update the last time we had activity using the current cookie
                pConnectionServer.LastSessionActivity = DateTime.Now;
             
                //if the response includes a new JSESSIONID (and/or JSESSIONIDSSO) update the cookie.
                if ((!string.IsNullOrEmpty(res.SessionCookie)) && (res.SessionCookie.IndexOf("JSESSIONID=") > 0))
                {
                    pConnectionServer.LastSessionCookie = res.SessionCookie;
                }
            }

            return res;
        }


        /// <summary>
        /// Primary method for sending/fetching data to and from the Connection server via CUMI - tries to parse results returned into JSON format.
        /// Use the GetCupiResponse if you're sending/recieving data in CUPI/XML format instead.
        /// </summary>
        /// <param name="pUrl">
        /// Full URL to send to Connection - format should look like:
        /// https://{Connection Server Name}:8443/vmrest/users
        /// </param>
        /// <param name="pMethod">
        /// GET, PUT, POST, DELETE method type
        /// </param>
        /// <param name="pLoginName">
        /// The login name used to authenticate to the CUPI interface on Connection.
        /// </param>
        /// <param name="pLoginPw">
        /// The login password used to authenticate to the CUPI interface on Connection
        /// </param>
        /// <param name="pJsonParams">
        /// Dictionary of name/value pairs as strings - this method will construct it into a JSON body and include it with the request.  You can 
        /// pass NULL for this value if the call does not require a body.
        /// </param>
        /// <param name="pResults">
        /// Results (if any) are passed back as a string/object dictionary - you must always pass this in even if you don't expect the server 
        /// to return any results.
        ///  </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... associated
        /// with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        public static WebCallResult GetJsonResponse(string pUrl, MethodType pMethod, string pLoginName, string pLoginPw,
                                                    Dictionary<string, string> pJsonParams, out Dictionary<string, object> pResults)
        {
            pResults = new Dictionary<string, object>();

            StringBuilder strRequestBody = new StringBuilder();
            //construct the JSON format request body based on name/value pairs passed in via dictionary
            if (pJsonParams != null && pJsonParams.Count > 0)
            {
                strRequestBody.Append("{");
                bool pFirstPair = true;
                foreach (KeyValuePair<string, string> oPair in pJsonParams)
                {
                    if (pFirstPair)
                    {
                        pFirstPair = false;
                    }
                    else
                    {
                        strRequestBody.Append(",");
                    }

                    strRequestBody.Append("\"");
                    strRequestBody.Append(oPair.Key);
                    strRequestBody.Append("\":");
                    strRequestBody.Append("\"");
                    strRequestBody.Append(oPair.Value);
                    strRequestBody.Append("\"");
                }
                strRequestBody.Append("}");
            }

            //fetch the raw results from the Connection server - anything returned by the server will be stored in the ResponseText
            //in the WebCallResult instance.
            WebCallResult res = GetHttpResponse(pUrl, pMethod, pLoginName, pLoginPw, strRequestBody.ToString(), true);

            //if we get a result text blob back, try and parse it out and check for a "total" item in there.  This gets used for 
            //paging scenarios and such.
            if (res.Success == false || string.IsNullOrEmpty(res.ResponseText))
            {
                return res;
            }

            //use the JavaScriptSerializer to dump what's in the returned text into the string/object dictionary.  
            try
            {
                //var jss = new JavaScriptSerializer();
                //pResults = jss.Deserialize<Dictionary<string, object>>(res.ResponseText);
                pResults = JsonConvert.DeserializeObject<Dictionary<string, object>>(res.ResponseText);
            }
            catch (Exception ex)
            {
                RaiseErrorEvent("Failed to parse JSON response:" + ex);
            }
            return res;
        }

        /// <summary>
        /// helper function to fetch the resonse text stream off a HTTPWebResponse object - this gets used to both get the response back from the server
        /// on a good call as well as dig the error details off the exception thrown on a bad call.
        /// </summary>
        /// <param name="pResponse">
        /// the HTTPWebResponse to parse out for text
        /// </param>
        /// <returns>
        /// String of the response text sent back from the server or an error message with info about the failure to pull the response out.
        /// </returns>
        private static string GetResponseText(HttpWebResponse pResponse)
        {
            StreamReader reader;

            if (pResponse==null)
            {
                return "";
            }
            
            try
            {
                var oStream = pResponse.GetResponseStream();
                if (oStream == null)
                {
                    return "Failure getting response stream";
                }
                reader = new StreamReader(oStream);
            }
            catch (Exception ex)
            {
                return "Failure getting stream reader from response stream in GetResponseText on HTTPFunctions.cs:" + ex.Message;
            }

            string strRet = reader.ReadToEnd();

            reader.Dispose();

            return strRet;

        }


        /// <summary>
        /// helper function to parse out the XML of a result string returned from the server - both good data returned on a fetch/find and error information
        /// returned on a bad operation return XML in the response body in most cases - this is used to parse it out so we can return nicely formatted 
        /// XML structures in the resturn class.  Ignore all errors here, some response bodies don't have valid XML so just return null and move on.
        /// </summary>
        /// <param name="pString">
        /// string containing an XML element to parse out and return
        /// </param>
        /// <returns>
        /// XElement object
        /// </returns>
        private static XElement GetXElementFromString(string pString)
        {
            try
            {
               return XElement.Parse(pString);
            }
            catch
            {
                //not all responses can be parsed into XML - so this isn't an error condition
                return null;
            }
        }

        #endregion


        #region Recorded Media Handling Methods

        /// <summary>
        ///     This routine is used for download a WAV file from a remote Connection server for a voice name or greeting.  Note that this cannot be used for 
        ///     downloading messages (voice mail or broadcast messages) or prompts - this is only used for voice names and greetings at present.
        /// </summary>
        /// <remarks>
        ///     This is a general purpose WAV download routine that can be used for greetings, voice names or interview handlers - this does
        ///     not leverage the URI style media specific formats of the REST interface into CUPI, but is the existing CUALS web interface that's
        ///     been in place since 7.0(2) and will work across all versions of Connection (COBRAS uses this).  I'm using it here to simplify
        ///     fetching media files through a single method here - all that's needed is the actual WAV file name (GUID followed by a .wav).
        /// </remarks>
        /// <param name="pServerName" type="string">
        ///     The server name or IP address of the Connection server to download the WAV file from.
        /// </param>
        /// <param name="pLogin" type="string">
        ///     The login name to use - the account used must have administrator rights (remote DB administration rights are not required).
        /// </param>
        /// <param name="pPassword" type="string">
        ///     The password for the login account.
        /// </param>
        /// <param name="pLocalWavFilePath" type="string">
        ///     The full path to stored the downloaded WAV file locally.
        /// </param>
        /// <param name="pConnectionFileName" type="string">
        ///     The file name on the remote Connection server to download.
        /// </param>
        /// <returns>
        ///     An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        ///     associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>    
        public static WebCallResult DownloadWavFile(string pServerName, string pLogin, string pPassword, string pLocalWavFilePath, string pConnectionFileName)
        {
            //if the target file is already occupied, delete it here.
            if (File.Exists(pLocalWavFilePath))
                File.Delete(pLocalWavFilePath);

            //this is the general CUALS web interface that will fetch any stream file exposed in the streams folder by name.
            string strUrl = @"https://" + pServerName + "/cuals/VoiceServlet?filename=" + pConnectionFileName;

            return DownloadMediaFile(strUrl, pLogin, pPassword, pLocalWavFilePath);

        }


        /// <summary>
        ///     This routine is used for download a message attachment from a remote Connection server for a voice name or greeting.  
        /// </summary>
        /// <remarks>
        ///     This is a general purpose WAV download routine that can be used for all binary attachments for messages (i.e. WAV files).
        /// </remarks>
        /// <param name="pBaseUrl">
        ///  Base URL for VMRest - usually something like "https://(server name):8442/vmrest/".  This is created and stored off the 
        /// ConnserverServer object when it's created.
        /// </param>
        /// <param name="pLogin" type="string">
        ///     The login name to use - the account used must have administrator rights (remote DB administration rights are not required).
        /// </param>
        /// <param name="pPassword" type="string">
        ///     The password for the login account.
        /// </param>
        /// <param name="pLocalWavFilePath" type="string">
        ///     The full path to stored the downloaded WAV file locally.
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
        public static WebCallResult DownloadMessageAttachment(string pBaseUrl, string pLogin, string pPassword, string pLocalWavFilePath, 
                                                                string pUserObjectId, string pMessageObjectId, int pAttachmentNumber)
        {
            //if the target file is already occupied, delete it here.
            if (File.Exists(pLocalWavFilePath))
                File.Delete(pLocalWavFilePath);

            //this is the general CUALS web interface that will fetch any stream file exposed in the streams folder by name.
            string strUrl = string.Format(@"{0}messages/{1}/attachments/{2}?userobjectid={3}", pBaseUrl, pMessageObjectId, 
                pAttachmentNumber, pUserObjectId);

            return DownloadMediaFile(strUrl, pLogin, pPassword, pLocalWavFilePath);

        }


        /// <summary>
        ///    General HTTP media fetch routine used for getting greetings, message attachments, voice names etc... you pass in the full URL
        /// to the media resource on the target along with the authentication to use and this will download the media to the file indicated
        /// in the target file name parameter.
        /// This method does "gate" access by creating an exclusive lock around the actual HTTP fetch here - this prevents Connection from 
        /// getting hammered with too many media requests if a multiple threaded applicaiton is at play.
        /// </summary>
        /// <remarks>
        ///     This is a general purpose WAV download routine that can be used for all binary attachments for messages (i.e. WAV files).
        /// </remarks>
        /// <param name="pFullUrl">
        /// Full URL including https:// etc... for fetching the resrouce - can be a CUALS call, VMRest stream file or a CUMI message attachment
        /// construction.
        /// </param>
        /// <param name="pLogin" type="string">
        ///     The login name to use - the account used must have administrator rights (remote DB administration rights are not required).
        /// </param>
        /// <param name="pPassword" type="string">
        ///     The password for the login account.
        /// </param>
        /// <param name="pLocalWavFilePath" type="string">
        ///     The full path to stored the downloaded WAV file locally.
        /// </param>
        /// <returns>
        ///     An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        ///     associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>    
        private static WebCallResult DownloadMediaFile(string pFullUrl, string pLogin, string pPassword, string pLocalWavFilePath)
        {

            WebCallResult res = new WebCallResult();
            byte[] buffer = new byte[4097];


            //ensure that only one thread at a time is in the web request/response section at a time
            lock (ThisLock)
            {
                //if the target file is already occupied, delete it here.
                if (File.Exists(pLocalWavFilePath))
                    File.Delete(pLocalWavFilePath);

                RaiseDebugEvent("**** Sending to server ****");
                RaiseDebugEvent("    URI:" + pFullUrl);
                RaiseDebugEvent("    Method: GET");

                //large try block here - many of these web calls can fail - wrapping them all individually is a bit much.
                try
                {
                    //create a web request to the URL   
                    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(pFullUrl);

                    webReq.Credentials = new NetworkCredential(pLogin, pPassword);
                    webReq.KeepAlive = false;
                    
                    //always stick the authorization item in the header - the .NET library only does this on a challange
                    //which wastes a trip
                    string authInfo = pLogin + ":" + pPassword;
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                    webReq.Headers["Authorization"] = "Basic " + authInfo;

                    HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                    Stream sourceStream = response.GetResponseStream();

                    //SourceStream has no ReadAll, so we must read data block-by-block   

                    //file stream to store wave file to
                    using (FileStream tempStream = File.Create(pLocalWavFilePath))
                    {
                        int blockSize;
                        do
                        {
                            if (sourceStream != null)
                            {
                                blockSize = sourceStream.Read(buffer, 0, 4096);
                            }
                            else
                            {
                                res.ErrorText = "(warning) empty source stream returned in DownloadMessageAttachment";
                                RaiseErrorEvent(res.ErrorText);
                                return res;
                            }
                            if (blockSize > 0)
                            {
                                tempStream.Write(buffer, 0, blockSize);
                            }
                        } while (blockSize > 0);
                    }

                    //GET the response handle
                    HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();

                    //Now, read the response (the string), and output it.
                    Stream answer = webResp.GetResponseStream();

                    string strResponse;
                    if (answer != null)
                    {
                        StreamReader myReader = new StreamReader(answer);
                        strResponse = myReader.ReadToEnd();
                    }
                    else
                    {
                        res.ErrorText = "(warning) empty answer response returned in DownloadMessageAttachment on ConnectionWAVFiles";
                        RaiseErrorEvent(res.ErrorText);
                        return res;
                    }

                    //close up shop
                    answer.Dispose();
                    webResp.Close();

                    //the response SHOULD be blank if all goes well - if it's not blank but did not raise an error, that's weird but 
                    //not technically something to get worked up about - note it.
                    if (strResponse.Length > 0)
                    {
                        res.ErrorText = "(warning) response handle returned in DownloadMessageAttachment:" + strResponse;
                        RaiseErrorEvent(res.ErrorText);
                        return res;
                    }

                    //only if we made it this far do we declare victory and pass back true
                    res.Success = true;
                    return res;

                }
                catch (Exception em)
                {
                    res.ErrorText = "(error) in DownloadWAVFile on DownloadMessageAttachment:" + em.Message;
                    RaiseErrorEvent(res.ErrorText);
                    return res;
                }
            }
        }


        /// <summary>
        ///     This routine is used for upload a WAV file to a remote Connection server for a voice name or greeting.  Note that this cannot be used for uploading
        ///     messages (voice mail or broadcast messages) or prompts - this is only used for voice names and greetings at present.
        ///     You must first allocate a stream file name on the Connection server which is passed into this routine along with the server name, login and 
        ///     password.
        /// </summary>
        /// <param name="pFullResourcePath" type="string">
        ///     <para>
        ///         Path to the resource stream.  For instance a user's voice name resource path look something like:
        ///         https://ConnectionServer1.MyCompany.com:8443/vmrest/users/51e94483-2dec-43b1-974e-2b9320b86d78/voicename
        ///     </para>
        /// </param>
        /// <param name="pLogin" type="string">
        ///     <para>
        ///         The login name to use - the account used must have administrator rights (remote DB administration rights are not required).
        ///     </para>
        /// </param>
        /// <param name="pPassword" type="string">
        ///     <para>
        ///         The password for the login account.
        ///     </para>
        /// </param>
        /// <param name="pLocalWavFilePath" type="string">
        ///     <para>
        ///         The full path to the local WAV file to upload to the remote Connection server.
        ///     </para>
        /// </param>
        /// <returns>
        ///     An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        ///     associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        public static WebCallResult UploadWavFile(string pFullResourcePath, string pLogin, string pPassword, string pLocalWavFilePath)
        {
            WebCallResult res = new WebCallResult();
            byte[] buffer;
            FileStream streamTemp = null;

            res.Success = false;
            
            //check the inputs up front
            if (File.Exists(pLocalWavFilePath) == false)
            {
                res.ErrorText = "(error) invalid local WAV file path passed to UploadWAVFile:" + pLocalWavFilePath;
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            if (string.IsNullOrEmpty(pFullResourcePath))
            {
                res.ErrorText = "(error) invalid resource path passed to UploadWAVFile:" + pFullResourcePath;
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            if (string.IsNullOrEmpty(pLogin) | string.IsNullOrEmpty(pPassword))
            {
                res.ErrorText = "Empty login or password passed to UploadWavFile";
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            //load the file up into a binary array and then close the file
            try
            {
                streamTemp = File.Open(pLocalWavFilePath, FileMode.Open);
                BinaryReader binReader = new BinaryReader(streamTemp);
                buffer = new byte[Convert.ToInt32(binReader.BaseStream.Length) + 1];
                binReader.Read(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                res.ErrorText="(error) opening wav file as binary stream in UploadWAVFile:" + ex.Message;
                return res;
            }
            finally
            {
                if (streamTemp != null) streamTemp.Dispose();
            }

            if (buffer.Length < 10)
            {
                res.ErrorText="(error) invalid WAV file referenced in UploadWAVFile - length is less than 10 bytes";
                return res;
            }

            string strUrl = pFullResourcePath;

            //large try block here - many of these web calls can fail - wrapping them all individually is a bit much.
            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(strUrl);

                webReq.Method = "PUT";
                webReq.ContentLength = buffer.Length;
                webReq.ContentType = "audio/wav";

                RaiseDebugEvent("**** Sending to server ****");
                RaiseDebugEvent("    URI:" + webReq.RequestUri);
                RaiseDebugEvent("    Method: PUT");
                RaiseDebugEvent("    ContentType:" + webReq.ContentLength);

                webReq.Credentials = new NetworkCredential(pLogin, pPassword);
                webReq.KeepAlive = false;
                webReq.ServicePoint.Expect100Continue = false;
                webReq.AllowWriteStreamBuffering = false;
                webReq.PreAuthenticate = true;

                //always stick the authorization item in the header - the .NET library only does this on a challange
                //which wastes a trip
                string authInfo = pLogin + ":" + pPassword;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                webReq.Headers["Authorization"] = "Basic " + authInfo;


                //open a stream for writing the postvars
                Stream postData = webReq.GetRequestStream();
                postData.Write(buffer, 0, buffer.Length);
                postData.Close();

                //GET the response handle
                HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();

                //Now, read the response (the string), and output it.
                Stream answer = webResp.GetResponseStream();
                string strResponse;
                if (answer != null)
                {
                    StreamReader myReader = new StreamReader(answer);

                    //slurp in the response
                    strResponse = myReader.ReadToEnd();
                }
                else
                {
                    res.ErrorText = "(warning) empty response returned in UploadWAVFile on HTTPFunctions.cs";
                    RaiseErrorEvent(res.ErrorText);
                    return res;
                }

                //close up shop
                answer.Dispose();
                webResp.Close();

                //the response SHOULD be blank if all goes well - if it's not blank but did not raise an error, that's weird but 
                //not technically something to get worked up about - note it.
                if (strResponse.Length > 0)
                {
                    res.ErrorText = "(warning) response handle returned in UploadWAVFile:" + strResponse;
                    RaiseErrorEvent(res.ErrorText);
                }

                //only if we made it this far do we declar victory and pass back true
                res.Success = true;
                return res;
            }
            catch (WebException ex)
            {
                //CUPI will return additional information about the error reason in teh ResponseText tucked into the exception's Resonse object
                //here - this only applies if the WebException thrown is a protocol error which in most cases it will be.
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response !=null)
                {
                    //fill out the return structure with as much detail as we call for the calling client to use to figure out what went wrong.
                    res.ErrorText = ((HttpWebResponse) ex.Response).StatusDescription;
                    res.ResponseText = GetResponseText(ex.Response as HttpWebResponse);
                    res.XmlElement = GetXElementFromString(res.ResponseText);
                    res.StatusDescription = ((HttpWebResponse) ex.Response).StatusDescription;
                    res.StatusCode = (int) ((HttpWebResponse) ex.Response).StatusCode;
                    
                    RaiseErrorEvent(res.ToString());
                }
                else
                {
                    //there's not a lot of good reasons the WebException will be thrown without it being a protocol error, but just in case.
                    res.ErrorText = "Web exception error in GetResponse on HTTPFunctions.cs:" + ex.Message;
                    RaiseErrorEvent(res.ErrorText);
                }
            }
            catch (Exception em)
            {
                res.ErrorText = "(error) in UploadWAVFile on HTTPFunctions.cs: " + em.Message;
                RaiseErrorEvent(res.ErrorText);
            }

            return res;
        }


        /// <summary>
        /// Upload a broadcast message to a specified server using CUMI funtions. 
        /// The authenticated user issuing this command MUST have the right to send broadcast messages on their account or this 
        /// will fail and unfortunately there's no clean way to check for that up front or specifically cathch the error that results
        /// from it - you get a generic 400 "bad request" coming back - this is deeply unfortunate as it's a common configuration 
        /// error and somethign that should probably be addressed in the API at some point.
        /// </summary>
        /// <param name="pServerName">
        /// Server name or IP address of the Connection server to upload the broadcast message to.
        /// </param>
        /// <param name="pLogin">
        /// Login name to authenticate against the server with - this account needs to have broadcast message send rights.
        /// </param>
        /// <param name="pPassword">
        /// Password for account to authenticate against the server with.
        /// </param>
        /// <param name="pWavFilePath">
        /// Full path on the local hard drive to a WAV file to use for the broadcast message.
        /// </param>
        /// <param name="pStartDate">
        /// start date for when the message will be active
        /// </param>
        /// <param name="pStartTime"> 
        /// start time (used with the date) for when the message will be active
        /// </param>
        /// <param name="pEndDate">
        /// end date for when the message will be inactivated
        /// </param>
        /// <param name="pEndTime">
        /// end time (used with date) for when the message will be inactivated
        ///  </param>
        /// <returns>
        /// instance of the WebCallResult class - the "Misc" section will contain the path to the wav file uploaded and the start/end 
        /// date/times for the broadcast message.
        /// </returns>
        public static WebCallResult UploadBroadcastMessage(string pServerName, string pLogin, string pPassword, string pWavFilePath, 
            DateTime pStartDate, DateTime pStartTime, DateTime pEndDate, DateTime pEndTime)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            //Disable Expect 100-Continue in header
            ServicePointManager.Expect100Continue = false;

            string uri = "https://" + pServerName + "/vmrest/mailbox/broadcastmessages";

            res.Url = uri;
            res.Method = "POST";

            if (string.IsNullOrEmpty(pServerName) | string.IsNullOrEmpty(pLogin)| string.IsNullOrEmpty(pPassword) | string.IsNullOrEmpty(pWavFilePath))
            {
                res.ErrorText = "Invalid parameters passed to UploadBroadcastMessage on HTTPFunctions.cs";
                RaiseErrorEvent(res.ErrorText);
                return res;

            }

            if (File.Exists(pWavFilePath)==false)
            {
              	res.ErrorText="File not found in UploadBroadcastMessage:"+pWavFilePath;
                return res;
            }

            res.Misc = "WAV file to upload=" + pWavFilePath+Environment.NewLine;

            CookieContainer cookies = new CookieContainer();

            string boundary = DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.CookieContainer = cookies;
            webrequest.UserAgent = "CUMILibraryFunctions";
            webrequest.KeepAlive = true;
            webrequest.Accept = "application/xml";
            webrequest.Timeout = 15 * 1000;
            webrequest.ContentType = "multipart/form-data;boundary=" + boundary;
            webrequest.Method = "POST";
            webrequest.Credentials = new NetworkCredential(pLogin, pPassword);

            //always stick the authorization item in the header - the .NET library only does this on a challange
            //which wastes a trip
            string authInfo = pLogin + ":" + pPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webrequest.Headers["Authorization"] = "Basic " + authInfo;

            //format the date/time so CUMI likes it.
            string strStartDate = string.Format("{0} {1}", pStartDate.ToString("yyyy-MM-dd"),pStartTime.ToString("HH:mm:ss.000"));
            string strEndDate = string.Format("{0} {1}", pEndDate.ToString("yyyy-MM-dd"), pEndTime.ToString("HH:mm:ss.000"));

            string partXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                    + "<BroadcastMessage>"
                        + "<StartDate>" + strStartDate + "</StartDate>" //2011-07-09 14:23:10.000
                        + "<EndDate>" + strEndDate + "</EndDate>" //2011-08-07 14:22:42.000
                        + "<StreamFile>" + Path.GetFileName(pWavFilePath) + "</StreamFile>"  //44.wav
                    + "</BroadcastMessage>";

            res.RequestBody = partXml;

            res.Misc += "Start date=" + pStartDate + Environment.NewLine;
            res.Misc += "End date=" + pEndDate + Environment.NewLine;

            // Build up the post message header
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n");
            sb.Append("--" + boundary);
            sb.Append("\r\n");

            sb.Append("Content-Type: ");
            sb.Append("application/xml");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append(partXml);
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("--" + boundary);

            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("audio/wav");

            sb.Append("\r\n");
            sb.Append("\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array
            // ensuring the boundary appears on a line by itself
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            try
            {
                FileStream fileStream=new FileStream(pWavFilePath,FileMode.Open, FileAccess.Read);

                Stream memStream=new MemoryStream();

                memStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                //add the wav file onto the HTTP 1KB at a time.
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }

                memStream.Write(boundaryBytes, 0, boundaryBytes.Length);

                fileStream.Close();

                webrequest.ContentLength = memStream.Length;
                Stream requestStream = webrequest.GetRequestStream();

                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed processing wav file in UploadBroadcastMessage on HTTPFunctions.cs: " + ex;
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            //this can fail or many reasons but the details sent back from the server are not very helpful unfortunately.
            try
            {
                HttpWebResponse response;
                using (response = webrequest.GetResponse() as HttpWebResponse)
                {
                    if (webrequest.HaveResponse && response != null)
                    {
                        var oStream = response.GetResponseStream();
                        if (oStream == null)
                        {
                            res.ErrorText = "Unable to get response stream in UploadBroadcastMessage in HTTPFunctions.cs";
                            RaiseErrorEvent(res.ErrorText);
                            return res;
                        }
                        var reader = new StreamReader(oStream);
                        res.StatusCode = (int) response.StatusCode;
                        res.ResponseText=GetResponseText(response);
                        res.Success = true;
                        reader.Dispose();
                    }
                }
            }
            catch (WebException ex)
            {
                //CUMI will return additional information about the error reason in teh ResponseText tucked into the exception's Resonse object
                //here - this only applies if the WebException thrown is a protocol error which in most cases it will be.
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response !=null)
                {
                    //fill out the return structure with as much detail as we call for the calling client to use to figure out what went wrong.
                    res.ErrorText = ((HttpWebResponse)ex.Response).StatusDescription;
                    res.ResponseText = GetResponseText(ex.Response as HttpWebResponse);
                    res.XmlElement = GetXElementFromString(res.ResponseText);
                    res.StatusDescription = ((HttpWebResponse)ex.Response).StatusDescription;
                    res.StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                    
                    RaiseErrorEvent(res.ToString());
                }
                else
                {
                    //there's not a lot of good reasons the WebException will be thrown without it being a protocol error, but just in case.
                    res.ErrorText = "Web exception error in GetResponse on HTTPFunctions.cs:" + ex.Message;
                    RaiseErrorEvent(res.ErrorText);
                }
            }
            catch(Exception ex)
            {
                res.ErrorText = string.Format("Failed sending broadcast message to {0}, error={1}", pServerName,ex);
                RaiseErrorEvent(res.ErrorText);
            }
            return res;
        }


        /// <summary>
        /// Upload a new message to the Connection server using a local WAV file as the voice mail attachment.
        /// </summary>
        /// <param name="pServerName">
        /// Connection server the message is being uploaded to.
        /// </param>
        /// <param name="pLogin">
        /// Login credentials to use to attach to Connection
        /// </param>
        /// <param name="pPassword">
        /// Password credentials to use to attach to Connection
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
        /// <param name="pSessionCookie">
        /// session cookie id to use (if any).
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
        public static WebCallResult UploadVoiceMessageWav(string pServerName, string pLogin, string pPassword, string pPathToLocalWav,
            string pMessageDetailsJsonString, string pSenderUserObjectId,string pSessionCookie, string pRecipientJsonString, 
            string pUriConstruction="")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (string.IsNullOrEmpty(pServerName) | string.IsNullOrEmpty(pLogin) | string.IsNullOrEmpty(pPassword) | string.IsNullOrEmpty(pPathToLocalWav))
            {
                res.ErrorText = "Invalid parameters passed to UploadVoiceMessageWav on HTTPFunctions.cs";
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            if (File.Exists(pPathToLocalWav) == false)
            {
                res.ErrorText = "File not found in UploadVoiceMessageWav:" + pPathToLocalWav;
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            //Disable Expect 100-Continue in header
            ServicePointManager.Expect100Continue = false;

            //if a fully qualified uri is passed in, use it - otherwise assume new message upload uri construction
            string uri;
            if (string.IsNullOrEmpty(pUriConstruction))
            {
                uri= "https://" + pServerName + "/vmrest/messages?userobjectid=" + pSenderUserObjectId;
            }
            else
            {
                uri = pUriConstruction;
            }

            res.Url = uri;
            res.Method = "POST";

            res.Misc = "WAV file to upload=" + pPathToLocalWav + Environment.NewLine;

            CookieContainer cookies = new CookieContainer();

            string boundary = "Boundary_"+ DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.CookieContainer = cookies;
            webrequest.UserAgent = "CUMILibraryFunctions";
            webrequest.KeepAlive = true;
            webrequest.Accept = "application/json";
            webrequest.Timeout = 15 * 1000;
            webrequest.ContentType = "multipart/form-data;boundary=" + boundary;
            webrequest.Method = "POST";
            webrequest.Credentials = new NetworkCredential(pLogin, pPassword);

            //always stick the authorization item in the header - the .NET library only does this on a challange
            //which wastes a trip
            string authInfo = pLogin + ":" + pPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webrequest.Headers["Authorization"] = "Basic " + authInfo;

            //if a session ID is passed in, include it in the header
            if (!string.IsNullOrEmpty(pSessionCookie))
            {
                webrequest.Headers["Cookie"] = pSessionCookie;
            }

            // Build up the post message header
            StringBuilder sb = new StringBuilder();

            //message details section
            sb.AppendLine();
            sb.AppendLine ("--" + boundary);
            sb.AppendLine("Content-Type: application/json");
            sb.AppendLine();
            sb.AppendLine(pMessageDetailsJsonString);
            
            //addressing section
            sb.AppendLine("--" + boundary);
            sb.AppendLine("Content-Type: application/json");
            sb.AppendLine();
            sb.AppendLine(pRecipientJsonString);
            
            //WAV attachment section
            sb.AppendLine("--" + boundary);
            sb.AppendLine("Content-Type: audio/wav");
            sb.AppendLine();

            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sb.ToString());

            // Build the trailing boundary string as a byte array
            // ensuring the boundary appears on a line by itself
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            try
            {
                FileStream fileStream = new FileStream(pPathToLocalWav, FileMode.Open, FileAccess.Read);
                Stream memStream = new MemoryStream();
                memStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                //add the wav file onto the HTTP 1KB at a time.
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }

                memStream.Write(boundaryBytes, 0, boundaryBytes.Length);

                fileStream.Close();

                webrequest.ContentLength = memStream.Length;
                Stream requestStream = webrequest.GetRequestStream();

                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed processing wav file in UploadVoiceMessageWav on HTTPFunctions.cs: " + ex;
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            //this can fail or many reasons but the details sent back from the server are not very helpful unfortunately.
            try
            {
                HttpWebResponse response;
                using (response = webrequest.GetResponse() as HttpWebResponse)
                {
                    if (webrequest.HaveResponse && response != null)
                    {
                        var oStream = response.GetResponseStream();
                        if (oStream == null)
                        {
                            res.ErrorText = "Unable to get response stream in UploadVoiceMessageWav on HTTPFunctions.cs";
                            RaiseErrorEvent(res.ErrorText);
                            return res;
                        }
                        var reader = new StreamReader(oStream);
                        res.StatusCode = (int)response.StatusCode;
                        res.ResponseText = GetResponseText(response);
                        res.Success = true;
                        reader.Dispose();
                    }
                }
            }
            catch (WebException ex)
            {
                //CUMI will return additional information about the error reason in the ResponseText tucked into the exception's Resonse object
                //here - this only applies if the WebException thrown is a protocol error which in most cases it will be.
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response !=null)
                {
                    //fill out the return structure with as much detail as we call for the calling client to use to figure out what went wrong.
                    res.ErrorText = ((HttpWebResponse)ex.Response).StatusDescription;
                    res.ResponseText = GetResponseText(ex.Response as HttpWebResponse);
                    res.XmlElement = GetXElementFromString(res.ResponseText);
                    res.StatusDescription = ((HttpWebResponse)ex.Response).StatusDescription;
                    res.StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode;

                    RaiseErrorEvent(res.ToString());
                }
                else
                {
                    //there's not a lot of good reasons the WebException will be thrown without it being a protocol error, but just in case.
                    res.ErrorText = "Web exception error in UploadVoiceMessageWav on HTTPFunctions.cs:" + ex.Message;
                    RaiseErrorEvent(res.ErrorText);
                }
            }
            catch (Exception ex)
            {
                res.ErrorText = string.Format("Failed sending message to {0}, error={1}", pServerName, ex);
                RaiseErrorEvent(res.ErrorText);
            }
            return res;
        }


        /// <summary>
        /// Create a new message using a resourceID for a stream already recorded on the Connection server - used when leveraging 
        /// CUTI to create new messages.
        /// </summary>
        /// <param name="pServerName">
        /// Connection server the message is being created on.
        /// </param>
        /// <param name="pLogin">
        /// Login credentials to use to attach to Connection
        /// </param>
        /// <param name="pPassword">
        /// Password credentials to use to attach to Connection
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
        /// <param name="pSessionCookie">
        /// session cookie id to use (if any).
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
        public static WebCallResult UploadVoiceMessageResourceId(string pServerName, string pLogin, string pPassword, string pResourceId,
            string pMessageDetailsJsonString, string pSenderUserObjectId, string pSessionCookie, string pRecipientJsonString,
             string pUriConstruction = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (string.IsNullOrEmpty(pServerName) | string.IsNullOrEmpty(pLogin) | string.IsNullOrEmpty(pPassword) | string.IsNullOrEmpty(pResourceId))
            {
                res.ErrorText = "Invalid parameters passed to UploadVoiceMessageResourceId on HTTPFunctions.cs";
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            //if a fully qualified uri is passed in, use it - otherwise assume new message upload uri construction
            string uri;
            if (string.IsNullOrEmpty(pUriConstruction))
            {
                uri = "https://" + pServerName + "/vmrest/messages?userobjectid=" + pSenderUserObjectId;
            }
            else
            {
                uri = pUriConstruction;
            }

            res.Url = uri;
            res.Method = "POST";

            res.Misc = "Resource ID to assign as message=" + pResourceId + Environment.NewLine;

            CookieContainer cookies = new CookieContainer();

            string boundary = "Boundary_" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.CookieContainer = cookies;
            webrequest.UserAgent = "CUMILibraryFunctions";
            webrequest.KeepAlive = true;
            webrequest.Accept = "application/json";
            webrequest.Timeout = 15 * 1000;
            webrequest.ContentType = "multipart/form-data;boundary=" + boundary;
            webrequest.Method = "POST";

            //always stick the authorization item in the header - the .NET library only does this on a challange
            //which wastes a trip
            string authInfo = pLogin + ":" + pPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webrequest.Headers["Authorization"] = "Basic " + authInfo;

            webrequest.Credentials = new NetworkCredential(pLogin, pPassword);

            //if a session ID is passed in, include it in the header
            if (!string.IsNullOrEmpty(pSessionCookie))
            {
                webrequest.Headers["Cookie"] = pSessionCookie;
            }

            string strResourceIdXmlString=string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                                                        "<CallControl>" +
                                                        "<op>PLAY</op>" +
                                                        "<resourceType>STREAM</resourceType>" +
                                                        "<resourceId>{0}</resourceId>" +
                                                        "<lastResult>0</lastResult>" +
                                                        "<speed>100</speed>" +
                                                        "<volume>100</volume>" +
                                                        "<startPosition>0</startPosition>" +
                                                        "</CallControl>",pResourceId);

            // Build up the post message header
            StringBuilder sb = new StringBuilder();

            //message details section
            sb.AppendLine();
            sb.AppendLine("--" + boundary);
            sb.AppendLine("Content-Type: application/json");
            sb.AppendLine();
            sb.AppendLine(pMessageDetailsJsonString);

            //addressing section
            sb.AppendLine("--" + boundary);
            sb.AppendLine("Content-Type: application/json");
            sb.AppendLine();
            sb.AppendLine(pRecipientJsonString);

            //Resource ID section
            sb.AppendLine("--" + boundary);
            sb.AppendLine("Content-Type: application/xml");
            sb.AppendLine();
            sb.AppendLine(strResourceIdXmlString);
            sb.AppendLine("--" + boundary + "--");

            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sb.ToString());

            try
            {
                Stream memStream = new MemoryStream();
                memStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                webrequest.ContentLength = memStream.Length;
                Stream requestStream = webrequest.GetRequestStream();

                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed processing wav file in UploadVoiceMessageResourceId on HTTPFunctions.cs: " + ex;
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            //this can fail or many reasons but the details sent back from the server are not very helpful unfortunately.
            try
            {
                HttpWebResponse response;
                using (response = webrequest.GetResponse() as HttpWebResponse)
                {
                    if (webrequest.HaveResponse && response != null)
                    {
                        var oStream = response.GetResponseStream();
                        if (oStream == null)
                        {
                            res.ErrorText = "Unable to get response stream in UploadVoiceMessageResourceId";
                            RaiseErrorEvent(res.ErrorText);
                            return res;
                        }
                        var reader = new StreamReader(oStream);
                        res.StatusCode = (int)response.StatusCode;
                        res.ResponseText = GetResponseText(response);
                        res.Success = true;
                        reader.Dispose();
                    }
                }
            }
            catch (WebException ex)
            {
                //CUMI will return additional information about the error reason in the ResponseText tucked into the exception's Resonse object
                //here - this only applies if the WebException thrown is a protocol error which in most cases it will be.
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response !=null)
                {
                    //fill out the return structure with as much detail as we call for the calling client to use to figure out what went wrong.
                    res.ErrorText = ((HttpWebResponse)ex.Response).StatusDescription;
                    res.ResponseText = GetResponseText(ex.Response as HttpWebResponse);
                    res.XmlElement = GetXElementFromString(res.ResponseText);
                    res.StatusDescription = ((HttpWebResponse)ex.Response).StatusDescription;
                    res.StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode;

                    RaiseErrorEvent(res.ToString());
                }
                else
                {
                    //there's not a lot of good reasons the WebException will be thrown without it being a protocol error, but just in case.
                    res.ErrorText = "Web exception error in UploadVoiceMessageResourceId on HTTPFunctions.cs:" + ex.Message;
                    RaiseErrorEvent(res.ErrorText);
                }
            }
            catch (Exception ex)
            {
                res.ErrorText = string.Format("Failed sending message to {0}, error={1}", pServerName, ex);
                RaiseErrorEvent(res.ErrorText);
            }
            return res;
        }



        /// <summary>
        /// This routine will generate a temporary WAV file name on Connecton and uplaod the local WAV file to that location.  If it completes the Connection stream file name
        /// will be returned and can be assigned as the stream file property for a voice name, greeting or interview handler question.  Note that these wav files can NOT be used
        /// for messages (regular or dispatch).
        /// </summary>
        /// <param name="pServerName">
        /// Name or IP address of the Connection server we are uploading the WAV file to.
        /// </param>
        /// <param name="pLogin">
        /// CUPI login name for the Connection server
        /// </param>
        /// <param name="pPassword">
        /// CUPI login password for the Connection server.
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
        public static WebCallResult UploadWavFileToStreamLibrary(string pServerName, string pLogin, string pPassword, string pLocalWavFilePath, out string pConnectionStreamFileName)
        {
            //first, get a temporary stream file name from Connection to use
            WebCallResult res = GetTemporaryStreamFileName(pServerName,pLogin,pPassword,out pConnectionStreamFileName);
            
            if (res.Success==false)
            {
                return res;
            }

            //now we need to upload the local WAV file to the slot we just allocated.
            string strUrl = string.Format("https://{0}:8443/vmrest/voicefiles/{1}", pServerName,pConnectionStreamFileName);

            res = UploadWavFile(strUrl, pLogin, pPassword, pLocalWavFilePath);

            return res;
        }


        /// <summary>
        /// Generates a temporary stream file "slot" on the Connection server so we can upload a wav file for a voice name, greeting, interview handler.  The 
        /// name returned can be used to upload the WAV file to the Connection server and is also used as the file name reference for the object in question 
        /// (i.e. the greeting's stream file name property).
        /// </summary>
        /// <param name="pServerName">
        /// Connection server name to request the temporary stream file on.
        /// </param>
        /// <param name="pLogin">
        /// CUPI login for the Conneciton server.
        /// </param>
        /// <param name="pPassword">
        /// CUPI login password for the Connection server.
        /// </param>
        /// <param name="pTempFileName">
        /// The temporary stream file name generated on the Connection server - this will stick around for a few hours if a wav file hasn't been uploaded to it
        /// and it hasn't been assigned to a property - it'll get deleted in the background if not.
        /// </param>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        /// associiated with the call so the calling party can easily log details in the event of a failure.
        /// <returns></returns>
        private static WebCallResult GetTemporaryStreamFileName(string pServerName, string pLogin, string pPassword,out string pTempFileName)
        {
            pTempFileName = "";

            string strUrl = string.Format("https://{0}:8443/vmrest/voicefiles", pServerName);
            
            WebCallResult res = GetCupiResponse(strUrl, MethodType.POST, pLogin, pPassword, "");

            if (res.Success==false)
            {
                res.ErrorText = "Failed generating new temporary stream file in GetTemporarySTreamFileName";
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            //if the call succeeded then the temporary file name will be in the response text
            if (String.IsNullOrEmpty(res.ResponseText))
            {
                res.ErrorText = "Empty response text returned in GetTemporaryStreamFileName";
                res.Success = false;
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            pTempFileName=res.ResponseText;
            return res;
        }

        #endregion

    }
}
