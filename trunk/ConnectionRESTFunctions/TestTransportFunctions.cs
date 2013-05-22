using System;
using System.Collections.Generic;

namespace Cisco.UnityConnection.RestFunctions
{
    class TestTransportFunctions :IConnectionRestCalls
    {
        public bool DebugMode { get; set; }
        public int HttpTimeoutSeconds { get; set; }
        public event RestTransportFunctions.LoggingEventHandler ErrorEvents;
        public event RestTransportFunctions.LoggingEventHandler DebugEvents;

        public List<T> GetObjectsFromJson<T>(string pJson, string pTypeNameOverride = "")
        {
            throw new NotImplementedException();
        }

        public T GetObjectFromJson<T>(string pJson, string pTypeNameOverride = "") where T : new()
        {
            throw new NotImplementedException();
        }

        public WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServer pConnectionServer, string pRequestBody,
                                             bool pJsonResponse = true)
        {
            throw new NotImplementedException();
        }

        public WebCallResult GetCupiResponse(string pUrl, MethodType pMethod, ConnectionServer pConnectionServer,
                                             Dictionary<string, string> pRequestDictionary)
        {
            throw new NotImplementedException();
        }

        public WebCallResult DownloadWavFile(ConnectionServer pConnectionServer, string pLocalWavFilePath, string pConnectionFileName)
        {
            throw new NotImplementedException();
        }

        public WebCallResult DownloadMessageAttachment(string pBaseUrl, ConnectionServer pConnectionServer, string pLocalWavFilePath,
                                                       string pUserObjectId, string pMessageObjectId, int pAttachmentNumber)
        {
            throw new NotImplementedException();
        }

        public WebCallResult UploadWavFile(string pFullResourcePath, ConnectionServer pConnectionServer, string pLocalWavFilePath)
        {
            throw new NotImplementedException();
        }

        public WebCallResult UploadBroadcastMessage(ConnectionServer pConnectionServer, string pWavFilePath, DateTime pStartDate,
                                                    DateTime pStartTime, DateTime pEndDate, DateTime pEndTime)
        {
            throw new NotImplementedException();
        }

        public WebCallResult UploadVoiceMessageWav(ConnectionServer pConnectionServer, string pPathToLocalWav,
                                                   string pMessageDetailsJsonString, string pSenderUserObjectId,
                                                   string pRecipientJsonString, string pUriConstruction = "")
        {
            throw new NotImplementedException();
        }

        public WebCallResult UploadVoiceMessageResourceId(ConnectionServer pConnectionServer, string pResourceId,
                                                          string pMessageDetailsJsonString, string pSenderUserObjectId,
                                                          string pRecipientJsonString, string pUriConstruction = "")
        {
            throw new NotImplementedException();
        }

        public WebCallResult UploadWavFileToStreamLibrary(ConnectionServer pConnectionServer, string pLocalWavFilePath,
                                                          out string pConnectionStreamFileName)
        {
            throw new NotImplementedException();
        }
    }
}
