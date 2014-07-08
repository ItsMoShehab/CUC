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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Internal class used by the ConnectionServer class for communicating via HTTP to target Connection servers.  This single static class
    /// can be used to talk to multiple Connection servers via multiple instances of the ConnectionServer class, however this demonostration
    /// application is NOT designed to be thread safe so it's assumed you are communicating to one server at a time.  That said there is basic
    /// thread locking functionality put into the GetResponse method to prevent messy accidents.
    /// </summary>
    [Serializable]
    public class RestTransportFunctions :IConnectionRestCalls
    {

        #region Fields and Properties

        //used for basic thread locking when communicating via HTTP.  If you wanted to do proper multi threading support via HTTP you'd want 
        //do implement a much more sophisticated approach that is well beyond the scope of this example solution.
        private readonly Object _thisLock = new Object();

        /// <summary>
        /// Used to customize JSON serialization and deserialization behavior - used internally only to hook the error handling event so we 
        /// can easily log out missing properties in classes or properties not coming from REST call that should be.
        /// </summary>
        public static JsonSerializerSettings JsonSerializerSettings { get; private set; }

        /// <summary>
        /// When in debug mode missing member flags are output to the command window, otherwise 
        /// </summary>
        public bool DebugMode { get; set; }

        private int _httpTimeoutSeconds = 25;
        /// <summary>
        /// Adjust the timeout for HTTP calls - defaults to 15 seconds at startup, can be adjusted here.
        /// Accepted values are 1 through 99 seconds.
        /// </summary>
        public int HttpTimeoutSeconds { 
            get
            {
                return _httpTimeoutSeconds;
            } 
            set
            {
                if ((value > 1) & (value < 100))
                {
                    _httpTimeoutSeconds = value;
                }
            } 
        }

        #endregion


        #region Constructors and Destructors


        /// <summary>
        /// Default construtor - attach the VAlidateRemoteCertificate to the validation check so we don't get errors on self signed certificates 
        /// when attaching to Connection servers.
        /// Also sets up the global JsonSerializerSettings for raising an error on a missing property.
        /// </summary>
        public RestTransportFunctions(bool pAllowSelfSignedCertificates=true)
        {
            DebugMode = false;

            //handle self signed certificates
            if (pAllowSelfSignedCertificates)
            {
                ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            }

            ServicePointManager.Expect100Continue = false;
            
            //Json serializer settings - we hook the event message for errors that is exposed via the errorevent we we can log missing properties
            //situations and the like.
            JsonSerializerSettings = new JsonSerializerSettings();
            JsonSerializerSettings.Error += JsonSerializerErrorEvent;
            JsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
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
        private void RaiseErrorEvent(string pLine)
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

        #endregion


        #region Helper Methods

        /// <summary>
        /// Method that fires on the JSON serialization or deserialization errors.  Raises an error event only if debug mode is turned on.
        /// Ignore any error about missing properties when it ends with "URI'" - we don't store the URIs for allt he sub objects since it's
        /// not necessary as all those can easily be derived.
        /// </summary>
        private void JsonSerializerErrorEvent(object sender, ErrorEventArgs e)
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
        private bool ValidateRemoteCertificate(object pSender, X509Certificate pCertificate, X509Chain pChain, SslPolicyErrors pPolicyErrors)
        {
            return true;
        }

        //pass in a string and this will return that string with characters that need to be "Escaped" out of it.
        //For instance the string "this is a test" would look like "this%20is%20a%20test".  Currently the space character
        //is all it handles but the RFC for URI construction does indicate there are other possibilities here so I leave it
        //in its own routine just in case.
        private string EscapeCharactersInUrl(string pUri)
        {
            string strRet= pUri.Replace(" ", "%20");
            return strRet;
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
            //chop out the "wrapper" JSON for easier parsing

            string strCleanJson;

            if (string.IsNullOrEmpty(pTypeNameOverride))
            {
                strCleanJson = ConnectionServerRest.StripJsonOfObjectWrapper(pJson, typeof(T).Name, true);
            }
            else
            {
                strCleanJson = ConnectionServerRest.StripJsonOfObjectWrapper(pJson, pTypeNameOverride, true);
            }

            var oList= JsonConvert.DeserializeObject<List<T>>(strCleanJson, JsonSerializerSettings);
            if (oList == null)
            {
                return new List<T>();
            }
            return oList;
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
            //chop out the "wrapper" JSON for easier parsing
            string strCleanJson;
            if (string.IsNullOrEmpty(pTypeNameOverride))
            {
                strCleanJson = ConnectionServerRest.StripJsonOfObjectWrapper(pJson, typeof(T).Name);
            }
            else
            {
                strCleanJson = ConnectionServerRest.StripJsonOfObjectWrapper(pJson, pTypeNameOverride);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(strCleanJson, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                RaiseErrorEvent("Failed in GetObjectFromJson:" + ex);
                return default(T);
            }
        }

        
        #endregion


        #region HTTP Request and Response Methods

        /// <summary>
        /// Before a request goes out, tack on the basic authentication string and include the JSESSIONID and token
        /// strings from the server we're communiating with if we have them.
        /// </summary>
        private void AddCredentialsAndTokens(ref HttpWebRequest pRequest, ConnectionServerRest pServer)
        {
            //always stick the authorization item in the header - the .NET library only does this on a challange
            //which wastes a trip
            string authInfo = pServer.LoginName + ":" + pServer.LoginPw;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

            pRequest.Credentials = new NetworkCredential(pServer.LoginName, pServer.LoginPw);

            pRequest.Headers["Authorization"] = "Basic " + authInfo;

            //if a session ID is passed in, include it in the header
            if (!string.IsNullOrEmpty(pServer.LastSessionCookie))
            {
                pRequest.Headers["Cookie"] = pServer.LastSessionCookie;
            }
        }

        /// <summary>
        /// Whenever we get a response from the server check the headers in the response and fish out the cookies if it
        /// includes a JSESSIONID and save them for the next request to this server.
        /// </summary>
        private void FetchCookieDetails(HttpWebResponse pResponse, ConnectionServerRest pServer)
        {
            if (pResponse == null | pServer == null)
            {
                return;
            }

            //invalidate the cookie if it's been more than a minute - more aggressive than necessary but safe.
            if ((DateTime.Now - pServer.TimeSessionCookieIssued).TotalSeconds > 60)
            {
                pServer.LastSessionCookie = "";
                pServer.TimeSessionCookieIssued = DateTime.Now;
            }


            //if the server set a session cookie in the header, return it here
            var cookie = pResponse.Headers["Set-Cookie"];
            if (cookie != null)
            {

                //if the response includes a new JSESSIONID (and/or JSESSIONIDSSO) update the cookie.
                if ((!string.IsNullOrEmpty(cookie)) && (cookie.IndexOf("JSESSIONID=") > 0))
                {
                    pServer.TimeSessionCookieIssued = DateTime.Now;
                    
                    string strTemp= ConstructCookieString(pResponse.Headers);
                    if (string.IsNullOrEmpty(strTemp))
                    {
                        pServer.LastSessionCookie = cookie;
                    }
                    else
                    {
                        pServer.LastSessionCookie = strTemp;
                    }
                    RaiseDebugEvent("Setting new cookie:" + cookie);
                }
            }
        }

        private string ConstructCookieString(WebHeaderCollection pHeaders)
        {
            string strNewCookie = "";
            //return "";
            var oVar = pHeaders.GetValues("Set-Cookie");
            if (oVar == null)
            {
                return "";
            }
            foreach (string oHeader in oVar)
            {
                if ((oHeader.Contains("JSESSION") | oHeader.Contains("REQUEST_TOKEN_KEY")) && !oHeader.Contains("expires"))
                {
                    strNewCookie += oHeader + ";";    
                }
                
            }

            return strNewCookie;
        }


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
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer class
        /// </param>
        /// <param name="pRequestBody">
        /// If the command (for instance a POST) include the need for a post body for additional data, include it here.  Not all commands
        /// require this (GET calls for instance).
        /// </param>
        /// <param name="pIsJson">
        /// If passed as true the resquest is formed as a JSON request and the results are assumed to be in the same format.  If the default of 
        /// false is passed it's sent as XML and the response is assumed to be the same.
        ///  </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... associated
        /// with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        private WebCallResult GetHttpResponse(string pUrl, MethodType pMethod, ConnectionServerRest pConnectionServer,
                        string pRequestBody, bool pIsJson = false)
        {
            WebCallResult res = new WebCallResult();
            HttpWebResponse response = null;

            pRequestBody = pRequestBody.HtmlBodySafe();

            //store the request parts in the resonse structure for ease of reference for error logging and such.
            res.Url = EscapeCharactersInUrl(pUrl);
            res.Method = pMethod.ToString();
            res.RequestBody = pRequestBody;
            res.Success = false;

            //ensure that only one thread at a time is in the web request/response section at a time
            lock (_thisLock)
            {
                try
                {
                    HttpWebRequest request = WebRequest.Create(pUrl) as HttpWebRequest;

                    if (request == null)
                    {
                        res.ErrorText ="Error - null returned for WebRequest create in GetResponse on RestTransportFunctions.cs using URL=" +pUrl;
                        RaiseErrorEvent(res.ErrorText);
                        return res;
                    }

                    RaiseDebugEvent("**** Sending to server ****");
                    RaiseDebugEvent("    URI:" + request.RequestUri);
                    RaiseDebugEvent("    Method:" + pMethod);
                    RaiseDebugEvent("    Body:" + pRequestBody);
                    
                    request.Method = pMethod.ToString();
                    request.KeepAlive = true;
                    request.Timeout = HttpTimeoutSeconds * 1000;
                    request.Headers.Add("Cache-Control", "no-cache");

                    AddCredentialsAndTokens(ref request,pConnectionServer);

                    if (pIsJson)
                    {
                        request.ContentType = @"application/json";
                        request.Accept = "application/json, */*";
                    }
                    else
                    {
                        request.ContentType = @"application/xml";
                        request.Accept = "application/xml, */*";
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
                        FetchCookieDetails(response,pConnectionServer);
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
                        else if (ex.Status == WebExceptionStatus.Timeout)
                        {
                            res.ErrorText = "Timeout";
                            res.ResponseText = "Timeout";
                            res.StatusDescription = "Timeout";
                            res.StatusCode = -1;
                            RaiseErrorEvent(res.ToString());
                            return res;
                        }
                        else
                        {
                            //there's not a lot of good reasons the WebException will be thrown without it being a protocol error, but just in case.
                            res.ErrorText = "Web exception error in GetResponse on RestTransportFunctions.cs:" + ex.Message;
                            RaiseErrorEvent(res.ErrorText);
                        }
                    }
                    catch (Exception ex)
                    {
                        //some other error (connection lost, server down etc...) happened, just return it as a general error.
                        res.ErrorText = "Error fetching request in GetResponse on RestTransportFunctions.cs:" + ex.Message;
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
                }//try
                catch (UriFormatException ex)
                {
                    //URI errors almost always mean an invalid parameter (i.e. an alias or ObjectId that is not found) was 
                    //passed
                    res.ErrorText = string.Format("URI Error encountered in GetResponse on RestTransportFunctions.cs{0}\n" 
                                                + "This usually means an invalid property name or ID that could not be found was passed on the URL."
                                                ,ex.Message);

                    RaiseErrorEvent(res.ErrorText);
                    
                    res.Success = false;
                }
                catch (Exception ex)
                {
                    res.ErrorText = "Error encountered in GetResponse on RestTransportFunctions.cs:" + ex.Message;
                    res.Success = false;
                    RaiseErrorEvent(res.ErrorText);
                }
                finally
                {
                    //clean up on the way out of town.
                    if (response != null) response.Close();
                }
            }//Lock
            
            return res;
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
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer object
        /// </param>
        /// <param name="pRequestDictionary">
        /// a string/string dictionary containing the parameters to send in the body of the request formatted for JSON
        /// </param>
        /// <returns>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... associated
        /// with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        public WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServerRest pConnectionServer,
                                                    Dictionary<string,string> pRequestDictionary)
        {
            StringBuilder strRequestBody = new StringBuilder();

            //construct the JSON format request body based on name/value pairs passed in via dictionary
            if (pRequestDictionary != null && pRequestDictionary.Count > 0)
            {
                strRequestBody.Append("{");
                bool pFirstPair = true;
                foreach (KeyValuePair<string, string> oPair in pRequestDictionary)
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

            return GetCupiResponse(pUrl, pMethod, pConnectionServer, strRequestBody.ToString());
        }

        /// <summary>
        /// Primary method for sending/fetching data to and from the Connection server via CUPI - tries to parse results returned into XML format
        /// if XML response is used (pass pJsonResponse = true to skip that).  Results are contained in the WebCallResult class returned.
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
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc... associated
        /// with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        public WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServerRest pConnectionServer, string pRequestBody,
            bool pJsonResponse = true)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "Null ConnectionServer passed to GetCupiResponse on RestTransportFunctions.cs"
                    };
            }

            WebCallResult res = GetHttpResponse(pUrl, pMethod, pConnectionServer, pRequestBody, pJsonResponse);
            
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
                res.JsonDictionary = GetDictionaryFromString(res.ResponseText);
                res.TotalObjectCount = GetTotalObjectCountFromJsonResponse(res.ResponseText);
                return res;
            }

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

            return res;
        }


        /// <summary>
        /// The total object count (used for paging through long lists of items) is presented in the response body in a couple
        /// different ways for different interfaces on Connection - this method checks for them both and digs out the total object
        /// count to pass back in the WebCallResult.
        /// </summary>
        /// <param name="pJsonResponse">
        /// Test of the HTTP response from a call returning JSON
        /// </param>
        /// <returns>
        /// Total object count - 0 is returned if there's a problem parsing the response (or it simply does not contain a total object
        /// count element)
        /// </returns>
        private int GetTotalObjectCountFromJsonResponse(string pJsonResponse)
        {
            if (string.IsNullOrEmpty(pJsonResponse))
            {
                return 0;
            }

            //if the query was a list the total number of telements will be given at the front of the response text like this:
            //{"@total":"2",
            //just manually parse it for a total attribute rather than spending the cycles putting the entire response into a 
            //dictionary first
            if (!string.IsNullOrEmpty(pJsonResponse))
            {
                //unfortunately the different interfaces send counts back differently - check for both
                int iPos = pJsonResponse.IndexOf("{\"@total\":\"");
                if (iPos < 0 | iPos > 10)
                {
                    iPos = pJsonResponse.IndexOf("{\"@total\"=\"");
                    if (iPos < 0 | iPos > 10)
                    {
                        //not a valid position or missing
                        return 0;
                    }
                }

                //account for length of "total" token
                iPos += 11;

                int iPos2 = pJsonResponse.IndexOf(",", iPos, StringComparison.InvariantCulture);
                if (iPos2 <= iPos | iPos2 > 20)
                {
                    //check for 2nd construction
                    iPos2 = pJsonResponse.IndexOf("}", iPos, StringComparison.InvariantCulture);
                    if (iPos2 <= iPos | iPos2 > 20)
                    {
                        //invalid
                        return 0;
                    }
                }

                string strCount = pJsonResponse.Substring(iPos, iPos2 - iPos).TrimEnd('\"').TrimStart('\"');
                int iTemp;
                if (int.TryParse(strCount, out iTemp))
                {
                    return iTemp;
                }

                RaiseErrorEvent("Failed converting count to integer in GetTotalObjectCountFromJsonResponse:" + strCount);
            }
            return 0;
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
        private string GetResponseText(HttpWebResponse pResponse)
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
                //the error will be returned in the WebCallResult but also raise an error event here
                RaiseDebugEvent("Failure getting stream reader from response stream in GetResponseText on RestTransportFunctions.cs:" + ex.Message);
                return "Failure getting stream reader from response stream in GetResponseText on RestTransportFunctions.cs:" + ex.Message;
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
        private XElement GetXElementFromString(string pString)
        {
            try
            {
               return XElement.Parse(pString);
            }
            catch (Exception ex)
            {
                //not all responses can be parsed into XML - so this isn't an error condition
                RaiseDebugEvent("Could not parse XML response body into XElements:"+ex+", body="+pString);
                return null;
            }
        }


        /// <summary>
        /// If the response from a call using JSON contains a text representing a dictionary (common) this will parse it into
        /// a simple string/object name value pair construction that is passed back on the WebCallResult class if the calling
        /// party wishes to use it.
        /// </summary>
        /// <param name="pResponse">
        /// Text of the response body to parse
        /// </param>
        /// <returns>
        /// String/object dictionary - can be empty
        /// </returns>
        private Dictionary<string, object> GetDictionaryFromString(string pResponse)
        {
            Dictionary<string, object> oDict;
            try
            {
                oDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(pResponse);
            }
            catch (Exception ex)
            {
                //not all response bodies will be parsable - not an error event, just raise a debug event
                RaiseDebugEvent("Failure parsing Json response into dictionary:" + ex+", body="+pResponse);
                return new Dictionary<string, object>();
            }

            return oDict;
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
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer class
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
        public WebCallResult DownloadWavFile(ConnectionServerRest pConnectionServer, string pLocalWavFilePath, string pConnectionFileName)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "Null ConnectionServer passed to DownloadWavFile on RestTransportFunctions.cs"
                    };
            }

            //if the target file is already occupied, delete it here.
            if (File.Exists(pLocalWavFilePath))
                File.Delete(pLocalWavFilePath);

            //this is the general CUALS web interface that will fetch any stream file exposed in the streams folder by name.
            string strUrl = @"https://" + pConnectionServer.ServerName + "/cuals/VoiceServlet?filename=" + pConnectionFileName;

            return DownloadMediaFile(strUrl, pConnectionServer, pLocalWavFilePath);

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
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer class
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
        public WebCallResult DownloadMessageAttachment(string pBaseUrl, ConnectionServerRest pConnectionServer, string pLocalWavFilePath, 
                                                                string pUserObjectId, string pMessageObjectId, int pAttachmentNumber)
        {
            //if the target file is already occupied, delete it here.
            if (File.Exists(pLocalWavFilePath))
                File.Delete(pLocalWavFilePath);

            //this is the general CUALS web interface that will fetch any stream file exposed in the streams folder by name.
            string strUrl = string.Format(@"{0}messages/{1}/attachments/{2}?userobjectid={3}", pBaseUrl, pMessageObjectId, 
                pAttachmentNumber, pUserObjectId);

            return DownloadMediaFile(strUrl, pConnectionServer, pLocalWavFilePath);

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
        /// <param name="pConnectionServer">
        /// ConnectonServer class instance
        /// </param>
        /// <param name="pLocalWavFilePath" type="string">
        ///     The full path to stored the downloaded WAV file locally.
        /// </param>
        /// <returns>
        ///     An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        ///     associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>    
        private WebCallResult DownloadMediaFile(string pFullUrl, ConnectionServerRest pConnectionServer, string pLocalWavFilePath)
        {

            WebCallResult res = new WebCallResult();
            byte[] buffer = new byte[4097];


            //ensure that only one thread at a time is in the web request/response section at a time
            lock (_thisLock)
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

                    AddCredentialsAndTokens(ref webReq,pConnectionServer);
                    webReq.KeepAlive = false;
                    
                    HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();

                    FetchCookieDetails(response, pConnectionServer);

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
                    if (!string.IsNullOrEmpty(strResponse))
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
        ///         Path to the resource stream.  For instance a user's voice name resource path look something like:
        ///         https://ConnectionServer1.MyCompany.com:8443/vmrest/users/51e94483-2dec-43b1-974e-2b9320b86d78/voicename
        /// </param>
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer class
        /// </param>
        /// <param name="pLocalWavFilePath" type="string">
        /// The full path to the local WAV file to upload to the remote Connection server.
        /// </param>
        /// <returns>
        ///     An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        ///     associiated with the call so the calling party can easily log details in the event of a failure.
        /// </returns>
        public WebCallResult UploadWavFile(string pFullResourcePath, ConnectionServerRest pConnectionServer, string pLocalWavFilePath)
        {
            WebCallResult res = new WebCallResult();
            byte[] buffer;
            FileStream streamTemp = null;
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer passed to UploadWavFile on RestTransportFunctions.cs";
                return res;
            }
            
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

                webReq.KeepAlive = false;
                webReq.ServicePoint.Expect100Continue = false;
                webReq.AllowWriteStreamBuffering = false;
                webReq.PreAuthenticate = true;

                AddCredentialsAndTokens(ref webReq, pConnectionServer);

                //open a stream for writing the postvars
                Stream postData = webReq.GetRequestStream();
                postData.Write(buffer, 0, buffer.Length);
                postData.Close();

                //GET the response handle
                HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();

                FetchCookieDetails(webResp, pConnectionServer);

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
                    res.ErrorText = "(warning) empty response returned in UploadWAVFile on RestTransportFunctions.cs";
                    RaiseErrorEvent(res.ErrorText);
                    return res;
                }

                //close up shop
                answer.Dispose();
                webResp.Close();

                //the response SHOULD be blank if all goes well - if it's not blank but did not raise an error, that's weird but 
                //not technically something to get worked up about - note it.
                if (!string.IsNullOrEmpty(strResponse))
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
                    res.ErrorText = "Web exception error in GetResponse on RestTransportFunctions.cs:" + ex.Message;
                    RaiseErrorEvent(res.ErrorText);
                }
            }
            catch (Exception em)
            {
                res.ErrorText = "(error) in UploadWAVFile on RestTransportFunctions.cs: " + em.Message;
                RaiseErrorEvent(res.ErrorText);
            }

            return res;
        }


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
        public WebCallResult UploadVoiceMessageWav(ConnectionServerRest pConnectionServer, string pPathToLocalWav,
            string pMessageDetailsJsonString, string pSenderUserObjectId, string pRecipientJsonString, 
            string pUriConstruction="")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer passed to UploadVoiceMessageWav on RestTransportFunctions.cs";
                return res;
            }

            if (string.IsNullOrEmpty(pPathToLocalWav) || !File.Exists(pPathToLocalWav))
            {
                res.ErrorText = "Invalid path to local WAV passed to UploadVoiceMessageWav on RestTransportFunctions.cs:"+pPathToLocalWav;
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            //Disable Expect 100-Continue in header
            ServicePointManager.Expect100Continue = false;

            //if a fully qualified uri is passed in, use it - otherwise assume new message upload uri construction
            string uri;
            if (string.IsNullOrEmpty(pUriConstruction))
            {
                uri= "https://" + pConnectionServer.ServerName + "/vmrest/messages?userobjectid=" + pSenderUserObjectId;
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
            webrequest.Timeout = HttpTimeoutSeconds * 1000;
            webrequest.ContentType = "multipart/form-data;boundary=" + boundary;
            webrequest.Method = "POST";
            
            AddCredentialsAndTokens(ref webrequest,pConnectionServer);

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
                res.ErrorText = "Failed processing wav file in UploadVoiceMessageWav on RestTransportFunctions.cs: " + ex;
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
                        FetchCookieDetails(response, pConnectionServer);

                        var oStream = response.GetResponseStream();
                        if (oStream == null)
                        {
                            res.ErrorText = "Unable to get response stream in UploadVoiceMessageWav on RestTransportFunctions.cs";
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
                    res.ErrorText = "Web exception error in UploadVoiceMessageWav on RestTransportFunctions.cs:" + ex.Message;
                    RaiseErrorEvent(res.ErrorText);
                }
            }
            catch (Exception ex)
            {
                res.ErrorText = string.Format("Failed sending message to {0}, error={1}", pConnectionServer.ServerName, ex);
                RaiseErrorEvent(res.ErrorText);
            }
            return res;
        }


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
        public WebCallResult UploadVoiceMessageResourceId(ConnectionServerRest pConnectionServer, string pResourceId,
            string pMessageDetailsJsonString, string pSenderUserObjectId, string pRecipientJsonString,
             string pUriConstruction = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer passed to UploadVoiceMessageResourceId on RestTransportFunctions.cs";
                return res;
            }

            if (string.IsNullOrEmpty(pResourceId))
            {
                res.ErrorText = "Empty ResourceId passed to UploadVoiceMessageResourceId on RestTransportFunctions.cs";
                RaiseErrorEvent(res.ErrorText);
                return res;
            }

            //if a fully qualified uri is passed in, use it - otherwise assume new message upload uri construction
            string uri;
            if (string.IsNullOrEmpty(pUriConstruction))
            {
                uri = "https://" + pConnectionServer.ServerName + "/vmrest/messages?userobjectid=" + pSenderUserObjectId;
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
            webrequest.Timeout = HttpTimeoutSeconds * 1000;
            webrequest.ContentType = "multipart/form-data;boundary=" + boundary;
            webrequest.Method = "POST";

            AddCredentialsAndTokens(ref webrequest,pConnectionServer);

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
                res.ErrorText = "Failed processing wav file in UploadVoiceMessageResourceId on RestTransportFunctions.cs: " + ex;
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
                        FetchCookieDetails(response, pConnectionServer);

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
                    res.ErrorText = "Web exception error in UploadVoiceMessageResourceId on RestTransportFunctions.cs:" + ex.Message;
                    RaiseErrorEvent(res.ErrorText);
                }
            }
            catch (Exception ex)
            {
                res.ErrorText = string.Format("Failed sending message to {0}, error={1}", pConnectionServer.ServerName, ex);
                RaiseErrorEvent(res.ErrorText);
            }
            return res;
        }


        /// <summary>
        /// This routine will generate a temporary WAV file name on Connecton and uplaod the local WAV file to that location.  If it completes the Connection 
        /// stream file name will be returned and can be assigned as the stream file property for a voice name, greeting or interview handler question.  
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
        public WebCallResult UploadWavFileToStreamLibrary(ConnectionServerRest pConnectionServer, string pLocalWavFilePath, out string pConnectionStreamFileName)
        {
            pConnectionStreamFileName = "";
            if (pConnectionServer == null)
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "Null ConnectionServer passed to UploadWavFileToStreamLibrayr"
                    };
            }

            //first, get a temporary stream file name from Connection to use
            WebCallResult res = GetTemporaryStreamFileName(pConnectionServer,out pConnectionStreamFileName);
            
            if (res.Success==false)
            {
                return res;
            }

            //now we need to upload the local WAV file to the slot we just allocated.
            string strUrl = string.Format("{0}voicefiles/{1}", pConnectionServer.BaseUrl,pConnectionStreamFileName);

            res = UploadWavFile(strUrl, pConnectionServer, pLocalWavFilePath);

            return res;
        }


        /// <summary>
        /// Generates a temporary stream file "slot" on the Connection server so we can upload a wav file for a voice name, greeting, interview handler.  The 
        /// name returned can be used to upload the WAV file to the Connection server and is also used as the file name reference for the object in question 
        /// (i.e. the greeting's stream file name property).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of the ConnectionServer Class
        /// </param>
        /// <param name="pTempFileName">
        /// The temporary stream file name generated on the Connection server - this will stick around for a few hours if a wav file hasn't been uploaded to it
        /// and it hasn't been assigned to a property - it'll get deleted in the background if not.
        /// </param>
        /// An instance of the WebCallResult class is returned containing the success of the call, return codes, raw return text etc...
        /// associiated with the call so the calling party can easily log details in the event of a failure.
        /// <returns></returns>
        private WebCallResult GetTemporaryStreamFileName(ConnectionServerRest pConnectionServer,out string pTempFileName)
        {
            pTempFileName = "";

            string strUrl = string.Format("{0}voicefiles", pConnectionServer.BaseUrl);
            
            WebCallResult res = GetCupiResponse(strUrl, MethodType.POST, pConnectionServer, "",false);

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
