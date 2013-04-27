using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    public class InterviewQuestion
    {

        #region Fields and Properties

        public int QuestionNumber { get; set; }
        public int MaxMsgLength { get; set; }
        public string StreamText { get; set; }
        public bool IsActive { get; set; }
        public string InterviewHandlerObjectId { get; set; }
        public string VoiceFile { get; set; }

        //reference to the ConnectionServer object used to create this handlers instance.
        internal ConnectionServer HomeServer { get; private set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Empty constructor for JSON parser
        /// </summary>
        public InterviewQuestion()
        {
        }

        public InterviewQuestion(ConnectionServer pHomeServer, string pInterviewHandlerObjectId, int pInterviewQuestionNumber)
        {
            if (pHomeServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to InterviewQuestion constructor");
            }

            HomeServer = pHomeServer;

            if (string.IsNullOrEmpty(pInterviewHandlerObjectId))
            {
                throw new ArgumentException("Empty interview handler objectId passed to InterviewQuestion constructor");
            }

            var res= GetInterviewQuestion(pInterviewHandlerObjectId, pInterviewQuestionNumber);

            if (res.Success == false)
            {
                throw new Exception("Failed fetching interview question:"+res);
            }
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// Returns all questions for a specific interview handler
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handler questions are being fetched from.
        /// </param>
        /// <param name="pInterviewHandlerObjectId">
        /// The unique identifier for the interview handler to fetch questions for.
        /// </param>
        /// <param name="pInterviewQuestions">
        /// The list of questions for the interviewer is passed back on this out param
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetInterviewQuestions(ConnectionServer pConnectionServer, string pInterviewHandlerObjectId,
            out List<InterviewQuestion> pInterviewQuestions)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pInterviewQuestions = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetInterviewQuestions";
                return res;
            }

            if (string.IsNullOrEmpty(pInterviewHandlerObjectId))
            {
                res.ErrorText = "Empty interview handler Id passed to GetInterviewQuestions";
                return res;
            }

            string strUrl = string.Format("{0}handlers/interviewhandlers/{1}/interviewquestions", pConnectionServer.BaseUrl, pInterviewHandlerObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is not an error, just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pInterviewQuestions = new List<InterviewQuestion>();
                return res;
            }

            pInterviewQuestions = HTTPFunctions.GetObjectsFromJson<InterviewQuestion>(res.ResponseText);

            if (pInterviewQuestions == null)
            {
                pInterviewQuestions = new List<InterviewQuestion>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pInterviewQuestions)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pInterviewHandlerObjectId"></param>
        /// <param name="pInterviewQuestionNumber"></param>
        /// <param name="pActive"></param>
        /// <param name="pMaxResponseLength">
        /// Maximum length of recorded response for question (in seconds)
        /// </param>
        /// <param name="pStreamText"></param>
        /// <returns></returns>
        public static WebCallResult UpdateInterviewHandlerQuestion(ConnectionServer pConnectionServer,
                                                                   string pInterviewHandlerObjectId,
                                                                   int pInterviewQuestionNumber, 
                                                                   bool pActive, int pMaxResponseLength,
                                                                   string pStreamText = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateInterviewHandler";
                return res;
            }

            ConnectionPropertyList oPropList=new ConnectionPropertyList();
            oPropList.Add("IsActive", pActive);
            oPropList.Add("MaxMsgLength",pMaxResponseLength);
            
            if (!string.IsNullOrEmpty(pStreamText))
            {
                oPropList.Add("StreamText", pStreamText);
            }

            string strBody = "<InterviewQuestion>";

            foreach (var oPair in oPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</InterviewQuestion>";

            string strUri = string.Format("{0}handlers/interviewhandlers/{1}/interviewquestions/{2}",
                                          pConnectionServer.BaseUrl, pInterviewHandlerObjectId, pInterviewQuestionNumber);

            return HTTPFunctions.GetCupiResponse(strUri,MethodType.PUT, pConnectionServer, strBody, false);

        }

        /// <summary>
        /// returns a single InterviewHandler object from an ObjectId or displayName string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the handler is homed on.
        /// </param>
        /// <param name="pInterviewQuestion">
        /// The out param that the filled out instance of the InterviewQuestion class is returned on.
        /// </param>
        /// <param name="pInterviewHandlerObjectId">
        /// The interview handler to fetch the question from
        /// </param>
        /// <param name="pInterviewQuestionNumber">
        /// Question number to fetch off the handler
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetInterviewQuestion(out InterviewQuestion pInterviewQuestion, ConnectionServer pConnectionServer,
            string pInterviewHandlerObjectId, int pInterviewQuestionNumber)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pInterviewQuestion = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetInterviewHandler";
                return res;
            }

            //you need an ObjectId and/or a display name - both being blank is not acceptable
            if (string.IsNullOrEmpty(pInterviewHandlerObjectId))
            {
                res.ErrorText = "Empty InterviewHandlerObjectId ppassed to GetInterviewQuestion";
                return res;
            }

            //create a new InterviewHandler instance passing the ObjectId (or display name) which fills out the data automatically
            try
            {
                pInterviewQuestion = new InterviewQuestion(pConnectionServer, pInterviewHandlerObjectId, pInterviewQuestionNumber);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch handler in GetInterviewQuestion:" + ex.Message;
                res.Success = false;
            }

            return res;
        }

        /// <summary>
        /// Fetches the WAV file for a interview question and stores it on the Windows file system at the file location specified.  If the question
        /// does not have voice recorded, the WebcallResult structure returns false in the success proeprty and notes the question has no voice in 
        /// the error text.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the question is homed.
        /// </param>
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the question's voice at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the interview handler that owns the question.  
        /// </param>
        /// <param name="pQuestionNumber">
        /// Number of the question to update (1 through 20)
        /// </param>
        /// <param name="pConnectionWavFileName">
        /// The the Connection stream file name is already known it can be passed in here and the question lookup does not need to take place.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetInterviewHandlerQuestionRecording(ConnectionServer pConnectionServer, string pTargetLocalFilePath, string pObjectId,
            int pQuestionNumber, string pConnectionWavFileName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetInterviewHandlerQuestionRecording";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pTargetLocalFilePath) || (Directory.GetParent(pTargetLocalFilePath).Exists == false))
            {
                res.ErrorText = "Invalid local file path passed to GetInterviewHandlerQuestionRecording: " + pTargetLocalFilePath;
                return res;
            }

            //if the WAV file name itself is passed in that's all we need, otherwise we need to go do a CallHandler fetch with the ObjectId 
            //and pull the VoiceName wav file name from there (if it's present).
            //fetch the call handler info which has the VoiceName property on it
            if (string.IsNullOrEmpty(pConnectionWavFileName))
            {
                InterviewHandler oInterviewHandler;

                try
                {
                    oInterviewHandler = new InterviewHandler(pConnectionServer, pObjectId);
                }
                catch (Exception ex)
                {
                    res.ErrorText = string.Format("Error fetching call handler in GetInterviewHandlerQuestionRecording with objectID{0}\n{1}",
                        pObjectId, ex.Message);
                    return res;
                }

                //now fetch the question
                InterviewQuestion oQuestion;
                res = GetInterviewQuestion(out oQuestion, pConnectionServer, pObjectId,pQuestionNumber);
                if (res.Success == false)
                {
                    return res;
                }

                //the property will be null if no voice is recorded.
                if (string.IsNullOrEmpty(oQuestion.VoiceFile))
                {
                    return new WebCallResult
                        {
                            Success = false,
                            ErrorText = "No question recorded for interview handler question."
                        };
                }

                pConnectionWavFileName = oInterviewHandler.VoiceName;
            }
            //fetch the WAV file
            res = HTTPFunctions.DownloadWavFile(pConnectionServer.ServerName,
                                                pConnectionServer.LoginName,
                                                pConnectionServer.LoginPw,
                                                pTargetLocalFilePath,
                                                pConnectionWavFileName);

            return res;
        }


        /// <summary>
        /// Uploads a WAV file as the target interview handler question referenced by the pObjectID and question number value.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the interview handler is homed.
        /// </param>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as an interview handler question
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the call handler that owns the question to be updated
        /// </param>
        /// <param name="pQuestionNumber">
        /// Interview question to update (1-20)
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// If passed as TRUE the routine will attempt to convert the target WAV file into raw PCM first before uploading it to the Connection
        /// server.  A failure to convert will be considered a failed upload attempt and false is returned.  This value defaults to FALSE meaning
        /// the file will attempt to be uploaded as is.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetInterviewHandlerQuestionRecording(ConnectionServer pConnectionServer, string pSourceLocalFilePath, string pObjectId,
            int pQuestionNumber, bool pConvertToPcmFirst = false)
        {
            string strConvertedWavFilePath = "";
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetInterviewHandlerQuestionRecording";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pSourceLocalFilePath) || (File.Exists(pSourceLocalFilePath) == false))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid local file path passed to SetInterviewHandlerQuestionRecording: " + pSourceLocalFilePath;
                return res;
            }

            //if the user wants to try and rip the WAV file into PCM 16/8/1 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pSourceLocalFilePath);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into PCM format in SetInterviewHandlerQuestionRecording.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted PCM WAV file path not found in SetInterviewHandlerQuestionRecording: " + strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pSourceLocalFilePath = strConvertedWavFilePath;
            }

            //string strResourcePath = string.Format(@"{0}handlers/interviewhandlers/{1}/interviewquestions/{2}", pConnectionServer.BaseUrl,
            //    pObjectId, pQuestionNumber);


            //need to do this via the older "two part" method - upload the file, get the ID back and then do another 
            //update of the object to save the stream file name
            string strStreamFileName;
            res = HTTPFunctions.UploadWavFileToStreamLibrary(pConnectionServer,pSourceLocalFilePath, out strStreamFileName);

            if (res.Success == false)
            {
                return res;
            }

            string strBody = "<InterviewQuestion>";
            strBody += string.Format("<{0}>{1}</{0}>", "VoiceFile", strStreamFileName);
            strBody += "</InterviewQuestion>";

            string strUri = string.Format("{0}handlers/interviewhandlers/{1}/interviewquestions/{2}",
                                          pConnectionServer.BaseUrl, pObjectId, pQuestionNumber);

            res= HTTPFunctions.GetCupiResponse(strUri, MethodType.PUT, pConnectionServer, strBody, false);

            //upload the WAV file to the server.
            //res = HTTPFunctions.UploadWavFile(strResourcePath, pConnectionServer.LoginName, pConnectionServer.LoginPw, pSourceLocalFilePath);

            //if we converted a file to G711 in the process clean up after ourselves here. Only delete it if the upload was good - otherwise
            //keep it around as it may be useful for diagnostic purposes.
            if (res.Success && !string.IsNullOrEmpty(strConvertedWavFilePath) && File.Exists(strConvertedWavFilePath))
            {
                try
                {
                    File.Delete(strConvertedWavFilePath);
                }
                catch (Exception ex)
                {
                    //this is not a show stopper error - just report it back but still return success if that's what we got back from the 
                    //wav upload routine.
                    res.ErrorText = "(warning) failed to delete temporary PCM wav file in SetInterviewHandlerVoiceName:" + ex.Message;
                }
            }
            return res;
        }


        /// <summary>
        /// If you have a recording stream already recorded and in the stream files table on the Connection server (for instance
        /// you are using the telephone as a media device) you can assign a recording stream file directly to an interview handler
        /// question recording
        /// </summary>
        /// <param name="pConnectionServer" type="ConnectionServer">
        ///   The Connection server that houses the recording to be updated      
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the interview handler that owns the question to be updated
        /// </param>
        /// <param name="pQuestionNumber">
        /// Interview handler question number to update (1-20)
        /// </param>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the recording stream to be assigned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetInterviewHandlerQuestionRecordingToStreamFile(ConnectionServer pConnectionServer, string pObjectId,
                                                     int pQuestionNumber, string pStreamFileResourceName)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetInterviewHandlerQuestionRecordingToStreamFile";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to SetInterviewHandlerQuestionRecordingToStreamFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pStreamFileResourceName))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid stream file resource id passed to SetInterviewHandlerQuestionRecordingToStreamFile";
                return res;
            }

            //construct the full URL to call for uploading the voice name file
            string strUrl = string.Format(@"{0}handlers/interviewhandlers/{1}/interviewquestions/{2}", pConnectionServer.BaseUrl, 
                pObjectId, pQuestionNumber);

            Dictionary<string, string> oParams = new Dictionary<string, string>();
            Dictionary<string, object> oOutput;

            oParams.Add("op", "RECORD");
            oParams.Add("ResourceType", "STREAM");
            oParams.Add("resourceId", pStreamFileResourceName);
            oParams.Add("lastResult", "0");
            oParams.Add("speed", "100");
            oParams.Add("volume", "100");
            oParams.Add("startPosition", "0");

            res = HTTPFunctions.GetJsonResponse(strUrl, MethodType.PUT, pConnectionServer.LoginName,
                                                 pConnectionServer.LoginPw, oParams, out oOutput);

            return res;
        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays the display name and extension of the handler by default.
        /// </summary>
        public override string ToString()
        {
            return String.Format("Interview question #{0} [{1}]", this.QuestionNumber, this.StreamText);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the interview handler object in "name=value" format - each pair is on its
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
        /// Fills the current instance of InterviewHandler in with properties fetched from the server.  If both the display name and ObjectId
        /// parameters are provided, the ObjectId is used for the search.
        /// </summary>
        /// <param name="pInterviewHandlerObjectId">
        /// Unique GUID of the interview handler to fetch - can be blank if the display name is passed in.
        /// </param>
        /// <param name="pQuestionNumber">
        /// Display name (required to be unique for all interview handlers) to search on an interview handler by.  Can be blank if the ObjectId 
        /// parameter is provided.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetInterviewQuestion(string pInterviewHandlerObjectId, int pQuestionNumber)
        {
            WebCallResult res;

            if (string.IsNullOrEmpty(pInterviewHandlerObjectId))
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "No value for ObjectId or display name passed to GetInterviewHandler."
                    };
            }

            string strUrl = string.Format("{0}handlers/interviewhandlers/{1}/interviewquestions/{2}", HomeServer.BaseUrl, pInterviewHandlerObjectId,pQuestionNumber);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(HTTPFunctions.StripJsonOfObjectWrapper(res.ResponseText, "InterviewQuestion"), this,
                    HTTPFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }

            return res;
        }


        /// <summary>
        /// Get an interview handler's recorded question wav file downloaded to a local file on the hard drive.
        /// If there is no recording this returns a failure.
        /// </summary>
        /// <param name="pTargetLocalFilePath">
        /// Local path to download the WAV file to.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetQuestionRecording(string pTargetLocalFilePath)
        {
            //no need to do the fetching if there's no voice file recording for the question
            if (string.IsNullOrEmpty(VoiceFile))
            {
                return new WebCallResult {Success = false, ErrorText = "No recording for question"};
            }

            return GetInterviewHandlerQuestionRecording(HomeServer, pTargetLocalFilePath, InterviewHandlerObjectId,
                                                        QuestionNumber, VoiceFile);
        }


        /// <summary>
        /// Set the recording of an interview handler question to a WAV file on the local hard drive.
        /// </summary>
        /// <param name="pSourceLocalFilePath">
        /// Full path to the WAV file on the local file to upload
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// Pass as true to convert to PCM file format Connection prefers for recordings prior to update.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetQuestionRecording(string pSourceLocalFilePath, bool pConvertToPcmFirst=false)
        {
            return SetInterviewHandlerQuestionRecording(HomeServer, pSourceLocalFilePath, InterviewHandlerObjectId,
                                                        QuestionNumber, pConvertToPcmFirst);
        }


        /// <summary>
        /// Set the recording for an interview handler question to the content of a stream file recording already on the 
        /// Unity Connection server (recorded via CUTI).
        /// </summary>
        /// <param name="pStreamFileId">
        /// Stream file Id of the recording to use for the question audio content on the server.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetQuestionRecordingToStreamFile(string pStreamFileId)
        {
            return SetInterviewHandlerQuestionRecordingToStreamFile(HomeServer, InterviewHandlerObjectId, QuestionNumber,
                                                                    pStreamFileId);
        }


        /// <summary>
        /// The only items you can update on an interview handler question are the stream text, the length of the recorded
        /// response and setting if it's active or not. The recorded question can be updated/fetched through the special 
        /// purpose methods for that off this class.
        /// </summary>
        /// <param name="pActive">
        /// Pass as true to make the quesiton active, false to disable it such that it's not presented to users.
        /// </param>
        /// <param name="pMaxResponseLength">
        /// Maximum length of recorded response for question (in seconds)
        /// </param>
        /// <param name="pStreamText">
        /// Text description of recorded stream
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update(bool pActive, int pMaxResponseLength, string pStreamText = "")
        {
            return UpdateInterviewHandlerQuestion(HomeServer, InterviewHandlerObjectId, QuestionNumber, pActive, 
                pMaxResponseLength, pStreamText);
        }

        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchInterviewHandlerData()
        {
            return GetInterviewQuestion(this.InterviewHandlerObjectId,QuestionNumber);
        }

        #endregion


    }
}
