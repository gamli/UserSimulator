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
   public class PositionVM : MacroBaseVM<Position>
   {
      private NotifyingTransformedProperty<ExpressionBaseVM> _xVM;
      public ExpressionBaseVM XVM
      {
         get
         {
            return _xVM.Value;
         }
      }

      private NotifyingTransformedProperty<ExpressionBaseVM> _yVM;
      public ExpressionBaseVM YVM
      {
         get
         {
            return _yVM.Value;
         }
      }

      public PositionVM(Position Model)
         : base(Model)
      {
         _xVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "X" }, "XVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.X),
               VM => VM.Dispose());

         _yVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Y" }, "YVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Y),
               VM => VM.Dispose());
      }
   }
}
