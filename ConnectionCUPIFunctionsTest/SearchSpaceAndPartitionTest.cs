﻿using System;
using System.Collections.Generic;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class SearchSpaceAndPartitionTest
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
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

        }

        #endregion


        #region Class Construction Error Checks

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void PartitionClassCreationFailure()
        {
            Partition oTest = new Partition(null);
        }

        /// <summary>
        /// Make sure an Exception is thrown if an invalid ObjectId is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void PartitionClassCreationFailure2()
        {
            Partition oTest = new Partition(_connectionServer,"bogus");
        }

        /// <summary>
        /// Make sure an Exception is thrown if an invalid name is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void PartitionClassCreationFailure3()
        {
            Partition oTest = new Partition(_connectionServer,"","bogus");
        }


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchSpaceClassCreationFailure()
        {
            SearchSpace oTest = new SearchSpace(null);
        }

        /// <summary>
        /// Make sure an Exception is thrown if an invalid objectId is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void SearchSpaceClassCreationFailure2()
        {
            SearchSpace oTest = new SearchSpace(_connectionServer,"bogus");
        }
        /// <summary>
        /// Make sure an Exception is thrown if an invalid name is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void SearchSpaceClassCreationFailure3()
        {
            SearchSpace oTest = new SearchSpace(_connectionServer,"","bogus");
        }

        #endregion


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
            }
            catch (Exception ex)
            {
                Assert.Fail("Creating new partition object with valid ObjectID failed:"+ex);
            }

            //get partition by name
            try
            {
                oNewPartition = new Partition(_connectionServer, "",strName);
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
            }
            catch{}

            //get partition by bogus objectId
            try
            {
                oNewPartition = new Partition(_connectionServer, "bogus");
                Assert.Fail("Creating new partition object with bogus ObjectId did not fail");
            }
            catch { }

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
            }
            catch (Exception ex)
            {
                Assert.Fail("Creating new SearchSpace object with valid ObjectID failed:" + ex);
            }

            //get SearchSpace by name
            try
            {
                oNewSearchSpace = new SearchSpace(_connectionServer, "", strName);
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
            }
            catch { }

            //get SearchSpace by bogus objectId
            try
            {
                oNewSearchSpace = new SearchSpace(_connectionServer, "bogus");
                Assert.Fail("Creating new SearchSpace object with bogus ObjectId did not fail");
            }
            catch { }
        }


        [TestMethod]
        public void SearchSpaceCreationDeletion()
        {
            SearchSpace oSearchSpace;
            string strName = "UnitTest_" + Guid.NewGuid().ToString();
            WebCallResult res = SearchSpace.AddSearchSpace(_connectionServer, out oSearchSpace, strName, "SearchSpace added by Unit Test");
            Assert.IsTrue(res.Success, "Creation of new SearchSpace failed");

            oSearchSpace.Description = "Updated description";
            res = oSearchSpace.Update();
            Assert.IsTrue(res.Success, "Update of SearchSpace description failed:" + res);

            //search space member functions
            List<Partition> oPartitions;
            res = Partition.GetPartitions(_connectionServer, out oPartitions);
            Assert.IsTrue(res.Success, "Fetching of partitions failed:" + res);
            Assert.IsTrue(oPartitions.Count>0,"No partitions returned in search");

            res= oSearchSpace.AddSearchSpaceMember(oPartitions[0].ObjectId,99);
            Assert.IsTrue(res.Success, "Adding partition as search space member failed:" + res);
            Assert.IsTrue(oSearchSpace.GetSearchSpaceMembers().Count==1, "Search space member count not accurate after partition add:" + res);

            res = oSearchSpace.DeleteSearchSpaceMember(oPartitions[0].ObjectId);
            Assert.IsTrue(res.Success, "Removing partition as search space member failed:" + res);

            res = SearchSpace.UpdateSearchSpace(_connectionServer, oSearchSpace.ObjectId, "NewName", "NewDescription");
            Assert.IsTrue(res.Success, "Update of SearchSpace via static method failed:" + res);

            res = oSearchSpace.Delete();
            Assert.IsTrue(res.Success, "Deletion of SearchSpace failed:" + res);

            //static method failures
            //empty name
            res = SearchSpace.AddSearchSpace(_connectionServer, out oSearchSpace, "");
            Assert.IsFalse(res.Success, "Static method for add SearchSpace did not fail with empty name");

            //null ConnectionServer 
            res = SearchSpace.AddSearchSpace(null, out oSearchSpace, "name");
            Assert.IsFalse(res.Success, "Static method for add SearchSpace did not fail with null ConnectionServer");

            //invalid locaiton
            res = SearchSpace.AddSearchSpace(_connectionServer, out oSearchSpace, "name", "description", "boguslocation");
            Assert.IsFalse(res.Success, "Static method for add SearchSpace did not fail with invalid Location");

            res = SearchSpace.DeleteSearchSpace(null, "bogus");
            Assert.IsFalse(res.Success, "Static method for delete SearchSpace did not fail with null Connection");

            res = SearchSpace.DeleteSearchSpace(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static method for delete SearchSpace did not fail with empty SearchSpace ObjectId");

            res = SearchSpace.UpdateSearchSpace(null, "bogus");
            Assert.IsFalse(res.Success, "Static method for update SearchSpace did not fail with null ConnectionServer");

            res = SearchSpace.UpdateSearchSpace(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static method for update SearchSpace did not fail with empty SearchSpace ObjectId");

            res = SearchSpace.AddSearchSpaceMember(null, "blah", "blah", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with null Connection server ");

            res = SearchSpace.AddSearchSpaceMember(_connectionServer, "", "", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with empty search space ID and partition ");

            res = SearchSpace.AddSearchSpaceMember(_connectionServer, "blah", "", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with bogus search space ID");

            res = SearchSpace.AddSearchSpaceMember(_connectionServer, "blah", "blah", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with bogus search space and partition IDs ");

            res = SearchSpace.DeleteSearchSpaceMember(null, "blah", "blah");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with null Connection server ");

            res = SearchSpace.DeleteSearchSpaceMember(_connectionServer, "","");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with blank search space and partition ids");

            res = SearchSpace.DeleteSearchSpaceMember(_connectionServer, "blah", "");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with bogus search space ID");
            
            res = SearchSpace.DeleteSearchSpaceMember(_connectionServer, "blah", "blah");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with bogus search space and partition Id");
        }




        [TestMethod]
        public void PartitionCreationDeletion()
        {
            Partition oPartition;
            string strName = "UnitTest_"+ Guid.NewGuid().ToString();
            WebCallResult res = Partition.AddPartition(_connectionServer, out oPartition, strName,"Partition added by Unit Test");
            Assert.IsTrue(res.Success,"Creation of new partition failed");

            oPartition.Description = "Updated description";
            res = oPartition.Update();
            Assert.IsTrue(res.Success, "Update of partition description failed:" + res);

            res = Partition.UpdatePartition(_connectionServer, oPartition.ObjectId, "NewName", "NewDescription");
            Assert.IsTrue(res.Success, "Update of partition via static method failed:" + res);

            res = oPartition.Delete();
            Assert.IsTrue(res.Success,"Deletion of partition failed:"+res);

            //static method failures
            //empty name
            res = Partition.AddPartition(_connectionServer, out oPartition, "");
            Assert.IsFalse(res.Success,"Static method for add partition did not fail with empty name");

            //null ConnectionServer 
            res = Partition.AddPartition(null, out oPartition, "name");
            Assert.IsFalse(res.Success, "Static method for add partition did not fail with null ConnectionServer");

            //invalid locaiton
            res = Partition.AddPartition(_connectionServer, out oPartition, "name","description", "boguslocation");
            Assert.IsFalse(res.Success, "Static method for add partition did not fail with invalid Location");

            res = Partition.DeletePartition(null, "bogus");
            Assert.IsFalse(res.Success, "Static method for delete partition did not fail with null Connection");

            res = Partition.DeletePartition(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static method for delete partition did not fail with empty partition ObjectId");

            res = Partition.UpdatePartition(null, "bogus");
            Assert.IsFalse(res.Success, "Static method for update partition did not fail with null ConnectionServer");

            res = Partition.UpdatePartition(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static method for update partition did not fail with empty Partition ObjectId");

        }

    }
}