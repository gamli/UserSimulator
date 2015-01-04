using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class ForLoopVM_TEST
   {
      [TestMethod]
      public void RepetitionCountVM_Property_TEST()
      {
         var forLoop = new ForLoop { RepetitionCount = new Constant(4711), Body = new Block() };
         using(var forLoopVM = new ForLoopVM(forLoop))
         {
            Assert.AreEqual(forLoop.RepetitionCount, forLoopVM.RepetitionCountVM.Model);
            forLoop.RepetitionCount = new Constant(-4711);
            Assert.AreEqual(forLoop.RepetitionCount, forLoopVM.RepetitionCountVM.Model);
         }
      }
   }
}
