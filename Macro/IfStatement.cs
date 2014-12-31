using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class IfStatement : MacroWithBodyBase
   {
      private ExpressionBase<bool> _expression;
      [ExcludeFromCodeCoverage]
      public ExpressionBase<bool> Expression { get { return _expression; } set { SetPropertyValue(ref _expression, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitIfStatement(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherIfStatement = (IfStatement)OtherMacro;
         return Expression.Equals(otherIfStatement.Expression) && base.BodyEquals(otherIfStatement);
      }
   }
}
