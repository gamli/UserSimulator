using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class ForLoop : MacroWithBodyBase
   {
      private int _repetitionCount;
      [ExcludeFromCodeCoverage]
      public int RepetitionCount { get { return _repetitionCount; } set { SetPropertyValue(ref _repetitionCount, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitForLoop(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherForLoop = (ForLoop)OtherMacro;
         return RepetitionCount == otherForLoop.RepetitionCount && Body.Equals(otherForLoop.Body);
      }
   }
}
