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
      public void ConditionVM_Property_TEST()
      {
         var ifExpression = new If { Condition = new Constant(true), Consequent = new Constant(4711), Alternative = new Constant(-4711) };
         using (var ifVM = new IfVM(ifExpression))
         {
            Assert.AreEqual(ifExpression.Condition, ifVM.ConditionVM.Model);
            ifExpression.Condition = new Constant(false);
            Assert.AreEqual(ifExpression.Condition, ifVM.ConditionVM.Model);
         }
      }

      [TestMethod]
      public void ConsequentVM_Property_TEST()
      {
         var ifExpression = new If { Condition = new Constant(true), Consequent = new Constant(4711), Alternative = new Constant(-4711) };
         using (var ifVM = new IfVM(ifExpression))
         {
            Assert.AreEqual(ifExpression.Consequent, ifVM.ConsequentVM.Model);
            ifExpression.Consequent = new Constant(-4711);
            Assert.AreEqual(ifExpression.Consequent, ifVM.ConsequentVM.Model);
         }
      }

      [TestMethod]
      public void AlternativeVM_Property_TEST()
      {
         var ifExpression = new If { Condition = new Constant(true), Consequent = new Constant(4711), Alternative = new Constant(-4711) };
         using (var ifVM = new IfVM(ifExpression))
         {
            Assert.AreEqual(ifExpression.Alternative, ifVM.AlternativeVM.Model);
            ifExpression.Alternative = new Constant(4711);
            Assert.AreEqual(ifExpression.Alternative, ifVM.AlternativeVM.Model);
         }
      }
   }
}
