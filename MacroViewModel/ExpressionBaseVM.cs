using System.Diagnostics.CodeAnalysis;
using Macro;

namespace MacroViewModel
{
   public abstract class ExpressionBaseVM<T> : ExpressionBaseVM
      where T : ExpressionBase
   {
      [ExcludeFromCodeCoverage]
      public new T Model
      {
         get
         {
            return (T)base.Model;
         }
      }

      [ExcludeFromCodeCoverage]
      protected ExpressionBaseVM(T Model)
         : base(Model)
      {
         // nothing to do
      }
   }

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
