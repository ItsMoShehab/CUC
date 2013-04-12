﻿using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class NotificationTemplateTest
    {
        
            //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        //class wide user reference for testing - gets filled in with operator user details
        private static UserBase _user;

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
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

            //get the operator to work with here
            try
            {
                _user = new UserBase(_connectionServer, "", "operator");
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to get operator user for testing:"+ex.Message);
            }

        }

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            NotificationTemplate oTemp = new NotificationTemplate(null, "aaa");
        }

        /// <summary>
        /// Make sure an Exception is thrown if an invalid ObjectID is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            NotificationTemplate oTemp = new NotificationTemplate(_connectionServer, "aaa");
        }

        /// <summary>
        /// Testing notification template failure scenarios
        /// </summary>
        [TestMethod]
        public void GetNotificationTemplate_Failure()
        {
            WebCallResult res;
            List<NotificationTemplate> oDevices;

            //get Notification Device list failure points.
            res = NotificationTemplate.GetNotificationTemplates(null, out oDevices);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");
        }

        /// <summary>
        /// Testing notification template failure scenarios
        /// </summary>
        [TestMethod]
        public void GetNotificationTemplate_Fetches()
        {
            WebCallResult res;
            List<NotificationTemplate> oDevices;

            //get Notification Device list failure points.
            res = NotificationTemplate.GetNotificationTemplates(_connectionServer, out oDevices);
            Assert.IsTrue(res.Success, "Failed to fetch HTTP notification devices from server");
            Assert.IsTrue(oDevices.Count>0,"No HTTP notification templates found on server");

            //create new valid device
            try
            {
                NotificationTemplate oNewTemplate = new NotificationTemplate(_connectionServer,
                                                                             oDevices[0].NotificationTemplateId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new NotificationTemplate instance off NEW keyword with valid ObjectId:"+ex);
            }

            //create new empty device
            try
            {
                NotificationTemplate oNewTemplate = new NotificationTemplate(_connectionServer,"");
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new NotificationTemplate instance off NEW keyword with no ObjectId:" + ex);
            }




            foreach (var oDevice in oDevices)
            {
                Console.WriteLine(oDevice.ToString());
                Console.WriteLine(oDevice.DumpAllProps());
            }


        }

    }
}
