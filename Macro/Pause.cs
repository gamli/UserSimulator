using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Pause : StatementBase
   {
      private ExpressionBase _duration;
      [ExcludeFromCodeCoverage]
      public ExpressionBase Duration { get { return _duration; } set { SetPropertyValue(ref _duration, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitPause(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherPause = (Pause)OtherMacro;
         return Duration.Equals(otherPause.Duration);
      }
   }
}
