using System.Collections.Generic;
using System.Windows.Forms;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ComboBoxHelperUnitTests : BaseUnitTests
    {

        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        public static ComboBox oComboBox = new ComboBox();

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            BaseUnitTests.ClassInitialize(testContext);
        }

        #endregion


        #region Static Call Failures

        [TestMethod]
        public void StaticFailure_FillDropDownWithObjects()
        {
            ComboBox oComboBox = null;

            var res = ComboBoxHelper.FillDropDownWithObjects(ref oComboBox, null);
            Assert.IsFalse(res.Success,"Call to FillDropDownWithObject with Null combobox and list should fail");

            List<CallHandler> oHandlers = new List<CallHandler>();
            res = ComboBoxHelper.FillDropDownWithObjects(ref oComboBox, oHandlers);
            Assert.IsFalse(res.Success, "Call to FillDropDownWithObject with Null combobox should fail");

            oComboBox=new ComboBox();

            res = ComboBoxHelper.FillDropDownWithObjects(ref oComboBox, oHandlers);
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
        public void UnityDisplayObjectCompare_Sort_Ascending()
        {
            List<CallHandler> oHandlers = new List<CallHandler>();
            CallHandler oHandler = new CallHandler(_mockServer);
            oHandler.DisplayName = "bbb";
            oHandlers.Add(oHandler);
            
            oHandler=new CallHandler(_mockServer);
            oHandler.DisplayName = "aaa";
            oHandlers.Add(oHandler);

            Assert.IsTrue(oHandlers[0].DisplayName=="bbb","Call handler list should being in FIFO order");

            UnityDisplayObjectCompare objectCompare = new UnityDisplayObjectCompare();
            oHandlers.Sort(objectCompare);

            Assert.IsTrue(oHandlers[0].DisplayName == "aaa", "Call handlers not in ascending order after sort call");
        }


        [TestMethod]
        public void GetComboBoxSelection_Success()
        {
            
            oComboBox=new ComboBox();
            oComboBox.Items.Add("test");

            List<CallHandler> oHandlers = new List<CallHandler>();
            CallHandler oHandler = new CallHandler(_mockServer);
            oHandler.DisplayName = "bbb";
            oHandlers.Add(oHandler);

            oHandler = new CallHandler(_mockServer);
            oHandler.DisplayName = "aaa";
            oHandlers.Add(oHandler);

            var res = ComboBoxHelper.FillDropDownWithObjects(ref oComboBox, oHandlers);
            Assert.IsTrue(res.Success, "Failed to fill comboBox with handlers:" + res);

            ComboBoxHelper.GetCurrentComboBoxSelection(oComboBox, out oHandler);

            Assert.IsNotNull(oHandlers, "Null call handler returned from fetch");
            //Assert.IsTrue(!string.IsNullOrEmpty(oHandler.ObjectId),"Empty call handler returned from fetch");
        }
    }
}
