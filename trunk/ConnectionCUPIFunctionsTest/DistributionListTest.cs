using System.IO;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ConnectionCUPIFunctionsTest
{


    /// <summary>
    ///This is a test class for DistributionListTest and is intended
    ///to contain all DistributionListTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DistributionListTest
    {

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
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
                throw new Exception("Unable to attach to Connection server to start DistributionList test:" + ex.Message);
            }
        }

        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreation_Failure()
        {

            DistributionList oTestList = new DistributionList(null);

        }

        /// <summary>
        ///A test for GetDistributionListVoiceName - this exercises the GetDistribitonList as well since it fetches the 
        /// AllVoiceMailUsers list which should always have a voice name.
        ///</summary>
        [TestMethod()]
        public void GetDistributionListVoiceName_Test()
        {
            WebCallResult res;

            //use the same string for the alias and display name here
            String strWavName = "TempWAV_" + Guid.NewGuid().ToString();

            //create new list with GUID in the name to ensure uniqueness
            DistributionList oList;

            res = DistributionList.GetDistributionList(out oList, _connectionServer, "", "allvoicemailusers");

            Assert.IsTrue(res.Success, "Fetch of AllVoiceMailUsers list failed:" + res.ToString());
            Assert.AreNotEqual(oList, null, "Fetch of AllVoiceMailUsers list failed returning an empty list");

            //first, get from static method without voice name
            res = DistributionList.GetDistributionListVoiceName(_connectionServer, strWavName, oList.ObjectId, "");
            Assert.IsTrue(res.Success, "Static method fetch of voice name for allVoiceMailUsers failed:" + res.ToString());
            Assert.IsTrue(File.Exists(strWavName), "Static method voice name fetch did not produce a wav file as expected.");

            File.Delete(strWavName);

            //generate new wav name just to be save
            strWavName = "TempWAV_" + Guid.NewGuid().ToString();

            //now fetch from list instance
            res = oList.GetVoiceName(strWavName);

            Assert.IsTrue(res.Success, "Instance method voice name fetch failed:" + res.ToString());
            Assert.IsTrue(File.Exists(strWavName), "Instance method voice name fetch did not produce a wav file as expected.");

            //clean up the temporary WAV file name
            File.Delete(strWavName);
        }


        /// <summary>
        /// Test common failure scenarios distribution list functions
        /// </summary>
        [TestMethod()]
        public void GetDistributionListTest_Failure()
        {
            WebCallResult res;

            //create new list with GUID in the name to ensure uniqueness
            DistributionList oList;

            //null connection server object
            res = DistributionList.GetDistributionList(out oList, null, "", "allvoicemailusers");
            Assert.IsFalse(res.Success, "Null Connection server on GetDistributionList did not fail.");

            //invalid alias/objectId pair
            res = DistributionList.GetDistributionList(out oList, _connectionServer, "", "");
            Assert.IsFalse(res.Success, "Blank alias/objectID params on GetDistributionList did not fail");
        }

        /// <summary>
        /// exercise GetDistributionLists failure points
        /// </summary>
        [TestMethod()]
        public void GetDistributionLists_Failure()
        {
            WebCallResult res;
            List<DistributionList> oList;
            res = DistributionList.GetDistributionLists(null,out oList , null);

            Assert.IsFalse(res.Success,"GetDistributionLists failed to catch null ConnectionServer object");
        }

        /// <summary>
        /// Exercise AddDistributionList failure points.
        /// </summary>
        [TestMethod()]
        public void AddDistributionList_Failure()
        {
            WebCallResult res;

            res = DistributionList.AddDistributionList(null, "aaa", "aaa", "123", null);
            Assert.IsFalse(res.Success, "AddDistributionList failed to catch null ConnectionServer object");


            res = DistributionList.AddDistributionList(_connectionServer, "", "", "123", null);
            Assert.IsFalse(res.Success, "AddDistributionList failed to catch empty alias and display name params");
        }

        /// <summary>
        /// Exercise GetDistributionListMember failure points.
        /// </summary>
        [TestMethod()]
        public void GetDistributionListMember_Failure()
        {
            WebCallResult res;

            List<DistributionListMember> oListMember;

            res = DistributionListMember.GetDistributionListMembers(null,"",out oListMember);
            Assert.IsFalse(res.Success,"Fetch of distribution list members should fail with null Connection Server object passed");

            res = DistributionListMember.GetDistributionListMembers(_connectionServer, "", out oListMember);
            Assert.IsFalse(res.Success,"GetDistributionListMember should fail with an invalid DistributionListObjectID passed to it");
        }

        
        /// <summary>
        /// Exercise UpdateDistrubitonList failure points
        /// </summary>
        [TestMethod()]
        public void UpdateDistributionList_Failure()
        {
            WebCallResult res;

            res = DistributionList.UpdateDistributionList(null, "aaa", null);
            Assert.IsFalse(res.Success, "UpdateDistributionList failed to catch null ConnectionServer object");


            res = DistributionList.UpdateDistributionList(_connectionServer, "aaa", null);
            Assert.IsFalse(res.Success, "UpdateDistributionList failed to catch empty property list");
        }


        /// <summary>
        /// Exercise DeleteDistributionList failure points
        /// </summary>
        [TestMethod()]
        public void DeleteDistributionList_Failure()
        {
            WebCallResult res;

            res = DistributionList.DeleteDistributionList(null, "aaa");
            Assert.IsFalse(res.Success, "DeleteDistributionList failed to catch null ConnectionServer object");
        }


        /// <summary>
        /// Exercise GetDistributionList failure points
        /// </summary>
        [TestMethod()]
        public void GetDistributionList_Failure()
        {
            WebCallResult res;
            DistributionList oList;

            res = DistributionList.GetDistributionList(out oList,null);
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch null ConnectionServer object");

            res = DistributionList.GetDistributionList(out oList, _connectionServer,"","");
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch empty alias and ObjectId being passed");

            res = DistributionList.GetDistributionList(out oList, _connectionServer, "","bogus alias" );
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch bogus alias and empty ObjectId being passed");

        }



        /// <summary>
        /// Exercise GetDistributionListVoiceName failure points
        /// </summary>
        [TestMethod()]
        public void GetDistributionListVoiceNameTest_Failure()
        {
            WebCallResult res;

            //use the same string for the alias and display name here
            String strWavName = @"c:\";

            //invalid local WAV file name
            res = DistributionList.GetDistributionListVoiceName(null, "aaa", "");
            Assert.IsFalse(res.Success, "GetDistributionListVoiceName did not fail for null Conneciton server");

            //empty target file path
            res = DistributionList.GetDistributionListVoiceName(_connectionServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "GetDistributionListVoiceName did not fail with invalid target path passed");

            //invalid objectId 
            res = DistributionList.GetDistributionListVoiceName(_connectionServer, "", strWavName);
            Assert.IsFalse(res.Success, "GetDistributionListVoiceName did not fail with invalid ObjectId passed");

        }

        /// <summary>
        /// Exercise SetDistributionListVoiceName failure points
        /// </summary>
        [TestMethod()]
        public void SetDistributionListVoiceNameTest_Failure()
        {
            WebCallResult res;

            //use the same string for the alias and display name here
            String strWavName = @"c:\";

            //invalid Connection server
            res = DistributionList.SetDistributionListVoiceName(null, "", "");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with null Connection server passed.");

            //invalid target path
            res = DistributionList.SetDistributionListVoiceName(_connectionServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with invalid target path");

            //invalid ObjectId
            res = DistributionList.SetDistributionListVoiceName(_connectionServer, strWavName, "");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with invalid obejctID");

        }

        /// <summary>
        /// Add a new list, change it's name, save it and delete it.  This covers a lot of ground but since we're working with a real, live 
        /// Connection server we need to consolidate edits like this into a routine where we're working with a temporary list that we clean
        /// up afterwards.
        /// </summary>
        [TestMethod()]
        public void AddEditDeleteDistributionList_Test()
        {
            WebCallResult res;

            //use the same string for the alias and display name here
            String strListName = "TempList_" + Guid.NewGuid().ToString();

            //create new list with GUID in the name to ensure uniqueness
            DistributionList oList;
            res = DistributionList.AddDistributionList(_connectionServer, strListName, strListName, "", null, out oList);

            Assert.AreEqual(res.Success, true);
            Assert.AreNotEqual(oList, null);

            //call update with no edits - this should fail
            res=oList.Update();
            Assert.IsFalse(res.Success,"Call to update with no pending changes should fail");

            //Edit the list's name
            oList.DisplayName = strListName + "x";
            res = oList.Update();
            Assert.IsTrue(res.Success, "Call to update distribution list failed:"+res.ToString());

            //try to download voice name- this should fail
            res = oList.GetVoiceName(@"c:\temp.wav");
            Assert.IsFalse(res.Success,"Empty voice name fetch should return false for newly created list");

            //upload an invalid wav file
            res = oList.SetVoiceName("wavcopy.exe", true);
            Assert.IsFalse(res.Success, "Updating invalid voice wav file was not caught");

            //upload a voice name to the list
            res = oList.SetVoiceName("Dummy.wav", true);
            Assert.IsTrue(res.Success, "Updating voice name on new distribution list failed: "+res.ToString());

            //Add a user to it.
            UserBase oUser;
            res = UserBase.GetUser(out oUser, _connectionServer, "", "operator");
            Assert.IsTrue(res.Success, "Adding operator to new distribution list failed: "+res.ToString());

            res = oList.AddMemberUser(oUser.ObjectId);
            Assert.IsTrue(res.Success, "Adding operator user to new distribution list failed: "+res.ToString());

            //Add a list to it
            DistributionList oNewList;
            res = DistributionList.GetDistributionList(out oNewList, _connectionServer, "", "allvoicemailusers");
            Assert.IsTrue(res.Success, "Get AllVoiceMail users list failed: "+res.ToString());

            res = oList.AddMemberList(oNewList.ObjectId);
            Assert.IsTrue(res.Success, "Adding AllUsersDistribution list as a member to the new list failed: "+res.ToString());

            //get count - make sure it equals 2
            List<DistributionListMember> oMemberList;
            res = oList.GetMembersList(out oMemberList);
            Assert.IsTrue(res.Success, "Getting membership list from new distribution list failed: "+res.ToString());
            Assert.AreEqual(oMemberList.Count, 2,"Membership list fetch from new distribution list did not return 2 members as it should.");

            //remove both members from it - also exercise the DistributionListMember toString and DumpAll here
            foreach (DistributionListMember oMember in oMemberList)
            {
                Console.WriteLine(oMember.ToString());
                Console.WriteLine(oMember.DumpAllProps());
                res = oList.RemoveMember(oMember.ObjectId);

                Assert.IsTrue(res.Success,"Removal of member from new distribution list failed for:"+oMember.ToString());
            }

            //get count - make sure it equals 0
            res = oList.GetMembersList(out oMemberList);
            Assert.AreEqual(oMemberList.Count, 0,"After removal of members from the new distribution list the count of members reports more than 0.");

            //delete the list
            res = oList.Delete();
            Assert.IsTrue(res.Success, "Removal of new list at end of test failed: "+res.ToString());

        }

        /// <summary>
        /// A test for GetDistributionLists - fetches the first 3 found in the directory and lists them out.  Exercises the ToString and 
        /// DumpAllProps methods as well.
        ///</summary>
        [TestMethod()]
        public void GetDistributionLists_Test()
        {

            ConnectionServer pConnectionServer = _connectionServer;
            List<DistributionList> pDistributionLists = null;

            //limit the fetch to the first 3 lists to be sure this passes even on a default install
            string[] pClauses = { "rowsPerPage=3" };

            WebCallResult res = DistributionList.GetDistributionLists(pConnectionServer, out pDistributionLists, pClauses);
            Assert.IsTrue(res.Success, "Fetching of top three distribution lists failed: "+res.ToString());
            Assert.AreEqual(pDistributionLists.Count, 3,"Fetching of the top three distribution list returned a different number of lists: "+res.ToString());

            //exercise the ToString and DumpAllProperties as part of this test as well
            foreach (DistributionList oList in pDistributionLists)
            {
                Console.WriteLine(oList.ToString());
                Console.WriteLine(oList.DumpAllProps());
            }

        }

    }
}
