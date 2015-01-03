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
   public class MoveVM : MacroBaseVM<Move>
   {
      private NotifyingTransformedProperty<IntegerExpressionBaseVM> _translationXVM;
      public IntegerExpressionBaseVM TranslationXVM
      {
         get
         {
            return _translationXVM.Value;
         }
      }

      private NotifyingTransformedProperty<IntegerExpressionBaseVM> _translationYVM;
      public IntegerExpressionBaseVM TranslationYVM
      {
         get
         {
            return _translationYVM.Value;
         }
      }

      public MoveVM(Move Model)
         : base(Model)
      {
         _translationXVM =
            new NotifyingTransformedProperty<IntegerExpressionBaseVM>(
               new[] { "TranslationX" }, "TranslationXVM",
               Model, this,
               () => (IntegerExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.TranslationX),
               VM => VM.Dispose());

         _translationYVM =
            new NotifyingTransformedProperty<IntegerExpressionBaseVM>(
               new[] { "TranslationY" }, "TranslationYVM",
               Model, this,
               () => (IntegerExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.TranslationY),
               VM => VM.Dispose());
      }
   }
}
