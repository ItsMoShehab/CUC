using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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


        #region Construction Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            Contact oTemp = new Contact(null);
            Console.WriteLine(oTemp);
        }

        [TestMethod]
        public void Constructor_EmptyObjectIdAndAlias_Success()
        {
            Contact oTemp = new Contact(_mockServer);
            Console.WriteLine(oTemp.DumpAllProps());
            Console.WriteLine(oTemp.ToString());
            Console.WriteLine(oTemp.SelectionDisplayString);
            Console.WriteLine(oTemp.UniqueIdentifier);
        }

        [TestMethod]
        public void Constructor_Default_Success()
        {
            Contact oTemp = new Contact();
        }

        [TestMethod]
        [ExpectedException(typeof (UnityConnectionRestException))]
        public void Constructor_AliasNotFound_Failure()
        {
            Contact oTemp = new Contact(_mockServer, "", "Alias");
            Console.WriteLine(oTemp);
        }

       
        #endregion


        #region Static Call Tests

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
            var res = Contact.GetContacts(null, out oContacts,1,10,null);
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
            Contact oContact;
            var res = Contact.AddContact(_mockServer, "", "bogus", "bogus", "bogus", "bogus", null,out oContact);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for empty template alias string ");
        }


        [TestMethod]
        public void AddContact_EmptyAlias_Failure()
        {
            var res = Contact.AddContact(_mockServer, "bogus", "bogus", "bogus", "bogus", "", null);
            Assert.IsFalse(res.Success, "Static call to AddContact did not return failure for empty alias string");
        }

        #endregion


        #region Property Tests

        [TestMethod]
        public void PropertyGetFetch_AltFirstName()
        {
            Contact oContact = new Contact();
            const string expectedValue = "Test string";
            oContact.AltFirstName = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("AltFirstName", expectedValue),"AltFirstName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_AltLastName()
        {
            Contact oContact = new Contact();
            const string expectedValue = "Test string";
            oContact.AltLastName = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("AltLastName", expectedValue), "AltLastName value get fetch failed");
        }
        
        [TestMethod]
        public void PropertyGetFetch_AutoCreateCallHandler()
        {
            Contact oContact = new Contact();
            const bool expectedValue = false;
            oContact.AutoCreateCallHandler = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("AutoCreateCallHandler", expectedValue), "AutoCreateCallHandler value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_DisplayName()
        {
            Contact oContact = new Contact();
            const string expectedValue = "Test string";
            oContact.DisplayName = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("DisplayName", expectedValue), "DisplayName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_FirstName()
        {
            Contact oContact = new Contact();
            const string expectedValue = "Test string";
            oContact.FirstName = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("FirstName", expectedValue), "FirstName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_LastName()
        {
            Contact oContact = new Contact();
            const string expectedValue = "Test string";
            oContact.LastName = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("LastName", expectedValue), "LastName value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_ListInDirectory()
        {
            Contact oContact = new Contact();
            const bool expectedValue = true;
            oContact.ListInDirectory = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("ListInDirectory", expectedValue), "ListInDirectory value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_PartitionObjectId()
        {
            Contact oContact = new Contact();
            const string expectedValue = "Test string";
            oContact.PartitionObjectId = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("PartitionObjectId", expectedValue), "PartitionObjectId value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferType()
        {
            Contact oContact = new Contact();
            const TransferTypes expectedValue = TransferTypes.Supervised;
            oContact.TransferType = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("TransferType", (int)expectedValue), "TransferType value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferRings()
        {
            Contact oContact = new Contact();
            const int expectedValue = 5;
            oContact.TransferRings = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("TransferRings", expectedValue), "TransferRings value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferEnabled()
        {
            Contact oContact = new Contact();
            const bool expectedValue = false;
            oContact.TransferEnabled = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("TransferEnabled", expectedValue), "TransferEnabled value get fetch failed");
        }

        [TestMethod]
        public void PropertyGetFetch_TransferExtension()
        {
            Contact oContact = new Contact();
            const string expectedValue = "Test string";
            oContact.TransferExtension = expectedValue;
            Assert.IsTrue(oContact.ChangeList.ValueExists("TransferExtension", expectedValue), "TransferExtension value get fetch failed");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetContacts_EmptyResult_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                       {
                                           Success = true,
                                           ResponseText = ""
                                       });

            List<Contact> oContacts;
            var res = Contact.GetContacts(_mockServer, out oContacts, 1, 5, "");
            Assert.IsFalse(res.Success, "Calling GetContacts with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetContacts_GarbageResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      TotalObjectCount = 1,
                                      ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                  });

            List<Contact> oContacts;
            var res = Contact.GetContacts(_mockServer, out oContacts, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetContacts with garbage results should fail");
            Assert.IsTrue(oContacts.Count == 0, "Invalid result text should produce an empty list");
        }


        [TestMethod]
        public void GetContacts_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            List<Contact> oContacts;
            var res = Contact.GetContacts(_mockServer, out oContacts, 1, 5, null);
            Assert.IsFalse(res.Success, "Calling GetContacts with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetContacts_ZeroCount_Success()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "junk text",
                                        TotalObjectCount = 0
                                    });

            List<Contact> oContacts;
            var res = Contact.GetContacts(_mockServer, out oContacts, 1, 5, null);
            Assert.IsTrue(res.Success, "Calling GetContacts with ZeroCount failed:" + res);
        }


        [TestMethod]
        public void GetContact_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            Contact oContact;
            var res = Contact.GetContact(out oContact, _mockServer, "","Alias");
            Assert.IsFalse(res.Success, "Calling GetContact with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetContact_GarbageResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "Response text that will not be parsed as JSON for a contact",
                                        TotalObjectCount = 1
                                    });
            Contact oContact;
            var res = Contact.GetContact(out oContact, _mockServer, "ObjectId");
            Assert.IsFalse(res.Success, "Calling GetContact with Garbage response body did not fail");
        }

        [TestMethod]
        public void AddContact_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.POST, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            Contact oContact;
            var res = Contact.AddContact(_mockServer, "TemplateAlias", "Display Name","First Name","Last Name","Alias",null,out oContact);
            Assert.IsFalse(res.Success, "Calling AddContact with ErrorResponse did not fail");
        }

        [TestMethod]
        public void DeleteContact_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.DELETE, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            var res = Contact.DeleteContact(_mockServer, "ObjectId");
            Assert.IsFalse(res.Success, "Calling DeleteContact with ErrorResponse did not fail");
        }

        [TestMethod]
        public void UpdateContact_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.PUT, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            oProps.Add("Test","test");
            var res = Contact.UpdateContact(_mockServer, "ObjectId",oProps);
            Assert.IsFalse(res.Success, "Calling UpdateContact with ErrorResponse did not fail");
        }

        [TestMethod]
        public void GetContactVoiceName_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), true)).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });
            var res = Contact.GetContactVoiceName(_mockServer, "ObjectId", "dummy.wav");
            Assert.IsFalse(res.Success, "Calling GetContactVoiceName with ErrorResponse did not fail");
        }

        [TestMethod]
        public void SetContactVoiceName_ErrorResponse_Failure()
        {
            
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = Contact.SetContactVoiceName(_mockServer, "Dummy.wav", "ObjectId",true);
            Assert.IsFalse(res.Success, "Calling SetContactVoiceName with ErrorResponse did not fail");
        }

        [TestMethod]
        public void SetContactVoiceName_InvalidWavFile_Failure()
        {
            BaseUnitTests.Reset();
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "dummy text",
                                    });

            var res = Contact.SetContactVoiceName(_mockServer, "moq.dll", "ObjectId", true);
            Assert.IsFalse(res.Success, "Calling SetContactVoiceName with invalid wav file did not fail");
        }

        [TestMethod]
        public void SetContactVoiceNameToStreamFile_ErrorResponse_Failure()
        {
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = false,
                                        ResponseText = "error text",
                                        StatusCode = 404
                                    });

            var res = Contact.SetContactVoiceNameToStreamFile(_mockServer, "ObjectId", "StreamFileName");
            Assert.IsFalse(res.Success, "Calling SetContactVoiceNameToStreamFile with ErrorResponse did not fail");
        }

        [TestMethod]
        public void RefetchContactData_EmptyClass_Failure()
        {
            Contact oContact = new Contact(_mockServer);
            var res = oContact.RefetchContactData();
            Assert.IsFalse(res.Success, "Calling RefetchContactData from empty class instance should fail");
        }

        [TestMethod]
        public void Update_NoPendingChanges_Failure()
        {
            Contact oContact = new Contact(_mockServer);
            var res = oContact.Update();
            Assert.IsFalse(res.Success, "Calling Update with no pending changes should fail");
        }

        [TestMethod]
        public void Update_ErrorResponse_Failure()
        {
            BaseUnitTests.Reset();
            Contact oContact = new Contact(_mockServer);

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                   {
                                       Success = false,
                                       ResponseText = "error text",
                                       StatusCode = 404
                                   });

            oContact.DisplayName = "New Display Name";
            var res = oContact.Update();
            Assert.IsFalse(res.Success, "Calling Update with error response should fail");
        }


        [TestMethod]
        public void Delete_EmptyClass_Failure()
        {
            Contact oContact = new Contact(_mockServer);
            var res = oContact.Delete();
            Assert.IsFalse(res.Success, "Calling Delete from empty class instance should fail");
        }

        [TestMethod]
        public void SetVoiceName_EmptyClass_Failure()
        {
            Contact oContact = new Contact(_mockServer);
            var res = oContact.SetVoiceName("Dummy.wav",false);
            Assert.IsFalse(res.Success, "Calling SetVoiceName from empty class instance should fail");
        }

        [TestMethod]
        public void SetVoiceNameToStreamFile_EmptyClass_Failure()
        {
            Contact oContact = new Contact(_mockServer);
            var res = oContact.SetVoiceNameToStreamFile("StreamFileName");
            Assert.IsFalse(res.Success, "Calling SetVoiceNameToStreamFile from empty class instance should fail");
        }

        [TestMethod]
        public void GetVoiceName_EmptyClass_Failure()
        {
            Contact oContact = new Contact(_mockServer);
            var res = oContact.GetVoiceName("temp.wav");
            Assert.IsFalse(res.Success, "Calling GetVoiceName from empty class instance should fail");
        }

        #endregion
    }
}
