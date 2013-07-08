using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RestrictionTableUnitTests : BaseUnitTests
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


        #region Class Construction Error Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestrictionTable_Constructor_NullConnectionServer_Failure()
        {
            RestrictionTable oTest = new RestrictionTable(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException if invalid name passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RestrictionTable_Constructor_EmptyObjectId_Failure()
        {
            RestrictionTable oTest = new RestrictionTable(_mockServer, "", "bogusDisplayName");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestrictionTable_Constructor_NullConnectionServerWithObjectIds_Failure()
        {
            RestrictionPattern oTest = new RestrictionPattern(null,"bogus","bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// ArgumentException passed if empty objectId passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestrictionPattern_Constructor_EmptyObjectId_Failure()
        {
            RestrictionPattern oTest = new RestrictionPattern(_mockServer, "", "bogus");
            Console.WriteLine(oTest);
        }


        #endregion


        #region Static Call Failures

        [TestMethod]
        public void GetRestrictionTables_NullConnectionServer_Failure()
        {
            //static fetch failures
            List<RestrictionTable> oTables;
            var res = RestrictionTable.GetRestrictionTables(null, out oTables);
            Assert.IsFalse(res.Success, "Static restriction table creation did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void GetRestrictionPatterns_NullConnectionServer_Failure()
        {
            List<RestrictionPattern> oPatterns;
            var res = RestrictionPattern.GetRestrictionPatterns(null, "bogus", out oPatterns);
            Assert.IsFalse(res.Success, "Static call to GetRestrictionPattners did not fail with null Connection server");
        }

        [TestMethod]
        public void GetRestrictionPatterns_EmptyObjectId_Failure()
        {
            List<RestrictionPattern> oPatterns;

            var res = RestrictionPattern.GetRestrictionPatterns(_mockServer, "", out oPatterns);
            Assert.IsFalse(res.Success, "Static call to GetRestrictionPattners did not fail with empty objectId");
        }

        #endregion


        #region Test Harness

        // EmptyResultText, InvalidResultText, ErrorResponse
        [TestMethod]
        public void RestrictionPattern_Constructor_EmptyResponse_Failure()
        {
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
                RestrictionPattern oPattern = new RestrictionPattern(_mockServer, "EmptyResultText", "objectid");
                Assert.Fail("Creating restriction pattern with empty response text should fail");
            }
            catch
            {
            }
        }

        [TestMethod]
        public void RestrictionPattern_Constructor_GarbageResponse_Failure()
        {
            //garbage response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = "garbage result",
                                               TotalObjectCount = 1
                                           });

            try
            {
                RestrictionPattern oPattern = new RestrictionPattern(_mockServer, "InvalidResultText", "objectid");
                Assert.Fail("Creating restriction pattern with garbage response text should fail");
            }
            catch
            {
            }
        }

        [TestMethod]
        public void RestrictionPattern_Constructor_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            try
            {
                RestrictionPattern oPattern = new RestrictionPattern(_mockServer, "ErrorResponse", "objectid");
                Assert.Fail("Creating restriction pattern with error response text should fail");
            }
            catch { }

        }

        [TestMethod]
        public void GetRestrictionPatterns_EmptyResponse_Failure()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,ResponseText = ""
                });

            List<RestrictionPattern> oPatterns;
            var res = RestrictionPattern.GetRestrictionPatterns(_mockServer, "EmptyResultText", out oPatterns, 1, 2);
            Assert.IsFalse(res.Success, "Calling GetRestrictionPatterns with empty result text should fail");
            Assert.IsTrue(oPatterns.Count==0,"Empty response text should result in an empty list");

         }

        [TestMethod]
        public void GetRestrictionPatterns_GarbageResponse_Success()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,ResponseText = "garbage result"
                                  });
            List<RestrictionPattern> oPatterns;
            var res = RestrictionPattern.GetRestrictionPatterns(_mockServer, "InvalidResultText", out oPatterns, 1, 2);
            Assert.IsTrue(res.Success, "Calling GetRestrictionPatterns with InvalidResultText should not fail:"+res);
            Assert.IsTrue(oPatterns.Count == 0, "Invalid response text should result in an empty list");

         }

        [TestMethod]
        public void GetRestrictionPatterns_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,ResponseText = "error text",StatusCode = 404
                                    });
            List<RestrictionPattern> oPatterns;
            var res = RestrictionPattern.GetRestrictionPatterns(_mockServer, "ErrorResponse", out oPatterns, 1, 2);
            Assert.IsFalse(res.Success, "Calling GetRestrictionPatterns with ErrorResponse should fail");
            Assert.IsTrue(oPatterns.Count == 0, "Error response should result in an empty list");
        }

        [TestMethod]
        public void GetRestrictionTables_EmptyResponse_Failure()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            List<RestrictionTable> oTables;
            var res = RestrictionTable.GetRestrictionTables(_mockServer, out oTables, 1, 2, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetRestrictionTables with empty result text should fail");
            Assert.IsTrue(oTables.Count == 0, "Empty response text should result in an empty list");

            }

        [TestMethod]
        public void GetRestrictionTables_GarbageResponse_Success()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });
            List<RestrictionTable> oTables;
            var res = RestrictionTable.GetRestrictionTables(_mockServer, out oTables, 1, 2, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetRestrictionTables with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oTables.Count == 0, "Invalid response text should result in an empty list");

            }

        [TestMethod]
        public void GetRestrictionTables_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            List<RestrictionTable> oTables;
            var res = RestrictionTable.GetRestrictionTables(_mockServer, out oTables, 1, 2, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetRestrictionTables with ErrorResponse should fail");
            Assert.IsTrue(oTables.Count == 0, "Error response should result in an empty list");
        }


        [TestMethod]
        public void RestrictionTable_Constructor_EmptyResponse_Failure()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            try
            {
                RestrictionTable oTable = new RestrictionTable(_mockServer, "EmptyResultText");
                Assert.Fail("Creating restriction table with empty response text should fail");
            }
            catch { }

            }


        [TestMethod]
        public void RestrictionTable_Constructor_GarbageResponse_Failure()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            try
            {
                RestrictionTable oTable = new RestrictionTable(_mockServer, "InvalidResultText");
                Assert.Fail("Creating restriction table with garbage response text should fail");
            }
            catch { }

            }


        [TestMethod]
        public void RestrictionTable_Constructor_ErrorResponse_Failure()
        {
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
                RestrictionTable oTable = new RestrictionTable(_mockServer, "ErrorResponse");
                Assert.Fail("Creating restriction table with error response text should fail");
            }
            catch { }

        }

        #endregion
    }
}
