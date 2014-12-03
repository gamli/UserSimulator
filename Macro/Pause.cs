using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Pause : MacroBase
   {
      private TimeSpan _duration;
      [ExcludeFromCodeCoverage]
      public TimeSpan Duration { get { return _duration; } set { SetPropertyValue(ref _duration, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitPause(this);
      }
   }
}
