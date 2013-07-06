using System;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class MailboxInfoIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static UserFull _tempUser;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
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
                Assert.IsTrue(res.Success, "Failed to delete temporary user on cleanup.");
            }
        }

        #endregion


        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            MailboxInfo otest = new MailboxInfo(_connectionServer, "bogus");
            Console.WriteLine(otest);
        }

        #endregion


        #region Live Tests


        [TestMethod]
        public void MailboxInfo_MessageInfoFailure()
        {
            MailboxInfo oInfo = null;
            try
            {
                oInfo = new MailboxInfo(_connectionServer, "junk");
                Assert.Fail("Mailbox info with invalid objectId should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exepcted failure:"+ex);
            }
        }

        [TestMethod]
        public void MailboxInfo_MessageInfoTests()
        {
            MailboxInfo oInfo=null;
            try
            {
                oInfo = new MailboxInfo(_connectionServer, _tempUser.ObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed creating new MailboxInfo object:"+ex);
            }

            Console.WriteLine(oInfo.DumpAllProps());
            Console.WriteLine(oInfo.ToString());

            int iInboxCount, iDeletedCount, iSentCount;
            
            //check counts
            WebCallResult res = oInfo.GetFolderMessageCounts(out iInboxCount, out iDeletedCount, out iSentCount);
            Assert.IsTrue(res.Success,"Failed to fetch message folder counts off empty mailbox:"+res);
            Assert.IsTrue(iInboxCount==0,"New mailbox reporting more than 0 inbox messages");


            Assert.IsTrue(oInfo.CurrentSizeInBytes==0,"Newly created mailbox reporting non empty mailbox");
            Assert.IsTrue(oInfo.IsMailboxMounted,"Newly created mailbox reporting it's not mounted");
            Assert.IsTrue(oInfo.IsPrimary,"Newly created mailbox is not reproting itself as primary");
            Assert.IsFalse(oInfo.IsReceiveQuotaExceeded,"Newly created mailbox is reporting over quota");

            //leave message
            MessageAddress oRecipient = new MessageAddress();
            oRecipient.AddressType = MessageAddressType.TO;
            oRecipient.SmtpAddress = _tempUser.SmtpAddress;
            res = UserMessage.CreateMessageLocalWav(_connectionServer, _tempUser.ObjectId, "test subject", "dummy.wav", false, SensitivityType.Normal
                                                   , false, false, false, false, new CallerId { CallerNumber = "1234" },
                                                   true, oRecipient);
            Assert.IsTrue(res.Success, "Failed to create new message from WAV file:" + res);

            //wait for message to be delivered
            Thread.Sleep(10000);

            //refetch mailbox info
             res = oInfo.RefetchMailboxData();
            Assert.IsTrue(res.Success,"Failed to refetch mailbox data:"+res);

            //recheck counts
            res = oInfo.GetFolderMessageCounts(out iInboxCount, out iDeletedCount, out iSentCount);
            Assert.IsTrue(res.Success, "Failed to fetch message folder counts off mailbox with one message:" + res);
            Assert.IsTrue(iInboxCount == 1, "Mailbox reporting with single message not reporting correct inbox count:"+iInboxCount);

        }

        #endregion
   }
}
