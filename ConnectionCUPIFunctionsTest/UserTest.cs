using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Summary description for UserTest
    /// </summary>
    [TestClass]
    public class UserTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        //used for editing/adding items to a temporary user that gets cleaned up after the tests are complete
        private static UserFull _tempUser;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


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
                 _connectionServer = new ConnectionServerRest(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start User test:" + ex.Message);
            }

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
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            UserBase oTemp = new UserBase(null);
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void ClassCreationEmpty()
        {
            UserBase oTemp = new UserBase();
            Assert.IsFalse(oTemp == null, "Empty class creation did not create a UserBase");
        }


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserTemplateClassCreationFailure()
        {
            UserTemplate oTemp = new UserTemplate(null, "xxxx");
            Console.WriteLine(oTemp);
        }

       

        /// <summary>
        /// ArgumentException on null ConnectionServer
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MWiCreationFailure()
        {
            Mwi oTemp = new Mwi(null,"bogus");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid ObjectID
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void MWiCreationFailure2()
        {
            Mwi oTemp = new Mwi(_connectionServer, "bogus","bogus");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// ArgumentException on empty objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MWiCreationFailure3()
        {
            Mwi oTemp = new Mwi(_connectionServer, "", "bogus");
            Console.WriteLine(oTemp);
        }


        #endregion


        #region Static Call Failure Tests

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_GetUser()
        {
            UserBase oUserBase;
            UserFull oUserFull;

            WebCallResult res = UserBase.GetUser(out oUserBase, null, "aaa");
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = UserBase.GetUser(out oUserBase, _connectionServer, "aaa");
            Assert.IsFalse(res.Success, "Invalid Usser ObjectId should fail");

            res = UserBase.GetUser(out oUserBase, _connectionServer, "");
            Assert.IsFalse(res.Success, "Empty User ObjectId and alias should fail");

            res = UserBase.GetUser(out oUserBase, _connectionServer, "","aaa");
            Assert.IsFalse(res.Success, "Invalid Alias should fail");


            //same calls with userFull class
            res = UserFull.GetUser(null, "aaa", out oUserFull);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = UserFull.GetUser(_connectionServer, "aaa", out oUserFull);
            Assert.IsFalse(res.Success, "Invalid Usser ObjectId should fail");

            res = UserFull.GetUser(_connectionServer, "", out oUserFull);
            Assert.IsFalse(res.Success, "Empty User ObjectId should fail");

        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_GetUsers()
        {
            List<UserBase> oUserList;
            string[] strList = new string[2];

            strList[0] = "rowsPerPage=1";
            strList[1] = ""; //make sure empty clause is handled

            WebCallResult res = UserBase.GetUsers(_connectionServer, out oUserList, strList);
            Assert.IsTrue(res.Success, "Could not fetch first user");
            Assert.IsNotNull(oUserList, "Null user list returned");
            Assert.IsTrue(oUserList.Count > 0, "Empty list of users returned");

            res = UserBase.GetUsers(null, out oUserList, strList);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            strList[0] = "query=(blah is blah)";
            res = UserBase.GetUsers(_connectionServer, out oUserList, strList);
            Assert.IsFalse(res.Success, "Invalid que param should fail");

        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_UpdateUsers()
        {
            ConnectionPropertyList oPropList = new ConnectionPropertyList();
            oPropList.Add("test", "value");

            WebCallResult res = UserBase.UpdateUser(_connectionServer, "aaa", oPropList);
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");

            res = UserBase.UpdateUser(_connectionServer, "", oPropList);
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");

            res = UserBase.UpdateUser(null, "aaa", oPropList);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

            res = UserBase.UpdateUser(_connectionServer, "aaa", null);
            Assert.IsFalse(res.Success, "Empty property list should fail");

        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_GetUserVoiceName()
        {
            WebCallResult res = UserBase.GetUserVoiceName(_connectionServer, "temp.wav", "aaa");
            Assert.IsFalse(res.Success, "Invalid user objectID should fail");

            res = UserBase.GetUserVoiceName(_connectionServer, "temp.wav", "");
            Assert.IsFalse(res.Success, "Empty user objectID should fail");

            res = UserBase.GetUserVoiceName(_connectionServer, "", "aaa");
            Assert.IsFalse(res.Success, "Empty target wav path should fail");

            res = UserBase.GetUserVoiceName(null, "temp.wav", "aaa");
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");

        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_SetUserVoiceName()
        {
            WebCallResult res = UserBase.SetUserVoiceName(_connectionServer, "Dummy.wav", "aaa", true);
            Assert.IsFalse(res.Success, "Invalid user objectID should fail");

            res = UserBase.SetUserVoiceName(_connectionServer, "wavcopy.exe", "aaa", true);
            Assert.IsFalse(res.Success, "Invalid WAV file should fail");

            res = UserBase.SetUserVoiceName(_connectionServer, "Dummy.wav", "");
            Assert.IsFalse(res.Success, "Empty user objectID should fail");

            res = UserBase.SetUserVoiceName(_connectionServer, "", "aaa");
            Assert.IsFalse(res.Success, "Empty target wav path should fail");

            res = UserBase.SetUserVoiceName(null, "Dummy.wav", "aaa");
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");

        }

        [TestMethod]
        public void StaticCallFailure_SetUserVoiceNameToStreamFile()
        {
            WebCallResult res = UserBase.SetUserVoiceNameToStreamFile(_connectionServer, "aaa", "bbb");
            Assert.IsFalse(res.Success, "Invalid user objectID should fail");

            res = UserBase.SetUserVoiceNameToStreamFile(_connectionServer, "", "aaa");
            Assert.IsFalse(res.Success, "Empty user objectID should fail");

            res = UserBase.SetUserVoiceNameToStreamFile(_connectionServer, "asdfasdf", "");
            Assert.IsFalse(res.Success, "Empty wav file wav path should fail");

            res = UserBase.SetUserVoiceNameToStreamFile(null, "aaa", "aaa");
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");

        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_AddUser()
        {
            WebCallResult res = UserBase.AddUser(null, "", "", "", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the ConnectionServerRest parameter is null");

            res = UserBase.AddUser(_connectionServer, "", "aaa", "123", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the template parameter is empty");

            res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", "aaa", "", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the extension parameter is empty");

            res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", "", "1234", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the DisplayName parameter is empty");
        }

        /// <summary>
        /// DELETE user static method failure paths
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_DeleteUser()
        {
            WebCallResult res = UserBase.DeleteUser(_connectionServer, "aaa");
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");

            res = UserBase.DeleteUser(_connectionServer, "");
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");

            res = UserBase.DeleteUser(null, "aaa");
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

        }

        #endregion


        #region Live Tests

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void GetAndListUsers_Test()
        {
            UserFull oUserFull;
            List<UserBase> oUserBaseList;

            //get the first couple user found (could be only 1 -operator- but that doesn't matter here).
            WebCallResult res = UserBase.GetUsers(_connectionServer, out oUserBaseList,1,5,null);
            Assert.IsTrue(res.Success, "Failed to fetch first user in system");
            Assert.IsNotNull(oUserBaseList, "Null user list returned");
            Assert.IsTrue(oUserBaseList.Count > 0, "Empty user list returned");


            Console.WriteLine(oUserBaseList.First().ToString());
            Console.WriteLine(oUserBaseList.First().DumpAllProps());

            //get the base user properties
            UserBase oTempBase;
            res = UserBase.GetUser(out oTempBase, _connectionServer, oUserBaseList.First().ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch base user properties for selected user");

            //get the full user properties 
            res = UserFull.GetUser(_connectionServer, oUserBaseList.First().ObjectId, out oUserFull);
            Assert.IsTrue(res.Success, "Failed to fetch full user properties for selected user");

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

            //Fetch credentials via static function
            Credential oCredential;
            res = Credential.GetCredential(_connectionServer, oUserFull.ObjectId, CredentialType.Pin, out oCredential);
            Assert.IsTrue(res.Success, "Failed to fetch PIN credential manually:" + res.ToString());

            res = Credential.GetCredential(_connectionServer, oUserFull.ObjectId, CredentialType.Password, out oCredential);
            Assert.IsTrue(res.Success, "Failed to fetch password credential manually:" + res.ToString());



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


            res = UserBase.GetUsers(_connectionServer, out oUserBaseList, 1, 5, "query=(ObjectId is bogus)");
            Assert.IsTrue(res.Success, "fetching users with invalid query should not fail:" + res);
            Assert.IsTrue(oUserBaseList.Count == 0, "Invalid query string should return an empty user list:" + oUserBaseList.Count);
        }

        /// <summary>
        /// Large test that edits/reads/adds many items for users
        /// </summary>
        [TestMethod]
        public void EditUser_TopLevel()
        {
            //update a user property
            WebCallResult res = _tempUser.Update();
            Assert.IsFalse(res.Success, "Updating a user with no pending changes should fail");

            //fill in the user with some items not populated on a newly created user.
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


            res = _tempUser.Update();
            Assert.IsTrue(res.Success, "Failed to update user object:" + res.ToString());

            _tempUser.ClearPendingChanges();

            _tempUser.ExitAction = ActionTypes.Invalid;
            _tempUser.ExitTargetConversation = ConversationNames.PHInterview;
            _tempUser.ExitTargetHandlerObjectId = _tempUser.ObjectId;

            res = _tempUser.Update();
            Assert.IsFalse(res.Success, "Setting exit action to an illegal action did not fail");

            _tempUser.ClearPendingChanges();

            _tempUser.ExitAction = ActionTypes.GoTo;
            _tempUser.ExitTargetConversation = ConversationNames.PHInterview;
            _tempUser.ExitTargetHandlerObjectId = _tempUser.ObjectId;
            res = _tempUser.Update();
            Assert.IsFalse(res.Success, "Setting exit action to phInterview and an illegal objectId did not fail");

            _tempUser.ClearPendingChanges();

            _tempUser.ExitAction = ActionTypes.GoTo;
            _tempUser.ExitTargetConversation = ConversationNames.PHGreeting;
            _tempUser.ExitTargetHandlerObjectId = _tempUser.PrimaryCallHandler().ObjectId;
            res = _tempUser.Update();

            Assert.IsTrue(res.Success,"Updating exit conversation to valid values failed:"+res);
        }


         [TestMethod]
         public void EditUser_VoiceName()
         {
             //fetch voice name - this should fail
             WebCallResult res = _tempUser.GetVoiceName("temp.wav");
             Assert.IsFalse(res.Success, "Newly created user should have no voice name, fetch should fail");

             //update the voice name
             res = _tempUser.SetVoiceName("Dummy.wav", true);
             Assert.IsTrue(res.Success, "Failed updating the voice name:" + res.ToString());

             //failure case for an instance method of voice name update

             res = _tempUser.SetVoiceNameToStreamFile("blah");
             Assert.IsFalse(res.Success, "Invalid voice name file should fail");

             //get the voice name
             res = _tempUser.GetVoiceName("Temp.wav");
             Assert.IsTrue(res.Success, "Failed to fetch the newly added user voice name:" + res.ToString());
         }

        [TestMethod]
        public void EditUser_NotificationDevices()
        {
            //iterate the notificaitond Devices
            foreach (NotificationDevice oDevice in _tempUser.NotificationDevices())
            {
                Console.WriteLine(oDevice.ToString());
                Console.WriteLine(oDevice.DumpAllProps());
            }

            //add a new Phone notification device - first we need to grab any valid media switch ObjectId to use for 
            //creating the device - this will get used for both the Phone and Pager device add/removes
            List<PhoneSystem> oPhoneSystems;
            WebCallResult res = PhoneSystem.GetPhoneSystems(_connectionServer, out oPhoneSystems);
            Assert.IsTrue(res.Success, "Failed fetching phone systems:" + res.ToString());
            Assert.IsNotNull(oPhoneSystems, "Null phone system list returned");
            Assert.IsTrue(oPhoneSystems.Count > 0, "Empty list of phone systems returned");

            res = NotificationDevice.AddPhoneDevice(_connectionServer, _tempUser.ObjectId, "NewPhoneDevice", oPhoneSystems.First().ObjectId, "12345",
                                             NotificationEventTypes.NewUrgentVoiceMail.ToString(), true);
            Assert.IsTrue(res.Success, "Failed to add new Phone notification device:" + res.ToString());
            Assert.IsTrue(res.ReturnedObjectId.Length > 0, "Empty objectID returned for new Phone device creation");

            //force a refetch of data
            Console.WriteLine(_tempUser.NotificationDevices(true).ToString());

            //GET the HomePhone notification device, edit and save it
            NotificationDevice oNotificationDevice;
            res = _tempUser.GetNotificationDevice("Home Phone", out oNotificationDevice);
            Assert.IsTrue(res.Success, "Failed to fetch HomePhone notificaiton device:" + res.ToString());

            oNotificationDevice.ClearPendingChanges();

            //saving with no pending changes should throw an error
            res = oNotificationDevice.Update();
            Assert.IsFalse(res.Success, "Update of a notificaiton device with no pending changes should fail");

            oNotificationDevice.PhoneNumber = "1234";
            res = oNotificationDevice.Update();
            Assert.IsTrue(res.Success, "Failed updating Home Phone notification device:" + res.ToString());

            //add a new SMTP notification device
            res = NotificationDevice.AddSmtpDevice(_connectionServer, _tempUser.ObjectId, "NewSMTPDevice", "test@test.com",
                                             NotificationEventTypes.NewUrgentVoiceMail.ToString(), true);

            Assert.IsTrue(res.Success, "Failed to add new SMTP notification device:" + res.ToString());
            Assert.IsTrue(res.ReturnedObjectId.Length > 0, "Empty objectID returned for new SMTP device creation");

            //update phone notification device - be sure to ask for an updated list to be created since we just added a device and the user
            //already can have a cached list around.
            res = _tempUser.GetNotificationDevice("NewSMTPDevice", out oNotificationDevice, true);
            Assert.IsTrue(res.Success, "Failed to fetch newly created SMTP notification device:" + res.ToString());

            //test failure point of updating an unedited device
            oNotificationDevice.ClearPendingChanges();
            res = oNotificationDevice.Update();
            Assert.IsFalse(res.Success, "Updating a notificaiton device with no pending changes should fail");

            oNotificationDevice.SmtpAddress = "test2@test.com";
            oNotificationDevice.StaticText = "static text";
            oNotificationDevice.FailDeviceObjectId = oNotificationDevice.ObjectId;
            res = oNotificationDevice.Update();
            Assert.IsFalse(res.Success, "Setting failed path for notification did not fail when set to self:" + res.ToString());

            res = oNotificationDevice.Delete();
            Assert.IsTrue(res.Success, "Failed removing newly added SMTP notification device:" + res.ToString());

            //update phone notification device - be sure to ask for an updated list to be created since we just added a device and the user
            //already can have a cached list around.
            res = _tempUser.GetNotificationDevice("NewPhoneDevice", out oNotificationDevice, true);
            Assert.IsTrue(res.Success, "Failed to fetch newly created phone notification device");

            oNotificationDevice.AfterDialDigits = "123";
            res = oNotificationDevice.Update();
            Assert.IsTrue(res.Success, "Failed updating notification device:" + res.ToString());

            //delete the newly added notifiication device
            res = oNotificationDevice.Delete();
            Assert.IsTrue(res.Success, "Failed removing newly added Phone notification device:" + res.ToString());

            //add a new Pager notification device
            res = NotificationDevice.AddPagerDevice(_connectionServer, _tempUser.ObjectId, "NewPagerDevice", oPhoneSystems.First().ObjectId,
                                             "12345", NotificationEventTypes.NewUrgentVoiceMail.ToString(), true);

            Assert.IsTrue(res.Success, "Failed to add new Pager notification device:" + res.ToString());
            Assert.IsTrue(res.ReturnedObjectId.Length > 0, "Empty objectID returned for new Pager device creation");

            //delete the newly added notifiication device
            //res = NotificationDevice.DeleteNotificationDevice(_connectionServer, _tempUser.ObjectId, res.ReturnedObjectId,NotificationDeviceTypes.Pager);
            Assert.IsTrue(res.Success, "Failed removing newly added Pager notification device:" + res.ToString());

            //error case - invalid notification device name
            res = _tempUser.GetNotificationDevice("blah", out oNotificationDevice);
            Assert.IsFalse(res.Success, "Invalid notification device name should result in an error");
        }


        

        [TestMethod]
        public void EditUser_Passwords()
        {
            //Reset the user's password
            WebCallResult res = _tempUser.ResetPassword("ASDF234232sdf", false, false, false, true);
            Assert.IsTrue(res.Success, "Resetting user password failed:" + res.ToString());

            //Reset the user's PIN
            res = _tempUser.ResetPin("2349808", false, false, false, true, true);
            Assert.IsTrue(res.Success, "Resetting user PIN failed:" + res.ToString());

            //reset the pin failure
            res = UserBase.ResetUserPin(null, _tempUser.ObjectId, "1323424323");
            Assert.IsFalse(res.Success, "Resetting user PIN with null ConnectionServerRest should fail");

            //reset the PIN and Password
            res = _tempUser.ResetPin("234098234");
            Assert.IsTrue(res.Success, "Reset PIN failed:" + res.ToString());

            res = _tempUser.ResetPassword("asdfasdfui09au8dsf");
            Assert.IsTrue(res.Success, "Reset password failed:" + res.ToString());

        }


        /// <summary>
        /// Fetch, create and delete MWI devices in the temp user's account before they're deleted
        /// </summary>
        [TestMethod]
        public void MwiTests()
        {
            if (_tempUser == null)
            {
                Assert.Fail("Temp user not created, cannot run tests");
            }

            //add an MWI

            //fail case 1
            WebCallResult res = UserBase.AddMwi(null, _tempUser.ObjectId, "112341324123", "testMWI device");
            Assert.IsFalse(res.Success, "AddMwi should fail if Comserver instance is passed as null");

            //fail case 2
            res = UserBase.AddMwi(_connectionServer, _tempUser.ObjectId, "", "testMWI device");
            Assert.IsFalse(res.Success, "AddMwi should fail if MWI extension is passed as blank");

            //good case
            res = UserBase.AddMwi(_connectionServer, _tempUser.ObjectId, "112341324123", "MWI2");
            Assert.IsTrue(res.Success, "Failed to add MWI manually:" + res.ToString());

            //static method calls - create, fetch then delete an MWI device

            res = Mwi.AddMwi(_connectionServer, _tempUser.ObjectId, "display name", _tempUser.MediaSwitchObjectId, 
                "1234321", false);
            Assert.IsTrue(res.Success,"Failed to create MWI device for user:"+res);

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
            Assert.IsTrue(res.Success,"Updating MWI properties failed:"+res);

            Thread.Sleep(3000);

            List<Mwi> oMwis;
            res = Mwi.GetMwiDevices(_connectionServer, _tempUser.ObjectId, out oMwis);
            Assert.IsTrue(res.Success, "Failed to fetch MWI device for user:" + res);
            Assert.IsTrue(oMwis.Count >= 2, "Mwi count is not at least 2 after adding device, instead its"+oMwis.Count);

            res = Mwi.DeleteMwiDevice(_connectionServer, _tempUser.ObjectId, oMwiDevice.ObjectId);
            Assert.IsTrue(res.Success, "Failed to delete MWI device for user:" + res);

            //static call failures
            //AddMwi
            res = Mwi.AddMwi(null, "bogus", "display name", "MediaSwitch", "1234", false);
            Assert.IsFalse(res.Success,"Static call to AddMwi did not fail with: null Connection server");

            res = Mwi.AddMwi(_connectionServer, "", "display name", "MediaSwitch", "1234", false);
            Assert.IsFalse(res.Success, "Static call to AddMwi did not fail with: empty ObjectId ");

            res = Mwi.AddMwi(_connectionServer, "bogus", "display name", "MediaSwitch", "1234", false);
            Assert.IsFalse(res.Success, "Static call to AddMwi did not fail with: invalid ObjectId");

            //DeleteMwi
            res = Mwi.DeleteMwiDevice(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteMwiDevice did not fail with: null ConnectionServer");

            res = Mwi.DeleteMwiDevice(_connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteMwiDevice did not fail with: invalid ObjectId");

            res = Mwi.DeleteMwiDevice(_connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteMwiDevice did not fail with: empty ObjectId");

            //GetMwiDevices
            res = Mwi.GetMwiDevices(null, "bogus", out oMwis);
            Assert.IsFalse(res.Success, "Static call to GetMwiDevices did not fail with: null ConnectionServer");

            res = Mwi.GetMwiDevices(_connectionServer, "bogus", out oMwis);
            Assert.IsFalse(res.Success, "Static call to GetMwiDevices did not fail with: invalid ObjectId");

            res = Mwi.GetMwiDevices(_connectionServer, "", out oMwis);
            Assert.IsFalse(res.Success, "Static call to GetMwiDevices did not fail with: empty ObjectId");

            //GetMwiDevice
            res = Mwi.GetMwiDevice(null, "bogus", "bogus", out oMwiDevice);
            Assert.IsFalse(res.Success, "Static call to GetMwiDevice did not fail with: Null ConnectionServer");

            res = Mwi.GetMwiDevice(_connectionServer, "bogus", "bogus", out oMwiDevice);
            Assert.IsFalse(res.Success, "Static call to GetMwiDevice did not fail with: invalid ObjectId");

            res = Mwi.GetMwiDevice(_connectionServer, "", "bogus", out oMwiDevice);
            Assert.IsFalse(res.Success, "Static call to GetMwiDevice did not fail with: empty ObjectId");
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
