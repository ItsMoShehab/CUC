﻿using System;
using System.Collections.Generic;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RoleTest
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
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

        }

        #endregion


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            Role oTest = new Role(null);
        }

        
        /// <summary>
        /// Make sure an exception is thrown for an invalid ObjectID
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            Role oTest = new Role(_connectionServer,"bogus");
        }

        /// <summary>
        /// Make sure an exception is thrown on an invalid name
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            Role oTest = new Role(_connectionServer,"","bogus");
        }


        [TestMethod]
        public void TestMethod1()
        {
            List<Role> oRoles;
            WebCallResult res= Role.GetRoles(_connectionServer, out oRoles);
            Assert.IsTrue(res.Success,"Failed to fetch roles list:"+res);

            foreach (var oRole in oRoles)
            {
                Console.WriteLine(oRole.ToString());
            }

            res = Role.GetRoles(null, out oRoles);
            Assert.IsFalse(res.Success,"Static fetch of rules did not fail with null ConnectionServer");
            string strObjectId = "";
            try
            {
                Role oNewRole = new Role(_connectionServer, "", "Audio Text Administrator");
                strObjectId = oNewRole.ObjectId;
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false,"Class construction with valid role name failed:"+ex);
            }

            try
            {
                Role oNewRole = new Role(_connectionServer, strObjectId);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, "Class construction with valid objectId failed:" + ex);
            }


        }
    }
}