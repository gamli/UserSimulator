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
      private NotifyingTransformedProperty<IntegerExpressionBaseVM> _xVM;
      public IntegerExpressionBaseVM XVM
      {
         get
         {
            return _xVM.Value;
         }
      }

      private NotifyingTransformedProperty<IntegerExpressionBaseVM> _yVM;
      public IntegerExpressionBaseVM YVM
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
            new NotifyingTransformedProperty<IntegerExpressionBaseVM>(
               new[] { "X" }, "XVM",
               Model, this,
               () => (IntegerExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.X),
               VM => VM.Dispose());

         _yVM =
            new NotifyingTransformedProperty<IntegerExpressionBaseVM>(
               new[] { "Y" }, "YVM",
               Model, this,
               () => (IntegerExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Y),
               VM => VM.Dispose());
      }
   }
}
