using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class ImageEqualsWindowContent : WithBodyBase
   {
      private Image _image;
      public Image Image { get { return _image; } set { SetPropertyValue(ref _image, value); } }

      private IntPtr _window;
      public IntPtr Window { get { return _window; } set { SetPropertyValue(ref _window, value); } }

      private int _positionX;
      public int PositionX { get { return _positionX; } set { SetPropertyValue(ref _positionX, value); } }

      private int _positionY;
      public int PositionY { get { return _positionY; } set { SetPropertyValue(ref _positionY, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitImageEqualsWindowContent(this);
      }
   }
}
