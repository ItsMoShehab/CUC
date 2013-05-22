using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortGroupServerTest
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
                _connectionServer = new ConnectionServer(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start PortGroupServer test:" + ex.Message);
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
            PortGroupServer oTemp = new PortGroupServer(null,"PortGroupId","objectID");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// throw ArgumentException on empty objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            PortGroupServer oTemp = new PortGroupServer(_connectionServer, "");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// throw UnityConnectionRestException on invalid objectID
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            PortGroupServer oTemp = new PortGroupServer(_connectionServer, "bogus","bogus");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticCallFailures_AddPortGroupServer()
        {
            var res = PortGroupServer.AddPortGroupServer(null, "", 100, "10.20.30.40");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.AddPortGroupServer(_connectionServer, "", 100, "10.20.30.40");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.AddPortGroupServer(_connectionServer, "invalid", 100, "10.20.30.40");
            Assert.IsFalse(res.Success, "Static call to did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_DeletePortGroupServer()
        {
            var res = PortGroupServer.DeletePortGroupServer(null, "objectid", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.DeletePortGroupServer(_connectionServer, "", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.DeletePortGroupServer(_connectionServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.DeletePortGroupServer(_connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_GetPortGroupServer()
        {
            PortGroupServer oPortGroupServer;
            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, null, "objectid", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _connectionServer, "objectid", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _connectionServer, "", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _connectionServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.UpdatePortGroupServer(null, "portgroupobjectid", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "portgroupobjectid", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_UpdatePortGroupServers()
        {

            var res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "portgroupobjectid", "", null);
            Assert.IsFalse(res.Success, "Static call to did not fail");

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Bogus", "invalid");

            res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "portgroupobjectid", "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_GetPortGroupServers()
        {
            List<PortGroupServer> oList;
            WebCallResult res = PortGroupServer.GetPortGroupServers(null,"", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: null ConnectionServer");

            res = PortGroupServer.GetPortGroupServers(_connectionServer, "", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: empty objectId");

            res = PortGroupServer.GetPortGroupServers(_connectionServer, "bogus", out oList);
            Assert.IsTrue(res.Success,"Fetching port group servers with invalid objectId should not fail");
            Assert.IsTrue(oList.Count==0, "Static call to GetPortGroupTemplates did not fail with: invalid objectId");

        }

        #endregion
    }
}
