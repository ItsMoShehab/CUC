using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ServerResponseJsonParsingTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties
        
        //used for editing/adding items to a temporary user that gets cleaned up after the tests are complete
        private static UserFull _tempUser;
        private static Contact _tempContact;
        private static InterviewHandler _tempInterviewer;
        private static PostGreetingRecording _tempRecording;
       
        private static string _errorString;

        #endregion


        #region Additional test attributes

        private static void ServerOnErrorEvents(object sender, ConnectionServerRest.LogEventArgs logEventArgs)
        {
            Console.WriteLine(logEventArgs.Line);
            _errorString += logEventArgs.Line;
        }

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            _connectionServer.ErrorEvents += ServerOnErrorEvents;

            //create new list with GUID in the name to ensure uniqueness
            String strAlias = "TempUserServerResp_" + Guid.NewGuid().ToString().Replace("-", "");

            //generate a random number and tack it onto the end of some zeros so we're sure to avoid any legit numbers on the system.
            Random random = new Random();
            int iExtPostfix = random.Next(100000, 999999);
            string strExtension = "000000" + iExtPostfix.ToString();

            //use a bogus extension number that's legal but non dialable to avoid conflicts
            var res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", strAlias, strExtension, null, out _tempUser);
            Assert.IsTrue(res.Success, "Failed creating temporary user:" + res.ToString());

            strAlias = "TempContactJsn_" + Guid.NewGuid().ToString().Replace("-", "");
            res = Contact.AddContact(_connectionServer, "systemcontacttemplate", strAlias, "", "", strAlias, null, out _tempContact);
            Assert.IsTrue(res.Success, "Failed creating temporary contact:" + res.ToString());

            strAlias = "TempInterviewer_" + Guid.NewGuid().ToString().Replace("-", "");
            res = InterviewHandler.AddInterviewHandler(_connectionServer, strAlias, _tempUser.ObjectId, "", null,out _tempInterviewer);
            Assert.IsTrue(res.Success, "Failed creating temporary interviewer:" + res.ToString());

            strAlias = "TempRecording_" + Guid.NewGuid().ToString().Replace("-", "");
            res = PostGreetingRecording.AddPostGreetingRecording(_connectionServer, strAlias, out _tempRecording);
            Assert.IsTrue(res.Success, "Failed creating temporary post greeting recording:" + res.ToString());

        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempInterviewer != null)
            {
                var res = _tempInterviewer.Delete();
                Assert.IsTrue(res.Success,"Failed to delete temporary interview handler on cleanup");
            }
            if (_tempUser != null)
            {
                WebCallResult res = _tempUser.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary user on cleanup.");
            }

            if (_tempContact != null)
            {
                var res = _tempContact.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary contact on cleanup.");
            }

            if (_tempRecording != null)
            {
                var res = _tempRecording.Delete();
                Assert.IsTrue(res.Success,"Failed to delete temporary post greeting recording on cleanup");
            }

        }

        #endregion

        [TestMethod]
        public void AlternateExtension_Test()
        {
            var res = AlternateExtension.AddAlternateExtension(_connectionServer, _tempUser.ObjectId, 1,
                                                               Guid.NewGuid().ToString().Replace("-", ""));
            Assert.IsTrue(res.Success,"Failed to create alternate extension for testing");

            _errorString = "";
            List<AlternateExtension> oAlternateExtensions = _tempUser.AlternateExtensions(true);
            if (oAlternateExtensions == null || oAlternateExtensions.Count == 0)
            Assert.Fail("Failed to fetch alterante extensions:");
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for CallHandler:" + _errorString);
        }

        [TestMethod]
        public void CallHandlers_Test()
        {
            _errorString = "";
            List<CallHandler> oCallHandlers;
            var res = CallHandler.GetCallHandlers(_connectionServer, out oCallHandlers, 1, 2);
            Assert.IsTrue(res.Success,"Failed to fetch call handlers:"+res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString),"Error parsing Json for CallHandler:"+_errorString);
        }

        [TestMethod]
        public void CallHandlerTemplates_Test()
        {
            _errorString = "";
            List<CallHandlerTemplate> oCallHandlerTemplates;
            var res = CallHandlerTemplate.GetCallHandlerTemplates(_connectionServer, out oCallHandlerTemplates, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch call handler templates:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for CallHandlerTemplate:" + _errorString);
        }

        [TestMethod]
        public void Cos_Test()
        {
            _errorString = "";
            List<ClassOfService> oClassesOfService;
            var res = ClassOfService.GetClassesOfService(_connectionServer, out oClassesOfService);
            Assert.IsTrue(res.Success, "Failed to fetch COSes:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for COSes:" + _errorString);
        }

        [TestMethod]
        public void Cluster_Test()
        {
            _errorString = "";

            try
            {
                Cluster oCluster = new Cluster(_connectionServer);
            }
            catch (Exception ex)
            {
                Assert.Fail("[ERROR] fetching cluster:" + ex);
                return;
            }
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for Cluster:" + _errorString);
        }

        [TestMethod]
        public void ConfigurationValue_Test()
        {
            _errorString = "";
            List<ConfigurationValue> oValues;
            var res = ConfigurationValue.GetConfigurationValues(_connectionServer, out oValues, 1, 10);
            Assert.IsTrue(res.Success, "Failed to fetch configuration value:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for configuration value:" + _errorString);
        }

        [TestMethod]
        public void Contact_Test()
        {
            _errorString = "";
            List<Contact> oContacts;
            var res = Contact.GetContacts(_connectionServer, out oContacts, 1, 5);
            Assert.IsTrue(res.Success & oContacts.Count > 0, "Failed to fetch contacts:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for contact:" + _errorString);
        }

        [TestMethod]
        public void Credential_Password_Test()
        {
            _errorString = "";
            Credential oCredential;
            var res = Credential.GetCredential(_connectionServer, _tempUser.ObjectId, CredentialType.Password,out oCredential);
            Assert.IsTrue(res.Success, "Failed to fetch password credentials:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for password credential:" + _errorString);
        }

        [TestMethod]
        public void Credential_Pin_Test()
        {
            _errorString = "";
            Credential oCredential;
            var res = Credential.GetCredential(_connectionServer, _tempUser.ObjectId, CredentialType.Pin, out oCredential);
            Assert.IsTrue(res.Success, "Failed to fetch pin credentials:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for pin credential:" + _errorString);
        }

        [TestMethod]
        public void DirectoryHandler_Test()
        {
            _errorString = "";
            List<DirectoryHandler> oDirectoryHandlers;
            var res = DirectoryHandler.GetDirectoryHandlers(_connectionServer, out oDirectoryHandlers, 1, 2);
            Assert.IsTrue(res.Success & oDirectoryHandlers.Count > 0, "Failed to fetch directory handlers:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for directory handlers:" + _errorString);
        }

        [TestMethod]
        public void DistributionList_Test()
        {
            _errorString = "";
            List<DistributionList> oDistributionLists;
            var res = DistributionList.GetDistributionLists(_connectionServer, out oDistributionLists, 1, 5);
            Assert.IsTrue(res.Success, "Failed to fetch distribution lists:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for distribution lists:" + _errorString);

            //Distribution List Member
            foreach (var oList in oDistributionLists)
            {
                List<DistributionListMember> oMemberList;
                res = oList.GetMembersList(out oMemberList);
                Assert.IsTrue(res.Success, "Failed to fetch distribution list members:" + res);
                if (oMemberList.Count > 0)
                {
                    break;
                }
            }
            
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for distribution list members:" + _errorString);
        }


        [TestMethod]
        public void GlobalUser_Test()
        {
            _errorString = "";
            List<GlobalUser> oGlobalUsers;
            var res = GlobalUser.GetUsers(_connectionServer, out oGlobalUsers, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch global users:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for global users:" + _errorString);
        }


        [TestMethod]
        public void Greeting_Test()
        {
            _errorString = "";
            List<Greeting> oGreetings = _tempUser.PrimaryCallHandler().GetGreetings();
            Assert.IsFalse(oGreetings == null || oGreetings.Count == 0, "Failed to fetch greetings:");
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for greetings:" + _errorString);
        }


        [TestMethod]
        public void InstalledLanguage_Test()
        {
            _errorString = "";
            try
            {
                InstalledLanguage oLanguage = new InstalledLanguage(_connectionServer);
            }
            catch (Exception ex)
            {
                Assert.Fail("[ERROR] fetching installed languages:" + ex);
            }

            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for installed languages:" + _errorString);
        }

        [TestMethod]
        public void Interviewer_Test()
        {
            _errorString = "";
            List<InterviewHandler> oInterviewHandlers;
            var res = InterviewHandler.GetInterviewHandlers(_connectionServer, out oInterviewHandlers, 1, 2);
            Assert.IsTrue(res.Success & oInterviewHandlers.Count > 0, "Failed to fetch interview handlers:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for interview handlers:" + _errorString);

            List<InterviewQuestion> oQuestions;
            res = InterviewQuestion.GetInterviewQuestions(_connectionServer, oInterviewHandlers[0].ObjectId,out oQuestions);
            Assert.IsFalse(res.Success == false || oQuestions.Count == 0, "Failed to fetch interviewer questions");
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for interview handler questions:" + _errorString);
        }

        [TestMethod]
        public void Location_Test()
        {
            _errorString = "";
            List<Location> oLocations;
            var res = Location.GetLocations(_connectionServer, out oLocations, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch locations:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for locations:" + _errorString);
        }

        [TestMethod]
        public void MainboxInfo_Test()
        {
            _errorString = "";
            try
            {
                MailboxInfo oMailboxInfo = new MailboxInfo(_connectionServer, _tempUser.ObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("[ERROR] fetching MailboxInfo:" + ex);
            }

            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for mailbox info:" + _errorString);
        }

        [TestMethod]
        public void MenuEntry_Test()
        {
            _errorString = "";
            List<MenuEntry> oMenuEntries;
            var res = MenuEntry.GetMenuEntries(_connectionServer, _tempUser.PrimaryCallHandler().ObjectId, out oMenuEntries);
            Assert.IsTrue(res.Success, "Failed to fetch menu entries:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for menu entries:" + _errorString);
        }

        [TestMethod]
        public void Mwi_Test()
        {
            _errorString = "";
            List<Mwi> oMwis;
            var res = Mwi.GetMwiDevices(_connectionServer, _tempUser.ObjectId, out oMwis);
            Assert.IsTrue(res.Success & oMwis.Count > 0, "Failed to fetch mwis:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), "Error parsing Json for mwis:" + _errorString);
        }


        [TestMethod]
        public void NotificationDevice_Test()
        {
            _errorString = "";
            List<NotificationDevice> oNotificationDevices;
            var res = NotificationDevice.GetNotificationDevices(_connectionServer, _tempUser.ObjectId,out oNotificationDevices);
            Assert.IsTrue(res.Success, "Failed to fetch notificationdevice:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void NotificationTemplate_Test()
        {
            _errorString = "";
            List<NotificationTemplate> oNotificationTemplates;
            var res = NotificationTemplate.GetNotificationTemplates(_connectionServer, out oNotificationTemplates, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch notificationtemplate:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void Partition_Test()
        {
            _errorString = "";
            List<Partition> oPartitions;
            var res = Partition.GetPartitions(_connectionServer, out oPartitions, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch partitions:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void PhoneSystem_Test()
        {
            _errorString = "";
            List<PhoneSystem> oPhoneSystems;
            var res = PhoneSystem.GetPhoneSystems(_connectionServer, out oPhoneSystems, 1, 2);
            Assert.IsTrue(res.Success & oPhoneSystems.Count > 0, "Failed to fetch phone systems:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void Policies_Test()
        {
            _errorString = "";
            List<Policy> oPolicies;
            var res = Policy.GetPolicies(_connectionServer, out oPolicies, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch policy:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void Port_Test()
        {
            _errorString = "";
            List<Port> oPorts;
            var res = Port.GetPorts(_connectionServer, out oPorts, 1, 2);
            Assert.IsTrue(res.Success & oPorts.Count > 0, "Failed to fetch ports:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void PortGroup_Test()
        {
            _errorString = "";
            List<PortGroup> oPortGroups;
            var res = PortGroup.GetPortGroups(_connectionServer, out oPortGroups, 1, 2);
            Assert.IsTrue(res.Success & oPortGroups.Count > 0, "Failed to fetch port groups:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);

            //PortGroupCodec
            List<PortGroupCodec> oPortGroupCodecs;
            res = PortGroupCodec.GetPortGroupCodecs(_connectionServer, oPortGroups[0].ObjectId, out oPortGroupCodecs);
            Assert.IsTrue(res.Success & oPortGroupCodecs.Count > 0, "Failed to fetch port group codecs:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);

            //PortGroupServer
            List<PortGroupServer> oPortGroupServers;
            res = PortGroupServer.GetPortGroupServers(_connectionServer, oPortGroups[0].ObjectId, out oPortGroupServers);
            Assert.IsTrue(res.Success & oPortGroupServers.Count > 0, "Failed to fetch port group server:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void PortGroupTemplate_Test()
        {
            _errorString = "";
            List<PortGroupTemplate> oPortGroupTemplates;
            var res = PortGroupTemplate.GetPortGroupTemplates(_connectionServer, out oPortGroupTemplates);
            Assert.IsTrue(res.Success & oPortGroupTemplates.Count>0, "Failed to fetch portgrouptemplates:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void PostGreetingRecording_Test()
        {
            _errorString = "";
            List<PostGreetingRecording> oPostGreetingRecordings;
            var res = PostGreetingRecording.GetPostGreetingRecordings(_connectionServer, out oPostGreetingRecordings);
            Assert.IsTrue(res.Success & oPostGreetingRecordings.Count>0, "Failed to fetch postgreetingrecordings:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void PrivateList_Test()
        {
            PrivateList oList;
            var res= PrivateList.AddPrivateList(_connectionServer, _tempUser.ObjectId, "Test list 1", 1,out oList);
            Assert.IsTrue(res.Success,"Failed to create private list for user:"+res);

            res= oList.AddMemberUser(_tempUser.ObjectId);
            Assert.IsTrue(res.Success,"Failed to add member to private list");

            _errorString = "";
            List<PrivateList> oPrivateLists;
            res = PrivateList.GetPrivateLists(_connectionServer, _tempUser.ObjectId, out oPrivateLists, 1, 2);
            Assert.IsTrue(res.Success & oPrivateLists.Count > 0, "Failed to fetch private lists:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);

            //private list member
            foreach (var oPrivateList in oPrivateLists)
            {
                List<PrivateListMember> oPrivateListMembers;
                res = PrivateList.GetMembersList(_connectionServer, oPrivateList.ObjectId, _tempUser.ObjectId,
                                                 out oPrivateListMembers);
                Assert.IsTrue(res.Success & oPrivateLists.Count > 0, "Failed to fetch private list members:" + res);
                if (oPrivateListMembers.Count > 0)
                {
                    break;
                }
            }

            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void RestrictionTable_Test()
        {
            _errorString = "";
            List<RestrictionTable> oRestrictionTables;
            var res = RestrictionTable.GetRestrictionTables(_connectionServer, out oRestrictionTables, 1, 2);
            Assert.IsTrue(res.Success & oRestrictionTables.Count > 0, "Failed to fetch restrictiontable:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);

            //Restriction table pattern
            List<RestrictionPattern> oRestrictionPatterns;
            res = RestrictionPattern.GetRestrictionPatterns(_connectionServer, oRestrictionTables[0].ObjectId,
                                                            out oRestrictionPatterns);
            Assert.IsTrue(res.Success & oRestrictionTables.Count > 0, "Failed to fetch restrictiontablepattern:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void Role_Test()
        {
            _errorString = "";
            List<Role> oRoles;
            var res = Role.GetRoles(_connectionServer, out oRoles);
            Assert.IsTrue(res.Success, "Failed to fetch role:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void RoutingRule_Test()
        {
            _errorString = "";
            List<RoutingRule> oRoutingRules;
            var res = RoutingRule.GetRoutingRules(_connectionServer, out oRoutingRules, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch routingrule:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void RtpCodecDef_Test()
        {
            _errorString = "";
            List<RtpCodecDef> oRtpCodecDefs;
            var res = RtpCodecDef.GetRtpCodecDefs(_connectionServer, out oRtpCodecDefs);
            Assert.IsTrue(res.Success & oRtpCodecDefs.Count>0, "Failed to fetch RTPCodecDef:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void Schedule_Test()
        {
            _errorString = "";
            List<Schedule> oSchedules;
            var res = Schedule.GetSchedules(_connectionServer, out oSchedules, 1, 2);
            Assert.IsTrue(res.Success & oSchedules.Count > 0, "Failed to fetch Schedule:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);

            //ScheduleDetail
            foreach (var oSchedule in oSchedules)
            {
                List<ScheduleDetail> oScheduleDetails;
                res = oSchedule.GetScheduleDetails(out oScheduleDetails);
                Assert.IsTrue(res.Success & oSchedules.Count > 0, "Failed to fetch Schedule:" + res);
                if (oScheduleDetails.Count > 0)
                {
                    break;
                }
            }

            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void ScheduleSet_Test()
        {
            _errorString = "";
            List<ScheduleSet> oScheduleSets;
            var res = ScheduleSet.GetSchedulesSets(_connectionServer, out oScheduleSets, 1, 2);
            Assert.IsTrue(res.Success & oScheduleSets.Count > 0, "Failed to fetch scheduleset:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);

            //ScheduleSetMember
            List<ScheduleSetMember> oScheduleSetMembers;
            res = ScheduleSet.GetSchedulesSetsMembers(_connectionServer, oScheduleSets[0].ObjectId,
                                                      out oScheduleSetMembers);
            Assert.IsTrue(res.Success & oScheduleSetMembers.Count > 0, "Failed to fetch schedulesetmembers:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void SearchSpace_Test()
        {
            _errorString = "";
            List<SearchSpace> oSearchSpaces;
            var res = SearchSpace.GetSearchSpaces(_connectionServer, out oSearchSpaces, 1, 2);
            Assert.IsTrue(res.Success & oSearchSpaces.Count > 0, "Failed to fetch SearchSpace:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void SmppProvider_Test()
        {
            _errorString = "";
            List<SmppProvider> oSmppProviders;
            var res = SmppProvider.GetSmppProviders(_connectionServer, out oSmppProviders, 1, 2);
            Assert.IsTrue(res.Success & oSmppProviders.Count > 0, "Failed to fetch SMPPPRovider:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void TimeZone_Test()
        {
            _errorString = "";
            try
            {
                TimeZones oTimeZones = new TimeZones(_connectionServer);
            }
            catch (Exception ex)
            {
                Assert.Fail("[ERROR] fetching time zones:" + ex);
            }
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void TransferOptions_Test()
        {
            _errorString = "";
            List<TransferOption> oTransferOptions;
            var res = TransferOption.GetTransferOptions(_connectionServer, _tempUser.PrimaryCallHandler().ObjectId,
                                                    out oTransferOptions);
            Assert.IsTrue(res.Success & oTransferOptions.Count > 0, "Failed to fetch TransferOptions:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void UserBase_Test()
        {
            _errorString = "";
            List<UserBase> oUserBases;
            var res = UserBase.GetUsers(_connectionServer, out oUserBases, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch UserBase:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void UserMessage_Test()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.TO;
            oRecipient.SmtpAddress = _tempUser.SmtpAddress;
            var res =UserMessage.CreateMessageLocalWav(_connectionServer, _tempUser.ObjectId, "test", "Dummy.wav", false,
                                              SensitivityType.Normal,
                                              false, false, false, false, null, true, oRecipient);
            Assert.IsTrue(res.Success,"Failed leaving test message");
            Thread.Sleep(1000);

            _errorString = "";
            List<UserMessage> oMessages;
            res = UserMessage.GetMessages(_connectionServer, _tempUser.ObjectId, out oMessages, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch UserBase:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void UserTemplate_Test()
        {
            _errorString = "";
            List<UserTemplate> oUserTemplates;
            var res = UserTemplate.GetUserTemplates(_connectionServer, out oUserTemplates, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch UserTemplate:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }

        [TestMethod]
        public void VmsServer_Test()
        {
            _errorString = "";
            List<VmsServer> oVmsServers;
            var res = VmsServer.GetVmsServers(_connectionServer, out oVmsServers);
            Assert.IsTrue(res.Success, "Failed to fetch VMSServer:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }
    }
}
