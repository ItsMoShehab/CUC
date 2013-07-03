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

        /// <summary>
        /// Test common failure scenarios distribution list functions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionListTest()
        {
            //create new list with GUID in the name to ensure uniqueness
            DistributionList oList;

            //null connection server object
            WebCallResult res = DistributionList.GetDistributionList(out oList, null, "", "allvoicemailusers");
            Assert.IsFalse(res.Success, "Null Connection server on GetDistributionList did not fail.");

            //invalid alias/objectId pair
            res = DistributionList.GetDistributionList(out oList, _mockServer);
            Assert.IsFalse(res.Success, "Blank alias/objectID params on GetDistributionList did not fail");
        }

        /// <summary>
        /// exercise GetDistributionLists failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionLists()
        {
            List<DistributionList> oList;
            WebCallResult res = DistributionList.GetDistributionLists(null, out oList, null);

            Assert.IsFalse(res.Success, "GetDistributionLists failed to catch null ConnectionServerRest object");
        }

        /// <summary>
        /// Exercise AddDistributionList failure points.
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_AddDistributionList()
        {
            WebCallResult res = DistributionList.AddDistributionList(null, "aaa", "aaa", "123", null);
            Assert.IsFalse(res.Success, "AddDistributionList failed to catch null ConnectionServerRest object");


            res = DistributionList.AddDistributionList(_mockServer, "", "", "123", null);
            Assert.IsFalse(res.Success, "AddDistributionList failed to catch empty alias and display name params");
        }

        /// <summary>
        /// Exercise GetDistributionListMember failure points.
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionListMember()
        {
            List<DistributionListMember> oListMember;

            WebCallResult res = DistributionListMember.GetDistributionListMembers(null, "", out oListMember);
            Assert.IsFalse(res.Success, "Fetch of distribution list members should fail with null Connection Server object passed");

            res = DistributionListMember.GetDistributionListMembers(_mockServer, "", out oListMember, 1, 2, null);
            Assert.IsFalse(res.Success, "GetDistributionListMember should fail with an empty DistributionListObjectID passed to it");
        }


        /// <summary>
        /// Exercise UpdateDistrubitonList failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateDistributionList()
        {
            WebCallResult res = DistributionList.UpdateDistributionList(null, "aaa", null);
            Assert.IsFalse(res.Success, "UpdateDistributionList failed to catch null ConnectionServerRest object");


            res = DistributionList.UpdateDistributionList(_mockServer, "aaa", null);
            Assert.IsFalse(res.Success, "UpdateDistributionList failed to catch empty property list");
        }


        /// <summary>
        /// Exercise DeleteDistributionList failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_DeleteDistributionList()
        {
            WebCallResult res = DistributionList.DeleteDistributionList(null, "aaa");
            Assert.IsFalse(res.Success, "DeleteDistributionList failed to catch null ConnectionServerRest object");
        }


        /// <summary>
        /// Exercise GetDistributionList failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionList()
        {
            DistributionList oList;

            WebCallResult res = DistributionList.GetDistributionList(out oList, null);
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch null ConnectionServerRest object");

            res = DistributionList.GetDistributionList(out oList, _mockServer);
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch empty alias and ObjectId being passed");

            res = DistributionList.GetDistributionList(out oList, _mockServer, "", "bogus alias");
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch bogus alias and empty ObjectId being passed");

        }



        /// <summary>
        /// Exercise GetDistributionListVoiceName failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionListVoiceName()
        {
            //use the same string for the alias and display name here
            const string strWavName = @"c:\";

            //invalid local WAV file name
            WebCallResult res = DistributionList.GetDistributionListVoiceName(null, "aaa", "");
            Assert.IsFalse(res.Success, "GetDistributionListVoiceName did not fail for null Conneciton server");

            //empty target file path
            res = DistributionList.GetDistributionListVoiceName(_mockServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "GetDistributionListVoiceName did not fail with invalid target path passed");

            //invalid objectId 
            res = DistributionList.GetDistributionListVoiceName(_mockServer, "", strWavName);
            Assert.IsFalse(res.Success, "GetDistributionListVoiceName did not fail with invalid ObjectId passed");

        }

        /// <summary>
        /// Exercise SetDistributionListVoiceName failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_SetDistributionListVoiceName()
        {
            //use the same string for the alias and display name here
            const string strWavName = @"c:\";

            //invalid Connection server
            WebCallResult res = DistributionList.SetDistributionListVoiceName(null, "", "");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with null Connection server passed.");

            //invalid target path
            res = DistributionList.SetDistributionListVoiceName(_mockServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with invalid target path");

            //invalid ObjectId
            res = DistributionList.SetDistributionListVoiceName(_mockServer, strWavName, "");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with invalid obejctID");

        }


        /// <summary>
        /// Exercise SetDistributionListVoiceName failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_SetDistributionListVoiceNameToStreamFile()
        {
            var res = DistributionList.SetDistributionListVoiceNameToStreamFile(null, "objectid", "resourceId");
            Assert.IsFalse(res.Success, "Calling SetDistributionListVoiceNameToStreamFile with null ConnectionServerRest did not fail");

            res = DistributionList.SetDistributionListVoiceNameToStreamFile(_mockServer, "objectid", "resourceId");
            Assert.IsFalse(res.Success, "Calling SetDistributionListVoiceNameToStreamFile with invalid objectId did not fail");

            res = DistributionList.SetDistributionListVoiceNameToStreamFile(_mockServer, "", "resourceId");
            Assert.IsFalse(res.Success, "Calling SetDistributionListVoiceNameToStreamFile with empty objectId did not fail");

            res = DistributionList.SetDistributionListVoiceNameToStreamFile(_mockServer, "objectid", "");
            Assert.IsFalse(res.Success, "Calling SetDistributionListVoiceNameToStreamFile with empty resource ID did not fail");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetDistributionLists_HarnessTestFailures()
        {
            var oTestTransport = new Mock<IConnectionRestCalls>();

            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                });

            ConnectionServerRest oServer = new ConnectionServerRest(oTestTransport.Object, "test", "test", "test", false);

            //empty results
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            List<DistributionList> oLists;
            var res = DistributionList.GetDistributionLists(oServer, out oLists, 1, 5, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetDistributionLists with EmptyResultText did not fail");

            //garbage response
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            res = DistributionList.GetDistributionLists(oServer, out oLists, 1, 5, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetDistributionLists with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oLists.Count == 0, "Invalid result text should produce an empty list of templates");

            //error response
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            res = DistributionList.GetDistributionLists(oServer, out oLists, 1, 5, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetDistributionLists with ErrorResponse did not fail");
        }

        [TestMethod]
        public void PublicListConstructor_HarnessTestFailure()
        {
            var oTestTransport = new Mock<IConnectionRestCalls>();

            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                });

            ConnectionServerRest oServer = new ConnectionServerRest(oTestTransport.Object, "test", "test", "test", false);

            //garbage response
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            //fetch by ObjectId
            try
            {
                DistributionList oList = new DistributionList(oServer, "InvalidResultText");
                Assert.Fail("Creating new list with InvalidResultText should produce failure");
            }
            catch{}

            //empty results
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            try
            {
                DistributionList oList = new DistributionList(oServer,"", "EmptyResultText");
                Assert.Fail("Creating new list with InvalidResultText should produce failure");
            }
            catch { }

            //error response
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            try
            {
                DistributionList oList = new DistributionList(oServer, "", "ErrorResponse");
                Assert.Fail("Creating new list with InvalidResultText should produce failure");
            }
            catch { }

        }

        [TestMethod]
        public void GetDistributionListMembers_HarnessTestFailures()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            List<DistributionListMember> oMembers;
            var res = DistributionListMember.GetDistributionListMembers(_mockServer, "objectid", out oMembers, 1, 5, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetDistributionListMembers with EmptyResultText did not fail");

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            res = DistributionListMember.GetDistributionListMembers(_mockServer, "objectid", out oMembers, 1, 5, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetDistributionListMembers with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oMembers.Count == 0, "Invalid result text should produce an empty list of templates");

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            res = DistributionListMember.GetDistributionListMembers(_mockServer, "objectid", out oMembers, 1, 5, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetDistributionListMembers with ErrorResponse did not fail");
        }


        #endregion
    }
}
