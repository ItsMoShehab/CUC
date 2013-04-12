#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Extensions to the string class for commonly used items - encodings are a bit of a pain here - UTF8 and UTF16 need to be handled properly
    /// when dealing with data from Windows and querying/inserting into Informix DBs - it can go badly in a hurry
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Handles removing single quotes from a query - deals with the edge case of the same string being passed through the 
        /// query safe filter more than once.
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static string QuerySafe(this string pString)
        {
            if (string.IsNullOrEmpty(pString))
            {
                return "";
            }
            
             //first, replace any existing doubled up single quotes with just a single quote
            string strTemp = pString.Replace("''", "'");
    
            //now double up any single quotes - this looks odd but prevents problems if the string is passed through a query safe
            //filter more than once.
            strTemp = strTemp.Replace("'","''");

            //if it contains a percent makes sure it's escaped out - again, remove any pre escaped back slashes and put them back in to make sure there's no
            //messy accidents here.  Be sure to use the @ prefix on the string here so C# doesn't try and treat the backslash like an escape code 
            //strTemp = strTemp.Replace("\\%", "%");
            //strTemp = strTemp.Replace("%", "[%]");
 
            return strTemp;
        }

        /// <summary>
        /// Trim off everything from the start of a string up to the end of the token passed in.
        /// </summary>
        /// <param name="pString"></param>
        /// <param name="pToken"></param>
        /// <returns></returns>
        public static string TrimToEndOfToken(this string pString, string pToken)
        {
            if (string.IsNullOrEmpty(pString))
            {
                return "";
            }

            if (string.IsNullOrEmpty(pToken))
            {
                return pString;
            }

            int iPos = pString.IndexOf(pToken, StringComparison.InvariantCultureIgnoreCase);
            if (iPos < 1)
            {
                return pString;
            }

            return pString.Substring(iPos + pToken.Length);

        }

        /// <summary>
        /// Pulls a single instance of a token (which can be a character) from the end of a stirng if it's 
        /// present.  Unlike trimEnd it does NOT remove more than a single instance of it.
        /// </summary>
        /// <param name="pString"></param>
        /// <param name="pToken"></param>
        /// <returns></returns>
        public static string TrimTokenFromEnd(this string pString, string pToken)
        {
            if (string.IsNullOrEmpty(pString))
            {
                return "";
            }

            if (string.IsNullOrEmpty(pToken))
            {
                return pString;
            }

            if (pString.EndsWith(pToken))
            {
                return pString.Substring(0, pString.Length - pToken.Length);
            }
            return pString;
        }


        /// <summary>
        /// Convert a string into a date format if possible - if the string cannot be converted into a date the original string is 
        /// returned
        /// </summary>
        /// <param name="pString">
        /// String representing a date
        /// </param>
        /// <param name="pFormat">
        /// Format type:
        /// f=full with short time
        /// F=full with long time
        /// G = general date time format (Default)
        /// t= short time
        /// T= long time
        /// d= short date
        /// D= long date
        /// </param>
        /// <returns></returns>
        public static string ConvertToDateFormat(this string pString, string pFormat="G")
        {
            string strTemp = pString;

            DateTime oDate;
            if (DateTime.TryParse(pString, out oDate)==false)
                return strTemp;

            try
            {
                return oDate.ToString(pFormat);
            }
            catch
            {
                return pString;
            }
        }

        /// <summary>
        /// Convert a string into a date format if possible.  Returns current date/time if no.
        /// </summary>
        /// <param name="pString">
        /// string for date format.
        /// </param>
        /// <returns>
        /// DateTime instance corresponding to the string's date or date.min if not.
        /// </returns>
        public static DateTime ToDate(this string pString)
        {
            DateTime oDate;

            if (DateTime.TryParse(pString,out oDate))
            {
                return oDate;
            }

            return DateTime.MinValue;
        }


        /// <summary>
        /// Forces string to ASII representation
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static string ToAscii(this string pString)
        {
        
            if (string.IsNullOrEmpty(pString))
            {
                return "";
            }

            ASCIIEncoding oAsciiEncoding = new ASCIIEncoding();

            return oAsciiEncoding.GetString(oAsciiEncoding.GetBytes(pString));

        }

        /// <summary>
        /// Forces string to UTF8
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static string ToUtf8(this string pString)
        {
            if (string.IsNullOrEmpty(pString))
            {
                return "";
            }
            
            ASCIIEncoding oAsciiEncoding = new ASCIIEncoding();
            UTF8Encoding oUtf8Encoding = new UTF8Encoding();

            return oUtf8Encoding.GetString(oAsciiEncoding.GetBytes(pString));
        }

        /// <summary>
        /// Determines if the encoding is UTF8 or ASCI or the like.
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static Encoding DetectEncoding(this string pString)
        {
            string strContents;

            byte[] byteArray = Encoding.ASCII.GetBytes( pString );
            MemoryStream stream = new MemoryStream(byteArray);
            // open the file with the stream-reader: 
            using (StreamReader reader = new StreamReader(stream, true))
            {
                // read the contents of the file into a string 
                strContents = reader.ReadToEnd();

                // return the encoding. 
                return reader.CurrentEncoding;
            } 

        }

        /// <summary>
        /// converts a string to boolean - handles both the typical "true/false" construct as well as 0 and non zero 
        /// representation of booleans.
        /// </summary>
        /// <param name="pString"> </param>
        /// <returns></returns>
        public static bool ToBool(this string pString)
        {
            bool oBool;
            //first see if it's a "true", "false" construct
            if (bool.TryParse(pString, out oBool))
            {
                return oBool;
            }

            //check of it's a simple 0, non zero value
            int iTemp;
            if (int.TryParse(pString, out iTemp))
            {
                return (iTemp != 0);
            }

            return false;
        }


        /// <summary>
        /// simplified conversion of string to int with error catch
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static int ToInt(this string pString)
        {
            int iTemp;
            if (int.TryParse(pString, out iTemp))
            {
                return iTemp;
            }

            return 0;
        }
        /// <summary>
        /// Simplified conversion of string to long with error catch
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static long ToLong(this string pString)
        {
            long iTemp;
            if (long.TryParse(pString, out iTemp))
            {
                return iTemp;
            }

            return 0;
        }

        /// <summary>
        /// Simple helper function to allow you to compare a string value against membership in a list of other strings - comes up 
        /// often enough that it's easier to tuck it into an extension like this.
        /// CASE INSENSITIVE
        /// </summary>
        /// <param name="pValue"></param>
        /// <param name="pList">
        /// one or more values to compare against 
        /// </param>
        /// <returns>
        /// True if the string is found in the list, false if it is not (or the list is empty)
        /// </returns>
        public static bool ContainedInList(this string pValue, params string[] pList)
        {
            return pList.Any(strValue => strValue.Equals(pValue, StringComparison.InvariantCultureIgnoreCase));
        }


        /// <summary>
        /// GUIDs in Windows based backups contain curly braces - short hand for removing them
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        public static string RemoveBraces(this string pValue)
        {
            return pValue.Replace("{", "").Replace("}", "");
        }
    }
}
