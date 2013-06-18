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
    /// The AlternateExtension class contains all the properties associated with an Alternate Extension in Unity Connection that can be fetched 
    /// via the CUPI interface.  This class also contains a number of static and instance methods for finding, deleting, editing and listing
    /// alternate extensions.
    /// </summary>
    public class AlternateExtension : IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// General constructor used in the JSON parsing routines.
        /// </summary>
        public AlternateExtension()
        {
            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();
        }

        /// <summary>
        /// Creates a new instance of the AlternateExtension class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this extension.  
        /// If you pass the pObjectID parameter the extension is automatically filled with data for that exension from the server.  If no pObjectID is passed an
        /// empty instance of the AlternateExtension class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the extension being created.
        /// </param>
        /// <param name="pUserObjectId">
        /// Unique ID for the user that owns the alternate extension - required.
        ///  </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the extension on the home server provided.  If no ObjectId is passed then an empty instance of the 
        /// AlternateExtension class is returned instead.
        /// </param>
        public AlternateExtension(ConnectionServerRest pConnectionServer, string pUserObjectId, string pObjectId = ""):this()
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to AlternateExtension constructor.");
            }

            if (String.IsNullOrEmpty(pUserObjectId))
            {
                throw new ArgumentException("Invalid UserObjectID passed to AlternateExtension constructor.");
            }

            HomeServer = pConnectionServer;

            //remember the objectID of the owner of the alternate extension as the CUPI interface requires this in the URL construction
            //for operations editing/deleting them.
            UserObjectId = pUserObjectId;

            //if the user passed in a specific ObjectId then go load that extenson up, otherwise just return an empty instance.
            if (pObjectId.Length == 0) return;

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetAlternateExtension(pObjectId);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res, string.Format("Alternate Extension not found in AlternateExtension constructor using " +
                                                                          "ObjectId={0}\n\r{1}", pObjectId, res.ErrorText));
            }
        }

        #endregion


        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DtmfAccessId; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        //reference to the ConnectionServer object used to create this Alternate Extension instance.
        public ConnectionServerRest HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region Alternate Extension Properties

        private int _idIndex;

        public int IdIndex
        {
            get { return _idIndex; }
            set
            {
                _changedPropList.Add("IdIndex", value);
                _idIndex = value;
            }
        }

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

        private string _locationObjectId;

        public string LocationObjectId
        {
            get { return _locationObjectId; }
            set
            {
                _changedPropList.Add("LocationObjectId", value);
                _locationObjectId = value;
            }
        }

        //can't change the ObjectId value of a  standing object
        [JsonProperty]
        public string ObjectId { get; private set; }

        //owner of the alternate extension - set only in constructor
        public string UserObjectId { get; private set; }

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

        #endregion


        #region Static Method

        /// <summary>
        /// Fetches an alternate extension object filled with all the properties for a specific alternate extension identified with the ObjectId
        /// of the user that owns it and the ObjectId of the alternate extension itself.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that the alternate extension is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// The objectID of the user that owns the alternate extension to be fetched.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the alternate extension
        /// </param>
        /// <param name="pAlternateExtension">
        /// The out parameter that the instance of the AlternateExtension class filled in with the details of the fetched alternate extension is
        /// passed back on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetAlternateExtension(ConnectionServerRest pConnectionServer, 
                                                        string pUserObjectId, 
                                                        string pObjectId, 
                                                        out  AlternateExtension pAlternateExtension)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pAlternateExtension = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetAlternateExtension";
                return res;
            }

            //create a new Alternate Extension instance passing the ObjectId which fills out the data automatically
            try
            {
                pAlternateExtension = new AlternateExtension(pConnectionServer, pUserObjectId, pObjectId);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch alternate extension in GetAlternateExtension:" + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Returns all the alternate extensions for a user.  If there are no alternate extensions a null list is returned.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the alternate extensions are being fetched from.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID identifying the user that owns the alternate extensions to be fetched.
        /// </param>
        /// <param name="pAlternateExtensions">
        /// The list of alternate extension objects are returned using this out parameter.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetAlternateExtensions(ConnectionServerRest pConnectionServer, 
                                                            string pUserObjectId,
                                                           out List<AlternateExtension> pAlternateExtensions)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pAlternateExtensions = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetAlternateExtensions";
                return res;
            }

            string strUrl = string.Format("{0}users/{1}/alternateextensions", pConnectionServer.BaseUrl, pUserObjectId);

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - return true here along with an empty list.
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                pAlternateExtensions = new List<AlternateExtension>();
                return res;
            }

            //not an error, just no extensions returned in query - return empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 10)
            {
                pAlternateExtensions = new List<AlternateExtension>();
                return res;
            }

            pAlternateExtensions = pConnectionServer.GetObjectsFromJson<AlternateExtension>(res.ResponseText);

            if (pAlternateExtensions == null)
            {
                res.Success = false;
                res.ErrorText = "Unable to parse response body into alternate extensions list:" + res.ResponseText;
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pAlternateExtensions)
            {
                oObject.ClearPendingChanges();
                oObject.HomeServer = pConnectionServer;
                oObject.UserObjectId = pUserObjectId;
            }

            return res;
        }



        /// <summary>
        /// Adds a new alternate extension at the index passed in for the user identified with the UserObjectID parameter.  If the extension
        /// conflicts with another in the partition or that alternate extension "slot" is already in use, Connection will return an error.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that houses the user that owns the new alternate extension to be added.
        /// </param>
        /// <param name="pUserObjectId">
        /// GUID that identifies the user to add the alternate extension for.
        /// </param>
        /// <param name="pIdIndex">
        /// The alternate extension id to add the alternate extension to.  1-10 are administrator added extensions, 11 through 20 are user added.
        /// </param>
        /// <param name="pExtension">
        /// The DMTFAccessID (extension) to add.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddAlternateExtension(ConnectionServerRest pConnectionServer,
                                                          string pUserObjectId,
                                                          int pIdIndex,
                                                          string pExtension)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddAlternateExtension";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pExtension) || (string.IsNullOrEmpty(pUserObjectId)))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddAlternateExtension";
                return res;
            }

            //1 through 10 is admin added, 11 through 20 is user added.  Different versions of Connection allow for different numbers 
            //in the SA etc... however 20 is currently the max upper bound so we check for that here - the back end CUPI call will fail
            //appropriately.
            if (pIdIndex>20)
            {
                res.ErrorText = "Invalid IDIndex passed to AddAlternateExtension:" + pIdIndex.ToString();
                return res;
            }

            string strBody = "<AlternateExtension>";

            //tack on the property value pair with appropriate tags
            strBody += string.Format("<IdIndex>{0}</IdIndex>", pIdIndex);
            strBody += string.Format("<DtmfAccessId>{0}</DtmfAccessId>", pExtension);

            strBody += "</AlternateExtension>";

            res =
                pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/alternateextensions", 
                pConnectionServer.BaseUrl, pUserObjectId),MethodType.POST,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = @"/vmrest/users/" + pUserObjectId + "/alternateextensions/";
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId =res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// Adds an alternate extension and returns the newly added extension as an instance of the AlternateExtension class via an out param.
        /// You can also add an alternate extension without this optional parameter which saves the extra CUPI fetch to retrieve it.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that houses the user that the alternate extension will be added for.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID identifying the user to add the alternate extension for.
        /// </param>
        /// <param name="pIdIndex">
        /// The alternate extension id to add the alternate extension to.  1-10 are administrator added extensions, 11 through 20 are user added.
        /// </param>
        /// <param name="pExtension">
        /// The DTMFAccessID (extension) to add.
        /// </param>
        /// <param name="pAlternateExtension">
        /// the out param that the instance of the AlternateExtension class filled with the newly added Alternate Extension's datails is passed 
        /// back on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddAlternateExtension(ConnectionServerRest pConnectionServer,
                                                          string pUserObjectId,
                                                          int pIdIndex,
                                                          string pExtension,
                                                          out AlternateExtension pAlternateExtension)
        {
            pAlternateExtension = null;

            WebCallResult res = AddAlternateExtension(pConnectionServer, pUserObjectId,pIdIndex, pExtension);

            //if the create goes through, fetch the alternate extension as an object and return it.
            if (res.Success)
            {
                res = GetAlternateExtension(pConnectionServer, pUserObjectId, res.ReturnedObjectId, out pAlternateExtension);
            }

            return res;
       }


        /// <summary>
        /// DELETE an alternate extension associated with a user.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that houses the user that owns the alternate extension to be removed.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID of the user that owns the alternate extension to be removed.
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the alternate extension to remove
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteAlternateExtension(ConnectionServerRest pConnectionServer, string pUserObjectId, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteAlternateExtension";
                return res;
            }

            return pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/alternateextensions/{2}", pConnectionServer.BaseUrl, pUserObjectId, 
                                                    pObjectId),MethodType.DELETE,"");
        }

        /// <summary>
        /// Allows one or more properties on a extension to be udpated (for instance DTMFAccessID).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the alternate extension is homed.
        /// </param>
        /// <param name="pUserObjectId">
        /// Unique identifier for user that owns the alternate extension being edited.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the alternate extension owned by the user to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a alternate extension property name and a new value for that property to apply to the extension 
        /// being updated. This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one
        /// property pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateAlternateExtension(ConnectionServerRest pConnectionServer, string pUserObjectId, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateAlternateExtension";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList==null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateAlternateExtension on ConnectonServer.cs";
                return res;
            }

            string strBody = "<AlternateExtension>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</AlternateExtension>";

            return pConnectionServer.GetCupiResponse(string.Format("{0}users/{1}/alternateextensions/{2}", pConnectionServer.BaseUrl, pUserObjectId, pObjectId),
                                            MethodType.PUT,strBody,false);

        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// AlternateExtension display function - outputs the extension's ID (0 to 20), the "slot" that the ID represents (admin added alternate extension
        /// #2 for instance) and the DTMFAccessID value for the alternate extension.
        /// </summary>
        /// <returns>
        /// String describing the alternate extension
        /// </returns>
        public override string ToString()
        {
            return string.Format("Extension id={0} [{1}], ext={2}", this.IdIndex, (AlternateExtensionId)this.IdIndex, this.DtmfAccessId);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the call extension object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the alternate extension object instance.
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
        /// Allows one or more properties on an extension to be udpated (for instance display name, DTMFAccessID etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the extension intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for alternate extension:{0}, objectid=[{1}]",
                                              this,this.ObjectId);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateAlternateExtension(HomeServer,UserObjectId, ObjectId, _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
            }

            return res;
        }

        /// <summary>
        /// DELETE an alternate extension from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeleteAlternateExtension(HomeServer,UserObjectId, ObjectId);
        }


        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made externaly this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchAlternateExtensionData()
        {
            return GetAlternateExtension(this.ObjectId);
        }

        /// <summary>
        /// Builds an instance of an AlternateExtension object, filling it with the details of an alternate extension idenitified by the 
        /// UserObjectID and the ObjectId of the alternate extension owned by that user.
        /// This AlternateExtension has already been created - you can use this to "re fill" an existing alternate extension with possibly
        /// updated information or if you created an "empty" AlternateExtension object and now want to populate it.
        /// </summary>
        /// <param name="pObjectId">
        /// The GUID identifying the alternate extension to fetch
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetAlternateExtension(string pObjectId)
        {
            string strUrl = string.Format("{0}users/{1}/alternateextensions/{2}", HomeServer.BaseUrl,UserObjectId, pObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(ConnectionServerRest.StripJsonOfObjectWrapper(res.ResponseText, "AlternateExtension"), this,
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
        /// If the alternate extension object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }

        #endregion


    }
}
