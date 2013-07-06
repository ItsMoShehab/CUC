using System;
using System.Collections.Generic;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cisco.UnityConnection.RestFunctions;

namespace ConnectionCUPIFunctionsTest
{
    /// <summary>
    ///This is a test class for InterviewHandlerUnitTests and is intended
    ///to contain all InterviewHandler Unit Tests
    ///</summary>
    [TestClass]
    public class InterviewHandlerUnitTests : BaseUnitTests
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


        #region Interview Handler Class Construction Failures

        /// <summary>
        /// Make sure an ArgumentException is thrown if a null ConnectionServer is passed in.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnectionServer_Failure()
        {
            InterviewHandler oTestInterviewer = new InterviewHandler(null);
            Console.WriteLine(oTestInterviewer);
        }

        #endregion


        #region Interview Question Class Construction Errors
        
        /// <summary>
        /// Throw an ArgumentException for null Connection server
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Question_Consturctor_NullConnectionServer_Failure()
        {
            var oTest = new InterviewQuestion(null, "bogus", 1);
            Console.WriteLine(oTest);
        }

        /// <summary>
        /// Throw an ARgumentException for an empty objectId
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Question_Constructor_EmptyObjectId_Failure()
        {
            var oTest = new InterviewQuestion(_mockServer, "", 1);
            Console.WriteLine(oTest);
        }

        #endregion


        #region Interview Handler Tests

