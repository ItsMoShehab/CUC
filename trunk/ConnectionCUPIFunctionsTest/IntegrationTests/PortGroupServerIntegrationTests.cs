using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortGroupServerIntegrationTests : BaseIntegrationTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
        }

        #endregion


        #region Class Creation Failures

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            PortGroupServer oTemp = new PortGroupServer(_connectionServer, "bogus","bogus");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void AddPortGroupServer_InvalidObjectId_Failure()
        {
            var res = PortGroupServer.AddPortGroupServer(_connectionServer, "invalidObjectId", 100, "10.20.30.40");
            Assert.IsFalse(res.Success, "Static call to AddPortGroupServer did not fail with invalid ObjectID");
        }


        [TestMethod]
        public void DeletePortGroupServer_EmptyPortGroupObjectId_Failure()
        {
            var res = PortGroupServer.DeletePortGroupServer(_connectionServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupServer did not fail with an empty port groupObjectId");
        }

        [TestMethod]
        public void DeletePortGroupServer_InvalidPortGropObjectId_Failure()
        {
            var res = PortGroupServer.DeletePortGroupServer(_connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupServer did not fail with an invalid PortGroupObjectId");
        }

        [TestMethod]
        public void GetPortGroupServer_InvalidObjectId_Failure()
        {
            PortGroupServer oPortGroupServer;
            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _connectionServer, "objectid","portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to GetPortGroupServer did not fail with invalid ObjectId");
        }

        [TestMethod]
        public void GetPortGroupServer_EmptyObjectId_Failure()
        {
            PortGroupServer oPortGroupServer;

            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _connectionServer, "", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to GetPortGroupServer did not fail with empty objectId");
        }

        [TestMethod]
        public void GetPortGroupServer_EmptyPortGroupObjectId_Failure()
        {
            PortGroupServer oPortGroupServer; 
            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _connectionServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to GetPortGroupServer did not fail with empty portgroupobjectid");
        }

        [TestMethod]
        public void UpdatePortGroupServer_NullPropertyList_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "portgroupobjectid", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with a null property list");
        }

        [TestMethod]
        public void UpdatePortGroupServer_EmptyPropertyList_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "portgroupobjectid", "objectid", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with an empty property list");
        }


        [TestMethod]
        public void UpdatePortGroupServer_EmptyPortGroupObjectId_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Bogus", "invalid");

            var res = PortGroupServer.UpdatePortGroupServer(_connectionServer, "portgroupobjectid", "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with an empty property list");
        }

        [TestMethod]
        public void GetPortGroupServers_InvalidObjectId_Failure()
        {
            List<PortGroupServer> oList;

            var res = PortGroupServer.GetPortGroupServers(_connectionServer, "bogus", out oList);
            Assert.IsTrue(res.Success,"Fetching port group servers with invalid objectId should not fail");
            Assert.IsTrue(oList.Count==0, "Static call to GetPortGroupTemplates did not fail with: invalid objectId");
        }

        #endregion
    }
}
