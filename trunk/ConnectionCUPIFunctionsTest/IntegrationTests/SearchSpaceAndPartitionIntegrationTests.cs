using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class SearchSpaceAndPartitionIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static SearchSpace _searchSpace;

        private static Partition _partition;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
            
            string strName = "Temp_" + Guid.NewGuid().ToString();
            WebCallResult res = SearchSpace.AddSearchSpace(_connectionServer, out _searchSpace, strName, "SearchSpace added by Unit Test");
            Assert.IsTrue(res.Success, "Creation of new SearchSpace failed");

            strName = "Temp_" + Guid.NewGuid().ToString();
            res = Partition.AddPartition(_connectionServer, out _partition, strName, "Partition added by Unit Test");
            Assert.IsTrue(res.Success, "Creation of new partition failed");
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            WebCallResult res;

            if (_partition != null)
            {
                res = _partition.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary partition on cleanup.");
            }

            if (_searchSpace != null)
            {
                res = _searchSpace.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary search space on cleanup.");
            }
        }

        #endregion


        #region Class Construction Error Checks

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Partition_Constructor_InvalidObjectId_Failure()
        {
            Partition oTest = new Partition(_connectionServer,"ObjectId");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Partition_Constructor_InvalidDisplayName_Failure()
        {
            Partition oTest = new Partition(_connectionServer,"","bogus display name");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void SearchSpace_Constructor_InvalidObjectID_Failure()
        {
            SearchSpace oTest = new SearchSpace(_connectionServer,"ObjectId");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void SearchSpace_Constructor_InvalidDisplayName_Failure()
        {
            SearchSpace oTest = new SearchSpace(_connectionServer,"","bogus display name");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Search Space Static Call Failure Tests

        [TestMethod]
        public void DeleteSearchSpaceMember_InvalidObjectIds_Failure()
        {
            var res = SearchSpace.DeleteSearchSpaceMember(_connectionServer, "SearchSpaceId", "PartitionId");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with bogus search space and partition Id");
        }

        [TestMethod]
        public void AddSearchSpaceMember_InvalidObjectIds_Failure()
        {
            var res = SearchSpace.AddSearchSpaceMember(_connectionServer, "SearchSpaceId", "PartitionId", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with bogus search space and partition IDs ");
        }

        [TestMethod]
        public void UpdateSearchSpace_InvalidObjectId_Failure()
        {
            var res = SearchSpace.UpdateSearchSpace(_connectionServer, "ObjectId");
            Assert.IsFalse(res.Success, "Static method for update SearchSpace did not fail with empty SearchSpace ObjectId");
        }

        [TestMethod]
        public void AddSearchSpace_InvalidLocationId_Failure()
        {
            SearchSpace oSearchSpace;

            //invalid locaiton
            var res = SearchSpace.AddSearchSpace(_connectionServer, out oSearchSpace, "name", "description", "boguslocation");
            Assert.IsFalse(res.Success, "Static method for add SearchSpace did not fail with invalid Location");
        }

        #endregion


        #region Partition Static Call Failure Tests

        [TestMethod]
        public void DeletePartition_InvalidPartitionObjectId_Failure()
        {
            var res = Partition.DeletePartition(_connectionServer, "PartitionObjectId");
            Assert.IsFalse(res.Success, "Static method for delete partition did not fail with invalid partition ObjectId");
        }

        [TestMethod]
        public void UpdatePartition_Success()
        {
            var res = Partition.UpdatePartition(_connectionServer, _partition.ObjectId, "NewName" + Guid.NewGuid().ToString(), "NewDescription");
            Assert.IsTrue(res.Success, "Update of partition via static method failed:" + res);
        }

        [TestMethod]
        public void UpdatePartition_InvalidObjectId_Failure()
        {
            var res = Partition.UpdatePartition(_connectionServer,"ObjectId");
            Assert.IsFalse(res.Success, "Static method for update partition did not fail with invalid Partition ObjectId");
        }

        [TestMethod]
        public void AddPartition_InvalidLocationId_Failure()
        {
            Partition oPartition;

            var res = Partition.AddPartition(_connectionServer, out oPartition, "name", "description", "boguslocation");
            Assert.IsFalse(res.Success, "Static method for add partition did not fail with invalid Location");
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void PartitionFetchTests()
        {
            List<Partition> oPartitions;
            WebCallResult res = Partition.GetPartitions(null, out oPartitions);
            Assert.IsFalse(res.Success,"Static partitions fetch did not fail with null Connection server");

            res = Partition.GetPartitions(_connectionServer, out oPartitions);
            Assert.IsTrue(res.Success, "Static partitions fetch failed:"+res);

            Assert.IsTrue(oPartitions.Count>0,"No partitions found on target Connection server");

            string strObjectId = "";
            string strName = "";

            foreach (var oPartition in oPartitions)
            {
                Console.WriteLine(oPartition.ToString());
                Console.WriteLine(oPartition.DumpAllProps());
                strObjectId = oPartition.ObjectId;
                strName = oPartition.Name;
            }

            //get partition by ObjectId
            Partition oNewPartition;
            try
            {
                oNewPartition = new Partition(_connectionServer, strObjectId);
                Console.WriteLine(oNewPartition);
            }
            catch (Exception ex)
            {
                Assert.Fail("Creating new partition object with valid ObjectID failed:"+ex);
            }

            //get partition by name
            try
            {
                oNewPartition = new Partition(_connectionServer, "",strName);
                Console.WriteLine(oNewPartition);
            }
            catch (Exception ex)
            {
                Assert.Fail("Creating new partition object with valid Name failed:" + ex);
            }

            //get partition by bogus name
            try
            {
                oNewPartition = new Partition(_connectionServer, "", "bogus");
                Assert.Fail("Creating new partition object with bogus Name did not fail");
                Console.WriteLine(oNewPartition);
            }
            catch (Exception)
            {
                Console.WriteLine("Expected failure on creation");
            }

            //get partition by bogus objectId
            try
            {
                oNewPartition = new Partition(_connectionServer, "bogus");
                Assert.Fail("Creating new partition object with bogus ObjectId did not fail");
                Console.WriteLine(oNewPartition);
            }
            catch (Exception)
            {
                Console.WriteLine("Expected failure on creation");
            }

            res = Partition.GetPartitions(_connectionServer, out oPartitions,1,2,"query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching partitions with invalid query should not fail:" + res);
            Assert.IsTrue(oPartitions.Count == 0, "Invalid query string should return an empty partition list:" + oPartitions.Count);

        }


        [TestMethod]
        public void SearchSpaceFetchTests()
        {
            List<SearchSpace> oSearchSpaces;
            WebCallResult res = SearchSpace.GetSearchSpaces(null, out oSearchSpaces);
            Assert.IsFalse(res.Success, "Static SearchSpaces fetch did not fail with null Connection server");

            res = SearchSpace.GetSearchSpaces(_connectionServer, out oSearchSpaces);
            Assert.IsTrue(res.Success, "Static SearchSpaces fetch failed:" + res);

            Assert.IsTrue(oSearchSpaces.Count > 0, "No SearchSpaces found on target Connection server");

            string strObjectId = "";
            string strName = "";

            foreach (var oSearchSpace in oSearchSpaces)
            {
                Console.WriteLine(oSearchSpace.ToString());
                Console.WriteLine(oSearchSpace.DumpAllProps());
                Console.WriteLine(oSearchSpace.GetSearchSpaceMembers().Count);
                Console.WriteLine(oSearchSpace.GetSearchSpaceMembers(true).Count);
                strObjectId = oSearchSpace.ObjectId;
                strName = oSearchSpace.Name;

            }

            //get SearchSpace by ObjectId
            SearchSpace oNewSearchSpace;
            try
            {
                oNewSearchSpace = new SearchSpace(_connectionServer, strObjectId);
                Console.WriteLine(oNewSearchSpace);
            }
            catch (Exception ex)
            {
                Assert.Fail("Creating new SearchSpace object with valid ObjectID failed:" + ex);
            }

            //get SearchSpace by name
            try
            {
                oNewSearchSpace = new SearchSpace(_connectionServer, "", strName);
                Console.WriteLine(oNewSearchSpace);
            }
            catch (Exception ex)
            {
                Assert.Fail("Creating new SearchSpace object with valid Name failed:" + ex);
            }

            //get SearchSpace by bogus name
            try
            {
                oNewSearchSpace = new SearchSpace(_connectionServer, "", "bogus");
                Assert.Fail("Creating new SearchSpace object with bogus Name did not fail");
                Console.WriteLine(oNewSearchSpace);
            }
            catch (Exception)
            {
                Console.WriteLine("Expected failure on creation failure");
            }

            //get SearchSpace by bogus objectId
            try
            {
                oNewSearchSpace = new SearchSpace(_connectionServer, "bogus");
                Assert.Fail("Creating new SearchSpace object with bogus ObjectId did not fail");
                Console.WriteLine(oNewSearchSpace);
            }
            catch (Exception)
            {
                Console.WriteLine("Expected error on creation failure");
            }

            res = SearchSpace.GetSearchSpaces(_connectionServer, out oSearchSpaces,1,2,"query=(ObjectId is Bogus)");
            Assert.IsTrue(res.Success, "fetching search spaces with invalid query should not fail:" + res);
            Assert.IsTrue(oSearchSpaces.Count == 0, "Invalid query string should return an empty search space list:" + oSearchSpaces.Count);
        }


       

        [TestMethod]
        public void SearchSpaceUpdateTests()
        {

            var res = _searchSpace.Update(_searchSpace.Name, _searchSpace.Description + "new");
            Assert.IsTrue(res.Success, "Update of SearchSpace description failed:" + res);

            //search space member functions
            List<Partition> oPartitions;
            res = Partition.GetPartitions(_connectionServer, out oPartitions);
            Assert.IsTrue(res.Success, "Fetching of partitions failed:" + res);
            Assert.IsTrue(oPartitions.Count>0,"No partitions returned in search");

            res = _searchSpace.AddSearchSpaceMember(oPartitions[0].ObjectId, 99);
            Assert.IsTrue(res.Success, "Adding partition as search space member failed:" + res);
            Assert.IsTrue(_searchSpace.GetSearchSpaceMembers().Count == 1, "Search space member count not accurate after partition add:" + res);

            res = _searchSpace.DeleteSearchSpaceMember(oPartitions[0].ObjectId);
            Assert.IsTrue(res.Success, "Removing partition as search space member failed:" + res);

            res = SearchSpace.UpdateSearchSpace(_connectionServer, _searchSpace.ObjectId, "NewName"+Guid.NewGuid(), "NewDescription");
            Assert.IsTrue(res.Success, "Update of SearchSpace via static method failed:" + res);

        }

        #endregion
    }
}
