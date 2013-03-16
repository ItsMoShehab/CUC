using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWResetASP
{
    public static class GlobalItems
    {

        /// <summary>
        /// Simple function to help convert strings formatted for textbox/WinForms string output into HTML output - replaces newline and \n
        /// characters with <br/> line breaks needed by HTML for output formatting with line breaks.
        /// </summary>
        /// <param name="pStringToFormat"></param>
        /// <returns></returns>
        public static string FormatStringForHTML(string pStringToFormat)
        {
            string strFormatted = "";

            if (string.IsNullOrEmpty(pStringToFormat))
            {
                return "";
            }

            strFormatted = pStringToFormat.Replace(Environment.NewLine, "<br/>");
            strFormatted = strFormatted.Replace("/n", "<br/>");
            return strFormatted;
        }


    }
}