using System.Collections.Generic;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RestTransportFunctionsUnitTests : BaseUnitTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        [TestMethod]
        public void HttpTimeoutSeconds_SetTo3_Success()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            oFunctions.HttpTimeoutSeconds = 3;
            Assert.IsTrue(oFunctions.HttpTimeoutSeconds == 3, "HTTP Timeout was not set to 3");
        }

        [TestMethod]
        public void HttpTimeoutSeconds_SetToNegative1_Ignore()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();
            int iTemp = oFunctions.HttpTimeoutSeconds;
            oFunctions.HttpTimeoutSeconds = -1;
            Assert.IsTrue(oFunctions.HttpTimeoutSeconds == iTemp, "HTTP Timeout did not ignore -1 as an illegal value");
        }

        [TestMethod]
        public void HttpTimeoutSeconds_SetTo101_Ignore()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();
            int iTemp = oFunctions.HttpTimeoutSeconds;
            oFunctions.HttpTimeoutSeconds = 101;
            Assert.IsTrue(oFunctions.HttpTimeoutSeconds == iTemp, "HTTP Timeout did not ignore 101 as an illegal value");
        }


        [TestMethod]
        public void GetObjectsFromJson_InvalidJson_Success()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();
            var oList = oFunctions.GetObjectsFromJson<List<CallHandler>>("bogus JSON string");
            Assert.IsNotNull(oList, "List returned from invalid GetObjectsFromJson call should not be null");
            Assert.IsTrue(oList.Count == 0, "List returned from invalid GetObjectsFromJson call should be empty");
        }

        [TestMethod]
        public void GetObjectsFromJson_InvalidJsonWithNameOverride_Success()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var oList = oFunctions.GetObjectsFromJson<List<CallHandler>>("bogus JSON string","OverrideName");
            Assert.IsNotNull(oList, "List returned from invalid GetObjectsFromJson call should not be null");
            Assert.IsTrue(oList.Count == 0, "List returned from invalid GetObjectsFromJson call should be empty");
        }

        [TestMethod]
        public void GetObjectFromJson_InvalidJson_Success()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var oObject = oFunctions.GetObjectFromJson<CallHandler>("bogus JSON string");
            Assert.IsNull(oObject, "Object returned from invalid GetObjectFromJson call should be null");
        }

        [TestMethod]
        public void GetObjectFromJson_InvalidJsonWithNameOverride_Success()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var oObject = oFunctions.GetObjectFromJson<CallHandler>("bogus JSON string","OverrideName");
            Assert.IsNull(oObject, "Object returned from invalid GetObjectFromJson call should be null");
        }


        [TestMethod]
        public void GetCupiResponse_NullConnectionServer_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var res = oFunctions.GetCupiResponse("bogus", MethodType.GET, null, null);
            Assert.IsFalse(res.Success,"Calling GetCupiResponse with null Connection server did not fail.");
        }

        [TestMethod]
        public void DownloadWavFile_NullConnectionServer_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();
            var res = oFunctions.DownloadWavFile(null, "bogus", "bogus");
            Assert.IsFalse(res.Success,"Calling DownloadWavFile with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void UploadWavFile_NullConnectionServer_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();
            var res = oFunctions.UploadWavFile("bogus", null, "bogus");
            Assert.IsFalse(res.Success, "Calling UploadWavFile with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void UploadVoiceMessageWav_NullConnectionServer_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var res = oFunctions.UploadVoiceMessageWav(null, "bogus", "bogus", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling UplaodVoiceMessageWav null ConnectionServerRest did not fail.");
        }

        [TestMethod]
        public void UploadVoiceMessageWav_EmptyWavFilePath_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var res = oFunctions.UploadVoiceMessageWav(new ConnectionServerRest(oFunctions), "", "bogus", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling UplaodVoiceMessageWav with empty local Wav file did not fail.");
        }

        [TestMethod]
        public void UploadVoiceMessageWav_EmptyJsonDetails_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var res = oFunctions.UploadVoiceMessageWav(new ConnectionServerRest(oFunctions), "c:\\temp.wav", "", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling UplaodVoiceMessageWav with empty local Wav file did not fail.");
        }

        [TestMethod]
        public void UploadVoiceMessageResourceId_NullConnectionServer_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var res = oFunctions.UploadVoiceMessageResourceId(null, "bogus", "bogus", "bogus", "bogus");
            Assert.IsFalse(res.Success,"Calling UplaodVoiceMessageResourceId with null ConnectionServerRest did not fail.");

            }

        [TestMethod]
        public void UploadVoiceMessageResourceId_EmptyResourceId_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();
            var res = oFunctions.UploadVoiceMessageResourceId(new ConnectionServerRest(oFunctions), "", "bogus", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling UplaodVoiceMessageResourceId with empty resourceId did not fail.");
        }

        [TestMethod]
        public void UploadWavFileToStreamLibrary_NullConnectionServer_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            string strRet;
            var res = oFunctions.UploadWavFileToStreamLibrary(null, "c:\\temp.wav",out strRet);
            Assert.IsFalse(res.Success, "Calling UploadWavFileToStreamLibrary with null ConnectionServerRest did not fail.");
        }

        [TestMethod]
        public void UploadWavFileToStreamLibrary_EmptyWavFilePath_Failure()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            string strRet;
            var res = oFunctions.UploadWavFileToStreamLibrary(_mockServer, "", out strRet);
            Assert.IsFalse(res.Success, "Calling UploadWavFileToStreamLibrary with null ConnectionServerRest did not fail.");
        }

    }
}