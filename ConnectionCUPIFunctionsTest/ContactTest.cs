using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ContactTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private static Contact _tempContact;

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
                throw new Exception("Unable to attach to Connection server to start contact test:" + ex.Message);
            }

            //create new handler with GUID in the name to ensure uniqueness
            String strName = "TempHandler_" + Guid.NewGuid().ToString().Replace("-", "");

            WebCallResult res = Contact.AddContact(_connectionServer, "systemcontacttemplate", strName, "", "", strName, null, out _tempContact);
            Assert.IsTrue(res.Success, "Failed creating temporary contact:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempContact != null)
            {
                WebCallResult res = _tempContact.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary contact on cleanup.");
            }
        }

        #endregion


        #region Construction Failure Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            Contact oTemp = new Contact(null);
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// throw an UnityConnectionRestException is thrown if an invalid objectId is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure2()
        {
            Contact oTemp = new Contact(_connectionServer,"bogus");
            Console.WriteLine(oTemp);
        }

        /// <summary>
        /// Throw an UnityConnectionRestException if an invalid alias is passed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void ClassCreationFailure3()
        {
            Contact oTemp = new Contact(_connectionServer,"","bogus");
            Console.WriteLine(oTemp);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticMethodFailure_DeleteContact()
        {
            //DeleteContact
            var res = Contact.DeleteContact(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for null ConnectionServer");

            res = Contact.DeleteContact(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for empty objectId");

            res = Contact.DeleteContact(_connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for invalid  ObjectID");
        }

        [TestMethod]
        public void StaticMethodFailure_GetContact()
        {
            //GetContact
            Contact oContact;
            var res = Contact.GetContact(out oContact, null, "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for null ConnectionServer");

            res = Contact.GetContact(out oContact, _connectionServer);
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for empty objectId and alias");

            res = Contact.GetContact(out oContact, _connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for invalid ObjectId");

            res = Contact.GetContact(out oContact, _connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for invalid alias");
        }

        [TestMethod]
        public void StaticMethodFailure_GetContactVoiceName()
        {
            //GetContactVoiceName
            var res = Contact.GetContactVoiceName(null, "boguspath", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for ");

            res = Contact.GetContactVoiceName(_connectionServer, "", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for empty local wav path");

            res = Contact.GetContactVoiceName(_connectionServer, "bogus", "", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for empty objectId");

            res = Contact.GetContactVoiceName(_connectionServer, "temp.wav", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for invalid objectId");

            res = Contact.GetContactVoiceName(_connectionServer, "temp.wav", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for invalid objectId");
        }

        [TestMethod]
        public void StaticMethodFailure_SetContactVoiceName()
        {
            //SetContactVoiceName
            var res = Contact.SetContactVoiceName(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for null ConnectionServer");

            res = Contact.SetContactVoiceName(_connectionServer, "bogus", "", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for empty objectId");

            res = Contact.SetContactVoiceName(_connectionServer, "", "bogus", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for empty wav path ");

            res = Contact.SetContactVoiceName(_connectionServer, "Dummy.wav", "bogus", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for invalid ObjectId");
        }

        [TestMethod]
        public void StaticMethodFailure_SetContactVoiceNameToStream()
        {
            //SetContactVoiceNameToStream
            var res = Contact.SetContactVoiceNameToStreamFile(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for null ConnectionServer");

            res = Contact.SetContactVoiceNameToStreamFile(_connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for empty objectId");

            res = Contact.SetContactVoiceNameToStreamFile(_connectionServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for empty Wav resource Id");

            res = Contact.SetContactVoiceNameToStreamFile(_connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for invalid objectId");
        }

        [TestMethod]
        public void StaticMethodFailure_GetContacts()
        {
            //GetContacts
            List<Contact> oContacts;
            var res = Contact.GetContacts(null, out oContacts);
            Assert.IsFalse(res.Success, "Static call to GetContacts did not return failure for null ConnectionServer");

            res = Contact.GetContacts(_connectionServer, out oContacts, "query=(bogus)", "", "sort=(bogus)");
            Assert.IsFalse(res.Success, "Static call to GetContacts did not return failure for invalid query construction");
        }

        [TestMethod]
        public void StaticMethodFailure_Update()
        {
            //Update
            var res = Contact.UpdateContact(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for null ConnectionServer");

            res = Contact.UpdateContact(_connectionServer, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for null property list");

            ConnectionPropertyList oProps = new ConnectionPropertyList();

            res = Contact.UpdateContact(_connectionServer, "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for empty property list");

            oProps.Add("bogus", "bogus");
            res = Contact.UpdateContact(_connectionServer, "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for invalid objectId");

        }

        [TestMethod]
        public void StaticMethodFailure_AddContact()
        {
            //AddContact
            WebCallResult res = Contact.AddContact(null, "bogus", "bogus", "bogus", "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for null Connection server");

            res = Contact.AddContact(_connectionServer, "", "bogus", "bogus", "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for empty template alias string ");

            res = Contact.AddContact(_connectionServer, "bogus", "bogus", "bogus", "bogus", "", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for empty alias string");
        }

        #endregion


        #region Live Tests

        /// <summary>
        /// Since a clean system will not have any contacts on it we need to do the add test up front so we can then do 
        /// the fetch tests.
        /// </summary>
        [TestMethod]
        public void ContactAddUpdateDeleteTests()
        {
            
            WebCallResult res = _tempContact.Update();
            Assert.IsFalse(res.Success,"Calling update on contact without any pending changes did not return an error");

            //update tests
            _tempContact.DisplayName = "Updated display name";
            _tempContact.FirstName = "UpdateFirst";
            _tempContact.LastName = "UpdatedLast";
            _tempContact.AltFirstName = "altFirst";
            _tempContact.AltLastName = "altLast";
            _tempContact.AutoCreateCallHandler = false;
            _tempContact.ListInDirectory = true;
            _tempContact.TransferType = TransferTypes.Supervised;
            _tempContact.TransferRings = 4;
            _tempContact.TransferEnabled = false;
            _tempContact.TransferExtension = "";

            res = _tempContact.Update();
            Assert.IsTrue(res.Success,"Updating contact failed:"+res);

            _tempContact.PartitionObjectId = "bogus";
            res = _tempContact.Update();
            Assert.IsFalse(res.Success,"Setting partition to invalid value did not return error:"+res);
        }

         [TestMethod]
         public void ContactVoiceNameTests()
         {
             //Voice name tests
             WebCallResult res = _tempContact.GetVoiceName("temp.wav");
             Assert.IsFalse(res.Success, "Newly created contact should have no voice name but the fetch call did not fail");

             res = _tempContact.SetVoiceName("Dummy.wav", true);
             Assert.IsTrue(res.Success, "Updating contact voice name failed:" + res.ToString());

             res = _tempContact.RefetchContactData();
             Assert.IsTrue(res.Success, "Failed to refetch contact:" + res);

             res = _tempContact.GetVoiceName("temp.wav");
             Assert.IsTrue(res.Success, "Failed to fetch voice name from contact after updating it:" + res);

             res = Contact.GetContact(out _tempContact, _connectionServer, "", _tempContact.Alias);
             Assert.IsTrue(res.Success, "Failed to refetch contact with alias:" + res);

             res = _tempContact.SetVoiceNameToStreamFile("");
             Assert.IsFalse(res.Success, "Call to set voice name to streamFile with empty stream file name did not fail");
         }

         [TestMethod]
        public void ContactFetchTests()
        {
            List<Contact> oContacts;
            WebCallResult res = Contact.GetContacts(_connectionServer, out oContacts);
            Assert.IsTrue(res.Success,"Failed fetching list of contacts");
            Assert.IsTrue(oContacts.Count>0,"No contacts returned from fetch");

            foreach (var oContact in oContacts)
            {
                Console.WriteLine(oContact.ToString());
                Console.WriteLine(oContact.DumpAllProps());
            }

             res = Contact.GetContacts(_connectionServer, out oContacts, 1, 20, "query=(blah is blah)");
             Assert.IsFalse(res.Success,"Fetching contacts with invalid query construction did not fail");

             
             res = Contact.GetContacts(_connectionServer, out oContacts, 1, 20, "query=(objectId is blah)");
             Assert.IsTrue(res.Success, "Fetching contacts with valid query failed:"+res);
             Assert.IsTrue(oContacts.Count==0,"Contacts for invalid objectId query did not return 0:"+res);

        }

        #endregion
    }
}
