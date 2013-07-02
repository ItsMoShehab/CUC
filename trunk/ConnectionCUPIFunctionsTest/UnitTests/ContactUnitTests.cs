using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ContactUnitTests : BaseUnitTests
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

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticMethodFailure_DeleteContact()
        {
            //DeleteContact
            var res = Contact.DeleteContact(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for null ConnectionServer");

            res = Contact.DeleteContact(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for empty objectId");

        }

        [TestMethod]
        public void StaticMethodFailure_GetContact()
        {
            //GetContact
            Contact oContact;
            var res = Contact.GetContact(out oContact, null, "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for null ConnectionServer");

            res = Contact.GetContact(out oContact, _mockServer);
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for empty objectId and alias");

            res = Contact.GetContact(out oContact, _mockServer, "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for invalid ObjectId");

            res = Contact.GetContact(out oContact, _mockServer, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for invalid alias");
        }

        [TestMethod]
        public void StaticMethodFailure_GetContactVoiceName()
        {
            //GetContactVoiceName
            var res = Contact.GetContactVoiceName(null, "boguspath", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for ");

            res = Contact.GetContactVoiceName(_mockServer, "", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for empty local wav path");

            res = Contact.GetContactVoiceName(_mockServer, "bogus", "", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for empty objectId");

            res = Contact.GetContactVoiceName(_mockServer, "temp.wav", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for invalid objectId");

            res = Contact.GetContactVoiceName(_mockServer, "temp.wav", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for invalid objectId");
        }

        [TestMethod]
        public void StaticMethodFailure_SetContactVoiceName()
        {
            //SetContactVoiceName
            var res = Contact.SetContactVoiceName(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for null ConnectionServer");

            res = Contact.SetContactVoiceName(_mockServer, "bogus", "", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for empty objectId");

            res = Contact.SetContactVoiceName(_mockServer, "", "bogus", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for empty wav path ");

            res = Contact.SetContactVoiceName(_mockServer, "Dummy.wav", "bogus", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for invalid ObjectId");
        }

        [TestMethod]
        public void StaticMethodFailure_SetContactVoiceNameToStream()
        {
            //SetContactVoiceNameToStream
            var res = Contact.SetContactVoiceNameToStreamFile(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for null ConnectionServer");

            res = Contact.SetContactVoiceNameToStreamFile(_mockServer, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for empty objectId");

            res = Contact.SetContactVoiceNameToStreamFile(_mockServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for empty Wav resource Id");

            res = Contact.SetContactVoiceNameToStreamFile(_mockServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for invalid objectId");
        }

        [TestMethod]
        public void StaticMethodFailure_GetContacts()
        {
            //GetContacts
            List<Contact> oContacts;
            var res = Contact.GetContacts(null, out oContacts);
            Assert.IsFalse(res.Success, "Static call to GetContacts did not return failure for null ConnectionServer");

        }

        [TestMethod]
        public void StaticMethodFailure_Update()
        {
            //Update
            var res = Contact.UpdateContact(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for null ConnectionServer");

            res = Contact.UpdateContact(_mockServer, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for null property list");

            ConnectionPropertyList oProps = new ConnectionPropertyList();

            res = Contact.UpdateContact(_mockServer, "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for empty property list");

            oProps.Add("bogus", "bogus");
            res = Contact.UpdateContact(_mockServer, "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for invalid objectId");

        }

        [TestMethod]
        public void StaticMethodFailure_AddContact()
        {
            //AddContact
            WebCallResult res = Contact.AddContact(null, "bogus", "bogus", "bogus", "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for null Connection server");

            res = Contact.AddContact(_mockServer, "", "bogus", "bogus", "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for empty template alias string ");

            res = Contact.AddContact(_mockServer, "bogus", "bogus", "bogus", "bogus", "", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for empty alias string");
        }

        #endregion

    }
}
