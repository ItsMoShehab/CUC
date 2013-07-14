using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Test for the credential class - just construction failure tests in here - the User tests contain most of the coverage
    /// for fetching/listing PIN and Password credential details.
    /// </summary>
    [TestClass]
    public class CredentialUnitTests : BaseUnitTests
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


        #region Class Construction Tests 

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            Credential oTest = new Credential(null,"blah",CredentialType.Pin);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an exception is thrown if an empty ObjectId is presented
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyObjectId_Failure()
        {
            Credential oTest = new Credential(_mockServer, "", CredentialType.Pin);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            Credential oTest = new Credential(_mockServer, "ObjectId", CredentialType.Pin);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            Credential oTest = new Credential();
            Console.WriteLine(oTest.ToString());
            Console.WriteLine(oTest.DumpAllProps());
        }


        #endregion


        #region Static Methods 

        [TestMethod]
        public void GetCredential_NullConnectionServer_Failure()
        {
            Credential oCredential;

            var res = Credential.GetCredential(null, "UserObjectId",CredentialType.Password, out oCredential);
            Assert.IsFalse(res.Success, "GetCredential should fail with null ConnectionServerRest passed to it");
        }

        [TestMethod]
        public void GetCredential_EmptyObjectId_Failure()
        {
            Credential oCredential;

            var res = Credential.GetCredential(_mockServer, "", CredentialType.Password, out oCredential);
            Assert.IsFalse(res.Success, "GetCredential should fail with empty UserObjectID  passed to it");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetCredential_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            Credential oCredential;
            var res = Credential.GetCredential(_mockServer, "UserObjectId", CredentialType.Password, out oCredential);
            Assert.IsFalse(res.Success, "GetCredential should fail with empty results");
        }


        [TestMethod]
        public void GetCredential_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            Credential oCredential;
            var res = Credential.GetCredential(_mockServer, "UserObjectId", CredentialType.Password, out oCredential);
            Assert.IsFalse(res.Success, "GetCredential should fail with garbage results");
        }


        [TestMethod]
        public void GetCredential_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            Credential oCredential;
            var res = Credential.GetCredential(_mockServer, "UserObjectId", CredentialType.Password, out oCredential);
            Assert.IsFalse(res.Success, "GetCredential should fail with error results");
        }

        [TestMethod]
        public void GetCredential_ValidResponse_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(),It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "{\"ObjectID\":\"testObjectId\",\"Alias\":\"testAlias\"}"
                                    });

            Credential oCredential;
            var res = Credential.GetCredential(_mockServer, "UserObjectId", CredentialType.Pin, out oCredential);
            Assert.IsTrue(res.Success, "GetCredential call failed:"+res);
        }

        #endregion
    }
}
