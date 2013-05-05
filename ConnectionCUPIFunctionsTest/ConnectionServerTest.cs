using System.Threading;
using System.Xml.Linq;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for ConnectionServerTest and is intended
    ///to contain all ConnectionServerTest Unit Tests
    ///</summary>
    [TestClass]
    public class ConnectionServerTest
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
                throw new Exception("Unable to attach to Connection server to start ConnectionServer test:" + ex.Message);
            }

        }

        #endregion


        #region Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a blank server name is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            //invalid login value - empty server name
            ConnectionServer oTestServer = new ConnectionServer("", "login", "Pw");
            Console.WriteLine(oTestServer);
        }

        /// <summary>
        /// Make sure an Exception is thrown if 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailureBadLogin()
        {
            ConnectionServer oTestServer = new ConnectionServer("badservername", "badloginname", "badpassword");
            Console.WriteLine(oTestServer);
        }

        #endregion


        /// <summary>
        /// check all failure paths for version check
        /// </summary>
        [TestMethod]
        public void VersionCheck_Failure()
        {
            Console.WriteLine(_connectionServer.Version.ToString());
            Assert.IsTrue(_connectionServer.Version.IsVersionAtLeast(7, 0, 0, 0), "Minimum version check failed");

            Assert.IsFalse(_connectionServer.Version.IsVersionAtLeast(9999, 0, 0, 0), "Invalid major version not caught");

            Assert.IsFalse(_connectionServer.Version.IsVersionAtLeast(_connectionServer.Version.Major, 9999, 0, 0),
                            "Invalid minor version not caught");

            Assert.IsFalse(_connectionServer.Version.IsVersionAtLeast(_connectionServer.Version.Major, _connectionServer.Version.Minor, 9999, 0),
                            "Invalid rev version not caught");

            Assert.IsFalse(_connectionServer.Version.IsVersionAtLeast(_connectionServer.Version.Major, _connectionServer.Version.Minor, _connectionServer.Version.Rev, 9999),
                            "Invalid build version not caught");
        }


        /// <summary>
        /// check all insertion routes into the XMLFetch routine.
        /// </summary>
        [TestMethod]
        public void SaveXmlFetchTest()
        {
            UserBase oUser = new UserBase(_connectionServer);
            UserFull oUserFull = new UserFull(_connectionServer);

            //integer
            XElement oElement = XElement.Parse("<Language>1234</Language>");
            _connectionServer.SafeXmlFetch(oUser,oElement);
            Assert.AreEqual(oUser.Language, 1234, "Language integer did not insert properly.");

            //string
            oElement = XElement.Parse("<ConversationTui>SubMenu</ConversationTui>");
            _connectionServer.SafeXmlFetch(oUserFull, oElement);
            Assert.AreEqual(oUserFull.ConversationTui, "SubMenu", "SubMenu string did not insert properly");

            //boolean
            oElement = XElement.Parse("<IsTemplate>false</IsTemplate>");
            _connectionServer.SafeXmlFetch(oUserFull, oElement);
            Assert.IsFalse(oUserFull.IsTemplate, "IsTemplate boolean did not insert properly");

            //DateTime
            oElement = XElement.Parse("<CreationTime>2011-08-27T05:00:21Z</CreationTime>");
            _connectionServer.SafeXmlFetch(oUser, oElement);
            //Time above is universal time
            Assert.IsTrue(oUser.CreationTime.Equals(DateTime.Parse("2011/08/27 5:00:21").ToLocalTime()),"Creation time failed to parse correctly");

            //Unknown property name
            oElement = XElement.Parse("<Bogus>false</Bogus>");
            _connectionServer.SafeXmlFetch(oUser, oElement);
        }

        /// <summary>
        /// display name and version number of server
        /// </summary>
        [TestMethod]
        public void DisplayInfo()
        {
            Console.WriteLine(_connectionServer.ToString());
        }


        /// <summary>
        /// Create a new instance of a Connection server object without logging in
        /// </summary>
        [TestMethod]
        public void ConstructorPlain()
        {
            ConnectionServer oTestServer = new ConnectionServer();
            Console.WriteLine(oTestServer);
        }


        /// <summary>
        ///A test for ParseVersionString - use a seperate instance of the ConnectionServer object for this so as not to 
        /// corrupt other tests.
        ///</summary>
        [TestMethod]
        public void ParseVersionString_Failure()
        {
            ConnectionServer oTempServer = new ConnectionServer();

            Assert.IsFalse(oTempServer.ParseVersionString(""), "Empty string");
            Assert.IsFalse(oTempServer.ParseVersionString("1"), "Invalid number of version parts");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2"), "Invalid number of version parts");
            Assert.IsFalse(oTempServer.ParseVersionString("a.2.3"), "Invalid major");
            Assert.IsFalse(oTempServer.ParseVersionString("1.a.3"), "Invalid minor");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.a"), "Invalid rev");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.3.a"), "Invalid build");
            Assert.IsFalse(oTempServer.ParseVersionString("1.2.3.4ES4ES"), "Invalid ES");

            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3"), "Failed to parse legal version string with 3 elements");

            Assert.IsTrue(oTempServer.ParseVersionString("1.2.3.4ES5"),"Failed to parse legal version string");
            Assert.AreEqual(oTempServer.Version.Major, 1, "Major is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Minor, 2, "Minor is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Rev, 3, "Rev is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Build,4,"Build is not parsed out correctly");
            Assert.AreEqual(oTempServer.Version.Es, 5, "ES is not parsed out correctly");

        }

        /// <summary>
        /// Test various paths down the action string construction path
        /// </summary>
        [TestMethod]
        public void ActionStringConstructions()
        {
            //terminal action types
            Console.WriteLine(_connectionServer.GetActionDescription(0, "", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(1,"",""));
            Console.WriteLine(_connectionServer.GetActionDescription(3, "", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(4, "", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(5, "", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(6, "", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(7, "", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(8, "", ""));

            //invalid
            Console.WriteLine(_connectionServer.GetActionDescription(99, "", ""));

            //goto actions
            //Converstaion names
            Console.WriteLine(_connectionServer.GetActionDescription(2, "greetingsadministrator", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "convhotelcheckedout", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "convcvmmboxreset", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "subsystransfer", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "easysignin", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "transferaltcontactnumber", ""));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "broadcastmessageadministrator", ""));
            
            //route to object types
            Console.WriteLine(_connectionServer.GetActionDescription(2, "ad", "blah"));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "phtransfer", "blah"));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "phgreeting", "blah"));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "phinterview", "blah"));
            Console.WriteLine(_connectionServer.GetActionDescription(2, "blah", "blah"));
            

        }
    }
}
