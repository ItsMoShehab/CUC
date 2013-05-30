using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for ConnectionServerTest and is intended
    ///to contain all ConnectionServerTest Unit Tests
    ///</summary>
    [TestClass]
    public class ConnectionServerTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Used for making sure json parsing errors raise events properly
        /// </summary>
        private static int _errorsRaised = 0;


        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            //create a connection server instance used for all tests - rather than using a mockup 
            //for fetching data I prefer this "real" testing approach using a public server I keep up
            //and available for the purpose - the conneciton information is stored in the test project's 
            //settings and can be changed to a local instance easily.
            Settings mySettings = new Settings();
            Thread.Sleep(300);
            try
            {
                 _connectionServer = new ConnectionServerRest(mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start ConnectionServerRest test:" + ex.Message);
            }

        }

        #endregion


        #region Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a blank server name is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            //invalid login value - empty server name
            ConnectionServerRest oTestServer = new ConnectionServerRest(new RestTransportFunctions(), "", "login", "Pw");
            Console.WriteLine(oTestServer);
        }

        /// <summary>
        /// Make sure an Exception is thrown if 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailureBadLogin()
        {
            ConnectionServerRest oTestServer = new ConnectionServerRest(new RestTransportFunctions(),  "badservername", "badloginname", "badpassword");
            Console.WriteLine(oTestServer);
        }

        #endregion


        [TestMethod]
        public void ConstructorsWithDifferentTransports()
        {
            ConnectionServerRest oServer;
            
            oServer= new ConnectionServerRest(null);
            Assert.IsFalse(string.IsNullOrEmpty(oServer.Version.ToString()),"No version string for default constructor");

            oServer = new ConnectionServerRest(new RestTransportFunctions());
            Assert.IsFalse(string.IsNullOrEmpty(oServer.Version.ToString()), "No version string for REstTransportFunctions constructor");

            oServer = new ConnectionServerRest(new TestTransportFunctions());
            Assert.IsFalse(string.IsNullOrEmpty(oServer.Version.ToString()), "No version string for TestTransportFunctions constructor");
        }


        [TestMethod]
        public void ConnectionVersion()
        {
            ConnectionVersion oVersion=null;
            try
            {
                oVersion = new ConnectionVersion(1,2,3,4,5);
            }
            catch (Exception ex)
            {
                Assert.Fail("Construction of version failed:"+ex);
            }
                
            string strVersion = oVersion.ToString();
            Assert.IsTrue(strVersion.Contains("ES"),"Version with ES value does not produce ES in string:"+oVersion);

            try
            {
                oVersion = new ConnectionVersion(1, 2, 3, 4,0);
            }
            catch (Exception ex)
            {
                Assert.Fail("Construction of version failed:" + ex);
            }

            strVersion = oVersion.ToString();
            Assert.IsFalse(strVersion.Contains("ES"), "Version without ES value produce ES in string:" + oVersion);
        }

        /// <summary>
        /// check all failure paths for version check
        /// </summary>
        [TestMethod]
        public void VersionCheck_Failure()
        {
            Console.WriteLine(_connectionServer.Version.ToString());
            Assert.IsTrue(_connectionServer.Version.IsVersionAtLeast(7, 0, 0, 0), "Minimum version check failed");

            Assert.IsFalse(_connectionServer.Version.IsVersionAtLeast(9999, 0, 0, 0), "Invalid major version not caught");

            Assert.IsFalse(_connectionServer.Version.IsVersionAtLeast(_connectionServer.Version.Major, 9999, 0, 0),
                            "Invalid minor version not caught");

            Assert.IsFalse(_connectionServer.Version.IsVersionAtLeast(_connectionServer.Version.Major, _connectionServer.Version.Minor, 9999, 0),
                            "Invalid rev version not caught");

            Assert.IsFalse(_connectionServer.Version.IsVersionAtLeast(_connectionServer.Version.Major, _connectionServer.Version.Minor, _connectionServer.Version.Rev, 9999),
                            "Invalid build version not caught");
        }


        /// <summary>
        /// check all insertion routes into the XMLFetch routine.
        /// </summary>
        [TestMethod]
        public void SafeXmlFetchTest_User()
        {
            UserBase oUser = new UserBase(_connectionServer);
            UserFull oUserFull = new UserFull(_connectionServer);

            //integer
            XElement oElement = XElement.Parse("<Language>1234</Language>");
            _connectionServer.SafeXmlFetch(oUser,oElement);
            Assert.AreEqual(oUser.Language, 1234, "Language integer did not insert properly.");

            //string
            oElement = XElement.Parse("<ConversationTui>SubMenu</ConversationTui>");
            _connectionServer.SafeXmlFetch(oUserFull, oElement);
            Assert.AreEqual(oUserFull.ConversationTui.Description(), "SubMenu", "SubMenu string did not insert properly");

            //boolean
            oElement = XElement.Parse("<IsTemplate>false</IsTemplate>");
            _connectionServer.SafeXmlFetch(oUserFull, oElement);
            Assert.IsFalse(oUserFull.IsTemplate, "IsTemplate boolean did not insert properly");

            //DateTime
            oElement = XElement.Parse("<CreationTime>2011-08-27T05:00:21Z</CreationTime>");
            _connectionServer.SafeXmlFetch(oUser, oElement);
            //Time above is universal time
            Assert.IsTrue(oUser.CreationTime.Equals(DateTime.Parse("2011/08/27 5:00:21").ToLocalTime()),"Creation time failed to parse correctly");

            //Unknown property name
            oElement = XElement.Parse("<Bogus>false</Bogus>");
            _connectionServer.SafeXmlFetch(oUser, oElement);
        }

        /// <summary>
        /// display name and version number of server
        /// </summary>
        [TestMethod]
        public void DisplayInfo()
        {
            Console.WriteLine(_connectionServer.ToString());
        }


        /// <summary>
        /// Create a new instance of a Connection server object without logging in
        /// </summary>
        [TestMethod]
        public void ConstructorPlain()
        {
            ConnectionServerRest oTestServer = new ConnectionServerRest(new RestTransportFunctions());
            Console.WriteLine(oTestServer);
        }


        /// <summary>
        ///A test for ParseVersionString - use a seperate instance of the ConnectionServer object for this so as not to 
        /// corrupt other tests.
        ///</summary>
        [TestMethod]
        public void ParseVersionString_Failure()
        {
            ConnectionServerRest oTempServer = new ConnectionServerRest(new RestTransportFunctions());

            Assert.IsFalse(oTempServer.ParseVersionString(""), "Empty string");
            Assert.IsFalse(oTempServer.ParseVersionString("1"), "Invalid number of version parts");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2"), "Invalid number of version parts");
            Assert.IsFalse(oTempServer.ParseVersionString("a.2.3"), "Invalid major");
            Assert.IsFalse(oTempServer.ParseVersionString("1.a.3"), "Invalid minor");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.a"), "Invalid rev");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.3.a"), "Invalid build");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.3.4ES4ES"), "Invalid ES");

            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3"), "Failed to parse legal version string with 3 elements");

            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3.4ES5"),"Failed to parse legal version string");
            Assert.AreEqual(oTempServer.Version.Major, 1, "Major is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Minor, 2, "Minor is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Rev, 3, "Rev is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Build,4,"Build is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Es, 5, "ES is not parsed out correctly");

        }

        /// <summary>
        /// Test various paths down the action string construction path
        /// </summary>
        [TestMethod]
        public void ActionStringConstructions()
        {
            //terminal action types
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.Ignore, ConversationNames.Invalid, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.Hangup, ConversationNames.Invalid, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.Error,ConversationNames.Invalid, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.TakeMessage, ConversationNames.Invalid, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.SkipGreeting, ConversationNames.Invalid, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.RestartGreeting, ConversationNames.Invalid, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.TransferToAlternateContactNumber, ConversationNames.Invalid, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.RouteFromNextCallRoutingRule, ConversationNames.Invalid, ""));

            //goto actions
            //Converstaion names
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.GreetingsAdministrator, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.ConvHotelCheckedOut, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.ConvCvmMboxReset, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.SubSysTransfer, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.EasySignIn, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.TransferAltContactNumber, ""));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.BroadcastMessageAdministrator, ""));
            
            //route to object types
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo,ConversationNames.Ad , "blah"));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo,ConversationNames.PHTransfer, "blah"));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHGreeting, "blah"));
            Console.WriteLine(_connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHInterview, "blah"));
        }


        [TestMethod]
        public void JsonParsingErrors()
        {
            ConnectionServerRest oTemp = null;
            try
            {
                oTemp = new ConnectionServerRest(_connectionServer.ServerName,_connectionServer.LoginName,_connectionServer.LoginPw);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unable to attach to Connection server to start ConnectionServerRest test:" + ex.Message);
            }

            oTemp.ErrorEvents += (sender, args) => { _errorsRaised++; };

            var oVmsServer = oTemp.GetObjectFromJson<VmsServer>("{\"URI\":\"/vmrest/vmsservers/99846e45-c254-4755-aec4-341503683cee\",\"ObjectId2\":\"99846e45-c254-4755-aec4-341503683cee\",\"ServerName2\":\"cuc10b164\",\"IpAddress2\":\"192.168.0.186\",\"VmsServerObjectId2\":\"99846e45-c254-4755-aec4-341503683cee\",\"ClusterMemberId\":\"0\",\"ServerState\":\"1\",\"HostName\":\"cuc10b164.jefflocal.org\",\"ServerDisplayState\":\"3\",\"SubToPerformReplicationRole\":\"false\"}");
            Thread.Sleep(1000);
            Assert.IsTrue(_errorsRaised>0,"Invalid Json parsing for object did not raise an error");
        }


        [TestMethod]
        public void HarnessTest_Constructor()
        {
            try
            {
                ConnectionServerRest oServer = new ConnectionServerRest(new TestTransportFunctions(), "server", "login",
                                                                "password", false);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed creating server for version older than 9.x:" + ex);
            }

            try
            {
                ConnectionServerRest oServer = new ConnectionServerRest(_connectionServer.ServerName,_connectionServer.LoginName,
                    _connectionServer.LoginPw,false);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed creating server without admin login:" + ex);
            }

            try
            {
                ConnectionServerRest oServer = new ConnectionServerRest(new TestTransportFunctions(), "EmptyResultText", "login","password", false);
                Assert.Fail("Creating server with empty results text for version check did not fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected creation failure:"+ex);
            }

            try
            {
                ConnectionServerRest oServer = new ConnectionServerRest(new TestTransportFunctions(), "ErrorResponse", "login", "password", false);
                Assert.Fail("Creating server with error response for version check did not fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected creation failure:" + ex);
            }

            try
            {
                ConnectionServerRest oServer = new ConnectionServerRest(new TestTransportFunctions(), "InvalidResultText", "login", "password", false);
                Assert.Fail("Creating server with invalid results text for version check did not fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected creation failure:" + ex);
            }
        }


        public class TestClass
        {
            public string URI { get; set; }
            public System.Int16 Int16 { get; set; }
            public System.Int32 Int32 { get; set; }
            public System.Int64 Int64 { get; set; }
            public string String { get; set; }
            public bool Boolean { get; set; }
            public DateTime DateTime { get; set; }
            public Cisco.UnityConnection.RestFunctions.SubscriberConversationTui SubscriberConversationTui { get; set; }
            public Cisco.UnityConnection.RestFunctions.TransferOptionTypes TransferOptionType { get; set; }
            public Cisco.UnityConnection.RestFunctions.GreetingTypes GreetingType { get; set; }
            public Cisco.UnityConnection.RestFunctions.ConversationNames ConversationName { get; set; }
            public Cisco.UnityConnection.RestFunctions.MessageType MessageType { get; set; }
            public Cisco.UnityConnection.RestFunctions.SensitivityType SensitivityType { get; set; }
            public Cisco.UnityConnection.RestFunctions.PriorityType PriorityType { get; set; }
            public Cisco.UnityConnection.RestFunctions.SipTransportEnum Bogus { get; set; }
        }

        [TestMethod]
        public void SafeXmlFetchTests()
        {
            TestClass oTestClass = new TestClass();
            XElement oElement = new XElement("test");
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<URI>1234</URI>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<Int16>0</Int16>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<Int32>1</Int32>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<Int64>2</Int64>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<String>3</String>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<Boolean>false</Boolean>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<DateTime>1/2/2013</DateTime>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<SubscriberConversationTui>SubMenuOpt1</SubscriberConversationTui>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<ConversationName>SubSignIn</ConversationName>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<TransferOptionType>Alternate</TransferOptionType>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<GreetingType>Busy</GreetingType>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<MessageType>2</MessageType>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<SensitivityType>0</SensitivityType>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<PriorityType>0</PriorityType>"));
            _connectionServer.SafeXmlFetch(oTestClass, XElement.Parse("<Bogus>0</Bogus>"));

            Assert.IsTrue(oTestClass.Int32==1,"Int32 did not equal 1");
            Assert.IsTrue(oTestClass.Int16 == 0, "Int16 did not equal 0");
            Assert.IsTrue(oTestClass.Int64 == 2, "Int64 did not equal 2");
            Assert.IsTrue(oTestClass.String.Equals("3"), "String did not equal 3");
            Assert.IsTrue(oTestClass.Boolean==false, "Boolean did not equal false");
            Assert.IsTrue(oTestClass.SubscriberConversationTui == SubscriberConversationTui.SubMenuOpt1,"Subscriberconversation was not correct");
            Assert.IsTrue(oTestClass.ConversationName == ConversationNames.SubSignIn , "ConversationName was not correct");
            Assert.IsTrue(oTestClass.TransferOptionType == TransferOptionTypes.Alternate,"TransferOptionType was not correct");
            Assert.IsTrue(oTestClass.GreetingType == GreetingTypes.Busy, "GreetingType was not correct");
            Assert.IsTrue(oTestClass.MessageType == MessageType.Voice, "MessageType was not correct");
        }

        [TestMethod]
        public void GetActionDescripiton_TopLevel()
        {
            string strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.SystemTransfer,
                                                                   "");
            Assert.IsTrue(strRet.Equals("Route to system transfer", StringComparison.CurrentCultureIgnoreCase),
                          "Descripiton incorrect for system transfer:" + strRet);

            strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.Invalid, "");
            Assert.IsTrue(strRet.ToLower().Contains("(error)"),"Invalid conversation name did not return error:"+strRet);

            strRet = _connectionServer.GetActionDescription(ActionTypes.Invalid, ConversationNames.SubSignIn, "");
            Assert.IsTrue(strRet.ToLower().Contains("(error)"), "Invalid action did not return error:" + strRet);

        }

        [TestMethod]
        public void GetActionDescripiton_DirectoryHandlers()
        {
            List<DirectoryHandler> oDirHandlers;
            var res = DirectoryHandler.GetDirectoryHandlers(_connectionServer, out oDirHandlers, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch directory handlers:" + res);
            Assert.IsTrue(oDirHandlers.Count > 0, "Failed to find any directory handlers");

            string strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.Ad,
                                                            oDirHandlers[0].ObjectId);
            Assert.IsTrue(strRet.ToLower().Contains("route to name lookup handler:"),
                          "Directory handler route description not correct:" + strRet);

            strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.Ad, "bogus");
            Assert.IsTrue(strRet.ToLower().Contains("invalid link"),
                          "Directory handler route description not correct for missing link:"
                          + strRet);
        }

        [TestMethod]
        public void GetActionDescripiton_CallHandlers()
        {
            List<CallHandler> oHandlers;
            var res = CallHandler.GetCallHandlers(_connectionServer, out oHandlers, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch call handlers:" + res);
            Assert.IsTrue(oHandlers.Count > 0, "Failed to find any call handlers");

            string strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHTransfer, oHandlers[0].ObjectId);
            Assert.IsTrue(strRet.ToLower().Contains("ring phone for "), "Call handler route description not correct");

            strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHTransfer, "bogus");
            Assert.IsTrue(strRet.ToLower().Contains("invalid link"), "Call handler route description not correct for missing link:"+strRet);

            strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHGreeting, oHandlers[0].ObjectId);
            Assert.IsTrue(strRet.ToLower().Contains("send to greeting for "), "Call handler route description not correct" + strRet);

            strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHGreeting, "bogus");
            Assert.IsTrue(strRet.ToLower().Contains("invalid link"), "Call handler route description not correct for missing link");

        }

        [TestMethod]
        public void GetActionDescripiton_Subscribers()
        {
            List<UserBase> oUsers;
            var res = UserBase.GetUsers(_connectionServer, out oUsers, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch users" + res);
            Assert.IsTrue(oUsers.Count > 0, "Failed to find any users");

            string strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHTransfer, 
                oUsers[0].PrimaryCallHandler().ObjectId);
            Assert.IsTrue(strRet.ToLower().Contains("ring phone for "), "User route description not correct");

            strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHGreeting, 
                oUsers[0].PrimaryCallHandler().ObjectId);
            Assert.IsTrue(strRet.ToLower().Contains("send to greeting for "), "User route description not correct" + strRet);

        }

        [TestMethod]
        public void GetActionDescripiton_InterviewHandlers()
        {
            List<InterviewHandler> oInterviewHandlers;
            var res = InterviewHandler.GetInterviewHandlers(_connectionServer, out oInterviewHandlers, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch interview handlers:" + res);
            Assert.IsTrue(oInterviewHandlers.Count > 0, "Failed to find any interview handlers");

            string strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHInterview,
                                                            oInterviewHandlers[0].ObjectId);
            Assert.IsTrue(strRet.ToLower().Contains("send to interview handler:"),
                          "Interview handler route description not correct:" + strRet);

            strRet = _connectionServer.GetActionDescription(ActionTypes.GoTo, ConversationNames.PHInterview, "bogus");
            Assert.IsTrue(strRet.ToLower().Contains("invalid link"),
                          "Interview handler route description not correct for missing link:"+ strRet);
        }


        [TestMethod]
        public void ValidateUserTest()
        {
            UserBase oUser;
            Assert.IsFalse(_connectionServer.ValidateUser("bogus", "bogus", out oUser), "Validating invalid user did not fail");

            Assert.IsFalse(_connectionServer.ValidateUser("", "bogus"), "Validating user with blank ID did not fail");

            Assert.IsFalse(_connectionServer.ValidateUser("bogus", "", out oUser), "Validating invalid user with blank PW did not fail");

        }

        [TestMethod]
        public void GetCucaUrlForObjectTests()
        {

            string strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.CallHandlerTemplate, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for CallHandlerTemplate");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.Cos, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for Cos");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.DistributionList, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for DistributionList");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.DistributionList, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for DistributionList");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.GlobalUser, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for GlobalUser");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.Handler, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for CallHandler");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.InterviewHandler, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for InterviewHandler");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.Location, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for Location");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.NameLookupHandler, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for NameLookupHandler");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.NoObjectType, "objectid");
            Assert.IsTrue(string.IsNullOrEmpty(strRet), "Invalid object type did not return empty string");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.Partition, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for Partition");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.RestrictionTable, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for RestrictionTable");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.Role, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for Role");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.RoutingRuleDirect, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for RoutingRuleDirect");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.RoutingRuleForwarded, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for RoutingRuleForwarded");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.ScheduleSet, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for ScheduleSet");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.SearchSpace, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for SearchSpace");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.SmppProvider, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for SmppProvider");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.Subscriber, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for Subscriber");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.SubscriberTemplate, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for SubscriberTemplate");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.Switch, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for Switch");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.SystemCallHandler, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for SystemCallHandler");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.PersonalCallTransferRule, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for PersonalCallTransferRule");

            strRet = _connectionServer.GetCucaUrlForObject(ConnectionObjectType.SystemContact, "objectid");
            Assert.IsFalse(string.IsNullOrEmpty(strRet), "Failed to get URL for SystemContact");


        }
    }


}
