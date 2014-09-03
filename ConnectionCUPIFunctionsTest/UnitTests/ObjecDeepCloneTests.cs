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

            // ReSharper disable once ExpressionIsAlwaysNull
            var oCopy = ObjectDeepClone.Clone(oTest);
            Assert.IsTrue(oCopy==null, "Null clone should return null without exception");
        }

        [TestMethod]
        public void NonSerializableTest_Failure()
        {
            NonSerializableClass oTest = new NonSerializableClass();
            
            try
            {
                // ReSharper disable once UnusedVariable
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
        // ReSharper disable UnusedField.Compiler
        private string _strString;
        private int _iInt;
    }
}