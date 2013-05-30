using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class RestTransportFunctionsTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion

        [TestMethod]
        public void HttpTimeout_Test()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            oFunctions.HttpTimeoutSeconds = 3;
            Assert.IsTrue(oFunctions.HttpTimeoutSeconds==3,"HTTP Timeout was not set to 3");

            oFunctions.HttpTimeoutSeconds = -1;
            Assert.IsTrue(oFunctions.HttpTimeoutSeconds == 3, "HTTP Timeout did not ignore -1 as an illegal value");

            oFunctions.HttpTimeoutSeconds = 101;
            Assert.IsTrue(oFunctions.HttpTimeoutSeconds == 3, "HTTP Timeout did not ignore 101 as an illegal value");
        }

        [TestMethod]
        public void GetObjectsFromJson_Tests()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();
            var oList= oFunctions.GetObjectsFromJson<List<CallHandler>>("bogus JSON string");
            Assert.IsNotNull(oList,"List returned from invalid GetObjectsFromJson call should not be null");
            Assert.IsTrue(oList.Count==0,"List returned from invalid GetObjectsFromJson call should be empty");

            oList = oFunctions.GetObjectsFromJson<List<CallHandler>>("bogus JSON string","OverrideName");
            Assert.IsNotNull(oList, "List returned from invalid GetObjectsFromJson call should not be null");
            Assert.IsTrue(oList.Count == 0, "List returned from invalid GetObjectsFromJson call should be empty");

        }

        [TestMethod]
        public void GetObjectFromJson_Test()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var oObject = oFunctions.GetObjectFromJson<CallHandler>("bogus JSON string");
            Assert.IsNull(oObject, "Object returned from invalid GetObjectFromJson call should be null");

            oObject = oFunctions.GetObjectFromJson<CallHandler>("bogus JSON string","OverrideName");
            Assert.IsNull(oObject, "Object returned from invalid GetObjectFromJson call should be null");

        }


        [TestMethod]
        public void GetCupiResponse_Test()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var res = oFunctions.GetCupiResponse("bogus", MethodType.GET, null, null);
            Assert.IsFalse(res.Success,"Calling GetCupiResponse with null Connection server did not fail.");
        }

        [TestMethod]
        public void DownloadWavFile_Test()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();
            var res = oFunctions.DownloadWavFile(null, "bogus", "bogus");
            Assert.IsFalse(res.Success,"Calling DownloadWavFile with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void UploadWavFile_Test()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();
            var res = oFunctions.UploadWavFile("bogus", null, "bogus");
            Assert.IsFalse(res.Success, "Calling UploadWavFile with null ConnectionServerRest did not fail");
        }

        [TestMethod]
        public void UploadVoiceMessageWav_Test()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var res = oFunctions.UploadVoiceMessageWav(null, "bogus", "bogus", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling UplaodVoiceMessageWav null ConnectionServerRest did not fail.");

            res = oFunctions.UploadVoiceMessageWav(new ConnectionServerRest(oFunctions), "", "bogus", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling UplaodVoiceMessageWav with empty local Wav file did not fail.");
        }


        [TestMethod]
        public void UploadVoiceMessageResourceId_Test()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            var res = oFunctions.UploadVoiceMessageResourceId(null, "bogus", "bogus", "bogus", "bogus");
            Assert.IsFalse(res.Success,"Calling UplaodVoiceMessageResourceId with null ConnectionServerRest did not fail.");

            res = oFunctions.UploadVoiceMessageResourceId(new ConnectionServerRest(oFunctions), "", "bogus", "bogus", "bogus");
            Assert.IsFalse(res.Success, "Calling UplaodVoiceMessageResourceId with empty resourceId did not fail.");
        }

        [TestMethod]
        public void UploadWavFileToStreamLibrary_Test()
        {
            RestTransportFunctions oFunctions = new RestTransportFunctions();

            string strRet;
            var res = oFunctions.UploadWavFileToStreamLibrary(null, "bogus",out strRet);
            Assert.IsFalse(res.Success, "Calling UploadWavFileToStreamLibrary with null ConnectionServerRest did not fail.");
        }

    }
}