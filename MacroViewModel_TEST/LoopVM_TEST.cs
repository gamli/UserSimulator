using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class LoopVM_TEST
   {
      [TestMethod]
      public void ConditionVM_Property_TEST()
      {
         var forLoop = new Loop { Condition = new Constant(true), Body = new List() };
         using(var forLoopVM = new LoopVM(forLoop))
         {
            Assert.AreEqual(forLoop.Condition, forLoopVM.ConditionVM.Model);
            forLoop.Condition = new Constant(false);
            Assert.AreEqual(forLoop.Condition, forLoopVM.ConditionVM.Model);
         }
      }

      [TestMethod]
      public void BodyVM_Property_TEST()
      {
         var forLoop = new Loop { Condition = new Constant(true), Body = new Constant(4711) };
         using (var forLoopVM = new LoopVM(forLoop))
         {
            Assert.AreEqual(forLoop.Body, forLoopVM.BodyVM.Model);
            forLoop.Body = new Constant(-4711);
            Assert.AreEqual(forLoop.Body, forLoopVM.BodyVM.Model);
         }
      }
   }
}
