using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroViewModel
{
   public class IfVM : StatementWithBodyBaseVM<If>
   {
      private NotifyingTransformedProperty<BooleanExpressionBaseVM> _expressionVM;
      public BooleanExpressionBaseVM ExpressionVM
      {
         get
         {
            return _expressionVM.Value;
         }
      }

      public IfVM(If Model)
         : base(Model)
      {
         _expressionVM =
            new NotifyingTransformedProperty<BooleanExpressionBaseVM>(
               new[] { "Expression" }, "ExpressionVM",
               Model, this,
               () => (BooleanExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Expression),
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
