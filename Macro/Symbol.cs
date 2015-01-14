using System.Diagnostics.CodeAnalysis;

namespace Macro
{
   public class Symbol : AtomicExpression
   {
      private string _value;
      public string Value { get { return _value; } set { SetPropertyValue(ref _value, value); } }

      public Symbol()
      {
         // nothing to do
      }

      public Symbol(string Value)
      {
         this.Value = Value;
      }


      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitSymbol(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var other = (Symbol)OtherMacro;
         return Equals(Value, other.Value);
      }

      protected override int MacroGetHashCode()
      {
         return Value == null ? 0 : Value.GetHashCode();
      }

      [ExcludeFromCodeCoverage]
      public override string ToString()
      {
         return Value;
      }
   }
}
