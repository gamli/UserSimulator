using Common;
using Macro;

namespace MacroViewModel
{
   public class IfVM : ExpressionBaseVM
   {
      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _conditionVM;
      public ExpressionBaseVM ConditionVM
      {
         get
         {
            return _conditionVM.Value;
         }
      }

      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _consequentVM;
      public ExpressionBaseVM ConsequentVM
      {
         get
         {
            return _consequentVM.Value;
         }
      }

      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _alternativeVM;
      public ExpressionBaseVM AlternativeVM
      {
         get
         {
            return _alternativeVM.Value;
         }
      }

      public IfVM(If Model)
         : base(Model)
      {
         _conditionVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Condition" }, "ConditionVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Condition),
               VM => VM.Dispose());

         _consequentVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Consequent" }, "ConsequentVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Consequent),
               VM => VM.Dispose());

         _alternativeVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Alternative" }, "AlternativeVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Alternative),
               VM => VM.Dispose());
      }

      protected override void Dispose(bool Disposing)
      {
         if (Disposing)
         {
            _conditionVM.Dispose();
            _consequentVM.Dispose();
            if (_alternativeVM != null)
               _alternativeVM.Dispose();
         }
         base.Dispose(Disposing);
      }
   }
}
