using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Position : StatementBase
   {
      private ExpressionBase _x;
      [ExcludeFromCodeCoverage]
      public ExpressionBase X { get { return _x; } set { SetPropertyValue(ref _x, value); } }

      private ExpressionBase _y;
      [ExcludeFromCodeCoverage]
      public ExpressionBase Y { get { return _y; } set { SetPropertyValue(ref _y, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitPosition(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherPosition = (Position)OtherMacro;
         return X.Equals(otherPosition.X) && Y.Equals(otherPosition.Y);
      }
   }
}
