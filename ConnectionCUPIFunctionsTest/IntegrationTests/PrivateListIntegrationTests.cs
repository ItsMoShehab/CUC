using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PrivateListIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static UserFull _tempUser;

        private static PrivateList _tempPrivateList;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            //create new list with GUID in the name to ensure uniqueness
            String strUserAlias = "TempUserPrivList_" + Guid.NewGuid().ToString().Replace("-", "");

            //generate a random number and tack it onto the end of some zeros so we're sure to avoid any legit numbers on the system.
            Random random = new Random();
            int iExtPostfix = random.Next(100000, 999999);
            string strExtension = "000000" + iExtPostfix.ToString();

            //use a bogus extension number that's legal but non dialable to avoid conflicts
            WebCallResult res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", strUserAlias, strExtension, null, out _tempUser);
            Assert.IsTrue(res.Success, "Failed creating temporary user:" + res.ToString());

            //create a new private list off the new user to test with
            res = PrivateList.AddPrivateList(_connectionServer, _tempUser.ObjectId, "Test Private List1", 1, out _tempPrivateList);
            Assert.IsTrue(res.Success, "Failed to create new private list for user:" + res);
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempPrivateList != null)
            {
                WebCallResult res = _tempPrivateList.Delete();
                Assert.IsTrue(res.Success,"Failed to delete temporary private list on cleanup");
            }

            if (_tempUser != null)
            {
                WebCallResult res = _tempUser.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary user on cleanup.");
            }
        }

        #endregion


        #region Constructor Tests

        /// <summary>
        /// UnityConnectionRestException on invalid ObjectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            PrivateList oTest = new PrivateList(_connectionServer, "blah","blah");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failure Tests 

        [TestMethod]
        public void AddPrivateList_InvalidObjectId_Failure()
        {
            var res = PrivateList.AddPrivateList(_connectionServer, "objectid", "display name", 1);
            Assert.IsFalse(res.Success, "Adding private list with invalid UserObjectId did not fail");
        }

        [TestMethod]
        public void AddPrivateList_InvalidListNumber_Failure()
        {
            var res = PrivateList.AddPrivateList(_connectionServer, _tempPrivateList.ObjectId, "display name", 200);
            Assert.IsFalse(res.Success, "Adding private list with invalid numeric id did not fail");
        }

        [TestMethod]
        public void GetPrivateList_InvalidOwnerObjectId_Failure()
        {
            PrivateList oNewPrivateList;

            var res = PrivateList.GetPrivateList(out oNewPrivateList, _connectionServer, "ownerobjectid");
            Assert.IsFalse(res.Success, "Fetching private list with invalid user objectId did not fail");
        }

        [TestMethod]
        public void GetPrivateList_InvalidListObjectId_Failure()
        {
            PrivateList oNewPrivateList; 
            var res = PrivateList.GetPrivateList(out oNewPrivateList, _connectionServer, _tempUser.ObjectId, "blah");
            Assert.IsFalse(res.Success, "Fetching private list with invalid objectId did not fail");
        }

        [TestMethod]
        public void GetPrivateList_InvalidListId_Failure()
        {
            PrivateList oNewPrivateList;
            var res = PrivateList.GetPrivateList(out oNewPrivateList, _connectionServer, _tempUser.ObjectId, "",999);
            Assert.IsFalse(res.Success, "Fetching private list with invalid listId should fail");
        }

        [TestMethod]
        public void UpdatePrivateList_InvalidObjectIdAndOwnerObjectId_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("blah", "blah");
            var res = PrivateList.UpdatePrivateList(_connectionServer, "Object", oProps, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with invalid owner and objectIds ");
        }

        [TestMethod]
        public void DeletePrivateList_InvalidUserObjectIdAndObjectId_Failure()
        {
            var res = PrivateList.DeletePrivateList(_connectionServer, "Object", "UserObjectId");
            Assert.IsFalse(res.Success, "Deleting private list via static method did not fail with invalid user and ObjectIds");
        }


        [TestMethod]
        public void GetPrivateListVoiceName_InvalidObjectId_Failure()
        {
            var res = PrivateList.GetPrivateListVoiceName(_connectionServer, "userobjectid", "c:\\temp.wav", "ObjectId",
                                                          "wavname");
            Assert.IsFalse(res.Success,
                           "Getting private list voice name via static method did not fail with invalid objectId");
        }

        [TestMethod]
        public void SetPrivateListVoiceName_InvalidObjectId_Failure()
        {
            var res = PrivateList.SetPrivateListVoiceName(_connectionServer, "c:\\temp.wav", "objectid", "UserObjectId", true);
            Assert.IsFalse(res.Success, "Setting private list voice name via static method did not fail with ObjectId");
        }


        [TestMethod]
        public void AddMemberPublicList_InvalidObjectIds_Failure()
        {
            var res = PrivateList.AddMemberPublicList(_connectionServer, "PrivateListObjectId", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list public DL member via static method did not fail with invalid objectId");
        }

        [TestMethod]
        public void AddMemberUser_InvalidObjectIds_Failure()
        {
            var res = PrivateList.AddMemberUser(_connectionServer, "PrivateListObjectId", "UserObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list user member via static method did not fail with invalid objectIds");
        }


        [TestMethod]
        public void GetPrivateLists_Success()
        {
            List<PrivateList> oLists;
            var res = PrivateList.GetPrivateLists(_connectionServer, _tempUser.ObjectId, out oLists);
            Assert.IsTrue(res.Success, "Failed fetching private lists for user:" + res);
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void PrivateListVoiceName()
        {
            WebCallResult res = _tempPrivateList.SetVoiceName("blah.blah");
            Assert.IsFalse(res.Success, "Voice name update with invalid file name did not fail");

            res = _tempPrivateList.SetVoiceName("Dummy.wav", true);
            Assert.IsTrue(res.Success, "Failed updating private list voice name:" + res);

            //fetch updated VoiceName string by reloading the private list
            PrivateList oPrivateList = null;
            try
            {
                oPrivateList = new PrivateList(_connectionServer, _tempPrivateList.UserObjectId, _tempPrivateList.ObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create updated private list instance:" + ex);
            }

            res = oPrivateList.GetVoiceName("DummyDownload.wav");
            Assert.IsTrue(res.Success, "Failed fetching private list voice name:" + res);

        }

        [TestMethod]
        public void PrivateListTopLevelUpdate()
        {
            PrivateList oTestList;
            try
            {
                oTestList = new PrivateList(_connectionServer, _tempUser.ObjectId, "", 1);
                Console.WriteLine(oTestList);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new private list class instance with list Id of 1" + ex);
            }

            try
            {
                oTestList = new PrivateList(_connectionServer, _tempUser.ObjectId, _tempPrivateList.ObjectId);
                Console.WriteLine(oTestList);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new private list class instance with valid ObjectId" + ex);
            }

            WebCallResult res = _tempPrivateList.Update();
            Assert.IsFalse(res.Success, "Calling Update with no pending changes did not result in an error");

            Console.WriteLine(_tempPrivateList.ToString());
            Console.WriteLine(_tempPrivateList.DumpAllProps());

            res = _tempPrivateList.AddMemberUser(_tempUser.ObjectId);
            Assert.IsTrue(res.Success, "Failed to add user to private list:" + res);

            List<DistributionList> oPublicLists;
            res = DistributionList.GetDistributionLists(_connectionServer, out oPublicLists,1,20);
            Assert.IsTrue(res.Success, "Failed to fetch public lists:" + res);
            Assert.IsTrue(oPublicLists.Count>0,"No public lists found");

            res = _tempPrivateList.AddMemberPublicList(oPublicLists[0].ObjectId);
            Assert.IsTrue(res.Success, "Failed to add public list as private list member:" + res);

            List<PrivateListMember> oMembers;
            res = _tempPrivateList.GetMembersList(out oMembers);
            Assert.IsTrue(res.Success, "Failed to fetch members of private list:" + res);
            Assert.IsTrue(oMembers.Count==2,"Two members not returned from new private list");

            res = _tempPrivateList.RemoveMember(oMembers[0].ObjectId);
            Assert.IsTrue(res.Success, "Failed removing private list member:" + res);

            _tempPrivateList.DisplayName = "New display name";
            res = _tempPrivateList.Update();
            Assert.IsTrue(res.Success, "Failed updating private list:" + res);

            res = _tempPrivateList.RefetchPrivateListData();
            Assert.IsTrue(res.Success,"Failed to refetch private list data:"+res);
        }


         [TestMethod]
        public void TestPrivateListMembers()
        {
            List<PrivateListMember> oMembers;
            WebCallResult res = _tempPrivateList.GetMembersList(out oMembers);
            Assert.IsTrue(res.Success,"Failed to get a private list with members passed to TestPrivateListMembers");

            Assert.IsTrue(oMembers.Count>0,"No members in the private list passed to TestPrivateListMembers");

            Console.WriteLine(oMembers[0].ToString());
            Console.WriteLine(oMembers[0].DumpAllProps());

            List<PrivateListMember> oMembers2 = _tempPrivateList.PrivateListMembers(true);
            Assert.IsNotNull(oMembers2, "Null returned for private list member fetch");
            Assert.IsTrue(oMembers2.Count == oMembers.Count, "Fetch of members via static vs instance methods do not match");
        }

       #endregion

    }
}
