using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Program : StatementWithBodyBase
   {
      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitProgram(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherProgram = (Program)OtherMacro;
         return base.BodyEquals(otherProgram);
      }
   }
}
