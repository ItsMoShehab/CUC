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
    /// Class for adding, editing, deleting and fetching/searching for contact objects in Connection.
    /// </summary>
    public class Contact :IUnityDisplayInterface
    {
        
        #region Constructors and Destructors


        /// <summary>
        /// Creates a new instance of the Contact class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this contact.  
        /// If you pass the pObjectID or pAlias parameter the contact is automatically filled with data for that contact from the server.  
        /// If neither are passed an empty instance of the Contact class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the contact being created.
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the contact on the home server provided.  If no ObjectId is passed then an empty instance of the CallHander
        /// class is returned instead.
        /// </param>
        /// <param name="pAlias">
        /// 
        /// </param>
        public Contact(ConnectionServerRest pConnectionServer, string pObjectId = "", string pAlias = ""):this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to Contact constructor.");
            }

            HomeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or display name then go load that contact up, otherwise just return an empty instance.
            if ((pObjectId.Length == 0) & (pAlias.Length == 0)) return;

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetContact(pObjectId, pAlias);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Contact not found in Contact constructor using ObjectId={0} or Alias={1}\n\r{2}"
                                 , pObjectId,pAlias, res.ErrorText));
            }
        }

        /// <summary>
        /// Generic constructor for JSON parsing libraries
        /// </summary>
        public Contact()
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

        //reference to the ConnectionServer object used to create this contact instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region Contact Properties

        public DateTime CreationTime { get; set; }
        
        public string Alias { get; set; }

        private string _altFirstName;
        public string AltFirstName
        {
            get { return _altFirstName; }
            set
            {
                _altFirstName = value;
                _changedPropList.Add("AltFirstName", value);
            }
        }

        private string _altLastName;
        public string AltLastName
        {
            get { return _altLastName; }
            set
            {
                _altLastName = value;
                _changedPropList.Add("AltLastName", value);
            }
        }

        private bool _autoCreateCallHandler;
        public bool AutoCreateCallHandler
        {
            get { return _autoCreateCallHandler; }
            set
            {
                _autoCreateCallHandler = value;
                _changedPropList.Add("AutoCreateCallHandler", value);
            }
        }            

        public string CallHandlerObjectId { get; set; }

        public int ContactType { get; set; }

        public string DeliveryLocationObjectId { get; set; }

        public string DeliveryLocationPartitionObjectId { get; set; }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                _changedPropList.Add("DisplayName", value);
            }
        }

        public bool DisableOutbound { get; set; }

        public string DtmfNameFirstLast { get; set; }

        public string DtmfNameLKastFirst { get; set; }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                _changedPropList.Add("FirstName", value);
            }
        }

        public bool IsAddressable { get; set; }

        //you cannot change the is template setting via CUPI
        [JsonProperty]
        public bool IsTemplate { get; private set; }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                _changedPropList.Add("LastName", value);
            }
        }

        private bool _listInDirectory;
        public bool ListInDirectory
        {
            get { return _listInDirectory; }
            set
            {
                _listInDirectory = value;
                _changedPropList.Add("ListInDirectory", value);
            }
        }

        //you cannot change the location objectID
        [JsonProperty]
        public string LocationObjectId { get; private set; }

        //you cannot edit the ObjectId value
        [JsonProperty]
        public string ObjectId { get; private set; }

        private string _partitionObjectId;
        public string PartitionObjectId
        {
            get { return _partitionObjectId; }
            set
            {
                _partitionObjectId = value;
                _changedPropList.Add("PartitionObjectId", value);
            }
        }

        public string RemoteAddress { get; set; }

        private TransferTypes _transferType;
        public TransferTypes TransferType
        {
            get { return _transferType; }
            set
            {
                _transferType = value;
                _changedPropList.Add("TransferType", (int) value);
            }
        }

        private int _transferRings;
        public int TransferRings
        {
            get { return _transferRings; }
            set
            {
                _transferRings = value;
                _changedPropList.Add("TransferRings", value);
            }
        }

        private bool _transferEnabled;
        public bool TransferEnabled
        {
            get { return _transferEnabled; }
            set
            {
                _transferEnabled = value;
                _changedPropList.Add("TransferEnabled", value);
            }
        }

        private string _transferExtension;
        public string TransferExtension
        {
            get { return _transferExtension; }
            set
            {
                _transferExtension = value;
                _changedPropList.Add("TransferExtension", value);
            }
        }

        //voice name needs to be edited via a special interface
        [JsonProperty]
        public string VoiceName { get;  private set; }


        #endregion


        #region Static Methods


        /// <summary>
        /// This function allows for a GET of contacts from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(displayname startswith ab)"
        /// sort: "sort=(displayname asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the contacts are being fetched from.
        /// </param>
        /// <param name="pContacts">
        /// The list of contacts returned from the CUPI call (if any) is returned as a generic list of Contact class instances via this out param.  
        /// If no contacts are  found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetContacts(ConnectionServerRest pConnectionServer, out List<Contact> pContacts, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pContacts = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetContacts";
                return res;
            }

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "contacts", pClauses);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's an error
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                pContacts = new List<Contact>();
                return res;
            }

            //not an error, just return an empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                pContacts=new List<Contact>();
                return res;
            }

            pContacts = pConnectionServer.GetObjectsFromJson<Contact>(res.ResponseText);

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pContacts)
            {
                oObject.HomeServer = pConnectionServer;
                
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// This function allows for a GET of contacts from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(displayname startswith ab)"
        /// sort: "sort=(displayname asc)"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the contacts are being fetched from.
        /// </param>
        /// <param name="pContacts">
        /// The list of contacts returned from the CUPI call (if any) is returned as a generic list of Contact class instances via this out param.  
        /// If no contacts are  found NULL is returned for this parameter.
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

        public static WebCallResult GetContacts(ConnectionServerRest pConnectionServer, out List<Contact> pContacts,int pPageNumber=1, 
            int pRowsPerPage=20,params string[] pClauses)
        {
            //tack on the paging items to the parameters list
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

            return GetContacts(pConnectionServer, out pContacts, temp.ToArray());
        }

        /// <summary>
        /// returns a single Contact object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the contact is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the contact to load
        /// </param>
        /// <param name="pContact">
        /// The out param that the filled out instance of the Contact class is returned on.
        /// </param>
        /// <param name="pAlias">
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetContact(out Contact pContact,ConnectionServerRest pConnectionServer, string pObjectId="", string pAlias="")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pContact = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetContact";
                return res;
            }

            //you need an objectID and/or an alias - both being blank is not acceptable
            if ((pObjectId.Length == 0) & (pAlias.Length == 0))
            {
                res.ErrorText = "Empty objectId and alias name passed to GetContact";
                return res;
            }

            //create a new Contact instance passing the ObjectId (or alias) which fills out the data automatically
            try
            {
                pContact = new Contact(pConnectionServer, pObjectId, pAlias);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch contact in GetContact:" + ex.Message;
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new contact on the Connection server directory.  The alias name must be provided but the 
        /// first/last/display names can be blank.  The alias a template to use when creating the new contact is required, however other contact 
        /// properties and their values may be passed in via the ConnectonPropertyList structure.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the contact is being added.
        /// </param>
        /// <param name="pContactTemplateAlias">
        /// The alias of a contact template on Connection - this provides important details such as dial partition assignment.  It's
        /// required and must exist on the server or the user creation will fail.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new contact.  This must be unique against all contacts on the local Connection server.
        /// </param>
        /// <param name="pAlias"></param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a contacts property name and a new value for that property to apply to the contact being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <param name="pFirstName"></param>
        /// <param name="pLastName"></param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddContact(ConnectionServerRest pConnectionServer,
                                                    string pContactTemplateAlias, 
                                                    string pDisplayName, 
                                                    string pFirstName, 
                                                    string pLastName,
                                                    string pAlias,
                                                    ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddContact";
                return res;
            }

            //make sure that something is passed in for the 2 required params
            if (String.IsNullOrEmpty(pContactTemplateAlias) || string.IsNullOrEmpty(pAlias))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddContact on ConnectionServer.cs";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            //cheat here a bit and simply add the alias and extension values to the proplist where it can be tossed into the body later.
            pPropList.Add("Alias", pAlias);

            if (!string.IsNullOrEmpty(pDisplayName))
                pPropList.Add("DisplayName", pDisplayName);

            if (!string.IsNullOrEmpty(pFirstName))
                pPropList.Add("FirstName", pFirstName);

            if (!string.IsNullOrEmpty(pLastName))
                pPropList.Add("LastName", pLastName);

            string strBody = "<Contact>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Contact>";

            res = pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "contacts?templateAlias=" + pContactTemplateAlias, 
                    MethodType.POST,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/contacts/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/contacts/", "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new contact on the Connection server directory.  The alias must be provided but the 
        /// first/last/display names can be blank.  The alias a template to use when creating the new contact is required, however other contact 
        /// properties and their values may be passed in via the ConnectonPropertyList structure.
        /// </summary>
        /// <remarks>
        /// This is an alternateive AddContact that passes back a Contact object with the newly created contact filled out in it if the 
        /// add goes through.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the contact is being added.
        /// </param>
        /// <param name="pContactTemplateAlias">
        /// The alias of a contact template on Connection - this provides important details such as dial partition assignment.  It's
        /// required and must exist on the server or the user creation will fail.
        /// </param>
        /// <param name="pDisplayName">
        /// Display name to be used for the new contact.  This must be unique against all contats on the local Connection server.
        /// </param>
        /// <param name="pAlias"></param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a contacs property name and a new value for that property to apply to the contact being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null here.
        /// </param>
        /// <param name="pFirstName"></param>
        /// <param name="pLastName"></param>
        /// <param name="oContact"></param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddContact(ConnectionServerRest pConnectionServer, 
                                                    string pContactTemplateAlias, 
                                                    string pDisplayName, 
                                                    string pFirstName, 
                                                    string pLastName,
                                                    string pAlias,
                                                    ConnectionPropertyList pPropList,
                                                    out Contact oContact)
           {
            oContact = null;

               WebCallResult res = AddContact(pConnectionServer, pContactTemplateAlias, pDisplayName,pFirstName,pLastName,pAlias, pPropList);

               //if the create goes through, fetch the contact as an object and return it.
               if (res.Success)
               {
                   res = GetContact(out oContact, pConnectionServer, res.ReturnedObjectId);
               }

               return res;

           }


        /// <summary>
        /// DELETE a contact from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the contact is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the contact in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteContact(ConnectionServerRest pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteContact";
                return res;
            }

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "contacts/" + pObjectId, MethodType.DELETE, "");
        }


        /// <summary>
        /// Allows one or more properties on a contact to be udpated (for instance display name/DTMFAccessID etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the contact is homed.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the contact to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a contact property name and a new value for that property to apply to the contact being updated.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one property
        /// pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateContact(ConnectionServerRest pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateContact";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList==null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateContact on ConnectonServer.cs";
                return res;
            }

            string strBody = "<Contact>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Contact>";

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "contacts/" + pObjectId,
                                            MethodType.PUT,strBody,false);

        }


        /// <summary>
        /// Fetches the WAV file for a contact's voice name and stores it on the Windows file system at the file location specified.  If the contact does 
        /// not have a voice name recorded, the WebcallResult structure returns false in the success proeprty and notes the contact has no voice name in 
        /// the error text.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the contact is homed.
        /// </param>
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the contact's voice name at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the contact.  
        /// </param>
        /// <param name="pConnectionWavFileName">
        /// The the Connection stream file name is already known it can be passed in here and the contact lookup does not need to take place.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetContactVoiceName(ConnectionServerRest pConnectionServer, string pTargetLocalFilePath, string pObjectId, string pConnectionWavFileName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetContactVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pTargetLocalFilePath) || (Directory.GetParent(pTargetLocalFilePath).Exists == false))
            {
                res.ErrorText = "Invalid local file path passed to GetContactVoiceName: " + pTargetLocalFilePath;
                return res;
            }

            //if the WAV file name itself is passed in that's all we need, otherwise we need to go do a contact fetch with the ObjectId 
            //and pull the VoiceName wav file name from there (if it's present).
            //fetch the contact info which has the VoiceName property on it
            if (string.IsNullOrEmpty(pConnectionWavFileName))
            {
                Contact oContact;

                try
                {
                    oContact = new Contact(pConnectionServer, pObjectId);
                }
                catch (UnityConnectionRestException ex)
                {
                    return ex.WebCallResult;
                }
                catch (Exception ex)
                {
                    res.ErrorText = string.Format("Error fetching contact in GetContactVoiceName with objectID{0}\n{1}", pObjectId, ex.Message);
                    return res;
                }

                //the property will be null if no voice name is recorded for the contact.
                if (string.IsNullOrEmpty(oContact.VoiceName))
                {
                    res = new WebCallResult();
                    res.Success = false;
                    res.ErrorText = "No voice named recorded for contact.";
                    return res;
                }

                pConnectionWavFileName = oContact.VoiceName;
            }
            //fetch the WAV file
            return pConnectionServer.DownloadWavFile(pTargetLocalFilePath,pConnectionWavFileName);
        }


        /// <summary>
        /// Uploads a WAV file indicated as a voice name for the target contact referenced by the pObjectID value.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the contact is homed.
        /// </param>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as a voice name for the contact referenced.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the contact to upload the voice name WAV file for.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// If passed as TRUE the routine will attempt to convert the target WAV file into raw PCM first before uploading it to the Connection
        /// server.  A failure to convert will be considered a failed upload attempt and false is returned.  This value defaults to FALSE meaning
        /// the file will attempt to be uploaded as is.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetContactVoiceName(ConnectionServerRest pConnectionServer, string pSourceLocalFilePath, string pObjectId, bool pConvertToPcmFirst = false)
        {
            string strConvertedWavFilePath = "";
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetContactVoiceName";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pSourceLocalFilePath) || (File.Exists(pSourceLocalFilePath) == false))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid local file path passed to SetContactVoiceName: " + pSourceLocalFilePath;
                return res;
            }

            //if the user wants to try and rip the WAV file into PCM 16/8/1 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pSourceLocalFilePath);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into PCM format in SetContactVoiceName.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted PCM WAV file path not found in SetContactVoiceName: " + strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pSourceLocalFilePath = strConvertedWavFilePath;
            }

            //use the 8.5 and later voice name formatting here which simplifies things a great deal.
            string strResourcePath = string.Format(@"{0}contacts/{1}/voicename", pConnectionServer.BaseUrl, pObjectId);

            //upload the WAV file to the server.
            res = pConnectionServer.UploadWavFile(strResourcePath, pSourceLocalFilePath);

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
                    res.ErrorText = "(warning) failed to delete temporary PCM wav file in SetContactVoiceName:" + ex.Message;
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
        /// ObjectId of the contact to apply the stream file to the voice name for.
        /// </param>
        /// <param name="pStreamFileResourceName" type="string">
        ///  the unique identifier (usually GUID.wav type construction) for the recording stream to be assigned.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult SetContactVoiceNameToStreamFile(ConnectionServerRest pConnectionServer, string pObjectId,
                                                     string pStreamFileResourceName)
        {
            WebCallResult res = new WebCallResult();

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to SetContactVoiceNameToStreamFile";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to SetContactVoiceNameToStreamFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pStreamFileResourceName))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Invalid stream file resource id passed to SetContactVoiceNameToStreamFile";
                return res;
            }

            //construct the full URL to call for uploading the voice name file
            string strUrl = string.Format(@"{0}contacts/{1}/voicename", pConnectionServer.BaseUrl, pObjectId);

            Dictionary<string, string> oParams = new Dictionary<string, string>();

            oParams.Add("op", "RECORD");
            oParams.Add("ResourceType", "STREAM");
            oParams.Add("resourceId", pStreamFileResourceName);
            oParams.Add("lastResult", "0");
            oParams.Add("speed", "100");
            oParams.Add("volume", "100");
            oParams.Add("startPosition", "0");

            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.PUT, oParams);

            return res;
        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays the alias and display name 
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} [{1}]", this.Alias, this.DisplayName);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the contact object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the contact object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix="")
        {
            StringBuilder strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n",pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
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
        public WebCallResult RefetchContactData()
        {
            return GetContact(this.ObjectId);
        }


        //Fills the current instance of Contact in with properties fetched from the server.
        private WebCallResult GetContact(string pObjectId, string pAlias="")
        {
            string strObjectId = pObjectId;
            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdByAlias(pAlias);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult
                        {
                            Success = false,
                            ErrorText = "No value for ObjectId or Alias passed to GetContact."
                        };
                }
            }

             string strUrl = string.Format("{0}contacts/{1}", HomeServer.BaseUrl, strObjectId);
            
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

            //failed to find contact
            if (string.IsNullOrEmpty(this.Alias))
            {
                res.Success = false;
                res.ErrorText = "Failed to find contact by alias=" + pAlias + " or objectId=" + pObjectId;
            }

            //all the updates above will flip pending changes into the queue - clear that here.
            this.ClearPendingChanges();

            return res;
        }

        /// <summary>
        /// Fetch the ObjectId of a contact by it's alias.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pAlias">
        /// Alias of contact to find
        /// </param>
        /// <returns>
        /// ObjectId of contact if found or empty string if not.
        /// </returns>
        private string GetObjectIdByAlias(string pAlias)
        {
            string strUrl = string.Format("{0}contacts/?query=(Alias is {1})", HomeServer.BaseUrl, pAlias.UriSafe());

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET,  "");

            if (res.Success == false || res.TotalObjectCount==0)
            {
                return "";
            }

            List<Contact> oContacts = HomeServer.GetObjectsFromJson<Contact>(res.ResponseText);

            foreach (var oContact in oContacts)
            {
                if (oContact.Alias.Equals(pAlias, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oContact.ObjectId;
                }
            }

            return "";
        }

        /// <summary>
        /// If the contact object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
          	_changedPropList.Clear();
        }


        /// <summary>
        /// Allows one or more properties on a contact to be udpated (for instance display name etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;
            
            //check if the contact intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
              	res=new WebCallResult();
                res.Success = false;
                res.ErrorText =string.Format("Update called but there are no pending changes for contact {0}",this);
                return res;
            }

            //just call the static method with the info from the instance 
            res=UpdateContact(HomeServer, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
              	_changedPropList.Clear();
            }

            return res;
        }

        /// <summary>
        /// DELETE a contact from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeleteContact(HomeServer, ObjectId);
        }


        /// <summary>
        /// Uploads a WAV file indicated as a voice name for the target contact
        /// </summary>
        /// <param name="pSourceLocalFilePath">
        /// Full path on the local file system pointing to a WAV file to be uploaded as a voice name for the contact referenced.
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
            return SetContactVoiceName(HomeServer, pSourceLocalFilePath, ObjectId, pConvertToPcmFirst);
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
            return SetContactVoiceNameToStreamFile(HomeServer, ObjectId, pStreamFileResourceName);
        }

        /// <summary>
        /// Fetches the WAV file for a contact's voice name and stores it on the Windows file system at the file location specified.  If the contact does 
        /// not have a voice name recorded, the WebcallResult structure returns false in the success proeprty and notes the contact has no voice name in 
        /// the error text.
        /// </summary>
        /// <param name="pTargetLocalFilePath">
        /// Full path to the location to store the WAV file of the contact's voice name at on the local file system.  If a file already exists in the 
        /// location, it will be deleted.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetVoiceName(string pTargetLocalFilePath)
        {
            //just call the static method with the info from the instance of this object
            return GetContactVoiceName(HomeServer, pTargetLocalFilePath, ObjectId, VoiceName);
        }

        #endregion

    }
}
