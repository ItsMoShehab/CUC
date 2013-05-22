using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PrivateListTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private static UserFull _tempUser;

        private static PrivateList _tempPrivateList;

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
                 _connectionServer = new ConnectionServer(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start PrivateList test:" + ex.Message);
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
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            PrivateList oTest = new PrivateList(null,"blah","blah");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a empty objectId passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            PrivateList oTest = new PrivateList(_connectionServer, "", "blah");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid ObjectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            PrivateList oTest = new PrivateList(_connectionServer, "blah","blah");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Static Call Failure Tests 

        [TestMethod]
        public void StaticCallFailures_AddPrivateList()
        {
            var res = PrivateList.AddPrivateList(null, "blah", "blah", 1);
            Assert.IsFalse(res.Success, "Adding private list with null connection server did not fail");

            res = PrivateList.AddPrivateList(_connectionServer, "blah", "blah", 1);
            Assert.IsFalse(res.Success, "Adding private list with invalid UserObjectId did not fail");

            res = PrivateList.AddPrivateList(_connectionServer, "", "blah", 1);
            Assert.IsFalse(res.Success, "Adding private list with blank UserObjectId did not fail");

            res = PrivateList.AddPrivateList(_connectionServer, "blah", "", 1);
            Assert.IsFalse(res.Success, "Adding private list with blank display name did not fail");

            res = PrivateList.AddPrivateList(_connectionServer, "blah", "blah", 200);
            Assert.IsFalse(res.Success, "Adding private list with invalid numeric id did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_GetPrivateList()
        {
            PrivateList oNewPrivateList;

            var res = PrivateList.GetPrivateList(out oNewPrivateList, null, "blah");
            Assert.IsFalse(res.Success, "Fetching private list with null ConnectionServer did not fail");

            res = PrivateList.GetPrivateList(out oNewPrivateList, _connectionServer, "blah");
            Assert.IsFalse(res.Success, "Fetching private list with invalid user objectId did not fail");

            res = PrivateList.GetPrivateList(out oNewPrivateList, _connectionServer, "");
            Assert.IsFalse(res.Success, "Fetching private list with empty owner objectId not fail");
        }

        [TestMethod]
        public void StaticCallFailures_UpdatePrivateList()
        {
            var res = PrivateList.UpdatePrivateList(null, "Object", null, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with null ConnectionServer");

            ConnectionPropertyList oProps = new ConnectionPropertyList();

            res = PrivateList.UpdatePrivateList(_connectionServer, "", oProps, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with empty objectId");

            res = PrivateList.UpdatePrivateList(_connectionServer, "Object", oProps, "");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with empty owner id");

            res = PrivateList.UpdatePrivateList(_connectionServer, "Object", oProps, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with invalid owner and objectIds ");

            oProps.Add("blah", "blah");
            res = PrivateList.UpdatePrivateList(_connectionServer, "Object", oProps, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with invalid owner and objectIds ");
        }

        [TestMethod]
        public void StaticCallFailures_DeletePrivateList()
        {
            var res = PrivateList.DeletePrivateList(null, "Object", "UserObjectId");
            Assert.IsFalse(res.Success, "Deleting private list via static method did not fail with invalid null ConnectionString");

            res = PrivateList.DeletePrivateList(_connectionServer, "Object", "UserObjectId");
            Assert.IsFalse(res.Success, "Deleting private list via static method did not fail with invalid user and ObjectIds");
        }


        [TestMethod]
        public void StaticCallFailures_GetPrivateListVoiceName()
        {
            var res = PrivateList.GetPrivateListVoiceName(null, "UserObjectId", @"c:\", "ObjectId", "WavName");
            Assert.IsFalse(res.Success, "Getting private list voice name via static method did not fail with null Connection server ");

            res = PrivateList.GetPrivateListVoiceName(_connectionServer, "", @"c:\", "ObjectId", "WavName");
            Assert.IsFalse(res.Success, "Getting private list voice name via static method did not fail with empty user objectId ");

            res = PrivateList.GetPrivateListVoiceName(_connectionServer, "UserObjectId", "", "ObjectId", "");
            Assert.IsFalse(res.Success, "Getting private list voice name via static method did not fail with empty wav file name and target path ");


            res = PrivateList.SetPrivateListVoiceName(null, "UserObjectId", "", "ObjectId", true);
            Assert.IsFalse(res.Success, "Setting private list voice name via static method did not fail with null ConnectionServer");

            res = PrivateList.SetPrivateListVoiceName(_connectionServer, "UserObjectId", "", "ObjectId", true);
            Assert.IsFalse(res.Success, "Setting private list voice name via static method did not fail with ObjectId");
        }


        [TestMethod]
        public void StaticCallFailures_AddMemberPublicList()
        {
            var res = PrivateList.AddMemberPublicList(null, "ObjectId", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list public DL member via static method did not fail with null ConnectionServer ");

            res = PrivateList.AddMemberPublicList(_connectionServer, "", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list public DL member via static method did not fail with empty private list objectId");
        }

        [TestMethod]
        public void StaticCallFailures_AddMemberUser()
        {
            var res = PrivateList.AddMemberUser(null, "ObjectId", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list user member via static method did not fail with null ConnectionServer ");

            res = PrivateList.AddMemberUser(_connectionServer, "", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list user member via static method did not fail with empty private list objectId");
        }


        [TestMethod]
        public void StaticCallFailures_GetPrivateLists()
        {
            //static method calls
            List<PrivateList> oLists;
            WebCallResult res = PrivateList.GetPrivateLists(null, "blah", out oLists);
            Assert.IsFalse(res.Success, "Fetching private lists with null ConnectionServer did not fail");

            res = PrivateList.GetPrivateLists(_connectionServer, _tempUser.ObjectId, out oLists);
            Assert.IsTrue(res.Success, "Failed fetching private lists for user:" + res);
        }

        #endregion


        #region Live Testts

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

            res = PrivateListMember.GetPrivateListMembers(null, _tempPrivateList.ObjectId, _tempPrivateList.UserObjectId, out oMembers);
            Assert.IsFalse(res.Success,"Getting private list members via static method did not fail with null Connection server");

            res = PrivateListMember.GetPrivateListMembers(_connectionServer, "", _tempPrivateList.UserObjectId, out oMembers);
            Assert.IsFalse(res.Success, "Getting private list members via static method did not fail with blank private list ObjectId");

            res = PrivateListMember.GetPrivateListMembers(_connectionServer, _tempPrivateList.ObjectId, "", out oMembers);
            Assert.IsFalse(res.Success, "Getting private list members via static method did not fail with blank owner ObjectId");
        }

        #endregion
    }
}
