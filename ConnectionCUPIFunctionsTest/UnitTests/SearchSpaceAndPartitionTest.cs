using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class SearchSpaceAndPartitionTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        private static SearchSpace _searchSpace;

        private static Partition _partition;

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
                throw new Exception("Unable to attach to Connection server to start SearchSpaceAndPartition test:" + ex.Message);
            }

            
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

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PartitionClassCreationFailure()
        {
            Partition oTest = new Partition(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid ObjectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void PartitionClassCreationFailure2()
        {
            Partition oTest = new Partition(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid name is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void PartitionClassCreationFailure3()
        {
            Partition oTest = new Partition(_connectionServer,"","bogus");
            Console.WriteLine(oTest);
        }


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchSpaceClassCreationFailure()
        {
            SearchSpace oTest = new SearchSpace(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void SearchSpaceClassCreationFailure2()
        {
            SearchSpace oTest = new SearchSpace(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }
        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid name is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void SearchSpaceClassCreationFailure3()
        {
            SearchSpace oTest = new SearchSpace(_connectionServer,"","bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Search Space Static Call Failure Tests

        [TestMethod]
        public void StaticCallFailure_DeleteSearchSpaceMember()
        {
            var res = SearchSpace.DeleteSearchSpaceMember(null, "blah", "blah");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with null Connection server ");

            res = SearchSpace.DeleteSearchSpaceMember(_connectionServer, "", "");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with blank search space and partition ids");

            res = SearchSpace.DeleteSearchSpaceMember(_connectionServer, "blah", "");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with bogus search space ID");

            res = SearchSpace.DeleteSearchSpaceMember(_connectionServer, "blah", "blah");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with bogus search space and partition Id");
        }

        [TestMethod]
        public void StaticCallFailure_AddSearchSpaceMember()
        {
            var res = SearchSpace.AddSearchSpaceMember(null, "blah", "blah", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with null Connection server ");

            res = SearchSpace.AddSearchSpaceMember(_connectionServer, "", "", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with empty search space ID and partition ");

            res = SearchSpace.AddSearchSpaceMember(_connectionServer, "blah", "", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with bogus search space ID");

            res = SearchSpace.AddSearchSpaceMember(_connectionServer, "blah", "blah", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with bogus search space and partition IDs ");
        }

        [TestMethod]
        public void StaticCallFailure_DeleteSearchSpace()
        {
            var res = SearchSpace.DeleteSearchSpace(null, "bogus");
            Assert.IsFalse(res.Success, "Static method for delete SearchSpace did not fail with null Connection");

            res = SearchSpace.DeleteSearchSpace(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static method for delete SearchSpace did not fail with empty SearchSpace ObjectId");

            res = SearchSpace.UpdateSearchSpace(null, "bogus");
            Assert.IsFalse(res.Success, "Static method for update SearchSpace did not fail with null ConnectionServer");

            res = SearchSpace.UpdateSearchSpace(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static method for update SearchSpace did not fail with empty SearchSpace ObjectId");
        }

        [TestMethod]
        public void StaticCallFailure_AddSearchSpace()
        {
            //empty name
            SearchSpace oSearchSpace;
            var res = SearchSpace.AddSearchSpace(_connectionServer, out oSearchSpace, "");
            Assert.IsFalse(res.Success, "Static method for add SearchSpace did not fail with empty name");

            //null ConnectionServer 
            res = SearchSpace.AddSearchSpace(null, out oSearchSpace, "name");
            Assert.IsFalse(res.Success, "Static method for add SearchSpace did not fail with null ConnectionServer");

            //invalid locaiton
            res = SearchSpace.AddSearchSpace(_connectionServer, out oSearchSpace, "name", "description", "boguslocation");
            Assert.IsFalse(res.Success, "Static method for add SearchSpace did not fail with invalid Location");
        }

        #endregion


        #region Partition Static Call Failure Tests

        [TestMethod]
        public void StaticCallFailure_DeletePartition()
        {
            var res = Partition.DeletePartition(null, "bogus");
            Assert.IsFalse(res.Success, "Static method for delete partition did not fail with null Connection");

            res = Partition.DeletePartition(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static method for delete partition did not fail with empty partition ObjectId");
        }

        [TestMethod]
        public void StaticCallFailure_UpdatePartition()
        {
            var res = Partition.UpdatePartition(_connectionServer, _partition.ObjectId, "NewName" + Guid.NewGuid().ToString(), "NewDescription");
            Assert.IsTrue(res.Success, "Update of partition via static method failed:" + res);

            res = Partition.UpdatePartition(null, "bogus");
            Assert.IsFalse(res.Success, "Static method for update partition did not fail with null ConnectionServer");

            res = Partition.UpdatePartition(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static method for update partition did not fail with empty Partition ObjectId");
        }

        [TestMethod]
        public void StaticCallFailure_AddPartition()
        {
            Partition oPartition;
            //empty name
            var res = Partition.AddPartition(_connectionServer, out oPartition, "");
            Assert.IsFalse(res.Success, "Static method for add partition did not fail with empty name");

            //null ConnectionServer 
            res = Partition.AddPartition(null, out oPartition, "name");
            Assert.IsFalse(res.Success, "Static method for add partition did not fail with null ConnectionServer");

            //invalid locaiton
            res = Partition.AddPartition(_connectionServer, out oPartition, "name", "description", "boguslocation");
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
