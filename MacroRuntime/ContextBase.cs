using System;
using System.Collections.Generic;
using Macro;

namespace MacroRuntime
{
   public abstract class ContextBase
   {
      private readonly Dictionary<string, Expression> _values = new Dictionary<string, Expression>();

      public void DefineValue(Symbol Symbol, Expression Value)
      {
         try
         {
            _values.Add(Symbol.Value, Value);
         }
         catch(ArgumentException e)
         {
            string exceptionMessage = string.Format("Symbol >>{0}<< is already defined (did you mean 'set' instead of 'define'?)", Symbol.Value);
            throw new RuntimeException(exceptionMessage, Symbol, this, e);
         }
      }

      public bool IsValueDefined(Symbol Symbol)
      {
         if (_values.ContainsKey(Symbol.Value))
            return true;
         return SymbolNotFoundIsValueDefined(Symbol);
      }

      protected abstract bool SymbolNotFoundIsValueDefined(Symbol Symbol);

      public void SetValue(Symbol Symbol, Expression Value)
      {
         if (_values.ContainsKey(Symbol.Value))
            _values[Symbol.Value] = Value;
         else
            SymbolNotFoundSetValue(Symbol, Value);
      }

      protected abstract void SymbolNotFoundSetValue(Symbol Symbol, Expression Value);

      public Expression GetValue(Symbol Symbol)
      {
         if (_values.ContainsKey(Symbol.Value))
            return _values[Symbol.Value];
         return SymbolNotFoundGetValue(Symbol);
      }

      protected abstract Expression SymbolNotFoundGetValue(Symbol Symbol);
   }
}
