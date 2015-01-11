using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroRuntime
{
   public abstract class ProcedureBase : ExpressionBase
   {
      private ContextBase _definingContext;
      public ContextBase DefiningContext { get { return _definingContext; } set { SetPropertyValue(ref _definingContext, value); } }

      private SymbolList _argumentSymbols;
      public SymbolList ArgumentSymbols { get { return _argumentSymbols; } set { SetPropertyValue(ref _argumentSymbols, value); } }

      public ExpressionBase Call(ExpressionList ArgumentValues)
      {
         var context = new HierarchicalContext(DefiningContext);

         var isVarargProcedure =
            (ArgumentSymbols.Symbols.Count != 0 && ArgumentSymbols.Symbols.Last().Value == ".");

         if (ArgumentValues.Expressions.Count != ArgumentSymbols.Symbols.Count && !isVarargProcedure)
            throw 
               new RuntimeException(
                  string.Format("Expected {0} argument(s) but got {1}", ArgumentSymbols.Expressions.Count, ArgumentValues.Expressions.Count),
                  this,
                  context);

         if (isVarargProcedure)
         {
            foreach (var symbolAndArgumentValue in ArgumentSymbols.Symbols.Zip(ArgumentValues.Expressions, Tuple.Create).Take(ArgumentSymbols.Symbols.Count - 1))
               context.DefineValue(symbolAndArgumentValue.Item1, symbolAndArgumentValue.Item2);
            var varArgValues = new ExpressionList();
            foreach (var varArgValue in ArgumentValues.Expressions.Skip(ArgumentSymbols.Symbols.Count - 1))
               varArgValues.Expressions.Add(varArgValue);
            context.DefineValue(new Symbol("."), varArgValues);
         }
         else
            foreach (var symbolAndArgumentValue in ArgumentSymbols.Symbols.Zip(ArgumentValues.Expressions, Tuple.Create))
               context.DefineValue(symbolAndArgumentValue.Item1, symbolAndArgumentValue.Item2);

         return ExecuteCall(context);
      }

      protected abstract ExpressionBase ExecuteCall(ContextBase Context);

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
            ArgumentSymbols.Equals(otherProcedure.ArgumentSymbols);
      }
   }
}
