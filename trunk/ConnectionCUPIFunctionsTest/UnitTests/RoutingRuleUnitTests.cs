using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;


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


        #region Construction Failure Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RoutingRule_Constructor_NullConnectionServer_Failure()
        {
            RoutingRule oTemp = new RoutingRule(null,"objectid","displayname");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RoutingRuleCondition_Constructor_NullConnectionServer_Failure()
        {
            RoutingRuleCondition oTemp = new RoutingRuleCondition(null, "bogus", "bogus");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RoutingRuleCondition_Constructor_EmptyObjectId_Failure()
        {
            RoutingRuleCondition oTemp = new RoutingRuleCondition(_mockServer, "");
            Console.WriteLine(oTemp);
        }

        
        #endregion


        #region Routing Rule Static Failure Tests

         [TestMethod]
         public void AddRoutingRule_NullConnectionServer_Failure()
         {
             var res = RoutingRule.AddRoutingRule(null, "display name", null);
             Assert.IsFalse(res.Success, "Calling AddRoutingRule with null ConnectionServerRest should fail");
         }

         [TestMethod]
        public void AddRoutingRule_EmptyDisplayName_Failure()
         {
            var res = RoutingRule.AddRoutingRule(_mockServer, "",null);
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

         #endregion


        #region Routing Rule Condition Static Failure Tests

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
                                                                    RoutingRuleConditionParameter.ForwardingStation, "value", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with null ConnectionServerRest should fail");

             }


         [TestMethod]
         public void AddRoutingRuleCondition_EmptyObjectId_Failure()
         {
             RoutingRuleCondition oRule;
             var res = RoutingRuleCondition.AddRoutingRuleCondition(_mockServer, "",
                                                                    RoutingRuleConditionOperator.Equals,
                                                                    RoutingRuleConditionParameter.ForwardingStation, "value", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with empty objectId should fail");

             }


         [TestMethod]
         public void AddRoutingRuleCondition_InvalidOperator_Failure()
         {
             RoutingRuleCondition oRule;
             var res = RoutingRuleCondition.AddRoutingRuleCondition(_mockServer, "objectId",
                                                                    RoutingRuleConditionOperator.Invalid,
                                                                    RoutingRuleConditionParameter.ForwardingStation, "value", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with invalid operator should fail");
             }


         [TestMethod]
         public void AddRoutingRuleCondition_InvalidConditionParameter_Failure()
         {
             RoutingRuleCondition oRule;
             var res = RoutingRuleCondition.AddRoutingRuleCondition(_mockServer, "objectId",
                                                                    RoutingRuleConditionOperator.Equals,
                                                                    RoutingRuleConditionParameter.Invalid, "value", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with invalid parameter should fail");
             }


         [TestMethod]
         public void AddRoutingRuleCondition_EmptyValueString_Failure()
         {
             RoutingRuleCondition oRule;
             var res = RoutingRuleCondition.AddRoutingRuleCondition(_mockServer, "objectId",
                                                                    RoutingRuleConditionOperator.Equals,
                                                                    RoutingRuleConditionParameter.ForwardingStation, "", out oRule);
             Assert.IsFalse(res.Success, "Calling AddRoutingRuleCondition with empty value should fail");
         }
         
         [TestMethod]
         public void DeleteRoutingRuleCondition_NullConnectionServer_Failure()
         {
             var res=RoutingRuleCondition.DeleteRoutingRuleCondition(null, "ruleobjectid", "objectid");
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

    }
}