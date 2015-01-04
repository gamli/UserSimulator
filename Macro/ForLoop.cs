using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class ForLoop : StatementWithBodyBase
   {
      private ExpressionBase _repetitionCount;
      [ExcludeFromCodeCoverage]
      public ExpressionBase RepetitionCount { get { return _repetitionCount; } set { SetPropertyValue(ref _repetitionCount, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitForLoop(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherForLoop = (ForLoop)OtherMacro;
         return RepetitionCount.Equals(otherForLoop.RepetitionCount) && base.BodyEquals(otherForLoop);
      }
   }
}
