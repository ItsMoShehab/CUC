using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{
    /// <summary>
    /// Read only class for fetching and listing notification templates - these are used when creating HTML notification devices
    /// in Connection 9.0 and later systems.
    /// </summary>
    public class NotificationTemplate
    {
        #region Fields and Properties

        public string NotificationTemplateID { get; set; }
        public string NotificationTemplateName { get; set; }

        private ConnectionServer _homeServer;

        #endregion


        #region Constructors

        public NotificationTemplate(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer==null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to NotificationTemplate construtor");
            }

            _homeServer = pConnectionServer;

            //if no objectId is passed in just create an empty version of the class - used for constructing lists from XML fetches.
            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetNotificationTemplate(pObjectId);

            if (res.Success == false)
            {
                throw new Exception(string.Format("Notification template not found in NotificationTemplate constructor using ObjectId={0}\n\r{1}"
                                                 , pObjectId, res.ErrorText));
            }
        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the name and ID of the template
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", NotificationTemplateName, NotificationTemplateID);
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
        /// Fetch details for a single notification template by ObjectId and populate the local instance's properties with it
        /// </summary>
        /// <param name="pObjectId">
        /// Unique identifier for notification template to fetch
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class with details of the fetch results.
        /// </returns>
        private WebCallResult GetNotificationTemplate(string pObjectId)
        {
            string strUrl = string.Format("{0}notificationtemplates/{1}", _homeServer.BaseUrl, pObjectId);

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
        /// Gets the list of all notification templates and resturns them as a generic list of NotificationTemplate objects.  This
        /// list can be used for providing drop down list selection for user creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the templates should be pulled from
        /// </param>
        /// <param name="pTemplates">
        /// Out parameter that is used to return the list of NotificationTemplate objects defined on Connection - 
        /// there must be at least one.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetNotificationTemplates(ConnectionServer pConnectionServer, out List<NotificationTemplate> pTemplates)
        {
            WebCallResult res;
            pTemplates = null;

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetNotificationTemplates";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + "notificationtemplates";

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

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

            pTemplates = GetNotificationTemplatesFromXElements(pConnectionServer, res.XmlElement);
            return res;
        }


        //Helper function to take an XML blob returned from the REST interface for NotificationTemplate returned and convert it into an generic
        //list of NotificationTemplate class objects.  
        private static List<NotificationTemplate> GetNotificationTemplatesFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to GetNotificationTemplatesFromXElements");
            }

            List<NotificationTemplate> oTemplates = new List<NotificationTemplate>();

            //Use LINQ to XML to create a list of NotificationTemplate objects in a single statement. 
            var notificationTemplates = from e in pXElement.Elements()
                               where e.Name.LocalName == "NotificationTemplate"
                               select e;

            //for each object returned in the list from the XML, construct a class object using the elements associated with that 
            //object.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlTemplates in notificationTemplates)
            {
                NotificationTemplate oTemplate= new NotificationTemplate(pConnectionServer);
                foreach (XElement oElement in oXmlTemplates.Elements())
                {
                    //adds the XML property to the UserBase object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXmlFetch(oTemplate, oElement);
                }

                //add the fully populated UserBase object to the list that will be returned to the calling routine.
                oTemplates.Add(oTemplate);
            }

            return oTemplates;
        }


        #endregion

    }
}
