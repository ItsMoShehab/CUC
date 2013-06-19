using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{


    /// <summary>
    ///This is a test class for AlternateExtensionTest and is intended
    ///to contain all AlternateExtensionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AlternateExtensionTest
    {

        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        //used for editing/adding items to a temporary user that gets cleaned up after the tests are complete
        private static UserFull _tempUser;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


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
                throw new Exception("Unable to attach to Connection server to start Alternate Extension test:" + ex.Message);
            }

            //create new list with GUID in the name to ensure uniqueness
            String strUserAlias = "TempUser_" + Guid.NewGuid().ToString().Replace("-", "");

            //generate a random number and tack it onto the end of some zeros so we're sure to avoid any legit numbers on the system.
            Random random = new Random();
            int iExtPostfix = random.Next(100000, 999999);
            string strExtension = "000000" + iExtPostfix.ToString();

            //use a bogus extension number that's legal but non dialable to avoid conflicts
            WebCallResult res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", strUserAlias, strExtension, null, out _tempUser);
            Assert.IsTrue(res.Success, "Failed creating temporary user:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempUser != null)
            {
                WebCallResult res = _tempUser.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary user on cleanup.");
            }
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
            AlternateExtension oTemp = new AlternateExtension(null, "aaa");
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ObjectId is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            AlternateExtension oTemp = new AlternateExtension(_connectionServer, "");
        }

        #endregion


        #region Static Call Failures

        /// <summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_DeleteAlternateExtension()
        {
            WebCallResult res;

            //static delete failure calls
            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, _tempUser.ObjectId, "aaa");
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, _tempUser.ObjectId, "");
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, "", "aaa");
            Assert.IsFalse(res.Success, "Empty user objectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "Invalid user objectId should fail");

            res = AlternateExtension.DeleteAlternateExtension(null, _tempUser.ObjectId, "aaa");
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

        }

        ///<summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetAlternateExtensions()
        {
            WebCallResult res;
            List<AlternateExtension> oAltExts;

            //static GetAlternateExtensions calls
            res = AlternateExtension.GetAlternateExtensions(null, _tempUser.ObjectId, out oAltExts);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

        }

        ///<summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetAlternateExtension()
        {
            AlternateExtension oAltExt;
            WebCallResult res;

            //static GetAlternateExtension calls
            res = AlternateExtension.GetAlternateExtension(_connectionServer, _tempUser.ObjectId, "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Invalid objecTId should fail");

            res = AlternateExtension.GetAlternateExtension(null, _tempUser.ObjectId, "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Null ConnectonServer object should fail");

            res = AlternateExtension.GetAlternateExtension(_connectionServer, "", "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Empty UServerObjectID should fail");

            res = AlternateExtension.GetAlternateExtension(_connectionServer, "aaa", "aaa", out oAltExt);
            Assert.IsFalse(res.Success, "Invalid UserObjectID should fail");

        }


        ///<summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateAlternateExtension()
        {
            WebCallResult res;

            //static UpdateAlternateExtension calls
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("name", "value");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, _tempUser.ObjectId, "aaa", oProps);
            Assert.IsFalse(res.Success, "Invalid ObjectId should fail");

            res = AlternateExtension.UpdateAlternateExtension(null, _tempUser.ObjectId, "aaa", oProps);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, "", "aaa", oProps);
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, "aaa", "aaa", oProps);
            Assert.IsFalse(res.Success, "Invalid UserObjectID should fail");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, _tempUser.ObjectId, "aaa", null);
            Assert.IsFalse(res.Success, "Empty property list should fail");

        }

        /// <summary>
        /// testing failure conditiions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_AddAlternateExtension()
        {
            WebCallResult res;

            //Static AddAlternateExtension calls

            res = AlternateExtension.AddAlternateExtension(null, _tempUser.ObjectId, 1, "1234");
            Assert.IsFalse(res.Success, "Null Connection server object should fail");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "", 1, "1234");
            Assert.IsFalse(res.Success, "Empty UserObjectID should fail");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "aaa", 1, "1234");
            Assert.IsFalse(res.Success, "Invalid UserOBjectID should fail");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, _tempUser.ObjectId, 99, "1234");
            Assert.IsFalse(res.Success, "Invalid alternate extension index ID should fail");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, _tempUser.ObjectId, 1, "");
            Assert.IsFalse(res.Success, "Empty extension string should fail");

        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void AddAndFetchAlternateExtension()
        {
            AlternateExtension oAltExt;
            string strExtension = Guid.NewGuid().ToString().Replace("-", "");
            var res = AlternateExtension.AddAlternateExtension(_connectionServer, _tempUser.ObjectId, 1,strExtension, out oAltExt);
            Assert.IsTrue(res.Success,"Failed to create new alternate extension:"+res);

            const string strNewDisplayName = "New Display Name";
            oAltExt.DisplayName = strNewDisplayName;
            res = oAltExt.Update();
            Assert.IsTrue(res.Success,"Failed to update alternate extension:"+res);

            res =oAltExt.RefetchAlternateExtensionData();
            Assert.IsTrue(res.Success,"Failed to refetch alternate extension data:"+res);
            Assert.IsTrue(oAltExt.DisplayName.Equals(strNewDisplayName));


            //refetch using objectId
            AlternateExtension oNewAltExt;
            res = AlternateExtension.GetAlternateExtension(_connectionServer, _tempUser.ObjectId, oAltExt.ObjectId,out oNewAltExt);
            Assert.IsTrue(res.Success,"Failed to fetch the alternate extension based on valid ObjectId:"+res);
            Assert.IsTrue(oNewAltExt.ObjectId.Equals(oAltExt.ObjectId),"Fetch of existing alternate extension does not match");

            res = oAltExt.Delete();
            Assert.IsTrue(res.Success, "Calling Delete on AlternateExtension on instance method failed:" + res);


            res = AlternateExtension.GetAlternateExtension(_connectionServer, "", "test", out oAltExt);
            Assert.IsFalse(res.Success,"Calling GetAlternateExtension with empty user objectId should fail");

            res = AlternateExtension.GetAlternateExtension(_connectionServer, "bogus", "test", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with bogus user objectId should fail");
        }

        [TestMethod]
        public void AlternateExtensions_AddDelete()
        {
            //Add an alternate extension
            string strExtension = Guid.NewGuid().ToString().Replace("-", "");
            WebCallResult res = AlternateExtension.AddAlternateExtension(_connectionServer, _tempUser.ObjectId, 1, strExtension);
            Assert.IsTrue(res.Success, "Failed adding alternate extension to user:" + res.ToString());

            //Iterate the alternate extensiosn
            foreach (AlternateExtension oExt in _tempUser.AlternateExtensions(true))
            {
                Console.WriteLine(oExt.ToString());
                Console.WriteLine(oExt.DumpAllProps());
            }

            AlternateExtension oAltExt;

            //alt extension that doesn't exist should fail
            res = _tempUser.GetAlternateExtension(5, out oAltExt);
            Assert.IsFalse(res.Success, "Invalid alternate extension ID should fail to fetch");

            res = _tempUser.GetAlternateExtension(1, out oAltExt, true);
            Assert.IsTrue(res.Success, "Failed to fetch alternate extension added to new user:" + res.ToString());

            oAltExt.ClearPendingChanges();

            //update it with no outstanding items should fail
            res = oAltExt.Update();
            Assert.IsFalse(res.Success, "Updating an alternate extension with no pending changes should fail");

            //edit it
            oAltExt.DtmfAccessId = _tempUser.DtmfAccessId + "2";
            res = oAltExt.Update();
            Assert.IsTrue(res.Success, "Failed to update alternate extension added:" + res.ToString());

            //delete it
            res = oAltExt.Delete();
            Assert.IsTrue(res.Success, "Failed to delete alternate extension:" + res.ToString());

            //add alternate extension through alternate route via static method with return via out param

            res = AlternateExtension.AddAlternateExtension(_connectionServer, _tempUser.ObjectId, 2, _tempUser.DtmfAccessId + "321", out oAltExt);
            Assert.IsTrue(res.Success, "Failed adding alternate extension:" + res.ToString());

            res = oAltExt.RefetchAlternateExtensionData();
            Assert.IsTrue(res.Success, "Failed to refresh alternate extension:" + res.ToString());

            //get the alternate extension via alternative static method route - we'll cheat a bit here and just pass the 
            //ObjectId of the guy we just created for fetching - just need to exercise the code path
            res = AlternateExtension.GetAlternateExtension(_connectionServer, _tempUser.ObjectId, oAltExt.ObjectId,
                                                           out oAltExt);
            Assert.IsTrue(res.Success, "Failed to fetch newly created alternate extension:" + res.ToString());

            //one last alternative fetching code path here - create an alternate extension object and then fetch it as an 
            //instance method
            AlternateExtension oAltExt2 = new AlternateExtension(_connectionServer, _tempUser.ObjectId);
            Assert.IsNotNull(oAltExt2, "Failed to create new instance of an alternate extension");

            //some static method failures for alternate extenions

            //AddAlternateExtension
            res = AlternateExtension.AddAlternateExtension(null, "bogus", 1, "1234");
            Assert.IsFalse(res.Success, "Adding alternate extension with static AddAlternateExtension did not fail with null Connection server");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "bogus", 1, "1234");
            Assert.IsFalse(res.Success, "Adding alternate extension with static AddAlternateExtension did not fail with invalid objectId");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "", 1, "1234");
            Assert.IsFalse(res.Success, "Adding alternate extension with static AddAlternateExtension did not fail with blank objectId");

            res = AlternateExtension.AddAlternateExtension(_connectionServer, "bogus", 49, "1234");
            Assert.IsFalse(res.Success, "Adding alternate extension with static AddAlternateExtension did not fail with invalid index number");

            //DeleteAlternateExtension
            res = AlternateExtension.DeleteAlternateExtension(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Adding alternate extension with static DeleteAlternateExtension did not fail with null ConnectionServer");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Adding alternate extension with static DeleteAlternateExtension did not fail with invalid objectId");

            res = AlternateExtension.DeleteAlternateExtension(_connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Adding alternate extension with static DeleteAlternateExtension did not fail with blank objectId");

            //GetAlternateExtension
            res = AlternateExtension.GetAlternateExtension(null, "bogus", "bogus", out oAltExt);
            Assert.IsFalse(res.Success, "Getting alternate extension with static GetAlternateExtension did not fail with null Connection server");

            res = AlternateExtension.GetAlternateExtension(_connectionServer, "bogus", "bogus", out oAltExt);
            Assert.IsFalse(res.Success, "Getting alternate extension with static GetAlternateExtension did not fail with invalid objectId");

            res = AlternateExtension.GetAlternateExtension(_connectionServer, "", "bogus", out oAltExt);
            Assert.IsFalse(res.Success, "Getting alternate extension with static GetAlternateExtension did not fail with blank objectId");

            //GetAlternateExtensions
            List<AlternateExtension> oAltExts;
            res = AlternateExtension.GetAlternateExtensions(null, "bogus", out oAltExts);
            Assert.IsFalse(res.Success, "Getting alternate extensions with static GetAlternateExtensions did not fail with null Connection server");


            res = AlternateExtension.GetAlternateExtensions(_connectionServer, "", out oAltExts);
            Assert.IsFalse(res.Success, "Getting alternate extensions with static GetAlternateExtensions did not fail with empty objectId");

            //UpdateAlternateExtension
            res = AlternateExtension.UpdateAlternateExtension(null, "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "updating alternate extensions with static UpdateAlternateExtension did not fail with null Connection server");

            res = AlternateExtension.UpdateAlternateExtension(_connectionServer, "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "updating alternate extensions with static UpdateAlternateExtension did not fail with empty property list");

        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetAlternateExtension_Harness()
        {
            var oTestTransport = new Mock<IConnectionRestCalls>();

            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                });

            ConnectionServerRest oServer = new ConnectionServerRest(oTestTransport.Object, "test", "test", "test", false);

            AlternateExtension oAltExt;

            //error response
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = AlternateExtension.GetAlternateExtension(oServer, "userObjectID", "ErrorResponse", out oAltExt);
            Assert.IsFalse(res.Success,"Calling GetAlternateExtension with server error response should fail");

            //garbage response
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            res = AlternateExtension.GetAlternateExtension(oServer, "userObjectID", "InvalidResultText", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with InvalidResultText should fail");


            //empty results
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            res = AlternateExtension.GetAlternateExtension(oServer, "userObjectID", "EmptyResultText", out oAltExt);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtension with EmptyResultText should fail");
        }

        [TestMethod]
        public void GetAlternateExtensions_Harness()
        {
            var oTestTransport = new Mock<IConnectionRestCalls>();

            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                });

            ConnectionServerRest oServer = new ConnectionServerRest(oTestTransport.Object, "test", "test", "test", false);

            List<AlternateExtension> oAltExts;

            //error response
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = AlternateExtension.GetAlternateExtensions(oServer, "ErrorResponse", out oAltExts);
            Assert.IsFalse(res.Success, "Calling GetAlternateExtensions with server error response should fail");

            //garbage response
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            res = AlternateExtension.GetAlternateExtensions(oServer, "InvalidResultText", out oAltExts);
            Assert.IsTrue(res.Success, "Calling GetAlternateExtensions with InvalidResultText should not fail:"+res);
            Assert.IsTrue(oAltExts.Count==0,"Invalid text should result in empty list of alternate extensions being returned");

            //empty results
            oTestTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                It.IsAny<string>(), true)).Returns(new WebCallResult
                {
                    Success = true,
                    ResponseText = ""
                });

            res = AlternateExtension.GetAlternateExtensions(oServer, "EmptyResultText", out oAltExts);
            Assert.IsTrue(res.Success, "Calling GetAlternateExtensions with EmptyResultText should not fail:"+res);
            Assert.IsTrue(oAltExts.Count == 0, "Empty text should result in empty list of alternate extensions being returned");
        }


        #endregion
    }
}
