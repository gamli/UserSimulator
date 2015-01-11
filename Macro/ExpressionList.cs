using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class ExpressionList : ListExpressionBase<ExpressionBase>
   {
      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitExpressionList(this);
      }
   }
}
