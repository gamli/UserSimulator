using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroViewModel
{
   public class LeftClickVM : MacroBaseVM<LeftClick>
   {
      [ExcludeFromCodeCoverage]
      public LeftClickVM(LeftClick Model)
         : base (Model)
      {
         // nothing to do
      }
   }
}
