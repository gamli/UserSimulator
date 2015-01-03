using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroViewModel
{
   public class BooleanExpressionBaseVM : ExpressionBaseVM<bool>
   {
      [ExcludeFromCodeCoverage]
      protected BooleanExpressionBaseVM(ExpressionBase<bool> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class StringExpressionBaseVM : ExpressionBaseVM<string>
   {
      [ExcludeFromCodeCoverage]
      protected StringExpressionBaseVM(ExpressionBase<string> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class IntegerExpressionBaseVM : ExpressionBaseVM<int>
   {
      [ExcludeFromCodeCoverage]
      protected IntegerExpressionBaseVM(ExpressionBase<int> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public abstract class ExpressionBaseVM<T> : MacroBaseVM<ExpressionBase<T>>
   {
      [ExcludeFromCodeCoverage]
      protected ExpressionBaseVM(ExpressionBase<T> Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
