using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    public class InterviewQuestion
    {

        #region Fields and Properties

        public int QuestionNumber { get; set; }
        public int MaxMsgLength { get; set; }
        public string StreamText { get; set; }
        public bool IsActive { get; set; }
        public string InterviewHandlerObjectId { get; set; }

        //reference to the ConnectionServer object used to create this handlers instance.
        internal ConnectionServer HomeServer { get; private set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Empty constructor for JSON parser
        /// </summary>
        public InterviewQuestion()
        {
        }

        public InterviewQuestion(ConnectionServer pHomeServer, string pInterviewHandlerObjectId, int pInterviewQuestionNumber)
        {
            if (pHomeServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to InterviewQuestion constructor");
            }
            if (string.IsNullOrEmpty(pInterviewHandlerObjectId))
            {
                throw new ArgumentException("Empty interview handler objectId passed to InterviewQuestion constructor");
            }

            var res= GetInterviewQuestion(pInterviewHandlerObjectId, pInterviewQuestionNumber);

            if (res.Success == false)
            {
                throw new Exception("Failed fetching interview question:"+res);
            }

            HomeServer = pHomeServer;
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// Returns all questions for a specific interview handler
        /// </summary>
        /// <param name="pConnectionServer">
        /// Reference to the ConnectionServer object that points to the home server where the handler questions are being fetched from.
        /// </param>
        /// <param name="pInterviewHandlerObjectId">
        /// The unique identifier for the interview handler to fetch questions for.
        /// </param>
        /// <param name="pInterviewQuestions">
        /// The list of questions for the interviewer is passed back on this out param
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetInterviewQuestions(ConnectionServer pConnectionServer, string pInterviewHandlerObjectId,
            out List<InterviewQuestion> pInterviewQuestions)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pInterviewQuestions = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetInterviewQuestions";
                return res;
            }

            if (string.IsNullOrEmpty(pInterviewHandlerObjectId))
            {
                res.ErrorText = "Empty interview handler Id passed to GetInterviewQuestions";
                return res;
            }

            string strUrl = string.Format("{0}handlers/interviewhandlers/{1}/interviewquestions", pConnectionServer.BaseUrl, pInterviewHandlerObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is not an error, just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pInterviewQuestions = new List<InterviewQuestion>();
                return res;
            }

            pInterviewQuestions = HTTPFunctions.GetObjectsFromJson<InterviewQuestion>(res.ResponseText);

            if (pInterviewQuestions == null)
            {
                pInterviewQuestions = new List<InterviewQuestion>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pInterviewQuestions)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// returns a single InterviewHandler object from an ObjectId or displayName string passed in.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that the handler is homed on.
        /// </param>
        /// <param name="pInterviewQuestion">
        /// The out param that the filled out instance of the InterviewQuestion class is returned on.
        /// </param>
        /// <param name="pInterviewHandlerObjectId">
        /// The interview handler to fetch the question from
        /// </param>
        /// <param name="pInterviewQuestionNumber">
        /// Question number to fetch off the handler
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetInterviewQuestion(out InterviewQuestion pInterviewQuestion, ConnectionServer pConnectionServer,
            string pInterviewHandlerObjectId, int pInterviewQuestionNumber)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            pInterviewQuestion = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetInterviewHandler";
                return res;
            }

            //you need an ObjectId and/or a display name - both being blank is not acceptable
            if (string.IsNullOrEmpty(pInterviewHandlerObjectId))
            {
                res.ErrorText = "Empty InterviewHandlerObjectId ppassed to GetInterviewQuestion";
                return res;
            }

            //create a new InterviewHandler instance passing the ObjectId (or display name) which fills out the data automatically
            try
            {
                pInterviewQuestion = new InterviewQuestion(pConnectionServer, pInterviewHandlerObjectId, pInterviewQuestionNumber);
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed to fetch handler in GetInterviewQuestion:" + ex.Message;
                res.Success = false;
            }

            return res;
        }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Diplays the display name and extension of the handler by default.
        /// </summary>
        public override string ToString()
        {
            return String.Format("Interview question #{0} [{1}]", this.QuestionNumber, this.StreamText);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the interview handler object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the call handler object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            StringBuilder strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Fills the current instance of InterviewHandler in with properties fetched from the server.  If both the display name and ObjectId
        /// parameters are provided, the ObjectId is used for the search.
        /// </summary>
        /// <param name="pInterviewHandlerObjectId">
        /// Unique GUID of the interview handler to fetch - can be blank if the display name is passed in.
        /// </param>
        /// <param name="pQuestionNumber">
        /// Display name (required to be unique for all interview handlers) to search on an interview handler by.  Can be blank if the ObjectId 
        /// parameter is provided.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetInterviewQuestion(string pInterviewHandlerObjectId, int pQuestionNumber)
        {
            WebCallResult res;

            if (string.IsNullOrEmpty(pInterviewHandlerObjectId))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "No value for ObjectId or display name passed to GetInterviewHandler.";
                return res;
            }

            string strUrl = string.Format("{0}handlers/interviewhandlers/{1}/{2}", HomeServer.BaseUrl, pInterviewHandlerObjectId,pQuestionNumber);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            if (res.TotalObjectCount == 0)
            {
                res.Success = false;
                res.ErrorText = "Interviewer question not found by objectId=" + pInterviewHandlerObjectId + " #" + pQuestionNumber;
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(HTTPFunctions.StripJsonOfObjectWrapper(res.ResponseText, "InterviewQuestion"), this,
                    HTTPFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }

            return res;
        }


        #endregion


    }
}
