using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Constant : AtomicExpressionBase
   {
      private object _value;
      public object Value { get { return _value; } set { SetPropertyValue(ref _value, value); } }

      public Constant()
      {
         // nothing to do
      }

      public Constant(object Value)
      {
         this.Value = Value;
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitConstant(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherConstantExpression = (Constant)OtherMacro;
         return Value == null ? otherConstantExpression.Value == null : Value.Equals(otherConstantExpression.Value);
      }
   }
}
