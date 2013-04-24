using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortGroupTest
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


        #region Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            PortGroup oPorts = new PortGroup(null);
        }


        /// <summary>
        /// Empty Connection class instance should fail.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            PortGroup oPorts = new PortGroup(new ConnectionServer(), "blah");
        }

        /// <summary>
        /// Valid connection server but invalid object ID should fail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            PortGroup oPorts = new PortGroup(_connectionServer, "blah");
        }

        #endregion

        [TestMethod]
        public void StaticMethodFailures()
        {
            List<PortGroup> oPortGroups;
            WebCallResult res;

            res = PortGroup.GetPortGroups(null, out oPortGroups);
            Assert.IsFalse(res.Success, "Fetching port groups with null Connection server should fail.");

            res = PortGroup.GetPortGroups(new ConnectionServer(), out oPortGroups);
            Assert.IsFalse(res.Success, "Fetching port groups with invalid Connection server should fail.");
        }


        [TestMethod]
        public void TestMethod1()
        {
            List<PortGroup> oPortGroups;
            WebCallResult res = PortGroup.GetPortGroups(_connectionServer, out oPortGroups);
            Assert.IsTrue(res.Success, "Fetching port groups failed:" + res);


            string strPortGroupObjectId="";
            string strPortGroupDisplayName = "";

            foreach (var oPortGroup in oPortGroups)
            {
                Console.WriteLine(oPortGroup.ToString());
                Console.WriteLine(oPortGroup.DumpAllProps());
                strPortGroupObjectId = oPortGroup.ObjectId;
                strPortGroupDisplayName = oPortGroup.DisplayName;
            }

            try
            {
                PortGroup oNewGroup = new PortGroup(_connectionServer, strPortGroupObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("PortGroup creation with valid ObjectId of PortGroup failed:"+ex);
            }

            try
            {
                PortGroup oNewGroup = new PortGroup(_connectionServer, "",strPortGroupDisplayName);
            }
            catch (Exception ex)
            {
                Assert.Fail("PortGroup creation with valid display name of PortGroup failed:" + ex);
            }

            try
            {
                PortGroup oNewGroup = new PortGroup(_connectionServer, "","bogus");
                Assert.Fail("PortGroup creation with invalid display name of PortGroup did not fail");
            }
            catch {}

        }
    }
}
