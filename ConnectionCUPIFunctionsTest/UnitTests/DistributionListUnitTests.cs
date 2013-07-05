using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for DistributionListUnitTests and is intended
    ///to contain all DistributionListUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class DistributionListUnitTests : BaseUnitTests
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
        public void ClassCreation_Failure()
        {

            DistributionList oTestList = new DistributionList(null);
            Console.WriteLine(oTestList);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void GetDistributionList_NullConnectionServer_Failure()
        {
            DistributionList oList;

            WebCallResult res = DistributionList.GetDistributionList(out oList, null, "", "allvoicemailusers");
            Assert.IsFalse(res.Success, "Null Connection server on GetDistributionList did not fail.");

        }
        [TestMethod]
        public void GetDistributionList_BlankAliasAndName_Failure()
        {
            DistributionList oList;
            var res = DistributionList.GetDistributionList(out oList, _mockServer);
            Assert.IsFalse(res.Success, "Blank alias/objectID params on GetDistributionList did not fail");
        }

        [TestMethod]
        public void GetDistributionLists_NullConnectionServer_Failure()
        {
            List<DistributionList> oList;
            WebCallResult res = DistributionList.GetDistributionLists(null, out oList, null);

            Assert.IsFalse(res.Success, "GetDistributionLists failed to catch null ConnectionServerRest object");
        }


        [TestMethod]
        public void AddDistributionList_NullConnectionServer_Failure()
        {
            WebCallResult res = DistributionList.AddDistributionList(null, "aaa", "aaa", "123", null);
            Assert.IsFalse(res.Success, "AddDistributionList failed to catch null ConnectionServerRest object");
        }

        [TestMethod]
        public void AddDistributionList_EmptyAliasAndName_Failure()
        {
            var res = DistributionList.AddDistributionList(_mockServer, "", "", "123", null);
            Assert.IsFalse(res.Success, "AddDistributionList failed to catch empty alias and display name params");
        }

        [TestMethod]
        public void GetDistributionListMembers_NullConnectionServer_Failure()
        {
            List<DistributionListMember> oListMember;

            WebCallResult res = DistributionListMember.GetDistributionListMembers(null, "", out oListMember);
            Assert.IsFalse(res.Success,
                           "Fetch of distribution list members should fail with null Connection Server object passed");
        }

        [TestMethod]
        public void GetDistributionListMembers_EmptyListObjectId_Failure()
        {
            List<DistributionListMember> oListMember;
            var res = DistributionListMember.GetDistributionListMembers(_mockServer, "", out oListMember, 1, 2, null);
            Assert.IsFalse(res.Success, "GetDistributionListMember should fail with an empty DistributionListObjectID passed to it");
        }


        [TestMethod]
        public void UpdateDistributionList_NullConnectionServer_Failure()
        {
            WebCallResult res = DistributionList.UpdateDistributionList(null, "aaa", null);
            Assert.IsFalse(res.Success, "UpdateDistributionList failed to catch null ConnectionServerRest object");

        }

        [TestMethod]
        public void UpdateDistributionList_EmptyPropertyList_Failure()
        {
            var res = DistributionList.UpdateDistributionList(_mockServer, "aaa", null);
            Assert.IsFalse(res.Success, "UpdateDistributionList failed to catch empty property list");
        }


        [TestMethod]
        public void DeleteDistributionList_NullConnectionServer_Failure()
        {
            WebCallResult res = DistributionList.DeleteDistributionList(null, "aaa");
            Assert.IsFalse(res.Success, "DeleteDistributionList failed to catch null ConnectionServerRest object");
        }


        [TestMethod]
        public void GetDistributionList_EmptyObjectIdAndAlias_Failure()
        {
            DistributionList oList;
            var res = DistributionList.GetDistributionList(out oList, _mockServer);
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch empty alias and ObjectId being passed");

        }


        [TestMethod]
        public void GetDistributionListVoiceName_NullConnectionServer_Failure()
        {
            WebCallResult res = DistributionList.GetDistributionListVoiceName(null, "aaa", "");
            Assert.IsFalse(res.Success, "GetDistributionListVoiceName did not fail for null Conneciton server");
        }

        [TestMethod]
        public void GetDistributionListVoiceName_InvalidTargetWavPath_Failure()
        {
            var res = DistributionList.GetDistributionListVoiceName(_mockServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "GetDistributionListVoiceName did not fail with invalid target path passed");
        }

        [TestMethod]
        public void GetDistributionListVoiceName_EmptyObjectId_Failure()
        {
            const string strWavName = @"c:\";
            var res = DistributionList.GetDistributionListVoiceName(_mockServer, "", strWavName);
            Assert.IsFalse(res.Success, "GetDistributionListVoiceName did not fail with empty ObjectId passed");
        }

        [TestMethod]
        public void SetDistributionListVoiceName_NullConnectionServer_Failure()
        {
            WebCallResult res = DistributionList.SetDistributionListVoiceName(null, "", "");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with null Connection server passed.");
        }

        [TestMethod]
        public void SetDistributionListVoiceName_InvalidTargetWavPath_Failure()
        {
            var res = DistributionList.SetDistributionListVoiceName(_mockServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with invalid target path");
        }

        [TestMethod]
        public void SetDistributionListVoiceName_EmptyObjectId_Failure()
        {
            const string strWavName = @"c:\";
            var res = DistributionList.SetDistributionListVoiceName(_mockServer, strWavName, "");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with empty obejctID");
        }


        /// <summary>
        /// Exercise SetDistributionListVoiceName failure points
        /// </summary>
        [TestMethod]
        public void SetDistributionListVoiceNameToStreamFile_NullConnectionServer_Failure()
        {
            var res = DistributionList.SetDistributionListVoiceNameToStreamFile(null, "objectid", "resourceId");
            Assert.IsFalse(res.Success,"Calling SetDistributionListVoiceNameToStreamFile with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void SetDistributionListVoiceNameToStreamFile_EmptyObjectId_Failure()
        {
            var res = DistributionList.SetDistributionListVoiceNameToStreamFile(_mockServer, "", "resourceId");
            Assert.IsFalse(res.Success, "Calling SetDistributionListVoiceNameToStreamFile with empty objectId did not fail");
        }

        [TestMethod]
        public void SetDistributionListVoiceNameToStreamFile_EmptyResourceId_Failure()
        {
            var res = DistributionList.SetDistributionListVoiceNameToStreamFile(_mockServer, "objectid", "");
            Assert.IsFalse(res.Success, "Calling SetDistributionListVoiceNameToStreamFile with empty resource ID did not fail");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetDistributionLists_EmptyResults_Failure()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            List<DistributionList> oLists;
            var res = DistributionList.GetDistributionLists(_mockServer, out oLists, 1, 5, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetDistributionLists with EmptyResultText did not fail");

        }

        [TestMethod]
        public void GetDistributionLists_GarbageResult_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            List<DistributionList> oLists;
            var res = DistributionList.GetDistributionLists(_mockServer, out oLists, 1, 5, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetDistributionLists with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oLists.Count == 0, "Invalid result text should produce an empty list of templates");

            }

        [TestMethod]
        public void GetDistributionLists_ErrorResult_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<DistributionList> oLists;
            var res = DistributionList.GetDistributionLists(_mockServer, out oLists, 1, 5, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetDistributionLists with ErrorResponse did not fail");
        }

        [TestMethod]
        public void PublicListConstructor_GarbageResponse_Failure()
        {
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = "garbage result"
                                           });

            try
            {
                DistributionList oList = new DistributionList(_mockServer, "InvalidResultText");
                Assert.Fail("Creating new list with InvalidResultText should produce failure");
            }
            catch
            {
            }
        }

        [TestMethod]
        public void PublicListConstructor_EmptyResponse_Failure()
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
                DistributionList oList = new DistributionList(_mockServer, "", "EmptyResultText");
                Assert.Fail("Creating new list with InvalidResultText should produce failure");
            }
            catch { }

            }

        [TestMethod]
        public void PublicListConstructor_ErrorResponse_Failure()
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
                DistributionList oList = new DistributionList(_mockServer, "", "ErrorResponse");
                Assert.Fail("Creating new list with InvalidResultText should produce failure");
            }
            catch { }

        }

        [TestMethod]
        public void GetDistributionListMembers_EmptyResult_Failure()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            List<DistributionListMember> oMembers;
            var res = DistributionListMember.GetDistributionListMembers(_mockServer, "objectid", out oMembers, 1, 5,
                                                                        "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetDistributionListMembers with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetDistributionListMembers_GarbageResponse_Success()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            List<DistributionListMember> oMembers;
            var res = DistributionListMember.GetDistributionListMembers(_mockServer, "objectid", out oMembers, 1, 5, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetDistributionListMembers with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oMembers.Count == 0, "Invalid result text should produce an empty list of templates");
        }

        [TestMethod]
        public void GetDistributionListMembers_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<DistributionListMember> oMembers;
            var res = DistributionListMember.GetDistributionListMembers(_mockServer, "objectid", out oMembers, 1, 5, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetDistributionListMembers with ErrorResponse did not fail");
        }


        #endregion
    }
}
