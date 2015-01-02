using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Windowshot : ExpressionBase<bool>
   {
      private ExpressionBase<string> _imageUrl;
      [ExcludeFromCodeCoverage]
      public ExpressionBase<string> ImageUrl { get { return _imageUrl; } set { SetPropertyValue(ref _imageUrl, value); } }

      private ExpressionBase<int> _positionX;
      [ExcludeFromCodeCoverage]
      public ExpressionBase<int> PositionX { get { return _positionX; } set { SetPropertyValue(ref _positionX, value); } }

      private ExpressionBase<int> _positionY;
      [ExcludeFromCodeCoverage]
      public ExpressionBase<int> PositionY { get { return _positionY; } set { SetPropertyValue(ref _positionY, value); } }

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
