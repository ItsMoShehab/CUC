using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class UserLdapUnitTests : BaseUnitTests
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


        #region Static Call Failures

        [TestMethod]
        public void GetLdapUsers_NullConnectionServer_Failure()
        {
            List<UserLdap> oUsers;
            var res = UserLdap.GetLdapUsers(null, out oUsers, 0, 20, "");
            Assert.IsFalse(res.Success,"GetLdapUsers with null Connection server should fail");
        }


        [TestMethod]
        public void ImportLdapUser_NullConnectionServer_Failure()
        {
            UserFull oUser;
            ConnectionPropertyList oProps = new ConnectionPropertyList();

            var res =UserLdap.ImportLdapUser(null, "templatealias", "pkid", "alias", "firstname", "lastname", "extension", oProps,out oUser);
            Assert.IsFalse(res.Success,"Calling ImportLdapUser with null connection server should fail");

            }


        [TestMethod]
        public void ImportLdapUser_EmptyTemplateAlias_Failure()
        {
            UserFull oUser;
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = UserLdap.ImportLdapUser(_mockServer, "", "pkid", "alias", "firstname", "lastname", "extension", oProps, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with empty template alias should fail");

            }


        [TestMethod]
        public void ImportLdapUser_EmptyAlias_Failure()
        {
            UserFull oUser;
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = UserLdap.ImportLdapUser(_mockServer, "templatealias", "", "alias", "firstname", "lastname", "extension", null, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with empty alias should fail");

            }


        [TestMethod]
        public void ImportLdapUser_EmptyExtension_Failure()
        {
            UserFull oUser;
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = UserLdap.ImportLdapUser(_mockServer, "templatealias", "pkid", "alias", "firstname", "lastname", "", oProps, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with empty extension should fail");

            }


        [TestMethod]
        public void ImportLdapUser_NullProperties_Failure()
        {
            UserFull oUser;
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = UserLdap.ImportLdapUser(_mockServer, "templatealias", "pkid", "alias", "firstname", "lastname", "extension", null, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with null properties should fail");

            }


        [TestMethod]
        public void ImportLdapUser_EmptyProperties_Failure()
        {
            UserFull oUser;
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = UserLdap.ImportLdapUser(_mockServer, "templatealias", "pkid", "alias", "firstname", "lastname", "extension", oProps, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with empty properties should fail");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetLdapUsers_EmptyResults_Failure()
        {
            List<UserLdap> oUsers;

            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            var res = UserLdap.GetLdapUsers(_mockServer, out oUsers, 0, 20, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetLdapUsers with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetLdapUsers_GarbageResults_Success()
        {
            List<UserLdap> oUsers;

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            var res = UserLdap.GetLdapUsers(_mockServer, out oUsers, 0, 20, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetLdapUsers with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oUsers.Count == 0, "Invalid result text should produce an empty list of templates");

            }

        [TestMethod]
        public void GetLdapUsers_ErrorResponse_Failure()
        {
            List<UserLdap> oUsers;
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = UserLdap.GetLdapUsers(_mockServer, out oUsers, 0, 20, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetLdapUsers with ErrorResponse did not fail");
        }

        [TestMethod]
        public void ImportLdapUser_EmptyResponse_Failure()
        {
            UserFull oUser;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = UserLdap.ImportLdapUser(_mockServer, "EmptyResultText", "pkid", "Alias", "FirstName", "LastName", "Extension", null, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with EmptyResultText did not fail");

            }

        [TestMethod]
        public void ImportLdapUser_GarbageResults_Failure()
        {
            UserFull oUser;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,ResponseText = "garbage result"
                                  });

            var res = UserLdap.ImportLdapUser(_mockServer, "InvalidResultText", "pkid", "Alias", "FirstName", "LastName", "Extension", null);
            Assert.IsTrue(res.Success, "Calling ImportLdapUser with InvalidResultText should not fail:"+res);

            }

        [TestMethod]
        public void ImportLdapUser_ErrorResponse_Failure()
        {
            UserFull oUser;
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = UserLdap.ImportLdapUser(_mockServer, "ErrorResponse", "pkid", "Alias", "FirstName", "LastName", "Extension", null);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with ErrorResponse did not fail");

            }

        [TestMethod]
        public void ImportLdapUser_ReturnedObjectId_Success()
        {
            UserFull oUser;
            //return objectId
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "/vmrest/users/junk"
                });
            var res = UserLdap.ImportLdapUser(_mockServer, "test", "pkid", "Alias", "FirstName", "LastName", "Extension", null);
            Assert.IsTrue(res.Success, "Calling ImportLdapUser with ReturnSpecificText using junk ObjectId value failed:"+res);
            Assert.IsTrue(res.ReturnedObjectId.Equals("junk"),"junk not returned as ObjecId of newly imported uses:"+res);
        }

        #endregion

    }
}
