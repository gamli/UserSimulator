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
   public class MoveVM : MacroBaseVM<Move>
   {
      public MoveVM(Move Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
