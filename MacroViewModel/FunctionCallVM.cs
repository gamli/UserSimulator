using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroViewModel
{
   public class FunctionCallVM : ListVM
   {
      private NotifyingTransformedProperty<ExpressionBaseVM> _function;
      public ExpressionBaseVM FunctionVM
      {
         get
         {
            return _function.Value;
         }
      }

      private TransformedCollection<ExpressionBase, ExpressionBaseVM> _argumentsVM;
      public ReadOnlyObservableCollection<ExpressionBaseVM> ArgumentsVM { get { return _argumentsVM.Transformed; } }

      public FunctionCallVM(FunctionCall Model)
         : base(Model)
      {
         _function =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Function" }, "FunctionVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Function),
               VM => VM.Dispose());

         _argumentsVM =
            new TransformedCollection<ExpressionBase, ExpressionBaseVM>(
               Model.Arguments,
               Argument => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Argument),
               null,
               ArgumentVM => ArgumentVM.Dispose());
      }
   }
}
