using System.Diagnostics.CodeAnalysis;
using Macro;

namespace MacroViewModel
{
   public class ConstantVM : ExpressionVM
   {
      public new Constant Model
      {
         get { return (Constant) base.Model; }
      }

      [ExcludeFromCodeCoverage]
      public ConstantVM(Constant Model)
         : base(Model)
      {
      }
   }
}
