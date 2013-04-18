using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimpleLogger;
using Cisco.UnityConnection.RestFunctions;
using Newtonsoft.Json.Serialization;


namespace LoadEachObjectTypeTest
{
    /// <summary>
    /// Test project - requires a user be created that has messages, alternate extensions, private lists, MWIs, 
    /// notification devices to work completely.
    /// Loads every object type supported in the SDK and checks if any data is coming from Connection that does 
    /// not have a property in the corresponding class object for that data.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Connection server to run tests against
        /// </summary>
        private static ConnectionServer _server;

        /// <summary>
        /// User that has private lists, mwi, notification device(s) etc... configured to use for test
        /// </summary>
        private static string _userAliasToUse;

        /// <summary>
        /// Simple method to run through and load up objects of every type supported in the SDK.  This checks specifically
        /// for items missing in the class defnitions that have data coming over from REST.  The details are logged out 
        /// to a file for review.
        /// </summary>
        static void Main()
        {
            HTTPFunctions.JsonSerializerSettings.Error += JsonSerializerError;

            HTTPFunctions.ErrorEvents += HttpFunctionsOnErrorEvents;

            try
            {
                _server = new ConnectionServer("192.168.0.184", "CCMAdministrator", "ecsbulab");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to log into Connection server:"+ex);
                Console.WriteLine("Press enter to exit");
                Console.WriteLine();
            }

            //test user that has private lists, notification devices, mwis, alternate extensions and messages
            _userAliasToUse = "jlindborg";

            WriteOutput("Starting Object load test on:" + _server);

            RunTests();

            Logger.StopLogging();
            Process.Start(Logger.GetCurrentLogFilePath());

            Console.WriteLine("Test compelte.  Press enter to exit");
            Console.ReadLine();
        }


        private static void HttpFunctionsOnErrorEvents(object sender, HTTPFunctions.LogEventArgs logEventArgs)
        {
            Console.WriteLine(logEventArgs.Line);
        }


        private static void RunTests()
        {
            // keep these in rough alphabetical order

            //grab the user to test with first - it gets used by many classes that need it
            UserFull oTestUser;
            WebCallResult res = UserBase.GetUser(out oTestUser, _server, "", _userAliasToUse);
            if (res.Success == false || oTestUser == null)
            {
                WriteOutput("[ERROR] test user not found on target server with alias="+_userAliasToUse);
                return;
            }

            //Alternate Extensions
            List<AlternateExtension> oAlternateExtensions = oTestUser.AlternateExtensions(true);
            if (oAlternateExtensions == null || oAlternateExtensions.Count == 0)
            {
                WriteOutput("[ERROR] fetching alternate extensions off test user:" + res);
                return;
            }

            //call handlers
            List<CallHandler> oCallHandlers;
            res = CallHandler.GetCallHandlers(_server, out oCallHandlers, 1, 2);
            if (res.Success == false || oCallHandlers.Count==0)
            {
                WriteOutput("[ERROR] fetching call handlers:" + res);
                return;
            }
            
            //call handler templates
            List<CallHandlerTemplate> oCallHandlerTemplates;
            res = CallHandlerTemplate.GetCallHandlerTemplates(_server, out oCallHandlerTemplates, 1, 2);
            if (res.Success == false || oCallHandlerTemplates.Count==0)
            {
                WriteOutput("[ERROR] fetching call handler templates:" + res);
                return;
            }

            //Class of Service
            List<ClassOfService> oClassesOfService;
            res = ClassOfService.GetClassesOfService(_server, out oClassesOfService);
            if (res.Success == false || oClassesOfService.Count == 0)
            {
                WriteOutput("[ERROR] fetching COSes:" + res);
                return;
            }

            //Cluster
            try
            {
                Cluster oCluster = new Cluster(_server);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] fetching cluster:"+ex);
                return;
            }

            //ConfigurationValue
            List<ConfigurationValue> oValues;
            res = ConfigurationValue.GetConfigurationValues(_server, out oValues, 1, 10);
            if (res.Success == false || oValues.Count == 0)
            {
                WriteOutput("[ERROR] fetching Configuration values:" + res);
                return;
            }


            //Contact
            List<Contact> oContacts;
            res = Contact.GetContacts(_server, out oContacts, 1, 5);
            if (res.Success == false || oContacts.Count == 0)
            {
                WriteOutput("[ERROR] fetching contacts:" + res);
                return;
            }

            //Credential
            Credential oCredential;
            res = Credential.GetCredential(_server, oTestUser.ObjectId, CredentialType.Password, out oCredential);
            if (res.Success == false)
            {
                WriteOutput("[ERROR] fetching password credential:" + res);
                return;
            }

