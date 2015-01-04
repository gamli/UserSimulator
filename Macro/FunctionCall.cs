using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class FunctionCall : List
   {
      private ExpressionBase _function;
      public ExpressionBase Function { get { return _function; } private set { SetPropertyValue(ref _function, value); } }

      public FunctionCall()
      {
         Expressions.CollectionChanged += (Sender, Arsg) => Function = Expressions[0];
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitFunctionCall(this);
      }
   }
}
