﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
     [TestClass]
    public class PortGroupTemplateTest
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
            PortGroupTemplate oTemp = new PortGroupTemplate(null);
        }

         /// <summary>
         /// Make sure an invalid ObjectId passed into the constructor fails
         /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            PortGroupTemplate oTemp = new PortGroupTemplate(_connectionServer,"bogus");
        }

        #endregion


        [TestMethod]
        public void StaticMethodFailures()
        {
            List<PortGroupTemplate> oList;
            WebCallResult res = PortGroupTemplate.GetPortGroupTemplates(null, out oList);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplates did not fail with: null ConnectionServer");

            string strObjectId;
            res = PortGroupTemplate.GetPortGroupTemplateObjectId(null,TelephonyIntegrationMethodEnum.SCCP, out strObjectId);
            Assert.IsFalse(res.Success, "Static call to GetPortGroupTemplateObjectId did not fail with: null ConnectionServer");
        }


         [TestMethod]
         public void TestMethods()
         {
             List<PortGroupTemplate> oList;
             WebCallResult res = PortGroupTemplate.GetPortGroupTemplates(_connectionServer, out oList);
             Assert.IsTrue(res.Success, "Static call to GetPortGroupTemplates failed:"+res);
             Assert.IsTrue(oList.Count>0,"No port group templates found");
             
             string strObjectId="";
             foreach (var oTemplate in oList)
             {
                 strObjectId = oTemplate.ObjectId;
                 Console.WriteLine(oTemplate.ToString());
                 Console.WriteLine(oTemplate.DumpAllProps());
             }

             //test creation with objectId
             try
             {
                 PortGroupTemplate oTest = new PortGroupTemplate(_connectionServer, strObjectId);
             }
             catch (Exception ex)
             {
                 Assert.Fail("Failed creating PortGroupTemplate from ObjectId:"+strObjectId);
             }
             
             res = PortGroupTemplate.GetPortGroupTemplateObjectId(_connectionServer, TelephonyIntegrationMethodEnum.SCCP, out strObjectId);
             Assert.IsTrue(res.Success, "Static call to GetPortGroupTemplateObjectId for SCCP failed:"+res);

             res = PortGroupTemplate.GetPortGroupTemplateObjectId(_connectionServer, TelephonyIntegrationMethodEnum.SIP, out strObjectId);
             Assert.IsTrue(res.Success, "Static call to GetPortGroupTemplateObjectId for SIP failed:" + res);

             res = PortGroupTemplate.GetPortGroupTemplateObjectId(_connectionServer, TelephonyIntegrationMethodEnum.PIMG, out strObjectId);
             Assert.IsTrue(res.Success, "Static call to GetPortGroupTemplateObjectId for PIMG failed:" + res);


         }

    }
}
