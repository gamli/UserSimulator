using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class FunctionCall : List
   {
      private ExpressionBase _function;
      public ExpressionBase Function { get { return _function; } private set { SetPropertyValue(ref _function, value, 0); } }

      public ObservableCollection<ExpressionBase> Arguments { get; private set; }

      public FunctionCall()
      {
         Arguments = new ObservableCollection<ExpressionBase>();
         Expressions.CollectionChanged +=
            (Sender, Args) =>
               {
                  Function = Expressions[0];
                  // todo bad algorithm
                  Arguments.Clear();
                  foreach (var expression in Expressions.Skip(1))
                     Arguments.Add(expression);
               };
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitFunctionCall(this);
      }
   }
}
