﻿#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{

    #region Message Related Classes and Enums
    
    /// <summary>
    /// Messages can be sorted by one of: newest, oldest or urgent first.  there is no compound sorting supported in CUMI
    /// </summary>
    public enum MessageSortOrder { NEWEST_FIRST, OLDEST_FIRST, URGENT_FIRST }

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
        public MessageAddressType AddressType;
        public string SmtpAddress;
    }

    /// <summary>
    /// Caller ID can be passed as part of a message send
    /// </summary>
    public class CallerId
    {
        public string CallerNumber;
        public string CallerName;
        public string CallerImage;
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
        #region Properties and Fields

        public enum SensitivityType {Normal, Personal, Private, Confidential}
        public enum PriorityType { Normal, Urgent }

        public string UserObjectId { get; private set; }

        //properties returned from the server for each message found in the user's inbox.
        public string Subject { get; set; }
        public string MsgId { get; set; }
        public bool Dispatch { get; set; }
        public bool Secure { get; set; }
        public string Priority { get; set; }
        public string Sensitivity { get; set; }
        public long ArrivalTime { get; set; }
        public long Size { get; set; }
        public long Duration { get; set; }
        public bool FromSub { get; set; }
        public string From_DtmfAccessId { get; set; }
        public bool Flagged { get; set; }
        public long IMAPUid { get; set; }
        public long ModificationTime { get; set; }
        public string MsgType { get; set; }
        public bool IsDraft { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSent { get; set; }
        public bool IsFuture { get; set; }
        public bool Read { get; set; }

        //complex types
        //FROM
        public string From_DisplayName { get; set; }
        public string From_SmtpAddress { get; set; }

        //CallerId
        public string CallerId_CallerNumber { get; set; }
        public string CallerId_CallerName { get; set; }



        //reference to the ConnectionServer object used to create this user instance.
        internal ConnectionServer HomeServer;

        #endregion


        #region Constructors 

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
        public UserMessage(ConnectionServer pConnectionServer, string pUserObjectId, string pMessageObjectId="")
        {
            if (pConnectionServer==null)
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
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// CUPI stores time as milliseconds from 1970/1/1 - convert it using the span here.
        /// Time is stored in GMT - convert it to local time as necessary
        /// </summary>
        /// <param name="pMilliseconds">millisecond offset from 1970 for a date</param>
        /// <param name="pConvertToLocal">
        /// convert to local time of the PC we're running on.  Defaults to true otherwise
        /// the value is return as UTC
        /// </param>
        /// <returns>
        /// TimeDate instance.
        /// </returns>
        public static DateTime ConvertFromMillisecondsToTimeDate(long pMilliseconds, bool pConvertToLocal = true)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            origin = origin.AddMilliseconds(pMilliseconds);

            if (pConvertToLocal)
                origin = origin.ToLocalTime();

            return origin;
        }

        /// <summary>
        /// Converts from a TimeDate to milliseconds from 1970 which is how CUPI wants timestamps - this isn't used
        /// in the current library but is provided for completeness here.
        /// </summary>
        /// <param name="pDateTime">
        /// Date/time to convert
        /// </param>
        /// <param name="pConvertToUtc">
        /// Converts the time to UTC (default).  IF the time is already in UTC pass this as FALSE.
        ///  </param>
        /// <returns>
        /// long representing the number of milliseconds from 1970
        /// </returns>
        public static long ConvertFromTimeDateToMilliseconds(DateTime pDateTime, bool pConvertToUtc = true)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff;
            if (pConvertToUtc)
            {
                diff = pDateTime.ToUniversalTime() - origin;
            }
            else
            {
                diff = pDateTime - origin;
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
        /// <param name="pPrivate">
        /// Pass as true to send message with personal flag (cannot be forwarded)
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
        public static WebCallResult CreateMessageLocalWav(ConnectionServer pConnectionServer, string pSenderUserObjectId,
                                                          string pSubject, string pPathToLocalWavFile, 
                                                          bool pUrgent, bool pPrivate, bool pSecure, bool pDispatch,
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
                string strConvertedWavFilePath = pConnectionServer.ConvertWAVFileToPCM(pPathToLocalWavFile);

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
            string strMessageJsonString = ConstructMessageDetailsJsonString(pSubject, pUrgent, pSecure, pPrivate,
                                                                            pDispatch, pReadReceipt, pDeliveryReceipt,
                                                                            true, false, pCallerId);

            //upload message
            return HTTPFunctions.UploadVoiceMessageWav(pConnectionServer.ServerName, pConnectionServer.LoginName,
                                                    pConnectionServer.LoginPw,pPathToLocalWavFile, strMessageJsonString,
                                                    pSenderUserObjectId, pConnectionServer.LastSessionCookie,
                                                    strRecipientJsonString);
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
        /// <param name="pPrivate">
        /// Pass as true to send message with personal flag (cannot be forwarded)
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
        public static WebCallResult CreateMessageResourceId(ConnectionServer pConnectionServer, string pSenderUserObjectId,
                                                          string pSubject, string pResourceId,
                                                          bool pUrgent, bool pPrivate, bool pSecure, bool pDispatch,
                                                          bool pReadReceipt, bool pDeliveryReceipt,
                                                          CallerId pCallerId, params MessageAddress[] pRecipients)
        {
            var res = new WebCallResult {Success = false};

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to CreateMessageResourceId";
                return res;
            }

            //construct the JSON strings needed in the message details and the message addressing sections of the upload message 
            //API call for Connection
            string strRecipientJsonString = ConstructRecipientJsonStringFromRecipients(pRecipients);
            string strMessageJsonString = ConstructMessageDetailsJsonString(pSubject, pUrgent, pSecure, pPrivate,
                                                                            pDispatch, pReadReceipt, pDeliveryReceipt,
                                                                            true, false, pCallerId);

            //upload message
            return HTTPFunctions.UploadVoiceMessageResourceId(pConnectionServer.ServerName, pConnectionServer.LoginName,
                                                    pConnectionServer.LoginPw, pResourceId, strMessageJsonString,
                                                    pSenderUserObjectId, pConnectionServer.LastSessionCookie,
                                                    strRecipientJsonString);
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
        /// <param name="pPrivate">
        /// True marks message as Personal.
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
        /// <param name="pFromSub"></param>
        /// <param name="pFromVmIntSub"></param>
        /// <param name="pCallerId">
        /// Instance of the caller ID class that can set the ANI (phone number), caller name and/or the URI to a graphic file
        /// on a web server (used for HTTP notification device scenarios).
        /// </param>
        /// <returns>
        /// JSON formatted string
        /// </returns>
        private static string ConstructMessageDetailsJsonString(string pSubject, 
                                                               bool pUrgent,bool pSecure, bool pPrivate, bool pDispatch,
                                                               bool pReadReceipt, bool pDeliveryReceipt,
                                                               bool pFromSub, bool pFromVmIntSub, CallerId pCallerId)
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

            if (pFromVmIntSub)
            {
                sb.Append(",\"FromVmIntSub\":\"true\"");
            }
            else
            {
                sb.Append(",\"FromVmIntSub\":\"false\"");
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

            if (pPrivate)
            {
                sb.Append(",\"Sensitivity\":\"Personal\"");
            }

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
        /// <param name="pMessagesPerPage"> 
        /// Number of messages to include in the fetch - best to keep it under 100.  Defaults to 10
        /// </param>
        /// <param name="pSortOrder">
        /// Can be sorted by newest first, oldest first or by urgency - newest first is the default
        ///  </param>
        /// <param name="pFilter"> 
        /// Can be filtered by message type, read status priority and dispatch flag.  Multiple filters can be combined.
        /// </param>        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetMessages(ConnectionServer pConnectionServer, string pUserObjectId, out List<UserMessage> pMessage,
            int pPageNumber = 1, int pMessagesPerPage = 10, MessageSortOrder pSortOrder = MessageSortOrder.NEWEST_FIRST, 
            MessageFilter pFilter = MessageFilter.None)
        {
            WebCallResult res;
            pMessage = new List<UserMessage>();
            List<KeyValuePair<string, string>> oParams = new List<KeyValuePair<string, string>>();

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetMessages";
                return res;
            }

            //add row limits

            oParams.Add(new KeyValuePair<string, string>("pagenumber", pPageNumber.ToString()));
            oParams.Add(new KeyValuePair<string, string>("rowsperpage", pMessagesPerPage.ToString()));

            //add sort order
            switch (pSortOrder)
            {
                case MessageSortOrder.NEWEST_FIRST:
                    //add nothing, this is the default
                    break;
                case MessageSortOrder.OLDEST_FIRST:
                    oParams.Add(new KeyValuePair<string, string>("sortkey", "arrivaltime"));
                    oParams.Add(new KeyValuePair<string, string>("sortorder", "ascending"));
                    break;
                case MessageSortOrder.URGENT_FIRST:
                    oParams.Add(new KeyValuePair<string, string>("sortkey", "priority"));
                    oParams.Add(new KeyValuePair<string, string>("sortorder", "descending"));
                    break;
            }

            //add filtering flags
            if (pFilter.HasFlag(MessageFilter.Dispatch_True))
            {
                oParams.Add(new KeyValuePair<string, string>("dispatch", "true"));
            }
            if (pFilter.HasFlag(MessageFilter.Dispatch_False))
            {
                oParams.Add(new KeyValuePair<string, string>("dispatch", "false"));
            }
            if (pFilter.HasFlag(MessageFilter.Priority_Low))
            {
                oParams.Add(new KeyValuePair<string, string>("priority", "low"));
            }
            if (pFilter.HasFlag(MessageFilter.Priority_Normal))
            {
                oParams.Add(new KeyValuePair<string, string>("priority", "normal"));
            }
            if (pFilter.HasFlag(MessageFilter.Priority_Urgent))
            {
                oParams.Add(new KeyValuePair<string, string>("priority", "urgent"));
            }
            if (pFilter.HasFlag(MessageFilter.Read_False))
            {
                oParams.Add(new KeyValuePair<string, string>("read", "false"));
            }
            if (pFilter.HasFlag(MessageFilter.Read_True))
            {
                oParams.Add(new KeyValuePair<string, string>("read", "true"));
            }
            if (pFilter.HasFlag(MessageFilter.Type_Email))
            {
                oParams.Add(new KeyValuePair<string, string>("type", "email"));
            }
            if (pFilter.HasFlag(MessageFilter.Type_Fax))
            {
                oParams.Add(new KeyValuePair<string, string>("type", "fax"));
            }
            if (pFilter.HasFlag(MessageFilter.Type_Receipt))
            {
                oParams.Add(new KeyValuePair<string, string>("type", "receipt"));
            }
            if (pFilter.HasFlag(MessageFilter.Type_Voice))
            {
                oParams.Add(new KeyValuePair<string, string>("type", "voice"));
            }

            StringBuilder strUrl = new StringBuilder(pConnectionServer.BaseUrl);
            strUrl.Append("mailbox/folders/inbox/messages?userobjectid=");
            strUrl.Append(pUserObjectId);

            //tack on all the params that apply here
            foreach (KeyValuePair<string, string> oElement in oParams)
            {
                strUrl.Append("&");
                strUrl.Append(oElement.Key);
                strUrl.Append("=");
                strUrl.Append(oElement.Value);
            }

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCUPIResponse(strUrl.ToString(), MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            //don't return failure here - there may be no messages and that's ok.
            if (res.XMLElement == null || res.XMLElement.HasElements == false)
            {
                res.Success = true;
                return res;
            }

            //convert the XML into properties on the class instance.
            pMessage = GetMessagesFromXElements(pConnectionServer,pUserObjectId, res.XMLElement);
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
        private static List<UserMessage> GetMessagesFromXElements(ConnectionServer pConnetionServer, string pUserObjectId, XElement pXElement)
        {
            List<UserMessage> oMessageList = new List<UserMessage>();

            if (pConnetionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to GetMessagesFromXElements");
            }

            //pull out a set of XMLElements for each Message object returned using the power of LINQ
            var messages = from e in pXElement.Elements()
                           where e.Name.LocalName == "Message"
                           select e;

            //for each message returned in the list of messages from the XML, construct a message object using the elements associated with that 
            //message.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //typed objects.
            foreach (var oXmlMessage in messages)
            {
                UserMessage oMessage = new UserMessage(pConnetionServer, pUserObjectId);
                foreach (XElement oElement in oXmlMessage.Elements())
                {
                    //adds the XML property to the InterviewHandler object if the proeprty name is found as a property on the object.
                    pConnetionServer.SafeXMLFetch(oMessage, oElement);
                }

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
        public static WebCallResult GetMessageAttachment(ConnectionServer pConnectionServer, string pTargetLocalFilePath, string pMessageObjectId, 
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
            res = HTTPFunctions.DownloadMessageAttachment(pConnectionServer.BaseUrl,
                                                          pConnectionServer.LoginName,
                                                          pConnectionServer.LoginPw,
                                                          pTargetLocalFilePath, 
                                                          pUserObjectId, 
                                                          pMessageObjectId,
                                                          pAttachmentNumber);

            return res;
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
        public static WebCallResult GetMessageAttachmentCount(ConnectionServer pConnectionServer, string pMessageObjectId,string pUserObjectId, 
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
            string strUrl = string.Format(@"{0}messages/{1}?userobjectid={2}", pConnectionServer.BaseUrl,pMessageObjectId, pUserObjectId);

            res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success==false)
            {
                return res;
            }

            //count up all the elements that have a local name of "Attachments"- usually there's only one but there can potentially be 
            //many if it's a multiply forwarded message with lots of intros or the like.
            foreach (XElement oElement in res.XMLElement.Elements())
            {
                if (oElement.Name.LocalName == "Attachments")
                {
                    pAttachmentCount = oElement.Elements().Count();
                    return res;
                }
            }

            return res;
        }


        //public WebCallResult SendMessageStreamId(ConnectionServer pConnectionServer, string pStreamId,string pSenderUserObjectId,
        //                                         bool pIsUrgent, bool pIsSecure, bool pIsPrivate,params string[] pToAddresses)
        //{
        //    WebCallResult res = new WebCallResult();
        //    //TODO

        //    return res;
        //}


        //public WebCallResult SendMessageLocalWavFile(ConnectionServer pConnectionServer, string pStreamId, string pSenderUserObjectId,
        //                                 bool pIsUrgent, bool pIsSecure, bool pIsPrivate, params string[] pToAddresses)
        //{
        //    WebCallResult res = new WebCallResult();


        //    return res;
        //}



        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the SMTP address of the sender, the time arrived and the subject of the message.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("From: {1}, At:{2} Subject:{0}", Subject, From_SmtpAddress, ConvertFromMillisecondsToTimeDate(ArrivalTime));
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
        /// Gets the total attachment count for the message passed into it.  Typically the count is 1 for most voice mail messages, 
        /// however if it's a multiply forwarded message it can be higher.
        /// </summary>
        /// <param name="pAttachmentCount">
        /// Returns the total attachment count for the message
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetMessageAttachmentCount(out int pAttachmentCount)
        {
            return GetMessageAttachmentCount(this.HomeServer, this.MsgId, this.UserObjectId, out pAttachmentCount);
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

        #endregion
    }
}
