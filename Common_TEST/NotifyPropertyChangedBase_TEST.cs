using System;
using Common;
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
         var propertyName = "";
         var eventCounter = 0;
         testModel.PropertyChanged +=
            (Sender, Args) =>
            {
               Assert.AreEqual(Args.PropertyName, propertyName);
               eventCounter++;
            };
         propertyName = "Klaus";
         testModel.RaisePropertyChanged_TEST(propertyName);
         Assert.AreEqual(eventCounter, 1);
         propertyName = "Helga";
         testModel.RaisePropertyChanged_TEST(propertyName);
         Assert.AreEqual(eventCounter, 2);
         propertyName = null;
         testModel.RaisePropertyChanged_TEST(propertyName);
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
