using System;
using System.Collections.Generic;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ContactTest
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


        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            Contact oTemp = new Contact(null);
        }

        /// <summary>
        /// throw an exception if an invalid objectId is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            Contact oTemp = new Contact(_connectionServer,"bogus");
        }

        /// <summary>
        /// Throw an exception if an invalid alias is passed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure3()
        {
            Contact oTemp = new Contact(_connectionServer,"","bogus");
        }


        /// <summary>
        /// Since a clean system will not have any contacts on it we need to do the add test up front so we can then do 
        /// the fetch tests.
        /// </summary>
        [TestMethod]
        public void ContactAddUpdateDeleteTests()
        {
            Contact oContact;
            
            //create new list with GUID in the name to ensure uniqueness
            String strAlias = "TempContact_" + Guid.NewGuid().ToString().Replace("-", "");

            WebCallResult res = Contact.AddContact(_connectionServer, "systemcontacttemplate", "Test Contact", "Test", "Contact", strAlias, null, out oContact);
            Assert.IsTrue(res.Success, "Failed creating temporary contact:" + res.ToString());

            res = oContact.Update();
            Assert.IsFalse(res.Success,"Calling update on contact without any pending changes did not return an error");

            //Voice name tests
            res = oContact.GetVoiceName("temp.wav");
            Assert.IsFalse(res.Success,"Newly created contact should have no voice name but the fetch call did not fail");

            res = oContact.SetVoiceName("Dummy.wav", true);
            Assert.IsTrue(res.Success,"Updating contact voice name failed:"+res.ToString());

            res = oContact.RefetchContactData();
            Assert.IsTrue(res.Success,"Failed to refetch contact:"+res);

            res = oContact.GetVoiceName("temp.wav");
            Assert.IsTrue(res.Success, "Failed to fetch voice name from contact after updating it:"+res);

            res = Contact.GetContact(out oContact, _connectionServer, "", oContact.Alias);
            Assert.IsTrue(res.Success, "Failed to refetch contact with alias:"+res);

            res =oContact.SetVoiceNameToStreamFile("");
            Assert.IsFalse(res.Success,"Call to set voice name to streamFile with empty stream file name did not fail");

            //update tests
            oContact.DisplayName = "Updated display name";
            oContact.FirstName = "UpdateFirst";
            oContact.LastName = "UpdatedLast";
            oContact.AltFirstName = "altFirst";
            oContact.AltLastName = "altLast";
            oContact.AutoCreateCallHandler = false;
            oContact.ListInDirectory = true;
            oContact.TransferType = 1;
            oContact.TransferRings = 4;
            oContact.TransferEnabled = false;
            oContact.TransferExtension = "";

            
            res = oContact.Update();
            Assert.IsTrue(res.Success,"Updating contact failed:"+res);



            //Do the fetch tests now that we know we have at least one contact
            ContactFetchTests();

            res = oContact.Delete();
            Assert.IsTrue(res.Success, "Failed deleting temporary contact:" + res.ToString());

        }



        [TestMethod]
        public void StaticMethodFailures()
        {
            //AddContact
            WebCallResult res = Contact.AddContact(null, "bogus", "bogus", "bogus", "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for null Connection server");

            res = Contact.AddContact(_connectionServer, "", "bogus", "bogus", "bogus", "bogus", null);
            Assert.IsFalse(res.Success,"Static call to AddContact did not return failure for empty template alias string ");

            res = Contact.AddContact(_connectionServer, "bogus", "bogus", "bogus", "bogus", "", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for empty alias string");

            //DeleteContact
            res = Contact.DeleteContact(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for null ConnectionServer");

            res = Contact.DeleteContact(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for empty objectId");

            res = Contact.DeleteContact(_connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for invalid ObjectID");

            //GetContact
            Contact oContact;
            res = Contact.GetContact(out oContact, null, "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for null ConnectionServer");

            res = Contact.GetContact(out oContact, _connectionServer);
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for empty objectId and alias");

            res = Contact.GetContact(out oContact, _connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for invalid ObjectId");

            res = Contact.GetContact(out oContact, _connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for invalid alias");

            //GetContactVoiceName
            res = Contact.GetContactVoiceName(null, "boguspath", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for ");

            res = Contact.GetContactVoiceName(_connectionServer, "", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for empty local wav path");

            res = Contact.GetContactVoiceName(_connectionServer, "bogus", "", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for empty objectId");

            res = Contact.GetContactVoiceName(_connectionServer, "temp.wav", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for invalid objectId");

            res = Contact.GetContactVoiceName(_connectionServer, "temp.wav", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for invalid objectId");


            //SetContactVoiceName
            res = Contact.SetContactVoiceName(null, "bogus", "bogus", false);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for null ConnectionServer");
            
            res = Contact.SetContactVoiceName(_connectionServer, "bogus", "", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for empty objectId");

            res = Contact.SetContactVoiceName(_connectionServer, "", "bogus", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for empty wav path ");

            res = Contact.SetContactVoiceName(_connectionServer, "Dummy.wav", "bogus", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for invalid ObjectId");


            //SetContactVoiceNameToStream
            res = Contact.SetContactVoiceNameToStreamFile(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for null ConnectionServer");

            res = Contact.SetContactVoiceNameToStreamFile(_connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for empty objectId");

            res = Contact.SetContactVoiceNameToStreamFile(_connectionServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for empty Wav resource Id");

            res = Contact.SetContactVoiceNameToStreamFile(_connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for invalid objectId");


            //GetContacts
            List<Contact> oContacts;
            res = Contact.GetContacts(null, out oContacts);
            Assert.IsFalse(res.Success, "Static call to GetContacts did not return failure for null ConnectionServer");

            res = Contact.GetContacts(_connectionServer, out oContacts,"query=(bogus)","","sort=(bogus)");
            Assert.IsFalse(res.Success, "Static call to GetContacts did not return failure for invalid query construction");

            //Update
            res = Contact.UpdateContact(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for null ConnectionServer");

            res = Contact.UpdateContact(_connectionServer, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for null property list");

            ConnectionPropertyList oProps = new ConnectionPropertyList();

            res = Contact.UpdateContact(_connectionServer, "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for empty property list");

            oProps.Add("bogus","bogus");
            res = Contact.UpdateContact(_connectionServer, "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for invalid objectId");
        }


        private void ContactFetchTests()
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

        }


    }
}
