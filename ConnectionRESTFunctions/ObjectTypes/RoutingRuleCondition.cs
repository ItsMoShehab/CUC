using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Class that contains methods for fetching, creating and deleting routing rule conditions.  There is no editing of a 
    /// condition, you simply delete them and add a new one with the properties you want.  Order does not matter as all 
    /// conditions must evaluate to true for the rule to fire.
    /// </summary>
    public class RoutingRuleCondition
    {
        #region Constructors and Destructors


        /// <summary>
        /// Creates a new instance of the routing rule condition class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this condition as well as the routing rule identifier that this condition is associated with. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the condition being created.
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the condition on the home server provided.  If no ObjectId is passed then an empty instance of the
        /// class is returned instead.
        /// </param>
        /// <param name="pRoutingRuleObjectId">
        /// Routing rule that this rule condition is associated with.
        /// </param>
        public RoutingRuleCondition(ConnectionServer pConnectionServer, string pRoutingRuleObjectId, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to RoutingRuleCondition constructor.");
            }

            if (string.IsNullOrEmpty(pRoutingRuleObjectId))
            {
                throw new ArgumentException("Empty routing rule ObjectId RoutingRuleCondition constructor.");
            }

            HomeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or display name then go load that condition up, otherwise just return an empty instance.
            if ((string.IsNullOrEmpty(pObjectId))) return;

            ObjectId = pObjectId;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetRoutingRuleCondition(pObjectId);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res, string.Format("Condition not found in RoutingRuleCondition constructor using ObjectId={0} {1}", 
                    pObjectId, res.ErrorText));
            }
        }


        /// <summary>
        /// General constructor for Json parsing library
        /// </summary>
        public RoutingRuleCondition()
        {
        }

        #endregion


        #region Fields and Properties

        public ConnectionServer HomeServer { get; private set; }

        #endregion


        #region PrivateListMember Properties

        [JsonProperty]
        public string RoutingRuleObjectId { get; private set; }

        [JsonProperty]
        public RoutingRuleConditionParameter Parameter { get; private set; }

        [JsonProperty]
        public RoutingRuleConditionOperator Operator { get; private set; }

        [JsonProperty]
        public string OperandValue { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all routing rule conditions associated with a routing rule.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the routing rule should be pulled from
        /// </param>
        /// <param name="pRoutingRuleObjectId">
        /// Routing rule to fetch conditions for
        /// </param>
        /// <param name="pRoutingRuleConditions">
        /// Out parameter that is used to return the list of RoutingRuleCondition objects defined on Connection.  This
        /// list can be empty, conditions are not required
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
        public static WebCallResult GetRoutingRuleConditions(ConnectionServer pConnectionServer, string pRoutingRuleObjectId,
            out List<RoutingRuleCondition> pRoutingRuleConditions, int pPageNumber = 1,int pRowsPerPage = 20)
        {
            pRoutingRuleConditions = null;

            WebCallResult res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetRoutingRuleConditions";
                return res;
            }

            if (string.IsNullOrEmpty(pRoutingRuleObjectId))
            {
                res.ErrorText = "Empty RoutingRuleObjectId referenced passed to GetRoutingRuleConditions";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "routingrules/"+pRoutingRuleObjectId+"/routingruleconditions", 
                "pageNumber=" + pPageNumber,"rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - routing rules don't need conditions
            if (string.IsNullOrEmpty(res.ResponseText) || (res.TotalObjectCount==0))
            {
                pRoutingRuleConditions = new List<RoutingRuleCondition>();
                return res;
            }

            pRoutingRuleConditions = HTTPFunctions.GetObjectsFromJson<RoutingRuleCondition>(res.ResponseText);

            if (pRoutingRuleConditions == null)
            {
                pRoutingRuleConditions = new List<RoutingRuleCondition>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pRoutingRuleConditions)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }

        /// <summary>
        /// returns a single RoutingRuleCondition object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the rule condition is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the condition to load
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRoutingRuleCondition(out RoutingRuleCondition pCondition, ConnectionServer pConnectionServer, 
                                                            string pRoutingRuleObjectId, string pObjectId)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCondition = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetRoutingRuleCondition";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((pObjectId.Length == 0))
            {
                res.ErrorText = "Empty objectId passed to GetRoutingRuleCondition";
                return res;
            }

            if ((pRoutingRuleObjectId.Length == 0))
            {
                res.ErrorText = "Empty RoutingRuleObjectId passed to GetRoutingRuleCondition";
                return res;
            }

            //create a new routingRuleCondition instance passing the ObjectId which fills out the data automatically
            try
            {
                pCondition = new RoutingRuleCondition(pConnectionServer, pRoutingRuleObjectId, pObjectId);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch condition in GetRoutingRuleCondition:" + ex.Message;
            }

            return res;
        }


        /// <summary>
        /// Create a new routing rule condition in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pRoutingRuleObjectId">
        /// Routing rule to add the condition for
        /// </param>
        /// <param name="pOperator">
        /// operator (equals, lessThan etc...)
        /// </param>
        /// <param name="pParameter">
        /// Parameter (calling number, called number etc...)
        /// </param>
        /// <param name="pOperandValue">
        /// Value to evaluate the parameter against using the operator
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddRoutingRuleCondition(ConnectionServer pConnectionServer,
                                                    string pRoutingRuleObjectId,
                                                    RoutingRuleConditionOperator pOperator,
                                                    RoutingRuleConditionParameter pParameter,
                                                    string pOperandValue)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddRoutingRuleCondition";
                return res;
            }

            if (string.IsNullOrEmpty(pRoutingRuleObjectId))
            {
                res.ErrorText = "Empty RoutingRuleObjectID passed to AddRoutingRuleCondition";
                return res;
            }

            if (string.IsNullOrEmpty(pOperandValue))
            {
                res.ErrorText = "Empty pOperandValue passed to AddRoutingRuleCondition";
                return res;
            }

            string strBody = "<RoutingRuleCondition>";

            //strBody += string.Format("<{0}>{1}</{0}>", "RoutingRuleObjectId", pRoutingRuleObjectId);
            strBody += string.Format("<{0}>{1}</{0}>", "Parameter", (int) pParameter);
            strBody += string.Format("<{0}>{1}</{0}>", "Operator", (int) pOperator);
            strBody += string.Format("<{0}>{1}</{0}>", "OperandValue", pOperandValue);

            strBody += "</RoutingRuleCondition>";

            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "routingrules/"+pRoutingRuleObjectId, MethodType.POST, 
                pConnectionServer, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/routingrules/"+pRoutingRuleObjectId+"/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/routingrules/"+pRoutingRuleObjectId+"/", "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Add a new routing rule condition to a routing rule
        /// </summary>
        /// <param name="pConnectionServer">ConnectionServer the rule is homed on</param>
        /// <param name="pRoutingRuleObjectId">Identifier for the rule to add the condition for</param>
        /// <param name="pOperator">operator (equals, less than...)</param>
        /// <param name="pParameter">parameter (calling number, dialed number, port...)</param>
        /// <param name="pOperandValue">value to evaluate the parameter for using the operator</param>
        /// <param name="pCondition">New condition object is returned on this out parameter</param>
        /// <returns></returns>
        public static WebCallResult AddRoutingRuleCondition(ConnectionServer pConnectionServer,
                                                    string pRoutingRuleObjectId,
                                                    RoutingRuleConditionOperator pOperator,
                                                    RoutingRuleConditionParameter pParameter,
                                                    string pOperandValue, 
                                                    out RoutingRuleCondition pCondition)
        {
            pCondition = null;

            WebCallResult res = AddRoutingRuleCondition(pConnectionServer, pRoutingRuleObjectId, pOperator, pParameter, pOperandValue);

            //if the create goes through, fetch the condition as an object and return it all filled in.
            if (res.Success)
            {
                res = GetRoutingRuleCondition(out pCondition, pConnectionServer, pRoutingRuleObjectId, res.ReturnedObjectId);
            }

            return res;
        }


        /// <summary>
        /// Remove a routing rule condition from a routing rule in the Connection directory.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pRoutingRuleObjectId">
        /// ObjectId of the routing rule that owns the condition to be deleted
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the routing rule condition to be removed.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult DeleteRoutingRuleCondition(ConnectionServer pConnectionServer, string pRoutingRuleObjectId, string pObjectId)
        {
            WebCallResult res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer reference passed to DeleteRoutingRuleCondition";
                return res;
            }

            if (string.IsNullOrEmpty(pRoutingRuleObjectId))
            {
                res.ErrorText = "Empty RoutingRuleObjectId  passed to DeleteRoutingRuleCondition";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId  passed to DeleteRoutingRuleCondition";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "routingrules/" + pRoutingRuleObjectId+"/"+pObjectId,
                                            MethodType.DELETE, pConnectionServer, "");
        }

        #endregion


        #region Instance Methods


        public override string ToString()
        {
            return string.Format("{0} {1} {2} [{3}], holiday={2}",Parameter, Operator,OperandValue, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the condition object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the routing rule condition object instance.
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
        /// Fills the current instance of RoutineRuleCondition in with properties fetched from the server.  If both the display name and ObjectId
        /// parameters are provided, the ObjectId is used for the search.
        /// </summary>
        /// <param name="pObjectId">
        /// Unique GUID of the routing rule condition to fetch - can be blank if the display name is passed in.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetRoutingRuleCondition(string pObjectId)
        {
            string strObjectId = pObjectId;

            if (string.IsNullOrEmpty(pObjectId))
            {
                return new WebCallResult
                {
                    Success = false,
                    ErrorText = "Empty objectId passed to GetRoutingRuleCondition"
                };
            }

            string strUrl = string.Format("{0}routingrules/{1}/{2}", HomeServer.BaseUrl, RoutingRuleObjectId, strObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

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
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchRoutingRuleConditionData()
        {
            return GetRoutingRuleCondition(this.ObjectId);
        }

        /// <summary>
        /// Remove a routing rule condition from a routing rule in the Connection directory.  
        /// </summary>
        public WebCallResult Delete()
        {
            return DeleteRoutingRuleCondition(HomeServer, RoutingRuleObjectId, ObjectId);
        }

        #endregion

    }
}
