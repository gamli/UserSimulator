using System;
using Macro;

namespace MacroRuntime
{
   public static class TypeConversion
   {
      public static bool ConvertToBoolean(Expression Expression, IContext Context)
      {
         var constant = Expression as Constant;
         if (constant != null)
            return Convert.ToBoolean(constant.Value);

         var expressionList = Expression as List;
         if (expressionList != null)
            return expressionList.Expressions.Count != 0;

         throw new RuntimeException(
            string.Format("Expression >> {0} << can not be converted to boolean", Expression),
            Expression,
            Context);
      }
   }
}