            res = Credential.GetCredential(_server, oTestUser.ObjectId, CredentialType.Pin, out oCredential);
            if (res.Success == false)
            {
                WriteOutput("[ERROR] fetching pin credential:" + res);
                return;
            }

            //Directory Handler
            List<DirectoryHandler> oDirectoryHandlers;
            res = DirectoryHandler.GetDirectoryHandlers(_server, out oDirectoryHandlers, 1, 2);
            if (res.Success == false || oDirectoryHandlers.Count == 0)
            {
                WriteOutput("[ERROR] fetching directory handlers:" + res);
                return;
            }


            //Distribution List
            List<DistributionList> oDistributionLists;
            res = DistributionList.GetDistributionLists(_server, out oDistributionLists, 1, 5);
            if (res.Success == false || oDistributionLists.Count == 0)
            {
                WriteOutput("[ERROR] fetching distribution lists:" + res);
                return;
            }

            //Distribution List Member
            foreach (var oList in oDistributionLists)
            {
                List<DistributionListMember> oMemberList;
                res = oList.GetMembersList(out oMemberList);
                if (res.Success == false)
                {
                    WriteOutput("[ERROR] fetching distribution list members:" + res);
                    return;
                }
                if (oMemberList.Count > 0)
                {
                    break;
                }
            }

            //GlobalUser
            List<GlobalUser> oGlobalUsers;
            res = GlobalUser.GetUsers(_server, out oGlobalUsers, 1, 2);
            if (res.Success == false || oGlobalUsers.Count == 0)
            {
                WriteOutput("[ERROR] fetching global users:" + res);
                return;
            }

            //Greeting
            List<Greeting> oGreetings = oTestUser.PrimaryCallHandler().GetGreetings();
            if (oGreetings == null || oGreetings.Count == 0)
            {
                WriteOutput("[ERROR] fetching greetings");
                return;
            }

            //InstalledLanguage
             try
             {
                 InstalledLanguage oLanguage = new InstalledLanguage(_server);
             }
             catch (Exception ex)
             {
                 WriteOutput("[ERROR] fetching installed languages:"+ex);
                 return;
             }

            //InterviewHandler
            List<InterviewHandler> oInterviewHandlers;
            res = InterviewHandler.GetInterviewHandlers(_server, out oInterviewHandlers, 1, 2);
            if (res.Success == false || oInterviewHandlers.Count == 0)
            {
                WriteOutput("[ERROR] fetching interview handlers:" + res);
                return;
            }

            List<InterviewQuestion> oQuestions;
            res = InterviewQuestion.GetInterviewQuestions(_server, oInterviewHandlers[0].ObjectId, out oQuestions);
            if (res.Success == false || oQuestions.Count == 0)
            {
                WriteOutput("[ERROR] fetching interview handler questions:" + res);
                return;
            }


            //Location
            List<Location> oLocations;
            res = Location.GetLocations(_server, out oLocations, 1, 2);
            if (res.Success == false || oLocations.Count == 0)
            {
                WriteOutput("[ERROR] fetching locations:" + res);
                return;
            }


            //MailboxInfo
            try
            {
                MailboxInfo oMailboxInfo = new MailboxInfo(_server,oTestUser.ObjectId);
            }
            catch (Exception ex)
            {
                WriteOutput("[ERROR] fetching MailboxInfo:" + ex);
                return;
            }

            //MenuEntry
            List<MenuEntry> oMenuEntries;
            res = MenuEntry.GetMenuEntries(_server, oTestUser.PrimaryCallHandler().ObjectId, out oMenuEntries);
            if (res.Success == false || oMenuEntries.Count == 0)
            {
                WriteOutput("[ERROR] fetching menu entries:" + res);
                return;
            }

            //MWI
            List<Mwi> oMwis;
            res = Mwi.GetMwiDevices(_server, oTestUser.ObjectId, out oMwis);
            if (res.Success == false || oMwis.Count == 0)
            {
                WriteOutput("[ERROR] fetching MWIs:" + res);
                return;
            }

            //NotificationDevice
            List<NotificationDevice> oNotificationDevices;
            res = NotificationDevice.GetNotificationDevices(_server, oTestUser.ObjectId, out oNotificationDevices);
            if (res.Success == false || oNotificationDevices.Count == 0)
            {
                WriteOutput("[ERROR] fetching notification devices:" + res);
                return;
            }


            //NotificationTemplate
            List<NotificationTemplate> oNotificationTemplates;
            res = NotificationTemplate.GetNotificationTemplates(_server, out oNotificationTemplates, 1, 2);
            if (res.Success == false || oNotificationTemplates.Count == 0)
            {
                WriteOutput("[ERROR] fetching notification templates:" + res);
                return;
            }


