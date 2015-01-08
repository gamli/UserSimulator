using System.Diagnostics.CodeAnalysis;
using Macro;

namespace MacroViewModel
{
   public abstract class ExpressionBaseVM : MacroBaseVM<ExpressionBase>
   {
      [ExcludeFromCodeCoverage]
      protected ExpressionBaseVM(ExpressionBase Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
