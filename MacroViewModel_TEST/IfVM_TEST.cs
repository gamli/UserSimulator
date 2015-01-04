using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class IfVM_TEST
   {
      [TestMethod]
      public void ExpressionVM_Property_TEST()
      {
         var ifStatement = new If { Expression = new Constant(true), Body = new Block() };
         using (var forLoopVM = new IfVM(ifStatement))
         {
            Assert.AreEqual(ifStatement.Expression, forLoopVM.ExpressionVM.Model);
            ifStatement.Expression = new Constant(false);
            Assert.AreEqual(ifStatement.Expression, forLoopVM.ExpressionVM.Model);
         }
      }
   }
}
