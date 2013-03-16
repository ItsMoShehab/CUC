#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
{
    /// <summary>
    /// Class to hold all the data for Connection timezone definitions
    /// </summary>
    public class ConnectionTimeZone
    {
        public int TimeZoneId { get; set; }
        public string DisplayName { get; set; }
        public int Bias { get; set; }
        public int DaylightBias { get; set; }
        public int DaylightStartMonth { get; set; }
        public int DaylightStartDayOfWeek { get; set; }
        public int DaylightStartWeek { get; set; }
        public int DaylightStartHour { get; set; }
        public int DaylightEndMonth { get; set; }
        public int DaylightEndDayOfWeek { get; set; }
        public int DaylightEndWeek { get; set; }
        public int DaylightEndHour { get; set; }
        public int LanguageCode { get; set; }

        /// <summary>
        /// Returns a string with the text name and time zone Id of the timezone
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} [{1}], bias={2}", TimeZoneId, DisplayName,Bias);
        }

    }

    /// <summary>
    /// Class that will fetch all the timezones defined on a Connection server and allow fetching timezone definitions from that collection
    /// by Timezone ID.
    /// </summary>
    public class TimeZones
    {
        
        #region Fields and Properties 

        private Dictionary<int, ConnectionTimeZone> _timeZones;

        #endregion


        #region Constructors

        //constructor
        public TimeZones(ConnectionServer pConnectionServer)
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to TimeZones construtor");
            }

            WebCallResult res= LoadTimeZones(pConnectionServer);
            if (res.Success == false)
            {
                throw new Exception("Failed to fetch timezones in TimeZones constructor:"+res.ToString());
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Fetch all the timezones defined on the target Connection server and load them into a dictionary for fast fetching later.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server to fetch zones from.
        /// </param>
        private WebCallResult LoadTimeZones(ConnectionServer pConnectionServer)
        {
            _timeZones = new Dictionary<int, ConnectionTimeZone>();

            string strUrl = pConnectionServer.BaseUrl + "timezones";

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, pConnectionServer, "");

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

            //populate a list of ConnectionTimeZone structs with data for each timezone found in the system and add it to the private timezone dictionary.
            foreach (XElement oElement in res.XMLElement.Elements())
            {
                ConnectionTimeZone oConnectionTimeZone = new ConnectionTimeZone();
                foreach (XElement oSubElement in oElement.Elements())
                {
                    pConnectionServer.SafeXMLFetch(oConnectionTimeZone, oSubElement);
                }

                try
                {
                    _timeZones.Add(oConnectionTimeZone.TimeZoneId, oConnectionTimeZone);
                }
                catch (Exception ex)
                {
                    res.Success = false;
                    res.ErrorText = ex.ToString();
                    return res;
                }
            }
            res.Success = true;
            return res;
        }


        /// <summary>
        /// return the ConnectionTimeZone instance for a timezone if found by ID, otherwise an empty class instance is returned with default values for 
        /// all members.
        /// </summary>
        /// <param name="pTimeZoneId">
        /// Timezone ID to search for 
        /// </param>
        /// <param name="pConnectionConnectionTimeZone">
        /// Timezone instance passed back on this out param 
        /// </param>
        /// <returns>
        /// WebCallResult instance
        /// </returns>
        public WebCallResult GetTimeZone(int pTimeZoneId, out ConnectionTimeZone pConnectionConnectionTimeZone)
        {
            WebCallResult res = new WebCallResult();
            res.Success = true;

            if (!_timeZones.TryGetValue(pTimeZoneId, out pConnectionConnectionTimeZone))
            {
                pConnectionConnectionTimeZone=new ConnectionTimeZone();
                if (Debugger.IsAttached) Debugger.Break();
                res.Success = false;
                res.ErrorText = "Unable to find timezone by ID=" + pTimeZoneId.ToString();
            }

            return res;
        }


        #endregion

    }
}
