using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroRuntime
{
   public abstract class ContextBase
   {
      private Dictionary<string, object> _values = new Dictionary<string, object>();

      public void DefineValue(Symbol Symbol, object Value)
      {
         try
         {
            _values.Add(Symbol.Value, Value);
         }
         catch(ArgumentException E)
         {
            string exceptionMessage = string.Format("Symbol >>{0}<< is already defined", Symbol.Value);
            throw new RuntimeException(exceptionMessage, Symbol, this, E);
         }
      }

      public void SetValue(Symbol Symbol, object Value)
      {
         if (!_values.ContainsKey(Symbol.Value))
         {
            string exceptionMessage = string.Format("Symbol >>{0}<< is not defined", Symbol.Value);
            throw new RuntimeException(exceptionMessage, Symbol, this);
         }
         _values[Symbol.Value] = Value;
      }

      public object GetValue(Symbol Symbol)
      {
         if (_values.ContainsKey(Symbol.Value))
            return _values[Symbol.Value];
         return SymbolNotFoundGetValue(Symbol);
      }

      protected abstract object SymbolNotFoundGetValue(Symbol Symbol);
   }
}
