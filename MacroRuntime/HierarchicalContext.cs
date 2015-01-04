using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroRuntime
{
   public class HierarchicalContext : ContextBase
   {
      private ContextBase _parentContext;

      public HierarchicalContext(ContextBase ParentContext)
      {
         _parentContext = ParentContext;
      }

      public object SymbolNotFoundGetValue(Symbol Symbol)
      {
         return _parentContext.GetValue(Symbol);
      }
   }
}
