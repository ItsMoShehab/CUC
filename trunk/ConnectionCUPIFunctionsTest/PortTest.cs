using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

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
                throw new Exception("Unable to attach to Connection server to start Port test:" + ex.Message);
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
            Port oPort = new Port(null);
            Console.WriteLine(oPort);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            Port oPort = new Port(new ConnectionServer(),"blah");
            Console.WriteLine(oPort);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            Port oPort = new Port(_connectionServer,"blah");
            Console.WriteLine(oPort);
        }

        #endregion


        #region Static Call Failures 

        [TestMethod]
        public void StaticCallFailures_UpdatePort()
        {
            var res = Port.UpdatePort(null, "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePort did not fail with null Connection server");

            res = Port.UpdatePort(_connectionServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePort did not fail with empty objectid");

            res = Port.UpdatePort(_connectionServer, "objectId", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePort did not fail with empty prop list");
        }

        [TestMethod]
        public void StaticCallFailures_AddPort()
        {
            var res = Port.AddPort(null, "portgroupid", 4, null);
            Assert.IsFalse(res.Success, "Static call to AddPort did not fail with null connection server");

            res = Port.AddPort(_connectionServer, "", 4, null);
            Assert.IsFalse(res.Success, "Static call to AddPort did not fail with empty port group objectId");

            res = Port.AddPort(_connectionServer, "portgroupid", 4, null);
            Assert.IsFalse(res.Success, "Static call to AddPort did not fail with invalid prop group objectId");
        }

        [TestMethod]
        public void StaticCallFailures_DeletePort()
        {
            var res = Port.DeletePort(null, "objectId");
            Assert.IsFalse(res.Success, "Static call to DeletePort did not fail with null Connection server");

            res = Port.DeletePort(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static call to DeletePort did not fail with empty objectId");

            res = Port.DeletePort(_connectionServer, "objectId");
            Assert.IsFalse(res.Success, "Static call to DeletePort did not fail with Invalid objectId");
        }

        [TestMethod]
        public void StaticCallFailures_GetPort()
        {
            Port oPort;
            WebCallResult res = Port.GetPort(out oPort, null, "objectId");
            Assert.IsFalse(res.Success,"Static call to GetPort did not fail with null Connection server");

            res = Port.GetPort(out oPort, _connectionServer, "");
            Assert.IsFalse(res.Success, "Static call to GetPort did not fail with empty objectId");

            res = Port.GetPort(out oPort, _connectionServer, "objectId");
            Assert.IsFalse(res.Success, "Static call to GetPort did not fail with invalid objectId");

        }

        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            List<Port> oPorts;

            WebCallResult res = Port.GetPorts(_connectionServer, out oPorts);
            Assert.IsTrue(res.Success,"Failed to fetch ports:"+res.ToString());

            string strObjectId="";
            foreach (var oPort in oPorts)
            {
                Console.WriteLine(oPort.ToString());
                Console.WriteLine(oPort.DumpAllProps());
                strObjectId = oPort.ObjectId;
            }

            if (!string.IsNullOrEmpty(strObjectId))
            {
                try
                {
                    Port oNewPort = new Port(_connectionServer, strObjectId);
                    Console.WriteLine(oNewPort);
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(false,"Failed to create an instance of the Port class with valid ObjectId:"+ex);
                }
            }

            res = Port.GetPorts(null, out oPorts);
            Assert.IsFalse(res.Success, "Fetching ports via static class with null Connection server should fail");

            res = Port.GetPorts(new ConnectionServer(), out oPorts);
            Assert.IsFalse(res.Success, "Fetching ports via static class with invalid Connection server should fail");

        }
    }
}
