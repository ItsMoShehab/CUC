﻿#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Threading;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// When initiating a TRAP call on 10.5.2 or later you can indicate it's a video or audio only call.
    /// </summary>
    public enum CallType {Audio, Video}
    
    /// <summary>
    /// Allows you to use the phone as a media device.  Recordings created via this class are stored in the media recordings
    /// table on the Connection server where you can apply them to voice names, greetings or message attachments as needed.
    /// This is an alternative to uploading a local WAV file from the hard drive via HTTP.
    /// </summary>
    public class PhoneRecording : IDisposable
    {

        #region Fields and Properties

        private ConnectionServerRest _homeServer;
        private readonly string _phoneNumber;
        private readonly CallType _callType;
        private readonly int _rings = 4;
        private int _callId;

        /// <summary>
        /// The RecordingResourceId of the last recording done on the call session.  Multiple recordings can be done in a single
        /// call session, this Id holds the last recording to indicate success.
        /// </summary>
        public string RecordingResourceId { get; private set; }

        /// <summary>
        /// In Unity Connection 10.5.2 and later video recording is an option via the phone - if it's a video recording both a RecordingResourceId
        /// and a SessionId will be returned - both are needed for updating the greeting and initiating a playback.
        /// </summary>
        public string RecordingSessionId { get; private set; }

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Constructor for the PhoneRecording class - this sets up the phone connection, dials the phone and waits for it to 
        /// connect or fail.  Only if the call completes and the connection is established does this construtor complete without
        /// throwing an exception.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server to have call your phone.
        /// </param>
        /// <param name="pPhoneNumberToDial">
        /// The phone number to dial
        /// </param>
        /// <param name="pRings">
        /// The number of rings to wait for - defaults to 4
        /// </param>
        /// <param name="pCallType">
        /// In Connection 10.5.2 and later video recording and playback is possible if the system is configured properly.  Defaults
        /// to audio only
        /// </param>
        public PhoneRecording(ConnectionServerRest pConnectionServer, string pPhoneNumberToDial, int pRings=4,CallType pCallType = CallType.Audio )
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null Connection Server passed to the PhoneRecording constructor");
            }

            if (string.IsNullOrEmpty(pPhoneNumberToDial))
            {
                throw new ArgumentException("Empty phone number passed the PhoneRecording constructor");
            }

            _homeServer = pConnectionServer;
            _phoneNumber = pPhoneNumberToDial;
            _callType = pCallType;
            _rings = pRings;

            WebCallResult res = AttachToPhone();
            int iCounter = 0;
            if (res.Success)
            {
                Thread.Sleep(200);
                if (IsCallConnected())
                {
                    return;
                }
                iCounter++;
                if (iCounter > 4)
                {
                    throw new UnityConnectionRestException(res, "Failed to connect to phone" + res);
                }
            }

            throw new UnityConnectionRestException(res,"Failed to connect to phone"+res);
        }

        #endregion


        #region Instance Methods

        /// <summary>
        ///     Attach to phone device which will act as our media recording/playback interface.
        /// </summary>
        /// <returns>
        ///     A ConnectionCUPIClientFunctions.WebCallResult value...
        /// </returns>
        private WebCallResult AttachToPhone()
        {
            string strUrl = string.Format("{0}calls", _homeServer.BaseUrl);

            Dictionary<string,string> oParams = new Dictionary<string, string>();
           
            oParams.Add("number",_phoneNumber);
            oParams.Add("maximumRings", _rings.ToString());

            if (_homeServer.Version.IsVersionAtLeast(10, 5, 2, 0))
            {
                oParams.Add("callType", _callType.ToString());
            }
 
            WebCallResult res = _homeServer.GetCupiResponse(strUrl, MethodType.POST, oParams);
            if (res.Success==false)
            {
                return res;
            }

            //the response will be in the form of:
            // "vmrest/calls/2"
            // where "2" there can be different - this is the call ID we need to use for this instance of the phone recording 
            // class as there may be many calls active on the server at the same time.

            res.Success = false;
            res.ErrorText = "Failed to get CallId back on phone call creation - could not parse from response text.";

            if (string.IsNullOrEmpty(res.ResponseText) || res.ResponseText.Contains("vmrest/calls/")==false)
            {
                //oops
                return res;
            }

            //get the trailing number after the last "/"
            int iPos = res.ResponseText.LastIndexOf('/');
            int iPos2 = res.ResponseText.LastIndexOf('\n');
            if (iPos2 < 0) iPos2 = res.ResponseText.Length-1;

            if (res.ResponseText.Length-1<=iPos)
            {
                return res;
            }

            string strId = res.ResponseText.Substring(iPos + 1, iPos2 - iPos);

            if (int.TryParse(strId,out _callId)==false)
            {
                return res;
            }

            //if we requested video and didn't get it, fail the call.
            if (_homeServer.Version.IsVersionAtLeast(10, 5, 2, 0))
            {
                object oValue;
                if (res.JsonDictionary.TryGetValue("callType", out oValue))
                {
                    if (!oValue.ToString().ToLower().Equals(_callType.ToString().ToLower()))
                    {
                        res.ErrorText = "Failed to get video chanel on TRAP request";
                        return res;
                    }
                }
            }

            res.Success = true;
            res.ErrorText = "";
            return res;
        }



        /// <summary>
        ///  Checks to see if the current callId we got back in the constructor is still connected to a phone.  If the user 
        /// hangs up or the like we wont know about it unless we check. 
        /// </summary>
        /// <returns>
        ///  true if the call is still connected, false if not.
        /// </returns>
        public bool IsCallConnected()
        {
            string strUrl = string.Format("{0}calls/{1}", _homeServer.BaseUrl,_callId);

            WebCallResult res = _homeServer.GetCupiResponse(strUrl, MethodType.GET, "");
            if (res.Success == false || res.JsonDictionary==null)
            {
                return false;
            }

            object oValue;
            if (res.JsonDictionary.TryGetValue("connected", out oValue))
            {
                if (oValue.ToString().Equals("true"))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///  Private method called by the dispose function for this class - asks Connection to hang up the call.  This will not
        /// throw an error if the call is not active.
        /// </summary>
        public void HangUp()
        {
            string strUrl = string.Format("{0}calls/{1}", _homeServer.BaseUrl, _callId);

            _homeServer.GetCupiResponse(strUrl, MethodType.DELETE, "");
        }


        /// <summary>
        /// Starts a new recording on the current call session.  Each time this is called the RecordingResourceId property
        /// on this instance is updated to a new recording ID.  You can create many recordings on the same call if you like.
        /// This is a blocking call - it will not return until the recording is terminated (by the user pressing pound or 
        /// hanging up).  Depending on your application you may want to call this off a background thread.
        /// </summary>
        /// <returns>
        /// A WebCallResult instance containing the results of the call.
        /// </returns>
        public WebCallResult RecordStreamFile()
        {
            string strUrl = string.Format("{0}calls/{1}", _homeServer.BaseUrl,_callId);

            Dictionary<string, string> oParams = new Dictionary<string, string>();

            oParams.Add("op", "RECORD");

            //the results from the call are returned in a string/object pair dictionary
            WebCallResult res = _homeServer.GetCupiResponse(strUrl, MethodType.POST, oParams);

            if (res.Success == false)
            {
                return res;
            }

            if (res.JsonDictionary == null)
            {
                res.Success = false;
                res.ErrorText = "JsonDictionary returned as null";
                return res;
            }

            //pull the values out of the result set returned from the call (if any).  In this case we need to see
            // the "lastResult and "resourceId" values returned or we'll consider it a failure.
            object oValue;
            res.Success = false;
            if (res.JsonDictionary.ContainsKey("lastResult") == false | res.JsonDictionary.ContainsKey("resourceId") == false)
            {
                res.ErrorText = "No Result or resoruce ID returned from play in the response text";
                return res;
            }

            res.JsonDictionary.TryGetValue("lastResult", out oValue);
            if (oValue == null || oValue.ToString().Equals("0")==false)
            {
                res.ErrorText = "Empty or null Result returned from play";
                return res;
            }

            res.JsonDictionary.TryGetValue("resourceId", out oValue);
            if (oValue==null || string.IsNullOrEmpty(oValue.ToString()))
            {
                res.ErrorText = "Invalid resource ID resturned from play";
                return res;
            }
            RecordingResourceId = oValue.ToString();

            res.JsonDictionary.TryGetValue("sessionId", out oValue);
            if (oValue == null || string.IsNullOrEmpty(oValue.ToString()))
            {
                //no error - may not be a video enabled system.
            }
            else
            {
                RecordingSessionId = oValue.ToString();    
            }

            res.Success = true;
            return res;
        }


        /// <summary>
        /// Plays a stream file out of the current call session.  You can pass in any stream file Id you like (i.e. from the vw_StreamFiles view)
        /// or you can pass in blank for that parameter and it will use the current stream file Id if you've just done a recording for instance.
        /// This is a blocking call, it will not return until the stream has completed playing or the call is terminated.  Depending on your 
        /// application you may want to call this from a background thread.
        /// </summary>
        /// <param name="pResourceId">
        /// The stream file Id to play out the phone interface.
        /// </param>
        /// <param name="pSessionId">
        /// if it's a video recording a sessionID is required in addition to the resource Id to play back.
        /// </param>
        /// <param name="pSpeed">
        /// Speed value - 50, 100, 150, 200 - 100 is normal speed, 50 is half etc...
        /// </param>
        /// <param name="pVolume">
        /// Volume value - 50, 100, 150, 200 - 100 is normal volume, 50 is half etc...
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with the results of the call.
        /// </returns>
        public WebCallResult PlayStreamFile(string pResourceId="", string pSessionId="", int pSpeed=100, int pVolume = 100)
        {
            WebCallResult res = new WebCallResult();

            if (string.IsNullOrEmpty(pResourceId) && string.IsNullOrEmpty(RecordingResourceId))
            {
                res.Success = false;
                res.ErrorText =
                    "No recording resource Id passed in and no recording is associated with the active session";
                return res;
            }

            string strStreamFileId = string.IsNullOrEmpty(pResourceId) ? RecordingResourceId : pResourceId;

            string strUrl = string.Format("{0}calls/{1}", _homeServer.BaseUrl, _callId);

            Dictionary<string, string> oParams = new Dictionary<string, string>();

            oParams.Add("op", "PLAY");
            oParams.Add("resourceType", "STREAM");
            oParams.Add("resourceId", strStreamFileId);
            
            if (!string.IsNullOrEmpty(pSessionId))
            {
                oParams.Add("sessionId",pSessionId);
            }
            
            oParams.Add("speed", pSpeed.ToString());
            oParams.Add("volume", pVolume.ToString());
            oParams.Add("startPosition", "0");
            oParams.Add("lastResult", "0");

            res = _homeServer.GetCupiResponse(strUrl, MethodType.POST, oParams);

            if (res.Success == false)
            {
                return res;
            }

            //the only value we're interested in here is the lastResult - if it's 0 then playback finished ok.  
            object oValue;

            res.JsonDictionary.TryGetValue("lastResult", out oValue);
            if (oValue==null || oValue.ToString().Equals("0") == false)
            {
                res.Success = false;
                if (oValue != null)
                {
                    res.ErrorText = "Result returned from play=" + oValue;
                }
                else
                {
                    res.ErrorText = "Empty result returned from play";
                }
                return res;
            }

            return res;
        }


        /// <summary>
        /// Plays a message file out of the current call session.  You can pass in any message Id you like from a mailbox you have rights to play 
        /// messages for - typically this is only used for users logged into CUPI as themselves but the delegate mailbox role should also give you 
        /// rights to other messages as well.
        /// </summary>
        /// <param name="pMessageId">
        /// The message ID to play out the phone interface.  THis is not just the ObjectId of the Message - it's the ID generated from the REST 
        /// mailbox interface so it's preceeded with the file attachment number and a colon first.
        /// </param>
        /// <param name="pSpeed">
        /// Speed value - 50, 100, 150, 200 - 100 is normal speed, 50 is half etc...
        /// </param>
        /// <param name="pVolume">
        /// Volume value - 50, 100, 150, 200 - 100 is normal volume, 50 is half etc...
        /// </param>
        /// <param name="pStartPosition">
        /// Starting play position in milliseconds, defaults to 0 for the beginning of the file.  If you pass a value that exceeds the size of 
        /// the message being played the play simply wont start.
        ///  </param>
        /// <param name="pUserObjectId">
        /// If running the phone interface as an administrator you need to pass the ObjectId of the user the message 
        /// belongs to here - if using the "play my own message" type scenario, this is not necessary.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class with the results of the call.
        /// </returns>
        public WebCallResult PlayMessageFile(string pMessageId = "", int pSpeed = 100, int pVolume = 100, int pStartPosition=0,
            string pUserObjectId="")
        {
            WebCallResult res = new WebCallResult();

            if (string.IsNullOrEmpty(pMessageId) && string.IsNullOrEmpty(RecordingResourceId))
            {
                res.Success = false;
                res.ErrorText =
                    "No recording resource Id passed in and no recording is associated with the active session";
                return res;
            }

            string strStreamFileId = string.IsNullOrEmpty(pMessageId) ? RecordingResourceId : pMessageId;

            string strUrl = string.Format("{0}calls/{1}", _homeServer.BaseUrl, _callId);

            if (!string.IsNullOrEmpty(pUserObjectId))
            {
                strUrl += "?userobjectid=" + pUserObjectId;
            }
            Dictionary<string, string> oParams = new Dictionary<string, string>();

            oParams.Add("op", "PLAY");
            oParams.Add("resourceType", "MESSAGE");
            oParams.Add("resourceId", strStreamFileId);
            oParams.Add("speed", pSpeed.ToString());
            oParams.Add("volume", pVolume.ToString());
            oParams.Add("startPosition", pStartPosition.ToString());
            oParams.Add("lastResult", "0");

            res = _homeServer.GetCupiResponse(strUrl, MethodType.POST, oParams);

            if (res.Success == false)
            {
                return res;
            }

            //the only value we're interested in here is the lastResult - if it's 0 then playback finished ok.  
            object oValue;

            if (res.JsonDictionary == null)
            {
                _homeServer.RaiseErrorEvent("Error playing back message via CUTI, null value returned for last result");
                res.Success = false;
                res.ErrorText = "Null result returned from play";
                return res;
            }

            res.JsonDictionary.TryGetValue("lastResult", out oValue);
            if (oValue==null)
            {
                _homeServer.RaiseErrorEvent("Error playing back message via CUTI, null value returned for last result");
                res.Success = false;
                res.ErrorText = "Null result returned from play";
                return res;
            }
            if (oValue.ToString().Equals("0") == false)
            {
                res.Success = false;
                res.ErrorText = "Result returned from play=" + oValue;
                return res;
            }

            return res;
        }

        /// <summary>
        /// issue a hangup before disposing an instance.  No error is thrown here if there is no active call.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HangUp();
                _homeServer = null;
            }
        }

        #endregion

    }
}
