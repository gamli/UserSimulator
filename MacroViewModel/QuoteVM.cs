using Common;
using Macro;

namespace MacroViewModel
{
   public class QuoteVM : ExpressionBaseVM
   {
      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _expressionVM;
      public ExpressionBaseVM ExpressionVM
      {
         get
         {
            return _expressionVM.Value;
         }
      }

      public QuoteVM(Quote Model)
         : base(Model)
      {
         _expressionVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Expression" }, "ExpressionVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Expression),
               VM => VM.Dispose());
      }
   }
}
