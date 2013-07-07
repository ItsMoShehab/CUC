using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cisco.UnityConnection.RestFunctions;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PostGreetingRecordingIntegrationTests : BaseIntegrationTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static PostGreetingRecording _tempGreeting;

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

            //create new greeting with GUID in the name to ensure uniqueness
            String strName = "TempGreeting_" + Guid.NewGuid().ToString().Replace("-", "");

            WebCallResult res = PostGreetingRecording.AddPostGreetingRecording(_connectionServer, strName, out _tempGreeting);
            Assert.IsTrue(res.Success, "Failed creating temporary interview handler:" + res.ToString());
            Assert.IsNotNull(_tempGreeting,"Null temorary greeting passed back");
        }


        [ClassCleanup]
        public static void MyClassCleanup()
        {
            if (_tempGreeting != null)
            {
                WebCallResult res = _tempGreeting.Delete();
                Assert.IsTrue(res.Success, "Failed to delete temporary post greeting recording on cleanup.");
            }
        }

        #endregion


        #region Class Construction Failures

        /// <summary>
        /// UnityConnectionRestException on invalid objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidObjectId_Failure()
        {
            PostGreetingRecording oTest = new PostGreetingRecording(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// UnityConnectionRestException on invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnityConnectionRestException))]
        public void Constructor_InvalidName_Failure()
        {
            PostGreetingRecording oTest = new PostGreetingRecording(_connectionServer, "","bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        #region PostGreetingRecording Static Call Failures

        [TestMethod]
        public void DeletePostGreetingRecording_InvalidObjectId_Failure()
        {
            var res = PostGreetingRecording.DeletePostGreetingRecording(_connectionServer, "objectID");
            Assert.IsFalse(res.Success, "Calling DeletePostGreetingRecording with invalid ObjectId did not fail");
        }

        [TestMethod]
        public void GetPostGreetingRecording_InvalidObjectId_Failure()
        {
            PostGreetingRecording oGreeting;
            var res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer, "objectID");
            Assert.IsFalse(res.Success, "Calling GetPostGreetingRecording with invalid ObjectId did not fail");

        }

        [TestMethod]
        public void GetPostGreetingRecording_MissingObjectIdAndName_Failure()
        {
            PostGreetingRecording oGreeting;
            var res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer);
            Assert.IsFalse(res.Success, "Calling GetPostGreetingRecording with missing ObjectId and nane did not fail");

        }

        [TestMethod]
        public void GetPostGreetingRecording_InvalidName_Failure()
        {
            PostGreetingRecording oGreeting;
            var res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer, "", "bogus");
            Assert.IsFalse(res.Success, "Calling GetPostGreetingRecording with invalid Name did not fail");
        }

        [TestMethod]
        public void SetPostGreetingRecordingWavFile_InvalidObjectId_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingWavFile(_connectionServer, "dummy.wav", "objectid", 1033, true);
            Assert.IsFalse(res.Success, "calling SetPostGreetingRecordingWavFile with invalid ObjectId did not fail");
        }

        [TestMethod]
        public void SetPostGreetingRecordingWavFile_InvalidWavFile_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingWavFile(_connectionServer, "wavcopy.exe", "objectid",1033, true);
            Assert.IsFalse(res.Success, "Calling SetPostGreetingRecordingWavFile with invalid WAV file did not fail");
        }

        [TestMethod]
        public void SetPostGreetingRecordingToStreamFile_InvalidStreamIdAndObjectId_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingToStreamFile(_connectionServer, "streamid", "objectid", 1033);
            Assert.IsFalse(res.Success, "Calling SetPostGreetingRecordingToStreamFile with invalid streaId and ObjectId did not fail");
        }

        [TestMethod]
        public void UpdatePostGreetingRecording_InvalidObjectId_Failure()
        {
            var res = PostGreetingRecording.UpdatePostGreetingRecording(_connectionServer, "objectid", "display name");
            Assert.IsFalse(res.Success, "calling UpdatePostGreetingRecording with invalid ObjectId did not fail");
        }

       #endregion


        #region PostGreetingRecordingStreamFile Static Call Failures 

        [TestMethod]
        public void GetGreetingStreamFiles_InvalidObjectId_Failure()
        {
            List<PostGreetingRecordingStreamFile> oGreetingStreams;
            var res = PostGreetingRecordingStreamFile.GetGreetingStreamFiles(_connectionServer, "objectId", out oGreetingStreams);
            Assert.IsTrue(res.Success, "calling GetGreetingStreamFiles with invalid ObjectId should not fail:"+res);
            Assert.IsTrue(oGreetingStreams.Count==0,"Calling GetGreetingStreamFiles with invalid ObjectId should reutrn an empty list");
        }

        [TestMethod]
        public void GetGreetingWavFile_InvalidStreamName_Failure()
        {
            var res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_connectionServer, "c:\\temp.wav", "invalidstreamname");
            Assert.IsFalse(res.Success, "calling GetGreetingWavFile with invalid streamname did not fail");
        }
        
        [TestMethod]
        public void GetGreetingWavFile_InvalidTargetFolder_Failure()
        {
            var res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_connectionServer, "e:\\bogus\\bogus.xyz", "streamfilename", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetingWavFile with invalid target folder did not fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_InvalidObjectId_Failure()
        {
            var res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_connectionServer, "objectid", 1033,"c:\\temp.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with invalid ObjectId did not fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_InvalidWavPath_Failure()
        {
            var res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_connectionServer, _tempGreeting.ObjectId, 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with invalid WAV path did not fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_InvalidLanguageCode_Failure()
        {
            var res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_connectionServer, _tempGreeting.ObjectId, 9999, "Dummy.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with invalid language code did not fail");
        }

        [TestMethod]
        public void GetGreetingStreamFile_InvalidObjectId_Failure()
        {
            PostGreetingRecordingStreamFile oGreetingStream;
            var res = PostGreetingRecordingStreamFile.GetGreetingStreamFile(_connectionServer, "objectid", 1033, out oGreetingStream);
            Assert.IsFalse(res.Success, "calling GetGreetingStreamFile with invalid ObjectId did not fail");
        }

        [TestMethod]
        public void GetGreetingStreamFile_InvalidLanguageCode_Failure()
        {
            PostGreetingRecordingStreamFile oGreetingStream;
            var res = PostGreetingRecordingStreamFile.GetGreetingStreamFile(_connectionServer, _tempGreeting.ObjectId, 9999, out oGreetingStream);
            Assert.IsFalse(res.Success, "calling GetGreetingStreamFile with invalid language code did not fail");
        }

        #endregion


        #region Live Tests


        [TestMethod]
        public void GetPostGreetingRecordings_Success()
        {
            List<PostGreetingRecording> oGreetings;
            WebCallResult res = PostGreetingRecording.GetPostGreetingRecordings(_connectionServer, out oGreetings, 1, 2);
            Assert.IsTrue(res.Success, "Failed to fetch post greeting recordings:" + res);
            Assert.IsTrue(oGreetings.Count > 0, "No post greeting recordings returned on fetch");
        }

        [TestMethod]
        public void GetPostGreetingRecordings_ConstructorWithObjectId_Success()
        {
            PostGreetingRecording oGreeting;
            var res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer,
                                                                     _tempGreeting.ObjectId);
            Assert.IsTrue(res.Success, "Failed to fetch post greeting recording by valid objectId:" + res);
        }

        [TestMethod]
        public void GetPostGreetingRecordings_ConstructorWithDisplayName_Success()
        {
            PostGreetingRecording oGreeting;

            var res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer,"", _tempGreeting.DisplayName);
            Assert.IsTrue(res.Success, "Failed to fetch post greeting recording by valid name:" + res);

            Console.WriteLine(oGreeting);
            Console.WriteLine(oGreeting.DumpAllProps());
        }


        [TestMethod]
        public void Update_Success()
        {
            WebCallResult res = _tempGreeting.Update("newDisplayName" + Guid.NewGuid());
            Assert.IsTrue(res.Success, "Failed to update post greeting recording display name:" + res);
        }

        [TestMethod]
        public void Update_EmptyDisplayName_Failure()
        {
            var res = _tempGreeting.Update("");
            Assert.IsFalse(res.Success,"Setting greeting display name to blank did not fail");
        }


        [TestMethod]
        public void GetGreetingStreamFiles_Success()
        {
            List<PostGreetingRecordingStreamFile> oStreams = _tempGreeting.GetGreetingStreamFiles();
            Assert.IsNotNull(oStreams, "Null streams list returned from fetch");
            Assert.IsTrue(oStreams.Count == 0, "Streams count not 0 on new post greeting recording");
        }

        [TestMethod]
        public void SetRecordingToWavFile_Success()
        {
            WebCallResult res = _tempGreeting.SetRecordingToWavFile("Dummy.wav", 1033, true);
            Assert.IsTrue(res.Success, "Failed to upload greeting stream for 1033:" + res);

            Thread.Sleep(1000);

            var oStreams = _tempGreeting.GetGreetingStreamFiles(true);
            Assert.IsNotNull(oStreams, "Null streams list returned from fetch");
            Assert.IsTrue(oStreams.Count == 1, "Streams count not 1 after upload of stream");

            oStreams[0].DumpAllProps();
            Console.WriteLine(oStreams[0].ToString());

            res = oStreams[0].SetGreetingWavFile("Dummy.wav", true);
            Assert.IsTrue(res.Success, "Failed to upload greeting stream existing stream object:" + res);

            string strFileName = Guid.NewGuid().ToString() + ".wav";
            res = oStreams[0].GetGreetingWavFile(strFileName);
            Assert.IsTrue(res.Success, "Failed downloading WAV file from stream:" + res);
            Assert.IsTrue(File.Exists(strFileName), "Wav file download did not create local file:" + strFileName);

            try
            {
                File.Delete(strFileName);
            }
            catch (Exception ex)
            {
                Assert.Fail("Could not delete local temporary file:" + strFileName + ", error=" + ex);
            }
        }

        [TestMethod]
        public void SetRecordingToStreamFile_InvalidResourceName_Failure()
        {
            var res =_tempGreeting.SetRecordingToStreamFile("bogus", 1024);
            Assert.IsFalse(res.Success,"Setting recording to invalid stream did not fail");
        }

        #endregion
    }
}
