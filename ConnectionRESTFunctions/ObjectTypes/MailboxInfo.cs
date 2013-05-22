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
    /// Mailbox details for the user passed into the constructor.
    /// NOTE: The administrator account you are authenticated with must have the "Mailbox Access Delegate Account" role for this to work.  This
    /// role is NOT applied to administrator accounts by default.    
    /// </summary>
    public class MailboxInfo
    {

        #region Constructors and Destructors


        /// <summary>
        /// Creates a new instance of the MailboxInfo class.  Requires you pass a handle to a ConnectionServer object which will be used for fetching 
        /// data for the user's mailbox.
        /// NOTE: The administrator account you are authenticated with must have the "Mailbox Access Delegate Account" role for this call to work.  This
        /// role is NOT applied to administrator accounts by default.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the list being created.
        /// </param>
        /// <param name="pUserObjectId">
        /// User to fetch mailbox details for.
        /// </param>
        public MailboxInfo(ConnectionServer pConnectionServer, string pUserObjectId)
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to MailboxInfo constructor.");
            }

            if (string.IsNullOrEmpty(pUserObjectId))
            {
                throw new ArgumentException("Empty ObjectId passed to MailboxInfo constructor.");
            }

            HomeServer = pConnectionServer;
            UserObjectId = pUserObjectId;

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetMailboxInfo(pUserObjectId);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,"Failed to fetch mailbox details in MailboxInfo constructor"+res.ErrorText);
            }
        }

        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object 
        public ConnectionServer HomeServer { get; private set; }

        //not returned from server, saved from constructor
        public string UserObjectId { get; private set; }

        #endregion


        #region MailboxInfo Properties

        //all the properties returned from Connection for the mailbox info

        [JsonProperty]
        public string DisplayName { get; private set; }

        [JsonProperty]
        public long CurrentSizeInBytes { get; private set; }

        [JsonProperty]
        public bool IsPrimary { get; private set; }

        [JsonProperty]
        public bool IsStoreOverFlowed { get;set; }

        [JsonProperty]
        public bool IsStoreMounted { get; private set; }

        [JsonProperty]
        public bool IsMailboxMounted { get;  set; }

        [JsonProperty]
        public bool IsWarningQuotaExceeded { get; private set; }

        [JsonProperty]
        public bool IsReceiveQuotaExceeded { get; private set; }

        [JsonProperty]
        public bool IsSendQuotaExceeded { get; private set; }

        [JsonProperty]
        public long ReceiveQuota { get; private set; }

        [JsonProperty]
        public long WarningQuota { get; private set; }

        [JsonProperty]
        public long SendQuota { get; private set; }

        [JsonProperty]
        public bool IsDeletedFolderEnabled { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the name and size of the mailstore
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Name: {0}, Current Size:{1}", DisplayName, CurrentSizeInBytes);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the mailbox info object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the alternate extension object instance.
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
        /// Helper function to fill in the mailbox info instance with data from Connection.
        /// </summary>
        /// <returns>
        /// WebCallResult instance
        /// </returns>
        private WebCallResult GetMailboxInfo(string pUserObjectId)
        {
            string strUrl = string.Format("{0}mailbox?userobjectid={1}", HomeServer.BaseUrl,pUserObjectId);

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


        /// <summary>
        /// Update the information for this instance of the mailbox info class
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class with details of the fetch and results from the server
        /// </returns>
        public WebCallResult RefetchMailboxData()
        {
            return GetMailboxInfo(UserObjectId);
        }


        /// <summary>
        /// Returns the message counts for the inbox, deleted items and sent items folders for the user tied to the 
        /// mailbox of this MailboxInfo instance.
        /// </summary>
        /// <param name="pInboxCount"></param>
        /// <param name="pDeletedItemsCount"></param>
        /// <param name="pSentItemsCount"></param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the fetch and results from the server
        /// </returns>
        public WebCallResult GetFolderMessageCounts(out int pInboxCount, out int pDeletedItemsCount,out int pSentItemsCount)
        {
            pInboxCount = 0;
            pSentItemsCount = 0;
            pDeletedItemsCount = 0;

            WebCallResult res = GetFolderCount(FolderTypes.Inbox, out pInboxCount);
            if (res.Success == false) return res;

            res = GetFolderCount(FolderTypes.Deleted, out pDeletedItemsCount);
            if (res.Success == false) return res;

            res = GetFolderCount(FolderTypes.Sent, out pSentItemsCount);
            return res;
        }

        /// <summary>
        /// for fetching message counts from folders
        /// </summary>
        private class Folder
        {
            [JsonProperty]
            public string DisplayName { get; private set; }
            
            [JsonProperty]
            public int MessageCount { get; private set; }
        }

        private enum FolderTypes {Inbox, Deleted, Sent}

        /// <summary>
        /// Returns the message count for a specific folder type (inbox, sent, deleted)
        /// </summary>
        /// <param name="pFolder">
        /// Mailbox folder to fetch count for
        /// </param>
        /// <param name="pCount">
        /// Message count for the folder type
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the fetch and results from the server
        /// </returns>
        private WebCallResult GetFolderCount(FolderTypes pFolder, out int pCount)
        {
            pCount = 0;
            string strUrl = string.Format("{0}mailbox/folders/{1}?userobjectid={2}", HomeServer.BaseUrl, pFolder.ToString(), UserObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            Folder oFolder;
            try
            {
                oFolder = HomeServer.GetObjectFromJson<Folder>(res.ResponseText, "Folder");
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
                return res;
            }

            pCount = oFolder.MessageCount;
            return res;
        }

        #endregion

    }
}
