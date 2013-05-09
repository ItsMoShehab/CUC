﻿using System.IO;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
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
    [TestClass]
    public class DistributionListTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static DistributionList _tempList;

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
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start DistributionListTest test:" + ex.Message);
            }

            //create new list with GUID in the name to ensure uniqueness
            String strName = "TempList_" + Guid.NewGuid().ToString().Replace("-", "");

            WebCallResult res = DistributionList.AddDistributionList(_connectionServer, strName, strName,"", null, out _tempList);
            Assert.IsTrue(res.Success, "Failed creating temporary distribution list:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempList != null)
            {
                WebCallResult res = _tempList.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary distribution list on cleanup.");
            }
        }

        #endregion


        #region Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreation_Failure()
        {

            DistributionList oTestList = new DistributionList(null);
            Console.WriteLine(oTestList);
        }

        #endregion


        #region Static Call Failures

        /// <summary>
        /// Test common failure scenarios distribution list functions
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionListTest()
        {
            //create new list with GUID in the name to ensure uniqueness
            DistributionList oList;

            //null connection server object
            WebCallResult res = DistributionList.GetDistributionList(out oList, null, "", "allvoicemailusers");
            Assert.IsFalse(res.Success, "Null Connection server on GetDistributionList did not fail.");

            //invalid alias/objectId pair
            res = DistributionList.GetDistributionList(out oList, _connectionServer);
            Assert.IsFalse(res.Success, "Blank alias/objectID params on GetDistributionList did not fail");
        }

        /// <summary>
        /// exercise GetDistributionLists failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionLists()
        {
            List<DistributionList> oList;
            WebCallResult res = DistributionList.GetDistributionLists(null, out oList, null);

            Assert.IsFalse(res.Success, "GetDistributionLists failed to catch null ConnectionServer object");
        }

        /// <summary>
        /// Exercise AddDistributionList failure points.
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_AddDistributionList()
        {
            WebCallResult res = DistributionList.AddDistributionList(null, "aaa", "aaa", "123", null);
            Assert.IsFalse(res.Success, "AddDistributionList failed to catch null ConnectionServer object");


            res = DistributionList.AddDistributionList(_connectionServer, "", "", "123", null);
            Assert.IsFalse(res.Success, "AddDistributionList failed to catch empty alias and display name params");
        }

        /// <summary>
        /// Exercise GetDistributionListMember failure points.
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionListMember()
        {
            List<DistributionListMember> oListMember;

            WebCallResult res = DistributionListMember.GetDistributionListMembers(null, "", out oListMember);
            Assert.IsFalse(res.Success, "Fetch of distribution list members should fail with null Connection Server object passed");

            res = DistributionListMember.GetDistributionListMembers(_connectionServer, "", out oListMember);
            Assert.IsFalse(res.Success, "GetDistributionListMember should fail with an invalid DistributionListObjectID passed to it");
        }


        /// <summary>
        /// Exercise UpdateDistrubitonList failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_UpdateDistributionList()
        {
            WebCallResult res = DistributionList.UpdateDistributionList(null, "aaa", null);
            Assert.IsFalse(res.Success, "UpdateDistributionList failed to catch null ConnectionServer object");


            res = DistributionList.UpdateDistributionList(_connectionServer, "aaa", null);
            Assert.IsFalse(res.Success, "UpdateDistributionList failed to catch empty property list");
        }


        /// <summary>
        /// Exercise DeleteDistributionList failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_DeleteDistributionList()
        {
            WebCallResult res = DistributionList.DeleteDistributionList(null, "aaa");
            Assert.IsFalse(res.Success, "DeleteDistributionList failed to catch null ConnectionServer object");
        }


        /// <summary>
        /// Exercise GetDistributionList failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionList()
        {
            DistributionList oList;

            WebCallResult res = DistributionList.GetDistributionList(out oList, null);
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch null ConnectionServer object");

            res = DistributionList.GetDistributionList(out oList, _connectionServer);
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch empty alias and ObjectId being passed");

            res = DistributionList.GetDistributionList(out oList, _connectionServer, "", "bogus alias");
            Assert.IsFalse(res.Success, "GetDistributionList failed to catch bogus alias and empty ObjectId being passed");

        }



        /// <summary>
        /// Exercise GetDistributionListVoiceName failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDistributionListVoiceName()
        {
            //use the same string for the alias and display name here
            const string strWavName = @"c:\";

            //invalid local WAV file name
            WebCallResult res = DistributionList.GetDistributionListVoiceName(null, "aaa", "");
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
        [TestMethod]
        public void StaticCallFailures_SetDistributionListVoiceName()
        {
            //use the same string for the alias and display name here
            const string strWavName = @"c:\";

            //invalid Connection server
            WebCallResult res = DistributionList.SetDistributionListVoiceName(null, "", "");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with null Connection server passed.");

            //invalid target path
            res = DistributionList.SetDistributionListVoiceName(_connectionServer, "aaa", "aaa");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with invalid target path");

            //invalid ObjectId
            res = DistributionList.SetDistributionListVoiceName(_connectionServer, strWavName, "");
            Assert.IsFalse(res.Success, "SetDistributionListVoiceName did not fail with invalid obejctID");

        }
        
        #endregion


        #region Live Tests

        /// <summary>
        ///A test for GetDistributionListVoiceName - this exercises the GetDistribitonList as well since it fetches the 
        /// AllVoiceMailUsers list which should always have a voice name.
        ///</summary>
        [TestMethod]
        public void GetDistributionListVoiceName_Test()
        {
            //use the same string for the alias and display name here
            String strWavName = "TempWAV_" + Guid.NewGuid().ToString();

            //create new list with GUID in the name to ensure uniqueness
            DistributionList oList;

            WebCallResult res = DistributionList.GetDistributionList(out oList, _connectionServer, "", "allvoicemailusers");

            Assert.IsTrue(res.Success, "Fetch of AllVoiceMailUsers list failed:" + res.ToString());
            Assert.AreNotEqual(oList, null, "Fetch of AllVoiceMailUsers list failed returning an empty list");

            //first, get from static method without voice name
            res = DistributionList.GetDistributionListVoiceName(_connectionServer, strWavName, oList.ObjectId);
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
        /// Add a new list, change it's name, save it and delete it.  This covers a lot of ground but since we're working with a real, live 
        /// Connection server we need to consolidate edits like this into a routine where we're working with a temporary list that we clean
        /// up afterwards.
        /// </summary>
        [TestMethod]
        public void DistributionList_TopLevel()
        {
            //call update with no edits - this should fail
            WebCallResult res = _tempList.Update();
            Assert.IsFalse(res.Success,"Call to update with no pending changes should fail");

            //Edit the list's name
            _tempList.DisplayName = _tempList.DisplayName + "x";
            res = _tempList.Update();
            Assert.IsTrue(res.Success, "Call to update distribution list failed:"+res.ToString());

            _tempList.PartitionObjectId = "bogus";
            res = _tempList.Update();
            Assert.IsFalse(res.Success,"Setting invalid partition value did not fail:"+res);

        }


        [TestMethod]
        public void DistributionList_VoiceName()
        {
            //try to download voice name- this should fail
            WebCallResult res = _tempList.GetVoiceName(@"c:\temp.wav");
            Assert.IsFalse(res.Success, "Empty voice name fetch should return false for newly created list");

            //upload an invalid wav file
            res = _tempList.SetVoiceName("wavcopy.exe", true);
            Assert.IsFalse(res.Success, "Updating invalid voice wav file was not caught");

            //upload a voice name to the list
            res = _tempList.SetVoiceName("Dummy.wav", true);
            Assert.IsTrue(res.Success, "Updating voice name on new distribution list failed: " + res.ToString());

            //upload real name
            res = _tempList.SetVoiceName("temp.wav");
            Assert.IsTrue(res.Success,"Failed uploading voice name:"+res);

            string strFileName = Guid.NewGuid().ToString() + ".wav";
            res = _tempList.GetVoiceName(strFileName);
            Assert.IsTrue(res.Success,"Failed to donwload voice name just uploaded:"+res);
            Assert.IsTrue(File.Exists(strFileName),"Voice name just downloaded does not exist on hard drive:"+strFileName);
            
            try
            {
                File.Delete(strFileName);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete temporary file:"+ex);
            }

        }

        [TestMethod]
        public void DistributionList_MembershipEdit()
        {
            //Add a user to it.
            UserBase oUser;
            WebCallResult res = UserBase.GetUser(out oUser, _connectionServer, "", "operator");
            Assert.IsTrue(res.Success, "Getting operator for new distribution list failed: " + res.ToString());

            res = _tempList.AddMemberUser(oUser.ObjectId);
            Assert.IsTrue(res.Success, "Adding operator user to new distribution list failed: " + res.ToString());

            //Add a list to it
            DistributionList oNewList;
            res = DistributionList.GetDistributionList(out oNewList, _connectionServer, "", "allvoicemailusers");
            Assert.IsTrue(res.Success, "Get AllVoiceMail users list failed: " + res.ToString());

            res = _tempList.AddMemberList(oNewList.ObjectId);
            Assert.IsTrue(res.Success, "Adding AllUsersDistribution list as a member to the new list failed: " + res.ToString());

            //get count - make sure it equals 2
            List<DistributionListMember> oMemberList;
            res = _tempList.GetMembersList(out oMemberList);
            Assert.IsTrue(res.Success, "Getting membership list from new distribution list failed: " + res.ToString());
            Assert.AreEqual(oMemberList.Count, 2, "Membership list fetch from new distribution list did not return 2 members as it should.");

            //remove both members from it - also exercise the DistributionListMember toString and DumpAll here
            foreach (DistributionListMember oMember in oMemberList)
            {
                Console.WriteLine(oMember.ToString());
                Console.WriteLine(oMember.DumpAllProps());
                res = _tempList.RemoveMember(oMember.ObjectId);

                Assert.IsTrue(res.Success, "Removal of member from new distribution list failed for:" + oMember);
            }

            //get count - make sure it equals 0
            res = _tempList.GetMembersList(out oMemberList);
            Assert.IsTrue(res.Success, "Failed getting members list:" + res);
            Assert.AreEqual(oMemberList.Count, 0, "After removal of members from the new distribution list the count of members reports more than 0.");


        }

        /// <summary>
        /// A test for GetDistributionLists - fetches the first 3 found in the directory and lists them out.  Exercises the ToString and 
        /// DumpAllProps methods as well.
        ///</summary>
        [TestMethod]
        public void DistributionList_GetDistributionLists()
        {

            ConnectionServer pConnectionServer = _connectionServer;
            List<DistributionList> pDistributionLists;

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

        #endregion
    }
}