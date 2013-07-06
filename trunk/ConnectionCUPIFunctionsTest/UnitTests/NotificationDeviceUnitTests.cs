using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for NotificationDeviceUnitTests and is intended
    ///to contain all NotificationDeviceUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class NotificationDeviceUnitTests : BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            NotificationDevice oTemp = new NotificationDevice(null, "aaa");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// make sure an argumentException is thrown if an empty user objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyObjectId_Failure()
        {
            NotificationDevice oTemp = new NotificationDevice(_mockServer, "");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void AddSmtpDevice_NullConnectionServer_Failure()
        {
            var res = NotificationDevice.AddSmtpDevice(null, "objectId", "device name", "address@fun.com", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void AddSmtpDevice_NullEventType_Failure()
        {
            var res = NotificationDevice.AddSmtpDevice(_mockServer, "objectId", "device name", "address@fun.com", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with null event type did not fail");
        }

        [TestMethod]
        public void AddSmtpDevice_EmptyObjectId_Failure()
        {
            var res = NotificationDevice.AddSmtpDevice(_mockServer, "", "device name", "address@fun.com", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with empty objectId did not fail");
        }

        [TestMethod]
        public void AddSmtpDevice_EmptyName_Failure()
        {
            var res = NotificationDevice.AddSmtpDevice(_mockServer, "objectId", "", "address@fun.com", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with empty name did not fail");
        }

        [TestMethod]
        public void AddSmtpDevice_EmptySmtpAddress_Failure()
        {
            var res = NotificationDevice.AddSmtpDevice(_mockServer, "objectId", "device name", "", null, true);
            Assert.IsFalse(res.Success, "Calling AddSmtpDevice with empty address did not fail");
        }

        [TestMethod]
        public void AddSmtpDevice_InvalidEventTrigger_Failure()
        {
            var res = NotificationDevice.AddSmtpDevice(_mockServer, "objectId", "device name","smtpAddress", "BogusEvent", true);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with bogus device trigger type did not fail");
        }

        [TestMethod]
        public void AddHtmlDevice_NullConnectionServer_Failure()
        {
            var res = NotificationDevice.AddHtmlDevice(null, "objectid", "templateid", "device name", "address@fun.com","event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with null Connection server did not fail");

            }

        [TestMethod]
        public void AddHtmlDevice_EmptyObjectId_Failure()
        {
            var res = NotificationDevice.AddHtmlDevice(_mockServer, "", "templateid", "device name", "address@fun.com","event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with empty objectId did not fail");

         }

        [TestMethod]
        public void AddHtmlDevice_EmptyTemplateId_Failure()
        {
            var res = NotificationDevice.AddHtmlDevice(_mockServer, "objectid", "", "device name", "address@fun.com", "event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with empty templateID did not fail");

            }

        [TestMethod]
        public void AddHtmlDevice_EmptyName_Failure()
        {
            var res = NotificationDevice.AddHtmlDevice(_mockServer, "objectid", "templateid", "", "address@fun.com", "event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with empty name did not fail");

            }

        [TestMethod]
        public void AddHtmlDevice_EmptyAddress_Failure()
        {
            var res = NotificationDevice.AddHtmlDevice(_mockServer, "objectid", "templateid", "device name", "", "event", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with empty address did not fail");

            }

        [TestMethod]
        public void AddHtmlDevice_InvalidEventTrigger_Failure()
        {
            var res = NotificationDevice.AddHtmlDevice(_mockServer, "objectid", "templateid", "device name", "address@fun.com", "BogusEvent", false);
            Assert.IsFalse(res.Success, "Calling AddHtmlDevice with bogus device trigger type did not fail");
        }


        [TestMethod]
        public void AddSmsDevice_EmptyObjectId_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "", "devicename", "providerID", "recipient@fun.com",
                                            "sender@fun.com", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with empty objectid did not fail");

            }


        [TestMethod]
        public void AddSmsDevice_EmptyProviderId_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "objectid", "devicename", "", "recipient@fun.com",
                                "sender@fun.com", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with empty providerID did not fail");

         }


        [TestMethod]
        public void AddSmsDevice_EmptyRecipientAddress_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "objectid", "devicename", "providerID", "",
                                "sender@fun.com", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with empty recipient address did not fail");

            }


        [TestMethod]
        public void AddSmsDevice_EmptySenderAddress_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "objectid", "devicename", "providerID", "recipient@fun.com",
                                "", "eventtype", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with empty senderaddress did not fail");
        }


        [TestMethod]
        public void AddSmsDevice_InvalidEventTrigger_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "objectid", "devicename", "providerID", "recipient@fun.com",
                                "sender@fun.com", "bogusevent", true);
            Assert.IsFalse(res.Success, "Calling AddSmsDevice with invalid event type trigger did not fail");
        }


        [TestMethod]
        public void AddPhoneDevice_NullConnectionServer_Failure()
        {
            var res = NotificationDevice.AddPhoneDevice(null, "objectid", "devicename", "mediaswitchid", "234234",
                                                        "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with null connection server did not fail");

           }


        [TestMethod]
        public void AddPhoneDevice_EmptyObjectId_Failure()
        {
            var res = NotificationDevice.AddPhoneDevice(_mockServer, "", "devicename", "mediaswitchid", "234234",
                                                        "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with empty objectId did not fail");

            }


        [TestMethod]
        public void AddPhoneDevice_EmptyDeviceName_Failure()
        {
            var res = NotificationDevice.AddPhoneDevice(_mockServer, "objectid", "", "mediaswitchid", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with empty device name did not fail");

            }


        [TestMethod]
        public void AddPhoneDevice_EmptyMediaSwitchId_Failure()
        {
            var res = NotificationDevice.AddPhoneDevice(_mockServer, "objectid", "devicename", "", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with empty mediaswitchid did not fail");

            }


        [TestMethod]
        public void AddPhoneDevice_EmptyPhoneNumber_Failure()
        {
            var res = NotificationDevice.AddPhoneDevice(_mockServer, "objectid", "devicename", "mediaswitchid", "",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with empty phone number did not fail");

            }


        [TestMethod]
        public void AddPhoneDevice_InvalidEventTrigger_Failure()
        {
            var res = NotificationDevice.AddPhoneDevice(_mockServer, "objectid", "devicename", "mediaswitchid", "234234",
                                            "invalidevent", true);
            Assert.IsFalse(res.Success, "Calling AddPhoneDevice with invalid media even type trigger did not fail");
        }

        [TestMethod]
        public void AddPagerDevice_NullConnectionServer_Failure()
        {
            var res = NotificationDevice.AddPagerDevice(null, "objectid", "devicename", "mediaswitchid", "234234",
                                                        "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with null Connection server did not fail");

         }

        [TestMethod]
        public void AddPagerDevice_EmptyObjectId_Failure()
        {
            var res = NotificationDevice.AddPagerDevice(_mockServer, "", "devicename", "mediaswitchid", "234234",
                                                        "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with empty objectId did not fail");

            }

        [TestMethod]
        public void AddPagerDevice_EmptyDeviceName_Failure()
        {
            var res = NotificationDevice.AddPagerDevice(_mockServer, "objectid", "", "mediaswitchid", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with empty device name did not fail");

            }

        [TestMethod]
        public void AddPagerDevice_EmptyMediaSwitchId_Failure()
        {
            var res = NotificationDevice.AddPagerDevice(_mockServer, "objectid", "devicename", "", "234234",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with empty mediaswitchid did not fail");

            }

        [TestMethod]
        public void AddPagerDevice_EmptyPhoneNumber_Failure()
        {
            var res = NotificationDevice.AddPagerDevice(_mockServer, "objectid", "devicename", "mediaswitchid", "",
                                            "event", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with empty phone number did not fail");

            }

        [TestMethod]
        public void AddPagerDevice_InvalidEventTrigger_Failure()
        {
            var res = NotificationDevice.AddPagerDevice(_mockServer, "objectid", "devicename", "mediaswitchid", "234234",
                                            "invalidevent", true);
            Assert.IsFalse(res.Success, "Calling AddPagerDevice with invalid eventID did not fail");
        }

        [TestMethod]
        public void GetNotificationDevices_NullConnectionServer_Failure()
        {
            List<NotificationDevice> oDevices;

            //get Notification Device list failure points.
            WebCallResult res = NotificationDevice.GetNotificationDevices(null, "objectid", out oDevices);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");
        }

        [TestMethod]
        public void GetNotificationDevices_EmptyUserObjectId_Failure()
        {
            List<NotificationDevice> oDevices;

            var res = NotificationDevice.GetNotificationDevices(_mockServer, "", out oDevices);
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail.");
        }

        [TestMethod]
        public void DeleteNotificationDevice_NullConnectionServer_Failure()
        {
            WebCallResult res = NotificationDevice.DeleteNotificationDevice(null, "objectid", "aaa",NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");
        }

        [TestMethod]
        public void DeleteNotificationDevice_EmptyUserObjectId_Failure()
        {
            var res = NotificationDevice.DeleteNotificationDevice(_mockServer, "", "aaa",NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Empty UserobjectID should fail");

         }

        [TestMethod]
        public void DeleteNotificationDevice_EmptyDeviceObjectId_Failure()
        {
            var res = NotificationDevice.DeleteNotificationDevice(_mockServer, "objectid", "",NotificationDeviceTypes.Sms);
            Assert.IsFalse(res.Success, "Empty device objectID should fail");
        }


        [TestMethod]
        public void UpdateNotificationDevice_NullConnectionServer_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("item", "value");

            WebCallResult res = NotificationDevice.UpdateNotificationDevice(null, "objectid", "aaa",NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");
         }


        [TestMethod]
        public void UpdateNotificationDevice_EmptyUserObjectId_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("item", "value");

            var res = NotificationDevice.UpdateNotificationDevice(_mockServer, "", "aaa",
                                                              NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Empty user objectId should fail");
        }


        [TestMethod]
        public void UpdateNotificationDevice_EmptyDeviceObjectId_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("item", "value");

            var res = NotificationDevice.UpdateNotificationDevice(_mockServer, "objectid", "",NotificationDeviceTypes.Pager, oProps);
            Assert.IsFalse(res.Success, "Empty device objectID should fail");
        }


        [TestMethod]
        public void UpdateNotificationDevice_EmptyPropertyList_Failure()
        {
            var res = NotificationDevice.UpdateNotificationDevice(_mockServer, "objectid", "aaa",
                                                  NotificationDeviceTypes.Pager, null);
            Assert.IsFalse(res.Success, "Empty prop list should fail");
        }

        [TestMethod]
        public void AddSmsDevice_NullConnectionServer_Failure()
        {
            //since we can't add an SMS device without a provider (which we can't dummy up) just hit the failure routes here and call
            //it good.
            WebCallResult res = NotificationDevice.AddSmsDevice(null, "objectid", "SMSDevice", "aaa","recipient@test.com", "Sender@test.com",
                                                                NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

         }

        [TestMethod]
        public void AddSmsDevice_EmptyUserObjectId_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "", "SMSDevice", "aaa","recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty user ObjectId param should fail");

            }

        [TestMethod]
        public void AddSmsDevice_EmptyDeviceName_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "objectid", "", "aaa","recipient@test.com", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty device name should fail");

            }

        [TestMethod]
        public void AddSmsDevice_EmptySmppProviderId_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "objectid", "SMSDevice", "","recipient@test.com", "Sender@test.com",
                                                  NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty SMPP provider Id should fail");

            }

        [TestMethod]
        public void AddSmsDevice_EmptyRecipient_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "objectid", "SMSDevice", "aaa","", "Sender@test.com",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty recipient parameter should fail");

            }

        [TestMethod]
        public void AddSmsDevice_EmptySender_Failure()
        {
            var res = NotificationDevice.AddSmsDevice(_mockServer, "objectid", "SMSDevice", "aaa","recipient@test.com", "",
                                      NotificationEventTypes.NewVoiceMail.ToString(), true);
            Assert.IsFalse(res.Success, "Empty sender parameter should fail");
        }

        #endregion

    }
}
