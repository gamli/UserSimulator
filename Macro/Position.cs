using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Position : MacroBase
   {
      private ExpressionBase<int> _x;
      [ExcludeFromCodeCoverage]
      public ExpressionBase<int> X { get { return _x; } set { SetPropertyValue(ref _x, value); } }

      private ExpressionBase<int> _y;
      [ExcludeFromCodeCoverage]
      public ExpressionBase<int> Y { get { return _y; } set { SetPropertyValue(ref _y, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitPosition(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherPosition = (Position)OtherMacro;
         return X == otherPosition.X && Y == otherPosition.Y;
      }
   }
}
