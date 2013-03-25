﻿using System;
using System.Collections.Generic;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class UserTemplateTest
    {

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
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
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start CallHandler test:" + ex.Message);
            }

        }

        #endregion


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            UserTemplate oTemp = new UserTemplate(null);
        }

        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            UserTemplate oTemp = new UserTemplate(_connectionServer,"bogus");
        }

        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            UserTemplate oTemp = new UserTemplate(_connectionServer, "","bogus");
        }

        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            UserTemplate oUserTemplate;
            WebCallResult res= UserTemplate.AddUserTemplate(_connectionServer, "voicemailusertemplate", "test" + Guid.NewGuid().ToString(), 
                "test_" + Guid.NewGuid().ToString(), null,out  oUserTemplate);
            Assert.IsTrue(res.Success,"Failed to create new User Template:"+res);

            Console.WriteLine(oUserTemplate.ToString());
            Console.WriteLine(oUserTemplate.DumpAllProps());
            oUserTemplate.Cos();
            oUserTemplate.Cos(true);
            oUserTemplate.Password();
            oUserTemplate.PhoneSystem();
            oUserTemplate.PhoneSystem(true);
            oUserTemplate.Pin();
            oUserTemplate.PrimaryCallHandler();
            oUserTemplate.PrimaryCallHandler(true);
            
            res=oUserTemplate.RefetchUserTemplateData();
            Assert.IsTrue(res.Success,"Failed to refetch user template data:"+res);

            res = oUserTemplate.ResetPassword("Adf8124v");
            Assert.IsTrue(res.Success, "Failed to reset user template Password:"+res);

            res = oUserTemplate.ResetPin("1109481");
            Assert.IsTrue(res.Success, "Failed to reset user template PIN:"+res);

            res = oUserTemplate.Update();
            Assert.IsFalse(res.Success,"Calling update on user template with no pending changes did not fail");

            NotificationDevice oDevice;
            res = oUserTemplate.GetNotificationDevice("bogus", out oDevice);
            Assert.IsFalse(res.Success,"Fetching notification device with invalid name did not fail");

            res = oUserTemplate.GetNotificationDevice("SMTP", out oDevice,true);
            Assert.IsTrue(res.Success, "Fetching notification device SMTP failed: "+res);


            oUserTemplate.NotificationDevices();
            oUserTemplate.NotificationDevices(true);


            oUserTemplate.ClearPendingChanges();
            
            oUserTemplate.AnnounceUpcomingMeetings = 1;
            oUserTemplate.AddressAfterRecord = true;
            res = oUserTemplate.Update();
            Assert.IsTrue(res.Success,"Updating UserTemplate template failed:"+res);

            UserTemplate oTemplate2;
            res = UserTemplate.GetUserTemplate(_connectionServer, "", oUserTemplate.Alias, out oTemplate2);
            Assert.IsTrue(res.Success,"Failed to fetch user template using alias:"+res);

            List<UserTemplate> oTemplates;
            res = UserTemplate.GetUserTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success,"Calling GetUserTemplates with null connection server did not fail");

            res = UserTemplate.GetUserTemplates(_connectionServer, out oTemplates);
            Assert.IsTrue(res.Success,"Fetching user templates failed:"+res);
            Assert.IsTrue(oTemplates.Count>1,"Templates not returned from fetch");

            res=oUserTemplate.Delete();
            Assert.IsTrue(res.Success,"Failed to deletde user template:"+res);
        }


        [TestMethod]
        public void StaticCallFailures()
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

            //DeleteUserTemplate
            res = UserTemplate.DeleteUserTemplate(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: null ConnectionServer");

            UserTemplate oTemplate;
            res = UserTemplate.GetUserTemplate(null, "bogus", "bogus", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: null ConnectionServer");

            res = UserTemplate.GetUserTemplate(_connectionServer, "bogus", "bogus", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid objectId and name");

            res = UserTemplate.GetUserTemplate(_connectionServer, "", "bogus", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid name");

            res = UserTemplate.GetUserTemplate(_connectionServer, "bogus", "", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: invalid objectID");

            res = UserTemplate.GetUserTemplate(_connectionServer, "", "", out oTemplate);
            Assert.IsFalse(res.Success, "Static call to DeleteUserTemplate did not fail with: empty objectID and name");

            //GetUserTemplates
            List<UserTemplate> oTemplates;
            res = UserTemplate.GetUserTemplates(null, out oTemplates);
            Assert.IsFalse(res.Success, "Static call to GetUserTemplates did not fail with: null ConnectionServer");

            //RestUserTemplatePassword
            res = UserTemplate.ResetUserTemplatePassword(null, "bogus", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePassword did not fail with: null ConnectionServer");

            res = UserTemplate.ResetUserTemplatePassword(_connectionServer, "bogus", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePassword did not fail with: invalid objectID");

            res = UserTemplate.ResetUserTemplatePassword(_connectionServer, "", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePassword did not fail with: empty objectId");

            //ResetUserTemplatePin
            res = UserTemplate.ResetUserTemplatePin(null, "bogus", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePin did not fail with: null ConnectionServer");
            
            res = UserTemplate.ResetUserTemplatePin(_connectionServer, "bogus", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePin did not fail with: invalid objectId");

            res = UserTemplate.ResetUserTemplatePin(_connectionServer, "", "newPW");
            Assert.IsFalse(res.Success, "Static call to ResetUserTemplatePin did not fail with: empty objectId");

            //UpdateUserTemplate
            res = UserTemplate.UpdateUserTemplate(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateUserTemplate did not fail with: null ConnectionServer");

            res = UserTemplate.UpdateUserTemplate(_connectionServer, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateUserTemplate did not fail with: invalid objectID"); 
            
            res = UserTemplate.UpdateUserTemplate(_connectionServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdateUserTemplate did not fail with: empty objectID");
        }
    }
}