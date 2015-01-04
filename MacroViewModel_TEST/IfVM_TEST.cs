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
         var ifStatement = new If { Condition = new Constant(true), Alternative = new Block() };
         using (var forLoopVM = new IfVM(ifStatement))
         {
            Assert.AreEqual(ifStatement.Condition, forLoopVM.ExpressionVM.Model);
            ifStatement.Condition = new Constant(false);
            Assert.AreEqual(ifStatement.Condition, forLoopVM.ExpressionVM.Model);
         }
      }
   }
}
