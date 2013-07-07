using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
     [TestClass]
    public class PortGroupTemplateIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
        }

        #endregion


        #region Class Creation Failures

         /// <summary>
         /// throw UnityConnectionRestException invalid objectId
         /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            PortGroupTemplate oTemp = new PortGroupTemplate(_connectionServer,"bogus");
            Console.WriteLine(oTemp);
        }

        #endregion


         #region Live Tests

         private PortGroupTemplate HelperGetPortGroupTemplate()
         {
             List<PortGroupTemplate> oList;
             WebCallResult res = PortGroupTemplate.GetPortGroupTemplates(_connectionServer, out oList);
             Assert.IsTrue(res.Success, "Static call to GetPortGroupTemplates failed:" + res);
             Assert.IsTrue(oList.Count > 0, "No port group templates found");
             return oList[0];
         }

         [TestMethod]
         public void GetPortGroupTemplates_Success()
         {
             var oTemplate = HelperGetPortGroupTemplate();

             Console.WriteLine(oTemplate.ToString());
             Console.WriteLine(oTemplate.DumpAllProps());
         }


         [TestMethod]
         public void PortGroupTemplate_Constructor_ObjectId_Sucess()
         {
             var oTemplate = HelperGetPortGroupTemplate();

             try
             {
                 PortGroupTemplate oTest = new PortGroupTemplate(_connectionServer, oTemplate.ObjectId);
                 Console.WriteLine(oTest);
             }
             catch (Exception ex)
             {
                 Assert.Fail("Failed creating PortGroupTemplate from ObjectId:" + oTemplate.ObjectId + ", error=" + ex);
             }

         }

         [TestMethod]
         public void GetPortGroupTemplateObjectId_Sccp_Success()
         {
             string strObjectId;
             var res = PortGroupTemplate.GetPortGroupTemplateObjectId(_connectionServer, TelephonyIntegrationMethodEnum.SCCP, out strObjectId);
             Assert.IsTrue(res.Success, "Static call to GetPortGroupTemplateObjectId for SCCP failed:"+res);
         }

         [TestMethod]
         public void GetPortGroupTemplateObjectId_Sip_Success()
         {
             string strObjectId;
             var res = PortGroupTemplate.GetPortGroupTemplateObjectId(_connectionServer, TelephonyIntegrationMethodEnum.SIP, out strObjectId);
             Assert.IsTrue(res.Success, "Static call to GetPortGroupTemplateObjectId for SIP failed:" + res);

         }

         [TestMethod]
         public void GetPortGroupTemplateObjectId_Pimg_Success()
         {
             string strObjectId;
             var res = PortGroupTemplate.GetPortGroupTemplateObjectId(_connectionServer, TelephonyIntegrationMethodEnum.PIMG, out strObjectId);
             Assert.IsTrue(res.Success, "Static call to GetPortGroupTemplateObjectId for PIMG failed:" + res);
         }

        #endregion
    }
}
