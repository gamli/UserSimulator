using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using IO;
using Macro;

namespace MacroRuntime
{
   public class ExpressionEvaluator
   {
      private readonly IContext _context;

      public static Expression Evaluate(Expression Expression, IContext Context)
      {
         return new ExpressionEvaluator(Context).Evaluate(Expression);
      }

      public ExpressionEvaluator(IContext Context)
      {
         _context = Context;
      }

      public Expression Evaluate(Expression Expression)
      {
         var visitor = new ExpressionVisitor();
         try
         {
            CheckAbortedException(Expression);
            return visitor.EvaluateExpression(Expression, _context);
         }
         catch (Exception e)
         {
            Logger.Instance.Log("ExpressionEvaluator.Evaluate: " + e.Message);
            throw new RuntimeException("Exception during evaluation", Expression, _context, e);
         }
      }

      [ExcludeFromCodeCoverage]
      private void CheckAbortedException(Expression Expression)
      {
         if (!Keyboard.IsControlKeyDown() && Keyboard.IsF12KeyDown())
            throw new RuntimeException("Aborted by User", Expression, _context);
      }

      private class ExpressionVisitor : SpecialFormAwareVisitor
      {
         private Expression _value;
         private readonly Stack<Continuation> _continuationStack = new Stack<Continuation>();
         private class Continuation
         {
            public Expression Expression { get; set; }
            public IContext Context { get; set; }
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
                  FormalArguments = Lambda.Expressions[1]
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
            
            var argumentValues = EvaluatedArguments.ToList();

            var formalArguments = Procedure.FormalArguments;
            var symbol = formalArguments as Symbol;

            if (symbol != null)
            {

               var variableArgumentsList = new List(argumentValues.ToArray());
               procedureCallContext.DefineValue(symbol, variableArgumentsList);
            }
            else
            {
               var argumentSymbols = ((List)Procedure.FormalArguments).Expressions.Cast<Symbol>().ToList();

               if (argumentValues.Count != argumentSymbols.Count)
                  throw
                     new RuntimeException(
                        string.Format("Expected {0} argument(s) but got {1}", argumentSymbols.Count, argumentValues.Count),
                        ProcedureCall,
                        CurrentContext());

               foreach (var symbolAndArgumentValue in argumentSymbols.Zip(argumentValues, Tuple.Create))
                  procedureCallContext.DefineValue(symbolAndArgumentValue.Item1, symbolAndArgumentValue.Item2);
            }

            return procedureCallContext;
         }

         public override void VisitQuote(List Quote)
         {
            _value = Quote.Expressions[1];
         }

         public override void VisitLoop(List Loop)
         {
            _value = new List();
            var currentContext = CurrentContext();
            while (TypeConversion.ConvertToBoolean(EvaluateGeneric<Expression>(Loop.Expressions[1], currentContext), currentContext))
               _value = EvaluateGeneric<Expression>(Loop.Expressions[2], currentContext);
         }

         public override void VisitSetValue(List SetValue)
         {
            var currentContext = CurrentContext();
            currentContext.SetValue((Symbol)SetValue.Expressions[1], EvaluateExpression(SetValue.Expressions[2], currentContext));
         }

         public override void VisitSymbol(Symbol Symbol)
         {
            _value = CurrentContext().GetValue(Symbol);
         }

         public override void VisitProcedure(ProcedureBase Procedure)
         {
            _value = Procedure;
         }

         private IContext CurrentContext()
         {
            return _currentContext;
         }
         private IContext _currentContext;


         private T EvaluateGeneric<T>(Expression Expression, IContext Ctxt)
            where T : Expression
         {
            return (T)Evaluate(Expression, Ctxt);
         }

         public Expression EvaluateExpression(Expression Expression, IContext Context)
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
