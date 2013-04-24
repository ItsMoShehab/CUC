using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RtpCodecDefTest
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
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            RtpCodecDef oTemp = new RtpCodecDef(null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            RtpCodecDef oTemp = new RtpCodecDef(_connectionServer, "bogus");
        }

        #endregion


        [TestMethod]
        public void StaticMethodFailures()
        {
            List<RtpCodecDef> oList;
            WebCallResult res = RtpCodecDef.GetRtpCodecDefs(null, out oList);
            Assert.IsFalse(res.Success, "Static call to GetRtpCodecDefs did not fail with: null ConnectionServer");
        }


        [TestMethod]
        public void TestMethods()
        {
            List<RtpCodecDef> oList;
            WebCallResult res = RtpCodecDef.GetRtpCodecDefs(_connectionServer, out oList);
            Assert.IsTrue(res.Success, "Static call to GetRtpCodecDefs failed:"+res);
            Assert.IsTrue(oList.Count>0,"No RTPCodecs defined on server");

            string strObjectId = "";
            foreach (var oCodec in oList)
            {
                strObjectId = oCodec.ObjectId;
                Console.WriteLine(oCodec.ToString());
            }

            try
            {
                RtpCodecDef oTest = new RtpCodecDef(_connectionServer, strObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Creating new RtpCodecDef from ObjectId failed:"+ex);
            }

        }

    }
}
