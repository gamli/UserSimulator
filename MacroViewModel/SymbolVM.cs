using System.Diagnostics.CodeAnalysis;
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
