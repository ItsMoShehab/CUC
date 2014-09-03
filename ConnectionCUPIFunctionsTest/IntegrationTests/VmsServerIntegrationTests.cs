using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class VmsServerIntegrationTests : BaseIntegrationTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
        }

        #endregion


        #region Class Creation Failures

        /// <summary>
        /// UnityConnectionRestException on invalid objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            VmsServer oTemp = new VmsServer(_connectionServer, "ObjectId");
            Console.WriteLine(oTemp);
        }

        #endregion


        [TestMethod]
        public void GetVmsServers_Success()
        {
            List<VmsServer> oList;
            WebCallResult res = VmsServer.GetVmsServers(_connectionServer, out oList);
            Assert.IsTrue(res.Success, "Fetch of VMSservers failed:" + res);
            Assert.IsTrue(oList.Count > 0, "No VMSServers found on Connection");

            foreach (var oServer in oList)
            {
                Console.WriteLine(oServer.ToString());
                Console.WriteLine(oServer.DumpAllProps());
            }
        }


        [TestMethod]
        public void VmsServer_Constructor_ObjectId_Success()
        {
            List<VmsServer> oList;
            WebCallResult res = VmsServer.GetVmsServers(_connectionServer, out oList);
            Assert.IsTrue(res.Success, "Fetch of VMSservers failed:" + res);
            Assert.IsTrue(oList.Count > 0, "No VMSServers found on Connection");

            try
            {
                VmsServer oVmsServer = new VmsServer(_connectionServer, oList[0].ObjectId);
                Console.WriteLine(oVmsServer);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed creating new VmsServer instance with ObjectId:"+ex);
            }
        }
    }
}
