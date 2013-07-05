using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class NotificationTemplateUnitTests : BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
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

        #endregion

        [TestMethod]
        public void GetNotificationTemplate_NullConnectionServer_Failure()
        {
            List<NotificationTemplate> oDevices;

            WebCallResult res = NotificationTemplate.GetNotificationTemplates(null, out oDevices);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");
        }

        [TestMethod]
        public void NotificationTemplate_EmptyConstructor_Success()
        {
            try
            {
                NotificationTemplate oNewTemplate = new NotificationTemplate(_mockServer);
                Console.WriteLine(oNewTemplate.ToString());
                Console.WriteLine(oNewTemplate.DumpAllProps());
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new NotificationTemplate instance off NEW keyword with no ObjectId:" + ex);
            }
        }

    }
}
