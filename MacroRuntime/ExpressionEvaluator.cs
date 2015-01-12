using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
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

      public Expression Evaluate(Expression Expression)
      {
         try
         {
            Expression.Accept(_visitor);
            return _visitor.Value;
         }
         catch (Exception e)
         {
            Logger.Instance.Log("ExpressionEvaluator.Evaluate: " + e.Message);
            throw new RuntimeException("Exception during evaluation", Expression, _context, e);
         }
      }

      private class ExpressionVisitor : IVisitor
      {
         public Expression Value { get; private set; }

         private readonly ContextBase _context;

         public ExpressionVisitor(ContextBase Context)
         {
            _context = Context;
         }

         public void VisitConstant(Constant Constant)
         {
            Value = Constant;
         }

         public void VisitSymbol(Symbol Symbol)
         {
            Value = _context.GetValue(Symbol);
         }

         public void VisitList(List List)
         {
            if (List.Expressions.Count > 0)
            {
               var first = List.Expressions.First();

               var symbol = first as Symbol;
               if (symbol != null)
               {
                  switch (symbol.Value)
                  {
                     case "define":
                        EvaluateDefinition(List);
                        break;
                     case "if":
                        EvaluateIf(List);
                        break;
                     case "lambda":
                        EvaluateLambda(List);
                        break;
                     case "quote":
                        EvaluateQuote(List);
                        break;
                     case "loop":
                        EvaluateLoop(List);
                        break;
                     default:
                        EvaluateProcedureCall(List);
                        break;
                  }
               }
               else
                  EvaluateProcedureCall(List);
            }
            else
               Value = List;
         }

         private void EvaluateDefinition(List Definition)
         {
            var evaluatedExpression = EvaluateExpression<Expression>(Definition.Expressions[2], _context);
            _context.DefineValue((Symbol) Definition.Expressions[1], evaluatedExpression); // TODO is cast ok?
            Value = evaluatedExpression;
         }

         private void EvaluateIf(List If)
         {
            var condition = EvaluateExpression<Expression>(If.Expressions[1], _context);
            var consequentOrAlternative = ConvertToBoolean(condition) ? If.Expressions[2] : If.Expressions[3];
            Value = EvaluateExpression<Expression>(consequentOrAlternative, _context);
         }

         private void EvaluateLambda(List Lambda)
         {
            Value = new Procedure { DefiningContext = _context, Lambda = Lambda, FormalArguments = (List)Lambda.Expressions[1] };// TODO is cast ok?
         }

         private void EvaluateLoop(List Loop)
         {
            while ((bool)EvaluateExpression<Constant>(Loop.Expressions[1], _context).Value)
               Value = EvaluateExpression<Expression>(Loop.Expressions[2], _context);
         }

         private void EvaluateProcedureCall(List ProcedureCall)
         {
            var procedure = EvaluateExpression<ProcedureBase>(ProcedureCall.Expressions[0], _context);
            var evaluatedArgs = 
               new List(ProcedureCall.Expressions.Skip(1).Select(
                  Expression => EvaluateExpression<Expression>(Expression, _context)).ToArray());
            Value = procedure.Call(evaluatedArgs);
         }

         private void EvaluateQuote(List Quote)
         {
            Value = Quote.Expressions[1];
         }


         

         private T EvaluateExpression<T>(Expression Expression, ContextBase Context)
            where T : Expression
         {
            return (T)new ExpressionEvaluator(Context).Evaluate(Expression);
         }

         private bool ConvertToBoolean(Expression Expression)
         {
            var constant = Expression as Constant;
            if (constant != null)
               return Convert.ToBoolean(constant.Value);

            var expressionList = Expression as List;
            if (expressionList != null)
               return expressionList.Expressions.Count != 0;

            throw new RuntimeException(
               string.Format("Expression >> {0} << can not be converted to boolean", Expression),
               Expression,
               _context);
         }
      }
   }
}
