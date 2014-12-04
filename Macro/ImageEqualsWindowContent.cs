using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class ImageEqualsWindowContent : MacroWithBodyBase
   {
      private Image _image;
      [ExcludeFromCodeCoverage]
      public Image Image { get { return _image; } set { SetPropertyValue(ref _image, value); } }

      private IntPtr _window;
      [ExcludeFromCodeCoverage]
      public IntPtr Window { get { return _window; } set { SetPropertyValue(ref _window, value); } }

      private int _positionX;
      [ExcludeFromCodeCoverage]
      public int PositionX { get { return _positionX; } set { SetPropertyValue(ref _positionX, value); } }

      private int _positionY;
      [ExcludeFromCodeCoverage]
      public int PositionY { get { return _positionY; } set { SetPropertyValue(ref _positionY, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitImageEqualsWindowContent(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var other = (ImageEqualsWindowContent)OtherMacro;
         return
            Image.Equals(other.Image) &&
            Window.Equals(other.Window) &&
            PositionX == other.PositionX &&
            PositionY == other.PositionY &&
            Body.Equals(other.Body);
      }
   }
}
