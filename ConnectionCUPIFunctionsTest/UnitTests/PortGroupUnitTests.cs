using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PortGroupUnitTests : BaseUnitTests
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
        public void Constructor_NullConnectionServer_Failure()
        {
            PortGroup oPorts = new PortGroup(null);
            Console.WriteLine(oPorts);
        }


        /// <summary>
        /// Empty Connection class instance should fail with UnityConnectionRestException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_EmptyConnectionServer_Failure()
        {
            PortGroup oPorts = new PortGroup(new ConnectionServerRest(new RestTransportFunctions()), "blah");
            Console.WriteLine(oPorts);
        }

        /// <summary>
        /// Empty Connection class instance should fail with UnityConnectionRestException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_FetchByInvalidName_Failure()
        {
            PortGroup oPorts = new PortGroup(_mockServer, "","Invalid Name");
            Console.WriteLine(oPorts);
        }

        [TestMethod]
        public void Constructor_NoIds_Success()
        {
            PortGroup oPorts = new PortGroup(_mockServer);
            Console.WriteLine(oPorts.ToString());
            Console.WriteLine(oPorts.SelectionDisplayString);
            Console.WriteLine(oPorts.UniqueIdentifier);
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            PortGroup oPorts = new PortGroup();
            Console.WriteLine(oPorts.DumpAllProps());
        }


        #endregion


        #region Static Call Tests

        [TestMethod]
        public void GetPortGroups_NullConnectionServer_Failure()
        {
            List<PortGroup> oPortGroups;

            WebCallResult res = PortGroup.GetPortGroups(null, out oPortGroups);
            Assert.IsFalse(res.Success, "Fetching port groups with null Connection server should fail.");
        }

        [TestMethod]
        public void GetPortGroups_EmptyConnectionServer_Failure()
        {
            List<PortGroup> oPortGroups;

            var res = PortGroup.GetPortGroups(new ConnectionServerRest(new RestTransportFunctions()), out oPortGroups);
            Assert.IsFalse(res.Success, "Fetching port groups with invalid Connection server should fail.");
        }

        [TestMethod]
        public void GetPortGroups_EmptyMediaSwitchObjectId_Failure()
        {
            List<PortGroup> oPortGroups;

            var res = PortGroup.GetPortGroups(_mockServer, out oPortGroups,"");
            Assert.IsFalse(res.Success, "Fetching port groups with empty MediaSwitchObjectId should fail.");
            Assert.IsTrue(oPortGroups.Count==0,"Fetching port groups with empty MediaSwitchObjectId should return an empty list");
        }

        [TestMethod]
        public void GetPortGroup_NullConnectionServer_Failure()
        {
            PortGroup oPortGroup;

            var res = PortGroup.GetPortGroup(out oPortGroup,null,"ObjectId");
            Assert.IsFalse(res.Success, "Fetching port group with null Connection server should fail.");
        }

        [TestMethod]
        public void GetPortGroup_EmptyObjectIdAndName_Failure()
        {
            PortGroup oPortGroup;

            var res = PortGroup.GetPortGroup(out oPortGroup, _mockServer, "");
            Assert.IsFalse(res.Success, "Fetching port group with empty objectId and name should fail.");
        }

        [TestMethod]
        public void UpdatePortGroup_EmptyObjectId_Failure()
        {
            var res = PortGroup.UpdatePortGroup(_mockServer, "",new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdatePortGroup with empty objectId should fail.");
        }

        [TestMethod]
        public void UpdatePortGroup_NullConnectionServer_Failure()
        {
            var res = PortGroup.UpdatePortGroup(null, "ObjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdatePortGroup with null ConnectionServer should fail.");
        }

        [TestMethod]
        public void UpdatePortGroup_EmptyPropList_Failure()
        {
            var res = PortGroup.UpdatePortGroup(_mockServer, "ObjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdatePortGroup with empty objectId and name should fail.");
        }


        [TestMethod]
        public void AddPortGroup_NullConnectionServer_Failure()
        {
            var res = PortGroup.AddPortGroup(null, "Display Name", "PhoneSystemId", "HostAddress",
                                             TelephonyIntegrationMethodEnum.SCCP, "sccpPrefix");
            Assert.IsFalse(res.Success, "Calling UpdatePortGroup with null ConnectionServer should fail.");
        }

        [TestMethod]
        public void AddPortGroup_EmptyDisplayName_Failure()
        {
            var res = PortGroup.AddPortGroup(_mockServer, "", "PhoneSystemId", "HostAddress",
                                             TelephonyIntegrationMethodEnum.SCCP, "sccpPrefix");
            Assert.IsFalse(res.Success, "Calling UpdatePortGroup with empty display name should fail.");
        }

        [TestMethod]
        public void AddPortGroup_EmptyPhoneSystemId_Failure()
        {
            var res = PortGroup.AddPortGroup(_mockServer, "Display Name", "", "HostAddress",
                                             TelephonyIntegrationMethodEnum.SCCP, "sccpPrefix");
            Assert.IsFalse(res.Success, "Calling UpdatePortGroup with empty phone system ID should fail.");
        }

        [TestMethod]
        public void AddPortGroup_EmptyHostAddress_Failure()
        {
            PortGroup oPortGroup;
            var res = PortGroup.AddPortGroup(_mockServer, "Display Name", "PhoneSystemId", "",
                                             TelephonyIntegrationMethodEnum.SCCP, "sccpPrefix",out oPortGroup);
            Assert.IsFalse(res.Success, "Calling UpdatePortGroup with empty host address should fail.");
        }

        [TestMethod]
        public void DeletePortGroup_EmptyObjectId_Failure()
        {
            var res = PortGroup.DeletePortGroup(_mockServer,"");
            Assert.IsFalse(res.Success, "Calling DeletePortGroup with empty ObjectId should fail.");
        }

        [TestMethod]
        public void DeletePortGroup_NullConnectionServer_Failure()
        {
            var res = PortGroup.DeletePortGroup(null, "ObjectId");
            Assert.IsFalse(res.Success, "Calling DeletePortGroup with null ConnectionServer should fail.");
        }

        #endregion


        #region Harness Tests


        [TestMethod]
        public void GetPortGroups_EmptyResult_Failure()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<PortGroup> oPortGroups;
            var res = PortGroup.GetPortGroups(_mockServer, out oPortGroups, 1, 10, "");
            Assert.IsFalse(res.Success, "Calling GetPortGroups with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetPortGroups_GarbageResponse_Failure()
        {
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as port group JSON data"
                                  });

            List<PortGroup> oPortGroups;
            var res = PortGroup.GetPortGroups(_mockServer, out oPortGroups, 1, 10, null);
            Assert.IsFalse(res.Success, "Calling GetPortGroups with garbage results should fail");
            Assert.IsTrue(oPortGroups.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetPortGroups_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<PortGroup> oPortGroups;
            var res = PortGroup.GetPortGroups(_mockServer, out oPortGroups, 1, 10, "");
            Assert.IsFalse(res.Success, "Calling GetPortGroups with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetPortGroups_ZeroCount_Success()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<PortGroup> oPortGroups;
            var res = PortGroup.GetPortGroups(_mockServer, out oPortGroups, 1, 10, "");
            Assert.IsTrue(res.Success, "Calling GetPortGroups with ZeroCount failed:" + res);
        }



        [TestMethod]
        public void GetPortGroups_MediaSwitchObjectId_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<PortGroup> oPortGroups;
            var res = PortGroup.GetPortGroups(_mockServer, out oPortGroups, "MediaSwitchObjectId");
            Assert.IsFalse(res.Success, "Calling GetPortGroups with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetPortGroup_ErrorResponse_Failure()
        {
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            PortGroup oPortGroup;
            var res = PortGroup.GetPortGroup(out oPortGroup, _mockServer,"objectId");
            Assert.IsFalse(res.Success, "Calling GetPortGroup with ErrorResponse did not fail");
        }


        [TestMethod]
        public void UpdatePortGroup_ErrorResponse_Failure()
        {
            
            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.POST, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test","test");
            var res = PortGroup.UpdatePortGroup(_mockServer,"PortGroupOBjectId",oProps);
            Assert.IsFalse(res.Success, "Calling UpdatePortGroup with ErrorResponse did not fail");
        }


        [TestMethod]
        public void AddPortGroup_ErrorResponse_Failure()
        {

            //setup so the fetch for port group templates returns a match on the integration method we're using (PIMG)
            PortGroupTemplate oTemplate = new PortGroupTemplate();
            oTemplate.CopyTelephonyIntegrationMethodEnum = TelephonyIntegrationMethodEnum.PIMG;
            List<PortGroupTemplate> oList = new List<PortGroupTemplate>();
            oList.Add(oTemplate);

            _mockTransport.Setup(x => x.GetObjectsFromJson<PortGroupTemplate>(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(oList);

            //make sure all "gets" return true.
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                   {
                                       Success = true,
                                       ResponseText = "dummy text",
                                       TotalObjectCount = 1
                                   });

            //error response for post
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.POST, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            PortGroup oPortGroup;
            var res = PortGroup.AddPortGroup(_mockServer,"display name","PhoneSysteId","HostAddress", TelephonyIntegrationMethodEnum.PIMG);
            Assert.IsFalse(res.Success, "Calling AddPortGroup with ErrorResponse did not fail");
        }


        [TestMethod]
        public void DeletePortGroup_ErrorResponse_Failure()
        {

            //error response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = PortGroup.DeletePortGroup(_mockServer, "PortGroupOBjectId");
            Assert.IsFalse(res.Success, "Calling DeletePortGroup with ErrorResponse did not fail");
        }

        [TestMethod]
        public void RefreshortGroupData_EmptyObjectId_Failure()
        {
            PortGroup oPortGroup = new PortGroup();
            var res = oPortGroup.RefetchPortGroupData();
            Assert.IsFalse(res.Success,"Refreshing data with empty object should fail");
        }


        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            PortGroup oPortGroup = new PortGroup();
            var res = oPortGroup.Update();
            Assert.IsFalse(res.Success, "Updating data with no pending changes should fail");
        }

        [TestMethod]
        public void Update_EmptyObject_Failure()
        {
            PortGroup oPortGroup = new PortGroup();
            oPortGroup.DisplayName = "New Display Name";
            var res = oPortGroup.Update();
            Assert.IsFalse(res.Success, "Updating data with empty object should fail");
        }

        [TestMethod]
        public void Delete_EmptyObject_Failure()
        {
            PortGroup oPortGroup = new PortGroup();
            var res = oPortGroup.Delete();
            Assert.IsFalse(res.Success, "Deleting data with empty object should fail");
        }

        #endregion


        #region Property Tests


        [TestMethod]
        public void PropertyGetFetch_CcmDoAutoFailback()
        {
            PortGroup oPortGroup = new PortGroup();
            const bool expectedValue = true;
            oPortGroup.CcmDoAutoFailback = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("CcmDoAutoFailback", expectedValue),"CcmDoAutoFailback value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DelayBeforeOpeningMs()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.DelayBeforeOpeningMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("DelayBeforeOpeningMs", expectedValue), "DelayBeforeOpeningMs value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            PortGroup oPortGroup = new PortGroup();
            const string expectedValue = "display string test";
            oPortGroup.DisplayName = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("DisplayName", expectedValue), "DisplayName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DtmfDialInterDigitDelayMs()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.DtmfDialInterDigitDelayMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("DtmfDialInterDigitDelayMs", expectedValue), "DtmfDialInterDigitDelayMs value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_EnableMWI()
        {
            PortGroup oPortGroup = new PortGroup();
            const bool expectedValue = false;
            oPortGroup.EnableMWI = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("EnableMWI", expectedValue), "EnableMWI value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_EnableAGC()
        {
            PortGroup oPortGroup = new PortGroup();
            const bool expectedValue = true;
            oPortGroup.EnableAGC = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("EnableAGC", expectedValue), "EnableAGC value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MediaSipSecurityProfileObjectId()
        {
            PortGroup oPortGroup = new PortGroup();
            const string expectedValue = "Test string";
            oPortGroup.MediaSipSecurityProfileObjectId = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("MediaSipSecurityProfileObjectId", expectedValue), "MediaSipSecurityProfileObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MwiOnCode()
        {
            PortGroup oPortGroup = new PortGroup();
            const string expectedValue = "1234";
            oPortGroup.MwiOnCode = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("MwiOnCode", expectedValue), "MwiOnCode value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MwiOffCode()
        {
            PortGroup oPortGroup = new PortGroup();
            const string expectedValue = "4321";
            oPortGroup.MwiOffCode = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("MwiOffCode", expectedValue), "MwiOffCode value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MwiRetryCountOnSuccess()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 7;
            oPortGroup.MwiRetryCountOnSuccess = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("MwiRetryCountOnSuccess", expectedValue), "MwiRetryCountOnSuccess value get fetch failed");
        }
        
        [TestMethod]
        public void PropertyGetFetch_MwiRetryIntervalOnSuccessMs()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.MwiRetryIntervalOnSuccessMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("MwiRetryIntervalOnSuccessMs", expectedValue), "MwiRetryIntervalOnSuccessMs value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_MwiMinRequestIntervalMs()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.MwiMinRequestIntervalMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("MwiMinRequestIntervalMs", expectedValue), "MwiMinRequestIntervalMs value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_MwiMaxConcurrentRequests()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.MwiMaxConcurrentRequests = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("MwiMaxConcurrentRequests", expectedValue), "MwiMaxConcurrentRequests value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_NoiseFreeEnable()
        {
            PortGroup oPortGroup = new PortGroup();
            const bool expectedValue = true;
            oPortGroup.NoiseFreeEnable = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("NoiseFreeEnable", expectedValue), "NoiseFreeEnable value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_OutgoingGuardTimeMs()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.OutgoingGuardTimeMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("OutgoingGuardTimeMs", expectedValue), "OutgoingGuardTimeMs value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_OutgoingPostDialDelayMs()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.OutgoingPostDialDelayMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("OutgoingPostDialDelayMs", expectedValue), "OutgoingPostDialDelayMs value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_OutgoingPreDialDelayMs()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.OutgoingPreDialDelayMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("OutgoingPreDialDelayMs", expectedValue), "OutgoingPreDialDelayMs value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PreferredCallControl()
        {
            PortGroup oPortGroup = new PortGroup();
            const PreferredTransport expectedValue = PreferredTransport.Ipv4;
            oPortGroup.PreferredCallControl = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("PreferredCallControl", (int)expectedValue), "PreferredCallControl value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RecordingDTMFClipMs()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.RecordingDTMFClipMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("RecordingDTMFClipMs", expectedValue), "RecordingDTMFClipMs value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RecordingToneExtraClipMss()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 1234;
            oPortGroup.RecordingToneExtraClipMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("RecordingToneExtraClipMs", expectedValue), "RecordingToneExtraClipMs value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ResetStatusEnum()
        {
            PortGroup oPortGroup = new PortGroup();
            const ResetStatusEnum expectedValue = ResetStatusEnum.InProgress;
            oPortGroup.ResetStatusEnum = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("ResetStatusEnum", (int)expectedValue), "ResetStatusEnum value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SipDoSrtp()
        {
            PortGroup oPortGroup = new PortGroup();
            const bool expectedValue = true;
            oPortGroup.SipDoSRTP = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SipDoSRTP", expectedValue), "SipDoSRTP value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SipContactLineName()
        {
            PortGroup oPortGroup = new PortGroup();
            const string expectedValue = "String test value";
            oPortGroup.SipContactLineName = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SipContactLineName", expectedValue), "SipContactLineName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SipDoAuthenticate()
        {
            PortGroup oPortGroup = new PortGroup();
            const bool expectedValue = true;
            oPortGroup.SipDoAuthenticate = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SipDoAuthenticate", expectedValue), "SipDoAuthenticate value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SipDoDtmfRfc2833()
        {
            PortGroup oPortGroup = new PortGroup();
            const bool expectedValue = true;
            oPortGroup.SipDoDtmfRfc2833 = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SipDoDtmfRfc2833", expectedValue), "SipDoDtmfRfc2833 value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SipDoDtmfKpml()
        {
            PortGroup oPortGroup = new PortGroup();
            const bool expectedValue = true;
            oPortGroup.SipDoDtmfKPML = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SipDoDtmfKPML", expectedValue), "SipDoDtmfKPML value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SipPreferredMedia()
        {
            PortGroup oPortGroup = new PortGroup();
            const PreferredTransport expectedValue = PreferredTransport.Ipv6;
            oPortGroup.SipPreferredMedia = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SipPreferredMedia",(int)expectedValue), "SipPreferredMedia value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SipRegisterWithProxyServer()
        {
            PortGroup oPortGroup = new PortGroup();
            const bool expectedValue = true;
            oPortGroup.SipRegisterWithProxyServer = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SipRegisterWithProxyServer", expectedValue), "SipRegisterWithProxyServer value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SipTLSModeEnum()
        {
            PortGroup oPortGroup = new PortGroup();
            const SipTlsModes expectedValue = SipTlsModes.Authenticated;
            oPortGroup.SipTLSModeEnum = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SipTLSModeEnum",(int)expectedValue), "SipTLSModeEnum value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SipTransportProtocolEnum()
        {
            PortGroup oPortGroup = new PortGroup();
            const SipTransportEnum expectedValue = SipTransportEnum.Tcp;
            oPortGroup.SipTransportProtocolEnum = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SipTransportProtocolEnum", (int)expectedValue), "SipTransportProtocolEnum value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SkinnyDevicePrefix()
        {
            PortGroup oPortGroup = new PortGroup();
            const string expectedValue = "String test";
            oPortGroup.SkinnyDevicePrefix = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("SkinnyDevicePrefix", expectedValue), "SkinnyDevicePrefix value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_WaitForCallInfoMs()
        {
            PortGroup oPortGroup = new PortGroup();
            const int expectedValue = 345;
            oPortGroup.WaitForCallInfoMs = expectedValue;
            Assert.IsTrue(oPortGroup.ChangeList.ValueExists("WaitForCallInfoMs", expectedValue), "WaitForCallInfoMs value get fetch failed");
        }
        #endregion
    }
}
