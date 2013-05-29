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
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private static ClassOfService _tempCos;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
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
                _connectionServer = new ConnectionServer(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                   mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start Class of Service test:" + ex.Message);
            }

            //create new handler with GUID in the name to ensure uniqueness
            String strName = "TempCOS_" + Guid.NewGuid().ToString().Replace("-", "");

            WebCallResult res = ClassOfService.AddClassOfService(_connectionServer, strName, null, out _tempCos);
            Assert.IsTrue(res.Success, "Failed creating temporary class of service:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempCos != null)
            {
                WebCallResult res = _tempCos.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary COS on cleanup.");
            }
        }

        #endregion


        #region Class Creation Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            ClassOfService oTemp = new ClassOfService(null);
            Console.WriteLine(oTemp);
        }

        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure2()
        {
            ClassOfService oTemp = new ClassOfService(null,"bogus");
            Console.WriteLine(oTemp);
        }


        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticMethodFailures_DeleteClassOfService()
        {
            //DeleteClassOfService
            var res = ClassOfService.DeleteClassOfService(null, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: null connectionServer");

            res = ClassOfService.DeleteClassOfService(_connectionServer, "bogus");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: invalid objectid");

            res = ClassOfService.DeleteClassOfService(_connectionServer, "");
            Assert.IsFalse(res.Success, "Static call to DeleteClassOfService did not fail with: empty objectid");
        }

        [TestMethod]
        public void StaticMethodFailures_GetClassOfService()
        {
            //GetClassOfService
            ClassOfService oCos;
            var res = ClassOfService.GetClassOfService(out oCos, null, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: null ConnectionServer");

            res = ClassOfService.GetClassOfService(out oCos, _connectionServer, "bogus", "bogus");
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: invalid ObjectId");

            res = ClassOfService.GetClassOfService(out oCos, _connectionServer);
            Assert.IsFalse(res.Success, "Static call to GetClassOfService did not fail with: empty objectId and Name");
        }

        [TestMethod]
        public void StaticMethodFailures_UpdateClassOfService()
        {
            //GetClassesOfService
            var res = ClassOfService.UpdateClassOfService(null, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: null ConnectionServer");

            res = ClassOfService.UpdateClassOfService(_connectionServer, "bogus", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: invalid objectId");

            res = ClassOfService.UpdateClassOfService(_connectionServer, "", null);
            Assert.IsFalse(res.Success, "Static call to UpdateClassOfService did not fail with: empty objectId");
        }

        [TestMethod]
        public void StaticMethodFailures_GetClassesOfService()
        {
            //GetClassesOfService
            List<ClassOfService> oCoses;
            var res = ClassOfService.GetClassesOfService(null, out oCoses);
            Assert.IsFalse(res.Success, "Static call to GetClassesOfService did not fail with: null ConnectionServer");

            res = ClassOfService.GetClassesOfService(_connectionServer, out oCoses, "query=(bogus)", "", "sort=(bogus)");
            Assert.IsFalse(res.Success, "Static call to GetClassesOfService did not fail with: invalid query construction");
        }

        [TestMethod]
        public void StaticMethodFailures_AddClassOfService()
        {
            //AddClassOfService
            WebCallResult res = ClassOfService.AddClassOfService(null, "display", null);
            Assert.IsFalse(res.Success, "Static call to AddClassOfSerice did not fail with: null ConnectionServer");

            res = ClassOfService.AddClassOfService(_connectionServer, "", null);
            Assert.IsFalse(res.Success, "Static call to AddClassOfSerice did not fail with: empty objectId");

        }

        #endregion


        #region Live Tests

        [TestMethod]
        public void Test_CosFetchTests()
        {
            List<ClassOfService> oCoses;
            WebCallResult res = ClassOfService.GetClassesOfService(_connectionServer, out oCoses);
            Assert.IsTrue(res.Success,"Failed to fetch COSes:"+res);
            Assert.IsNotNull(oCoses,"Empty list returned for classes of service on fetch");
            Assert.IsTrue(oCoses.Count>0,"no classes of service returned on fetch");

            ClassOfService oCos;

            res = ClassOfService.GetClassOfService(out oCos, _connectionServer, oCoses[0].ObjectId);
            Assert.IsTrue(res.Success,"Failed to fetch full COS from ObjectId:"+res);

            Console.WriteLine(oCos.ToString());
            Console.WriteLine(oCos.DumpAllProps());

            Assert.IsFalse(string.IsNullOrEmpty(oCos.FaxRestrictionTable().ObjectId),"No fax restriction table found for COS");
            oCos.FaxRestrictionTable(true);

            Assert.IsFalse(string.IsNullOrEmpty(oCos.OutcallRestrictionTable().ObjectId), "No outcall restriction table found for COS");
            oCos.OutcallRestrictionTable(true);

            Assert.IsFalse(string.IsNullOrEmpty(oCos.TransferRestrictionTable().ObjectId), "No transfer restriction table found for COS");
            oCos.TransferRestrictionTable(true);

            res = oCos.RefetchClassOfServiceData();
            Assert.IsTrue(res.Success,"Failed to refetch COS data:"+res);

            ClassOfService oCos2;
            res = ClassOfService.GetClassOfService(out oCos2, _connectionServer, "", oCos.DisplayName);
            Assert.IsTrue(res.Success,"Faled to fetch COS by display name");

            res = ClassOfService.GetClassesOfService(_connectionServer, out oCoses, 1, 2, "query=(ObjectId is bogus)");
            Assert.IsTrue(res.Success, "fetching COSes with invalid query should not fail:" + res);
            Assert.IsTrue(oCoses.Count == 0, "Invalid query string should return an empty COS list:" + oCoses.Count);
        }

        [TestMethod]
        public void Test_CosUpdateTests()
        {
            WebCallResult res = _tempCos.Update();
            Assert.IsFalse(res.Success, "Calling update to COS instance with no pending changes did not fail");

            _tempCos.CanRecordName = true;
            _tempCos.MaxPrivateDlists = 92;
            res = _tempCos.Update();
            Assert.IsTrue(res.Success, "COS failed to update:" + res);

            _tempCos.FaxRestrictionObjectId = "bogus";
            res = _tempCos.Update();
            Assert.IsFalse(res.Success,"Setting COS fax restriction table to bogus value did not return an error");

            res = _tempCos.RefetchClassOfServiceData();
            Assert.IsTrue(res.Success,"Refetch of data for COS failed:"+res);
            Assert.IsTrue(_tempCos.MaxPrivateDlists==92,"Max list value pulled on refetch does not matched what was set:"+res);
        }

        [TestMethod]
        public void TransferRestrictionTable_FetchTest()
        {
            ClassOfService oTempCos;
            var res = ClassOfService.GetClassOfService(out oTempCos, _connectionServer, _tempCos.ObjectId);
            Assert.IsTrue(res.Success,"Failed to create instance of Cos from valie ObjectId:"+res);

            oTempCos.XferRestrictionObjectId = "Bogus";
            var oTable  = oTempCos.TransferRestrictionTable();
            Assert.IsNull(oTable, "Forcing invalid restriction table fetch should return null restriction table");
        }

        [TestMethod]
        public void FaxRestrictionTable_FetchTest()
        {
            ClassOfService oTempCos;
            var res = ClassOfService.GetClassOfService(out oTempCos, _connectionServer, _tempCos.ObjectId);
            Assert.IsTrue(res.Success, "Failed to create instance of Cos from valie ObjectId:" + res);

            oTempCos.FaxRestrictionObjectId = "Bogus";
            var oTable = oTempCos.FaxRestrictionTable();
            Assert.IsNull(oTable, "Forcing invalid restriction table fetch should return null restriction table");
        }

        [TestMethod]
        public void OutcallRestrictionTable_FetchTest()
        {
            ClassOfService oTempCos;
            var res = ClassOfService.GetClassOfService(out oTempCos, _connectionServer, _tempCos.ObjectId);
            Assert.IsTrue(res.Success, "Failed to create instance of Cos from valie ObjectId:" + res);

            oTempCos.OutcallRestrictionObjectId = "Bogus";
            var oTable = oTempCos.OutcallRestrictionTable();
            Assert.IsNull(oTable, "Forcing invalid restriction table fetch should return null restriction table");
        }


        #endregion


        #region Harness Tests

        [TestMethod]
        public void GetClassesOfService_HarnessTestFailures()
        {
            ConnectionServer oServer = new ConnectionServer(new TestTransportFunctions(), "test", "test", "test");
            List<ClassOfService> oCoses;

            var res = ClassOfService.GetClassesOfService(oServer, out oCoses, 1, 5, "EmptyResultText");
            Assert.IsFalse(res.Success, "Calling GetClassesOfService with EmptyResultText did not fail");

            res = ClassOfService.GetClassesOfService(oServer, out oCoses, 1, 5, "InvalidResultText");
            Assert.IsTrue(res.Success, "Calling GetClassesOfService with InvalidResultText should not fail:" + res);
            Assert.IsTrue(oCoses.Count == 0, "Invalid result text should produce an empty list of Coeses");

            res = ClassOfService.GetClassesOfService(oServer, out oCoses, 1, 5, "ErrorResponse");
            Assert.IsFalse(res.Success, "Calling GetClassesOfService with ErrorResponse did not fail");


           

        }

        #endregion
    }
}
