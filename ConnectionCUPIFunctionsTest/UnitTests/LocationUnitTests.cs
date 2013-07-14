using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class LocationUnitTests : BaseUnitTests
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


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            Location oTestLocation = new Location(null);
            Console.WriteLine(oTestLocation);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_DisplayName_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                    });
            Location oTestLocation = new Location(_mockServer,"","Display Name");
            Console.WriteLine(oTestLocation);
        }

        [TestMethod]
        public void Constructor_EmptyObjectIdAndName_Success()
        {
            Location oTestLocation = new Location(_mockServer);
            Console.WriteLine(oTestLocation.SelectionDisplayString);
            Console.WriteLine(oTestLocation.UniqueIdentifier);
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            Location oTestLocation = new Location();
            Console.WriteLine(oTestLocation.ToString());
            Console.WriteLine(oTestLocation.DumpAllProps());
        }


        #endregion


        #region Static Call Tests 

        [TestMethod]
        public void GetLocations_NullConnectionServer_Failure()
        {
            List<Location> oLocations;
            WebCallResult res = Location.GetLocations(null, out oLocations,1,20,null);
            Assert.IsFalse(res.Success, "Call to GetLocations did not fail with null ConnectionServer");
        }

        [TestMethod]
        public void GetLocation_NullConnectionServer_Failure()
        {
            Location oNewLocation;

            var res = Location.GetLocation(out oNewLocation, null, "", "displayname");
            Assert.IsFalse(res.Success, "Call to GetLocation did not fail with null ConnectionServer");

        }

        [TestMethod]
        public void GetLocation_EmptyObjectId_Failure()
         {
             Location oNewLocation;

            var res = Location.GetLocation(out oNewLocation, _mockServer, "");
             Assert.IsFalse(res.Success, "Call to GetLocation did not fail with empty name and objectId parameters");
         }


        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetLocations_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<Location> oLocations;
            var res = Location.GetLocations(_mockServer, out oLocations, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetLocations with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetLocations_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<Location> oLocations;
            var res = Location.GetLocations(_mockServer, out oLocations, 1, 5, "");
            Assert.IsFalse(res.Success, "Calling GetLocations with garbage results should fail");
            Assert.IsTrue(oLocations.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetLocations_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<Location> oLocations;
            var res = Location.GetLocations(_mockServer, out oLocations, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetLocations with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetLocations_ZeroCount_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<Location> oLocations;
            var res = Location.GetLocations(_mockServer, out oLocations, 1, 5, null);
            Assert.IsTrue(res.Success, "Calling GetLocations with ZeroCount failed:" + res);
        }

        [TestMethod]
        public void GetLocation_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            Location oLocation;
            var res = Location.GetLocation(out oLocation, _mockServer,"ObjectId");
            Assert.IsFalse(res.Success, "Calling GetLocation with ErrorResponse did not fail");
        }


        [TestMethod]
        public void RefetchLocationData_NoPendingChanges_Failure()
        {
            Location oLocation = new Location(_mockServer);
            var res = oLocation.RefetchLocationData();
            Assert.IsFalse(res.Success,"Calling RefertchLocationData on empty class should fail");
        }

        #endregion
    }
}
