using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortGroupUnitTests : BaseUnitTests
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


        #region Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            PortGroup oPorts = new PortGroup(null);
            Console.WriteLine(oPorts);
        }


        /// <summary>
        /// Empty Connection class instance should fail with UnityConnectionRestException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_EmptyConnectionServer_Failure()
        {
            PortGroup oPorts = new PortGroup(new ConnectionServerRest(new RestTransportFunctions()), "blah");
            Console.WriteLine(oPorts);
        }

        #endregion


        [TestMethod]
        public void GetPortGroups_NullConnectionServer_Failure()
        {
            List<PortGroup> oPortGroups;

            WebCallResult res = PortGroup.GetPortGroups(null, out oPortGroups);
            Assert.IsFalse(res.Success, "Fetching port groups with null Connection server should fail.");
        }

        [TestMethod]
        public void GetPortGroups_EmptyConnectionServer_Failure()
        {
            List<PortGroup> oPortGroups;

            var res = PortGroup.GetPortGroups(new ConnectionServerRest(new RestTransportFunctions()), out oPortGroups);
            Assert.IsFalse(res.Success, "Fetching port groups with invalid Connection server should fail.");
        }
    }
}
