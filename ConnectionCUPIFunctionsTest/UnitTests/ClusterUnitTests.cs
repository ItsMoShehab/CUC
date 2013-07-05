using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Cluster is a simple read only class that has very little logic other than it's cosntrctor
    /// </summary>
    [TestClass]
    public class ClusterUnitTests : BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes
        // 
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Class Construction Errors

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            Cluster oTest = new Cluster(null);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an empty Connection server is passed in
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            Cluster oTest = new Cluster(new ConnectionServerRest(new RestTransportFunctions()));
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void ClusterConstructor_EmptyResult_Failure()
        {

            Cluster oCluster = null;

            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            try
            {
                oCluster = new Cluster(_mockServer);
                Assert.Fail("Creating cluster with empty response text should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("expected error:" + ex);
            }

            try
            {
                Console.WriteLine(oCluster.Servers.Count);
                Assert.Fail("Getting server count with invalid cluster fetch should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected Failure getting servers count:" + ex);
            }
        }


        [TestMethod]
        public void ClusterConstructor_ErrorResponse_Failure()
        {
            Cluster oCluster = null;

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            try
            {
                oCluster = new Cluster(_mockServer);
                Assert.Fail("Creating cluster with error response should fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine("expected error:" + ex);
            }
        }

        #endregion
    }
}
