using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    /// Cluster is a simple read only class that has very little logic other than it's cosntrctor
    /// </summary>
    [TestClass]
    public class ClusterIntegrationTest : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

       #endregion


        #region Additional test attributes
        // 
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);
        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void ClusterConstructor_Fetch_Success()
        {
            Cluster oCluster = new Cluster(_connectionServer);

            Console.WriteLine(oCluster.ToString());

            Assert.IsTrue(oCluster.Servers.Count>0,"There should always be at least 1 server in a cluster");

            foreach (var oServer in oCluster.Servers)
            {
                Console.WriteLine(oServer.ToString());
                Console.WriteLine(oServer.DumpAllProps());
                
            }
        }

        #endregion
    }
}
