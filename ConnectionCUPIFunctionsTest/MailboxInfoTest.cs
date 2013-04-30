using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class MailboxInfoTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start MailboxInfo test:" + ex.Message);
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
            MailboxInfo otest = new MailboxInfo(null,"");
            Console.WriteLine(otest);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a blank user objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            MailboxInfo otest = new MailboxInfo(_connectionServer, "");
            Console.WriteLine(otest);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if an invalid Connection server is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            MailboxInfo otest = new MailboxInfo(new ConnectionServer(), "blah");
            Console.WriteLine(otest);
        }

        #endregion


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
            res = UserMessage.CreateMessageLocalWav(_connectionServer, _tempUser.ObjectId, "test subject", "dummy.wav", false,
                                                   false, false, false, false, false, new CallerId { CallerNumber = "1234" },
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

    }
}
