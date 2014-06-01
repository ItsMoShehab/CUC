using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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


        #region Class Construction Tests

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

        [TestMethod]
        public void Partition_Constructor_EmptyOBjectIdAndName_Success()
        {
            Reset();
            Partition oTest = new Partition(_mockServer);
            Console.WriteLine(oTest.ToString());
            Console.WriteLine(oTest.DumpAllProps());
            Console.WriteLine(oTest.SelectionDisplayString);
            Console.WriteLine(oTest.UniqueIdentifier);
        }

        [TestMethod]
        public void Partition_Constructor_ObjectId_Success()
        {
            Reset();
            Partition oTest = new Partition(_mockServer,"ObjectId");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Partition_Constructor_DisplayNameNotFound_Success()
        {
            Reset();
            Partition oTest = new Partition(_mockServer, "","Name");
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

        [TestMethod]
        public void SearchSpace_Constructor_EmptyObjectIdAndName_Success()
        {
            Reset();
            SearchSpace oTest = new SearchSpace(_mockServer);
            Console.WriteLine(oTest.ToString());
            Console.WriteLine(oTest.DumpAllProps());
            Console.WriteLine(oTest.UniqueIdentifier);
            Console.WriteLine(oTest.SelectionDisplayString);
        }

        [TestMethod]
        public void SearchSpace_Constructor_Default_Success()
        {
            SearchSpace oTest = new SearchSpace();
            Console.WriteLine(oTest);
        }

        [TestMethod]
        public void SearchSpace_Constructor_ObjectId_Success()
        {
            Reset();
            SearchSpace oTest = new SearchSpace(_mockServer,"ObjectId");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void SearchSpace_Constructor_DisplayNameNotFound_Success()
        {
            Reset();
            SearchSpace oTest = new SearchSpace(_mockServer, "","Name");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Property Tests

        [TestMethod]
        public void GetSearchSpaceMembers_EmptyResponse_EmptyListReturn()
        {
            Reset();
            SearchSpace oSpace = new SearchSpace(_mockServer, "ObjectId");
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                      It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                      {
                                          Success = true,
                                          ErrorText = ""
                                      });
            var oList = oSpace.GetSearchSpaceMembers(true);
            BaseUnitTests.ClassInitialize(null);
            Assert.IsTrue(oList.Count==0, "GetSearchSpaceMembers called with empty response should return an empty list");
        }

        #endregion


        #region Search Space Static Call Failure Tests

        [TestMethod]
        public void GetSearchSpaces_NullConnectionServer_Failure()
        {
            List<SearchSpace> oSpaces;
            var res = SearchSpace.GetSearchSpaces(null,out oSpaces,1,10,"params");
            Assert.IsFalse(res.Success, "Calling GetSearchSpaces member did not fail with null Connection server ");
        }

        [TestMethod]
        public void DeleteSearchSpaceMember_NullConnectionServer_Failure()
        {
            var res = SearchSpace.DeleteSearchSpaceMember(null, "SearchSpaceObjectId", "partitionObjectID");
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
            var res = SearchSpace.AddSearchSpaceMember(null, "SearchSpaceObjectId", "partitionObjectID", 1);
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


        #region Search Space Harness Tests

        [TestMethod]
        public void GetSearchSpaces_EmptyResult_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<SearchSpace> oSpaces;
            var res = SearchSpace.GetSearchSpaces(_mockServer, out oSpaces, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetSearchSpaces with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetSearchSpaces_GarbageResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<SearchSpace> oSpaces;
            var res = SearchSpace.GetSearchSpaces(_mockServer, out oSpaces, 1, 5, "");
            Assert.IsFalse(res.Success, "Calling GetSearchSpaces with garbage results should fail");
            Assert.IsTrue(oSpaces.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetSearchSpaces_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<SearchSpace> oSpaces;
            var res = SearchSpace.GetSearchSpaces(_mockServer, out oSpaces, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetSearchSpaces with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetSearchSpaces_ZeroCount_Success()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<SearchSpace> oSpaces;
            var res = SearchSpace.GetSearchSpaces(_mockServer, out oSpaces, 1, 5, null);
            Assert.IsTrue(res.Success, "Calling GetSearchSpaces with ZeroCount failed:" + res);
        }

        [TestMethod]
        public void AddSearchSpace_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.POST, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            SearchSpace oSpace;
            var res = SearchSpace.AddSearchSpace(_mockServer, out oSpace, "Name","Description","LocaitonObjectId");
            Assert.IsFalse(res.Success, "Calling AddSearchSpace with ErrorResponse did not fail");
        }


        [TestMethod]
        public void DeleteSearchSpace_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.DELETE, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = SearchSpace.DeleteSearchSpace(_mockServer, "ObjectId");
            Assert.IsFalse(res.Success, "Calling DeleteSearchSpace with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdateSearchSpace_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.PUT, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = SearchSpace.UpdateSearchSpace(_mockServer, "ObjectId","New Name","New Description");
            Assert.IsFalse(res.Success, "Calling UpdateSearchSpace with ErrorResponse did not fail");
        }

        [TestMethod]
        public void AddSearchSpaceMember_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.POST, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = SearchSpace.AddSearchSpaceMember(_mockServer, "SSObjectId", "PartitionObjectID", 1);
            Assert.IsFalse(res.Success, "Calling AddSearchSpaceMember with ErrorResponse did not fail");
        }

        [TestMethod]
        public void DeleteSearchSpaceMember_ErrorResponse_Failure()
        {
            Reset();
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.DELETE, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = SearchSpace.DeleteSearchSpaceMember(_mockServer, "SSObjectId", "PartitionObjectID");
            Assert.IsFalse(res.Success, "Calling DeleteSearchSpaceMember with ErrorResponse did not fail");
        }

        [TestMethod]
        public void RefetchSearchSpaceData_EmptyClassData_Failure()
        {
            Reset();
            SearchSpace oSpace = new SearchSpace(_mockServer);
            var res = oSpace.RefetchSearchSpaceData();
            Assert.IsFalse(res.Success,"Calling RefetchSearchSpaceData from empty class instance should fail");
        }

        [TestMethod]
        public void Delete_EmptyClassData_Failure()
        {
            Reset();
            SearchSpace oSpace = new SearchSpace(_mockServer);
            var res = oSpace.Delete();
            Assert.IsFalse(res.Success, "Calling Delete from empty class instance should fail");
        }

        [TestMethod]
        public void Update_EmptyClassData_Failure()
        {
            Reset();
            SearchSpace oSpace = new SearchSpace(_mockServer);
            var res = oSpace.Update("New Name","New Description");
            Assert.IsFalse(res.Success, "Calling Update from empty class instance should fail");
        }

        [TestMethod]
        public void AddSearchSpaceMember_EmptyClassData_Failure()
        {
            SearchSpace oSpace = new SearchSpace(_mockServer);
            var res = oSpace.AddSearchSpaceMember("PartitionObjectId", 1);
            Assert.IsFalse(res.Success, "Calling AddSearchSpaceMember from empty class instance should fail");
        }

        [TestMethod]
        public void DeleteSearchSpaceMember_EmptyClassData_Failure()
        {
            SearchSpace oSpace = new SearchSpace(_mockServer);
            var res = oSpace.DeleteSearchSpaceMember("PartitionObjectId");
            Assert.IsFalse(res.Success, "Calling DeleteSearchSpaceMember from empty class instance should fail");
        }
        #endregion


        #region Partition Harness Tests

        [TestMethod]
        public void GetPartitions_EmptyResult_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<Partition> oPartitions;
            var res = Partition.GetPartitions(_mockServer, out oPartitions, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetPartitions with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetPartitions_GarbageResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<Partition> oPartitions;
            var res = Partition.GetPartitions(_mockServer, out oPartitions, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetPartitions with garbage results should fail");
            Assert.IsTrue(oPartitions.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetPartitions_ErrorResponse_Failure()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<Partition> oPartitions;
            var res = Partition.GetPartitions(_mockServer, out oPartitions, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetPartitions with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetPartitions_ZeroCount_Success()
        {
            Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<Partition> oPartitions;
            var res = Partition.GetPartitions(_mockServer, out oPartitions, 1, 5, null);
            Assert.IsTrue(res.Success, "Calling GetPartitions with ZeroCount failed:" + res);
        }

        #endregion
    }
}
