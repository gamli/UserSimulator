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
   public class ForLoopVM : StatementWithBodyBaseVM<Loop>
   {
      private NotifyingTransformedProperty<ExpressionBaseVM> _repetitionCountVM;
      public ExpressionBaseVM RepetitionCountVM
      {
         get
         {
            return _repetitionCountVM.Value;
         }
      }

      public ForLoopVM(Loop Model)
         : base(Model)
      {
         _repetitionCountVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "RepetitionCount" }, "RepetitionCountVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Body),
               VM => VM.Dispose());
      }
   }
}
