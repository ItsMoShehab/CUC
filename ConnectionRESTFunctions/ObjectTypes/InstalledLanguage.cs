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
    /// Definitions of the langauge type enum value from the data dictionary
    /// </summary>
    public enum LanguageTypes {TUI, GUI, VUI, TTS }
    
    /// <summary>
    /// Class that allows for a static method to fetch the list of installed languages for all purposes in Connection and returning it as a generic
    /// list of installed languages.
    /// </summary>
    public class InstalledLanguage
    {
        
        #region Fields and Properties 

        public int LanguageType { get; set; }
        public int LanguageCode { get; set; }
        public bool Loaded { get; set; }
        public bool IsLicensed { get; set; }

        private readonly ConnectionServer _homeServer;

        public List<InstalledLanguage> InstalledLanguages { get; private set; }

        /// <summary>
        /// Fetch the language description from the static Language Helper class.
        /// This is implemented as a method instead of a proeprty so it's not called when a generic list of InstalledLanguage
        /// objects is bound to a grid or the like.
        /// </summary>
        /// <returns></returns>
        public string LanguageDescription()
        {
            return LanguageHelper.GetLanguageNameFromLanguageId(LanguageCode);
        }

        /// <summary>
        /// Language type (TUI, GUI, VUI or TTS) is returned based on enum value
        /// </summary>
        public string LanguageTypeDescription()
        {
            return ((LanguageTypes) LanguageType).ToString();
        }

        #endregion


        #region Constructors

        /// <summary>
        /// constructor takes a ConnectionServer as a parameter - passed as null when constructing lists internally
        /// </summary>
        /// <param name="pConnectionServer"></param>
        public InstalledLanguage(ConnectionServer pConnectionServer=null)
        {
            if (pConnectionServer == null)
            {
                return;
            }

            _homeServer = pConnectionServer;

            GetInstalledLanguages();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Returns a string with all the details about the installed language class values.
        /// </summary>
        public override string ToString()
        {
            return string.Format("type:{0} [{1}], licensed:{2}, loaded:{3}, code:{4} [{5}]", LanguageType,LanguageTypeDescription(),
                IsLicensed, Loaded, LanguageCode, LanguageDescription() );
        }


        /// <summary>
        /// Gets the list of all installed languages and resturns them as a generic list of InstalledLangauge objects.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that the langauges should be pulled from
        /// </param>
        /// <param name="pInstalledLanguages">
        /// Out parameter that is used to return the list of languages installed on Connection - there must be at least one.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetInstalledLanguages()
        {
            string strUrl = _homeServer.BaseUrl + "installedlanguages";

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, _homeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements should always be populated with something, but just in case do a check here.
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                res.Success = false;
                return res;
            }

            InstalledLanguages = GetInstalledLanguagesFromXElements(_homeServer, res.XmlElement);
            return res;
        }


        /// <summary>
        /// Will return true if a specific language code is installed for the type of language desired (TUI/VUI/GUI
        /// </summary>
        /// <param name="pLanguageCode">
        /// Language code integer to check for (1033 US English for instance).
        /// </param>
        /// <param name="pLanguageType">
        /// Language type (GUI/TUI/VUI/TTS)
        /// </param>
        /// <returns>
        /// True if the language is installed, false if not
        /// </returns>
        public bool IsLanguageInstalled(int pLanguageCode, LanguageTypes pLanguageType = LanguageTypes.TUI)
        {
            if (InstalledLanguages == null)
            {
                return false;
            }

            //look through all languages to see if one matches the code and type.
            foreach (var oLanguage in InstalledLanguages)
            {
                if (oLanguage.LanguageCode == pLanguageCode & oLanguage.LanguageType == (int)pLanguageType)
                {
                    return true;
                }
            }

            return false;
        }

        //Helper function to take an XML blob returned from the REST interface for languages returned and convert it into an generic
        //list of InstalledLanguage class objects.  
        private static List<InstalledLanguage> GetInstalledLanguagesFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {

            List<InstalledLanguage> oLanguages = new List<InstalledLanguage>();

            //Use LINQ to XML to create a list of InstalledLanguage objects in a single statement.  
            var languages = from e in pXElement.Elements()
                            where e.Name.LocalName == "InstalledLanguage"
                            select e;

            //for each object returned in the list from the XML, construct a class object using the elements associated with that 
            //object.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlLanguage in languages)
            {
                InstalledLanguage oLanguage = new InstalledLanguage();
                foreach (XElement oElement in oXmlLanguage.Elements())
                {
                    //adds the XML property to the object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXmlFetch(oLanguage, oElement);
                }

                //add the fully populated object to the list that will be returned to the calling routine.
                oLanguages.Add(oLanguage);
            }

            return oLanguages;
        }


        #endregion

    }
}
