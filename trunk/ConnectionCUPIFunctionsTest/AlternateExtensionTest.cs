using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{


    /// <summary>
    ///This is a test class for AlternateExtensionTest and is intended
    ///to contain all AlternateExtensionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AlternateExtensionTest
    {

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        //class wide _user instance to use for testing
        private static UserBase _user;

        private TestContext testContextInstance;

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
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

            //get the operator to work with as our user
            try
            {
                _user = new UserBase(_connectionServer, "", "operator");
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to get Operator user for testing:" + ex.Message);
            }

        }

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

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            AlternateExtension oTemp = new AlternateExtension(null, "aaa");
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ObjectId is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            AlternateExtension oTemp = new AlternateExtension(_connectionServer, "");
        }

        /// <summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void DeleteAlternateExtension_Failure()
        {
            WebCallResult res;

            //static delete failure calls
            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, _user.ObjectId, "aaa");
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, _user.ObjectId, "");
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, "", "aaa");
            Assert.IsFalse(res.Success, "Empty user objectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "Invalid user objectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(null, _user.ObjectId, "aaa");
            Assert.IsFalse(res.Success, "Null ConnectionServer object should fail");

        }

        ///<summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void GetAlternateExtensions_Failure()
        {
            WebCallResult res;
            List<AlternateExtension> oAltExts;

            //static GetAlternateExtensions calls
            res = AlternateExtension.GetAlternateExtensions(null, _user.ObjectId, out oAltExts);
            Assert.IsFalse(res.Success, "Null ConnectionServer object should fail");

        }

        ///<summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void GetAlternateExtension_Failure()
        {
            AlternateExtension oAltExt;
            WebCallResult res;

            //static GetAlternateExtension calls
            res = AlternateExtension.GetAlternateExtension(_connectionServer, _user.ObjectId, "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Invalid objecTId should fail");

            res = AlternateExtension.GetAlternateExtension(null, _user.ObjectId, "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Null ConnectonServer object should fail");

            res = AlternateExtension.GetAlternateExtension(_connectionServer, "", "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Empty UServerObjectID should fail");

            res = AlternateExtension.GetAlternateExtension(_connectionServer, "aaa", "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Invalid UserObjectID should fail");

        }


        ///<summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void UpdateAlternateExtension_Failure()
        {
            WebCallResult res;

            //static UpdateAlternateExtension calls
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("name", "value");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, _user.ObjectId, "aaa", oProps);
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");

            res = AlternateExtension.UpdateAlternateExtension(null, _user.ObjectId, "aaa", oProps);
            Assert.IsFalse(res.Success, "Null ConnectionServer object should fail");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, "", "aaa", oProps);
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, "aaa", "aaa", oProps);
            Assert.IsFalse(res.Success, "Invalid UserObjectID should fail");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, _user.ObjectId, "aaa", null);
            Assert.IsFalse(res.Success, "Empty property list should fail");

        }

        /// <summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void AddAlternateExtension_Failure()
        {
            WebCallResult res;

            //Static AddAlternateExtension calls

            res = AlternateExtension.AddAlternateExtension(null, _user.ObjectId, 1, "1234");
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "", 1, "1234");
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "aaa", 1, "1234");
            Assert.IsFalse(res.Success, "Invalid UserOBjectID should fail");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, _user.ObjectId, 99, "1234");
            Assert.IsFalse(res.Success, "Invalid alternate extension index ID should fail");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, _user.ObjectId, 1, "");
            Assert.IsFalse(res.Success, "Empty extension string should fail");

        }

    }
}
