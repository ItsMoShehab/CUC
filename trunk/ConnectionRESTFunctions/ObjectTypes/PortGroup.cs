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
    /// Read only class for fetching port group details - get them all in a generic list or fetch a specific one by ObjectId
    /// </summary>
    public class PortGroup
    {

        #region Fields and Properties
        //reference to the ConnectionServer object used to create this user instance.
        private readonly ConnectionServer _homeServer;

        public string DisplayName { get; set; }
        public string ObjectId { get; set; }

        public string MediaPortGroupTemplateObjectId { get; set; }
        public string MediaSwitchObjectId { get; set; }
        public int TelephonyIntegrationMethodEnum { get; set; }
        public bool EnableMWI { get; set; }
        public bool EnableAGC { get; set; }
        public bool CcmDoAutoFailback { get; set; }
        public string MwiOnCode { get; set; }
        public string MwiOffCode { get; set; }
        public int MwiRetryCountOnSuccess { get; set; }
        public int MwiRetryIntervalOnSuccessMs { get; set; }
        public string SkinnyDevicePrefix { get; set; }
        public int MwiMinRequestIntervalMs { get; set; }
        public int OutgoingGuardTimeMs { get; set; }
        public int OutgoingPostDialDelayMs { get; set; }
        public int OutgoingPreDialDelayMs { get; set; }
        public int DelayBeforeOpeningMs { get; set; }
        public int DtmfDialInterDigitDelayMs { get; set; }
        public int MwiMaxConcurrentRequests { get; set; }
        public string MediaSwitchDisplayName { get; set; }
        public int PortCount { get; set; }
        public bool SipDoSRTP { get; set; }
        public int SipTLSModeEnum { get; set; }
        public int ResetStatusEnum { get; set; }
        public int RecordingDTMFClipMs { get; set; }
        public int RecordingToneExtraClipMs { get; set; }
        public bool NoiseFreeEnable { get; set; }

        #endregion

        
        #region Constructors

        /// <summary>
        /// Constructor for the PortGroup class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        /// <param name="pObjectId">
        /// Optional - if passed in the specifics of the switch identified by this GUID is fetched and the properties are filled in.
        /// </param>
        public PortGroup(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer==null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to PortGroup construtor");
            }

            _homeServer = pConnectionServer;
            ObjectId = pObjectId;

            //if no objectId is passed in just create an empty version of the class - used for constructing lists from XML fetches.
            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetPortGroup(pObjectId);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Port group not found in PortGroup constructor using ObjectId={0}\n\r{1}"
                                                 , pObjectId, res.ErrorText));
            }
        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the port group
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the port group object in "name=value" format - each pair is on its
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
        /// Fetch details for a single port group by ObjectId and populate the local instance's properties with it
        /// </summary>
        /// <param name="pObjectId">
        /// Unique identifier for port group to fetch
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        private WebCallResult GetPortGroup(string pObjectId)
        {
            string strUrl = string.Format("{0}portgroups/{1}", _homeServer.BaseUrl, pObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, _homeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XMLElement == null || res.XMLElement.HasElements == false)
            {
                res.Success = false;
                return res;
            }

            //load all of the elements returned into the class object properties
            foreach (XElement oElement in res.XMLElement.Elements())
            {
                _homeServer.SafeXMLFetch(this, oElement);
            }

            return res;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all port groups and resturns them as a generic list of PortGroup objects.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that is being queried
        /// </param>
        /// <param name="pPortGroups">
        /// Out parameter that is used to return the list of PortGroup objects defined on Connection - there may be none - this list can be 
        /// returned empty.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPortGroups(ConnectionServer pConnectionServer, out List<PortGroup> pPortGroups)
        {
            WebCallResult res;
            pPortGroups = new List<PortGroup>();

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetPortGroups";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + "portgroups";

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty, that's legal
            if (res.XMLElement == null || res.XMLElement.HasElements == false)
            {
                pPortGroups = new List<PortGroup>();
                return res;
            }

            pPortGroups = GetPortGroupsFromXElements(pConnectionServer, res.XMLElement);
            return res;
        }


        //Helper function to take an XML blob returned from the REST interface for PortGroup returned and convert it into an generic
        //list of PortGroup class objects.  
        private static List<PortGroup> GetPortGroupsFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {

            List<PortGroup> oPortGroupList = new List<PortGroup>();

            //Use LINQ to XML to create a list of PortGroup objects in a single statement. 
            var portGroups = from e in pXElement.Elements()
                                                where e.Name.LocalName == "PortGroup"
                                                select e;

            //for each object returned in the list from the XML, construct a class object using the elements associated with that 
            //object.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlPortGroup in portGroups)
            {
                PortGroup oPortGroup = new PortGroup(pConnectionServer);
                foreach (XElement oElement in oXmlPortGroup.Elements())
                {
                    //adds the XML property to the object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXMLFetch(oPortGroup, oElement);
                }

                //add the fully populated object to the list that will be returned to the calling routine.
                oPortGroupList.Add(oPortGroup);
            }
            
            return oPortGroupList;
        }

    
        #endregion

    }
}
