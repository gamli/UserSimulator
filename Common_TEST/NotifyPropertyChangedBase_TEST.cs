using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common_TEST
{
   [TestClass]
   public class NotifyPropertyChangedBase_TEST
   {
      [TestMethod]
      public void RaisePropertyChanged_TEST()
      {
         var testModel = MockNotifyPropertyChanged.Create();
         string[] propertyName = {""};
         var eventCounter = 0;
         testModel.PropertyChanged +=
            (Sender, Args) =>
               {
                  Assert.AreEqual(propertyName[0], Args.PropertyName);
                  eventCounter++;
               };
         propertyName[0] = "Klaus";
         testModel.RaisePropertyChanged_TEST(propertyName[0]);
         Assert.AreEqual(eventCounter, 1);
         propertyName[0] = "Helga";
         testModel.RaisePropertyChanged_TEST(propertyName[0]);
         Assert.AreEqual(eventCounter, 2);
         propertyName[0] = null;
         testModel.RaisePropertyChanged_TEST(propertyName[0]);
         Assert.AreEqual(eventCounter, 3);
      }

      [TestMethod]
      public void SetPropertyValue_TEST()
      {
         var testModel = MockNotifyPropertyChanged.Create();
         var eventCounter = 0;
         testModel.PropertyChanged +=
            (Sender, Args) =>
            {
               Assert.AreEqual(Args.PropertyName, "SomeProperty");
               eventCounter++;
            };
         Assert.AreEqual(eventCounter, 0);
         testModel.SomeProperty = "Hugo";
         Assert.AreEqual(eventCounter, 0);
         testModel.SomeProperty = "Klaus";
         Assert.AreEqual(eventCounter, 1);
      }
   }
}
