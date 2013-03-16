﻿#region Legal Disclaimer

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
    /// Class that allows for fetching all or some or specific system configuration values.  This is a read only class currently.
    /// </summary>
    public class ConfigurationValue
    {
            #region Fields and Properties

            public int Type { get; set; }
            public string FullName { get; set; }
            public bool UserSetting { get; set; }
            public int MinValue { get; set; }
            public int MaxValue { get; set; }
            public bool RequiresRestart { get; set; }
            public DateTime LastModifiedTime { get; set; }
            public string LastModifiedByComponent { get; set; }
            public string RegexValidation { get; set; }
            public string Value { get; set; }
        
            //reference to the ConnectionServer object used to create this handlers instance.
            internal ConnectionServer HomeServer;

            #endregion


            #region Constructors

            /// <summary>
            /// Creates a new instance of the ConfigurationValue class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
            /// updating data for this handler.  
            /// If you pass the pFullName parameter the configuration value for that name is automatically filled with data for that value from the server.  
            /// </summary>
            /// <param name="pConnectionServer">
            /// Instance of a ConnectonServer object which points to the home server being queried
            /// </param>
            /// <param name="pFullName">
            /// Optional full name of the value to fetch 
            /// </param>
            public ConfigurationValue(ConnectionServer pConnectionServer, string pFullName = "")
            {
                if (pConnectionServer == null)
                {
                    throw new ArgumentException("Null ConnectionServer referenced pasted to DirectoryHandler construtor");
                }

                //keep track of the home Connection server this handler is created on.
                HomeServer = pConnectionServer;

                //if the user passed in a specific ObjectId or display name then go load that handler up, otherwise just return an empty instance.
                if (string.IsNullOrEmpty(pFullName)) return;

                //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
                WebCallResult res = GetConfigurationValue(pFullName);

                if (res.Success == false)
                {
                    throw new Exception(string.Format("Configuration value not found in ConfigurationValue constructor using FullName={0}\n\r{1}"
                                     , pFullName, res.ErrorText));
                }
            }

            #endregion


            #region Static Methods

        /// <summary>
        /// This method allows for a GET of configuration valies from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(FullName startswith System)"
        /// sort: "sort=(fullname asc)"
        /// page: "pageNumber=0"
        ///     : "rowsPerPage=8"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handlers are being fetched from.
        /// </param>
        /// <param name="pConfigurationValues">
        /// The values found will be returned in this generic list of ConfigurationValues
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetConfigurationValues(ConnectionServer pConnectionServer, out List<ConfigurationValue> pConfigurationValues, 
                params string[] pClauses)
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                pConfigurationValues = null;

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null Connection server object passed to GetConfigurationValues";
                    return res;
                }

                string strUrl = pConnectionServer.BaseUrl + "configurationvalues";

                //the spaces get "escaped out" in the HTTPFunctions class call at a lower level, don't worry about it here.
                //Tack on all the search/query/page clauses here if any are passed in.  If an empty string is passed in account
                //for it here.
                for (int iCounter = 0; iCounter < pClauses.Length; iCounter++)
                {
                    if (pClauses[iCounter].Length == 0)
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

                //issue the command to the CUPI interface
                res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, pConnectionServer, "");

                if (res.Success == false)
                {
                    return res;
                }

                //if the call was successful the XML elements can be empty, that's legal
                if (res.XMLElement == null || res.XMLElement.HasElements == false)
                {
                    pConfigurationValues = new List<ConfigurationValue>();
                    return res;
                }

                pConfigurationValues = GetConfigurationValuesFromXElements(pConnectionServer, res.XMLElement);
                return res;

            }


            /// <summary>
            /// returns a single DirectoryHandler object from an ObjectId or displayName string passed in.
            /// </summary>
            /// <param name="pConnectionServer">
            /// Connection server that the handler is homed on.
            /// </param>
            /// <param name="pFullName">
            /// Full name of the value to fetch.
            /// </param>
            /// <param name="pConfigurationValue">
            /// The out param that the filled out instance of the DirectoryHandler class is returned on.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            public static WebCallResult GetConfigurationValue(out ConfigurationValue pConfigurationValue, ConnectionServer pConnectionServer, string pFullName)
            {
                WebCallResult res = new WebCallResult();
                res.Success = false;

                pConfigurationValue = null;

                if (pConnectionServer == null)
                {
                    res.ErrorText = "Null Connection server object passed to GetConfigurationValue";
                    return res;
                }

                if (string.IsNullOrEmpty(pFullName))
                {
                    res.ErrorText = "Empty full name passed to GetConfiguraitonValue";
                    return res;
                }

                //create a new DirectoryHandler instance passing the ObjectId (or display name) which fills out the data automatically
                try
                {
                    pConfigurationValue = new ConfigurationValue(pConnectionServer,pFullName);
                    res.Success = true;
                }
                catch (Exception ex)
                {
                    res.ErrorText = "Failed to fetch value in GetConfigurationValue:" + ex.Message;
                    res.Success = false;
                }

                return res;
            }


            /// <summary>
            /// Helper function to take an XML blob returned from the REST interface for a handler (or handlers) return and convert it into an generic
            /// list of DirectoryHandler class objects. 
            /// </summary>
            /// <param name="pConnectionServer">
            /// Connection server the handler is homed on.
            /// </param>
            /// <param name="pXElement">
            /// The list of XML elements fetched from the Connection server via a CUPI call
            /// </param>
            /// <returns>
            /// List of DirectoryHandlers constructed from the XML elements.
            /// </returns>
            private static List<ConfigurationValue> GetConfigurationValuesFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
            {
                List<ConfigurationValue> oConfigurationValueList = new List<ConfigurationValue>();

                //pull out a set of XMLElements for each CallHandler object returned using the power of LINQ
                var configurationValues = from e in pXElement.Elements()
                               where e.Name.LocalName == "ConfigurationValue"
                               select e;

                //for each handler returned in the list of handlers from the XML, construct an DirectoryHandler object using the elements associated with that 
                //handler.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
                //typed objects.
                foreach (var oXmlConfigurationValue in configurationValues)
                {
                    ConfigurationValue oConfiguration = new ConfigurationValue(pConnectionServer);
                    foreach (XElement oElement in oXmlConfigurationValue.Elements())
                    {
                        //adds the XML property to the ConfigurationValue object if the proeprty name is found as a property on the object.
                        pConnectionServer.SafeXMLFetch(oConfiguration, oElement);
                    }

                    //add the fully populated ConfigurationValue object to the list that will be returned to the calling routine.
                    oConfigurationValueList.Add(oConfiguration);
                }

                return oConfigurationValueList;
            }

            #endregion


            #region Instance Methods

            /// <summary>
            /// Diplays the display name and extension of the handler by default.
            /// </summary>
            public override string ToString()
            {
                return String.Format("{0} [{1}]", this.FullName, this.Value);
            }

            /// <summary>
            /// Dumps out all the properties associated with the instance of the interview handler object in "name=value" format - each pair is on its
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
            /// Fills the current instance of ConfigurationSetting in with properties fetched from the server.  
            /// </summary>
            /// <param name="pFullName">
            /// Full name of the configuraiton setting (i.e. "System.Notifier.TaskManager.TaskMaxThreadWaitMinutes") to fetch.
            /// </param>
            /// <returns>
            /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
            /// </returns>
            private WebCallResult GetConfigurationValue(string pFullName)
            {
                WebCallResult res;

                string strUrl = string.Format("{0}configurationvalues/{1}", HomeServer.BaseUrl, pFullName);

                //issue the command to the CUPI interface
                res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, HomeServer, "");

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

                //populate this configuration value instance with data from the XML fetch
                foreach (XElement oElement in res.XMLElement.Elements())
                {
                    HomeServer.SafeXMLFetch(this, oElement);
                }

                return res;
            }


            #endregion

    }
}
