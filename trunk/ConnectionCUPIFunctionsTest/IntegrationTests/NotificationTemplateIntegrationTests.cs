using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class NotificationTemplateIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ObjectID is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure()
        {
            NotificationTemplate oTemp = new NotificationTemplate(_connectionServer, "aaa");
            Console.WriteLine(oTemp);
        }

        #endregion


        [TestMethod]
        public void GetNotificationTemplates_Success()
        {
            List<NotificationTemplate> oDevices;

            WebCallResult res = NotificationTemplate.GetNotificationTemplates(_connectionServer, out oDevices);
            Assert.IsTrue(res.Success, "Failed to fetch HTTP notification devices from server");
            Assert.IsTrue(oDevices.Count > 0, "No HTTP notification templates found on server");

            //exercise tostring and dumpallprops
            Console.WriteLine(oDevices[0].ToString());
            Console.WriteLine(oDevices[0].DumpAllProps());
        }

        [TestMethod]
        public void NotificationTemplate_ConstructorWithObjectId_Success()
        {
            List<NotificationTemplate> oDevices;

            WebCallResult res = NotificationTemplate.GetNotificationTemplates(_connectionServer, out oDevices);
            Assert.IsTrue(res.Success, "Failed to fetch HTTP notification devices from server");
            Assert.IsTrue(oDevices.Count > 0, "No HTTP notification templates found on server");

            try
            {
                NotificationTemplate oNewTemplate = new NotificationTemplate(_connectionServer,oDevices[0].NotificationTemplateId);
                Console.WriteLine(oNewTemplate);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new NotificationTemplate instance off NEW keyword with valid ObjectId:" +ex);
            }
        }


        [TestMethod]
        public void NotificationTemplate_EmptyConstructor_Success()
        {
            try
            {
                NotificationTemplate oNewTemplate = new NotificationTemplate(_connectionServer);
                Console.WriteLine(oNewTemplate);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new NotificationTemplate instance off NEW keyword with no ObjectId:" + ex);
            }
        }

    }
}
