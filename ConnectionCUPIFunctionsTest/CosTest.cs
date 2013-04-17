using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class CosTest
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
                HTTPFunctions.DebugMode = mySettings.DebugOn;
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
            ClassOfService oTemp = new ClassOfService(null);
        }

        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            ClassOfService oTemp = new ClassOfService(null,"bogus");
        }


        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            List<ClassOfService> oCoses;
            WebCallResult res = ClassOfService.GetClassesOfService(_connectionServer, out oCoses);
            Assert.IsTrue(res.Success,"Failed to fetch COSes:"+res);

            foreach (var oTempCos in oCoses)
            {
                Console.WriteLine(oTempCos.ToString());
                Console.WriteLine(oTempCos.DumpAllProps());
            }

            ClassOfService oCos;
            res = ClassOfService.AddClassOfService(_connectionServer, "TestCOS"+Guid.NewGuid(), null,out oCos);
            Assert.IsTrue(res.Success,"Failed to create new COS:"+res);

            res = oCos.Update();
            Assert.IsFalse(res.Success,"Calling update to COS instance with no pending changes did not fail");

            oCos.CanRecordName = true;
            oCos.MaxPrivateDlists = 92;
            res = oCos.Update();
            Assert.IsTrue(res.Success,"COS failed to update:"+res);

            oCos.ClearPendingChanges();

            oCos.FaxRestrictionTable();
            oCos.FaxRestrictionTable(true);

            oCos.OutcallRestrictionTable();
            oCos.OutcallRestrictionTable(true);

            oCos.TransferRestrictionTable();
            oCos.TransferRestrictionTable(true);

            res = oCos.RefetchClassOfServiceData();
            Assert.IsTrue(res.Success,"Failed to refetch COS data:"+res);

            ClassOfService oCos2;
            res = ClassOfService.GetClassOfService(out oCos2, _connectionServer, "", oCos.DisplayName);

            res = oCos.Delete();
            Assert.IsTrue(res.Success,"Failed to delete COS:"+res);
        }


        [TestMethod]
        public void StaticMethodFailures()
        {
            //AddClassOfService
            WebCallResult res = ClassOfService.AddClassOfService(null, "display", null);
            res = ClassOfService.AddClassOfService(_connectionServer, "", null);
            Assert.IsFalse(res.Success,"Static call to AddClassOfSerice did not fail with: null ConnectionServer");
            
            //DeleteClassOfService
            res = ClassOfService.DeleteClassOfService(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: null ConnectionServer");

            res = ClassOfService.DeleteClassOfService(_connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: invalid objectid");

            res = ClassOfService.DeleteClassOfService(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: empty objectid");

            //GetClassOfService
            ClassOfService oCos;
            res = ClassOfService.GetClassOfService(out oCos, null, "bogus","bogus");
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: null ConnectionServer");

            res = ClassOfService.GetClassOfService(out oCos, _connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: invalid ObjectId");
            
            res = ClassOfService.GetClassOfService(out oCos, _connectionServer, "");
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: empty objectId and Name");

            //GetClassesOfService
            res = ClassOfService.UpdateClassOfService(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: null ConnectionServer");

            res = ClassOfService.UpdateClassOfService(_connectionServer, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: invalid objectId");

            res = ClassOfService.UpdateClassOfService(_connectionServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: empty objectId");

            //GetClassesOfService
            List<ClassOfService> oCoses;
            res = ClassOfService.GetClassesOfService(null, out oCoses);
            Assert.IsFalse(res.Success, "Static call to GetClassesOfService did not fail with: null ConnectionServer");

            res = ClassOfService.GetClassesOfService(_connectionServer, out oCoses,"query=(bogus)","","sort=(bogus)");
            Assert.IsFalse(res.Success, "Static call to GetClassesOfService did not fail with: invalid query construction");
        }

    }
}
