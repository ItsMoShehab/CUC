#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{
    /// <summary>
    /// Read only class that lets you fetch restriction table patternss
    /// </summary>
    public class RestrictionPattern
    {

        #region Fields and Properties

        private readonly ConnectionServer _homeServer;

        public bool Blocked { get; set; }
        public string NumberPattern { get; set; }
        public string RestrictionTableObjectId { get; set; }
        public string ObjectId { get; set; }
        public int SequenceNumber { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Pass in the objectId of the restriction table to load, it's display name (which should be unique) or neither and the constructor
        /// will create an uninitalized instance - this is used when constructing lists of restriction tables via the static method.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Instance of the Connection Server class we are doing queries against.
        /// </param>
        /// <param name="pRestrictionTableObjectId">
        /// ObjectId of the restriction table that owns the pattern
        /// </param>
        /// <param name="pObjectId">
        /// Optional ObjecTId of the restriction table to load.
        /// </param>
        public RestrictionPattern(ConnectionServer pConnectionServer, string pRestrictionTableObjectId, string pObjectId="")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to RestrictionPattern constructor.");
            }

            if (string.IsNullOrEmpty(pRestrictionTableObjectId))
            {
                throw new ArgumentException("Empty restriction table ObjectId passed to RestrictionPattern constructor.");
            }

            _homeServer = pConnectionServer;
            RestrictionTableObjectId = pRestrictionTableObjectId;

            //if the user passed in a specific ObjectId then go load that pattern up, otherwise just return an empty instance.
            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            ObjectId = pObjectId;

            //if the ObjectId or display name are passed in then fetch the data on the fly and fill out this instance
            WebCallResult res = GetRestrictionTablePattern(pObjectId);

            if (res.Success == false)
            {
                throw new Exception(string.Format("RestrictionPattern not found in RestrictionPattern constructor using ObjectId={0}\n\r{1}"
                                 , pObjectId, res.ErrorText));
            }
            
        }


        #endregion


        #region Instance Methods

        public override string ToString()
        {
            return string.Format("Restriction pattern: {0}, blocked={1}", NumberPattern, Blocked);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the schedule object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the schedule object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            var strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Fills the current instance of RestrictionTable in with properties fetched from the server.  If both the display name and ObjectId
        /// parameters are provided, the ObjectId is used for the search.
        /// </summary>
        /// <param name="pObjectId">
        /// Unique GUID of the RT to fetch - can be blank if the display name is passed in.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetRestrictionTablePattern(string pObjectId)
        {
            string strUrl;
            WebCallResult res;

            //when fetching a RT use the query construct in both cases so the XML parsing is identical
            strUrl = string.Format("{0}restrictiontables/{1}/restrictionpatterns/{2}", _homeServer.BaseUrl,RestrictionTableObjectId, pObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, _homeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XMLElement == null || res.XMLElement.HasElements == false)
            {
                res.Success = false;
                return res;
            }

            //populate this call handler instance with data from the XML fetch
            foreach (XElement oElement in res.XMLElement.Elements())
            {
                _homeServer.SafeXMLFetch(this, oElement);
            }

            return res;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all restriction patterns and resturns them as a generic list of RestrictionPattern objects.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the templates should be pulled from
        /// </param>
        /// <param name="pRestrictionTableObjectId">
        /// The objectId of the restriction table to fetch patterns for.
        /// </param>
        /// <param name="pRestrictionPatterns">
        /// Out parameter that is used to return the list of RestrictionTable objects defined on Connection - there must be at least one.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetRestrictionPatterns(ConnectionServer pConnectionServer, string pRestrictionTableObjectId, 
            out List<RestrictionPattern> pRestrictionPatterns)
        {
            WebCallResult res;
            pRestrictionPatterns = null;

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetRestrictionPatterns";
                return res;
            }

            if (string.IsNullOrEmpty(pRestrictionTableObjectId))
            {
                res = new WebCallResult();
                res.ErrorText = "Empty restriction table objectId passed to GetRestrictionPatterns";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + string.Format("restrictiontables/{0}/restrictionpatterns",pRestrictionTableObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty, that's legal
            if (res.XMLElement == null || res.XMLElement.HasElements == false)
            {
                pRestrictionPatterns = new List<RestrictionPattern>();
                return res;
            }

            pRestrictionPatterns = GetRestrictionPatternsFromXElements(pConnectionServer, pRestrictionTableObjectId, res.XMLElement);
            return res;
        }


        //Helper function to take an XML blob returned from the REST interface for user RT patterns returned and convert it into an generic
        //list of RestrictionPattern class objects.  
        private static List<RestrictionPattern> GetRestrictionPatternsFromXElements(ConnectionServer pConnectionServer, string pRestrictionTableObjectId,
            XElement pXElement)
        {

            List<RestrictionPattern> oRestrictionPatternList = new List<RestrictionPattern>();


            //Use LINQ to XML to create a list of RT pattern objects in a single statement.
            var restrictionPattern = from e in pXElement.Elements()
                                    where e.Name.LocalName == "RestrictionPattern"
                                    select e;

            //for each object returned in the list from the XML, construct a class object using the elements associated with that 
            //object.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlRestrictionPattern in restrictionPattern)
            {
                RestrictionPattern oRestrictionPattern = new RestrictionPattern(pConnectionServer,pRestrictionTableObjectId);
                foreach (XElement oElement in oXmlRestrictionPattern.Elements())
                {
                    //adds the XML property to the object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXMLFetch(oRestrictionPattern, oElement);
                }

                //add the fully populated object to the list that will be returned to the calling routine.
                oRestrictionPatternList.Add(oRestrictionPattern);
            }

            return oRestrictionPatternList;
        }


        #endregion

    }
}
