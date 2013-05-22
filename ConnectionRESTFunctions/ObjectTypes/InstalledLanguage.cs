#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System.Collections.Generic;
using Newtonsoft.Json;


namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Class that allows for a static method to fetch the list of installed languages for all purposes in Connection and returning it as a generic
    /// list of installed languages.
    /// </summary>
    public class InstalledLanguage
    {

        #region Constructors and Destructors


        /// <summary>
        /// constructor takes a ConnectionServer as a parameter - passed as null when constructing lists internally
        /// </summary>
        /// <param name="pConnectionServer"></param>
        public InstalledLanguage(ConnectionServer pConnectionServer = null)
        {
            if (pConnectionServer == null)
            {
                return;
            }

            HomeServer = pConnectionServer;

            var res = GetInstalledLanguages();
            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,"Failed to load installed languages:" + res);
            }
        }


        /// <summary>
        /// generic constructor for Json parsing libraries
        /// </summary>
        public InstalledLanguage()
        {
        }

        #endregion


        #region Fields and Properties 

        public ConnectionServer HomeServer { get; private set; }

        public List<InstalledLanguage> InstalledLanguages { get; private set; }

        #endregion


        #region InstalledLanguage Properties

        [JsonProperty]
        public int LanguageType { get; private set; }

        [JsonProperty]
        public int LanguageCode { get; private set; }

        [JsonProperty]
        public bool Loaded { get; private set; }

        [JsonProperty]
        public bool IsLicensed { get; private set; }

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
            return ((ConnectionLanguageTypes)LanguageType).ToString();
        }

        /// <summary>
        /// Gets the list of all installed languages and resturns them as a generic list of InstalledLangauge objects.  
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        private WebCallResult GetInstalledLanguages()
        {
            string strUrl = HomeServer.BaseUrl + "installedlanguages";

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one template
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                InstalledLanguages = new List<InstalledLanguage>();
                res.Success = false;
                return res;
            }

            InstalledLanguages = HomeServer.GetObjectsFromJson<InstalledLanguage>(res.ResponseText);

            if (InstalledLanguages == null || InstalledLanguages.Count==0)
            {
                res.Success = false;
                res.ErrorText = "Failed to fetch installed languages on server";
                return res;
            }

            foreach (var oLanguage in InstalledLanguages)
            {
                oLanguage.HomeServer = HomeServer;
            }

            return res;
        }


        /// <summary>
        /// Will return true if a specific language code is installed for the type of language desired (TUI/VUI/GUI
        /// </summary>
        /// <param name="pLanguageCode">
        /// Language code integer to check for (1033 US English for instance).
        /// </param>
        /// <param name="pConnectionLanguageType">
        /// Language type (GUI/TUI/VUI/TTS)
        /// </param>
        /// <returns>
        /// True if the language is installed, false if not
        /// </returns>
        public bool IsLanguageInstalled(int pLanguageCode, ConnectionLanguageTypes pConnectionLanguageType = ConnectionLanguageTypes.TUI)
        {
            if (InstalledLanguages == null)
            {
                return false;
            }

            //look through all languages to see if one matches the code and type.
            foreach (var oLanguage in InstalledLanguages)
            {
                if (oLanguage.LanguageCode == pLanguageCode & oLanguage.LanguageType == (int)pConnectionLanguageType)
                {
                    return true;
                }
            }

            return false;
        }

       
        #endregion
    }
}
