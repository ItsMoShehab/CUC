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
    /// The phone system class is used only to provide an interface for user to select a switch from those configured on the Connection server.  You
    /// cannot create/edit/delete phone system settings through the ConnectionCUPIFunctions library. 
    /// </summary>
    public class PhoneSystem
    {
        #region Fields and Properties

        public string DisplayName { get; set; }
        public string ObjectId { get; set; }

        public bool MwiAlwaysUpdate { get; set; }
        public bool MwiPortMemory { get; set; }
        public string CcmAXLPassword { get; set; }
        public bool CallLoopSupervisedTransferDetect { get; set; }
        public bool CallLoopForwardNotificationDetect { get; set; }
        public string CallLoopDTMF { get; set; }
        public int CallLoopGuardTimeMs { get; set; }
        public int PortCount { get; set; }
        public string CcmCtiPassword { get; set; }
        public bool EnablePhoneApplications { get; set; }
        public bool DefaultTRaPSwitch { get; set; }
        public bool MwiForceOff { get; set; }
        public bool RestrictDialUnconditional { get; set; }
        public bool RestrictDialScheduled { get; set; }
        public int RestrictDialStartTime { get; set; }
        public int RestrictDialEndTime { get; set; }
        public bool CallLoopExtensionDetect { get; set; }

        //reference to the ConnectionServer object used to create this user instance.
        private readonly ConnectionServer _homeServer;

        #endregion


        #region Constructors

        /// <summary>
        /// Constructor for the PhoneSystem class
        /// </summary>
        /// <param name="pConnectionServer">
        /// ConnectionServer data is being fetched from.
        /// </param>
        /// <param name="pObjectId">
        /// Optional - if passed in the specifics of the switch identified by this GUID is fetched and the properties are filled in.
        /// </param>
        public PhoneSystem(ConnectionServer pConnectionServer, string pObjectId="")
        {
            if (pConnectionServer==null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to MediaSwitch construtor");
            }

            _homeServer = pConnectionServer;

            //if no objectId is passed in just create an empty version of the class - used for constructing lists from XML fetches.
            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetPhoneSystem(pObjectId);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Phone system not found in PhoneSystem constructor using ObjectId={0}\n\r{1}"
                                                 , pObjectId, res.ErrorText));
            }
        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the phone system
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the object in "name=value" format - each pair is on its
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
        /// Fetch details for a single phone system by ObjectId and populate the local instance's properties with it
        /// </summary>
        /// <param name="pObjectId">
        /// Unique identifier for phone system to fetch
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        private WebCallResult GetPhoneSystem(string pObjectId)
        {
            string strUrl = string.Format("{0}phonesystems/{1}", _homeServer.BaseUrl, pObjectId);

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
        /// Gets the list of all phone systems and resturns them as a generic list of PhoneSystem objects.  This
        /// list can be used for providing drop down list selection for user creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the phone systems should be pulled from
        /// </param>
        /// <param name="pPhoneSystems">
        /// Out parameter that is used to return the list of PhoneSystem objects defined on Connection - there must be at least one.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPhoneSystems(ConnectionServer pConnectionServer, out List<PhoneSystem> pPhoneSystems)
        {
            WebCallResult res;
            pPhoneSystems = null;

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetPhoneSystems";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + "phonesystems";

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty (no phone systems is legal)
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                pPhoneSystems = new List<PhoneSystem>();
                return res;
            }

            pPhoneSystems = GetPhoneSystemsFromXElements(pConnectionServer, res.XmlElement);
            return res;
        }


        //Helper function to take an XML blob returned from the REST interface for PhoneSystems returned and convert it into an generic
        //list of PhoneSystem class objects.  
        private static List<PhoneSystem> GetPhoneSystemsFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to GetPhoneSystemsFromXElements");
            }

            List<PhoneSystem> oPhoneSystems = new List<PhoneSystem>();

            //Use LINQ to XML to create a list of PhoneSystem objects in a single statement.  We're only interested in 2 properties for phone systems
            //here - they should always be present but protect from missing properties anyway.
            var phoneSystems = from e in pXElement.Elements()
                                                    where e.Name.LocalName == "PhoneSystem"
                                                    select e;

            //for each object returned in the list from the XML, construct a class object using the elements associated with that 
            //object.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlPhoneSystem in phoneSystems)
            {
                PhoneSystem oPhoneSystem = new PhoneSystem(pConnectionServer);
                foreach (XElement oElement in oXmlPhoneSystem.Elements())
                {
                    //adds the XML property to the UserBase object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXmlFetch(oPhoneSystem, oElement);
                }

                //add the fully populated UserBase object to the list that will be returned to the calling routine.
                oPhoneSystems.Add(oPhoneSystem);
            }

            return oPhoneSystems;
        }

    
        #endregion

    }
}
