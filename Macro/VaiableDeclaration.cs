using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class VaiableDeclaration : StatementBase
   {
      public override void Accept(IVisitor Visitor)
      {
         throw new NotImplementedException();
         // Visitor.VisitVariableDeclaration(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         throw new NotImplementedException();
      }
   }
}
