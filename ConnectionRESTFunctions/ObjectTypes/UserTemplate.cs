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
    /// The user template class provides the ability to enumerate, create, edit an delete user templates in the Connection directory.
    /// </summary>
    public class UserTemplate
    {

        #region Fields and Properties

        private string _alias;
        public string Alias
        {
            get { return _alias; }
            set
            {
                _alias = value;
                _changedPropList.Add("Alias", value);
            }
        }

        //you cannot change the primary call handler Id.
        private string _callHandlerObjectId;
        public string CallHandlerObjectId
        {
            get { return _callHandlerObjectId; }
            set
            {
                //only allow it to be changed if it's empty, otherwise this is a no-op
                if (string.IsNullOrEmpty(_callHandlerObjectId))
                    _callHandlerObjectId = value;
            }
        }

        private string _cosObjectId;
        public string CosObjectId
        {
            get { return _cosObjectId; }
            set
            {
                _cosObjectId = value;
                _changedPropList.Add("CosObjectId", value);
            }
        }

        //can't edit creation time.
        public DateTime CreationTime { get; set; }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                _changedPropList.Add("DisplayName", value);
            }
        }

        private bool _isVmEnrolled;
        /// <summary>
        /// Set to true if the user has completed their first time enrollment conversation, false if they have not.
        /// Set this to false to turn on first time enrollment for a user.
        /// </summary>
        public bool IsVmEnrolled
        {
            get { return _isVmEnrolled; }
            set
            {
                _isVmEnrolled = value;
                _changedPropList.Add("IsVmEnrolled", value);
            }
        }

        private int _language;
        /// <summary>
        /// The preferred language of this user. For a user with a voice mailbox, it is the language in which the subscriber hears instructions 
        /// played to them. If the subscriber has TTS enabled by their COS, it is the language used for TTS
        /// </summary>
        public int Language
        {
            get { return _language; }
            set
            {
                _language = value;
                _changedPropList.Add("Language", value);
            }
        }

        private bool _listInDirectory;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection should list the subscriber in the phone directory for outside callers.
        /// This does not affect the ability of other users from finding them when addressing messages.
        /// </summary>
        public bool ListInDirectory
        {
            get { return _listInDirectory; }
            set
            {
                _listInDirectory = value;
                _changedPropList.Add("ListInDirectory", value);
            }
        }

        //you cannot change the location objectId
        private string _locationObjectId;
        public string LocationObjectId
        {
            get { return _locationObjectId; }
            set
            {
                //only allow it to be updated if it's empty, otherwise this is a no-op
                if (string.IsNullOrEmpty(_locationObjectId))
                    _locationObjectId = value;
            }
        }

        private string _mediaSwitchObjectId;
        /// <summary>
        /// The unique identifier of the MediaSwitch object Cisco Unity Connection uses for subscriber Telephone Record and Playback (TRAP) sessions 
        /// and to dial MWI on or off requests when the Cisco Unity Connection system has a dual switch integration.
        /// </summary>
        public string MediaSwitchObjectId
        {
            get { return _mediaSwitchObjectId; }
            set
            {
                _mediaSwitchObjectId = value;
                _changedPropList.Add("MediaSwitchObjectId", value);
            }
        }

        /// <summary>
        /// Unique identifier for the user - this cannot be changed for an existing user.
        /// </summary>
        private string _objectId;
        public string ObjectId
        {
            get { return _objectId; }
            set
            {
                //only allow it to be set if it's empty
                if (string.IsNullOrEmpty(_objectId))
                    _objectId = value;
            }
        }

        private string _partitionObjectId;
        /// <summary>
        /// The unique identifier of the Partition to which the DtmfAccessId is assigned
        /// </summary>
        public string PartitionObjectId
        {
            get { return _partitionObjectId; }
            set
            {
                _partitionObjectId = value;
                _changedPropList.Add("PartitionObjectId", value);
            }
        }

        private int _timeZone;
        public int TimeZone
        {
            get { return _timeZone; }
            set
            {
                _timeZone = value;
                _changedPropList.Add("TimeZone", value);
            }
        }

        private bool _voiceNameRequired;
        public bool VoiceNameRequired
        {
            get { return _voiceNameRequired; }
            set
            {
                _voiceNameRequired = value;
                _changedPropList.Add("VoiceNameRequired", value);
            }
        }


        private bool _addressAfterRecord;
        /// <summary>
        /// A flag indicating whether the subscriber will be prompted to address message before or after it is recorded
        /// </summary>
        public bool AddressAfterRecord
        {
            get { return _addressAfterRecord; }
            set
            {
                _changedPropList.Add("AddressAfterRecord", value);
                _addressAfterRecord = value;
            }
        }

        private int _addressMode;
        /// <summary>
        /// The default method the subscriber will use to address messages to other subscribers.
        /// 1=Extension, 2=FirstNameFirst, 0=LastNameFirst
        /// </summary>
        public int AddressMode
        {
            get { return _addressMode; }
            set
            {
                _changedPropList.Add("AddressMode", value);
                _addressMode = value;
            }
        }

        //while the schema lists these as included, they don't get returned from any version I've tested.
        //public string AltFirstFame { get; set; }
        //public string AltLastName { get; set; }

        private int _announceUpcomingMeetings;
        public int AnnounceUpcomingMeetings
        {
            get { return _announceUpcomingMeetings; }
            set
            {
                _changedPropList.Add("AnnounceUpcomingMeetings", value);
                _announceUpcomingMeetings = value;
            }
        }

        private int _assistantRowsPerPage;
        /// <summary>
        /// This controls the number of entries to display per page for all tables in the Unity Assistant, e.g. the Private List Members table
        /// </summary>
        public int AssistantRowsPerPage
        {
            get { return _assistantRowsPerPage; }
            set
            {
                _changedPropList.Add("AssistantRowsPerPage", value);
                _assistantRowsPerPage = value;
            }
        }

        private bool _autoAdvanceMsgs;
        public bool AutoAdvanceMsgs
        {
            get { return _autoAdvanceMsgs; }
            set
            {
                _changedPropList.Add("AutoAdvanceMsgs", value);
                _autoAdvanceMsgs = value;
            }
        }


        private int _callAnswerTimeout;
        /// <summary>
        /// The number of rings to wait for a subscriber destination to answer before the call is forwarded to the subscriber's primary phone
        /// </summary>
        public int CallAnswerTimeout
        {
            get { return _callAnswerTimeout; }
            set
            {
                _changedPropList.Add("CallAnswerTimeout", value);
                _callAnswerTimeout = value;
            }
        }


        private int _clockMode;
        /// <summary>
        /// The time format used for the message timestamps that the subscriber hears when they listen to their messages over the phone.
        /// 1=HourClock12, 2=HourClock24, 0=SystemDefaultClock
        /// </summary>
        public int ClockMode
        {
            get { return _clockMode; }
            set
            {
                _changedPropList.Add("ClockMode", value);
                _clockMode = value;
            }
        }

        private int _confirmationConfidenceThreshold;
        /// <summary>
        /// Voice Recognition Confirmation Confidence Threshold
        /// </summary>
        public int ConfirmationConfidenceThreshold
        {
            get { return _confirmationConfidenceThreshold; }
            set
            {
                _changedPropList.Add("ConfirmationConfidenceThreshold", value);
                _confirmationConfidenceThreshold = value;
            }
        }

        private bool _confirmDeleteMessage;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will request confirmation from a subscriber before proceeding with a deletion 
        /// of a single new or saved message
        /// </summary>
        public bool ConfirmDeleteMessage
        {
            get { return _confirmDeleteMessage; }
            set
            {
                _changedPropList.Add("ConfirmDeleteMessage", value);
                _confirmDeleteMessage = value;
            }
        }

        private bool _confirmDeleteDeletedMessage;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection will request confirmation from a subscriber before proceeding with a deletion of a 
        /// single deleted message
        /// </summary>
        public bool ConfirmDeleteDeletedMessage
        {
            get { return _confirmDeleteDeletedMessage; }
            set
            {
                _changedPropList.Add("ConfirmDeleteDeletedMessage", value);
                _confirmDeleteDeletedMessage = value;
            }
        }

        private bool _confirmDeleteMultipleMessages;
        public bool ConfirmDeleteMultipleMessages
        {
            get { return _confirmDeleteMultipleMessages; }
            set
            {
                _changedPropList.Add("ConfirmDeleteMultipleMessages", value);
                _confirmDeleteMultipleMessages = value;
            }
        }

        private bool _continuousAddMode;
        /// <summary>
        /// A flag indicating whether when addressing, after entering one recipient name, whether the subscriber is asked to enter another name 
        /// or assume the subscriber is finished adding names and is ready to move on to recording the message or applying message options
        /// </summary>
        public bool ContinuousAddMode
        {
            get { return _continuousAddMode; }
            set
            {
                _changedPropList.Add("ContinuousAddMode", value);
                _continuousAddMode = value;
            }
        }

        private string _conversationTui;
        /// <summary>
        /// The name of the conversation the subscriber uses to set up, send, and retrieve messages when using touch tones
        /// </summary>
        public string ConversationTui
        {
            get { return _conversationTui; }
            set
            {
                _changedPropList.Add("ConversationTui", value);
                _conversationTui = value;
            }
        }

        private string _conversationVui;
        /// <summary>
        /// The name of the conversation the subscriber uses to set up, send, and retrieve messages when using voice.  Currently there is only
        /// a single VUI driven conversation so there's no need to edit this.
        /// </summary>
        public string ConversationVui
        {
            get { return _conversationVui; }
            set
            {
                _changedPropList.Add("ConversationVui", value);
                _conversationVui = value;
            }
        }

        private int _commandDigitTimeout;
        /// <summary>
        /// The amount of time (in milliseconds) between digits on a multiple digit menu command entry (i.e. different than the inter digit timeout 
        /// that is used for strings of digits such as extensions and transfer strings).
        /// </summary>
        public int CommandDigitTimeout
        {
            get { return _commandDigitTimeout; }
            set
            {
                _changedPropList.Add("CommandDigitTimeout", value);
                _commandDigitTimeout = value;
            }
        }

        private int _delayAfterGreeting;
        /// <summary>
        /// The amount of time (in milliseconds) Cisco Unity Connection will delay after playing greeting
        /// </summary>
        public int DelayAfterGreeting
        {
            get { return _delayAfterGreeting; }
            set
            {
                _changedPropList.Add("DelayAfterGreeting", value);
                _delayAfterGreeting = value;
            }
        }

        private int _deletedMessageSortOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection presents deleted messages to the subscriber.
        /// 2=FIFO, 1=LIFO
        /// </summary>
        public int DeletedMessageSortOrder
        {
            get { return _deletedMessageSortOrder; }
            set
            {
                _changedPropList.Add("DeletedMessageSortOrder", value);
                _deletedMessageSortOrder = value;
            }
        }

        private bool _createSmtpProxyFromCorp;
        public bool CreateSmtpProxyFromCorp
        {
            get { return _createSmtpProxyFromCorp; }
            set
            {
                _changedPropList.Add("CreateSmtpProxyFromCorp", value);
                _createSmtpProxyFromCorp = value;
            }
        }


        private int _displayNameRule;
        public int DisplayNameRule
        {
            get { return _displayNameRule; }
            set
            {
                _changedPropList.Add("DisplayNameRule", value);
                _displayNameRule = value;
            }
        }

        private bool _doesntExpire;
        public bool DoesntExpire
        {
            get { return _doesntExpire; }
            set
            {
                _changedPropList.Add("DoesntExpire", value);
                _doesntExpire = value;
            }
        }

        private bool _cantChange;
        public bool CantChange
        {
            get { return _cantChange; }
            set
            {
                _changedPropList.Add("CantChange", value);
                _cantChange = value;
            }
        }


        private bool _enablePersonalRules;
        /// <summary>
        /// A flag indicating whether a subscriber's personal rules are enabled. Subscribers can use this setting to disable all personal rules at once
        /// </summary>
        public bool EnablePersonalRules
        {
            get { return _enablePersonalRules; }
            set
            {
                _changedPropList.Add("EnablePersonalRules", value);
                _enablePersonalRules = value;
            }
        }

        private bool _enableMessageLocator;
        /// <summary>
        /// A flag indicating whether the message locator feature is enabled for the subscriber
        /// </summary>
        public bool EnableMessageLocator
        {
            get { return _enableMessageLocator; }
            set
            {
                _changedPropList.Add("EnableMessageLocator", value);
                _enableMessageLocator = value;
            }
        }

        private bool _enableVisualMessageLocator;
        public bool EnableVisualMessageLocator
        {
            get { return _enableVisualMessageLocator; }
            set
            {
                _changedPropList.Add("EnableVisualMessageLocator", value);
                _enableVisualMessageLocator = value;
            }
        }

        private bool _enableMessageBookmark;
        public bool EnableMessageBookmark
        {
            get { return _enableMessageBookmark; }
            set
            {
                _changedPropList.Add("EnableMessageBookmark", value);
                _enableMessageBookmark = value;
            }
        }

        private bool _enableSaveDraft;
        public bool EnableSaveDraft
        {
            get { return _enableSaveDraft; }
            set
            {
                _changedPropList.Add("EnableSaveDraft", value);
                _enableSaveDraft = value;
            }
        }


        private bool _enableTts;
        /// <summary>
        /// A flag indicating whether TTS is enabled for the subscriber. Only relevant if TTS enabled in User's COS also
        /// </summary>
        public bool EnableTts
        {
            get { return _enableTts; }
            set
            {
                _changedPropList.Add("EnableTts", value);
                _enableTts = value;
            }
        }

        private bool _encryptPrivateMessages;
        public bool EncryptPrivateMessages
        {
            get { return _encryptPrivateMessages; }
            set
            {
                _changedPropList.Add("EncryptPrivateMessages", value);
                _encryptPrivateMessages = value;
            }
        }

        private bool _enAltGreetDontRingPhone;
        /// <summary>
        /// A flag indicating whether a caller is prevented from being transferred to the subscriber phone when the subscriber alternate greeting is turned on
        /// </summary>
        public bool EnAltGreetDontRingPhone
        {
            get { return _enAltGreetDontRingPhone; }
            set
            {
                _changedPropList.Add("EnAltGreetDontRingPhone", value);
                _enAltGreetDontRingPhone = value;
            }
        }

        private bool _enAltGreetPreventSkip;
        /// <summary>
        /// A flag indicating whether callers can skip the greeting while it is playing when the alternate greeting is turned on
        /// </summary>
        public bool EnAltGreetPreventSkip
        {
            get { return _enAltGreetPreventSkip; }
            set
            {
                _changedPropList.Add("EnAltGreetPreventSkip", value);
                _enAltGreetPreventSkip = value;
            }
        }

        private bool _enAltGreetPreventMsg;
        /// <summary>
        /// A flag indicating whether callers can leave a message after the greeting when the subscriber alternate greeting is turned on
        /// </summary>
        public bool EnAltGreetPreventMsg
        {
            get { return _enAltGreetPreventMsg; }
            set
            {
                _changedPropList.Add("EnAltGreetPreventMsg", value);
                _enAltGreetPreventMsg = value;
            }
        }

        private string _enhancedSecurityAlias;
        /// <summary>
        /// The unique text name used to idenitify and authenticate the user with an RSA SecurID security system
        /// </summary>
        public string EnhancedSecurityAlias
        {
            get { return _enhancedSecurityAlias; }
            set
            {
                _changedPropList.Add("EnhancedSecurityAlias", value);
                _enhancedSecurityAlias = value;
            }
        }

        private int _exitAction;
        /// <summary>
        /// The type of call action to take, e.g., hang-up, goto another object, etc
        /// 3=Error, 2=Goto, 1=Hangup, 0=Ignore, 5=SkipGreeting, 4=TakeMsg, 6=RestartGreeting, 7=TransferAltContact, 8=RouteFromNextRule
        /// </summary>
        public int ExitAction
        {
            get { return _exitAction; }
            set
            {
                _changedPropList.Add("ExitAction", value);
                _exitAction = value;
            }
        }

        private string _exitCallActionObjectId;
        public string ExitCallActionObjectId
        {
            get { return _exitCallActionObjectId; }
            set
            {
                _changedPropList.Add("ExitCallActionObjectId", value);
                _exitCallActionObjectId = value;
            }
        }

        private string _exitTargetConversation;
        public string ExitTargetConversation
        {
            get { return _exitTargetConversation; }
            set
            {
                _changedPropList.Add("ExitTargetConversation", value);
                _exitTargetConversation = value;
            }
        }

        private string _exitTargetHandlerObjectId;
        public string ExitTargetHandlerObjectId
        {
            get { return _exitTargetHandlerObjectId; }
            set
            {
                _changedPropList.Add("ExitTargetHandlerObjectId", value);
                _exitTargetHandlerObjectId = value;
            }
        }

   
        private int _firstDigitTimeout;
        /// <summary>
        /// The amount of time to wait (in milliseconds) for first digit when collecting touch tones
        /// </summary>
        public int FirstDigitTimeout
        {
            get { return _firstDigitTimeout; }
            set
            {
                _changedPropList.Add("FirstDigitTimeout", value);
                _firstDigitTimeout = value;
            }
        }

        private bool _greetByName;
        /// <summary>
        /// A flag indicating whether the subscriber hears his/her name when they log into their mailbox over the phone
        /// </summary>
        public bool GreetByName
        {
            get { return _greetByName; }
            set
            {
                _changedPropList.Add("GreetByName", value);
                _greetByName = value;
            }
        }

        private int _inboxAutoRefresh;
        /// <summary>
        /// The rate (in minutes) at which Unity Inbox performs a refresh
        /// </summary>
        public int InboxAutoRefresh
        {
            get { return _inboxAutoRefresh; }
            set
            {
                _changedPropList.Add("InboxAutoRefresh", value);
                _inboxAutoRefresh = value;
            }
        }

        private bool _inboxAutoResolveMessageRecipients;
        public bool InboxAutoResolveMessageRecipients
        {
            get { return _inboxAutoResolveMessageRecipients; }
            set
            {
                _changedPropList.Add("InboxAutoResolveMessageRecipients", value);
                _inboxAutoResolveMessageRecipients = value;
            }
        }

        private int _inboxMessagesPerPage;
        public int InboxMessagesPerPage
        {
            get { return _inboxMessagesPerPage; }
            set
            {
                _changedPropList.Add("InboxMessagesPerPage", value);
                _inboxMessagesPerPage = value;
            }
        }

        private int _interdigitDelay;
        /// <summary>
        /// The amount of time to wait (in milliseconds) for input between touch tones when collecting digits in TUI
        /// </summary>
        public int InterdigitDelay
        {
            get { return _interdigitDelay; }
            set
            {
                _changedPropList.Add("InterdigitDelay", value);
                _interdigitDelay = value;
            }
        }

        private bool _isClockMode24Hour;
        public bool IsClockMode24Hour
        {
            get { return _isClockMode24Hour; }
            set
            {
                _changedPropList.Add("IsClockMode24Hour", value);
                _isClockMode24Hour = value;
            }
        }

        private bool _isSetForVmEnrollment;
        /// <summary>
        /// Temporary placeholder until IsVmEnrolled can be phased out. At that point, delete this column and rename tbl_UserSubscriber.IsVmEnrolled 
        /// to IsSetForVmEnrollment. A flag indicating whether Cisco Unity Connection plays the enrollment conversation (record a voice name, 
        /// indicate if they are listed in the directory, etc.) for the subscriber when they login
        /// </summary>
        public bool IsSetForVmEnrollment
        {
            get { return _isSetForVmEnrollment; }
            set
            {
                _changedPropList.Add("IsSetForVmEnrollment", value);
                _isSetForVmEnrollment = value;
            }
        }

        private bool _jumpToMessagesOnLogin;
        /// <summary>
        /// A flag indicating whether the subscriber conversation jumps directly to the first message in the message stack after subscriber sign-in
        /// </summary>
        public bool JumpToMessagesOnLogin
        {
            get { return _jumpToMessagesOnLogin; }
            set
            {
                _changedPropList.Add("JumpToMessagesOnLogin", value);
                _jumpToMessagesOnLogin = value;
            }
        }

        public int LdapType { get; set; }


        private string _mailboxStoreObjectId;
        public string MailboxStoreObjectId
        {
            get { return _mailboxStoreObjectId; }
            set
            {
                _changedPropList.Add("MailboxStoreObjectId", value);
                _mailboxStoreObjectId = value;
            }
        }

        private string _messageAgingPolicyObjectId;
        public string MessageAgingPolicyObjectId
        {
            get { return _messageAgingPolicyObjectId; }
            set
            {
                _changedPropList.Add("MessageAgingPolicyObjectId", value);
                _messageAgingPolicyObjectId = value;
            }
        }


        private int _messageLocatorSortOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection will sort messages when the "Message Locator" feature is enabled.
        /// 2=FIFO, 1=LIFO
        /// </summary>
        public int MessageLocatorSortOrder
        {
            get { return _messageLocatorSortOrder; }
            set
            {
                _changedPropList.Add("MessageLocatorSortOrder", value);
                _messageLocatorSortOrder = value;
            }
        }

        private bool _messageTypeMenu;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection plays the message type menu when the subscriber logs on to Cisco Unity Connection over the phone
        /// </summary>
        public bool MessageTypeMenu
        {
            get { return _messageTypeMenu; }
            set
            {
                _changedPropList.Add("MessageTypeMenu", value);
                _messageTypeMenu = value;
            }
        }

        private bool _nameConfirmation;
        /// <summary>
        /// Indicates whether the voice name of the subscriber or distribution list added to an address list when a subscriber addresses a message to other 
        /// subscribers is played. The default value for this is off (no voice name played) since the voice name was just played as part of the list of matches . 
        /// To most users this sounds redundant when on, but some users prefer it.
        /// </summary>
        public bool NameConfirmation
        {
            get { return _nameConfirmation; }
            set
            {
                _changedPropList.Add("NameConfirmation", value);
                _nameConfirmation = value;
            }
        }

        private int _newMessageSortOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection will sort new messages.
        /// 2=FIFO, 1=LIFO
        /// </summary>
        public int NewMessageSortOrder
        {
            get { return _newMessageSortOrder; }
            set
            {
                _changedPropList.Add("NewMessageSortOrder", value);
                _newMessageSortOrder = value;
            }
        }

        private string _newMessageStackOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection plays the following types of "new" messages: * Urgent voice messages * Non-urgent voice messages * 
        /// Urgent fax messages * Non-urgent fax messages * Urgent e-mail messages * Non-urgent e-mail messages * Receipts and notices
        /// </summary>
        public string NewMessageStackOrder
        {
            get { return _newMessageStackOrder; }
            set
            {
                _changedPropList.Add("NewMessageStackOrder", value);
                _newMessageStackOrder = value;
            }
        }

        private int _notificationType;
        public int NotificationType
        {
            get { return _notificationType; }
            set
            {
                _changedPropList.Add("NotificationType", value);
                _notificationType = value;
            }
        }



        private int _pcaAddressBookRowsPerPage;
        public int PcaAddressBookRowsPerPage
        {
            get { return _pcaAddressBookRowsPerPage; }
            set
            {
                _changedPropList.Add("PcaAddressBookRowsPerPage", value);
                _pcaAddressBookRowsPerPage = value;
            }
        }

        private int _promptSpeed;
        /// <summary>
        /// The audio speed Cisco Unity Connection uses to play back prompts to the subscriber.
        /// 50 (slow) 100 (normal) 150 (faster) 200 (fastest)
        /// </summary>
        public int PromptSpeed
        {
            get { return _promptSpeed; }
            set
            {
                _changedPropList.Add("PromptSpeed", value);
                _promptSpeed = value;
            }
        }

        private int _promptVolume;
        /// <summary>
        /// The volume level for playback of system prompts
        /// 50 is normal, 25 is quiet and 100 is loud.
        /// </summary>
        public int PromptVolume
        {
            get { return _promptVolume; }
            set
            {
                _changedPropList.Add("PromptVolume", value);
                _promptVolume = value;
            }
        }

        private bool _readOnly;
        /// <summary>
        /// set to true user is read-only and cannot be modified via SA.
        /// </summary>
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _changedPropList.Add("ReadOnly", value);
                _readOnly = value;
            }
        }

        private int _receiveQuota;
        public int ReceiveQuota
        {
            get { return _receiveQuota; }
            set
            {
                _changedPropList.Add("ReceiveQuota", value);
                _receiveQuota = value;
            }
        }


        private bool _recordUnknownCallerName;
        /// <summary>
        /// A flag indicating whether a caller should be promoted to record his/her name if Unity does not receive caller id
        /// </summary>
        public bool RecordUnknownCallerName
        {
            get { return _recordUnknownCallerName; }
            set
            {
                _changedPropList.Add("RecordUnknownCallerName", value);
                _recordUnknownCallerName = value;
            }
        }

        private int _repeatMenu;
        /// <summary>
        /// The number of times to repeat a menu in TUI
        /// </summary>
        public int RepeatMenu
        {
            get { return _repeatMenu; }
            set
            {
                _changedPropList.Add("RepeatMenu", value);
                _repeatMenu = value;
            }
        }

        private bool _retainUrgentMessageFlag;
        public bool RetainUrgentMessageFlag
        {
            get { return _retainUrgentMessageFlag; }
            set
            {
                _changedPropList.Add("RetainUrgentMessageFlag", value);
                _retainUrgentMessageFlag = value;
            }
        }


        private bool _ringPrimaryPhoneFirst;
        /// <summary>
        /// A flag indicating whether a subscriber's primary phone should be rung before trying other destinations in a personal group
        /// </summary>
        public bool RingPrimaryPhoneFirst
        {
            get { return _ringPrimaryPhoneFirst; }
            set
            {
                _changedPropList.Add("RingPrimaryPhoneFirst", value);
                _ringPrimaryPhoneFirst = value;
            }
        }

        private bool _routeNdrToSender;
        /// <summary>
        /// A flag indicating, for an undeliverable message, whether NDR messages will appear in the subscriber's mailbox or are deleted by the system
        /// </summary>
        public bool RouteNDRToSender
        {
            get { return _routeNdrToSender; }
            set
            {
                _changedPropList.Add("RouteNDRToSender", value);
                _routeNdrToSender = value;
            }
        }

        private int _savedMessageSortOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection will sort saved messages
        /// 2=FIFO, 1=LIFO
        /// </summary>
        public int SavedMessageSortOrder
        {
            get { return _savedMessageSortOrder; }
            set
            {
                _changedPropList.Add("SavedMessageSortOrder", value);
                _savedMessageSortOrder = value;
            }
        }

        private string _savedMessageStackOrder;
        /// <summary>
        /// The order in which Cisco Unity Connection plays the following types of "saved" messages: * Urgent voice messages * Non-urgent voice messages 
        /// * Urgent fax messages * Non-urgent fax messages * Urgent e-mail messages * Non-urgent e-mail messages * Receipts and notices
        /// </summary>
        public string SavedMessageStackOrder
        {
            get { return _savedMessageStackOrder; }
            set
            {
                _changedPropList.Add("SavedMessageStackOrder", value);
                _savedMessageStackOrder = value;
            }
        }

        private bool _saveMessageOnHangup;
        /// <summary>
        /// A flag indicating when hanging up while listening to a new message, whether the message is marked new again or is marked read
        /// </summary>
        public bool SaveMessageOnHangup
        {
            get { return _saveMessageOnHangup; }
            set
            {
                _changedPropList.Add("SaveMessageOnHangup", value);
                _saveMessageOnHangup = value;
            }
        }

        private bool _sayAltGreetWarning;
        /// <summary>
        /// A flag indicating whether Cisco Unity Connection notifies the subscriber when they login via the phone (plays conversation) or CPCA 
        /// (displays a warning banner) if their alternate greeting is turned on.
        /// </summary>
        public bool SayAltGreetWarning
        {
            get { return _sayAltGreetWarning; }
            set
            {
                _changedPropList.Add("SayAltGreetWarning", value);
                _sayAltGreetWarning = value;
            }
        }

        private bool _sayAni;
        public bool SayAni
        {
            get { return _sayAni; }
            set
            {
                _changedPropList.Add("SayAni", value);
                _sayAni = value;
            }
        }

        private bool _sayAniAfter;
        public bool SayAniAfter
        {
            get { return _sayAniAfter; }
            set
            {
                _changedPropList.Add("SayAniAfter", value);
                _sayAniAfter = value;
            }
        }

        private bool _sayCopiedNames;
        public bool SayCopiedNames
        {
            get { return _sayCopiedNames; }
            set
            {
                _changedPropList.Add("SayCopiedNames", value);
                _sayCopiedNames = value;
            }
        }

        private bool _sayDistributionList;
        public bool SayDistributionList
        {
            get { return _sayDistributionList; }
            set
            {
                _changedPropList.Add("SayDistributionList", value);
                _sayDistributionList = value;
            }
        }

        private bool _sayMessageLength;
        public bool SayMessageLength
        {
            get { return _sayMessageLength; }
            set
            {
                _changedPropList.Add("SayMessageLength", value);
                _sayMessageLength = value;
            }
        }

        private bool _sayMessageLengthAfter;
        public bool SayMessageLengthAfter
        {
            get { return _sayMessageLengthAfter; }
            set
            {
                _changedPropList.Add("SayMessageLengthAfter", value);
                _sayMessageLengthAfter = value;
            }
        }

        private bool _sayMsgNumber;
        public bool SayMsgNumber
        {
            get { return _sayMsgNumber; }
            set
            {
                _changedPropList.Add("SayMsgNumber", value);
                _sayMsgNumber = value;
            }
        }

        private bool _sayMsgNumberAfter;
        public bool SayMsgNumberAfter
        {
            get { return _sayMsgNumberAfter; }
            set
            {
                _changedPropList.Add("SayMsgNumberAfter", value);
                _sayMsgNumberAfter = value;
            }
        }

        private bool _saySender;
        public bool SaySender
        {
            get { return _saySender; }
            set
            {
                _changedPropList.Add("SaySender", value);
                _saySender = value;
            }
        }

        private bool _saySenderAfter;
        public bool SaySenderAfter
        {
            get { return _saySenderAfter; }
            set
            {
                _changedPropList.Add("SaySenderAfter", value);
                _saySenderAfter = value;
            }
        }


        private bool _saySenderExtension;
        public bool SaySenderExtension
        {
            get { return _saySenderExtension; }
            set
            {
                _changedPropList.Add("SaySenderExtension", value);
                _saySenderExtension = value;
            }
        }

        private bool _saySenderExtensionAfter;
        public bool SaySenderExtensionAfter
        {
            get { return _saySenderExtensionAfter; }
            set
            {
                _changedPropList.Add("SaySenderExtensionAfter", value);
                _saySenderExtensionAfter = value;
            }
        }


        private bool _sayTimestampAfter;
        public bool SayTimestampAfter
        {
            get { return _sayTimestampAfter; }
            set
            {
                _changedPropList.Add("SayTimestampAfter", value);
                _sayTimestampAfter = value;
            }
        }

        private bool _sayTimestampBefore;
        public bool SayTimestampBefore
        {
            get { return _sayTimestampBefore; }
            set
            {
                _changedPropList.Add("SayTimestampBefore", value);
                _sayTimestampBefore = value;
            }
        }

        private bool _sayTotalDraftMsg;
        public bool SayTotalDraftMsg
        {
            get { return _sayTotalDraftMsg; }
            set
            {
                _changedPropList.Add("SayTotalDraftMsg", value);
                _sayTotalDraftMsg = value;
            }
        }


        private bool _sayTotalNew;
        public bool SayTotalNew
        {
            get { return _sayTotalNew; }
            set
            {
                _changedPropList.Add("SayTotalNew", value);
                _sayTotalNew = value;
            }
        }

        private bool _sayTotalNewEmail;
        public bool SayTotalNewEmail
        {
            get { return _sayTotalNewEmail; }
            set
            {
                _changedPropList.Add("SayTotalNewEmail", value);
                _sayTotalNewEmail = value;
            }
        }

        private bool _sayTotalNewFax;
        public bool SayTotalNewFax
        {
            get { return _sayTotalNewFax; }
            set
            {
                _changedPropList.Add("SayTotalNewFax", value);
                _sayTotalNewFax = value;
            }
        }

        private bool _sayTotalNewVoice;
        public bool SayTotalNewVoice
        {
            get { return _sayTotalNewVoice; }
            set
            {
                _changedPropList.Add("SayTotalNewVoice", value);
                _sayTotalNewVoice = value;
            }
        }

        private bool _sayTotalReceipts;
        public bool SayTotalReceipts
        {
            get { return _sayTotalReceipts; }
            set
            {
                _changedPropList.Add("SayTotalReceipts", value);
                _sayTotalReceipts = value;
            }
        }

        private bool _sayTotalSaved;
        public bool SayTotalSaved
        {
            get { return _sayTotalSaved; }
            set
            {
                _changedPropList.Add("SayTotalSaved", value);
                _sayTotalSaved = value;
            }
        }

        private string _searchByExtensionSearchSpaceObjectId;
        /// <summary>
        /// The unique identifier of the SearchSpace which is used to limit the visibility to dialable/addressable objects when searching by extension (dial string).
        /// </summary>
        public string SearchByExtensionSearchSpaceObjectId
        {
            get { return _searchByExtensionSearchSpaceObjectId; }
            set
            {
                _changedPropList.Add("SearchByExtensionSearchSpaceObjectId", value);
                _searchByExtensionSearchSpaceObjectId = value;
            }
        }

        private string _searchByNameSearchSpaceObjectId;
        /// <summary>
        /// The unique identifier of the SearchSpace which is used to limit the visibility to dialable/addressable objects when searching by name (character string)
        /// </summary>
        public string SearchByNameSearchSpaceObjectId
        {
            get { return _searchByNameSearchSpaceObjectId; }
            set
            {
                _changedPropList.Add("SearchByNameSearchSpaceObjectId", value);
                _searchByNameSearchSpaceObjectId = value;
            }
        }

        private bool _sendBroadcastMsg;
        /// <summary>
        /// A flag indicating whether the subscriber may send broadcast messages
        /// </summary>
        public bool SendBroadcastMsg
        {
            get { return _sendBroadcastMsg; }
            set
            {
                _changedPropList.Add("SendBroadcastMsg", value);
                _sendBroadcastMsg = value;
            }
        }

        private int _sendMessageOnHangup;
        /// <summary>
        /// An enum indicating when hanging up while addressing a message that has a recording and at least one recipient, whether the message is discarded, 
        /// sent or saved as a draft message if the subscriber explicitly issues the command to send the message either via DTMF or voice input.
        /// 0=Discard, 1=Send, 2=Save
        /// </summary>
        public int SendMessageOnHangup
        {
            get { return _sendMessageOnHangup; }
            set
            {
                _changedPropList.Add("SendMessageOnHangup", value);
                _sendMessageOnHangup = value;
            }
        }

        private int _sendReadReceipts;
        public int SendReadReceipts
        {
            get { return _sendReadReceipts; }
            set
            {
                _changedPropList.Add("SendReadReceipts", value);
                _sendReadReceipts = value;
            }
        }

        private int _sendQuota;
        public int SendQuota
        {
            get { return _sendQuota; }
            set
            {
                _changedPropList.Add("SendQuota", value);
                _sendQuota = value;
            }
        }

        private int _skipForwardTime;
        /// <summary>
        /// Indicates the amount of time (in milliseconds) to jump forward when skipping ahead in a voice or TTS message using either DTMF or voice 
        /// commands while reviewing messages
        /// </summary>
        public int SkipForwardTime
        {
            get { return _skipForwardTime; }
            set
            {
                _changedPropList.Add("SkipForwardTime", value);
                _skipForwardTime = value;
            }
        }

        private bool _skipPasswordForKnownDevice;
        /// <summary>
        /// A flag indicating whether the subscriber will be asked for his/her PIN when attempting to sign-in from a known device
        /// </summary>
        public bool SkipPasswordForKnownDevice
        {
            get { return _skipPasswordForKnownDevice; }
            set
            {
                _changedPropList.Add("SkipPasswordForKnownDevice", value);
                _skipPasswordForKnownDevice = value;
            }
        }

        private int _skipReverseTime;
        /// <summary>
        /// Indicates the amount of time (in milliseconds) to jump backward when skipping in reverse in a voice or TTS message using either DTMF or voice 
        /// commands while reviewing messages.
        /// </summary>
        public int SkipReverseTime
        {
            get { return _skipReverseTime; }
            set
            {
                _changedPropList.Add("SkipReverseTime", value);
                _skipReverseTime = value;
            }
        }

        private int _speechCompleteTimeout;
        /// <summary>
        /// Specifies the required length of silence (in milliseconds) following user speech before the recognizer finalizes a result (either accepting it 
        /// or throwing a nomatch event). The SpeechCompleteTimeout property is used when the speech prior to the silence matches an active grammar. A long 
        /// SpeechCompleteTimeout value delays the result completion and therefore makes the system's response slow. A short SpeechCompleteTimeout value may 
        /// lead to the inappropriate break up of an utterance. Reasonable SpeechCompleteTimeout values are typically in the range of 0.3 seconds to 1.0 
        /// seconds
        /// </summary>
        public int SpeechCompleteTimeout
        {
            get { return _speechCompleteTimeout; }
            set
            {
                _changedPropList.Add("SpeechCompleteTimeout", value);
                _speechCompleteTimeout = value;
            }
        }

        private int _speechConfidenceThreshold;
        /// <summary>
        /// When the engine matches a spoken phrase, it associates a confidence level with that conclusion. This parameter determines what confidence level 
        /// should be considered a successful match. A higher value means the engine is will report fewer successful matches, but it will be more confident 
        /// in the matches that it reports
        /// </summary>
        public int SpeechConfidenceThreshold
        {
            get { return _speechConfidenceThreshold; }
            set
            {
                _changedPropList.Add("SpeechConfidenceThreshold", value);
                _speechConfidenceThreshold = value;
            }
        }

        private int _speechIncompleteTimeout;
        /// <summary>
        /// Specifies the required length of silence (in milliseconds) from when the speech prior to the silence matches an active grammar, but where it is 
        /// possible to speak further and still match the grammar. By contrast, the SpeechCompleteTimeout property is used when the speech prior to the silence 
        /// matches an active grammar and no further words can be spoken. A long SpeechIncompleteTimeout value delays the result completion and therefore makes 
        /// the system's response slow. A short SpeechIncompleteTimeout value may lead to the inappropriate break up of an utterance. The SpeechIncompleteTimeout
        /// value is usually longer than the completetimeout value to allow users to pause mid-utterance (for example, to breathe). Note that values set for 
        /// the completetimeout property are only supported if they are less than the incompletetimeout property
        /// </summary>
        public int SpeechIncompleteTimeout
        {
            get { return _speechIncompleteTimeout; }
            set
            {
                _changedPropList.Add("SpeechIncompleteTimeout", value);
                _speechIncompleteTimeout = value;
            }
        }

        private int _speechSensitivity;
        /// <summary>
        /// A variable level of sound sensitivity that enables the speech engine to filter out background noise and not mistake it for speech. A higher value 
        /// means higher sensitivity
        /// </summary>
        public int SpeechSensitivity
        {
            get { return _speechSensitivity; }
            set
            {
                _changedPropList.Add("SpeechSensitivity", value);
                _speechSensitivity = value;
            }
        }

        private int _speechSpeedVsAccuracy;
        /// <summary>
        /// Tunes the engine towards Performance or Accuracy. A higher value for this setting means faster matches that may be less accurate. A lower value 
        /// for this setting means more accurate matches along with more processing and higher CPU utilization
        /// </summary>
        public int SpeechSpeedVsAccuracy
        {
            get { return _speechSpeedVsAccuracy; }
            set
            {
                _changedPropList.Add("SpeechSpeedVsAccuracy", value);
                _speechSpeedVsAccuracy = value;
            }
        }

        private int _speed;
        /// <summary>
        /// The audio speed Cisco Unity Connection uses to play back messages to the subscriber
        /// 50 (slow) 100 (normal) 150 (faster) 200 (fastest)        
        /// </summary>
        public int Speed
        {
            get { return _speed; }
            set
            {
                _changedPropList.Add("Speed", value);
                _speed = value;
            }
        }

        private string _synchScheduleObjectId;
        /// <summary>
        /// The unique identifier of the Schedule object to use for synchronization Calendar information from groupware (such as Exchange)
        /// </summary>
        public string SynchScheduleObjectId
        {
            get { return _synchScheduleObjectId; }
            set
            {
                _changedPropList.Add("SynchScheduleObjectId", value);
                _synchScheduleObjectId = value;
            }
        }

        private bool _undeletable;
        /// <summary>
        /// A flag indicating whether this subscriber can be deleted via an administrative application such as Cisco Unity Connection Administration. 
        /// It is used to prevent deletion of factory defaults
        /// </summary>
        public bool Undeletable
        {
            get { return _undeletable; }
            set
            {
                _changedPropList.Add("Undeletable", value);
                _undeletable = value;
            }
        }

        private bool _updateBroadcastMsg;
        /// <summary>
        /// A flag indicating whether the subscriber has the ability to update broadcast messages that are active or will be active in the future
        /// </summary>
        public bool UpdateBroadcastMsg
        {
            get { return _updateBroadcastMsg; }
            set
            {
                _changedPropList.Add("UpdateBroadcastMsg", value);
                _updateBroadcastMsg = value;
            }
        }

        private bool _useBriefPrompts;
        /// <summary>
        /// A flag indicating whether the subscriber hears brief or full phone menus when accessing Cisco Unity Connection over the phone
        /// </summary>
        public bool UseBriefPrompts
        {
            get { return _useBriefPrompts; }
            set
            {
                _changedPropList.Add("UseBriefPrompts", value);
                _useBriefPrompts = value;
            }
        }

        private bool _useDefaultLanguage;
        /// <summary>
        /// Set to true if call handler is using default language from the location it belongs to
        /// </summary>
        public bool UseDefaultLanguage
        {
            get { return _useDefaultLanguage; }
            set
            {
                _changedPropList.Add("UseDefaultLanguage", value);
                _useDefaultLanguage = value;
            }
        }

        private bool _useDefaultTimeZone;
        /// <summary>
        /// Indicates if the default timezone is being used
        /// </summary>
        public bool UseDefaultTimeZone
        {
            get { return _useDefaultTimeZone; }
            set
            {
                _changedPropList.Add("UseDefaultTimeZone", value);
                _useDefaultTimeZone = value;
            }
        }

        private bool _useDynamicNameSearchWeight;
        /// <summary>
        /// Use dynamic name search weight. When this user addresses objects, the name search weight for those objects will automatically be incremented
        /// </summary>
        public bool UseDynamicNameSearchWeight
        {
            get { return _useDynamicNameSearchWeight; }
            set
            {
                _changedPropList.Add("UseDynamicNameSearchWeight", value);
                _useDynamicNameSearchWeight = value;
            }
        }

        private bool _useShortPollForCache;
        /// <summary>
        /// A flag indicating whether the user's polling cycle for retrieving calendar information will be the shorter "power user" polling cycle
        /// </summary>
        public bool UseShortPollForCache
        {
            get { return _useShortPollForCache; }
            set
            {
                _changedPropList.Add("UseShortPollForCache", value);
                _useShortPollForCache = value;
            }
        }

        private bool _useVui;
        /// <summary>
        /// A flag indicating whether the speech recognition conversation is the default conversation for the subscriber
        /// </summary>
        public bool UseVui
        {
            get { return _useVui; }
            set
            {
                _changedPropList.Add("UseVui", value);
                _useVui = value;
            }
        }

        private int _volume;
        /// <summary>
        /// Volume at which messages are played back by default (can be changed by user during playback).
        ///  50 is normal, 25 is quiet and 100 is loud.
        /// </summary>
        public int Volume
        {
            get { return _volume; }
            set
            {
                _changedPropList.Add("Volume", value);
                _volume = value;
            }
        }

        private int _warningQuota;
        public int WarningQuota
        {
            get { return _warningQuota; }
            set
            {
                _changedPropList.Add("WarningQuota", value);
                _warningQuota = value;
            }
        }


        private CallHandler _primaryCallHandler;
        /// <summary>
        /// Funtion to fetch the PrimaryCallHandler of a user and return it as a CallHandler object instance.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>
        /// <returns>
        /// Instance of the CallHandler object is passed back.  If there's a problem the instance will be NULL.
        /// </returns>
        public CallHandler PrimaryCallHandler(bool pForceRefetchOfData = false)
        {
            if (pForceRefetchOfData)
            {
                _primaryCallHandler = null;
            }

            //fetch the primary call handler only if it's asked for.
            if (_primaryCallHandler == null)
            {
                GetPrimaryCallHandler(out _primaryCallHandler);
            }

            return _primaryCallHandler;
        }


        private PhoneSystem _phoneSystem;
        /// <summary>
        /// Funtion to fetch the Phone system of a user and return it as a PhoneSystem object instance.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>
        /// <returns>
        /// Instance of the PhoneSystem object is passed back.  If there's a problem the instance will be NULL.
        /// </returns>
        public PhoneSystem PhoneSystem(bool pForceRefetchOfData = false)
        {
            if (pForceRefetchOfData)
            {
                _phoneSystem = null;
            }
            //fetch the primary call handler only if it's asked for.
            if (_phoneSystem == null)
            {
                try
                {
                    _phoneSystem = new PhoneSystem(this.HomeServer, this.MediaSwitchObjectId);
                }
                catch {}
            }

            return _phoneSystem;
        }


        private List<NotificationDevice> _notificationDevices;
        /// <summary>
        /// Funtion to fetch all the notification devices of a user and return them as a list of NotificationDEvice objects.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>
        /// <returns>
        /// List of NotificationDevice objects - there should be at least 5 for every user, more if they have created additional
        /// devices.
        /// </returns>
        public List<NotificationDevice> NotificationDevices(bool pForceRefetchOfData = false)
        {
            //if the user wants to force a refetch of data, null out the private cache of notification devices.
            if (pForceRefetchOfData)
            {
                _notificationDevices = null;
            }
            //fetch notification device list only if it's asked for.
            if (_notificationDevices == null)
            {
                GetNotificationDevices(out _notificationDevices);
            }

            return _notificationDevices;
        }

        private ClassOfService _cos;

        /// <summary>
        /// Funtion to fetch the COS instance this user is associated with.
        /// This has to be implemented as a function, not property, so they don't get "lazy fetched" when you bind a list 
        /// of users to a grid, do a LINQ query on them or the like
        /// </summary>    
        /// <returns>
        /// instance of ClassOfService object for the COS this user is associated with.
        /// </returns>
        public ClassOfService Cos(bool pForceRefetchOfData = false)
        {
            if (pForceRefetchOfData)
            {
                _cos = null;
            }

            if (_cos == null)
            {
                _cos = new ClassOfService(this.HomeServer, this.CosObjectId);
            }

            return _cos;
        }



        private Credential _pin;

        /// <summary>
        /// Returns details of the PIN (phone password) settings - including if it's locked, time last changed, if it's set to must-change 
        /// etc... this object does NOT allow for editing of credentials - use the ResetUserPassword method off the User object for that.
        /// This is done as a method since if it's done as a property it'll attempt to fill it in from data pulled from Connection during
        /// populate of a user's data.
        /// </summary>
        /// <returns>
        /// instance of the credential object
        /// </returns>
        public Credential Pin()
        {
            if (_pin == null)
            {
                _pin = new Credential(this.HomeServer, this.ObjectId, CredentialType.Pin);
            }

            return _pin;
        }

        private Credential _password;

        /// <summary>
        /// Returns details of the Password (GUI password) settings - including if it's locked, time last changed, if it's set to must-change 
        /// etc... this object does NOT allow for editing of credentials - use the ResetUserPassword method off the User object for that.
        /// This is done as a method since if it's done as a property it'll attempt to fill it in from data pulled from Connection during
        /// populate of a user's data.
        /// </summary>
        /// <returns>
        /// instance of the credential object
        /// </returns>
        public Credential Password()
        {
            if (_password == null)
            {
                _password = new Credential(this.HomeServer, this.ObjectId, CredentialType.Password);
            }

            return _password;
        }        
        //reference to the ConnectionServer object used to create this user instance.
        public ConnectionServer HomeServer { get; private set; }

        //used to keep track of which properties have been updated
        private readonly ConnectionPropertyList _changedPropList;

        #endregion


        #region Constructors

        /// <summary>
        /// generic constructor used by JSON parser
        /// </summary>
        public UserTemplate()
        {
            _changedPropList = new ConnectionPropertyList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pConnectionServer"></param>
        /// <param name="pObjectId"></param>
        /// <param name="pAlias"></param>
        public UserTemplate(ConnectionServer pConnectionServer, string pObjectId="", string pAlias = ""):this()
        {
            if (pConnectionServer==null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to UserTemplate construtor");
            }

            HomeServer = pConnectionServer;
            
             if (pObjectId.Length == 0 & pAlias.Length == 0)
            {
                return;
            }

             WebCallResult res = this.GetUserTemplate(pObjectId, pAlias);
            
            if (res.Success == false)
            {
                throw new Exception(string.Format("UserTemplate not found in UserTemplate constructor using Alias={0} and/or ObjectId={1}\n\rError={2}"
                                                , pAlias, pObjectId, res.ErrorText));
            }
        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string containing the alias, display name and ObjectId of the user template
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}], {2}", Alias, DisplayName, ObjectId);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the template object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the user object instance.
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
        public WebCallResult RefetchUserTemplateData()
        {
            return GetUserTemplate(this.ObjectId);
        }

        /// <summary>
        /// Helper function to fill in the user instance with data from a user by their objectID string or their alias string.
        /// </summary>
        /// <param name="pObjectId">
        /// ObjectId of the user template to fetch - can be passed as a blank string if the alias is being used instead.
        /// </param>
        /// <param name="pAlias">
        /// Alias of the template to fetch.  If both the objectId and alias are passed, the objectId is used. 
        /// </param>
        /// <returns>
        /// instance of the WebCallResult class
        /// </returns>
        private WebCallResult GetUserTemplate(string pObjectId, string pAlias = "")
        {
            string strUrl;
            WebCallResult res;

            string strObjectId = pObjectId;
            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdAlias(pAlias);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    return new WebCallResult {Success = false, ErrorText = "No template found for alias=" + pAlias};
                }
            }

            strUrl = string.Format("{0}usertemplates/{1}", HomeServer.BaseUrl, strObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(HTTPFunctions.StripJsonOfObjectWrapper(res.ResponseText, "UserTemplate"), this);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }

            //the above fetch will set the proeprties as "changed", need to clear them out here
            _changedPropList.Clear();

            return res;
        }

        /// <summary>
        /// Pass in the alias of a distribution list and this routine will return it's ObjectId
        /// </summary>
        /// <param name="pAlias">
        /// Alias uniquely identifying a distribution list.
        /// </param>
        /// <returns></returns>
        private string GetObjectIdAlias(string pAlias)
        {
            string strUrl = string.Format("{0}usertemplates?query=(Alias is {1})", HomeServer.BaseUrl, pAlias);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return "";
            }

            List<UserTemplate> oTemplates = HTTPFunctions.GetObjectsFromJson<UserTemplate>(res.ResponseText);

            if (oTemplates.Count != 1)
            {
                return "";
            }

            return oTemplates.First().ObjectId;
        }

        /// <summary>
        /// Allows one or more properties on a userTemplate to be udpated (for instance DisplayName etc...). At least one property pair needs to 
        /// be edited in the "dirty" queue of changed properties, ut as many as are desired can be included in a single call.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Update()
        {
            WebCallResult res;

            //check if the intance has any pending changes, if not return false with an appropriate error message
            if (!_changedPropList.Any())
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = string.Format("Update called but there are no pending changes for user template {0} [{1}]", Alias, ObjectId);
                return res;
            }
            //just call the static method with the info from the instance 
            res = UpdateUserTemplate(this.HomeServer, this.ObjectId, _changedPropList);

            //if the update goes through clear the queue of changed items
            if (res.Success)
            {
                this.ClearPendingChanges();
            }

            return res;

        }


        /// <summary>
        /// DELETE a usertemplate from the Connection directory.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult Delete()
        {
            //just call the static method with the info on the instance
            return DeleteUserTemplate(this.HomeServer, this.ObjectId);
        }

        /// <summary>
        /// Resets the PIN for a userTemplate - be aware that this routine does not do any validation of the PIN format or complixty/length.  If the PIN
        /// is not valid the error will be returned via the WebCallResult class that will contain information and a failure code from the server.
        /// </summary>
        /// <param name="pNewPin">
        /// New PIN (phone password) to apply to the template. If passed as blank this value is skipped.  You can, for instance, pass a blank
        /// password if you wish to change the "mustchange" flag on a credential but not reset the password itself.  If you pass blank here and you 
        /// pass no other values then CUPI will return an error.
        /// </param>
        /// <param name="pLocked">
        /// Nullable value for locking/unlocking the PIN.  by default this value passes NULL and no change is made to the property.  Passing True
        /// or False will lock and unlock the value respectively.
        /// </param>
        /// <param name="pMustChange">
        /// Nullable value for setting the "must change" value for the credential so the user will have to change it the next time they log in.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pCantChange">
        /// Nullable value for setting the "Cant Change" value for the credential so the user will not be able to change their credential.  
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pDoesntExpire">
        /// Nullable value for setting the "Doesnt expire" value for the credential so the user will never be asked to change the credentail.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        ///<param name="pClearHackedCount">
        /// Nullable value for clearing the hacked count for the user's PIN.
        ///  </param>
        ///<returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult ResetPin(string pNewPin,
                                      bool? pLocked = null,
                                      bool? pMustChange = null,
                                      bool? pCantChange = null,
                                      bool? pDoesntExpire = null,
                                      bool? pClearHackedCount = null
            )
        {
            return ResetUserTemplatePin(HomeServer, ObjectId, pNewPin, pLocked, pMustChange, pCantChange, pDoesntExpire, pClearHackedCount);
        }


        /// <summary>
        /// Resets the password for a template - be aware that this routine does not do any validation of the password format or complixty/length.  
        /// If the password is not valid the error will be returned via the WebCallResult class that will contain information and a failure 
        /// code from the server.
        /// </summary>
        /// <param name="pNewPassword">
        /// New password (GUI password) to apply to the template. If passed as blank this value is skipped.  You can, for instance, pass a blank
        /// password if you wish to change the "mustchange" flag on a credential but not reset the password itself.  If you pass blank here and you 
        /// pass no other values then CUPI will return an error.
        /// </param>
        /// <param name="pLocked">
        /// Nullable value for locking/unlocking the PIN.  by default this value passes NULL and no change is made to the property.  Passing True
        /// or False will lock and unlock the value respectively.
        /// </param>
        /// <param name="pMustChange">
        /// Nullable value for setting the "must change" value for the credential so the user will have to change it the next time they log in.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pCantChange">
        /// Nullable value for setting the "Cant Change" value for the credential so the user will not be able to change their credential.  
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pDoesntExpire">
        /// Nullable value for setting the "Doesnt expire" value for the credential so the user will never be asked to change the credentail.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult ResetPassword(string pNewPassword,
                                        bool? pLocked = null,
                                        bool? pMustChange = null,
                                        bool? pCantChange = null,
                                        bool? pDoesntExpire = null)
        {
            return ResetUserTemplatePassword(HomeServer, ObjectId, pNewPassword, pLocked, pMustChange, pCantChange, pDoesntExpire);
        }

        
        //helper function used when a call is made to get the primary call handler for a usertemplate.
        private WebCallResult GetPrimaryCallHandler(out CallHandler pPrimaryCallHandler)
        {
            WebCallResult res = new WebCallResult();

            try
            {
                pPrimaryCallHandler = new CallHandler(HomeServer, this.CallHandlerObjectId,"",true);
                res.Success = true;
            }
            catch(Exception ex)
            {
                pPrimaryCallHandler = null;
                res.Success = false;
                res.ErrorText =string.Format("Error fetching primary call handler for a user template using ObjectId={0}, error={1}, request details={2}",
                                  this.CallHandlerObjectId , ex, res);
            }

            return res;
        }


        //helper function used when a call is made to get a list of notification devices from the property list.
        private WebCallResult GetNotificationDevices(out List<NotificationDevice> pNotificationDevices)
        {
            return NotificationDevice.GetNotificationDevices(HomeServer, ObjectId, out pNotificationDevices);
        }
   
        /// <summary>
        /// Pass in the notificaiton device display name and this will return an instance of the NotificationDevice class for it (if found).
        /// </summary>
        /// <remarks>
        /// This routine will fetch the full list of notification devices if they have not yet been fetched for this user and return the 
        /// one of interest. If the devices have already been fetched it simply returns the appropriate instance.
        /// </remarks>
        /// <param name="pDeviceName">
        /// The display name of the notification device to fetch.  For the system defined devices the names are:
        /// Work Phone, Home Phone, Mobil Phone, Pager, SMTP.
        /// For devices you've added it's the name you give them.  The search is not case sensitive.
        /// </param>
        /// <param name="pNotificationDevice">
        /// Out param on which the notificaiton device is passed back.  If there is an error finding the device then null is returned.
        /// </param>
        /// <param name="pForceRefetchOfData">
        /// If passed as true this forces a reload of notification devices - this should be passed if devices are being added/removed after 
        /// the user has been fetched.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetNotificationDevice(string pDeviceName, out NotificationDevice pNotificationDevice, bool pForceRefetchOfData = false)
        {
            WebCallResult res;

            pNotificationDevice = null;

            //if a refetch is asked for then null out the private list that may be holding a cached set of devices.
            if (pForceRefetchOfData)
            {
                _notificationDevices = null;
            }
            //fetch the full notification device list if it hasn't been fetched yet.
            if (_notificationDevices == null)
            {
                res = GetNotificationDevices(out _notificationDevices);

                //if there's some sort of error getting the list, pass it back and bail.
                if (res.Success == false)
                {
                    return res;
                }
            }

            //get the correct device off the list
            res = new WebCallResult();

            foreach (NotificationDevice oDevice in _notificationDevices)
            {
                //case insenstive search on display name
                if (oDevice.DisplayName.Equals(pDeviceName, StringComparison.InvariantCultureIgnoreCase))
                {
                    pNotificationDevice = oDevice;
                    res.Success = true;
                    return res;
                }
            }

            //if we're here then there was a probllem
            res.Success = false;
            res.ErrorText = "Could not find notification device by name=" + pDeviceName;
            return res;
        }

        /// <summary>
        /// Clear the queue of changed properties for this user instance.
        /// </summary>
        public void ClearPendingChanges()
        {
            _changedPropList.Clear();
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// returns a single UserTemplate object from an ObjectId string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the user template is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// The ObjectId of the user template to load - can be passed as blank if the alias is being used instead.
        /// </param>
        /// <param name="pAlias">
        /// Optional alias to search for a user by - pass as blank string if the objectId is being provided.
        /// </param>
        /// <param name="pUserTemplate">
        /// If a match for the ObjectId is found an instance of the UserTemplate class will be returned in this out param.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetUserTemplate(ConnectionServer pConnectionServer, string pObjectId, string pAlias, out UserTemplate pUserTemplate)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pUserTemplate = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetUserTemplate";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pAlias))
            {
                res.ErrorText = "Empty ObjectId and Alias passed to GetUserTemplate";
                return res;
            }

            //just leverage the static user template fetch here.
            try
            {
                pUserTemplate = new UserTemplate(pConnectionServer, pObjectId, pAlias);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = string.Format("Failed to fetch user template in GetUserTemplate using objectid={0}, alias={1}, error={2}",
                    pObjectId,pAlias, ex.Message);
                res.Success = false;
                return res;
            }

            //filling the user will put a bunch of entries into the change list - clear them out here
            pUserTemplate.ClearPendingChanges();

            return res;
        }

        /// <summary>
        /// Gets the list of all user templates (For users with voice mail, not admins) and resturns them as a generic list of User Template objects.  This
        /// list can be used for providing drop down list selection for user creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the templates should be pulled from
        /// </param>
        /// <param name="pUserTemplates">
        /// Out parameter that is used to return the list of UserTemplate objects defined on Connection - there must be at least one.
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
        public static WebCallResult GetUserTemplates(ConnectionServer pConnectionServer, out List<UserTemplate> pUserTemplates, int pPageNumber = 1, 
            int pRowsPerPage = 20)
        {
            WebCallResult res;
            pUserTemplates = null;

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetUserTemplates";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "usertemplates", "pageNumber=" + pPageNumber, 
                "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }


            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - return true here along with an empty list.
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                pUserTemplates = new List<UserTemplate>();
                return res;
            }

            pUserTemplates = HTTPFunctions.GetObjectsFromJson<UserTemplate>(res.ResponseText);

            if (pUserTemplates == null)
            {
                pUserTemplates = new List<UserTemplate>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pUserTemplates)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.ClearPendingChanges();
            }

            return res;
        }


        /// <summary>
        /// Allows for the creation of a new user with a mailbox on the Connection server directory.  Both the alias and extension number must be 
        /// provided along with a template alias to use when creating the new user, however other user properties and their values may be passed 
        /// in via the ConnectonPropertyList structure.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users is being added.
        /// </param>
        /// <param name="pTemplateAlias">
        /// The alias of a user template on Connection - this provides importat details such as the Class of Service and dial partition assignment.  It's
        /// required and must exist on the server or the user creation will fail.
        /// </param>
        /// <param name="pAlias">
        /// Alias to be used for the new user with a mailbox.  This must be unique against all users in the directory or the add will fail.
        /// </param>
        /// <param name="pDisplayName">
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a user property name and a new value for that property to apply to the user being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddUserTemplate(ConnectionServer pConnectionServer,
                                            string pTemplateAlias,
                                            string pAlias,
                                            string pDisplayName,
                                            ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddUserTemplate";
                return res;
            }

            //make sure that something is passed in for the 3 required params 
            if (String.IsNullOrEmpty(pTemplateAlias) || string.IsNullOrEmpty(pAlias) || string.IsNullOrEmpty(pDisplayName))
            {
                res.ErrorText = "Empty value passed for one or more required parameters in AddUserTemplate on ConnectionServer.cs";
                return res;
            }

            //create an empty property list if it's passed as null since we use it below
            if (pPropList == null)
            {
                pPropList = new ConnectionPropertyList();
            }

            //cheat here a bit and simply add the alias and extension values to the proplist where it can be tossed into the body later.
            pPropList.Add("Alias", pAlias);
            pPropList.Add("DisplayName", pDisplayName);
            string strBody = "<UserTemplate>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</UserTemplate>";

            res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "usertemplates?templateAlias=" + pTemplateAlias, MethodType.POST, pConnectionServer, 
                strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/usertemplates/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/usertemplates/", "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Allows for the creation of a new user with a mailbox on the Connection server directory.  Both the alias and extension number must be 
        /// provided along with a template alias to use when creating the new user, however other user properties and their values may be passed 
        /// in via the ConnectonPropertyList structure.
        /// </summary>
        /// <remarks>
        /// This is an alternateive AddUser that passes back a UserFull object with the newly created user filled out in it if the add goes through.
        /// </remarks>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the users is being added.
        /// </param>
        /// <param name="pTemplateAlias">
        /// The alias of a user template on Connection - this provides importat details such as the Class of Service and dial partition assignment.  It's
        /// required and must exist on the server or the user creation will fail.
        /// </param>
        /// <param name="pAlias">
        /// Alias to be used for the new user with a mailbox.  This must be unique against all users in the directory or the add will fail.
        /// </param>
        /// <param name="pExtension">
        /// The primary extension number to be assigned to the new user.  This must be unqiue in the partition the user is created in or the 
        /// new user creation will fail.  The partition is determined by the user template used to created new users.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a user property name and a new value for that property to apply to the user being created.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  Can be passed as null.
        /// </param>
        /// <param name="pUserTemplate">
        /// Out paramter that passes back a UserFull object with the details of the newly added user.  If the new user add fails, NULL is returned 
        /// for this value.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult AddUserTemplate(ConnectionServer pConnectionServer,
                                            string pTemplateAlias,
                                            string pAlias,
                                            string pExtension,
                                            ConnectionPropertyList pPropList,
                                            out UserTemplate pUserTemplate)
        {
            pUserTemplate = null;

            WebCallResult res = AddUserTemplate(pConnectionServer, pTemplateAlias, pAlias, pExtension, pPropList);

            //if the add goes through, fill a UserFull object out and pass it back.
            if (res.Success)
            {
                res = UserTemplate.GetUserTemplate(pConnectionServer, res.ReturnedObjectId, "", out pUserTemplate);

                //stuff the objectID back in the res structure since we took overwrote it with the GetUser fetch.
                res.ReturnedObjectId = pUserTemplate.ObjectId;
            }

            return res;
        }

        /// <summary>
        /// DELETE a user from the Connection directory.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pObjectId">
        /// GUID to uniquely identify the user in the directory.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult DeleteUserTemplate(ConnectionServer pConnectionServer, string pObjectId)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteUserTemplate";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "usertemplates/" + pObjectId, MethodType.DELETE, pConnectionServer, "");
        }

        /// <summary>
        /// Allows one or more properties on a template to be udpated (for instance FirstName, LastName etc...).  The caller needs to construct a list
        /// of property names and new values using the ConnectionPropertyList class's "Add" method.  At least one property pair needs to be passed in 
        /// but as many as are desired can be included in a single call.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pObjectId">
        /// The unqiue GUID identifying the user to be updated.
        /// </param>
        /// <param name="pPropList">
        /// List ConnectionProperty pairs that identify a user property name and a new value for that property to apply to the user being updated.
        /// This is passed in as a ConnectionPropertyList instance which contains 1 or more ConnectionProperty instances.  At least one property
        /// pair needs to be included for the funtion to execute.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult UpdateUserTemplate(ConnectionServer pConnectionServer, string pObjectId, ConnectionPropertyList pPropList)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateUserTemplate";
                return res;
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            if (pPropList == null || pPropList.Count < 1)
            {
                res.ErrorText = "empty property list passed to UpdateUserTemplate";
                return res;
            }

            string strBody = "<UserTemplate>";

            foreach (var oPair in pPropList)
            {
                //tack on the property value pair with appropriate tags
                strBody += string.Format("<{0}>{1}</{0}>", oPair.PropertyName, oPair.PropertyValue);
            }

            strBody += "</UserTemplate>";

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "usertemplates/" + pObjectId, MethodType.PUT, pConnectionServer, 
                strBody,false);
        }

        /// <summary>
        /// Resets the PIN for a user - be aware that this routine does not do any validation of the PIN format or complixty/length.  If the PIN
        /// is not valid the error will be returned via the WebCallResult class that will contain information and a failure code from the server.
        /// You can set/clear the locked, must-change, cant-change and doesnt-expire options for the credential as well - by default all values are 
        /// left alone.  There is no checking of these values for consistency (i.e. if you pass cant change and must change).  Handling that is 
        /// left to the server.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pObjectId">
        /// Unique GUID of the user to reset the PIN for.
        /// </param>
        /// <param name="pNewPin">
        /// New PIN (phone password) to apply to the user's account.  If passed as blank this value is skipped.  You can, for instance, pass a blank
        /// password if you wish to change the "mustchange" flag on a credential but not reset the password itself.  If you pass blank here and you 
        /// pass no other values then CUPI will return an error.
        /// </param>
        /// <param name="pLocked">
        /// Nullable value for locking/unlocking the PIN.  by default this value passes NULL and no change is made to the property.  Passing True
        /// or False will lock and unlock the value respectively.
        /// </param>
        /// <param name="pMustChange">
        /// Nullable value for setting the "must change" value for the credential so the user will have to change it the next time they log in.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pCantChange">
        /// Nullable value for setting the "Cant Change" value for the credential so the user will not be able to change their credential.  
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pDoesntExpire">
        /// Nullable value for setting the "Doesnt expire" value for the credential so the user will never be asked to change the credentail.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pClearHackedLockout">
        /// Nullable value for clearing a hacked lockout condition (i.e. user provided wrong password too many times in a row and is now locked out for 
        /// a period of time).  This is seperate from an admin lockout condition which is cleared/set using the pLocked parameter.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult ResetUserTemplatePin(ConnectionServer pConnectionServer,
                                                string pObjectId,
                                                string pNewPin,
                                                bool? pLocked = null,
                                                bool? pMustChange = null,
                                                bool? pCantChange = null,
                                                bool? pDoesntExpire = null,
                                                bool? pClearHackedLockout = null)
        {
            return ResetUserTemplateCredential(pConnectionServer,
                                        pObjectId,
                                        pNewPin,
                                        CredentialType.Pin,
                                        pLocked,
                                        pMustChange,
                                        pCantChange,
                                        pDoesntExpire,
                                        pClearHackedLockout);
        }

        /// <summary>
        /// Resets the password for a user (GUI) - be aware that this routine does not do any validation of the password format or complixty/length.  
        /// If the password is not valid the error will be returned via the WebCallResult class that will contain information and a failure code 
        /// from the server.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the user is homed.
        /// </param>
        /// <param name="pObjectId">
        /// Unique GUID of the user to reset the password for.
        /// </param>
        /// <param name="pNewPassword">
        /// New password (GUI password) to apply to the user's account.  If passed as blank this value is skipped.  You can, for instance, pass a blank
        /// password if you wish to change the "mustchange" flag on a credential but not reset the password itself.  If you pass blank here and you 
        /// pass no other values then CUPI will return an error.
        /// </param>
        /// <param name="pLocked">
        /// Nullable value for locking/unlocking the PIN.  by default this value passes NULL and no change is made to the property.  Passing True
        /// or False will lock and unlock the value respectively.
        /// </param>
        /// <param name="pMustChange">
        /// Nullable value for setting the "must change" value for the credential so the user will have to change it the next time they log in.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pCantChange">
        /// Nullable value for setting the "Cant Change" value for the credential so the user will not be able to change their credential.  
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <param name="pDoesntExpire">
        /// Nullable value for setting the "Doesnt expire" value for the credential so the user will never be asked to change the credentail.
        /// By default this value passes NULL and no change is made to it.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult ResetUserTemplatePassword(ConnectionServer pConnectionServer,
                                                    string pObjectId,
                                                    string pNewPassword,
                                                    bool? pLocked = null,
                                                    bool? pMustChange = null,
                                                    bool? pCantChange = null,
                                                    bool? pDoesntExpire = null)
        {
            return ResetUserTemplateCredential(pConnectionServer,
                                           pObjectId,
                                           pNewPassword,
                                           CredentialType.Password,
                                           pLocked,
                                           pMustChange,
                                           pCantChange,
                                           pDoesntExpire);
        }


        //back end helper function to reset PIN or Password
        private static WebCallResult ResetUserTemplateCredential(ConnectionServer pConnectionServer,
                                        string pObjectId,
                                        string pNewPCredential,
                                        CredentialType pCredentialType,
                                        bool? pLocked = null,
                                        bool? pMustChange = null,
                                        bool? pCantChange = null,
                                        bool? pDoesntExpire = null,
                                        bool? pClearHackedLockout = null)
        {
            if (pConnectionServer == null)
            {
                WebCallResult res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to ResetUserTemplateCredential";
                return res;
            }

            ConnectionPropertyList oProps = new ConnectionPropertyList();

            //only add the credential if it's not empty - This interface does NOT allow for blank credentials which are a 
            //horrible idea.
            if (!String.IsNullOrEmpty(pNewPCredential)) oProps.Add("Credentials", pNewPCredential);

            //add in nullable optional flags that can be psssed in as needed.
            if (pLocked != null) oProps.Add("Locked", pLocked.Value);
            if (pMustChange != null) oProps.Add("CredMustChange", pMustChange.Value);
            if (pCantChange != null) oProps.Add("CantChange", pCantChange.Value);
            if (pDoesntExpire != null) oProps.Add("DoesntExpire", pDoesntExpire.Value);
            if (pClearHackedLockout != null && pClearHackedLockout == true)
            {
                //when clearing the hacked lockout you need to clear both the hacked count and the boolean Hacked flag to unlock
                //the account - this method is only for clearing the hack lockout, not setting it.
                oProps.Add("Hacked", false);
                oProps.Add("HackCount", 0);
            }

            //the update command takes a body in the request, construct it based on the name/value pair of properties passed in.  
            //at lest one such pair needs to be present
            string strBody = "<Credential>";

            foreach (ConnectionObjectPropertyPair oPair in oProps)
            {
                strBody += string.Format("<{0}>{1}</{2}>", oPair.PropertyName, oPair.PropertyValue, oPair.PropertyName);
            }


            strBody += "</Credential>";

            //the only difference between setting a PIN vs. Password is the URL path used.  The body/property names are identical
            //otherwise.
            string strUrl;
            if (pCredentialType == CredentialType.Pin)
            {
                strUrl = pConnectionServer.BaseUrl + "usertemplates/" + pObjectId + "/credential/pin";
            }
            else
            {
                strUrl = pConnectionServer.BaseUrl + "usertemplates/" + pObjectId + "/credential/password";
            }

            return HTTPFunctions.GetCupiResponse(strUrl, MethodType.PUT, pConnectionServer, strBody,false);
        }


        #endregion

    }
}
