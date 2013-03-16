using System;
using System.Collections.Generic;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PrivateListTest
    {

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

        }

        #endregion


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            PrivateList oTest = new PrivateList(null,"blah","blah");
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure2()
        {
            PrivateList oTest = new PrivateList(_connectionServer, "", "blah");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            PrivateList oTest = new PrivateList(_connectionServer, "blah","blah");
        }


        [TestMethod]
        public void PrivateListAddUpdateDelete()
        {
            UserFull oUser;

             //create new list with GUID in the name to ensure uniqueness
            String strUserAlias = "TempUser_" + Guid.NewGuid().ToString().Replace("-", "");

            //generate a random number and tack it onto the end of some zeros so we're sure to avoid any legit numbers on the system.
            Random random = new Random();
            int iExtPostfix = random.Next(100000, 999999);
            string strExtension = "000000" + iExtPostfix.ToString();

            //use a bogus extension number that's legal but non dialable to avoid conflicts
            WebCallResult res = UserBase.AddUser(_connectionServer, "voicemailusertemplate", strUserAlias, strExtension, null, out oUser);
            Assert.IsTrue(res.Success, "Failed creating temporary user:" + res.ToString());

            //create a new private list off the new user to test with
            PrivateList oPrivateList;
            res = PrivateList.AddPrivateList(_connectionServer, oUser.ObjectId, "Test Private List1", 1,out oPrivateList);
            Assert.IsTrue(res.Success,"Failed to create new private list for user:"+res);

            PrivateList oTestList;
            try
            {
                oTestList = new PrivateList(_connectionServer, oUser.ObjectId, "", 1);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new private list class instance with list Id of 1" + ex);
            }

            try
            {
                oTestList = new PrivateList(_connectionServer, oUser.ObjectId, oPrivateList.ObjectId, 0);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new private list class instance with valid ObjectId" + ex);
            }

            res = oPrivateList.Update();
            Assert.IsFalse(res.Success, "Calling Update with no pending changes did not result in an error");


            Console.WriteLine(oPrivateList.ToString());
            Console.WriteLine(oPrivateList.DumpAllProps());

            res = oPrivateList.AddMemberUser(oUser.ObjectId);
            Assert.IsTrue(res.Success, "Failed to add user to private list:" + res);

            List<DistributionList> oPublicLists;
            res = DistributionList.GetDistributionLists(_connectionServer, out oPublicLists);
            Assert.IsTrue(res.Success, "Failed to fetch public lists:" + res);
            Assert.IsTrue(oPublicLists.Count>0,"No public lists found");

            res = oPrivateList.AddMemberPublicList(oPublicLists[0].ObjectId);
            Assert.IsTrue(res.Success, "Failed to add public list as private list member:" + res);

            List<PrivateListMember> oMembers;
            res = oPrivateList.GetMembersList(out oMembers);
            Assert.IsTrue(res.Success, "Failed to fetch members of private list:" + res);
            Assert.IsTrue(oMembers.Count==2,"Two members not returned from new private list");

            //Run through private list member tests while we have the list created with a couple members
            TestPrivateListMembers(oPrivateList);


            res = oPrivateList.RemoveMember(oMembers[0].ObjectId);
            Assert.IsTrue(res.Success, "Failed removing private list member:" + res);

            oPrivateList.DisplayName = "New display name";
            res = oPrivateList.Update();
            Assert.IsTrue(res.Success, "Failed updating private list:" + res);

            res = oPrivateList.SetVoiceName("blah.blah");
            Assert.IsFalse(res.Success, "Voice name update with invalid file name did not fail");

            res = oPrivateList.SetVoiceName("Dummy.wav",true);
            Assert.IsTrue(res.Success, "Failed updating private list voice name:" + res);

            //fetch updated VoiceName string by reloading the private list
            try
            {
                oPrivateList = new PrivateList(_connectionServer, oPrivateList.UserObjectId, oPrivateList.ObjectId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create updated private list instance:"+ex);
            }
            
            res = oPrivateList.GetVoiceName("DummyDownload.wav");
            Assert.IsTrue(res.Success, "Failed fetching private list voice name:" + res);


            //static method calls
            PrivateList oNewPrivateList;
            List<PrivateList> oLists;
            res = PrivateList.GetPrivateLists(null, "blah", out oLists);
            Assert.IsFalse(res.Success, "Fetching private lists with null ConnectionServer did not fail");

            res = PrivateList.GetPrivateLists(_connectionServer, oUser.ObjectId, out oLists);
            Assert.IsTrue(res.Success, "Failed fetching private lists for user:" + res);

            res = PrivateList.AddPrivateList(null, "blah","blah",1);
            Assert.IsFalse(res.Success, "Adding private list with null connection server did not fail");

            res = PrivateList.AddPrivateList(_connectionServer, "blah", "blah", 1);
            Assert.IsFalse(res.Success, "Adding private list with invalid UserObjectId did not fail");

            res = PrivateList.AddPrivateList(_connectionServer, "", "blah", 1);
            Assert.IsFalse(res.Success, "Adding private list with blank UserObjectId did not fail");

            res = PrivateList.AddPrivateList(_connectionServer, "blah", "", 1);
            Assert.IsFalse(res.Success, "Adding private list with blank display name did not fail");

            res = PrivateList.AddPrivateList(_connectionServer, "blah", "blah", 200);
            Assert.IsFalse(res.Success, "Adding private list with invalid numeric id did not fail");

            res = PrivateList.GetPrivateList(out oNewPrivateList, null, "blah");
            Assert.IsFalse(res.Success, "Fetching private list with null ConnectionServer did not fail");

            res = PrivateList.GetPrivateList(out oNewPrivateList, _connectionServer, "blah");
            Assert.IsFalse(res.Success, "Fetching private list with invalid user objectId did not fail");
            
            res = PrivateList.GetPrivateList(out oNewPrivateList, _connectionServer, "");
            Assert.IsFalse(res.Success, "Fetching private list with empty owner objectId not fail");


            res = PrivateList.UpdatePrivateList(null, "Object",null,"ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with null ConnectionServer");

            ConnectionPropertyList oProps = new ConnectionPropertyList();

            res = PrivateList.UpdatePrivateList(_connectionServer, "", oProps, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with empty objectId");

            res = PrivateList.UpdatePrivateList(_connectionServer, "Object", oProps, "");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with empty owner id");

            res = PrivateList.UpdatePrivateList(_connectionServer, "Object", oProps, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with invalid owner and objectIds ");

            oProps.Add("blah","blah");
            res = PrivateList.UpdatePrivateList(_connectionServer, "Object", oProps, "ownerId");
            Assert.IsFalse(res.Success, "Updating private list via static method did not fail with invalid owner and objectIds ");

            res = PrivateList.DeletePrivateList(null, "Object","UserObjectId");
            Assert.IsFalse(res.Success, "Deleting private list via static method did not fail with invalid null ConnectionString");

            res = PrivateList.DeletePrivateList(_connectionServer, "Object", "UserObjectId");
            Assert.IsFalse(res.Success, "Deleting private list via static method did not fail with invalid user and ObjectIds");


            res = PrivateList.GetPrivateListVoiceName(null, "UserObjectId", @"c:\","ObjectId","WavName");
            Assert.IsFalse(res.Success, "Getting private list voice name via static method did not fail with null Connection server ");

            res = PrivateList.GetPrivateListVoiceName(_connectionServer, "", @"c:\", "ObjectId", "WavName");
            Assert.IsFalse(res.Success, "Getting private list voice name via static method did not fail with empty user objectId ");

            res = PrivateList.GetPrivateListVoiceName(_connectionServer, "UserObjectId", "", "ObjectId", "");
            Assert.IsFalse(res.Success, "Getting private list voice name via static method did not fail with empty wav file name and target path ");


            res = PrivateList.SetPrivateListVoiceName(null, "UserObjectId", "", "ObjectId", true);
            Assert.IsFalse(res.Success, "Setting private list voice name via static method did not fail with null ConnectionServer");

            res = PrivateList.SetPrivateListVoiceName(_connectionServer, "UserObjectId", "", "ObjectId",true);
            Assert.IsFalse(res.Success, "Setting private list voice name via static method did not fail with ObjectId");

            res = PrivateList.AddMemberPublicList(null, "ObjectId", "PublicObjectId","OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list public DL member via static method did not fail with null ConnectionServer ");

            res = PrivateList.AddMemberPublicList(_connectionServer, "", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list public DL member via static method did not fail with empty private list objectId");

            res = PrivateList.AddMemberUser(null, "ObjectId", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list user member via static method did not fail with null ConnectionServer ");

            res = PrivateList.AddMemberUser(_connectionServer, "", "PublicObjectId", "OwnerObjectId");
            Assert.IsFalse(res.Success, "Adding private list user member via static method did not fail with empty private list objectId");


            res = oPrivateList.Delete();
            Assert.IsTrue(res.Success, "Failed deleting private list:" + res);

            res = oUser.Delete();
            Assert.IsTrue(res.Success, "Failed deleting temporary user:" + res);
        }



        private void TestPrivateListMembers(PrivateList oPrivateList)
        {
            List<PrivateListMember> oMembers;
            WebCallResult res = oPrivateList.GetMembersList(out oMembers);
            Assert.IsTrue(res.Success,"Failed to get a private list with members passed to TestPrivateListMembers");

            Assert.IsTrue(oMembers.Count>0,"No members in the private list passed to TestPrivateListMembers");

            foreach (var oMember in oMembers)
            {
                Console.WriteLine(oMember.ToString());
                Console.WriteLine(oMember.DumpAllProps());
            }

            res =PrivateListMember.GetPrivateListMembers(null, oPrivateList.ObjectId, oPrivateList.UserObjectId, out oMembers);
            Assert.IsFalse(res.Success,"Getting private list members via static method did not fail with null Connection server");

            res = PrivateListMember.GetPrivateListMembers(_connectionServer, "", oPrivateList.UserObjectId, out oMembers);
            Assert.IsFalse(res.Success, "Getting private list members via static method did not fail with blank private list ObjectId");

            res = PrivateListMember.GetPrivateListMembers(_connectionServer, oPrivateList.ObjectId, "", out oMembers);
            Assert.IsFalse(res.Success, "Getting private list members via static method did not fail with blank owner ObjectId");

        }
    }
}
