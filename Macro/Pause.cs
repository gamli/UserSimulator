using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Pause : Base
   {
      private TimeSpan _duration;
      public TimeSpan Duration { get { return _duration; } set { SetPropertyValue(ref _duration, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitPause(this);
      }
   }
}
