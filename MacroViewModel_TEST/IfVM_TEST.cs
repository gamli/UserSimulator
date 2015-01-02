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
         var ifStatement = new If { Expression = ConstantExpressions.Create(true), Body = new NoOp() };
         using (var forLoopVM = new IfVM(ifStatement))
         {
            Assert.AreEqual(ifStatement.Expression, forLoopVM.ExpressionVM.Model);
            ifStatement.Expression = ConstantExpressions.Create(false);
            Assert.AreEqual(ifStatement.Expression, forLoopVM.ExpressionVM.Model);
         }
      }
   }
}
