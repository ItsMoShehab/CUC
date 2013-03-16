using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ConnectionCUPIFunctions;
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

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

        }
    
        #endregion


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            UserBase oTemp = new UserBase(null);
        }

        [TestMethod()]
        public void ClassCreationEmpty()
        {
            UserBase oTemp = new UserBase();
            Assert.IsFalse(oTemp == null, "Empty class creation did not create a UserBase");
        }


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UserTemplateClassCreationFailure()
        {
            UserTemplate oTemp = new UserTemplate(null, "xxxx");
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a blank ObjectId is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AlternateExtensionCreationFailure2()
        {
            AlternateExtension oTemp = new AlternateExtension(_connectionServer, "");
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AlternateExtensionCreationFailure()
        {
            AlternateExtension oTemp = new AlternateExtension(null, "xxxx");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MWiCreationFailure()
        {
            Mwi oTemp = new Mwi(null,"bogus");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void MWiCreationFailure2()
        {
            Mwi oTemp = new Mwi(_connectionServer, "bogus","bogus");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MWiCreationFailure3()
        {
            Mwi oTemp = new Mwi(_connectionServer, "", "bogus");
        }


        #endregion


        /// <summary>
        /// Exercise the user template class in here - it's small and only gets used with users so rather than making a 
        /// seperate use test file for it, take care of it here.
        /// </summary>
        [TestMethod()]
        public void UserTemplateTests()
        {
            WebCallResult res;

            List<UserTemplate> oTemplates;

            res = UserTemplate.GetUserTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success, "Null Connection server should fail");

            res = UserTemplate.GetUserTemplates(_connectionServer, out oTemplates);
            Assert.IsTrue(res.Success, "Failed to fetch user templates");

            //dump string for all templates
            foreach (UserTemplate oTemp in oTemplates)
            {
                Console.WriteLine(oTemp.ToString());
            }
        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void GetAndListUsers_Test()
        {
            WebCallResult res;
            UserFull oUserFull;
            List<UserBase> oUserBaseList;
            
            //get the first couple user found (could be only 1 -operator- but that doesn't matter here).
            res = UserBase.GetUsers(_connectionServer, out oUserBaseList, "rowsPerPage=2");
            Assert.IsTrue(res.Success, "Failed to fetch first user in system");
            Assert.IsNotNull(oUserBaseList, "Null user list returned");
            Assert.IsTrue(oUserBaseList.Count > 0, "Empty user list returned");


            Console.WriteLine(oUserBaseList.First().ToString());
            Console.WriteLine(oUserBaseList.First().DumpAllProps());

            //get the base user properties
            UserBase oTempBase;
            res = UserBase.GetUser(out oTempBase,_connectionServer, oUserBaseList.First().ObjectId);
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
            Console.WriteLine("User PIN credential details:"+oUserFull.Pin().ToString());
            Console.WriteLine(oUserFull.Pin().DumpAllProps());

            Console.WriteLine("User Password credential details:"+oUserFull.Password().ToString());
            Console.WriteLine(oUserFull.Password().DumpAllProps());

            //Fetch credentials via static function
            Credential oCredential;
            res=Credential.GetCredential(_connectionServer, oUserFull.ObjectId, CredentialType.PIN, out oCredential);
            Assert.IsTrue(res.Success, "Failed to fetch PIN credential manually:"+res.ToString());

            res = Credential.GetCredential(_connectionServer, oUserFull.ObjectId, CredentialType.Password, out oCredential);
            Assert.IsTrue(res.Success, "Failed to fetch password credential manually:" + res.ToString());

            

            //hit a couple of the list sorting entries
            UserComparer oCompareer = new UserComparer("DTMFAccessID");
            oUserBaseList.Sort(oCompareer);

            oCompareer = new UserComparer("Alias");
            oUserBaseList.Sort(oCompareer);

        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void GetUser_Failure()
        {
            WebCallResult res;
            UserBase oUserBase;
            UserFull oUserFull;

            res = UserBase.GetUser(out oUserBase, null, "aaa");
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = UserBase.GetUser(out oUserBase, _connectionServer, "aaa");
            Assert.IsFalse(res.Success, "Invalid Usser ObjectId should fail");

            res = UserBase.GetUser(out oUserBase, _connectionServer, "","");
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
        [TestMethod()]
        public void GetUsers_Failure()
        {
            WebCallResult res;
            List<UserBase> oUserList;
            string[] strList;
            strList = new string[2];

            strList[0] = "rowsPerPage=1";
            strList[1] = ""; //make sure empty clause is handled

            res = UserBase.GetUsers(_connectionServer, out oUserList, strList);
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
        [TestMethod()]
        public void UpdateUsers_Failure()
        {
            WebCallResult res;
            ConnectionPropertyList oPropList = new ConnectionPropertyList();
            oPropList.Add("test", "value");

            res = UserBase.UpdateUser(_connectionServer, "aaa", oPropList);
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");

            res = UserBase.UpdateUser(_connectionServer, "", oPropList);
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");

            res = UserBase.UpdateUser(null, "aaa", oPropList);
            Assert.IsFalse(res.Success, "Null ConnectionServer object should fail");

            res = UserBase.UpdateUser(_connectionServer, "aaa", null);
            Assert.IsFalse(res.Success, "Empty property list should fail");

        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod()]
        public void GetUserVoiceName_Failure()
        {
            WebCallResult res;
            res = UserBase.GetUserVoiceName(_connectionServer, "temp.wav", "aaa");
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
        [TestMethod()]
        public void SetUserVoiceName_Failure()
        {
            WebCallResult res;
            res = UserBase.SetUserVoiceName(_connectionServer, "Dummy.wav", "aaa", true);
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

        [TestMethod()]
        public void SetUserVoiceNameToStreamFile_Failure()
        {
            WebCallResult res;

            res = UserBase.SetUserVoiceNameToStreamFile(_connectionServer, "aaa", "bbb");
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
        [TestMethod()]
        public void AddUser_Failure()
        {
            WebCallResult res;

            res = UserBase.AddUser(null, "", "", "", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the ConnectionServer parameter is null");

            res = UserBase.AddUser(_connectionServer, "", "aaa", "123", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the template parameter is empty");

            res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", "aaa", "", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the extension parameter is empty");

            res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", "", "1234", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the DisplayName parameter is empty");
        }

        /// <summary>
        /// Delete user static method failure paths
        /// </summary>
        [TestMethod()]
        public void DeleteUser_Failure()
        {
            WebCallResult res;

            res = UserBase.DeleteUser(_connectionServer, "aaa");
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");

            res = UserBase.DeleteUser(_connectionServer, "");
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");

            res = UserBase.DeleteUser(null, "aaa");
            Assert.IsFalse(res.Success, "Null ConnectionServer object should fail");

        }

        /// <summary>
        /// Large test that adds a temporary user, edits/reads many items and then deletes them.
        /// When working in a "live" data setup like this it's necessary to have all these editing tests in one spot so we can create a 
        /// single temporary user and then remove them.
        /// </summary>
        [TestMethod()]
        public void AddEditRemoveUser()
        {
            WebCallResult res;

            UserFull oUser;

            //create new list with GUID in the name to ensure uniqueness
            String strUserAlias = "TempUser_" + Guid.NewGuid().ToString().Replace("-", "");

            //generate a random number and tack it onto the end of some zeros so we're sure to avoid any legit numbers on the system.
            Random random = new Random();
            int iExtPostfix = random.Next(100000, 999999);
            string strExtension = "000000" + iExtPostfix.ToString();

            //use a bogus extension number that's legal but non dialable to avoid conflicts
            res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", strUserAlias, strExtension, null, out oUser);
            Assert.IsTrue(res.Success, "Failed creating temporary user:" + res.ToString());

            //now that the user is created, go do some message tests.
            UserMessageTests(oUser.ObjectId);

            //now run and do some MWI tests
            MwiTests(oUser.ObjectId, oUser.MediaSwitchObjectId);

            //fetch voice name - this should fail
            res = oUser.GetVoiceName("temp.wav");
            Assert.IsFalse(res.Success, "Newly created user should have no voice name, fetch should fail");

            //update a user property
            oUser.ClearPendingChanges();
            res = oUser.Update();
            Assert.IsFalse(res.Success, "Updating a user with no pending changes should fail");

            //fill in the user with some items not populated on a newly created user.
            oUser.FirstName = "FirstName";
            oUser.LastName = "LastName";
            oUser.Department = "Department";
            oUser.Title = "Title";
            oUser.State = "State";
            oUser.Manager = "Manager";
            oUser.PostalCode = "99999";
            oUser.City = "City";
            oUser.BillingId = "1234";
            oUser.XferString = "87654";
            oUser.Building = "Building";
            oUser.EmailAddress = "test@test.com";
            oUser.Initials = "abc";
            oUser.EmployeeId = "1234";
            oUser.Address = "123 Maple way";
            oUser.EnhancedSecurityAlias = "testing";

            res = oUser.Update();
            Assert.IsTrue(res.Success, "Failed to update user object:" + res.ToString());

            //reset the PIN and Password
            res = oUser.ResetPin("234098234");
            Assert.IsTrue(res.Success, "Reset PIN failed:" + res.ToString());

            res = oUser.ResetPassword("asdfasdfui09au8dsf");
            Assert.IsTrue(res.Success, "Reset password failed:" + res.ToString());

            //update the voice name
            res = oUser.SetVoiceName("Dummy.wav", true);
            Assert.IsTrue(res.Success, "Failed updating the voice name:" + res.ToString());

            //failure case for an instance method of voice name update
            res = oUser.SetVoiceNameToStreamFile("blah");
            Assert.IsFalse(res.Success, "Invalid voice name file should fail");

            //get the voice name
            res = oUser.GetVoiceName("Temp.wav");
            Assert.IsTrue(res.Success, "Failed to fetch the newly added user voice name:" + res.ToString());

            //iterate the notificaitond Devices
            foreach (NotificationDevice oDevice in oUser.NotificationDevices())
            {
                Console.WriteLine(oDevice.ToString());
                Console.WriteLine(oDevice.DumpAllProps());
            }

            //force a refetch of data
            Console.WriteLine(oUser.NotificationDevices(true).ToString());

            //Get the HomePhone notification device, edit and save it
            NotificationDevice oNotificationDevice;
            oUser.GetNotificationDevice("Home Phone", out oNotificationDevice);
            Assert.IsTrue(res.Success, "Failed to fetch HomePhone notificaiton device:" + res.ToString());

            oNotificationDevice.ClearPendingChanges();

            //saving with no pending changes should throw an error
            res = oNotificationDevice.Update();
            Assert.IsFalse(res.Success, "Update of a notificaiton device with no pending changes should fail");

            oNotificationDevice.PhoneNumber = "1234";
            res = oNotificationDevice.Update();
            Assert.IsTrue(res.Success, "Failed updating Home Phone notification device:" + res.ToString());

            //add a new SMTP notification device
            res = NotificationDevice.AddSmtpDevice(_connectionServer, oUser.ObjectId, "NewSMTPDevice", "test@test.com",
                                             NotificationEventTypes.NewUrgentVoiceMail.ToString(), true);

            Assert.IsTrue(res.Success, "Failed to add new SMTP notification device:" + res.ToString());
            Assert.IsTrue(res.ReturnedObjectId.Length > 0, "Empty objectID returned for new SMTP device creation");

            //update phone notification device - be sure to ask for an updated list to be created since we just added a device and the user
            //already can have a cached list around.
            res = oUser.GetNotificationDevice("NewSMTPDevice", out oNotificationDevice, true);
            Assert.IsTrue(res.Success, "Failed to fetch newly created SMTP notification device:" + res.ToString());

            //test failure point of updating an unedited device
            oNotificationDevice.ClearPendingChanges();
            res = oNotificationDevice.Update();
            Assert.IsFalse(res.Success, "Updating a notificaiton device with no pending changes should fail");

            oNotificationDevice.SmtpAddress = "test2@test.com";
            oNotificationDevice.StaticText = "static text";
            oNotificationDevice.FailDeviceObjectId = oNotificationDevice.ObjectId;
            res = oNotificationDevice.Update();
            Assert.IsTrue(res.Success, "Failed updating notification device:" + res.ToString());

            res = oNotificationDevice.Delete();
            Assert.IsTrue(res.Success, "Failed removing newly added SMTP notification device:" + res.ToString());

            //add an MWI

            //fail case 1
            res = UserBase.AddMwi(null, oUser.ObjectId, "112341324123", "testMWI device");
            Assert.IsFalse(res.Success, "AddMwi should fail if Comserver instance is passed as null");

            //fail case 2
            res = UserBase.AddMwi(_connectionServer, oUser.ObjectId, "", "testMWI device");
            Assert.IsFalse(res.Success, "AddMwi should fail if MWI extension is passed as blank");

            //good case
            res = UserBase.AddMwi(_connectionServer, oUser.ObjectId, "112341324123", "MWI2");
            Assert.IsTrue(res.Success, "Failed to add MWI manually:" + res.ToString());

            //add a new Phone notification device - first we need to grab any valid media switch ObjectId to use for 
            //creating the device - this will get used for both the Phone and Pager device add/removes
            List<PhoneSystem> oPhoneSystems;
            res = PhoneSystem.GetPhoneSystems(_connectionServer, out oPhoneSystems);
            Assert.IsTrue(res.Success, "Failed fetching phone systems:" + res.ToString());
            Assert.IsNotNull(oPhoneSystems, "Null phone system list returned");
            Assert.IsTrue(oPhoneSystems.Count > 0, "Empty list of phone systems returned");

            res = NotificationDevice.AddPhoneDevice(_connectionServer, oUser.ObjectId, "NewPhoneDevice", oPhoneSystems.First().ObjectId, "12345",
                                             NotificationEventTypes.NewUrgentVoiceMail.ToString(), true);
            Assert.IsTrue(res.Success, "Failed to add new Phone notification device:" + res.ToString());
            Assert.IsTrue(res.ReturnedObjectId.Length > 0, "Empty objectID returned for new Phone device creation");

            //update phone notification device - be sure to ask for an updated list to be created since we just added a device and the user
            //already can have a cached list around.
            res = oUser.GetNotificationDevice("NewPhoneDevice", out oNotificationDevice, true);
            Assert.IsTrue(res.Success, "Failed to fetch newly created phone notification device");

            oNotificationDevice.AfterDialDigits = "123";
            res = oNotificationDevice.Update();
            Assert.IsTrue(res.Success, "Failed updating notification device:" + res.ToString());

            //delete the newly added notifiication device
            res = oNotificationDevice.Delete();
            Assert.IsTrue(res.Success, "Failed removing newly added Phone notification device:" + res.ToString());

            //add a new Pager notification device
            res = NotificationDevice.AddPagerDevice(_connectionServer, oUser.ObjectId, "NewPagerDevice", oPhoneSystems.First().ObjectId,
                                             "12345", NotificationEventTypes.NewUrgentVoiceMail.ToString(), true);

            Assert.IsTrue(res.Success, "Failed to add new Pager notification device:" + res.ToString());
            Assert.IsTrue(res.ReturnedObjectId.Length > 0, "Empty objectID returned for new Pager device creation");

            //delete the newly added notifiication device
            res = NotificationDevice.DeleteNotificationDevice(_connectionServer, oUser.ObjectId, res.ReturnedObjectId,
                                                        NotificationDeviceTypes.Pager);
            Assert.IsTrue(res.Success, "Failed removing newly added Pager notification device:" + res.ToString());

            //error case - invalid notification device name
            res = oUser.GetNotificationDevice("blah", out oNotificationDevice);
            Assert.IsFalse(res.Success, "Invalid notification device name should result in an error");

            //Add an alternate extension
            res = AlternateExtension.AddAlternateExtension(_connectionServer, oUser.ObjectId, 1, strExtension + "1");
            Assert.IsTrue(res.Success, "Failed adding alternate extension to user:" + res.ToString());

            //Iterate the alternate extensiosn
            foreach (AlternateExtension oExt in oUser.AlternateExtensions(true))
            {
                Console.WriteLine(oExt.ToString());
                Console.WriteLine(oExt.DumpAllProps());
            }

            AlternateExtension oAltExt;

            //alt extension that doesn't exist should fail
            res = oUser.GetAlternateExtension(5, out oAltExt);
            Assert.IsFalse(res.Success, "Invalid alternate extension ID should fail to fetch");

            res = oUser.GetAlternateExtension(1, out oAltExt, true);
            Assert.IsTrue(res.Success, "Failed to fetch alternate extension added to new user:" + res.ToString());

            oAltExt.ClearPendingChanges();

            //update it with no outstanding items should fail
            res = oAltExt.Update();
            Assert.IsFalse(res.Success, "Updating an alternate extension with no pending changes should fail");

            //edit it
            oAltExt.DtmfAccessId = strExtension + "2";
            res = oAltExt.Update();
            Assert.IsTrue(res.Success, "Failed to update alternate extension added:" + res.ToString());

            //delete it
            res = oAltExt.Delete();
            Assert.IsTrue(res.Success, "Failed to delete alternate extension:" + res.ToString());

            //add alternate extension through alternate route via static method with return via out param

            res = AlternateExtension.AddAlternateExtension(_connectionServer, oUser.ObjectId, 1, strExtension + "3" + "2", out oAltExt);
            Assert.IsTrue(res.Success, "Failed adding alternate extension:" + res.ToString());

            res = oAltExt.RefetchAlternateExtensionData();
            Assert.IsTrue(res.Success, "Failed to refresh alternate extension:" + res.ToString());

            //get the alternate extension via alternative static method route - we'll cheat a bit here and just pass the 
            //ObjectId of the guy we just created for fetching - just need to exercise the code path
            res = AlternateExtension.GetAlternateExtension(_connectionServer, oUser.ObjectId, oAltExt.ObjectId,
                                                           out oAltExt);
            Assert.IsTrue(res.Success, "Failed to fetch newly created alternate extension:" + res.ToString());

            //one last alternative fetching code path here - create an alternate extension object and then fetch it as an 
            //instance method
            AlternateExtension oAltExt2 = new AlternateExtension(_connectionServer, oUser.ObjectId);
            Assert.IsNotNull(oAltExt2, "Failed to create new instance of an alternate extension");

            //some static method failures for alternate extenions

            //AddAlternateExtension
            res = AlternateExtension.AddAlternateExtension(null, "bogus", 1, "1234");
            Assert.IsFalse(res.Success,"Adding alternate extension with static AddAlternateExtension did not fail with null Connection server");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "bogus", 1, "1234");
            Assert.IsFalse(res.Success, "Adding alternate extension with static AddAlternateExtension did not fail with invalid objectId");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "", 1, "1234");
            Assert.IsFalse(res.Success, "Adding alternate extension with static AddAlternateExtension did not fail with blank objectId");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "bogus", 49, "1234");
            Assert.IsFalse(res.Success, "Adding alternate extension with static AddAlternateExtension did not fail with invalid index number");

            //DeleteAlternateExtension
            res = AlternateExtension.DeleteAlternateExtension(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Adding alternate extension with static DeleteAlternateExtension did not fail with null ConnectionServer");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Adding alternate extension with static DeleteAlternateExtension did not fail with invalid objectId");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Adding alternate extension with static DeleteAlternateExtension did not fail with blank objectId");

            //GetAlternateExtension
            res = AlternateExtension.GetAlternateExtension(null, "bogus", "bogus", out oAltExt);
            Assert.IsFalse(res.Success, "Getting alternate extension with static GetAlternateExtension did not fail with null Connection server");

            res = AlternateExtension.GetAlternateExtension(_connectionServer, "bogus", "bogus", out oAltExt);
            Assert.IsFalse(res.Success, "Getting alternate extension with static GetAlternateExtension did not fail with invalid objectId");

            res = AlternateExtension.GetAlternateExtension(_connectionServer, "", "bogus", out oAltExt);
            Assert.IsFalse(res.Success, "Getting alternate extension with static GetAlternateExtension did not fail with blank objectId");

            //GetAlternateExtensions
            List<AlternateExtension> oAltExts;
            res = AlternateExtension.GetAlternateExtensions(null, "bogus", out oAltExts);
            Assert.IsFalse(res.Success, "Getting alternate extensions with static GetAlternateExtensions did not fail with null Connection server");
            
           
            res = AlternateExtension.GetAlternateExtensions(_connectionServer, "", out oAltExts);
            Assert.IsFalse(res.Success, "Getting alternate extensions with static GetAlternateExtensions did not fail with empty objectId");

            //UpdateAlternateExtension
            res = AlternateExtension.UpdateAlternateExtension(null, "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "updating alternate extensions with static UpdateAlternateExtension did not fail with null Connection server");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "updating alternate extensions with static UpdateAlternateExtension did not fail with empty property list");

            //Reset the user's password
            res = oUser.ResetPassword("ASDF234232sdf", false, false, false, true);
            Assert.IsTrue(res.Success, "Resetting user password failed:"+res.ToString());

            //Reset the user's PIN
            res = oUser.ResetPin("2349808", false, false, false, true,true);
            Assert.IsTrue(res.Success, "Resetting user PIN failed:" + res.ToString());

            //reset the pin failure
            res = UserBase.ResetUserPin(null, oUser.ObjectId, "1323424323");
            Assert.IsFalse(res.Success, "Resetting user PIN with null ConnectionServer should fail");

            //remove the user
            res = oUser.Delete();
            Assert.IsTrue(res.Success, "Error removing test user:" + res.ToString());
        }


        private void UserMessageTests(string pUserObjectId)
        {
            List<UserMessage> oMessages;

            WebCallResult res = UserMessage.GetMessages(_connectionServer, pUserObjectId, out oMessages);
            Assert.IsTrue(res.Success,"Failed fetching messages on new user");
            Assert.IsTrue(oMessages.Count==0,"Test user account is reporting more than 0 messages");

            //static method failures

            //GetmessageAttachment
            res = UserMessage.GetMessageAttachment(_connectionServer, "temp.wav", "bogus", "bogus", 1);
            Assert.IsFalse(res.Success,"Call to static GetMessageAttachment did not fail with invalid user and message ObjectIds");

            res = UserMessage.GetMessageAttachment(_connectionServer, "", "bogus", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with blank local file target ");

            res = UserMessage.GetMessageAttachment(_connectionServer, "temp.wav", "", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with blank user ObjectId");

            res = UserMessage.GetMessageAttachment(_connectionServer, "temp.wav", "bogus", "", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with blank message objectId");

            res = UserMessage.GetMessageAttachment(null, "temp.wav", "bogus", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with null Connection server");

            //GetMessageAttachmentCount
            int iCount;
            res = UserMessage.GetMessageAttachmentCount(null, "bogus", "bogus", out iCount);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachmentCount did not fail with null Connection server");

            res = UserMessage.GetMessageAttachmentCount(_connectionServer, "bogus", "bogus", out iCount);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachmentCount did not fail with invalid objectIds");

            res = UserMessage.GetMessageAttachmentCount(_connectionServer, "", "bogus", out iCount);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachmentCount did not fail with empty objectId");

            //GetMessages
            res = UserMessage.GetMessages(null, "bogus", out oMessages);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with null Connection server");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages,1,10,MessageSortOrder.URGENT_FIRST,MessageFilter.Dispatch_False);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId1");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.OLDEST_FIRST,MessageFilter.Dispatch_True);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId2");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST,MessageFilter.Priority_Low);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId3");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST, MessageFilter.Priority_Normal);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId4");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST, MessageFilter.Priority_Urgent);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId5");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST, MessageFilter.Read_False);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId6");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST, MessageFilter.Read_True);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId7");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST, MessageFilter.Type_Email);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId8");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST, MessageFilter.Type_Fax);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId9");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST, MessageFilter.Type_Receipt);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId10");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST, MessageFilter.Type_Voice);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId11");

            
            Console.WriteLine(UserMessage.ConvertFromMillisecondsToTimeDate(1000, true).ToString());
            Console.WriteLine(UserMessage.ConvertFromMillisecondsToTimeDate(1000, false).ToString());
            
            Console.WriteLine(UserMessage.ConvertFromTimeDateToMilliseconds(DateTime.Now, true));
            Console.WriteLine(UserMessage.ConvertFromTimeDateToMilliseconds(DateTime.Now, false));

            UserMessage oMessage = new UserMessage(_connectionServer,pUserObjectId);

            Console.WriteLine(oMessage.ToString());
            Console.WriteLine(oMessage.DumpAllProps());

            try
            {
                UserMessage oNewMessage = new UserMessage(null,pUserObjectId);
                Assert.Fail("Creating new UserMessage object with null Connection server did not fail");
            }
            catch {}

            try
            {
                UserMessage oNewMessage = new UserMessage(_connectionServer, "");
                Assert.Fail("Creating new UserMessage object with and empty user ObjectID did not fail");
            }
            catch { }

        }


        private void MwiTests(string pUserObjectId, string pMediaSwitchObjectId)
        {
            //static method calls - create, fetch then delete an MWI device

            WebCallResult res = Mwi.AddMwi(_connectionServer, pUserObjectId, "display name", pMediaSwitchObjectId, "1234321", false);
            Assert.IsTrue(res.Success,"Failed to create MWI device for user:"+res);

            Mwi oMwiDevice;
            res = Mwi.GetMwiDevice(_connectionServer, pUserObjectId, res.ReturnedObjectId, out oMwiDevice);
            Assert.IsTrue(res.Success, "Failed to fetch single MWI device just added for user:" + res);

            Console.WriteLine(oMwiDevice.ToString());
            Console.WriteLine(oMwiDevice.DumpAllProps());

            res = oMwiDevice.Update();
            Assert.IsFalse(res.Success, "Calling update on MWi device with no pending changes did not fail");
            
            oMwiDevice.DisplayName = "Updated Display Name";
            oMwiDevice.IncludeFaxMessages = true;

            res = oMwiDevice.Update();
            Assert.IsTrue(res.Success,"Updating MWI properties failed:"+res);

            List<Mwi> oMwis;
            res = Mwi.GetMwiDevices(_connectionServer, pUserObjectId, out oMwis);
            Assert.IsTrue(res.Success, "Failed to fetch MWI device for user:" + res);
            Assert.IsTrue(oMwis.Count == 2, "Mwi count is not 2 after adding 2nd device");

            res = Mwi.DeleteMwiDevice(_connectionServer, pUserObjectId, oMwiDevice.ObjectId);
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
    }
}
