using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Macro;

namespace MacroRuntime
{
   public class ExpressionEvaluator
   {
      private readonly ContextBase _context;

      public static Expression Evaluate(Expression Expression, ContextBase Context)
      {
         return new ExpressionEvaluator(Context).Evaluate(Expression);
      }

      public ExpressionEvaluator(ContextBase Context)
      {
         _context = Context;
      }

      public Expression Evaluate(Expression Expression)
      {
         var visitor = new ExpressionVisitor();
         try
         {
            return visitor.EvaluateExpression(Expression, _context);
         }
         catch (Exception e)
         {
            Logger.Instance.Log("ExpressionEvaluator.Evaluate: " + e.Message);
            throw new RuntimeException("Exception during evaluation", Expression, _context, e);
         }
      }

      private class ExpressionVisitor : SpecialFormAwareVisitor
      {
         private Expression _value;
         private readonly Stack<Continuation> _continuationStack = new Stack<Continuation>();
         private class Continuation
         {
            public Expression Expression { get; set; }
            public ContextBase Context { get; set; }
         }

         public override void VisitConstant(Constant Constant)
         {
            _value = Constant;
         }

         public override void VisitNil(List Nil)
         {
            _value = Nil;
         }

         public override void VisitDefinition(List Definition)
         {
            var currentContext = CurrentContext();
            var evaluatedExpression = EvaluateGeneric<Expression>(Definition.Expressions[2], currentContext);
            currentContext.DefineValue((Symbol)Definition.Expressions[1], evaluatedExpression);
            _value = evaluatedExpression;
         }

         public override void VisitIf(List If)
         {
            var currentContext = CurrentContext();
            var evaluatedCondition = EvaluateGeneric<Expression>(If.Expressions[1], currentContext);
            var consequentOrAlternative =
               TypeConversion.ConvertToBoolean(evaluatedCondition, currentContext)
                  ? If.Expressions[2]
                  : If.Expressions[3];
            _continuationStack.Push(new Continuation { Expression = consequentOrAlternative, Context = currentContext });
         }

         public override void VisitLambda(List Lambda)
         {
            _value = 
               new Procedure
               {
                  DefiningContext = CurrentContext(),
                  Lambda = Lambda,
                  FormalArguments = (List)Lambda.Expressions[1]
               };
         }

         public override void VisitProcedureCall(List ProcedureCall)
         {
            var currentContext = CurrentContext();
            var procedureBase = EvaluateGeneric<ProcedureBase>(ProcedureCall.Expressions[0], currentContext);
            var evaluatedArguments = 
               ProcedureCall.Expressions.Skip(1).Select(
                  Expression => EvaluateGeneric<Expression>(Expression, currentContext)).ToList();

            var procedureCallContext = CreateProcedureCallContext(ProcedureCall, procedureBase, evaluatedArguments);

            var procedure = procedureBase as Procedure;
            if (procedure != null)
               _continuationStack.Push(new Continuation { Expression = procedure.Lambda.Expressions[2], Context = procedureCallContext });
            else
               _value = ((IntrinsicProcedure)procedureBase).Function(procedureCallContext);
         }

         private ContextBase CreateProcedureCallContext(List ProcedureCall, ProcedureBase Procedure, IEnumerable<Expression> EvaluatedArguments)
         {
            var procedureCallContext = new HierarchicalContext(Procedure.DefiningContext);

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
                        CurrentContext());

               var fixedArgumentCount = Procedure.FormalArguments.Expressions.Count - 1;
               SetVarargProcedureArguments(
                  procedureCallContext,
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
                        CurrentContext());

               SetFixedargProcedureArguments(
                  procedureCallContext, 
                  argumentSymbols, 
                  argumentValues);
            }

            return procedureCallContext;
         }

         private static void SetFixedargProcedureArguments(
            ContextBase Context,
            IEnumerable<Symbol> ArgumentSymbols, 
            IEnumerable<Expression> ArgumentValues)
         {
            foreach (var symbolAndArgumentValue in ArgumentSymbols.Zip(ArgumentValues, Tuple.Create))
               Context.DefineValue(symbolAndArgumentValue.Item1, symbolAndArgumentValue.Item2);
         }

         private static void SetVarargProcedureArguments(
            ContextBase Context,
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
            _value = Quote.Expressions[1];
         }

         public override void VisitLoop(List Loop)
         {
            var currentContext = CurrentContext();
            while (TypeConversion.ConvertToBoolean(EvaluateGeneric<Expression>(Loop.Expressions[1], currentContext), currentContext))
               _value = EvaluateGeneric<Expression>(Loop.Expressions[2], currentContext);
         }

         public override void VisitSymbol(Symbol Symbol)
         {
            _value = CurrentContext().GetValue(Symbol);
         }

         private ContextBase CurrentContext()
         {
            return _currentContext;
         }
         private ContextBase _currentContext;


         private T EvaluateGeneric<T>(Expression Expression, ContextBase Ctxt)
            where T : Expression
         {
            return (T)Evaluate(Expression, Ctxt);
         }

         public Expression EvaluateExpression(Expression Expression, ContextBase Context)
         {
            Contract.Assert(_continuationStack.Count == 0);

            _value = null;
            _continuationStack.Push(new Continuation { Expression = Expression, Context = Context });

            while (_value == null)
            {
               var continuation = _continuationStack.Pop();
               _currentContext = continuation.Context;
               continuation.Expression.Accept(this);
            }

            return _value;
         }
      }
   }
}
