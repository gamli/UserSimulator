using System.Collections.ObjectModel;
using System.Linq;

namespace Macro
{
   public class FunctionCall : List
   {
      private ExpressionBase _function;
      public ExpressionBase Function 
      { 
         get { return _function; } 
         set 
         {
            if (SetPropertyValue(ref _function, value))
               if(!Equals(Expressions[0], _function))
                  Expressions[0] = _function;
         } 
      }

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

         Expressions.Add(null);
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitFunctionCall(this);
      }
   }
}
