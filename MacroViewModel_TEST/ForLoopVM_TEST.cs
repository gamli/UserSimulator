﻿using System;
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
         var forLoop = new ForLoop { RepetitionCount = ConstantExpressions.Create(4711), Body = new NoOp() };
         using(var forLoopVM = new ForLoopVM(forLoop))
         {
            Assert.AreEqual(forLoop.RepetitionCount, forLoopVM.RepetitionCountVM.Model);
            forLoop.RepetitionCount = ConstantExpressions.Create(-4711);
            Assert.AreEqual(forLoop.RepetitionCount, forLoopVM.RepetitionCountVM.Model);
         }
      }
   }
}
