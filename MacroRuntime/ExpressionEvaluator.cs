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

      public ExpressionBase Evaluate(ExpressionBase Expression)
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
         public ExpressionBase Value { get; private set; }

         private readonly ContextBase _context;

         public ExpressionVisitor(ContextBase Context)
         {
            _context = Context;
         }

         public void VisitConstant(Constant Constant)
         {
            Value = Constant;
         }

         public void VisitDefinition(Definition Definition)
         {
            var evaluatedExpression = EvaluateExpression<ExpressionBase>(Definition.Expression, _context);
            _context.DefineValue(Definition.Symbol, evaluatedExpression);
            Value = evaluatedExpression;
         }

         // TODO should not happen?
         public void VisitExpressionList(ExpressionList ExpressionList)
         {
            throw new RuntimeException("Can not evaluate expression list", ExpressionList, _context);
         }

         public void VisitIf(If If)
         {
            var consequentOrAlternative = (bool)EvaluateExpression<Constant>(If.Condition, _context).Value ? If.Consequent : If.Alternative;
            Value = EvaluateExpression<ExpressionBase>(consequentOrAlternative, _context);
         }

         public void VisitLambda(Lambda Lambda)
         {
            Value = new Procedure { DefiningContext = _context, Lambda = Lambda, ArgumentSymbols = Lambda.ArgumentSymbols };
         }

         public void VisitLoop(Loop Loop)
         {
            while ((bool)EvaluateExpression<Constant>(Loop.Condition, _context).Value)
               Value = EvaluateExpression<ExpressionBase>(Loop.Body, _context);
         }

         public void VisitProcedureCall(ProcedureCall ProcedureCall)
         {
            var procedure = EvaluateExpression<ProcedureBase>(ProcedureCall.Procedure, _context);
            var evaluatedArgs = new ExpressionList();
            foreach (var arg in ProcedureCall.Arguments)
               evaluatedArgs.Expressions.Add(EvaluateExpression<ExpressionBase>(arg, _context));
            Value = procedure.Call(evaluatedArgs);
         }

         public void VisitQuote(Quote Quote)
         {
            Value = Quote.Expression;
         }

         public void VisitSymbol(Symbol Symbol)
         {
            Value = _context.GetValue(Symbol);
         }

         // TODO should not happen?
         public void VisitSymbolList(SymbolList SymbolList)
         {
            throw new RuntimeException("Can not evaluate symbol list", SymbolList, _context);
         }

         private T EvaluateExpression<T>(ExpressionBase Expression, ContextBase Context)
            where T : ExpressionBase
         {
            return (T)new ExpressionEvaluator(Context).Evaluate(Expression);
         }
      }
   }
}
