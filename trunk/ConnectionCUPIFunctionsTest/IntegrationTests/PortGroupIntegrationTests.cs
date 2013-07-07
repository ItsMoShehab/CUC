using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortGroupIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
        }

        #endregion


        #region Class Construction Failures

        /// <summary>
        /// Valid connection server but invalid object ID should fail with UnityConnectionRestException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            PortGroup oPorts = new PortGroup(_connectionServer, "blah");
            Console.WriteLine(oPorts);
        }

        #endregion


        #region Live Tests

        private PortGroup GetPortGroup()
        {
            List<PortGroup> oPortGroups;
            WebCallResult res = PortGroup.GetPortGroups(_connectionServer, out oPortGroups);
            Assert.IsTrue(res.Success, "Fetching port groups failed:" + res);
            Assert.IsTrue(oPortGroups.Count>0,"No port groups found");
            return oPortGroups[0];
        }

        [TestMethod]
        public void GetPortGroups_Success()
        {
            var oPortGroup = GetPortGroup();

            Console.WriteLine(oPortGroup.ToString());
            Console.WriteLine(oPortGroup.DumpAllProps());
        }

        [TestMethod]
        public void PortGroup_Constructor_WithObjectId_Success()
        {
            var oPortGroup = GetPortGroup();
            try
            {
                PortGroup oNewGroup = new PortGroup(_connectionServer, oPortGroup.ObjectId);
                Console.WriteLine(oNewGroup);
            }
            catch (Exception ex)
            {
                Assert.Fail("PortGroup creation with valid ObjectId of PortGroup failed:" + ex);
            }
        }

        [TestMethod]
        public void PortGroup_Constructor_WithDisplayName_Success()
        {
            var oPortGroup = GetPortGroup();
            try
            {
                PortGroup oNewGroup = new PortGroup(_connectionServer, "", oPortGroup.DisplayName);
                Console.WriteLine(oNewGroup);
            }
            catch (Exception ex)
            {
                Assert.Fail("PortGroup creation with valid display name of PortGroup failed:" + ex);
            }
        }

        [TestMethod]
        public void PortGroup_Constructor_InvalidDisplayName_Failure()
        {
            try
            {
                PortGroup oNewGroup = new PortGroup(_connectionServer, "", "bogus");
                Assert.Fail("PortGroup creation with invalid display name of PortGroup did not fail:" + oNewGroup);
            }
            catch (Exception)
            {
                Console.WriteLine("Expected creation error with invalid display name");
            }

        }

        [TestMethod]
        public void PortGroup_Constructor_InvalidObjectId_Failure()
        {
            try
            {
                PortGroup oNewGroup = new PortGroup(_connectionServer, "bogus");
                Assert.Fail("PortGroup creation with invalid objectId of PortGroup did not fail:" + oNewGroup);
            }
            catch (Exception)
            {
                Console.WriteLine("Expected creation error with invalid objectID");
            }
        }

        #endregion
    }
}
