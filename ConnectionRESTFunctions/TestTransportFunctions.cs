using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    public class TestTransportFunctions :IConnectionRestCalls
    {

        #region Fields and Properties

        public bool DebugMode { get; set; }
        public int HttpTimeoutSeconds { get; set; }
        public event RestTransportFunctions.LoggingEventHandler ErrorEvents;
        public event RestTransportFunctions.LoggingEventHandler DebugEvents;

        /// <summary>
        /// Used to customize JSON serialization and deserialization behavior - used internally only to hook the error handling event so we 
        /// can easily log out missing properties in classes or properties not coming from REST call that should be.
        /// </summary>
        public static JsonSerializerSettings JsonSerializerSettings { get; private set; }

        #endregion

        #region Constructors and Destructors


        // Default construtor - attach the VAlidateRemoteCertificate to the validation check so we don't get errors on self signed certificates 
        //when attaching to Connection servers.
        public TestTransportFunctions()
        {
            //Json serializer settings - we hook the event message for errors that is exposed via the errorevent we we can log missing properties
            //situations and the like.
            JsonSerializerSettings = new JsonSerializerSettings();
            JsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
        }

        #endregion

        #region Error and Debug Methods

        /// <summary>
        /// If there's one or more clients registered for the ErrorEvent event then issue it here.
        /// </summary>
        /// <param name="pLine">
        /// String to pass back to the receiving method
        /// </param>
        private void RaiseErrorEvent(string pLine)
        {
            //notify registered clients 
            RestTransportFunctions.LoggingEventHandler handler = ErrorEvents;

            if (handler != null)
            {
                RestTransportFunctions.LogEventArgs oArgs = new RestTransportFunctions.LogEventArgs(pLine);
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
            RestTransportFunctions.LoggingEventHandler handler = DebugEvents;

            if (handler != null)
            {
                RestTransportFunctions.LogEventArgs oArgs = new RestTransportFunctions.LogEventArgs(pLine);
                handler(null, oArgs);
            }
        }

        #endregion


        //ReturnSpecificText[blah, blah]
        public enum TestCommandValues { EmptyResultText, InvalidResultText, ErrorResponse, ReturnRequestBody, 
            ReturnSpecificText, GenerateEmptyFile, GenerateMissingFile }


        public List<T> GetObjectsFromJson<T>(string pJson, string pTypeNameOverride = "")
        {
            //chop out the "wrapper" JSON for easier parsing

            string strCleanJson;

            if (string.IsNullOrEmpty(pTypeNameOverride))
            {
                strCleanJson = ConnectionServer.StripJsonOfObjectWrapper(pJson, typeof(T).Name, true);
            }
            else
            {
                strCleanJson = ConnectionServer.StripJsonOfObjectWrapper(pJson, pTypeNameOverride, true);
            }

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(strCleanJson, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                RaiseErrorEvent("Error deserializing Json in GetObjectsFromJson:"+ex);
                return new List<T>();
            }
        }

        public T GetObjectFromJson<T>(string pJson, string pTypeNameOverride = "") where T : new()
        {
            //chop out the "wrapper" JSON for easier parsing
            string strCleanJson;
            if (string.IsNullOrEmpty(pTypeNameOverride))
            {
                strCleanJson = ConnectionServer.StripJsonOfObjectWrapper(pJson, typeof(T).Name);
            }
            else
            {
                strCleanJson = ConnectionServer.StripJsonOfObjectWrapper(pJson, pTypeNameOverride);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(strCleanJson, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                
                RaiseErrorEvent("Error converting Json in GetObjectFromJson:"+ex);
                return default(T);
            }
        }

        /// <summary>
        /// Test harness - pass response command via the pUrl string parameter using the TestCommandValues enum as a guide.
        /// </summary>
        /// <param name="pUrl"></param>
        /// <param name="pMethod"></param>
        /// <param name="pConnectionServer"></param>
        /// <param name="pRequestBody"></param>
        /// <param name="pJsonResponse"></param>
        /// <returns></returns>
        public WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServer pConnectionServer, string pRequestBody,
                                             bool pJsonResponse = true)
        {
            WebCallResult res = new WebCallResult {Success = true, StatusCode = 200};

            //contains string can come in on the url or in the body depending on the test
            string pSpecificString="";
            if (pRequestBody.Contains("ReturnSpecificText["))
            {
                pSpecificString = pRequestBody;
            }
            if (pUrl.Contains("ReturnSpecificText["))
            {
                pSpecificString = pUrl;
            }

            if (pSpecificString.Contains("ReturnSpecificText["))
            {
                //pull out the text between [ and ] and return it
                int iPos = pSpecificString.IndexOf("ReturnSpecificText[");
                int iPos2 = pSpecificString.IndexOf("]", iPos);
                if (iPos2 <= iPos)
                {
                    pConnectionServer.RaiseErrorEvent("Error parsing ReturnSpecificText harness call:" + pSpecificString);
                }
                else
                {
                    res.ResponseText = pSpecificString.Substring(iPos + 19, iPos2 - iPos - 19);
                    return res;
                }
            }

            if (pUrl.Contains("EmptyResultText"))
            {
                return res;
            }

            if (pUrl.Contains("InvalidResultText"))
            {
                res.TotalObjectCount = 1;
                res.ResponseText = "invalid text in body response";
                return res;
            }
    
            if (pUrl.Contains("ErrorResponse"))
            {
                res.Success = false;
                res.ErrorText = "testing error condition";
                res.StatusCode = 444;
                res.StatusDescription = "testing error condition";
                return res;
            }

            if (pUrl.Contains("ReturnRequestBody"))
            {
                res.ResponseText = pRequestBody;
                return res;
            }

            //dummy up the version check
            if (pUrl.Contains("vmrest/version"))
            {
                res.ResponseText = "{\"name\":\"vmrest\",\"version\":\"1.0.0.1\"}";
                return res;
            }

            //dummy up the servers (cluster) check
            if (pUrl.Contains("vmrest/vmsservers"))
            {
                res.ResponseText = "{\"@total\":\"1\",\"VmsServer\":[{\"URI\":\"/vmrest/vmsservers/99846e45-c254-4755-aec4-341503683cee\",\"ObjectId\":\"99846e45-c254-4755-aec4-341503683cee\",\"ServerName\":\"cuc10b164\",\"IpAddress\":\"192.168.0.186\",\"VmsServerObjectId\":\"99846e45-c254-4755-aec4-341503683cee\",\"ClusterMemberId\":\"0\",\"ServerState\":\"1\",\"HostName\":\"cuc10b164.jefflocal.org\",\"ServerDisplayState\":\"3\",\"SubToPerformReplicationRole\":\"false\"}]}";
                return res;
            }

            return res;
        }


        /// <summary>
        /// Test harness - pass response command via the pUrl string parameter using the TestCommandValues enum as a guide.
        /// </summary>
        /// <param name="pUrl"></param>
        /// <param name="pMethod"></param>
        /// <param name="pConnectionServer"></param>
        /// <param name="pRequestDictionary"></param>
        /// <returns></returns>
        public WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServer pConnectionServer,
                                             Dictionary<string, string> pRequestDictionary)
        {
            return GetCupiResponse(pUrl, pMethod, pConnectionServer, "");
        }

        /// <summary>
        /// Test harness - pass response command via the pConnectionFileName string parameter using the TestCommandValues 
        /// enum as a guide
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pLocalWavFilePath"></param>
        /// <param name="pConnectionFileName"></param>
        /// <returns></returns>
        public WebCallResult DownloadWavFile(ConnectionServer pConnectionServer, string pLocalWavFilePath, string pConnectionFileName)
        {
            WebCallResult res = new WebCallResult {Success = true};

            switch (pConnectionFileName)
            {
                case "EmptyResultText": //return empty response text
                    break;
                case "InvalidResultText": //return response text that is not going to parse into an object
                    res.ResponseText = "invalid text in body response";
                    break;
                case "Error": //generate a generic error with no response text
                    res.Success = false;
                    res.ErrorText = "testing error condition";
                    res.StatusCode = 444;
                    res.StatusDescription = "testing error condition";
                    break;
                case "GenerateEmptyFile":  //create an empty file at the location
                    using (File.Create(pLocalWavFilePath))
                    break;
                case "GenerateMissingFile":
                    if (File.Exists(pLocalWavFilePath))
                    {
                        try
                        {
                            File.Delete(pLocalWavFilePath);
                        }
                        catch (Exception ex)
                        {
                            res.ErrorText = "failed deleting temporary file:" + ex;
                            res.Success = false;
                        }
                    }
                    break;
                default:
                    //just fall through for now
                    break;
            }
            return res;
        }

        public WebCallResult DownloadMessageAttachment(string pBaseUrl, ConnectionServer pConnectionServer, string pLocalWavFilePath,
                                                       string pUserObjectId, string pMessageObjectId, int pAttachmentNumber)
        {
            WebCallResult res = new WebCallResult { Success = true };

            switch (pBaseUrl)
            {
                case "EmptyResultText": //return empty response text
                    break;
                case "InvalidResultText": //return response text that is not going to parse into an object
                    res.ResponseText = "invalid text in body response";
                    break;
                case "Error": //generate a generic error with no response text
                    res.Success = false;
                    res.ErrorText = "testing error condition";
                    res.StatusCode = 444;
                    res.StatusDescription = "testing error condition";
                    break;
                case "GenerateEmptyFile":  //create an empty file at the location
                    using (File.Create(pLocalWavFilePath))
                        break;
                case "GenerateMissingFile":
                    if (File.Exists(pLocalWavFilePath))
                    {
                        try
                        {
                            File.Delete(pLocalWavFilePath);
                        }
                        catch (Exception ex)
                        {
                            res.ErrorText = "failed deleting temporary file:" + ex;
                            res.Success = false;
                        }
                    }
                    break;
                default:
                    //just fall through for now
                    break;
            }
            return res;
        }



        public WebCallResult UploadWavFile(string pFullResourcePath, ConnectionServer pConnectionServer, string pLocalWavFilePath)
        {
            throw new NotImplementedException();
        }

        public WebCallResult UploadVoiceMessageWav(ConnectionServer pConnectionServer, string pPathToLocalWav,
                                                   string pMessageDetailsJsonString, string pSenderUserObjectId,
                                                   string pRecipientJsonString, string pUriConstruction = "")
        {
            throw new NotImplementedException();
        }

        public WebCallResult UploadVoiceMessageResourceId(ConnectionServer pConnectionServer, string pResourceId,
                                                          string pMessageDetailsJsonString, string pSenderUserObjectId,
                                                          string pRecipientJsonString, string pUriConstruction = "")
        {
            throw new NotImplementedException();
        }

        public WebCallResult UploadWavFileToStreamLibrary(ConnectionServer pConnectionServer, string pLocalWavFilePath,
                                                          out string pConnectionStreamFileName)
        {
            throw new NotImplementedException();
        }
    }
}
