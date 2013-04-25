using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
    // ReSharper disable HeuristicUnreachableCode

    [TestClass]
    public class PortGroupCodecTest
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
                throw new Exception("Unable to attach to Connection server to start PortGroupCodec test:" + ex.Message);
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
            PortGroupCodec oPort = new PortGroupCodec(null,"PortGroupId");
            Console.WriteLine(oPort);
        }

        /// <summary>
        /// Make sure an Exception is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            PortGroupCodec oPort = new PortGroupCodec(_connectionServer, "");
            Console.WriteLine(oPort);
        }

        /// <summary>
        /// Make sure an Exception is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            PortGroupCodec oPort = new PortGroupCodec(_connectionServer, "bogus","bogus");
            Console.WriteLine(oPort);
        }


        #endregion


        [TestMethod]
        public void PortGroupCodecFetchTests()
        {
            List<PortGroup> oPortGroups;
            WebCallResult res = PortGroup.GetPortGroups(_connectionServer, out oPortGroups);
            Assert.IsTrue(res.Success, "Failed to fetch port groups:" + res);
            Assert.IsTrue(oPortGroups.Count>0,"No port groups found on Connection server:"+res);

            PortGroup oPortGroup = oPortGroups[0];

            List<PortGroupCodec> oPortGroupCodecs;
            res = PortGroupCodec.GetPortGroupCodecs(_connectionServer, oPortGroup.ObjectId, out oPortGroupCodecs);
            Assert.IsTrue(res.Success,"Failed to fetch port group codecs:"+res);
            Assert.IsTrue(oPortGroupCodecs.Count>0,"No port group codecs found in port group:"+res);

            PortGroupCodec oPortGroupCodec = oPortGroupCodecs[0];

            Console.WriteLine(oPortGroupCodec.ToString());
            Console.WriteLine(oPortGroupCodec.DumpAllProps());

            try
            {
                PortGroupCodec oTest = new PortGroupCodec(_connectionServer, oPortGroup.ObjectId,
                                                          oPortGroupCodec.ObjectId);
                Console.WriteLine(oTest);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new port group codec instance with valid objectId:"+ex);
            }

        }


        [TestMethod]
        public void StaticMethodFailures()
        {
            PortGroupCodec oPortGroupCodec;
            WebCallResult res = PortGroupCodec.AddPortGroupCodec(null, "portgroupid", "rtpobjectid", 20, 1,out oPortGroupCodec);
            Assert.IsFalse(res.Success,"Static call to AddPortGroupCodec did not fail with null connection server ");

            res = PortGroupCodec.AddPortGroupCodec(_connectionServer, "", "rtpobjectid", 20, 1);
            Assert.IsFalse(res.Success, "Static call to AddPortGroupCodec did not fail with empty media port objectId");

            res = PortGroupCodec.AddPortGroupCodec(_connectionServer, "portgroupid", "", 20, 1);
            Assert.IsFalse(res.Success, "Static call to AddPortGroupCodec did not fail with empty objectid");

            res = PortGroupCodec.DeletePortGroupCodec(null, "portgroupobjectid", "objectid");
            Assert.IsFalse(res.Success, "Static call to AddPortGroupCodec did not fail with null connection server");

            res = PortGroupCodec.DeletePortGroupCodec(_connectionServer, "", "objectid");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupCodec did not fail with empty media port objectId");

            res = PortGroupCodec.DeletePortGroupCodec(_connectionServer, "portgroupobjectid", "");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupCodec did not fail with empty objectid");

            res = PortGroupCodec.DeletePortGroupCodec(_connectionServer, "portgroupobjectid", "objectid");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupCodec did not fail with invalid port group and objectid");

            List<PortGroupCodec> oList;
            res = PortGroupCodec.GetPortGroupCodecs(null, "portgroupobjectid", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupCodecs did not fail with null connection server");

            res = PortGroupCodec.GetPortGroupCodecs(_connectionServer, "", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupCodecs did not fail with empty media port objectId");
        }

    }
}
