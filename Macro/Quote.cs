using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Quote : SpecialFormBase
   {
      private ExpressionBase _expression;
      public ExpressionBase Expression { get { return _expression; } set { SetPropertyValue(ref _expression, value); } }

      public Quote()
         : base("quote", "Expression")
      {
         // nothing to do
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitQuote(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherQuote = (Quote)OtherMacro;
         return Expression.Equals(otherQuote.Expression) && base.MacroEquals(otherQuote);
      }
   }
}
