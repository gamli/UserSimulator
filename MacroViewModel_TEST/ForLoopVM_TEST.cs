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
         var forLoop = new Loop { Body = new Constant(4711), Body = new Block() };
         using(var forLoopVM = new ForLoopVM(forLoop))
         {
            Assert.AreEqual(forLoop.Body, forLoopVM.RepetitionCountVM.Model);
            forLoop.Body = new Constant(-4711);
            Assert.AreEqual(forLoop.Body, forLoopVM.RepetitionCountVM.Model);
         }
      }
   }
}
