using System;
using System.Collections.Generic;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cisco.UnityConnection.RestFunctions;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PostGreetingRecordingUnitTests : BaseUnitTests 
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            PostGreetingRecording oTest = new PostGreetingRecording(null);
            Console.WriteLine(oTest);
        }

        #endregion


        #region PostGreetingRecording Static Call Failures

        [TestMethod]
        public void DeletePostGreetingRecording_NullConnectionServer_Failure()
        {
            var res = PostGreetingRecording.DeletePostGreetingRecording(null, "objectID");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void DeletePostGreetingRecording_EmptyObjectId_Failure()
        {
            var res = PostGreetingRecording.DeletePostGreetingRecording(_mockServer, "");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetPostGreetingRecording_NullconnectionServer_Failure()
        {
            PostGreetingRecording oGreeting;
            var res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, null, "objectID");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetPostGreetingRecording_EmptyObjectIdAndName_Failure()
        {
            PostGreetingRecording oGreeting;
            var res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _mockServer);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetPostGreetingRecordings_NullConnectionServer_Failure()
        {
            List<PostGreetingRecording> oGreetings;
            var res = PostGreetingRecording.GetPostGreetingRecordings(null, out oGreetings, 1, 10);
            Assert.IsFalse(res.Success, "");
        }


        [TestMethod]
        public void SetPostGreetingRecordingWavFile_NullConnectionServer_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingWavFile(null, "test.wav", "objectid", 1033, true);
            Assert.IsFalse(res.Success, "");
        }


        [TestMethod]
        public void SetPostGreetingRecordingWavFile_EmptyWavFilePath_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingWavFile(_mockServer, "", "objectid", 1033, true);
            Assert.IsFalse(res.Success, "");
        }


        [TestMethod]
        public void SetPostGreetingRecordingWavFile_InvalidWavFile_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingWavFile(_mockServer, "bogus.xyz", "objectid", 1033, true);
            Assert.IsFalse(res.Success, "");
        }


        [TestMethod]
        public void SetPostGreetingRecordingWavFile_EmptyObjectId_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingWavFile(_mockServer, "wavcopy.exe", "", 1033, true);
            Assert.IsFalse(res.Success, "");
        }


        [TestMethod]
        public void SetPostGreetingRecordingToStreamFile_NullConnectionServer_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingToStreamFile(null, "streamid", "objectid", 1033);
            Assert.IsFalse(res.Success, "");
         }


        [TestMethod]
        public void SetPostGreetingRecordingToStreamFile_EmptyStreamId_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingToStreamFile(_mockServer, "", "objectid", 1033);
            Assert.IsFalse(res.Success, "");
        }


        [TestMethod]
        public void SetPostGreetingRecordingToStreamFile_EmptyObjectId_Failure()
        {
            var res = PostGreetingRecording.SetPostGreetingRecordingToStreamFile(_mockServer, "streamid", "", 1033);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void UpdatePostGreetingRecording_NullConnectionServer_Failure()
        {
            var res = PostGreetingRecording.UpdatePostGreetingRecording(null, "objectid", "display name");
            Assert.IsFalse(res.Success, "");
         }

        [TestMethod]
        public void UpdatePostGreetingRecording_EmptyObjectId_Failure()
        {
            var res = PostGreetingRecording.UpdatePostGreetingRecording(_mockServer, "", "display name");
            Assert.IsFalse(res.Success, "");
         }

        [TestMethod]
        public void UpdatePostGreetingRecording_EmptyDisplayName_Failure()
        {
            var res = PostGreetingRecording.UpdatePostGreetingRecording(_mockServer, "objectid", "");
            Assert.IsFalse(res.Success, "");
        }


        [TestMethod]
        public void AddPostGreetingRecording_NullConnectionServer_Failure()
        {
            WebCallResult res = PostGreetingRecording.AddPostGreetingRecording(null, "displayname");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void AddPostGreetingRecording_EmptyDisplayName_Failure()
        {
            var res = PostGreetingRecording.AddPostGreetingRecording(_mockServer, "");
            Assert.IsFalse(res.Success, "");
        }

        #endregion


        #region PostGreetingRecordingStreamFile Static Call Failures 

        [TestMethod]
        public void GetGreetingStreamFiles_NullConnectionServer_Failure()
        {
            List<PostGreetingRecordingStreamFile> oGreetingStreams;
            var res = PostGreetingRecordingStreamFile.GetGreetingStreamFiles(null, "objectid", out oGreetingStreams);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetGreetingStreamFiles_EmptyObjectId_Failure()
        {
            List<PostGreetingRecordingStreamFile> oGreetingStreams;

            var res = PostGreetingRecordingStreamFile.GetGreetingStreamFiles(_mockServer, "", out oGreetingStreams);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetGreetingWavFile_NullConnectionServer_Failure()
        {
            var res = PostGreetingRecordingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "streamfilename");
            Assert.IsFalse(res.Success, "");
         }

        [TestMethod]
        public void GetGreetingWavFile_InvalidTargetFolder_Failure()
        {
            var res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_mockServer, "e:\\bogus\\bogus.xyz", "bogus");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetGreetingWavFile_EmptyStreamFileName_Failure()
        {
            var res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_mockServer, "bogus.xyz", "");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetGreetingWavFile_NullConnectionServerWithLanguageCode_Failure()
        {
            var res = PostGreetingRecordingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "streamfilename", 1033);
            Assert.IsFalse(res.Success, "");
        }


        [TestMethod]
        public void GetGreetingWavFile_InvalidTargetFolderWithLanguageCode_Failure()
        {
            var res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_mockServer, "e:\\bogus\\bogus.xyz", "bogus", 1033);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetGreetingWavFile_EmptyStreamFileNameWithLanguageCode_Failure()
        {
            var res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_mockServer, "bogus.xyz", "", 1033);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void SetGreetingWavFile_NullConnectionServer_Failure()
        {
            var res = PostGreetingRecordingStreamFile.SetGreetingWavFile(null, "objectid", 1033, "c:\\temp.wav", true);
            Assert.IsFalse(res.Success, "");

         }

        [TestMethod]
        public void SetGreetingWavFile_EmptyObjectId_Failure()
        {
            var res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_mockServer, "", 1033, "c:\\temp.wav", true);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void SetGreetingWavFile_EmptySourcePath_Failure()
        {
            var res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_mockServer, "objectid", 1033, "", true);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetGreetingStreamFile_NullConnectionServer_Failure()
        {
            PostGreetingRecordingStreamFile oGreetingStream;
            WebCallResult res = PostGreetingRecordingStreamFile.GetGreetingStreamFile(null, "objectid", 1033,out oGreetingStream);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void GetGreetingStreamFile_EmptyObjectId_Failure()
        {
            PostGreetingRecordingStreamFile oGreetingStream;
            var res = PostGreetingRecordingStreamFile.GetGreetingStreamFile(_mockServer, "", 1033, out oGreetingStream);
            Assert.IsFalse(res.Success, "");
        }

        #endregion

    }
}
