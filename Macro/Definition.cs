using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Definition : SpecialFormBase
   {
      private Symbol _symbol;
      public Symbol Symbol { get { return _symbol; } set { SetPropertyValue(ref _symbol, value, 0); } }

      private ExpressionBase _expression;
      public ExpressionBase Expression { get { return _expression; } set { SetPropertyValue(ref _expression, value, 1); } }

      public Definition()
      {
         Expressions.CollectionChanged +=
            (Sender, Arsg) => 
               {
                  Symbol = (Symbol)Expressions[0];
                  Expression = Expressions[1];
               };
      }

      public override void Accept(IVisitor Visitor)
      {
          Visitor.VisitDefinition(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var other = (Definition)OtherMacro;
         return
            Symbol.Equals(other.Symbol) &&
            Expression.Equals(other.Expression);
      }
   }
}
