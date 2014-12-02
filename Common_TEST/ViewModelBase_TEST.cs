using System;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common_TEST
{
   [TestClass]
   public class ViewModelBase_TEST : NotifyPropertyChangedBase_TEST
   {
      [TestMethod]
      public void Model_Property_TEST()
      {
         var testModel = TestNotifyPropertyChanged.Create();
         using (var testViewModel = TestViewModel.Create(testModel))
         {
            Assert.AreEqual(testViewModel.Model, testModel);
            testViewModel.Dispose();
            testViewModel.Dispose();
            testViewModel.Dispose();
         }
      }
   }
}
