#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cisco.UnityConnection.RestFunctions
{

    #region Message Related Classes and Enums
    
    /// <summary>
    /// Messages can be sorted by one of: newest, oldest or urgent first.  there is no compound sorting supported in CUMI
    /// </summary>
    public enum MessageSortOrder { NEWEST_FIRST, OLDEST_FIRST, URGENT_FIRST }

    /// <summary>
    /// Each mailbox has 3 folders that can be accessed, this enum is provided to allow easy selection of which folder 
    /// messages are being pulled from
    /// </summary>
// ReSharper disable InconsistentNaming
    public enum MailboxFolder {inbox, deletedItems, sentItems}
// ReSharper restore InconsistentNaming

    /// <summary>
    /// All the message filter flags supported by CUMI - you can "stack" read, dispatch, type and priority flags.  If you mis
    /// flags like read and unread it will still work but is a little silly.
    /// </summary>
    [Flags]
    public enum MessageFilter
    {
        None = 0,
        Read_True = 1,
        Read_False = 2,
        Dispatch_True = 4,
        Dispatch_False = 8,
        Type_Voice = 16,
        Type_Fax = 32,
        Type_Email = 64,
        Type_Receipt = 128,
        Priority_Urgent = 256,
        Priority_Normal = 512,
        Priority_Low = 1024
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageType
    {
        Dr = 16,
        Email=1,
        Fax=4,
        Ndr = 8,
        Rr = 32,
        Voice=2,
        Video=64,
        Text=128,
        Msg = 256
    }

    /// <summary>
    /// Sensitivity settings for a message
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SensitivityType { Normal, Personal, Private, Confidential }
    
    /// <summary>
    /// message priority (only normal and urgent supported here)
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PriorityType { Normal, Urgent }

    /// <summary>
    /// Messages can be addressed with multiple targets - each target can be listed as a TO, BCC or CC recipient
    /// </summary>
    public enum MessageAddressType {TO, CC, BCC}

    /// <summary>
    /// When addressing messages it's possible to have multiple targets for different types (to, cc, bcc).  The SDK only supports
    /// addressing messages to SMTP addresses for users and list, not objectIds.
    /// </summary>
    public class MessageAddress
    {
        public MessageAddressType AddressType { get; set; }
        public string SmtpAddress { get; set; }
    }

    /// <summary>
    /// Caller ID can be passed as part of a message send
    /// </summary>
    public class CallerId
    {
        public string CallerNumber { get; set; }
        public string CallerName { get; set; }
        public string CallerImage { get; set; }
    }

    /// <summary>
    /// Address data is used for both the "From" and the list of recipients for a message
    /// </summary>
    public class AddressData
    {
        public string DisplayName { get; set; }
        public string SmtpAddress { get; set; }
        public string DtmfAccessId { get; set; }
    }

    /// <summary>
    /// There must be at least one but can be many recipients - it can be a TO, CC or a BCC recipient type.
    /// </summary>
    public class Recipient
    {
        public string Type { get; set; }
        public AddressData Address { get; set; }
    }

    /// <summary>
    /// Messages can contain no attachments or many - a typical voice mail contains at least one WAV attachment.
    /// </summary>
    public class Attachment
    {
        public string URI { get; set; }
        public string contentType { get; set; }
        public string contentTransferEncoding { get; set; }
        public string contentDisposition { get; set; }
    }

    #endregion

    /// <summary>
    /// Class to allow administrators to get a list of messages, the parts of each message and to fetch the media from a particular 
    /// message (the WAV file(s)) and download them to the local hard drive.
    /// Note that you must have the "Mailbox Access Delegate Account" role on the administrator account you are logging into Connection
    /// with - this is not on by default, even for the administrator account you create during install - you must manually add it.
    /// Without the role the message list returned will always be empty.
    /// </summary>
    public class UserMessage
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor for user message class.  If you pass in the message objectId it will be stored in the instance, if not
        /// you can fill it in later.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server instance that holds the message (or more to the point the user that owns the message).
        /// </param>
        /// <param name="pUserObjectId">
        /// User's mailbox to fetch messages for.
        /// </param>
        /// <param name="pMessageObjectId">
        /// ObjectId of the message.
        /// </param>
        public UserMessage(ConnectionServerRest pConnectionServer, string pUserObjectId, string pMessageObjectId = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to Message construtor");
            }
            if (string.IsNullOrEmpty(pUserObjectId))
            {
                throw new ArgumentException("Empty user ObjectId passed to UserMessage constructor");
            }

            UserObjectId = pUserObjectId;
            HomeServer = pConnectionServer;
            MsgId = pMessageObjectId;

            //make an instanced of the changed prop list to keep track of updated properties on this object
            _changedPropList = new ConnectionPropertyList();

            //create instances for the complex data types
            From = new AddressData();
            Recipients = new List<Recipient>();
            Attachments = new List<Attachment>();
            CallerId = new CallerId();

            if (!string.IsNullOrEmpty(pMessageObjectId))
            {
                //fill in full message details
                WebCallResult res = GetMessage(pMessageObjectId, pUserObjectId);
                if (res.Success == false)
                {
                    throw new UnityConnectionRestException(res,"Failed to find message using messageObjectId:" + res);
                }
            }
        }

        #endregion


        #region Properties and Fields

        //owner of the mailbox that the message is pulled from
        public string UserObjectId { get; private set; }

        //reference to the ConnectionServer object used to create this object instance.
        internal ConnectionServerRest HomeServer;

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region UserMessage Properties

        //properties returned from the server for each message found in the user's inbox.
        
        //subject and read are the only two items that can be updated on a message
        private string _subject;
        public string Subject
        {
            get { return _subject; }
            set
            {
                _changedPropList.Add("Subject", value);
                _subject = value;
            }
        }

        //you can update the read status on any inbox message - for deleted or sent items this change
        //will fail
        private bool _read;
        public bool Read
        {
            get { return _read; }
            set
            {
                _changedPropList.Add("Read", value);
                _read = value;
            }
        }

        //read only properties at the top level
        [JsonProperty]
        public string MsgId { get; private set; }

        [JsonProperty]
        public bool Dispatch { get; private set; }

        [JsonProperty]
        public bool Secure { get; private set; }

        [JsonProperty]
        public PriorityType Priority { get; private set; }

        [JsonProperty]
        public SensitivityType Sensitivity { get; private set; }

        [JsonProperty]
        public long ArrivalTime { get; private set; }

        [JsonProperty]
        public long Size { get; private set; }

        [JsonProperty]
        public long Duration { get; private set; }

        [JsonProperty]
        public bool FromSub { get; private set; }

        [JsonProperty]
        public string From_DtmfAccessId { get; private set; }

        [JsonProperty]
        public bool Flagged { get; private set; }

        [JsonProperty]
        public long IMAPUid { get; private set; }

        [JsonProperty]
        public long ModificationTime { get; private set; }

        [JsonProperty]
        public MessageType MsgType { get; private set; }

        [JsonProperty]
        public bool IsDraft { get; private set; }

        [JsonProperty]
        public bool IsDeleted { get; private set; }

        [JsonProperty]
        public bool IsSent { get; private set; }

        [JsonProperty]
        public bool IsFuture { get; private set; }

  
        //complex types
        public AddressData From { get; set; }
        public List<Recipient> Recipients { get; set; }

        public CallerId CallerId { get; set; }

        public List<Attachment> Attachments { get; set; }


        #endregion


        #region Static Methods


        /// <summary>
        /// CUPI stores time as milliseconds from 1970/1/1 - convert it using the span here.
        /// Time is stored in GMT - convert it to local time as necessary
        /// </summary>
        /// <param name="pMilliseconds">
        /// millisecond offset from 1970 for a date
        /// </param>
        /// <param name="pConvertToLocal">
        /// defaults to null which means the date is not converted.  If passed as true the time will be converted
        /// into local time, if passed as false it will be converted into UTC.
        /// </param>
        /// <returns>
        /// TimeDate instance.
        /// </returns>
        public static DateTime ConvertFromMillisecondsToTimeDate(long pMilliseconds, bool? pConvertToLocal = null)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            origin = origin.AddMilliseconds(pMilliseconds);

            if (pConvertToLocal == null)
            {
                return origin;
            }
            
            if (pConvertToLocal == false)
            {
                origin = origin.ToUniversalTime();
            }
            else
            {
                origin = origin.ToLocalTime();
            }

            return origin;
        }

        /// <summary>
        /// Converts from a TimeDate to milliseconds from 1970 which is how CUPI wants timestamps - this isn't used
        /// in the current library but is provided for completeness here.
        /// </summary>
        /// <param name="pDateTime">
        /// Date/time to convert
        /// </param>
        /// <param name="pConvertToLocal">
        /// Defaults to null which means the date is not converted.
        /// If passed as true, it's converted into the local time of the client.
        /// If passed as false it's converted into UTC
        ///  </param>
        /// <returns>
        /// long representing the number of milliseconds from 1970
        /// </returns>
        public static long ConvertFromTimeDateToMilliseconds(DateTime pDateTime, bool? pConvertToLocal = null)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff;
            
            if (pConvertToLocal== null)
            {
                diff = pDateTime - origin;
            }
            else if (pConvertToLocal==false)
            {
                diff = pDateTime.ToUniversalTime() - origin;
            }
            else
            {
                diff = pDateTime.ToLocalTime() - origin;
            }

            return (long)Math.Floor(diff.TotalMilliseconds);
        }


        /// <summary>
        /// Create a new voice message using a WAV file on the local hard drive.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to upload the message to.
        /// </param>
        /// <param name="pSenderUserObjectId">
        /// ObjectId of the user to send the message from (on behalf of)
        /// </param>
        /// <param name="pSubject">
        /// Subject text for the message
        /// </param>
        /// <param name="pPathToLocalWavFile">
        /// full path to the WAV file on the local hard drive to include as the voice mail attachment.
        /// </param>
        /// <param name="pUrgent">
        /// Pass as true to send message with urgent flag.
        /// </param>
        /// <param name="pSensitivity">
        /// sensitivity of the message (personal, confidential)
        /// </param>
        /// <param name="pSecure">
        /// Pass as true to set the message as secure (cannot be downloaded from server).
        /// </param>
        /// <param name="pDispatch">
        /// Pass as true to det the message for dispatch delivery - only makes sense if the addressing target is a 
        /// list.
        /// </param>
        /// <param name="pReadReceipt">
        /// Pass as true to flag for read receipt.
        /// </param>
        /// <param name="pDeliveryReceipt">
        /// PAss as true to flag for delivery receipt.
        /// </param>
        /// <param name="pCallerId">
        /// Installed of the CallerId class containing the calling number and/or calling name and/or URI to an image file to use for
        /// HTTP notification device scenarios.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// If passed as TRUE the routine will attempt to convert the target WAV file into raw PCM first before uploading it to the 
        /// Connection server.  A failure to convert will be considered a failed upload attempt and false is returned.  
        /// </param>
        /// <param name="pRecipients">
        /// One or more instances of the MessageAddress class defining the type and address of a message recipient.  As many recipients as you
        /// like can be included but at least one must be provided or the call fails.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with the details of the call and results from the server.
        /// </returns>
        public static WebCallResult CreateMessageLocalWav(ConnectionServerRest pConnectionServer, string pSenderUserObjectId,
                                                          string pSubject, string pPathToLocalWavFile, 
                                                          bool pUrgent, SensitivityType pSensitivity, bool pSecure, bool pDispatch,
                                                          bool pReadReceipt, bool pDeliveryReceipt,
                                                          CallerId pCallerId, bool pConvertToPcmFirst,
                                                          params MessageAddress[] pRecipients)
        {
            var res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to CreateMessageLocalWav";
                return res;
            }

            if (string.IsNullOrEmpty(pPathToLocalWavFile) || !File.Exists(pPathToLocalWavFile))
            {
                res.ErrorText = "Invalid path to wav file provided to CreateMessageLocalWav";
                return res;
            }

            if (pRecipients == null || !pRecipients.Any())
            {
                res.ErrorText = "No recipients passed to CreateMessageLocalWav";
                return res;
            }

            if (string.IsNullOrEmpty(pSubject))
            {
                res.ErrorText = "No subject passed to CreateNewMessageLocalWav";
                return res;
            }

            //if the user wants to try and rip the WAV file into PCM 16/8/1 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                string strConvertedWavFilePath = pConnectionServer.ConvertWavFileToPcm(pPathToLocalWavFile);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into PCM format in CreateMessageLocalWav.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted PCM WAV file path not found in CreateMessageLocalWav: " + strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pPathToLocalWavFile = strConvertedWavFilePath;

            }

            //construct the JSON strings needed in the message details and the message addressing sections of the upload message 
            //API call for Connection
            string strRecipientJsonString = ConstructRecipientJsonStringFromRecipients(pRecipients);
            string strMessageJsonString = ConstructMessageDetailsJsonString(pSubject, pUrgent, pSecure, pSensitivity,
                                                                            pDispatch, pReadReceipt, pDeliveryReceipt,
                                                                            false, pCallerId);

            //upload message
            return pConnectionServer.UploadVoiceMessageWav(pPathToLocalWavFile, strMessageJsonString,
                                                    pSenderUserObjectId, strRecipientJsonString);
        }


        /// <summary>
        /// Create a new voice message using a resourceId string provided by the CUTI interface.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to upload the message to.
        /// </param>
        /// <param name="pSenderUserObjectId">
        /// ObjectId of the user to send the message from (on behalf of)
        /// </param>
        /// <param name="pSubject">
        /// Subject text for the message
        /// </param>
        /// <param name="pResourceId">
        /// ResourceId string provided by the CUTI phone interface when recording a message on the server.
        /// </param>
        /// <param name="pUrgent">
        /// Pass as true to send message with urgent flag.
        /// </param>
        /// <param name="pSensitivity">
        ///  Message sensitivity (private/confidential)
        /// </param>
        /// <param name="pSecure">
        /// Pass as true to set the message as secure (cannot be downloaded from server).
        /// </param>
        /// <param name="pDispatch">
        /// Pass as true to det the message for dispatch delivery - only makes sense if the addressing target is a 
        /// list.
        /// </param>
        /// <param name="pReadReceipt">
        /// Pass as true to flag for read receipt.
        /// </param>
        /// <param name="pDeliveryReceipt">
        /// PAss as true to flag for delivery receipt.
        /// </param>
        /// <param name="pCallerId">
        /// Installed of the CallerId class containing the calling number and/or calling name and/or URI to an image file to use for
        /// HTTP notification device scenarios.
        /// </param>
        /// <param name="pRecipients">
        /// One or more instances of the MessageAddress class defining the type and address of a message recipient.  As many recipients as you
        /// like can be included but at least one must be provided or the call fails.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with the details of the call and results from the server.
        /// </returns>
        public static WebCallResult CreateMessageResourceId(ConnectionServerRest pConnectionServer, string pSenderUserObjectId,
                                                          string pSubject, string pResourceId,
                                                          bool pUrgent, SensitivityType pSensitivity, bool pSecure, bool pDispatch,
                                                          bool pReadReceipt, bool pDeliveryReceipt,
                                                          CallerId pCallerId, params MessageAddress[] pRecipients)
        {
            var res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to CreateMessageResourceId";
                return res;
            }

            if (pRecipients == null || !pRecipients.Any())
            {
                res.ErrorText = "No recipients passed to CreateMessageLocalWav";
                return res;
            }

            if (string.IsNullOrEmpty(pSubject))
            {
                res.ErrorText = "No subject passed to CreateNewMessageLocalWav";
                return res;
            }

            if (string.IsNullOrEmpty(pResourceId))
            {
                res.ErrorText = "No subject passed to CreateNewMessageLocalWav";
                return res;
            }

            //construct the JSON strings needed in the message details and the message addressing sections of the upload message 
            //API call for Connection
            string strRecipientJsonString = ConstructRecipientJsonStringFromRecipients(pRecipients);
            string strMessageJsonString = ConstructMessageDetailsJsonString(pSubject, pUrgent, pSecure, pSensitivity,
                                                                            pDispatch, pReadReceipt, pDeliveryReceipt,
                                                                            false, pCallerId);

            //upload message
            return pConnectionServer.UploadVoiceMessageResourceId(pResourceId, strMessageJsonString,
                                                    pSenderUserObjectId, strRecipientJsonString);
        }

        /// <summary>
        /// Builds the message recipient JSON string which can contain one or many address targets for a message.
        /// </summary>
        /// <param name="pMessageAddresses">
        /// One or more MessageAddress class instances that will construct into a address list
        /// </param>
        /// <returns>
        /// JSON formatted string for the message address targets for a message
        /// </returns>
        private static string ConstructRecipientJsonStringFromRecipients(params MessageAddress[] pMessageAddresses)
        {
            StringBuilder strJson = new StringBuilder("{\"Recipient\":[");

            foreach (MessageAddress oAddress in pMessageAddresses)
            {
                if (strJson.Length > 14)
                {
                    strJson.Append(",");
                }

                //address type
                strJson.Append("{\"Type\":");
                if (oAddress.AddressType == MessageAddressType.BCC)
                {
                    strJson.Append("\"BCC\",");
                }
                else if (oAddress.AddressType == MessageAddressType.CC)
                {
                    strJson.Append("\"CC\",");
                }
                else
                {
                    strJson.Append("\"TO\",");
                }

                //smtp target
                strJson.Append("\"Address\":{\"SmtpAddress\":\"");
                strJson.Append(oAddress.SmtpAddress);
                strJson.Append("\"}");
                strJson.Append("}");
            }
            strJson.Append("]}");

            return strJson.ToString();
        }


        /// <summary>
        /// Constructs the JSON string included in the first section of a new message request - this section includes details
        /// about the message such as subject, caller Id and flags for private/secure/urgent etc... Optional items such as
        /// dispatch and urgency flags are not included in the string if they are default (i.e. false).
        /// </summary>
        /// <param name="pSubject">
        /// Message subject string
        /// </param>
        /// <param name="pUrgent">
        /// True marks message as urgent
        /// </param>
        /// <param name="pSecure">
        /// True marks message as secure
        /// </param>
        /// <param name="pSensitivity">
        /// message sensitivity setting
        /// </param>
        /// <param name="pDispatch">
        /// True marks message for dispatch - only practical if the addressing target is a list.
        /// </param>
        /// <param name="pReadReceipt">
        /// True flags the message for read receipt.
        /// </param>
        /// <param name="pDeliveryReceipt">
        /// True flas the message for delivery receipt.
        /// </param>
        /// <param name="pFromSub">
        /// Pass as true to indicate it should be left as a sub to sub message, false to be an outside caller message.
        /// NOTE: as of 9.1 this does not work - all messages left via CUMI are left as sub to sub messages.
        /// </param>
        /// <param name="pCallerId">
        /// Instance of the caller ID class that can set the ANI (phone number), caller name and/or the URI to a graphic file
        /// on a web server (used for HTTP notification device scenarios).
        /// </param>
        /// <returns>
        /// JSON formatted string
        /// </returns>
        private static string ConstructMessageDetailsJsonString(string pSubject, 
                                                               bool pUrgent,bool pSecure, SensitivityType pSensitivity, bool pDispatch,
                                                               bool pReadReceipt, bool pDeliveryReceipt,
                                                               bool pFromSub, CallerId pCallerId)
        {
            //subject is required
            StringBuilder sb = new StringBuilder("{\"Subject\":\""+pSubject+"\"");

            //empty arrival time is always included - this cannot be edited
            sb.Append(",\"ArrivalTime\":\"0\"");

            if (pFromSub)
            {
                sb.Append(",\"FromSub\":\"true\"");
            }
            else
            {
                sb.Append(",\"FromSub\":\"false\"");
            }

            //tack on properties if they deviate from the default
            
            if (pUrgent)
            {
                sb.Append(",\"Priority\":\"Urgent\"");
            }

            if (pSecure)
            {
                sb.Append(",\"Secure\":\"true\"");
            }

            sb.AppendFormat(",\"Sensitivity\":\"{0}\"", pSensitivity.Description());

            if (pReadReceipt)
            {
                sb.Append(",\"ReadReceiptRequested\":\"true\"");
            }

            if (pDeliveryReceipt)
            {
                sb.Append(",\"DeliveryReceiptRequested\":\"true\"");
            }

            if (pDispatch)
            {
                sb.Append(",\"Dispatch\":\"true\"");
            }

            if (pCallerId != null)
            {
                sb.Append(",\"CallerId\":{");
                sb.Append("\"CallerNumber\":\""+pCallerId.CallerNumber+"\"");

                if (!string.IsNullOrEmpty(pCallerId.CallerNumber))
                {
                    sb.Append(",\"CallerName\":\"" + pCallerId.CallerName + "\"");
                }
                
                //caller image is used for HTTP notification device embedded graphics
                if (!string.IsNullOrEmpty(pCallerId.CallerImage))
                {
                    sb.Append(",\"CallerImage\":\"" + pCallerId.CallerImage + "\"");
                }
                
                sb.Append("}");
            }

            sb.Append("}");

            return sb.ToString();
        }


        /// <summary>
        /// Gets the total attachment count for the message passed into it.  Typically the count is 1 for most voice mail messages, 
        /// however if it's a multiply forwarded message it can be higher.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The connection server that houses the message being examined
        /// </param>
        /// <param name="pMessageObjectId">
        /// The GUID identifier for the message that contains the attachments to be counted.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID identifier for the user that owns the message that is being examined.
        /// </param>
        /// <param name="pAttachmentCount">
        /// Returns the total attachment count for the message
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMessageAttachmentCount(ConnectionServerRest pConnectionServer, string pMessageObjectId, string pUserObjectId,
                                                              out int pAttachmentCount)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;
            pAttachmentCount = 0;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetMessageAttachmentCount";
                return res;
            }

            //we need to get the message details which includes an attachment collection
            string strUrl = string.Format(@"{0}messages/{1}?userobjectid={2}", pConnectionServer.BaseUrl, pMessageObjectId, pUserObjectId);

            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "", false);

            if (res.Success == false)
            {
                return res;
            }

            //count up all the elements that have a local name of "Attachments"- usually there's only one but there can potentially be 
            //many if it's a multiply forwarded message with lots of intros or the like.
            foreach (XElement oElement in res.XmlElement.Elements())
            {
                if (oElement.Name.LocalName == "Attachments")
                {
                    pAttachmentCount = oElement.Elements().Count();
                    return res;
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the list of all messages for a particular user and passes them back as a generic list of UserMessage class objects.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the user is homed on.
        /// </param>
        /// <param name="pUserObjectId">
        /// User to fetch messages details for.
        /// </param>
        /// <param name="pMessage">
        /// Out parameter that is used to return the list of Message objects, if any, for the users's inbox.
        /// </param>
        /// <param name="pPageNumber"> 
        /// page number for multiple page searches - starts with 1.  If you go beyond the end an empty 
        /// list is returned in pMessages so you know you're at the end
        /// </param>
        /// <param name="pRowsPerPage"> 
        /// Number of messages to include in the fetch - best to keep it under 100.  Defaults to 10
        /// </param>
        /// <param name="pSortOrder">
        /// Can be sorted by newest first, oldest first or by urgency - newest first is the default
        ///  </param>
        /// <param name="pFilter"> 
        /// Can be filtered by message type, read status priority and dispatch flag.  Multiple filters can be combined.
        /// </param>
        /// <param name="pFolder">
        /// Defaults to the inbox folder but messages can be fetched from the deleted or sent items folders as well.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMessages(ConnectionServerRest pConnectionServer, string pUserObjectId, out List<UserMessage> pMessage,
            int pPageNumber = 1, int pRowsPerPage = 20, MessageSortOrder pSortOrder = MessageSortOrder.NEWEST_FIRST, 
            MessageFilter pFilter = MessageFilter.None, MailboxFolder pFolder = MailboxFolder.inbox)
        {
            WebCallResult res;
            pMessage = new List<UserMessage>();
            ConnectionPropertyList oParams = new ConnectionPropertyList();

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetMessages";
                return res;
            }

            //add row limits

            oParams.Add("pageNumber", pPageNumber.ToString());
            oParams.Add("rowsPerPage", pRowsPerPage.ToString());

            //add sort order
            switch (pSortOrder)
            {
                case MessageSortOrder.NEWEST_FIRST:
                    //add nothing, this is the default
                    break;
                case MessageSortOrder.OLDEST_FIRST:
                    oParams.Add("sortkey", "arrivaltime");
                    oParams.Add("sortorder", "ascending");
                    break;
                case MessageSortOrder.URGENT_FIRST:
                    oParams.Add("sortkey", "priority");
                    oParams.Add("sortorder", "descending");
                    break;
            }

            //add filtering flags
            if (pFilter.HasFlag(MessageFilter.Dispatch_True))
            {
                oParams.Add("dispatch", "true");
            }
            if (pFilter.HasFlag(MessageFilter.Dispatch_False))
            {
                oParams.Add("dispatch", "false");
            }
            if (pFilter.HasFlag(MessageFilter.Priority_Low))
            {
                oParams.Add("priority", "low");
            }
            if (pFilter.HasFlag(MessageFilter.Priority_Normal))
            {
                oParams.Add("priority", "normal");
            }
            if (pFilter.HasFlag(MessageFilter.Priority_Urgent))
            {
                oParams.Add("priority", "urgent");
            }
            if (pFilter.HasFlag(MessageFilter.Read_False))
            {
                oParams.Add("read", "false");
            }
            if (pFilter.HasFlag(MessageFilter.Read_True))
            {
                oParams.Add("read", "true");
            }
            if (pFilter.HasFlag(MessageFilter.Type_Email))
            {
                oParams.Add("type", "email");
            }
            if (pFilter.HasFlag(MessageFilter.Type_Fax))
            {
                oParams.Add("type", "fax");
            }
            if (pFilter.HasFlag(MessageFilter.Type_Receipt))
            {
                oParams.Add("type", "receipt");
            }
            if (pFilter.HasFlag(MessageFilter.Type_Voice))
            {
                oParams.Add("type", "voice");
            }

            oParams.Add("userobjectid",pUserObjectId);

            //construct the URL
            string strUrl = pConnectionServer.BaseUrl;
            if (pFolder == MailboxFolder.deletedItems)
            {
                strUrl+="mailbox/folders/deleted/messages";
            }
            else if (pFolder == MailboxFolder.inbox)
            {
                strUrl+="mailbox/folders/inbox/messages";
            }
            else
            {
                strUrl+="mailbox/folders/sent/messages";
            }
            
            strUrl = ConnectionServerRest.AddClausesToUri(strUrl, oParams.ToArrayOfStrings());

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "", false);

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            //don't return failure here - there may be no messages and that's ok.
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                res.Success = true;
                return res;
            }

            //convert the XML into properties on the class instance.
            pMessage = GetMessagesFromXElements(pConnectionServer,pUserObjectId, res.XmlElement);
            return res;
        }


        ///  <summary>
        /// Helper function to take an XML blob returned from the REST interface for Messages returned and convert it into an generic
        /// list of Message class objects.  
        ///  </summary>
        ///  <param name="pConnetionServer">
        ///  Connection server to communicate with.
        ///  </param>
        /// <param name="pUserObjectId">
        /// User that owns the mailbox the message is stored in
        /// </param>
        /// <param name="pXElement">
        ///  XMLElements holding the data fetched from Connection for messages
        ///  </param>
        ///  <returns>
        ///  List of userMessage objects.
        ///  </returns>
        private static List<UserMessage> GetMessagesFromXElements(ConnectionServerRest pConnetionServer, string pUserObjectId, XElement pXElement)
        {
            List<UserMessage> oMessageList = new List<UserMessage>();

            //pull out a set of XMLElements for each Message object returned using the power of LINQ
            var messages = from e in pXElement.Elements()
                           where e.Name.LocalName == "Message"
                           select e;

            //for each message returned in the list of messages from the XML, construct a message object using the elements associated with that 
            //message.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //typed objects.
            foreach (var oXmlMessage in messages)
            {
                UserMessage oMessage = null;// = new UserMessage(pConnetionServer, pUserObjectId);
                foreach (XElement oElement in oXmlMessage.Elements())
                {
                    if (oElement.Name == "MsgId")
                    {
                        //create a new full instance using the messageID and move on to the next one
                        oMessage = new UserMessage(pConnetionServer, pUserObjectId,oElement.Value);
                        break;
                    }
                }

                if (oMessage == null)
                {
                    pConnetionServer.RaiseErrorEvent("Could not find MsgId element in GetMessageFromXElements:"+pXElement);
                    continue;
                }
                
                oMessage.ClearPendingChanges();

                //add the fully populated message object to the list that will be returned to the calling routine.
                oMessageList.Add(oMessage);
            }

            return oMessageList;
        }


        /// <summary>
        /// Gets the media associated with a message attachment - typically this is a WAV file for a voice mail message and there is only
        /// one attachment.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The connection server that houses the message attachment to be fetched.
        /// </param>
        /// <param name="pTargetLocalFilePath">
        /// The location on the local hard drive to store the attachment.  If a file is already there it will be deleted and replaced.
        /// </param>
        /// <param name="pMessageObjectId">
        /// The GUID identifier for the message that contains the attachment to be downloaded.
        /// </param>
        /// <param name="pUserObjectId">
        /// The GUID identifier for the user that owns the message that has the attachment to be downloaded.
        /// </param>
        /// <param name="pAttachmentNumber">
        /// The zero based number indicating which attachment to download.  Usually 0 unless it's a multiple part message.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMessageAttachment(ConnectionServerRest pConnectionServer, string pTargetLocalFilePath, string pMessageObjectId, 
                                                        string pUserObjectId, int pAttachmentNumber)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetMessageAttachment";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pTargetLocalFilePath) || (Directory.GetParent(pTargetLocalFilePath).Exists == false))
            {
                res.ErrorText = "Invalid local file path passed to GetMessageAttachment: " + pTargetLocalFilePath;
                return res;
            }

            //go fetch the message attachment media file 
            res = pConnectionServer.DownloadMessageAttachment(pConnectionServer.BaseUrl,
                                                          pTargetLocalFilePath, 
                                                          pUserObjectId, 
                                                          pMessageObjectId,
                                                          pAttachmentNumber);

            return res;
        }


        /// <summary>
        /// DELETE a message from a user's mailstore (any folder).  
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that houses the message to be deleted.
        /// </param>
        /// <param name="pMessageObjectId">
        /// The unique identifier of the message to be removed.
        /// </param>
        /// <param name="pUserObjectId">
        /// The unique ID of the user that owns the message to be deleted.
        /// </param>
        /// <param name="pHardDelete">
        /// If passed as true the message is hard deleted which means it is not copied to the deleted items folder even if the
        /// users COS is configured to do that.  By default this is passed as false.
        /// </param>
        /// <returns>
        /// Instance of teh WebCallResult class with details of the call and the results.
        /// </returns>
        public static WebCallResult DeleteMessage(ConnectionServerRest pConnectionServer, string pMessageObjectId,
                                                  string pUserObjectId, bool pHardDelete=false)
        {
            string strHardDelete;
            if (pHardDelete)
            {
                strHardDelete = "harddelete=true";
            }
            else
            {
                strHardDelete = "harddelete=false";
            }

            string strUrl = string.Format("{0}messages/{1}?userobjectid={2}&{3}",pConnectionServer.BaseUrl,pMessageObjectId,
                pUserObjectId,strHardDelete);

            return pConnectionServer.GetCupiResponse(strUrl, MethodType.DELETE, "", false);
        }



        /// <summary>
        /// Allows one or more properties on a message to be udpated.  Only the read and subject values can currently be updated on 
        /// a message.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the message is homed.
        /// </param>
        /// <param name="pMessageObjectId">
        /// Unique identifier to message to be updated
        /// </param>
        /// <param name="pUserObjectId">
        /// Unique identifier for user that owns the message
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a message property name and a new value for that property to apply
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUMI interface.
        /// </returns>
        public static WebCallResult UpdateUserMessage(ConnectionServerRest pConnectionServer, string pMessageObjectId, 
            string pUserObjectId,ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateUserMessage";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateUserMessage";
                return res;
            }

            string strBody = "<Message>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</Message>";

            string strUrl = string.Format("{0}messages/{1}?userobjectid={2}", pConnectionServer.BaseUrl,pMessageObjectId, pUserObjectId);

            return pConnectionServer.GetCupiResponse(strUrl, MethodType.PUT, strBody, false);
        }


        /// <summary>
        /// Clear the entire contents of the deleted items folder (i.e. perform a hard delete on all soft deleted items).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server housing the user's mailbox to clear deleted items for.
        /// </param>
        /// <param name="pUserObjectId">
        /// Unique identifier for the user that owns the mailbox.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUMI interface.
        /// </returns>
        public static WebCallResult ClearDeletedItemsFolder(ConnectionServerRest pConnectionServer, string pUserObjectId)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult {Success = false, ErrorText = "Null ConnectionServer passed"};
            }

            string strUrl = string.Format("{0}mailbox/folders/deleted/messages?method=empty&userobjectid={1}",
                                          pConnectionServer.BaseUrl, pUserObjectId);

            return pConnectionServer.GetCupiResponse(strUrl, MethodType.POST, "", false);
        }



        /// <summary>
        /// This call allows you to recall a sent message - if you pass the MessageObjectId of a message from the sent items 
        /// folder, this will recall any unread instance of that message sitting in a recipient's mailbox still.  If the message
        /// has already been seen/read then it's not removed.  
        /// This is only available if the sent items retention is set longer than the default of 0 so the sent items folder can
        /// be accessed.
        /// NOTE: This is not available without an ES installed to support it - 9.1 will have an ES and it will be availalbe in
        /// the 10.0 release as well.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server where the user's mailbox asking for a recall is homed.
        /// </param>
        /// <param name="pUserObjectId"></param>
        /// Unique identifier for user asking for a recall
        /// <param name="pMessageObjectId"></param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUMI interface.
        /// </returns>
        public static WebCallResult RecallMessage(ConnectionServerRest pConnectionServer, string pUserObjectId,
                                                  string pMessageObjectId)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult { Success = false, ErrorText = "Null ConnectionServer passed" };
            }

            string strUrl = string.Format("{0}messages/{1}/recall?userobjectid={2}",
                                          pConnectionServer.BaseUrl, pMessageObjectId, pUserObjectId);

            return pConnectionServer.GetCupiResponse(strUrl, MethodType.POST, "", false);
        }


        /// <summary>
        /// A deleted message can be restored to the inbox using this method.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server where the mailbox that houses the message to be restored resides
        /// </param>
        /// <param name="pUserObjectId">
        /// User that owns the mailbox with the deleted message that will be restored to the inbox
        /// </param>
        /// <param name="pMessageObjectId">
        /// Unique identifier for the message to be restored.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUMI interface.
        /// </returns>
        public static WebCallResult RestoreDeletedMessage(ConnectionServerRest pConnectionServer, string pUserObjectId,
                                                          string pMessageObjectId)
        {
            if (pConnectionServer == null)
            {
                return new WebCallResult { Success = false, ErrorText = "Null ConnectionServer passed" };
            }

            string strUrl = string.Format("{0}messages/{1}?method=undelete&userobjectid={2}",
                                          pConnectionServer.BaseUrl, pMessageObjectId, pUserObjectId);

            return pConnectionServer.GetCupiResponse(strUrl, MethodType.POST, "", false);
        }


        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the SMTP address of the sender, the time arrived and the subject of the message.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("From: {1}, At:{2} Subject:{0}", Subject, From.SmtpAddress, ConvertFromMillisecondsToTimeDate(ArrivalTime));
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the UserMessage object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the UserMessage object instance.
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
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchMessageData()
        {
            return GetMessage(this.MsgId,this.UserObjectId);
        }


        //Fills the current instance of Message in with properties fetched from the server.
        private WebCallResult GetMessage(string pMessageObjectId, string pUserObjectId)
        {
            string strUrl = string.Format("{0}messages/{1}/?userobjectid={2}", HomeServer.BaseUrl, pMessageObjectId, pUserObjectId);

            //issue the command to the CUMI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "", false);

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

            //populate this Message instance with data from the XML fetch
            foreach (XElement oElement in res.XmlElement.Elements())
            {
                //deal with the complex types seperately - these are objects that require a "new" operation 
                //for collection management for each item in a list

                //recipients
                if (oElement.Name == "Recipients")
                {
                    foreach (XElement oRecipientElement in oElement.Elements())
                    {
                        Recipient oRecipient = new Recipient();
                        oRecipient.Address = new AddressData();

                        foreach (XElement oSubElement in oRecipientElement.Elements())
                        {

                            if (oSubElement.Name == "Address")
                            {
                                foreach (XElement oSubSubElement in oSubElement.Elements())
                                {
                                    HomeServer.SafeXmlFetch(oRecipient.Address, oSubSubElement);
                                }
                                continue;
                            }
                            HomeServer.SafeXmlFetch(oRecipient, oSubElement);
                        }

                        this.Recipients.Add(oRecipient);
                    }
                    
                    continue;
                }

                //attachments 
                if (oElement.Name == "Attachments")
                {
                    foreach (XElement oSubElement in oElement.Elements())
                    {
                        Attachment oAttachment = new Attachment();
                        foreach (XElement oSubSubElement in oSubElement.Elements())
                        {
                            HomeServer.SafeXmlFetch(oAttachment, oSubSubElement);
                        }

                        this.Attachments.Add(oAttachment);
                    }
                    continue;
                }

                //toplevel
                HomeServer.SafeXmlFetch(this, oElement);
            }

            ClearPendingChanges();

            return res;
        }

        
        /// <summary>
        /// Gets the media associated with a message attachment - typically this is a WAV file for a voice mail message and there is only
        /// one attachment.
        /// </summary>
        /// <param name="pTargetLocalFilePath">
        /// The location on the local hard drive to store the attachment.  If a file is already there it will be deleted and replaced.
        /// </param>
        /// <param name="pAttachmentNumber">
        /// The zero based number indicating which attachment to download.  Usually 0 unless it's a multiple part message.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetMessageAttachment(string pTargetLocalFilePath, int pAttachmentNumber)
        {
            return GetMessageAttachment(this.HomeServer, pTargetLocalFilePath, this.MsgId, this.UserObjectId, pAttachmentNumber);
        }


        /// <summary>
        /// DELETE a message from a user's mailstore (any folder).  
        /// </summary>
        /// <param name="pHardDelete">
        /// If passed as true the message is hard deleted which means it is not copied to the deleted items folder even if the
        /// users COS is configured to do that.  By default this is passed as false.
        /// </param>
        /// <returns>
        /// Instance of teh WebCallResult class with details of the call and the results.
        /// </returns>
        public WebCallResult Delete(bool pHardDelete=false)
        {
            return DeleteMessage(HomeServer, this.MsgId, UserObjectId, pHardDelete);
        }


        /// <summary>
        /// Allows one or more properties on a message to be udpated.  Currently only the subject and read status can be updated on
        /// a standing message.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update(bool pRefetchDataAfterSuccessfulUpdate = false)
        {
            WebCallResult res;

            //check if the message intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for user message {0}", this);
                return res;
            }

            //just call the static method with the info from the instance 
            res = UpdateUserMessage(HomeServer, MsgId, UserObjectId,  _changedPropList);

            //if the update went through then clear the changed properties list.
            if (res.Success)
            {
                _changedPropList.Clear();
                if (pRefetchDataAfterSuccessfulUpdate)
                {
                    return RefetchMessageData();
                }
            }

            return res;
        }

        /// <summary>
        /// Forward an existing message to one or more recipients - optionally include a wav file reference as an introduction.
        /// </summary>
        /// <param name="pSubject">
        /// Subject text for the message
        /// </param>
        /// <param name="pPathToWavFile">
        /// full path to the WAV file on the local hard drive to include as the voice mail attachment.
        /// </param>
        /// <param name="pUrgent">
        /// Pass as true to send message with urgent flag.
        /// </param>
        /// <param name="pSensitivityType">
        /// Message sensitivity (private/confidential)
        /// </param>
        /// <param name="pSecure">
        /// Pass as true to set the message as secure (cannot be downloaded from server).
        /// </param>
        /// <param name="pReadReceipt">
        /// Pass as true to flag for read receipt.
        /// </param>
        /// <param name="pDeliveryReceipt">
        /// PAss as true to flag for delivery receipt.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// If passed as TRUE the routine will attempt to convert the target WAV file into raw PCM first before uploading it to the 
        /// Connection server.  A failure to convert will be considered a failed upload attempt and false is returned.  
        /// </param>
        /// <param name="pRecipients">
        /// One or more instances of the MessageAddress class defining the type and address of a message recipient.  As many recipients as you
        /// like can be included but at least one must be provided or the call fails.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with the details of the call and results from the server.
        /// </returns>
        public WebCallResult ForwardMessageLocalWav(string pSubject, bool pUrgent, SensitivityType pSensitivityType, bool pSecure, bool pReadReceipt,
            bool pDeliveryReceipt,string pPathToWavFile, bool pConvertToPcmFirst, params MessageAddress[] pRecipients)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (!pRecipients.Any())
            {
                res.ErrorText = "No recipients included in ForwardMessageLocalWav call";
                return res;
            }

            //if a wav file is passed make sure it's valid
            if (!string.IsNullOrEmpty(pPathToWavFile) && !File.Exists(pPathToWavFile))
            {
                res.ErrorText = "Invalid wav file path provided to ForwardMEssageLocalWav:" + pPathToWavFile;
                return res;
            }

            //if the user wants to try and rip the WAV file into PCM 16/8/1 first before uploading the file, do that conversion here
            if (pConvertToPcmFirst)
            {
                string strConvertedWavFilePath = HomeServer.ConvertWavFileToPcm(pPathToWavFile);

                if (string.IsNullOrEmpty(strConvertedWavFilePath))
                {
                    res.ErrorText = "Failed converting WAV file into PCM format in CreateMessageLocalWav.";
                    return res;
                }

                if (File.Exists(strConvertedWavFilePath) == false)
                {
                    res.ErrorText = "Converted PCM WAV file path not found in CreateMessageLocalWav: " + strConvertedWavFilePath;
                    return res;
                }

                //point the wav file we'll be uploading to the newly converted G711 WAV format file.
                pPathToWavFile = strConvertedWavFilePath;
            }


            //construct the JSON strings needed in the message details and the message addressing sections of the upload message 
            //API call for Connection
            string strRecipientJsonString = ConstructRecipientJsonStringFromRecipients(pRecipients);
            string strMessageJsonString = ConstructMessageDetailsJsonString(pSubject, pUrgent, pSecure, pSensitivityType,
                                                                            false, pReadReceipt, pDeliveryReceipt,
                                                                            true, CallerId);

            //use the URI that indicates to forward the message with all other attachments
            string strUri = string.Format("{0}messages?messageid={1}&userobjectid={2}",HomeServer.BaseUrl,
                MsgId, UserObjectId);
            
            //forward message
            return HomeServer.UploadVoiceMessageWav(pPathToWavFile, strMessageJsonString,UserObjectId,strRecipientJsonString,strUri);
        }


        /// <summary>
        /// Forward an existing message to one or more recipients - optionally include a wav file reference as an introduction.  The introduction
        /// can be recorded using CUTI on the server and referenced via a ResourceId provided by the PhoneRecording class.
        /// </summary>
        /// <param name="pSubject">
        /// Subject text for the message
        /// </param>
        /// <param name="pResourceId">
        /// Resource Id of the system recording (CUTI interface) to use as the voice message.
        /// </param>
        /// <param name="pUrgent">
        /// Pass as true to send message with urgent flag.
        /// </param>
        /// <param name="pSensitivity">
        /// Message sensitivity
        /// </param>
        /// <param name="pSecure">
        /// Pass as true to set the message as secure (cannot be downloaded from server).
        /// </param>
        /// <param name="pReadReceipt">
        /// Pass as true to flag for read receipt.
        /// </param>
        /// <param name="pDeliveryReceipt">
        /// PAss as true to flag for delivery receipt.
        /// </param>
        /// <param name="pRecipients">
        /// One or more instances of the MessageAddress class defining the type and address of a message recipient.  As many recipients as you
        /// like can be included but at least one must be provided or the call fails.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with the details of the call and results from the server.
        /// </returns>
        public WebCallResult ForwardMessageResourceId(string pSubject, bool pUrgent, SensitivityType pSensitivity, bool pSecure, bool pReadReceipt,
           bool pDeliveryReceipt, string pResourceId, params MessageAddress[] pRecipients)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (!pRecipients.Any())
            {
                res.ErrorText = "No recipients included in ForwardMessageLocalWav call";
                return res;
            }

            //if a wav file is passed make sure it's valid
            if (string.IsNullOrEmpty(pResourceId))
            {
                res.ErrorText = "Invalid resource ID provided to ForwardMessageResourceId:" + pResourceId;
                return res;
            }

            //construct the JSON strings needed in the message details and the message addressing sections of the upload message 
            //API call for Connection
            string strRecipientJsonString = ConstructRecipientJsonStringFromRecipients(pRecipients);
            string strMessageJsonString = ConstructMessageDetailsJsonString(pSubject, pUrgent, pSecure, pSensitivity,
                                                                            false, pReadReceipt, pDeliveryReceipt,
                                                                            true, CallerId);

            //use the URI that indicates to forward the message with all other attachments
            string strUri = string.Format("{0}messages?messageid={1}&userobjectid={2}", HomeServer.BaseUrl,
                MsgId, UserObjectId);

            //forward message
            return HomeServer.UploadVoiceMessageResourceId(pResourceId, strMessageJsonString,
                                                    UserObjectId, strRecipientJsonString,strUri);
        }

        /// <summary>
        /// Reply to a message using a voice message constructed from a WAV file on the local hard drive.  This is just
        /// leaving a new message addressed to the sender of the original voice message - if the pReplyToAll flag is 
        /// passed as true the message is sent to every SMTP address that was listed as a recipient of the original
        /// message.
        /// </summary>
        /// <param name="pSubject">
        /// Subject to include.
        /// </param>
        /// <param name="pPathToLocalWavFile">
        /// full path to the WAV file on the local hard drive to include as the voice mail attachment.
        /// </param>
        /// <param name="pUrgent">
        /// Pass as true to send message with urgent flag.
        /// </param>
        /// <param name="pSensitivity">
        /// Message sensitivity
        /// </param>
        /// <param name="pSecure">
        /// Pass as true to set the message as secure (cannot be downloaded from server).
        /// </param>
        /// <param name="pReadReceipt">
        /// Pass as true to flag for read receipt.
        /// </param>
        /// <param name="pDeliveryReceipt">
        /// PAss as true to flag for delivery receipt.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// Convert the WAV to raw PCM before uploading.
        /// </param>
        /// <param name="pReplyToAll">
        /// If passed as true all the recipients of the original message are added to the "TO" addressing line for 
        /// the new message.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with details of the call and result from the CUMI call.
        /// </returns>
         public WebCallResult ReplyWithLocalWav(string pSubject, bool pUrgent, SensitivityType pSensitivity, bool pSecure,bool pReadReceipt,
                                                     bool pDeliveryReceipt, string pPathToLocalWavFile,
                                                     bool pConvertToPcmFirst, bool pReplyToAll=false)
         {
             List<MessageAddress> oRecipients = new List<MessageAddress>();
             MessageAddress oAddress;

             if (pReplyToAll)
             {
                 //add all recipients of the message to the list of TO addresses.
                foreach (var oRecipient in Recipients)
                {
                    oAddress = new MessageAddress();
                    oAddress.AddressType = MessageAddressType.TO;
                    oAddress.SmtpAddress = oRecipient.Address.SmtpAddress;
                    oRecipients.Add(oAddress);
                }
             }
             else
             {
                 //just add the sender of the message as the recipient
                 oAddress = new MessageAddress();
                 oAddress.AddressType = MessageAddressType.TO;
                 oAddress.SmtpAddress = this.From.SmtpAddress;
                 oRecipients.Add(oAddress);
             }

             //send the new message
             return CreateMessageLocalWav(HomeServer, UserObjectId, pSubject, pPathToLocalWavFile, pUrgent, pSensitivity, pSecure,
                                          false, pReadReceipt,pDeliveryReceipt, CallerId, pConvertToPcmFirst, oRecipients.ToArray());
         }


         /// <summary>
         /// Reply to a message using a voice message constructed on the server using the CUTI interface and referencing the resourceId
         /// generated by that call.  This functionality can be accessed through the PhoneRecording class in the library.
         /// message.
         /// </summary>
         /// <param name="pSubject">
         /// Subject to include.
         /// </param>
         /// <param name="pResourceId">
         /// Resource Id of the system recording (CUTI interface) to use as the voice message.
         /// </param>
         /// <param name="pUrgent">
         /// Pass as true to send message with urgent flag.
         /// </param>
         /// <param name="pSensitivity">
         /// Message sensitivity
         /// </param>
         /// <param name="pSecure">
         /// Pass as true to set the message as secure (cannot be downloaded from server).
         /// </param>
         /// <param name="pReadReceipt">
         /// Pass as true to flag for read receipt.
         /// </param>
         /// <param name="pDeliveryReceipt">
         /// PAss as true to flag for delivery receipt.
         /// </param>
         /// <param name="pReplyToAll">
         /// If passed as true all the recipients of the original message are added to the "TO" addressing line for 
         /// the new message.
         /// </param>
         /// <returns>
         /// Instance of the WebCallResult class with details of the call and result from the CUMI call.
         /// </returns>
         public WebCallResult ReplyWithResourceId(string pSubject, string pResourceId, bool pUrgent, SensitivityType pSensitivity,bool pSecure, 
                                                   bool pReadReceipt, bool pDeliveryReceipt,bool pReplyToAll = false)
         {
             List<MessageAddress> oRecipients = new List<MessageAddress>();
             MessageAddress oAddress;

             if (pReplyToAll)
             {
                 //add all recipients of the message to the list of TO addresses.
                 foreach (var oRecipient in Recipients)
                 {
                     oAddress = new MessageAddress();
                     oAddress.AddressType = MessageAddressType.TO;
                     oAddress.SmtpAddress = oRecipient.Address.SmtpAddress;
                     oRecipients.Add(oAddress);
                 }
             }
             else
             {
                 //just add the sender of the message as the recipient
                 oAddress = new MessageAddress();
                 oAddress.AddressType = MessageAddressType.TO;
                 oAddress.SmtpAddress = this.From.SmtpAddress;
                 oRecipients.Add(oAddress);
             }

             //send the new message
             return CreateMessageResourceId(HomeServer, UserObjectId, pSubject, pResourceId, pUrgent, pSensitivity, pSecure,
                                          false, pReadReceipt, pDeliveryReceipt, CallerId, oRecipients.ToArray());
         }


         /// <summary>
         /// This call allows you to recall a sent message - if you pass the MessageObjectId of a message from the sent items 
         /// folder, this will recall any unread instance of that message sitting in a recipient's mailbox still.  If the message
         /// has already been seen/read then it's not removed.  
         /// This is only available if the sent items retention is set longer than the default of 0 so the sent items folder can
         /// be accessed.
         /// NOTE: This is not available without an ES installed to support it - 9.1 will have an ES and it will be availalbe in
         /// the 10.0 release as well.
         /// </summary>
         /// <returns>
         /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUMI interface.
         /// </returns>
        public WebCallResult Recall()
        {
            return RecallMessage(HomeServer, UserObjectId, MsgId);
        }


        /// <summary>
        /// A deleted message can be restored to the inbox using this method.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUMI interface.
        /// </returns>
        public WebCallResult Restore()
        {
            return RestoreDeletedMessage(HomeServer, UserObjectId, MsgId);
        }

        /// <summary>
        /// If the call object has andy pending updates that have not yet be comitted, this will clear them out.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }

        #endregion
    }
}
