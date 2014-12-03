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
   public class ForLoopVM : MacroWithBodyBaseVM<ForLoop>
   {
      public ForLoopVM(ForLoop Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