            //Partition
            List<Partition> oPartitions;
            res = Partition.GetPartitions(_server, out oPartitions, 1, 2);
            if (res.Success == false || oPartitions.Count == 0)
            {
                WriteOutput("[ERROR] fetching partitions:" + res);
                return;
            }

            //PhoneSystem
            List<PhoneSystem> oPhoneSystems;
            res = PhoneSystem.GetPhoneSystems(_server, out oPhoneSystems, 1, 2);
            if (res.Success == false || oPhoneSystems.Count == 0)
            {
                WriteOutput("[ERROR] fetching phone systems:" + res);
                return;
            }

            //Policy
            List<Policy> oPolicies;
            res = Policy.GetPolicies(_server, out oPolicies, 1, 2);
            if (res.Success == false || oPolicies.Count == 0)
            {
                WriteOutput("[ERROR] fetching policies:" + res);
                return;
            }

            //Port
            List<Port> oPorts;
            res = Port.GetPorts(_server, out oPorts, 1, 2);
            if (res.Success == false || oPorts.Count == 0)
            {
                WriteOutput("[ERROR] fetching ports:" + res);
                return;
            }

            //PortGroup
            List<PortGroup> oPortGroups;
            res = PortGroup.GetPortGroups(_server, out oPortGroups, 1, 2);
            if (res.Success == false || oPortGroups.Count == 0)
            {
                WriteOutput("[ERROR] fetching port groups:" + res);
                return;
            }

            //PortGroupCodec
            List<PortGroupCodec> oPortGroupCodecs;
            res = PortGroupCodec.GetPortGroupCodecs(_server, oPortGroups[0].ObjectId, out oPortGroupCodecs);
            if (res.Success == false || oPortGroupCodecs.Count == 0)
            {
                WriteOutput("[ERROR] fetching port group codecs:" + res);
                return;
            }

            //PortGroupServer
            List<PortGroupServer> oPortGroupServers;
            res = PortGroupServer.GetPortGroupServers(_server, oPortGroups[0].ObjectId, out oPortGroupServers);
            if (res.Success == false || oPortGroupServers.Count == 0)
            {
                WriteOutput("[ERROR] fetching port group servers:" + res);
                return;
            }

            //PortGroupTemplate
            List<PortGroupTemplate> oPortGroupTemplates;
            res = PortGroupTemplate.GetPortGroupTemplates(_server, out oPortGroupTemplates);
            if (res.Success == false || oPortGroupTemplates.Count == 0)
            {
                WriteOutput("[ERROR] fetching port group templates:" + res);
                return;
            }

            //privatelist
            List<PrivateList> oPrivateLists;
            res = PrivateList.GetPrivateLists(_server, oTestUser.ObjectId, out oPrivateLists, 1, 2);
            if (res.Success == false || oPrivateLists.Count == 0)
            {
                WriteOutput("[ERROR] fetching private lists:" + res);
                return;
            }

            //private list member
            foreach (var oPrivateList in oPrivateLists)
            {
                List<PrivateListMember> oPrivateListMembers;
                res = PrivateList.GetMembersList(_server, oPrivateList.ObjectId, oTestUser.ObjectId,out oPrivateListMembers);
                if (res.Success == false)
                {
                    WriteOutput("[ERROR] fetching private members:" + res);
                    return;
                }
                if (oPrivateListMembers.Count > 0)
                {
                    break;
                }
            }

            //Restriction Table
            List<RestrictionTable> oRestrictionTables;
            res = RestrictionTable.GetRestrictionTables(_server, out oRestrictionTables, 1, 2);
            if (res.Success == false || oRestrictionTables.Count == 0)
            {
                WriteOutput("[ERROR] fetching restriction tables:" + res);
                return;
            }


            //Restriction table pattern
            List<RestrictionPattern> oRestrictionPatterns;
            res = RestrictionPattern.GetRestrictionPatterns(_server, oRestrictionTables[0].ObjectId,out oRestrictionPatterns);
            if (res.Success == false || oRestrictionPatterns.Count == 0)
            {
                WriteOutput("[ERROR] fetching restriction patterns:" + res);
                return;
            }

            //Role
            List<Role> oRoles;
            res = Role.GetRoles(_server, out oRoles);
            if (res.Success == false || oRoles.Count == 0)
            {
                WriteOutput("[ERROR] fetching roles:" + res);
                return;
            }

            //RTPCodecDef
            List<RtpCodecDef> oRtpCodecDefs;
            res = RtpCodecDef.GetRtpCodecDefs(_server, out oRtpCodecDefs);
            if (res.Success == false || oRtpCodecDefs.Count == 0)
            {
                WriteOutput("[ERROR] fetching RTP Codec defs:" + res);
                return;
            }

