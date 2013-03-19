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
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{
    /// <summary>
    /// The SMPP Provider class is used only to provide an interface for user to select which provider to use when creating new SMS notification 
    /// devices. 
    /// </summary>
    public class SmppProvider
    {
        #region Fields and Properties 

        public string TextName { get; set; }
        public string ObjectId { get; set; }

        #endregion


        #region Constructors

        //constructor
        public SmppProvider(ConnectionServer pConnectionServer, string pObjectId="")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer passed to SmppProvider constructor");
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                return;
            }

            WebCallResult res = GetSmppProvider(pConnectionServer, pObjectId);

            if (res.Success == false)
            {
                throw new Exception("Failed to find SmppProvider in SmppConstructor:"+res.ToString());
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the SMPP provider
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", TextName, ObjectId);
        }

        /// <summary>
        /// Fills current instance of class with details of smpp provider for objectId passed in if found.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to query
        /// </param>
        /// <param name="pObjectId">
        /// Unique Id for search space to load
        /// </param>
        /// <returns>
        /// Instance of WebCallResult class
        /// </returns>
        private WebCallResult GetSmppProvider(ConnectionServer pConnectionServer, string pObjectId)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            string strUrl = pConnectionServer.BaseUrl + "smppproviders/" + pObjectId;

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                res.Success = false;
                res.ErrorText = "No XML elements returned from search space fetch";
                return res;
            }

            foreach (var oElement in res.XmlElement.Elements())
            {
                pConnectionServer.SafeXmlFetch(this, oElement);
            }

            res.Success = true;
            return res;
        }

        /// <summary>
        /// Gets the list of all SmppProviders and resturns them as a generic list of SmppProvider objects.  This
        /// list can be used for providing drop down list selection for notification device creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that references the server the SmppProviders should be pulled from
        /// </param>
        /// <param name="pSMppProviders">
        /// Out parameter that is used to return the list of SmppProvider objects defined on Connection - the list may be empty
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetSmppProviders(ConnectionServer pConnectionServer, out List<SmppProvider> pSMppProviders)
        {
            WebCallResult res;
            pSMppProviders = null;

            if (pConnectionServer==null)
            {
              	res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetSmppProviders";
                return res;
            }

            string strUrl = pConnectionServer.BaseUrl + "smppproviders";

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                pSMppProviders=new List<SmppProvider>();
                return res;
            }

            pSMppProviders = GetSmppProvidersFromXElements(pConnectionServer, res.XmlElement);
            return res;
        }


        //Helper function to take an XML blob returned from the REST interface for SmppProviders returned and convert it into an generic
        //list of SmppProvider class objects.  
        private static List<SmppProvider> GetSmppProvidersFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {
            //Use LINQ to XML to create a list of Smpp provider objects in a single statement.  We're only interested in 2 properties for providers
            //here - they should always be present but protect from missing properties anyway.
            IEnumerable<SmppProvider> smppProviders = from e in pXElement.Elements()
                                                      where e.Name.LocalName == "SmppProvider"
                                                      select new SmppProvider(pConnectionServer)
                                                                 {
                                                                     ObjectId = (e.Element("ObjectId") == null) ? "" : e.Element("ObjectId").Value,
                                                                     TextName = (e.Element("TextName") == null) ? "" : e.Element("TextName").Value,
                                                                 };
            return smppProviders.ToList();
        }

    
        #endregion


    }
}
