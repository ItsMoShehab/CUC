using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
    // ReSharper disable HeuristicUnreachableCode

    [TestClass]
    public class PortGroupCodecIntegrationTests : BaseIntegrationTests
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
        public void ClassConstructor_InvalidObjectId_Failure()
        {
            PortGroupCodec oPort = new PortGroupCodec(_connectionServer, "bogus","bogus");
            Console.WriteLine(oPort);
        }


        #endregion


        #region Static Call Failures

        [TestMethod]
        public void DeletePortGroupCodec_InvalidPortGroupAndObjectId_Failure()
        {
            var res = PortGroupCodec.DeletePortGroupCodec(_connectionServer, "portgroupobjectid", "objectid");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupCodec did not fail with invalid port group and objectid");
        }

        [TestMethod]
        public void GetPortGroupCodec_InvalidObjectId_Success()
        {
            List<PortGroupCodec> oList;

            var res = PortGroupCodec.GetPortGroupCodecs(_connectionServer, "bogus", out oList);
            Assert.IsTrue(res.Success, "Static call to GetPortGroupCodecs with invalid objectId should not fail:"+res);
            Assert.IsTrue(oList.Count==0,"Call to GetPortGroupCodec with invalid ObjectId should return an empty list");
        }


        [TestMethod]
        public void AddPortGroupCodec_InvalidPortGroupId_Failure()
        {
            var res = PortGroupCodec.AddPortGroupCodec(_connectionServer, "portgroupid", "objectid", 20, 1);
            Assert.IsFalse(res.Success, "Static call to AddPortGroupCodec did not fail with invalid port group Id and objectid");
        }

        #endregion


        #region Live Tests

        private PortGroup HelperGetPortGroup()
        {
            List<PortGroup> oPortGroups;
            WebCallResult res = PortGroup.GetPortGroups(_connectionServer, out oPortGroups);
            Assert.IsTrue(res.Success, "Failed to fetch port groups:" + res);
            Assert.IsTrue(oPortGroups.Count > 0, "No port groups found on Connection server:" + res);
            
            return oPortGroups[0];
        }

        private PortGroupCodec HelperGetPortGroupCodec()
        {
            PortGroup oPortGroup = HelperGetPortGroup();

            List<PortGroupCodec> oPortGroupCodecs;
            var res = PortGroupCodec.GetPortGroupCodecs(_connectionServer, oPortGroup.ObjectId, out oPortGroupCodecs);
            Assert.IsTrue(res.Success, "Failed to fetch port group codecs:" + res);
            Assert.IsTrue(oPortGroupCodecs.Count > 0, "No port group codecs found in port group:" + res);
            return oPortGroupCodecs[0];
        }


        [TestMethod]
        public void GetPortGroups_Success()
        {
            List<PortGroup> oPortGroups;
            WebCallResult res = PortGroup.GetPortGroups(_connectionServer, out oPortGroups);
            Assert.IsTrue(res.Success, "Failed to fetch port groups:" + res);
            Assert.IsTrue(oPortGroups.Count > 0, "No port groups found on Connection server:" + res);

        }

        [TestMethod]
        public void GetPortGroupCodecs_Success()
        {
            PortGroup oPortGroup = HelperGetPortGroup();

            //exercise ToString and DumpAllProps interfaces
            Console.WriteLine(oPortGroup.ToString());
            Console.WriteLine(oPortGroup.DumpAllProps());

            List<PortGroupCodec> oPortGroupCodecs;
            var res = PortGroupCodec.GetPortGroupCodecs(_connectionServer, oPortGroup.ObjectId, out oPortGroupCodecs);
            Assert.IsTrue(res.Success, "Failed to fetch port group codecs:" + res);
            Assert.IsTrue(oPortGroupCodecs.Count > 0, "No port group codecs found in port group:" + res);
        }


        [TestMethod]
        public void PortGroupCodec_ConstructorWithObjectId_Success()
        {
            PortGroupCodec oPortGroupCodec = HelperGetPortGroupCodec();
            PortGroup oPortGroup = HelperGetPortGroup();

            //exercise ToString and DumpAllProps interfaces
            Console.WriteLine(oPortGroupCodec.ToString());
            Console.WriteLine(oPortGroupCodec.DumpAllProps());

            try
            {
                PortGroupCodec oTest = new PortGroupCodec(_connectionServer, oPortGroup.ObjectId,oPortGroupCodec.ObjectId);
                Console.WriteLine(oTest);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new port group codec instance with valid objectId:"+ex);
            }
        }

        #endregion

    }
}
