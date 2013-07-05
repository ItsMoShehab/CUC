using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace ConnectionCUPIFunctionsTest
{

    /// <summary>
    ///This is a test class for CallHandlerTest and is intended
    ///to contain all CallHandlerTest Unit Tests
    ///</summary>
   [TestClass]
    public class CallHandlerTemplateUnitTest : BaseUnitTests 
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


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CallHandlerTemplate_ClassCreationFailure()
        {
            CallHandlerTemplate oTestTemplate = new CallHandlerTemplate(null, "aaa");
            Console.WriteLine(oTestTemplate);
        }


        #endregion


        #region Static Call Failures

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void GetCallHandlerTemplates_NullConnectionServer_Failure()
        {
            List<CallHandlerTemplate> oTemplates;
            WebCallResult res = CallHandlerTemplate.GetCallHandlerTemplates(null, out oTemplates,1,10,null);
            Assert.IsFalse(res.Success, "Passing null connection server should fail.");
        }

        [TestMethod]
        public void AddCallHandlerTemplate_NullConnectionServer_Failure()
        {
            CallHandlerTemplate oTemplate;
            var res = CallHandlerTemplate.AddCallHandlerTemplate(null, "display name", "mediaswitchobjectid", "recipientlist","recipientuser", 
                null, out oTemplate);
            Assert.IsFalse(res.Success,"AddCallHandlerTemplate with null ConnectionServerRest did not fail");
         }

        [TestMethod]
        public void AddCallHandlerTemplate_EmptyDisplayName_Failure()
        {
            CallHandlerTemplate oTemplate; 
            var res = CallHandlerTemplate.AddCallHandlerTemplate(_mockServer, "", "mediaswitchobjectid", "recipientlist", "recipientuser", null, out oTemplate);
            Assert.IsFalse(res.Success, "AddCallHandlerTemplate with empty display name did not fail");
            }

        [TestMethod]
        public void AddCallHandlerTemplate_EmptyMediaSwitchId_Failure()
        {
            CallHandlerTemplate oTemplate; 
            var res = CallHandlerTemplate.AddCallHandlerTemplate(_mockServer, "displayname", "", "recipientlist", "recipientuser", null, out oTemplate);
            Assert.IsFalse(res.Success, "AddCallHandlerTemplate with empty mediaswitch objectId did not fail");
        }

        [TestMethod]
        public void AddCallHandlerTemplate_EmptyRecipients_Failure()
        {
            CallHandlerTemplate oTemplate; 
            var res = CallHandlerTemplate.AddCallHandlerTemplate(_mockServer, "displayname", "mediaswitchobjectid", "", "", null, out oTemplate);
            Assert.IsFalse(res.Success, "AddCallHandlerTemplate with no recipients did not fail");
        }



        [TestMethod]
        public void DeleteCallHandlerTemplate_NullconnectionServer_Failure()
        {
            var res = CallHandlerTemplate.DeleteCallHandlerTemplate(null, "objectid");
            Assert.IsFalse(res.Success, "DeleteCallHandlerTemplate with null connection server did not fail");
        }

        [TestMethod]
        public void DeleteCallHandlerTemplate_EmptyObjectId_Failure()
        {
            var res = CallHandlerTemplate.DeleteCallHandlerTemplate(_mockServer, "");
            Assert.IsFalse(res.Success, "DeleteCallHandlerTemplate with empty objectid did not fail");
        }


        [TestMethod]
        public void GetCallHandlerTemplate_NullConnectionServer_Failure()
        {
            CallHandlerTemplate oTemplate;
            var res= CallHandlerTemplate.GetCallHandlerTemplate(out oTemplate, null, "objectid", "displayname");
            Assert.IsFalse(res.Success, "GetCallHandlerTemplate with null ConnectionServerRest did not fail");
        }


        [TestMethod]
        public void GetCallHandlerTemplate_EmptyObjectId_Failure()
        {
            CallHandlerTemplate oTemplate;
            var res = CallHandlerTemplate.GetCallHandlerTemplate(out oTemplate, _mockServer, "", "");
            Assert.IsFalse(res.Success, "GetCallHandlerTemplate with empty objectId and name did not fail");
         }


        [TestMethod]
        public void UpdateCallHandlerTemplate_NullConnectionServer_Failure()
        {
            var res= CallHandlerTemplate.UpdateCallHandlerTemplate(null, "objectId", null);
            Assert.IsFalse(res.Success, "UpdateCallHandlerTemplate with null Connection server did not fail");
         }


        [TestMethod]
        public void UpdateCallHandlerTemplate_EmptyObjectId_Failure()
        {
            var res = CallHandlerTemplate.UpdateCallHandlerTemplate(_mockServer, "", null);
            Assert.IsFalse(res.Success, "UpdateCallHandlerTemplate with empty object did not fail");
        }


        [TestMethod]
        public void UpdateCallHandlerTemplate_EmptyPropertyList_Failure()
        {
            ConnectionPropertyList oProps = new ConnectionPropertyList();
            var res = CallHandlerTemplate.UpdateCallHandlerTemplate(_mockServer, "objectId", oProps);
            Assert.IsFalse(res.Success, "UpdateCallHandlerTemplate with empty property list did not fail");
        }

        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetCallHandlerTemplates_EmptyResults_Failure()
        {
            //empty results
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                       It.IsAny<string>(), true)).Returns(new WebCallResult
                                           {
                                               Success = true,
                                               ResponseText = ""
                                           });

            List<CallHandlerTemplate> oTemplates;
            var res = CallHandlerTemplate.GetCallHandlerTemplates(_mockServer, out oTemplates, 1, 5, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetCallHandlerTemplates with EmptyResultText did not fail");
        }

        [TestMethod]
        public void GetCallHandlerTemplates_GarbageResults_Success()
        {
            //garbage response
           _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                 It.IsAny<string>(), true)).Returns(new WebCallResult
                                 {
                                     Success = true,
                                     ResponseText = "garbage result"
                                 });

           List<CallHandlerTemplate> oTemplates;
           var res = CallHandlerTemplate.GetCallHandlerTemplates(_mockServer, out oTemplates, 1, 5, "InvalidResultText");
           Assert.IsTrue(res.Success, "Calling GetCallHandlerTemplates with InvalidResultText should not fail:"+res);
           Assert.IsTrue(oTemplates.Count==0,"Invalid result text should produce an empty list of templates");
       }


        [TestMethod]
        public void GetCallHandlerTemplates_ErrorResult_Failure()
        {
            //error response
           _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), true)).Returns(new WebCallResult
                                   {
                                       Success = false,
                                       ResponseText = "error text",
                                       StatusCode = 404
                                   });

           List<CallHandlerTemplate> oTemplates;
           var res = CallHandlerTemplate.GetCallHandlerTemplates(_mockServer, out oTemplates, 1, 5, "ErrorResponse");
           Assert.IsFalse(res.Success, "Calling GetCallHandlerTemplates with ErrorResponse did not fail");
       }


        [TestMethod]
        public void AddCallHandlerTemplate_JunkObjectIdReturn_Failure()
        {
            //invalid objectId response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                 It.IsAny<string>(), true)).Returns(new WebCallResult
                                 {
                                     Success = true,
                                     ResponseText = "/vmrest/callhandlertemplates/junk"
                                 });

            CallHandlerTemplate oTemplate;
            var res = CallHandlerTemplate.AddCallHandlerTemplate(_mockServer, "test", "test", "", "test", null,out oTemplate);
            Assert.IsFalse(res.Success,"AddCallHandlerTemplate that produces invalid new ObjectId did not fail");
        }

        [TestMethod]
        public void AddCallHandlerTemplate_EmptyResponse_Failure()
        {
            //invalid objectId response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                 It.IsAny<string>(), true)).Returns(new WebCallResult
                                 {
                                     Success = true,
                                     ResponseText = ""
                                 });

            CallHandlerTemplate oTemplate;
            var res = CallHandlerTemplate.AddCallHandlerTemplate(_mockServer, "test", "test", "", "test", null, out oTemplate);
            Assert.IsFalse(res.Success, "AddCallHandlerTemplate that returns empty response did not fail");
        }
        [TestMethod]
        public void AddCallHandlerTemplate_GarbageResponse_Failure()
        {
            //invalid objectId response
            //garbage response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(),It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                  It.IsAny<string>(), true)).Returns(new WebCallResult
                                  {
                                      Success = true,
                                      ResponseText = "garbage result"
                                  });

            CallHandlerTemplate oTemplate;
            var res = CallHandlerTemplate.AddCallHandlerTemplate(_mockServer, "test", "test", "", "test", null, out oTemplate);
            Assert.IsFalse(res.Success, "AddCallHandlerTemplate that produces garbage response text did not fail");
        }

        [TestMethod]
        public void AddCallHandlerTemplate_ErrorResult_Failure()
        {
            //invalid objectId response
            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                 It.IsAny<string>(), true)).Returns(new WebCallResult
                                 {
                                     Success = false,
                                     StatusCode = 404,
                                 });

            CallHandlerTemplate oTemplate;
            var res = CallHandlerTemplate.AddCallHandlerTemplate(_mockServer, "test", "test", "", "test", null, out oTemplate);
            Assert.IsFalse(res.Success, "AddCallHandlerTemplate that produces error response did not fail");
        }

        #endregion
    }
}