using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for HTTPFunctionsTest and is intended
    ///to contain all HTTPFunctionsTest Unit Tests
    ///</summary>
    [TestClass]
    public class HttpFunctionsUnitTests : BaseUnitTests
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


        [TestMethod]
        public void UploadWavFile_InvalidResourcePath_Failure()
        {
            WebCallResult res = _mockServer.UploadWavFile("bogusresourcepath", "Dummy.wav");
            Assert.IsFalse(res.Success, "Invalid resource path should fail");
        }

        [TestMethod]
        public void UploadWavFile_EmptyResourcePath_Failure()
        {

            var res = _mockServer.UploadWavFile("", "Dummy.wav");
            Assert.IsFalse(res.Success, "Empty resource path should fail");

        }
        [TestMethod]
        public void UploadWavFile_NonExistentPath_Failure()
        {
            var res = _mockServer.UploadWavFile("bogusresourcepath", "");
            Assert.IsFalse(res.Success, "File path that does not exist should fail");
                
        }

   }
}
