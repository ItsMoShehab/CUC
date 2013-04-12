#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
        /// <summary>
        /// The InterviewHandler class contains all the properties associated with a Interview Handler in Unity Connection that can be fetched via the 
        /// CUPI interface.  This class also contains a number of static and instance methods for finding and listing interview handlers.  At the time
        /// of this writing CUPI does not support creating, deleting or editing interview handlers or fetching/setting voice names.
        /// </summary>
        public class InterviewHandler
        {
            #region Fields and Properties

            private int _afterMessageAction;
            public int AfterMessageAction
            {
                get { return _afterMessageAction; }
                set
                {
                    _changedPropList.Add("_afterMessageAction", value);
                    _afterMessageAction = value;
                }
            }

            private string _afterMessageTargetConversation;
            public string AfterMessageTargetConversation
            {
                get { return _afterMessageTargetConversation; }
                set
                {
                    _changedPropList.Add("AfterMessageTargetConversation", value);
                    _afterMessageTargetConversation = value;
                }
            }

            private string _afterMessageTargetHandlerObjectId;
            public string AfterMessageTargetHandlerObjectId
            {
                get { return _afterMessageTargetHandlerObjectId; }
                set
                {
                    _changedPropList.Add("AfterMessageTargetHandlerObjectId", value);
                    _afterMessageTargetHandlerObjectId = value;
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

            private bool _dispatchDelivery;
            public bool DispatchDelivery
            {
                get { return _dispatchDelivery; }
                set
                {
                    _changedPropList.Add("DispatchDelivery", value);
                    _dispatchDelivery = value;
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
            
            public string LocationObjectId { get; set; }
            public string ObjectId { get; set; }

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

            private string _recipientSubscriberObjectId;
            public string RecipientSubscriberObjectId
            {
                get { return _recipientSubscriberObjectId; }
                set
                {
                    _changedPropList.Add("RecipientSubscriberObjectId", value);
                    _recipientSubscriberObjectId = value;
                }
            }

            private string _recipientDistributionListObjectId;
            public string RecipientDistributionListObjectId
            {
                get { return _recipientDistributionListObjectId; }
                set
                {
                    _changedPropList.Add("RecipientDistributionListObjectId", value);
                    _recipientDistributionListObjectId = value;
                }
            }

            private int _sendUrgentMsg;
            public int SendUrgentMsg
            {
                get { return _sendUrgentMsg; }
                set
                {
                    _changedPropList.Add("SendUrgentMsg", value);
                    _sendUrgentMsg = value;
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

            public string VoiceName { get; set; }

            //reference to the ConnectionServer object used to create this handlers instance.
            internal ConnectionServer HomeServer { get; private set; }

            //used to keep track of whic properties have been updated
            private readonly ConnectionPropertyList _changedPropList;

            #endregion


            #region Constructors

            /// <summary>
            /// Creates a new instance of the InterviewHandler class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
            /// updating data for this handler.  
            /// If you pass the pObjectID or pDisplayName parameter the handler is automatically filled with data for that handler from the server.  
            /// If neither are passed an empty instance of the InterviewHandler class is returned (so you can fill it out on your own).
            /// </summary>
            /// <param name="pConnectionServer">
            /// Instance of a ConnectonServer object which points to the home server for the handler being created.
            /// </param>
            /// <param name="pObjectId">
            /// Optional parameter for the unique ID of the handler on the home server provided.  If no ObjectId is passed then an empty instance of the InterviewHandler
            /// class is returned instead.
            /// </param>
            /// <param name="pDisplayName">
            /// Optional display name search critiera - if both ObjectId and DisplayName are passed, ObjectId is used.  The display name search is not case
            /// sensitive.
            /// </param>
            public InterviewHandler(ConnectionServer pConnectionServer, string pObjectId="", string pDisplayName=""):this()
            {
                if (pConnectionServer == null)
                {
                    throw new ArgumentException("Null ConnectionServer referenced pasted to InterviewHandler construtor");
                }

                //keep track of the home Connection server this handler is created on.
                HomeServer = pConnectionServer;

                //if the user passed in a specific ObjectId or display name then go load that handler up, otherwise just return an empty instance.
                if ((string.IsNullOrEmpty(pObjectId)) & (string.IsNullOrEmpty(pDisplayName))) return;

                //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
                WebCallResult res = GetInterviewHandler(pObjectId,pDisplayName);

                if (res.Success == false)
                {
                    throw new Exception(
                        string.Format("Interview Handler not found in InterviewHandler constructor using ObjectId={0} and DisplayName={1}\n\r{2}"
                                     , pObjectId, pDisplayName, res.ErrorText));
                }
            }


            /// <summary>
            /// generic constructor for Json parsing libraries
            /// </summary>
            public InterviewHandler()
            {
                //make an instanced of the changed prop list to keep track of updated properties on this object
                _changedPropList = new ConnectionPropertyList();
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
            /// <param name="pInterviewHandlers">
            /// The list of handlers returned from the CUPI call (if any) is returned as a generic list of InterviewHandler class instances via this out param.  
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
            public static WebCallResult GetInterviewHandlers(ConnectionServer pConnectionServer, out List<InterviewHandler> pInterviewHandlers, params string[] pClauses)
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                pInterviewHandlers = null;

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null Connection server object passed to GetInterviewHandlers";
                    return res;
                }

                string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "handlers/interviewhandlers",pClauses);

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
                    pInterviewHandlers = new List<InterviewHandler>();
                    return res;
                }

                pInterviewHandlers = HTTPFunctions.GetObjectsFromJson<InterviewHandler>(res.ResponseText);

                if (pInterviewHandlers == null)
                {
                    pInterviewHandlers = new List<InterviewHandler>();
                    return res;
                }

                //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
                //run through here and assign it for all instances.
                foreach (var oObject in pInterviewHandlers)
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
            /// <param name="pInterviewHandlers">
            /// The list of handlers returned from the CUPI call (if any) is returned as a generic list of InterviewHandler class instances via this out param.  
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

            public static WebCallResult GetInterviewHandlers(ConnectionServer pConnectionServer,out List<InterviewHandler> pInterviewHandlers,
                int pPageNumber=1, int pRowsPerPage=20,params string[] pClauses)
            {
                //tack on the paging items to the parameters list
                var temp = pClauses.ToList();
                temp.Add("pageNumber=" + pPageNumber);
                temp.Add("rowsPerPage=" + pRowsPerPage);

                return GetInterviewHandlers(pConnectionServer, out pInterviewHandlers, temp.ToArray());
            }


            /// <summary>
            /// returns a single InterviewHandler object from an ObjectId or displayName string passed in.
            /// </summary>
            /// <param name="pConnectionServer">
            /// Connection server that the handler is homed on.
            /// </param>
            /// <param name="pObjectId">
            /// The ObjectId of the handler to load
            /// </param>
            /// <param name="pInterviewHandler">
            /// The out param that the filled out instance of the InterviewHandler class is returned on.
            /// </param>
            /// <param name="pDisplayName">
            /// Optional display name to search for an interview handler on.  If both the ObjectId and display name are passed, the ObjectId is used.
            /// The display name search is not case sensitive.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            public static WebCallResult GetInterviewHandler(out InterviewHandler pInterviewHandler, ConnectionServer pConnectionServer, 
                string pObjectId = "", string pDisplayName = "")
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                pInterviewHandler = null;

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null Connection server object passed to GetInterviewHandler";
                    return res;
                }

                //you need an ObjectId and/or a display name - both being blank is not acceptable
                if ((pObjectId.Length == 0) & (pDisplayName.Length == 0))
                {
                    res.ErrorText = "Empty ObjectId and display name passed to GetInterviewHandler";
                    return res;
                }

                //create a new InterviewHandler instance passing the ObjectId (or display name) which fills out the data automatically
                try
                {
                    pInterviewHandler = new InterviewHandler(pConnectionServer, pObjectId,pDisplayName);
                    res.Success = true;
                }
                catch (Exception ex)
                {
                    res.ErrorText = "Failed to fetch handler in GetInterviewHandler:" + ex.Message;
                    res.Success = false;
                }

                return res;
            }


            /// <summary>
            /// Create a new interview handler in the Connection directory 
            /// </summary>
            /// <param name="pConnectionServer">
            /// Connection server being edited
            /// </param>
            /// <param name="pDisplayName">
            /// Display Name of the new interview handler - must be unique.
            /// </param>
            /// <param name="pPropList">
            /// List ConnectionProperty pairs that identify a handlers property name and a new value for that property to apply to the handler being created.
            /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
            /// </param>
            /// <param name="pInterviewHandler">
            /// An instance of the InterviewHandler class will be created for the newly added handler and passed back on this parameter
            /// </param>
            /// <returns>
            /// Instance of the WebCallResult class.
            /// </returns>
            public static WebCallResult AddInterviewHandler(ConnectionServer pConnectionServer,
                                                             string pDisplayName,
                                                             ConnectionPropertyList pPropList,
                                                             out InterviewHandler pInterviewHandler)
            {
                pInterviewHandler = null;

                WebCallResult res = AddInterviewHandler(pConnectionServer, pDisplayName, pPropList);

                //if the call went through then the ObjectId will be returned in the URI form.
                if (res.Success)
                {
                    //fetc the instance of the directory handler just created.
                    try
                    {
                        pInterviewHandler = new InterviewHandler(pConnectionServer, res.ReturnedObjectId);
                    }
                    catch (Exception)
                    {
                        res.Success = false;
                        res.ErrorText = "Could not find newly created interview handler by objectId:" + res;
                    }
                }

                return res;
            }

            /// <summary>
            /// Create a new interview handler in the Connection directory 
            /// </summary>
            /// <param name="pConnectionServer">
            /// Connection server being edited
            /// </param>
            /// <param name="pDisplayName">
            /// Display Name of the new interview handler - must be unique.
            /// </param>
            /// <param name="pPropList">
            /// List ConnectionProperty pairs that identify a handlers property name and a new value for that property to apply to the handler being created.
            /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResult class.
            /// </returns>
            public static WebCallResult AddInterviewHandler(ConnectionServer pConnectionServer,
                                                        string pDisplayName,
                                                        ConnectionPropertyList pPropList)
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null ConnectionServer referenced passed to AddInterviewHandler";
                    return res;
                }

                if (String.IsNullOrEmpty(pDisplayName))
                {
                    res.ErrorText = "Empty value passed for display name in AddInterviewHandler";
                    return res;
                }

                //create an empty property list if it's passed as null since we use it below
                if (pPropList == null)
                {
                    pPropList = new ConnectionPropertyList();
                }

                pPropList.Add("DisplayName", pDisplayName);

                string strBody = "<InterviewHandler>";

                foreach (var oPair in pPropList)
                {
                    //tack on the property value pair with appropriate tags
                    strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
                }

                strBody += "</InterviewHandler>";

                res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "handlers/interviewhandlers",MethodType.POST,pConnectionServer,strBody);

                //fetch the objectId of the newly created object off the return
                if (res.Success)
                {
                    if (res.ResponseText.Contains(@"/vmrest/handlers/interviewhandlers/"))
                    {
                        res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/handlers/interviewhandlers/", "").Trim();
                    }
                }

                return res;
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
            public static WebCallResult UpdateInterviewHandler(ConnectionServer pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null ConnectionServer referenced passed to UpdateInterviewHandler";
                    return res;
                }

                //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
                //at lest one such pair needs to be present
                if (pPropList == null || pPropList.Count < 1)
                {
                    res.ErrorText = "empty property list passed to UpdateInterviewHandler";
                    return res;
                }

                string strBody = "<InterviewHandler>";

                foreach (var oPair in pPropList)
                {
                    //tack on the property value pair with appropriate tags
                    strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
                }

                strBody += "</InterviewHandler>";

                return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "handlers/interviewhandlers/" + pObjectId,
                                                MethodType.PUT,pConnectionServer,strBody);
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
            public static WebCallResult DeleteInterviewHandler(ConnectionServer pConnectionServer, string pObjectId)
            {
                if (pConnectionServer == null)
                {
                    WebCallResult res = new WebCallResult();
                    res.ErrorText = "Null ConnectionServer referenced passed to DeleteInterviewHandler";
                    return res;
                }

                return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "handlers/interviewhandlers/" + pObjectId,
                                                MethodType.DELETE,pConnectionServer, "");
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
            /// Pull the data from the Connection server for this object again - if changes have been made external this will 
            /// "refresh" the object
            /// </summary>
            /// <returns>
            /// Instance of the WebCallResult class.
            /// </returns>
            public WebCallResult RefetchInterviewHandlerData()
            {
                return GetInterviewHandler(this.ObjectId);
            }


            /// <summary>
            /// Fills the current instance of InterviewHandler in with properties fetched from the server.  If both the display name and ObjectId
            /// parameters are provided, the ObjectId is used for the search.
            /// </summary>
            /// <param name="pObjectId">
            /// Unique GUID of the interview handler to fetch - can be blank if the display name is passed in.
            /// </param>
            /// <param name="pDisplayName">
            /// Display name (required to be unique for all interview handlers) to search on an interview handler by.  Can be blank if the ObjectId 
            /// parameter is provided.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            private WebCallResult GetInterviewHandler(string pObjectId, string pDisplayName = "")
            {
                string strUrl;
                WebCallResult res;

                //when fetching a handler use the query construct in both cases so the XML parsing is identical
                if (pObjectId.Length > 0)
                {
                    strUrl = string.Format("{0}handlers/interviewhandlers/?query=(ObjectId is {1})", HomeServer.BaseUrl, pObjectId);
                }
                else if (pDisplayName.Length > 0)
                {
                    strUrl = string.Format("{0}handlers/interviewhandlers/?query=(DisplayName is {1})", HomeServer.BaseUrl, pDisplayName);
                }
                else
                {
                    res = new WebCallResult();
                    res.Success = false;
                    res.ErrorText = "No value for ObjectId or display name passed to GetInterviewHandler.";
                    return res;
                }

                //issue the command to the CUPI interface
                res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

                if (res.Success == false)
                {
                    return res;
                }

                if (res.TotalObjectCount == 0)
                {
                    res.Success = false;
                    res.ErrorText="Interviewer not found by objectId="+pObjectId+" or name="+pDisplayName;
                    return res;
                }

                try
                {
                    JsonConvert.PopulateObject(HTTPFunctions.StripJsonOfObjectWrapper(res.ResponseText,"InterviewHandler"), this);
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
                    res.ErrorText = string.Format("Update called but there are no pending changes for interview handler {0}", this);
                    return res;
                }

                //just call the static method with the info from the instance 
                res = UpdateInterviewHandler(HomeServer, ObjectId, _changedPropList);

                //if the update went through then clear the changed properties list.
                if (res.Success)
                {
                    _changedPropList.Clear();
                }

                return res;
            }


            /// <summary>
            /// Remove an interview handler from the directory
            /// </summary>
            public WebCallResult Delete()
            {
                return DeleteInterviewHandler(this.HomeServer, this.ObjectId);
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
