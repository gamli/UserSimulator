using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO;
using Macro;

namespace MacroRuntime
{
   public class ExpressionEvaluator
   {
      ExpressionVisitor _visitor;

      public ExpressionEvaluator(ContextBase Context)
      {
         _visitor = new ExpressionVisitor(Context);
      }

      public object Evaluate(ExpressionBase Expression)
      {
         Expression.Accept(_visitor);
         return _visitor.Value;
      }

      private class ExpressionVisitor : IVisitor
      {
         public object Value { get; private set; }

         private ContextBase _context;

         public ExpressionVisitor(ContextBase Context)
         {
            _context = Context;
         }

         public void VisitConstant(Constant Constant)
         {
            Value = Constant.Value;
         }

         public void VisitDefinition(Definition Definition)
         {
            var evaluatedExpression = EvaluateExpression<object>(Definition.Expression);
            _context.SetValue(Definition.Symbol, evaluatedExpression);
            Value = evaluatedExpression;
         }

         public void VisitFunctionCall(FunctionCall FunctionCall)
         {
            var function = (Func<object, IEnumerable<object>>)_context.GetValue(EvaluateExpression<Symbol>(FunctionCall.Function));
            Value = function(FunctionCall.Arguments.Select(EvaluateExpression<object>));
         }

         public void VisitIf(If If)
         {
            if (EvaluateExpression<bool>(If.Condition))
               Value = EvaluateExpression<object>(If.Consequent);
            else
               Value = EvaluateExpression<object>(If.Alternative);
         }

         public void VisitList(List List)
         {
            Value = List;
         }

         public void VisitLoop(Loop Loop)
         {
            while (EvaluateExpression<bool>(Loop.Condition))
               Value = EvaluateExpression<object>(Loop.Body);
         }

         public void VisitQuote(Quote Quote)
         {
            Value = Quote.Expression;
         }

         public void VisitSymbol(Symbol Symbol)
         {
            Value = _context.GetValue(Symbol);
         }

         private TValue EvaluateExpression<TValue>(ExpressionBase Expression)
         {
            return (TValue)new ExpressionEvaluator(new HierarchicalContext(_context)).Evaluate(Expression);
         }
      }
   }
}
