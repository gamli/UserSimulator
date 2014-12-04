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
