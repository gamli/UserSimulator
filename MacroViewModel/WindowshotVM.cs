using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroViewModel
{
   [ExcludeFromCodeCoverage]
   public class WindowshotVM : MacroBaseVM<Windowshot>
   {
      public WindowshotVM(Windowshot Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
