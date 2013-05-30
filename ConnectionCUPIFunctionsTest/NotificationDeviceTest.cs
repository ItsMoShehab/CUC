using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for NotificationDeviceTest and is intended
    ///to contain all NotificationDeviceTest Unit Tests
    ///</summary>
    [TestClass]
    public class NotificationDeviceTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        //class wide user reference for testing - gets filled in with operator user details
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
                throw new Exception("Unable to attach to Connection server to start NotificationDevice test:" + ex.Message);
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
                Assert.IsTrue(res.Success, "Failed to delete temporary user on cleanup.");
            }
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            NotificationDevice oTemp = new NotificationDevice(null, "aaa");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// make sure an argumentException is thrown if an empty user objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure4()
        {
            NotificationDevice oTemp = new NotificationDevice(_connectionServer, "");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid objectId.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            NotificationDevice oTemp = new NotificationDevice(_connectionServer, "bogus","bogus");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid name passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            NotificationDevice oTemp = new NotificationDevice(_connectionServer, "bogus","","bogus");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticCallFailure_AddSmtpDevice()
        {
            var res = NotificationDevice.AddSmtpDevice(null, "objectId", "device name", "address@fun.com", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with null ConnectionServerRest did not fail");

            res = NotificationDevice.AddSmtpDevice(_connectionServer, "objectId", "device name", "address@fun.com", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with null event type did not fail");

            res = NotificationDevice.AddSmtpDevice(_connectionServer, "", "device name", "address@fun.com", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with empty objectId did not fail");

            res = NotificationDevice.AddSmtpDevice(_connectionServer, "objectId", "", "address@fun.com", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with empty name did not fail");

            res = NotificationDevice.AddSmtpDevice(_connectionServer, "objectId", "device name", "", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with empty address did not fail");

            res = NotificationDevice.AddSmtpDevice(_connectionServer, "objectId", "device name", "address@fun.com", "bogus", true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with invalid trigger type did not fail");

        }


        [TestMethod]
        public void StaticCallFailure_AddHtmlDevice()
        {
            var res = NotificationDevice.AddHtmlDevice(null, "objectid", "templateid", "device name", "address@fun.com",
                                                       "event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with null Connection server did not fail");

            res = NotificationDevice.AddHtmlDevice(_connectionServer, "", "templateid", "device name", "address@fun.com",
                                           "event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with empty objectId did not fail");

            res = NotificationDevice.AddHtmlDevice(_connectionServer, "objectid", "", "device name", "address@fun.com",
                               "event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with empty templateID did not fail");

            res = NotificationDevice.AddHtmlDevice(_connectionServer, "objectid", "templateid", "", "address@fun.com",
                               "event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with empty name did not fail");

            res = NotificationDevice.AddHtmlDevice(_connectionServer, "objectid", "templateid", "device name", "","event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with empty address did not fail");

            res = NotificationDevice.AddHtmlDevice(_connectionServer, "objectid", "templateid", "device name", "address@fun.com",
                               "Bogus", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with bogus device trigger type did not fail");

        }

        [TestMethod]
        public void StaticCallFailure_AddSmsDevice()
        {
            var res= NotificationDevice.AddSmsDevice(null, "objectid", "devicename", "providerID", "recipient@fun.com",
                                            "sender@fun.com", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with null Connection server did not fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, "", "devicename", "providerID", "recipient@fun.com",
                                            "sender@fun.com", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with empty objectid did not fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, "objectid", "", "providerID", "recipient@fun.com",
                                "sender@fun.com", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with empty device name did not fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, "objectid", "devicename", "", "recipient@fun.com",
                                "sender@fun.com", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with empty providerID did not fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, "objectid", "devicename", "providerID", "",
                                "sender@fun.com", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with empty recipient address did not fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, "objectid", "devicename", "providerID", "recipient@fun.com",
                                "", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with empty senderaddress did not fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, "objectid", "devicename", "providerID", "recipient@fun.com",
                                "sender@fun.com", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with invalid event type trigger did not fail");
        }


        [TestMethod]
        public void StaticCallFailure_AddPhoneDevice()
        {
            var res = NotificationDevice.AddPhoneDevice(null, "objectid", "devicename", "mediaswitchid", "234234",
                                                        "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with null connection server did not fail");

            res = NotificationDevice.AddPhoneDevice(_connectionServer, "", "devicename", "mediaswitchid", "234234",
                                                        "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with empty objectId did not fail");

            res = NotificationDevice.AddPhoneDevice(_connectionServer, "objectid", "", "mediaswitchid", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with empty device name did not fail");

            res = NotificationDevice.AddPhoneDevice(_connectionServer, "objectid", "devicename", "", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with empty mediaswitchid did not fail");

            res = NotificationDevice.AddPhoneDevice(_connectionServer, "objectid", "devicename", "mediaswitchid", "",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with empty phone number did not fail");

            res = NotificationDevice.AddPhoneDevice(_connectionServer, "objectid", "devicename", "mediaswitchid", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with invalid media even type trigger did not fail");
        }

        [TestMethod]
        public void StaticCallFailure_AddPagerDevice()
        {
            var res = NotificationDevice.AddPagerDevice(null, "objectid", "devicename", "mediaswitchid", "234234",
                                                        "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with null Connection server did not fail");

            res = NotificationDevice.AddPagerDevice(_connectionServer, "", "devicename", "mediaswitchid", "234234",
                                                        "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with empty objectId did not fail");

            res = NotificationDevice.AddPagerDevice(_connectionServer, "objectid", "", "mediaswitchid", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with empty device name did not fail");

            res = NotificationDevice.AddPagerDevice(_connectionServer, "objectid", "devicename", "", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with empty mediaswitchid did not fail");

            res = NotificationDevice.AddPagerDevice(_connectionServer, "objectid", "devicename", "mediaswitchid", "",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with empty phone number did not fail");

            res = NotificationDevice.AddPagerDevice(_connectionServer, "objectid", "devicename", "mediaswitchid", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with invalid eventID did not fail");
        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_GetNotificationDevices()
        {
            List<NotificationDevice> oDevices;

            //get Notification Device list failure points.
            WebCallResult res = NotificationDevice.GetNotificationDevices(null, _tempUser.ObjectId, out oDevices);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = NotificationDevice.GetNotificationDevices(_connectionServer, "", out oDevices);
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail.");

            res = NotificationDevice.GetNotificationDevices(_connectionServer, "aaa", out oDevices);
            Assert.IsTrue(res.Success, "Fetching notification devices with invalid name should not fail.");
            Assert.IsFalse(oDevices == null, "Invalid UserObjectID fetch returned null devices list.");
            Assert.IsTrue(oDevices.Count == 0, "Invalid UserObjectID fetch returned one or more devices.");
        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_DeleteNotificationDevices()
        {
            //failure paths for delete device calls
            WebCallResult res = NotificationDevice.DeleteNotificationDevice(null, _tempUser.ObjectId, "aaa",
                                                                            NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");

            res = NotificationDevice.DeleteNotificationDevice(_connectionServer, "", "aaa",
                                                  NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Empty UserobjectID should fail");

            res = NotificationDevice.DeleteNotificationDevice(_connectionServer, _tempUser.ObjectId, "aaa",
                                                  NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Invalid device objectID should fail");

            res = NotificationDevice.DeleteNotificationDevice(_connectionServer, _tempUser.ObjectId, "",
                                                  NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Empty device objectID should fail");

            Assert.IsFalse(res.Success, "Empty device objectID should fail");
            

        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_UpdateNotificationDevices()
        {
            //failure paths for update calls
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("item", "value");

            WebCallResult res = NotificationDevice.UpdateNotificationDevice(null, _tempUser.ObjectId, "aaa",
                                                                            NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");

            res = NotificationDevice.UpdateNotificationDevice(_connectionServer, "", "aaa",
                                                              NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Empty user objectId should fail");

            res = NotificationDevice.UpdateNotificationDevice(_connectionServer, _tempUser.ObjectId, "aaa",
                                                  NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Invalid device objectId should fail");

            res = NotificationDevice.UpdateNotificationDevice(_connectionServer, _tempUser.ObjectId, "",
                                                  NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Empty device objectID should fail");

            res = NotificationDevice.UpdateNotificationDevice(_connectionServer, _tempUser.ObjectId, "aaa",
                                                  NotificationDeviceTypes.Pager, null);
            Assert.IsFalse(res.Success, "Empty prop list should fail");

        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_AddNotificationDevice()
        {
            //since we can't add an SMS device without a provider (which we can't dummy up) just hit the failure routes here and call
            //it good.
            WebCallResult res = NotificationDevice.AddSmsDevice(null, _tempUser.ObjectId, "SMSDevice", "aaa",
                                                                "recipient@test.com", "Sender@test.com",
                                                                NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, "", "SMSDevice", "aaa",
                                      "recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty user ObjectId param should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _tempUser.ObjectId, "", "aaa",
                                      "recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty device name should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _tempUser.ObjectId, "SMSDevice", "",
                                                  "recipient@test.com", "Sender@test.com",
                                                  NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty SMPP provider Id should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _tempUser.ObjectId, "SMSDevice", "aaa",
                                      "recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Invalid SMPP provider Id should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _tempUser.ObjectId, "SMSDevice", "aaa",
                                      "", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty recipient parameter should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _tempUser.ObjectId, "SMSDevice", "aaa",
                                      "recipient@test.com", "",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty sender parameter should fail");

        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void StaticCallFailure_GetNotificationDevice()
        {
            NotificationDevice oDevice;
            List<NotificationDevice> oDevices;

            //get the notificaiton devices for the operator
            WebCallResult res = NotificationDevice.GetNotificationDevices(_connectionServer, _tempUser.ObjectId, out oDevices);
            Assert.IsTrue(res.Success, "Failed to fetch notification devices for operator:" + res);

            //fetch the single device returned as the first in the list from the last test
            res = NotificationDevice.GetNotificationDeivce(_connectionServer, _tempUser.ObjectId, oDevices.First().ObjectId, "", out oDevice);
            Assert.IsTrue(res.Success, "Failed to fetch notification device for operator:" + res);

            //now check some failure points.
            res = NotificationDevice.GetNotificationDeivce(null, _tempUser.ObjectId, oDevices.First().ObjectId, "", out oDevice);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = NotificationDevice.GetNotificationDeivce(_connectionServer, _tempUser.ObjectId, "", "", out oDevice);
            Assert.IsFalse(res.Success, "Empty objectID should fail");

            res = NotificationDevice.GetNotificationDeivce(_connectionServer, _tempUser.ObjectId, "aaa", "", out oDevice);
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
            var res= NotificationDevice.GetNotificationDeivce(_connectionServer,_tempUser.ObjectId,"", oDevices[0].DisplayName,out oDevice);
            Assert.IsTrue(res.Success,"Failed to fetch notificaiton device by name:"+res);
        }

        [TestMethod]
        public void NotificationDevice_AddHtmlDevice()
        {
            List<NotificationTemplate> oHtmlTemplates;
            var res = NotificationTemplate.GetNotificationTemplates(_connectionServer, out oHtmlTemplates, 1, 1);
            Assert.IsTrue(res.Success, "Failed to fetch notification templates:" + res);
            Assert.IsTrue(oHtmlTemplates.Count > 0, "No HTML templates found");

            res = NotificationDevice.AddHtmlDevice(_connectionServer, _tempUser.ObjectId,
                                                   oHtmlTemplates[0].NotificationTemplateId,
                                                   Guid.NewGuid().ToString(), "blah@fun.com","NewVoiceMail" , true);
            Assert.IsTrue(res.Success, "Failed to create new HTML notification device:" + res);

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
