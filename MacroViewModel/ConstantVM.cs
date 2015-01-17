using System.Diagnostics.CodeAnalysis;
using Macro;

namespace MacroViewModel
{
   [ExcludeFromCodeCoverage]
   public class ConstantVM : ExpressionVM
   {
      public new Constant Model
      {
         get { return (Constant) base.Model; }
      }

      public ConstantVM(Constant Model)
         : base(Model)
      {
      }
   }
}
