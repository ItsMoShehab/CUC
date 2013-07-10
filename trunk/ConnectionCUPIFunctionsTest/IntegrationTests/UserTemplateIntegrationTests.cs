using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class UserTemplateIntegrationTests : BaseIntegrationTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static UserTemplate _userTemplate;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
    
            WebCallResult res = UserTemplate.AddUserTemplate(_connectionServer, "voicemailusertemplate", "test" + Guid.NewGuid().ToString(),
                "test_" + Guid.NewGuid().ToString(), null, out  _userTemplate);
            Assert.IsTrue(res.Success, "Failed to create new User Template:" + res);
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_userTemplate != null)
            {
                WebCallResult res = _userTemplate.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary user template on cleanup.");
            }
        }

        #endregion


        #region Class Creation Failures

        [ExpectedException(typeof(Exception))]
        public void Constructor_InvalidObjectId_Failure()
        {
            UserTemplate oTemp = new UserTemplate(_connectionServer,"ObjectId");
            Console.WriteLine(oTemp);
        }

        [ExpectedException(typeof(Exception))]
        public void Constructor_InvalidAlias_Failure()
        {
            UserTemplate oTemp = new UserTemplate(_connectionServer, "","bogus alias");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void DeleteUserTemplate_InvalidObjectId_Failure()
        {
            var res = UserTemplate.DeleteUserTemplate(_connectionServer, "ObjectId");
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid objectId");
        }


        [TestMethod]
        public void GetUserTemplate_InvalidObjectIdAndAlias_Failure()
        {
            UserTemplate oTemplate;
            var res = UserTemplate.GetUserTemplate(_connectionServer, "ObjectId", "bogus alias", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid objectId and alias");
        }


        [TestMethod]
        public void GetUserTemplate_InvalidName_Failure()
        {
            UserTemplate oTemplate;
            var res = UserTemplate.GetUserTemplate(_connectionServer, "", "bogus alias", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid alias");
        }

        [TestMethod]
        public void GetUserTemplate_InvalidObjectId_Failure()
        {
            UserTemplate oTemplate;
            var res = UserTemplate.GetUserTemplate(_connectionServer, "ObjectId", "", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid objectID");
        }

        [TestMethod]
        public void ResetUserTemplatePassword_InvalidObjectId_Failure()
        {
            var res = UserTemplate.ResetUserTemplatePassword(_connectionServer, "ObjectId", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePassword did not fail with: invalid objectID");
        }

        [TestMethod]
        public void ResetUserTemplatePin_InvalidOBjectId_Failure()
        {
            var res = UserTemplate.ResetUserTemplatePin(_connectionServer, "ObjectId", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePin did not fail with: invalid objectId");
        }

        [TestMethod]
        public void UpdateUserTemplate_InvalidObjectId()
        {
            var res = UserTemplate.UpdateUserTemplate(_connectionServer, "ObjectId", null);
            Assert.IsFalse(res.Success, "Static call to UpdateUserTemplate did not fail with: invalid objectID");
        }

        [TestMethod]
        public void AddUserTemplate_InvalidTemplateAlias_Failure()
        {
            var res = UserTemplate.AddUserTemplate(_connectionServer, "bogus", "alias", "name", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: invalid template alias");
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            WebCallResult res = _userTemplate.Update();
            Assert.IsFalse(res.Success, "Updating user template with no pending changes should fail");
        }

        [TestMethod]
        public void ExerciseFetchToStringAndDumpProps()
        {
            Console.WriteLine(_userTemplate.ToString());
            Console.WriteLine(_userTemplate.DumpAllProps());
            _userTemplate.Cos();
            _userTemplate.Cos(true);
            _userTemplate.Password();
            _userTemplate.PhoneSystem();
            _userTemplate.PhoneSystem(true);
            _userTemplate.Pin();
            _userTemplate.PrimaryCallHandler();
            _userTemplate.PrimaryCallHandler(true);
        }

        [TestMethod]
        public void Update_Success()
        {
            const string strAddress = "1234 abcd";
            _userTemplate.Address = strAddress;

            var res = _userTemplate.Update();
            Assert.IsTrue(res.Success,"Failed to update properties on the User Template:"+res);
            
            _userTemplate.RefetchUserTemplateData();
            Assert.IsTrue(_userTemplate.Address.Equals(strAddress),"Update of teplate's address did not work");
        }

        [TestMethod]
        public void ResetPassword_Success()
        {
            var res = _userTemplate.ResetPassword("Adf8124v");
            Assert.IsTrue(res.Success, "Failed to reset user template Password:"+res);
        }

        [TestMethod]
        public void ResetPin_Success()
        {
            var res = _userTemplate.ResetPin("1109481");
            Assert.IsTrue(res.Success, "Failed to reset user template PIN:"+res);

            }

        [TestMethod]
        public void GetNotificationDevice_InvalidDeviceName_Failure()
        {
            NotificationDevice oDevice;
            var res = _userTemplate.GetNotificationDevice("bogus", out oDevice);
            Assert.IsFalse(res.Success,"Fetching notification device with invalid name did not fail");

            }

        [TestMethod]
        public void GetNotificationDevice_Success()
        {
            NotificationDevice oDevice;
            var res = _userTemplate.GetNotificationDevice("SMTP", out oDevice, true);
            Assert.IsTrue(res.Success, "Fetching notification device SMTP failed: "+res);

            _userTemplate.NotificationDevices();
            _userTemplate.NotificationDevices(true);

            }

        [TestMethod]
        public void GetUserTemplate_Alias_Success()
        {
            UserTemplate oTemplate;
            var res = UserTemplate.GetUserTemplate(_connectionServer, "", _userTemplate.Alias, out oTemplate);
            Assert.IsTrue(res.Success,"Failed to fetch user template using alias:"+res);
        }

        [TestMethod]
        public void GetUserTemplates_Success()
        {
            List<UserTemplate> oTemplates;
            var res = UserTemplate.GetUserTemplates(_connectionServer, out oTemplates);
            Assert.IsTrue(res.Success,"Fetching user templates failed:"+res);
            Assert.IsTrue(oTemplates.Count>1,"Templates not returned from fetch");
        }

        [TestMethod]
        public void GetUserTemplates_PageCounts_Success()
        {
            List<UserTemplate> oTemplates;
            var res = UserTemplate.GetUserTemplates(_connectionServer, out oTemplates, 1, 2,null);
            Assert.IsTrue(res.Success,"Failed to fetch user templates:"+res);
            Assert.IsTrue(oTemplates.Count>0,"At least one template should be fetched yet none were returned.");

            }

        [TestMethod]
        public void GetUserTemplates_QueryWithNoResults_Success()
        {
            List<UserTemplate> oTemplates;
            var res = UserTemplate.GetUserTemplates(_connectionServer, out oTemplates, 1, 2,"query=(Alias is _Bogus)");
            Assert.IsTrue(res.Success, "fetching templates with invalid query should not fail:" + res);
            Assert.IsTrue(oTemplates.Count == 0, "Invalid query string should return an empty template list:" + oTemplates.Count);
        }

        #endregion
    }
}
