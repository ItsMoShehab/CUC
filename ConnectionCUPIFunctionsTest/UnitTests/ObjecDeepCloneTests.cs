using System;
using Cisco.UnityConnection.RestFunctions;
using ConnectionCUPIFunctionsTest.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConnectionCUPIFunctionsTest
{
    [TestClass]
    public class ObjectDeepCloneTests : BaseUnitTests 
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
            UserBase oUser1 = new UserBase();
            oUser1.FirstName = "bbb";

            UserBase oUserCopy = oUser1.Clone();
            Assert.IsTrue(oUser1.FirstName.Equals(oUserCopy.FirstName),"First name fields should match after clone");

            oUserCopy.FirstName = "new name";
            Assert.IsFalse(oUser1.FirstName.Equals(oUserCopy.FirstName), "First name fields should not match after clone update");
        }

        [TestMethod]
        public void CloneNullTest()
        {
            UserBase oTest = null;

            var oCopy = ObjectDeepClone.Clone(oTest);
            Assert.IsTrue(oCopy==null, "Null clone should return null without exception");
        }

        [TestMethod]
        public void NonSerializableTest_Failure()
        {
            NonSerializableClass oTest = new NonSerializableClass();

            try
            {
                var oCopy = oTest.Clone();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected error for non serializable object:"+ex);
                return;
            }
            Assert.Fail("Cloning a non serializable object should throw exception");
        }
    }

    public class NonSerializableClass
    {
        private string strString;
        private int iInt;
    }
}