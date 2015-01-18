using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

      public ExpressionEvaluator(ContextBase Context)
      {
         _visitor = new ExpressionVisitor(Context);
      }

      public Expression Evaluate(Expression Expression)
      {
         try
         {
            _visitor.Value = null;
            _visitor.Continuation = Expression;
            while (_visitor.Value == null)
               _visitor.Continuation.Accept(_visitor);
            return _visitor.Value;
         }
         catch (Exception e)
         {
            Logger.Instance.Log("ExpressionEvaluator.Evaluate: " + e.Message);
            throw new RuntimeException("Exception during evaluation", _visitor.Value ?? _visitor.Continuation, _visitor.Context, e);
         }
      }

      private class ExpressionVisitor : SpecialFormAwareVisitor
      {
         public Expression Value { get; set; }
         public Expression Continuation { get; set; }

         public ContextBase Context { get; private set; }

         public ExpressionVisitor(ContextBase Context)
         {
            this.Context = Context;
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
            var evaluatedExpression = EvaluateExpression<Expression>(Definition.Expressions[2], Context);
            Context.DefineValue((Symbol)Definition.Expressions[1], evaluatedExpression); // TODO is cast ok?
            Value = evaluatedExpression;
         }

         public override void VisitIf(List If)
         {
            var condition = EvaluateExpression<Expression>(If.Expressions[1], Context);
            Continuation = TypeConversion.ConvertToBoolean(condition, Context) ? If.Expressions[2] : If.Expressions[3];
         }

         public override void VisitLambda(List Lambda)
         {
            Value = new Procedure
            {
               DefiningContext = Context,
               Lambda = Lambda,
               FormalArguments = (List)Lambda.Expressions[1]
            }; // TODO is cast ok?
         }

         public override void VisitProcedureCall(List ProcedureCall)
         {
            var procedureBase = EvaluateExpression<ProcedureBase>(ProcedureCall.Expressions[0], Context);
            var evaluatedArguments = 
               ProcedureCall.Expressions.Skip(1).Select(
                  Expression => EvaluateExpression<Expression>(Expression, Context)).ToList();

            InitializeProcedureCallContext(ProcedureCall, procedureBase, evaluatedArguments);

            var procedure = procedureBase as Procedure;
            if (procedure != null)
               Continuation = procedure.Lambda.Expressions[2];
            else
               Value = ((IntrinsicProcedure)procedureBase).Function(Context);
         }

         private void InitializeProcedureCallContext(List ProcedureCall, ProcedureBase Procedure, IEnumerable<Expression> EvaluatedArguments)
         {
            var callerContext = Context;

            Context = new HierarchicalContext(Procedure.DefiningContext);

            var argumentSymbols = Procedure.FormalArguments.Expressions.Cast<Symbol>().ToList();
            var argumentValues = EvaluatedArguments.ToList();

            if (IsVarargProcedure(argumentSymbols))
            {
               if (argumentValues.Count < argumentSymbols.Count - 1)
                  throw
                     new RuntimeException(
                        string.Format("Expected minimum of {0} argument(s) but got {1}", argumentSymbols.Count - 1,
                           argumentValues.Count),
                        ProcedureCall,
                        callerContext);

               var fixedArgumentCount = Procedure.FormalArguments.Expressions.Count - 1;
               SetVarargProcedureArguments(
                  argumentSymbols, 
                  argumentValues.Take(fixedArgumentCount),
                  argumentValues.Skip(fixedArgumentCount));
            }
            else
            {

               if (argumentValues.Count != argumentSymbols.Count && !IsVarargProcedure(argumentSymbols))
                  throw
                     new RuntimeException(
                        string.Format("Expected {0} argument(s) but got {1}",
                           Procedure.FormalArguments.Expressions.Count, argumentValues.Count),
                        ProcedureCall,
                        callerContext);

               SetFixedargProcedureArguments(argumentSymbols, argumentValues);
            }
         }

         private void SetFixedargProcedureArguments(
            IEnumerable<Symbol> ArgumentSymbols, 
            IEnumerable<Expression> ArgumentValues)
         {
            foreach (var symbolAndArgumentValue in ArgumentSymbols.Zip(ArgumentValues, Tuple.Create))
               Context.DefineValue(symbolAndArgumentValue.Item1, symbolAndArgumentValue.Item2);
         }

         private void SetVarargProcedureArguments(
            IEnumerable<Symbol> ArgumentSymbols, 
            IEnumerable<Expression> FixedArgumentValues,
            IEnumerable<Expression> VariableArgumentValues)
         {
            foreach (var symbolAndArgumentValue in FixedArgumentValues.Zip(ArgumentSymbols, Tuple.Create))
               Context.DefineValue(symbolAndArgumentValue.Item2, symbolAndArgumentValue.Item1);

            var variableArgumentsList = new List(VariableArgumentValues.ToArray());
            Context.DefineValue(new Symbol("."), variableArgumentsList);
         }

         private static bool IsVarargProcedure(List<Symbol> ArgumentSymbols)
         {
            return (ArgumentSymbols.Count != 0 && ArgumentSymbols.Last().Value == ".");
         }

         public override void VisitQuote(List Quote)
         {
            Value = Quote.Expressions[1];
         }

         public override void VisitLoop(List Loop)
         {
            var context = Context;

            while (TypeConversion.ConvertToBoolean(EvaluateExpression<Expression>(Loop.Expressions[1], context), context))
               Value = EvaluateExpression<Expression>(Loop.Expressions[2], context);
         }

         public override void VisitSymbol(Symbol Symbol)
         {
            Value = Context.GetValue(Symbol);
         }


         private T EvaluateExpression<T>(Expression Expression, ContextBase Context)
            where T : Expression
         {
            return (T)new ExpressionEvaluator(Context).Evaluate(Expression);
         }
      }
   }
}
