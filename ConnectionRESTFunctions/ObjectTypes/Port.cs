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
    /// Read only class for fetching details about ports - get them all or a specific one based on ObjectId
    /// </summary>
    public class Port
    {
        #region Fields and Properties

        //reference to the ConnectionServer object used to create this user instance.
        private readonly ConnectionServer _homeServer;

        public string DisplayName { get; set; }
        public string ObjectId { get; set; }
        public string MediaPortGroupObjectId { get; set; }
        public int TelephonyIntegrationMethodEnum { get; set; }
        public int SkinnySecurityModeEnum { get; set; }
        public string VmsServerObjectId { get; set; }
        public int HuntOrder { get; set; }
        public bool CapAnswer { get; set; }
        public bool CapNotification { get; set; }
        public bool CapMWI { get; set; }
        public bool CapEnabled { get; set; }
        public bool CapTrapConnection { get; set; }
        public string MediaSwitchDisplayName { get; set; }
        public string MediaSwitchObjectId { get; set; }
        public string MediaPortGroupDisplayName { get; set; }
        public string VmsServerName { get; set; }

        #endregion

           
        #region Constructors

        /// <summary>
        /// Constructor for the Port class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        /// <param name="pObjectId">
        /// Optional - if passed in the specifics of the switch identified by this GUID is fetched and the properties are filled in.
        /// </param>
        public Port(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer==null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to Port construtor");
            }

            _homeServer = pConnectionServer;
            ObjectId = pObjectId;

            //if no objectId is passed in just create an empty version of the class - used for constructing lists from XML fetches.
            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetPort(pObjectId);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Port not found in Port constructor using ObjectId={0}\n\r{1}"
                                                 , pObjectId, res.ErrorText));
            }
        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the port
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the port object in "name=value" format - each pair is on its
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
        /// Fetch details for a single port by ObjectId and populate the local instance's properties with it
        /// </summary>
        /// <param name="pObjectId">
        /// Unique identifier for port to fetch
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        private WebCallResult GetPort(string pObjectId)
        {
            string strUrl = string.Format("{0}ports/{1}", _homeServer.BaseUrl, pObjectId);

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

            //load all of the elements returned into the class object properties
            foreach (XElement oElement in res.XmlElement.Elements())
            {
                _homeServer.SafeXmlFetch(this, oElement);
            }

            return res;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all ports and resturns them as a generic list of Port objects.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that is being queried
        /// </param>
        /// <param name="pPorts">
        /// Out parameter that is used to return the list of Port objects defined on Connection - there may be none - this list can be 
        /// returned empty.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPorts(ConnectionServer pConnectionServer, out List<Port> pPorts)
        {
            WebCallResult res;
            pPorts = new List<Port>();

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetPorts";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + "ports";

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty, that's legal
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                pPorts = new List<Port>();
                return res;
            }

            pPorts = GetPortsFromXElements(pConnectionServer, res.XmlElement);
            return res;
        }


        //Helper function to take an XML blob returned from the REST interface for Port returned and convert it into an generic
        //list of Port class objects.  
        private static List<Port> GetPortsFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {

            List<Port> oPortList = new List<Port>();
            
            //Use LINQ to XML to create a list of PortGroup objects in a single statement. 
            var ports = from e in pXElement.Elements()
                                      where e.Name.LocalName == "Port"
                                      select e;

            //for each object returned in the list from the XML, construct a class object using the elements associated with that 
            //object.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlPort in ports)
            {
                Port oPort = new Port(pConnectionServer);
                foreach (XElement oElement in oXmlPort.Elements())
                {
                    //adds the XML property to the object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXmlFetch(oPort, oElement);
                }

                //add the fully populated object to the list that will be returned to the calling routine.
                oPortList.Add(oPort);
            }

            
            return oPortList;
        }

    
        #endregion

    }
}
