using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroViewModel
{
   public class BooleanExpressionBaseVM : ExpressionBaseVM
   {
      [ExcludeFromCodeCoverage]
      protected BooleanExpressionBaseVM(ExpressionBase<bool> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class StringExpressionBaseVM : ExpressionBaseVM
   {
      [ExcludeFromCodeCoverage]
      protected StringExpressionBaseVM(ExpressionBase<string> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class IntegerExpressionBaseVM : ExpressionBaseVM
   {
      [ExcludeFromCodeCoverage]
      protected IntegerExpressionBaseVM(ExpressionBase<int> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class ExpressionBaseVM : MacroBaseVM<ExpressionBase>
   {
      [ExcludeFromCodeCoverage]
      protected ExpressionBaseVM(ExpressionBase Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
