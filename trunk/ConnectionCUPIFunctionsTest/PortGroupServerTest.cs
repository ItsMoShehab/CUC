﻿using System;
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
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

        }

        #endregion


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            PortGroupServer oTemp = new PortGroupServer(null,"PortGroupId","objectID");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            PortGroupServer oTemp = new PortGroupServer(_connectionServer, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            PortGroupServer oTemp = new PortGroupServer(_connectionServer, "bogus","bogus");
        }

        #endregion


        [TestMethod]
        public void StaticMethodFailures()
        {
        
            List<PortGroupServer> oList;
            WebCallResult res = PortGroupServer.GetPortGroupServers(null,"", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: null ConnectionServer");

            res = PortGroupServer.GetPortGroupServers(_connectionServer, "", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: empty objectId");

            res = PortGroupServer.GetPortGroupServers(_connectionServer, "bogus", out oList);
            Assert.IsTrue(oList.Count==0, "Static call to GetPortGroupTemplates did not fail with: invalid objectId");


            res = PortGroupServer.AddPortGroupServer(null, "",100, "10.20.30.40");
            Assert.IsFalse(res.Success,"Static call to did not fail");

            res = PortGroupServer.AddPortGroupServer(_connectionServer, "",100, "10.20.30.40");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.AddPortGroupServer(_connectionServer, "invalid",100, "10.20.30.40");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.DeletePortGroupServer(null, "objectid", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.DeletePortGroupServer(_connectionServer, "", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.DeletePortGroupServer(_connectionServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.DeletePortGroupServer(_connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not fail");

            PortGroupServer oPortGroupServer;
            res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, null, "objectid", "portgroupobjectid");
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
            
            res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to did not fail");

            res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "portgroupobjectid", "", null);
            Assert.IsFalse(res.Success, "Static call to did not fail");

            ConnectionPropertyList oProps=new ConnectionPropertyList();
            oProps.Add("Bogus","invalid");

            res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "portgroupobjectid", "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to did not fail");

        }


    }
}