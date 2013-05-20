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
    /// Read only class that provides methods for fetching port group templates from Unity Connection.
    /// The ObjectIds for PortGroup templates are needed when creating new port groups - they setup the details for
    /// SIP vs SCCP vs PIMG/TIMG phone system integrations.
    /// </summary>
    public class PortGroupTemplate
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires a ConnectionServer where the port group template is homed.  Optionally you can
        /// pass the objectId of the template and it will load values for that object.
        /// </summary>
        public PortGroupTemplate(ConnectionServer pConnectionServer, string pObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to PortGroupTemplate construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            WebCallResult res = GetPortGroupTemplate(pObjectId);
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Failed to fetch PortGroupTemplate by ObjectId={0}", pObjectId));
            }
        }

        /// <summary>
        /// general constructor for Json parsing libararies
        /// </summary>
        public PortGroupTemplate()
        {

        }

        #endregion


        #region Fields and Properties 

        public ConnectionServer HomeServer;

        #endregion


        #region PortGrpoupTemplate Properties

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string TemplateDescriptionDefault { get; private set; }

        [JsonProperty]
        public TelephonyIntegrationMethodEnum CopyTelephonyIntegrationMethodEnum { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the description of the objectId
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("PorgGroupTemplate:{0}, ObjectId:{1}", TemplateDescriptionDefault, ObjectId);
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
        /// string containing all the name value pairs defined in the PortGroupTemplate object instance.
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
        /// Fetch a role by objectId or name and fill the properties (if found) of the current class instance with what's found
        /// </summary>
        /// <param name="pObjectId">
        /// GUID of the role to find.  
        /// </param>
        /// <returns>
        /// WebCallResults instance.
        /// </returns>
        private WebCallResult GetPortGroupTemplate(string pObjectId)
        {
            string strUrl = HomeServer.BaseUrl + "portgrouptemplates/" + pObjectId;

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, HTTPFunctions.JsonSerializerSettings);
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
        /// Simple helper method that returns the port group template ObjectId based on the integration method - most often this is all
        /// you need from the port group templates when creating a new phone system configuration.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the port group templates are stored on
        /// </param>
        /// <param name="pIntegrationMethod">
        /// SIP, SCCP, PIMG/TIMG
        /// </param>
        /// <param name="pPortGroupTemplateObjectId">
        /// The ObjectId of the template associated with the integration method is passed back out on this parameter
        /// </param>
        /// <returns>
        /// instance of the WebCallResult class with details of the call and it's results from the server.
        /// </returns>
        public static WebCallResult GetPortGroupTemplateObjectId(ConnectionServer pConnectionServer,
                                                                 TelephonyIntegrationMethodEnum pIntegrationMethod,
                                                                 out string pPortGroupTemplateObjectId)
        {
            pPortGroupTemplateObjectId = "";
            List<PortGroupTemplate> oList;
            WebCallResult res = GetPortGroupTemplates(pConnectionServer, out oList);
            if (res.Success == false)
            {
                return res;
            }

            foreach (var oTemplate in oList)
            {
                if (oTemplate.CopyTelephonyIntegrationMethodEnum == pIntegrationMethod)
                {
                    pPortGroupTemplateObjectId = oTemplate.ObjectId;
                    return res;
                }
            }

            //if we're here the integration type wasn't found
            res.Success = false;
            res.ErrorText = "Telephony Integration Method not found with passed in value:" + pIntegrationMethod;
            return res;
        }


        /// <summary>
        /// This function allows for a GET of port group templates from Connection via HTTP - typically there are only three templates defined
        /// on the server and there's no provision for creating more so there's no filter clauses or the like supported to keep it simple.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the templates are being fetched from.
        /// </param>
        /// <param name="pTemplates">
        /// The list of port group templates is returned via this out parameter
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPortGroupTemplates(ConnectionServer pConnectionServer, out List<PortGroupTemplate> pTemplates)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pTemplates = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPortGroupTemplates";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "portgrouptemplates");

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's not an error, just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pTemplates = new List<PortGroupTemplate>();
                return res;
            }

            pTemplates = HTTPFunctions.GetObjectsFromJson<PortGroupTemplate>(res.ResponseText);

            if (pTemplates == null)
            {
                pTemplates = new List<PortGroupTemplate>();
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
