using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PrivateListUnitTests : BaseUnitTests 
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


        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            PrivateList oTest = new PrivateList(null,"OwnerObjectId","ObjectId");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyOwnerObjectId_Failure()
        {
            PrivateList oTest = new PrivateList(_mockServer, "", "ObjectId");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failure Tests 

        [TestMethod]
        public void AddPrivateList_NullConnectionServer_Failure()
        {
            var res = PrivateList.AddPrivateList(null, "OwnerObjectId", "ObjectId", 1);
            Assert.IsFalse(res.Success, "Adding private list with null connection server did not fail");
        }

        [TestMethod]
        public void AddPrivateList_BlankOwnerObjectId_Failure()
        {
            var res = PrivateList.AddPrivateList(_mockServer, "", "ObjectId", 1);
            Assert.IsFalse(res.Success, "Adding private list with blank UserObjectId did not fail");
        }

        [TestMethod]
        public void AddPrivateList_BlankObjectId_Failure()
        {
            var res = PrivateList.AddPrivateList(_mockServer, "OwnerObjectId", "", 1);
            Assert.IsFalse(res.Success, "Adding private list with blank display name did not fail");
        }

        [TestMethod]
        public void GetPrivateList_NullConnectionServer_Failure()
        {
            PrivateList oNewPrivateList;

            var res = PrivateList.GetPrivateList(out oNewPrivateList, null, "OwnerObjectId");
            Assert.IsFalse(res.Success, "Fetching private list with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void GetPrivateList_EmptyUserObjectId_Failure()
        {
            PrivateList oNewPrivateList;
            var res = PrivateList.GetPrivateList(out oNewPrivateList, _mockServer, "");
            Assert.IsFalse(res.Success, "Fetching private list with empty owner objectId not fail");
        }

        [TestMethod]
        public void UpdatePrivateList_NullConnectionServer_Failure()
        {
            var res = PrivateList.UpdatePrivateList(null, "Object", null, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void UpdatePrivateList_EmptyObjectId_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();

            var res = PrivateList.UpdatePrivateList(_mockServer, "", oProps, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with empty objectId");
        }

        [TestMethod]
        public void UpdatePrivateList_EmptyOwnerObjectId_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = PrivateList.UpdatePrivateList(_mockServer, "Object", oProps, "");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with empty owner id");
        }

        [TestMethod]
        public void UpdatePrivateList_NullPropertyList_Failure()
        {
            var res = PrivateList.UpdatePrivateList(_mockServer, "Object", null, "ownerid");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with null property list");
        }

        [TestMethod]
        public void UpdatePrivateList_EmptyPropertyList_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = PrivateList.UpdatePrivateList(_mockServer, "Object", oProps, "ownerid");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with empty property list");
        }

        [TestMethod]
        public void DeletePrivateList_NullConnectionServer_Failure()
        {
            var res = PrivateList.DeletePrivateList(null, "Object", "UserObjectId");
            Assert.IsFalse(res.Success, "Deleting private list via static method did not fail with invalid null ConnectionString");
        }

        [TestMethod]
        public void DeletePrivateList_EmptyObjectId_Failure()
        {
            var res = PrivateList.DeletePrivateList(_mockServer, "", "UserObjectId");
            Assert.IsFalse(res.Success, "Deleting private list via static method did not fail with empty objectID");
        }

        [TestMethod]
        public void DeletePrivateList_EmptyUserObjectId_Failure()
        {
            var res = PrivateList.DeletePrivateList(_mockServer, "Object", "");
            Assert.IsFalse(res.Success, "Deleting private list via static method did not fail with empty user ObjectId");
        }

        [TestMethod]
        public void GetPrivateListVoiceName_NullConnectionServer_Failure()
        {
            var res = PrivateList.GetPrivateListVoiceName(null, "UserObjectId", @"c:\", "ObjectId", "WavName");
            Assert.IsFalse(res.Success, "Getting private list voice name via static method did not fail with null Connection server ");

         }

        [TestMethod]
        public void GetPrivateListVoiceName_EmptyUserObjectId_Failure()
        {
            var res = PrivateList.GetPrivateListVoiceName(_mockServer, "", @"c:\", "ObjectId", "WavName");
            Assert.IsFalse(res.Success, "Getting private list voice name via static method did not fail with empty user objectId ");

            }

        [TestMethod]
        public void GetPrivateListVoiceName_EmptyWavFilePath_Failure()
        {
            var res = PrivateList.GetPrivateListVoiceName(_mockServer, "UserObjectId", "", "ObjectId", "");
            Assert.IsFalse(res.Success, "Getting private list voice name via static method did not fail with empty wav file name and target path ");


            }

        [TestMethod]
        public void SetPrivateListVoiceName_NullConnectionServer_Failure()
        {
            var res = PrivateList.SetPrivateListVoiceName(null, "Dummy.wav", "", "UserObjectId", true);
            Assert.IsFalse(res.Success, "Setting private list voice name via static method did not fail with null ConnectionServer");

            }

        [TestMethod]
        public void SetPrivateListVoiceName_EmptyObjectId_Failure()
        {
            var res = PrivateList.SetPrivateListVoiceName(_mockServer, "Dummy.wav", "", "UserObjectId", true);
            Assert.IsFalse(res.Success, "Setting private list voice name via static method did not fail with empty ObjectId");
        }

        [TestMethod]
        public void SetPrivateListVoiceName_EmptyUserObjectId_Failure()
        {
            var res = PrivateList.SetPrivateListVoiceName(_mockServer, "Dummy.wav", "ObjectId", "", true);
            Assert.IsFalse(res.Success, "Setting private list voice name via static method did not fail with empty UserObjectId");
        }


        [TestMethod]
        public void AddMemberPublicList_NullConnectionServer_Failure()
        {
            var res = PrivateList.AddMemberPublicList(null, "ObjectId", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list public DL member via static method did not fail with null ConnectionServerRest ");
        }

        [TestMethod]
        public void AddMemberPublicList_EmptyObjectId_Failure()
        {
            var res = PrivateList.AddMemberPublicList(_mockServer, "", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list public DL member via static method did not fail with empty private list objectId");
        }

        [TestMethod]
        public void AddMemberPublicList_EmptyPublicListObjectId_Failure()
        {
            var res = PrivateList.AddMemberPublicList(_mockServer, "PrivateListObjectId", "", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list public DL member via static method did not fail with empty public list objectId");
        }

        [TestMethod]
        public void AddMemberPublicList_OwnerObjectId_Failure()
        {
            var res = PrivateList.AddMemberPublicList(_mockServer, "PrivateListObjectId", "PublicObjectId", "");
            Assert.IsFalse(res.Success, "Adding private list public DL member via static method did not fail with empty owner objectId");
        }

        [TestMethod]
        public void AddMemberUser_NullConnectionServer_Failure()
        {
            var res = PrivateList.AddMemberUser(null, "ObjectId", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list user member via static method did not fail with null ConnectionServerRest ");

            }

        [TestMethod]
        public void AddMemberUser_EmptyObjectId_Failure()
        {
            var res = PrivateList.AddMemberUser(_mockServer, "", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list user member via static method did not fail with empty private list objectId");
        }

        [TestMethod]
        public void AddMemberUser_EmptyUserObjectId_Failure()
        {
            var res = PrivateList.AddMemberUser(_mockServer, "ObjectId", "", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list user member via static method did not fail with empty private list objectId");
        }

        [TestMethod]
        public void AddMemberUser_EmptyOwnerObjectId_Failure()
        {
            var res = PrivateList.AddMemberUser(_mockServer, "ObjectId", "UserObjectId", "");
            Assert.IsFalse(res.Success, "Adding private list user member via static method did not fail with empty private list objectId");
        }


        [TestMethod]
        public void GetPrivateLists_NullConnectionServer_Failure()
        {
            List<PrivateList> oLists;
            WebCallResult res = PrivateList.GetPrivateLists(null, "blah", out oLists);
            Assert.IsFalse(res.Success, "Fetching private lists with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void GetPrivateLists_EmptyOwnerObjectId_Failure()
        {
            List<PrivateList> oLists;
            WebCallResult res = PrivateList.GetPrivateLists(_mockServer, "", out oLists);
            Assert.IsFalse(res.Success, "Fetching private lists with empty owner objectId did not fail");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetPrivateListMembers_EmptyResults_Failure()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            List<PrivateListMember> oMembers;
            var res = PrivateListMember.GetPrivateListMembers(_mockServer, "objectid", "EmptyResultText", out oMembers);
            Assert.IsFalse(res.Success, "Calling GetPrivateListMembers with EmptyResultText did not fail");
            Assert.IsTrue(oMembers.Count == 0, "Empty result text should result in empty list returned");
        }

        [TestMethod]
        public void GetPrivateListMembers_GarbageResults_Success()
        {

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });
            List<PrivateListMember> oMembers;
            var res = PrivateListMember.GetPrivateListMembers(_mockServer, "objectid", "InvalidResultText", out oMembers);
            Assert.IsTrue(res.Success, "Calling GetPrivateListMembers with InvalidResultText should not fail:"+res);
            Assert.IsTrue(oMembers.Count == 0, "Invalid result text should result in empty list returned");

            }

        [TestMethod]
        public void GetPrivateListMembers_ErrorResponse_Success()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            List<PrivateListMember> oMembers;
            var res = PrivateListMember.GetPrivateListMembers(_mockServer, "objectid", "ErrorResponse", out oMembers);
            Assert.IsFalse(res.Success, "Calling GetPrivateListMembers with ErrorResponse did not fail");
            Assert.IsTrue(oMembers.Count == 0, "Error result should result in empty list returned");
        }


        [TestMethod]
        public void GetPrivateLists_EmptyResults_Failure()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            List<PrivateList> oLists;
            var res = PrivateList.GetPrivateLists(_mockServer, "EmptyResultText", out oLists);
            Assert.IsFalse(res.Success, "Calling GetPrivateLists with EmptyResultText did not fail");
            Assert.IsTrue(oLists.Count == 0, "Empty result text should result in empty list returned");

            }


        [TestMethod]
        public void GetPrivateLists_GarbageResults_Success()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });
            List<PrivateList> oLists;
            var res = PrivateList.GetPrivateLists(_mockServer, "InvalidResultText", out oLists);
            Assert.IsTrue(res.Success, "Calling GetPrivateLists with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oLists.Count == 0, "Invalid result text should result in empty list returned");

            }


        [TestMethod]
        public void GetPrivateLists_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            List<PrivateList> oLists;
            var res = PrivateList.GetPrivateLists(_mockServer, "ErrorResponse", out oLists);
            Assert.IsFalse(res.Success, "Calling GetPrivateLists with ErrorResponse did not fail");
            Assert.IsTrue(oLists.Count == 0, "Error result should result in empty list returned");
        }

        #endregion
    }
}
