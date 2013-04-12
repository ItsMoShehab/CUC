using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for PhoneSystemTest and is intended
    ///to contain all PhoneSystemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PhoneSystemTest
    {

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion


        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
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
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start DistributionList test:" + ex.Message);
            }
        }

        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreation_Failure()
        {
            PhoneSystem oTest = new PhoneSystem(null, "aaa");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreation_Failure2()
        {
            PhoneSystem oTest = new PhoneSystem(_connectionServer, "aaa");
        }

        
        #endregion

        [TestMethod()]
        public void PhoneSystem_StaticFailureTests()
        {
            WebCallResult res;

            res = PhoneSystem.AddPhoneSystem(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to AddPhoneSystem with null ConnectionServer did not fail");

            res = PhoneSystem.AddPhoneSystem(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static call to AddPhoneSystem with empty name did not fail");

            PhoneSystem oPhoneSystem;
            res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, null, "objectid");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with null ConnectionServer did not fail");

            res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, _connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with invalid ObjectId did not fail");

            res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, _connectionServer, "","");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with invalid empty objectId and name did not fail");

            res = PhoneSystem.GetPhoneSystem(out oPhoneSystem, _connectionServer, "","bogus");
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystem with invalid display name did not fail");

            res = PhoneSystem.UpdatePhoneSystem(null, "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePhoneSystem with null ConnectionServer did not fail");

            res = PhoneSystem.UpdatePhoneSystem(_connectionServer, "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePhoneSystem with invalid ObjectId did not fail");

            res = PhoneSystem.DeletePhoneSystem(null, "objectid");
            Assert.IsFalse(res.Success, "Static call to DeletePhoneSystem with null ConnectionServer did not fail");

            res = PhoneSystem.DeletePhoneSystem(_connectionServer, "objectid");
            Assert.IsFalse(res.Success, "Static call to DeletePhoneSystem with invalid ObjectId did not fail");

            List<PhoneSystemAssociation> oList;
            res = PhoneSystem.GetPhoneSystemAssociations(null, "objectid", out oList);
            Assert.IsFalse(res.Success, "Static call to GetPhoneSystemAssociations with null ConnectionServer did not fail");

            res = PhoneSystem.GetPhoneSystemAssociations(_connectionServer, "objectid", out oList);
            Assert.IsTrue(oList.Count==0, "Static call to GetPhoneSystemAssociations with invalid ObjectId did not return empty list");
        }


        /// <summary>
        ///A test for getting and listing PhoneSystems
        ///</summary>
        [TestMethod()]
        public void PhoneSystem_GetTest()
        {
            WebCallResult res;
            List<PhoneSystem> oSystems;

            res = PhoneSystem.GetPhoneSystems(null, out oSystems);
            Assert.IsFalse(res.Success,"Null Connection server param should fail");

            res = PhoneSystem.GetPhoneSystems(_connectionServer, out oSystems);
            Assert.IsTrue(res.Success,"Failed to fetch phone systems");
            Assert.IsTrue(oSystems.Count>0,"No phone systems found in Connection");

            PhoneSystem oSystem = oSystems[0];

            Console.WriteLine(oSystem.ToString());
            Console.WriteLine(oSystem.DumpAllProps());

            List<PhoneSystemAssociation> oAssociations;
            res = oSystem.GetPhoneSystemAssociations(out oAssociations);
            Assert.IsTrue(res.Success,"Failed to fetch phone system associations:"+res);

            foreach (var oUser in oAssociations)
            {
                Console.WriteLine(oUser.ToString());
            }
        }


        [TestMethod()]
        public void PhoneSystem_AddUpdateDeleteTests()
        {
            WebCallResult res;

            //Creates
            PhoneSystem oPhoneSystem;
            res = PhoneSystem.AddPhoneSystem(_connectionServer, "UnitTest_" + Guid.NewGuid(), out oPhoneSystem);
            Assert.IsTrue(res.Success,"Creating new temporary phone system failed:"+res);

            PortGroup oPortGroup;
            res = PortGroup.AddPortGroup(_connectionServer, "UnitTest_" + Guid.NewGuid(), oPhoneSystem.ObjectId,
                                         _connectionServer.ServerName,TelephonyIntegrationMethodEnum.SIP, "", out oPortGroup);
            Assert.IsTrue(res.Success, "Creating new temporary port group failed:" + res);

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("CapMwi",true);
            res = Port.AddPort(_connectionServer, oPortGroup.ObjectId, 2, oProps);
            Assert.IsTrue(res.Success,"Failed to create ports for port group:"+res);

            List<Port> oPorts;
            res =Port.GetPorts(_connectionServer, out oPorts, oPortGroup.ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch ports for port group:" + res);
            Assert.IsTrue(oPorts.Count==2,"Two ports added to port group but number returned on fetch ="+oPorts.Count);

            //Updates
            res = oPhoneSystem.Update();
            Assert.IsFalse(res.Success,"Update of PhoneSystem with no pending changes did not fail:"+res);

            oPhoneSystem.DisplayName = "Updated_" + Guid.NewGuid();
            oPhoneSystem.RestrictDialUnconditional = true;
            res = oPhoneSystem.Update();
            Assert.IsTrue(res.Success,"Failed to update phone system:"+res);

            res = oPortGroup.Update();
            Assert.IsFalse(res.Success,"Update of port group with no pending changes did not fail:"+res);

            oPortGroup.DisplayName = "Updated_" + Guid.NewGuid();
            res = oPortGroup.Update();
            Assert.IsTrue(res.Success, "Failed to update port group:" + res);

            res = oPorts[0].Update();
            Assert.IsFalse(res.Success, "Updating port with no pending changes did not fail:" + res);
            
            oPorts[0].CapMWI = true;
            res = oPorts[0].Update();
            Assert.IsTrue(res.Success,"Failed updating port:"+res);

            //remove codecs
            List<PortGroupCodec> oPortGroupCodecs;
            res = PortGroupCodec.GetPortGroupCodecs(_connectionServer, oPortGroup.ObjectId, out oPortGroupCodecs);
            Assert.IsTrue(res.Success,"Failed to fetch port group codecs:"+res);

            foreach (var oCodec in oPortGroupCodecs)
            {
                res = oCodec.Delete();
                Assert.IsTrue(res.Success,"Failed to delete port group codec:"+res);
            }

            //add codec in
            List<RtpCodecDef> oCodecs;
            res = RtpCodecDef.GetRtpCodecDefs(_connectionServer, out oCodecs);
            Assert.IsTrue(res.Success,"Failed to fetch RtpCodec Definitions from server:"+res);
            Assert.IsTrue(oCodecs.Count>0,"No codecs fetched from server:"+res);

            PortGroupCodec oPortGroupCodec;
            res = PortGroupCodec.AddPortGroupCodec(_connectionServer, oPortGroup.ObjectId, oCodecs[0].ObjectId, 20, 1,out oPortGroupCodec);
            Assert.IsTrue(res.Success,"Failed to add RtpCode to port group:"+res);

            //check servers
            List<PortGroupServer> oPortGroupServers;
            res = PortGroupServer.GetPortGroupServers(_connectionServer, oPortGroup.ObjectId, out oPortGroupServers);
            Assert.IsTrue(res.Success,"Failed to fetch port group servers from port group:"+res);
            Assert.IsTrue(oPortGroupServers.Count>0,"No port group servers found in port group:"+res);

            //add servers
            PortGroupServer oPortGroupServer;
            res = PortGroupServer.AddPortGroupServer(_connectionServer, oPortGroup.ObjectId,101,_connectionServer.ServerName, "",out oPortGroupServer);
            Assert.IsTrue(res.Success, "Failed to add new port group server to port group servers from port group:" + res);

            Console.WriteLine(oPortGroupServer.ToString());
            Console.WriteLine(oPortGroupServer.DumpAllProps());

            oPortGroupServer.Port = 1234;
            res = oPortGroupServer.Update();
            Assert.IsTrue(res.Success,"Failed to update port group server:"+res);

            //Deletes
            res = oPortGroupServer.Delete();
            Assert.IsTrue(res.Success, "Failed to delete port group server:" + res);
    
            foreach (var oPort in oPorts)
            {
                res = oPort.Delete();
                Assert.IsTrue(res.Success,"Failed to delete port:"+res);
            }

            res = oPortGroup.Delete();
            Assert.IsTrue(res.Success, "Deleting temporary port group failed:" + res);

            res = oPhoneSystem.Delete();
            Assert.IsTrue(res.Success, "Deleting temporary phone system failed:" + res);
        }


    }
}
