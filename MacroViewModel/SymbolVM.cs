using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroViewModel
{
   public class SymbolVM : ExpressionBaseVM
   {
      [ExcludeFromCodeCoverage]
      public SymbolVM(Symbol Model)
         : base (Model)
      {
         // nothing to do
      }
   }
}
