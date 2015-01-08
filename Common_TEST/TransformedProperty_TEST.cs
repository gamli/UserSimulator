using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common_TEST
{
   [TestClass]
   public class TransformedProperty_TEST
   {
      [TestMethod]
      public void Value_Property_TEST()
      {
         var testModel = MockNotifyPropertyChanged.Create();
         using (var testObject = MockWithTransformedProperty.Create(testModel, null))
         {
            testModel.SomeProperty = "Hugo";
            Assert.IsTrue(testObject.IsYourNameHugo);
            testModel.SomeProperty = "Karl";
            Assert.IsFalse(testObject.IsYourNameHugo);
            testModel.SomeProperty = "Hugo";
            Assert.IsTrue(testObject.IsYourNameHugo);
            testModel.SomeOtherProperty = "Karl";
            Assert.IsTrue(testObject.IsYourNameHugo);
            testModel.SomeOtherProperty = "Hugo";
            Assert.IsTrue(testObject.IsYourNameHugo);
            testModel.SomeOtherProperty = null;
            Assert.IsTrue(testObject.IsYourNameHugo);
         }
      }

      [TestMethod]
      public void Release_Delegate_TEST()
      {
         var testModel = MockNotifyPropertyChanged.Create();
         var releaseCounter = 0;
         // ReSharper disable once UnusedVariable - testObject is implicitly used through releaseCounter
         using (var testObject = MockWithTransformedProperty.Create(testModel, BoolValue => releaseCounter++))
         {
            Assert.AreEqual(releaseCounter, 0);
            testModel.SomeProperty = "Hugo";
            Assert.AreEqual(releaseCounter, 0);
            testModel.SomeProperty = "Karl";
            Assert.AreEqual(releaseCounter, 1);
            testModel.SomeProperty = "Hugo";
            Assert.AreEqual(releaseCounter, 2);
         }
      }
   }
}
