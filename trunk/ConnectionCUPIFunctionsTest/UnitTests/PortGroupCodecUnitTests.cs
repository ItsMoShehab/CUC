using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
    // ReSharper disable HeuristicUnreachableCode

    [TestClass]
    public class PortGroupCodecUnitTests : BaseUnitTests
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
            PortGroupCodec oPort = new PortGroupCodec(null,"PortGroupId");
            Console.WriteLine(oPort);
        }

        /// <summary>
        /// Make sure an Exception is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyObjectId_Failure()
        {
            PortGroupCodec oPort = new PortGroupCodec(_mockServer, "");
            Console.WriteLine(oPort);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void DeletePortGroupCodec_NullConnectionServer_Failure()
        {
            var res = PortGroupCodec.DeletePortGroupCodec(null, "portgroupobjectid", "objectid");
            Assert.IsFalse(res.Success, "Static call to AddPortGroupCodec did not fail with null connection server");
        }
        
        [TestMethod]
        public void DeletePortGroupCodec_EmptyPortGroupObjectId_Failure()
        {
            var res = PortGroupCodec.DeletePortGroupCodec(_mockServer, "", "objectid");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupCodec did not fail with empty media port objectId");

         }

        [TestMethod]
        public void DeletePortGroupCodec_EmptyObjectId_Failure()
        {
            var res = PortGroupCodec.DeletePortGroupCodec(_mockServer, "portgroupobjectid", "");
            Assert.IsFalse(res.Success, "Static call to DeletePortGroupCodec did not fail with empty objectid");
        }

        [TestMethod]
        public void GetPortGroupCodecs_NullConnectionServer_Failure()
        {
            List<PortGroupCodec> oList;
            var res = PortGroupCodec.GetPortGroupCodecs(null, "portgroupobjectid", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupCodecs did not fail with null connection server");
        }
        [TestMethod]
        public void GetPortGroupCodecs_EmptyPortGroupObjectId_Failure()
        {
            List<PortGroupCodec> oList;
            var res = PortGroupCodec.GetPortGroupCodecs(_mockServer, "", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupCodecs did not fail with empty media port objectId");
        }


        [TestMethod]
        public void AddPortGroupCodec_NullConnectionServer_Failure()
        {
            PortGroupCodec oPortGroupCodec;
            WebCallResult res = PortGroupCodec.AddPortGroupCodec(null, "portgroupid", "rtpobjectid", 20, 1,out oPortGroupCodec);
            Assert.IsFalse(res.Success, "Static call to AddPortGroupCodec did not fail with null connection server ");
        }

        [TestMethod]
        public void AddPortGroupCodec_EmptyPortGroupObjectId_Failure()
        {
            PortGroupCodec oPortGroupCodec;
            var res = PortGroupCodec.AddPortGroupCodec(_mockServer, "", "rtpobjectid", 20, 1);
            Assert.IsFalse(res.Success, "Static call to AddPortGroupCodec did not fail with empty media port objectId");
        }

        [TestMethod]
        public void AddPortGroupCodec_EmptyRtpDefObjectId_Failure()
        {
            PortGroupCodec oPortGroupCodec;
            var res = PortGroupCodec.AddPortGroupCodec(_mockServer, "portgroupid", "", 20, 1);
            Assert.IsFalse(res.Success, "Static call to AddPortGroupCodec did not fail with empty objectid");
        }

        #endregion

    
    }
}
