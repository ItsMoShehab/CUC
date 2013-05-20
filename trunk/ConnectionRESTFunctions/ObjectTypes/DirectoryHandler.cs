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
        /// The DirectoryHandler class contains all the properties associated with a directory Handler in Unity Connection that can be fetched via the 
        /// CUPI interface.  This class also contains a number of static and instance methods for finding and listing directory handlers.  At the time
        /// of this writing CUPI does not support creating, deleting or editing directory handlers or fetching/setting voice names.
        /// </summary>
        public class DirectoryHandler : IUnityDisplayInterface
        {

            #region Constructors and Destructors


            /// <summary>
            /// Creates a new instance of the DirectoryHandler class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
            /// updating data for this handler.  
            /// If you pass the pObjectID or pDisplayName parameter the handler is automatically filled with data for that handler from the server.  
            /// If neither are passed an empty instance of the DirectoryHandler class is returned (so you can fill it out on your own).
            /// </summary>
            /// <param name="pConnectionServer">
            /// Instance of a ConnectonServer object which points to the home server for the handler being created.
            /// </param>
            /// <param name="pObjectId">
            /// Optional parameter for the unique ID of the handler on the home server provided.  If no ObjectId is passed then an empty instance of the DirectoryHandler
            /// class is returned instead.
            /// </param>
            /// <param name="pDisplayName">
            /// Optional display name search critiera - if both ObjectId and DisplayName are passed, ObjectId is used.  The display name search is not case
            /// sensitive.
            /// </param>
            public DirectoryHandler(ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = "")
                : this()
            {
                if (pConnectionServer == null)
                {
                    throw new ArgumentException("Null ConnectionServer referenced pasted to DirectoryHandler construtor");
                }

                //keep track of the home Connection server this handler is created on.
                HomeServer = pConnectionServer;

                //if the user passed in a specific ObjectId or display name then go load that handler up, otherwise just return an empty instance.
                if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pDisplayName))) return;

                //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
                WebCallResult res = GetDirectoryHandler(pObjectId, pDisplayName);

                if (res.Success == false)
                {
                    throw new UnityConnectionRestException(res,
                        string.Format("Directory Handler not found in DirectoryHandler constructor using ObjectId={0} and DisplayName={1}\n\r{2}"
                                     , pObjectId, pDisplayName, res.ErrorText));
                }
            }

            /// <summary>
            /// Generic constructor for Json parsing libraries
            /// </summary>
            public DirectoryHandler()
            {
                //make an instanced of the changed prop list to keep track of updated properties on this object
                _changedPropList = new ConnectionPropertyList();
            }

            #endregion


            #region Fields and Properties

            //used for displaying in grids and drop downs
            public string SelectionDisplayString { get { return DisplayName; } }

            //used for displaying/selecting in grids/dropdowns
            public string UniqueIdentifier { get { return ObjectId; } }

            //reference to the ConnectionServer object used to create this handlers instance.
            internal ConnectionServer HomeServer { get; private set; }

            //used to keep track of which properties have been updated
            private readonly ConnectionPropertyList _changedPropList;

            //greeting stream files are fetched on the fly if referenced
            private List<DirectoryHandlerGreetingStreamFile> _greetingStreamFiles;
            public List<DirectoryHandlerGreetingStreamFile> GetGreetingStreamFiles()
            {
                //fetch greeting options only if they are referenced
                if (_greetingStreamFiles == null)
                {
                    GetGreetingStreamFiles(out _greetingStreamFiles);
                }

                return _greetingStreamFiles;
            }

            #endregion


            #region DirectoryHandler Properties

            private bool _autoRoute;
            public bool AutoRoute 
            {
                get { return _autoRoute; } 
                set
                {
                    _changedPropList.Add("AutoRoute", value);
                    _autoRoute = value;
                } 
            }

            public DateTime CreationTime { get; set; }

            private string _displayName;
            public string DisplayName 
            {
                get { return _displayName; } 
                set
                {
                    _changedPropList.Add("DisplayName", value);
                    _displayName = value;
                } 
            }

            private string _dtmfAccessId;
            public string DtmfAccessId
            {
                get { return _dtmfAccessId; } 
                set
                {
                    _changedPropList.Add("DtmfAccessId", value);
                    _dtmfAccessId = value;
                } 
            }

            private int _endDialDelay;
            public int EndDialDelay
            {
                get { return _endDialDelay; } 
                set
                {
                    _changedPropList.Add("EndDialDelay", value);
                    _endDialDelay = value;
                } 
            }

            private int _language;
            public int Language
            {
                get { return _language; } 
                set
                {
                    _changedPropList.Add("Language", value);
                    _language = value;
                } 
            }

            private int _maxMatches;
            public int MaxMatches
            {
                get { return _maxMatches; } 
                set
                {
                    _changedPropList.Add("MaxMatches", value);
                    _maxMatches = value;
                } 
            }

            private bool _menuStyle;
            public bool MenuStyle 
            {
                get { return _menuStyle; } 
                set
                {
                    _changedPropList.Add("MenuStyle", value);
                    _menuStyle = value;
                } 
            }

            [JsonProperty]
            public string LocationObjectId { get; private set; }
            
            [JsonProperty]
            public string ObjectId { get; private set; }

            private string _partitionObjectId;
            public string PartitionObjectId
            {
                get { return _partitionObjectId; } 
                set
                {
                    _changedPropList.Add("PartitionObjectId", value);
                    _partitionObjectId = value;
                } 
            }

            private bool _playAllNames;
            public bool PlayAllNames
            {
                get { return _playAllNames; }
                set
                {
                    _changedPropList.Add("PlayAllNames", value);
                    _playAllNames = value;
                }
            }


            private bool _sayCity;
            public bool SayCity
            {
                get { return _sayCity; }
                set
                {
                    _changedPropList.Add("SayCity", value);
                    _sayCity = value;
                }
            }

            private bool _sayDepartment;
            public bool SayDepartment
            {
                get { return _sayDepartment; }
                set
                {
                    _changedPropList.Add("SayDepartment", value);
                    _sayDepartment = value;
                }
            }

            private bool _sayExtension;
            public bool SayExtension
            {
                get { return _sayExtension; }
                set
                {
                    _changedPropList.Add("SayExtension", value);
                    _sayExtension = value;
                }
            }

            private bool _searchByFirstName;
            public bool SearchByFirstName
            {
                get { return _searchByFirstName; }
                set
                {
                    _changedPropList.Add("SearchByFirstName", value);
                    _searchByFirstName = value;
                }
            }

            private DirectoryHandlerSearchScope _searchScope;
            public DirectoryHandlerSearchScope SearchScope
            {
                get { return _searchScope; }
                set
                {
                    _changedPropList.Add("SearchScope", (int)value);
                    _searchScope = value;
                }
            }

            private string _searchScopeObjectId;
            public string SearchScopeObjectId
            {
                get { return _searchScopeObjectId; }
                set
                {
                    _changedPropList.Add("SearchScopeObjectId", value);
                    _searchScopeObjectId = value;
                }
            }

            private int _speechConfidenceThreshold;
            public int SpeechConfidenceThreshold
            {
                get { return _speechConfidenceThreshold; }
                set
                {
                    _changedPropList.Add("SpeechConfidenceThreshold", value);
                    _speechConfidenceThreshold = value;
                }
            }

            private int _startDelay;
            public int StartDialDelay
            {
                get { return _startDelay; }
                set
                {
                    _changedPropList.Add("StartDialDelay", value);
                    _startDelay = value;
                }
            }

            private string _scopeObjectCosObjectId;
            public string ScopeObjectCosObjectId
            {
                get { return _scopeObjectCosObjectId; }
                set
                {
                    _changedPropList.Add("ScopeObjectCosObjectId", value);
                    _scopeObjectCosObjectId = value;
                }
            }

            private string _scopeObjectLocationObjectId;
            public string ScopeObjectLocationObjectId
            {
                get { return _scopeObjectLocationObjectId; }
                set
                {
                    _changedPropList.Add("ScopeObjectLocationObjectId", value);
                    _scopeObjectLocationObjectId = value;
                }
            }

            private string _scopeObjectDistributionListObjectId;
            public string ScopeObjectDistributionListObjectId
            {
                get { return _scopeObjectDistributionListObjectId; }
                set
                {
                    _changedPropList.Add("ScopeObjectDistributionListObjectId", value);
                    _scopeObjectDistributionListObjectId = value;
                }
            }

            private string _scopeObjectSearchSaceObjectId;
            public string ScopeObjectSearchSpaceObjectId
            {
                get { return _scopeObjectSearchSaceObjectId; }
                set
                {
                    _changedPropList.Add("ScopeObjectSearchSpaceObjectId", value);
                    _scopeObjectSearchSaceObjectId = value;
                }
            }

            private int _tries;
            public int Tries
            {
                get { return _tries; }
                set
                {
                    _changedPropList.Add("Tries", value);
                    _tries = value;
                }
            }

            private bool _useCustomGreeting;
            public bool UseCustomGreeting
            {
                get { return _useCustomGreeting; }
                set
                {
                    _changedPropList.Add("UseCustomGreeting", value);
                    _useCustomGreeting = value;
                }
            }

            private bool _undeletable;
            public bool Undeletable
            {
                get { return _undeletable; }
                set
                {
                    _changedPropList.Add("Undeletable", value);
                    _undeletable = value;
                }
            }

            private bool _useDefaultLanguage;
            public bool UseDefaultLanguage
            {
                get { return _useDefaultLanguage; }
                set
                {
                    _changedPropList.Add("UseDefaultLanguage", value);
                    _useDefaultLanguage = value;
                }
            }

            private bool _useCallLanguage;
            public bool UseCallLanguage
                        {
                            get { return _useCallLanguage; } 
                set
                {
                    _changedPropList.Add("UseCallLanguage", value);
                    _useCallLanguage = value;
                } 
            }

            private bool _useStarToExit;
            public bool UseStarToExit
            {
                get { return _useStarToExit; }
                set
                {
                    _changedPropList.Add("UseStarToExit", value);
                    _useStarToExit = value;
                }
            }

            [JsonProperty]
            public bool VoiceEnabled { get; private set; }

            [JsonProperty]
            public string VoiceName { get; private set; }

            //all exit methods

            private ActionTypes _exitAction;
            public ActionTypes ExitAction
            {
                get { return _exitAction; }
                set
                {
                    _changedPropList.Add("ExitAction", (int)value);
                    _exitAction = value;
                }
            }

            private ConversationNames _exitTargetConversation;
            public ConversationNames ExitTargetConversation
            {
                get { return _exitTargetConversation; }
                set
                {
                    _changedPropList.Add("ExitTargetConversation", value.Description());
                    _exitTargetConversation = value;
                }
            }

            private string _exitTargetHandlerObjectId;
            public string ExitTargetHandlerObjectId
            {
                get { return _exitTargetHandlerObjectId; }
                set
                {
                    _changedPropList.Add("ExitTargetHandlerObjectId", value);
                    _exitTargetHandlerObjectId = value;
                }
            }

            private ActionTypes _noInputAction;
            public ActionTypes NoInputAction
            {
                get { return _noInputAction; }
                set
                {
                    _changedPropList.Add("NoInputAction", (int) value);
                    _noInputAction = value;
                }
            }

            private ConversationNames _noInputTargetConversation;
            public ConversationNames NoInputTargetConversation
            {
                get { return _noInputTargetConversation; }
                set
                {
                    _changedPropList.Add("NoInputTargetConversation", value.Description());
                    _noInputTargetConversation = value;
                }
            }

            private string _noInputTargetHandlerObjectId;
            public string NoInputTargetHandlerObjectId
            {
                get { return _noInputTargetHandlerObjectId; }
                set
                {
                    _changedPropList.Add("NoInputTargetHandlerObjectId", value);
                    _noInputTargetHandlerObjectId = value;
                }
            }

            private ActionTypes _noSelectionAction;
            public ActionTypes NoSelectionAction
            {
                get { return _noSelectionAction; }
                set
                {
                    _changedPropList.Add("NoSelectionAction",(int) value);
                    _noSelectionAction = value;
                }
            }

            private ConversationNames _noSelectionTargetConversation;
            public ConversationNames NoSelectionTargetConversation
            {
                get { return _noSelectionTargetConversation; }
                set
                {
                    _changedPropList.Add("NoSelectionTargetConversation", value.Description());
                    _noSelectionTargetConversation = value;
                }
            }

            private string _notSelectionTargetHandlerObjectId;
            public string NoSelectionTargetHandlerObjectId
            {
                get { return _notSelectionTargetHandlerObjectId; }
                set
                {
                    _changedPropList.Add("NoSelectionTargetHandlerObjectId", value);
                    _notSelectionTargetHandlerObjectId = value;
                }
            }

            private ActionTypes _zeroAction;
            public ActionTypes ZeroAction
            {
                get { return _zeroAction; }
                set
                {
                    _changedPropList.Add("ZeroAction", (int)value);
                    _zeroAction = value;
                }
            }

            private ConversationNames _zeroTargetConversation;
            public ConversationNames ZeroTargetConversation
            {
                get { return _zeroTargetConversation; }
                set
                {
                    _changedPropList.Add("ZeroTargetConversation", value.Description());
                    _zeroTargetConversation = value;
                }
            }

            private string _zeroTargetHandlerObjectId;
            public string ZeroTargetHandlerObjectId
            {
                get { return _zeroTargetHandlerObjectId; }
                set
                {
                    _changedPropList.Add("ZeroTargetHandlerObjectId", value);
                    _zeroTargetHandlerObjectId = value;
                }
            }


            #endregion


            #region Static Methods


            /// <summary>
            /// This method allows for a GET of handlers from Connection via HTTP - it allows for passing any number of additional clauses  
            /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
            /// filter: "query=(displayname startswith ab)"
            /// sort: "sort=(displayname asc)"
            /// page: "pageNumber=0"
            ///     : "rowsPerPage=8"
            /// Escaping of spaces is done automatically, no need to account for that.
            /// </summary>
            /// <param name="pConnectionServer">
            /// Reference to the ConnectionServer object that points to the home server where the handlers are being fetched from.
            /// </param>
            /// <param name="pDirectoryHandlers">
            /// The list of handlers returned from the CUPI call (if any) is returned as a generic list of DirectoryHandler class instances via this out param.  
            /// If no handlers are found NULL is returned for this parameter.
            /// </param>
            /// <param name="pClauses">
            /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
            /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
            /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            public static WebCallResult GetDirectoryHandlers(ConnectionServer pConnectionServer, out List<DirectoryHandler> pDirectoryHandlers, 
                params string[] pClauses)
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                pDirectoryHandlers = new List<DirectoryHandler>();

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null Connection server object passed to GetDirectoryHandlers";
                    return res;
                }

                string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "handlers/directoryhandlers",pClauses);

                //issue the command to the CUPI interface
                res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

                if (res.Success == false)
                {
                    return res;
                }


                //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
                //if this is empty this isn't an error - just return an empty list
                if (string.IsNullOrEmpty(res.ResponseText))
                {
                    return res;
                }

                pDirectoryHandlers = HTTPFunctions.GetObjectsFromJson<DirectoryHandler>(res.ResponseText);

                if (pDirectoryHandlers == null)
                {
                    return res;
                }

                //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
                //run through here and assign it for all instances.
                foreach (var oObject in pDirectoryHandlers)
                {
                    oObject.HomeServer = pConnectionServer;
                    oObject.ClearPendingChanges();
                }

                return res;
            }


            /// <summary>
            /// This method allows for a GET of handlers from Connection via HTTP - it allows for passing any number of additional clauses  
            /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
            /// filter: "query=(displayname startswith ab)"
            /// sort: "sort=(displayname asc)"
            /// Escaping of spaces is done automatically, no need to account for that.
            /// </summary>
            /// <param name="pConnectionServer">
            /// Reference to the ConnectionServer object that points to the home server where the handlers are being fetched from.
            /// </param>
            /// <param name="pDirectoryHandlers">
            /// The list of handlers returned from the CUPI call (if any) is returned as a generic list of DirectoryHandler class instances via this out param.  
            /// If no handlers are found NULL is returned for this parameter.
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
            public static WebCallResult GetDirectoryHandlers(ConnectionServer pConnectionServer,out List<DirectoryHandler> pDirectoryHandlers,
                int pPageNumber=1, int pRowsPerPage=20,params string[] pClauses)
            {
                //tack on the paging items to the parameters list
                var temp = pClauses.ToList();
                temp.Add("pageNumber=" + pPageNumber);
                temp.Add("rowsPerPage=" + pRowsPerPage);

                return GetDirectoryHandlers(pConnectionServer, out pDirectoryHandlers, temp.ToArray());
            }

            /// <summary>
            /// returns a single DirectoryHandler object from an ObjectId or displayName string passed in.
            /// </summary>
            /// <param name="pConnectionServer">
            /// Connection server that the handler is homed on.
            /// </param>
            /// <param name="pObjectId">
            /// The ObjectId of the handler to load
            /// </param>
            /// <param name="pDirectoryHandler">
            /// The out param that the filled out instance of the DirectoryHandler class is returned on.
            /// </param>
            /// <param name="pDisplayName">
            /// Optional display name to search for an directory handler on.  If both the ObjectId and display name are passed, the ObjectId is used.
            /// The display name search is not case sensitive.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            public static WebCallResult GetDirectoryHandler(out DirectoryHandler pDirectoryHandler, ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = "")
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                pDirectoryHandler = null;

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null Connection server object passed to GetDirectoryHandler";
                    return res;
                }

                //you need an ObjectId and/or a display name - both being blank is not acceptable
                if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pDisplayName)))
                {
                    res.ErrorText = "Empty ObjectId and display name passed to GetDirectoryHandler";
                    return res;
                }

                //create a new DirectoryHandler instance passing the ObjectId (or display name) which fills out the data automatically
                try
                {
                    pDirectoryHandler = new DirectoryHandler(pConnectionServer, pObjectId,pDisplayName);
                    res.Success = true;
                }
                catch (UnityConnectionRestException ex)
                {
                    return ex.WebCallResult;
                }
                catch (Exception ex)
                {
                    res.ErrorText = "Failed to fetch handler in GetDirectoryHandler:" + ex.Message;
                    res.Success = false;
                }

                return res;
            }


            /// <summary>
            /// Create a new directory handler in the Connection directory, only works for Connection 10.0 builds and later
            /// </summary>
            /// <param name="pConnectionServer">
            /// Connection server being edited
            /// </param>
            /// <param name="pDisplayName">
            /// Display Name of the new directory handler - must be unique.
            /// </param>
            /// <param name="pVoiceEnabled">
            /// True creates a VUI directory handler, false creates a TUI (touch tone) handler.
            /// </param>
            /// <param name="pPropList">
            /// List ConnectionProperty pairs that identify a handlers property name and a new value for that property to apply to the handler being created.
            /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
            /// </param>
            /// <param name="pDirectoryHandler">
            /// An instance of the DirectoryHandler class will be created for the newly added directory handler and passed back on this parameter
            /// </param>
            /// <returns>
            /// Instance of the WebCallResult class.
            /// </returns>
            public static WebCallResult AddDirectoryHandler(ConnectionServer pConnectionServer,
                                                             string pDisplayName,
                                                             bool pVoiceEnabled,
                                                             ConnectionPropertyList pPropList,
                                                             out DirectoryHandler pDirectoryHandler)
             {
                 pDirectoryHandler = null;

                 WebCallResult res = AddDirectoryHandler(pConnectionServer, pDisplayName, pVoiceEnabled, pPropList);
                 
                 //if the call went through then the ObjectId will be returned in the URI form.
                 if (res.Success)
                 {
                     //fetc the instance of the directory handler just created.
                     try
                     {
                         pDirectoryHandler = new DirectoryHandler(pConnectionServer, res.ReturnedObjectId);
                     }
                     catch (Exception)
                     {
                         res.Success = false;
                         res.ErrorText = "Could not find newly created directory handler by objectId:" + res;
                     }
                 }

                 return res;
             }

            /// <summary>
            /// Create a new directory handler in the Connection directory 
            /// </summary>
            /// <param name="pConnectionServer">
            /// Connection server being edited
            /// </param>
            /// <param name="pDisplayName">
            /// Display Name of the new directory handler - must be unique.
            /// </param>
            /// <param name="pVoiceEnabled">
            /// True creates a VUI directory handler, false creates a TUI (touch tone) handler.
            /// </param>
            /// <param name="pPropList">
            /// List ConnectionProperty pairs that identify a handlers property name and a new value for that property to apply to the handler being created.
            /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResult class.
            /// </returns>
            public static WebCallResult AddDirectoryHandler(ConnectionServer pConnectionServer,
                                                        string pDisplayName,
                                                        bool pVoiceEnabled,
                                                        ConnectionPropertyList pPropList)
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null ConnectionServer referenced passed to AddDirectoryHandler";
                    return res;
                }

                if (String.IsNullOrEmpty(pDisplayName))
                {
                    res.ErrorText = "Empty value passed for display name in AddDirectoryHandler";
                    return res;
                }

                //create an empty property list if it's passed as null since we use it below
                if (pPropList == null)
                {
                    pPropList = new ConnectionPropertyList();
                }

                pPropList.Add("DisplayName", pDisplayName);
                pPropList.Add("VoiceEnabled",pVoiceEnabled);

                string strBody = "<DirectoryHandler>";

                foreach (var oPair in pPropList)
                {
                    //tack on the property value pair with appropriate tags
                    strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
                }

                strBody += "</DirectoryHandler>";

                res= HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "handlers/directoryhandlers", MethodType.POST,pConnectionServer,strBody,false);

                //fetch the objectId of the newly created object off the return
                if (res.Success)
                {
                    if (res.ResponseText.Contains(@"/vmrest/handlers/directoryhandlers/"))
                    {
                        res.ReturnedObjectId =res.ResponseText.Replace(@"/vmrest/handlers/directoryhandlers/", "").Trim();
                    }
                }

                return res;
            }

            /// <summary>
            /// DELETE a handler from the Connection directory.
            /// </summary>
            /// <param name="pConnectionServer">
            /// Reference to the ConnectionServer object that points to the home server where the handler is homed.
            /// </param>
            /// <param name="pObjectId">
            /// GUID to uniquely identify the handler in the directory.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            public static WebCallResult DeleteDirectoryHandler(ConnectionServer pConnectionServer, string pObjectId)
            {
                if (pConnectionServer == null)
                {
                    WebCallResult res = new WebCallResult();
                    res.ErrorText = "Null ConnectionServer referenced passed to DeleteDirectoryHandler";
                    return res;
                }

                return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "handlers/directoryhandlers/" + pObjectId,
                                                MethodType.DELETE,pConnectionServer, "");
            }


            /// <summary>
            /// Allows one or more properties on a handler to be udpated (for instance display name/DTMFAccessID etc...).  The caller needs to construct a list
            /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
            /// but as many as are desired can be included in a single call.
            /// </summary>
            /// <param name="pConnectionServer">
            /// Reference to the ConnectionServer object that points to the home server where the handler is homed.
            /// </param>
            /// <param name="pObjectId">
            /// The unqiue GUID identifying the handler to be updated.
            /// </param>
            /// <param name="pPropList">
            /// List ConnectionProperty pairs that identify a handler property name and a new value for that property to apply to the handler being updated.
            /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one property
            /// pair needs to be included for the funtion to execute.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            public static WebCallResult UpdateDirectoryHandler(ConnectionServer pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null ConnectionServer referenced passed to UpdateDirectoryHandler";
                    return res;
                }

                if (string.IsNullOrEmpty(pObjectId))
                {
                    res.ErrorText = "Empty objectId passed to UpdateDirectoryHandler";
                    return res;
                }

                //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
                //at lest one such pair needs to be present
                if (pPropList==null || pPropList.Count < 1)
                {
                    res.ErrorText = "empty property list passed to UpdateDirectoryHandler";
                    return res;
                }

                string strBody = "<DirectoryHandler>";

                foreach (var oPair in pPropList)
                {
                    //tack on the property value pair with appropriate tags
                    strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
                }

                strBody += "</DirectoryHandler>";

                return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "handlers/directoryhandlers/" + pObjectId,
                                                MethodType.PUT,pConnectionServer,strBody,false);

            }


            /// <summary>
            /// Sets the custom greeting recording for a particular language on a directory handler.  The WAV file is uploaded (after optionally being
            /// converted to a format Conneciton will accept), however the "use custom greeting" flag is not set automatically on the directory handler
            /// when you upload a new greeting - that needs to be set seperately on the directory handler object and saved.
            /// </summary>
            /// <param name="pConnectionServer">
            /// The Connection server that houses the directory handler being edited.
            /// </param>
            /// <param name="pSourceLocalFilePath">
            /// Full path on the local file system to the WAV file to be uploaded as the greeting.
            /// </param>
            /// <param name="pDirectoryHandlerObjectId">
            /// The GUID identifying the directory handler that owns the greeting being edited.
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
            public static WebCallResult SetGreetingWavFile(ConnectionServer pConnectionServer,
                                                            string pSourceLocalFilePath,
                                                            string pDirectoryHandlerObjectId,
                                                            int pLanguageId,
                                                            bool pConvertToPcmFirst = false)
            {
                WebCallResult res = new WebCallResult();

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null ConnectionServer referenced passed to SetGreetingWavFile";
                    return res;
                }

                //check and make sure a legit folder is referenced in the target path
                if (String.IsNullOrEmpty(pSourceLocalFilePath) || (File.Exists(pSourceLocalFilePath) == false))
                {
                    res = new WebCallResult();
                    res.Success = false;
                    res.ErrorText = "Invalid local file path passed to SetGreetingWavFile: " + pSourceLocalFilePath;
                    return res;
                }

                //if the user wants to try and rip the WAV file into G711 first before uploading the file, do that conversion here
                if (pConvertToPcmFirst)
                {
                    string strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pSourceLocalFilePath);

                    if (string.IsNullOrEmpty(strConvertedWavFilePath))
                    {
                        res.ErrorText = "Failed converting WAV file into G711 format in SetGreetingWavFile.";
                        return res;
                    }

                    if (File.Exists(strConvertedWavFilePath) == false)
                    {
                        res.ErrorText = "Converted G711 WAV file path not found in SetGreetingWavFile: " +
                                        strConvertedWavFilePath;
                        return res;
                    }

                    //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                    pSourceLocalFilePath = strConvertedWavFilePath;

                }

                //new construction - requires 8.5 or later and is done in one step to send the greeting to the server.
                string strGreetingStreamUriPath = string.Format("https://{0}:8443/vmrest/handlers/directoryhandlers/{1}/directoryhandlerstreamfiles/{2}/audio",
                                             pConnectionServer.ServerName, pDirectoryHandlerObjectId, pLanguageId);

                return HTTPFunctions.UploadWavFile(strGreetingStreamUriPath, pConnectionServer, pSourceLocalFilePath);
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
            /// <param name="pDirectoryHandlerObjectId"> 
            /// The GUID identifying the directory handler that owns the greeting being edited.
            /// </param>
            /// <param name="pLanguageId">
            /// The language ID of the WAV file being uploaded (for US English this is 1033).  The LanguageCodes enum defined in the ConnectionTypes
            /// class can be helpful here.  The language must be installed and active on the Connection server for this to be allowed.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            public static WebCallResult SetGreetingRecordingToStreamFile(ConnectionServer pConnectionServer,
                                                         string pStreamFileResourceName,
                                                         string pDirectoryHandlerObjectId,
                                                         int pLanguageId)
            {
                WebCallResult res = new WebCallResult();

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null ConnectionServer referenced passed to SetGreetingRecordingToStreamFile";
                    return res;
                }

                //check and make sure a legit folder is referenced in the target path
                if (String.IsNullOrEmpty(pStreamFileResourceName))
                {
                    res = new WebCallResult();
                    res.Success = false;
                    res.ErrorText = "Invalid stream file resource id passed to SetGreetingRecordingToStreamFile";
                    return res;
                }

                //construct the full URL to call for updating the greeting to a stream file id
                string strUrl = string.Format(@"{0}handlers/directoryhandlers/{1}/directoryhandlerstreamfiles/{2}",
                        pConnectionServer.BaseUrl, pDirectoryHandlerObjectId, pLanguageId);

                Dictionary<string, string> oParams = new Dictionary<string, string>();
                Dictionary<string, object> oOutput;

                oParams.Add("op", "RECORD");
                oParams.Add("ResourceType", "STREAM");
                oParams.Add("resourceId", pStreamFileResourceName);
                oParams.Add("lastResult", "0");
                oParams.Add("speed", "100");
                oParams.Add("volume", "100");
                oParams.Add("startPosition", "0");

                return HTTPFunctions.GetJsonResponse(strUrl, MethodType.PUT, pConnectionServer, oParams, out oOutput);
            }



            #endregion


            #region Instance Methods

            /// <summary>
            /// Diplays the display name and extension of the handler by default.
            /// </summary>
            public override string ToString()
            {
                return String.Format("{0} x{1}", this.DisplayName, this.DtmfAccessId);
            }

            /// <summary>
            /// Dumps out all the properties associated with the instance of the directory handler object in "name=value" format - each pair is on its
            /// own line in the string returned.
            /// </summary>
            /// <param name="pPrefix">
            /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
            /// property dump when writing to a log file for instance.
            /// </param>
            /// <returns>
            /// string containing all the name value pairs defined in the directory handler object instance.
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
            public WebCallResult RefetchDirectoryHandlerData()
            {
                return GetDirectoryHandler(this.ObjectId);
            }

            /// <summary>
            /// Fills the current instance of DirectoryHandler in with properties fetched from the server.  If both the display name and ObjectId
            /// parameters are provided, the ObjectId is used for the search.
            /// </summary>
            /// <param name="pObjectId">
            /// Unique GUID of the directory handler to fetch - can be blank if the display name is passed in.
            /// </param>
            /// <param name="pDisplayName">
            /// Display name (required to be unique for all directory handlers) to search on an directory handler by.  Can be blank if the ObjectId 
            /// parameter is provided.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            private WebCallResult GetDirectoryHandler(string pObjectId, string pDisplayName = "")
            {
                string strObjectId = pObjectId;
                if (string.IsNullOrEmpty(pObjectId))
                {
                    strObjectId = GetObjectIdFromName(pDisplayName);
                    if (string.IsNullOrEmpty(strObjectId))
                    {
                        return new WebCallResult
                        {
                            Success = false,
                            ErrorText = "No directory handler found for name=" + pDisplayName
                        };
                    }
                }

                string strUrl = string.Format("{0}handlers/directoryhandlers/{1}", HomeServer.BaseUrl, strObjectId);

                //issue the command to the CUPI interface
                WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

                if (res.Success == false)
                {
                    return res;
                }

                if (string.IsNullOrEmpty(res.ResponseText))
                {
                    res.Success = false;
                    res.ErrorText = "Failed to find directory handler by objectid=" + pObjectId + " or name=" +DisplayName;
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

                ClearPendingChanges();

                return res;
            }


            /// <summary>
            /// Fetch the ObjectId of a directory handler by it's name.  Empty string returned if not match is found.
            /// </summary>
            /// <param name="pName">
            /// name of the directory handler to find
            /// </param>
            /// <returns>
            /// ObjectId of directory handler if found or empty string if not.
            /// </returns>
            private string GetObjectIdFromName(string pName)
            {
                // string strUrl = string.Format("{0}coses/?query=(DisplayName is {1})", HomeServer.BaseUrl, pCosName);
                string strUrl = string.Format("{0}handlers/directoryhandlers/?query=(DisplayName is {1})", HomeServer.BaseUrl, pName);

                //issue the command to the CUPI interface
                WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

                if (res.Success == false || res.TotalObjectCount == 0)
                {
                    return "";
                }

                List<DirectoryHandler> oHandlers = HTTPFunctions.GetObjectsFromJson<DirectoryHandler>(res.ResponseText);

                foreach (var oHandler in oHandlers)
                {
                    if (oHandler.DisplayName.Equals(pName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return oHandler.ObjectId;
                    }
                }

                return "";
            }

            /// <summary>
            /// DELETE a directory handler from the Connection directory.
            /// </summary>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            public WebCallResult Delete()
            {
                //just call the static method with the info on the instance
                return DeleteDirectoryHandler(this.HomeServer, ObjectId);
            }

            /// <summary>
            /// Allows one or more properties on a handler to be udpated (for instance display name, DTMFAccessID etc...).  The caller needs to construct a list
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
                if (!_changedPropList.Any())
                {
                    res = new WebCallResult();
                    res.Success = false;
                    res.ErrorText = string.Format("Update called but there are no pending changes for directory handler {0}", this);
                    return res;
                }

                //just call the static method with the info from the instance 
                res = UpdateDirectoryHandler(HomeServer, ObjectId, _changedPropList);

                //if the update went through then clear the changed properties list.
                if (res.Success)
                {
                    _changedPropList.Clear();
                }

                return res;
            }

            /// <summary>
            /// If the directory handler object has andy pending updates that have not yet be comitted, this will clear them out.
            /// </summary>
            public void ClearPendingChanges()
            {
                _changedPropList.Clear();
            }


            /// <summary>
            /// Sets the custom greeting recording for a particular language on a directory handler.  The WAV file is uploaded (after optionally being
            /// converted to a format Conneciton will accept), however the "use custom greeting" flag is not set automatically on the directory handler
            /// when you upload a new greeting - that needs to be set seperately on the directory handler object and saved.
            /// </summary>
            /// <param name="pSourceLocalFilePath">
            /// Full path on the local file system to the WAV file to be uploaded as the greeting.
            /// </param>
            /// <param name="pLanguageId">
            /// The language ID of the WAV file being uploaded (for US English this is 1033).  The LanguageCodes enum defined in the ConnectionTypes
            /// class can be helpful here.  The language must be installed and active on the Connection server for this to be allowed.
            /// </param>
            /// <param name="pConvertToPcmFirst">
            /// Defaults to false, but if passed as true this has the target WAV file first converted PCM, 16 Khz, 8 bit mono before uploading.  This 
            /// helps ensure Connection will not complain about the media file format.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            public WebCallResult SetGreetingWavFile(string pSourceLocalFilePath,
                                                           int pLanguageId,
                                                           bool pConvertToPcmFirst = false)
            {
                return SetGreetingWavFile(HomeServer, pSourceLocalFilePath, ObjectId, pLanguageId, pConvertToPcmFirst);
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
            public WebCallResult SetGreetingRecordingToStreamFile(string pStreamFileResourceName,int pLanguageId)
            {
                return SetGreetingRecordingToStreamFile(HomeServer, pStreamFileResourceName, ObjectId, pLanguageId);
            }


            //helper function to fetch all greeting stream files associated with this dir handler (if any).
            //If there are no custom recorded greetings the pGreetingStreamFiles out param is returned as null.
            private WebCallResult GetGreetingStreamFiles(out List<DirectoryHandlerGreetingStreamFile> pGreetingStreamFiles)
            {
                return DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(HomeServer, ObjectId, out pGreetingStreamFiles);
            }

            #endregion

        }
}
