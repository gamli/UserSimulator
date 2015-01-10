using System;
using System.Collections.Generic;
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

      public ExpressionBase Call(List<ExpressionBase> ArgumentValues)
      {
         var context = new HierarchicalContext(DefiningContext);
       
         // TODO implement partial argument list
         if (ArgumentValues.Count != ArgumentSymbols.Expressions.Count)
            throw 
               new RuntimeException(
                  string.Format("Expected {0} argument(s) but got {1}", ArgumentSymbols.Expressions.Count, ArgumentValues.Count),
                  this,
                  context);

         foreach (var symbolAndArgumentValue in ArgumentSymbols.Symbols.Zip(ArgumentValues, Tuple.Create))
            context.DefineValue(symbolAndArgumentValue.Item1, symbolAndArgumentValue.Item2);

         return ExecuteCall(context);
      }

      protected abstract ExpressionBase ExecuteCall(ContextBase Context);

      // TODO extract superclass from MacroBase to make this dummy implementation unnecessary
      public override void Accept(IVisitor Visitor)
      {
         throw new NotImplementedException();
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         return DefiningContext.Equals(((ProcedureBase) OtherMacro).DefiningContext);
      }
   }
}
