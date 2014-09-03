using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class CosUnitTest : BaseUnitTests
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


        #region Class Creation Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constuctor_NullConnectionServer_Failure()
        {
            ClassOfService oTemp = new ClassOfService(null);
            Console.WriteLine(oTemp);
        }

        [ExpectedException(typeof(Exception))]
        public void Constructor_NullConnectionServerWithNonEmptyOBjectId_Failure()
        {
            ClassOfService oTemp = new ClassOfService(null,"bogus");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_FailureSearchingForDisplayName_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                       It.IsAny<string>(), true)).Returns(new WebCallResult
                       {
                           Success = false,
                           ResponseText = "error text",
                       });
            ClassOfService oTemp = new ClassOfService(_mockServer, "","Invalid Display Name");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_FailureSearchingForObjectId_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), true)).Returns(new WebCallResult
                                   {
                                       Success = false,
                                       ResponseText = "error text",
                                   });
            ClassOfService oTemp = new ClassOfService(_mockServer, "ObjectId");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void Constructor_EmptyNameAndId_Success()
        {
            ClassOfService oTemp = new ClassOfService(_mockServer);
            Console.WriteLine(oTemp.ToString());
            Console.WriteLine(oTemp.DumpAllProps());
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            ClassOfService oTemp = new ClassOfService();
            Console.WriteLine(oTemp.UniqueIdentifier);
            Console.WriteLine(oTemp.SelectionDisplayString);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void DeleteClassOfService_NullConnectionServer_Failure()
        {
            //DeleteClassOfService
            var res = ClassOfService.DeleteClassOfService(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: null connectionServer");
        }

        [TestMethod]
        public void DeleteClassOfService_EmptyObjectId_Failure()
        {
            var res = ClassOfService.DeleteClassOfService(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: empty objectid");
        }

        [TestMethod]
        public void GetClassOfService_NullConnectionServer_Failure()
        {
            ClassOfService oCos;
            var res = ClassOfService.GetClassOfService(out oCos, null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void GetClassOfService_EmptyObjectIdAndName_Failure()
        {
            ClassOfService oCos;
            var res = ClassOfService.GetClassOfService(out oCos, _mockServer);
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: empty objectId and Name");
        }

        [TestMethod]
        public void UpdateClassOfService_NullConnectionServer_Failure()
        {
            //GetClassesOfService
            var res = ClassOfService.UpdateClassOfService(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: null ConnectionServer");

         }

        [TestMethod]
        public void UpdateClassOfService_EmptyObjectId_Failure()
        {
            var res = ClassOfService.UpdateClassOfService(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: empty objectId");
        }

        [TestMethod]
        public void GetClassesOfService_NullConnectionServer_Failure()
        {
            List<ClassOfService> oCoses;
            var res = ClassOfService.GetClassesOfService(null, out oCoses);
            Assert.IsFalse(res.Success, "Static call to GetClassesOfService did not fail with: null ConnectionServer");

        }

        [TestMethod]
        public void AddClassOfService_NullConnectionServer_Failure()
        {
            ClassOfService oCos;
            WebCallResult res = ClassOfService.AddClassOfService(null, "display",null, out oCos);
            Assert.IsFalse(res.Success, "Static call to AddClassOfSerice did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void AddClassOfService_EmptyObjectId_Failure()
        {
            var res = ClassOfService.AddClassOfService(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Static call to AddClassOfSerice did not fail with: empty objectId");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetClassesOfService_EmptyResults_Failure()
        {

            List<ClassOfService> oCoses;

            //empty results
            _mockTransport.Setup(
                x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            var res = ClassOfService.GetClassesOfService(_mockServer, out oCoses, 1, 5, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetClassesOfService with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetClassesOfService_GarbageResponse_Failure()
        {
           
            List<ClassOfService> oCoses;
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed as COS JSON string"
                                  });

            var res = ClassOfService.GetClassesOfService(_mockServer, out oCoses, 1, 5, "InvalidResultText");
            Assert.IsFalse(res.Success, "Calling GetClassesOfService with garbage results should fail");
            Assert.IsTrue(oCoses.Count == 0, "Invalid result text should produce an empty list of Coeses");

            }

        [TestMethod]
        public void GetClassesOfService_ZeroCount_Success()
        {
            List<ClassOfService> oCoses;
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 0,
                                      ResponseText = "test body"
                                  });

            var res = ClassOfService.GetClassesOfService(_mockServer, out oCoses, 1, 5, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetClassesOfService with zero results should not fail:" + res);
            Assert.IsTrue(oCoses.Count == 0, "zero results  should produce an empty list of Coeses");

        }

        [TestMethod]
        public void GetClassesOfService_ErrorResponse_Failure()
        {

            List<ClassOfService> oCoses;
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = ClassOfService.GetClassesOfService(_mockServer, out oCoses, 1, 5, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetClassesOfService with ErrorResponse did not fail");

        }


        [TestMethod]
        public void AddClassOfService_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                    });

            ConnectionPropertyList oProps= new ConnectionPropertyList();
            oProps.Add("Test","Test");
            var res = ClassOfService.AddClassOfService(_mockServer,"Display Name",oProps);
            Assert.IsFalse(res.Success, "Calling AddClassOfService with ErrorResponse did not fail");

        }

        [TestMethod]
        public void GetClassOfService_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                    });

            ClassOfService oCos;
            var res = ClassOfService.GetClassOfService(out oCos, _mockServer, "ObjectId");
            Assert.IsFalse(res.Success, "Calling GetClassOfService with ErrorResponse did not fail");

        }

        [TestMethod]
        public void GetClassOfService_DeleteClassOfService_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                    });

            var res = ClassOfService.DeleteClassOfService(_mockServer, "ObjectId");
            Assert.IsFalse(res.Success, "Calling DeleteClassOfService with ErrorResponse did not fail");
        }

        [TestMethod]
        public void RefetchClassOfServiceData_EmptyObject_Failure()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            var res = oCos.RefetchClassOfServiceData();
            Assert.IsFalse(res.Success,"Refetching data on an empty class instance should fail");
        }

        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            oCos.ClearPendingChanges();
            var res = oCos.Update();
            Assert.IsFalse(res.Success, "Updating with no pending changes should fail");
        }

        [TestMethod]
        public void Update_EmptyObject_Failure()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            oCos.DisplayName="New display name";
            var res = oCos.Update();
            Assert.IsFalse(res.Success, "Updating with empty object should fail");
        }

        #endregion


        #region Property Tests


        [TestMethod]
        public void PropertyGetFetch_AccessFaxMail()
        {
            ClassOfService oCos = new ClassOfService();
            const bool expectedValue = true;
            oCos.AccessFaxMail = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("AccessFaxMail", expectedValue), "AccessFaxMail value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_AccessTts()
        {
            ClassOfService oCos = new ClassOfService();
            const bool expectedValue = true;
            oCos.AccessTts = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("AccessTts", expectedValue), "AccessTts value get fetch failed");
        }
        
        [TestMethod]
        public void PropertyGetFetch_CallHoldAvailable()
        {
            ClassOfService oCos = new ClassOfService();
            const bool expectedValue = true;
            oCos.CallHoldAvailable = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("CallHoldAvailable", expectedValue), "CallHoldAvailable value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CallScreenAvailable()
        {
            ClassOfService oCos = new ClassOfService();
            const bool expectedValue = true;
            oCos.CallScreenAvailable = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("CallScreenAvailable", expectedValue), "CallScreenAvailable value get fetch failed");
        }

      [TestMethod]
        public void PropertyGetFetch_CanRecordName()
        {
            ClassOfService oCos = new ClassOfService();
            const bool expectedValue = true;
            oCos.CanRecordName = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("CanRecordName", expectedValue), "CanRecordName value get fetch failed");
        }

      [TestMethod]
      public void PropertyGetFetch_FaxRestrictionObjectId()
      {
          ClassOfService oCos = new ClassOfService();
          const string expectedValue = "String test value";
          oCos.FaxRestrictionObjectId = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("FaxRestrictionObjectId", expectedValue), "FaxRestrictionObjectId value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_ListInDirectoryStatus()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.ListInDirectoryStatus = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("ListInDirectoryStatus", expectedValue), "ListInDirectoryStatus value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_MaxGreetingLength()
      {
          ClassOfService oCos = new ClassOfService();
          const int expectedValue = 99;
          oCos.MaxGreetingLength = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("MaxGreetingLength", expectedValue), "MaxGreetingLength value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_MaxMsgLength()
      {
          ClassOfService oCos = new ClassOfService();
          const int expectedValue = 999;
          oCos.MaxMsgLength = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("MaxMsgLength", expectedValue), "MaxMsgLength value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_MaxNameLength()
      {
          ClassOfService oCos = new ClassOfService();
          const int expectedValue = 777;
          oCos.MaxNameLength = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("MaxNameLength", expectedValue), "MaxNameLength value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_MaxPrivateDlists()
      {
          ClassOfService oCos = new ClassOfService();
          const int expectedValue = 555;
          oCos.MaxPrivateDlists = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("MaxPrivateDlists", expectedValue), "MaxPrivateDlists value get fetch failed");
      }


      [TestMethod]
      public void PropertyGetFetch_MovetoDeleteFolder()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.MovetoDeleteFolder = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("MovetoDeleteFolder", expectedValue), "MovetoDeleteFolder value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_OutcallRestrictionObjectId()
      {
          ClassOfService oCos = new ClassOfService();
          const string expectedValue = "String test value";
          oCos.OutcallRestrictionObjectId = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("OutcallRestrictionObjectId", expectedValue), "OutcallRestrictionObjectId value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_PersonalAdministrator()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.PersonalAdministrator = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("PersonalAdministrator", expectedValue), "PersonalAdministrator value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_XferRestrictionObjectId()
      {
          ClassOfService oCos = new ClassOfService();
          const string expectedValue = "Test string value";
          oCos.XferRestrictionObjectId = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("XferRestrictionObjectId", expectedValue), "XferRestrictionObjectId value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_WarnIntervalMsgEnd()
      {
          ClassOfService oCos = new ClassOfService();
          const int expectedValue = 99;
          oCos.WarnIntervalMsgEnd = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("WarnIntervalMsgEnd", expectedValue), "WarnIntervalMsgEnd value get fetch failed");
      }


      [TestMethod]
      public void PropertyGetFetch_CanSendToPublicDl()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.CanSendToPublicDl = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("CanSendToPublicDl", expectedValue), "CanSendToPublicDl value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_EnableEnhancedSecurity()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.EnableEnhancedSecurity = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("EnableEnhancedSecurity", expectedValue), "EnableEnhancedSecurity value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_AccessVmi()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.AccessVmi = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("AccessVmi", expectedValue), "AccessVmi value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_AccessLiveReply()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.AccessLiveReply = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("AccessLiveReply", expectedValue), "AccessLiveReply value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_UaAlternateExtensionAccess()
      {
          ClassOfService oCos = new ClassOfService();
          const int expectedValue = 77;
          oCos.UaAlternateExtensionAccess = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("UaAlternateExtensionAccess", expectedValue), "UaAlternateExtensionAccess value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_AccessCallRoutingRules()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.AccessCallRoutingRules = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("AccessCallRoutingRules", expectedValue), "AccessCallRoutingRules value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_WarnMinMsgLength()
      {
          ClassOfService oCos = new ClassOfService();
          const int expectedValue = 55;
          oCos.WarnMinMsgLength = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("WarnMinMsgLength", expectedValue), "WarnMinMsgLength value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_SendBroadcastMessage()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.SendBroadcastMessage = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("SendBroadcastMessage", expectedValue), "SendBroadcastMessage value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_UpdateBroadcastMessage()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.UpdateBroadcastMessage = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("UpdateBroadcastMessage", expectedValue), "UpdateBroadcastMessage value get fetch failed");
      }


      [TestMethod]
      public void PropertyGetFetch_AccessVui()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.AccessVui = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("AccessVui", expectedValue), "AccessVui value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_ImapCanFetchMessageBody()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.ImapCanFetchMessageBody = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("ImapCanFetchMessageBody", expectedValue), "ImapCanFetchMessageBody value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_ImapCanFetchPrivateMessageBody()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.ImapCanFetchPrivateMessageBody = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("ImapCanFetchPrivateMessageBody", expectedValue), "ImapCanFetchPrivateMessageBody value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_MaxMembersPVL()
      {
          ClassOfService oCos = new ClassOfService();
          const int expectedValue = 44;
          oCos.MaxMembersPVL = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("MaxMembersPVL", expectedValue), "MaxMembersPVL value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_AccessIMAP()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.AccessIMAP = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("AccessIMAP", expectedValue), "AccessIMAP value get fetch failed");
      }


      [TestMethod]
      public void PropertyGetFetch_ReadOnly()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.ReadOnly = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("ReadOnly", expectedValue), "ReadOnly value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_AccessAdvancedUserFeatures()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.AccessAdvancedUserFeatures = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("AccessAdvancedUserFeatures", expectedValue), "AccessAdvancedUserFeatures value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_AccessUnifiedClient()
      {
          ClassOfService oCos = new ClassOfService();
          const bool expectedValue = true;
          oCos.AccessUnifiedClient = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("AccessUnifiedClient", expectedValue), "AccessUnifiedClient value get fetch failed");
      }

      [TestMethod]
      public void PropertyGetFetch_RequireSecureMessages()
      {
          ClassOfService oCos = new ClassOfService(_mockServer);
          const int expectedValue = 33;
          oCos.RequireSecureMessages = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("RequireSecureMessages", expectedValue), "RequireSecureMessages value get fetch failed");
      }

        [TestMethod]
        public void PropertyGetFetch_AccessOutsideLiveReply()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            const bool expectedValue = true;
            oCos.AccessOutsideLiveReply = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("AccessOutsideLiveReply", expectedValue),
                          "AccessOutsideLiveReply value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_AccessSTT()
      {
          ClassOfService oCos = new ClassOfService(_mockServer);
          const bool expectedValue = true;
          oCos.AccessSTT = expectedValue;
          Assert.IsTrue(oCos.ChangeList.ValueExists("AccessSTT", expectedValue), "AccessSTT value get fetch failed");
      }

        [TestMethod]
        public void PropertyGetFetch_EnableSTTSecureMessage()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            const int expectedValue = 22;
            oCos.EnableSTTSecureMessage = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("EnableSTTSecureMessage", expectedValue), "EnableSTTSecureMessage value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MessagePlaybackRestriction()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            const int expectedValue = 11;
            oCos.MessagePlaybackRestriction = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("MessagePlaybackRestriction", expectedValue), "MessagePlaybackRestriction value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SttType()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            const int expectedValue = 34;
            oCos.SttType = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("SttType", expectedValue), "SttType value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            const string expectedValue = "String test value";
            oCos.DisplayName = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("DisplayName", expectedValue), "DisplayName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Undeletable()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            const bool expectedValue = true;
            oCos.Undeletable = expectedValue;
            Assert.IsTrue(oCos.ChangeList.ValueExists("Undeletable", expectedValue), "Undeletable value get fetch failed");
        }


        [TestMethod]
        public void TransferRestrictionTable_ErrorResponse_Null()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            RestrictionTable oTable = oCos.TransferRestrictionTable(true);
            Assert.IsNull(oTable, "Fetching restriction table on empty instance should return null");
        }

        [TestMethod]
        public void FaxRestrictionTable_ErrorResponse_Null()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);

            RestrictionTable oTable = oCos.FaxRestrictionTable(true);
            Assert.IsNull(oTable, "Fetching restriction table on empty instance should return null");
        }

        [TestMethod]
        public void OutcallRestrictionTable_ErrorResponse_Null()
        {
            ClassOfService oCos = new ClassOfService(_mockServer);
            RestrictionTable oTable = oCos.OutcallRestrictionTable(true);
            Assert.IsNull(oTable, "Fetching restriction table on empty instance should return null");
        }


        #endregion
    }
}
