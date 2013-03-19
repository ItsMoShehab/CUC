using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for NotificationDeviceTest and is intended
    ///to contain all NotificationDeviceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NotificationDeviceTest
    {

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        //class wide user reference for testing - gets filled in with operator user details
        private static UserBase _user;

        private TestContext testContextInstance;

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
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

            //get the operator to work with here
            try
            {
                _user = new UserBase(_connectionServer, "", "operator");
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to get operator user for testing:"+ex.Message);
            }

        }

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            NotificationDevice oTemp = new NotificationDevice(null, "aaa");
        }

        [TestMethod]
        public void NotificationDevice_Test()
        {
            List<NotificationDevice> oDevices;

            oDevices = _user.NotificationDevices(true);
            Assert.IsNotNull(oDevices, "Failted to get devices from user");
            Assert.IsNotNull(oDevices, "Null device list returned from user");

            foreach (NotificationDevice oDevice in oDevices)
            {
                Console.WriteLine(oDevice.ToString());
                Console.WriteLine(oDevice.DumpAllProps());
            }

        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void GetNotificationDevices_Failure()
        {
            WebCallResult res;
            List<NotificationDevice> oDevices;

            //get Notification Device list failure points.
            res = NotificationDevice.GetNotificationDevices(null, _user.ObjectId, out oDevices);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = NotificationDevice.GetNotificationDevices(_connectionServer, "", out oDevices);
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail.");

            res = NotificationDevice.GetNotificationDevices(_connectionServer, "aaa", out oDevices);
            Assert.IsFalse(res.Success, "Invalid UserObjectID should fail.");

        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void DeleteNotificationDevices_Failure()
        {
            WebCallResult res;

            //failure paths for delete device calls
            res = NotificationDevice.DeleteNotificationDevice(null, _user.ObjectId, "aaa",
                                                  NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");

            res = NotificationDevice.DeleteNotificationDevice(_connectionServer, "", "aaa",
                                                  NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Empty UserobjectID should fail");

            res = NotificationDevice.DeleteNotificationDevice(_connectionServer, _user.ObjectId, "aaa",
                                                  NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Invalid device objectID should fail");

            res = NotificationDevice.DeleteNotificationDevice(_connectionServer, _user.ObjectId, "",
                                                  NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Empty device objectID should fail");

        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void UpdateNotificationDevices_Failure()
        {
            WebCallResult res;
            //failure paths for update calls
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("item", "value");

            res = NotificationDevice.UpdateNotificationDevice(null, _user.ObjectId, "aaa",
                                                              NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");

            res = NotificationDevice.UpdateNotificationDevice(_connectionServer, "", "aaa",
                                                              NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Empty user objectId should fail");

            res = NotificationDevice.UpdateNotificationDevice(_connectionServer, _user.ObjectId, "aaa",
                                                  NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Invalid device objectId should fail");

            res = NotificationDevice.UpdateNotificationDevice(_connectionServer, _user.ObjectId, "",
                                                  NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Empty device objectID should fail");

            res = NotificationDevice.UpdateNotificationDevice(_connectionServer, _user.ObjectId, "aaa",
                                                  NotificationDeviceTypes.Pager, null);
            Assert.IsFalse(res.Success, "Empty prop list should fail");

        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void AddNotificationDevice_Failure()
        {
            WebCallResult res;
            //since we can't add an SMS device without a provider (which we can't dummy up) just hit the failure routes here and call
            //it good.
            res = NotificationDevice.AddSmsDevice(null, _user.ObjectId, "SMSDevice", "aaa",
                                      "recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, "", "SMSDevice", "aaa",
                                      "recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty user ObjectId param should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _user.ObjectId, "", "aaa",
                                      "recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty device name should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _user.ObjectId, "SMSDevice", "",
                                                  "recipient@test.com", "Sender@test.com",
                                                  NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty SMPP provider Id should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _user.ObjectId, "SMSDevice", "aaa",
                                      "recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Invalid SMPP provider Id should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _user.ObjectId, "SMSDevice", "aaa",
                                      "", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty recipient parameter should fail");

            res = NotificationDevice.AddSmsDevice(_connectionServer, _user.ObjectId, "SMSDevice", "aaa",
                                      "recipient@test.com", "",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty sender parameter should fail");

        }

        /// <summary>
        /// Testing notification device failure scenarios
        /// </summary>
        [TestMethod]
        public void GetNotificationDevice_Failure()
        {
            WebCallResult res;
            NotificationDevice oDevice;
            List<NotificationDevice> oDevices;

            //get the notificaiton devices for the operator
            res = NotificationDevice.GetNotificationDevices(_connectionServer, _user.ObjectId, out oDevices);
            Assert.IsTrue(res.Success, "Failed to fetch notification devices for operator");

            //fetch the single device returned as the first in the list from the last test
            res = NotificationDevice.GetNotificationDeivce(_connectionServer, _user.ObjectId, oDevices.First().ObjectId,"", out oDevice);
            Assert.IsTrue(res.Success, "Failed to fetch notification device for operator");

            //now check some failure points.
            res = NotificationDevice.GetNotificationDeivce(null, _user.ObjectId, oDevices.First().ObjectId,"", out oDevice);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = NotificationDevice.GetNotificationDeivce(_connectionServer, _user.ObjectId, "", "",out oDevice);
            Assert.IsFalse(res.Success, "Empty objectID should fail");

            res = NotificationDevice.GetNotificationDeivce(_connectionServer, _user.ObjectId, "aaa","", out oDevice);
            Assert.IsFalse(res.Success, "Invalid objectID should fail");

        }

    }
}
