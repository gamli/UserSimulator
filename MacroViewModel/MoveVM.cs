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
      private NotifyingTransformedProperty<ExpressionBaseVM> _translationXVM;
      public ExpressionBaseVM TranslationXVM
      {
         get
         {
            return _translationXVM.Value;
         }
      }

      private NotifyingTransformedProperty<ExpressionBaseVM> _translationYVM;
      public ExpressionBaseVM TranslationYVM
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
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "TranslationX" }, "TranslationXVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.TranslationX),
               VM => VM.Dispose());

         _translationYVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "TranslationY" }, "TranslationYVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.TranslationY),
               VM => VM.Dispose());
      }
   }
}
