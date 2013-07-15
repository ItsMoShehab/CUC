using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ConnectionCUPIFunctionsTest
{
     [TestClass]
    public class RoutingRuleIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static RoutingRule _tempRule;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            //create new handler with GUID in the name to ensure uniqueness
            String strName = "TempRule_" + Guid.NewGuid().ToString().Replace("-", "");

            WebCallResult res = RoutingRule.AddRoutingRule(_connectionServer, strName, null, out _tempRule);
            Assert.IsTrue(res.Success, "Failed creating temporary routing rule:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempRule != null)
            {
                WebCallResult res = _tempRule.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary routing rule on cleanup.");
            }
        }

        #endregion


        #region Construction Failure Tests

        /// <summary>
        /// throw an UnityConnectionRestException is thrown if an invalid objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RoutingRule_Constructor_InvalidObjectId_Failure()
        {
            RoutingRule oTemp = new RoutingRule(_connectionServer, "ObjectId");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Throw an UnityConnectionRestException if an invalid alias is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RoutingRule_Constructor_InvalidDisplayName_Failure()
        {
            RoutingRule oTemp = new RoutingRule(_connectionServer, "", "Bogus display anme");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void RoutingRuleCondition_Constructor_InvalidObjectId_Failure()
        {
            RoutingRuleCondition oTemp = new RoutingRuleCondition(_connectionServer, "ObjectId", "ObjectId");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Routing Rule Static Failure Tests

         [TestMethod]
        public void DeleteRoutingRule_InvalidObjectId_Failure()
         {
             var res = RoutingRule.DeleteRoutingRule(_connectionServer, "objectid");
             Assert.IsFalse(res.Success, "Calling DeleteRoutingRule with invalid ObjectId should fail");
         }

         [TestMethod]
         public void GetRoutingRule_InvalidObjectId_Failure()
         {
             RoutingRule oRule;
             var res = RoutingRule.GetRoutingRule(out oRule, _connectionServer, "objectId");
             Assert.IsFalse(res.Success, "Calling GetRoutingRule with invalid objectId should fail");
         }

         [TestMethod]
         public void GetRoutingRule_InvalidDisplayName_Failure()
         {
             RoutingRule oRule;
             var res = RoutingRule.GetRoutingRule(out oRule, _connectionServer, "", "bogus");
             Assert.IsFalse(res.Success, "Calling GetRoutingRule with invalid dispaly name should fail");
         }

         [TestMethod]
         public void GetRoutingRules_InvalidQuery_Failure()
         {
             List<RoutingRule> oRules;
             var res = RoutingRule.GetRoutingRules(_connectionServer, out oRules,1,20,"query=(blah is blah)");
             Assert.IsFalse(res.Success, "Calling GetRoutingRules with invalid query should fail:");
             Assert.IsTrue(oRules.Count==0,"Getting rules with invalid query should return 0 rules, returned:"+oRules.Count);
         }

         [TestMethod]
         public void GetRoutingRules_ValidQuery_NoResults()
         {
             List<RoutingRule> oRules;
             var res = RoutingRule.GetRoutingRules(_connectionServer, out oRules, 1, 20, "query=(ObjectId is blah)");
             Assert.IsTrue(res.Success, "Calling GetRoutingRules with valid query should not fail:"+res);
             Assert.IsTrue(oRules.Count == 0, "Getting rules with valid query for no results should return 0 rules, returned:" + oRules.Count);
         }

         [TestMethod]
         public void StaticMethodFailure_UpdateRoutingRule()
         {
             ConnectionPropertyList oProps = new ConnectionPropertyList();
             oProps.Add("test","test");
             var res = RoutingRule.UpdateRoutingRule(_connectionServer, "objectid", oProps);
             Assert.IsFalse(res.Success, "Calling UpdateRoutingRule with invalid ObjectId should fail");
         }

         #endregion


        #region Routing Rule Condition Static Failure Tests

         [TestMethod]
         public void StaticMethodFailure_GetRoutingRuleConditions()
         {
             List<RoutingRuleCondition> oConditions;
             var res = RoutingRuleCondition.GetRoutingRuleConditions(null, "objectid", out oConditions);
             Assert.IsFalse(res.Success, "Calling GetRoutingRuleConditions with null ConnectionServerRest should fail");

             res = RoutingRuleCondition.GetRoutingRuleConditions(_connectionServer, "objectid", out oConditions);
             Assert.IsTrue(res.Success, "Calling GetRoutingRuleConditions with invalid objectId should not fail:"+res);
             Assert.IsTrue(oConditions.Count==0,"Fetching conditions with invalid objectId should return a 0 count:"+oConditions.Count);

             res = RoutingRuleCondition.GetRoutingRuleConditions(_connectionServer, "", out oConditions);
             Assert.IsFalse(res.Success, "Calling GetRoutingRuleConditions with empty objectId should fail");
         }

         [TestMethod]
         public void StaticMethodFailure_AddRoutingRuleCondition()
         {
             RoutingRuleCondition oRule;

             var res = RoutingRuleCondition.AddRoutingRuleCondition(null, "objectId",
                                                                    RoutingRuleConditionOperator.Equals,
                                                                    RoutingRuleConditionParameter.ForwardingStation, "value", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with null ConnectionServerRest should fail");

             res = RoutingRuleCondition.AddRoutingRuleCondition(_connectionServer, "",
                                                                    RoutingRuleConditionOperator.Equals,
                                                                    RoutingRuleConditionParameter.ForwardingStation, "value", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with empty objectId should fail");

             res = RoutingRuleCondition.AddRoutingRuleCondition(_connectionServer, "objectId",
                                                       RoutingRuleConditionOperator.Equals    ,
                                                       RoutingRuleConditionParameter.ForwardingStation, "value", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with invalid objectId should fail");


             res = RoutingRuleCondition.AddRoutingRuleCondition(_connectionServer, "objectId",
                                                                    RoutingRuleConditionOperator.Invalid,
                                                                    RoutingRuleConditionParameter.ForwardingStation, "value", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with invalid operator should fail");

             res = RoutingRuleCondition.AddRoutingRuleCondition(_connectionServer, "objectId",
                                                                    RoutingRuleConditionOperator.Equals,
                                                                    RoutingRuleConditionParameter.Invalid, "value", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with invalid parameter should fail");

             res = RoutingRuleCondition.AddRoutingRuleCondition(_connectionServer, "objectId",
                                                                    RoutingRuleConditionOperator.Equals,
                                                                    RoutingRuleConditionParameter.ForwardingStation, "", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with empty value should fail");

         }
         
         [TestMethod]
         public void StaticMethodFailure_DeleteRoutingRuleCondition()
         {
             var res=RoutingRuleCondition.DeleteRoutingRuleCondition(null, "ruleobjectid", "objectid");
             Assert.IsFalse(res.Success, "Calling DeleteRoutingRuleCondition with null ConnectionServerRest should fail");

             res = RoutingRuleCondition.DeleteRoutingRuleCondition(_connectionServer, "ruleobjectid", "objectid");
             Assert.IsFalse(res.Success, "Calling DeleteRoutingRuleCondition with invalid objectIds should fail");

             res = RoutingRuleCondition.DeleteRoutingRuleCondition(_connectionServer, "", "objectid");
             Assert.IsFalse(res.Success, "Calling DeleteRoutingRuleCondition with empty rule objectId should fail");

             res = RoutingRuleCondition.DeleteRoutingRuleCondition(_connectionServer, "ruleobjectid", "");
             Assert.IsFalse(res.Success, "Calling DeleteRoutingRuleCondition with empty condition objectId should fail");
         }
         
         [TestMethod]
         public void StaticMethodFailure_GetRoutingRuleCondition()
         {
             RoutingRuleCondition oCondition;
             var res = RoutingRuleCondition.GetRoutingRuleCondition(out oCondition, null, "ruleObjectId", "objectId");
             Assert.IsFalse(res.Success, "Calling GetRoutingRuleCondition with null ConnectionServerRest should fail");

             res = RoutingRuleCondition.GetRoutingRuleCondition(out oCondition, _connectionServer, "ruleObjectId", "objectId");
             Assert.IsFalse(res.Success, "Calling GetRoutingRuleCondition with invalid objectIds should fail");

             res = RoutingRuleCondition.GetRoutingRuleCondition(out oCondition, _connectionServer, "", "objectId");
             Assert.IsFalse(res.Success, "Calling GetRoutingRuleCondition with empty rule objectId should fail");

             res = RoutingRuleCondition.GetRoutingRuleCondition(out oCondition, _connectionServer, "ruleObjectId", "");
             Assert.IsFalse(res.Success, "Calling GetRoutingRuleCondition with empty condition objectId should fail");
         }

         #endregion


        #region Live Tests

         [TestMethod]
         public void FetchTests()
         {
             List<RoutingRule> oRules;
             var res = RoutingRule.GetRoutingRules(_connectionServer, out oRules, 1, 10,null);
             Assert.IsTrue(res.Success,"Fetching routing rules failed:"+res);
             Assert.IsTrue(oRules.Count>0,"No rules returned in fetch:"+res);

             RoutingRule oRule = oRules[0];

             Console.WriteLine(oRule.ToString());
             Console.WriteLine(oRule.DumpAllProps("--->"));

             RoutingRule oTest;
             res=RoutingRule.GetRoutingRule(out oTest, _connectionServer, oRule.ObjectId);
             Assert.IsTrue(res.Success,"Failed to create routing rule with valid ObjectId:"+res);
             Assert.IsTrue(oTest.ObjectId.Equals(oRule.ObjectId),"Fetched routing rule does not match objectId");

             res = RoutingRule.GetRoutingRule(out oTest, _connectionServer, "",oRule.DisplayName);
             Assert.IsTrue(res.Success, "Failed to create routing rule with valid display name:" + res);
             Assert.IsTrue(oTest.ObjectId.Equals(oRule.ObjectId), "Fetched routing rule does not match objectId");

             res = oTest.RefetchRoutingRuleData();
             Assert.IsTrue(res.Success,"Failed to refetch routing rule data");

             res = RoutingRule.GetRoutingRules(_connectionServer, out oRules, 1, 10,"query=(ObjectId is Bogus)");
             Assert.IsTrue(res.Success, "fetching rules with invalid query should not fail:" + res);
             Assert.IsTrue(oRules.Count == 0, "Invalid query string should return an empty rule list:" + oRules.Count);
         }

         [TestMethod]
         public void UpdateTests()
         {
             var res = _tempRule.Update();
             Assert.IsFalse(res.Success,"Updating rule without any pending changes should fail");

             _tempRule.LanguageCode = 1033;
             _tempRule.CallType = RoutingRuleCallType.Both;
             _tempRule.DisplayName = "UpdatedName" + Guid.NewGuid().ToString();
             _tempRule.RouteAction = RoutingRuleActionType.Hangup;
             _tempRule.State = RoutingRuleState.Inactive;
             _tempRule.Type = RoutingRuleType.System;
             _tempRule.Undeletable = false;
             _tempRule.UseCallLanguage = true;

             res = _tempRule.Update();
             Assert.IsTrue(res.Success,"Failed to update routing rule:"+res);

             res= _tempRule.AddRoutingRuleCondition(RoutingRuleConditionOperator.Equals,
                                               RoutingRuleConditionParameter.CallingNumber, "1234");

             Assert.IsTrue(res.Success,"Failed to add a routing rule condition to rule:"+res);

             List<RoutingRuleCondition> oConditions;
             res = RoutingRuleCondition.GetRoutingRuleConditions(_connectionServer, _tempRule.ObjectId, out oConditions);
             Assert.IsTrue(res.Success,"Failed fetching routing rule conditions:"+res);
             Assert.IsTrue(oConditions.Count == 1, "1 Condition should be returned, instead count=" + oConditions.Count);

             RoutingRuleCondition oCondition = oConditions[0];

             Console.WriteLine(oCondition.ToString());
             Console.WriteLine(oCondition.DumpAllProps("-->"));

             //fetch by objectid
             RoutingRuleCondition oTestCondition;
             res = RoutingRuleCondition.GetRoutingRuleCondition(out oTestCondition, _connectionServer,
                                                                oCondition.RoutingRuleObjectId,
                                                                oCondition.ObjectId);
             Assert.IsTrue(res.Success,"Failed to fetch routing rule condition using valid ObjectId:"+res);
             Assert.IsTrue(oTestCondition.ObjectId==oCondition.ObjectId,"Fetched condition does not match ObjectId of existin condition");

             res = oTestCondition.RefetchRoutingRuleConditionData();
             Assert.IsTrue(res.Success,"Failed to refetch the routing rule condition items:"+res);

             res = oCondition.Delete();
             Assert.IsTrue(res.Success, "Failed to delete condition:" + res);

             res = RoutingRuleCondition.AddRoutingRuleCondition(_connectionServer,_tempRule.ObjectId,
                 RoutingRuleConditionOperator.GreaterThan, RoutingRuleConditionParameter.DialedNumber, "1234", out oTestCondition);
             Assert.IsTrue(res.Success,"Failed to create new routing rule condition:"+res);

             res = oTestCondition.Delete();
             Assert.IsTrue(res.Success, "Failed to delete condition:" + res);
         }



         #endregion
    }
}