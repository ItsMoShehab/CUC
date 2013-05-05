using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for CallHandlerTest and is intended
    ///to contain all CallHandlerTest Unit Tests
    ///</summary>
   [TestClass]
    public class CallHandlerTemplateTest 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static CallHandlerTemplate _tempHandlerTemplate;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
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
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandlerTemplate test:" + ex.Message);
            }

            var res = CreateTemplateHandler();
            if (res.Success == false)
            {
                throw new Exception("Failed to create template handler to start CallHandlerTemplate test");
            }
        }

       /// <summary>
       /// Helper method to create a temporary handler template for use in these tests
       /// </summary>
       private static WebCallResult CreateTemplateHandler()
       {
           //grab the first template - should always be one and it doesn't matter which
           List<CallHandlerTemplate> oTemplates;
           WebCallResult res = CallHandlerTemplate.GetCallHandlerTemplates(_connectionServer, out oTemplates);
           if (res.Success == false || oTemplates == null || oTemplates.Count == 0)
           {
               Assert.Fail("Could not fetch call handler templates:" + res);
           }

           CallHandlerTemplate oTemplate = oTemplates[0];

           //create new handler with GUID in the name to ensure uniqueness
           String strName = "TempHandlerTemplate_" + Guid.NewGuid().ToString().Replace("-", "");

           return CallHandlerTemplate.AddCallHandlerTemplate(_connectionServer, strName, oTemplate.MediaSwitchObjectId,
                                                             oTemplate.RecipientDistributionListObjectId,
                                                             oTemplate.RecipientSubscriberObjectId, null,
                                                             out _tempHandlerTemplate);
       }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempHandlerTemplate != null)
            {
                WebCallResult res = _tempHandlerTemplate.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary call handler template on cleanup.");
            }
        }

        #endregion


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CallHandlerTemplate_ClassCreationFailure()
        {
            CallHandlerTemplate oTestTemplate = new CallHandlerTemplate(null, "aaa");
            Console.WriteLine(oTestTemplate);
        }


        #endregion


        #region Live Tests

        [TestMethod]
        public void CallHandlerTemplate_AddDeleteTest()
        {
            List<CallHandlerTemplate> oTemplates;
            WebCallResult res = CallHandlerTemplate.GetCallHandlerTemplates(_connectionServer, out oTemplates);
            Assert.IsTrue(res.Success, "Failed to get call handler templates");
            Assert.IsNotNull(oTemplates, "Null call handler template returned");
            Assert.IsTrue(oTemplates.Count > 0, "Empty list of templates returned");

            CallHandlerTemplate oTemp = oTemplates[0];

            string strRecipientUser = "";
            string strRecipientList = "";
            string strMediaSwitchId = oTemp.MediaSwitchObjectId;

            //fish the recipient off the template
            if (string.IsNullOrEmpty(oTemp.RecipientSubscriberObjectId))
            {
                strRecipientList = oTemp.RecipientSubscriberObjectId;
            }
            else
            {
                strRecipientUser = oTemp.RecipientSubscriberObjectId;
            }

            string strName = "Temp_" + Guid.NewGuid().ToString();

            CallHandlerTemplate oTemplate;
            res = CallHandlerTemplate.AddCallHandlerTemplate(_connectionServer, strName, strMediaSwitchId,
                strRecipientList, strRecipientUser, null, out oTemplate);

            Assert.IsTrue(res.Success, "Failed creating new call handler template:" + res);

            res = oTemplate.Delete();
            Assert.IsTrue(res.Success, "Failed deleting call handler template:" + res);
        }


        [TestMethod]
        public void CallHandlerTemplate_UpdateTests()
        {
            _tempHandlerTemplate.ClearPendingChanges();
            var res = _tempHandlerTemplate.Update();
            Assert.IsFalse(res.Success,"Updating template with no pending changes did not fail");

            _tempHandlerTemplate.SendPrivateMsg = 0;
            _tempHandlerTemplate.PlayAfterMessage = 0;
            _tempHandlerTemplate.UseCallLanguage = true;
            _tempHandlerTemplate.UseDefaultLanguage = true;
            _tempHandlerTemplate.UseDefaultTimeZone = true;

            res = _tempHandlerTemplate.Update();
            Assert.IsTrue(res.Success,"Failed to update call handler template:"+res);

        }

        /// <summary>
        /// exercise call handler templates - call handler templates is a pretty simple class and it only gets used by handlers so 
        /// test it here - make sure it handles an invalid conneciton server passed in and can get the list back out and hit the 
        /// ToString override for it - that about covers it.
        /// </summary>
        [TestMethod]
        public void CallHandlerTemplate_FetchTest()
        {
            List<CallHandlerTemplate> oTemplates;

            WebCallResult res = CallHandlerTemplate.GetCallHandlerTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success, "Null ConnectionServer parameter should fail");

            res = CallHandlerTemplate.GetCallHandlerTemplates(_connectionServer, out oTemplates);
            Assert.IsTrue(res.Success, "Failed to get call handler templates");
            Assert.IsNotNull(oTemplates, "Null call handler template returned");
            Assert.IsTrue(oTemplates.Count > 0, "Empty list of templates returned");

            //exercise the toString method
            Console.WriteLine(oTemplates[0].ToString());
            Console.WriteLine(oTemplates[0].DumpAllProps());

            res = oTemplates[0].RefetchUserTemplateData();
            Assert.IsTrue(res.Success, "Failed refetching template data:" + res);

            //exercise the NEW create methods
            CallHandlerTemplate oNewTemplate;
            try
            {
                oNewTemplate = new CallHandlerTemplate(_connectionServer, oTemplates[0].ObjectId);
                Console.WriteLine(oNewTemplate);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get call handler template via NEW by objectId:" + ex);
            }

            try
            {
                oNewTemplate = new CallHandlerTemplate(_connectionServer, "", oTemplates[0].DisplayName);
                Console.WriteLine(oNewTemplate);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get call handler template via NEW by displayName:" + ex);
            }

            try
            {
                oNewTemplate = new CallHandlerTemplate(_connectionServer, "");
                Console.WriteLine(oNewTemplate);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create empty call handler template via NEW:" + ex);
            }

            //exercise the static methods
            res = CallHandlerTemplate.GetCallHandlerTemplate(out oNewTemplate, _connectionServer);
            Assert.IsFalse(res.Success, "Static call to get call handler template did not fail with empty objectid and name");

            res = CallHandlerTemplate.GetCallHandlerTemplate(out oNewTemplate, null);
            Assert.IsFalse(res.Success, "Static call to get call handler template did not fail with null ConnectionServer");

            res = CallHandlerTemplate.GetCallHandlerTemplate(out oNewTemplate, _connectionServer, oTemplates[0].ObjectId);
            Assert.IsTrue(res.Success, "Failed to get call handler via static call using ObjectID:" + res);

            res = CallHandlerTemplate.GetCallHandlerTemplate(out oNewTemplate, _connectionServer, "", oTemplates[0].DisplayName);
            Assert.IsTrue(res.Success, "Failed to get call handler via static call using DisplayName:" + res);

            res = CallHandlerTemplate.GetCallHandlerTemplate(out oNewTemplate, _connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Call to get call handler via static call using invalid DisplayName did not fail.");

        }

        #endregion


        #region Static Call Failures

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetCallHandlerTemplates()
        {
            List<CallHandlerTemplate> oTemplates;
            WebCallResult res = CallHandlerTemplate.GetCallHandlerTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success, "Passing null connection server should fail.");


        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures__AddCallHandlerTemplate()
        {

            CallHandlerTemplate oTemplate;
            var res = CallHandlerTemplate.AddCallHandlerTemplate(null, "display name", "mediaswitchobjectid", "recipientlist","recipientuser", 
                null, out oTemplate);
            Assert.IsFalse(res.Success,"AddCallHandlerTemplate with null ConnectionServer did not fail");

            res = CallHandlerTemplate.AddCallHandlerTemplate(_connectionServer, "", "mediaswitchobjectid", "recipientlist", "recipientuser",null, out oTemplate);
            Assert.IsFalse(res.Success, "AddCallHandlerTemplate with empty display name did not fail");

            res = CallHandlerTemplate.AddCallHandlerTemplate(_connectionServer, "displayname", "", "recipientlist", "recipientuser",null, out oTemplate);
            Assert.IsFalse(res.Success, "AddCallHandlerTemplate with empty mediaswitch objectId did not fail");

            res = CallHandlerTemplate.AddCallHandlerTemplate(_connectionServer, "displayname", "mediaswitchobjectid", "", "",null, out oTemplate);
            Assert.IsFalse(res.Success, "AddCallHandlerTemplate with no recipients did not fail");

        }

        [TestMethod]
        public void StaticCallFailures__DeleteCallHandlerTemplate()
        {
            var res= CallHandlerTemplate.DeleteCallHandlerTemplate(null, "objectid");
            Assert.IsFalse(res.Success, "DeleteCallHandlerTemplate with null connection server did not fail");

            res = CallHandlerTemplate.DeleteCallHandlerTemplate(_connectionServer, "objectid");
            Assert.IsFalse(res.Success, "DeleteCallHandlerTemplate with invalid objectId did not fail");

            res = CallHandlerTemplate.DeleteCallHandlerTemplate(_connectionServer,"");
            Assert.IsFalse(res.Success, "DeleteCallHandlerTemplate with empty objectid did not fail");
        }


        [TestMethod]
        public void StaticCallFailures__GetCallHandlerTemplate()
        {
            CallHandlerTemplate oTemplate;
            var res= CallHandlerTemplate.GetCallHandlerTemplate(out oTemplate, null, "objectid", "displayname");
            Assert.IsFalse(res.Success, "GetCallHandlerTemplate with null ConnectionServer did not fail");

            res = CallHandlerTemplate.GetCallHandlerTemplate(out oTemplate, _connectionServer, "", "");
            Assert.IsFalse(res.Success, "GetCallHandlerTemplate with empty objectId and name did not fail");

            res = CallHandlerTemplate.GetCallHandlerTemplate(out oTemplate, _connectionServer, "objectId", "");
            Assert.IsFalse(res.Success, "GetCallHandlerTemplate with invalid objectId did not fail");

            res = CallHandlerTemplate.GetCallHandlerTemplate(out oTemplate, _connectionServer, "", "_bogus_");
            Assert.IsFalse(res.Success, "GetCallHandlerTemplate with invalid name did not fail");

        }

        [TestMethod]
        public void StaticCallFailures__UpdateCallHandlerTemplate()
        {
            var res= CallHandlerTemplate.UpdateCallHandlerTemplate(null, "objectId", null);
            Assert.IsFalse(res.Success, "UpdateCallHandlerTemplate with null Connection server did not fail");

            res = CallHandlerTemplate.UpdateCallHandlerTemplate(_connectionServer, "", null);
            Assert.IsFalse(res.Success, "UpdateCallHandlerTemplate with empty object did not fail");

            ConnectionPropertyList oProps = new ConnectionPropertyList();

            res = CallHandlerTemplate.UpdateCallHandlerTemplate(_connectionServer, "objectId", oProps);
            Assert.IsFalse(res.Success, "UpdateCallHandlerTemplate with empty property list did not fail");

            oProps.Add("dummy","dummy");

            res = CallHandlerTemplate.UpdateCallHandlerTemplate(_connectionServer, "objectId", oProps);
            Assert.IsFalse(res.Success, "UpdateCallHandlerTemplate with invalid ObjectId did not fail");
        }

        #endregion
    }
}