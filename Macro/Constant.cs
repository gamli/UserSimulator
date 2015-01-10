using System.Diagnostics.CodeAnalysis;
using System.Globalization;

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

      [ExcludeFromCodeCoverage]
      public override string ToString()
      {
         return 
            Value == null 
               ? "null" 
               : Value is string 
                  ? "\"" + Value + "\"" 
                  : Value is double 
                     ? ((double)Value).ToString(CultureInfo.InvariantCulture) 
                     : Value.ToString();
      }
   }
}
