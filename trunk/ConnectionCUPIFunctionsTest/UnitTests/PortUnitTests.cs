using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortUnitTests : BaseUnitTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Class Creation Failures
        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            Port oPort = new Port(null);
            Console.WriteLine(oPort);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidConnectionServer_Failure()
        {
            Port oPort = new Port(new ConnectionServerRest(new RestTransportFunctions()),"blah");
            Console.WriteLine(oPort);
        }

        #endregion


        #region Static Call Failures 

        [TestMethod]
        public void UpdatePort_NullConnectionServer_Failure()
        {
            var res = Port.UpdatePort(null, "objectid", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePort did not fail with null Connection server");
        }

        [TestMethod]
        public void UpdatePort_EmptyObjectId_Failure()
        {
            var res = Port.UpdatePort(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdatePort did not fail with empty objectid");
        }

        [TestMethod]
        public void AddPort_NullConnectionServer_Failure()
        {
            var res = Port.AddPort(null, "portgroupid", 4, null);
            Assert.IsFalse(res.Success, "Static call to AddPort did not fail with null connection server");

                    }

        [TestMethod]
        public void AddPort_EmptyObjectId_Failure()
        {
            var res = Port.AddPort(_mockServer, "", 4, null);
            Assert.IsFalse(res.Success, "Static call to AddPort did not fail with empty port group objectId");
        }

        [TestMethod]
        public void DeletePort_NullConnectionServer_Failure()
        {
            var res = Port.DeletePort(null, "objectId");
            Assert.IsFalse(res.Success, "Static call to DeletePort did not fail with null Connection server");

        }

        [TestMethod]
        public void DeletePort_EmptyObjectId_Failure()
        {
            var res = Port.DeletePort(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to DeletePort did not fail with empty objectId");
        }

        [TestMethod]
        public void GetPort_NullConnectionServer_Failure()
        {
            Port oPort;
            WebCallResult res = Port.GetPort(out oPort, null, "objectId");
            Assert.IsFalse(res.Success,"Static call to GetPort did not fail with null Connection server");

            }

        [TestMethod]
        public void GetPort_EmptyObjectId_Failure()
        {
            Port oPort;
            var res = Port.GetPort(out oPort, _mockServer, "");
            Assert.IsFalse(res.Success, "Static call to GetPort did not fail with empty objectId");
        }

        #endregion

    }
}
