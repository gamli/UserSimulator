using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Windowshot : ExpressionBase
   {
      private ExpressionBase _imageUrl;
      public ExpressionBase ImageUrl { get { return _imageUrl; } set { SetPropertyValue(ref _imageUrl, value); } }

      private ExpressionBase _positionX;
      [ExcludeFromCodeCoverage]
      public ExpressionBase PositionX { get { return _positionX; } set { SetPropertyValue(ref _positionX, value); } }

      private ExpressionBase _positionY;
      [ExcludeFromCodeCoverage]
      public ExpressionBase PositionY { get { return _positionY; } set { SetPropertyValue(ref _positionY, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitWindowshot(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var other = (Windowshot)OtherMacro;
         return
            ImageUrl.Equals(other.ImageUrl) &&
            PositionX.Equals(other.PositionX) &&
            PositionY.Equals(other.PositionY);
      }
   }
}
