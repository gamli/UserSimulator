using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class VariableAssignmentVM_TEST
   {
      [TestMethod]
      public void ExpressionVM_Property_TEST()
      {
         ExpressionVM_Property_TEST(true, false);
         ExpressionVM_Property_TEST("variableName1", "variableName2");
         ExpressionVM_Property_TEST(4711, -4711);
      }
      private void ExpressionVM_Property_TEST<T>(T Value1, T Value2)
      {
         var assignment = new Definition { Symbol = "variableName", Expression = new Constant(Value1) };
         using (var assignmentVM = new VariableAssignmentVM(assignment))
         {
            Assert.AreEqual(assignment.Expression, assignmentVM.ExpressionVM.Model);
            assignment.Expression = new Constant(Value2);
            Assert.AreEqual(assignment.Expression, assignmentVM.ExpressionVM.Model);
         }
      }
   }
}
