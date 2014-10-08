using System.Collections.Generic;
using System.Linq;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for NotificationDeviceIntegrationTests and is intended
    ///to contain all NotificationDeviceIntegrationTests Unit Tests
    ///</summary>
    [TestClass]
    public class NotificationDeviceIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide user reference for testing - gets filled in with operator user details
        private static UserFull _tempUser;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            //create new list with GUID in the name to ensure uniqueness
            String strUserAlias = "TempUserNotDev_" + Guid.NewGuid().ToString().Replace("-", "");

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
                Assert.IsTrue(res.Success, "Failed to delete temporary user on cleanup.");
            }
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid objectId.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            NotificationDevice oTemp = new NotificationDevice(_connectionServer, "bogus","bogus");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid name passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidName_Failure()
        {
            NotificationDevice oTemp = new NotificationDevice(_connectionServer, "bogus","","bogus");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void AddSmtpDevice_InvalidEventTriggerType_Failure()
        {
            var res = NotificationDevice.AddSmtpDevice(_connectionServer, "objectId", "device name", "address@fun.com", "bogusevent", true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with invalid trigger type did not fail");
        }


        [TestMethod]
        public void AddHtmlDevice_InvalidEventTriggerType_Failure()
        {
            var res = NotificationDevice.AddHtmlDevice(_connectionServer, "objectid", "templateid", "device name", "address@fun.com",
                               "bogusevent", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with bogus device trigger type did not fail");
        }


        [TestMethod]
        public void AddSmsDevice_InvalidEventTriggerType_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_connectionServer, "objectid", "devicename", "providerID", "recipient@fun.com",
                                "sender@fun.com", "bogusevent", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with invalid event type trigger did not fail");
        }


        [TestMethod]
        public void AddPhoneDevice_InvalidEventTriggerType_Failure()
        {
            var res = NotificationDevice.AddPhoneDevice(_connectionServer, "objectid", "devicename", "mediaswitchid", "234234",
                                            "bogusevent", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with invalid media even type trigger did not fail");
        }

        [TestMethod]
        public void AddPagerDevice_InvalidEventTriggerType_Failure()
        {
            var res = NotificationDevice.AddPagerDevice(_connectionServer, "objectid", "devicename", "mediaswitchid", "234234",
                                            "bogusevent", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with invalid eventID did not fail");
        }


        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void GetNotificationDevices_InvalidObjectId_Success()
        {
            List<NotificationDevice> oDevices;

            var res = NotificationDevice.GetNotificationDevices(_connectionServer, "aaa", out oDevices);
            Assert.IsTrue(res.Success, "Fetching notification devices with invalid name should not fail.");
            Assert.IsFalse(oDevices == null, "Invalid UserObjectID fetch returned null devices list.");
            Assert.IsTrue(oDevices.Count == 0, "Invalid UserObjectID fetch returned one or more devices.");
        }

        [TestMethod]
        public void DeleteNotificationDevice_InvalidObjectId_Failure()
        {
            var res = NotificationDevice.DeleteNotificationDevice(_connectionServer, _tempUser.ObjectId, "aaa",
                                                                  NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Invalid device objectID should fail");

        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void UpdateNotificationDevice_InvalidObjectId_Failure()
        {
            //failure paths for update calls
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("item", "value");

            var res = NotificationDevice.UpdateNotificationDevice(_connectionServer, _tempUser.ObjectId, "aaa",
                                                  NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Invalid device objectId should fail");
        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void AddSmsDevice_InvalidSmppProvider_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_connectionServer, _tempUser.ObjectId, "SMSDevice", "aaa",
                                      "recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Invalid SMPP provider Id should fail");
        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void GetNotificationDevices_Success()
        {
            List<NotificationDevice> oDevices;

            //get the notificaiton devices for the operator
            WebCallResult res = NotificationDevice.GetNotificationDevices(_connectionServer, _tempUser.ObjectId,out oDevices);
            Assert.IsTrue(res.Success, "Failed to fetch notification devices for operator:" + res);
        }

        [TestMethod]
        public void GetNotificationDevice_Success()
        {
            NotificationDevice oDevice;
            List<NotificationDevice> oDevices;

            WebCallResult res = NotificationDevice.GetNotificationDevices(_connectionServer, _tempUser.ObjectId, out oDevices);
            Assert.IsTrue(res.Success, "Failed to fetch notification devices for operator:" + res);

            //fetch the single device returned as the first in the list from the last test
            res = NotificationDevice.GetNotificationDevice(_connectionServer, _tempUser.ObjectId, oDevices.First().ObjectId, "", out oDevice);
            Assert.IsTrue(res.Success, "Failed to fetch notification device for operator:" + res);

            }

        [TestMethod]
        public void GetNotificationDevice_InvalidObjectId_Failure()
        {
            NotificationDevice oDevice;

            var res = NotificationDevice.GetNotificationDevice(_connectionServer, _tempUser.ObjectId, "aaa", "", out oDevice);
            Assert.IsFalse(res.Success, "Invalid objectID should fail");
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void NotificationDevice_FetchTests()
        {
            List<NotificationDevice> oDevices = _tempUser.NotificationDevices(true);
            Assert.IsNotNull(oDevices, "Failted to get devices from user");
            Assert.IsNotNull(oDevices, "Null device list returned from user");
            Assert.IsTrue(oDevices.Count>0,"No devices returned from user");
            Console.WriteLine(oDevices[0].ToString());
            Console.WriteLine(oDevices[0].DumpAllProps());

            NotificationDevice oDevice;
            var res= NotificationDevice.GetNotificationDevice(_connectionServer,_tempUser.ObjectId,"", oDevices[0].DisplayName,out oDevice);
            Assert.IsTrue(res.Success,"Failed to fetch notificaiton device by name:"+res);
        }

        [TestMethod]
        public void NotificationDevice_AddHtmlDevice()
        {
            List<NotificationTemplate> oHtmlTemplates;
            var res = NotificationTemplate.GetNotificationTemplates(_connectionServer, out oHtmlTemplates, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch notification templates:" + res);
            Assert.IsTrue(oHtmlTemplates.Count > 0, "No HTML templates found");

            string strDisplayName = Guid.NewGuid().ToString();
            res = NotificationDevice.AddHtmlDevice(_connectionServer, _tempUser.ObjectId,
                                                   oHtmlTemplates[0].NotificationTemplateId,
                                                   strDisplayName, "blah@fun.com","NewVoiceMail" , true);
            Assert.IsTrue(res.Success, "Failed to create new HTML notification device:" + res);

            NotificationDevice oDevice;
            res = NotificationDevice.GetNotificationDevice(_connectionServer, _tempUser.ObjectId, "", strDisplayName,out oDevice);
            Assert.IsTrue(res.Success, "Failed to fetch newly created HTML notification device:" + res);

            res= oDevice.Delete();
            Assert.IsTrue(res.Success, "Failed to delete new HTML notification device:" + res);
        }

        [TestMethod]
        public void NotificationDevice_AddPagerDevice()
        {
            var res = NotificationDevice.AddPagerDevice(_connectionServer, _tempUser.ObjectId, Guid.NewGuid().ToString(),
                                                    _tempUser.MediaSwitchObjectId, "234234", "NewUrgentFax", false);
            Assert.IsTrue(res.Success,"Failed to create new pager notification device");
        }


        [TestMethod]
        public void NotificationDevice_AddPhoneDevice()
        {
            var res = NotificationDevice.AddPhoneDevice(_connectionServer, _tempUser.ObjectId, Guid.NewGuid().ToString(),
                                                    _tempUser.MediaSwitchObjectId, "234234","NewUrgentVoiceMail", false);
            Assert.IsTrue(res.Success, "Failed to create new phone notification device");
        }

        [TestMethod]
        public void NotificationDevice_AddSmsDevice()
        {
            List<SmppProvider> oProviders;
            var res = SmppProvider.GetSmppProviders(_connectionServer, out oProviders);
            Assert.IsTrue(res.Success,"Failed fetching SMPP providers:"+res);
            Assert.IsTrue(oProviders.Count>0,"No SMPP providers found");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _tempUser.ObjectId, Guid.NewGuid().ToString(),
                                                      oProviders[0].ObjectId, "test@fun.com", "test@fun.com",
                                                      "DispatchMessage", true);

            Assert.IsTrue(res.Success, "Failed to create new SMS notification device:"+res);
        }

        [TestMethod]
        public void NotificationDevice_AddSmtpDevice()
        {
            var res = NotificationDevice.AddSmtpDevice(_connectionServer, _tempUser.ObjectId, Guid.NewGuid().ToString(),
                                                       "test@fun.com", "AllMessage", true);
            Assert.IsTrue(res.Success,"Failed to create new SMTP device:"+res);
        }

        #endregion


    }
}
