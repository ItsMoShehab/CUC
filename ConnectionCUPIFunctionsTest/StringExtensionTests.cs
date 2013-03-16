using System;
using System.Text;
using System.Threading;
using ConnectionCUPIFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class StringExtensionTests
    {

        #region Fields and Properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion



        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue("true".ToBool(),"String extension failed to parse 'true' as bool");
            Assert.IsFalse("false".ToBool(), "String extension failed to parse 'true' as bool");
            Assert.IsTrue("1".ToBool(), "String extension failed to parse '1' as bool");
            Assert.IsFalse("0".ToBool(), "String extension failed to parse '0' as bool");
            Assert.IsFalse("bogus".ToBool(),"string extension failed to return false on invalid boolean string");


            int iTemp = "1".ToInt();
            Assert.IsTrue(iTemp == 1, "String extension failed to parse '1' as integer");

            iTemp = "-14".ToInt();
            Assert.IsTrue(iTemp == -14, "String extension failed to parse '-14' as integer");

            iTemp = "bogus".ToInt();
            Assert.IsTrue(iTemp == 0, "String extension failed to return 0 for 'bogus' as integer");

            long lTemp = "1".ToLong();
            Assert.IsTrue(lTemp == 1, "String extension failed to parse '1' as integer");

            lTemp = "-14".ToLong();
            Assert.IsTrue(lTemp == -14, "String extension failed to parse '-14' as integer");

            lTemp = "bogus".ToLong();
            Assert.IsTrue(lTemp == 0, "String extension failed to return 0 for 'bogus' as long");
            
            string strTemp = "O'Riley".QuerySafe();
            Assert.IsTrue(strTemp.Contains("''"), "string extension query safe did not replace single quote with double");

            strTemp = "".QuerySafe();
            Assert.IsTrue(string.IsNullOrEmpty(strTemp), "string extension query safe did not return empty string for empty string");

            DateTime oDate = "1/20/2010".ToDate();
            Assert.IsTrue(oDate != DateTime.MinValue, "String extension failed to parse '1/20/2010' as date");

            oDate = "bogus".ToDate();
            Assert.IsTrue(oDate == DateTime.MinValue, "String extension failed to return min date on 'bogus' as date");

            strTemp = "1/20/2010".ConvertToDateFormat("G");
            Assert.IsFalse(string.IsNullOrEmpty(strTemp),"Failed to convert date string into general date format");

            strTemp = "bogus".ConvertToDateFormat("G");
            Assert.IsTrue(strTemp.Equals("bogus"), "Invalid date format did not return string passed in for ConvertToDateFormat");

            strTemp = "1234".ToAscii();
            Assert.IsFalse(string.IsNullOrEmpty(strTemp), "Failed to convert string into asci");

            strTemp = "".ToAscii();
            Assert.IsTrue(string.IsNullOrEmpty(strTemp), "string extension ToAscii did not return empty string for empty string");

            strTemp = "1234".ToUtf8();
            Assert.IsFalse(string.IsNullOrEmpty(strTemp), "Failed to convert string into utf8");

            strTemp = "".ToUtf8();
            Assert.IsTrue(string.IsNullOrEmpty(strTemp), "string extension ToUtf8 did not return empty string for empty string");

            Encoding oEncoding = "blah".DetectEncoding();
            Assert.IsFalse(oEncoding == null, "Failed to detect string encoding");

            strTemp = "{remove braces}".RemoveBraces();
            Assert.IsTrue(strTemp.Equals("remove braces"),"Failed to remove braces.");

            bool bTemp = "item4".ContainedInList("item1", "item2", "item3", "item4", "item5");
            Assert.IsTrue(bTemp,"Failed to find an item in a list of strings");

            bTemp = "item6".ContainedInList("item1", "item2", "item3", "item4", "item5");
            Assert.IsFalse(bTemp, "Returned true incorrectly for list membership check");

        }
    }
}
