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

      public void SetValue(Symbol Symbol, object Value)
      {
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
