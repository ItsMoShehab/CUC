using Cisco.UnityConnection.RestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    
    
    /// <summary>
    ///This is a test class for HTTPFunctionsTest and is intended
    ///to contain all HTTPFunctionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HTTPFunctionsTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Test for WAV upload call failures.
        /// Note that "Dummy.wav" does exist in the output folder the test is being run from.
        ///</summary>
        [TestMethod]
        public void UploadWAVFile_Failure()
                {
                    WebCallResult res;

                    res = HTTPFunctions.UploadWavFile("bogusresourcepath", "BogusLogin", "BogusPassword", "Dummy.wav");
            Assert.IsFalse(res.Success,"Invalid resource path should fail");

                    res = HTTPFunctions.UploadWavFile("", "BogusLogin", "BogusPassword", "Dummy.wav");
                    Assert.IsFalse(res.Success, "Empty resource path should fail");

                    res = HTTPFunctions.UploadWavFile("bogusresourcepath", "", "BogusPassword", "Dummy.wav");
                    Assert.IsFalse(res.Success, "empty login or password should fail");

                    res = HTTPFunctions.UploadWavFile("bogusresourcepath", "BogusLogin", "BogusPassword", "");
                    Assert.IsFalse(res.Success, "File path that does not exist should fail");

                }


   }
}
