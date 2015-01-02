using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class NoOp : StatementBase
   {
      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitNoOp(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         return true;
      }
   }
}
