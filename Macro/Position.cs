using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Position : Base
   {
      private int _x;
      public int X { get { return _x; } set { SetPropertyValue(ref _x, value); } }

      private int _y;
      public int Y { get { return _y; } set { SetPropertyValue(ref _y, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitPosition(this);
      }
   }
}
