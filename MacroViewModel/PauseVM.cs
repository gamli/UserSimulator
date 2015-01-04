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
      private NotifyingTransformedProperty<ExpressionBaseVM> _durationVM;
      public ExpressionBaseVM DurationVM
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
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Duration" }, "DurationVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Duration),
               VM => VM.Dispose());
      }
   }
}
