using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common_TEST
{
   [TestClass]
   public class NotifyingTransformedProperty_TEST
   {
      [TestMethod]
      public void Value_Property_TEST()
      {
         var testModel = MockNotifyPropertyChanged.Create();
         var testObject = MockWithNotifyingTransformedProperty.Create(testModel);
         var isTestObjectsNameHugo = testObject.IsYourNameHugo;
         testObject.PropertyChanged +=
            (Sender, Args) =>
            {
               Assert.AreEqual(Args.PropertyName, "IsYourNameHugo");
               isTestObjectsNameHugo = testObject.IsYourNameHugo;
            };
         testModel.SomeProperty = "Hugo";
         Assert.IsTrue(isTestObjectsNameHugo);
         testModel.SomeProperty = "Karl";
         Assert.IsFalse(isTestObjectsNameHugo);
         testModel.SomeProperty = "Hugo";
         Assert.IsTrue(isTestObjectsNameHugo);         
      }
   }
}
