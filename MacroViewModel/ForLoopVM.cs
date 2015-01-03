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
   public class ForLoopVM : StatementWithBodyBaseVM<ForLoop>
   {
      private NotifyingTransformedProperty<IntegerExpressionBaseVM> _repetitionCountVM;
      public IntegerExpressionBaseVM RepetitionCountVM
      {
         get
         {
            return _repetitionCountVM.Value;
         }
      }

      public ForLoopVM(ForLoop Model)
         : base(Model)
      {
         _repetitionCountVM =
            new NotifyingTransformedProperty<IntegerExpressionBaseVM>(
               new[] { "RepetitionCount" }, "RepetitionCountVM",
               Model, this,
               () => (IntegerExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.RepetitionCount),
               VM => VM.Dispose());
      }
   }
}
