using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class IfStatementVM_TEST
   {
      [TestMethod]
      public void ExpressionVM_Property_TEST()
      {
         var ifStatement = new IfStatement { Expression = ConstantExpressions.Create(true), Body = new NoOp() };
         using (var forLoopVM = new IfStatementVM(ifStatement))
         {
            Assert.AreEqual(ifStatement.Expression, forLoopVM.ExpressionVM.Model);
            ifStatement.Expression = ConstantExpressions.Create(false);
            Assert.AreEqual(ifStatement.Expression, forLoopVM.ExpressionVM.Model);
         }
      }
   }
}
