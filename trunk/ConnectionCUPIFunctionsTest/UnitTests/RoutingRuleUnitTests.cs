using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RoutingRuleUnitTests : BaseUnitTests
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


        #region Construction Tests

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void RoutingRule_Constructor_NullConnectionServer_Failure()
        {
            RoutingRule oTemp = new RoutingRule(null, "objectid", "displayname");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void RoutingRule_Constructor_Default_Success()
        {
            RoutingRule oTemp = new RoutingRule();
            Console.WriteLine(oTemp.ToString());
            Console.WriteLine(oTemp.DumpAllProps());
        }

        [TestMethod]
        public void RoutingRule_Constructor_EmptyObjectIdAndDisplayName_Success()
        {
            RoutingRule oTemp = new RoutingRule(_mockServer, "");
            Console.WriteLine(oTemp.SelectionDisplayString);
            Console.WriteLine(oTemp.UniqueIdentifier);
        }

        [TestMethod]
        [ExpectedException(typeof (UnityConnectionRestException))]
        public void RoutingRule_Constructor_DisplayNameNotFound_Failure()
        {
            RoutingRule oTemp = new RoutingRule(_mockServer, "", "Display Name");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void RoutingRule_Constructor_ObjectId_Success()
        {
            RoutingRule oTemp = new RoutingRule(_mockServer, "ObjectId");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void RoutingRuleCondition_Constructor_NullConnectionServer_Failure()
        {
            RoutingRuleCondition oTemp = new RoutingRuleCondition(null, "RuleObjectId", "ObjectId");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void RoutingRuleCondition_Constructor_EmptyObjectId_Failure()
        {
            RoutingRuleCondition oTemp = new RoutingRuleCondition(_mockServer, "");
            Console.WriteLine(oTemp);
        }


        #endregion


        #region Routing Rule Static Tests

        [TestMethod]
        public void AddRoutingRule_NullConnectionServer_Failure()
        {
            var res = RoutingRule.AddRoutingRule(null, "display name", null);
            Assert.IsFalse(res.Success, "Calling AddRoutingRule with null ConnectionServerRest should fail");
        }

        [TestMethod]
        public void AddRoutingRule_EmptyDisplayName_Failure()
        {
            RoutingRule oRoutingRule;
            var res = RoutingRule.AddRoutingRule(_mockServer, "", null,out oRoutingRule);
            Assert.IsFalse(res.Success, "Calling AddRoutingRule with empty display name should fail");
        }

        [TestMethod]
        public void DeleteRoutingRule_NullConnectionServer_Failure()
        {
            var res = RoutingRule.DeleteRoutingRule(null, "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteRoutingRule with null ConnectionServerRest should fail");

        }

        [TestMethod]
        public void DeleteRoutingRule_EmptyObjectId_Failure()
        {
            var res = RoutingRule.DeleteRoutingRule(_mockServer, "");
            Assert.IsFalse(res.Success, "Calling DeleteRoutingRule with empty ObjectId should fail");
        }

        [TestMethod]
        public void GetRoutingRule_NullConnectionServer_Failure()
        {
            RoutingRule oRule;
            var res = RoutingRule.GetRoutingRule(out oRule, null, "objectId", "displayname");
            Assert.IsFalse(res.Success, "Calling GetRoutingRule with null ConnectionServerRest should fail");

        }

        [TestMethod]
        public void GetRoutingRule_EmptyObjectIdAndDisplayName_Failure()
        {
            RoutingRule oRule;
            var res = RoutingRule.GetRoutingRule(out oRule, _mockServer, "", "");
            Assert.IsFalse(res.Success, "Calling GetRoutingRule with empty objectId and display name should fail");
        }

        [TestMethod]
        public void GetRoutingRules_NullConnectionServer_Failure()
        {
            List<RoutingRule> oRules;
            var res = RoutingRule.GetRoutingRules(null, out oRules);
            Assert.IsFalse(res.Success, "Calling GetRoutingRules with null ConnectionServerRest should fail");
        }

        [TestMethod]
        public void UpdateRoutingRule_NullConnectionServer_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();

            var res = RoutingRule.UpdateRoutingRule(null, "objectid", oProps);
            Assert.IsFalse(res.Success, "Calling UpdateRoutingRule with null ConnectionServerRest should fail");

        }

        [TestMethod]
        public void UpdateRoutingRule_NullProperties_Failure()
        {
            var res = RoutingRule.UpdateRoutingRule(_mockServer, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateRoutingRule with null properties should fail");
        }

        [TestMethod]
        public void UpdateRoutingRule_EmptyObjectId_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = RoutingRule.UpdateRoutingRule(_mockServer, "", oProps);
            Assert.IsFalse(res.Success, "Calling UpdateRoutingRule with empty objectId should fail");

        }

        [TestMethod]
        public void UpdateRoutingRule_EmptyProperties_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = RoutingRule.UpdateRoutingRule(_mockServer, "objectid", oProps);
            Assert.IsFalse(res.Success, "Calling UpdateRoutingRule with empty properties should fail");
        }

        [TestMethod]
        public void UpdateOrderOfAllRoutingRules_NullConnectionServer_Failure()
        {
            var res = RoutingRule.UpdateOrderOfAllRoutingRules(null, "properties");
            Assert.IsFalse(res.Success, "Calling UpdateOrderOfAllRoutingRules with null ConnectionServer should fail");
        }

        [TestMethod]
        public void UpdateOrderOfAllRoutingRules_NullProperties_Failure()
        {
            var res = RoutingRule.UpdateOrderOfAllRoutingRules(_mockServer, null);
            Assert.IsFalse(res.Success, "Calling UpdateOrderOfAllRoutingRules with null params should fail");
        }

        #endregion


        #region Routing Rule Condition Static Tests

        [TestMethod]
        public void GetRoutingRuleConditions_NullConnectionServer_Failure()
        {
            List<RoutingRuleCondition> oConditions;
            var res = RoutingRuleCondition.GetRoutingRuleConditions(null, "objectid", out oConditions);
            Assert.IsFalse(res.Success, "Calling GetRoutingRuleConditions with null ConnectionServerRest should fail");
        }

        [TestMethod]
        public void GetRoutingRuleConditions_EmptyObjectId_Failure()
        {
            List<RoutingRuleCondition> oConditions;
            var res = RoutingRuleCondition.GetRoutingRuleConditions(_mockServer, "", out oConditions);
            Assert.IsFalse(res.Success, "Calling GetRoutingRuleConditions with empty objectId should fail");
        }


        [TestMethod]
        public void AddRoutingRuleCondition_NullConnectionServer_Failure()
        {
            RoutingRuleCondition oRule;

            var res = RoutingRuleCondition.AddRoutingRuleCondition(null, "objectId",
                                                                   RoutingRuleConditionOperator.Equals,
                                                                   RoutingRuleConditionParameter.ForwardingStation,
                                                                   "value", out oRule);
            Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with null ConnectionServerRest should fail");

        }


        [TestMethod]
        public void AddRoutingRuleCondition_EmptyObjectId_Failure()
        {
            RoutingRuleCondition oRule;
            var res = RoutingRuleCondition.AddRoutingRuleCondition(_mockServer, "",
                                                                   RoutingRuleConditionOperator.Equals,
                                                                   RoutingRuleConditionParameter.ForwardingStation,
                                                                   "value", out oRule);
            Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with empty objectId should fail");

        }


        [TestMethod]
        public void AddRoutingRuleCondition_InvalidOperator_Failure()
        {
            RoutingRuleCondition oRule;
            var res = RoutingRuleCondition.AddRoutingRuleCondition(_mockServer, "objectId",
                                                                   RoutingRuleConditionOperator.Invalid,
                                                                   RoutingRuleConditionParameter.ForwardingStation,
                                                                   "value", out oRule);
            Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with invalid operator should fail");
        }


        [TestMethod]
        public void AddRoutingRuleCondition_InvalidConditionParameter_Failure()
        {
            RoutingRuleCondition oRule;
            var res = RoutingRuleCondition.AddRoutingRuleCondition(_mockServer, "objectId",
                                                                   RoutingRuleConditionOperator.Equals,
                                                                   RoutingRuleConditionParameter.Invalid, "value",
                                                                   out oRule);
            Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with invalid parameter should fail");
        }


        [TestMethod]
        public void AddRoutingRuleCondition_EmptyValueString_Failure()
        {
            RoutingRuleCondition oRule;
            var res = RoutingRuleCondition.AddRoutingRuleCondition(_mockServer, "objectId",
                                                                   RoutingRuleConditionOperator.Equals,
                                                                   RoutingRuleConditionParameter.ForwardingStation, "",
                                                                   out oRule);
            Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with empty value should fail");
        }

        [TestMethod]
        public void DeleteRoutingRuleCondition_NullConnectionServer_Failure()
        {
            var res = RoutingRuleCondition.DeleteRoutingRuleCondition(null, "ruleobjectid", "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteRoutingRuleCondition with null ConnectionServerRest should fail");

        }

        [TestMethod]
        public void DeleteRoutingRuleCondition_EmptyRuleObjectId_Failure()
        {
            var res = RoutingRuleCondition.DeleteRoutingRuleCondition(_mockServer, "", "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteRoutingRuleCondition with empty rule objectId should fail");
        }

        [TestMethod]
        public void DeleteRoutingRuleCondition_EmptyConditionObjectId_Failure()
        {
            var res = RoutingRuleCondition.DeleteRoutingRuleCondition(_mockServer, "ruleobjectid", "");
            Assert.IsFalse(res.Success, "Calling DeleteRoutingRuleCondition with empty condition objectId should fail");
        }

        [TestMethod]
        public void GetRoutingRuleCondition_NullConnectionServer_Failure()
        {
            RoutingRuleCondition oCondition;
            var res = RoutingRuleCondition.GetRoutingRuleCondition(out oCondition, null, "ruleObjectId", "objectId");
            Assert.IsFalse(res.Success, "Calling GetRoutingRuleCondition with null ConnectionServerRest should fail");
        }

        [TestMethod]
        public void GetRoutingRuleCondition_EmptyRuleObjectId_Failure()
        {
            RoutingRuleCondition oCondition;
            var res = RoutingRuleCondition.GetRoutingRuleCondition(out oCondition, _mockServer, "", "objectId");
            Assert.IsFalse(res.Success, "Calling GetRoutingRuleCondition with empty rule objectId should fail");
        }

        [TestMethod]
        public void GetRoutingRuleCondition_EmptyConditionObjectId_Failure()
        {
            RoutingRuleCondition oCondition;
            var res = RoutingRuleCondition.GetRoutingRuleCondition(out oCondition, _mockServer, "ruleObjectId", "");
            Assert.IsFalse(res.Success, "Calling GetRoutingRuleCondition with empty condition objectId should fail");
        }

        #endregion


        #region RoutingRule Property Tests

        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            RoutingRule oRule = new RoutingRule();
            const string expectedValue = "String test";
            oRule.DisplayName = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("DisplayName", expectedValue), "DisplayName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_State()
        {
            RoutingRule oRule = new RoutingRule();
            const RoutingRuleState expectedValue = RoutingRuleState.Inactive;
            oRule.State = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("State", (int)expectedValue), "State value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Type()
        {
            RoutingRule oRule = new RoutingRule();
            const RoutingRuleType expectedValue = RoutingRuleType.Forwarded;
            oRule.Type = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("Type", (int)expectedValue), "Type value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RouteTargetConversation()
        {
            RoutingRule oRule = new RoutingRule();
            const ConversationNames expectedValue = ConversationNames.SystemTransfer;
            oRule.RouteTargetConversation = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("RouteTargetConversation", expectedValue.ToString()), "RouteTargetConversation value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RouteTargetHandlerObjectId()
        {
            RoutingRule oRule = new RoutingRule();
            const string expectedValue = "String test";
            oRule.RouteTargetHandlerObjectId = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("RouteTargetHandlerObjectId", expectedValue), "RouteTargetHandlerObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_RouteAction()
        {
            RoutingRule oRule = new RoutingRule();
            const RoutingRuleActionType expectedValue = RoutingRuleActionType.Hangup;
            oRule.RouteAction = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("RouteAction", (int)expectedValue), "RouteAction value get fetch failed");
        }


        [TestMethod]
        public void PropertyGetFetch_LanguageCode()
        {
            RoutingRule oRule = new RoutingRule();
            const int expectedValue = 1066;
            oRule.LanguageCode = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("LanguageCode", expectedValue), "LanguageCode value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_UseDefaultLanguage()
        {
            RoutingRule oRule = new RoutingRule();
            const bool expectedValue = false;
            oRule.UseDefaultLanguage = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("UseDefaultLanguage", expectedValue), "UseDefaultLanguage value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_UseCallLanguage()
        {
            RoutingRule oRule = new RoutingRule();
            const bool expectedValue = true;
            oRule.UseCallLanguage = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("UseCallLanguage", expectedValue), "UseCallLanguage value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_CallType()
        {
            RoutingRule oRule = new RoutingRule();
            const RoutingRuleCallType expectedValue = RoutingRuleCallType.External;
            oRule.CallType = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("CallType", (int)expectedValue), "CallType value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_SearchSpaceObjectId()
        {
            RoutingRule oRule = new RoutingRule();
            const string expectedValue = "String test";
            oRule.SearchSpaceObjectId = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("SearchSpaceObjectId", expectedValue), "SearchSpaceObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_Undeletable()
        {
            RoutingRule oRule = new RoutingRule();
            const bool expectedValue = true;
            oRule.Undeletable = expectedValue;
            Assert.IsTrue(oRule.ChangeList.ValueExists("Undeletable", expectedValue), "Undeletable value get fetch failed");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetRoutingRules_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<RoutingRule> oRules;
            var res = RoutingRule.GetRoutingRules(_mockServer, out oRules, 1, 5, "");
            Assert.IsFalse(res.Success, "Calling GetRoutingRules with EmptyResultText did not fail");
        }


        [TestMethod]
        public void GetRoutingRules_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<RoutingRule> oRules;
            var res = RoutingRule.GetRoutingRules(_mockServer, out oRules, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetRoutingRules with garbage results should fail");
            Assert.IsTrue(oRules.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetRoutingRules_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<RoutingRule> oRules;
            var res = RoutingRule.GetRoutingRules(_mockServer, out oRules, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetRoutingRules with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetRoutingRules_ZeroCount_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<RoutingRule> oRules;
            var res = RoutingRule.GetRoutingRules(_mockServer, out oRules, 1, 5, null);
            Assert.IsTrue(res.Success, "Calling GetRoutingRules with ZeroCount failed:" + res);
        }

        [TestMethod]
        public void GetRoutingRule_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            RoutingRule oRule;
            var res = RoutingRule.GetRoutingRule(out oRule, _mockServer, "","Display Name");
            Assert.IsFalse(res.Success, "Calling GetRoutingRule with ErrorResponse did not fail");
        }

        [TestMethod]
        public void AddRoutingRule_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = RoutingRule.AddRoutingRule(_mockServer,"Display Name",null);
            Assert.IsFalse(res.Success, "Calling AddRoutingRule with ErrorResponse did not fail");
        }

        [TestMethod]
        public void DeleteRoutingRule_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.DELETE, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            RoutingRule oRoutingRule = new RoutingRule(_mockServer,"");

            var res = oRoutingRule.Delete();
            Assert.IsFalse(res.Success, "Calling DeleteRoutingRule with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdateRoutingRule_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.PUT, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test","test");
            var res = RoutingRule.UpdateRoutingRule(_mockServer, "ObjectId",oProps);
            Assert.IsFalse(res.Success, "Calling UpdateRoutingRule with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdateOrderOfAllRoutingRules_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            RoutingRule oRoutingRule = new RoutingRule(_mockServer,"");

            var res =  RoutingRule.UpdateOrderOfAllRoutingRules(_mockServer, "params");
            Assert.IsFalse(res.Success, "Calling UpdateOrderOfAllRoutingRules with ErrorResponse did not fail");
        }

        [TestMethod]
        public void AddRoutingRuleCondition_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            RoutingRule oRule = new RoutingRule(_mockServer,"");

            var res = oRule.AddRoutingRuleCondition(RoutingRuleConditionOperator.GreaterThanOrEqual,RoutingRuleConditionParameter.DialedNumber, "1234");
            Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with ErrorResponse did not fail");
        }

        [TestMethod]
        public void RefetchRoutingRuleData_ErrorResponse_Failure()
        {
            RoutingRule oRule = new RoutingRule(_mockServer, "");

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                   {
                                       Success = false,
                                       ResponseText = "error text",
                                       StatusCode = 404
                                   });
            var res = oRule.RefetchRoutingRuleData();
            Assert.IsFalse(res.Success,"Calling RefetchRoutingRuleData with Error response should fail");
        }

        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            RoutingRule oRule = new RoutingRule(_mockServer, "");
            oRule.ClearPendingChanges();
            var res = oRule.Update();
            Assert.IsFalse(res.Success, "Calling Update with no pending changes should fail");
        }

        [TestMethod]
        public void Update_ErrorResponse_Failure()
        {
            RoutingRule oRule = new RoutingRule(_mockServer, "");
            oRule.UseCallLanguage = false;

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                       It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                       {
                           Success = false,
                           ResponseText = "error text",
                           StatusCode = 404
                       });

            var res = oRule.Update();
            Assert.IsFalse(res.Success, "Calling Update with error response should fail");
        }

        #endregion

    }
}