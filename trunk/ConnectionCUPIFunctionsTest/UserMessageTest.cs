using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Test user message fetching, sending, filtering, deleting
    /// </summary>
    [TestClass]
    public class UserMessageTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

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
                 _connectionServer = new ConnectionServer(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
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
                Assert.IsTrue(res.Success, "Failed to delete temporary user on cleanup.");
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
            UserMessage oTemp = new UserMessage(null,"objectId","messageobjectid");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if an empty user objectId is passed in
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            UserMessage oTemp = new UserMessage(_connectionServer, "", "messageobjectid");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Make sure a UnityConnectionRestException is thrown if an invalid ObjectId for a message is provided
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            UserMessage oTemp = new UserMessage(_connectionServer, "bogus","bogus");
            Console.WriteLine(oTemp);
        }

        
        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticCallFailure_CreateMessageResourceId()
        {
            //CreateMessageLocalWav
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;
            
            //CreateMessageResourceId
            var res = UserMessage.CreateMessageResourceId(null, "userobjectId", "subject", "resourceId", false,
                                                      SensitivityType.Normal, false, false, false, false, null, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageResourceId with null ConnectionServer did not fail.");

            res = UserMessage.CreateMessageResourceId(_connectionServer, "userobjectId", "subject", "resourceId", false,
                  SensitivityType.Normal, false, false, false, false, null);
            Assert.IsFalse(res.Success, "Call to CreateMessageResourceId with empty recipients list did not fail.");

            res = UserMessage.CreateMessageResourceId(_connectionServer, "userobjectId", "", "resourceId", false,
                              SensitivityType.Normal, false, false, false, false, null, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageResourceId with empty subject did not fail.");

            res = UserMessage.CreateMessageResourceId(_connectionServer, "userobjectId", "subject", "", false,
                              SensitivityType.Normal, false, false, false, false, null, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageResourceId with empty resourceId did not fail.");

            res = UserMessage.CreateMessageResourceId(_connectionServer, "userobjectId", "subject", "resourceId", true,
                  SensitivityType.Private, true, true, true, true, new CallerId(), oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageResourceId with invalid resourceId did not fail.");
        }

        [TestMethod]
        public void StaticCallFailure_GetmessageAttachment()
        {
            //GetmessageAttachment
            var res = UserMessage.GetMessageAttachment(_connectionServer, "temp.wav", "bogus", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with invalid user and message ObjectIds");

            res = UserMessage.GetMessageAttachment(_connectionServer, "", "bogus", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with blank local file target ");

            res = UserMessage.GetMessageAttachment(_connectionServer, "temp.wav", "", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with blank user ObjectId");

            res = UserMessage.GetMessageAttachment(_connectionServer, "temp.wav", "bogus", "", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with blank message objectId");

            res = UserMessage.GetMessageAttachment(null, "temp.wav", "bogus", "bogus", 1);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachment did not fail with null Connection server");
        }



        [TestMethod]
        public void StaticCallFailure_GetMessageAttachmentCount()
        {
            //GetMessageAttachmentCount
            int iCount;
            var res = UserMessage.GetMessageAttachmentCount(null, "bogus", "bogus", out iCount);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachmentCount did not fail with null Connection server");

            res = UserMessage.GetMessageAttachmentCount(_connectionServer, "bogus", "bogus", out iCount);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachmentCount did not fail with invalid objectIds");

            res = UserMessage.GetMessageAttachmentCount(_connectionServer, "", "bogus", out iCount);
            Assert.IsFalse(res.Success, "Call to static GetMessageAttachmentCount did not fail with empty objectId");

        }

        [TestMethod]
        public void StaticCallFailure_GetMessages()
        {

            List<UserMessage> oMessages;
            //GetMessages
            var res = UserMessage.GetMessages(null, "bogus", out oMessages);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with null Connection server");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.URGENT_FIRST, MessageFilter.Dispatch_False);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId1");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.OLDEST_FIRST, MessageFilter.Dispatch_True);
            Assert.IsFalse(res.Success, "Call to static GetMessages did not fail with invalid objectId2");

            res = UserMessage.GetMessages(_connectionServer, "bogus", out oMessages, 1, 10, MessageSortOrder.NEWEST_FIRST, MessageFilter.Priority_Low);
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
        }

        [TestMethod]
        public void StaticCallFailure_UpdateUserMessage()
        {
            //UpdateUserMessage
            var res = UserMessage.UpdateUserMessage(null, "bogus", "userobjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateUserMessage with null ConnectionServer did not fail");

            res = UserMessage.UpdateUserMessage(_connectionServer, "bogus", "userobjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateUserMessage with empty parameter list did not fail");
        }

        [TestMethod]
        public void StaticCallFailure_CreateMessageLocalWav()
        {
            //CreateMessageLocalWav
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.BCC;
            MessageAddress oRecipient2 = new MessageAddress();
            oRecipient2.AddressType = MessageAddressType.TO;
            MessageAddress oRecipient3 = new MessageAddress();
            oRecipient3.AddressType = MessageAddressType.CC;

            WebCallResult res = UserMessage.CreateMessageLocalWav(null, "userobjectID", "subject", "dummy.wav", false,
                                                                  SensitivityType.Normal, false, false, false, false, null, false, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with null ConnectionServer did not fail.");

            res = UserMessage.CreateMessageLocalWav(_connectionServer, "", "subject", "dummy.wav", false,
                                                    SensitivityType.Normal, false, false, false, false, null, false, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with empty user obejectID did not fail.");

            res = UserMessage.CreateMessageLocalWav(_connectionServer, "userobjectID", "subject", "bogus.wav", false,
                                        SensitivityType.Normal, false, false, false, false, null, false, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with invalid WAV path did not fail.");

            res = UserMessage.CreateMessageLocalWav(_connectionServer, "userobjectID", "subject", "dummy.wav", false,
                            SensitivityType.Normal, false, false, false, false, null, false);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with no recipient did not fail.");

            res = UserMessage.CreateMessageLocalWav(_connectionServer, "userobjectID", "subject", "dummy.wav", false,
                                        SensitivityType.Normal, false, false, false, false, null, true, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with invalid UserObjectId did not fail.");

            res = UserMessage.CreateMessageLocalWav(_connectionServer, "userobjectID", "", "dummy.wav", false,
                                        SensitivityType.Normal, false, false, false, false, null, false, oRecipient, oRecipient2, oRecipient3);
            Assert.IsFalse(res.Success, "Call to CreateMessageLocalWav with empty subject did not fail.");

        }


        #endregion

        [TestMethod]
        public void UserMessage_DateConversionCalls()
        {
            DateTime oNow = DateTime.Now;
            long lMs = UserMessage.ConvertFromTimeDateToMilliseconds(oNow);
            long lMs2 = UserMessage.ConvertFromTimeDateToMilliseconds(oNow.ToUniversalTime(), true);
            Assert.IsTrue(lMs==lMs2,"Converting from local time to universal time did not match times");

            lMs = UserMessage.ConvertFromTimeDateToMilliseconds(oNow.ToUniversalTime());
            lMs2 = UserMessage.ConvertFromTimeDateToMilliseconds(oNow, false);
            Assert.IsTrue(lMs == lMs2,"Converting from UTC to local did not produce the same starting time");

            
            lMs = UserMessage.ConvertFromTimeDateToMilliseconds(oNow);
            DateTime oTemp = UserMessage.ConvertFromMillisecondsToTimeDate(lMs);
            TimeSpan oSpan = oTemp - oNow;
            Assert.IsTrue(oSpan.Minutes==0,"Converting from date to seconds and back did not produce the same time");
        }

        /// <summary>
        /// Create, delete, change messages in the temp user's mailbox before they're deleted
        /// </summary>
        [TestMethod]
        public void UserMessageTests()
        {
            List<UserMessage> oMessages;

            WebCallResult res = UserMessage.GetMessages(_connectionServer, _tempUser.ObjectId, out oMessages);
            Assert.IsTrue(res.Success, "Failed fetching messages on new user");
            Assert.IsTrue(oMessages.Count == 0, "Test user account is reporting more than 0 messages");

            //create a new message
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.TO;
            oRecipient.SmtpAddress = _tempUser.SmtpAddress;
            res = UserMessage.CreateMessageLocalWav(_connectionServer, _tempUser.ObjectId, "test subject", "dummy.wav", false,
                                                   SensitivityType.Normal, false, false, false, false, new CallerId { CallerNumber = "1234" },
                                                   true, oRecipient);
            Assert.IsTrue(res.Success, "Failed to create new message from WAV file:" + res);



            UserMessage oMessage = new UserMessage(_connectionServer, _tempUser.ObjectId);

            Console.WriteLine(oMessage.ToString());
            Console.WriteLine(oMessage.DumpAllProps());

            res = UserMessage.GetMessages(_connectionServer, _tempUser.ObjectId, out oMessages, 1, 10, MessageSortOrder.OLDEST_FIRST, MessageFilter.None,
                MailboxFolder.DeletedItems);
            Assert.IsTrue(res.Success, "Failed fetching deleted messages on new user");

            res = UserMessage.GetMessages(_connectionServer, _tempUser.ObjectId, out oMessages, 1, 10, MessageSortOrder.OLDEST_FIRST, MessageFilter.None,
                MailboxFolder.SentItems);
            Assert.IsTrue(res.Success, "Failed fetching send messages on new user");

            try
            {
                UserMessage oNewMessage = new UserMessage(null, _tempUser.ObjectId);
                Assert.Fail("Creating new UserMessage object with null Connection server did not fail:" + oNewMessage);
            }
            catch { }

            try
            {
                UserMessage oNewMessage = new UserMessage(_connectionServer, "");
                Assert.Fail("Creating new UserMessage object with and empty user ObjectID did not fail:" + oNewMessage);
            }
            catch { }

            //give the message time to be delivered
            Thread.Sleep(5000);

            //fetch
            res = UserMessage.GetMessages(_connectionServer, _tempUser.ObjectId, out oMessages);
            Assert.IsTrue(res.Success, "Failed to fetch messages:" + res);
            Assert.IsTrue(oMessages.Count > 0, "No messages found for temp user after leaving a new message");

            oMessage = oMessages[0];

            //some static helper methods
            Console.WriteLine(UserMessage.ConvertFromTimeDateToMilliseconds(DateTime.Now));
            Console.WriteLine(UserMessage.ConvertFromTimeDateToMilliseconds(DateTime.Now, false));

            //update failure
            res = oMessage.Update();
            Assert.IsFalse(res.Success, "Calling update on message with no pending changes did not fail");

            //update it
            oMessage.Subject = "New test subject";
            oMessage.Read = true;
            res = oMessage.Update();
            Assert.IsTrue(res.Success, "Message update failed");

            //get attachment count
            int iCount;
            res = UserMessage.GetMessageAttachmentCount(_connectionServer, oMessage.MsgId, _tempUser.ObjectId, out iCount);
            Assert.IsTrue(res.Success, "Failed calling GetMessageAttachmentCount:" + res);
            Assert.IsTrue(iCount > 0, "Attachment count not valid");

            res = UserMessage.GetMessageAttachmentCount(null, oMessage.MsgId, _tempUser.ObjectId, out iCount);
            Assert.IsFalse(res.Success, "Calling GetMessageAttachmentAccount with null Connection server did not fail.");
            //get attachments

            Assert.IsTrue(oMessage.Attachments.Count > 0, "Message fetched has no attachments");

            string filename = "c:\\" + Guid.NewGuid().ToString() + ".wav";
            res = oMessage.GetMessageAttachment(filename, 0);
            Assert.IsTrue(res.Success, "Failed to fetch attachment");

            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to delete temporary wav file:" + filename + ", error=" + ex);
            }

            //forward it to mailbox
            res = oMessage.ForwardMessageLocalWav("FW:" + oMessage.Subject, true, SensitivityType.Private, true, true, true, "dummy.wav", true, oRecipient);
            Assert.IsTrue(res.Success, "Failed to forward message");

            //Forward failures
            res = oMessage.ForwardMessageLocalWav("FW:" + oMessage.Subject, true, SensitivityType.Private, true, false, false, "dummy.wav", true);
            Assert.IsFalse(res.Success, "Forwarding with wav with no address did not fail");

            res = oMessage.ForwardMessageLocalWav("FW:" + oMessage.Subject, true, SensitivityType.Private, true, false, false, "bogus.wav", true, oRecipient);
            Assert.IsFalse(res.Success, "Forwarding with wav with invalid WAV file did not fail");

            res = oMessage.ForwardMessageResourceId("FW:" + oMessage.Subject, true, SensitivityType.Private, true, false, false, "", oRecipient);
            Assert.IsFalse(res.Success, "Forwarding with empty resource Id did not fail");

            res = oMessage.ForwardMessageResourceId("FW:" + oMessage.Subject, true, SensitivityType.Private, true, false, false, "bogus");
            Assert.IsFalse(res.Success, "Forwarding resourceId with no addresses did not fail");

            res = oMessage.ForwardMessageResourceId("FW:" + oMessage.Subject, true, SensitivityType.Private, true, false, false, "bogus", oRecipient);
            Assert.IsFalse(res.Success, "Forwarding resourceId with invalid resource Id did not fail");

            //reply
            res = oMessage.ReplyWithLocalWav("RE:" + oMessage.Subject, true, SensitivityType.Private, true, false, false, "dummy.wav", true);
            Assert.IsTrue(res.Success, "Failed to reply");

            //reply to all
            res = oMessage.ReplyWithLocalWav("RE:" + oMessage.Subject, true, SensitivityType.Private, true, false, false, "dummy.wav", true, true);
            Assert.IsTrue(res.Success, "Failed to reply to all");

            //reply failures
            res = oMessage.ReplyWithResourceId("RE:" + oMessage.Subject, "", true, SensitivityType.Private, true, false, false, true);
            Assert.IsFalse(res.Success, "Reply with empty resource ID did not fail");

            res = oMessage.ReplyWithResourceId("RE:" + oMessage.Subject, "bogus", true, SensitivityType.Private, true, false, false, true);
            Assert.IsFalse(res.Success, "Reply to all with bogus resource ID did not fail");

            res = oMessage.ReplyWithResourceId("RE:" + oMessage.Subject, "bogus", true, SensitivityType.Private, true, false, false);
            Assert.IsFalse(res.Success, "Reply with bogus resource ID did not fail");

            //delete it
            res = oMessage.Delete(true);
            Assert.IsTrue(res.Success, "Failed to delete messages:" + res);

            //clear deleted items folder
            res = UserMessage.ClearDeletedItemsFolder(_connectionServer, _tempUser.ObjectId);
            Assert.IsTrue(res.Success, "Failed to clear deleted items folder");

            //failure
            res = UserMessage.ClearDeletedItemsFolder(null, _tempUser.ObjectId);
            Assert.IsFalse(res.Success, "Calling ClearDeletedItemsFolder with null ConnectionServer did not fail");


            res = UserMessage.RecallMessage(null, _tempUser.ObjectId, "bugus");
            Assert.IsFalse(res.Success, "Calling RecallMessage with null Connection server did not fail");

            res = UserMessage.RecallMessage(_connectionServer, _tempUser.ObjectId, "bugus");
            Assert.IsFalse(res.Success, "Calling RecallMessage with invalid message ID did not fail");

            res = UserMessage.RestoreDeletedMessage(null, _tempUser.ObjectId, "bugus");
            Assert.IsFalse(res.Success, "Calling RestoreDeletedMessage with null ConnectionServer did not fail");

            res = UserMessage.RestoreDeletedMessage(_connectionServer, _tempUser.ObjectId, "bugus");
            Assert.IsFalse(res.Success, "Calling RestoreDeletedMessage with invalid messae Id did not fail");
        }


        [TestMethod]
        public void UserMessageSendFetchCompare()
        {
            List<UserMessage> oMessages;

            var res = UserMessage.GetMessages(_connectionServer, _tempUser.ObjectId, out oMessages);
            Assert.IsTrue(res.Success, "Failed fetching messages on new user");
            
            foreach (var oTemp in oMessages)
            {
                res = oTemp.Delete(true);
                if (res.Success == false)
                {
                    Assert.Fail("Failed deleting messages from test inbox:"+res);
                }
            }


            //create a new message
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.TO;
            oRecipient.SmtpAddress = _tempUser.SmtpAddress;

            //send urgent, secure, private
            res = UserMessage.CreateMessageLocalWav(_connectionServer, _tempUser.ObjectId, "test subject", "dummy.wav",true,SensitivityType.Private, 
                true,false,false,false,new CallerId { CallerNumber = "1234" },true, oRecipient);
            Assert.IsTrue(res.Success, "Failed to create new message from WAV file:" + res);

            Thread.Sleep(1000);

            //fetch the message
            res = UserMessage.GetMessages(_connectionServer, _tempUser.ObjectId, out oMessages);
            Assert.IsTrue(res.Success, "Failed fetching messages on new user");
            Assert.IsTrue(oMessages.Count==1,"1 message should be fetched from store, instead messages returned ="+oMessages.Count);

            //compare
            UserMessage oMessage = oMessages[0];
            Assert.IsTrue(oMessage.Sensitivity == SensitivityType.Private,"Message is not flagged as private and it should be");
            Assert.IsTrue(oMessage.Priority == PriorityType.Urgent,"Message not marked urgent and it should be" );
            Assert.IsTrue(oMessage.Secure,"Message not marked secure and it should be");
            
            //delete
            res = oMessage.Delete(true);
            Assert.IsTrue(res.Success,"Failed deleting messages from test inbox:" + res);

            //send NOT secure, private, urgent
            res = UserMessage.CreateMessageLocalWav(_connectionServer, _tempUser.ObjectId, "test subject", "dummy.wav", false, SensitivityType.Normal, 
                false, false, false, false,new CallerId { CallerNumber = "3456" }, true, oRecipient);
            Assert.IsTrue(res.Success, "Failed to create new message from WAV file:" + res);

            Thread.Sleep(1000);
            
            //fetch the message
            res = UserMessage.GetMessages(_connectionServer, _tempUser.ObjectId, out oMessages);
            Assert.IsTrue(res.Success, "Failed fetching messages on new user");
            Assert.IsTrue(oMessages.Count == 1, "1 message should be fetched from store, instead messages returned =" + oMessages.Count);

            //compare
            oMessage = oMessages[0];
            Assert.IsTrue(oMessage.Sensitivity == SensitivityType.Normal, "Message is not flagged as not private and it should be");
            Assert.IsTrue(oMessage.Priority == PriorityType.Normal, "Message not marked normal priority and it should be");
            Assert.IsTrue(!oMessage.Secure, "Message marked secure and it should not be");
            
            //delete
            res = oMessage.Delete(true);
            Assert.IsTrue(res.Success,"Failed deleting messages from test inbox:" + res);


        }

    }
}
