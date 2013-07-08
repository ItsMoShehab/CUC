using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RtpCodecDefIntegrationTests : BaseIntegrationTests 
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
        /// UnityConnectionRestException on invalid ObjectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            RtpCodecDef oTemp = new RtpCodecDef(_connectionServer, "bogus");
            Console.WriteLine(oTemp);
        }

        #endregion


        [TestMethod]
        public void GetRtpCodecDefs_Success()
        {
            List<RtpCodecDef> oList;
            WebCallResult res = RtpCodecDef.GetRtpCodecDefs(_connectionServer, out oList);
            Assert.IsTrue(res.Success, "Static call to GetRtpCodecDefs failed:" + res);
            Assert.IsTrue(oList.Count > 0, "No RTPCodecs defined on server");
        }

        [TestMethod]
        public void RtpCodecDef_Constructor_ObjectId_Success()
        {
            List<RtpCodecDef> oList;
            var res = RtpCodecDef.GetRtpCodecDefs(_connectionServer, out oList);
            Assert.IsTrue(oList.Count > 0, "No RTPCodecs defined on server");

            Console.WriteLine(oList[0].ToString());
            
            try
            {
                RtpCodecDef oTest = new RtpCodecDef(_connectionServer, oList[0].ObjectId);
                Console.WriteLine(oTest);
            }
            catch (Exception ex)
            {
                Assert.Fail("Creating new RtpCodecDef from ObjectId failed:"+ex);
            }

        }

    }
}
