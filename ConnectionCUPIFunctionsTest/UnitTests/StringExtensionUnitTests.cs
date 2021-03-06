﻿using System;
using System.Text;
using Cisco.UnityConnection.RestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class StringExtensionUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion


        [TestMethod]
        public void ToBool_True_Success()
        {
            Assert.IsTrue("true".ToBool(),"String extension failed to parse 'true' as bool");
        }
        
        [TestMethod]
        public void ToBool_False_Success()
        {
            Assert.IsFalse("false".ToBool(), "String extension failed to parse 'true' as bool");
        }

        [TestMethod]
        public void ToBool_1_Success()
        {
            Assert.IsTrue("1".ToBool(), "String extension failed to parse '1' as bool");
        }

        [TestMethod]
        public void ToBool_0_Success()
        {
            Assert.IsFalse("0".ToBool(), "String extension failed to parse '0' as bool");
        }

        [TestMethod]
        public void ToBool_Invalid_Failure()
        {
            Assert.IsFalse("bogus".ToBool(), "string extension failed to return false on invalid boolean string");
        }

        [TestMethod]
        public void ToInt_1_Success()
        {
            int iTemp = "1".ToInt();
            Assert.IsTrue(iTemp == 1, "String extension failed to parse '1' as integer");

            }

        [TestMethod]
        public void ToInt_Negative_Success()
        {
            int iTemp = "-14".ToInt();
            Assert.IsTrue(iTemp == -14, "String extension failed to parse '-14' as integer");
        }

        [TestMethod]
        public void ToInt_Invalid_Success()
        {
            int iTemp = "bogus".ToInt();
            Assert.IsTrue(iTemp == 0, "String extension failed to return 0 for 'bogus' as integer");
         }

        [TestMethod]
        public void ToLong_1_Success()
        {
            long lTemp = "1".ToLong();
            Assert.IsTrue(lTemp == 1, "String extension failed to parse '1' as integer");
        }

        [TestMethod]
        public void ToLong_Negative_Success()
        {
            long lTemp = "-14".ToLong();
            Assert.IsTrue(lTemp == -14, "String extension failed to parse '-14' as integer");

        }

        [TestMethod]
        public void ToLong_Invalid_Failure()
        {
            long lTemp = "bogus".ToLong();
            Assert.IsTrue(lTemp == 0, "String extension failed to return 0 for 'bogus' as long");
        }

        [TestMethod]
        public void QuerySafe_SingleQuote_Success()
        {
            string strTemp = "O'Riley".QuerySafe();
            Assert.IsTrue(strTemp.Contains("''"), "string extension query safe did not replace single quote with double");
        }

        [TestMethod]
        public void QuerySafe_EmptyString_Success()
        {
            string strTemp = "".QuerySafe();
            Assert.IsTrue(string.IsNullOrEmpty(strTemp), "string extension query safe did not return empty string for empty string");
        }

        [TestMethod]
        public void ToDate_Success()
        {
            DateTime oDate = "1/20/2010".ToDate();
            Assert.IsTrue(oDate != DateTime.MinValue, "String extension failed to parse '1/20/2010' as date");
        }

        [TestMethod]
        public void ToDate_InvalidDate_Failure()
        {
            DateTime oDate = "bogus".ToDate();
            Assert.IsTrue(oDate == DateTime.MinValue, "String extension failed to return min date on 'bogus' as date");
        }

        [TestMethod]
        public void ConvertToDateFormat_Success()
        {
            string strTemp = "1/20/2010".ConvertToDateFormat();
            Assert.IsFalse(string.IsNullOrEmpty(strTemp),"Failed to convert date string into general date format");
        }

        [TestMethod]
        public void ConvertToDateFormat_InvalidDate()
        {
            string strTemp = "bogus".ConvertToDateFormat();
            Assert.IsTrue(strTemp.Equals("bogus"), "Invalid date format did not return string passed in for ConvertToDateFormat");
        }

        [TestMethod]
        public void ToAscii_Success()
        {
            string strTemp = "1234".ToAscii();
            Assert.IsFalse(string.IsNullOrEmpty(strTemp), "Failed to convert string into asci");
        }

        [TestMethod]
        public void ToAscii_EmptyString()
        {
            string strTemp = "".ToAscii();
            Assert.IsTrue(string.IsNullOrEmpty(strTemp), "string extension ToAscii did not return empty string for empty string");
        }

        [TestMethod]
        public void ToUtf8_Success()
        {
            string strTemp = "1234".ToUtf8();
            Assert.IsFalse(string.IsNullOrEmpty(strTemp), "Failed to convert string into utf8");
        }

        [TestMethod]
        public void ToUtf8_EmptyString()
        {
            string strTemp = "".ToUtf8();
            Assert.IsTrue(string.IsNullOrEmpty(strTemp), "string extension ToUtf8 did not return empty string for empty string");
        }

        [TestMethod]
        public void DetectEncoding_Success()
        {
            Encoding oEncoding = "blah".DetectEncoding();
            Assert.IsFalse(oEncoding == null, "Failed to detect string encoding");
        }

        [TestMethod]
        public void RemoveBraces_WithBraces()
        {
            string strTemp = "{remove braces}".RemoveBraces();
            Assert.IsTrue(strTemp.Equals("remove braces"),"Failed to remove braces.");
        }

        [TestMethod]
        public void RemoveBraces_WithOutBraces()
        {
            string strTemp = "remove braces".RemoveBraces();
            Assert.IsTrue(strTemp.Equals("remove braces"), "Failed to remove braces.");
        }


        [TestMethod]
        public void ContainedInList_InList()
        {
            bool bTemp = "item4".ContainedInList("item1", "item2", "item3", "item4", "item5");
            Assert.IsTrue(bTemp,"Failed to find an item in a list of strings");
        }

        [TestMethod]
        public void ContainedInList_NotInList()
        {
            bool bTemp = "item6".ContainedInList("item1", "item2", "item3", "item4", "item5");
            Assert.IsFalse(bTemp, "Returned true incorrectly for list membership check");
        }

        [TestMethod]
        public void UriSafe_EmptyString()
        {
            string ret = "".UriSafe();
            Assert.IsTrue(string.IsNullOrEmpty(ret), "Empty string to UriSafe should return empty string");
        }

        [TestMethod]
        public void UriSafe_RemoveSpace()
        {
            string ret = "string with spaces".UriSafe();
            Assert.IsFalse(ret.Contains(" "), "Spaces should be removed by UriSafe");
        }

        [TestMethod]
        public void UriSafe_RemoveAndSign()
        {
            string ret = "string&with&Ands".UriSafe();
            Assert.IsFalse(ret.Contains("&"), "And signs should be removed by UriSafe");
        }

        [TestMethod]
        public void UriSafe_RemoveQuestionMark()
        {
            string ret = "string?with?QuestionMarks".UriSafe();
            Assert.IsFalse(ret.Contains("?"), "Question marks should be removed by UriSafe");
        }

        [TestMethod]
        public void UriSafe_RemoveSemiColon()
        {
            string ret = "string;with;SemiColons".UriSafe();
            Assert.IsFalse(ret.Contains(";"), "Semi colons should be removed by UriSafe");
        }

        [TestMethod]
        public void HtmlBodySafe_EmptyString()
        {
            string ret = "".HtmlBodySafe();
            Assert.IsTrue(string.IsNullOrEmpty(ret), "Empty string input to HtmlBodySafe should return empty string");
        }

        [TestMethod]
        public void HtmlBodySafe_RemoveAndSign()
        {
            string ret = "string&withAnds".HtmlBodySafe();
            Assert.IsTrue(ret.Contains("&#38;"), "And signs should be replaced by &#38; by HtmlBodySafe");
        }

        [TestMethod]
        public void TrimToEndOfToken_EmptyString()
        {
            string ret = "".TrimToEndOfToken("junk");
            Assert.IsTrue(string.IsNullOrEmpty(ret), "Empty string to TrimToEndOfToken should return empty string");
        }

        [TestMethod]
        public void TrimToEndOfToken_EmptyToken()
        {
            const string strString = "Junk";
            string ret = strString.TrimToEndOfToken("");
            Assert.IsTrue(ret.Equals(strString), "Empty token passed to TrimToEndOfToken should return original string");
        }

        [TestMethod]
        public void TrimToEndOfToken_NoTokenMatch()
        {
            const string strString = "testing string with no token in it";
            string ret = strString.TrimToEndOfToken("junk");
            Assert.IsTrue(ret.Equals(strString), "If token does not appear in string, string should be returned unchanged");
        }

        [TestMethod]
        public void TrimToEndOfToken_TokenMatch()
        {
            const string strString = "testing string with no token in it";
            const string strExpected = "no token in it";
            string ret = strString.TrimToEndOfToken("with ");
            Assert.IsTrue(ret.Equals(strExpected), "String not trimed from token:" + ret);
        }


        [TestMethod]
        public void TrimTokenFromEnd_EmptyString()
        {
            string ret = "".TrimTokenFromEnd("junk");
            Assert.IsTrue(string.IsNullOrEmpty(ret), "Empty string to TrimTokenFromEnd should return empty string");
        }

        [TestMethod]
        public void TrimTokenFromEnd_EmptyToken()
        {
            const string strString = "Junk";
            string ret = strString.TrimTokenFromEnd("");
            Assert.IsTrue(ret.Equals(strString), "Empty token passed to TrimTokenFromEnd should return original string");
        }

        [TestMethod]
        public void TrimTokenFromEnd_TokenMatch()
        {
            const string strString = "testing string with no token in it";
            const string strExpected = "testing string with no";
            string ret = strString.TrimTokenFromEnd(" token in it");
            Assert.IsTrue(ret.Equals(strExpected), "Token not trimmed form end:" + ret);
        }

        [TestMethod]
        public void TrimTokenFromEnd_NoTokenFound()
        {
            const string strString = "testing string with no token in it";
            string ret = strString.TrimTokenFromEnd("junk");
            Assert.IsTrue(ret.Equals(strString), "If token not found original string should be returned:" + ret);
        }
    }
}