        [TestMethod]
        public void AddInterviewHandler_NullConnectionServer_Failure()
        {
            InterviewHandler oInterviewer;

            var res = InterviewHandler.AddInterviewHandler(null, "display name", "", "", null, out oInterviewer);
            Assert.IsFalse(res.Success,
                           "Calling static method AddInterviewHandler did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void AddInterviewHandler_EmptyObjectId_Failure()
            {
               InterviewHandler oInterviewer;
               var res = InterviewHandler.AddInterviewHandler(_mockServer, "", "bogus", "", null, out oInterviewer);
               Assert.IsFalse(res.Success,"Calling static method AddInterviewHandler did not fail with: empty objectid ");
            }

        [TestMethod]
        public void AddInterviewHandler_EmptyRecipientObjectIds_Failure()
        {
            InterviewHandler oInterviewer;
            var res = InterviewHandler.AddInterviewHandler(_mockServer, "bogus", "", "", null, out oInterviewer);
            Assert.IsFalse(res.Success, "Calling static method AddInterviewHandler did not fail with: empty recipient objectIds");
        }


        [TestMethod]
        public void UpdateInterviewHandler_NullConnectionServer_Failure()
        {
            var res = InterviewHandler.UpdateInterviewHandler(null, "objectId", null);
            Assert.IsFalse(res.Success,"Calling static method UpdateInterviewHandler did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void UpdateInterviewHandler_EmptyObjectId_Failure()
        {
            var res = InterviewHandler.UpdateInterviewHandler(_mockServer, "", null);
            Assert.IsFalse(res.Success, "Calling static method UpdateInterviewHandler did not fail with: empty object id");

         }

        [TestMethod]
        public void UpdateInterviewHandler_NullPropertyList_Failure()
        {
            var res = InterviewHandler.UpdateInterviewHandler(_mockServer, "ObjectId", null);
            Assert.IsFalse(res.Success, "Calling static method UpdateInterviewHandler did not fail with: null property list");
        }

        [TestMethod]
        public void UpdateInterviewHandler_EmptyPropertyList_Failure()
        {
            var res = InterviewHandler.UpdateInterviewHandler(_mockServer, "ObjectId", new ConnectionPropertyList());
            Assert.IsFalse(res.Success, "Calling static method UpdateInterviewHandler did not fail with: empty property list");
        }
 
        [TestMethod]
        public void DeleteInterviewHandler_StaticMethodFailures()
        {
            var res = InterviewHandler.DeleteInterviewHandler(null, "objectid");
            Assert.IsFalse(res.Success, "Calling static method DeleteInterviewHandler did not fail with: null ConnectionServer");

        }


        [TestMethod]
        public void GetInterviewHandler_NullConnectionServer_Failure()
        {
            InterviewHandler oInterviewer;

            var res = InterviewHandler.GetInterviewHandler(out oInterviewer, null, "objectId", "DisplayName");
            Assert.IsFalse(res.Success,"Calling static method GetInterviewHandler did not fail with: null ConnectionServer");
        }

        [TestMethod]
        public void GetInterviewHandler_EmptyObjectIdAndDisplayName_Failure()
          {
              InterviewHandler oInterviewer;
              var res = InterviewHandler.GetInterviewHandler(out oInterviewer, _mockServer);
              Assert.IsFalse(res.Success, "Calling static method GetInterviewHandler did not fail with: empty objectID and display name");
          }

        [TestMethod]
        public void GetInterviewHandlers_NullConnectionServer_Failure()
        {
            List<InterviewHandler> oHandlers;
            WebCallResult res = InterviewHandler.GetInterviewHandlers(null, out oHandlers);
            Assert.IsFalse(res.Success,
                           "Calling static method GetInterviewHandlers did not fail with: null ConnectionServer");
        }


        [TestMethod]
        public void GetInterviewHandlerVoiceName_NullConnectionServer_Failure()
        {
            WebCallResult res = InterviewHandler.GetInterviewHandlerVoiceName(null, "c:\\test.wav", "objectId");
            Assert.IsFalse(res.Success, "Fetching interview handler voice name did not fail with null connection server");
        }

        [TestMethod]
        public void GetInterviewHandlerVoiceName_EmptyObjectId_Failure()
        {
            var res = InterviewHandler.GetInterviewHandlerVoiceName(_mockServer, "c:\\test.wav", "");
            Assert.IsFalse(res.Success, "Fetching interview handler voice name did not fail with empty objectid");
        }

        [TestMethod]
        public void GetInterviewHandlerVoiceName_EmptyWavFilePath_Failure()
        {
            var res = InterviewHandler.GetInterviewHandlerVoiceName(_mockServer, "", "bogus");
            Assert.IsFalse(res.Success, "Fetching interview handler voice name did not fail with empty path");
        }

        [TestMethod]
        public void SetInterviewHandlerVoiceName_NullConnectionServer_Failure()
        {
            var res = InterviewHandler.SetInterviewHandlerVoiceName(null, "c:\\test.wav", "objectid", true);
            Assert.IsFalse(res.Success, "Setting interview handler voice name did not fail with null connection server");
        }

        [TestMethod]
        public void SetInterviewHandlerVoiceName_EmptyObjectId_Failure()
        {
            var res = InterviewHandler.SetInterviewHandlerVoiceName(_mockServer, "c:\\test.wav", "", true);
            Assert.IsFalse(res.Success, "Setting interview handler voice name did not fail with empty objectid");
        }

        [TestMethod]
        public void SetInterviewHandlerVoiceName_EmptyWavFilePath_Failure()
        {
            var res = InterviewHandler.SetInterviewHandlerVoiceName(_mockServer, "", "objectid", true);
            Assert.IsFalse(res.Success, "Setting interview handler voice name did not fail with empty wav file path");
        }

        [TestMethod]
        public void SetInterviewHandlerVoiceNameToStreamFile_NullConnectionServer_Failure()
        {
            var res = InterviewHandler.SetInterviewHandlerVoiceNameToStreamFile(null, "objectid", "streamid");
            Assert.IsFalse(res.Success,
                           "Setting interview handler voice name to stream file did not fail with null connection server");
        }

        [TestMethod]
        public void SetInterviewHandlerVoiceNameToStreamFile_EmptyObjectId_Failure()
        {
            var res = InterviewHandler.SetInterviewHandlerVoiceNameToStreamFile(_mockServer, "", "streamid");
            Assert.IsFalse(res.Success, "Setting interview handler voice name to stream file did not fail with empty objectid");
        }

        [TestMethod]
        public void SetInterviewHandlerVoiceNameToStreamFile_EmptyStreamId_Failure()
        {
            var res = InterviewHandler.SetInterviewHandlerVoiceNameToStreamFile(_mockServer, "objectid", "");
            Assert.IsFalse(res.Success, "Setting interview handler voice name to stream file did not fail with empty stream id");
        }

        [TestMethod]
        public void GetInterviewHandler_BlankObjectIdAndName_Failure()
        {
            InterviewHandler oHandler;

            var res = InterviewHandler.GetInterviewHandler(out oHandler, _mockServer);
            Assert.IsFalse(res.Success, "GetInterviewHandler should fail if the ObjectId and display name are both blank");
        }

        #endregion


        #region Interview Question Tests

        [TestMethod]
        public void GetInterviewHandlerQuestionRecording_NullConnectionServer_Failure()
        {
            //GetInterviewHandlerQuestionRecording
            var res = InterviewQuestion.GetInterviewHandlerQuestionRecording(null, "c:\\test.wav", "objectid", 1);
            Assert.IsFalse(res.Success, "Calling GetInterviewHandlerQuestionRecording with null ConnectionServer did not fail");
        }

        [TestMethod]
        public void GetInterviewHandlerQuestionRecording_EmptyWavFilePath_Failure()
        {
            var res = InterviewQuestion.GetInterviewHandlerQuestionRecording(_mockServer, "", "objectid", 1);
            Assert.IsFalse(res.Success, "Calling GetInterviewHandlerQuestionRecording with empty wav path did not fail");
        }

        [TestMethod]
        public void GetInterviewHandlerQuestionRecording_InvalidWavFilePath_Failure()
        {
            var res = InterviewQuestion.GetInterviewHandlerQuestionRecording(_mockServer, "c:\\bogus\\bogus\\temp.wav","objectid", 1);
            Assert.IsFalse(res.Success, "Calling GetInterviewHandlerQuestionRecording with invalid file path did not fail");
        }

        [TestMethod]
        public void GetInterviewHandlerQuestionRecording_EmptyObjectId_Failure()
        {
            var res = InterviewQuestion.GetInterviewHandlerQuestionRecording(_mockServer, "c:\\test.wav", "", 1);
            Assert.IsFalse(res.Success, "Calling GetInterviewHandlerQuestionRecording with empty object id did not fail");
        }

        [TestMethod]
        public void GetInterviewQuestion_NullConnectionServer_Failure()
        {
            InterviewQuestion oQuestion;
            var res = InterviewQuestion.GetInterviewQuestion(out oQuestion, null, "objectid", 1);
            Assert.IsFalse(res.Success, "Calling GetInterviewQuestion with did not fail");

        }

        [TestMethod]
        public void GetInterviewQuestion_EmptyObjectId_Failure()
        {
            InterviewQuestion oQuestion;

            var res = InterviewQuestion.GetInterviewQuestion(out oQuestion, _mockServer, "", 1);
            Assert.IsFalse(res.Success, "Calling GetInterviewQuestion with did not fail");
         }


        [TestMethod]
        public void GetInterviewQuestions_NullConnectionServer_Failure()
        {
            List<InterviewQuestion> oQuestions;
            var res = InterviewQuestion.GetInterviewQuestions(null, "objectid", out oQuestions);
            Assert.IsFalse(res.Success, "Calling GetInterviewQuestion with did not fail");

        }
        [TestMethod]
        public void GetInterviewQuestions_EmptyObjectId_Failure()
        {
            List<InterviewQuestion> oQuestions;
            var res = InterviewQuestion.GetInterviewQuestions(_mockServer, "", out oQuestions);
            Assert.IsFalse(res.Success, "Calling GetInterviewQuestion with did not fail");
        }

        [TestMethod]
        public void SetInterviewHandlerQuestionRecording_NullConnectionServer_Failure()
        {
            var res = InterviewQuestion.SetInterviewHandlerQuestionRecording(null, "c:\\temp.wav", "objectid", 1, true);
            Assert.IsFalse(res.Success, "Calling SetInterviewHandlerQuestionRecording with null ConnectionServer did not fail");
        }

        [TestMethod]
        public void SetInterviewHandlerQuestionRecording_EmptyObjectId_Failure()
        {
            var res = InterviewQuestion.SetInterviewHandlerQuestionRecording(_mockServer, "bogus.wav", "", 1, true);
            Assert.IsFalse(res.Success, "Calling SetInterviewHandlerQuestionRecording with empty objectId did not fail");
        }

        [TestMethod]
        public void SetInterviewHandlerQuestionRecording_EmptyWavFilePath_Failure()
        {
            var res = InterviewQuestion.SetInterviewHandlerQuestionRecording(_mockServer, "", "objectId", 1, true);
            Assert.IsFalse(res.Success, "Calling SetInterviewHandlerQuestionRecording with empty objectId did not fail");
        }

        [TestMethod]
        public void SetInterviewHandlerQuestionRecording_InvalidWavFilePath_Failure()
        {
            var res = InterviewQuestion.SetInterviewHandlerQuestionRecording(_mockServer, "bogus.wav", "objectId", 1, true);
            Assert.IsFalse(res.Success, "Calling SetInterviewHandlerQuestionRecording with empty objectId did not fail");
        }


        [TestMethod]
        public void SetInterviewHandlerQuestionRecordingToStreamFile_NullConnectionServer_Failure()
        {
            var res = InterviewQuestion.SetInterviewHandlerQuestionRecordingToStreamFile(null, "objectid", 1, "streamid");
            Assert.IsFalse(res.Success, "Calling SetInterviewHandlerQuestionRecordingToStreamFile with null ConnectionServer did not fail");
        }

        [TestMethod]
        public void SetInterviewHandlerQuestionRecordingToStreamFile_EmptyObjectId_Failure()
        {
            var res = InterviewQuestion.SetInterviewHandlerQuestionRecordingToStreamFile(_mockServer, "", 1, "streamid");
            Assert.IsFalse(res.Success, "Calling SetInterviewHandlerQuestionRecordingToStreamFile with empty objectId did not fail");
        }

        [TestMethod]
        public void SetInterviewHandlerQuestionRecordingToStreamFile_EmptyStreamId_Failure()
        {
            var res = InterviewQuestion.SetInterviewHandlerQuestionRecordingToStreamFile(_mockServer, "ObjectId", 1, "");
            Assert.IsFalse(res.Success, "Calling SetInterviewHandlerQuestionRecordingToStreamFile with empty streamId did not fail");
        }


        [TestMethod]
        public void UpdateInterviewHandlerQuestion_NullConnectionServer_Failure()
        {
            var res = InterviewQuestion.UpdateInterviewHandlerQuestion(null, "objectid", 1, true, 1, "test");
            Assert.IsFalse(res.Success, "Calling UpdateInterviewHandlerQuestion with did not fail");
        }

        [TestMethod]
        public void UpdateInterviewHandlerQuestion_EmptyOBjectId_Failure()
        {
            var res = InterviewQuestion.UpdateInterviewHandlerQuestion(_mockServer, "", 1, true, 1, "test");
            Assert.IsFalse(res.Success, "Calling UpdateInterviewHandlerQuestion with empty objectid did not fail");
        }

        #endregion
    }
}
