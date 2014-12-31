using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public static class ConstantExpressions
   {  
      public static ConstantExpression<T> Create<T>(T Value)
      {
         return new ConstantExpression<T> { Value = Value };
      }
   }

   public class ConstantExpression<T> : ExpressionBase<T>
   {
      private T _value;
      [ExcludeFromCodeCoverage]
      public T Value { get { return _value; } set { SetPropertyValue(ref _value, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitConstantExpression(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherConstantExpression = (ConstantExpression<T>)OtherMacro;
         return Value.Equals(otherConstantExpression.Value);
      }
   }
}