            //Schedule
            List<Schedule> oSchedules;
            res = Schedule.GetSchedules(_server, out oSchedules, 1, 2);
            if (res.Success == false || oSchedules.Count == 0)
            {
                WriteOutput("[ERROR] fetching schedules:" + res);
                return;
            }

            //ScheduleDetail
            foreach (var oSchedule in oSchedules)
            {
                List<ScheduleDetail> oScheduleDetails;
                res = oSchedule.GetScheduleDetails(out oScheduleDetails);
                if (res.Success == false)
                {
                    WriteOutput("[ERROR] fetching schedule details:");
                    return;
                }
                if (oScheduleDetails.Count > 0)
                {
                    break;
                }
            }


            //Sechedule Set
            List<ScheduleSet> oScheduleSets;
            res = ScheduleSet.GetSchedulesSets(_server, out oScheduleSets, 1, 2);
            if (res.Success == false || oScheduleSets.Count == 0)
            {
                WriteOutput("[ERROR] fetching schedule sets:" + res);
                return;
            }

            //ScheduleSetMember
            List<ScheduleSetMember> oScheduleSetMembers;
            res = ScheduleSet.GetSchedulesSetsMembers(_server, oScheduleSets[0].ObjectId, out oScheduleSetMembers);
            if (res.Success == false || oScheduleSetMembers.Count == 0)
            {
                WriteOutput("[ERROR] fetching schedule set members:" + res);
                return;
            }


            //Search Space
            List<SearchSpace> oSearchSpaces;
            res = SearchSpace.GetSearchSpaces(_server, out oSearchSpaces, 1, 2);
            if (res.Success == false || oSearchSpaces.Count == 0)
            {
                WriteOutput("[ERROR] fetching SearchSpaces:" + res);
                return;
            }

            //SmppProvider
            List<SmppProvider> oSmppProviders;
            res = SmppProvider.GetSmppProviders(_server, out oSmppProviders, 1, 2);
            if (res.Success == false || oSmppProviders.Count == 0)
            {
                WriteOutput("[ERROR] fetching SMPP providers :" + res);
                return;
            }

            //TimeZones
            try
            {
                TimeZones oTimeZones = new TimeZones(_server);
            }
            catch (Exception ex)
            {
                WriteOutput("[ERROR] fetching time zones:"+ex);
                return;
            }
            

            //Transfer Options
            List<TransferOption> oTransferOptions;
            res = TransferOption.GetTransferOptions(_server, oTestUser.PrimaryCallHandler().ObjectId,out oTransferOptions);
            if (res.Success == false || oTransferOptions.Count == 0)
            {
                WriteOutput("[ERROR] fetching transfer options:" + res);
                return;
            }

            //UserBase
            List<UserBase> oUserBases;
            res = UserBase.GetUsers(_server, out oUserBases, 1, 2);
            if (res.Success == false || oUserBases.Count == 0)
            {
                WriteOutput("[ERROR] fetching user base:" + res);
                return;
            }

            //UserFull
            //fetched above

            //UserMessage
            List<UserMessage> oMessages;
            res = UserMessage.GetMessages(_server, oTestUser.ObjectId, out oMessages, 1, 2);
            if (res.Success == false || oMessages.Count == 0)
            {
                WriteOutput("[ERROR] fetching user messages:" + res);
                return;
            }

            //UserTemplate
            List<UserTemplate> oUserTemplates;
            res = UserTemplate.GetUserTemplates(_server, out oUserTemplates, 1, 2);
            if (res.Success == false || oUserTemplates.Count == 0)
            {
                WriteOutput("[ERROR] fetching user templates:" + res);
                return;
            }

            //VmsServer
            List<VmsServer> oVmsServers;
            res = VmsServer.GetVmsServers(_server, out oVmsServers);
            if (res.Success == false || oVmsServers.Count == 0)
            {
                WriteOutput("[ERROR] fetching VMSServers:" + res);
                return;
            }



            Logger.Log("All Tests Executed");
        }


        private static void WriteOutput(string pLine)
        {
            Console.WriteLine(pLine);
            Logger.Log(pLine);
        }


        /// <summary>
        /// Fires when the JSON serializer indicates there's a missing property error when populating a class.
        /// Ignores missing URI properties since the SDK doesn't keep those around - they can easily be reconstructed
        /// </summary>
        private static void JsonSerializerError(object sender, ErrorEventArgs e)
        {
            if (!e.ErrorContext.Error.Message.Contains("URI'"))
            {
                Logger.Log(string.Format("[DEBUG][{0}]:{1}",e.CurrentObject.GetType().Name, 
                    e.ErrorContext.Error.Message));
            }
            
        }
    }
}
