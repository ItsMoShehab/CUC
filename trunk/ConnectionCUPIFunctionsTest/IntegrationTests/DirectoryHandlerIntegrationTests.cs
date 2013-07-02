using System;
using System.Collections.Generic;
using System.IO;
using ConnectionCUPIFunctionsTest.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cisco.UnityConnection.RestFunctions;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for DirectoryHandlerIntegrationTests and is intended
    ///to contain all DirectoryHandler Unit Tests
    ///</summary>
    [TestClass]
    public class DirectoryHandlerIntegrationTests : BaseIntegrationTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        private static DirectoryHandler _tempHandler;

        #endregion


        #region Additional test attributes
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public new static void MyClassInitialize(TestContext testContext)
        {
            BaseIntegrationTests.MyClassInitialize(testContext);

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


        #region Static Call Failures


        [TestMethod]
        public void StaticCallFailures_DeleteDirectoryHandler()
        {

            var res = DirectoryHandler.DeleteDirectoryHandler(_connectionServer, "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with invalid objectId did not fail");

        }

        [TestMethod]
        public void StaticCallFailures_UpdateDirectoryHandler()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(_connectionServer, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with null property list did not fail");

            res = DirectoryHandler.UpdateDirectoryHandler(_connectionServer, "objectid", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with empty property list did not fail");

            res = DirectoryHandler.UpdateDirectoryHandler(_connectionServer, "objectid", new ConnectionPropertyList("name", "value"));
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with invalid objectId did not fail");
        }

        /// <summary>
        /// exercise failure points
        /// </summary>
        [TestMethod]
        public void StaticCallFailures_GetDirectoryHandler()
        {
            DirectoryHandler oHandler;

            var res = DirectoryHandler.GetDirectoryHandler(out oHandler, _connectionServer);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ObjectId and display name are both blank");
        }

        [TestMethod]
        public void StaticCallFailures_GetGreetingStreamFiles()
        {
            //GetGreetingStreamFiles
            List<DirectoryHandlerGreetingStreamFile> oStreams;

            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(_connectionServer, "objectid", out oStreams);
            Assert.IsTrue(res.Success, "Fetching greeting stream files with an invalid objectId shouldn't fail:" + res);
            Assert.IsTrue(oStreams.Count == 0, "Fetching streams with an invalid objectId should return an empty list");
        }

        [TestMethod]
        public void StaticCallFailures_GetGreetingWavFile()
        {
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_connectionServer, "Dummy.wav", _tempHandler.ObjectId, 9999);
            Assert.IsFalse(res.Success, "Getting greeting wav file with illegal language Id should fail");
        }

        [TestMethod]
        public void StaticCallFailures_SetGreetingWavFile()
        {
            //SetGreetingWavFile
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_connectionServer, "objectid", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "Setting greeting wav file with bogus ObjectId should fail");

            res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_connectionServer, _tempHandler.ObjectId, 9999, "Dummy.wav", true);
            Assert.IsFalse(res.Success, "Setting greeting wav file with illegal language code should fail");
        }

        [TestMethod]
        public void StaticCallFailures_SetGreetingStreamFile()
        {
            var res = DirectoryHandler.SetGreetingRecordingToStreamFile(null, "streamname", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with null ConnectionServerRest did not fail");

            res = DirectoryHandler.SetGreetingRecordingToStreamFile(_connectionServer, "streamname", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with invalid objectId did not fail");

            res = DirectoryHandler.SetGreetingRecordingToStreamFile(_connectionServer, "", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with empty streamID did not fail");

            res = DirectoryHandler.SetGreetingRecordingToStreamFile(_connectionServer, "streamname", "", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with empty objecTId did not fail");

            res = DirectoryHandler.SetGreetingRecordingToStreamFile(_connectionServer, "streamname", "objectId", 9999);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with invalid language ID did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_GetGreetingStreamFile()
        {
            //GetGreetingStreamFile
            DirectoryHandlerGreetingStreamFile oStream;

            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(_connectionServer, "objectid", 1033, out oStream);
            Assert.IsFalse(res.Success, "Getting greeting stream with invalie ObjectId should fail");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(_connectionServer, _tempHandler.ObjectId, 9999, out oStream);
            Assert.IsFalse(res.Success, "Getting greeting stream with invalid language code should fail");
        }


        #endregion


        #region Live Tests

        /// <summary>
        /// GET first handler in directory using static method call, iterate over it and use the ToString and DumpAllProps
        /// methods on it.
        /// For Directory handlers there should always be one in a valid Connection installation
        /// </summary>
        [TestMethod]
        public void GetDirectoryHandlers_FetchTest()
        {
            List<DirectoryHandler> oHandlerList;
            DirectoryHandler oNewHandler;

            WebCallResult res = DirectoryHandler.GetDirectoryHandlers(_connectionServer, out oHandlerList,1,1);

            Assert.IsTrue(res.Success, "Fetching of first directory handler failed: " + res.ToString());
            Assert.AreEqual(oHandlerList.Count, 1, "Fetching of the first directory handler returned a different number of handlers: " + res.ToString());

            //exercise the ToString and DumpAllProperties as part of this test as well
            foreach (DirectoryHandler oHandler in oHandlerList)
            {
                Console.WriteLine(oHandler.ToString());
                Console.WriteLine(oHandler.DumpAllProps());

                //fetch a new directory handler using the objectId 
                res = DirectoryHandler.GetDirectoryHandler(out oNewHandler, _connectionServer, oHandler.ObjectId);
                Assert.IsTrue(res.Success, "Fetching directory handler by ObjectId: " + res.ToString());
            }

            try
            {
                oNewHandler = new DirectoryHandler(_connectionServer, "", oHandlerList[0].DisplayName);
                Console.WriteLine(oNewHandler);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to create new direcotry handler using valid display name for search:"+ex);
            }

            //hit failed searches
            res = DirectoryHandler.GetDirectoryHandler(out oNewHandler, _connectionServer, "", "bogus name that shouldnt match");
            Assert.IsFalse(res.Success, "Fetching directory handler by bogus name did not fail");

            res = DirectoryHandler.GetDirectoryHandlers(_connectionServer, out oHandlerList, 1, 1, "query=(ObjectId is bogus)");
            Assert.IsTrue(res.Success, "fetching handlers with invalid query should not fail:" + res);
            Assert.IsTrue(oHandlerList.Count == 0, "Invalid query string should return an empty handler list:" + oHandlerList.Count);


        }


        [TestMethod]
        public void DirectoryHandler_EditTopLevel()
        {
            //updating without any pending changes should fail
            WebCallResult res = _tempHandler.Update();
            Assert.IsFalse(res.Success,"Calling update on newly created handler did not fail");

            _tempHandler.DisplayName = "New" + Guid.NewGuid().ToString();
            res = _tempHandler.Update();
            Assert.IsTrue(res.Success,"Updating directory handler failed:"+res);

            res = _tempHandler.RefetchDirectoryHandlerData();
            Assert.IsTrue(res.Success, "Refetching directory handler data failed:" + res);
        }

         [TestMethod]
        public void DirectoryHandler_CustomStreamTests()
         {
             WebCallResult res = _tempHandler.SetGreetingWavFile("dummy.wav",1033,true);
             Assert.IsTrue(res.Success,"Failed to upload custom directory handler greeting:"+res);

             _tempHandler.UseCustomGreeting = true;
             res = _tempHandler.Update();
             Assert.IsTrue(res.Success,"Failed to update directory handler to play custom greeting:"+res);

             res = _tempHandler.SetGreetingRecordingToStreamFile("blah", 1033);
             Assert.IsFalse(res.Success,"Setting custom stream to invalid streamId did not fail");

             List<DirectoryHandlerGreetingStreamFile> oStreamFiles = _tempHandler.GetGreetingStreamFiles();
             if (oStreamFiles == null || oStreamFiles.Count == 0)
             {
                 Assert.Fail("Fetching greeting streams after uploading one failed to return any");
             }

             Console.WriteLine(oStreamFiles[0].ToString());
             Console.WriteLine(oStreamFiles[0].DumpAllProps());

             string strFileName = Guid.NewGuid().ToString() + ".wav";
             res = oStreamFiles[0].GetGreetingWavFile(strFileName);
             Assert.IsTrue(res.Success,"Failed to download stream file as WAV:"+res);

             Assert.IsTrue(File.Exists(strFileName),"Stream file downloaded as WAV not written to disk");

             try
             {
                 File.Delete(strFileName);
             }
             catch (Exception ex)
             {
                 Assert.Fail("Temporary file failed to delete:"+ex);
             }

             res = oStreamFiles[0].SetGreetingWavFile("Dummy.wav", true);
             Assert.IsTrue(res.Success,"Failed to set greeting via DirectoryHandlerGreetingStreamFile instance method:"+res);
         }

        #endregion

    }
}
