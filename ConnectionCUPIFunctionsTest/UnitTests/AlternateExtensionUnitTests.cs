using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for AlternateExtensionUnitTests and is intended
    ///to contain all AlternateExtensionUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class AlternateExtensionUnitTests : BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        [ClassInitialize]
        public new static void ClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #region Class Construction Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            AlternateExtension oTemp = new AlternateExtension(null, "aaa");
            Console.WriteLine(oTemp.ToString());
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ObjectId is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyObjectId_Failure()
        {
            AlternateExtension oTemp = new AlternateExtension(_mockServer, "");
            Console.WriteLine(oTemp.ToString());
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            var oTemp = new AlternateExtension();
            Console.WriteLine(oTemp.ToString());
            Console.WriteLine(oTemp.DumpAllProps());
            Console.WriteLine(oTemp.SelectionDisplayString);
            Console.WriteLine(oTemp.UniqueIdentifier);
        }

        #endregion


        #region Static Call Tests

        [TestMethod]
        public void DeleteAlternateExtension_EmptyObjectId_Failure()
        {
            //static delete failure calls
            var res = AlternateExtension.DeleteAlternateExtension(_mockServer, "objectid", "");
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");
        }

        [TestMethod]
        public void DeleteAlternateExtension_EmptyUserObjectId_Failure()
        {
            var res = AlternateExtension.DeleteAlternateExtension(_mockServer, "", "aaa");
            Assert.IsFalse(res.Success, "Empty user objectId should fail");

            }

        [TestMethod]
        public void DeleteAlternateExtension_NullConnectionServer_Failure()
        {
            var res = AlternateExtension.DeleteAlternateExtension(null, "objectid", "aaa");
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

        }


        [TestMethod]
        public void GetAlternateExtensions_NullConnectionServer_Failure()
        {
            List<AlternateExtension> oAltExts;

            //static GetAlternateExtensions calls
            var res = AlternateExtension.GetAlternateExtensions(null, "objectid", out oAltExts);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");
        }


        [TestMethod]
        public void GetAlternateExtension_NullConnectionServer_Failure()
        {
            AlternateExtension oAltExt;

            var res = AlternateExtension.GetAlternateExtension(null, "objectid", "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Null ConnectonServer object should fail");
        }


        [TestMethod]
        public void GetAlternateExtension_EmptyUserObjectId_Failure()
        {
            AlternateExtension oAltExt;

            var res = AlternateExtension.GetAlternateExtension(_mockServer, "", "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Empty UserverObjectId should fail");
        }


        [TestMethod]
        public void UpdateAlternateExtension_NullConnectionServer_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("name", "value");

            var res = AlternateExtension.UpdateAlternateExtension(null, "objectid", "aaa", oProps);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");
        }


        [TestMethod]
        public void UpdateAlternateExtension_EmptyUserObjectId_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("name", "value");

            var res = AlternateExtension.UpdateAlternateExtension(_mockServer, "", "aaa", oProps);
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail");
        }


        [TestMethod]
        public void UpdateAlternateExtension_EmptyPropertyList_Failure()
        {
            var res = AlternateExtension.UpdateAlternateExtension(_mockServer, "objectid", "aaa", null);
            Assert.IsFalse(res.Success, "Empty property list should fail");
        }


        [TestMethod]
        public void AddAlternateExtension_NullConnectionServer_Failure()
        {
            var res = AlternateExtension.AddAlternateExtension(null,"objectid", 1, "1234");
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

         }


        [TestMethod]
        public void AddAlternateExtension_EmptyUserObjectId_Failure()
        {
            var res = AlternateExtension.AddAlternateExtension(_mockServer, "", 1, "1234");
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail");
        }

        [TestMethod]
        public void AddAlternateExtension_InvalidIdIndex_Failure()
        {
            var res = AlternateExtension.AddAlternateExtension(_mockServer, "objectid", 99999, "1234");
            Assert.IsFalse(res.Success, "IdIndex greater than 20 should fail");
        }

        [TestMethod]
        public void AddAlternateExtension_EmptyExtension_Failure()
        {
            var res = AlternateExtension.AddAlternateExtension(_mockServer, "objectid", 1, "");
            Assert.IsFalse(res.Success, "Empty extension string should fail");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetAlternateExtension_ErrorResponse_Failure()
        {
            AlternateExtension oAltExt;

            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = false,
                                               ResponseText = "error text",
                                               StatusCode = 404
                                           });

            var res = AlternateExtension.GetAlternateExtension(_mockServer, "userObjectID", "ErrorResponse", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with server error response should fail");

        }
        [TestMethod]
        public void GetAlternateExtension_GarbageResponse_Failure()
        {
            AlternateExtension oAltExt;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            var res = AlternateExtension.GetAlternateExtension(_mockServer, "userObjectID", "InvalidResultText", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with InvalidResultText should fail");
         }

        [TestMethod]
        public void GetAlternateExtension_EmptyResult_Failure()
        {
            AlternateExtension oAltExt;
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

           var res = AlternateExtension.GetAlternateExtension(_mockServer, "userObjectID", "EmptyResultText", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with EmptyResultText should fail");
        }

        [TestMethod]
        public void GetAlternateExtension_ZeroCount_Failure()
        {
            AlternateExtension oAltExt;
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "Garbage",
                    TotalObjectCount = 0
                });

            var res = AlternateExtension.GetAlternateExtension(_mockServer, "userObjectID", "EmptyResultText", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with zero object count should fail");
        }



        [TestMethod]
        public void GetAlternateExtensions_ErrorResponse_Failure()
        {
            List<AlternateExtension> oAltExts;

            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = false,
                                               ResponseText = "error text",
                                               StatusCode = 404
                                           });

            var res = AlternateExtension.GetAlternateExtensions(_mockServer, "ErrorResponse", out oAltExts);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtensions with server error response should fail");

        }

        [TestMethod]
        public void GetAlternateExtensions_GarbageResponse_Failure()
        {
            List<AlternateExtension> oAltExts;
            //garbage response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               TotalObjectCount = 1,
                                               ResponseText = "garbage result"
                                           });

            var res = AlternateExtension.GetAlternateExtensions(_mockServer, "InvalidResultText", out oAltExts);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtensions with InvalidResultText should fail:");
            Assert.IsTrue(oAltExts.Count == 0,"Invalid text should result in empty list of alternate extensions being returned");
        }

        [TestMethod]
        public void GetAlternateExtensions_EmptyResponse_Success()
        {
            List<AlternateExtension> oAltExts;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = AlternateExtension.GetAlternateExtensions(_mockServer, "EmptyResultText", out oAltExts);
            Assert.IsTrue(res.Success, "Calling GetAlternateExtensions with EmptyResultText should not fail:"+res);
            Assert.IsTrue(oAltExts.Count == 0, "Empty text should result in empty list of alternate extensions being returned");
        }

        [TestMethod]
        public void GetAlternateExtensions_ZeroCountResponse_Success()
        {
            List<AlternateExtension> oAltExts;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "garbage result",
                    TotalObjectCount = 0
                });

            var res = AlternateExtension.GetAlternateExtensions(_mockServer, "EmptyResultText", out oAltExts);
            Assert.IsTrue(res.Success, "Calling GetAlternateExtensions with zero count should not fail:" + res);
            Assert.IsTrue(oAltExts.Count == 0, "Zero count response should result in empty list of alternate extensions being returned");
        }


        [TestMethod]
        public void DeleteAlternateExtension_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                       {
                                           Success = false,
                                           ResponseText = "error text",
                                           StatusCode = 404
                                       });

            var res = AlternateExtension.DeleteAlternateExtension(_mockServer, "UserObjectId", "ObjectId");
            Assert.IsFalse(res.Success, "Calling DeleteAlternateExtension with server error response should fail");

        }

        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            AlternateExtension oAltExt = new AlternateExtension();
            var res = oAltExt.Update(true);
            Assert.IsFalse(res.Success,"Calling Update with no pending changes should fail");
        }

        [TestMethod]
        public void Update_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                       {
                                           Success = false,
                                           ResponseText = "dummy text",
                                           StatusCode = 444
                                       });

            AlternateExtension oAltExt = new AlternateExtension(_mockServer,"UserObjectId");
            oAltExt.DisplayName = "updated display name";
            var res = oAltExt.Update(true);
            Assert.IsFalse(res.Success,"Calling update with an error response should fail:"+res);
            Assert.IsTrue(res.StatusCode==444,"Status code returned from updated call did not match error code issues in moq:"+res.StatusCode);
        }

        #endregion

        #region Property Tests

        [TestMethod]
        public void PropertyGetFetch_IdIndex()
        {
            AlternateExtension oAltExt = new AlternateExtension();
            const int expectedValue = 2;
            oAltExt.IdIndex = expectedValue;
            Assert.IsTrue(oAltExt.ChangeList.ValueExists("IdIndex", expectedValue),"IdIndex value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            AlternateExtension oAltExt = new AlternateExtension();
            const string expectedValue = "test string value";
            oAltExt.DisplayName = expectedValue;
            Assert.IsTrue(oAltExt.ChangeList.ValueExists("DisplayName", expectedValue), "DisplayName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DtmfAccessId()
        {
            AlternateExtension oAltExt = new AlternateExtension();
            const string expectedValue = "test string value";
            oAltExt.DtmfAccessId = expectedValue;
            Assert.IsTrue(oAltExt.ChangeList.ValueExists("DtmfAccessId", expectedValue), "DtmfAccessId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_LocationObjectId()
        {
            AlternateExtension oAltExt = new AlternateExtension();
            const string expectedValue = "test string value";
            oAltExt.LocationObjectId = expectedValue;
            Assert.IsTrue(oAltExt.ChangeList.ValueExists("LocationObjectId", expectedValue), "LocationObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PartitionObjectId()
        {
            AlternateExtension oAltExt = new AlternateExtension();
            const string expectedValue = "test string value";
            oAltExt.PartitionObjectId = expectedValue;
            Assert.IsTrue(oAltExt.ChangeList.ValueExists("PartitionObjectId", expectedValue), "PartitionObjectId value get fetch failed");
        }

        [TestMethod]
        public void AddAlternateExtension_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = false,
                                           ResponseText = "error text",
                                           StatusCode = 404
                                       });

            AlternateExtension oAltExt;
            var res = AlternateExtension.AddAlternateExtension(_mockServer, "userObjectID", 1,"1234",out oAltExt);
            Assert.IsFalse(res.Success, "Calling AddAlternateExtension with server error response should fail");

        }

        [TestMethod]
        public void AddCallHandler_JunkObjectIdReturn_Failure()
        {
            //invalid objectId response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                 It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                 {
                                     Success = true,
                                     ResponseText = "/vmrest/users/userObjectID/alternateextensions/junk"
                                 });

            AlternateExtension oAltExt;
            var res = AlternateExtension.AddAlternateExtension(_mockServer, "userObjectID", 1, "1234", out oAltExt);
            Assert.IsFalse(res.Success, "Calling AddAlternateExtension with invalid return objectId should fail");
        }

        #endregion
    }
}
