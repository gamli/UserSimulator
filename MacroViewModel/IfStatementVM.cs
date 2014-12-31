using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroViewModel
{
   public class IfStatementVM : MacroWithBodyBaseVM<IfStatement>
   {
      private NotifyingTransformedProperty<ExpressionBaseVM> _expressionVM;
      public ExpressionBaseVM ExpressionVM
      {
         get
         {
            return _expressionVM.Value;
         }
      }

      public IfStatementVM(IfStatement Model)
         : base(Model)
      {
         _expressionVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Expression" }, "ExpressionVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Expression),
               VM => VM.Dispose());
      }

      protected override void Dispose(bool Disposing)
      {
         if (Disposing)
            _expressionVM.Dispose();
         base.Dispose(Disposing);
      }
   }
}
