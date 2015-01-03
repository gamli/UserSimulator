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
   public class PauseVM : MacroBaseVM<Pause>
   {
      private NotifyingTransformedProperty<IntegerExpressionBaseVM> _durationVM;
      public IntegerExpressionBaseVM DurationVM
      {
         get
         {
            return _durationVM.Value;
         }
      }

      public PauseVM(Pause Model)
         : base(Model)
      {
         _durationVM =
            new NotifyingTransformedProperty<IntegerExpressionBaseVM>(
               new[] { "Duration" }, "DurationVM",
               Model, this,
               () => (IntegerExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Duration),
               VM => VM.Dispose());
      }
   }
}
