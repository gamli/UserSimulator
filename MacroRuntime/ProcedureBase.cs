using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Macro;

namespace MacroRuntime
{
   public abstract class ProcedureBase : Expression
   {
      private ContextBase _definingContext;
      public ContextBase DefiningContext { get { return _definingContext; } set { SetPropertyValue(ref _definingContext, value); } }

      private List _formalArguments;
      public List FormalArguments { get { return _formalArguments; } set { SetPropertyValue(ref _formalArguments, value); } }

      public Expression Call(List ArgumentValues)
      {
         var context = new HierarchicalContext(DefiningContext);

         var argumentSymbols = FormalArguments.Expressions.Cast<Symbol>().ToList();
         
         var argumentValues = ArgumentValues.Expressions;
         
         var isVarargProcedure =
            (argumentSymbols.Count != 0 && argumentSymbols.Last().Value == ".");

         if (argumentValues.Count != argumentSymbols.Count && !isVarargProcedure)
            throw 
               new RuntimeException(
                  string.Format("Expected {0} argument(s) but got {1}", FormalArguments.Expressions.Count, argumentValues.Count),
                  this,
                  context);

         if (isVarargProcedure)
         {
            if (argumentValues.Count < argumentSymbols.Count - 1)
               throw
                  new RuntimeException(
                     string.Format("Expected minimum of {0} argument(s) but got {1}", argumentSymbols.Count - 1, argumentValues.Count),
                     this,
                     context);

            var fixedArgumentSymbols = argumentSymbols.Take(argumentSymbols.Count - 1);
            foreach (var symbolAndArgumentValue in fixedArgumentSymbols.Zip(argumentValues, Tuple.Create))
               context.DefineValue(symbolAndArgumentValue.Item1, symbolAndArgumentValue.Item2);
            
            var varArgValues = new List();
            foreach (var varArgValue in argumentValues.Skip(argumentSymbols.Count - 1))
               varArgValues.Expressions.Add(varArgValue);
            context.DefineValue(new Symbol("."), varArgValues);
         }
         else
            foreach (var symbolAndArgumentValue in argumentSymbols.Zip(argumentValues, Tuple.Create))
               context.DefineValue(symbolAndArgumentValue.Item1, symbolAndArgumentValue.Item2);

         return ExecuteCall(context);
      }

      protected abstract Expression ExecuteCall(ContextBase Context);

      // TODO extract superclass from MacroBase to make this dummy implementation unnecessary
      [ExcludeFromCodeCoverage]
      public override void Accept(IVisitor Visitor)
      {
         throw new NotImplementedException();
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherProcedure = (ProcedureBase) OtherMacro;
         return 
            DefiningContext.Equals(otherProcedure.DefiningContext) && 
            FormalArguments.Equals(otherProcedure.FormalArguments);
      }
   }
}
