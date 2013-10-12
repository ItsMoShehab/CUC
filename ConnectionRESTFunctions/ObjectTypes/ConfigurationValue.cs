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
    /// Class that allows for fetching all or some or specific system configuration values.  This is a read only class currently.
    /// </summary>
    [Serializable]
    public class ConfigurationValue
    {

        #region Constructors and Destructors


        /// <summary>
        /// Creates a new instance of the ConfigurationValue class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching and 
        /// updating data for this value.  
        /// If you pass the pFullName parameter the configuration value for that name is automatically filled with data for that value from the server.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server being queried
        /// </param>
        /// <param name="pFullName">
        /// Optional full name of the value to fetch 
        /// </param>
        public ConfigurationValue(ConnectionServerRest pConnectionServer, string pFullName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to ConfigurationValue construtor");
            }

            //keep track of the home Connection server this value is created on.
            HomeServer = pConnectionServer;

            //if the user passed in a specific ObjectId or display name then go load that value up, otherwise just return an empty instance.
            if (string.IsNullOrEmpty(pFullName)) return;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetConfigurationValue(pFullName);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Configuration value not found in ConfigurationValue constructor " +
                                                                         "using FullName={0}\n\r{1}", pFullName, res.ErrorText));
            }
        }

        /// <summary>
        /// Generic constructor for JSON library parsing
        /// </summary>
        public ConfigurationValue()
        {
        }

        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object used to create this value's instance.
        internal ConnectionServerRest HomeServer;

        #endregion


        #region ConfigurationValue Properties

        [JsonProperty]
        public int Type { get; private set; }

        [JsonProperty]
        public string FullName { get; private set; }
        
        [JsonProperty]
        public bool UserSetting { get; private set; }

        [JsonProperty]
        public int MinVal { get; private set; }
        
        [JsonProperty]
        public int MaxVal { get; private set; }

        [JsonProperty]
        public bool RequiresRestart { get; private set; }

        [JsonProperty]
        public DateTime LastModifiedTime { get; private set; }

        [JsonProperty]
        public string LastModifiedByComponent { get; private set; }

        [JsonProperty]
        public string RegexValidation { get; private set; }

        [JsonProperty]
        public string Value { get; private set; }

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
    /// Reference to the ConnectionServer object that points to the home server where the values are being fetched from.
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
    public static WebCallResult GetConfigurationValues(ConnectionServerRest pConnectionServer, out List<ConfigurationValue> pConfigurationValues, 
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

        string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "configurationvalues", pClauses);

        //issue the command to the CUPI interface
        res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

        if (res.Success == false)
        {
            return res;
        }

        //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
        //if this is empty that is not an error, just return the empty list
        if (string.IsNullOrEmpty(res.ResponseText))
        {
            pConfigurationValues = new List<ConfigurationValue>();
            res.Success = false;
            return res;
        }

        //no error, just return empty list.
        if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
         {
             pConfigurationValues= new List<ConfigurationValue>();
             return res;
         }

        pConfigurationValues = pConnectionServer.GetObjectsFromJson<ConfigurationValue>(res.ResponseText);

        foreach (var oObject in pConfigurationValues)
        {
            oObject.HomeServer = pConnectionServer;
        }

        return res;

        }


        /// <summary>
        /// This method allows for a GET of configuration valies from Connection via HTTP - it allows for passing any number of additional clauses  
        /// for filtering (query directives), sorting and paging of results.  The format of the clauses should look like:
        /// filter: "query=(FullName startswith System)"
        /// sort: "sort=(fullname asc)"
        /// Escaping of spaces is done automatically, no need to account for that.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the values are being fetched from.
        /// </param>
        /// <param name="pConfigurationValues">
        /// The values found will be returned in this generic list of ConfigurationValues
        /// </param>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
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

        public static WebCallResult GetConfigurationValues(ConnectionServerRest pConnectionServer,out List<ConfigurationValue> pConfigurationValues,
            int pPageNumber=1, int pRowsPerPage=20,params string[] pClauses)
        {
            //tack on the paging items to the parameters list
            List<string> temp;
            if (pClauses == null)
            {
                temp = new List<string>();
            }
            else
            {
                temp = pClauses.ToList();
            }

            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            return GetConfigurationValues(pConnectionServer, out pConfigurationValues, temp.ToArray());
        }


            /// <summary>
        /// returns a single ConfigurationValue object from an ObjectId or name string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the value is homed on.
        /// </param>
        /// <param name="pFullName">
        /// Full name of the value to fetch.
        /// </param>
        /// <param name="pConfigurationValue">
        /// The out param that the filled out instance of the ConfigurationValue class is returned on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetConfigurationValue(out ConfigurationValue pConfigurationValue, ConnectionServerRest pConnectionServer, string pFullName)
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

            //create a new ConfigurationValue instance passing the ObjectId (or name) which fills out the data automatically
            try
            {
                pConfigurationValue = new ConfigurationValue(pConnectionServer,pFullName);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch value in GetConfigurationValue:" + ex.Message;
                res.Success = false;
            }

            return res;
        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays the display name and value object by default.
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} [{1}]", this.FullName, this.Value);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the ConfigurationValue object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the object instance.
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
            string strUrl = string.Format("{0}configurationvalues/{1}", HomeServer.BaseUrl, pFullName);

            return HomeServer.FillObjectWithRestGetResults(strUrl,this);
        }


        #endregion

    }
}
