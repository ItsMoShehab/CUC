#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Read only class that provides methods to fetch TUI or GUI credentials for users
    /// </summary>
    public class Credential
    {

        #region Constructors and Destructors


        /// <summary>
        /// Creates a new instance of the Credential class.  You must provide the ConnectionServer reference that the user lives on and an ObjectId
        /// of the user that owns it.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that the Credential is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID that identifies the user that owns the credential
        /// </param>
        /// <param name="pCredentialType">
        /// The credential type to fetch for the user (PIN or GUI Password)
        ///  </param>
        public Credential(ConnectionServer pConnectionServer, string pUserObjectId, CredentialType pCredentialType)
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer reference passed to Credential constructor");
            }

            if (string.IsNullOrEmpty(pUserObjectId))
            {
                throw new ArgumentException("Emtpy UserObjectID passed to Credential constructor");
            }

            HomeServer = pConnectionServer;

            UserObjectId = pUserObjectId;

            CredentialType = pCredentialType;

            WebCallResult res = GetCredential(pUserObjectId, pCredentialType);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Credential not found in Credential constructor using UserObjectID={0}\n\r{1}"
                                                 , pUserObjectId, res.ErrorText));
            }
        }

        /// <summary>
        /// generic constructor for JSON parsing library
        /// </summary>
        public Credential()
        {

        }

        #endregion


        #region Fields and properties

        //reference to the ConnectionServer object used to create this credential instance.
        public ConnectionServer HomeServer { get; private set; }

        #endregion


        #region Credential Properties

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public CredentialType CredentialType { get; private set; }

        [JsonProperty]
        public string Alias { get; private set; }

        [JsonProperty]
        public bool CredMustChange { get; private set; }

        [JsonProperty]
        public string CredentialPolicyObjectId { get; private set; }

        [JsonProperty]
        public string Credentials { get; private set; }

        [JsonProperty]
        public bool CantChange { get; private set; }

        [JsonProperty]
        public bool DoesntExpire { get; private set; }

        [JsonProperty]
        public EncryptionType EncryptionType { get; private set; }

        [JsonProperty]
        public int HackCount { get; private set; }

        [JsonProperty]
        public bool Hacked { get; private set; }

        [JsonProperty]
        public bool IsPrimary { get; private set; }

        [JsonProperty]
        public bool Locked { get; private set; }

        [JsonProperty]
        public DateTime TimeChanged { get; private set; }

        [JsonProperty]
        public DateTime TimeLastHack { get; private set; }

        [JsonProperty]
        public string UserObjectId { get; private set; }

        #endregion


        #region Static Methods

        /// <summary>
        /// returns a single Credential object from a UserObjectID string passed in and a credential type.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the credential is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID of the user that owns the credential to be fetched.
        /// </param>
        /// <param name="pCredentialType">
        /// Type of credential to fetch for the user (PIN or Password)
        ///  </param>
        /// <param name="pCredential">
        /// Resulting credential instance is passed back on this value.
        ///  </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetCredential(ConnectionServer pConnectionServer, string pUserObjectId,CredentialType pCredentialType , out Credential pCredential)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pCredential = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetCredential";
                return res;
            }

            if (string.IsNullOrEmpty(pUserObjectId))
            {
                res.ErrorText = "Empty ObjectId passed to GetCredential";
                return res;
            }

            //create a new Credential instance passing the ObjectId which fills out the data automatically
            try
            {
                pCredential = new Credential(pConnectionServer, pUserObjectId, pCredentialType);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch device in GetCredential:" + ex.Message;
            }

            return res;
        }


        #endregion


        #region Instance Methods
        
        /// <summary>
        /// Diplays the credential type and owner's alias
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} [{1}]", this.Alias ,this.CredentialType);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the credential object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the credential object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            StringBuilder strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}{3}", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null),Environment.NewLine);
            }

            return strBuilder.ToString();
        }


        /// <summary>
        /// Fills the current instance of Credential in with properties fetched from the server.
        /// </summary>
        /// <param name="pUserObjectId">
        /// GUID that identifies the user that owns the credential itself.
        /// </param>
        /// <param name="pCredentialType">
        /// Credential type to fetch (PIN or Password)
        ///  </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetCredential(string pUserObjectId, CredentialType pCredentialType)
        {
            string strUrl;
            if (pCredentialType == CredentialType.Password)
            {
                strUrl = string.Format("{0}users/{1}/credential/password", HomeServer.BaseUrl,pUserObjectId);
            }
            else
            {
                strUrl = string.Format("{0}users/{1}/credential/pin", HomeServer.BaseUrl, pUserObjectId);
            }

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, RestTransportFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }
            return res;
        }


        #endregion


    }
}
