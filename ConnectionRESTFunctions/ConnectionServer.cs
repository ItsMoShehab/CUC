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
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Diagnostics;

namespace Cisco.UnityConnection.RestFunctions
{
    // Class to hold the Connection version information - this is gathered when the server is attached to along with a few other
    // commonly needed items such as the primary location object ID.
    #region Version Class

    public class ConnectionVersion
    {
        public int Major;
        public int Minor;
        public int Build;
        public int Rev;
        public int Es;

        //constructor requires all values for version number - pass 0 for values that are not present.
        public ConnectionVersion(int pMajor, int pMinor, int pBuild, int pRev, int pEs) 
        {
            Major = pMajor;
            Minor = pMinor;
            Build = pBuild;
            Rev = pRev;
            Es = pEs;
        }

        //displays version in a readable format for logging and display purposes.
        public override string ToString()
        {
            if (Es>0)
            {
                return string.Format("{0}.{1}({2}.{3}) ES {4}",Major,Minor,Rev, Build,Es);
            }
            return string.Format("{0}.{1}({2}.{3})",Major,Minor,Rev,Build);
        }


        /// <summary>
        /// Returns true if the version of Connection installed on the server currently attached is the same as or newer (greater) than
        /// the version passed in.  This is useful for situations where data schemas vary across versions of Connection.
        /// </summary>
        /// <param name="pMajor">
        /// Major version.
        /// </param>
        /// <param name="pMinor">
        /// Minor version
        /// </param>
        /// <param name="pBuild">
        /// Build version
        /// </param>
        /// <param name="pRev">
        /// Rev version
        /// </param>
        /// <param name="pEs">
        /// Engineering special number
        /// </param>
        /// <returns>
        /// TRUE is returned if the version passed in is the same as or less than the version of Connection.
        /// </returns>
        public bool IsVersionAtLeast(int pMajor, int pMinor, int pRev, int pBuild, int pEs = 0)
        {
            if (Major > pMajor
                | Major == pMajor & Minor > pMinor
                | Major == pMajor & Minor == pMinor & Rev > pRev
                | Major == pMajor & Minor == pMinor & Rev == pRev & Build > pBuild
                | Major == pMajor & Minor == pMinor & Rev == pRev & Build == pBuild & Es >= pEs)
            {
                return true;
            }
            return false;
        }
    }

    #endregion

    /// <summary>
    /// Primary class for teh ConnectionRestFunctions library.  This is used to connect to and get/set data to and from a remote Connection
    /// server via the CUPI REST based web interface.
    /// </summary>
    public class ConnectionServer
    {

        #region Fields and Properties

        //values set at class creation time that are read only externally
        public ConnectionVersion Version { get; private set; }
        public  string ServerName { get; private set; }
        public  string LoginName { get; private set; }
        public  string LoginPw { get; private set; }

        //the start of the URL construction to get to the VMRest calls for the server this instance is pointing to.
        public string BaseUrl { get; private set; }

        //keeps track of the last session cookie we got from this server via HTTPS
        public string LastSessionCookie { get; set; }

        //keeps track of the last time we sent anything or got anything to/from this server via HTTPS
        public DateTime LastSessionActivity { get; set; }

        //information about cluster servers (if any) filled in at login time.  If there is no cluster the list
        //will contain only the same server being attached to by this instance of ConnectionServer.
        public List<VmsServer> VmsServers { get; private set; }

        //lazy fetch implementation for fetching the primary location objectId for the Connection server this instance is 
        //pointing to.
        private Location _primaryLocation;
        public string PrimaryLocationObjectId { 
            get
            {
                if (_primaryLocation == null)
                {
                    _primaryLocation = GetPrimaryLocation();
                }
                
                if (_primaryLocation == null)
                {
                    return "";
                }

                return _primaryLocation.ObjectId;
            }  
        }

