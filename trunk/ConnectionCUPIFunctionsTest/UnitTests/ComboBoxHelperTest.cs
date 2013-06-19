using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ComboBoxHelperTest
    {

        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //class wide instance of a ConnectionServer object used for all tests - this is attached to in the class initialize
        //routine below.
        private static ConnectionServerRest _connectionServer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            //create a connection server instance used for all tests - rather than using a mockup 
            //for fetching data I prefer this "real" testing approach using a public server I keep up
            //and available for the purpose - the conneciton information is stored in the test project's 
            //settings and can be changed to a local instance easily.
            Settings mySettings = new Settings();
            Thread.Sleep(300);
            try
            {
                _connectionServer = new ConnectionServerRest(new RestTransportFunctions(), mySettings.ConnectionServer, mySettings.ConnectionLogin,
                    mySettings.ConnectionPW);
                _connectionServer.DebugMode = mySettings.DebugOn;
            }

            catch (Exception ex)
            {
                throw new Exception("Unable to attach to Connection server to start routing rule test:" + ex.Message);
            }
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticFailure_FillDropDownWithObjects()
        {
            var res= ComboBoxHelper.FillDropDownWithObjects(null, null);
            Assert.IsFalse(res.Success,"Call to FillDropDownWithObject with Null combobox and list should fail");

            List<CallHandler> oHandlers = new List<CallHandler>();
            res = ComboBoxHelper.FillDropDownWithObjects(null, oHandlers);
            Assert.IsFalse(res.Success, "Call to FillDropDownWithObject with Null combobox should fail");

            res = ComboBoxHelper.FillDropDownWithObjects(new ComboBox(), oHandlers);
            Assert.IsFalse(res.Success, "Call to FillDropDownWithObject with empty list should fail");

        }

        [TestMethod]
        public void StaticFailure_GetCurrentComboBoxSelection()
        {
            CallHandler oHandler;
            var res = ComboBoxHelper.GetCurrentComboBoxSelection(null, out oHandler);
            Assert.IsFalse(res,"Call to GetCurrentComboBoxSelection with null ComboBox did not fail");

            res = ComboBoxHelper.GetCurrentComboBoxSelection(new ComboBox(), out oHandler);
            Assert.IsFalse(res, "Call to GetCurrentComboBoxSelection with empty ComboBox did not fail");
        }


        #endregion

        [TestMethod]
        public void FetchAndSortTests()
        {
            ComboBox oComboBox = new ComboBox();

            List<CallHandler> oHandlers;
            var res = CallHandler.GetCallHandlers(_connectionServer, out oHandlers,1,10);
            Assert.IsTrue(res.Success,"Failed to fetch call handlers:"+res);
            Assert.IsTrue(oHandlers.Count>0,"No handlers fetched:"+res);

            UnityDisplayObjectCompare objectCompare = new UnityDisplayObjectCompare();

            oHandlers.Sort(objectCompare);

            res=ComboBoxHelper.FillDropDownWithObjects( oComboBox, oHandlers);
            Assert.IsTrue(res.Success,"Failed to fill comboBox with handlers:"+res);
            //Assert.IsTrue(oHandlers.Count==oComboBox.Items.Count,"Handler list count does not match combo box count");

            CallHandler oHandler;
            ComboBoxHelper.GetCurrentComboBoxSelection(oComboBox, out oHandler);

            Assert.IsNotNull(oHandlers,"Null call handler returned from fetch");
            //Assert.IsTrue(!string.IsNullOrEmpty(oHandler.ObjectId),"Empty call handler returned from fetch");

        }
    }
}
