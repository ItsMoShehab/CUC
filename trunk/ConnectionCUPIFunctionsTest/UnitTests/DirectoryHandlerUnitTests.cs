using System;
using System.Collections.Generic;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cisco.UnityConnection.RestFunctions;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for DirectoryHandlerUnitTests and is intended
    ///to contain all DirectoryHandler Unit Tests
    ///</summary>
    [TestClass]
    public class DirectoryHandlerUnitTests : BaseUnitTests
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


        #region Constructor Tests

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassCreationFailure()
        {
            DirectoryHandler oTestHandler = new DirectoryHandler(null);
            Console.WriteLine(oTestHandler);
        }

        #endregion


        #region Static Call Failures

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDirectoryHandlers()
        {
            List<DirectoryHandler> oHandlerList;

            WebCallResult res = DirectoryHandler.GetDirectoryHandlers(null, out oHandlerList, null);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail with null ConnectionServerRest passed to it");

        }

        [TestMethod]
        public void StaticCallFailures_AddDirectoryHandler()
        {
            DirectoryHandler oHandler;
            var res = DirectoryHandler.AddDirectoryHandler(null, "display name", true, null, out oHandler);
            Assert.IsFalse(res.Success, "Calling AddHandler with null ConnectionServerRest did not fail");

            res = DirectoryHandler.AddDirectoryHandler(_mockServer, "", true, null, out oHandler);
            Assert.IsFalse(res.Success, "Calling AddHandler with empty display name did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_DeleteDirectoryHandler()
        {
            var res = DirectoryHandler.DeleteDirectoryHandler(null, "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with null ConnectionServerRest did not fail");

            res = DirectoryHandler.DeleteDirectoryHandler(_mockServer, "");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with empty objectId did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_UpdateDirectoryHandler()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(null, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with null ConnectionServerRest did not fail");

            res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with null property list did not fail");

            res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with empty objectid did not fail");

            res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "objectid", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with empty property list did not fail");

            res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "objectid", new ConnectionPropertyList("name", "value"));
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with invalid objectId did not fail");
        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDirectoryHandler()
        {
            DirectoryHandler oHandler;

            WebCallResult res = DirectoryHandler.GetDirectoryHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ConnectionServerRest is null");

            res = DirectoryHandler.GetDirectoryHandler(out oHandler, _mockServer);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ObjectId and display name are both blank");
        }

        [TestMethod]
        public void StaticCallFailures_GetGreetingStreamFiles()
        {
            //GetGreetingStreamFiles
            List<DirectoryHandlerGreetingStreamFile> oStreams;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(null, "objectid", out oStreams);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFiles with null ConnectionServer should fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(_mockServer, "objectid", out oStreams);
            Assert.IsTrue(res.Success, "Fetching greeting stream files with an invalid objectId shouldn't fail:" + res);
            Assert.IsTrue(oStreams.Count == 0, "Fetching streams with an invalid objectId should return an empty list");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(_mockServer, "", out oStreams);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFiles with empty objectId should fail");
        }

        [TestMethod]
        public void StaticCallFailures_GetGreetingWavFile()
        {
            //GetGreetingWavFile
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "streamname.wav");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with null ConnectionServer should fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "c:\\temp.wav", "streamname.wav");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with invalid StreamName shoudl fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "", "streamname.wav");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty local wav path should fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "c:\\temp.wav", "");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty stream name should fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with null ConnectionServer should fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "c:\\temp.wav", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with invalid OBjectId should fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty local wav file path should fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "c:\\temp.wav", "", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty objectId should fail");

        }

        [TestMethod]
        public void StaticCallFailures_SetGreetingWavFile()
        {
            //SetGreetingWavFile
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(null, "objectid", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with null ConnectionServer should fail");

            res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_mockServer, "objectid", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with invalid local wav file should fail");

            res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_mockServer, "", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with Empty ObjectId should fail");

            res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_mockServer, "objectid", 1033, "", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with empty local wav file path should fail");

        }

        [TestMethod]
        public void StaticCallFailures_SetGreetingStreamFile()
        {
            var res = DirectoryHandler.SetGreetingRecordingToStreamFile(null, "streamname", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with null ConnectionServerRest did not fail");

            res = DirectoryHandler.SetGreetingRecordingToStreamFile(_mockServer, "streamname", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with invalid objectId did not fail");

            res = DirectoryHandler.SetGreetingRecordingToStreamFile(_mockServer, "", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with empty streamID did not fail");

            res = DirectoryHandler.SetGreetingRecordingToStreamFile(_mockServer, "streamname", "", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with empty objecTId did not fail");

            res = DirectoryHandler.SetGreetingRecordingToStreamFile(_mockServer, "streamname", "objectId", 9999);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with invalid language ID did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_GetGreetingStreamFile()
        {
            //GetGreetingStreamFile
            DirectoryHandlerGreetingStreamFile oStream;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(null, "objectid", 1033, out oStream);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFile with null ConnectionServer should fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(_mockServer, "", 1033, out oStream);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFile with empty objectId should fail");

        }


        #endregion

    }
}
