using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Summary description for UserUnitTests
    /// </summary>
    [TestClass]
    public class UserIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //used for editing/adding items to a temporary user that gets cleaned up after the tests are complete
        private static UserFull _tempUser;

        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            //create new list with GUID in the name to ensure uniqueness
            String strUserAlias = "TempUser_" + Guid.NewGuid().ToString().Replace("-", "");

            //generate a random number and tack it onto the end of some zeros so we're sure to avoid any legit numbers on the system.
            Random random = new Random();
            int iExtPostfix = random.Next(100000, 999999);
            string strExtension = "000000" + iExtPostfix.ToString();

            //use a bogus extension number that's legal but non dialable to avoid conflicts
            WebCallResult res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", strUserAlias, strExtension, null, out _tempUser);
            Assert.IsTrue(res.Success, "Failed creating temporary user:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempUser != null)
            {
                WebCallResult res = _tempUser.Delete();
                Assert.IsTrue(res.Success,"Failed to delete temporary user on cleanup.");
            }
        }
    
        #endregion


        #region Class Creation Failures

        /// <summary>
        /// UnityConnectionRestException on invalid ObjectID
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void User_Constructor_InvalidObjectIds_Failure()
        {
            var oTemp = new UserBase(_connectionServer, "UserObjectId");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid alias
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void User_Constructor_InvalidAlias_Failure()
        {
            var oTemp = new UserBase(_connectionServer, "","Bogus alias");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid ObjectID
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Mwi_Constructor_InvalidObjectIds_Failure()
        {
            Mwi oTemp = new Mwi(_connectionServer, "UserObjectId","MwiObjectId");
            Console.WriteLine(oTemp);
        }

       #endregion


        #region Static Call Tests

        [TestMethod]
        public void User_GetUser_InvalidObjectId_Failure()
        {
            UserBase oUserBase;
            var res = UserBase.GetUser(out oUserBase, _connectionServer, "ObjectId");
            Assert.IsFalse(res.Success, "Invalid Usser ObjectId should fail");
        }

        [TestMethod]
        public void User_GetUser_InvalidAlias_Failure()
        {
            UserBase oUserBase;
            var res = UserBase.GetUser(out oUserBase, _connectionServer, "","bogus alias");
            Assert.IsFalse(res.Success, "Invalid Alias should fail");
        }

        [TestMethod]
        public void UserFull_GetUser_InvalidObjectId_Failure()
        {
            UserFull oUserFull;
            var res = UserFull.GetUser(_connectionServer, "ObjectId", out oUserFull);
            Assert.IsFalse(res.Success, "Invalid Usser ObjectId should fail");
        }

        [TestMethod]
        public void GetUsers_ClauseFetch_Success()
        {
            List<UserBase> oUserList;
            string[] strList = new string[2];

            strList[0] = "rowsPerPage=1";
            strList[1] = ""; //make sure empty clause is handled

            WebCallResult res = UserBase.GetUsers(_connectionServer, out oUserList, strList);
            Assert.IsTrue(res.Success, "Could not fetch first user");
            Assert.IsNotNull(oUserList, "Null user list returned");
            Assert.IsTrue(oUserList.Count > 0, "Empty list of users returned");
        }

        [TestMethod]
        public void GetUsers_InvalidQuery_Success()
        {
            List<UserBase> oUserList;
            var res = UserBase.GetUsers(_connectionServer, out oUserList, "query=(blah is blah)");
            Assert.IsFalse(res.Success, "Invalid query param should fail");
        }

        [TestMethod]
        public void GetUsers_QueryWithNoResults_Success()
        {
            List<UserBase> oUserList;
            var res = UserBase.GetUsers(_connectionServer, out oUserList, "query=(ObjectId is blah)");
            Assert.IsTrue(res.Success, "query with zero results should not fail:"+res);
        }

        [TestMethod]
        public void UpdateUser_InvalidObjectId_Failure()
        {
            ConnectionPropertyList oPropList = new ConnectionPropertyList();
            oPropList.Add("test", "value");

            var res = UserBase.UpdateUser(_connectionServer, "ObjectId", oPropList);
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");
        }

        [TestMethod]
        public void GetUserVoiceName_InvalidObjectId()
        {
            var res = UserBase.GetUserVoiceName(_connectionServer, "temp.wav", "aaa");
            Assert.IsFalse(res.Success, "Invalid user objectID should fail");
        }

        [TestMethod]
        public void SetUserVoiceName_InvalidObjectId_Failure()
        {
            var res = UserBase.SetUserVoiceName(_connectionServer, "Dummy.wav", "ObjectId", true);
            Assert.IsFalse(res.Success, "Invalid user objectID should fail");
        }

        [TestMethod]
        public void SetUserVoiceNameToStreamFile_InvalidObjectId_Failure()
        {
            WebCallResult res = UserBase.SetUserVoiceNameToStreamFile(_connectionServer, "ObjectId", "StreamFileId");
            Assert.IsFalse(res.Success, "Invalid user objectID should fail");
        }

        [TestMethod]
        public void AddUser_InvalidTemplateAlias_Failure()
        {
            var res = UserBase.AddUser(_connectionServer, "invalid template alias", "Alias", "1234",null);
            Assert.IsFalse(res.Success, "AddUser should fail if the template alias is invalid");
        }

        [TestMethod]
        public void StaticCallFailure_DeleteUser()
        {
            var res = UserBase.DeleteUser(_connectionServer, "ObjectId");
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void GetUsers_Success()
        {
            List<UserBase> oUserBaseList;

            WebCallResult res = UserBase.GetUsers(_connectionServer, out oUserBaseList, 1, 5, null);
            Assert.IsTrue(res.Success, "Failed to fetch first user in system");
            Assert.IsNotNull(oUserBaseList, "Null user list returned");
            Assert.IsTrue(oUserBaseList.Count > 0, "Empty user list returned");

            Console.WriteLine(oUserBaseList.First().ToString());
            Console.WriteLine(oUserBaseList.First().DumpAllProps());
        }

        [TestMethod]
        public void GetUser_ByObjectId_Success()
        {
            UserBase oTempBase;
            var res = UserBase.GetUser(out oTempBase, _connectionServer, _tempUser.ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch base user properties for selected user by objectId");

            }

        [TestMethod]
        public void GetUser_ByAlias_Success()
        {
            UserBase oTempBase;
            var res = UserBase.GetUser(out oTempBase, _connectionServer, "", _tempUser.Alias);
            Assert.IsTrue(res.Success, "Failed to fetch base user properties for selected user by alias");
        }

        [TestMethod]
        public void UserFull_GetUser_ByObjectId_Success()
        {
            UserFull oUserFull;
            var res = UserFull.GetUser(_connectionServer, _tempUser.ObjectId, out oUserFull);
            Assert.IsTrue(res.Success, "Failed to fetch full user properties for selected user by objectId");
            
            Console.WriteLine(oUserFull.ToString());
            Console.WriteLine(oUserFull.DumpAllProps());

            //dump primary call handler details
            Console.WriteLine(oUserFull.PrimaryCallHandler().ToString());

            Console.WriteLine(oUserFull.PrimaryCallHandler(true).ToString());

            // dump the phone system details
            Console.WriteLine(oUserFull.PhoneSystem().ToString());
            Console.WriteLine(oUserFull.PhoneSystem(true).ToString());

            //dump the private list details
            Console.WriteLine(oUserFull.PrivateLists().ToString());
            Console.WriteLine(oUserFull.PrivateLists(true).ToString());

            //dump the MWI list
            Console.WriteLine(oUserFull.Mwis().ToString());
            Console.WriteLine(oUserFull.Mwis(true).ToString());

            //dump the COS details
            Console.WriteLine(oUserFull.Cos().ToString());
            Console.WriteLine(oUserFull.Cos(true).ToString());

            //Show the user's credentials for PIN and Password
            Console.WriteLine("User PIN credential details:" + oUserFull.Pin());
            Console.WriteLine(oUserFull.Pin().DumpAllProps());

            Console.WriteLine("User Password credential details:" + oUserFull.Password());
            Console.WriteLine(oUserFull.Password().DumpAllProps());
        }

        [TestMethod]
        public void GetCredential_Pin_Success()
        {
            Credential oCredential;
            var res = Credential.GetCredential(_connectionServer, _tempUser.ObjectId, CredentialType.Pin, out oCredential);
            Assert.IsTrue(res.Success, "Failed to fetch PIN credential manually:" + res.ToString());
        }

        [TestMethod]
        public void GetCredential_Password_Success()
        {
            Credential oCredential;
            var res = Credential.GetCredential(_connectionServer, _tempUser.ObjectId, CredentialType.Password, out oCredential);
            Assert.IsTrue(res.Success, "Failed to fetch password credential manually:" + res.ToString());
        }

        [TestMethod]
        public void UserCompare_Tests()
        {
            List<UserBase> oUserBaseList;

            var res = UserBase.GetUsers(_connectionServer, out oUserBaseList, 1, 5, null);
            Assert.IsTrue(res.Success,"GetUsers failed");

            //hit a couple of the list sorting entries
            UserComparer oCompareer = new UserComparer("DTMFAccessID");
            oUserBaseList.Sort(oCompareer);

            oCompareer = new UserComparer("Alias");
            oUserBaseList.Sort(oCompareer);

            oCompareer = new UserComparer("FirstName");
            oUserBaseList.Sort(oCompareer);

            oCompareer = new UserComparer("LastName");
            oUserBaseList.Sort(oCompareer);

            oCompareer = new UserComparer("LastName");
            oUserBaseList.Sort(oCompareer);

            //defaults to alias
            oCompareer = new UserComparer("bogus");
            oUserBaseList.Sort(oCompareer);
        }

        [TestMethod]
        public void GetUsers_QueryNoResults_Success()
        {
            List<UserBase> oUserBaseList;
            var res = UserBase.GetUsers(_connectionServer, out oUserBaseList, 1, 5, "query=(ObjectId is bogus)");
            Assert.IsTrue(res.Success, "fetching users with zero results query should not fail:" + res);
            Assert.IsTrue(oUserBaseList.Count == 0, "Zero results query string should return an empty user list:" + oUserBaseList.Count);
        }

        [TestMethod]
        public void GetUsers_InvalidQuery_Failure()
        {
            List<UserBase> oUserBaseList;
            var res = UserBase.GetUsers(_connectionServer, out oUserBaseList, 1, 5, "query=(bogus is bogus)");
            Assert.IsFalse(res.Success, "fetching users with invalid query should fail:" + res);
            Assert.IsTrue(oUserBaseList.Count == 0, "Invalid query string should return an empty user list:" + oUserBaseList.Count);
        }

        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            WebCallResult res = _tempUser.Update();
            Assert.IsFalse(res.Success, "Updating a user with no pending changes should fail");
        }

        [TestMethod]
        public void Update_TopLevel_Success()
        {
            _tempUser.FirstName = "FirstName";
            _tempUser.LastName = "LastName";
            _tempUser.Department = "Department";
            _tempUser.Title = "Title";
            _tempUser.State = "State";
            _tempUser.Manager = "Manager";
            _tempUser.PostalCode = "99999";
            _tempUser.City = "City";
            _tempUser.BillingId = "1234";
            _tempUser.XferString = "87654";
            _tempUser.Building = "Building";
            _tempUser.EmailAddress = "test@test.com";
            _tempUser.Initials = "abc";
            _tempUser.EmployeeId = "1234";
            _tempUser.Address = "123 Maple way";
            _tempUser.EnhancedSecurityAlias = "testing";

            var res = _tempUser.Update();
            Assert.IsTrue(res.Success, "Failed to update user object:" + res.ToString());
        }

        [TestMethod]
        public void Update_ExitActionInvalidActionType_Failure()
        {
            _tempUser.ClearPendingChanges();

            _tempUser.ExitAction = ActionTypes.Invalid;
            _tempUser.ExitTargetConversation = ConversationNames.PHInterview;
            _tempUser.ExitTargetHandlerObjectId = _tempUser.ObjectId;

            var res = _tempUser.Update();
            Assert.IsFalse(res.Success, "Setting exit action to an illegal action did not fail");
        }

        [TestMethod]
        public void Update_ExitActionIllegalTargetObjectId_Failure()
        {
            _tempUser.ClearPendingChanges();

            _tempUser.ExitAction = ActionTypes.GoTo;
            _tempUser.ExitTargetConversation = ConversationNames.PHInterview;
            _tempUser.ExitTargetHandlerObjectId = _tempUser.ObjectId;
            var res = _tempUser.Update();
            Assert.IsFalse(res.Success, "Setting exit action to phInterview and an illegal objectId did not fail");
        }

        [TestMethod]
        public void Update_ExitAction_Success()
        {
            _tempUser.ClearPendingChanges();

            _tempUser.ExitAction = ActionTypes.GoTo;
            _tempUser.ExitTargetConversation = ConversationNames.PHGreeting;
            _tempUser.ExitTargetHandlerObjectId = _tempUser.PrimaryCallHandler().ObjectId;
            var res = _tempUser.Update();

            Assert.IsTrue(res.Success,"Updating exit conversation to valid values failed:"+res);
        }

        [TestMethod]
        public void GetVoiceName_Success()
        {
             WebCallResult res = _tempUser.GetVoiceName("temp.wav");
             Assert.IsFalse(res.Success, "Newly created user should have no voice name, fetch should fail");
        }

        [TestMethod]
        public void SetVoiceName_Success()
        {
             var res = _tempUser.SetVoiceName("Dummy.wav", true);
             Assert.IsTrue(res.Success, "Failed updating the voice name:" + res.ToString());
        }

        [TestMethod]
        public void SetVoiceNameToStreamFile_InvalidStreamName_Failure()
        {
             var res = _tempUser.SetVoiceNameToStreamFile("blah");
             Assert.IsFalse(res.Success, "Invalid voice name file should fail");
        }

        [TestMethod]
        public void UserBase_NotificationDevices_Iteration()
        {
            foreach (NotificationDevice oDevice in _tempUser.NotificationDevices())
            {
                Console.WriteLine(oDevice.ToString());
                Console.WriteLine(oDevice.DumpAllProps());
            }
         }

        //all users will have a home phone device setup already
        private NotificationDevice HelperGetPhoneNotificationDevice()
        {
            NotificationDevice oNotificationDevice;
            var res = _tempUser.GetNotificationDevice("Home Phone", out oNotificationDevice);
            Assert.IsTrue(res.Success, "Failed to fetch HomePhone notificaiton device:" + res.ToString());
            return oNotificationDevice;
        }

        [TestMethod]
        public void GetNotificationDevice_Success()
        {
            NotificationDevice oNotificationDevice;
            var res = _tempUser.GetNotificationDevice("Home Phone", out oNotificationDevice);
            Assert.IsTrue(res.Success, "Failed to fetch HomePhone notificaiton device:" + res.ToString());
        }

        [TestMethod]
        public void NotificationDevice_Update_NoPendingChanges_Failure()
        {
            var oNotificationDevice = HelperGetPhoneNotificationDevice();
            oNotificationDevice.ClearPendingChanges();

            //saving with no pending changes should throw an error
            var res = oNotificationDevice.Update();
            Assert.IsFalse(res.Success, "Update of a notificaiton device with no pending changes should fail");
        }

        [TestMethod]
        public void NotificationDevice_Update_PhoneNumber_Success()
        {
            var oNotificationDevice = HelperGetPhoneNotificationDevice();
            oNotificationDevice.ClearPendingChanges();
            oNotificationDevice.PhoneNumber = "1234";

            var res = oNotificationDevice.Update();
            Assert.IsTrue(res.Success, "Failed updating Home Phone notification device:" + res.ToString());
        }

        [TestMethod]
        public void NotificationDevice_AddUpdateDelete_Success()
        {
            var res = NotificationDevice.AddSmtpDevice(_connectionServer, _tempUser.ObjectId, "NewSMTPDevice", "test@test.com",
                                             NotificationEventTypes.NewUrgentVoiceMail.ToString(), true);

            Assert.IsTrue(res.Success, "Failed to add new SMTP notification device:" + res.ToString());
            Assert.IsTrue(res.ReturnedObjectId.Length > 0, "Empty objectID returned for new SMTP device creation");

            NotificationDevice oNotificationDevice;
            res = _tempUser.GetNotificationDevice("NewSMTPDevice", out oNotificationDevice, true);
            Assert.IsTrue(res.Success, "Failed to fetch newly created SMTP notification device:" + res.ToString());

            res = oNotificationDevice.Delete();
            Assert.IsTrue(res.Success, "Failed removing newly added SMTP notification device:" + res.ToString());
        }

        [TestMethod]
        public void NotificationDevice_Update_SetFailoverToSelf_Failure()
        {
            var oNotificationDevice = HelperGetPhoneNotificationDevice();
            oNotificationDevice.ClearPendingChanges();

            oNotificationDevice.SmtpAddress = "test2@test.com";
            oNotificationDevice.StaticText = "static text";
            oNotificationDevice.FailDeviceObjectId = oNotificationDevice.ObjectId;
            var res = oNotificationDevice.Update();
            Assert.IsFalse(res.Success, "Setting failed path for notification did not fail when set to self:" + res.ToString());
        }

        [TestMethod]
        public void NotificationDevice_PhoneDevice_AddEditDelete_Success()
        {
            var oNotificationDevice = HelperGetPhoneNotificationDevice();

            var res = NotificationDevice.AddPhoneDevice(_connectionServer, _tempUser.ObjectId, "NewPhoneDevice", oNotificationDevice.MediaSwitchObjectId,
                "112233",NotificationEventTypes.NewUrgentVoiceMail.ToString(), true);

            Assert.IsTrue(res.Success, "Failed to add new phone notification device:" + res.ToString());
            Assert.IsTrue(res.ReturnedObjectId.Length > 0, "Empty objectID returned for new phone device creation");
            
            res = _tempUser.GetNotificationDevice("NewPhoneDevice", out oNotificationDevice, true);
            Assert.IsTrue(res.Success, "Failed to fetch newly created phone notification device");

            oNotificationDevice.AfterDialDigits = "123";
            res = oNotificationDevice.Update();
            Assert.IsTrue(res.Success, "Failed updating notification device:" + res.ToString());

            res = oNotificationDevice.Delete();
            Assert.IsTrue(res.Success, "Failed removing newly added Phone notification device:" + res.ToString());

            }

        [TestMethod]
        public void NotificationDevice_PagerDevice_AddEditDelete_Success()
        {
            var oNotificationDevice = HelperGetPhoneNotificationDevice();
            
            var res = NotificationDevice.AddPagerDevice(_connectionServer, _tempUser.ObjectId, "NewPagerDevice",oNotificationDevice.MediaSwitchObjectId,
                                             "12345", NotificationEventTypes.NewUrgentVoiceMail.ToString(), true);

            Assert.IsTrue(res.Success, "Failed to add new Pager notification device:" + res.ToString());
            Assert.IsTrue(res.ReturnedObjectId.Length > 0, "Empty objectID returned for new Pager device creation");

            res = _tempUser.GetNotificationDevice("blah", out oNotificationDevice);
            Assert.IsFalse(res.Success, "Invalid notification device name should result in an error");

            res = _tempUser.GetNotificationDevice("NewPagerDevice", out oNotificationDevice, true);
            Assert.IsTrue(res.Success, "Failed to fetch newly created pager notification device");

            res = oNotificationDevice.Delete();
            Assert.IsTrue(res.Success, "Failed removing newly added Pager notification device:" + res.ToString());
        }

        [TestMethod]
        public void ResetPassword_Success()
        {
            WebCallResult res = _tempUser.ResetPassword("as!aAsdfui09au", false, false, false, true);
            Assert.IsTrue(res.Success, "Resetting user password failed:" + res.ToString());

        }

        [TestMethod]
        public void AddMwi_FromUserInstance_Success()
        {
            var res = UserBase.AddMwi(_connectionServer, _tempUser.ObjectId, "112341324123", "MWI2");
            Assert.IsTrue(res.Success, "Failed to add MWI manually:" + res.ToString());
        }

        [TestMethod]
        public void AddEditDeleteMwi_Success()
        {
            var res = Mwi.AddMwi(_connectionServer, _tempUser.ObjectId, "display name", _tempUser.MediaSwitchObjectId,
                                 "1234321", false);
            Assert.IsTrue(res.Success, "Failed to create MWI device for user:" + res);

            Mwi oMwiDevice;
            res = Mwi.GetMwiDevice(_connectionServer, _tempUser.ObjectId, res.ReturnedObjectId, out oMwiDevice);
            Assert.IsTrue(res.Success, "Failed to fetch single MWI device just added for user:" + res);

            Console.WriteLine(oMwiDevice.ToString());
            Console.WriteLine(oMwiDevice.DumpAllProps());

            res = oMwiDevice.Update();
            Assert.IsFalse(res.Success, "Calling update on MWi device with no pending changes did not fail");

            oMwiDevice.DisplayName = "Updated Display Name";
            oMwiDevice.Active = false;

            res = oMwiDevice.Update();
            Assert.IsTrue(res.Success, "Updating MWI properties failed:" + res);

            Thread.Sleep(2000);

            List<Mwi> oMwis;
            res = Mwi.GetMwiDevices(_connectionServer, _tempUser.ObjectId, out oMwis);
            Assert.IsTrue(res.Success, "Failed to fetch MWI device for user:" + res);
            Assert.IsTrue(oMwis.Count >= 2, "Mwi count is not at least 2 after adding device, instead its" + oMwis.Count);

            res = Mwi.DeleteMwiDevice(_connectionServer, _tempUser.ObjectId, oMwiDevice.ObjectId);
            Assert.IsTrue(res.Success, "Failed to delete MWI device for user:" + res);
        }

        [TestMethod]
        public void AddMwi_InvalidObjectId_Failure()
        {
            var res = Mwi.AddMwi(_connectionServer, "ObjectId", "display name", "MediaSwitch", "1234", false);
            Assert.IsFalse(res.Success, "Static call to AddMwi did not fail with: invalid ObjectId");

         }

        [TestMethod]
        public void DeleteMwiDevice_InvalidObjectId_Failure()
        {
            var res = Mwi.DeleteMwiDevice(_connectionServer, "UserObjectId", "ObjectId");
            Assert.IsFalse(res.Success, "Static call to DeleteMwiDevice did not fail with: invalid ObjectId");

         }

        [TestMethod]
        public void GetMwiDevices_InvalidOBjectId_Failure()
        {
            List<Mwi> oMwis;
            var res = Mwi.GetMwiDevices(_connectionServer, "ObjectId", out oMwis);
            Assert.IsFalse(res.Success, "Static call to GetMwiDevices did not fail with: invalid ObjectId");
        }

        [TestMethod]
        public void GetMwiDevice_InvalidOBjectId_Failure()
        {
            Mwi oMwiDevice;
            var res = Mwi.GetMwiDevice(_connectionServer, "UserObjectId", "ObjectId", out oMwiDevice);
            Assert.IsFalse(res.Success, "Static call to GetMwiDevice did not fail with: invalid ObjectId");
        }

        #endregion


        #region User Compare Tests

        [TestMethod]
        public void UserCompareTests_FirstName()
        {
            UserBase oUser1=new UserBase();
            UserBase oUser2=new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUsers.Sort(new UserComparer("firstname"));
            oUser1.FirstName = "bbb";
            oUsers.Sort(new UserComparer("firstname"));
            Assert.IsTrue(oUsers[0]==oUser1);

            oUser2.FirstName = "aaa";
            oUsers.Sort(new UserComparer("firstname"));
            Assert.IsTrue(oUsers[0] == oUser2);

        }

        [TestMethod]
        public void UserCompareTests_LastName()
        {
            UserBase oUser1 = new UserBase();
            UserBase oUser2 = new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUsers.Sort(new UserComparer("lastname"));
            oUser1.LastName = "bbb";
            oUsers.Sort(new UserComparer("lastname"));
            Assert.IsTrue(oUsers[0] == oUser1);

            oUser2.LastName = "aaa";
            oUsers.Sort(new UserComparer("lastname"));
            Assert.IsTrue(oUsers[0] == oUser2);
        }

        [TestMethod]
        public void UserCompareTests_DisplayName()
        {
            UserBase oUser1 = new UserBase();
            UserBase oUser2 = new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUsers.Sort(new UserComparer("displayname"));
            oUser1.DisplayName = "bbb";
            oUsers.Sort(new UserComparer("displayname"));
            Assert.IsTrue(oUsers[0] == oUser1);

            oUser2.DisplayName = "aaa";
            oUsers.Sort(new UserComparer("displayname"));
            Assert.IsTrue(oUsers[0] == oUser2);

        }

        [TestMethod]
        public void UserCompareTests_Extension()
        {
            UserBase oUser1 = new UserBase();
            UserBase oUser2 = new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUser1.DtmfAccessId = "2222";
            oUser2.DtmfAccessId = "1111";
            oUsers.Sort(new UserComparer("dtmfaccessid"));
            Assert.IsTrue(oUsers[0] == oUser2);

        }

        [TestMethod]
        public void UserCompareTests_Alias()
        {
            UserBase oUser1 = new UserBase();
            UserBase oUser2 = new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUser1.Alias = "bbbb";
            oUser2.Alias = "aaaa";
            oUsers.Sort(new UserComparer("alias"));
            Assert.IsTrue(oUsers[0] == oUser2);

        }

        
        #endregion
    }
}
