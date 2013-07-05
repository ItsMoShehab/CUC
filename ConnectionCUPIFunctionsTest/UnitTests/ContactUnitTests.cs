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
        public void DeleteContact_NullConnectionServer_Failure()
        {
            var res = Contact.DeleteContact(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for null ConnectionServer");
        }

        [TestMethod]
        public void DeleteContact_EmptyObjectId_Failure()
        {
            var res = Contact.DeleteContact(_mockServer, "");
            Assert.IsFalse(res.Success, "Static call to DeleteContact did not return failure for empty objectId");
        }


        [TestMethod]
        public void GetContact_NullConnectionServer_Failure()
        {
            Contact oContact;
            var res = Contact.GetContact(out oContact, null, "bogus");
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for null ConnectionServer");
        }


        [TestMethod]
        public void GetContact_EmptyObjectIdAndAlias_Failure()
        {
            Contact oContact;
            var res = Contact.GetContact(out oContact, _mockServer);
            Assert.IsFalse(res.Success, "Static call to GetContact did not return failure for empty objectId and alias");
        }


        [TestMethod]
        public void GetContactVoiceName_NullConnectionServer_Failure()
        {
            //GetContactVoiceName
            var res = Contact.GetContactVoiceName(null, "boguspath", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for ");
         }


        [TestMethod]
        public void GetContactVoiceName_EmptyWavFilePath_Failure()
        {
            var res = Contact.GetContactVoiceName(_mockServer, "", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for empty local wav path");
        }


        [TestMethod]
        public void GetContactVoiceName_EmptyObjectId_Failure()
        {
            var res = Contact.GetContactVoiceName(_mockServer, "bogus", "", "bogus");
            Assert.IsFalse(res.Success, "Static call to did not return failure for empty objectId");
        }


        [TestMethod]
        public void SetContactVoiceName_NullConnectionServer_Failure()
        {
            //SetContactVoiceName
            var res = Contact.SetContactVoiceName(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for null ConnectionServer");
         }


        [TestMethod]
        public void SetContactVoiceName_EmptyObjectId_Failure()
        {
            var res = Contact.SetContactVoiceName(_mockServer, "bogus", "", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for empty objectId");
        }


        [TestMethod]
        public void SetContactVoiceName_EmptyWavFilePath_Failure()
        {
            var res = Contact.SetContactVoiceName(_mockServer, "", "bogus", true);
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceName did not return failure for empty wav path ");
        }


        [TestMethod]
        public void SetContactVoiceNameToStreamFile_NullConnectionServer_Failure()
        {
            //SetContactVoiceNameToStream
            var res = Contact.SetContactVoiceNameToStreamFile(null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for null ConnectionServer");
         }


        [TestMethod]
        public void SetContactVoiceNameToStreamFile_EmptyObjectId_Failure()
        {
            var res = Contact.SetContactVoiceNameToStreamFile(_mockServer, "", "bogus");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for empty objectId");
        }


        [TestMethod]
        public void SetContactVoiceNameToStreamFile_EmptyWavFilePath_Failure()
        {
            var res = Contact.SetContactVoiceNameToStreamFile(_mockServer, "bogus", "");
            Assert.IsFalse(res.Success, "Static call to SetContactVoiceNameToStreamFile did not return failure for empty Wav resource Id");
        }


        [TestMethod]
        public void GetContacts_NullConnectionServer_Failure()
        {
            List<Contact> oContacts;
            var res = Contact.GetContacts(null, out oContacts);
            Assert.IsFalse(res.Success, "Static call to GetContacts did not return failure for null ConnectionServer");
        }


        [TestMethod]
        public void UpdateContact_NullConnectionServer_Failure()
        {
            var res = Contact.UpdateContact(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for null ConnectionServer");
         }


        [TestMethod]
        public void UpdateContact_NullPropertyList_Failure()
        {
            var res = Contact.UpdateContact(_mockServer, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for null property list");
        }


        [TestMethod]
        public void UpdateContact_EmptyPropertyList_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();

            var res = Contact.UpdateContact(_mockServer, "bogus", oProps);
            Assert.IsFalse(res.Success, "Static call to UpdateContact did not return failure for empty property list");
        }


        [TestMethod]
        public void AddContact_NullConnectionServer_Failure()
        {
            //AddContact
            WebCallResult res = Contact.AddContact(null, "bogus", "bogus", "bogus", "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for null Connection server");
         }


        [TestMethod]
        public void AddContact_EmptyTemplateAlias_Failure()
        {
            var res = Contact.AddContact(_mockServer, "", "bogus", "bogus", "bogus", "bogus", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for empty template alias string ");
        }


        [TestMethod]
        public void AddContact_EmptyAlias_Failure()
        {
            var res = Contact.AddContact(_mockServer, "bogus", "bogus", "bogus", "bogus", "", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for empty alias string");
        }

        #endregion

    }
}
