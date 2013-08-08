using System.Collections.Generic;
using System.Xml.Linq;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for ConnectionServerIntegrationTests and is intended
    ///to contain all ConnectionServerIntegrationTests Unit Tests
    ///</summary>
    [TestClass]
    public class ConnectionServerIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static InterviewHandler _tempInterviewer;

        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            List<UserBase> tempUser;
            UserBase.GetUsers(_connectionServer, out tempUser, 1, 1);
            
            string strAlias = "TempInterviewer_" + Guid.NewGuid().ToString().Replace("-", "");
            var res = InterviewHandler.AddInterviewHandler(_connectionServer, strAlias, tempUser[0].ObjectId, "", null, out _tempInterviewer);
            Assert.IsTrue(res.Success, "Failed creating temporary interviewer:" + res.ToString());
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempInterviewer != null)
            {
                var res = _tempInterviewer.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary interview handler on cleanup");
            }
        }

        #endregion


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
