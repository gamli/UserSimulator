using System;
using System.Linq;
using Macro;

namespace MacroRuntime
{
   public class ExpressionEvaluator
   {
      public static Expression Evaluate(Expression Expression, ContextBase Context)
      {
         return new ExpressionEvaluator(Context).Evaluate(Expression);
      }

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

      private class ExpressionVisitor : SpecialFormAwareVisitor
      {
         public Expression Value { get; private set; }

         private readonly ContextBase _context;

         public ExpressionVisitor(ContextBase Context)
         {
            _context = Context;
         }

         public override void VisitConstant(Constant Constant)
         {
            Value = Constant;
         }

         public override void VisitNil(List Nil)
         {
            Value = Nil;
         }

         public override void VisitDefinition(List Definition)
         {
            var evaluatedExpression = EvaluateExpression<Expression>(Definition.Expressions[2], _context);
            _context.DefineValue((Symbol)Definition.Expressions[1], evaluatedExpression); // TODO is cast ok?
            Value = evaluatedExpression;
         }

         public override void VisitIf(List If)
         {
            var condition = EvaluateExpression<Expression>(If.Expressions[1], _context);
            var consequentOrAlternative = TypeConversion.ConvertToBoolean(condition, _context) ? If.Expressions[2] : If.Expressions[3];
            Value = EvaluateExpression<Expression>(consequentOrAlternative, _context);
         }

         public override void VisitLambda(List Lambda)
         {
            Value = new Procedure { DefiningContext = _context, Lambda = Lambda, FormalArguments = (List)Lambda.Expressions[1] };// TODO is cast ok?
         }

         public override void VisitProcedureCall(List ProcedureCall)
         {
            var procedure = EvaluateExpression<ProcedureBase>(ProcedureCall.Expressions[0], _context);
            var evaluatedArgs =
               new List(ProcedureCall.Expressions.Skip(1).Select(
                  Expression => EvaluateExpression<Expression>(Expression, _context)).ToArray());
            Value = procedure.Call(evaluatedArgs);
         }

         public override void VisitQuote(List Quote)
         {
            Value = Quote.Expressions[1];
         }

         public override void VisitLoop(List Loop)
         {
            while (TypeConversion.ConvertToBoolean(EvaluateExpression<Expression>(Loop.Expressions[1], _context), _context))
               Value = EvaluateExpression<Expression>(Loop.Expressions[2], _context);
         }

         public override void VisitSymbol(Symbol Symbol)
         {
            Value = _context.GetValue(Symbol);
         }
         

         private T EvaluateExpression<T>(Expression Expression, ContextBase Context)
            where T : Expression
         {
            return (T)new ExpressionEvaluator(Context).Evaluate(Expression);
         }
      }
   }
}
