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

namespace Cisco.UnityConnection.RestFunctions.ObjectTypes
{
    public class MessageHandler
    {
        #region Constructors and Destructors

        /// <summary>
        /// Generic constructor for Json parsing
        /// </summary>
        public MessageHandler()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
            
            ClearPendingChanges();
        }

        public MessageHandler(ConnectionServerRest pConnectionServer, string pUserObjectId):this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to MessageHandler constructor.");
            }

            //we must know what user we're associated with.
            if (String.IsNullOrEmpty(pUserObjectId))
            {
                throw new ArgumentException("Invalid UserObjectID passed to MessageHandler constructor.");
            }

            HomeServer = pConnectionServer;

            //remember the objectID of the owner of the message handler as the CUPI interface requires this in the URL construction
            //for operations editing them.
            SubscriberObjectId = pUserObjectId;

            WebCallResult res = GetMessageHandler();

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res, string.Format("Message Handler not found in MessageHandler constructor using " +
                                                                         "UserObjectID={0}, error={1}",pUserObjectId, res.ErrorText));
            }
        }

        #endregion

        #region Fields and Properties

        //reference to the ConnectionServer object used to create this TransferOption instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        //used to review the list of properties to be changed
        public ConnectionPropertyList ChangeList { get { return _changedPropList; } }

        #endregion

        #region MessageHandler Properties

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

        /// <summary>
        /// user owner of this message handler. This cannot be set or changed after creation.
        /// </summary>
        [JsonProperty]
        public string SubscriberObjectId { get; private set; }

        private string _relayAddress;
        public string RelayAddress
        {
            get { return _relayAddress; }
            set
            {
                _changedPropList.Add("RelayAddress", value);
                _relayAddress = value;
            }
        }

        private MessageHandlerAction _voicemailAction;
        public MessageHandlerAction VoicemailAction
        {
            get { return _voicemailAction; }
            set
            {
                _changedPropList.Add("VoicemailAction", (int)value);
                _voicemailAction = value;
            }
        }

        private MessageHandlerAction _emailmailAction;
        public MessageHandlerAction EmailAction
        {
            get { return _emailmailAction; }
            set
            {
                _changedPropList.Add("EmailAction", (int)value);
                _emailmailAction = value;
            }
        }

        private MessageHandlerAction _faxAction;
        public MessageHandlerAction FaxAction
        {
            get { return _faxAction; }
            set
            {
                _changedPropList.Add("FaxAction", (int)value);
                _faxAction = value;
            }
        }

        private MessageHandlerAction _deliveryReceiptAction;
        public MessageHandlerAction DeliveryReceiptAction
        {
            get { return _deliveryReceiptAction; }
            set
            {
                _changedPropList.Add("DeliveryReceiptAction", (int)value);
                _deliveryReceiptAction = value;
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Fetches a message action object filled with all the properties for a specific entry identified with the ObjectId
        /// of the user that owns it.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that the transfer option is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// The objectID of the user that owns the message handler to be fetched.
        /// </param>
        /// <param name="pMessageHandler">
        /// The out parameter that the instance of the MessageHandler class filled in with the details of the fetched entry is
        /// passed back on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMessageHandler(ConnectionServerRest pConnectionServer,
                                                        string pUserObjectId,
                                                        out  MessageHandler pMessageHandler)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pMessageHandler = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetMessageHandler";
                return res;
            }

            if (string.IsNullOrEmpty(pUserObjectId))
            {
                res.ErrorText = "Empty UserObjectId passed to GetMessageHandler";
                return res;
            }

            try
            {
                pMessageHandler = new MessageHandler(pConnectionServer, pUserObjectId);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch message handler in GetMessageHandler:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Allows one or more properties on a message handler to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the object is homed.
        /// </param>
        /// <param name="pUserObjectId">
        /// Unique identifier for user that owns the message handler being updated
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a transfer option property name and a new value for that property to apply to the option 
        /// being updated. This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one
        /// property pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateMessageHandler(ConnectionServerRest pConnectionServer, 
                                                        string pUserObjectId,
                                                        string pObjectId,
                                                        ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateMessageHandler";
                return res;
            }

            if (pPropList==null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateMessageHandler";
                return res;
            }

            string strBody = "<MessageHandler>";

            foreach (var oPair in pPropList)
            {
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</MessageHandler>";

            return pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/messagehandlers/{2}",
                pConnectionServer.BaseUrl, pUserObjectId, pObjectId), MethodType.PUT, strBody, false);

        }

        #endregion
   
        #region Instance Methods

        /// <summary>
        /// MEssageHandler display function 
        /// </summary>
        public override string ToString()
        {
            return string.Format("Message Handler={0}, VoicemailAction={1}, EmailAction{2}, FaxAction={3}, DeliveryReceiptAction={4}, RelayAddress={5}",
                ObjectId,VoicemailAction,EmailAction,FaxAction,DeliveryReceiptAction,RelayAddress);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the transfer option object in "name=value" format - each pair is on its
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
        public WebCallResult RefetchMessageHananderData()
        {
            return GetMessageHandler();
        }


        /// <summary>
        /// Fetches a message handler object filled with all the properties for a specific entry identified with the ObjectId
        /// of the user that owns it
        /// CUPI returns it as a list even though it's a single entry so we have to treat it as though we're fetching a list of 
        /// objects. 
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetMessageHandler()
        {
            string strUrl = string.Format("{0}users/{1}/messagehandlers", HomeServer.BaseUrl, SubscriberObjectId);
            List<MessageHandler> oTemp = new List<MessageHandler>();
            var res = HomeServer.GetCupiResponse(strUrl, MethodType.GET,null);
            if (!res.Success )
            {
                return res;
            }
            if (res.TotalObjectCount != 1)
            {
                res.Success = false;
                res.ErrorText = "Object count returned from messageHandler fetch not equal to 1";
                return res;
            }
            oTemp= HomeServer.GetObjectsFromJson<MessageHandler>(res.ResponseText);
            if (oTemp.Count != 1)
            {
                res.Success = false;
                res.ErrorText = "Objects returned from JSON parse of messageHandler fetch not equal to 1";
                return res;
            }

            var oItem = oTemp.First();

            this.DeliveryReceiptAction = oItem.DeliveryReceiptAction;
            this.EmailAction = oItem.EmailAction;
            this.FaxAction = oItem.FaxAction;
            this.ObjectId = oItem.ObjectId;
            this.RelayAddress = oItem.RelayAddress;
            this.VoicemailAction = oItem.VoicemailAction;


            ClearPendingChanges();
            return res;
        }



        /// <summary>
        /// If the object has any pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }


        /// <summary>
        /// Allows one or more properties on a transfer option to be udpated.  The caller needs to construct a list of property
        /// names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update(bool pRefetchDataAfterSuccessfulUpdate = false)
        {
            WebCallResult res;

            //check if the transfer option intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for transferoption:{0}, objectid=[{1}]",
                                              this, this.ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateMessageHandler(HomeServer, SubscriberObjectId,ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
                if (pRefetchDataAfterSuccessfulUpdate)
                {
                    return RefetchMessageHananderData();
                }
            }

            return res;
        }


        #endregion


    }
}
