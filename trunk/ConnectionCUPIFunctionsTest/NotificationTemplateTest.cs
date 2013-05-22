using System.Collections.Generic;
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

        // 
        //You can use the following additional attributes as you write your tests:
        //
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
                _connectionServer = new ConnectionServer(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start NotificationTemplate test:" + ex.Message);
            }
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            NotificationTemplate oTemp = new NotificationTemplate(null, "aaa");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ObjectID is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            NotificationTemplate oTemp = new NotificationTemplate(_connectionServer, "aaa");
            Console.WriteLine(oTemp);
        }

        #endregion

        /// <summary>
        /// Testing notification template failure scenarios
        /// </summary>
        [TestMethod]
        public void GetNotificationTemplate_Failure()
        {
            List<NotificationTemplate> oDevices;

            //get Notification Device list failure points.
            WebCallResult res = NotificationTemplate.GetNotificationTemplates(null, out oDevices);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");
        }

        /// <summary>
        /// Testing notification template failure scenarios
        /// </summary>
        [TestMethod]
        public void GetNotificationTemplate_Fetches()
        {
            List<NotificationTemplate> oDevices;

            //get Notification Device list failure points.
            WebCallResult res = NotificationTemplate.GetNotificationTemplates(_connectionServer, out oDevices);
            Assert.IsTrue(res.Success, "Failed to fetch HTTP notification devices from server");
            Assert.IsTrue(oDevices.Count>0,"No HTTP notification templates found on server");

            //create new valid device
            try
            {
                NotificationTemplate oNewTemplate = new NotificationTemplate(_connectionServer,
                                                                             oDevices[0].NotificationTemplateId);
                Console.WriteLine(oNewTemplate);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new NotificationTemplate instance off NEW keyword with valid ObjectId:"+ex);
            }

            //create new empty device
            try
            {
                NotificationTemplate oNewTemplate = new NotificationTemplate(_connectionServer);
                Console.WriteLine(oNewTemplate);
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