        //Lazy fetch for getting the VMSserver ObjectID
        private VmsServer _vmsServer;
        public string VmsServerObjectId
        {
            get
            {
                if (_vmsServer == null)
                {
                    _vmsServer = GetVmsServer();
                }

                if (_vmsServer == null) return "";
                return _vmsServer.VmsServerObjectId;
            }
        }

        //Laxy fetch for getting VMSServer name.
        public string VmsServerName
        {
            get
            {
                if (_vmsServer == null)
                {
                    _vmsServer = GetVmsServer();
                }
                if (_vmsServer == null) return "";

                return _vmsServer.ServerName;
            }
        }

        #endregion 


        #region Constructors and Destructors
 

        /// <summary>
        /// default constructor - initalize everything to blank/0s
        /// </summary>
        public ConnectionServer()
        {
            ServerName = "";
            LoginName = "";
            LoginPw = "";
            BaseUrl = "";
            Version=new ConnectionVersion(0,0,0,0,0);
        }

        /// <summary>
        /// Constructor for the ConnectionServer class that allows the caller to provide the server name, login name and login password used to 
        /// authenticate to the CUPI interface on a Connection server.
        /// </summary>
        /// <param name="pServerName">
        /// Name or IP address of the remote Connection server to attach to.
        /// </param>
        /// <param name="pLoginName">
        /// Login name used to authenticate to the CUPI interface on the Connection server provided.
        /// </param>
        /// <param name="pLoginPw">
        /// Login password used to authenticate to the CUPI interface on the Connection server provided.
        /// </param>
        /// <returns>
        /// Instance of the ConnectionServer class
        /// </returns>
        public ConnectionServer (string pServerName, string pLoginName, string pLoginPw)
        {
            BaseUrl = string.Format("https://{0}:8443/vmrest/", pServerName);
            Version = new ConnectionVersion(0,0,0,0,0);
            
            if (string.IsNullOrEmpty(pServerName) | string.IsNullOrEmpty(pLoginName) | string.IsNullOrEmpty(pLoginPw))
            {
                throw new ArgumentException("Empty server name, login name or password provided on constructor");
            }

            LastSessionCookie = "";
            LastSessionActivity = DateTime.MinValue;

            //validate login.  This fills in the version and primary location object ID details.
            if (LoginToConnectionServer(pServerName,pLoginName,pLoginPw).Success==false)
            {
                ServerName = "";
                LoginName = "";
                LoginPw = "";
                BaseUrl = "";
                throw new Exception ("Login failed to Connection server:"+pServerName);
            }

            ServerName = pServerName;
            LoginName = pLoginName;
            LoginPw = pLoginPw;
        }

        #endregion


        #region Server Methods

        public override string ToString()
        {
            return string.Format("{0} [{1}]", ServerName, Version);
        }

        /// <summary>
        /// When creating an instance of the ConnectionServer class normally the login function here is called to establish that the server name, login name and
        /// login password provided are all valid and allow attaching to the CUPI interface on the remote Connection server.  As part of the login process the 
        /// full version information is fetched and parsed out and stored as a property on the class instance for easy reference later.
        /// </summary>
        /// <param name="pServerName">
        /// Connection server name or IP address to attach to. 
        /// </param>
        /// <param name="pLoginName"></param>
        /// The Login name to use when attaching to the Connection server's CUPI interface.  Needs to have the admin role in Connection.
        /// <param name="pLoginPw">
        /// The login password to use when attachign to the Connection server's CUPI interface.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.  If the login fails the Success property
        /// on the return class will be FALSE, otherwise TRUE is returned.
        /// </returns>
        private WebCallResult LoginToConnectionServer(string pServerName, string pLoginName, string pLoginPw)
        {
            this.LoginName = pLoginName;
            this.LoginPw = pLoginPw;

            WebCallResult ret = GetVersionInfo(pServerName);
            if (ret.Success == false)
            {
                return ret;
            }

            return GetClusterInfo();
        }


