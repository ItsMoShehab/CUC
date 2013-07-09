using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for SmppProviderUnitTests and is intended
    ///to contain all SmppProviderUnitTests Unit Tests
    ///</summary>
    [TestClass]
    public class SmppProviderIntegrationTests : BaseIntegrationTests
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
        

        #region Constructor Tests

        /// <summary>
        /// Make sure an UnityConnectionRestException is thrown if the objectId for an SMPP provider is not found
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            SmppProvider oTest = new SmppProvider(_connectionServer, "objectId");
            Console.WriteLine(oTest);
        }

        #endregion


        #region Live Tests

        /// <summary>
        ///A test for getting and listing PhoneSystems
        ///</summary>
        [TestMethod]
        public void SmppProvider_FetchTest()
        {
            List<SmppProvider> oProviders;

            WebCallResult res = SmppProvider.GetSmppProviders(null, out oProviders,1,5);
            Assert.IsFalse(res.Success, "Null Connection server param should fail");

            res = SmppProvider.GetSmppProviders(_connectionServer, out oProviders,1,5,null);
            Assert.IsTrue(res.Success, "Failed to fetch SmppProviders");
            Assert.IsTrue(oProviders.Count>0,"No SMPP providers returned");

            string strObjectId="";
            foreach (SmppProvider oTemp in oProviders)
            {
                Console.WriteLine(oTemp.ToString());

                //check IUnityDisplayInterface properties
                Console.WriteLine(oTemp.SelectionDisplayString);
                Console.WriteLine(oTemp.UniqueIdentifier);
                strObjectId = oTemp.ObjectId;
            }

            try
            {
                SmppProvider oNewProvider = new SmppProvider(_connectionServer, strObjectId);
                Console.WriteLine(oNewProvider);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unable to fetch SMPP provider by valid ObjectId:"+ex);
            }

            

        }

        [TestMethod]
        public void SmppProvider_CreateEmpty()
        {
            try
            {
                SmppProvider oProvider = new SmppProvider(_connectionServer);
                Console.WriteLine(oProvider);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create an empty ObjectInstance with no ObjectId:"+ex);
            }
        }

        #endregion
    }
}
