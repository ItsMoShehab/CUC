using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Summary description for UserIntegrationTests
    /// </summary>
    [TestClass]
    public class UserUnitTests : BaseUnitTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties
        
        UserFull _tempUserFull = new UserFull(_mockServer);
        
        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Class Creation Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserBase_Constructor_NullConnectionServer_Failure()
        {
            UserBase oTemp = new UserBase(null);
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void UserBase_EmptyConstructor_Success()
        {
            UserBase oTemp = new UserBase();
            Assert.IsFalse(oTemp == null, "Empty class creation did not create a UserBase");
        }

        [TestMethod]
        public void UserFull_EmptyConstructor_Success()
        {
            UserBase oTemp = new UserFull(_mockServer);
            Assert.IsFalse(oTemp == null, "Empty class creation did not create a UserFull");
            Console.WriteLine(oTemp.ToString());
            Console.WriteLine(oTemp.DumpAllProps());
            Console.WriteLine(oTemp.SelectionDisplayString);
            Console.WriteLine(oTemp.UniqueIdentifier);
        }


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserTemplate_Constructor_NullConnectionServer_Failure()
        {
            UserTemplate oTemp = new UserTemplate(null, "ObjectId");
            Console.WriteLine(oTemp);
        }

       
        #endregion


        #region Static Call Failure Tests

        [TestMethod]
        public void GetUser_NullConnectionServer_Failure()
        {
            UserBase oUserBase;

            WebCallResult res = UserBase.GetUser(out oUserBase, null, "alias");
            Assert.IsFalse(res.Success, "Null Connection server object should fail");
        }

        [TestMethod]
        public void GetUser_EmptyObjectIdAndAlias_Failure()
        {
            UserBase oUserBase;

            var res = UserBase.GetUser(out oUserBase, _mockServer, "");
            Assert.IsFalse(res.Success, "Empty User ObjectId and alias should fail");
        }

        [TestMethod]
        public void UserFull_GetUser_NullConnectionServer_Failure()
        {
            UserFull oUserFull;
            var res = UserFull.GetUser(null, "ObjectId", out oUserFull);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");
        }

        [TestMethod]
        public void UserFull_GetUser_EmptyObjectId_Failure()
        {
            UserFull oUserFull;
            var res = UserFull.GetUser(_mockServer, "", out oUserFull);
            Assert.IsFalse(res.Success, "Empty User ObjectId should fail");
        }

        [TestMethod]
        public void GetUsers_NullConnectionServer_Failure()
        {
            List<UserBase> oUserList;

            var res = UserBase.GetUsers(null, out oUserList);
            Assert.IsFalse(res.Success, "Null Connection server object should fail");
        }

        [TestMethod]
        public void UpdateUser_EmptyObjectId_Failure()
        {
            ConnectionPropertyList oPropList = new ConnectionPropertyList();
            oPropList.Add("test", "value");

            var res = UserBase.UpdateUser(_mockServer, "", oPropList);
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");
        }

        [TestMethod]
        public void UpdateUser_NullConnectionServer_Failure()
        {
            ConnectionPropertyList oPropList = new ConnectionPropertyList();
            oPropList.Add("test", "value");

            var res = UserBase.UpdateUser(null, "ObjectId", oPropList);
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");
        }

        [TestMethod]
        public void UpdateUser_NullPropertyList_Failure()
        {
            var res = UserBase.UpdateUser(_mockServer, "ObjectId", null);
            Assert.IsFalse(res.Success, "Null property list should fail");
        }

        [TestMethod]
        public void UpdateUser_EmptyPropertyList_Failure()
        {
            var res = UserBase.UpdateUser(_mockServer, "ObjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Empty property list should fail");
        }

        [TestMethod]
        public void GetUserVoiceName_EmptyObjectId_Failure()
        {
            var res = UserBase.GetUserVoiceName(_mockServer, "temp.wav", "");
            Assert.IsFalse(res.Success, "Empty user objectID should fail");
         }

        [TestMethod]
        public void GetUserVoiceName_EmptyTargetWavPath_Failure()
        {
            var res = UserBase.GetUserVoiceName(_mockServer, "", "ObjectId");
            Assert.IsFalse(res.Success, "Empty target wav path should fail");
        }

        [TestMethod]
        public void GetUserVoiceName_NullConnectionServer_Failure()
        {
            var res = UserBase.GetUserVoiceName(null, "temp.wav", "ObjectId");
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");
        }

        [TestMethod]
        public void SetUserVoiceName_EmptyObjectId_Failure()
        {
            var res = UserBase.SetUserVoiceName(_mockServer, "Dummy.wav", "");
            Assert.IsFalse(res.Success, "Empty user objectID should fail");

         }

        [TestMethod]
        public void SetUserVoiceName_EmptyTargetWavPath_Failure()
        {
            var res = UserBase.SetUserVoiceName(_mockServer, "", "ObjectId");
            Assert.IsFalse(res.Success, "Empty target wav path should fail");
        }

        [TestMethod]
        public void SetUserVoiceName_NullConnectionServer_Failure()
        {
            var res = UserBase.SetUserVoiceName(null, "Dummy.wav", "ObjectId");
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");
        }

        [TestMethod]
        public void SetUserVoiceNameToStreamFile_EmptyObjectId_Failure()
        {
            var res = UserBase.SetUserVoiceNameToStreamFile(_mockServer, "", "StreamFileName");
            Assert.IsFalse(res.Success, "Empty user objectID should fail");
        }

        [TestMethod]
        public void SetUserVoiceNameToStreamFile_EmptyStreamName_Failure()
        {
            var res = UserBase.SetUserVoiceNameToStreamFile(_mockServer, "ObjectId", "");
            Assert.IsFalse(res.Success, "Empty wav Stream file nane should fail");
        }

        [TestMethod]
        public void SetUserVoiceNameToStreamFile_NullConnectionServer_Failure()
        {
            var res = UserBase.SetUserVoiceNameToStreamFile(null, "ObjectId", "StreamFileName");
            Assert.IsFalse(res.Success, "Null Connection Server object should fail");
        }

        [TestMethod]
        public void AddUser_NullConnectionServer_Failure()
        {
            WebCallResult res = UserBase.AddUser(null, "voicemailusertemplate", "alias", "1234", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the ConnectionServerRest parameter is null");
        }

        [TestMethod]
        public void AddUser_EmptyTemplateAlias_Failure()
        {
            var res = UserBase.AddUser(_mockServer, "", "Alias", "123", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the template parameter is empty");
        }

        [TestMethod]
        public void AddUser_EmptyExtension_Failure()
        {
            var res = UserBase.AddUser(_mockServer, "voicemailusertemplate", "Alias", "", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the extension parameter is empty");
        }

        [TestMethod]
        public void AddUser_EmptyDisplayName_Failure()
        {
            var res = UserBase.AddUser(_mockServer, "voicemailusertemplate", "", "1234", null);
            Assert.IsFalse(res.Success, "AddUser should fail if the DisplayName parameter is empty");
        }

        [TestMethod]
        public void DeleteUser_EmptyObjectId_Failure()
        {
            var res = UserBase.DeleteUser(_mockServer, "");
            Assert.IsFalse(res.Success, "Empty ObjectId should fail");

            }

        [TestMethod]
        public void DeleteUser_NullConnectionServer_Failure()
        {
            var res = UserBase.DeleteUser(null, "ObjectId");
            Assert.IsFalse(res.Success, "Null ConnectionServerRest object should fail");

        }

        #endregion


        #region User Compare Tests

        [TestMethod]
        public void UserCompareTests_FirstName()
        {
            UserBase oUser1=new UserBase();
            UserBase oUser2=new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUsers.Sort(new UserComparer("firstname"));
            oUser1.FirstName = "bbb";
            oUsers.Sort(new UserComparer("firstname"));
            Assert.IsTrue(oUsers[0]==oUser1);

            oUser2.FirstName = "aaa";
            oUsers.Sort(new UserComparer("firstname"));
            Assert.IsTrue(oUsers[0] == oUser2);

        }

        [TestMethod]
        public void UserCompareTests_LastName()
        {
            UserBase oUser1 = new UserBase();
            UserBase oUser2 = new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUsers.Sort(new UserComparer("lastname"));
            oUser1.LastName = "bbb";
            oUsers.Sort(new UserComparer("lastname"));
            Assert.IsTrue(oUsers[0] == oUser1);

            oUser2.LastName = "aaa";
            oUsers.Sort(new UserComparer("lastname"));
            Assert.IsTrue(oUsers[0] == oUser2);
        }

        [TestMethod]
        public void UserCompareTests_DisplayName()
        {
            UserBase oUser1 = new UserBase();
            UserBase oUser2 = new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUsers.Sort(new UserComparer("displayname"));
            oUser1.DisplayName = "bbb";
            oUsers.Sort(new UserComparer("displayname"));
            Assert.IsTrue(oUsers[0] == oUser1);

            oUser2.DisplayName = "aaa";
            oUsers.Sort(new UserComparer("displayname"));
            Assert.IsTrue(oUsers[0] == oUser2);

        }

        [TestMethod]
        public void UserCompareTests_Extension()
        {
            UserBase oUser1 = new UserBase();
            UserBase oUser2 = new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUser1.DtmfAccessId = "2222";
            oUser2.DtmfAccessId = "1111";
            oUsers.Sort(new UserComparer("dtmfaccessid"));
            Assert.IsTrue(oUsers[0] == oUser2);

        }

        [TestMethod]
        public void UserCompareTests_Alias()
        {
            UserBase oUser1 = new UserBase();
            UserBase oUser2 = new UserBase();
            List<UserBase> oUsers = new List<UserBase>();
            oUsers.Add(oUser1);
            oUsers.Add(oUser2);

            oUser1.Alias = "bbbb";
            oUser2.Alias = "aaaa";
            oUsers.Sort(new UserComparer("alias"));
            Assert.IsTrue(oUsers[0] == oUser2);

        }

        
        #endregion

        
        #region Harness Tests

        [ExpectedException(typeof(UnityConnectionRestException))]
        [TestMethod]
        public void UserFull_Constructor_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = false,
                                               ResponseText = "error text",
                                               StatusCode = 404
                                           });
            
          UserFull oUser = new UserFull(_mockServer,"objectid");
        }


        [TestMethod]
        public void UserFull_GetUser_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = false,
                                           ResponseText = "error text",
                                           StatusCode = 404
                                       });

            UserFull oUser;
            var res = UserFull.GetUser(_mockServer, "objectid", out oUser);
            Assert.IsFalse(res.Success,"Calling GetUser with error respone should fail");
        }

        #endregion

    }
}
