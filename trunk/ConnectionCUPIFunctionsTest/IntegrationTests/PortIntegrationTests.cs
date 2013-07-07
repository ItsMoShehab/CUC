using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortIntegrationTests : BaseIntegrationTests
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

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            Port oPort = new Port(_connectionServer,"blah");
            Console.WriteLine(oPort);
        }

        #endregion


        #region Static Call Failures 

        [TestMethod]
        public void UpdatePort_InvalidObjectId_Failure()
        {
            var res = Port.UpdatePort(_connectionServer,"invalidObjectId", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePort did not fail with invalid objectid");
        }

        [TestMethod]
        public void AddPort_InvalidPortGroupObjectId_Failure()
        {
            var res = Port.AddPort(_connectionServer, "portgroupid", 4, null);
            Assert.IsFalse(res.Success, "Static call to AddPort did not fail with invalid prop group objectId");
        }

        [TestMethod]
        public void DeletePort_InvalidObjectId_Failure()
        {
            var res = Port.DeletePort(_connectionServer, "objectId");
            Assert.IsFalse(res.Success, "Static call to DeletePort did not fail with Invalid objectId");
        }

        [TestMethod]
        public void GetPort_InvalidObjectId_Failure()
        {
            Port oPort;
            var res = Port.GetPort(out oPort, _connectionServer, "objectId");
            Assert.IsFalse(res.Success, "Static call to GetPort did not fail with invalid objectId");
        }

        #endregion


        #region Live Tests

        private Port HelperGetPort()
        {
            List<Port> oPorts;

            WebCallResult res = Port.GetPorts(_connectionServer, out oPorts);
            Assert.IsTrue(res.Success, "Failed to fetch ports:" + res.ToString());
            Assert.IsTrue(oPorts.Count>0,"No ports configured");
            return oPorts[0];
        }

        [TestMethod]
        public void GetPorts_Success()
        {
            var oPort = HelperGetPort();

            Console.WriteLine(oPort.ToString());
            Console.WriteLine(oPort.DumpAllProps());

        }

        [TestMethod]
        public void Port_ConstructorWithObjectId_Success()
        {
            var oPort = HelperGetPort();
            try
            {
                Port oNewPort = new Port(_connectionServer, oPort.ObjectId);
                Console.WriteLine(oNewPort);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, "Failed to create an instance of the Port class with valid ObjectId:" + ex);
            }

        }

        #endregion
    }
}
