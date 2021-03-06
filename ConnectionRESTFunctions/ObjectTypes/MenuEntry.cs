﻿#region Legal Disclaimer

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
    /// The MenuEntry class contains all the properties associated with a menu entries in Unity Connection that can be fetched 
    /// via the CUPI interface.  This class also contains a number of static and instance methods for finding, editing and listing
    /// menu entries.  You cannot add or remove menu entries.
    /// </summary>
    [Serializable]
    public class MenuEntry : IUnityDisplayInterface
    {

        #region Constructor

        /// <summary>
        /// default constructor for JSON parsing
        /// </summary>
        public MenuEntry()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Creates a new instance of the MenuEntry class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this entry.  
        /// If you pass the pObjectID parameter the menu entry is automatically filled with data for that entry from the server.  If no pObjectID is passed an
        /// empty instance of the Menuentry class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the menu entry being created.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// GIOD that identifies the call handler that owns the menu entry
        /// </param>
        /// <param name="pKey">
        /// Key name to fetch - can be 0-9, # or *
        /// </param>
        public MenuEntry(ConnectionServerRest pConnectionServer, string pCallHandlerObjectId, string pKey = ""):this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to MenuEntry constructor.");
            }

            //we must know what call handler we're associated with.
            if (String.IsNullOrEmpty(pCallHandlerObjectId))
            {
                throw new ArgumentException("Invalid CallHandlerObjectID passed to MenuEntry constructor.");
            }

            HomeServer = pConnectionServer;

            //remember the objectID of the owner of the menu entry as the CUPI interface requires this in the URL construction
            //for operations editing them.
            CallHandlerObjectId = pCallHandlerObjectId;

            //if the user passed in a specific ObjectId then go load that menu entry up, otherwise just return an empty instance.
            if (string.IsNullOrEmpty(pKey)) return;

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetMenuEntry(pKey);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Menu Entry not found in MenuEntry constructor using CallHandlerObjectID={0} " +
                                                                         "and key={1}\n\rError={2}",pCallHandlerObjectId, pKey, res.ErrorText));
            }
        }

        #endregion

        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return TouchtoneKey; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        //reference to the ConnectionServer object used to create this menu entry instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        //for checking on pending changes
        public ConnectionPropertyList ChangeList { get { return _changedPropList; } }

        #endregion

        #region MenuEntry Properties

        private ActionTypes _action;
        /// <summary>
        /// The type of call action to take, e.g., hang-up, goto another object, etc
        /// 3=Error, 2=Goto, 1=Hangup, 0=Ignore, 5=SkipGreeting, 4=TakeMsg, 6=RestartGreeting, 7=TransferAltContact, 8=RouteFromNextRule
        /// </summary>
        public ActionTypes Action
        {
            get { return _action; }
            set
            {
                _changedPropList.Add("Action",(int) value);
                _action = value;
            }
        }

        //you can't change the call handler owner
        [JsonProperty]
        public string CallHandlerObjectId { get; private set; }

        private bool _locked;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection ignores additional input after callers press this key. Values: 
        /// false: Additional input accepted true: Additional input ignored; Cisco Unity Connection performs the action assigned to the key
        /// </summary>
        public bool Locked
        {
            get { return _locked; }
            set
            {
                _changedPropList.Add("Locked", value);
                _locked = value;
            }
        }

        //you can't change the ObjectId of a standing object
        [JsonProperty]
        public string ObjectId { get; private set; }

        private ConversationNames _targetConversation;
        /// <summary>
        /// The name of the conversation to which the caller is routed
        /// </summary>
        public ConversationNames TargetConversation
        {
            get { return _targetConversation; }
            set
            {
                _changedPropList.Add("TargetConversation", value.Description());
                _targetConversation = value;
            }
        }

        private string _targetHandlerObjectId;
        public string TargetHandlerObjectId
        {
            get { return _targetHandlerObjectId; }
            set
            {
                _changedPropList.Add("TargetHandlerObjectId", value);
                _targetHandlerObjectId = value;
            }
        }


        //you cannot change the key name of a standing menu entry
        [JsonProperty]
        public string TouchtoneKey { get; private set; }

        private string _transferNumber;
        public string TransferNumber
        {
            get { return _transferNumber; }
            set
            {
                _changedPropList.Add("TransferNumber", value);
                _transferNumber = value;
            }
        }

        private int _transferRings;
        public int TransferRings
        {
            get { return _transferRings; }
            set
            {
                _changedPropList.Add("TransferRings", value);
                _transferRings = value;
            }
        }

        private TransferTypes _transferType;
        /// <summary>
        /// The type of call transfer Cisco Unity Connection will perform - supervised or unsupervised (also referred to as "Release to Switch" transfer).
        /// 1=Supervised, 0=Unsupervised
        /// </summary>
        public TransferTypes TransferType
        {
            get { return _transferType; }
            set
            {
                _changedPropList.Add("TransferType",(int) value);
                _transferType = value;
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Fetches a menu entry  object filled with all the properties for a specific entry identified with the ObjectId
        /// of the call handler that owns it and the key name of the key to fetch.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that the menu entry is homed on.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// The objectID of the call handler that owns the menu entry to be fetched.
        /// </param>
        /// <param name="pKeyName">
        /// The name of the menu entry key to fetch.  Can be 0-9, * or #
        /// </param>
        /// <param name="pMenuEntry">
        /// The out parameter that the instance of the MenuEntry class filled in with the details of the fetched entry is
        /// passed back on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMenuEntry(ConnectionServerRest pConnectionServer,
                                                        string pCallHandlerObjectId,
                                                        string pKeyName,
                                                        out  MenuEntry pMenuEntry)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pMenuEntry = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetMenuEntry";
                return res;
            }

            //create a new menu entry instance passing the key name which fills out the data automatically
            try
            {
                pMenuEntry = new MenuEntry(pConnectionServer, pCallHandlerObjectId,pKeyName );
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch menu entry in GetMenuEntry:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Returns all the menu entries for a call handler. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the menu entries are being fetched from.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// GUID identifying the call handler that owns the menu entries being fetched
        /// </param>
        /// <param name="pMenuEntries">
        /// The list of MenuEntry objects are returned using this out parameter.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMenuEntries(ConnectionServerRest pConnectionServer,
                                                            string pCallHandlerObjectId,
                                                           out List<MenuEntry> pMenuEntries)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pMenuEntries = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetMenuEntries";
                return res;
            }

            string strUrl = string.Format("{0}handlers/callhandlers/{1}/menuentries", pConnectionServer.BaseUrl, pCallHandlerObjectId);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty thats an error - there should alway be menu entries returned
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pMenuEntries = new List<MenuEntry>();
                res.Success = false;
                return res;
            }

            pMenuEntries = pConnectionServer.GetObjectsFromJson<MenuEntry>(res.ResponseText);

            if (pMenuEntries == null)
            {
                pMenuEntries = new List<MenuEntry>();
                res.ErrorText = "Could not parse JSON into MenuEntry list:" + res.ResponseText;
                res.Success = false;
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pMenuEntries)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.CallHandlerObjectId = pCallHandlerObjectId;
                oObject.ClearPendingChanges();
            }

            return res;
        }



        /// <summary>
        /// Allows one or more properties on a menu entry to be udpated (for instance Action).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the menu entry is homed.
        /// </param>
        /// <param name="pCallHandlerObjectId">
        /// Unique identifier for the call handler that owns the menu entry being edited.
        /// </param>
        /// <param name="pKeyName">
        /// The key name to update (0-9 * or #)
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a menu entry property name and a new value for that property to apply to the entry 
        /// being updated. This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one
        /// property pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateMenuEntry(ConnectionServerRest pConnectionServer, string pCallHandlerObjectId, string pKeyName, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateMenuEntry";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList==null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateMenuEntry";
                return res;
            }

            //both the objectID and key name should be passed here.
            if (string.IsNullOrEmpty(pCallHandlerObjectId) | string.IsNullOrEmpty(pKeyName))
            {
                res.ErrorText = "Empty call handler ObjectId or Key Name passed to UpdateMenuEntry";
                return res;
            }

            string strBody = "<MenuEntry>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</MenuEntry>";

            return pConnectionServer.GetCupiResponse(string.Format("{0}handlers/callhandlers/{1}/menuentries/{2}", pConnectionServer.BaseUrl, 
                pCallHandlerObjectId, pKeyName),MethodType.PUT,strBody,false);
        }

        #endregion

        #region Instance Methods

        /// <summary>
        /// MenuEntry display function - outputs the key name, action and if it's locked 
        /// </summary>
        /// <returns>
        /// String describing the menu entry
        /// </returns>
        public override string ToString()
        {
            return string.Format("Key name={0} Action={1} ({2}), locked={3}", TouchtoneKey,(int)Action,Action.ToString(),Locked);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the menu entry object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the alternate extension object instance.
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
        public WebCallResult RefetchMenuEntryData()
        {
            return GetMenuEntry(this.TouchtoneKey);
        }

        /// <summary>
        /// Builds an instance of an MenuEntry object, filling it with the details of a menu entry idenitified by the 
        /// CallHandlerObjectID and the key name of the menu entry.
        /// This MenuEntry has already been created - you can use this to "re fill" an existing menu entry with possibly
        /// updated information or if you created an "empty" MenuEntry object and now want to populate it.
        /// </summary>
        /// <param name="pKey">
        /// name of the menu entry to be fetched - can be 0-9, # or *
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetMenuEntry(string pKey)
        {
            WebCallResult res;

            if (string.IsNullOrEmpty(pKey))
            {
                res=new WebCallResult();
                res.ErrorText = "Empty menu entry key parameter passed to GetMenuEntry";
                return res;
            }

            string strUrl = string.Format("{0}handlers/callhandlers/{1}/menuentries/{2}", HomeServer.BaseUrl, CallHandlerObjectId, pKey);

            //issue the command to the CUPI interface
            res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(ConnectionServerRest.StripJsonOfObjectWrapper(res.ResponseText, "MenuEntry"), this, 
                    RestTransportFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }

            //all the updates above will flip pending changes into the queue - clear that here.
            this.ClearPendingChanges();
            return res;
        }


        /// <summary>
        /// If the menu entry object has any pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }


        /// <summary>
        /// Allows one or more properties on a menu entry to be udpated (for instance action).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update(bool pRefetchDataAfterSuccessfulUpdate = false)
        {
            WebCallResult res;

            //check if the menu entry intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for menu entry:{0}, objectid=[{1}]",
                                              this, this.ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateMenuEntry(HomeServer, CallHandlerObjectId, TouchtoneKey, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
                if (pRefetchDataAfterSuccessfulUpdate)
                {
                    return RefetchMenuEntryData();
                }
            }

            return res;
        }

        #endregion
    }
}
