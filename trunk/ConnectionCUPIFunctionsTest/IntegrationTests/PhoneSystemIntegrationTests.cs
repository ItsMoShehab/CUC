using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for PhoneSystemIntegrationTests and is intended
    ///to contain all PhoneSystemIntegrationTests Unit Tests
    ///</summary>
    [TestClass]
    public class PhoneSystemIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static PhoneSystem _phoneSystem;

        private static PortGroup _portGroup;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            WebCallResult res = PhoneSystem.AddPhoneSystem(_connectionServer, "UnitTest_" + Guid.NewGuid(), out _phoneSystem);
            Assert.IsTrue(res.Success, "Creating new temporary phone system failed:" + res);

            res = PortGroup.AddPortGroup(_connectionServer, "UnitTest_" + Guid.NewGuid(), _phoneSystem.ObjectId,
                                         _connectionServer.ServerName, TelephonyIntegrationMethodEnum.SIP, "", out _portGroup);
            Assert.IsTrue(res.Success, "Creating new temporary port group failed:" + res);
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            WebCallResult res;
            if (_portGroup != null)
            {
                res = _portGroup.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary port group on cleanup.");
            }

            if (_phoneSystem != null)
            {
                res = _phoneSystem.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary phone system on cleanup.");
            }
        }

        #endregion


        #region Class Creation Failures

        /// <summary>
        /// Throw UnityConnectionRestException on invalid objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            PhoneSystem oTest = new PhoneSystem(_connectionServer, "aaa");
            Console.WriteLine(oTest);
        }

        
        #endregion


        #region Static Call Failures 

        [TestMethod]
        public void DeletePhoneSystem_InvalidObjectId_Failure()
        {
            var res = PhoneSystem.DeletePhoneSystem(_connectionServer, "ObjectId");
            Assert.IsFalse(res.Success, "Static call to DeletePhoneSystem with invalid ObjectId did not fail");
        }

        [TestMethod]
        public void UpdatePhoneSystem_InvalidObjectId_Failure()
        {
            var res = PhoneSystem.UpdatePhoneSystem(_connectionServer, "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePhoneSystem with blank ObjectId did not fail");
        }

        [TestMethod]
        public void GetPhoneSystemAssociations_InvalidObjectId_Success()
        {
            List<PhoneSystemAssociation> oList;
            var res = PhoneSystem.GetPhoneSystemAssociations(_connectionServer, "objectid", out oList);
            Assert.IsTrue(res.Success, "Fetching phone system associations with invalid objectid should not fail:" + res);
            Assert.IsTrue(oList.Count == 0, "Static call to GetPhoneSystemAssociations with invalid ObjectId did not return empty list");
        }


        [TestMethod]
        public void GetPhoneSystem_InvalidObjectId_Failure()
        {
            PhoneSystem oPhoneSystem;

            var res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, _connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with invalid ObjectId did not fail");
        }

        [TestMethod]
        public void GetPhoneSystem_InvalidDisplayName_Failure()
        {
            PhoneSystem oPhoneSystem; 
            var res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, _connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with invalid display name did not fail");
        }

        #endregion


        #region Live tests

        /// <summary>
        ///A test for getting and listing PhoneSystems
        ///</summary>
        [TestMethod]
        public void PhoneSystem_FetchTest()
        {
            List<PhoneSystem> oSystems;

            WebCallResult res = PhoneSystem.GetPhoneSystems(null, out oSystems);
            Assert.IsFalse(res.Success,"Null Connection server param should fail");

            res = PhoneSystem.GetPhoneSystems(_connectionServer, out oSystems,1,1);
            Assert.IsTrue(res.Success,"Failed to fetch phone systems");
            Assert.IsTrue(oSystems.Count>0,"No phone systems found in Connection");

            PhoneSystem oSystem = oSystems[0];

            Console.WriteLine(oSystem.ToString());
            Console.WriteLine(oSystem.DumpAllProps());

            //fetch the phone system by name
            PhoneSystem oNewSystem;
            res = PhoneSystem.GetPhoneSystem(out oNewSystem, _connectionServer, "", oSystems[0].DisplayName);
            Assert.IsTrue(res.Success,"Failed to fetch phone system by name");

            res = PhoneSystem.GetPhoneSystem(out oNewSystem, _connectionServer, "", "_bogus_");
            Assert.IsFalse(res.Success, "Fetching phone system by invalid name did not fail");


            List<PhoneSystemAssociation> oAssociations;
            res = oSystem.GetPhoneSystemAssociations(out oAssociations);
            Assert.IsTrue(res.Success,"Failed to fetch phone system associations:"+res);

            foreach (var oUser in oAssociations)
            {
                Console.WriteLine(oUser.ToString());
            }

            res = PhoneSystem.GetPhoneSystems(_connectionServer, out oSystems, 1, 1,"query=(ObjectId Is Bogus)");
            Assert.IsTrue(res.Success, "fetching phone systems with invalid query should not fail:" + res);
            Assert.IsTrue(oSystems.Count == 0, "Invalid query string should return an empty phone system list:" + oSystems.Count);
        }


        [TestMethod]
        public void PhoneSystem_TopLevelUpdates()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("CapMwi",true);
            WebCallResult res = Port.AddPort(_connectionServer, _portGroup.ObjectId, 2, oProps);
            Assert.IsTrue(res.Success,"Failed to create ports for port group:"+res);

            res = _phoneSystem.Update();
            Assert.IsFalse(res.Success,"Update of PhoneSystem with no pending changes did not fail:"+res);

            _phoneSystem.DisplayName = "Updated_" + Guid.NewGuid();
            _phoneSystem.RestrictDialUnconditional = true;
            res = _phoneSystem.Update();
            Assert.IsTrue(res.Success,"Failed to update phone system:"+res);
        }

        [TestMethod]
        public void PhoneSystem_PortGroupTests()
        {
            WebCallResult res = _portGroup.Update();
            Assert.IsFalse(res.Success, "Update of port group with no pending changes did not fail:" + res);

            _portGroup.DisplayName = "Updated_" + Guid.NewGuid();
            res = _portGroup.Update();
            Assert.IsTrue(res.Success, "Failed to update port group:" + res);
        }


        [TestMethod]
        public void PhoneSystem_PortTests()
        {
            List<Port> oPorts;
            WebCallResult res = Port.GetPorts(_connectionServer, out oPorts, _portGroup.ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch ports for port group:" + res);
            Assert.IsTrue(oPorts.Count == 2, "Two ports added to port group but number returned on fetch =" + oPorts.Count);

            res = oPorts[0].Update();
            Assert.IsFalse(res.Success, "Updating port with no pending changes did not fail:" + res);

            oPorts[0].CapMWI = true;
            res = oPorts[0].Update();
            Assert.IsTrue(res.Success, "Failed updating port:" + res);

            foreach (var oPort in oPorts)
            {
                res = oPort.Delete();
                Assert.IsTrue(res.Success, "Failed to delete port:" + res);
            }
        }

        [TestMethod]
        public void PhoneSystem_ServerTests()
        {
            //check servers
            List<PortGroupServer> oPortGroupServers;
            WebCallResult res = PortGroupServer.GetPortGroupServers(_connectionServer, _portGroup.ObjectId, out oPortGroupServers);
            Assert.IsTrue(res.Success, "Failed to fetch port group servers from port group:" + res);
            Assert.IsTrue(oPortGroupServers.Count > 0, "No port group servers found in port group:" + res);

            //add servers
            PortGroupServer oPortGroupServer;
            res = PortGroupServer.AddPortGroupServer(_connectionServer, _portGroup.ObjectId, 101, _connectionServer.ServerName, "", out oPortGroupServer);
            Assert.IsTrue(res.Success, "Failed to add new port group server to port group servers from port group:" + res);

            Console.WriteLine(oPortGroupServer.ToString());
            Console.WriteLine(oPortGroupServer.DumpAllProps());

            oPortGroupServer.Port = 1234;
            res = oPortGroupServer.Update();
            Assert.IsTrue(res.Success, "Failed to update port group server:" + res);

            //Deletes
            res = oPortGroupServer.Delete();
            Assert.IsTrue(res.Success, "Failed to delete port group server:" + res);

        }

        [TestMethod]
        public void PhoneSystem_CodecTests()
        {
            //remove codecs
            List<PortGroupCodec> oPortGroupCodecs;
            WebCallResult res = PortGroupCodec.GetPortGroupCodecs(_connectionServer, _portGroup.ObjectId, out oPortGroupCodecs);
            Assert.IsTrue(res.Success, "Failed to fetch port group codecs:" + res);

            foreach (var oCodec in oPortGroupCodecs)
            {
                res = oCodec.Delete();
                Assert.IsTrue(res.Success, "Failed to delete port group codec:" + res);
            }

            //add codec in
            List<RtpCodecDef> oCodecs;
            res = RtpCodecDef.GetRtpCodecDefs(_connectionServer, out oCodecs);
            Assert.IsTrue(res.Success, "Failed to fetch RtpCodec Definitions from server:" + res);
            Assert.IsTrue(oCodecs.Count > 0, "No codecs fetched from server:" + res);

            PortGroupCodec oPortGroupCodec;
            res = PortGroupCodec.AddPortGroupCodec(_connectionServer, _portGroup.ObjectId, oCodecs[0].ObjectId, 20, 1, out oPortGroupCodec);
            Assert.IsTrue(res.Success, "Failed to add RtpCode to port group:" + res);

        }

        #endregion

    }
}

