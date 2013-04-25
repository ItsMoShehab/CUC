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

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion


        #region Additional test attributes

        //You can use the following additional attributes as you write your tests:
        //
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
        public void TestMethod1()
        {
            List<UserBase> oUsers;
            WebCallResult res = UserBase.GetUsers(_connectionServer, out oUsers);
            Assert.IsTrue(res.Success,"Unable to fetch users from server:"+res);

            Assert.IsTrue(oUsers.Count>0,"No users fetched");

            MailboxInfo oInfo = new MailboxInfo(_connectionServer,oUsers[0].ObjectId);

            Console.WriteLine(oInfo.DumpAllProps());

            Console.WriteLine(oInfo.ToString());
        }
    }
}
