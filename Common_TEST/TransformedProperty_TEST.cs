using System;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common_TEST
{
   [TestClass]
   public class TransformedProperty_TEST
   {
      [TestMethod]
      public void Value_Property_TEST()
      {
         var testModel = TestNotifyPropertyChanged.Create();
         using(var testObject = TestWithTransformedProperty.Create(testModel))
         {
            testModel.SomeProperty = "Hugo";
            Assert.IsTrue(testObject.IsYourNameHugo);
            testModel.SomeProperty = "Karl";
            Assert.IsFalse(testObject.IsYourNameHugo);
            testModel.SomeProperty = "Hugo";
            Assert.IsTrue(testObject.IsYourNameHugo);
         }
      }
   }
}
