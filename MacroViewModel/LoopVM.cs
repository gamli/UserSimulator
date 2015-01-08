using Common;
using Macro;

namespace MacroViewModel
{
   public class LoopVM : ExpressionBaseVM
   {
      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _conditionVM;
      public ExpressionBaseVM ConditionVM
      {
         get
         {
            return _conditionVM.Value;
         }
      }
      
      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _bodyVM;
      public ExpressionBaseVM BodyVM
      {
         get
         {
            return _bodyVM.Value;
         }
      }

      public LoopVM(Loop Model)
         : base(Model)
      {
         _conditionVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Condition" }, "ConditionVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Condition),
               VM => VM.Dispose());

         _bodyVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Body" }, "BodyVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Body),
               VM => VM.Dispose());
      }
   }
}
