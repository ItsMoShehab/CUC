using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest
{
     [TestClass]
    public class PortGroupTemplateUnitTests : BaseUnitTests
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

         [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            PortGroupTemplate oTemp = new PortGroupTemplate(null);
            Console.WriteLine(oTemp);
        }

         [TestMethod]
         public void Constructor_EmptyObjectId_Success()
         {
             PortGroupTemplate oTemp = new PortGroupTemplate(_mockServer);
             Console.WriteLine(oTemp.DumpAllProps());
         }

         [TestMethod]
         public void Constructor_Base_Success()
         {
             PortGroupTemplate oTemp = new PortGroupTemplate();
             Console.WriteLine(oTemp.ToString());
         }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void GetPortGroupTemplateObjectId_NullConnectionServer_Failure()
         {
             string strObjectId;
             var res = PortGroupTemplate.GetPortGroupTemplateObjectId(null, TelephonyIntegrationMethodEnum.SCCP, out strObjectId);
             Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplateObjectId did not fail with: null ConnectionServer");
         }

        [TestMethod]
        public void GetPortGroupTemplates_NullConnectionServer_Failure()
        {
            List<PortGroupTemplate> oList;
            WebCallResult res = PortGroupTemplate.GetPortGroupTemplates(null, out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: null ConnectionServer");
        }

        #endregion


        #region Harness Tests

         [TestMethod]
        public void GetPortGroupTemplateObjectId_Success()
         {
             //setup so the fetch for port group templates returns a match on the integration method we're using (PIMG)
             PortGroupTemplate oTemplate = new PortGroupTemplate();
             oTemplate.CopyTelephonyIntegrationMethodEnum = TelephonyIntegrationMethodEnum.PIMG;
             List<PortGroupTemplate> oList = new List<PortGroupTemplate>();
             oList.Add(oTemplate);

             _mockTransport.Setup(x => x.GetObjectsFromJson<PortGroupTemplate>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(oList);

             //make sure all "gets" return true.
             _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "dummy text long enough to be considered legitimate JSON body",
                                        TotalObjectCount = 1
                                    });

             string strObjectId;
             var res = PortGroupTemplate.GetPortGroupTemplateObjectId(_mockServer, TelephonyIntegrationMethodEnum.PIMG,out strObjectId);
             Assert.IsTrue(res.Success,"Fetching port group template for integration method failed:"+res);
         }

         [TestMethod]
         public void GGetPortGroupTemplates_IntegrationMethodNotFound_Failure()
         {
             //setup so the fetch for port group templates returns a match on the integration method we're using (PIMG)
             PortGroupTemplate oTemplate = new PortGroupTemplate();
             oTemplate.CopyTelephonyIntegrationMethodEnum = TelephonyIntegrationMethodEnum.PIMG;
             List<PortGroupTemplate> oList = new List<PortGroupTemplate>();
             oList.Add(oTemplate);

             _mockTransport.Setup(x => x.GetObjectsFromJson<PortGroupTemplate>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(oList);

             //make sure all "gets" return true.
             _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                    It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                    {
                                        Success = true,
                                        ResponseText = "dummy text long enough to be considered legitimate JSON body",
                                        TotalObjectCount = 1
                                    });

             string strObjectId;
             var res = PortGroupTemplate.GetPortGroupTemplateObjectId(_mockServer, TelephonyIntegrationMethodEnum.SCCP, out strObjectId);
             Assert.IsFalse(res.Success, "Fetching port group template for integration method not found should fail");
         }


         [TestMethod]
         public void GetPortGroupTemplates_EmptyResult_Failure()
         {
             _mockTransport.Setup(
                 x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                        It.IsAny<string>(), true)).Returns(new WebCallResult
                                        {
                                            Success = true,
                                            ResponseText = ""
                                        });

             List<PortGroupTemplate> oTemplates;
             var res = PortGroupTemplate.GetPortGroupTemplates(_mockServer, out oTemplates);
             Assert.IsFalse(res.Success, "Calling GetPortGroupTemplates with EmptyResultText did not fail");

         }

         [TestMethod]
         public void GetPortGroupTemplates_GarbageResponse_Failure()
         {
             BaseUnitTests.ClassInitialize(null);

             _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                   It.IsAny<string>(), It.IsAny<bool>())).Returns(new WebCallResult
                                   {
                                       Success = true,
                                       TotalObjectCount = 1,
                                       ResponseText = "garbage result that will not be parsed out as call handler JSON data"
                                   });

             List<PortGroupTemplate> oTemplates;
             var res = PortGroupTemplate.GetPortGroupTemplates(_mockServer, out oTemplates);
             Assert.IsFalse(res.Success, "Calling GetPortGroupTemplates with garbage results should fail");
             Assert.IsTrue(oTemplates.Count == 0, "Invalid result text should produce an empty list");
         }


         [TestMethod]
         public void GetPortGroupTemplates_ErrorResponse_Failure()
         {
             _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                     It.IsAny<string>(), true)).Returns(new WebCallResult
                                     {
                                         Success = false,
                                         ResponseText = "error text",
                                         StatusCode = 404
                                     });
             
             List<PortGroupTemplate> oTemplates;
             var res = PortGroupTemplate.GetPortGroupTemplates(_mockServer, out oTemplates);
             Assert.IsFalse(res.Success, "Calling GetPortGroupTemplates with ErrorResponse did not fail");
         }

         [TestMethod]
         public void GetPortGroupTemplates_ZeroCount_Success()
         {
             _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), MethodType.GET, It.IsAny<ConnectionServerRest>(),
                                     It.IsAny<string>(), true)).Returns(new WebCallResult
                                     {
                                         Success = true,
                                         ResponseText = "junk text",
                                         TotalObjectCount = 0
                                     });

             List<PortGroupTemplate> oTemplates;
             var res = PortGroupTemplate.GetPortGroupTemplates(_mockServer, out oTemplates);
             Assert.IsTrue(res.Success, "Calling GetPortGroupTemplates with ZeroCount failed:" + res);
         }


        #endregion
    }
}
