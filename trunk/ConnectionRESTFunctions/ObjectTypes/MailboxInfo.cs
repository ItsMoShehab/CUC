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
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{
    /// <summary>
    /// Mailbox details for the user passed into the constructor.
    /// NOTE: The administrator account you are authenticated with must have the "Mailbox Access Delegate Account" role for this to work.  This
    /// role is NOT applied to administrator accounts by default.    
    /// </summary>
    public class MailboxInfo
    {
       

        #region Constructors

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

            _homeServer = pConnectionServer;

            //if the ObjectId is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetMailboxInfo(pUserObjectId);

            if (res.Success == false)
            {
                throw new Exception("Failed to fetch mailbox details in MailboxInfo constructor"+res.ErrorText);
            }
        }


        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object 
        private readonly ConnectionServer _homeServer;

        //all the properties returned from Connection for the mailbox info

        public string DisplayName { get; private set; }
        public long CurrentSizeInBytes { get; private set; }
        public bool IsPrimary { get; private set; }
        public bool IsStoreOverFlowed { get;private set; }
        public bool IsStoreMounted { get; private set; }
        public bool IsMailboxMounted { get; private set; }
        public bool IsWarningQuotaExceeded { get; private set; }
        public bool IsReceiveQuotaExceeded { get; private set; }
        public bool IsSendQuotaExceeded { get; private set; }
        public long ReceiveQuota { get; private set; }
        public long WarningQuota { get; private set; }
        public long SendQuota { get; private set; }
        public bool IsDeletedFolderEnabled { get; private set; }

        #endregion


        #region Methods

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
            string strUrl = string.Format("{0}mailbox?userobjectid={1}", _homeServer.BaseUrl,pUserObjectId);

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

            //fill in the properties for this class from the data returned via XML from the server.
            foreach (XElement oElement in res.XmlElement.Elements())
            {
                _homeServer.SafeXmlFetch(this, oElement);
            }

            return res;
        }


        #endregion

    }
}
