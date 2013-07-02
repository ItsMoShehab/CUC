using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ContactIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

       private static Contact _tempContact;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            //create new contact with GUID in the name to ensure uniqueness
            String strName = "TempContact_" + Guid.NewGuid().ToString().Replace("-", "");

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
