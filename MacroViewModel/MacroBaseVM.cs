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
   public abstract class MacroWithBodyBaseVM<T> : MacroBaseVM<T>
      where T : MacroWithBodyBase
   {
      private NotifyingTransformedProperty<MacroBaseVM> _bodyVM;
      public MacroBaseVM BodyVM
      {
         get
         {
            return _bodyVM.Value;
         }
      }

      protected MacroWithBodyBaseVM(T Model)
         : base(Model)
      {
         _bodyVM =
            new NotifyingTransformedProperty<MacroBaseVM>(
               new[] { "Body" }, "BodyVM",
               Model, this,
               () => MacroViewModelFactory.Instance.Create(Model.Body),
               VM => VM.Dispose());
      }
   }

   [ExcludeFromCodeCoverage]
   public abstract class MacroBaseVM<T> : MacroBaseVM
      where T : MacroBase
   {
      public new T Model
      {
         get
         {
            return (T)base.Model;
         }
      }

      protected MacroBaseVM(T Model)
         : base(Model)
      {

      }
   }

   public abstract class MacroBaseVM : ViewModelBase<MacroBase>
   {
      [ExcludeFromCodeCoverage]
      protected MacroBaseVM(MacroBase Model)
         : base(Model)
      {

      }
   }
}
