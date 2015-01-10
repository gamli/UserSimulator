using System.Collections.ObjectModel;
using System.Linq;

namespace Macro
{
   public class ProcedureCall : ListExpressionBase<ExpressionBase>
   {
      private ExpressionBase _procedure;
      public ExpressionBase Procedure 
      { 
         get { return _procedure; } 
         set 
         {
            if (SetPropertyValue(ref _procedure, value))
               if(!Equals(Expressions[0], _procedure))
                  Expressions[0] = _procedure;
         } 
      }

      public ObservableCollection<ExpressionBase> Arguments { get; private set; }

      public ProcedureCall()
      {
         Arguments = new ObservableCollection<ExpressionBase>();

         Expressions.CollectionChanged +=
            (Sender, Args) =>
               {
                  Procedure = Expressions[0];
                  // todo bad algorithm
                  Arguments.Clear();
                  foreach (var expression in Expressions.Skip(1))
                     Arguments.Add(expression);
               };

         Expressions.Add(null);
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitProcedureCall(this);
      }
   }
}
