using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class PostGreetingRecordingTest
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServer _connectionServer;

        private static PostGreetingRecording _tempGreeting;

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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
                HTTPFunctions.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start PostGreetingRecording test:" + ex.Message);
            }

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
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure_NullServer()
        {
            PostGreetingRecording oTest = new PostGreetingRecording(null);
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure_InvalidObjectId()
        {
            PostGreetingRecording oTest = new PostGreetingRecording(_connectionServer,"bogus");
            Console.WriteLine(oTest);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ClassCreationFailure_InvalidName()
        {
            PostGreetingRecording oTest = new PostGreetingRecording(_connectionServer, "","bogus");
            Console.WriteLine(oTest);
        }

        #endregion


        [TestMethod]
        public void PostGreetingRecording_StaticMethodFailures()
        {
            //AddPostGreetingRecording
            WebCallResult res = PostGreetingRecording.AddPostGreetingRecording(null, "displayname");
            Assert.IsFalse(res.Success,"");

            res = PostGreetingRecording.AddPostGreetingRecording(_connectionServer, "");
            Assert.IsFalse(res.Success, "");

            //DeletePostGreetingRecording
            res = PostGreetingRecording.DeletePostGreetingRecording(null, "objectID");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.DeletePostGreetingRecording(_connectionServer, "objectID");
            Assert.IsFalse(res.Success, "");
            
            res = PostGreetingRecording.DeletePostGreetingRecording(_connectionServer, "");
            Assert.IsFalse(res.Success, "");

            //GetPostGreetingRecording
            PostGreetingRecording oGreeting;
            res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, null, "objectID");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer, "objectID");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer, "","bogus");
            Assert.IsFalse(res.Success, "");

            //GetPostGreetingRecordings
            List<PostGreetingRecording> oGreetings;
            res = PostGreetingRecording.GetPostGreetingRecordings(null, out oGreetings, 1, 10);
            Assert.IsFalse(res.Success, "");

            //SetPostGreetingRecordingWavFile
            res = PostGreetingRecording.SetPostGreetingRecordingWavFile(null, "test.wav", "objectid", 1033, true);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.SetPostGreetingRecordingWavFile(_connectionServer, "", "objectid", 1033, true);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.SetPostGreetingRecordingWavFile(_connectionServer, "wavcopy.exe", "objectid", 1033, true);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.SetPostGreetingRecordingWavFile(_connectionServer, "wavcopy.exe", "", 1033, true);
            Assert.IsFalse(res.Success, "");

            //SetPostGreetingRecordingToStreamFile
            res = PostGreetingRecording.SetPostGreetingRecordingToStreamFile(null, "streamid", "objectid", 1033);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.SetPostGreetingRecordingToStreamFile(_connectionServer, "streamid", "objectid", 1033);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.SetPostGreetingRecordingToStreamFile(_connectionServer, "", "objectid", 1033);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.SetPostGreetingRecordingToStreamFile(_connectionServer, "streamid", "", 1033);
            Assert.IsFalse(res.Success, "");

            //UpdatePostGreetingRecording
            res = PostGreetingRecording.UpdatePostGreetingRecording(null, "objectid", "display name");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.UpdatePostGreetingRecording(_connectionServer, "objectid", "display name");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.UpdatePostGreetingRecording(_connectionServer, "", "display name");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecording.UpdatePostGreetingRecording(_connectionServer, "objectid", "");
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void PostGreetingRecordingStreamFiles_StaticMethodFailures()
        {
            //GetGreetingStreamFile
            PostGreetingRecordingStreamFile oGreetingStream;
            WebCallResult res = PostGreetingRecordingStreamFile.GetGreetingStreamFile(null, "objectid", 1033,out oGreetingStream);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingStreamFile(_connectionServer, "objectid", 1033, out oGreetingStream);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingStreamFile(_connectionServer, "", 1033, out oGreetingStream);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingStreamFile(_connectionServer, _tempGreeting.ObjectId, 9999, out oGreetingStream);
            Assert.IsFalse(res.Success, "");

            //GetGreetingStreamFiles
            List<PostGreetingRecordingStreamFile> oGreetingStreams;
            res = PostGreetingRecordingStreamFile.GetGreetingStreamFiles(null, "objectid", out oGreetingStreams);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingStreamFiles(_connectionServer, "", out oGreetingStreams);
            Assert.IsFalse(res.Success, "");

            //GetGreetingWavFile
            res =PostGreetingRecordingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "streamfilename");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_connectionServer, "c:\\temp.wav", "streamfilename");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_connectionServer, "e:\\bogus\\bogus.xyz", "bogus");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_connectionServer, "bogus.xyz", "");
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "streamfilename",1033);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_connectionServer, "c:\\temp.wav", "streamfilename",1033);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_connectionServer, "e:\\bogus\\bogus.xyz", "bogus",1033);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.GetGreetingWavFile(_connectionServer, "bogus.xyz", "",1033);
            Assert.IsFalse(res.Success, "");

            //SetGreetingWavFile
            res =PostGreetingRecordingStreamFile.SetGreetingWavFile(null, "objectid", 1033, "c:\\temp.wav", true);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_connectionServer, "objectid", 1033, "c:\\temp.wav", true);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_connectionServer, "", 1033, "c:\\temp.wav", true);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_connectionServer, _tempGreeting.ObjectId, 1033, "", true);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_connectionServer, _tempGreeting.ObjectId, 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "");

            res = PostGreetingRecordingStreamFile.SetGreetingWavFile(_connectionServer, _tempGreeting.ObjectId, 9999, "Dummy.wav", true);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void FetchTests()
        {
            List<PostGreetingRecording> oGreetings;
            WebCallResult res = PostGreetingRecording.GetPostGreetingRecordings(_connectionServer, out oGreetings, 1, 2);
            Assert.IsTrue(res.Success,"Failed to fetch post greeting recordings:"+res);
            Assert.IsTrue(oGreetings.Count>0,"No post greeting recordings returned on fetch");

            PostGreetingRecording oGreeting;
            res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer,oGreetings[0].ObjectId);
            Assert.IsTrue(res.Success,"Failed to fetch post greeting recording by valid objectId:"+res);

            res = PostGreetingRecording.GetPostGreetingRecording(out oGreeting, _connectionServer,"", oGreetings[0].DisplayName);
            Assert.IsTrue(res.Success, "Failed to fetch post greeting recording by valid name:" + res);

            Console.WriteLine(oGreeting);
            oGreeting.DumpAllProps();

        }


        [TestMethod]
        public void UpdateTests()
        {
            WebCallResult res = _tempGreeting.Update("newDisplayName" + Guid.NewGuid());
            Assert.IsTrue(res.Success,"Failed to update post greeting recording display name:"+res);

            res = _tempGreeting.Update("");
            Assert.IsFalse(res.Success,"Setting greeting display name to blank did not fail");

        }

        [TestMethod]
        public void RecordedStreamsTest()
        {
            List<PostGreetingRecordingStreamFile> oStreams = _tempGreeting.GetGreetingStreamFiles();
            Assert.IsNotNull(oStreams,"Null streams list returned from fetch");
            Assert.IsTrue(oStreams.Count==0,"Streams count not 0 on new post greeting recording");

            WebCallResult res =_tempGreeting.SetRecordingToWavFile("Dummy.wav", 1033,true);
            Assert.IsTrue(res.Success,"Failed to upload greeting stream for 1033:"+res);

            Thread.Sleep(2000);

            oStreams = _tempGreeting.GetGreetingStreamFiles(true);
            Assert.IsNotNull(oStreams, "Null streams list returned from fetch");
            Assert.IsTrue(oStreams.Count == 1, "Streams count not 1 after upload of stream");

            oStreams[0].DumpAllProps();
            Console.WriteLine(oStreams[0].ToString());

            res = oStreams[0].SetGreetingWavFile("Dummy.wav", true);
            Assert.IsTrue(res.Success, "Failed to upload greeting stream existing stream object:" + res);


            string strFileName = Guid.NewGuid().ToString() + ".wav";
            res = oStreams[0].GetGreetingWavFile(strFileName);
            Assert.IsTrue(res.Success,"Failed downloading WAV file from stream:"+res);
            Assert.IsTrue(File.Exists(strFileName),"Wav file download did not create local file:"+strFileName);

            try
            {
                File.Delete(strFileName);
            }
            catch (Exception ex)
            {
                Assert.Fail("Could not delete local temporary file:"+strFileName+", error="+ex);
            }

            res =_tempGreeting.SetRecordingToStreamFile("bogus", 1024);
            Assert.IsFalse(res.Success,"Setting recording to invalid stream did not fail");

        }


    }
}
