using System;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for HTTPFunctionsTest and is intended
    ///to contain all HTTPFunctionsTest Unit Tests
    ///</summary>
    [TestClass]
    public class HttpFunctionsTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static DirectoryHandler _tempHandler;

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
                _connectionServer = new ConnectionServer(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start DirectoryHandler test:" + ex.Message);
            }

            //create new handler with GUID in the name to ensure uniqueness
            String strName = "TempHandler_" + Guid.NewGuid().ToString().Replace("-", "");

            WebCallResult res = DirectoryHandler.AddDirectoryHandler(_connectionServer, strName, false, null, out _tempHandler);
            Assert.IsTrue(res.Success, "Failed creating temporary directory handler:" + res.ToString());
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempHandler != null)
            {
                WebCallResult res = _tempHandler.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary directory handler on cleanup.");
            }
        }

        #endregion



        /// <summary>
        ///Test for WAV upload call failures.
        /// Note that "Dummy.wav" does exist in the output folder the test is being run from.
        ///</summary>
        [TestMethod]
        public void UploadWavFile_Failure()
                {
            WebCallResult res = _connectionServer.UploadWavFile("bogusresourcepath", "Dummy.wav");
                    Assert.IsFalse(res.Success,"Invalid resource path should fail");

                    res = _connectionServer.UploadWavFile("", "Dummy.wav");
                    Assert.IsFalse(res.Success, "Empty resource path should fail");

                    res = _connectionServer.UploadWavFile("bogusresourcepath", "");
                    Assert.IsFalse(res.Success, "File path that does not exist should fail");
                }

         [TestMethod]
         public void HttpFunctions_JsonParseTest()
         {
             const string strRawJson = "{\"URI\":\"/vmrest/coses/7216f133-92df-4ec3-8ad3-bb9fcddfa4cf\",\"ObjectId\":\"7216f133-92df-4ec3-8ad3-bb9fcddfa4cf\",\"AccessFaxMail\":\"false\",\"AccessTts\":\"true\",\"CallHoldAvailable\":\"false\",\"CallScreenAvailable\":\"false\",\"CanRecordName\":\"true\",\"FaxRestrictionObjectId\":\"4fd85d55-9003-421b-994a-5d3e9160109a\",\"ListInDirectoryStatus\":\"true\",\"LocationObjectId\":\"3b426c5d-e90a-45fc-90e1-4ab66ff1fd7e\",\"LocationURI\":\"/vmrest/locations/connectionlocations/3b426c5d-e90a-45fc-90e1-4ab66ff1fd7e\",\"MaxGreetingLength\":\"23\",\"MaxMsgLength\":\"300\",\"MaxNameLength\":\"30\",\"MaxPrivateDlists\":\"25\",\"MovetoDeleteFolder\":\"true\",\"OutcallRestrictionObjectId\":\"c3bbac2e-48d5-4586-9ad2-42dc2ebea7ad\",\"PersonalAdministrator\":\"true\",\"DisplayName\":\"TempCOS_8550a3a5d9644202a59d5daff4b96e3a\",\"XferRestrictionObjectId\":\"8dcf152b-9705-4097-b06c-781dbbdf9b24\",\"Undeletable\":\"false\",\"WarnIntervalMsgEnd\":\"0\",\"CanSendToPublicDl\":\"true\",\"EnableEnhancedSecurity\":\"false\"}";

             ClassOfService oCos = _connectionServer.GetObjectFromJson<ClassOfService>(strRawJson);

             Assert.IsNotNull(oCos, "COS parsing failed");
             Assert.IsTrue(oCos.ObjectId.Equals("7216f133-92df-4ec3-8ad3-bb9fcddfa4cf"), "COS objectId property did not match");
             Assert.IsFalse(oCos.AccessFaxMail, "AccessFaxMail property did not match false value");
             Assert.IsTrue(oCos.AccessTts, "AccessTTS property did not match true value");
             Assert.IsTrue(oCos.MaxGreetingLength == 23, "Max greeting length property did not match 23");

             oCos = _connectionServer.GetObjectFromJson<ClassOfService>("blah");
             Assert.IsNull(oCos,"Invalid json string should result in null object");

         }
   }
}
