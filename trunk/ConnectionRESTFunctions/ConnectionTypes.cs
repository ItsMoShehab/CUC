﻿#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cisco.UnityConnection.RestFunctions
{
    // Collection of enums, structs and classes to help make the code more readable and avoid mistakes when setting values for various 
    // Connection objects such as the addressing mode for a subscriber or the target conversation name for a menu entry key or the like.

    /// <summary>
    /// Action types (0 to 8) for menu entry keys, after greeting actions, exit actions etc... 
    /// </summary>
    public enum ActionTypes
    {
        Ignore,
        Hangup,
        GoTo,
        Error,
        TakeMessage,
        SkipGreeting,
        RestartGreeting,
        TransferToAlternateContactNumber,
        RouteFromNextCallRoutingRule,
        Invalid=99
    }

    /// <summary>
    /// When addressing messages using touch tones in the subscriber conversation Connection can default to starting with the last name, 
    /// starting with the first name or using the target's extension number.
    /// </summary>
    public enum AddressingMode
    {
        LastNameFirst,
        Extension,
        FirstNameFirst
    }

    /// <summary>
    /// List of the ID mappings for alternate extensions.  0 is the pimary extension, 1 through 10 is admin added alternate extensions and 11 through 20 are 
    /// user added alternate extensions
    /// </summary>
    public enum AlternateExtensionId
    {
        Primary,
        AdminAdded1,
        AdminAdded2,
        AdminAdded3,
        AdminAdded4,
        AdminAdded5,
        AdminAdded6,
        AdminAdded7,
        AdminAdded8,
        AdminAdded9,
        AdminAdded10,
        UserAdded1,
        UserAdded2,
        UserAdded3,
        UserAdded4,
        UserAdded5,
        UserAdded6,
        UserAdded7,
        UserAdded8,
        UserAdded9,
        UserAdded10
    }

    public enum CcmIdType{EndUser, ApplicationUser, DirectoryNumber, LdapUser, InactiveLdapUser}

    /// <summary>
    /// The clock mode for users can be whatever the system default setting is, 12 hour (AM/PM) or 24 hour mode.
    /// </summary>
    public enum ClockMode
    {
        SystemDefaultClock,
        HourClock12,
        HourClock24
    }

    public enum ClusterMemberId {Primary, Secondary}

    /// <summary>
    /// The ConnectionObjectPropertyPair is a very simple name/value pair construct that gets used for passing lists of property values 
    /// to functions for updating multiple items on various Connection objects.  For instance passing lists of user properties to a user 
    /// update function you create a list of these property pairs using the ConnectionPropertyList class below and pass that as a parameter
    /// to the function.
    /// We could use a prebuilt class such as a dictionary or hash table or the like but I prefer to be explicit with these classes thougout.
    /// </summary>
    public class ConnectionObjectPropertyPair
    {
        public string PropertyName;
        public string PropertyValue;

        /// <summary>
        /// Property pairing for Connection property lists - these are used to construct lists of property/name values that are passed 
        /// into methods for updating users, call handlers, menu entries etc... this allows for dyanamic property lists and easy construction
        /// and debugging routines within the class library.
        /// </summary>
        /// <param name="pPropertyName">
        /// The name of the property (case sensitive) on the Connection object you are wanting to update.
        /// </param>
        /// <param name="pPropertyValue">
        /// The value to assign to the property.
        /// </param>
        public ConnectionObjectPropertyPair(string pPropertyName, string pPropertyValue)
        {
            PropertyName = pPropertyName;
            PropertyValue = pPropertyValue;
        }

        /// <summary>
        /// Displays the property name and value for a ConnectionObjectPropertyPair object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", PropertyName, PropertyValue);
        }
    }

    /// <summary>
    /// List of property pairs used for passing multiple property values to object update/create functions.  These property lists are generally 
    /// constructed into HTML body strings in the form of "<PropertyName> PropertyValue </PropertyName>" as a list in the body when, say, updating
    /// values on a user object.
    /// </summary>
    public class ConnectionPropertyList : List<ConnectionObjectPropertyPair>
    {
        //base constructor
        public ConnectionPropertyList()
        {
        }

        //constructor that takes a property name/value pair
        public ConnectionPropertyList(string pPropertyName, string pPropertyValue)
        {
            Add(pPropertyName, pPropertyValue);
        }

        //Add function allows a new name value pair to be added.
        public void Add(string pPropertyName, string pPropertyValue)
        {
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName, pPropertyValue);
            Add(oPair);
        }

        //for adding an integer value
        public void Add(string pPropertyName, int pPropertyValue)
        {
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,
                                                                                  pPropertyValue.ToString());
            Add(oPair);
        }

        //for adding a boolean value - CUPI needs 0/1 passed instead of "true" or "false" here
        public void Add(string pPropertyName, bool pPropertyValue)
        {
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,
                                                                                  HTTPFunctions.BoolToString(
                                                                                      pPropertyValue));
            Add(oPair);
        }

        //for adding a date - Informix needs special formatting
        public void Add(string pPropertyName, DateTime pPropertyValue)
        {
            //The Informix time/date format is a little fussy...
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,
                                                                                  String.Format(
                                                                                      "{0:yyyy-MM-dd hh:mm:ss}",
                                                                                      pPropertyValue));
            Add(oPair);
        }

        //adding a nullable date - don't add if it's null
        public void Add(string pPropertyName, DateTime? pPropertyValue)
        {
            if (pPropertyValue == null)
            {
                return;
            }
            //The Informix time/date format is a little fussy...
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,
                                                                                  String.Format(
                                                                                      "{0:yyyy-MM-dd hh:mm:ss}",
                                                                                      pPropertyValue));
            Add(oPair);
        }

        /// <summary>
        /// return a simple list of all the name/value pairs in the list
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strRet = "";

            if (this.Count == 0) return "{empty}";

            foreach (ConnectionObjectPropertyPair oPair in this)
            {
                strRet += oPair + Environment.NewLine;
            }
            return strRet;
        }

        /// <summary>
        /// Simple helper function to convert the list of name/value pairs into strings in the form "name=value" as an 
        /// array for easy inclusion into URI segements and such.
        /// </summary>
        /// <returns></returns>
        public string[] ToArrayOfStrings()
        {
            List<string> oList = new List<string>();
            foreach (var oPair in this)
            {
                oList.Add(string.Format("{0}={1}", oPair.PropertyName, oPair.PropertyValue));
            }

            return oList.ToArray();
        }
    }


    /// <summary>
    /// Connection object types for constructing URIs into SA elements
    /// </summary>
    public enum ConnectionObjectType
    {
        NoObjectType = 0,
        SystemCallHandler = 3,
        SystemContact = 33,
        Cos = 8,
        CredentialPolicy=11,
        NameLookupHandler = 6,
        DistributionList = 2,
        InterviewHandler = 5,
        Location = 9,
        PersonalContact = 102,
        PersonalDistributionList = 64,
        Policy = 15,
        RestrictionTable = 12,
        Role = 13,
        Schedule = 7,
        Subscriber = 21,
        User = 1,
        SubscriberTemplate = 10,
        RoutingRule = 103,
        GlobalUser = 104,
        Partition = 105,
        SearchSpace = 106,
        ScheduleSet = 49,
        Handler = 134,
        CallHandlerTemplate = 135,
        PersonalCallTransferRule = 43,
        RoutingRuleDirect = 800,
        RoutingRuleForwarded = 801,
        SmppProvider = 802,
        UserWithoutMailbox = 803,
        Switch = 804,
        VpimContact = 805,
        BridgeContact = 806,
        UnityContact = 807,
    }

    /// <summary>
    /// Definitions of the langauge type enum value from the data dictionary
    /// </summary>
    public enum ConnectionLanguageTypes
    {
        TUI,
        GUI,
        VUI,
        TTS
    }

    /// <summary>
    /// Covnersation names that can be used as part of "action" values if the action is "goto"
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationNames
    {
        Ad,
        PHTransfer,
        PHGreeting,
        PHInterview,
        BroadcastMessageAdministrator,
        GreetingsAdministrator,
        SubSignIn,
        SubSysTransfer,
        SystemTransfer,
        ConvHotelCheckedOut,
        ConvCvmMboxReset,
        EasySignIn,
        TransferAltContactNumber,
        Invalid
    }

    /// <summary>
    /// Credential types used by CUPI (3=GUI Password, 4 = Phone PIN.  Other types (Domino, Windows...) do not get used in CUPI.
    /// </summary>
    public enum CredentialType
    {
        Password = 3,
        Pin = 4
    }

    public enum CxnSortOrder { Lifo = 1, Fifo = 2 }

    /// <summary>
    /// Destination types for Unity Connection locations
    /// </summary>
    public enum DestinationType {Unknown =0, Connection=1, Unity =2, Bridge = 7, Vpim=8, Branch=9}

    public enum DirectoryHandlerSearchScope{Vms, DialingDomain, Global, Location, DistributionList, Cos, SearchSpace, Inherit, Invalid}

    public enum DisplayNameRule {FirstLast=1, LastFirst=2}

    /// <summary>
    /// For human readable output of distribution list member types
    /// </summary>
    public enum DistributionListMemberType
    {
        LocalUser,
        GlobalUser,
        Contact,
        DistributionList
    }

    public enum EncryptionType{Unknown, HashMd5, HashSha1,HashIms, Reversible, None}

    /// <summary>
    /// The 7 types of greetings allowed in Connection
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GreetingTypes
    {
        Standard,
        [EnumMember(Value = "Off Hours")]
        [Description("Off Hours")]
        OffHours,
        Alternate,
        Busy,
        Holiday,
        Internal,
        Error,
        Invalid
    }

    /// <summary>
    /// Couple helper functions for displaying and converting language codes from the big enum of them all.
    /// </summary>
    public static class LanguageHelper
    {
        public static string GetLanguageNameFromLanguageId(int pLanguageId)
        {
            if (Enum.IsDefined(typeof (LanguageCodes), pLanguageId) == false)
            {
                return "Undefined";
            }

            return ((LanguageCodes) pLanguageId).ToString();
        }

        public static int GetLanguageIdFromLanguageEnum(LanguageCodes pLanguageCode)
        {

            if (Enum.IsDefined(typeof (LanguageCodes), pLanguageCode) == false)
            {
                return -1;
            }

            return (int) pLanguageCode;

        }

    }

    /// <summary>
    /// Language codes currently supported in Connection (as of 8.5)
    /// </summary>
    public enum LanguageCodes
    {
        Undefined = -1,
        SystemDefault = 0,
        Afrikaans = 1078,
        Albanian = 1052,
        ArabicAlgeria = 5121,
        ArabicBahrain = 15361,
        ArabicEgypt = 3073,
        ArabicIraq = 2049,
        ArabicJordan = 11265,
        ArabicKuwait = 13313,
        ArabicLebanon = 12289,
        ArabicLibya = 4097,
        ArabicMorocco = 6145,
        ArabicOman = 8193,
        ArabicQatar = 16385,
        ArabicSaudiArabia = 1025,
        ArabicSyria = 10241,
        ArabicTunisia = 7169,
        ArabicUae = 14337,
        ArabicYemen = 9217,
        Armenian = 1067,
        AzeriCyrillic = 2092,
        AzeriLtin = 1068,
        Basque = 1069,
        Belarusian = 1059,
        Bulgarian = 1026,
        Catalan = 1027,
        ChineseHongKong = 3076,
        ChineseMacau = 5124,
        ChinesePrc = 2052,
        ChineseSingapore = 4100,
        ChineseTaiwan = 1028,
        Croatian = 1050,
        Czech = 1029,
        Danish = 1030,
        Divehi = 1125,
        DutchBelgian = 2067,
        DutchStandard = 1043,
        EnglishAustralian = 3081,
        EnglishBelize = 10249,
        EnglishCandian = 4105,
        EnglishCaribean = 9225,
        EnglishIreland = 6153,
        EnglishJamaica = 8201,
        EnglishNewZealand = 5129,
        EnglishPhilippines = 13321,
        EnglishSouthAfrica = 7177,
        EnglishTrinidad = 11273,
        EnglishUnitedKingdom = 2057,
        EnglishUnitedStates = 1033,
        EnglishZimbabwe = 12297,
        Estonian = 1061,
        Faeroese = 1080,
        Farsi = 1065,
        Finnish = 1035,
        FrenchBelgian = 2060,
        FrenchCandian = 3084,
        FrenchLuxembourg = 5132,
        FrenchMonaco = 6156,
        FrenchStandard = 1036,
        FrenchSwiss = 4108,
        GaelicScots = 1084,
        Galician = 1110,
        Georgian = 1079,
        GermanAustrian = 3079,
        GermanLiechtenstein = 5127,
        GermanLuxembourg = 4103,
        GermanStandard = 1031,
        GermanSwiss = 2055,
        Greek = 1032,
        Gujarati = 1095,
        Hebrew = 1037,
        Hindi = 1081,
        Hungarian = 1038,
        Icelandic = 1039,
        Indonesian = 1057,
        ItalianStandard = 1040,
        ItalianSwiss = 2064,
        Japanese = 1041,
        Kannada = 1099,
        Kazakh = 1087,
        Konkani = 1111,
        Korean = 1042,
        Kyrgyz = 1088,
        Latvian = 1062,
        Lithuanian = 1063,
        Macedonian = 1071,
        MalayBruneiDarussalam = 2110,
        MalayMalaysia = 1086,
        Maltese = 1082,
        Marathi = 1102,
        Mongolian = 1104,
        NoewegianNynorsk = 2068,
        NorwegianBokmal = 1044,
        Polish = 1045,
        PortugeseStandard = 2070,
        PortugueseBrazilian = 1046,
        Punjabi = 1094,
        RhaetoRomanic = 1047,
        Romanian = 1048,
        RomanianMoldavia = 2072,
        Russian = 1049,
        RussianMoldavia = 2073,
        Sanskrit = 1103,
        SerbianCyrillic = 3098,
        SerbianLatin = 2074,
        Slovak = 1051,
        Slovenian = 1060,
        Sorbian = 1070,
        SpanishArgentina = 11274,
        SpanishBolivia = 16394,
        SpanishChile = 13322,
        SpanishColumbia = 9226,
        SpanishCostaRica = 5130,
        SpanishDominicanRepublic = 7178,
        SpanishEcuador = 12298,
        SpanishElSalvador = 17418,
        SpanishGuatemala = 4106,
        SpanishHonduras = 18442,
        SpanishMexican = 2058,
        SpanishModernSort = 3082,
        SpanishNicaragua = 19466,
        SpanishPanama = 6154,
        SpanishParaguay = 15370,
        SpanishPeru = 10250,
        SpanishPuertoRico = 20490,
        SpanishTraditionalSort = 1034,
        SpanishUruguay = 14346,
        SpanishVenezuela = 8202,
        Sutu = 1072,
        Swahili = 1089,
        Swedish = 1053,
        SwedishFinland = 2077,
        Syriac = 1114,
        Tanil = 1097,
        Tatar = 1092,
        Telugu = 1098,
        Thai = 1054,
        Tsonga = 1073,
        Tswana = 1074,
        Turkish = 1055,
        Ukrainian = 1058,
        Urdu = 1056,
        UzbekCyrillic = 2115,
        UzbekLatin = 1091,
        Vietnamese = 1066,
        Xhosa = 1076,
        Yiddish = 1085,
        Zulu = 1077,
        Enx = 33801,
        EnglishIndian = 16393,
    }

    public enum LdapType {None, Sync, Authenticate, Unknown, Inactive}
    
    public enum MediaRemoteServiceEnum{AsrMediaServer=105,CCM=100, CcmAxl=103, CcmTftp=101,Pimg=104,SipProxy=102,TtsMediaServer=106}

    /// <summary>
    /// Used in many places such as holding mode, mark private, mark setcure etc...
    /// </summary>
    public enum ModeYesNoAsk { No, Yes, Ask }

    /// <summary>
    /// List of notification device types.  Be aware that MP3 is a defined type but is not supported or used in Connection.
    /// </summary>
    public enum NotificationDeviceTypes
    {
        Fax = 3,
        Mp3 = 7,
        Pager = 2,
        Phone = 1,
        Sms = 6,
        Smtp = 4
    }

    /// <summary>
    /// All possible event notification types that can be set on a notificaiton device.  Note that you cannot mix "AllMessage" and other types
    /// or "DispatchMessage" and other message types.
    /// </summary>
    public enum NotificationEventTypes
    {
        AllMessage,
        NewFax,
        NewUrgentFax,
        NewVoiceMail,
        NewUrgentVoiceMail,
        DispatchMessage,
        UrgentDispatchMessage,
    }


    /// <summary>
    /// Used for playing the "sent message" prompt referenced by the post greeting recording
    /// </summary>
    public enum PlayAfterMessageTypes{No, Default, Recorded}
    
    /// <summary>
    /// Setting for handler indicating if post greetin
    /// </summary>
    public enum PlayPostGreetingRecordingTypes {Never, Always, ExternalCallersOnly}

    /// <summary>
    /// Greetings setting to determine which greeting plays.
    /// 2=NoGreeting, 1=RecordedGreeting, 0=SystemGreeting
    /// </summary>
    public enum PlayWhatTypes
    {
        SystemGreeting,
        RecordedGreeting,
        NoGreeting
    }

    public enum PreferredTransport { Ipv4, Ipv6, Ipv4V6 }

    /// <summary>
    /// Make the private list member type enum more human readable in output.
    /// </summary>
    public enum PrivateListMemberType
    {
        LocalUser,
        RemoteContact,
        DistributionList,
        PrivateList
    }

    public enum ResetStatusEnum{NotRequired=0, Required=100, InProgress=101}

    public enum RoutingRuleType { Unknown, Direct, Forwarded, System }

    public enum RoutingRuleFlag { Immutable, Deletable, Editable, EditableAndDeletable }

    public enum RoutintRuleActionType { Ignore, Hangup, Goto, Error, TakeMsg, SkipGreeting, RestartGreeting, TransferAtContact, RouteFromNextRule }

    public enum RoutingRuleCallType { Internal, External, Both }

    public enum RoutingRuleState { Active, Inactive, Invalid }

    public enum RoutingRuleConditionOperator
    {
        Invalid, CallingNumber, DialedNumber, ForwardingStation, Origin, PortId, Reason, Schedule, TrunkId, PhoneSystem
    }

    public enum RoutingRuleConditionParameter { Invalid = 0, In = 1, Equals = 2, GreaterThan = 3, LessThan = 4, LessThanOrEqual = 5, GreaterThanOrEqual = 6 }

    /// <summary>
    /// A schedule can be be in one of 3 states - active (on) inactive (off) or active for a holiday.
    /// </summary>
    public enum ScheduleState { INACTIVE, ACTIVE, HOLIDAY }

    public enum SendMessageOnHangup {Discard, Send, Save}
    
    public enum ServerDisplayState {Unknown, Down, Initalizing, Primary, Secondary, Idle, InDbSnc, InSbr}

    public enum ServerState
    {
        Pri_Init,
        Pri_Active,
        Pri_Act_Secondary,
        Pri_Idle,
        Pri_Failover,
        Pri_Takeover,
        Pri_SBR,
        Sec_Init,
        Sec_Active,
        Sec_Act_Primary,
        Sec_Idle,
        Sec_Takeover,
        Sec_Failover,
        Sec_SBR,
        Db_Sync,
        Set_Peer_Idle,
        Undefined,
        Pri_Active_Disconnected,
        Pri_Connecting,
        Pri_Choose_Role,
        Pri_Single_Server,
        Sec_Act_Primary_Disconnected,
        Sec_Connecting,
        Sec_Choose_Role,
        Shutdown
    }

    public enum SipTlsModes {Authenticated=10, Encrypted=11}

    public enum SipTransportEnum {Tcp=11, Udp=10}

    /// <summary>
    /// SCCP ports can be configured for 1 of 3 security modes
    /// </summary>
    public enum SkinnySecurityModes
    {
        Insecure,
        Authenticated,
        Encrypted
    }

    public enum SkinnyStateMachineEnum {Ccm=10, Ccme=20}

    /// <summary>
    /// All the valid conversations a subscriber can be assigned to for their inbox conversation (i.e. what they hear when they call 
    /// in to check messages).  This list has remained static since 7.0(2) thorugh 8.6.
    /// Note, the naming convention on these values must remain as is with underscores or they will not match what comes out of the 
    /// database for string matching.
    /// </summary>
     [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriberConversationTui
    {
        SubMenu,
        SubMenu_Alternate_Custom,
        SubMenu_Alternate_Custom1,
        SubMenu_Alternate_Custom2,
        SubMenu_Alternate_Custom3,
        SubMenu_Alternate_Custom4,
        SubMenu_Alternate_Custom5,
        SubMenu_Alternate_Custom6,
        SubMenu_AlternateI,
        SubMenu_AlternateN,
        SubMenu_AlternateS,
        SubMenu_AlternateX,
        SubMenuOpt1
    }

     public enum TenantAttributeType { Partition, PhoneSystem, Cos, ScheduleSet }

    /// <summary>
    /// The 3 transfer option types in Connection
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransferOptionTypes
    {
        Standard,
        [EnumMember(Value = "Off Hours")]
        [Description("Off Hours")]
        OffHours,
        Alternate,
        Invalid
    }

    
    /// <summary>
    /// Enum defining the 3 possible phone integration methods assigned when creating a port group.
    /// </summary>
    public enum TelephonyIntegrationMethodEnum
    {
        SCCP = 1,
        SIP = 2,
        PIMG = 3
    }

    /// <summary>
    /// A transfer rule can be set to ring the phone first or send the call directly to the active greeting.
    /// 0=Play greeting, 1= transfer 
    /// </summary>
    public enum TransferActionTypes
    {
        Transfer,
        PlayGreeting
    }

    /// <summary>
    /// Transfer type options 0=unsupervised, 1 = supervised.
    /// </summary>
    public enum TransferTypes
    {
        Unsupervised,
        Supervised
    }

}