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
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{

    /// <summary>
    /// Comparison class used for sorting lists of users by field names (first/last/display name, alias and extension)
    /// </summary>
    public class UserComparer : IComparer<UserBase>
    {
        readonly string _memberName = string.Empty; // specifies the member name to be sorted
        readonly SortOrder _sortOrder = SortOrder.None; // Specifies the SortOrder.  Not used here.

        /// <summary>
        /// constructor to set the sort column and sort order.
        /// </summary>
        /// <param name="pMemberName">
        /// Property name to compare by (i.e. Alias)
        /// </param>
        /// <param name="pSortOrder">
        /// Sort order to use - defaults to Ascending
        /// </param>
        public UserComparer(string pMemberName, SortOrder pSortOrder = SortOrder.Ascending)
        {
            _memberName = pMemberName;
            _sortOrder = pSortOrder;
        }

        /// <summary>
        /// User properties you can sort users by
        /// </summary>
        public enum UserSortElements {DtmfAccessId, FirstName, LastName, DisplayName, Alias}
        
        /// <summary>
        /// Compares two users based on member name and sort order and return the result.  Only ascending sorts are supported and 
        /// strings that are null get sorted after those that are non null.
        /// </summary>
        /// <returns>0 if items are equal (two null values are considered equal), -1 if item 1 is less than item 2 and 1 if item 1
        /// is greater than item 2</returns>
        public int Compare(UserBase pUser1, UserBase pUser2)
        {
            int returnValue;
            switch (_memberName.ToLower())
            {
                case "dtmfaccessid":
                    returnValue = pUser1.DtmfAccessId.CompareTo(pUser2.DtmfAccessId);
                    break;
                case "firstname":
                    if (string.IsNullOrEmpty(pUser1.FirstName) & string.IsNullOrEmpty(pUser2.FirstName))
                    {
                        returnValue = 0;
                    }
                    else if (string.IsNullOrEmpty(pUser1.FirstName))
                    {
                        returnValue = 1;
                    }
                    else if (string.IsNullOrEmpty(pUser2.FirstName))
                    {
                        returnValue = -1;
                    }
                    else
                    {
                        returnValue = pUser1.FirstName.CompareTo(pUser2.FirstName);
                    }
                    break;
                case "lastname":
                    if (string.IsNullOrEmpty(pUser1.LastName) & string.IsNullOrEmpty(pUser2.LastName))
                    {
                        returnValue = 0;
                    }
                    else if (string.IsNullOrEmpty(pUser1.LastName))
                    {
                        returnValue = 1;
                    }
                    else if (string.IsNullOrEmpty(pUser2.LastName))
                    {
                        returnValue = -1;
                    }
                    else
                    {
                        returnValue = pUser1.LastName.CompareTo(pUser2.LastName);
                    }
                    break;
                case "displayname":
                    if (string.IsNullOrEmpty(pUser1.DisplayName) & string.IsNullOrEmpty(pUser2.DisplayName))
                    {
                        returnValue = 0;
                    }
                    else if (string.IsNullOrEmpty(pUser1.DisplayName))
                    {
                        returnValue = 1;
                    }
                    else if (string.IsNullOrEmpty(pUser2.DisplayName))
                    {
                        returnValue = -1;
                    }
                    else
                    {
                        returnValue = pUser1.DisplayName.CompareTo(pUser2.DisplayName);
                    }
                    break;
                default:
                    //default to alias if any other field is passed in
                    returnValue =pUser1.Alias.CompareTo(pUser2.Alias);
                    break;
            }
            return returnValue;
        }
    }


    /// <summary>
    /// The UserBase class holds data about a user that gets passed back from CUPI when you do any kind of search (using a query construct for instance).
    /// The amount of data is considerably less than the toal amount which can be returned for a user with the /vmrest/{objectID} style user fetch and is 
    /// limited (for the most part) to what you'd need to present in search/selection lists.  As a rule you should stick to this data unless you have a very
    /// good reason for needing something in the UserFull list (see class below) since you must fetch this information one call at a time (i.e. you cannot
    /// get a list of full user data from CUPI with a query - it must be fetched one at a time).  So no only is more data coming across the wire and having
    /// to be parsed out on the client side, you have to do the full user fetches one at a time.  Lots of overhead involved to keep these to a minimum if 
    /// you can.
    /// Functions for finding, deleting, editing etc... users with mailboxes.  
    /// </summary>
    public class UserBase
    {
        #region Fields and Properties

        //reference to the ConnectionServer object used to create this user instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of whic properties have been updated
        internal ConnectionPropertyList ChangedPropList;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new instance of the UserBase class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this user.  The UserBase class contains much less data than the UserFull class and is, as a result, quicker to fetch and 
        /// load and is used for all list presentations from searches.  
        /// If you pass the pObjectID parameter the user is automatically filled with data for that user from the server.  If no pObjectID is passed an
        /// empty instance of the UserBase class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the user being created.
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the user on the home server provided.  If no ObjectId is passed then an empty instance of the UserBase
        /// class is returned instead.
        /// </param>
        /// <param name="pAlias">
        /// Optional parameter for fetching a user's data based on alias.  If both the ObjectId and the Alias are passed, the ObjectId will be used 
        /// for the search.
        /// </param>
        public UserBase(ConnectionServer pConnectionServer, string pObjectId = "", string pAlias = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null Connection Server passed to the UserBase constructor");
            }

            HomeServer = pConnectionServer;

            //create a repository to keep track of changed properties on an instance of a user
            ChangedPropList = new ConnectionPropertyList();

            if (pObjectId.Length == 0 & pAlias.Length == 0) return;

            //if the ObjectId or Alias are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetUser(pObjectId, pAlias);

            if (res.Success == false)
            {
                throw new Exception(string.Format("User not found in UserBase constructor using Alias={0} and/or ObjectId={1}\n\rError={2}"
                                                , pAlias, pObjectId,res.ErrorText));
            }
        }

        /// <summary>
        /// Empty constructor for using the class manually.
        /// </summary>
        public UserBase()
        {
            //create a repository to keep track of changed properties on an instance of a user
            ChangedPropList = new ConnectionPropertyList();
        }


        #endregion


        #region UserBase Properties

        //The names of the properties must match exactly the tags in XML for them including case - the routine that deserializes data from XML into the 
        //objects requires this to match them up.


        private string _alias;
        public string Alias
        {
            get { return _alias; }
            set
            {
                _alias = value;
                ChangedPropList.Add("Alias", value);
            }
        }

        //you cannot change the primary call handler Id.
        private string _callHandlerObjectId;
        public string CallHandlerObjectId
        {
            get { return _callHandlerObjectId; }
            set
            {
                //only allow it to be changed if it's empty, otherwise this is a no-op
               if (string.IsNullOrEmpty(_callHandlerObjectId))
                    _callHandlerObjectId = value;
            }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set
            {
                ChangedPropList.Add("City", value);
                _city = value;
            }
        }

        private string _cosObjectId;
        public string CosObjectId
        {
            get { return _cosObjectId; }
            set
            {
                _cosObjectId = value;
                ChangedPropList.Add("CosObjectId", value);
            }
        }

        //can't edit creation time.
        public DateTime CreationTime { get; set; }

        private string _department;
        public string Department
        {
            get { return _department; }
            set
            {
                ChangedPropList.Add("Department", value);
                _department = value;
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                ChangedPropList.Add("DisplayName", value);
            }
        }

        private string _dtmfAccessId;
        /// <summary>
        /// Primary extension of the user.
        /// </summary>
        public string DtmfAccessId
        {
            get { return _dtmfAccessId; }
            set
            {
                _dtmfAccessId = value;
                ChangedPropList.Add("DtmfAccessId", value);
            }
        }

        private string _employeeId;
        public string EmployeeId
        {
            get { return _employeeId; }
            set
            {
                ChangedPropList.Add("EmployeeId", value);
                _employeeId = value;
            }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                ChangedPropList.Add("FirstName", value);
            }
        }

        private bool _isVmEnrolled;
        /// <summary>
        /// Set to true if the user has completed their first time enrollment conversation, false if they have not.
        /// Set this to false to turn on first time enrollment for a user.
        /// </summary>
        public bool IsVmEnrolled
        {
            get { return _isVmEnrolled; }
            set
            {
                _isVmEnrolled = value;
                ChangedPropList.Add("IsVmEnrolled", value);
            }
        }

        private int _language;
        /// <summary>
        /// The preferred language of this user. For a user with a voice mailbox, it is the language in which the subscriber hears instructions 
        /// played to them. If the subscriber has TTS enabled by their COS, it is the language used for TTS
        /// </summary>
        public int Language
        {
            get { return _language; }
            set
            {
                _language = value;
                ChangedPropList.Add("Language", value);
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                ChangedPropList.Add("LastName", value);
            }
        }

        private bool _listInDirectory;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection should list the subscriber in the phone directory for outside callers.
        /// This does not affect the ability of other users from finding them when addressing messages.
        /// </summary>
        public bool ListInDirectory
        {
            get { return _listInDirectory; }
            set
            {
                _listInDirectory = value;
                ChangedPropList.Add("ListInDirectory", value);
            }
        }

        //you cannot change the location objectId
        private string _locationObjectId;
        public string LocationObjectId
        {
            get { return _locationObjectId; }
            set
            {
                //only allow it to be updated if it's empty, otherwise this is a no-op
                if (string.IsNullOrEmpty(_locationObjectId))
                    _locationObjectId = value;
            }
        }

        private string _mediaSwitchObjectId;
        /// <summary>
        /// The unique identifier of the MediaSwitch object Cisco Unity Connection uses for subscriber Telephone Record and Playback (TRAP) sessions 
        /// and to dial MWI on or off requests when the Cisco Unity Connection system has a dual switch integration.
        /// </summary>
        public string MediaSwitchObjectId
        {
            get { return _mediaSwitchObjectId; }
            set
            {
                _mediaSwitchObjectId = value;
                ChangedPropList.Add("MediaSwitchObjectId", value);
            }
        }

        /// <summary>
        /// Unique identifier for the user - this cannot be changed for an existing user.
        /// </summary>
        private string _objectId;
        public string ObjectId
        {
            get { return _objectId; }
            set
            {
                //only allow it to be set if it's empty
                if (string.IsNullOrEmpty(_objectId))
                    _objectId = value;
            }
        }

        private string _partitionObjectId;
        /// <summary>
        /// The unique identifier of the Partition to which the DtmfAccessId is assigned
        /// </summary>
        public string PartitionObjectId
        {
            get { return _partitionObjectId; }
            set
            {
                _partitionObjectId = value;
                ChangedPropList.Add("PartitionObjectId", value);
            }
        }

        private string _smtpAddress;
        /// <summary>
        /// SMTP address The full SMTP address for this object
        /// </summary>
        public string SmtpAddress
        {
            get { return _smtpAddress; }
            set
            {
                ChangedPropList.Add("SmtpAddress", value);
                _smtpAddress = value;
            }
        }

        private int _timeZone;
        public int TimeZone
        {
            get { return _timeZone; }
            set
            {
                _timeZone = value;
                ChangedPropList.Add("TimeZone", value);
            }
        }

        private bool _voiceNameRequired;
        public bool VoiceNameRequired
        {
            get { return _voiceNameRequired; }
            set
            {
                _voiceNameRequired = value;
                ChangedPropList.Add("VoiceNameRequired", value);
            }
        }


        private CallHandler _primaryCallHandler;
        /// <summary>
        /// Funtion to fetch the PrimaryCallHandler of a user and return it as a CallHandler object instance.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>
        /// <returns>
        /// Instance of the CallHandler object is passed back.  If there's a problem the instance will be NULL.
        /// </returns>
        public CallHandler PrimaryCallHandler(bool pForceRefetchOfData=false)
        {
            if (pForceRefetchOfData)
            {
                _primaryCallHandler = null;
            }

            //fetch the primary call handler only if it's asked for.
            if (_primaryCallHandler == null)
            {
                GetPrimaryCallHandler(out _primaryCallHandler);
            }

            return _primaryCallHandler;
        }


        private PhoneSystem _phoneSystem;
        /// <summary>
        /// Funtion to fetch the Phone system of a user and return it as a PhoneSystem object instance.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>
        /// <returns>
        /// Instance of the PhoneSystem object is passed back.  If there's a problem the instance will be NULL.
        /// </returns>
        public PhoneSystem PhoneSystem(bool pForceRefetchOfData=false)
        {
            if (pForceRefetchOfData)
            {
                _phoneSystem = null;
            }
            //fetch the primary call handler only if it's asked for.
            if (_phoneSystem == null)
            {
                try
                {
                    _phoneSystem = new PhoneSystem(this.HomeServer, this.MediaSwitchObjectId);
                }
                catch (Exception)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                }
            }

            return _phoneSystem;
        }


        private List<PrivateList> _privateLists;
        /// <summary>
        /// Funtion to fetch all the private lists of a user and return them as a list of PrivateList objects.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>
        /// <returns>
        /// List of PrivateList objects - there may be 0 or up to 99
        /// </returns>
        public List<PrivateList> PrivateLists(bool pForceRefetchOfData = false)
        {
            //if the user wants to force a refetch of data, null out the private cache of notification devices.
            if (pForceRefetchOfData)
            {
                _privateLists = null;
            }
            //fetch notification device list only if it's asked for.
            if (_privateLists == null)
            {
                WebCallResult res= PrivateList.GetPrivateLists(this.HomeServer, ObjectId, out _privateLists);
                if (res.Success == false)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                }
            }

            return _privateLists;
        }


        private List<NotificationDevice> _notificationDevices;
        /// <summary>
        /// Funtion to fetch all the notification devices of a user and return them as a list of NotificationDEvice objects.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>
        /// <returns>
        /// List of NotificationDevice objects - there should be at least 5 for every user, more if they have created additional
        /// devices.
        /// </returns>
        public List<NotificationDevice> NotificationDevices(bool pForceRefetchOfData = false)
        {
            //if the user wants to force a refetch of data, null out the private cache of notification devices.
            if (pForceRefetchOfData)
            {
                _notificationDevices = null;
            }
            //fetch notification device list only if it's asked for.
            if (_notificationDevices == null)
            {
                GetNotificationDevices(out _notificationDevices);
            }

            return _notificationDevices;
        }

        private List<Mwi> _mwis;
        /// <summary>
        /// Funtion to fetch all the MWI devices of a user and return them as a list of Mwi objects.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>
        /// <returns>
        /// List of Mwi objects - there should be at least 1 for every user, more if they have created additional
        /// devices.
        /// </returns>
        public List<Mwi> Mwis(bool pForceRefetchOfData = false)
        {
            //if the user wants to force a refetch of data, null out the private cache of MWIs
            if (pForceRefetchOfData)
            {
                _mwis = null;
            }
            //fetch notification device list only if it's asked for.
            if (_mwis == null)
            {
                GetMwiDevices(out _mwis);
            }

            return _mwis;
        }


        private List<AlternateExtension> _alternateExtensions;
        /// <summary>
        /// Funtion to fetch all the alternate extensions of a user and return them as a list of AlternateExtension objects.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>
        /// <returns>
        /// List of AlternateExtension objects - the primary extension is returned in the list (idIndex of 0) so the list should
        /// never be empty.
        /// </returns>
        public List<AlternateExtension> AlternateExtensions(bool pForceRefetchOfData=false)
        {
            //if the user wants to force a refetch of data, null out the private cache of alternate extensions.
            if (pForceRefetchOfData)
            {
                _alternateExtensions = null;
            }

            //fetch alternate extension list only if it's asked for.
            if (_alternateExtensions == null)
            {
                GetAlternateExtensions(out _alternateExtensions);
            }

            return _alternateExtensions;
        }

        private ClassOfService _cos;
        
        /// <summary>
        /// Funtion to fetch the COS instance this user is associated with.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>    
        /// <returns>
        /// instance of ClassOfService object for the COS this user is associated with.
        /// </returns>
        public ClassOfService Cos(bool pForceRefetchOfData = false)
        {
            if (pForceRefetchOfData)
            {
                _cos = null;
            }

            if (_cos == null)
            {
                _cos = new ClassOfService(this.HomeServer,this.CosObjectId);
            }

            return _cos;
        }



        private Credential _pin;
        
        /// <summary>
        /// Returns details of the PIN (phone password) settings - including if it's locked, time last changed, if it's set to must-change 
        /// etc... this object does NOT allow for editing of credentials - use the ResetUserPassword method off the User object for that.
        /// This is done as a method since if it's done as a property it'll attempt to fill it in from XML data pulled from Connection during
        /// populate of a user's data.
        /// </summary>
        /// <returns>
        /// instance of the credential object
        /// </returns>
        public Credential Pin()
        {
            if (_pin==null)
            {
                _pin= new Credential(this.HomeServer,this.ObjectId,CredentialType.Pin);
            }

            return _pin;
        }

        private Credential _password;

        /// <summary>
        /// Returns details of the Password (GUI password) settings - including if it's locked, time last changed, if it's set to must-change 
        /// etc... this object does NOT allow for editing of credentials - use the ResetUserPassword method off the User object for that.
        /// This is done as a method since if it's done as a property it'll attempt to fill it in from XML data pulled from Connection during
        /// populate of a user's data.
        /// </summary>
        /// <returns>
        /// instance of the credential object
        /// </returns>
        public Credential Password()
        {
            if (_password == null)
            {
                _password = new Credential(this.HomeServer, this.ObjectId, CredentialType.Password);
            }

            return _password;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of users from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(alias startswith ab)"
        /// sort: "sort=(alias asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <remarks>
        /// While this method name does have the plural in it, you'll want to use it for fetching single users as well.  If searching by
        /// ObjectId just construct a query in the form "query=(ObjectId is {ObjectId})".  This is just as fast as using the URI format of 
        /// "{server name}\vmrest\users\{ObjectId}" but returns consistently formatted XML code as multiple users does so the parsing of 
        /// the data to deserialize it into User objects is consistent.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users are being fetched from.
        /// </param>
        /// <param name="pUsers">
        /// The list of users returned from the CUPI call (if any) is returned as a generic list of User class instances via this out param.  
        /// If no users are  found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetUsers(ConnectionServer pConnectionServer, out List<UserBase> pUsers, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pUsers = new List<UserBase>();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetUsers";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "users", pClauses);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - return true here along with an empty list.
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                pUsers = new List<UserBase>();
                return res;
            }

            pUsers = HTTPFunctions.GetObjectsFromJson<UserBase>(res.ResponseText,"User");

            if (pUsers == null)
            {
                pUsers = new List<UserBase>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oUser in pUsers)
            {
                oUser.HomeServer = pConnectionServer;
                oUser.ClearPendingChanges();
            }

            return res;

        }


        /// <summary>
        /// This function allows for a GET of users from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(alias startswith ab)"
        /// sort: "sort=(alias asc)"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <remarks>
        /// While this method name does have the plural in it, you'll want to use it for fetching single users as well.  If searching by
        /// ObjectId just construct a query in the form "query=(ObjectId is {ObjectId})".  This is just as fast as using the URI format of 
        /// "{server name}\vmrest\users\{ObjectId}" but returns consistently formatted XML code as multiple users does so the parsing of 
        /// the data to deserialize it into User objects is consistent.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users are being fetched from.
        /// </param>
        /// <param name="pUsers">
        /// The list of users returned from the CUPI call (if any) is returned as a generic list of User class instances via this out param.  
        /// If no users are  found NULL is returned for this parameter.
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

        public static WebCallResult GetUsers(ConnectionServer pConnectionServer, out List<UserBase> pUsers,int pPageNumber=1, 
            int pRowsPerPage=20,params string[] pClauses)
        {
            //tack on the paging items to the parameters list
            var temp = pClauses.ToList();
            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            return GetUsers(pConnectionServer, out pUsers, temp.ToArray());
        }

        /// <summary>
        /// returns a single UserBase object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the user is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the user to load
        /// </param>
        /// <param name="pUser">
        /// Instance of the UserBase object if the user is found by ObjectId or alias gets passed back on this instance.
        /// </param>
        /// <param name="pAlias">
        /// Optional parameter - since alias is unique for users you may pass in an empty ObjectId and use an alias instead.  If both the alias and
        /// ObjectId are passed, the ObjectId will be used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetUser(out UserBase pUser, ConnectionServer pConnectionServer, string pObjectId, string pAlias = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pUser = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetUser";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId)&& string.IsNullOrEmpty(pAlias))
            {
                res.ErrorText = "Emtpy ObjectId and Alias passed to GetUser";
                return res;
            }
            
            //create a new UserBase instance passing the ObjectId which fills out the data automatically
            try
            {
                pUser = new UserBase(pConnectionServer, pObjectId, pAlias);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch user in GetUser:" + ex.Message;
                return res;
            }

            //clear the pending changes list before returning

            pUser.ClearPendingChanges();

            return res;
        }

        /// <summary>
        /// returns a single UserFull object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the user is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the user to load
        /// </param>
        /// <param name="pUser">
        /// Instance of the UserFull object if the user is found by ObjectId or alias gets passed back on this instance.
        /// </param>
        /// <param name="pAlias">
        /// Optional parameter - since alias is unique for users you may pass in an empty ObjectId and use an alias instead.  If both the alias and
        /// ObjectId are passed, the ObjectId will be used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetUser(out UserFull pUser, ConnectionServer pConnectionServer, string pObjectId, string pAlias = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pUser = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetUser";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId) && string.IsNullOrEmpty(pAlias))
            {
                res.ErrorText = "Emtpy ObjectId and Alias passed to GetUser";
                return res;
            }

            //create a new Userfull instance passing the ObjectId which fills out the data automatically
            try
            {
                pUser = new UserFull(pConnectionServer, pObjectId,pAlias);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch user in GetUser:" + ex.Message;
                return res;
            }

            //clear the pending changes list before returning

            pUser.ClearPendingChanges();

            return res;
        }

        /// <summary>
        /// Allows for the creation of a new user with a mailbox on the Connection server directory.  Both the alias and extension number must be 
        /// provided along with a template alias to use when creating the new user, however other user properties and their values may be passed 
        /// in via the ConnectonPropertyList structure.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users is being added.
        /// </param>
        /// <param name="pTemplateAlias">
        /// The alias of a user template on Connection - this provides importat details such as the Class of Service and dial partition assignment.  It's
        /// required and must exist on the server or the user creation will fail.
        /// </param>
        /// <param name="pAlias">
        /// Alias to be used for the new user with a mailbox.  This must be unique against all users in the directory or the add will fail.
        /// </param>
        /// <param name="pExtension">
        /// The primary extension number to be assigned to the new user.  This must be unqiue in the partition the user is created in or the 
        /// new user creation will fail.  The partition is determined by the user template used to created new users.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a user property name and a new value for that property to apply to the user being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddUser(ConnectionServer pConnectionServer,
                                            string pTemplateAlias,
                                            string pAlias,
                                            string pExtension,
                                            ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddUser";
                return res;
            }

            //make sure that something is passed in for the 3 required params 
            if (String.IsNullOrEmpty(pTemplateAlias) || string.IsNullOrEmpty(pAlias) || string.IsNullOrEmpty(pExtension))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddUser on ConnectionServer.cs";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            //cheat here a bit and simply add the alias and extension values to the proplist where it can be tossed into the body later.
            pPropList.Add("Alias", pAlias);
            pPropList.Add("DtmfAccessId", pExtension);

            string strBody = "<User>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</User>";

            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "users?templateAlias=" + pTemplateAlias, MethodType.POST,pConnectionServer,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/users/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/users/", "").Trim();
                }
            }

            return res;
        }



        /// <summary>
        /// Allows you to add a new MWI extension for a user -this implementation only allows you to set the extension and display name which is the 
        /// typical case - you can also add support for assigning to specific media switch instances on Connection 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users is being added.
        /// </param>
        /// <param name="pUserObjectId">
        /// Unique GUID of the user you want to add the MWI for
        /// </param>
        /// <param name="pExtension">
        /// Extension for the new MWI definition to dial.  
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to assign to the new MWI device - must be unique for all MWIs for that user.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddMwi(ConnectionServer pConnectionServer,
                                                      string pUserObjectId,
                                                      string pExtension,
                                                      string pDisplayName)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddMWI";
                return res;
            }

            //make sure that something is passed in for the 3 required params - all are necessary
            if (String.IsNullOrEmpty(pExtension) | (string.IsNullOrEmpty(pUserObjectId)) | (string.IsNullOrEmpty(pUserObjectId)))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddMWI";
                return res;
            }

           string strBody = "<Mwi>";

            //tack on the property value pair with appropriate tags
           strBody += string.Format("<MwiExtension>{0}</MwiExtension>", pExtension);
           strBody += string.Format("<DisplayName>{0}</DisplayName>", pDisplayName);

           strBody += "</Mwi>";

            res =
                HTTPFunctions.GetCupiResponse(string.Format("{0}users/{1}/mwis", pConnectionServer.BaseUrl, pUserObjectId),
                    MethodType.POST,pConnectionServer,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = @"/vmrest/users/" + pUserObjectId + "/mwis/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new user with a mailbox on the Connection server directory.  Both the alias and extension number must be 
        /// provided along with a template alias to use when creating the new user, however other user properties and their values may be passed 
        /// in via the ConnectonPropertyList structure.
        /// </summary>
        /// <remarks>
        /// This is an alternateive AddUser that passes back a UserFull object with the newly created user filled out in it if the add goes through.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users is being added.
        /// </param>
        /// <param name="pTemplateAlias">
        /// The alias of a user template on Connection - this provides importat details such as the Class of Service and dial partition assignment.  It's
        /// required and must exist on the server or the user creation will fail.
        /// </param>
        /// <param name="pAlias">
        /// Alias to be used for the new user with a mailbox.  This must be unique against all users in the directory or the add will fail.
        /// </param>
        /// <param name="pExtension">
        /// The primary extension number to be assigned to the new user.  This must be unqiue in the partition the user is created in or the 
        /// new user creation will fail.  The partition is determined by the user template used to created new users.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a user property name and a new value for that property to apply to the user being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null.
        /// </param>
        /// <param name="pUserFull">
        /// Out paramter that passes back a UserFull object with the details of the newly added user.  If the new user add fails, NULL is returned 
        /// for this value.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddUser(ConnectionServer pConnectionServer,
                                            string pTemplateAlias,
                                            string pAlias,
                                            string pExtension,
                                            ConnectionPropertyList pPropList,
                                            out UserFull pUserFull)
        {
            pUserFull = null;

            WebCallResult res = AddUser(pConnectionServer, pTemplateAlias, pAlias, pExtension, pPropList);

            //if the add goes through, fill a UserFull object out and pass it back.
            if (res.Success)
            {
                res = UserFull.GetUser(pConnectionServer, res.ReturnedObjectId, out pUserFull);

                //stuff the objectID back in the res structure since we took overwrote it with the GetUser fetch.
                res.ReturnedObjectId = pUserFull.ObjectId;
            }

            return res;
        }

        /// <summary>
        /// Resets the PIN for a user - be aware that this routine does not do any validation of the PIN format or complixty/length.  If the PIN
        /// is not valid the error will be returned via the WebCallResult class that will contain information and a failure code from the server.
        /// You can set/clear the locked, must-change, cant-change and doesnt-expire options for the credential as well - by default all values are 
        /// left alone.  There is no checking of these values for consistency (i.e. if you pass cant change and must change).  Handling that is 
        /// left to the server.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pObjectId">
        /// Unique GUID of the user to reset the PIN for.
        /// </param>
        /// <param name="pNewPin">
        /// New PIN (phone password) to apply to the user's account.  If passed as blank this value is skipped.  You can, for instance, pass a blank
        /// password if you wish to change the "mustchange" flag on a credential but not reset the password itself.  If you pass blank here and you 
        /// pass no other values then CUPI will return an error.
        /// </param>
        /// <param name="pLocked">
        /// Nullable value for locking/unlocking the PIN.  by default this value passes NULL and no change is made to the property.  Passing True
        /// or False will lock and unlock the value respectively.
        /// </param>
        /// <param name="pMustChange">
        /// Nullable value for setting the "must change" value for the credential so the user will have to change it the next time they log in.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pCantChange">
        /// Nullable value for setting the "Cant Change" value for the credential so the user will not be able to change their credential.  
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pDoesntExpire">
        /// Nullable value for setting the "Doesnt expire" value for the credential so the user will never be asked to change the credentail.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pClearHackedLockout">
        /// Nullable value for clearing a hacked lockout condition (i.e. user provided wrong password too many times in a row and is now locked out for 
        /// a period of time).  This is seperate from an admin lockout condition which is cleared/set using the pLocked parameter.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult ResetUserPin(ConnectionServer pConnectionServer, 
                                                string pObjectId, 
                                                string pNewPin, 
                                                bool? pLocked = null,
                                                bool? pMustChange=null, 
                                                bool? pCantChange=null, 
                                                bool? pDoesntExpire=null,
                                                bool? pClearHackedLockout=null)
        {
            return ResetUserCredential(pConnectionServer, 
                                        pObjectId, 
                                        pNewPin, 
                                        CredentialType.Pin, 
                                        pLocked, 
                                        pMustChange,
                                        pCantChange, 
                                        pDoesntExpire,
                                        pClearHackedLockout);
        }


        //back end helper function to reset PIN or Password
        private static WebCallResult ResetUserCredential(ConnectionServer pConnectionServer,
                                        string pObjectId,
                                        string pNewPCredential,
                                        CredentialType pCredentialType,
                                        bool? pLocked = null,
                                        bool? pMustChange = null,
                                        bool? pCantChange = null,
                                        bool? pDoesntExpire = null,
                                        bool? pClearHackedLockout = null)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to ResetUserCredential";
                return res;
            }

            ConnectionPropertyList oProps = new ConnectionPropertyList();

            //only add the credential if it's not empty - This interface does NOT allow for blank credentials which are a 
            //horrible idea.
            if (!String.IsNullOrEmpty(pNewPCredential)) oProps.Add("Credentials", pNewPCredential);

            //add in nullable optional flags that can be psssed in as needed.
            if (pLocked != null) oProps.Add("Locked", pLocked.Value);
            if (pMustChange != null) oProps.Add("CredMustChange", pMustChange.Value);
            if (pCantChange != null) oProps.Add("CantChange", pCantChange.Value);
            if (pDoesntExpire != null) oProps.Add("DoesntExpire", pDoesntExpire.Value);
            if (pClearHackedLockout != null && pClearHackedLockout==true)
            {
              	//when clearing the hacked lockout you need to clear both the hacked count and the boolean Hacked flag to unlock
                //the account - this method is only for clearing the hack lockout, not setting it.
                oProps.Add("Hacked",false);
                //oProps.Add("HackCount",0);
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            string strBody = "<Credential>";

            foreach (ConnectionObjectPropertyPair oPair in oProps)
            {
                strBody += string.Format("<{0}>{1}</{2}>", oPair.PropertyName,oPair.PropertyValue,oPair.PropertyName);
            }


            strBody += "</Credential>";

            //the only difference between setting a PIN vs. Password is the URL path used.  The body/property names are identical
            //otherwise.
            string strUrl;
            if (pCredentialType==CredentialType.Pin)
            {
                strUrl = pConnectionServer.BaseUrl + "users/" + pObjectId + "/credential/pin";
            }
            else
            {
                strUrl=pConnectionServer.BaseUrl + "users/" + pObjectId + "/credential/password";
            }

            return HTTPFunctions.GetCupiResponse(strUrl,MethodType.PUT,pConnectionServer,strBody,false);
        }



        /// <summary>
        /// Resets the password for a user (GUI) - be aware that this routine does not do any validation of the password format or complixty/length.  
        /// If the password is not valid the error will be returned via the WebCallResult class that will contain information and a failure code 
        /// from the server.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pObjectId">
        /// Unique GUID of the user to reset the password for.
        /// </param>
        /// <param name="pNewPassword">
        /// New password (GUI password) to apply to the user's account.  If passed as blank this value is skipped.  You can, for instance, pass a blank
        /// password if you wish to change the "mustchange" flag on a credential but not reset the password itself.  If you pass blank here and you 
        /// pass no other values then CUPI will return an error.
        /// </param>
        /// <param name="pLocked">
        /// Nullable value for locking/unlocking the PIN.  by default this value passes NULL and no change is made to the property.  Passing True
        /// or False will lock and unlock the value respectively.
        /// </param>
        /// <param name="pMustChange">
        /// Nullable value for setting the "must change" value for the credential so the user will have to change it the next time they log in.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pCantChange">
        /// Nullable value for setting the "Cant Change" value for the credential so the user will not be able to change their credential.  
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pDoesntExpire">
        /// Nullable value for setting the "Doesnt expire" value for the credential so the user will never be asked to change the credentail.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult ResetUserPassword(ConnectionServer pConnectionServer, 
                                                    string pObjectId, 
                                                    string pNewPassword,
                                                    bool? pLocked = null,
                                                    bool? pMustChange=null, 
                                                    bool? pCantChange=null, 
                                                    bool? pDoesntExpire=null)
        {
            return ResetUserCredential(pConnectionServer,
                                           pObjectId,
                                           pNewPassword,
                                           CredentialType.Password,
                                           pLocked,
                                           pMustChange,
                                           pCantChange,
                                           pDoesntExpire);
        }



        /// <summary>
        /// DELETE a user from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the user in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteUser(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteUser";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "users/" + pObjectId,MethodType.DELETE,pConnectionServer, "");
        }




        /// <summary>
        /// Allows one or more properties on a user to be udpated (for instance FirstName, LastName etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the user to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a user property name and a new value for that property to apply to the user being updated.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one property
        /// pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateUser(ConnectionServer pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateUser";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateUser";
                return res;
            }

            string strBody = "<User>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</User>";

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "users/" + pObjectId,MethodType.PUT,pConnectionServer,strBody,false);
        }


        /// <summary>
        /// Fetches the WAV file for a user's voice name and stores it on the Windows file system at the file location specified.  If the user does 
        /// not have a voice name recorded, the WebcallResult structure returns false in the success proeprty and notes the user has no voice name in 
        /// the error text.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the user's voice name at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the subscriber.  This value is not used if the optional pConnectionWavFileName is passed.  If the wav file name is not provided
        /// the full user data is fetched using this ObjectId value and the voice name wav file name is fetched from there.
        /// </param>
        /// <param name="pConnectionWavFileName">
        /// Optional parameter, if this value is passed no user lookup is done, instead the call to the Connection server to download the stream file name
        /// is called directly.  This can save cycles if you already have the voice name wav file name in hand.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetUserVoiceName(ConnectionServer pConnectionServer, string pTargetLocalFilePath, string pObjectId, string pConnectionWavFileName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetUserVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pTargetLocalFilePath) || (Directory.GetParent(pTargetLocalFilePath).Exists == false))
            {
                res.ErrorText = "Invalid local file path passed to GetUserVoiceName: " + pTargetLocalFilePath;
                return res;
            }

            //if the WAV file name itself is passed in that's all we need, otherwise we need to go do a UserFull fetch with the ObjectId 
            //and pull the VoiceName wav file name from there (if it's present).
            if (String.IsNullOrEmpty(pConnectionWavFileName))
            {
                //fetch the full user info which has the VoiceName property on it
                UserFull oUserFull;

                try
                {
                    oUserFull = new UserFull(pConnectionServer, pObjectId);
                }
                catch (Exception ex)
                {
                    res.ErrorText = string.Format("Error constructing new full user with objectID{0}\n{1}", pObjectId, ex.Message);
                    return res;
                }

                //the property will be null if no voice name is recorded for the user.
                if (string.IsNullOrEmpty(oUserFull.VoiceName))
                {
                    res = new WebCallResult();
                    res.Success = false;
                    res.ErrorText = "No voice named recorded for user.";
                    return res;
                }

                pConnectionWavFileName = oUserFull.VoiceName;
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
        /// Uploads a WAV file indicated as a voice name for the target subscriber referenced by the pObjectID value.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as a voice name for the user referenced.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the user to upload the voice name WAV file for.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// If passed as TRUE the routine will attempt to convert the target WAV file into raw PCM first before uploading it to the Connection
        /// server.  A failure to convert will be considered a failed upload attempt and false is returned.  This value defaults to FALSE meaning
        /// the file will attempt to be uploaded as is.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetUserVoiceName(ConnectionServer pConnectionServer, string pSourceLocalFilePath, string pObjectId, bool pConvertToPcmFirst = false)
        {
            string strConvertedWavFilePath = "";
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetUserVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pSourceLocalFilePath) || (File.Exists(pSourceLocalFilePath) == false))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid local file path passed to SetUserVoiceName: " + pSourceLocalFilePath;
                return res;
            }

            //if the user wants to try and rip the WAV file into PCM 16/8/1 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pSourceLocalFilePath);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into PCM format in SetUserVoiceName.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted PCM WAV file path not found in SetUserVoiceName: " + strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pSourceLocalFilePath = strConvertedWavFilePath;

            }

            //use the 8.5 and later voice name formatting here which simplifies things a great deal.
            string strResourcePath = string.Format(@"{0}users/{1}/voicename", pConnectionServer.BaseUrl, pObjectId);

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
                    res.ErrorText = "(warning) failed to delete temporary PCM wav file in SetUserVoiceName:" + ex.Message;
                }
            }
            return res;
        }


        /// <summary>
        /// If you have a recording stream already recorded and in the stream files table on the Connection server (for instance
        /// you are using the telephone as a media device) you can assign a greeting stream file directly to a voice name using this 
        /// method instead of uploading a WAV file from the local hard drive.
        /// </summary>
        /// <param name="pConnectionServer" type="ConnectionServer">
        ///   The Connection server that houses the voice name to be updated      
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the user to apply the stream file to the voice name for.
        /// </param>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the recording stream to be assigned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetUserVoiceNameToStreamFile(ConnectionServer pConnectionServer, string pObjectId,
                                                     string pStreamFileResourceName)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetUserVoiceNameToStreamFile";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to SetUserVoiceNameToStreamFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pStreamFileResourceName))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid stream file resource id passed to SetUserVoiceNameToStreamFile";
                return res;
            }

            //construct the full URL to call for uploading the voice name file
            string strUrl = string.Format(@"{0}users/{1}/voicename", pConnectionServer.BaseUrl,pObjectId);

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

        //Fills the current instance of UserBase in with properties fetched from the server.  The fetch uses a query construction instead of the full ObjectId
        //construction which returns less data and is quicker.

        /// <summary>
        /// Diplays the alias, display name and extension of the user by default.
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} [{1}] x{2}", this.Alias, this.DisplayName, this.DtmfAccessId);
        }


        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchUserData()
        {
            return GetUser(this.ObjectId);
        }

        /// <summary>
        /// Helper function to fill in the user instance with data from a user by their objectID string or their alias string.
        /// </summary>
        /// <param name="pObjectId"></param>
        /// <param name="pAlias"></param>
        /// <returns></returns>
        private WebCallResult GetUser(string pObjectId, string pAlias = "")
        {
            string strUrl;

            //when fetching a base user use the query construct (which returns less data and is quicker) than the users/(objectid) format for 
            //UserFull object.
            if (!string.IsNullOrEmpty(pObjectId))
            {
                strUrl = string.Format("{0}users?query=(ObjectId is {1})", HomeServer.BaseUrl, pObjectId);
            }
            else 
            {
                strUrl = string.Format("{0}users?query=(Alias is {1})", HomeServer.BaseUrl, pAlias);
            }

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            if (res.TotalObjectCount == 0)
            {
                res.Success = false;
                res.ErrorText = string.Format("No user found with alias={0} or objectI=d={1}", pAlias, pObjectId);
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(HTTPFunctions.StripJsonOfObjectWrapper(res.ResponseText,"User"), this);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }

            //the above fetch will set the proeprties as "changed", need to clear them out here
            ChangedPropList.Clear();

            return res;
        }

        



        /// <summary>
        /// Dumps out all the properties associated with the instance of the user object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the user object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix="")
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
        /// Resets the PIN for a user - be aware that this routine does not do any validation of the PIN format or complixty/length.  If the PIN
        /// is not valid the error will be returned via the WebCallResult class that will contain information and a failure code from the server.
        /// </summary>
        /// <param name="pNewPin">
        /// New PIN (phone password) to apply to the user's account. If passed as blank this value is skipped.  You can, for instance, pass a blank
        /// password if you wish to change the "mustchange" flag on a credential but not reset the password itself.  If you pass blank here and you 
        /// pass no other values then CUPI will return an error.
        /// </param>
        /// <param name="pLocked">
        /// Nullable value for locking/unlocking the PIN.  by default this value passes NULL and no change is made to the property.  Passing True
        /// or False will lock and unlock the value respectively.
        /// </param>
        /// <param name="pMustChange">
        /// Nullable value for setting the "must change" value for the credential so the user will have to change it the next time they log in.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pCantChange">
        /// Nullable value for setting the "Cant Change" value for the credential so the user will not be able to change their credential.  
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pDoesntExpire">
        /// Nullable value for setting the "Doesnt expire" value for the credential so the user will never be asked to change the credentail.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        ///<param name="pClearHackedCount">
        /// Nullable value for clearing the hacked count for the user's PIN.
        ///  </param>
        ///Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        ///<returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult ResetPin(string pNewPin, 
                                      bool? pLocked = null,
                                      bool? pMustChange=null, 
                                      bool? pCantChange=null, 
                                      bool? pDoesntExpire=null,
                                      bool? pClearHackedCount=null
            )
        {
            return ResetUserPin(HomeServer, ObjectId, pNewPin,pLocked,pMustChange,pCantChange,pDoesntExpire,pClearHackedCount);
        }


        /// <summary>
        /// Resets the password for a user - be aware that this routine does not do any validation of the password format or complixty/length.  
        /// If the password is not valid the error will be returned via the WebCallResult class that will contain information and a failure 
        /// code from the server.
        /// </summary>
        /// <param name="pNewPassword">
        /// New password (GUI password) to apply to the user's account. If passed as blank this value is skipped.  You can, for instance, pass a blank
        /// password if you wish to change the "mustchange" flag on a credential but not reset the password itself.  If you pass blank here and you 
        /// pass no other values then CUPI will return an error.
        /// </param>
        /// <param name="pLocked">
        /// Nullable value for locking/unlocking the PIN.  by default this value passes NULL and no change is made to the property.  Passing True
        /// or False will lock and unlock the value respectively.
        /// </param>
        /// <param name="pMustChange">
        /// Nullable value for setting the "must change" value for the credential so the user will have to change it the next time they log in.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pCantChange">
        /// Nullable value for setting the "Cant Change" value for the credential so the user will not be able to change their credential.  
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pDoesntExpire">
        /// Nullable value for setting the "Doesnt expire" value for the credential so the user will never be asked to change the credentail.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult ResetPassword(string pNewPassword,
                                        bool? pLocked = null,
                                        bool? pMustChange=null, 
                                        bool? pCantChange=null, 
                                        bool? pDoesntExpire=null)
        {
            return ResetUserPassword(HomeServer, ObjectId, pNewPassword,pLocked,pMustChange,pCantChange,pDoesntExpire);
        }


        /// <summary>
        /// Clear the queue of changed properties for this user instance.
        /// </summary>
        public void ClearPendingChanges()
        {
            ChangedPropList.Clear();
        }

        /// <summary>
        /// Allows one or more properties on a user to be udpated (for instance FirstName, LastName etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the handler intance has any pending changes, if not return false with an appropriate error message
            if (!ChangedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for user {0} [{1}]", Alias, ObjectId);
                return res;
            }
            //just call the static method with the info from the instance 
            res = UpdateUser(this.HomeServer, this.ObjectId, ChangedPropList);

            //if the update goes through clear the queue of changed items
            if (res.Success)
            {
                this.ClearPendingChanges();
            }

            return res;

        }

        /// <summary>
        /// DELETE a user from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeleteUser(this.HomeServer, this.ObjectId);
        }


        /// <summary>
        /// Uploads a WAV file indicated as a voice name for the target subscriber
        /// </summary>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as a voice name for the user referenced.
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
            return SetUserVoiceName(this.HomeServer, pSourceLocalFilePath, this.ObjectId, pConvertToPcmFirst);
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
            return SetUserVoiceNameToStreamFile(this.HomeServer, ObjectId, pStreamFileResourceName);
        }

        /// <summary>
        /// Fetches the WAV file for a user's voice name and stores it on the Windows file system at the file location specified.  If the user does 
        /// not have a voice name recorded, the WebcallResult structure returns false in the success proeprty and notes the user has no voice name in 
        /// the error text.
        /// </summary>
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the user's voice name at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <param name="pConnectionWavFileName">
        /// Optional parameter, if this value is passed no user lookup is done, instead the call to the Connection server to download the stream file name
        /// is called directly.  This can save cycles if you already have the voice name wav file name in hand.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetVoiceName(string pTargetLocalFilePath, string pConnectionWavFileName = "")
        {
            //just call the static method with the info from the instance of this object
            return GetUserVoiceName(this.HomeServer, pTargetLocalFilePath, this.ObjectId, pConnectionWavFileName);
        }


        //helper function used when a call is made to get a list of notification devices from the property list.
        private WebCallResult GetNotificationDevices(out List<NotificationDevice> pNotificationDevices)
        {

            return NotificationDevice.GetNotificationDevices(HomeServer, ObjectId, out pNotificationDevices);

        }

        //helper function used when a call is made to get a list of MWI devices from the property list
        private void GetMwiDevices(out List<Mwi> pMwiDevices)
        {
            Mwi.GetMwiDevices(HomeServer, ObjectId, out pMwiDevices);
        }

        /// <summary>
        /// Pass in the notificaiton device display name and this will return an instance of the NotificationDevice class for it (if found).
        /// </summary>
        /// <remarks>
        /// This routine will fetch the full list of notification devices if they have not yet been fetched for this user and return the 
        /// one of interest. If the devices have already been fetched it simply returns the appropriate instance.
        /// </remarks>
        /// <param name="pDeviceName">
        /// The display name of the notification device to fetch.  For the system defined devices the names are:
        /// Work Phone, Home Phone, Mobil Phone, Pager, SMTP.
        /// For devices you've added it's the name you give them.  The search is not case sensitive.
        /// </param>
        /// <param name="pNotificationDevice">
        /// Out param on which the notificaiton device is passed back.  If there is an error finding the device then null is returned.
        /// </param>
        /// <param name="pForceRefetchOfData">
        /// If passed as true this forces a reload of notification devices - this should be passed if devices are being added/removed after 
        /// the user has been fetched.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetNotificationDevice(string pDeviceName, out NotificationDevice pNotificationDevice, bool pForceRefetchOfData=false)
        {
            WebCallResult res;

            pNotificationDevice = null;

            //if a refetch is asked for then null out the private list that may be holding a cached set of devices.
            if (pForceRefetchOfData)
            {
                _notificationDevices = null;
            }
            //fetch the full notification device list if it hasn't been fetched yet.
            if (_notificationDevices == null)
            {
                res = GetNotificationDevices(out _notificationDevices);

                //if there's some sort of error getting the list, pass it back and bail.
                if (res.Success == false)
                {
                    return res;
                }
            }

            //get the correct device off the list
            res = new WebCallResult();

            foreach (NotificationDevice oDevice in _notificationDevices)
            {
                //case insenstive search on display name
                if (oDevice.DisplayName.Equals(pDeviceName,StringComparison.InvariantCultureIgnoreCase))
                {
                    pNotificationDevice = oDevice;
                    res.Success = true;
                    return res;
                }
            }

            //if we're here then there was a probllem
            res.Success = false;
            res.ErrorText = "Could not find notification device by name=" + pDeviceName;
            return res;
        }


        //helper function used when a call is made to get a list of alternate extensions from the property list.
        private WebCallResult GetAlternateExtensions(out List<AlternateExtension> pAlternateExtensions)
        {
            return AlternateExtension.GetAlternateExtensions(HomeServer, ObjectId, out pAlternateExtensions);
        }

        /// <summary>
        /// Pass in the alternate extension ID (1-10 admin added, 11-20 user added) and this will return an instance of the 
        /// AlternateExtension class for it (if found).
        /// </summary>
        /// <remarks>
        /// This routine will fetch the full list of alternate extension if they have not yet been fetched for this user and return the 
        /// one of interest. If the extensions have already been fetched it simply returns the appropriate instance.
        /// </remarks>
        /// <param name="pAltExtensionId">
        /// The ID of the alternate extension to fetch - 0 is primary, 1-10 is administrator added extensions and 11 - 20 are user added 
        /// extensions.
        /// </param>
        /// <param name="pAlternateExtension">
        /// Out param on which the alternate extension is passed back.  If there is an error finding the extension then null is returned.
        /// </param>
        /// <param name="pForceRefetchOfData">
        /// Pass in true for this parameter to force the alternate extensions to be refetched for this user.  You'll want to do this after 
        /// adding or removing alternate extensions for instance.  This defaults to FALSE.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetAlternateExtension(int pAltExtensionId, out AlternateExtension pAlternateExtension, bool pForceRefetchOfData = false)
        {
            WebCallResult res;

            pAlternateExtension = null;

            //of the user wants to force a refetch of alternate extension data (i.e. they just added or removed an alternate extension) 
            //then null out the private cache so it will rebuild it.
            if (pForceRefetchOfData)
            {
                _alternateExtensions = null;
            }

            //fetch the full alternate extension list if it hasn't been fetched yet.
            if (_alternateExtensions == null)
            {
                res = GetAlternateExtensions(out _alternateExtensions);

                //if there's some sort of error getting the list, pass it back and bail.
                if (res.Success == false)
                {
                    return res;
                }
            }

            //get the correct device off the list
            res = new WebCallResult();

            foreach (AlternateExtension oExt in _alternateExtensions)
            {
                //case insenstive search on display name
                if (oExt.IdIndex== pAltExtensionId)
                {
                    pAlternateExtension = oExt;
                    res.Success = true;
                    return res;
                }
            }

            //if we're here then there was a probllem
            res.Success = false;
            res.ErrorText = "Could not find alternate extension ID=" + pAltExtensionId.ToString();
            return res;
        }


        //helper function used when a call is made to get the primary call handler for a user.
        private WebCallResult GetPrimaryCallHandler(out CallHandler pPrimaryCallHandler)
        {
            WebCallResult res = new WebCallResult();

            try
            {
                pPrimaryCallHandler = new CallHandler(HomeServer, this.CallHandlerObjectId);
                res.Success = true;
            }
            catch
            {
                pPrimaryCallHandler = null;
                res.Success = false;
                res.ErrorText = "Error fetching primary call handler using ObjectId=" + this.CallHandlerObjectId;
            }

            return res;
        }


        #endregion

    }


    /// <summary>
    /// Class containing all top level user properties to create a stongly typed class based on the XML data returned for users in the 
    /// Connection directory using the CUPI web interface.
    /// This inherits from the UserBase which has the core data for a user - the only way to fill out the additional proeprties found in the
    /// UserFull class is to do a user fetch with the "{server name}/vmrest/{objectid}" construct one user at a time.  Clealry much more data 
    /// is passed back than for query searches which return less data and you have to do the searches one at a time.  You should leverage those
    /// functions and this class sparingly or your application will be considerably more slow and use more bandwidth than is necessary.
    /// </summary>
    public class UserFull : UserBase
    {

        #region Constructors

        /// <summary>
        /// Create an empty instance of the class - for testing purposes
        /// </summary>
        public UserFull(ConnectionServer pConnectionServer):base(pConnectionServer)
        {
            //only for testing
        }

        /// <summary>
        /// Creates a new instance of the UserFull class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this user.  The UserFull class contains much more data than the UserBase class and is, as a result, slower to fetch and 
        /// load.  Use it only if you have need of the additional properties on the UserFull definition. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the user being created.
        /// </param>
        /// <param name="pObjectId">
        /// Unique ID of the user on the home server provided.  
        /// </param>
        /// <param name="pAlias">
        /// Optional alias string which can be used instead of the ObjectId for fetching user data
        /// </param>
        public UserFull(ConnectionServer pConnectionServer, string pObjectId,string pAlias="")
            : base(pConnectionServer)
        {
            string strObjectId = pObjectId;
            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = this.GetObjectIdFromAlias(pAlias);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    throw new Exception("User not found in UserFull constructor using alias=" + pAlias);
                }
            }

            //construct a new UserFull object in the UserBase constructor and then call the GetUSerFull instance method to fetch and 
            //fill in all the full user properties from the server.
            WebCallResult res = GetUserFull(strObjectId);

            if (res.Success == false)
            {
                throw new Exception("User not found in UserFull constructor using ObjectId=" + pObjectId);
            }
        }

        #endregion


        #region UserFull Properties

        //The names of the properties must match exactly the tags in XML for them including case - the routine that deserializes data from XML into the 
        //objects requires this to match them up.

        private string _address;
        /// <summary>
        /// The physical address such as a house number and street name where the user is located, or with which a user is associated
        /// </summary>
        public string Address
        {
            get { return _address; }
            set
            {
                ChangedPropList.Add("Address", value);
                _address = value;
            }
        }

        private bool _addressAfterRecord;
        /// <summary>
        /// A flag indicating whether the subscriber will be prompted to address message before or after it is recorded
        /// </summary>
        public bool AddressAfterRecord
        {
            get { return _addressAfterRecord; }
            set
            {
                ChangedPropList.Add("AddressAfterRecord", value);
                _addressAfterRecord = value;
            }
        }

        private int _addressMode;
        /// <summary>
        /// The default method the subscriber will use to address messages to other subscribers.
        /// 1=Extension, 2=FirstNameFirst, 0=LastNameFirst
        /// </summary>
        public int AddressMode
        {
            get { return _addressMode; }
            set
            {
                ChangedPropList.Add("AddressMode", value);
                _addressMode = value;
            }
        }

        //while the schema lists these as included, they don't get returned from any version I've tested.
        //public string AltFirstFame { get; set; }
        //public string AltLastName { get; set; }
        
        private int _announceUpcomingMeetings;
        public int AnnounceUpcomingMeetings
        {
            get { return _announceUpcomingMeetings; }
            set
            {
                ChangedPropList.Add("AnnounceUpcomingMeetings", value);
                _announceUpcomingMeetings = value;
            }
        }

        private int _assistantRowsPerPage;
        /// <summary>
        /// This controls the number of entries to display per page for all tables in the Unity Assistant, e.g. the Private List Members table
        /// </summary>
        public int AssistantRowsPerPage
        {
            get { return _assistantRowsPerPage; }
            set
            {
                ChangedPropList.Add("AssistantRowsPerPage", value);
                _assistantRowsPerPage = value;
            }
        }

        private string _billingId;
        public string BillingId
        {
            get { return _billingId; }
            set
            {
                ChangedPropList.Add("BillingId", value);
                _billingId = value;
            }
        }

        private string _building;
        public string Building
        {
            get { return _building; }
            set
            {
                ChangedPropList.Add("Building", value);
                _building = value;
            }
        }

        private int _callAnswerTimeout;
        /// <summary>
        /// The number of rings to wait for a subscriber destination to answer before the call is forwarded to the subscriber's primary phone
        /// </summary>
        public int CallAnswerTimeout
        {
            get { return _callAnswerTimeout; }
            set
            {
                ChangedPropList.Add("CallAnswerTimeout", value);
                _callAnswerTimeout = value;
            }
        }

        /// <summary>
        /// Id of associated EndUser, ApplicationUser, or DirectoryNumber in Call Manager.
        /// cannot change CcmId or type
        /// </summary>
        [JsonProperty]
        public string CcmId { get; private set; }
        
        /// <summary>
        /// Type of CCM user/object this id refers to (end user, application user, or directory number)
        /// 0=EndUser, 1=ApplicationUser, 2=DirectoryNumber, 3=LdapUser, 4=InactiveLdapUser
        /// You cannot set or change this.
        /// </summary>
        [JsonProperty]
        public int CcmIdType { get; private set; }

  

        private string _clientMatterCode;
        /// <summary>
        /// The client matter code to transmit to Call Manger when a phone number is dialed on an outbound call. The CMC is entered after 
        /// a phone number is dialed so that the customer can assigning account or billing codes to the call. Whether or not the CMC will 
        /// be transmitted is dictated by a setting on outbound call. The subscriber's CMC is used only if the outbound call doesn't have 
        /// its own CMC.
        /// </summary>
        public string ClientMatterCode
        {
            get { return _clientMatterCode; }
            set
            {
                ChangedPropList.Add("ClientMatterCode", value);
                _clientMatterCode = value;
            }
        }

        private int _clockMode;
        /// <summary>
        /// The time format used for the message timestamps that the subscriber hears when they listen to their messages over the phone.
        /// 1=HourClock12, 2=HourClock24, 0=SystemDefaultClock
        /// </summary>
        public int ClockMode
        {
            get { return _clockMode; }
            set
            {
                ChangedPropList.Add("ClockMode", value);
                _clockMode = value;
            }
        }

        private int _confirmationConfidenceThreshold;
        /// <summary>
        /// Voice Recognition Confirmation Confidence Threshold
        /// </summary>
        public int ConfirmationConfidenceThreshold
        {
            get { return _confirmationConfidenceThreshold; }
            set
            {
                ChangedPropList.Add("ConfirmationConfidenceThreshold", value);
                _confirmationConfidenceThreshold = value;
            }
        }

        private bool _confirmDeleteMessage;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will request confirmation from a subscriber before proceeding with a deletion 
        /// of a single new or saved message
        /// </summary>
        public bool ConfirmDeleteMessage
        {
            get { return _confirmDeleteMessage; }
            set
            {
                ChangedPropList.Add("ConfirmDeleteMessage", value);
                _confirmDeleteMessage = value;
            }
        }

        private bool _confirmDeleteDeletedMessage;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will request confirmation from a subscriber before proceeding with a deletion of a 
        /// single deleted message
        /// </summary>
        public bool ConfirmDeleteDeletedMessage
        {
            get { return _confirmDeleteDeletedMessage; }
            set
            {
                ChangedPropList.Add("ConfirmDeleteDeletedMessage", value);
                _confirmDeleteDeletedMessage = value;
            }
        }

        private bool _confirmDeleteMultipleMessages;
        public bool ConfirmDeleteMultipleMessages
        {
            get { return _confirmDeleteMultipleMessages; }
            set
            {
                ChangedPropList.Add("ConfirmDeleteMultipleMessages", value);
                _confirmDeleteMultipleMessages = value;
            }
        }

        private bool _continuousAddMode;
        /// <summary>
        /// A flag indicating whether when addressing, after entering one recipient name, whether the subscriber is asked to enter another name 
        /// or assume the subscriber is finished adding names and is ready to move on to recording the message or applying message options
        /// </summary>
        public bool ContinuousAddMode
        {
            get { return _continuousAddMode; }
            set
            {
                ChangedPropList.Add("ContinuousAddMode", value);
                _continuousAddMode = value;
            }
        }

        private string _conversationName;
        /// <summary>
        /// The name of the conversation the subscriber uses to set up, send, and retrieve messages
        /// </summary>
        public string ConversationName
        {
            get { return _conversationName; }
            set
            {
                ChangedPropList.Add("ConversationName", value);
                _conversationName = value;
            }
        }

        private string _conversationTui;
        /// <summary>
        /// The name of the conversation the subscriber uses to set up, send, and retrieve messages when using touch tones
        /// </summary>
        public string ConversationTui
        {
            get { return _conversationTui; }
            set
            {
                ChangedPropList.Add("ConversationTui", value);
                _conversationTui = value;
            }
        }

        private string _conversationVui;
        /// <summary>
        /// The name of the conversation the subscriber uses to set up, send, and retrieve messages when using voice.  Currently there is only
        /// a single VUI driven conversation so there's no need to edit this.
        /// </summary>
        public string ConversationVui
        {
            get { return _conversationVui; }
            set
            {
                ChangedPropList.Add("ConversationVui", value);
                _conversationVui = value;
            }
        }

        private int _commandDigitTimeout;
        /// <summary>
        /// The amount of time (in milliseconds) between digits on a multiple digit menu command entry (i.e. different than the inter digit timeout 
        /// that is used for strings of digits such as extensions and transfer strings).
        /// </summary>
        public int CommandDigitTimeout
        {
            get { return _commandDigitTimeout; }
            set
            {
                ChangedPropList.Add("CommandDigitTimeout", value);
                _commandDigitTimeout = value;
            }
        }

        private string _country;
        public string Country
        {
            get { return _country; }
            set
            {
                ChangedPropList.Add("Country", value);
                _country = value;
            }
        }

        private int _delayAfterGreeting;
        /// <summary>
        /// The amount of time (in milliseconds) Cisco Unity Connection will delay after playing greeting
        /// </summary>
        public int DelayAfterGreeting
        {
            get { return _delayAfterGreeting; }
            set
            {
                ChangedPropList.Add("DelayAfterGreeting", value);
                _delayAfterGreeting = value;
            }
        }

        private int _deletedMessageSortOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection presents deleted messages to the subscriber.
        /// 2=FIFO, 1=LIFO
        /// </summary>
        public int DeletedMessageSortOrder
        {
            get { return _deletedMessageSortOrder; }
            set
            {
                ChangedPropList.Add("DeletedMessageSortOrder", value);
                _deletedMessageSortOrder = value;
            }
        }

    

        /// <summary>
        /// The digits corresponding to the numeric keypad mapping on a standard touchtone phone representing the first name followed by the last name of the user. 
        /// These digits are used for searching the user by name via the phone.
        /// Changing the first/last names of the user will automatically adjust this value - there should be no need to set it directly.
        /// </summary>
        [JsonProperty]
        public string DtmfNameFirstLast { get; private set; }
    
        /// <summary>
        /// The digits corresponding to the numeric keypad mapping on a standard touchtone phone representing the last name followed by the first name of the user. 
        /// These digits are used for searching the user by name via the phone.
        /// Changing the first/last names of the user will automatically adjust this value - there should be no need to set it directly.
        /// </summary>
        [JsonProperty]
        public string DtmfNameLastFirst { get; private set; }

        [JsonProperty]
        public string DtmfNameLast { get; private set; }

        [JsonProperty]
        public string DtmfNameFirst { get; private set; }

        private string _emailAddress;
        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                ChangedPropList.Add("EmailAddress", value);
                _emailAddress = value;
            }
        }

     

        private bool _enablePersonalRules;
        /// <summary>
        /// A flag indicating whether a subscriber's personal rules are enabled. Subscribers can use this setting to disable all personal rules at once
        /// </summary>
        public bool EnablePersonalRules
        {
            get { return _enablePersonalRules; }
            set
            {
                ChangedPropList.Add("EnablePersonalRules", value);
                _enablePersonalRules = value;
            }
        }

        private bool _enableMessageLocator;
        /// <summary>
        /// A flag indicating whether the message locator feature is enabled for the subscriber
        /// </summary>
        public bool EnableMessageLocator
        {
            get { return _enableMessageLocator; }
            set
            {
                ChangedPropList.Add("EnableMessageLocator", value);
                _enableMessageLocator = value;
            }
        }

        private bool _enableVisualMessageLocator;
        public bool EnableVisualMessageLocator
        {
            get { return _enableVisualMessageLocator; }
            set
            {
                ChangedPropList.Add("EnableVisualMessageLocator", value);
                _enableVisualMessageLocator = value;
            }
        }

        private bool _enableTts;
        /// <summary>
        /// A flag indicating whether TTS is enabled for the subscriber. Only relevant if TTS enabled in User's COS also
        /// </summary>
        public bool EnableTts
        {
            get { return _enableTts; }
            set
            {
                ChangedPropList.Add("EnableTts", value);
                _enableTts = value;
            }
        }

        private bool _encryptPrivateMessages;
        public bool EncryptPrivateMessages
        {
            get { return _encryptPrivateMessages; }
            set
            {
                ChangedPropList.Add("EncryptPrivateMessages", value);
                _encryptPrivateMessages = value;
            }
        }

        private bool _enAltGreetDontRingPhone;
        /// <summary>
        /// A flag indicating whether a caller is prevented from being transferred to the subscriber phone when the subscriber alternate greeting is turned on
        /// </summary>
        public bool EnAltGreetDontRingPhone
        {
            get { return _enAltGreetDontRingPhone; }
            set
            {
                ChangedPropList.Add("EnAltGreetDontRingPhone", value);
                _enAltGreetDontRingPhone = value;
            }
        }

        private bool _enAltGreetPreventSkip;
        /// <summary>
        /// A flag indicating whether callers can skip the greeting while it is playing when the alternate greeting is turned on
        /// </summary>
        public bool EnAltGreetPreventSkip
        {
            get { return _enAltGreetPreventSkip; }
            set
            {
                ChangedPropList.Add("EnAltGreetPreventSkip", value);
                _enAltGreetPreventSkip = value;
            }
        }

        private bool _enAltGreetPreventMsg;
        /// <summary>
        /// A flag indicating whether callers can leave a message after the greeting when the subscriber alternate greeting is turned on
        /// </summary>
        public bool EnAltGreetPreventMsg
        {
            get { return _enAltGreetPreventMsg; }
            set
            {
                ChangedPropList.Add("EnAltGreetPreventMsg", value);
                _enAltGreetPreventMsg = value;
            }
        }

        private string _enhancedSecurityAlias;
        /// <summary>
        /// The unique text name used to idenitify and authenticate the user with an RSA SecurID security system
        /// </summary>
        public string EnhancedSecurityAlias
        {
            get { return _enhancedSecurityAlias; }
            set
            {
                ChangedPropList.Add("EnhancedSecurityAlias", value);
                _enhancedSecurityAlias = value;
            }
        }

        private int _exitAction;
        /// <summary>
        /// The type of call action to take, e.g., hang-up, goto another object, etc
        /// 3=Error, 2=Goto, 1=Hangup, 0=Ignore, 5=SkipGreeting, 4=TakeMsg, 6=RestartGreeting, 7=TransferAltContact, 8=RouteFromNextRule
        /// </summary>
        public int ExitAction
        {
            get { return _exitAction; }
            set
            {
                ChangedPropList.Add("ExitAction", value);
                _exitAction = value;
            }
        }

        private string _exitCallActionObjectId;
        public string ExitCallActionObjectId
        {
            get { return _exitCallActionObjectId; }
            set
            {
                ChangedPropList.Add("ExitCallActionObjectId", value);
                _exitCallActionObjectId = value;
            }
        }

        private string _exitTargetConversation;
        public string ExitTargetConversation
        {
            get { return _exitTargetConversation; }
            set
            {
                ChangedPropList.Add("ExitTargetConversation", value);
                _exitTargetConversation = value;
            }
        }

        private string _exitTargetHandlerObjectId;
        public string ExitTargetHandlerObjectId
        {
            get { return _exitTargetHandlerObjectId; }
            set
            {
                ChangedPropList.Add("ExitTargetHandlerObjectId", value);
                _exitTargetHandlerObjectId = value;
            }
        }

        private string _faxServerObjectId;
        public string FaxServerObjectId
        {
            get { return _faxServerObjectId; }
            set
            {
                ChangedPropList.Add("FaxServerObjectId", value);
                _faxServerObjectId = value;
            }
        }

        private int _firstDigitTimeout;
        /// <summary>
        /// The amount of time to wait (in milliseconds) for first digit when collecting touch tones
        /// </summary>
        public int FirstDigitTimeout
        {
            get { return _firstDigitTimeout; }
            set
            {
                ChangedPropList.Add("FirstDigitTimeout", value);
                _firstDigitTimeout = value;
            }
        }

        private string _forcedAuthorizationCode;
        /// <summary>
        /// A valid authorization code that is entered prior to extending calls to classes of dialed numbers, for example, external, toll and international calls
        /// </summary>
        public string ForcedAuthorizationCode
        {
            get { return _forcedAuthorizationCode; }
            set
            {
                ChangedPropList.Add("ForcedAuthorizationCode", value);
                _forcedAuthorizationCode = value;
            }
        }

        private bool _greetByName;
        /// <summary>
        /// A flag indicating whether the subscriber hears his/her name when they log into their mailbox over the phone
        /// </summary>
        public bool GreetByName
        {
            get { return _greetByName; }
            set
            {
                ChangedPropList.Add("GreetByName", value);
                _greetByName = value;
            }
        }

        private int _inboxAutoRefresh;
        /// <summary>
        /// The rate (in minutes) at which Unity Inbox performs a refresh
        /// </summary>
        public int InboxAutoRefresh
        {
            get { return _inboxAutoRefresh; }
            set
            {
                ChangedPropList.Add("InboxAutoRefresh", value);
                _inboxAutoRefresh = value;
            }
        }

        private bool _inboxAutoResolveMessageRecipients;
        public bool InboxAutoResolveMessageRecipients
        {
            get { return _inboxAutoResolveMessageRecipients; }
            set
            {
                ChangedPropList.Add("InboxAutoResolveMessageRecipients", value);
                _inboxAutoResolveMessageRecipients = value;
            }
        }

        private int _inboxMessagesPerPage;
        public int InboxMessagesPerPage
        {
            get { return _inboxMessagesPerPage; }
            set
            {
                ChangedPropList.Add("InboxMessagesPerPage", value);
                _inboxMessagesPerPage = value;
            }
        }

        private string _initials;
        public string Initials
        {
            get { return _initials; }
            set
            {
                ChangedPropList.Add("Initials", value);
                _initials = value;
            }
        }

        private int _interdigitDelay;
        /// <summary>
        /// The amount of time to wait (in milliseconds) for input between touch tones when collecting digits in TUI
        /// </summary>
        public int InterdigitDelay
        {
            get { return _interdigitDelay; }
            set
            {
                ChangedPropList.Add("InterdigitDelay", value);
                _interdigitDelay = value;
            }
        }

        private bool _isClockMode24Hour;
        public bool IsClockMode24Hour
        {
            get { return _isClockMode24Hour; }
            set
            {
                ChangedPropList.Add("IsClockMode24Hour", value);
                _isClockMode24Hour = value;
            }
        }

        private bool _isSetForVmEnrollment;
        /// <summary>
        /// Temporary placeholder until IsVmEnrolled can be phased out. At that point, delete this column and rename tbl_UserSubscriber.IsVmEnrolled 
        /// to IsSetForVmEnrollment. A flag indicating whether Cisco Unity Connection plays the enrollment conversation (record a voice name, 
        /// indicate if they are listed in the directory, etc.) for the subscriber when they login
        /// </summary>
        public bool IsSetForVmEnrollment
        {
            get { return _isSetForVmEnrollment; }
            set
            {
                ChangedPropList.Add("IsSetForVmEnrollment", value);
                _isSetForVmEnrollment = value;
            }
        }

        /// <summary>
        /// cannot change the template value once built.
        /// </summary>
        [JsonProperty]
        public bool IsTemplate { get; private set; }

        private bool _jumpToMessagesOnLogin;
        /// <summary>
        /// A flag indicating whether the subscriber conversation jumps directly to the first message in the message stack after subscriber sign-in
        /// </summary>
        public bool JumpToMessagesOnLogin
        {
            get { return _jumpToMessagesOnLogin; }
            set
            {
                ChangedPropList.Add("JumpToMessagesOnLogin", value);
                _jumpToMessagesOnLogin = value;
            }
        }

        //cannot change LDAP properties
        /// <summary>
        /// The pkid of associated end user in the sleeping SeaDragon database.
        /// Cannot be set or edited.
        /// </summary>
        public string LdapCcmPkid { get; set; }
        
        /// <summary>
        /// The userid of associated end user in the sleeping SeaDragon database.
        /// Cannot be set or edited
        /// </summary>
        public string LdapCcmUserId { get; set; }
        public int LdapType { get; set; }

        /// <summary>
        /// The distinguished name of the mailbox.
        /// Cannot be set or edited
        /// </summary>
        public string MailboxDn { get; set; }

        private string _manager;
        public string Manager
        {
            get { return _manager; }
            set
            {
                ChangedPropList.Add("Manager", value);
                _manager = value;
            }
        }

        private int _messageLocatorSortOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection will sort messages when the "Message Locator" feature is enabled.
        /// 2=FIFO, 1=LIFO
        /// </summary>
        public int MessageLocatorSortOrder
        {
            get { return _messageLocatorSortOrder; }
            set
            {
                ChangedPropList.Add("MessageLocatorSortOrder", value);
                _messageLocatorSortOrder = value;
            }
        }

        private bool _messageTypeMenu;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection plays the message type menu when the subscriber logs on to Cisco Unity Connection over the phone
        /// </summary>
        public bool MessageTypeMenu
        {
            get { return _messageTypeMenu; }
            set
            {
                ChangedPropList.Add("MessageTypeMenu", value);
                _messageTypeMenu = value;
            }
        }

        private bool _nameConfirmation;
        /// <summary>
        /// Indicates whether the voice name of the subscriber or distribution list added to an address list when a subscriber addresses a message to other 
        /// subscribers is played. The default value for this is off (no voice name played) since the voice name was just played as part of the list of matches . 
        /// To most users this sounds redundant when on, but some users prefer it.
        /// </summary>
        public bool NameConfirmation
        {
            get { return _nameConfirmation; }
            set
            {
                ChangedPropList.Add("NameConfirmation", value);
                _nameConfirmation = value;
            }
        }

        private int _newMessageSortOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection will sort new messages.
        /// 2=FIFO, 1=LIFO
        /// </summary>
        public int NewMessageSortOrder
        {
            get { return _newMessageSortOrder; }
            set
            {
                ChangedPropList.Add("NewMessageSortOrder", value);
                _newMessageSortOrder = value;
            }
        }

        private string _newMessageStackOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection plays the following types of "new" messages: * Urgent voice messages * Non-urgent voice messages * 
        /// Urgent fax messages * Non-urgent fax messages * Urgent e-mail messages * Non-urgent e-mail messages * Receipts and notices
        /// </summary>
        public string NewMessageStackOrder
        {
            get { return _newMessageStackOrder; }
            set
            {
                ChangedPropList.Add("NewMessageStackOrder", value);
                _newMessageStackOrder = value;
            }
        }

        /// <summary>
        /// The date and time when the personal address book was last imported from a groupware package into the personal groups for a user.
        /// Cannot be set or edited
        /// </summary>
        public string PabLastImported { get; set; }

        private int _pcaAddressBookRowsPerPage;
        public int PcaAddressBookRowsPerPage
        {
            get { return _pcaAddressBookRowsPerPage; }
            set
            {
                ChangedPropList.Add("PcaAddressBookRowsPerPage", value);
                _pcaAddressBookRowsPerPage = value;
            }
        }

        private string _pcaHomePage;
        /// <summary>
        /// The Home Page is the first page that is displayed after logging in to the PCA
        /// </summary>
        public string PcaHomePage
        {
            get { return _pcaHomePage; }
            set
            {
                ChangedPropList.Add("PcaHomePage", value);
                _pcaHomePage = value;
            }
        }

        private string _postalCode;
        public string PostalCode
        {
            get { return _postalCode; }
            set
            {
                ChangedPropList.Add("PostalCode", value);
                _postalCode = value;
            }
        }

        private int _promptSpeed;
        /// <summary>
        /// The audio speed Cisco Unity Connection uses to play back prompts to the subscriber.
        /// 50 (slow) 100 (normal) 150 (faster) 200 (fastest)
        /// </summary>
        public int PromptSpeed
        {
            get { return _promptSpeed; }
            set
            {
                ChangedPropList.Add("PromptSpeed", value);
                _promptSpeed = value;
            }
        }

        private int _promptVolume;
        /// <summary>
        /// The volume level for playback of system prompts
        /// 50 is normal, 25 is quiet and 100 is loud.
        /// </summary>
        public int PromptVolume
        {
            get { return _promptVolume; }
            set
            {
                ChangedPropList.Add("PromptVolume", value);
                _promptVolume = value;
            }
        }

        private bool _readOnly;
        /// <summary>
        /// set to true user is read-only and cannot be modified via SA.
        /// </summary>
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                ChangedPropList.Add("ReadOnly", value);
                _readOnly = value;
            }
        }

        private bool _recordUnknownCallerName;
        /// <summary>
        /// A flag indicating whether a caller should be promoted to record his/her name if Unity does not receive caller id
        /// </summary>
        public bool RecordUnknownCallerName
        {
            get { return _recordUnknownCallerName; }
            set
            {
                ChangedPropList.Add("RecordUnknownCallerName", value);
                _recordUnknownCallerName = value;
            }
        }

        private int _repeatMenu;
        /// <summary>
        /// The number of times to repeat a menu in TUI
        /// </summary>
        public int RepeatMenu
        {
            get { return _repeatMenu; }
            set
            {
                ChangedPropList.Add("RepeatMenu", value);
                _repeatMenu = value;
            }
        }

        private bool _ringPrimaryPhoneFirst;
        /// <summary>
        /// A flag indicating whether a subscriber's primary phone should be rung before trying other destinations in a personal group
        /// </summary>
        public bool RingPrimaryPhoneFirst
        {
            get { return _ringPrimaryPhoneFirst; }
            set
            {
                ChangedPropList.Add("RingPrimaryPhoneFirst", value);
                _ringPrimaryPhoneFirst = value;
            }
        }

        private bool _routeNdrToSender;
        /// <summary>
        /// A flag indicating, for an undeliverable message, whether NDR messages will appear in the subscriber's mailbox or are deleted by the system
        /// </summary>
        public bool RouteNDRToSender
        {
            get { return _routeNdrToSender; }
            set
            {
                ChangedPropList.Add("RouteNDRToSender", value);
                _routeNdrToSender = value;
            }
        }

        private int _savedMessageSortOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection will sort saved messages
        /// 2=FIFO, 1=LIFO
        /// </summary>
        public int SavedMessageSortOrder
        {
            get { return _savedMessageSortOrder; }
            set
            {
                ChangedPropList.Add("SavedMessageSortOrder", value);
                _savedMessageSortOrder = value;
            }
        }

        private string _savedMessageStackOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection plays the following types of "saved" messages: * Urgent voice messages * Non-urgent voice messages 
        /// * Urgent fax messages * Non-urgent fax messages * Urgent e-mail messages * Non-urgent e-mail messages * Receipts and notices
        /// </summary>
        public string SavedMessageStackOrder
        {
            get { return _savedMessageStackOrder; }
            set
            {
                ChangedPropList.Add("SavedMessageStackOrder", value);
                _savedMessageStackOrder = value;
            }
        }

        private bool _saveMessageOnHangup;
        /// <summary>
        /// A flag indicating when hanging up while listening to a new message, whether the message is marked new again or is marked read
        /// </summary>
        public bool SaveMessageOnHangup
        {
            get { return _saveMessageOnHangup; }
            set
            {
                ChangedPropList.Add("SaveMessageOnHangup", value);
                _saveMessageOnHangup = value;
            }
        }

        private bool _sayAltGreetWarning;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection notifies the subscriber when they login via the phone (plays conversation) or CPCA 
        /// (displays a warning banner) if their alternate greeting is turned on.
        /// </summary>
        public bool SayAltGreetWarning
        {
            get { return _sayAltGreetWarning; }
            set
            {
                ChangedPropList.Add("SayAltGreetWarning", value);
                _sayAltGreetWarning = value;
            }
        }

        private bool _sayAni;
        public bool SayAni
        {
            get { return _sayAni; }
            set
            {
                ChangedPropList.Add("SayAni", value);
                _sayAni = value;
            }
        }

        private bool _sayCopiedNames;
        public bool SayCopiedNames
        {
            get { return _sayCopiedNames; }
            set
            {
                ChangedPropList.Add("SayCopiedNames", value);
                _sayCopiedNames = value;
            }
        }

        private bool _sayDistributionList;
        public bool SayDistributionList
        {
            get { return _sayDistributionList; }
            set
            {
                ChangedPropList.Add("SayDistributionList", value);
                _sayDistributionList = value;
            }
        }

        private bool _sayMsgNumber;
        public bool SayMsgNumber
        {
            get { return _sayMsgNumber; }
            set
            {
                ChangedPropList.Add("SayMsgNumber", value);
                _sayMsgNumber = value;
            }
        }

        private bool _saySender;
        public bool SaySender
        {
            get { return _saySender; }
            set
            {
                ChangedPropList.Add("SaySender", value);
                _saySender = value;
            }
        }

        private bool _saySenderExtension;
        public bool SaySenderExtension
        {
            get { return _saySenderExtension; }
            set
            {
                ChangedPropList.Add("SaySenderExtension", value);
                _saySenderExtension = value;
            }
        }

        private bool _sayTimestampAfter;
        public bool SayTimestampAfter
        {
            get { return _sayTimestampAfter; }
            set
            {
                ChangedPropList.Add("SayTimestampAfter", value);
                _sayTimestampAfter = value;
            }
        }

        private bool _sayTimestampBefore;
        public bool SayTimestampBefore
        {
            get { return _sayTimestampBefore; }
            set
            {
                ChangedPropList.Add("SayTimestampBefore", value);
                _sayTimestampBefore = value;
            }
        }

        private bool _sayTotalNew;
        public bool SayTotalNew
        {
            get { return _sayTotalNew; }
            set
            {
                ChangedPropList.Add("SayTotalNew", value);
                _sayTotalNew = value;
            }
        }

        private bool _sayTotalNewEmail;
        public bool SayTotalNewEmail
        {
            get { return _sayTotalNewEmail; }
            set
            {
                ChangedPropList.Add("SayTotalNewEmail", value);
                _sayTotalNewEmail = value;
            }
        }

        private bool _sayTotalNewFax;
        public bool SayTotalNewFax
        {
            get { return _sayTotalNewFax; }
            set
            {
                ChangedPropList.Add("SayTotalNewFax", value);
                _sayTotalNewFax = value;
            }
        }

        private bool _sayTotalNewVoice;
        public bool SayTotalNewVoice
        {
            get { return _sayTotalNewVoice; }
            set
            {
                ChangedPropList.Add("SayTotalNewVoice", value);
                _sayTotalNewVoice = value;
            }
        }

        private bool _sayTotalReceipts;
        public bool SayTotalReceipts
        {
            get { return _sayTotalReceipts; }
            set
            {
                ChangedPropList.Add("SayTotalReceipts", value);
                _sayTotalReceipts = value;
            }
        }

        private bool _sayTotalSaved;
        public bool SayTotalSaved
        {
            get { return _sayTotalSaved; }
            set
            {
                ChangedPropList.Add("SayTotalSaved", value);
                _sayTotalSaved = value;
            }
        }

        private string _searchByExtensionSearchSpaceObjectId;
        /// <summary>
        /// The unique identifier of the SearchSpace which is used to limit the visibility to dialable/addressable objects when searching by extension (dial string).
        /// </summary>
        public string SearchByExtensionSearchSpaceObjectId
        {
            get { return _searchByExtensionSearchSpaceObjectId; }
            set
            {
                ChangedPropList.Add("SearchByExtensionSearchSpaceObjectId", value);
                _searchByExtensionSearchSpaceObjectId = value;
            }
        }

        private string _searchByNameSearchSpaceObjectId;
        /// <summary>
        /// The unique identifier of the SearchSpace which is used to limit the visibility to dialable/addressable objects when searching by name (character string)
        /// </summary>
        public string SearchByNameSearchSpaceObjectId
        {
            get { return _searchByNameSearchSpaceObjectId; }
            set
            {
                ChangedPropList.Add("SearchByNameSearchSpaceObjectId", value);
                _searchByNameSearchSpaceObjectId = value;
            }
        }

        private bool _sendBroadcastMsg;
        /// <summary>
        /// A flag indicating whether the subscriber may send broadcast messages
        /// </summary>
        public bool SendBroadcastMsg
        {
            get { return _sendBroadcastMsg; }
            set
            {
                ChangedPropList.Add("SendBroadcastMsg", value);
                _sendBroadcastMsg = value;
            }
        }

        private int _sendMessageOnHangup;
        /// <summary>
        /// An enum indicating when hanging up while addressing a message that has a recording and at least one recipient, whether the message is discarded, 
        /// sent or saved as a draft message if the subscriber explicitly issues the command to send the message either via DTMF or voice input.
        /// 0=Discard, 1=Send, 2=Save
        /// </summary>
        public int SendMessageOnHangup
        {
            get { return _sendMessageOnHangup; }
            set
            {
                ChangedPropList.Add("SendMessageOnHangup", value);
                _sendMessageOnHangup = value;
            }
        }

        private int _skipForwardTime;
        /// <summary>
        /// Indicates the amount of time (in milliseconds) to jump forward when skipping ahead in a voice or TTS message using either DTMF or voice 
        /// commands while reviewing messages
        /// </summary>
        public int SkipForwardTime
        {
            get { return _skipForwardTime; }
            set
            {
                ChangedPropList.Add("SkipForwardTime", value);
                _skipForwardTime = value;
            }
        }

        private bool _skipPasswordForKnownDevice;
        /// <summary>
        /// A flag indicating whether the subscriber will be asked for his/her PIN when attempting to sign-in from a known device
        /// </summary>
        public bool SkipPasswordForKnownDevice
        {
            get { return _skipPasswordForKnownDevice; }
            set
            {
                ChangedPropList.Add("SkipPasswordForKnownDevice", value);
                _skipPasswordForKnownDevice = value;
            }
        }

        private int _skipReverseTime;
        /// <summary>
        /// Indicates the amount of time (in milliseconds) to jump backward when skipping in reverse in a voice or TTS message using either DTMF or voice 
        /// commands while reviewing messages.
        /// </summary>
        public int SkipReverseTime
        {
            get { return _skipReverseTime; }
            set
            {
                ChangedPropList.Add("SkipReverseTime", value);
                _skipReverseTime = value;
            }
        }

      
        private int _speechCompleteTimeout;
        /// <summary>
        /// Specifies the required length of silence (in milliseconds) following user speech before the recognizer finalizes a result (either accepting it 
        /// or throwing a nomatch event). The SpeechCompleteTimeout property is used when the speech prior to the silence matches an active grammar. A long 
        /// SpeechCompleteTimeout value delays the result completion and therefore makes the system's response slow. A short SpeechCompleteTimeout value may 
        /// lead to the inappropriate break up of an utterance. Reasonable SpeechCompleteTimeout values are typically in the range of 0.3 seconds to 1.0 
        /// seconds
        /// </summary>
        public int SpeechCompleteTimeout
        {
            get { return _speechCompleteTimeout; }
            set
            {
                ChangedPropList.Add("SpeechCompleteTimeout", value);
                _speechCompleteTimeout = value;
            }
        }

        private int _speechConfidenceThreshold;
        /// <summary>
        /// When the engine matches a spoken phrase, it associates a confidence level with that conclusion. This parameter determines what confidence level 
        /// should be considered a successful match. A higher value means the engine is will report fewer successful matches, but it will be more confident 
        /// in the matches that it reports
        /// </summary>
        public int SpeechConfidenceThreshold
        {
            get { return _speechConfidenceThreshold; }
            set
            {
                ChangedPropList.Add("SpeechConfidenceThreshold", value);
                _speechConfidenceThreshold = value;
            }
        }

        private int _speechIncompleteTimeout;
        /// <summary>
        /// Specifies the required length of silence (in milliseconds) from when the speech prior to the silence matches an active grammar, but where it is 
        /// possible to speak further and still match the grammar. By contrast, the SpeechCompleteTimeout property is used when the speech prior to the silence 
        /// matches an active grammar and no further words can be spoken. A long SpeechIncompleteTimeout value delays the result completion and therefore makes 
        /// the system's response slow. A short SpeechIncompleteTimeout value may lead to the inappropriate break up of an utterance. The SpeechIncompleteTimeout
        /// value is usually longer than the completetimeout value to allow users to pause mid-utterance (for example, to breathe). Note that values set for 
        /// the completetimeout property are only supported if they are less than the incompletetimeout property
        /// </summary>
        public int SpeechIncompleteTimeout
        {
            get { return _speechIncompleteTimeout; }
            set
            {
                ChangedPropList.Add("SpeechIncompleteTimeout", value);
                _speechIncompleteTimeout = value;
            }
        }

        private int _speechSensitivity;
        /// <summary>
        /// A variable level of sound sensitivity that enables the speech engine to filter out background noise and not mistake it for speech. A higher value 
        /// means higher sensitivity
        /// </summary>
        public int SpeechSensitivity
        {
            get { return _speechSensitivity; }
            set
            {
                ChangedPropList.Add("SpeechSensitivity", value);
                _speechSensitivity = value;
            }
        }

        private int _speechSpeedVsAccuracy;
        /// <summary>
        /// Tunes the engine towards Performance or Accuracy. A higher value for this setting means faster matches that may be less accurate. A lower value 
        /// for this setting means more accurate matches along with more processing and higher CPU utilization
        /// </summary>
        public int SpeechSpeedVsAccuracy
        {
            get { return _speechSpeedVsAccuracy; }
            set
            {
                ChangedPropList.Add("SpeechSpeedVsAccuracy", value);
                _speechSpeedVsAccuracy = value;
            }
        }

        private int _speed;
        /// <summary>
        /// The audio speed Cisco Unity Connection uses to play back messages to the subscriber
        /// 50 (slow) 100 (normal) 150 (faster) 200 (fastest)        
        /// </summary>
        public int Speed
        {
            get { return _speed; }
            set
            {
                ChangedPropList.Add("Speed", value);
                _speed = value;
            }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set
            {
                ChangedPropList.Add("State", value);
                _state = value;
            }
        }

        private string _synchScheduleObjectId;
        /// <summary>
        /// The unique identifier of the Schedule object to use for synchronization Calendar information from groupware (such as Exchange)
        /// </summary>
        public string SynchScheduleObjectId
        {
            get { return _synchScheduleObjectId; }
            set
            {
                ChangedPropList.Add("SynchScheduleObjectId", value);
                _synchScheduleObjectId = value;
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                ChangedPropList.Add("Title", value);
                _title = value;
            }
        }

        private bool _undeletable;
        /// <summary>
        /// A flag indicating whether this subscriber can be deleted via an administrative application such as Cisco Unity Connection Administration. 
        /// It is used to prevent deletion of factory defaults
        /// </summary>
        public bool Undeletable
        {
            get { return _undeletable; }
            set
            {
                ChangedPropList.Add("Undeletable", value);
                _undeletable = value;
            }
        }

        private bool _updateBroadcastMsg;
        /// <summary>
        /// A flag indicating whether the subscriber has the ability to update broadcast messages that are active or will be active in the future
        /// </summary>
        public bool UpdateBroadcastMsg
        {
            get { return _updateBroadcastMsg; }
            set
            {
                ChangedPropList.Add("UpdateBroadcastMsg", value);
                _updateBroadcastMsg = value;
            }
        }

        private bool _useBriefPrompts;
        /// <summary>
        /// A flag indicating whether the subscriber hears brief or full phone menus when accessing Cisco Unity Connection over the phone
        /// </summary>
        public bool UseBriefPrompts
        {
            get { return _useBriefPrompts; }
            set
            {
                ChangedPropList.Add("UseBriefPrompts", value);
                _useBriefPrompts = value;
            }
        }

        private bool _useDefaultLanguage;
        /// <summary>
        /// Set to true if call handler is using default language from the location it belongs to
        /// </summary>
        public bool UseDefaultLanguage
        {
            get { return _useDefaultLanguage; }
            set
            {
                ChangedPropList.Add("UseDefaultLanguage", value);
                _useDefaultLanguage = value;
            }
        }

        private bool _useDefaultTimeZone;
        /// <summary>
        /// Indicates if the default timezone is being used
        /// </summary>
        public bool UseDefaultTimeZone
        {
            get { return _useDefaultTimeZone; }
            set
            {
                ChangedPropList.Add("UseDefaultTimeZone", value);
                _useDefaultTimeZone = value;
            }
        }

        private bool _useDynamicNameSearchWeight;
        /// <summary>
        /// Use dynamic name search weight. When this user addresses objects, the name search weight for those objects will automatically be incremented
        /// </summary>
        public bool UseDynamicNameSearchWeight
        {
            get { return _useDynamicNameSearchWeight; }
            set
            {
                ChangedPropList.Add("UseDynamicNameSearchWeight", value);
                _useDynamicNameSearchWeight = value;
            }
        }

        private bool _useShortPollForCache;
        /// <summary>
        /// A flag indicating whether the user's polling cycle for retrieving calendar information will be the shorter "power user" polling cycle
        /// </summary>
        public bool UseShortPollForCache
        {
            get { return _useShortPollForCache; }
            set
            {
                ChangedPropList.Add("UseShortPollForCache", value);
                _useShortPollForCache = value;
            }
        }

        private bool _useVui;
        /// <summary>
        /// A flag indicating whether the speech recognition conversation is the default conversation for the subscriber
        /// </summary>
        public bool UseVui
        {
            get { return _useVui; }
            set
            {
                ChangedPropList.Add("UseVui", value);
                _useVui = value;
            }
        }

        /// <summary>
        /// The name of the WAV file containing the recorded audio (voice name, greeting, etc.) for the parent object.
        /// This value cannot be changed - use the SetUserVoiceName or GetUserVoiceName methods off the user object to
        /// get/set this WAV file.
        /// </summary>
        private string _voiceName;

        public string VoiceName
        {
            get { return _voiceName; }
            set
            {
                ChangedPropList.Add("VoiceName", value);
                _voiceName = value;
            }
        }

        private int _volume;
        /// <summary>
        /// Volume at which messages are played back by default (can be changed by user during playback).
        ///  50 is normal, 25 is quiet and 100 is loud.
        /// </summary>
        public int Volume
        {
            get { return _volume; }
            set
            {
                ChangedPropList.Add("Volume", value);
                _volume = value;
            }
        }

        private string _xferString;
        /// <summary>
        /// The cross-server transfer extension. If NULL, the user's primary extension is used
        /// </summary>
        public string XferString
        {
            get { return _xferString; }
            set
            {
                ChangedPropList.Add("XferString", value);
                _xferString = value;
            }
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// returns a single UserFull object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the user is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the user to load
        /// </param>
        /// <param name="pUser">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetUser(ConnectionServer pConnectionServer, string pObjectId, out UserFull pUser)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pUser = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetUser";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to GetUser";
                return res;
            }

            //just leverage the static full user fetch here.
            try
            {
                pUser = new UserFull(pConnectionServer, pObjectId);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch full user in GetUsers:" + ex.Message;
                res.Success = false;
                return res;
            }

            //filling the user will put a bunch of entries into the change list - clear them out here
            pUser.ClearPendingChanges();

            return res;
        }



        #endregion


        #region Instance Methods

        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        //public WebCallResult RefetchUserData()
        //{
        //    return GetUserFull(this.ObjectId);
        //}

        /// <summary>
        /// Returns a single full user object filled with all the data Conneciton provides for a user via CUPI.  Many more properties are provided on a 
        /// Full user than a base user that is provided for list presentation and selection purposes and they must be fetched one at a time, you cannot 
        /// provide a list of full users in one call.  As a rule you should refrain from fetching full user data into a list since it takes much more time 
        /// and bandwidth to construct the data.
        /// </summary>
        /// <param name="pObjectId">
        /// The objectID of the user to fetch data for.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetUserFull(string pObjectId)
        {
            string strUrl = HomeServer.BaseUrl + "users/" + pObjectId;

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

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

            return res;
        }

        /// <summary>
        /// Finds a user in the directory by alias (which are unique system wide) and will return the ObjectId if found or a blank
        /// string if not.
        /// </summary>
        /// <param name="pAlias">
        /// Alias of the user to search for.
        /// </param>
        /// <returns>
        /// ObjectId of the user if the alias is found, blank string if not.
        /// </returns>
        private string GetObjectIdFromAlias(string pAlias)
        {
            string strUrl = string.Format("{0}users/?query=(Alias is {1})", HomeServer.BaseUrl, pAlias);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return "";
            }

            List<UserBase> oUsers = HTTPFunctions.GetObjectsFromJson<UserBase>(res.ResponseText,"User");

            foreach (var oUser in oUsers)
            {
                if (oUser.Alias.Equals(pAlias, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oUser.ObjectId;
                }
            }

            return "";
        }
       #endregion

    }


}

