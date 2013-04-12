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
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The DistribtuionList class contains all the properties associated with a distribution list in Unity Connection that can be fetched via the 
    /// CUPI interface.  This class also contains a number of static and instance methods for finding, deleting, editing and listing 
    /// distribution lists.
    /// </summary>
    public class DistributionList
    {
        #region Fields and Properties

        //reference to the ConnectionServer object used to create this distribution list instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region Constructors


        /// <summary>
        /// Creates a new instance of the DistributionList class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this list.  
        /// If you pass the pObjectID or pAlias parameter the list is automatically filled with data for that list from the server.  If neither
        /// is passed an empty instance of the DistributionList class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the list being created.
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the list on the home server provided.  If no ObjectId or Alias is passed then an empty instance 
        /// of the DistributionList class is returned instead.
        /// </param>
        /// <param name="pAlias">
        /// Optional alias search critiera - if both ObjectId and Alias are passed, ObjectId is used.  
        /// </param>
        public DistributionList(ConnectionServer pConnectionServer, string pObjectId="",string pAlias=""):this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to DistributionList constructor.");
            }

            HomeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or display name then go load that list up, otherwise just return an empty instance.
            if ((pObjectId.Length == 0) & (pAlias.Length==0)) return;

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetDistributionList(pObjectId,pAlias);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Distribution List not found in DistributionList constructor using ObjectId={0} or DisplayName={1}\n\r{2}"
                                 , pObjectId,pAlias, res.ErrorText));
            }
        }


        /// <summary>
        /// Generic constructor for Json parsing libraries
        /// </summary>
        public DistributionList()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();

            //the data returned when using a search query or getting a list back is "light" a few properties - indicate that as the 
            //default starting state here - if a missing item is fetched a full data fetch is issues on the fly to fill it in.
            IsFullListData = false;
        }

        #endregion


        #region DistributionList Properties

        /// <summary>
        /// Unique for lists in the directory - cannot be changed post create.
        /// </summary>
        [JsonProperty]
        public String Alias { get; private set; }

        private bool _allowContacts;
        /// <summary>
        /// A flag indicating whether contacts (system, VPIM, virtual) are allowed to be members of this Distribution List. Purpose of this flag 
        /// is to enable administrators to create a Distribution List whose members are Unity users only. A flag indicating whether contacts 
        /// (system, VPIM, virtual) are allowed to be members of this Distribution List. Purpose of this flag is to enable administrators to create 
        /// a Distribution List whose members are Unity users only
        /// </summary>
        public bool AllowContacts
        {
            get
            {
                //if the list is "light" from a search result, fill in the missing properties with a seperate fetch.
                if (IsFullListData == false)
                {
                    GetDistributionList(ObjectId);
                }
                return _allowContacts;
            }
            set
            {
                _changedPropList.Add("AllowContacts", value);
                _allowContacts = value;
            }
        }

        private bool _allowForeignMessage;

        /// <summary>
        /// Controls if we allow foreign message systems to send to this particular distribution list. Only valid if the list is for subscribers 
        /// only (i.e. does not allow contacts)
        /// </summary>
        public bool AllowForeignMessage
        {
            get
            {
                //if the list is "light" from a search result, fill in the missing properties with a seperate fetch.
                if (IsFullListData == false)
                {
                    GetDistributionList(ObjectId);
                }
                return _allowForeignMessage;
            }
            set
            {
                _changedPropList.Add("AllowForeignMessage", value);
                _allowForeignMessage = value;
            }
        }

        private DateTime _creationTime;
        /// <summary>
        /// The date/time the list was created - cannot be changed post create.
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                //if the list is "light" from a search result, fill in the missing properties with a seperate fetch.
                if (IsFullListData == false)
                {
                    GetDistributionList(ObjectId);
                }
                return _creationTime;
            }
            set
            {
                _creationTime = value;
            }
        }

        
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

        private String _dtmfAccessId;
        /// <summary>
        ///  The optional extension assigned to a distribution list.
        /// </summary>
        public String DtmfAccessId
        {
            get
            {return _dtmfAccessId;}
            set
            {
                _changedPropList.Add("DtmfAccessId", value);
                _dtmfAccessId = value;
            }
        }

        private string _dtmfName;
        /// <summary>
        /// The series of digits corresponding to the numeric keypad mapping on a standard touchtone phone representing the display name of 
        /// the system distribution list. These digits are used for searching the distribution list by name via the phone.
        /// </summary>
        public String DtmfName
        {
            get
            {
                if (IsFullListData==false)
                {
                    //first - fetch the full list details for this guy then return the value
                    GetDistributionList(ObjectId);
                }
                return _dtmfName;
            }
            set
            {
                _changedPropList.Add("DtmfName", value);
                _dtmfName = value;
            }
        }

        private bool _isPublic;

        /// <summary>
        /// A flag indicating whether the system distribution list is addressable by all subscribers on all VMSes throughout the Unity Organization.
        /// Cannot be changed post creation.
        /// </summary>
        public bool IsPublic
        {
            get
            {
                //if the list is "light" from a search result, fill in the missing properties with a seperate fetch.
                if (IsFullListData == false)
                {
                    GetDistributionList(ObjectId);
                }
                return _isPublic;
            }
            set
            {
                _isPublic = value;
            }
        }

        /// <summary>
        /// By design the distribution list information returned in a list or query search holds back a few properties, a couple of which 
        /// are rather critical (including the voice name).  In that case the IsFullListData is set to false and if a reference is made to 
        /// one of those properties then a full fetch is requested on the fly.  This is awkward at best but avoids unnecessary fetches unless that
        /// data is really needed.
        /// </summary>
        public bool IsFullListData { get; private set; }

        /// <summary>
        /// The unique identifier of the LocationVMS object to which this system distribution list belongs.
        /// </summary>
        [JsonProperty]
        public String LocationObjectId { get; private set; }

        /// <summary>
        /// Unique GUID for this distribution list.  Cannot be changed post create.
        /// </summary>
        [JsonProperty]
        public String ObjectId { get; private set; }

        private string _partitionObjectId;
        /// <summary>
        /// The unique identifier of the Partition to which the DistributionList is assigned
        /// </summary>
        public string PartitionObjectId
        {
            get { return _partitionObjectId; }
            set
            {
                _changedPropList.Add("PartitionObjectId", value);
                _partitionObjectId = value;
            }
        }

        private bool _undeletable;
        /// <summary>
        /// A flag indicating whether this distribution list can be deleted via an administrative application such as Cisco Unity Connection 
        /// Administration. It is used to prevent deletion of factory defaults
        /// </summary>
        public bool Undeletable
        {
            get { return _undeletable; }
            set
            {
                _changedPropList.Add("Undeletable", value);
                _undeletable = value;
            }
        }

        private string _voiceName;
        /// <summary>
        /// The name of the WAV file containing the recorded audio (voice name, greeting, etc.) for the parent object.
        /// </summary>
        public string VoiceName
        {
            get
            {
                //if the list is "light" from a search result, fill in the missing properties with a seperate fetch.
                if (IsFullListData==false)
                {
                    GetDistributionList(ObjectId);
                }
                return _voiceName;
            }
            set
            {
                _changedPropList.Add("VoiceName", value);
                _voiceName = value;
            }
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of lists from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(displayname startswith ab)"
        /// sort: "sort=(displayname asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the lists are being fetched from.
        /// </param>
        /// <param name="pDistributionLists">
        /// The list of distribution lists returned from the CUPI call (if any) is returned as a generic list of DistributionList class 
        /// instances via this out param.  If no lists are found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetDistributionLists(ConnectionServer pConnectionServer, out List<DistributionList> pDistributionLists, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pDistributionLists = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetDistributionLists";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "distributionlists", pClauses);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one template
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                pDistributionLists = new List<DistributionList>();
                res.Success = false;
                return res;
            }

            pDistributionLists = HTTPFunctions.GetObjectsFromJson<DistributionList>(res.ResponseText);

            if (pDistributionLists == null)
            {
                pDistributionLists = new List<DistributionList>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pDistributionLists)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// This function allows for a GET of lists from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(displayname startswith ab)"
        /// sort: "sort=(displayname asc)"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the lists are being fetched from.
        /// </param>
        /// <param name="pDistributionLists">
        /// The list of distribution lists returned from the CUPI call (if any) is returned as a generic list of DistributionList class 
        /// instances via this out param.  If no lists are found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
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

        public static WebCallResult GetDistributionLists(ConnectionServer pConnectionServer,out List<DistributionList> pDistributionLists,
            int pPageNumber=1, int pRowsPerPage=20,params string[] pClauses)
        {
            //tack on the paging items to the parameters list
            var temp = pClauses.ToList();
            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            return GetDistributionLists(pConnectionServer, out pDistributionLists, temp.ToArray());
        }


        /// <summary>
        /// Allows for the creation of a new distribution list on the Connection server directory.  The display name and alias must be provided 
        /// but the extension can be blank.  Other distribution list properties and their values may be passed in via the ConnectonPropertyList 
        /// structure.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the call handler is being added.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new list.
        /// </param>
        /// <param name="pAlias">
        /// The alias of the new list - this needs to be unique for all lists in the directory.
        /// </param>
        /// <param name="pExtension">
        /// The extension number to be assigned to the new list.  This may be blank. The partition is the default partition of the Connection server (the one
        /// created by setup) but can be changed by passing in the ObjectId of a partition in the pPropList or editing it after creation.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a handlers property name and a new value for that property to apply to the list being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddDistributionList(ConnectionServer pConnectionServer,
                                                    string pDisplayName,
                                                    string pAlias,
                                                    string pExtension,
                                                    ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddDistributionList";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pAlias) || string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddDistributionList on ConnectionServer.cs";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            //cheat here a bit and simply add the alias and display name values to the proplist where it can be tossed into the body later.
            pPropList.Add("DisplayName", pDisplayName);
            pPropList.Add("Alias",pAlias);

            if (pExtension.Length > 0)
            {
                pPropList.Add("DtmfAccessId", pExtension);
            }

            string strBody = "<DistributionList>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</DistributionList>";

            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "distributionlists", MethodType.POST,pConnectionServer,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/distributionlists/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/distributionlists/", "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new distribution list on the Connection server directory.  The alias must be provided but the 
        /// extension can be blank.  
        /// </summary>
        /// <remarks>
        /// This is an alternateive AddDistributionList that passes back a DistributionList object with the newly created list filled 
        /// out in it if the add goes through.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the list is being added.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new distribution list.  
        /// </param>
        /// <param name="pAlias">
        /// Alias of the distribution list to be added - this must be unique for all distribution lists.
        /// </param>
        /// <param name="pExtension">
        /// The extension number to be assigned to the new list.  This may be blank. The partition is the default (the one created by Connection 
        /// setup), however it can be assigned to a different partition on the Connection server by editing the object post creation or passing it
        /// in via the pPropList parameter.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a list's property name and a new value for that property to apply to the list being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <param name="oDistributionList">
        /// Out parameter that returns an instance of a CallHandler object if the creation completes ok.  If the user fails the creation then this is 
        /// returned as NULL.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddDistributionList(ConnectionServer pConnectionServer,
                                                    string pDisplayName,
                                                    string pAlias,
                                                    string pExtension,
                                                    ConnectionPropertyList pPropList,
                                                    out DistributionList oDistributionList)
        {
            oDistributionList = null;

            WebCallResult res = AddDistributionList(pConnectionServer, pDisplayName, pAlias, pExtension, pPropList);

            //if the create goes through, fetch the list as an object and return it all filled in.
            if (res.Success)
            {
                res = GetDistributionList(out oDistributionList, pConnectionServer, res.ReturnedObjectId);
            }

            return res;
        }




        /// <summary>
        /// returns a single DistributionList object from an ObjectId string passed in or optionally an alias string.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the list is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the list to load
        /// </param>
        /// <param name="pDistributionList">
        /// The out param that the filled out instance of the DistributionList class is returned on.
        /// </param>
        /// <param name="pAlias">
        /// Optional alias to search for distribution list on.  If both the ObjectId and alias are passed, the objectID is used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetDistributionList(out DistributionList pDistributionList, ConnectionServer pConnectionServer, string pObjectId = "", 
            string pAlias = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pDistributionList = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetDistributionList";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((pObjectId.Length == 0) & (pAlias.Length == 0))
            {
                res.ErrorText = "Empty objectId and alias passed to GetDistributionList";
                return res;
            }

            //create a new DistributionList instance passing the ObjectId (or alias) which fills out the data automatically
            try
            {
                pDistributionList = new DistributionList(pConnectionServer, pObjectId, pAlias);
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
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateDistributionList(ConnectionServer pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateDistributionList";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList==null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateDistributionList on ConnectonServer.cs";
                return res;
            }

            string strBody = "<DistributionList>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</DistributionList>";

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "distributionlists/" + pObjectId,
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
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteDistributionList(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteDistributionList";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "distributionlists/" + pObjectId,
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
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the lists's voice name at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the list.  
        /// </param>
        /// <param name="pConnectionWavFileName">
        /// The the connection stream file name is already known it can be passed in here and the list lookup does not need to take place.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetDistributionListVoiceName(ConnectionServer pConnectionServer, string pTargetLocalFilePath, string pObjectId, 
            string pConnectionWavFileName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetDistributionListVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pTargetLocalFilePath) || (Directory.GetParent(pTargetLocalFilePath).Exists == false))
            {
                res.ErrorText = "Invalid local file path passed to GetDistributionListVoiceName: " + pTargetLocalFilePath;
                return res;
            }

            //if the WAV file name itself is passed in that's all we need, otherwise we need to go do a DistributionList fetch with the ObjectId 
            //and pull the VoiceName wav file name from there (if it's present).
            if (String.IsNullOrEmpty(pConnectionWavFileName))
            {
                DistributionList oDistributionList;

                try
                {
                    oDistributionList = new DistributionList(pConnectionServer, pObjectId);
                }
                catch (Exception ex)
                {
                    res.ErrorText = string.Format("Error fetching list in GetDistributionListVoiceName with objectID{0}\n{1}", pObjectId, ex.Message);
                    return res;
                }

                //the property will be null if no voice name is recorded for the list.
                if (string.IsNullOrEmpty(oDistributionList.VoiceName))
                {
                    res = new WebCallResult();
                    res.Success = false;
                    res.ErrorText = "No voice named recorded for distribution list.";
                    return res;
                }

                pConnectionWavFileName = oDistributionList.VoiceName;
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
        /// <param name="pConvertToPcmFirst">
        /// If passed as TRUE the routine will attempt to convert the target WAV file into raw PCM first before uploading it to the Connection
        /// server.  A failure to convert will be considered a failed upload attempt and false is returned.  This value defaults to FALSE meaning
        /// the file will attempt to be uploaded as is.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetDistributionListVoiceName(ConnectionServer pConnectionServer, string pSourceLocalFilePath, string pObjectId, 
            bool pConvertToPcmFirst = false)
        {
            string strConvertedWavFilePath = "";
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetDistributionListVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pSourceLocalFilePath) || (File.Exists(pSourceLocalFilePath) == false))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid local file path passed to SetDistributionListVoiceName: " + pSourceLocalFilePath;
                return res;
            }

            //if the user wants to try and rip the WAV file into PCM 16/8/1 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pSourceLocalFilePath);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into PCM format in SetDistributionListVoiceName.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted PCM WAV file path not found in SetDistributionListVoiceName: " + strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pSourceLocalFilePath = strConvertedWavFilePath;

            }

            //use the 8.5 and later voice name formatting here which simplifies things a great deal.
            string strResourcePath = string.Format(@"{0}distributionlists/{1}/voicename", pConnectionServer.BaseUrl, pObjectId);

            //upload the WAV file to the server.
            res = HTTPFunctions.UploadWavFile(strResourcePath, pConnectionServer.LoginName, pConnectionServer.LoginPw, pSourceLocalFilePath);

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
                    res.ErrorText = "(warning) failed to delete temporary PCM wav file in SetDistributionListVoiceName:" + ex.Message;
                }
            }
            return res;
        }

        /// <summary>
        /// If you have a recording stream already recorded and in the stream files table on the Connection server (for instance
        /// you are using the telephone as a media device) you can assign a recording stream file directly to a voice name using this 
        /// method instead of uploading a WAV file from the local hard drive.
        /// </summary>
        /// <param name="pConnectionServer" type="ConnectionServer">
        ///   The Connection server that houses the voice name to be updated      
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the list to apply the stream file to the voice name for.
        /// </param>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the recording stream to be assigned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetDistributionListVoiceNameToStreamFile(ConnectionServer pConnectionServer, string pObjectId,
                                                     string pStreamFileResourceName)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetDistributionListVoiceNameToStreamFile";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to SetDistributionListVoiceNameToStreamFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pStreamFileResourceName))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid stream file resource id passed to SetDistributionListVoiceNameToStreamFile";
                return res;
            }

            //construct the full URL to call for uploading the voice name file
            string strUrl = string.Format(@"{0}distributionlists/{1}/voicename", pConnectionServer.BaseUrl, pObjectId);

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


        /// <summary>
        /// Fetch the list of members for the current instance of the DistributionList.  All members of all types are returned in a generic list.
        /// </summary>
        /// <param name="pDistributionListObjectId">
        /// The GUID of the distribution list to get the membership list for
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
        public static WebCallResult GetMembersList(ConnectionServer pConnectionServer, string pDistributionListObjectId, out List<DistributionListMember> pMemberList)
        {
            return DistributionListMember.GetDistributionListMembers(pConnectionServer, pDistributionListObjectId, out pMemberList);
        }


        /// <summary>
        /// Add a user as a member of apublic distribution list
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pDistributionListObjectId"></param>
        /// <param name="pUserObjectId"></param>
        /// <returns></returns>
        public static WebCallResult AddMemberUser(ConnectionServer pConnectionServer, string pDistributionListObjectId, string pUserObjectId)
        {
            string strUrl = string.Format("{0}distributionlists/{1}/distributionlistmembers", pConnectionServer.BaseUrl, pDistributionListObjectId);

            string strBody = "<DistributionListMember>\n\r";

            strBody += string.Format("<MemberUserObjectId>{0}</MemberUserObjectId>\n\r", pUserObjectId);

            strBody += "</DistributionListMember>\n\r";

            return HTTPFunctions.GetCupiResponse(strUrl,MethodType.POST,pConnectionServer, strBody,false);
        }


        /// <summary>
        /// Add a new public distribution list as a member of another public list
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pDistributionListObjectId"></param>
        /// <param name="pListObjectId"></param>
        /// <returns></returns>
        public static WebCallResult AddMemberList(ConnectionServer pConnectionServer, string pDistributionListObjectId, string pListObjectId)
        {
            string strUrl = string.Format("{0}distributionlists/{1}/distributionlistmembers", pConnectionServer.BaseUrl, pDistributionListObjectId);

            string strBody = "<DistributionListMember>\n\r";

            strBody += string.Format("<MemberDistributionListObjectId>{0}</MemberDistributionListObjectId>\n\r", pListObjectId);

            strBody += "</DistributionListMember>\n\r";

            return HTTPFunctions.GetCupiResponse(strUrl,MethodType.POST,pConnectionServer, strBody,false);
        }


        /// <summary>
        /// Remove a member from a public distribution list
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pDistributionListObjectId"></param>
        /// <param name="pMemberUserObjectId"></param>
        /// <returns></returns>
        public static WebCallResult RemoveMember(ConnectionServer pConnectionServer, string pDistributionListObjectId, string pMemberUserObjectId)
        {
            string strUrl = string.Format("{0}distributionlists/{1}/distributionlistmembers/{2}", 
                        pConnectionServer.BaseUrl, 
                        pDistributionListObjectId,
                        pMemberUserObjectId);

            return HTTPFunctions.GetCupiResponse(strUrl,MethodType.DELETE,pConnectionServer, "");
        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays the alias, display name and extension of the list
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} [{1}] x{2}", this.Alias, this.DisplayName,this.DtmfAccessId);
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
        public WebCallResult RefetchDistributionListData()
        {
            return GetDistributionList(this.ObjectId);
        }


        /// <summary>
        /// Fill the current instance of a DistributionList in with properties fetched from the server.  When getting a list of 
        /// lists or searching by alias you get a "short" list of properties - unfortunatley this is short by 4 or 5 properties which 
        /// is not worth a nested "FullList" and "BaseList" approach used with users (which have a much larger delta in properties). Further
        /// two of those properties missing are the extension and voice name path which are so commonly needed items that it's worth the 
        /// extra fetch to get them here.
        /// </summary>
        /// <param name="pObjectId">
        /// GUID identifier of the list to be fetched.  Either this or the Alias needs to be provided.
        /// </param>
        /// <param name="pAlias">
        /// Alias identifying the list to be fetched.  If both this and the ObjectId are passed, the ObjectId is used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetDistributionList(string pObjectId, string pAlias = "")
        {
            WebCallResult res;

            //either alias or ObjectId needs to be passed here.
            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pAlias))
            {
                res = new WebCallResult();
                res.ErrorText = "No value for ObjectId or Alias passed to GetDistributionList.";
                return res;
            }

            //when fetching a list use the query construct in both cases so the XML parsing is identical
            if (string.IsNullOrEmpty(pObjectId))
            {
                //we need to fetch the ObjectId of the list so we can do a "full fetch" to get the 5 extra properties needed.
                res= GetObjectIdForListByAlias(pAlias);
                if (res.Success==false)
                {
                    res.ErrorText = string.Format("Could not find ObjectId from Alias={0} in GetDistributionList.",pAlias);
                    return res;
                }

                //the objectID is returned in the WebCallResult structure.
                pObjectId = res.ReturnedObjectId;
            }

            string strUrl = string.Format("{0}distributionlists/{1}", HomeServer.BaseUrl, pObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

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

            //flip the flag indicating all properties are filled in for the list now- if this had been a "search list" result a few properties
            //would be missing but since we just did a full ObjectId fetch we have them all.
            this.IsFullListData = true;

            //all the updates above will flip pending changes into the queue - clear that here.
            this.ClearPendingChanges();

            return res;
        }



        /// <summary>
        /// Pass in the alias of a distribution list and this routine will return it's ObjectId
        /// </summary>
        /// <param name="pAlias">
        /// Alias uniquely identifying a distribution list.
        /// </param>
        /// <returns></returns>
        private WebCallResult GetObjectIdForListByAlias(string pAlias)
        {
            string strUrl = string.Format("{0}distributionlists?query=(Alias is {1})", HomeServer.BaseUrl, pAlias);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount==0)
            {
                res.Success = false;
                return res;
            }

            List<DistributionList> oLists = HTTPFunctions.GetObjectsFromJson<DistributionList>(res.ResponseText);

            foreach (var oList in oLists)
            {
                if (oList.Alias.Equals(pAlias, StringComparison.InvariantCultureIgnoreCase))
                {
                    res.ReturnedObjectId= oList.ObjectId;
                    return res;
                }
            }

            res.ReturnedObjectId= "";
            res.Success = false;
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
            WebCallResult res;

            //check if the list intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for distribution list: {0}", this);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateDistributionList(HomeServer, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }

        /// <summary>
        /// DELETE a public list from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeleteDistributionList(HomeServer, ObjectId);
        }


        /// <summary>
        /// Uploads a WAV file indicated as a voice name for the target list
        /// </summary>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as a voice name for the list referenced.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// If passed as TRUE the routine will attempt to convert the target WAV file into raw PCM first before uploading it to the Connection
        /// server.  A failure to convert will be considered a failed upload attempt and false is returned.  This value defaults to FALSE meaning
        /// the file will attempt to be uploaded as is.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetVoiceName(string pSourceLocalFilePath, bool pConvertToPcmFirst = false)
        {
            //just call the static method with the information from the instance
            return SetDistributionListVoiceName(HomeServer, pSourceLocalFilePath, ObjectId, pConvertToPcmFirst);
        }

         /// <summary>
        /// If you have a recording stream already recorded and in the stream files table on the Connection server (for instance
        /// you are using the telephone as a media device) you can assign a recording stream file directly to a voice name using this 
        /// method instead of uploading a WAV file from the local hard drive.
        /// </summary>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the recording stream to be assigned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetVoiceNameToStreamFile(string pStreamFileResourceName)
         {
             return SetDistributionListVoiceNameToStreamFile(HomeServer, ObjectId, pStreamFileResourceName);
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
            return GetDistributionListVoiceName(HomeServer, pTargetLocalFilePath, ObjectId, VoiceName);
        }



        /// <summary>
        /// Fetch the list of members for the current instance of the DistributionList.  All members of all types are returned in a generic list.
        /// </summary>
        /// <param name="pMemberList">
        /// Generic list of DistributionListMember objects 
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetMembersList(out List<DistributionListMember> pMemberList)
        {
            return GetMembersList(HomeServer, ObjectId, out pMemberList);
        }



        /// <summary>
        /// add a user as a member of a public distribution list
        /// </summary>
        /// <param name="pUserObjectId"></param>
        /// <returns></returns>
        public WebCallResult AddMemberUser(string pUserObjectId)
        {
            return AddMemberUser(HomeServer, ObjectId, pUserObjectId);
        }


        /// <summary>
        /// Add a public distribution list as a member to another public distribution list.
        /// </summary>
        /// <param name="pListObjectId"></param>
        /// <returns></returns>
        public WebCallResult AddMemberList(string pListObjectId)
        {
            return AddMemberList(HomeServer, ObjectId, pListObjectId);
        }


        /// <summary>
        /// Remove a member from a public distribution list
        /// </summary>
        /// <param name="pMemberUserObjectId"></param>
        /// <returns></returns>
        public WebCallResult RemoveMember(string pMemberUserObjectId)
        {
            return RemoveMember(HomeServer, ObjectId, pMemberUserObjectId);
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
