#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;


namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Read only class for fetching and listing notification templates - these are used when creating HTML notification devices
    /// in Connection 9.0 and later systems.
    /// </summary>
    public class NotificationTemplate
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires the ConnectionServer the template lives on and optionally takes an ObjectId of a template to 
        /// load data for.
        /// </summary>
        public NotificationTemplate(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to NotificationTemplate construtor");
            }

            HomeServer = pConnectionServer;

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

        /// <summary>
        /// Generic constructor for Json parsing libraries
        /// </summary>
        public NotificationTemplate()
        {
        }

        #endregion


        #region Fields and Properties

        public ConnectionServer HomeServer { get; private set; }

        #endregion


        #region NotificationTemplate Properties

        public string NotificationTemplateId { get; set; }
        public string NotificationTemplateName { get; set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the name and ID of the template
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", NotificationTemplateName, NotificationTemplateId);
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
            string strUrl = string.Format("{0}notificationtemplates/{1}", HomeServer.BaseUrl, pObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

             try
            {
                JsonConvert.PopulateObject(res.ResponseText, this,HTTPFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
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
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetNotificationTemplates(ConnectionServer pConnectionServer, out List<NotificationTemplate> pTemplates,
            int pPageNumber=1, int pRowsPerPage=20)
        {
            WebCallResult res;
            pTemplates = new List<NotificationTemplate>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetNotificationTemplates";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "notificationtemplates",
                "pageNumber=" + pPageNumber, "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

             //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that isn't an error - having zero is ok - just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount==0)
            {
                return res;
            }

            pTemplates = HTTPFunctions.GetObjectsFromJson<NotificationTemplate>(res.ResponseText);

            if (pTemplates == null)
            {
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pTemplates)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        #endregion

    }
}
