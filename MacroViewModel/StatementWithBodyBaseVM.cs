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
   public abstract class StatementWithBodyBaseVM<T> : MacroBaseVM<T>
      where T : StatementWithBodyBase
   {
      private NotifyingTransformedProperty<MacroBaseVM> _bodyVM;
      public MacroBaseVM BodyVM
      {
         get
         {
            return _bodyVM.Value;
         }
      }

      protected StatementWithBodyBaseVM(T Model)
         : base(Model)
      {
         _bodyVM =
            new NotifyingTransformedProperty<MacroBaseVM>(
               new[] { "Body" }, "BodyVM",
               Model, this,
               () => MacroViewModelFactory.Instance.Create(Model.Body),
               VM => VM.Dispose());
      }

      protected override void Dispose(bool Disposing)
      {
         if (Disposing)
            _bodyVM.Dispose();
         base.Dispose(Disposing);
      }
   }
}
