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
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The PrivateList class contains all the properties associated with a private distribution list in Unity Connection that can be fetched via
    /// the CUPI client interface.  This class also contains a number of static and instance methods for finding, deleting, editing and listing 
    /// private distribution lists.
    /// </summary>
    public class PrivateList
    {
        #region Fields and Properties 

        //reference to the ConnectionServer object used to create this distribution list instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        //owner of the private list
        private readonly string _userOwnerObjectId;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new instance of the PrivateList class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this list.  
        /// If you pass the pObjectID or pAlias parameter the list is automatically filled with data for that list from the server.  If neither
        /// is passed an empty instance of the PrivateList class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the list being created.
        /// </param>
        /// <param name="pUserOwnerObjectId">
        /// The owner of the private list
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the list on the home server provided.  If no ObjectId or NumericId is passed then an empty instance 
        /// of the PrivateList class is returned instead.  If both an ObjectId and a numeric Id are passed then the objectId is used only.
        /// </param>
        /// <param name="pNumericId">
        /// Optional parameter for passing in the list ID (starts with 1 and goes up to 99 depending on user COS settings).  If the ObjectId is passed
        /// as blank and this value is greater than 0 then the constructor will attempt to fill in its properties with the data for that list number 
        /// if it can be found.  An exception will be thrown if it cannot be found.
        /// </param>
        public PrivateList(ConnectionServer pConnectionServer, string pUserOwnerObjectId, string pObjectId="", int pNumericId = 0):this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to PrivateList constructor.");
            }

            if (string.IsNullOrEmpty(pUserOwnerObjectId))
            {
                throw new ArgumentException("Empty UserOwnerObjectId passed to PrivateList constructor");
            }

            HomeServer = pConnectionServer;
            _userOwnerObjectId = pUserOwnerObjectId;

            //if the user passed in a specific ObjectId or display name then go load that list up, otherwise just return an empty instance.
            if (string.IsNullOrEmpty(pObjectId) & pNumericId==0)  return;

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetPrivateList(pObjectId, pNumericId);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Private List not found in PrivateList constructor using ObjectId={0} or NumericId={1}\n\r{2}"
                                 , pObjectId, pNumericId, res.ErrorText));
            }
        }


        /// <summary>
        /// General constructor for Json parsing libraries
        /// </summary>
        public PrivateList()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
        }


        #endregion


        #region Private List Properties

        [JsonProperty]
        public string VoiceName { get; private set; }

        /// <summary>
        /// Display name of the private list.
        /// </summary>
        private string _displayName;
        public String DisplayName
        {
            get { return _displayName; }
            set
            {
                _changedPropList.Add("DisplayName", value);
                _displayName = value;
            }
        }

        [JsonProperty]
        public string DtmfName { get; private set; }

        [JsonProperty]
        public string Alias { get; private set; }

        /// <summary>
        /// 1 based ID (up to 99) the private list is assigned to.  This is always filled in and cannot be changed once created.  You can delete
        /// and create a new list but you cannot change the ID on a standing list.
        /// </summary>
        [JsonProperty]
        public int NumericId { get; private set; }

        /// <summary>
        /// Unique GUID for this distribution list.
        /// </summary>
        [JsonProperty]
        public String ObjectId { get; private set; }

        [JsonProperty]
        public string UserObjectId { get; private set; }

        [JsonProperty]
        public bool IsAddressable { get; private set; }

        /// <summary>
        /// lazy fetch of private list members for this list - done as a method instead of a property so the fetch is not done when a 
        /// list of PrivateList objects is bound to a grid or the like.
        /// </summary>
        private List<PrivateListMember> _privateListMembers;
        public List<PrivateListMember> PrivateListMembers(bool pForceRefetchOfData=false)
        {
            if (pForceRefetchOfData)
            {
                _privateListMembers = null;
            }

            if (_privateListMembers == null)
            {
                WebCallResult res= PrivateListMember.GetPrivateListMembers(HomeServer, this.ObjectId, _userOwnerObjectId,
                                                        out _privateListMembers);
                if (res.Success == false)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                }
            }

            return _privateListMembers;
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// Fetches all private lists defined for the currently logged in user (if any).  An empty list may be returned.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the lists are being fetched from.
        /// </param>
        /// <param name="pOwnerUserObjectId">
        /// Owner of the private distribution list
        /// </param>
        /// <param name="pPrivateLists">
        /// The list of private lists returned from the CUPI call (if any) is returned as a generic list of PrivateList class 
        /// instances via this out param.  If no lists are found NULL is returned for this parameter.
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
        public static WebCallResult GetPrivateLists(ConnectionServer pConnectionServer, string pOwnerUserObjectId, out List<PrivateList> pPrivateLists, 
            int pPageNumber = 1, int pRowsPerPage = 20)
        {
            WebCallResult res = new WebCallResult {Success = false};

            pPrivateLists = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPrivateLists";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(string.Format("{0}users/{1}/privatelists", pConnectionServer.BaseUrl, pOwnerUserObjectId),
                "pageNumber=" + pPageNumber, "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's not an error, just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pPrivateLists = new List<PrivateList>();
                return res;
            }

            pPrivateLists = HTTPFunctions.GetObjectsFromJson<PrivateList>(res.ResponseText);

            if (pPrivateLists == null)
            {
                pPrivateLists = new List<PrivateList>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pPrivateLists)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.UserObjectId = pOwnerUserObjectId;
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new private list for the currently logged in user.  The display name and number ID must be provided. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the list is being added.
        /// </param>
        /// <param name="pUserOwnerObjectId">
        /// User that will own the newly created private list
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new list.
        /// </param>
        /// <param name="pNumericId">
        /// Private list Id from 1 to 99 (depending on settings - by default private lists are limited to 20, however this routine does not check
        /// for that). Pass the default of 0 and the list will be created in the next available slot - this is the best method.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddPrivateList(ConnectionServer pConnectionServer,
                                                    string pUserOwnerObjectId,
                                                    string pDisplayName,
                                                    int pNumericId=0)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddPrivateList";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for DisplayName in AddPrivateList on ConnectionServer.cs";
                return res;
            }

            if (pNumericId<1 | pNumericId>99)
            {
                res.ErrorText = "Invalid list Id passed to AddPrivateList on ConnectionServer.cs:" + pNumericId.ToString();
                return res;
            }

            ConnectionPropertyList oPropList = new ConnectionPropertyList();
           
            //cheat here a bit and simply add the alias and display name values to the proplist where it can be tossed into the body later.
            oPropList.Add("DisplayName", pDisplayName);
            oPropList.Add("NumericId", pNumericId);

            string strBody = "<PrivateList>";

            foreach (var oPair in oPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</PrivateList>";

            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + string.Format("users/{0}/privatelists",pUserOwnerObjectId), MethodType.POST,
                                            pConnectionServer,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strTemp = string.Format(@"/vmrest/users/{0}/privatelists/",pUserOwnerObjectId);
                if (res.ResponseText.Contains(strTemp))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strTemp, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new private list on the Connection server directory.  The alias must be provided but the 
        /// extension can be blank.  
        /// </summary>
        /// <remarks>
        /// This is an alternateive AddDistributionList that passes back a PrivateList object with the newly created list filled 
        /// out in it if the add goes through.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the list is being added.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new distribution list.  
        /// </param>
        /// <param name="pOwnerUserObjectId">
        /// The user that will be the owner of the new private list
        /// </param>
        /// <param name="pNumericId">
        /// Numeric id of the private list from 1 to 99.  Pass in 0 and the routine will create the list in the next available slot.
        /// </param>
        /// <param name="oDistributionList">
        /// Out parameter that returns an instance of a CallHandler object if the creation completes ok.  If the user fails the creation then this is 
        /// returned as NULL.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddPrivateList(ConnectionServer pConnectionServer,
                                                    string pOwnerUserObjectId,
                                                    string pDisplayName,
                                                    int pNumericId,
                                                    out PrivateList oDistributionList)
        {
            oDistributionList = null;

            WebCallResult res = AddPrivateList(pConnectionServer, pOwnerUserObjectId,pDisplayName, pNumericId);

            //if the create goes through, fetch the list as an object and return it all filled in.
            if (res.Success)
            {
                res = GetPrivateList(out oDistributionList, pConnectionServer, pOwnerUserObjectId, res.ReturnedObjectId);
            }

            return res;
        }


        /// <summary>
        /// returns a single PrivateList object from an ObjectId string passed in or optionally an alias string.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the list is homed on.
        /// </param>
        /// <param name="pOwnerUserObjectId">
        /// User that owns the private lists to be fetched
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the list to load
        /// </param>
        /// <param name="pDistributionList">
        /// The out param that the filled out instance of the PrivateList class is returned on.
        /// </param>
        /// <param name="pNumericId">
        /// Numeric ide of the private list (optional)
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPrivateList(out PrivateList pDistributionList, ConnectionServer pConnectionServer, string pOwnerUserObjectId,
            string pObjectId = "", int pNumericId=0)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pDistributionList = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetDistributionList";
                return res;
            }

            if (string.IsNullOrEmpty(pOwnerUserObjectId))
            {
                res.ErrorText = "Empty owner objectId passed to GetPrivateList";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((string.IsNullOrEmpty(pObjectId) && pNumericId==0))
            {
                res.ErrorText = "Empty ObjectId and invalid NuemricId passed to GetDistributionList";
                return res;
            }

            //create a new PrivateList instance passing the ObjectId (or alias) which fills out the data automatically
            try
            {
                pDistributionList = new PrivateList(pConnectionServer,pOwnerUserObjectId, pObjectId, pNumericId);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch list in GetDistributionList:" + ex.Message;
            }

            return res;
        }


        /// <summary>
        /// Allows one or more properties on a list to be udpated (for instance display name/DTMFAccessID etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the list is homed.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the list to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a list property name and a new value for that property to apply to the list being updated.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one property
        /// pair needs to be included for the funtion to execute.
        /// </param>
        /// <param name="pUserOwnerObjectId">
        /// User that owns the private list being updated
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdatePrivateList(ConnectionServer pConnectionServer, string pObjectId, ConnectionPropertyList pPropList,
            string pUserOwnerObjectId)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdatePrivateList";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to UpdatePrivateList";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdatePrivateList on ConnectonServer.cs";
                return res;
            }

            string strBody = "<PrivateList>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</PrivateList>";

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + string.Format("users/{0}/privatelists/{1}", pUserOwnerObjectId, pObjectId),
                                            MethodType.PUT,pConnectionServer,strBody,false);

        }


        /// <summary>
        /// DELETE a list from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the list is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the list in the directory.
        /// </param>
        /// <param name="pUserOwnerObjectId">
        /// User that owns the private list being removed.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeletePrivateList(ConnectionServer pConnectionServer, string pObjectId, string pUserOwnerObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeletePrivateList";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + string.Format("users/{0}/privatelists/{1}",pUserOwnerObjectId, pObjectId),
                                            MethodType.DELETE,pConnectionServer, "");
        }


        /// <summary>
        /// Fetches the WAV file for a lists's voice name and stores it on the Windows file system at the file location specified.  If the list does 
        /// not have a voice name recorded, the WebcallResult structure returns false in the success proeprty and notes the list has no voice name in 
        /// the error text.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the list is homed.
        /// </param>
        /// <param name="pUserObjectId">
        /// User that owns the private list to fetch
        /// </param>
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the lists's voice name at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the list.  
        /// </param>
        /// <param name="pConnectionWavFileName">
        /// File name (from the stream files table) to download
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPrivateListVoiceName(ConnectionServer pConnectionServer, string pUserObjectId, string pTargetLocalFilePath, string pObjectId,
            string pConnectionWavFileName)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetPrivateListVoiceName";
                return res;
            }

            if (string.IsNullOrEmpty(pUserObjectId) || string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty objectId or UserObjectId passed to GetPrivateListVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pTargetLocalFilePath) || (Directory.GetParent(pTargetLocalFilePath).Exists == false))
            {
                res.ErrorText = "Invalid local file path passed to GetPrivateListVoiceName: " + pTargetLocalFilePath;
                return res;
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
        /// Uploads a WAV file indicated as a voice name for the target distribution list referenced by the pObjectID value.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the list is homed.
        /// </param>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as a voice name for the list referenced.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the distribution list to upload the voice name WAV file for.
        /// </param>
        /// <param name="pUserObjectId">
        /// User that owns the private list to set the voice name for.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// Converts the wav file into a format Connection can handle before uploading
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetPrivateListVoiceName(ConnectionServer pConnectionServer, string pSourceLocalFilePath, string pObjectId,
            string pUserObjectId, bool pConvertToPcmFirst=false)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetPrivateListVoiceName";
                return res;
            }

            if (string.IsNullOrEmpty(pUserObjectId) || string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty objectId or UserObjectId passed to SetPrivateListVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pSourceLocalFilePath) || (File.Exists(pSourceLocalFilePath) == false))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid local file path passed to SetPrivateListVoiceName: " + pSourceLocalFilePath;
                return res;
            }

            //if the user wants to try and rip the WAV file into PCM 16/8/1 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                string strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pSourceLocalFilePath);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into PCM format in SetPrivateListVoiceName.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted PCM WAV file path not found in SetPrivateListVoiceName: " + strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pSourceLocalFilePath = strConvertedWavFilePath;

            }

            //use the 8.5 and later voice name formatting here which simplifies things a great deal.
            string strResourcePath = string.Format(@"{0}users/{1}/privatelists/{2}/voicename", pConnectionServer.BaseUrl, pUserObjectId, pObjectId);

            //upload the WAV file to the server.
            res = HTTPFunctions.UploadWavFile(strResourcePath, pConnectionServer.LoginName, pConnectionServer.LoginPw, pSourceLocalFilePath);

            return res;
        }


        /// <summary>
        /// Fetch the list of members for the current instance of the PrivateList.  All members of all types are returned in a generic list.
        /// </summary>
        /// <param name="pDistributionListObjectId">
        /// The GUID of the distribution list to get the membership list for
        /// </param>
        /// <param name="pOwnerUserObjectId">
        /// User that owns the private list
        /// </param>
        /// <param name="pMemberList">
        /// Generic list of DistributionListMember objects passed back on an out param.
        /// </param>
        /// <param name="pConnectionServer">
        /// The Connection server that the distribution list is homed on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMembersList(ConnectionServer pConnectionServer, string pDistributionListObjectId, string pOwnerUserObjectId,
            out List<PrivateListMember> pMemberList)
        {
            return PrivateListMember.GetPrivateListMembers(pConnectionServer, pDistributionListObjectId,pOwnerUserObjectId, out pMemberList);
        }


        /// <summary>
        /// Add a user as a member of a private distribution list
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being updated.
        /// </param>
        /// <param name="pPrivateListObjectId">
        /// Private list to add the user to
        /// </param>
        /// <param name="pUserObjectId">
        /// User to add to the list
        /// </param>
        /// <param name="pOwnerUserObjectId">
        /// User that owns the private list being added to
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class.
        /// </returns>
        public static WebCallResult AddMemberUser(ConnectionServer pConnectionServer, string pPrivateListObjectId, string pUserObjectId, string pOwnerUserObjectId)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult
                    {
                        ErrorText = "Null Connection server passed to AddMemberUser",
                        Success = false
                    };
            }

            if (string.IsNullOrEmpty(pUserObjectId) || string.IsNullOrEmpty(pPrivateListObjectId))
            {
                return new WebCallResult
                {
                    ErrorText = "Null Connection server passed to AddMemberUser",
                    Success = false
                };
            }

            string strUrl = string.Format("{0}users/{1}/privatelists/{2}/privatelistmembers", pConnectionServer.BaseUrl, pOwnerUserObjectId, pPrivateListObjectId);

            string strBody = "<PrivateListMember>\n\r";

            strBody += string.Format("<MemberSubscriberObjectId>{0}</MemberSubscriberObjectId>\n\r", pUserObjectId);

            strBody += "</PrivateListMember>\n\r";

            return HTTPFunctions.GetCupiResponse(strUrl,MethodType.POST,pConnectionServer, strBody,false);
        }


        /// <summary>
        /// Add a public distribution list as a member of a private list
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being updated
        /// </param>
        /// <param name="pPrivateListObjectId">
        /// Private list being updated
        /// </param>
        /// <param name="pListObjectId">
        /// Public list to add to the private list membership
        /// </param>
        /// <param name="pOwnerUserObjectId">
        /// User that is the owner of the list being edited.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddMemberPublicList(ConnectionServer pConnectionServer, string pPrivateListObjectId, string pListObjectId,string pOwnerUserObjectId)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult
                    {
                        ErrorText = "Null ConnectionServer passed to AddMemberPublicList",
                        Success = false
                    };
            }


            if (string.IsNullOrEmpty(pOwnerUserObjectId) || string.IsNullOrEmpty(pPrivateListObjectId))
            {
                return new WebCallResult
                {
                    ErrorText = "Null ConnectionServer passed to AddMemberPublicList",
                    Success = false
                };
            }

            string strUrl = string.Format("{0}users/{1}/privatelists/{2}/privatelistmembers", pConnectionServer.BaseUrl,pOwnerUserObjectId, pPrivateListObjectId);

            string strBody = "<PrivateListMember>\n\r";

            strBody += string.Format("<MemberDistributionListObjectId>{0}</MemberDistributionListObjectId>\n\r", pListObjectId);

            strBody += "</PrivateListMember>\n\r";

            return HTTPFunctions.GetCupiResponse(strUrl,MethodType.POST,pConnectionServer, strBody,false);
        }


        ///Not yet supported in CUPI
        /// <summary>
        /// Remove a member from a private distribution list (of any type)
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being updated
        /// </param>
        /// <param name="pPrivateListObjectId">
        /// Private list being updated.
        /// </param>
        /// <param name="pMemberObjectId">
        /// Member to be removed.
        /// </param>
        /// <param name="pOwnerUserObjectId">
        /// Owner of the private list being edited.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult RemoveMember(ConnectionServer pConnectionServer, string pPrivateListObjectId, string pMemberObjectId, string pOwnerUserObjectId)
        {
            string strUrl = string.Format("{0}users/{1}/privatelists/{2}/privatelistmembers/{3}",
                        pConnectionServer.BaseUrl,
                        pOwnerUserObjectId,
                        pPrivateListObjectId,
                        pMemberObjectId);

            return HTTPFunctions.GetCupiResponse(strUrl,MethodType.DELETE,pConnectionServer, "");
        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays the alias, display name and extension of the list
        /// </summary>
        public override string ToString()
        {
            return String.Format("[{0}] x{1}", this.DisplayName, this.NumericId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the distribution list object in "name=value" format - each pair is on its
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
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchPrivateListData()
        {
            return GetPrivateList(this.ObjectId,this.NumericId);
        }


        /// <summary>
        /// Fill the current instance of a PrivateList in with properties fetched from the server.  When getting a list of 
        /// lists or searching by alias you get a "short" list of properties - unfortunatley this is short by 4 or 5 properties which 
        /// is not worth a nested "FullList" and "BaseList" approach used with users (which have a much larger delta in properties). Further
        /// two of those properties missing are the extension and voice name path which are so commonly needed items that it's worth the 
        /// extra fetch to get them here.
        /// </summary>
        /// <param name="pObjectId">
        /// GUID identifier of the list to be fetched.  Either this or the Alias needs to be provided.
        /// </param>
        /// <param name="pNumericId">
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetPrivateList(string pObjectId, int pNumericId)
        {
            WebCallResult res;

            if (string.IsNullOrEmpty(pObjectId))
            {
                //get the list by ID - you can't do this via URL construction so just fetch the entire list of private lists and find the one
                //with the ID you want and copy it into the current instance
                List<PrivateList> oLists;
                res = GetPrivateLists(HomeServer,_userOwnerObjectId, out oLists);
                if (res.Success==false)
                {
                    return res;
                }

                foreach (PrivateList oList in oLists)
                {
                  	if (oList.NumericId==pNumericId)
                  	{
                        //cheesy object copy
                        this.DisplayName = oList.DisplayName;
                  	    this.NumericId = oList.NumericId;
                  	    this.ObjectId = oList.ObjectId;
                  	}
                }

            }
            else
            {
                //go fetch the private list by ObjectId
                string strUrl = string.Format("{0}users/{1}/privatelists/{2}", HomeServer.BaseUrl,_userOwnerObjectId, pObjectId);

                //issue the command to the CUPI interface
                res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer,"");

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
            }
            
            //all the updates above will flip pending changes into the queue - clear that here.
            this.ClearPendingChanges();

            return res;
        }



        /// <summary>
        /// Allows one or more properties on a list to be udpated (for instance display name, DTMFAccessID etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            //check if the list intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText =
                            string.Format("Update called but there are no pending changes for distribution list: {0}", this)
                    };
            }

            //just call the static method with the info from the instance 
            WebCallResult res = UpdatePrivateList(HomeServer, ObjectId, _changedPropList,_userOwnerObjectId);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }

        /// <summary>
        /// DELETE a call handler from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeletePrivateList(HomeServer, ObjectId,_userOwnerObjectId);
        }


        /// <summary>
        /// Uploads a WAV file indicated as a voice name for the target list
        /// </summary>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as a voice name for the list referenced.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// Converts to a WAV format that Connection can handle prior to uploading
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetVoiceName(string pSourceLocalFilePath, bool pConvertToPcmFirst = false)
        {
            //just call the static method with the information from the instance
            return SetPrivateListVoiceName(HomeServer, pSourceLocalFilePath, ObjectId,_userOwnerObjectId,pConvertToPcmFirst);
        }


        /// <summary>
        /// Fetches the WAV file for a lists's voice name and stores it on the Windows file system at the file location specified.  If the list does 
        /// not have a voice name recorded, the WebcallResult structure returns false in the success proeprty and notes the list has no voice name in 
        /// the error text.
        /// </summary>
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the lists's voice name at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetVoiceName(string pTargetLocalFilePath)
        {
            //just call the static method with the info from the instance of this object
            return GetPrivateListVoiceName(HomeServer, _userOwnerObjectId, pTargetLocalFilePath, ObjectId,this.VoiceName);
        }


        /// <summary>
        /// Fetch the list of members for the current instance of the PrivateList.  All members of all types are returned in a generic list.
        /// </summary>
        /// <param name="pMemberList">
        /// Generic list of DistributionListMember objects 
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetMembersList(out List<PrivateListMember> pMemberList)
        {
            return GetMembersList(HomeServer, ObjectId,_userOwnerObjectId, out pMemberList);
        }



        /// <summary>
        /// Add a user as a member of a distribution list
        /// </summary>
        /// <param name="pUserObjectId"></param>
        /// <returns></returns>
        public WebCallResult AddMemberUser(string pUserObjectId)
        {
            return AddMemberUser(HomeServer, ObjectId, pUserObjectId,_userOwnerObjectId);
        }


        /// <summary>
        /// Add a public list as a member of a private list
        /// </summary>
        /// <param name="pListObjectId"></param>
        /// <returns></returns>
        public WebCallResult AddMemberPublicList(string pListObjectId)
        {
            return AddMemberPublicList(HomeServer, ObjectId, pListObjectId,_userOwnerObjectId);
        }


        /// <summary>
        /// Remove a member of a private list
        /// </summary>
        /// <param name="pMemberUserObjectId"></param>
        /// <returns></returns>
        public WebCallResult RemoveMember(string pMemberUserObjectId)
        {
            return RemoveMember(HomeServer, ObjectId, pMemberUserObjectId, _userOwnerObjectId);
        }


        /// <summary>
        /// If the call handler object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }

        #endregion

    }
}