        /// <summary>
        /// Helper method to fetch the Conneciton version number off the server we're attaching to
        /// </summary>
        /// <returns></returns>
        private WebCallResult GetVersionInfo(string pServerName)
        {
            WebCallResult ret = HTTPFunctions.GetCupiResponse(BaseUrl + "version", MethodType.GET, this, "");

            if (ret.Success == false)
            {
                return ret;
            }

            string strVersion;
            Dictionary<string, string> oResponse;
            if (string.IsNullOrEmpty(ret.ResponseText))
            {
                //invalid JSON returned
                ret.ErrorText = string.Format("Invalid version XML returned logging into Connection server: {0}, return text={1}", pServerName, ret.ResponseText);
                ret.Success = false;
                return ret;
            }

            try
            {
                oResponse = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(ret.ResponseText);
            }
            catch (Exception ex)
            {
                ret.ErrorText = string.Format("Invalid version XML returned logging into Connection server: {0}, return text={1}, error={2}",
                    pServerName, ret.ResponseText, ex);
                ret.Success = false;
                return ret;
            }

            oResponse.TryGetValue("version", out strVersion);

            if (ParseVersionString(strVersion) == false)
            {
                //Invalid version string encountered (or no version string returned).
                ret.ErrorText = String.Format("No version version returned logging into Connection server: {0}, return text={1}", pServerName, ret.ResponseText);
                ret.Success = false;
                return ret;
            }

            return ret;
        }

        /// <summary>
        /// fetch the servers in the cluster - if the call succeeds the list (which may contain only the local server) will be stored
        /// on the VmsServers property off the instance of this class
        /// </summary>
        public WebCallResult GetClusterInfo()
        {
            List<VmsServer> oServers;
            var ret= VmsServer.GetVmsServers(this, out oServers);
            if (ret.Success == false)
            {
                return ret;
            }

            VmsServers = oServers;

            return ret;
        }


        /// <summary>
        ///Parse out the version string passed back from the version directive and break it into its individual parts so we can do 
        ///basic version checking easily.
        /// </summary>
        /// <param name="pVersionString">
        /// A valid version string must contain at least 3 parts like "8.5.1" - but will almost always contain 4 like "8.5.1.0ES22".   
        /// </param>
        /// <returns>
        /// TRUE if a valid version string was passed in to pad, false otherwise.
        /// </returns>
        internal bool ParseVersionString(string pVersionString)
        {
            int iTemp;
            string[] strVersionChunks = pVersionString.Split('.');

            if (strVersionChunks.Count()<3)
            {
                return false;
            }

            //major is first up
            if (int.TryParse(strVersionChunks[0], out iTemp) == false)
            {
                return false;
            }
            Version.Major = iTemp;

            //minor is up next
            if (int.TryParse(strVersionChunks[1], out iTemp) == false)
            {
                return false;
            }
            Version.Minor = iTemp;

            //build is the 3rd eleement
            if (int.TryParse(strVersionChunks[2], out iTemp) == false)
            {
                return false;
            }
            Version.Rev = iTemp;

            //The build element may or may not be there in some versions and it may contain both a build AND and ES number tacked 
            //onto the string so we have to handle this special.
            if (strVersionChunks.Count()<4)
            {
                Version.Build = 0;
                Version.Es = 0;
                return true;
            }

            //0ES22
            if (strVersionChunks[3].Contains("ES"))
            {
              	//there will be a leading digit or digits, followed by an "ES" followed by trailing digit/digits
                string[] esChunks = strVersionChunks[3].Replace("ES", ".").Split('.');
                if (esChunks.Count()!=2)
                {
                    return false;
                }

                //In rare cases the build can be blank - don't treat that as a failur although you shouldn't run into this in production builds.
                if (int.TryParse(esChunks[0], out iTemp))
                {
                    Version.Build = iTemp;
                }

                //get ES - in rare cases the ES will not be a number - this is technically a problem in the versioning scheme but don't 
                //treat it as a failure here
                if (int.TryParse(esChunks[1], out iTemp))
                {
                    Version.Es = iTemp;    
                }
            }
            else
            {
                //just a digit or digits for the build
                if (int.TryParse(strVersionChunks[3], out iTemp) == false)
                {
                    return false;
                }
    
                Version.Build = iTemp;
            }

            return true;
        }


