using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Move : StatementBase
   {
      private ExpressionBase<int> _translationX;
      [ExcludeFromCodeCoverage]
      public ExpressionBase<int> TranslationX { get { return _translationX; } set { SetPropertyValue(ref _translationX, value); } }

      private ExpressionBase<int> _translationY;
      [ExcludeFromCodeCoverage]
      public ExpressionBase<int> TranslationY { get { return _translationY; } set { SetPropertyValue(ref _translationY, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitMove(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherMove = (Move)OtherMacro;
         return TranslationX == otherMove.TranslationX && TranslationY == otherMove.TranslationY;
      }
   }
}
