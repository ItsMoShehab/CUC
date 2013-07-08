using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RtpCodecDefUnitTests : BaseUnitTests
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
            RtpCodecDef oTemp = new RtpCodecDef(null);
            Console.WriteLine(oTemp);
        }

        #endregion


        [TestMethod]
        public void GetRtpCodecDefs_NullConnectionServer_Failure()
        {
            List<RtpCodecDef> oList;
            WebCallResult res = RtpCodecDef.GetRtpCodecDefs(null, out oList);
            Assert.IsFalse(res.Success, "Static call to GetRtpCodecDefs did not fail with: null ConnectionServer");
        }

    }
}
