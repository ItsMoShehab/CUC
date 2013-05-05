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
    /// Read only class for fetching the post greeting recording instances off a Unity Connection installation
    /// </summary>
    public class PostGreetingRecording
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires ConnectionServer where the PostGreetingRecording is homed.  You can optionally pass the objectId
        /// or name of a post greeting recording to have it loaded automatically.
        /// </summary>
        public PostGreetingRecording(ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to PostGreetingRecording construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                return;
            }

            WebCallResult res = GetPostGreetingRecording(pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Unable to find post greeting recording by objectId={0}, name={1}. " +
                                                  "Error={2}", pObjectId, pDisplayName, res));
            }
        }

        /// <summary>
        /// General constructor for Json parsing library
        /// </summary>
        public PostGreetingRecording()
        {
        }

        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object used to create this instance.
        public ConnectionServer HomeServer { get; set; }

        //greeting stream files are fetched on the fly if referenced - implemented as a method instead of a prperty so the 
        //values doesn't get bound when tying a list of objects to a grid control or the like.
        private List<PostGreetingRecordingStreamFile> _greetingStreamFiles;
        public List<PostGreetingRecordingStreamFile> GetGreetingStreamFiles(bool pForceDataRefetch = false)
        {
            if (pForceDataRefetch)
            {
                _greetingStreamFiles = null;
            }
            //fetch greeting options only if they are referenced
            if (_greetingStreamFiles == null)
            {
                GetGreetingStreamFiles(out _greetingStreamFiles);
            }

            return _greetingStreamFiles;
        }

        #endregion


        #region PostGreetingRecording Properties

        [JsonProperty]
        public string DisplayName { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the post greeting recording
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the PostGreetingRecording object instance.
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
        /// Fills current instance of class with details of post greeting recording for objectId passed in if found.
        /// </summary>
        /// <param name="pObjectId">
        /// Unique Id for post greeting recording to load
        /// </param>
        /// <param name="pDisplayName">
        /// Optional name of post greeting recording to find
        /// </param>
        /// <returns>
        /// Instance of WebCallResult class
        /// </returns>
        private WebCallResult GetPostGreetingRecording(string pObjectId, string pDisplayName)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            string strObjectId = pObjectId;

            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdFromName(pDisplayName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    res.ErrorText = "Could not find post greeting recording by name=" + pDisplayName;
                    return res;
                }
            }

            string strUrl = HomeServer.BaseUrl + "postgreetingrecordings/" + strObjectId;

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, HTTPFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }


            return res;
        }

        /// <summary>
        /// Fetch the ObjectId of a post greeting recording by it's name.  Empty string returned if not match is found.
        /// Have to hack this for now since the DisplayName is not indexed for this table and so you cannot query against it via
        /// REST.  
        /// </summary>
        /// <param name="pName">
        /// Name of the post greeting recording to find
        /// </param>
        /// <returns>
        /// ObjectId of post greeting recording if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pName)
        {
            List<PostGreetingRecording> oGreetings;
            WebCallResult res = GetPostGreetingRecordings(HomeServer, out oGreetings);

            if (res.Success == false || oGreetings.Count==0)
            {
                return "";
            }

            foreach (PostGreetingRecording oGreeting in oGreetings)
            {
                if (oGreeting.DisplayName.Equals(pName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oGreeting.ObjectId;
                }
            }
            return "";
        }
        
        /// <summary>
        /// Update a post greeting recording - the only item you're allowed to change is the display name
        /// </summary>
        /// <param name="pDisplayName">
        /// Updated name
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public WebCallResult Update(string pDisplayName)
        {
            return UpdatePostGreetingRecording(HomeServer, ObjectId, pDisplayName);
        }


        public WebCallResult Delete()
        {
            return DeletePostGreetingRecording(HomeServer, ObjectId);
        }

        /// <summary>
        /// If you have a recording stream already recorded and in the stream files table on the Connection server (for instance
        /// you are using the telephone as a media device) you can assign a greeting stream file directly to a greeting using this 
        /// method instead of uploading a WAV file from the local hard drive.
        /// </summary>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the greeting stream to be assigned.
        /// </param>
        /// <param name="pLanguageId">
        /// The language ID of the WAV file being uploaded (for US English this is 1033).  The LanguageCodes enum defined in the ConnectionTypes
        /// class can be helpful here.  The language must be installed and active on the Connection server for this to be allowed.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetRecordingToStreamFile(string pStreamFileResourceName,int pLanguageId)
        {
            return SetPostGreetingRecordingToStreamFile(HomeServer, pStreamFileResourceName, ObjectId, pLanguageId);
        }


        /// <summary>
        /// Sets the post greeting recording for a particular language.  The WAV file is uploaded (after optionally being converted 
        /// to a format Conneciton will accept).
        /// </summary>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system to the WAV file to be uploaded as the greeting.
        /// </param>
        /// <param name="pLanguageId">
        /// The language ID of the WAV file being uploaded (for US English this is 1033).  The LanguageCodes enum defined in the ConnectionTypes
        /// class can be helpful here.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// Defaults to false, but if passed as true this has the target WAV file first converted PCM, 16 Khz, 8 bit mono before uploading.  This 
        /// helps ensure Connection will not complain about the media file format.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetRecordingToWavFile(string pSourceLocalFilePath,int pLanguageId,bool pConvertToPcmFirst = false)
        {
            return SetPostGreetingRecordingWavFile(HomeServer, pSourceLocalFilePath, ObjectId, pLanguageId,pConvertToPcmFirst);
        }


        //helper function to fetch all custom post greeting stream files devined on the server
        private WebCallResult GetGreetingStreamFiles(out List<PostGreetingRecordingStreamFile> pGreetingStreamFiles)
        {
            return PostGreetingRecordingStreamFile.GetGreetingStreamFiles(HomeServer, ObjectId, out pGreetingStreamFiles);
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all post greeting recordings and resturns them as a generic list of PostGreetingRecording objects.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that the greetings should be pulled from
        /// </param>
        /// <param name="pPostGreetingRecordings">
        /// Out parameter that is used to return the list of greeting objects defined on Connection - there must be at least one.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPostGreetingRecordings(ConnectionServer pConnectionServer, 
            out List<PostGreetingRecording> pPostGreetingRecordings, int pPageNumber = 1, int pRowsPerPage = 20)
        {
            WebCallResult res;
            pPostGreetingRecordings = null;

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetPostGreetingRecordings";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "postgreetingrecordings", 
                "pageNumber=" + pPageNumber, "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - there can be no post greeting recordings on a system
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pPostGreetingRecordings = new List<PostGreetingRecording>();
                return res;
            }

            pPostGreetingRecordings = HTTPFunctions.GetObjectsFromJson<PostGreetingRecording>(res.ResponseText);

            if (pPostGreetingRecordings == null)
            {
                pPostGreetingRecordings = new List<PostGreetingRecording>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pPostGreetingRecordings)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }

        /// <summary>
        /// Create a new post greeting recording in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the new post greeting recording - should be unique.
        /// </param>
        /// <param name="pPostGreetingRecording">
        /// Instance of newly created post greeting recording is returned on this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddPostGreetingRecording(ConnectionServer pConnectionServer,string pDisplayName, 
            out PostGreetingRecording pPostGreetingRecording)

        {
            pPostGreetingRecording = null;

            WebCallResult res = AddPostGreetingRecording(pConnectionServer, pDisplayName);

            if (res.Success)
            {
                //fetch the instance of the post greeting recording just created.
                try
                {
                    pPostGreetingRecording = new PostGreetingRecording(pConnectionServer, res.ReturnedObjectId);
                }
                catch (Exception)
                {
                    res.Success = false;
                    res.ErrorText = "Could not find newly created post greeting recording by objectId:" + res;
                }
            }

            return res;
        }

        /// <summary>
        /// returns a single PostGreetingRecording object from an ObjectId string or display name passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the greeting is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the post greeting recording to load
        /// </param>
        /// <param name="pPostGreetingRecording">
        /// The out param that the filled out instance of the PostGreetingRecording class is returned on.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional display name to search for recording on.  If both the ObjectId and display name are passed, the objectID is used.
        /// The display name search is not case sensitive.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPostGreetingRecording(out PostGreetingRecording pPostGreetingRecording, 
            ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pPostGreetingRecording = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPostGreetingRecording";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((pObjectId.Length == 0) & (pDisplayName.Length == 0))
            {
                res.ErrorText = "Empty objectId and display name passed to GetPostGreetingRecording";
                return res;
            }

            try
            {
                pPostGreetingRecording = new PostGreetingRecording(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch greeting in GetPostGreetingRecording:" + ex.Message;
            }

            return res;
        }


        /// <summary>
        /// Create a new post greeting recording in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pDisplayName">
        /// Name of the new post greeting recording - should be unique.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddPostGreetingRecording(ConnectionServer pConnectionServer,string pDisplayName)
        {

            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddPostGreetingRecording";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for display name in AddPostGreetingRecording";
                return res;
            }

            string strBody = "<PostGreetingRecording>";

            strBody += string.Format("<{0}>{1}</{0}>", "DisplayName", pDisplayName);

            strBody += "</PostGreetingRecording>";

            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "postgreetingrecordings", MethodType.POST, pConnectionServer, 
                strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/postgreetingrecordings/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/postgreetingrecordings/", "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Remove a post greeting recording from the Connection directory.  If this greeting is being referenced the removal will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the greeting is homed on.
        /// </param>
        /// <param name="pPostGreetingRecordingObjectId">
        /// ObjectId of the post greeting recording to delete
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult DeletePostGreetingRecording(ConnectionServer pConnectionServer, string pPostGreetingRecordingObjectId)
        {
            WebCallResult res;
            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Null ConnectionServer reference passed to DeletePostGreetingRecording";
                return res;
            }

            if (string.IsNullOrEmpty(pPostGreetingRecordingObjectId))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Empty objectId passed to DeletePostGreetingRecording";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "postgreetingrecordings/" + pPostGreetingRecordingObjectId,
                                            MethodType.DELETE, pConnectionServer, "");
        }


        /// <summary>
        /// Update a post greeting recording - the only item you're allowed to change is the display name
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that object is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// Unique identifier for the post greeting recording
        /// </param>
        /// <param name="pDisplayName">
        /// Updated name
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult UpdatePostGreetingRecording(ConnectionServer pConnectionServer, string pObjectId, string pDisplayName)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdatePostGreetingRecording";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty objectId passed to UpdatePostGreetingRecording";
                return res;
            }

            string strBody = "<PostGreetingRecording>";

            //tack on the property value pair with appropriate tags
            strBody += string.Format("<{0}>{1}</{0}>", "ObjectId", pObjectId);

            strBody += string.Format("<{0}>{1}</{0}>", "DisplayName", pDisplayName);

            strBody += "</PostGreetingRecording>";

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "postgreetingrecordings/" + pObjectId, 
                MethodType.PUT, pConnectionServer, strBody, false);
        }


        /// <summary>
        /// Sets the post greeting recording for a particular language.  The WAV file is uploaded (after optionally being converted 
        /// to a format Conneciton will accept).
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that houses the post greeting recording being edited.
        /// </param>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system to the WAV file to be uploaded as the greeting.
        /// </param>
        /// <param name="pPostGreetingRecordingObjectId">
        /// The GUID identifying the post greeting recording that owns the greeting being edited.
        /// </param>
        /// <param name="pLanguageId">
        /// The language ID of the WAV file being uploaded (for US English this is 1033).  The LanguageCodes enum defined in the ConnectionTypes
        /// class can be helpful here.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// Defaults to false, but if passed as true this has the target WAV file first converted PCM, 16 Khz, 8 bit mono before uploading.  This 
        /// helps ensure Connection will not complain about the media file format.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetPostGreetingRecordingWavFile(ConnectionServer pConnectionServer,
                                                        string pSourceLocalFilePath,
                                                        string pPostGreetingRecordingObjectId,
                                                        int pLanguageId,
                                                        bool pConvertToPcmFirst = false)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetPostGreetingRecordingWavFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pSourceLocalFilePath) || (File.Exists(pSourceLocalFilePath) == false))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid local file path passed to SetPostGreetingRecordingWavFile: " + pSourceLocalFilePath;
                return res;
            }

            //if the user wants to try and rip the WAV file into G711 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                string strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pSourceLocalFilePath);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into G711 format in SetPostGreetingRecordingWavFile.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted G711 WAV file path not found in SetPostGreetingRecordingWavFile: " +
                                    strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pSourceLocalFilePath = strConvertedWavFilePath;

            }

            //new construction - requires 8.5 or later and is done in one step to send the greeting to the server.
            string strGreetingStreamUriPath = string.Format("https://{0}:8443/vmrest/postgreetingrecordings/{1}/postgreetingrecordingstreamfiles/{2}/audio",
                                         pConnectionServer.ServerName, pPostGreetingRecordingObjectId, pLanguageId);


            res = HTTPFunctions.UploadWavFile(strGreetingStreamUriPath, pConnectionServer.LoginName,
                                              pConnectionServer.LoginPw, pSourceLocalFilePath);


            return res;
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
        /// <param name="pPostGreetingRecordingObjectId"> 
        /// The GUID identifying the post greeting recording that owns the greeting being edited.
        /// </param>
        /// <param name="pLanguageId">
        /// The language ID of the WAV file being uploaded (for US English this is 1033).  The LanguageCodes enum defined in the ConnectionTypes
        /// class can be helpful here.  The language must be installed and active on the Connection server for this to be allowed.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetPostGreetingRecordingToStreamFile(ConnectionServer pConnectionServer,
                                                     string pStreamFileResourceName,
                                                     string pPostGreetingRecordingObjectId,
                                                     int pLanguageId)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetPostGreetingRecordingToStreamFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pStreamFileResourceName))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid stream file resource id passed to SetPostGreetingRecordingToStreamFile";
                return res;
            }

            //construct the full URL to call for updating the greeting to a stream file id
            string strUrl = string.Format(@"{0}postgreetingrecordings/{1}/postgreetingrecordingstreamfiles/{2}",
                    pConnectionServer.BaseUrl, pPostGreetingRecordingObjectId, pLanguageId);

            Dictionary<string, string> oParams = new Dictionary<string, string>();
            Dictionary<string, object> oOutput;

            oParams.Add("op", "RECORD");
            oParams.Add("ResourceType", "STREAM");
            oParams.Add("resourceId", pStreamFileResourceName);
            oParams.Add("lastResult", "0");
            oParams.Add("speed", "100");
            oParams.Add("volume", "100");
            oParams.Add("startPosition", "0");

            return HTTPFunctions.GetJsonResponse(strUrl, MethodType.PUT, pConnectionServer.LoginName,
                                                 pConnectionServer.LoginPw, oParams, out oOutput);
        }

        #endregion
    }
}