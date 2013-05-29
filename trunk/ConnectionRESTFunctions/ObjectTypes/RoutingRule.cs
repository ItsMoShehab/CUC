using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Class for fetching, adding, editing and deleting routing rules.
    /// </summary>
    public class RoutingRule :IUnityDisplayInterface
    {

        #region Constructors and Destructors

        /// <summary>
        /// default constructor used by JSON parser
        /// </summary>
        public RoutingRule()
        {
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Constructor requires ConnectionServer instance for where the rule is homed on and will take the ObjectId or 
        /// the display name of the rule to load.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the rule is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectID of the rule to load - can be passed as blank if using name to lookup rule.
        /// </param>
        /// <param name="pDisplayName">
        /// display name to look for - it's possible to have multiple display names that conflict so the first match 
        /// is returned in this case.
        /// </param>
        public RoutingRule(ConnectionServer pConnectionServer, string pObjectId, string pDisplayName = "")
            : this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to RoutingRule construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                return;
            }

            WebCallResult res = GetRoutingRule(pObjectId, pDisplayName);
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,"Failed to fetch routing rule by name or objectId:" + res);
            }

        }

        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DisplayName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region Routing Rule Properties

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

        [JsonProperty]
        public string ObjectId { get; private set; }

       

        private RoutingRuleState _state;
        public RoutingRuleState State
        {
            get { return _state; }
            set
            {
                _changedPropList.Add("State", (int)value);
                _state = value;
            }
        }


       
        private RoutingRuleType _type;
        public RoutingRuleType Type
        {
            get { return _type; }
            set
            {
                _changedPropList.Add("Type", (int)value);
                _type = value;
            }
        }

        [JsonProperty]
        public int RuleIndex { get; private set; }

        [JsonProperty]
        public RoutingRuleFlag Flags { get; private set; }

        private ConversationNames _routeTargetConversation;
        public ConversationNames RouteTargetConversation
        {
            get { return _routeTargetConversation; }
            set
            {
                _changedPropList.Add("RouteTargetConversation", value.ToString());
                _routeTargetConversation = value;
            }
        }


        [JsonProperty]
        public ConnectionObjectType RouteTargetHandlerObjectType { get; private set; }


        private string _routeTargetHandlerObjectId;
        public string RouteTargetHandlerObjectId
        {
            get { return _routeTargetHandlerObjectId; }
            set
            {
                _changedPropList.Add("RouteTargetHandlerObjectId", value);
                _routeTargetHandlerObjectId = value;
            }
        }

        [JsonProperty]
        public string RouteTargetHandlerDisplayName { get; private set; }

        private RoutintRuleActionType _routeAction;
        public RoutintRuleActionType RouteAction
        {
            get { return _routeAction; }
            set
            {
                _changedPropList.Add("RouteAction", (int)value);
                _routeAction = value;
            }
        }


        private int _languageCode;
        public int LanguageCode
        {
            get { return _languageCode; }
            set
            {
                _changedPropList.Add("LanguageCode", value);
                _languageCode = value;
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

       

        private RoutingRuleCallType _callType;
        public RoutingRuleCallType CallType
        {
            get { return _callType; }
            set
            {
                _changedPropList.Add("CallType", (int)value);
                _callType = value;
            }
        }


        private string _searchSpaceObjectId;
        public string SearchSpaceObjectId
        {
            get { return _searchSpaceObjectId; }
            set
            {
                _changedPropList.Add("SearchSpaceObjectId", value);
                _searchSpaceObjectId = value;
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
        
        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all routing rules and resturns them as a generic list of RoutingRule objects. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the rule should be pulled from
        /// </param>
        /// <param name="pRoutingRules">
        /// Out parameter that is used to return the list of RoutingRule objects defined on Connection - there must be at least one.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRoutingRules(ConnectionServer pConnectionServer, out List<RoutingRule> pRoutingRules
            , int pPageNumber = 1, int pRowsPerPage = 20, params string[] pClauses)
        {
            WebCallResult res;
            pRoutingRules = new List<RoutingRule>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetRoutingRules";
                return res;
            }

            List<string> temp;
            if (pClauses == null)
            {
                temp = new List<string>();
            }
            else
            {
                temp = pClauses.ToList();
            }
            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            string strUrl = ConnectionServer.AddClausesToUri(pConnectionServer.BaseUrl + "routingrules", temp.ToArray());

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                return res;
            }

            //not an error, just return the empty list
            if (res.TotalObjectCount == 0)
            {
                return res;
            }

            pRoutingRules = pConnectionServer.GetObjectsFromJson<RoutingRule>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pRoutingRules)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }

        /// <summary>
        /// Fetch a single instance of a RoutingRule using the objectId or name of the rule
        /// </summary>
        /// <param name="pRoutingRule">
        /// Pass back the instance of the rule on this parameter
        /// </param>
        /// <param name="pConnectionServer">
        /// Connection server being searched
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the rule to fetch
        /// </param>
        /// <param name="pDisplayName">
        /// Display name of rule to search for
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult GetRoutingRule(out RoutingRule pRoutingRule, ConnectionServer pConnectionServer,
            string pObjectId = "", string pDisplayName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pRoutingRule = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetRoutingRule";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((pObjectId.Length == 0) & (pDisplayName.Length == 0))
            {
                res.ErrorText = "Empty objectId and DisplayName passed to GetRoutingRule";
                return res;
            }

            //create a new RoutingRule instance passing the ObjectId (or name) which fills out the data automatically
            try
            {
                pRoutingRule = new RoutingRule(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch rule in GetRoutingRule:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchRoutingRuleData()
        {
            return GetRoutingRule(this.ObjectId, "");
        }


        /// <summary>
        /// Create a new routing rule in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pDisplayName">
        /// Display Name of the new routing rule - must be unique.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a property name and a new value for that property to apply to the rule being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddRoutingRule(ConnectionServer pConnectionServer, string pDisplayName,ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddRoutingRule";
                return res;
            }

            if (String.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for display name in AddRoutingRule";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            pPropList.Add("DisplayName", pDisplayName);

            string strBody = "<RoutingRule>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</RoutingRule>";

            res = pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "routingrules",
                MethodType.POST, strBody, false);

            //fetch the objectId of the newly created object off the return
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/routingrules/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/routingrules/", "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Create a new routing rule in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pDisplayName">
        /// Display Name of the new routing rule - must be unique.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a property name and a new value for that property to apply to the rule being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <param name="pRoutingRule">
        /// Newly created routing rule instance is passed back in this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddRoutingRule(ConnectionServer pConnectionServer,
                                                        string pDisplayName,
                                                        ConnectionPropertyList pPropList,
                                                        out RoutingRule pRoutingRule)
        {
            pRoutingRule = null;

            WebCallResult res = AddRoutingRule(pConnectionServer, pDisplayName,pPropList);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                //fetch the instance of the rule just created.
                try
                {
                    pRoutingRule = new RoutingRule(pConnectionServer, res.ReturnedObjectId);
                }
                catch (Exception)
                {
                    res.Success = false;
                    res.ErrorText = "Could not find newly created routing rule by objectId:" + res;
                }
            }

            return res;
        }

        /// <summary>
        /// DELETE a routing rule from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the rule is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the rule in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteRoutingRule(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteRoutingRule";
                return res;
            }

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "routingrules/" + pObjectId,
                                            MethodType.DELETE, "");
        }

        /// <summary>
        /// Allows one or more properties on a rule to be udpated (for instance display name).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs 
        /// to be passed in but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the rule is homed.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the rule to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a list property name and a new value for that property to apply to the rule being updated.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one property
        /// pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateRoutingRule(ConnectionServer pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateRoutingRule";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to UpdateRoutingRule";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateRoutingRule on ConnectonServer.cs";
                return res;
            }

            string strBody = "<RoutingRule>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</RoutingRule>";

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + string.Format("routingrules/{0}", pObjectId),
                                            MethodType.PUT, strBody, false);

        }

        /// <summary>
        /// To reorder routing rules you must list ALL the routing rules in the order you wish them to be and it's done as 
        /// a single operation.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being updated
        /// </param>
        /// <param name="pParams">
        /// List of the routing rule ObjectIds (all of them) in the order you want the rules sorted
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the output of the operation
        /// </returns>
        public static WebCallResult UpdateOrderOfAllRoutingRules(ConnectionServer pConnectionServer,
                                                                 params string[] pParams)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult
                    {
                        Success = false,
                        ErrorText = "Null ConnectionServer passed to UpdateOrderOfAllRoutingRules"
                    };
            }

            if (pParams == null || pParams.Length < 1)
            {
                return new WebCallResult
                {
                    Success = false,
                    ErrorText = "Empty parameter list passed to UpdateOrderOfAllRoutingRules"
                };

            }

            StringBuilder strBody = new StringBuilder("<RoutingRules>");

            foreach (string pParam in pParams)
            {
                strBody.AppendFormat("<{0}><ObjectId>{1}</ObjectId></{0}>", "RoutingRule", pParam);
            }
            strBody.AppendFormat("</RoutingRules>");


            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "routingrules", MethodType.PUT,strBody.ToString(), false);
        }


        #endregion


        #region Instance Methods

        public override string ToString()
        {
            return string.Format("[{2}]: {0} [{1}]", DisplayName, ObjectId,RuleIndex);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the schedule object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the schedule object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            var strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Add a condition that must be met for this routing rule to fire.
        /// </summary>
        /// <param name="pOperator">
        /// Operator to use (equals, less than etc...)
        /// </param>
        /// <param name="pParameter">
        /// Which parameter to evaluate (calling number, dialed number etc...)
        /// </param>
        /// <param name="pOperand">
        /// Value to apply the operator on against the parameter.
        /// </param>
        /// <returns></returns>
        public WebCallResult AddRoutingRuleCondition(RoutingRuleConditionOperator pOperator, RoutingRuleConditionParameter pParameter,
            string pOperand)
        {
            return RoutingRuleCondition.AddRoutingRuleCondition(HomeServer, ObjectId, pOperator, pParameter, pOperand);
        }

        /// <summary>
        /// Fills the current instance with details of a routing rule fetched using the ObjectID or the name.
        /// </summary>
        /// <param name="pObjectId">
        ///     ObjectId to search for - can be empty if name provided.
        /// </param>
        /// <param name="pDisplayName">
        ///     display name to search for.
        /// </param>
        /// <returns>
        /// Instance of the webCallSearchResult class.
        /// </returns>
        private WebCallResult GetRoutingRule(string pObjectId, string pDisplayName)
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
                        ErrorText = "Empty ObjectId passed to GetRoutingRule"
                    };
                }
            }

            string strUrl = string.Format("{0}routingrules/{1}", HomeServer.BaseUrl, strObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, RestTransportFunctions.JsonSerializerSettings);
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
        /// Fetch the ObjectId of a rule by it's name.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pName">
        /// Name of the rule to find
        /// </param>
        /// <returns>
        /// ObjectId of rule if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pName)
        {
            string strUrl = string.Format("{0}routingrules/?query=(DisplayName is {1})", HomeServer.BaseUrl, pName);

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false || res.TotalObjectCount == 0)
            {
                return "";
            }

            List<RoutingRule> oRules = HomeServer.GetObjectsFromJson<RoutingRule>(res.ResponseText);

            foreach (var oRule in oRules)
            {
                if (oRule.DisplayName.Equals(pName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oRule.ObjectId;
                }
            }

            return "";
        }


        /// <summary>
        /// DELETE a routing rule from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            return DeleteRoutingRule(HomeServer, ObjectId);
        }

        /// <summary>
        /// If the object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }

        /// <summary>
        /// Allows one or more properties on a rule to be udpated (for instance display name).  
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
                    ErrorText =string.Format("Update called but there are no pending changes for routing rule: {0}", this)
                };
            }

            //just call the static method with the info from the instance 
            WebCallResult res = UpdateRoutingRule(HomeServer, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }
        #endregion

    }
}
