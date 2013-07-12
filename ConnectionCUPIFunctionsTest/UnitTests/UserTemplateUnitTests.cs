using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class UserTemplateUnitTests : BaseUnitTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            UserTemplate oTemp = new UserTemplate(null);
            Console.WriteLine(oTemp);
        }

       
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_Alias_Failure()
        {
            UserTemplate oTemp = new UserTemplate(_mockServer, "","InvalidAlias");
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void Constructor_Empty_Success()
        {
            UserTemplate oTemp = new UserTemplate();
            Console.WriteLine(oTemp.ToString());
            Console.WriteLine(oTemp.SelectionDisplayString);
            Console.WriteLine(oTemp.DumpAllProps());
            Console.WriteLine(oTemp.UniqueIdentifier);
        }


        #endregion


        #region Static Calls 

        [TestMethod]
        public void DeleteUserTemplate_NullConnectionServer_Failure()
        {
            var res = UserTemplate.DeleteUserTemplate(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void GetUserTemplate_NullConnectionServer_Failure()
        {
            UserTemplate oTemplate;
            var res = UserTemplate.GetUserTemplate(null, "objectid", "alias", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: null ConnectionServer");

            }

        [TestMethod]
        public void GetUserTemplate_EmptyObjectIdAndName_Failure()
        {
            UserTemplate oTemplate;
            var res = UserTemplate.GetUserTemplate(_mockServer, "", "", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: empty objectID and name");
        }

        [TestMethod]
        public void GetUserTemplates_NullConnectionServer_Failure()
        {
            List<UserTemplate> oTemplates;
            var res = UserTemplate.GetUserTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success, "Static call to GetUserTemplates did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void ResetUserTemplatePassword_NullConnectionServer_Failure()
        {
            var res = UserTemplate.ResetUserTemplatePassword(null, "ObjectId", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePassword did not fail with: null ConnectionServer");
         }

        [TestMethod]
        public void ResetUserTemplatePassword_EmptyObjectId_Failure()
        {
            var res = UserTemplate.ResetUserTemplatePassword(_mockServer, "", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePassword did not fail with: empty objectId");
        }

        [TestMethod]
        public void ResetUserTemplatePin_NullConnectionServer_Failure()
        {
            var res = UserTemplate.ResetUserTemplatePin(null, "ObjectId", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePin did not fail with: null ConnectionServer");

            }

        [TestMethod]
        public void ResetUserTemplatePin_EmptyObjectId_Failure()
        {
            var res = UserTemplate.ResetUserTemplatePin(_mockServer, "", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePin did not fail with: empty objectId");
        }

        [TestMethod]
        public void UpdateUserTemplate_NullConnectionServer_Failure()
        {
            var res = UserTemplate.UpdateUserTemplate(null, "ObjectId", null);
            Assert.IsFalse(res.Success, "Static call to UpdateUserTemplate did not fail with: null ConnectionServer");

            }

        [TestMethod]
        public void UpdateUserTemplate_EmptyObjectId_Failure()
        {
            var res = UserTemplate.UpdateUserTemplate(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdateUserTemplate did not fail with: empty objectID");
        }

        [TestMethod]
        public void AddUserTemplate_NullConnectionServer_Failure()
        {
            WebCallResult res = UserTemplate.AddUserTemplate(null, "voicemailusertemplate", "alias", "name", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: null ConnectionServer");
         }

        [TestMethod]
        public void AddUserTemplate_EmptyAlias_Failure()
        {
            var res = UserTemplate.AddUserTemplate(_mockServer, "voicemailusertemplate", "", "name", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: empty alias");
        }

        [TestMethod]
        public void AddUserTemplate_EmptyDisplayName_Failure()
        {
            var res = UserTemplate.AddUserTemplate(_mockServer, "voicemailusertemplate", "alias", "", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: empty display name");
        }

        [TestMethod]
        public void AddUserTemplate_EmptyTemplateAlias_Failure()
        {
            var res = UserTemplate.AddUserTemplate(_mockServer, "", "alias", "name", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: empty template alias");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void PhoneSystem_Fetch_Null()
        {
            UserTemplate oTemp = new UserTemplate();
            var oPhoneSystem = oTemp.PhoneSystem(true);
            Assert.IsNull(oPhoneSystem,"Fetching phone system from empty class should return null");
        }

        [TestMethod]
        public void NotifricationDevices_Fetch_Null()
        {
            UserTemplate oTemp = new UserTemplate();
            var oNotificationDevices = oTemp.NotificationDevices(true);
            Assert.IsNull(oNotificationDevices, "Fetching notification devices from empty class should return null");
        }

        [TestMethod]
        public void Cos_Fetch_Null()
        {
            UserTemplate oTemp = new UserTemplate();
            var oObject = oTemp.Cos(true);
            Assert.IsNull(oObject, "Fetching COS from empty class should return null");
        }

        [TestMethod]
        public void Pin_Fetch_Null()
        {
            UserTemplate oTemp = new UserTemplate();
            var oObject = oTemp.Pin();
            Assert.IsNull(oObject, "Fetching PIN from empty class should return null");
        }

        [TestMethod]
        public void Password_Fetch_Null()
        {
            UserTemplate oTemp = new UserTemplate();
            var oObject = oTemp.Password();
            Assert.IsNull(oObject, "Fetching Password from empty class should return null");
        }

        [TestMethod]
        public void PrimaryCallHandler_Fetch_Null()
        {
            UserTemplate oTemp = new UserTemplate();
            var oObject = oTemp.PrimaryCallHandler(true);
            Assert.IsNull(oObject, "Fetching primary call handler from empty class should return null");
        }

        #endregion

    }
}
