using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class GlobalUserUnitTests : BaseUnitTests
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


        #region Construction tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            GlobalUser oTest = new GlobalUser(null);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        public void Constructor_Success()
        {
            GlobalUser oTest = new GlobalUser(_mockServer);
            Console.WriteLine(oTest.ToString());
            Console.WriteLine(oTest.DumpAllProps());
        }

        #endregion


        #region Static Calls

        [TestMethod]
        public void GetUsers_NullConnectionServer_NullClause_Failure()
        {
            List<GlobalUser> oUsers;
            var res = GlobalUser.GetUsers(null, out oUsers,1,10,null);
            Assert.IsFalse(res.Success,"Calling GetUsers with null Connection server should fail");
        }

        [TestMethod]
        public void GetUsers_NullConnectionServer_EmptyClause_Failure()
        {
            List<GlobalUser> oUsers;
            var res = GlobalUser.GetUsers(null, out oUsers, 1, 10, "");
            Assert.IsFalse(res.Success, "Calling GetUsers with null Connection server should fail");
        }

        [TestMethod]
        public void GetUser_NullConnectionServer_Failure()
        {
            GlobalUser oGlobalUser;
            var res = GlobalUser.GetUser(out oGlobalUser,null, "ObjectId", "Alias");
            Assert.IsFalse(res.Success, "Calling GetUser with null Connection server should fail");
        }

        [TestMethod]
        public void GetUser_EmptyAliasAndObjectId_Failure()
        {
            GlobalUser oGlobalUser;
            var res = GlobalUser.GetUser(out oGlobalUser, _mockServer, "");
            Assert.IsFalse(res.Success, "Calling GetUser with empty alias and ObjectId server should fail");
        }

        [TestMethod]
        public void GetUser_Alias_Failure()
        {
            GlobalUser oGlobalUser;
            var res = GlobalUser.GetUser(out oGlobalUser, _mockServer, "","bogus alias");
            Assert.IsFalse(res.Success, "Calling GetUser with invalid alias did not fail:");
        }

        [TestMethod]
        public void GetUser_ObjectId_Success()
        {
            GlobalUser oGlobalUser;
            var res = GlobalUser.GetUser(out oGlobalUser, _mockServer, Guid.NewGuid().ToString());
            Assert.IsTrue(res.Success, "Calling GetUser with objectId failed:"+res);
        }

        #endregion

        #region Harness Tests

        [TestMethod]
        public void GetUsers_ZeroResults_Success()
        {
            //zero results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           TotalObjectCount = 0,
                                           ResponseText = "junk"
                                       });
            List<GlobalUser> oUsers;
            var res = GlobalUser.GetUsers(_mockServer, out oUsers);
            Assert.IsTrue(res.Success, "Calling GetUsers with zero results failed:"+res);
            Assert.IsTrue(oUsers.Count==0,"Callling GetUsers with zero results should return empty list.");
        }

        [TestMethod]
        public void GetUsers_EmptyResult_Failure()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<GlobalUser> oUsers;
            var res = GlobalUser.GetUsers(_mockServer, out oUsers);
            Assert.IsFalse(res.Success, "Calling GetUsers with EmptyResultText did not fail");

        }

        [TestMethod]
        public void GetUsers_GarbageResult_Failure()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result in the response body that will not parse properly to globalUser"
                                  });

            List<GlobalUser> oUsers;
            var res = GlobalUser.GetUsers(_mockServer, out oUsers);
            Assert.IsFalse(res.Success, "Calling GetUsers with garbage result did not fail");
        }


        [TestMethod]
        public void GetUsers_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<GlobalUser> oUsers;
            var res = GlobalUser.GetUsers(_mockServer, out oUsers);
            Assert.IsFalse(res.Success, "Calling GetUsers with error response did not fail");
        }




        [TestMethod]
        public void GetUser_EmptyResult_Failure()
        {
            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            GlobalUser oUser;
            var res = GlobalUser.GetUser(out oUser, _mockServer, "objectid");
            Assert.IsFalse(res.Success, "Calling GetUser with EmptyResultText did not fail");

        }

        [TestMethod]
        public void GetUser_GarbageResult_Failure()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result in the response body that will not parse properly to globalUser"
                                  });

            GlobalUser oUser;
            var res = GlobalUser.GetUser(out oUser, _mockServer, "objectid");
            Assert.IsFalse(res.Success, "Calling GetUser with garbage result did not fail");
        }


        [TestMethod]
        public void GetUser_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<GlobalUser> oUsers;
            var res = GlobalUser.GetUsers(_mockServer, out oUsers);
            Assert.IsFalse(res.Success, "Calling GetUser with error response did not fail");
        }
     
        #endregion
    }
}
