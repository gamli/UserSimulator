using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class PauseVM_TEST
   {
      [TestMethod]
      public void DurationVM_Property_TEST()
      {
         var pause = new Pause { Duration = new Constant(4711) };
         using (var pauseVM = new PauseVM(pause))
         {
            Assert.AreEqual(pause.Duration, pauseVM.DurationVM.Model);
            pause.Duration = new Constant(1147);
            Assert.AreEqual(pause.Duration, pauseVM.DurationVM.Model);
         }
      }
   }
}
