using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class NoOp : Base
   {
      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitNoOp(this);
      }
   }
}
