﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class VmsServerTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

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
                _connectionServer = new ConnectionServerRest(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                    mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start VmsServer test:" + ex.Message);
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
            VmsServer oTemp = new VmsServer(null);
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            VmsServer oTemp = new VmsServer(_connectionServer, "bogus");
            Console.WriteLine(oTemp);
        }

        #endregion


        [TestMethod]
        public void StaticCallFailures_GetVmsServers()
        {
            //GetVmsServers is the only static method offered in the class
            List<VmsServer> oList;
            WebCallResult res = VmsServer.GetVmsServers(null, out oList);
            Assert.IsFalse(res.Success, "Static call to GetVmsServer did not fail with: null ConnectionServer");
        }


        [TestMethod]
        public void TestMethod1()
        {
            List<VmsServer> oList;
            WebCallResult res = VmsServer.GetVmsServers(_connectionServer, out oList);
            Assert.IsTrue(res.Success,"Fetch of VMSservers failed:"+res);
            Assert.IsTrue(oList.Count>0,"No VMSServers found on Connection");

            string strObjectId="";
            foreach (var oServer in oList)
            {
                strObjectId = oServer.VmsServerObjectId;
                Console.WriteLine(oServer.ToString());
                Console.WriteLine(oServer.DumpAllProps());
            }

            //test creation
            try
            {
                VmsServer oVmsServer = new VmsServer(_connectionServer, strObjectId);
                Console.WriteLine(oVmsServer);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed creating new VmsServer instance with ObjectId:"+ex);
            }
        }
    }
}