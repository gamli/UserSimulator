using System.Diagnostics.CodeAnalysis;
using Macro;

namespace MacroViewModel
{
   public class ConstantVM : ExpressionVM
   {
      [ExcludeFromCodeCoverage]
      public ConstantVM(Constant Model)
         : base(Model)
      {
      }
   }
}
