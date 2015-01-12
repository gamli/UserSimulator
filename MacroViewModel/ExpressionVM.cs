using System.Diagnostics.CodeAnalysis;
using Macro;

namespace MacroViewModel
{
   public abstract class ExpressionVM : MacroBaseVM<Expression>
   {
      [ExcludeFromCodeCoverage]
      protected ExpressionVM(Expression Model) 
         : base(Model)
      {
      }
   }
}
