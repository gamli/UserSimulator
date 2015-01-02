using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class If : StatementWithBodyBase
   {
      private ExpressionBase<bool> _expression;
      [ExcludeFromCodeCoverage]
      public ExpressionBase<bool> Expression { get { return _expression; } set { SetPropertyValue(ref _expression, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitIf(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherIf = (If)OtherMacro;
         return Expression.Equals(otherIf.Expression) && base.BodyEquals(otherIf);
      }
   }
}
