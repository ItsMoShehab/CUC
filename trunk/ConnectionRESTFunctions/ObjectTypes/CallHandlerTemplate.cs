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
    /// The call handler template class is used only to provide an interface for user to select which template to use when creating new call 
    /// handlers.  
    /// </summary>
    public class CallHandlerTemplate
    {
        #region Fields and Properties

        public ConnectionServer HomeServer { get; private set; }

        public string DisplayName { get; set; }
        public string ObjectId { get; set; }

        #endregion


        #region Constructors


        /// <summary>
        /// default constructor used by JSON parser
        /// </summary>
        public CallHandlerTemplate()
        {
        }

        /// <summary>
        /// constructor with server and optional display name items
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pObjectId"></param>
        /// <param name="pDisplayName"></param>
        public CallHandlerTemplate(ConnectionServer pConnectionServer, string pObjectId, string pDisplayName="")
        {
            if (pConnectionServer==null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to CallHandlerTemplate construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pDisplayName))
            {
                return;
            }

            WebCallResult res = GetCallHandlerTemplate(pObjectId, pDisplayName);
            if (res.Success == false)
            {
                throw new Exception("Failed to fetch handler template by alias or objectId:"+res);
            }

        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all call handler templates and resturns them as a generic list of CallHandlerTemplate objects.  This
        /// list can be used for providing drop down list selection for handler creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the templates should be pulled from
        /// </param>
        /// <param name="pCallHandlerTemplates">
        /// Out parameter that is used to return the list of CallHandlerTemplate objects defined on Connection - there must be at least one.
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
        public static WebCallResult GetCallHandlerTemplates(ConnectionServer pConnectionServer, out List<CallHandlerTemplate> pCallHandlerTemplates
            , int pPageNumber = 1, int pRowsPerPage = 20)
        {
            WebCallResult res;
            pCallHandlerTemplates = new List<CallHandlerTemplate>();

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetCallHandlerTemplates";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "callhandlertemplates", "pageNumber=" + pPageNumber, 
                "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one template
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount==0)
            {
                res.Success = false;
                return res;
            }

            pCallHandlerTemplates = HTTPFunctions.GetObjectsFromJson<CallHandlerTemplate>(res.ResponseText);

            if (pCallHandlerTemplates == null)
            {
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pCallHandlerTemplates)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// Fetch a single instance of a CallHandlerTemplate using the objectId or name of the template
        /// </summary>
        /// <param name="pCallHandlerTemplate">
        /// Pass back the instance of the handler template on this parameter
        /// </param>
        /// <param name="pConnectionServer">
        /// Connection server being searched
        /// </param>
        /// <param name="pObjectId">
        /// ObjectId of the template to fetch
        /// </param>
        /// <param name="pDisplayName">
        /// Display name of template to search for
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult GetCallHandlerTemplate(out CallHandlerTemplate pCallHandlerTemplate, ConnectionServer pConnectionServer, 
            string pObjectId = "",string pDisplayName = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCallHandlerTemplate = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetCallHandlerTemplate";
                return res;
            }

            //you need an objectID and/or a display name - both being blank is not acceptable
            if ((pObjectId.Length == 0) & (pDisplayName.Length == 0))
            {
                res.ErrorText = "Empty objectId and DisplayName passed to GetCallHandlerTemplate";
                return res;
            }

            //create a new CallHandlerTemplate instance passing the ObjectId (or alias) which fills out the data automatically
            try
            {
                pCallHandlerTemplate = new CallHandlerTemplate(pConnectionServer, pObjectId, pDisplayName);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch handler template in GetCallHandlerTemplate:" + ex.Message;
            }

            return res;
        }


        #endregion


        #region Instance Methods

        public override string ToString()
        {
            return string.Format("{0} [{1}]", DisplayName, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the schedule object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the schedule object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            var strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Fills the current instance with details of a call handler template fetched using the ObjectID or the name.
        /// </summary>
        /// <param name="pObjectId">
        ///     ObjectId to search for - can be empty if name provided.
        /// </param>
        /// <param name="pDisplayName">
        ///     display name to search for.
        /// </param>
        /// <returns>
        /// Instance of the webCallSearchResult class.
        /// </returns>
        private WebCallResult GetCallHandlerTemplate(string pObjectId, string pDisplayName)

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
                            ErrorText = "Empty ObjectId passed to GetHandlerTemplate"
                        };
                }
            }

            string strUrl = string.Format("{0}callhandlertemplates/{1}", HomeServer.BaseUrl, strObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }
            return res;
        }

        /// <summary>
        /// Fetch the ObjectId of a schedule by it's name.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pName">
        /// Name of the schedule to find
        /// </param>
        /// <returns>
        /// ObjectId of schedule if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pName)
        {
            string strUrl = string.Format("{0}callhandlertemplates/?query=(DisplayName is {1})", HomeServer.BaseUrl, pName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false || res.TotalObjectCount ==0)
            {
                return "";
            }

            List<CallHandlerTemplate> oTemplates = HTTPFunctions.GetObjectsFromJson<CallHandlerTemplate>(res.ResponseText);

            foreach (var oTemplate in oTemplates)
            {
                if (oTemplate.DisplayName.Equals(pName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oTemplate.ObjectId;
                }
            }

            return "";
        }

        #endregion

    }
}
