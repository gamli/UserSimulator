using Common;
using Macro;

namespace MacroViewModel
{
   public class DefinitionVM : ExpressionBaseVM
   {
      private readonly NotifyingTransformedProperty<SymbolVM> _symbolVM;
      public SymbolVM SymbolVM
      {
         get
         {
            return _symbolVM.Value;
         }
      }

      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _expressionVM;
      public ExpressionBaseVM ExpressionVM
      {
         get
         {
            return _expressionVM.Value;
         }
      }

      public DefinitionVM(Definition Model)
         : base(Model)
      {
         _symbolVM =
            new NotifyingTransformedProperty<SymbolVM>(
               new[] { "Symbol" }, "SymbolVM",
               Model, this,
               () => (SymbolVM)MacroViewModelFactory.Instance.Create(Model.Symbol),
               VM => VM.Dispose());

         _expressionVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Expression" }, "ExpressionVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Expression),
               VM => VM.Dispose());
      }
   }
}
