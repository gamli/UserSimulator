using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Move : MacroBase
   {
      private int _translationX;
      public int TranslationX { get { return _translationX; } set { SetPropertyValue(ref _translationX, value); } }

      private int _translationY;
      public int TranslationY { get { return _translationY; } set { SetPropertyValue(ref _translationY, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitMove(this);
      }
   }
}