        /// <summary>
        ///General purpose routine that will take an instance of an object (forinstance a User instance) and an XMLElement (i.e. looks like
        /// "<FirstName>Jeff</FirstName>" and populates the appropriate field in the class instance with the value from the element.
        ///This requires that the name of the property on the class matches the element name (including case).  Proper type conversion is 
        ///handled and logic to make sure the property is represented on the class and the value is valid (exists) is included.
        ///NOTE: The User, CallHandler etc... classes all define the object id as "ObjectId" and in XML its preresented as "ObjectId" - this 
        ///is deliberate as I want that value skipped since it's the unique identifier used in the class constructors and I don't want them being
        ///updated during a sweep of properties.
        /// </summary>
        /// <param name="pObject">
        /// Object to fill in properties for.
        /// </param>
        /// <param name="pElement">
        /// XML Element that holds a value/name corresponding to a value in the object
        /// </param>
        internal void SafeXmlFetch(Object pObject, XElement pElement)
        {
            //if this is a "complex" type, process the sub elements - this comes into play mostly for messages which got a little cute with 
            //complex types for numerous properties
            if (pElement.HasElements)
            {
                //grab the handle to the sub object and then process the sub elements onto it.  If this is a generic list of instances
                //of a type then this scenario needs to be handled by hand.
                object pSubObject;
                PropertyInfo propInfo = pObject.GetType().GetProperty(pElement.Name.LocalName);
                if (propInfo != null)
                {
                    pSubObject = propInfo.GetValue(pObject, null);
                }
                else
                {
                    return;
                }

                if (pSubObject == null)
                {
                    return;
                }
                


                foreach (XElement subElement in pElement.Elements())
                {
                    GetXMLProperty(pSubObject, subElement, subElement.Name.LocalName);
                }
            }
            else
            {
                GetXMLProperty(pObject, pElement, pElement.Name.LocalName);
            }
        }

        /// <summary>
        ///General purpose routine that will take an instance of an object (forinstance a User instance) and an XMLElement (i.e. looks like
        /// "<FirstName>Jeff</FirstName>" and populates the appropriate field in the class instance with the value from the element.
        ///This requires that the name of the property on the class matches the element name (including case).  Proper type conversion is 
        ///handled and logic to make sure the property is represented on the class and the value is valid (exists) is included.
        ///NOTE: The User, CallHandler etc... classes all define the object id as "ObjectId" and in XML its preresented as "ObjectId" - this 
        ///is deliberate as I want that value skipped since it's the unique identifier used in the class constructors and I don't want them being
        ///updated during a sweep of properties.
        /// </summary>
        /// <param name="pObject">
        /// Object to fill in properties for.
        /// </param>
        /// <param name="pElement">
        /// XML Element that holds a value/name corresponding to a value in the object
        /// </param>
        /// <param name="pName">
        /// Name of the property to look for - if it's not on the object, return.  
        /// </param>
        private void GetXMLProperty(object pObject, XElement pElement, string pName)
        {
            //four value types to handle the four values we can pull from XML via the CUPI interface.

            //if the property is not defined on our class (some of the URI properties are redundant) return.
            if (pObject.GetType().GetProperty(pName) == null)
            {
                if (pName.Contains("URI"))
                {
                    return;
                }
                Console.WriteLine("Missing property value:" + pName);
                return;
            }

            //we need to know to target type of the element so we can cast is properly - for each type parse out the value field 
            //into the appropraite type and then add that type to the object property.
            switch (pObject.GetType().GetProperty(pName).PropertyType.FullName.ToLower())
            {
                case "system.int32":
                    int intValue = (pElement.Value == null) ? 0 : int.Parse(pElement.Value);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, intValue, null);
                    break;
                case "system.int64":
                    long longValue = (pElement.Value == null) ? 0 : long.Parse(pElement.Value);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, longValue, null);
                    break;
                case "system.string":
                    string strValue = (pElement.Value == null) ? "" : pElement.Value.ToString();
                    pObject.GetType().GetProperty(pName).SetValue(pObject, strValue, null);
                    break;
                case "system.boolean":
                    bool? boolValue = bool.Parse(pElement.Value);
                    pObject.GetType().GetProperty(pName).SetValue(pObject, boolValue, null);
                    break;
                case "system.datetime":
                    DateTime dateValue = DateTime.Parse(pElement.Value);

