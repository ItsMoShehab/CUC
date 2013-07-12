using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class MailboxInfoUnitTests : BaseUnitTests
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
            MailboxInfo otest = new MailboxInfo(null,"");
            Console.WriteLine(otest);
        }

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if an invalid user objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_EmptyConnectionServer_Failure()
        {
            MailboxInfo otest = new MailboxInfo(new ConnectionServerRest(new RestTransportFunctions()), "blah");
            Console.WriteLine(otest);
        }

        /// <summary>
        /// Make sure an ArgumentException is thrown if an empty user objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_EmptyObjectId_Failure()
        {
            MailboxInfo otest = new MailboxInfo(_mockServer, "");
            Console.WriteLine(otest);
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetFolderCount_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "{\"DisplayName\":\"Voice Mailbox\"}"
                                    });

            MailboxInfo oInfo = new MailboxInfo(_mockServer,"objectid");

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            int iCount;
            var res = oInfo.GetFolderCount(MailboxInfo.FolderTypes.deleted, out iCount);
            Assert.IsFalse(res.Success,"Calling GetFolderCount with error response should fail");
        }

        [TestMethod]
        public void GetFolderCount_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), true)).Returns(new WebCallResult
                                   {
                                       Success = true,
                                       ResponseText = "{\"DisplayName\":\"Voice Mailbox\"}"
                                   });

            MailboxInfo oInfo = new MailboxInfo(_mockServer, "objectid");

            Console.WriteLine(oInfo.DumpAllProps());

            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        TotalObjectCount = 1,
                                        ResponseText = "garbage response that cannot be parsed for mailbox info",
                                    });

            int iCount;
            var res = oInfo.GetFolderCount(MailboxInfo.FolderTypes.deleted, out iCount);
            Assert.IsFalse(res.Success, "Calling GetFolderCount with garbage response should fail");
        }

        #endregion
    }
}
