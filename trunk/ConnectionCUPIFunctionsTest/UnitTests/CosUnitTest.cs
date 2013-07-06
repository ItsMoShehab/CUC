using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class CosUnitTest : BaseUnitTests
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
        public void Constuctor_NullConnectionServer_Failure()
        {
            ClassOfService oTemp = new ClassOfService(null);
            Console.WriteLine(oTemp);
        }

        [ExpectedException(typeof(Exception))]
        public void Constructor_NullConnectionServerWithNonEmptyOBjectId_Failure()
        {
            ClassOfService oTemp = new ClassOfService(null,"bogus");
            Console.WriteLine(oTemp);
        }


        #endregion


        #region Static Call Failures

        [TestMethod]
        public void DeleteClassOfService_NullConnectionServer_Failure()
        {
            //DeleteClassOfService
            var res = ClassOfService.DeleteClassOfService(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: null connectionServer");
        }

        [TestMethod]
        public void DeleteClassOfService_EmptyObjectId_Failure()
        {
            var res = ClassOfService.DeleteClassOfService(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: empty objectid");
        }

        [TestMethod]
        public void GetClassOfService_NullConnectionServer_Failure()
        {
            ClassOfService oCos;
            var res = ClassOfService.GetClassOfService(out oCos, null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void GetClassOfService_EmptyObjectIdAndName_Failure()
        {
            ClassOfService oCos;
            var res = ClassOfService.GetClassOfService(out oCos, _mockServer);
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: empty objectId and Name");
        }

        [TestMethod]
        public void UpdateClassOfService_NullConnectionServer_Failure()
        {
            //GetClassesOfService
            var res = ClassOfService.UpdateClassOfService(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: null ConnectionServer");

         }

        [TestMethod]
        public void UpdateClassOfService_EmptyObjectId_Failure()
        {
            var res = ClassOfService.UpdateClassOfService(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: empty objectId");
        }

        [TestMethod]
        public void GetClassesOfService_NullConnectionServer_Failure()
        {
            List<ClassOfService> oCoses;
            var res = ClassOfService.GetClassesOfService(null, out oCoses);
            Assert.IsFalse(res.Success, "Static call to GetClassesOfService did not fail with: null ConnectionServer");

        }

        [TestMethod]
        public void AddClassOfService_NullConnectionServer_Failure()
        {
            //AddClassOfService
            WebCallResult res = ClassOfService.AddClassOfService(null, "display", null);
            Assert.IsFalse(res.Success, "Static call to AddClassOfSerice did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void AddClassOfService_EmptyObjectId_Failure()
        {
            var res = ClassOfService.AddClassOfService(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Static call to AddClassOfSerice did not fail with: empty objectId");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetClassesOfService_EmptyResults_Failure()
        {

            List<ClassOfService> oCoses;

            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            var res = ClassOfService.GetClassesOfService(_mockServer, out oCoses, 1, 5, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetClassesOfService with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetClassesOfService_GarbageResponse_Success()
        {
           
            List<ClassOfService> oCoses;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            var res = ClassOfService.GetClassesOfService(_mockServer, out oCoses, 1, 5, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetClassesOfService with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oCoses.Count == 0, "Invalid result text should produce an empty list of Coeses");

            }

        [TestMethod]
        public void GetClassesOfService_ErrorResponse_Failure()
        {

            List<ClassOfService> oCoses;
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = ClassOfService.GetClassesOfService(_mockServer, out oCoses, 1, 5, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetClassesOfService with ErrorResponse did not fail");

        }

        #endregion
    }
}
