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
        public void GetDirectoryHandlers_NullConnectionServer_Failure()
        {
            List<DirectoryHandler> oHandlerList;

            WebCallResult res = DirectoryHandler.GetDirectoryHandlers(null, out oHandlerList, null);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail with null ConnectionServerRest passed to it");

        }

        [TestMethod]
        public void AddDirectoryHandler_NullConnectionServer_Failure()
        {
            DirectoryHandler oHandler;
            var res = DirectoryHandler.AddDirectoryHandler(null, "display name", true, null, out oHandler);
            Assert.IsFalse(res.Success, "Calling AddHandler with null ConnectionServerRest did not fail");

            }

        [TestMethod]
        public void AddDirectoryHandler_EmptyDisplayName_Failure()
        {
            DirectoryHandler oHandler;
            var res = DirectoryHandler.AddDirectoryHandler(_mockServer, "", true, null, out oHandler);
            Assert.IsFalse(res.Success, "Calling AddHandler with empty display name did not fail");
        }


        [TestMethod]
        public void DeleteDirectoryHandler_NullConnectionServer_Failure()
        {
            var res = DirectoryHandler.DeleteDirectoryHandler(null, "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with null ConnectionServerRest did not fail");
        }


        [TestMethod]
        public void DeleteDirectoryHandler_EmptyObjectId_Failure()
        {
            var res = DirectoryHandler.DeleteDirectoryHandler(_mockServer, "");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with empty objectId did not fail");
        }

        [TestMethod]
        public void UpdateDirectoryHandler_NullConnectionServer_Failure()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(null, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with null ConnectionServerRest did not fail");

            }

        [TestMethod]
        public void UpdateDirectoryHandler_NullPropertyList_Failure()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with null property list did not fail");

            }

        [TestMethod]
        public void UpdateDirectoryHandler_EmptyObjectId_Failure()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with empty objectid did not fail");

            }

        [TestMethod]
        public void UpdateDirectoryHandler_EmptyPropertyList_Failure()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(_mockServer, "objectid", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with empty property list did not fail");
        }


        [TestMethod]
        public void GetDirectoryHandler_NullConnectionServer_Failure()
        {
            DirectoryHandler oHandler;

            WebCallResult res = DirectoryHandler.GetDirectoryHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ConnectionServerRest is null");
        }

        [TestMethod]
        public void GetDirectoryHandler_BlankNameAndObjectId_Failure()
        {
            DirectoryHandler oHandler;
            var res = DirectoryHandler.GetDirectoryHandler(out oHandler, _mockServer);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ObjectId and display name are both blank");
        }


        [TestMethod]
        public void GetGreetingStreamFiles_NullConnectionServer_Failure()
        {
            List<DirectoryHandlerGreetingStreamFile> oStreams;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(null, "objectid", out oStreams);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFiles with null ConnectionServer should fail");
        }


        [TestMethod]
        public void GetGreetingStreamFiles_EmptyObjectId_Failure()
        {
            List<DirectoryHandlerGreetingStreamFile> oStreams;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(_mockServer, "", out oStreams);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFiles with empty objectId should fail");
        }

        [TestMethod]
        public void GetGreetingWavFile_NullConnectionServer_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "streamname.wav");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with null ConnectionServer should fail");

         }

        [TestMethod]
        public void GetGreetingWavFile_EmptyWavPath_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "", "streamname.wav");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty local wav path should fail");
        }

        [TestMethod]
        public void GetGreetingWavFile_EmptyStreamName_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "c:\\temp.wav", "");
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty stream name should fail");
        }

        [TestMethod]
        public void GetGreetingWavFile2_NullConnectionServer_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with null ConnectionServer should fail");
        }

        [TestMethod]
        public void GetGreetingWavFile2_EmptyWavPath_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty local wav file path should fail");

            }

        [TestMethod]
        public void GetGreetingWavFile2_EmptyObjectId_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_mockServer, "c:\\temp.wav", "", 1033);
            Assert.IsFalse(res.Success, "Calling GetGreetinWavFile with empty objectId should fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_NullConnectionServer_Failure()
        {
            //SetGreetingWavFile
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(null, "objectid", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with null ConnectionServer should fail");
         }

        [TestMethod]
        public void SetGreetingWavFile_InvalidLocalWavFilePath_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_mockServer, "objectid", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with invalid local wav file should fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_EmptyObjectId_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_mockServer, "", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with Empty ObjectId should fail");
        }

        [TestMethod]
        public void SetGreetingWavFile_EmptyWavFilePath_Failure()
        {
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_mockServer, "objectid", 1033, "", true);
            Assert.IsFalse(res.Success, "calling SetGreetingWavFile with empty local wav file path should fail");
        }


        [TestMethod]
        public void SetGreetingRecordingToStreamFile_NullConnectionServer_Failure()
        {
            var res = DirectoryHandler.SetGreetingRecordingToStreamFile(null, "streamname", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with null ConnectionServerRest did not fail");
         }


        [TestMethod]
        public void SetGreetingRecordingToStreamFile_EmptyStreamId_Failure()
        {
            var res = DirectoryHandler.SetGreetingRecordingToStreamFile(_mockServer, "", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with empty streamID did not fail");
        }


        [TestMethod]
        public void SetGreetingRecordingToStreamFile_EmptyObjectId_Failure()
        {
            var res = DirectoryHandler.SetGreetingRecordingToStreamFile(_mockServer, "streamname", "", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with empty objecTId did not fail");
        }


        [TestMethod]
        public void GetGreetingStreamFile_NullConnectionServer_Failure()
        {
            DirectoryHandlerGreetingStreamFile oStream;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(null, "objectid", 1033, out oStream);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFile with null ConnectionServer should fail");
        }

        [TestMethod]
        public void GetGreetingStreamFile_EmptyObjectId_Failure()
        {
            DirectoryHandlerGreetingStreamFile oStream;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(_mockServer, "", 1033, out oStream);
            Assert.IsFalse(res.Success, "Calling GetGreetingStreamFile with empty objectId should fail");
        }


        #endregion

    }
}
