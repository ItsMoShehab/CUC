using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for DirectoryHandlerTest and is intended
    ///to contain all DirectoryHandler Unit Tests
    ///</summary>
    [TestClass]
    public class DirectoryHandlerTest
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
                _connectionServer = new ConnectionServer(mySettings.ConnectionServer, mySettings.ConnectionLogin, mySettings.ConnectionPW);
                HTTPFunctions.DebugMode = mySettings.DebugOn;
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
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail with null ConnectionServer passed to it");

        }

        [TestMethod]
        public void StaticCallFailures_AddDirectoryHandler()
        {
            DirectoryHandler oHandler;
            var res = DirectoryHandler.AddDirectoryHandler(null, "display name", true, null, out oHandler);
            Assert.IsFalse(res.Success, "Calling AddHandler with null ConnectionServer did not fail");

            res = DirectoryHandler.AddDirectoryHandler(_connectionServer, "", true, null, out oHandler);
            Assert.IsFalse(res.Success, "Calling AddHandler with empty display name did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_DeleteDirectoryHandler()
        {
            var res = DirectoryHandler.DeleteDirectoryHandler(null, "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with null ConnectionServer did not fail");

            res = DirectoryHandler.DeleteDirectoryHandler(_connectionServer, "objectid");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with invalid objectId did not fail");

            res = DirectoryHandler.DeleteDirectoryHandler(_connectionServer, "");
            Assert.IsFalse(res.Success, "Calling DeleteDirectoryHandler with empty objectId did not fail");
        }

        [TestMethod]
        public void StaticCallFailures_UpdateDirectoryHandler()
        {
            var res = DirectoryHandler.UpdateDirectoryHandler(null, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with null ConnectionServer did not fail");

            res = DirectoryHandler.UpdateDirectoryHandler(_connectionServer, "objectid", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with null property list did not fail");

            res = DirectoryHandler.UpdateDirectoryHandler(_connectionServer, "", null);
            Assert.IsFalse(res.Success, "Calling UpdateDirectoryHandler with empty objectid did not fail");

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

            WebCallResult res = DirectoryHandler.GetDirectoryHandler(out oHandler, null);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ConnectionServer is null");

            res = DirectoryHandler.GetDirectoryHandler(out oHandler, _connectionServer);
            Assert.IsFalse(res.Success, "GetDirectoryHandler should fail if the ObjectId and display name are both blank");
        }

        [TestMethod]
        public void StaticCallFailures_GetGreetingStreamFiles()
        {
            //GetGreetingStreamFiles
            List<DirectoryHandlerGreetingStreamFile> oStreams;
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(null, "objectid", out oStreams);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(_connectionServer, "objectid", out oStreams);
            Assert.IsTrue(res.Success, "Fetching greeting stream files with an invalid objectId shouldn't fail:" + res);
            Assert.IsTrue(oStreams.Count == 0, "Fetching streams with an invalid objectId should return an empty list");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFiles(_connectionServer, "", out oStreams);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void StaticCallFailures_GetGreetingWavFile()
        {
            //GetGreetingWavFile
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "streamname.wav");
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_connectionServer, "c:\\temp.wav", "streamname.wav");
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_connectionServer, "", "streamname.wav");
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_connectionServer, "c:\\temp.wav", "");
            Assert.IsFalse(res.Success, "");


            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(null, "c:\\temp.wav", "objectId", 1033);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_connectionServer, "c:\\temp.wav", "objectId", 1033);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_connectionServer, "", "objectId", 1033);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_connectionServer, "c:\\temp.wav", "", 1033);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingWavFile(_connectionServer, "c:\\temp.wav", _tempHandler.ObjectId, 9999);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void StaticCallFailures_SetGreetingWavFile()
        {
            //SetGreetingWavFile
            var res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(null, "objectid", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_connectionServer, "objectid", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_connectionServer, "", 1033, "bogus.wav", true);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_connectionServer, "objectid", 1033, "", true);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.SetGreetingWavFile(_connectionServer, _tempHandler.ObjectId, 9999, "Dummy.wav", true);
            Assert.IsFalse(res.Success, "");
        }

        [TestMethod]
        public void StaticCallFailures_SetGreetingStreamFile()
        {
            var res = DirectoryHandler.SetGreetingRecordingToStreamFile(null, "streamname", "objectId", 1033);
            Assert.IsFalse(res.Success, "Calling SetGreetingRecordingToStreamFile with null ConnectionServer did not fail");

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
            var res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(null, "objectid", 1033, out oStream);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(_connectionServer, "objectid", 1033, out oStream);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(_connectionServer, "", 1033, out oStream);
            Assert.IsFalse(res.Success, "");

            res = DirectoryHandlerGreetingStreamFile.GetGreetingStreamFile(_connectionServer, _tempHandler.ObjectId, 9999, out oStream);
            Assert.IsFalse(res.Success, "");

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
