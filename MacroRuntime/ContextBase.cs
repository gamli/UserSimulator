using System;
using System.Collections.Generic;
using Macro;

namespace MacroRuntime
{
   public abstract class ContextBase
   {
      private readonly Dictionary<string, ExpressionBase> _values = new Dictionary<string, ExpressionBase>();

      public void DefineValue(Symbol Symbol, ExpressionBase Value)
      {
         try
         {
            _values.Add(Symbol.Value, Value);
         }
         catch(ArgumentException e)
         {
            string exceptionMessage = string.Format("Symbol >>{0}<< is already defined (did you mean 'set!' instead of 'define'?)", Symbol.Value);
            throw new RuntimeException(exceptionMessage, Symbol, this, e);
         }
      }

      public void SetValue(Symbol Symbol, ExpressionBase Value)
      {
         if (_values.ContainsKey(Symbol.Value))
            _values[Symbol.Value] = Value;
         SymbolNotFoundSetValue(Symbol, Value);
      }

      protected abstract void SymbolNotFoundSetValue(Symbol Symbol, ExpressionBase Value);

      public ExpressionBase GetValue(Symbol Symbol)
      {
         if (_values.ContainsKey(Symbol.Value))
            return _values[Symbol.Value];
         return SymbolNotFoundGetValue(Symbol);
      }

      protected abstract ExpressionBase SymbolNotFoundGetValue(Symbol Symbol);
   }
}
