using System;
using System.Collections.Generic;
using System.Linq;
using Macro;

namespace MacroRuntime
{
   public class ExpressionEvaluator
   {
      private readonly ExpressionVisitor _visitor;
      private readonly ContextBase _context;

      public ExpressionEvaluator(ContextBase Context)
      {
         _context = Context;
         _visitor = new ExpressionVisitor(Context);
      }

      public object Evaluate(ExpressionBase Expression)
      {
         try
         {
            Expression.Accept(_visitor);
            return _visitor.Value;
         }
         catch (RuntimeException e)
         {
            Logger.Instance.Log("ExpressionEvaluator.Evaluate: " + e.Message);
            throw;
         }
         catch (Exception e)
         {
            Logger.Instance.Log("ExpressionEvaluator.Evaluate: " + e.Message);
            throw new RuntimeException("Unknown exception", Expression, _context, e);
         }
      }

      private class ExpressionVisitor : IVisitor
      {
         public object Value { get; private set; }

         private readonly ContextBase _context;

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
            _context.DefineValue(Definition.Symbol, evaluatedExpression);
            Value = evaluatedExpression;
         }

         public void VisitFunctionCall(FunctionCall FunctionCall)
         {
            var function = EvaluateExpression<Func<List<object>, object>>(FunctionCall.Function);
            var args = FunctionCall.Arguments.Select(EvaluateExpression<object>).ToList();
            Value = function(args);
         }

         public void VisitIf(If If)
         {
            var consequentOrAlternative = EvaluateExpression<bool>(If.Condition) ? If.Consequent : If.Alternative;
            Value = EvaluateExpression<object>(consequentOrAlternative);
         }

         public void VisitList(List List)
         {
            throw new RuntimeException("Cannot evaluate list", List, _context);
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