                    pObject.GetType().GetProperty(pName).SetValue(pObject, dateValue, null);
                    break;
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    Console.WriteLine("Unknown type encountered in GetXMLProperty on ConnectionServer.cs:"
                                   + pObject.GetType().GetProperty(pName).PropertyType.FullName.ToLower());
                    break;
            }
        }


        //Use a simple set of command line tools tools to convert just about any WAV format into raw PCM format that Connection will be 
        //happy with.  This will handle GSM6.10, mp3, G729a, G726 and many other WAV formats I've run into in the field - the same library
        //is used in COBRAS when importing Windows based backups (which may have numerous WAV formats for greetings and voice names) into 
        //Connection.
        public string ConvertWavFileToPcm(string pPathToWavFile)
        {
            //create a temporary file with a GUID file name in the temporary folder for the local OS install.
            string strConvertedWavFilePath = Path.GetTempFileName() + ".wav";


            // Use ProcessStartInfo class - this lets us set properties, hide the "box" window and wait for the process to complete before 
            //continuing.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;

            //When running in Unit Test mode MSTest struggles to get additional files copied over to the on-the-fly context folder properly so
            //sub folders are not recreated - check to see if the wavcopy.exe is in the folder we're currently operating in (unit test mode) or
            //a sub folder called WavConvert (production mode).  This is the only concession we have to make in production code to get unit tests
            //to run smoothly so it's not the end of the world.
            if (File.Exists(@"WAVConvert\wavcopy.exe"))
            {
                startInfo.FileName = @"WAVConvert\wavcopy.exe";
            }
            else
            {
                startInfo.FileName = @"wavcopy.exe";
            }
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            //Connection is happier with PCM at 8Khz, 16 bit and mono - to rip the wav file into that format if requested.
            startInfo.Arguments = string.Format("\"{0}\" \"{1}\" -pcm:8000,16,1", pPathToWavFile, strConvertedWavFilePath);
            try
            {
                // Start the process with the info we specified.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    //30 seconds is much more than enough, even if the file is enormous
                    exeProcess.WaitForExit(30000);
                }
            }
            catch
            {
                //log the error here in a production application - for our purposes we just pass back blank for the file 
                //name indicating there was an issue.
                strConvertedWavFilePath = "";
            }

            return strConvertedWavFilePath;

        }


        /// <summary>
        /// Helper function to convert an set of action data to human readable string for display.  The action data includes an action ID (0 is ignore, 
        /// 1 is hangup, 2 is goto etc...), conversation name (if any) and a target handler objectID (if any).  These three items are used repeatedly in
        /// Connection's database to indicate an action to take for, say, user input keys, after greeting, exiting the subscriber conversation, leaving
        /// a name lookup handler etc... 
        /// This does a database lookup of the target objectID to construct a simple human readable description for items such as "Ring phone for handler 
        /// [operator]"
        /// </summary>
        /// <param name="pAction">
        /// Action value (0 is ignore, 1 is hangup, 2 is goto, etc...)
        /// </param>
        /// <param name="pConversationName">
        /// Conversation name such as PHTransfer, PHGreeting, AD etc...  this can be blank
        /// </param>
        /// <param name="pTargetHandlerObjectId">
        /// The destination ObjectId of a handler (interviewer, name lookupg handler or call handler including a primary call handler for a user). This can 
        /// be blank.
        /// </param>
        /// <returns>
        /// Human readable string describing the action sequence for display/logging purposes.
        /// </returns>
        public string GetActionDescription(int pAction, string pConversationName, string pTargetHandlerObjectId)
        {
            switch (pAction)
            {
                    //Take care of the action types that do not reference any target or conversation name first.
                case 0:
                    return "Ignore";
                case 1:
                    return "Hang up.";
                case 3:
                    return "Play error greeting.";
                case 4:
                    return "Take message.";
                case 5:
                    return "Skip Greeting";
                case 6:
                    return "Repeat Greeting";
                case 8:
                    return "Route from next call routing rule";
                case 7:
                    return "Transfer to alternate contact number";
                case 2:
                    {
                        //2 is "goto" which requires a conversation name and, in many cases, also a target object to load up for 
                        //that conversation (for instance PHGreeting is the greeting conversation that takes a call handler as a
                        //target). 
                        if (string.IsNullOrEmpty(pConversationName))
                        {
                            return "(error) invalid empty conversaton name passed to GetActionDescription ";
                        }

                        WebCallResult ret;
                        CallHandler oHandler;
                        switch (pConversationName.ToLower())
                        {
                            case "subsignin":
                                return "Route to user sign in";
                            case "greetingsadministrator":
                                return "Route to Greetings administrator";
                            case "convhotelcheckedout":
                                return "Route to checked out hotel guest conversation";
                            case "convcvmmboxreset":
                                return "Route to Community Voice Mail box reset";

                            //a few of these had different names in Unity versions - I leave the overloads in here
                            case "avconvsystemtransfer":
                            case "subsystransfer":
                                return "Route to user system transfer";
                            case "systemtransfer":
                                return "Route to system transfer";
                            case "avconveasysignin":
                            case "easysignin":
                                return "Route to easy subscriber sign in";
                            case "transferaltcontactnumber":
                                return "Alternate contact number";
                            case "broadcastmessageadministrator":
                                return "Broadcast message administrator";
                            case "ad":
                                {
                                    //Alpha Directory (what we used to call Name Lookup Handlers) - this requires a name lookup handler target 
                                    //to load - fetch that handler here so we can showt he display name here.
                                    DirectoryHandler oDirHandler;
                                    ret = DirectoryHandler.GetDirectoryHandler(out oDirHandler, this, pTargetHandlerObjectId);
                                    if (ret.Success==false)
                                    {
                                        return "Invalid link!";
                                    }

                                    return "Route to name lookup handler: "+oDirHandler.DisplayName;
                                }
                            case "phtransfer":
                                {
                                    //PHTransfer is the transfer conversation entry point for a call handler (subscriber's primary call handler as well
                                    //of course).  It requires a valid target handler Object which we'll fetch here so we can include it's display name
                                    //in the output.
                                    ret = CallHandler.GetCallHandler(out oHandler, this, pTargetHandlerObjectId);
                                    if (ret.Success==false)
                                    {
                                        return "Invalid link!";
                                    }

                                    if (oHandler.IsPrimary)
                                    {
                                        return "Ring phone for subscriber:" + oHandler.DisplayName;
                                    }
                                    return "Ring phone for call handler:" + oHandler.DisplayName;
                                }
                            case "phgreeting":
                                {
                                    //PHGreeting is the greeting conversation entry point for a call handler (subscriber's primary call handler as well
                                    //of course).  It requires a valid target handler Object which we'll fetch here so we can include it's display name
                                    //in the output.
                                    ret = CallHandler.GetCallHandler(out oHandler, this, pTargetHandlerObjectId);
                                    if (ret.Success == false)
                                    {
                                        return "Invalid link!";
                                    }

                                    if (oHandler.IsPrimary)
                                    {
                                        return "Send to greeting for subscriber:" + oHandler.DisplayName;
                                    }
                                    return "Send to greeting for call handler:" + oHandler.DisplayName;
                                }

                            case "phinterview":
                            case "chinterview":
                                {
                                    //PHInterview or CHInterview (older style) is the conversation for an interview handler that requires a valid 
                                    //interview handler target to load.  Fetch it here so we can include it's display name in the output.
                                    InterviewHandler oInterviewHandler;
                                    ret = InterviewHandler.GetInterviewHandler(out oInterviewHandler, this,pTargetHandlerObjectId);
                                    if (ret.Success == false)
                                    {
                                        return "Invalid Link!";
                                    }
                                    return "Send to interview handler: "+oInterviewHandler.DisplayName;
                                }

                            default:
                                //Not a conversation name we recognize - there are clients that customize this so be sure to handle this cleanly.
                                if (Debugger.IsAttached) Debugger.Break();
                                return "(error) invalid conversation name:" + pConversationName;
                        }
                    }
                default:
                    //Unknown action type - this should never be possible due to FK constraints in the DB, just just in case.
                    return "(error) invalid action type:" + pAction.ToString();
            }
        }


        #endregion


        #region Helper Methods

        /// <summary>
        /// Simple function to take an alias of a user and their password and indicate if they are capable of authenticating against
        /// the server or not
        /// </summary>
        /// <param name="pLoginName">
        /// The login name (alias) of the user you want to validate
        /// </param>
        /// <param name="pPassword">
        /// Password (GUI password - not the PIN for the phone password) for the user to verify.
        /// </param>
        /// <param name="pUser">
        /// If a match is found and the password is valid an instance of User is created and filled in with the target user's 
        /// details and passed back on this out parameter
        /// </param>
        /// <returns>
        /// True if the user is found and the password hash matches.  False if not.
        /// </returns>
        public bool ValidateUser(string pLoginName, string pPassword, out UserBase pUser)
        {
            pUser = null;
            try
            {
                new ConnectionServer(this.ServerName, pLoginName, pPassword);
            }
            catch 
            {
                return false;
            }
            
            //fetch the objectId of the user - unfortunately if the user is not an admin then the ObjectId is not 
            //returned from a basic properties fetch so we have to do this as a 2nd call via the admin interface
            //instead.
            WebCallResult res= UserBase.GetUser(out pUser, this, "", pLoginName);
            return res.Success;
        }

        /// <summary>
        /// Version of the validate user method that does not return an instance of User - this eliminates the need for a 2nd
        /// fetch request and is quicker as a result - if you only need to verify that a user's password is valid but don't need 
        /// to do anything with that user, this is the version to use.
        /// </summary>
        /// <param name="pLoginName">
        /// The login name (alias) of the user you want to validate
        /// </param>
        /// <param name="pPassword">
        /// Password (GUI password - not the PIN for the phone password) for the user to verify.
        /// </param>
        /// <returns>
        /// True if the user is found and the password hash matches.  False if not.
        /// </returns>
        public bool ValidateUser(string pLoginName, string pPassword)
        {
            try
            {
                new ConnectionServer(this.ServerName, pLoginName, pPassword);
            }
            catch 
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Simple method to fetch the primary location ObjectId of the Connection server the instance of this class is pointing to.
        /// </summary>
        /// <returns>
        /// Primary location objectId for the Connection server.
        /// </returns>
        private Location GetPrimaryLocation()
        {
            List<Location> oList;

            //there should only ever be one location defined that has the "isprimary" set to true.
            WebCallResult res= Location.GetLocations(this, out oList,"query=(IsPrimary is 1)");

            if (res.Success == false)
            {
                return null;
            }

            if (oList.Count != 1)
            {
                return null;
            }

            return oList[0];
        }

        /// <summary>
        /// Simple method to fetch the VMSServer ObjectId for the Connection server attached to this instance of the ConnectionServer class.
        /// If this is a cluster the ObjectId of the VMSServer that corresponds to the IP address of the Connection server this instance is 
        /// attached to is returned.  
        /// </summary>
        /// <returns>
        /// ObjectId of the VMSServer currently attached.
        /// </returns>
        private VmsServer GetVmsServer()
        {
            List<VmsServer> oList;
            //there should only ever be one location defined that has the "isprimary" set to true.
            WebCallResult res = VmsServer.GetVmsServers(this, out oList);

            if (res.Success == false)
            {
                return null;
            }

            if (oList.Count == 0)
            {
                return null;
            }

            //select the one that matches the Connection server's IP address we're connected to
            foreach (var oServer in oList)
            {
                if (oServer.HomeServer.ServerName == this.ServerName )
                {
                    return oServer;
                }
            }

            return null;
        }


        /// <summary>
        ///armed with the object type, it's object ID and the server name we can construct a URL to open the SA web page to that 
        ///object directly.  Only the frame showing the object details itself are shown, the navigation at the top and tree to the 
        /// left in CUCA are hidden - this makes it easy to "embed" an admin interface and leverage the CUCA for your applications.
        /// It's not lazy, it's efficent.
        /// </summary>
        /// <param name="pObjectType">
        /// The type of object (call handler, subscriber, distribution list etc...) you want to launch the CUCA page for.
        /// </param>
        /// <param name="strObjectId">
        /// GUID identifying the object to be shown in CUCA.
        /// </param>
        /// <returns>
        /// full URI to get to the CUCA for the object in question - blank if there was an error processing the request.
        /// </returns>
        public string GetCucaUrlForObject(ConnectionObjectType pObjectType, string strObjectId)
        {
            string strUrl = "https://" + this.ServerName + ":8443/cuadmin/";

            switch (pObjectType)
            {
                case ConnectionObjectType.InterviewHandler:
                    strUrl += "interview-handler.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.NameLookupHandler:
                    strUrl += "directory-handler.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.User:
                    strUrl += "user.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.SystemCallHandler:
                    strUrl += "callhandler.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.RoutingRuleDirect:
                    strUrl += "routing-rule.do?op=readDirectRule&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.RoutingRuleForwarded:
                    strUrl += "routing-rule.do?op=readForwardedRule&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.SmppProvider:
                    strUrl += "smpp-provider.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.SubscriberTemplate:
                    strUrl += "user.do?op=readTemplate&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.CallHandlerTemplate:
                    strUrl += "callhandler.do?op=readTemplate&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Schedule:
                    strUrl += "schedule.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Partition:
                    strUrl += "partition.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.SearchSpace:
                    strUrl += "search-space.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Switch:
                    strUrl += "media-switch.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.DistributionList:
                    strUrl += "edit-distribution-list.do?objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.PersonalCallTransferRule:
                    //PCA "hop" link is constructed a bit differently
                    strUrl = "https://" + this.ServerName + ":8443/ciscopca/runas.do?userid=" + strObjectId + "&startat=unitycallroutingrules/rulesets.do";
                    break;
                case ConnectionObjectType.SystemContact:
                case ConnectionObjectType.VpimContact:
                    strUrl += "contact.do?op=read&objectId=" + strObjectId;
                    break;
                case ConnectionObjectType.Cos:
                    //https://fmstest3.cisco.com:8443/cuadmin/cos.do?op=read&objectId=85ba8e9b-1f46-491c-b9e1-8e03e3c89124
                    strUrl += "cos.do?op=read&objectId=" + strObjectId;
                    break;
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }

            return strUrl;
        }


        #endregion

    }
}

