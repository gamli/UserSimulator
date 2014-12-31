using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class MoveVM_TEST
   {
      [TestMethod]
      public void TranslationVM_Property_TEST()
      {
         var move = new Move { TranslationX = ConstantExpressions.Create(4711), TranslationY = ConstantExpressions.Create(-4711) };
         using (var moveVM = new MoveVM(move))
         {
            Assert.AreEqual(move.TranslationX, moveVM.TranslationXVM.Model);
            Assert.AreEqual(move.TranslationY, moveVM.TranslationYVM.Model);
            move.TranslationX = ConstantExpressions.Create(1147);
            move.TranslationY = ConstantExpressions.Create(-1147);
            Assert.AreEqual(move.TranslationX, moveVM.TranslationXVM.Model);
            Assert.AreEqual(move.TranslationY, moveVM.TranslationYVM.Model);
         }
      }
   }
}
