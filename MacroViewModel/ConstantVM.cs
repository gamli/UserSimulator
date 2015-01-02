using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Macro;

namespace MacroViewModel
{
   public class ConstantBooleanExpressionVM : BooleanExpressionBaseVM
   {
      [ExcludeFromCodeCoverage]
      public ConstantBooleanExpressionVM(ConstantExpression<bool> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class ConstantStringExpressionVM : StringExpressionBaseVM
   {
      [ExcludeFromCodeCoverage]
      public ConstantStringExpressionVM(ConstantExpression<string> Model)
         : base(Model)
      {
         // nothing to do
      }
   }

   public class ConstantIntegerExpressionVM : IntegerExpressionBaseVM
   {
      [ExcludeFromCodeCoverage]
      public ConstantIntegerExpressionVM(ConstantExpression<int> Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
