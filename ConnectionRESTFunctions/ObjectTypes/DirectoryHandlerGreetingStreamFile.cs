#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The DirectoryHandlerGreetingStreamFile class contains all the properties related to individual greeting streams associated with custom
    /// greetings for directory handlers.  Each greeting can have multiple greeting streams in different languages.
    /// This class contains methods to fetch and set the greeting recordings for a specific language for a greeting.  There are no other
    /// properties on a greeting stream that can be edited.
    /// </summary>
    public class DirectoryHandlerGreetingStreamFile
    {

        #region Constructor

        /// <summary>
        /// default constructor used for JSON parsing
        /// </summary>
        public DirectoryHandlerGreetingStreamFile()
        {
            
        }

        /// <summary>
        /// Creates a new instance of the DirectoryHandlerGreetingStreamFile class.  Requires you pass a handle to a ConnectionServer 
        /// object which will be used for fetching and updating data for this object.
        /// If you pass the pLanguageCode parameter the greeting stream is automatically filled with data for that entry from the server.  
        /// If not then an empty instance of the GreetingStreamFile class is returned (so you can fill it out on your own).
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of a ConnectonServer object which points to the home server for the greeting stream is being created.
        /// </param>
        /// <param name="pDirectoryHandlerObjectId">
        /// GUID identifying the directory Handler that owns the greeting that owns this greeting stream file.
        /// </param>
        /// <param name="pLanguageCode">
        /// The language of the greeting stream file to return (i.e. 1033 is US English).
        /// </param>
        public DirectoryHandlerGreetingStreamFile(ConnectionServer pConnectionServer, string pDirectoryHandlerObjectId, int pLanguageCode = -1)
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to GreetingStreamFile constructor.");
            }

            //we must know what directory handler we're associated with.
            if (String.IsNullOrEmpty(pDirectoryHandlerObjectId))
            {
                throw new ArgumentException("Invalid DirectoryHandlerObjectId passed to GreetingStreamFile constructor.");
            }

            HomeServer = pConnectionServer;

            //remember the objectID of the owner 
            DirectoryHandlerObjectId = pDirectoryHandlerObjectId;

            //if the user didn't pass in a specific language code then exit here, otherwise load up the greeting stream file instance with data 
            //from Connection
            if (pLanguageCode == -1) return;

            LanguageCode = pLanguageCode;

            //if the language code is passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetGreetingStreamFile(pDirectoryHandlerObjectId, pLanguageCode);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Greeting stream file not found in DirectoryHandlerGreetingStreamFile constructor " +
                                                  "using DirectoryHandlerObjectID={0} and language{1}\n\rError={2}",
                                                  pDirectoryHandlerObjectId, pLanguageCode, res.ErrorText));
            }
        }

        #endregion


        #region Fields and Properties

        //reference to the ConnectionServer object used to create this Greeting instance.
        public ConnectionServer HomeServer { get; private set; }

        #endregion


        #region DirectoryHandlerGreetingStreamFile Properties

        /// <summary>
        /// Reference to the directory handler that owns this greeting.
        /// You cannot set or change this value after creation.
        /// </summary>
        [JsonProperty]
        public string DirectoryHandlerObjectId { get; private set; }

        /// <summary>
        /// The Windows Locale ID (LCID) identifying the language in which this stream was recorded:
        ///1078=Afrikaans
        ///1052=Albanian
        ///5121=ArabicAlgeria
        ///15361=ArabicBahrain
        ///3073=ArabicEgypt
        ///2049=ArabicIraq
        ///11265=ArabicJordan
        ///13313=ArabicKuwait
        ///12289=ArabicLebanon
        ///4097=ArabicLibya
        ///6145=ArabicMorocco
        ///8193=ArabicOman
        ///16385=ArabicQatar
        ///1025=ArabicSaudiArabia
        ///10241=ArabicSyria
        ///7169=ArabicTunisia
        ///14337=ArabicUAE
        ///9217=ArabicYemen
        ///1067=Armenian
        ///2092=AzeriCyrillic
        ///1068=AzeriLtin
        ///1069=Basque
        ///1059=Belarusian
        ///1026=Bulgarian
        ///1027=Catalan
        ///3076=ChineseHongKong
        ///5124=ChineseMacau
        ///2052=ChinesePRC
        ///4100=ChineseSingapore
        ///1028=ChineseTaiwan
        ///1050=Croatian
        ///1029=Czech
        ///1030=Danish
        ///1125=Divehi
        ///2067=DutchBelgian
        ///1043=DutchStandard
        ///3081=EnglishAustralian
        ///10249=EnglishBelize
        ///4105=EnglishCandian
        ///9225=EnglishCaribean
        ///6153=EnglishIreland
        ///8201=EnglishJamaica
        ///5129=EnglishNewZealand
        ///13321=EnglishPhilippines
        ///7177=EnglishSouthAfrica
        ///11273=EnglishTrinidad
        ///2057=EnglishUnitedKingdom
        ///1033=EnglishUnitedStates
        ///12297=EnglishZimbabwe
        ///1061=Estonian
        ///1080=Faeroese
        ///1065=Farsi
        ///1035=Finnish
        ///2060=FrenchBelgian
        ///3084=FrenchCandian
        ///5132=FrenchLuxembourg
        ///6156=FrenchMonaco
        ///1036=FrenchStandard
        ///4108=FrenchSwiss
        ///1084=GaelicScots
        ///1110=Galician
        ///1079=Georgian
        ///3079=GermanAustrian
        ///5127=GermanLiechtenstein
        ///4103=GermanLuxembourg
        ///1031=GermanStandard
        ///2055=GermanSwiss
        ///1032=Greek
        ///1095=Gujarati
        ///1037=Hebrew
        ///1081=Hindi
        ///1038=Hungarian
        ///1039=Icelandic
        ///1057=Indonesian
        ///1040=ItalianStandard
        ///2064=ItalianSwiss
        ///1041=Japanese
        ///1099=Kannada
        ///1087=Kazakh
        ///1111=Konkani
        ///1042=Korean
        ///1088=Kyrgyz
        ///1062=Latvian
        ///1063=Lithuanian
        ///1071=Macedonian
        ///2110=MalayBruneiDarussalam
        ///1086=MalayMalaysia
        ///1082=Maltese
        ///1102=Marathi
        ///1104=Mongolian
        ///2068=NoewegianNynorsk
        ///1044=NorwegianBokmal
        ///1045=Polish
        ///2070=PortugeseStandard
        ///1046=PortugueseBrazilian
        ///1094=Punjabi
        ///1047=RhaetoRomanic
        ///1048=Romanian
        ///2072=RomanianMoldavia
        ///1049=Russian
        ///2073=RussianMoldavia
        ///1103=Sanskrit
        ///3098=SerbianCyrillic
        ///2074=SerbianLatin
        ///1051=Slovak
        ///1060=Slovenian
        ///1070=Sorbian
        ///11274=SpanishArgentina
        ///16394=SpanishBolivia
        ///13322=SpanishChile
        ///9226=SpanishColumbia
        ///5130=SpanishCostaRica
        ///7178=SpanishDominicanRepublic
        ///12298=SpanishEcuador
        ///17418=SpanishElSalvador
        ///4106=SpanishGuatemala
        ///18442=SpanishHonduras
        ///2058=SpanishMexican
        ///3082=SpanishModernSort
        ///19466=SpanishNicaragua
        ///6154=SpanishPanama
        ///15370=SpanishParaguay
        ///10250=SpanishPeru
        ///20490=SpanishPuertoRico
        ///1034=SpanishTraditionalSort
        ///14346=SpanishUruguay
        ///8202=SpanishVenezuela
        ///1072=Sutu
        ///1089=Swahili
        ///1053=Swedish
        ///2077=SwedishFinland
        ///1114=Syriac
        ///1097=Tanil
        ///1092=Tatar
        ///1098=Telugu
        ///1054=Thai
        ///1073=Tsonga
        ///1074=Tswana
        ///1055=Turkish
        ///1058=Ukrainian
        ///1056=Urdu
        ///2115=UzbekCyrillic
        ///1091=UzbekLatin
        ///1066=Vietnamese
        ///1076=Xhosa
        ///1085=Yiddish
        ///1077=Zulu
        ///33801=ENX
        ///16393=EnglishIndian
        /// </summary>
        [JsonProperty]
        public int LanguageCode { get; private set; }

        /// <summary>
        /// The name of the WAV file containing the recorded audio (voice name, greeting, etc.) for the parent object
        /// </summary>
        [JsonProperty]
        public string StreamFile { get; private set; }

        #endregion


        #region Static Classes


        /// <summary>
        /// Fetches a greeting stream file object filled with all the properties for a greeting stream for a custom greeting off a 
        /// directory handler
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server that the greeting is homed on.
        /// </param>
        /// <param name="pDirectoryHandlerObjectId">
        /// The objectID of the directory handler that owns the greeting to be fetched.
        /// </param>
        /// <param name="pLanguageCode">
        /// The language of the greeting stream to fetch
        /// </param>
        /// <param name="pGreetingStreamFile">
        /// The out parameter that the instance of the DirectoryHandlerGreetingStreamFile class filled in with the details of the 
        /// fetched entry is passed back on.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetGreetingStreamFile(ConnectionServer pConnectionServer,
                                                          string pDirectoryHandlerObjectId,
                                                          int pLanguageCode,
                                                          out DirectoryHandlerGreetingStreamFile pGreetingStreamFile)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pGreetingStreamFile = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetGreetingStreamFile";
                return res;
            }

            //create a new greeting instance passing the greeting type name and language which fills out the data automatically
            try
            {
                pGreetingStreamFile = new DirectoryHandlerGreetingStreamFile(pConnectionServer, pDirectoryHandlerObjectId, pLanguageCode);
                res.Success = true;
            }
            catch (UnityConnectionRestException ex)
            {
                return ex.WebCallResult;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch greeting stream file in GetGreetingStreamFile:" + ex.Message;
            }

            return res;

        }


        /// <summary>
        /// Returns all the greeting streams for a directory handler
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the greetings are being fetched from.
        /// </param>
        /// <param name="pDirectoryHandlerObjectId">
        /// GUID identifying the directory handler that owns the greetings being fetched
        /// </param>
        /// <param name="pGreetingStreamFiles">
        /// The list of DirectoryHandlerGreetingStreamFile objects are returned using this out parameter.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetGreetingStreamFiles(ConnectionServer pConnectionServer,
                                                           string pDirectoryHandlerObjectId,
                                                           out List<DirectoryHandlerGreetingStreamFile> pGreetingStreamFiles)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;
            pGreetingStreamFiles = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer passed to GetGreetingStreamFiles";
                return res;
            }
            
            if (string.IsNullOrEmpty(pDirectoryHandlerObjectId))
            {
                res.ErrorText = "Empty DirectoryHandlerObjectId passed to GetGreetingStreamFiles";
                return res;
            }

            string strUrl = string.Format("{0}handlers/directoryhandlers/{1}/directoryhandlerstreamfiles",
                                          pConnectionServer.BaseUrl, pDirectoryHandlerObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that does not mean an error - return true here along with an empty list.
            if (string.IsNullOrEmpty(res.ResponseText) || res.ResponseText.Equals("null"))
            {
                pGreetingStreamFiles = new List<DirectoryHandlerGreetingStreamFile>();
                return res;
            }

            pGreetingStreamFiles = HTTPFunctions.GetObjectsFromJson<DirectoryHandlerGreetingStreamFile>(res.ResponseText,"DirectoryHandlerStreamFile");

            if (pGreetingStreamFiles == null)
            {
                pGreetingStreamFiles = new List<DirectoryHandlerGreetingStreamFile>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pGreetingStreamFiles)
            {
                oObject.HomeServer = pConnectionServer;
                oObject.DirectoryHandlerObjectId = pDirectoryHandlerObjectId;
            }

            return res;
        }


        /// <summary>
        /// Fetches the recorded WAV file off the greeting stream file.  You need to pass in a target file location on the local files sytem to 
        /// store the WAV file and the stream file name on the server.  There is an overloaded version of this routine that takes the dir handler
        /// ID, greeting type and loanguage code instead of the stream file name which, in turn, looks up that stream file name and calls this 
        /// version.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that houses the greeting being fetched.
        /// </param>
        /// <param name="pTargetLocalFilePath">
        /// The fully qualified path to the file name where the WAV file will be stored on the local file system.
        /// </param>
        /// <param name="pConnectionStreamFileName">
        /// Stream file name for the greeting on the Connection server.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetGreetingWavFile(ConnectionServer pConnectionServer,
                                                       string pTargetLocalFilePath,
                                                       string pConnectionStreamFileName)
        {
            WebCallResult res = new WebCallResult();

            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetGreetingWavFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pTargetLocalFilePath) ||
                (Directory.GetParent(pTargetLocalFilePath).Exists == false))
            {
                res.ErrorText = "Invalid local file path passed to GetGreetingWavFile: " + pTargetLocalFilePath;
                return res;
            }

            if (string.IsNullOrEmpty(pConnectionStreamFileName))
            {
                res.ErrorText = "Empty wav file name passed to GetGreetingWavFile";
                return res;
            }

            //fetch the WAV file
            return HTTPFunctions.DownloadWavFile(pConnectionServer,
                                                pTargetLocalFilePath,
                                                pConnectionStreamFileName);
        }


        /// <summary>
        /// Overloaded GetGreetingWavFile function that takes the directory handler ID and language code which looks up 
        /// the stream file name to fetch from the Connection server.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that houses the greeting being fetched.
        /// </param>
        /// <param name="pTargetLocalFilePath">
        /// The fully qualified file path on the local OS to store the WAV file for this stream.
        /// </param>
        /// <param name="pDirectoryHandlerObjectId">
        /// The directory handler that owns the greeting being fetched.
        /// </param>
        /// <param name="pLanguageCode">
        /// The language code (i.e. US English = 1033).
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetGreetingWavFile(ConnectionServer pConnectionServer,
                                                       string pTargetLocalFilePath,
                                                       string pDirectoryHandlerObjectId,
                                                       int pLanguageCode)
        {

            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetGreetingWavFile";
                return res;
            }

            //check and make sure a legit folder is referenced in the target path
            if (String.IsNullOrEmpty(pTargetLocalFilePath) ||
                (Directory.GetParent(pTargetLocalFilePath).Exists == false))
            {
                res.ErrorText = "Invalid local file path passed to GetGreetingWavFile: " + pTargetLocalFilePath;
                return res;
            }

            if (string.IsNullOrEmpty(pDirectoryHandlerObjectId))
            {
                res.ErrorText = "Empty Directory handler ObjectId passed to GetGreetingWavFile";
                return res;
            }

            //fetch the greeting stream file object for this greeting and language code.
            DirectoryHandlerGreetingStreamFile oStreamFile;
            try
            {
                oStreamFile = new DirectoryHandlerGreetingStreamFile(pConnectionServer, pDirectoryHandlerObjectId, pLanguageCode);
            }
            catch (Exception ex)
            {
                //this will be rather common - keep the wording here toned down.
                res.ErrorText = "No greeting stream file found for greeting in GetGreetingWavFile: " + ex.Message;
                return res;
            }

            //fetch the StreamFile name from the directory - this identifies the actual WAV file in the streams folder on the Connection
            //server.  Normally if there's a greeting stream file record for a greeting there will be a stream file for it - but just 
            //in case.
            if (String.IsNullOrEmpty(oStreamFile.StreamFile))
            {
                res.ErrorText = "No recording found for stream file";
                return res;
            }

            //call the alternateive static definition that actually has the WAV file fetch logic in it.
            return GetGreetingWavFile(pConnectionServer, pTargetLocalFilePath, oStreamFile.StreamFile);
        }

  
      /// <summary>
      /// Update the recorded WAV file for a greeting stream for a specific language.  Each greeting can have multiple language versions recorded
      /// so the language along with the greeting itself need to be identified for updating.
      /// You have the option of having the WAV file converted into raw PCM prior to uploading to prevent codec compatibility issues with the 
      /// recording.
      /// </summary>
      /// <param name="pConnectionServer">
      /// The server the greeting being edited is homed on.
      /// </param>
      /// <param name="pDirectoryHandlerObjectId">
      /// The directory handler that owns the greeting being edited.
      /// </param>
      /// <param name="pLanguageCode">
      /// Language code to use (i.e. US English = 1033).  The LanguageCodes enum deinfed in the ConnectionServer class can be helpfull here.
      /// </param>
      /// <param name="pSourceLocalFilePath">
      /// The full path to a WAV file to upload as the greeting stream.
      /// </param>
      /// <param name="pConvertToPcmFirst">
      /// Optional parameter to convert the WAV file into PCM first.  Most codecs are handled by this conversion including G729a, MP3, GSM 610 etc...
      /// however if the file conversion fails then the upload itself is not attempted and failure is returned.
      /// If the file is already in PCM the conversion operation is just a copy and the upload proceeds as normal.
      /// </param>
      /// <returns>
      /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
      /// </returns>
        public static WebCallResult SetGreetingWavFile(ConnectionServer pConnectionServer,
                                                            string pDirectoryHandlerObjectId,
                                                            int pLanguageCode,
                                                            string pSourceLocalFilePath,
                                                            bool pConvertToPcmFirst = false)
      {

          return DirectoryHandler.SetGreetingWavFile(pConnectionServer, pSourceLocalFilePath, pDirectoryHandlerObjectId, pLanguageCode, pConvertToPcmFirst);
      }

        #endregion


        #region Instance Classes

        /// <summary>
        /// GreetingStreamFile display function - outputs the type, language and its stream file wav name (if any)
        /// </summary>
        /// <returns>
        /// String describing the greeting stream file
        /// </returns>
        public override string ToString()
        {
            return string.Format("Directory Handler Greeting LanguageCode={0} ({1}), recorded stream file={2}", 
                                 LanguageCode, (LanguageCodes)LanguageCode, StreamFile);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the greetingStreamFile object in "name=value" format - each pair is on its
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
        /// Fetches a greeting stream off a directory handler
        /// </summary>
        /// <param name="pDirectoryHandlerObjectId">
        /// The objectID of the directory handler that owns the greeting to be fetched.
        /// </param>
        /// <param name="pLanguageCode">
        /// Language of the stream file to fetch
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetGreetingStreamFile(string pDirectoryHandlerObjectId, int pLanguageCode)
        {
            string strUrl = string.Format("{0}handlers/directoryhandlers/{1}/directoryhandlerstreamfiles/{2}",
                                         HomeServer.BaseUrl, pDirectoryHandlerObjectId, pLanguageCode);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, HTTPFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }

            return res;
        }


        /// <summary>
        /// Gets the WAV file from the greeting stream file instance if it exists and stores it to the target file path on the local file system
        /// provided.
        /// </summary>
        /// <param name="pTargetLocalFilePath">
        /// Fully qualified path on the local file system to store the WAV file.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult GetGreetingWavFile(string pTargetLocalFilePath)
        {
            return GetGreetingWavFile(HomeServer, pTargetLocalFilePath, DirectoryHandlerObjectId,LanguageCode);
        }



        /// <summary>
        /// Update the recorded WAV file for a greeting stream for a specific language.  Each greeting can have multiple language versions recorded
        /// so the language along with the greeting itself need to be identified for updating.
        /// You have the option of having the WAV file converted into raw PCM prior to uploading to prevent codec compatibility issues with the 
        /// recording.
        /// This does NOT set the "play what" flag to indicate the greeting should play this WAV file instead of the system generated greeting. You
        /// will need to edit the greeting properties directly for that.
        /// </summary>
        /// <param name="pSourceLocalFilePath">
        /// The full path to a WAV file to upload as the greeting stream.
        /// </param>
        /// <param name="pConvertToPcmFirst">
        /// Optional parameter to convert the WAV file into PCM first.  Most codecs are handled by this conversion including G729a, MP3, GSM 610 etc...
        /// however if the file conversion fails then the upload itself is not attempted and failure is returned.
        /// If the file is already in PCM the conversion operation is just a copy and the upload proceeds as normal.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public WebCallResult SetGreetingWavFile(string pSourceLocalFilePath, bool pConvertToPcmFirst=false)
        {
            return SetGreetingWavFile(HomeServer, DirectoryHandlerObjectId, LanguageCode, pSourceLocalFilePath,pConvertToPcmFirst);
        }
        

        #endregion

    }
}

