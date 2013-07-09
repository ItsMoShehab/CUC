using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Test user message fetching, sending, filtering, deleting
    /// </summary>
    [TestClass]
    public class UserMessageUnitTests : BaseUnitTests 
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


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            UserMessage oTemp = new UserMessage(null,"objectId","messageobjectid");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if an empty user objectId is passed in
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmtpyUserObjectId_Failure()
        {
            UserMessage oTemp = new UserMessage(_mockServer, "", "messageobjectid");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Calls 

        [TestMethod]
        public void CreateMessageResourceId_NullConnectionServer_Failure()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;

            var res = UserMessage.CreateMessageResourceId(null, "userobjectId", "subject", "resourceId", false,
                                                          SensitivityType.Normal, false, false, false, false, null,
                                                          oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageResourceId with null ConnectionServerRest did not fail.");
        }

        [TestMethod]
        public void CreateMessageResourceId_EmptyRecipientsList_Failure()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;

            var res = UserMessage.CreateMessageResourceId(_mockServer, "userobjectId", "subject", "resourceId", false,
                  SensitivityType.Normal, false, false, false, false, null);
            Assert.IsFalse(res.Success, "Call to CreateMessageResourceId with empty recipients list did not fail.");

            }

        [TestMethod]
        public void CreateMessageResourceId_EmptySubject_Failure()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;

            var res = UserMessage.CreateMessageResourceId(_mockServer, "userobjectId", "", "resourceId", false,
                              SensitivityType.Normal, false, false, false, false, null, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageResourceId with empty subject did not fail.");

            }

        [TestMethod]
        public void CreateMessageResourceId_EmptyResourceId_Failure()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;

            var res = UserMessage.CreateMessageResourceId(_mockServer, "userobjectId", "subject", "", false,
                              SensitivityType.Normal, false, false, false, false, null, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageResourceId with empty resourceId did not fail.");
        }

        [TestMethod]
        public void GetMessageAttachment_EmptyLocalFileTarget_Failure()
        {
            var res = UserMessage.GetMessageAttachment(_mockServer, "", "bogus", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with blank local file target ");
        }

        [TestMethod]
        public void GetMessageAttachment_EmptyUserObjectId_Failure()
        {
            var res = UserMessage.GetMessageAttachment(_mockServer, "temp.wav", "", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with blank user ObjectId");
        }

        [TestMethod]
        public void GetMessageAttachment_EmptyMessageObjectId_Failure()
        {
            var res = UserMessage.GetMessageAttachment(_mockServer, "temp.wav", "bogus", "", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with blank message objectId");
        }

        [TestMethod]
        public void GetMessageAttachment_NullConnectionServer_Failure()
        {
            var res = UserMessage.GetMessageAttachment(null, "temp.wav", "bogus", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with null Connection server");
        }

        [TestMethod]
        public void GetMessageAttachmentCount_NullConnectionServer_Failure()
        {
            int iCount;
            var res = UserMessage.GetMessageAttachmentCount(null, "bogus", "bogus", out iCount);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachmentCount did not fail with null Connection server");
        }

        [TestMethod]
        public void GetMessageAttachmentCount_EmptyObjectId_Failure()
        {
            int iCount;
            var res = UserMessage.GetMessageAttachmentCount(_mockServer, "", "bogus", out iCount);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachmentCount did not fail with empty objectId");
        }

        [TestMethod]
        public void GetMessages_NullConnectionServer_Failure()
        {
            List<UserMessage> oMessages;
            var res = UserMessage.GetMessages(null, "ObjectId", out oMessages);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with null Connection server");
        }

        [TestMethod]
        public void UpdateUserMessage_NullConnectionServer_Failure()
        {
            var res = UserMessage.UpdateUserMessage(null, "MessageObjectId", "userobjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateUserMessage with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void UpdateUserMessage_EmptyParameterList_Failure()
        {
            var res = UserMessage.UpdateUserMessage(_mockServer, "MessageObjectId", "userobjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateUserMessage with empty parameter list did not fail");
        }

        [TestMethod]
        public void UpdateUserMessage_NullParameterList_Failure()
        {
            var res = UserMessage.UpdateUserMessage(_mockServer, "MessageObjectId", "userobjectId", null);
            Assert.IsFalse(res.Success, "Calling UpdateUserMessage with null parameter list did not fail");
        }

        [TestMethod]
        public void CreateMessageLocalWav_NullConnectionServer_Failure()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;

            WebCallResult res = UserMessage.CreateMessageLocalWav(null, "userobjectID", "subject", "dummy.wav", false,
                                                                  SensitivityType.Normal, false, false, false, false, null, false, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with null ConnectionServerRest did not fail.");

            }

        [TestMethod]
        public void CreateMessageLocalWav_EmptyUserObjectId_Failure()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;

            var res = UserMessage.CreateMessageLocalWav(_mockServer, "", "subject", "dummy.wav", false,
                                                    SensitivityType.Normal, false, false, false, false, null, false, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with empty user objectID did not fail.");

}

        [TestMethod]
        public void CreateMessageLocalWav_InvalidWavPath_Failure()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;
            var res = UserMessage.CreateMessageLocalWav(_mockServer, "userobjectID", "subject", "bogus.wav", false,
                                        SensitivityType.Normal, false, false, false, false, null, false, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with invalid WAV path did not fail.");

}

        [TestMethod]
        public void CreateMessageLocalWav_EmptyRecipeint_Failure()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;
            
            var res = UserMessage.CreateMessageLocalWav(_mockServer, "userobjectID", "subject", "dummy.wav", false,
                            SensitivityType.Normal, false, false, false, false, null, false);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with no recipient did not fail.");
        }

        [TestMethod]
        public void CreateMessageLocalWav_EmptySubject_Failure()
        {
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;
            var res = UserMessage.CreateMessageLocalWav(_mockServer, "userobjectID", "", "dummy.wav", false,
                                        SensitivityType.Normal, false, false, false, false, null, false, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with empty subject did not fail.");
        }

        [TestMethod]
        public void ConvertFromTimeDateToMilliseconds_LocalToUnivseralTime()
        {
            DateTime oNow = DateTime.Now;
            long lMs = UserMessage.ConvertFromTimeDateToMilliseconds(oNow);
            long lMs2 = UserMessage.ConvertFromTimeDateToMilliseconds(oNow.ToUniversalTime(), true);
            Assert.IsTrue(lMs == lMs2, "Converting from local time to universal time did not match times");
        }

        [TestMethod]
        public void ConvertFromTimeDateToMilliseconds_UnivseralTimeToLocal()
        {
            DateTime oNow = DateTime.Now;

            long lMs = UserMessage.ConvertFromTimeDateToMilliseconds(oNow.ToUniversalTime());
            long lMs2 = UserMessage.ConvertFromTimeDateToMilliseconds(oNow, false);
            Assert.IsTrue(lMs == lMs2, "Converting from UTC to local did not produce the same starting time");
        }

        [TestMethod]
        public void ConvertFromTimeDateToMilliseconds_NoConversion()
        {
            DateTime oNow = DateTime.Now;
            long lMs = UserMessage.ConvertFromTimeDateToMilliseconds(oNow);
            DateTime oTemp = UserMessage.ConvertFromMillisecondsToTimeDate(lMs);
            TimeSpan oSpan = oTemp - oNow;
            Assert.IsTrue(oSpan.Minutes == 0, "Converting from date to seconds and back did not produce the same time");
        }

        #endregion
    }
}
