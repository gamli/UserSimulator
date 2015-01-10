using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroViewModel
{
   public class SymbolListVM : ListExpressionBaseVM<Symbol, SymbolVM>
   {
      public SymbolListVM(ListExpressionBase<Symbol> Model) 
         : base(Model)
      {
      }
   }
}
