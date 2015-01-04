using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Loop : SpecialFormBase
   {
      private ExpressionBase _condition;
      [ExcludeFromCodeCoverage]
      public ExpressionBase Condition { get { return _condition; } set { SetPropertyValue(ref _condition, value, 0); } }

      private ExpressionBase _body;
      [ExcludeFromCodeCoverage]
      public ExpressionBase Body { get { return _body; } set { SetPropertyValue(ref _body, value, 1); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitLoop(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherLoop = (Loop)OtherMacro;
         return 
            Condition.Equals(otherLoop.Condition) && 
            Body.Equals(otherLoop.Body);
      }
   }
}
