using Cisco.UnityConnection.RestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest.UnitTests
{
    [TestClass]
    public class ObjectFlagAllPropertiesTests : BaseUnitTests 
    {
        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        #endregion

        [TestMethod]
        public void ClonePropertyCompareTests_FirstName()
        {
            FlagPropertiesTestClass oTest = new FlagPropertiesTestClass();
           oTest.FlagAllPropertiesForUpdate();
           Assert.IsTrue(oTest.ChangeList.Count==1, "Only the public read/write property should be added to the change list");
        }
    }

    public class FlagPropertiesTestClass
    {
         //used to keep track of which properties have been updated
         private readonly ConnectionPropertyList _changedPropList;

         //for checking on pending changes
         public ConnectionPropertyList ChangeList { get { return _changedPropList; } }


        public FlagPropertiesTestClass()
        {
            _changedPropList=new ConnectionPropertyList();
            _publicReadWrite = true;
            _publicReadOnly = "testing";
            _publicReadPrivteWrite = "testing";
        }

        private bool _publicReadWrite;
            public bool PublicReadWrite 
            {
                get { return _publicReadWrite; } 
                set
                {
                    _changedPropList.Add("PublicReadWrite", value);
                    _publicReadWrite = value;
                } 
            }

            private string _publicReadWriteString;
            public string PublicReadWriteString
            {
                get { return _publicReadWriteString; }
                set
                {
                    _changedPropList.Add("PublicReadWriteString", value);
                    _publicReadWriteString = value;
                }
            }
        private string _publicReadOnly;
            public string PublicReadOnly 
            {
                get { return _publicReadOnly; } 
            }


        private string _publicReadPrivteWrite;
        public string PublicReadPrivateWrite
        {
            get
            {
                return _publicReadPrivteWrite;
            }
            private set
                {
                    _changedPropList.Add("PublicReadPrivateWrite", value);
                    _publicReadPrivteWrite = value;
                } 

        }
    }

}
