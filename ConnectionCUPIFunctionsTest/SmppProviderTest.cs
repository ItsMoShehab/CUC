﻿using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for SmppProviderTest and is intended
    ///to contain all SmppProviderTest Unit Tests
    ///</summary>
    [TestClass]
    public class SmppProviderTest
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
                throw new Exception("Unable to attach to Connection server to start DistributionList test:" + ex.Message);
            }
        }

        #endregion
        

        #region Constructor Tests

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if the objectId for an SMPP provider is not found
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreation_Failure()
        {
            SmppProvider oTest = new SmppProvider(_connectionServer, "aaa");
            Console.WriteLine(oTest);
        }


        /// <summary>
        /// Make sure an ArgumentException is thrown if an empty objectID is passed in
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreation_Failure2()
        {
            SmppProvider oTest = new SmppProvider(null, "");
            Console.WriteLine(oTest);
        }

        #endregion


        /// <summary>
        ///A test for getting and listing PhoneSystems
        ///</summary>
        [TestMethod]
        public void SmppProvider_Test()
        {
            List<SmppProvider> oProviders;

            WebCallResult res = SmppProvider.GetSmppProviders(null, out oProviders);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

            res = SmppProvider.GetSmppProviders(_connectionServer, out oProviders);
            Assert.IsTrue(res.Success, "Failed to fetch SmppProviders");
            Assert.IsTrue(oProviders.Count>0,"No SMPP providers returned");

            string strObjectId="";
            foreach (SmppProvider oTemp in oProviders)
            {
                Console.WriteLine(oTemp.ToString());
                strObjectId = oTemp.ObjectId;
            }

            try
            {
                SmppProvider oNewProvider = new SmppProvider(_connectionServer, strObjectId);
                Console.WriteLine(oNewProvider);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unable to fetch SMPP provider by valid ObjectId:"+ex);
            }
        }
    }
}