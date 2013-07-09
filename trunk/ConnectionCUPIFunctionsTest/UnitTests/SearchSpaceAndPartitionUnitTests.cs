using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class SearchSpaceAndPartitionUnitTests : BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Class Construction Error Checks

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Partition_Constructor_NullConnectionServer_Failure()
        {
            Partition oTest = new Partition(null);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchSpace_Constructor_NullConnectionServer_Failure()
        {
            SearchSpace oTest = new SearchSpace(null);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Search Space Static Call Failure Tests

        [TestMethod]
        public void DeleteSearchSpaceMember_NullConnectionServer_Failure()
        {
            var res = SearchSpace.DeleteSearchSpaceMember(null, "blah", "blah");
            Assert.IsFalse(res.Success,"Static method for delete searchspace member did not fail with null Connection server ");
        }

        [TestMethod]
        public void DeleteSearchSpaceMember_EmptySearchSpaceAndPartition_Failure()
        {
            var res = SearchSpace.DeleteSearchSpaceMember(_mockServer, "", "");
            Assert.IsFalse(res.Success, "Static method for delete searchspace member did not fail with blank search space and partition ids");
        }


        [TestMethod]
        public void AddSearchSpaceMember_NullConnectionServer_Failure()
        {
            var res = SearchSpace.AddSearchSpaceMember(null, "blah", "blah", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with null Connection server ");

         }

        [TestMethod]
        public void AddSearchSpaceMember_EmptySearchSpaceAndPartition_Failure()
        {
            var res = SearchSpace.AddSearchSpaceMember(_mockServer, "", "", 1);
            Assert.IsFalse(res.Success, "Static method for add searchspace member did not fail with empty search space ID and partition ");
        }

        [TestMethod]
        public void DeleteSearchSpace_NullConnectionServer_Failure()
        {
            var res = SearchSpace.DeleteSearchSpace(null, "ObjectId");
            Assert.IsFalse(res.Success, "Static method for delete SearchSpace did not fail with null Connection");

            }

        [TestMethod]
        public void DeleteSearchSpace_EmptyObjectId_Failure()
        {
            var res = SearchSpace.DeleteSearchSpace(_mockServer, "");
            Assert.IsFalse(res.Success, "Static method for delete SearchSpace did not fail with empty SearchSpace ObjectId");

            }

        [TestMethod]
        public void UpdateSearchSpace_NullConnectionServer_Failure()
        {
            var res = SearchSpace.UpdateSearchSpace(null, "ObjectId");
            Assert.IsFalse(res.Success, "Static method for update SearchSpace did not fail with null ConnectionServer");

            }

        [TestMethod]
        public void UpdateSearchSpace_EmptyObjectId_Failure()
        {
            var res = SearchSpace.UpdateSearchSpace(_mockServer, "");
            Assert.IsFalse(res.Success, "Static method for update SearchSpace did not fail with empty SearchSpace ObjectId");
        }

        [TestMethod]
        public void AddSearchSpace_EmptyName_Failure()
        {
            SearchSpace oSearchSpace;
            var res = SearchSpace.AddSearchSpace(_mockServer, out oSearchSpace, "");
            Assert.IsFalse(res.Success, "Static method for add SearchSpace did not fail with empty name");

            }

        [TestMethod]
        public void AddSearchSpace_NullConnectionServer_Failure()
        {
            SearchSpace oSearchSpace; 
            var res = SearchSpace.AddSearchSpace(null, out oSearchSpace, "name");
            Assert.IsFalse(res.Success, "Static method for add SearchSpace did not fail with null ConnectionServer");
        }

        #endregion


        #region Partition Static Call Failure Tests

        [TestMethod]
        public void DeletePartition_NullConnectionServer_Failure()
        {
            var res = Partition.DeletePartition(null, "bogus");
            Assert.IsFalse(res.Success, "Static method for delete partition did not fail with null Connection");
        }

        [TestMethod]
        public void DeletePartition_EmptyObjectId_Failure()
        {
            var res = Partition.DeletePartition(_mockServer, "");
            Assert.IsFalse(res.Success, "Static method for delete partition did not fail with empty partition ObjectId");
        }


        [TestMethod]
        public void UpdatePartition_NullConnectionServer_Failure()
        {
            var res = Partition.UpdatePartition(null, "ObjectId");
            Assert.IsFalse(res.Success, "Static method for update partition did not fail with null ConnectionServer");
        }


        [TestMethod]
        public void UpdatePartition_EmptyObjectId_Failure()
        {
            var res = Partition.UpdatePartition(_mockServer, "");
            Assert.IsFalse(res.Success, "Static method for update partition did not fail with empty Partition ObjectId");
        }

        [TestMethod]
        public void AddPartition_EmptyName_Failure()
        {
            Partition oPartition;
            var res = Partition.AddPartition(_mockServer, out oPartition, "");
            Assert.IsFalse(res.Success, "Static method for add partition did not fail with empty name");
        }

        [TestMethod]
        public void AddPartition_NullConnectionServer_Failure()
        {
            Partition oPartition;
            var res = Partition.AddPartition(null, out oPartition, "name");
            Assert.IsFalse(res.Success, "Static method for add partition did not fail with null ConnectionServer");
        }

        #endregion

    }
}
