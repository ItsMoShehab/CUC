﻿using System.Collections.Generic;
using System.Threading;
using ConnectionCUPIFunctions;
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

        /// <summary>
        ///A test for getting and listing PhoneSystems
        ///</summary>
        [TestMethod()]
        public void PhoneSystem_Test()
        {
            WebCallResult res;
            List<PhoneSystem> oSystems;

            res = PhoneSystem.GetPhoneSystems(null, out oSystems);
            Assert.IsFalse(res.Success,"Null Connection server param should fail");

            res = PhoneSystem.GetPhoneSystems(_connectionServer, out oSystems);
            Assert.IsTrue(res.Success,"Failed to fetch phone systems");

            foreach (PhoneSystem oTemp in oSystems)
            {
                Console.WriteLine(oTemp.ToString());
            }
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreation_Failure()
        {
            PhoneSystem oTest = new PhoneSystem(null, "aaa");
        }

    }
}
