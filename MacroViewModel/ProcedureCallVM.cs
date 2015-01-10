using System.Collections.ObjectModel;
using Common;
using Macro;

namespace MacroViewModel
{
   public class ProcedureCallVM : ListExpressionBaseVM<ExpressionBase, ExpressionBaseVM>
   {
      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _procedureVM;
      public ExpressionBaseVM ProcedureVM
      {
         get
         {
            return _procedureVM.Value;
         }
      }

      private readonly TransformedCollection<ExpressionBase, ExpressionBaseVM> _argumentsVM;
      public ReadOnlyObservableCollection<ExpressionBaseVM> ArgumentsVM { get { return _argumentsVM.Transformed; } }

      public ProcedureCallVM(ProcedureCall Model)
         : base(Model)
      {
         _procedureVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Procedure" }, "ProcedureVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Procedure),
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
