using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class UserTemplateTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static UserTemplate _userTemplate;
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
                _connectionServer = new ConnectionServer(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                    mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start UserTemplate test:" + ex.Message);
            }

            
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

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            UserTemplate oTemp = new UserTemplate(null);
            Console.WriteLine(oTemp);
        }

        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            UserTemplate oTemp = new UserTemplate(_connectionServer,"bogus");
            Console.WriteLine(oTemp);
        }

        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            UserTemplate oTemp = new UserTemplate(_connectionServer, "","bogus");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticCallFailures_DeleteUserTemplate()
        {
            //DeleteUserTemplate
            var res = UserTemplate.DeleteUserTemplate(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void StaticCallFailures_GetUserTemplate()
        {
            UserTemplate oTemplate;
            var res = UserTemplate.GetUserTemplate(null, "bogus", "bogus", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: null ConnectionServer");

            res = UserTemplate.GetUserTemplate(_connectionServer, "bogus", "bogus", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid objectId and name");

            res = UserTemplate.GetUserTemplate(_connectionServer, "", "bogus", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid name");

            res = UserTemplate.GetUserTemplate(_connectionServer, "bogus", "", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid objectID");

            res = UserTemplate.GetUserTemplate(_connectionServer, "", "", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: empty objectID and name");
        }


        [TestMethod]
        public void StaticCallFailures_GetUserTemplates()
        {
            //GetUserTemplates
            List<UserTemplate> oTemplates;
            var res = UserTemplate.GetUserTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success, "Static call to GetUserTemplates did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void StaticCallFailures_RestUserTemplatePassword()
        {
            //RestUserTemplatePassword
            var res = UserTemplate.ResetUserTemplatePassword(null, "bogus", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePassword did not fail with: null ConnectionServer");

            res = UserTemplate.ResetUserTemplatePassword(_connectionServer, "bogus", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePassword did not fail with: invalid objectID");

            res = UserTemplate.ResetUserTemplatePassword(_connectionServer, "", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePassword did not fail with: empty objectId");
        }

        [TestMethod]
        public void StaticCallFailures_ResetUserTemplatePin()
        {
            //ResetUserTemplatePin
            var res = UserTemplate.ResetUserTemplatePin(null, "bogus", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePin did not fail with: null ConnectionServer");

            res = UserTemplate.ResetUserTemplatePin(_connectionServer, "bogus", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePin did not fail with: invalid objectId");

            res = UserTemplate.ResetUserTemplatePin(_connectionServer, "", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePin did not fail with: empty objectId");
        }

        [TestMethod]
        public void StaticCallFailures_UpdateUserTemplate()
        {
            //UpdateUserTemplate
            var res = UserTemplate.UpdateUserTemplate(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateUserTemplate did not fail with: null ConnectionServer");

            res = UserTemplate.UpdateUserTemplate(_connectionServer, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateUserTemplate did not fail with: invalid objectID");

            res = UserTemplate.UpdateUserTemplate(_connectionServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdateUserTemplate did not fail with: empty objectID");
        }

        [TestMethod]
        public void StaticCallFailures_AddUserTemplate()
        {
            //AddUserTemplate
            WebCallResult res = UserTemplate.AddUserTemplate(null, "voicemailusertemplate", "alias", "name", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: null ConnectionServer");

            res = UserTemplate.AddUserTemplate(_connectionServer, "voicemailusertemplate", "", "name", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: empty alias");

            res = UserTemplate.AddUserTemplate(_connectionServer, "voicemailusertemplate", "alias", "", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: empty display name");

            res = UserTemplate.AddUserTemplate(_connectionServer, "", "alias", "name", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: empty template alias");

            res = UserTemplate.AddUserTemplate(_connectionServer, "bogus", "alias", "name", null);
            Assert.IsFalse(res.Success, "Static call to AddUserTemplate did not fail with: invalid template alias");

        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void TopLevelPropertyTests()
        {
            WebCallResult res = _userTemplate.Update();
            Assert.IsFalse(res.Success,"Updating user template with no pending changes should fail");

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

            res = _userTemplate.RefetchUserTemplateData();
            Assert.IsTrue(res.Success,"Failed to refetch user template data:"+res);

            res = _userTemplate.ResetPassword("Adf8124v");
            Assert.IsTrue(res.Success, "Failed to reset user template Password:"+res);

            res = _userTemplate.ResetPin("1109481");
            Assert.IsTrue(res.Success, "Failed to reset user template PIN:"+res);

            res = _userTemplate.Update();
            Assert.IsFalse(res.Success,"Calling update on user template with no pending changes did not fail");

            NotificationDevice oDevice;
            res = _userTemplate.GetNotificationDevice("bogus", out oDevice);
            Assert.IsFalse(res.Success,"Fetching notification device with invalid name did not fail");

            res = _userTemplate.GetNotificationDevice("SMTP", out oDevice, true);
            Assert.IsTrue(res.Success, "Fetching notification device SMTP failed: "+res);


            _userTemplate.NotificationDevices();
            _userTemplate.NotificationDevices(true);


            _userTemplate.ClearPendingChanges();

            _userTemplate.AnnounceUpcomingMeetings = 1;
            _userTemplate.AddressAfterRecord = true;
            res = _userTemplate.Update();
            Assert.IsTrue(res.Success,"Updating UserTemplate template failed:"+res);

            UserTemplate oTemplate2;
            res = UserTemplate.GetUserTemplate(_connectionServer, "", _userTemplate.Alias, out oTemplate2);
            Assert.IsTrue(res.Success,"Failed to fetch user template using alias:"+res);

            List<UserTemplate> oTemplates;
            res = UserTemplate.GetUserTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success,"Calling GetUserTemplates with null connection server did not fail");

            res = UserTemplate.GetUserTemplates(_connectionServer, out oTemplates);
            Assert.IsTrue(res.Success,"Fetching user templates failed:"+res);
            Assert.IsTrue(oTemplates.Count>1,"Templates not returned from fetch");
        }

        [TestMethod]
        public void FetchTests()
        {
            List<UserTemplate> oTemplates;
            var res = UserTemplate.GetUserTemplates(_connectionServer, out oTemplates, 1, 2,null);
            Assert.IsTrue(res.Success,"Failed to fetch user templates:"+res);
            Assert.IsTrue(oTemplates.Count>0,"At least one template should be fetched yet none were returned.");

            res = UserTemplate.GetUserTemplates(_connectionServer, out oTemplates, 1, 2,"query=(Alias is _Bogus)");
            Assert.IsTrue(res.Success, "fetching templates with invalid query should not fail:" + res);
            Assert.IsTrue(oTemplates.Count == 0, "Invalid query string should return an empty template list:" + oTemplates.Count);
        }

        #endregion
    }
}
