using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortGroupServerUnitTests : BaseUnitTests
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
            PortGroupServer oTemp = new PortGroupServer(null,"PortGroupId","objectID");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// throw ArgumentException on empty objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyObjectId_Failure()
        {
            PortGroupServer oTemp = new PortGroupServer(_mockServer, "");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void AddPortGroupServer_NullConnectionServer_Failure()
        {
            var res = PortGroupServer.AddPortGroupServer(null, "", 100, "10.20.30.40");
            Assert.IsFalse(res.Success, "Static call to AddPortGroupServer did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void AddPortGroupServer_EmptyObjectId_Failure()
        {
            var res = PortGroupServer.AddPortGroupServer(_mockServer   , "", 100, "10.20.30.40");
            Assert.IsFalse(res.Success, "Static call to AddPortGroupServer did not fail with empty objectId");
        }

        [TestMethod]
        public void DeletePortGroupServer_NullConnectionServer_Failure()
        {
            var res = PortGroupServer.DeletePortGroupServer(null, "objectid", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupServer did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void DeletePortGroupServer_EmptyObjectId_Failure()
        {
            var res = PortGroupServer.DeletePortGroupServer(_mockServer, "", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupServer did not fail with empty ObjectId");
        }

        [TestMethod]
        public void DeletePortGroupServer_EmptyPortGroupObjectId_Failure()
        {
            var res = PortGroupServer.DeletePortGroupServer(_mockServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupServer did not fail with empty PortGroupObjectId");
        }

        [TestMethod]
        public void GetPortGroupServer_NullConnectionServer_Failure()
        {
            PortGroupServer oPortGroupServer;
            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, null, "objectid", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to GetPortGroupServer did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void GetPortGroupServer_EmptyObjectId_Failure()
        {
            PortGroupServer oPortGroupServer;
            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _mockServer, "", "portgroupobjectid");
            Assert.IsFalse(res.Success, "Static call to GetPortGroupServer did not fail with empty ObjectId");
        }

        [TestMethod]
        public void GetPortGroupServer_EmptyPortGroupObjectId_Failure()
        {
            PortGroupServer oPortGroupServer;
            var res = PortGroupServer.GetPortGroupServer(out oPortGroupServer, _mockServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to GetPortGroupServer did not fail with empty portgroupObjectId");
        }

        [TestMethod]
        public void UpdatePortGroupServer_NullConnectionServer_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(null, "portgroupobjectid", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void UpdatePortGroupServer_EmptyPortGroupObjectId_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_mockServer, "", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with empty PortGroupObjectId");
        }

        [TestMethod]
        public void UpdatePortGroupServer_EmptyObjectId_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_mockServer, "portgroupobjectid", "", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with empty ObjectId");
        }

        [TestMethod]
        public void UpdatePortGroupServer_NullPropertyList_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_mockServer, "portgroupobjectid", "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with null property list");
        }

        [TestMethod]
        public void UpdatePortGroupServer_EmptyPropertyList_Failure()
        {
            var res = PortGroupServer.UpdatePortGroupServer(_mockServer, "portgroupobjectid", "objectid", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Static call to UpdatePortGroupServer did not fail with empty property list");
        }


        [TestMethod]
        public void GetPortGroupServers_NullConnectionServer_Failure()
        {
            List<PortGroupServer> oList;
            WebCallResult res = PortGroupServer.GetPortGroupServers(null, "", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void GetPortGroupServers_EmptyPortGroupObjectId_Failure()
        {
            List<PortGroupServer> oList;

            var res = PortGroupServer.GetPortGroupServers(_mockServer, "", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: empty objectId");
        }

        #endregion
    }
}
