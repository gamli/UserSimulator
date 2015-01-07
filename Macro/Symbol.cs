using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Symbol : AtomicExpressionBase
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
         return Value.Equals(other.Value);
      }

      [ExcludeFromCodeCoverage]
      public override string ToString()
      {
         return Value.ToString();
      }
   }
}
