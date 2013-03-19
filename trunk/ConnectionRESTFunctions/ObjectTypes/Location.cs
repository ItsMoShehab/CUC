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
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{
    /// <summary>
    /// The Location class holds data about a Location.
    /// You cannot add/delete or edit locations - only search them - used mostly for finding other servers in a digital Connection network.
    /// </summary>
    public class Location
    {

        #region Fields and Properties

        //reference to the ConnectionServer object used to create this location instance.
        private readonly ConnectionServer _homeServer;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new instance of the Location class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this location.  The Location class contains much less data than the Location class and is, as a result, quicker to fetch and 
        /// load and is used for all list presentations from searches.  
        /// If you pass the pObjectID parameter the location is automatically filled with data for that location from the server.  If no pObjectID is passed an
        /// empty instance of the Location class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the location being created.
        /// </param>
        /// <param name="pObjectId">
        /// Optional parameter for the unique ID of the location on the home server provided.  If no ObjectId is passed then an empty instance of the Location
        /// class is returned instead.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional parameter for fetching a location's data based on display name.  If both the ObjectId and the name are passed, the ObjectId will be used 
        /// for the search.
        /// </param>
        public Location(ConnectionServer pConnectionServer, string pObjectId = "", string pDisplayName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null Connection Server passed to the Location constructor");
            }

            _homeServer = pConnectionServer;

            if (pObjectId.Length == 0 & pDisplayName.Length == 0) return;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetLocation(pObjectId, pDisplayName);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Location not found in Location constructor using HostAddress={0} and/or ObjectId={1}\n\rError={2}"
                                                , pDisplayName, pObjectId,res.ErrorText));
            }
        }

       
        #endregion


        #region Location Properties

        //The names of the properties must match exactly the tags in XML for them including case - the routine that deserializes data from XML into the 
        //objects requires this to match them up.

        public string DtmfAccessId { get; private set; }
        public string DisplayName { get; private set; }
        public int DestinationType { get; private set; }
        public string HostAddress { get; private set; }
        public string ObjectId { get; private set; }
        public string SmtpDomain { get; private set; }
        public int TransferNumberOfRings { get; private set; }
        public int TransferTimeout { get; private set; }
        public bool IsPrimary { get; private set; }
        public string DefaultWaveFormatObjectId { get; private set; }
        public bool IncludeLocations { get; private set; }
        public int KeypadMapId { get; private set; }
        public string DtmfName { get; private set; }
        public int TimeZone { get; private set; }
        public int DefaultLanguage { get; private set; }
        public int DefaultTTSLanguage { get; private set; }
        public int MaxGreetingLength { get; private set; }
        public int MaxContacts { get; private set; }
        public int AgcTargetDb { get; private set; }
        public string SynchronizationUserObjectId { get; private set; }
        public int LastUSNSent { get; private set; }
        public string SystemVersion { get; private set; }
        public string DefaultPartitionObjectId { get; private set; }
        public string DefaultSearchSpaceObjectId { get; private set; }
        public int SmtpUnknownRecipientAction { get; private set; }
        public int LastUSNReceived { get; private set; }
        public bool PushDirectory { get; private set; }
        public bool PullDirectory { get; private set; }
        public int ReplicationSet { get; private set; }
        public string EncryptionKey { get; private set; }
        public bool UseSmartSmtpHost { get; private set; }
        public bool AllowCrossBoxLogin { get; private set; }
        public bool AllowCrossBoxTransfer { get; private set; }
        public int CrossBoxMaxRings { get; private set; }
        public int CrossBoxSendDelay { get; private set; }
        public int CrossBoxResponseTimeout { get; private set; }
        public int LastUSNAck { get; private set; }
        public int ReplicationSetIncoming { get; private set; }
        public int ReplicationSetOutgoing { get; private set; }
        public int PushState { get; private set; }
        public int PullState { get; private set; }
        public int Status { get; private set; }

        #endregion


        #region Static Methods

        /// <summary>
        /// This function allows for a GET of Locations from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(hostaddress startswith ab)"
        /// sort: "sort=(hostaddress asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <remarks>
        /// While this method name does have the plural in it, you can use it for fetching single locations as well.  If searching by
        /// ObjectId just construct a query in the form "query=(ObjectId is {ObjectId})".  This is just as fast as using the URI format of 
        /// "{server name}\vmrest\locations\{ObjectId}" but returns consistently formatted XML code as multiple locations does so the parsing of 
        /// the data to deserialize it into Location objects is consistent.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the Locations are being fetched from.
        /// </param>
        /// <param name="pLocations">
        /// The list of Locations returned from the CUPI call (if any) is returned as a generic list of Location class instances via this out param.  
        /// If no Locations are  found NULL is returned for this parameter.
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(hostaddress startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetLocations(ConnectionServer pConnectionServer, out List<Location> pLocations, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pLocations = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetLocations";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + "locations/connectionlocations";

            //the spaces get "escaped out" in the HTTPFunctions class call at a lower level, don't worry about it here.
            //Tack on all the search/query/page clauses here if any are passed in.  If an empty string is passed in account
            //for it here.
            if (pClauses != null)
            {
                for (int iCounter = 0; iCounter < pClauses.Length; iCounter++)
                {
                    if (string.IsNullOrEmpty(pClauses[iCounter]))
                    {
                        continue;
                    }

                    //if it's the first param seperate the clause from the URL with a ?, otherwise append compound clauses 
                    //seperated by &
                    if (iCounter == 0)
                    {
                        strUrl += "?";
                    }
                    else
                    {
                        strUrl += "&";
                    }
                    strUrl += pClauses[iCounter];
                }
            }
            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty, that's legal
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                pLocations = new List<Location>();
                return res;
            }

            pLocations = GetLocationsFromXElements(pConnectionServer, res.XmlElement);
            return res;

        }


        /// <summary>
        /// returns a single Location object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the Location is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the Location to load
        /// </param>
        /// <param name="pLocation">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(hostaddress startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <param name="pDisplayName">
        /// Optional parameter - since DisplayName is unique for Location you may pass in an empty ObjectId and use an name instead.  If both the 
        /// name and ObjectId are passed, the ObjectId will be used.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetLocation(out Location pLocation, ConnectionServer pConnectionServer, string pObjectId, string pDisplayName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pLocation = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetLocation";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId)&& string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Emtpy ObjectId and HostAddress passed to GetLocation";
                return res;
            }

            //create a new Location instance passing the ObjectId which fills out the data automatically
            try
            {
                pLocation = new Location(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch location in GetLocation:" + ex.Message;
                return res;
            }

            return res;
        }



        //Helper function to take an XML blob returned from the REST interface for a Location (or Locations) return and convert it into an generic
        //list of Location class objects. 
        private static List<Location> GetLocationsFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {
            List<Location> oLocationList = new List<Location>();

            //pulls all the Locations returned in the XML as set of elements using the power of LINQ
            var locations = from e in pXElement.Elements()
                            where e.Name.LocalName == "ConnectionLocation"
                        select e;

            //for each Location returned in the list of Locations from the XML, construct a Location object using the elements associated with that 
            //Location.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlLocation in locations)
            {
                Location oLocation = new Location(pConnectionServer);
                foreach (XElement oElement in oXmlLocation.Elements())
                {
                    //adds the XML property to the Location object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXmlFetch(oLocation, oElement);
                }

                //add the fully populated Location object to the list that will be returned to the calling routine.
                oLocationList.Add(oLocation);
            }

            return oLocationList;
        }


        #endregion


        #region Instance Methods

        //Fills the current instance of Location in with properties fetched from the server.  The fetch uses a query construction instead of the full ObjectId
        //construction which returns less data and is quicker.

        /// <summary>
        /// Diplays the hostaddress, display name and extension of the Location by default.
        /// </summary>
        public override string ToString()
        {
            return String.Format("Location:{0} [{1}] x{2}", this.HostAddress, this.DisplayName, this.DtmfAccessId);
        }


        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchLocationData()
        {
            return GetLocation(this.ObjectId);
        }

        /// <summary>
        /// Helper function to fill in the Location instance with data from a Location by its objectID string or their hostaddress string.
        /// </summary>
        /// <param name="pObjectId"></param>
        /// <param name="pDisplayName"></param>
        /// <returns></returns>
        private WebCallResult GetLocation(string pObjectId, string pDisplayName = "")
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
                            ErrorText = "No location found for host address=" + pDisplayName
                        };
                }
            }

            string strUrl = string.Format("{0}locations/connectionlocations/{1}", _homeServer.BaseUrl, strObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, _homeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                res.Success = false;
                return res;
            }

            //in the case of a single base Location fetch construct, the list of elements is the full list of properties for the Location, but it's nexted in 
            //a "uers" sub element, not at the top level as a full Location fetch is - so we have to go another level deep here.
            //Call the same SafeXMLFetch routine for each to let the full Location class instance "drive" the fetching of data
            //from the XML elements.
            foreach (XElement oElement in res.XmlElement.Elements())
            {
                _homeServer.SafeXmlFetch(this, oElement);
            }

            return res;
        }

        /// <summary>
        /// Fetch the ObjectId of a location by it's name.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pName">
        /// name of the location to find
        /// </param>
        /// <returns>
        /// ObjectId of location if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pName)
        {
            // string strUrl = string.Format("{0}coses/?query=(DisplayName is {1})", _homeServer.BaseUrl, pCosName);
            string strUrl = string.Format("{0}locations/connectionlocations/?query=(DisplayName is {1})", _homeServer.BaseUrl, pName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, _homeServer, "");

            if (res.Success == false)
            {
                return "";
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                return "";
            }

            foreach (var oElement in res.XmlElement.Elements().Elements())
            {
                if (oElement.Name.ToString().Equals("ObjectId"))
                {
                    return oElement.Value;
                }
            }

            return "";
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the Location object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the Location object instance.
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

     
        #endregion

    }
}
