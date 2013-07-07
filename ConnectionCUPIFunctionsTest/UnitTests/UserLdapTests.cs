﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class UserLdapTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        //Mock transport interface - 
        private static Mock<IConnectionRestCalls> _mockTransport;

        //Mock REST server
        private static ConnectionServerRest _mockServer;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            //create a connection server instance used for all tests - rather than using a mockup 
            //for fetching data I prefer this "real" testing approach using a public server I keep up
            //and available for the purpose - the conneciton information is stored in the test project's 
            //settings and can be changed to a local instance easily.
            Settings mySettings = new Settings();
            Thread.Sleep(300);
            try
            {
                _connectionServer = new ConnectionServerRest(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start UserLdap test:" + ex.Message);
            }

            //setup mock server interface 
            _mockTransport = new Mock<IConnectionRestCalls>();

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                });

            _mockServer = new ConnectionServerRest(_mockTransport.Object, "test", "test", "test", false);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticMethod_GetLdapUsers()
        {
            List<UserLdap> oUsers;
            var res = UserLdap.GetLdapUsers(null, out oUsers, 0, 20, "");
            Assert.IsFalse(res.Success,"GetLdapUsers with null Connection server should fail");

            res = UserLdap.GetLdapUsers(_connectionServer, out oUsers, 99999, 100, null);
            Assert.IsFalse(res.Success, "GetLdapUsers with invalid page number should fail");

            res = UserLdap.GetLdapUsers(_connectionServer, out oUsers, 0, 20, "query=(alias is _bogus_)");

            if (res.ResponseText.Contains("LDAP_DIRECTORY_SYNCH_NOT_ACTIVATED"))
            {
                Console.WriteLine("LDAP not enabled on box -skipping invalid query test");
                return;
            }

            Assert.IsTrue(res.Success, "GetLdapUsers with invalid query server should not fail:"+res);
            Assert.IsNotNull(oUsers,"Bogus query should not return a null list");
            Assert.IsTrue(oUsers.Count==0,"Bogus query should return empty list");


        }


        [TestMethod]
        public void StaticMethod_ImportLdapUser()
        {
            UserFull oUser;
            ConnectionPropertyList oProps = new ConnectionPropertyList();

            var res =UserLdap.ImportLdapUser(null, "templatealias", "pkid", "alias", "firstname", "lastname", "extension", oProps,out oUser);
            Assert.IsFalse(res.Success,"Calling ImportLdapUser with should fail");

            res = UserLdap.ImportLdapUser(_connectionServer, "", "pkid", "alias", "firstname", "lastname", "extension", oProps, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with should fail");

            res = UserLdap.ImportLdapUser(_connectionServer, "templatealias", "", "alias", "firstname", "lastname", "extension", null, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with should fail");

            res = UserLdap.ImportLdapUser(_connectionServer, "templatealias", "pkid", "alias", "firstname", "lastname", "", oProps, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with should fail");

            res = UserLdap.ImportLdapUser(_connectionServer, "templatealias", "pkid", "alias", "firstname", "lastname", "extension", null, out oUser);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with should fail");
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void FetchTests()
        {
            List<UserLdap> oUsers;
            var res = UserLdap.GetLdapUsers(_connectionServer, out oUsers, 0, 5);
            
            if (res.ResponseText.Contains("LDAP_DIRECTORY_SYNCH_NOT_ACTIVATED"))
            {
                Console.WriteLine("LDAP not enabled on box -skipping live tests of LDAP import");
                return;
            }
            
            Assert.IsTrue(res.Success,"Failed fetching LDAP users:"+res);
            Assert.IsNotNull(oUsers,"Fetch of LDAP users returned null list");
            
            //this may not return anything, but that's ok - should be an empty list
            foreach (var oUser in oUsers)
            {
                Console.WriteLine(oUser.ToString());
                Console.WriteLine(oUser.SelectionDisplayString);
                Console.WriteLine(oUser.UniqueIdentifier);
            }
        }

        [TestMethod]
        public void EmptyInstanceTests()
        {
            UserLdap oUserLdap = new UserLdap();
            UserFull oUser;
            var res = oUserLdap.Import("1234234", "alias",out oUser);
            Assert.IsFalse(res.Success,"Importing an empty instance should fail");

            res = oUserLdap.Import("1234234", "alias");
            Assert.IsFalse(res.Success, "Importing an empty instance should fail");
        }


        #endregion


        #region Harness Tests

        [TestMethod]
        public void HarnessTest_GetLdapUsers()
        {
            List<UserLdap> oUsers;

            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            var res = UserLdap.GetLdapUsers(_mockServer, out oUsers, 0, 20, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetLdapUsers with EmptyResultText did not fail");

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            res = UserLdap.GetLdapUsers(_mockServer, out oUsers, 0, 20, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetLdapUsers with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oUsers.Count == 0, "Invalid result text should produce an empty list of templates");

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            res = UserLdap.GetLdapUsers(_mockServer, out oUsers, 0, 20, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetLdapUsers with ErrorResponse did not fail");
        }


        [TestMethod]
        public void HarnessTest_ImportLdapUser()
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

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,ResponseText = "garbage result"
                                  });

            res = UserLdap.ImportLdapUser(_mockServer, "InvalidResultText", "pkid", "Alias", "FirstName", "LastName", "Extension", null);
            Assert.IsTrue(res.Success, "Calling ImportLdapUser with InvalidResultText should not fail:"+res);

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            res = UserLdap.ImportLdapUser(_mockServer, "ErrorResponse", "pkid", "Alias", "FirstName", "LastName", "Extension", null);
            Assert.IsFalse(res.Success, "Calling ImportLdapUser with ErrorResponse did not fail");

            //invalid return objectId
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "/vmrest/users/junk"
                });
            res = UserLdap.ImportLdapUser(_mockServer, "test", "pkid", "Alias", "FirstName", "LastName", "Extension", null);
            Assert.IsTrue(res.Success, "Calling ImportLdapUser with ReturnSpecificText using junk ObjectId value failed:"+res);
            Assert.IsTrue(res.ReturnedObjectId.Equals("junk"),"junk not returned as ObjecId of newly imported uses:"+res);
        }

        #endregion

    }
}