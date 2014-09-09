using System;
using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ServerResponseJsonParsingTestsTenants : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties
        
        //used for editing/adding items to a temporary user that gets cleaned up after the tests are complete
        private static Tenant _tempTenant;
        
        private static string _errorString;

        #endregion


        #region Additional test attributes

        private static void ServerOnErrorEvents(object sender, ConnectionServerRest.LogEventArgs logEventArgs)
        {
            Console.WriteLine(logEventArgs.Line);
            _errorString += logEventArgs.Line;
        }

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            _connectionServer.ErrorEvents += ServerOnErrorEvents;

            string strAlias = "Tenant" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 13);
            var res = Tenant.AddTenant(_connectionServer, strAlias, strAlias + ".org", strAlias, "","",out _tempTenant);
            Assert.IsTrue(res.Success, "Failed to create temproary teannt:" + res);
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempTenant != null)
            {
                var res = _tempTenant.Delete();
                Assert.IsTrue(res.Success,"Failed to delete temporary tenant on cleanup");
            }
        }

        #endregion


        [TestMethod]
        public void Tenant_Test()
        {
            _errorString = "";
            List<Tenant> oTenants;
            var res = Tenant.GetTenants(_connectionServer, out oTenants);
            Assert.IsTrue(res.Success & oTenants.Count > 0, "Failed to fetch Tenants:" + res);
            Assert.IsTrue(string.IsNullOrEmpty(_errorString), _errorString);
        }
    }
}
