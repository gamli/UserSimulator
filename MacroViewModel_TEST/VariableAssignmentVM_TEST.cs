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
         ExpressionVM_Property_TEST<bool, BooleanVariableAssignmentVM>(true, false);
         ExpressionVM_Property_TEST<string, StringVariableAssignmentVM>("variableName1", "variableName2");
         ExpressionVM_Property_TEST<int, IntegerVariableAssignmentVM>(4711, -4711);
      }
      private void ExpressionVM_Property_TEST<T, TVM>(T Value1, T Value2)
         where TVM : VariableAssignmentBaseVM<T>
      {
         var boolAssignment = new VariableAssignment<T> { Symbol = "variableName", Expression = ConstantExpressions.Create(Value1) };
         using (var assignmentVM = (VariableAssignmentBaseVM<T>)typeof(TVM).GetConstructors()[0].Invoke(new []{ boolAssignment }))
         {
            Assert.AreEqual(boolAssignment.Expression, assignmentVM.ExpressionVM.Model);
            boolAssignment.Expression = ConstantExpressions.Create(Value2);
            Assert.AreEqual(boolAssignment.Expression, assignmentVM.ExpressionVM.Model);
         }
      }
   }
}
