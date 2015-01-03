using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class VariableAssignment<T> : StatementBase
   {
      private string _symbol;
      public string Symbol { get { return _symbol; } set { SetPropertyValue(ref _symbol, value); } }

      private ExpressionBase<T> _expression;
      public ExpressionBase<T> Expression { get { return _expression; } set { SetPropertyValue(ref _expression, value); } }


      public override void Accept(IVisitor Visitor)
      {
          Visitor.VisitVariableAssignment(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var other = (VariableAssignment<T>)OtherMacro;
         return
            Symbol.Equals(other.Symbol) &&
            Expression.Equals(other.Expression);
      }
   }
}
