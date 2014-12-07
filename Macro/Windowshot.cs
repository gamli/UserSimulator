using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Windowshot : MacroWithBodyBase
   {
      private string _imageUrl;
      [ExcludeFromCodeCoverage]
      public string ImageUrl { get { 
         return _imageUrl; } set { SetPropertyValue(ref _imageUrl, value); } }

      private int _positionX;
      [ExcludeFromCodeCoverage]
      public int PositionX { get { return _positionX; } set { SetPropertyValue(ref _positionX, value); } }

      private int _positionY;
      [ExcludeFromCodeCoverage]
      public int PositionY { get { return _positionY; } set { SetPropertyValue(ref _positionY, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitWindowshot(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var other = (Windowshot)OtherMacro;
         return
            ImageUrl.Equals(other.ImageUrl) &&
            PositionX == other.PositionX &&
            PositionY == other.PositionY &&
            Body.Equals(other.Body);
      }
   }
}
