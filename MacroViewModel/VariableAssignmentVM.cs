using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroViewModel
{
   public class BooleanVariableAssignmentVM : VariableAssignmentBaseVM<bool>
   {
      [ExcludeFromCodeCoverage]
      public BooleanVariableAssignmentVM(VariableAssignment<bool> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class StringVariableAssignmentVM : VariableAssignmentBaseVM<string>
   {
      [ExcludeFromCodeCoverage]
      public StringVariableAssignmentVM(VariableAssignment<string> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class IntegerVariableAssignmentVM : VariableAssignmentBaseVM<int>
   {
      [ExcludeFromCodeCoverage]
      public IntegerVariableAssignmentVM(VariableAssignment<int> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class VariableAssignmentBaseVM<T> : MacroBaseVM<VariableAssignment<T>>
   {
      private NotifyingTransformedProperty<ExpressionBaseVM<T>> _expressionVM;
      public ExpressionBaseVM<T> ExpressionVM
      {
         get
         {
            return _expressionVM.Value;
         }
      }

      public VariableAssignmentBaseVM(VariableAssignment<T> Model)
         : base(Model)
      {
         _expressionVM =
            new NotifyingTransformedProperty<ExpressionBaseVM<T>>(
               new[] { "Expression" }, "ExpressionVM",
               Model, this,
               () => (ExpressionBaseVM<T>)MacroViewModelFactory.Instance.Create(Model.Expression),
               VM => VM.Dispose());
      }
   }
}
