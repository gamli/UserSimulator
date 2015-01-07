using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;
using MacroRuntime;

namespace MacroRuntime_TEST
{
   class MockContext : ContextBase
   {
      public static object DEFAULT_VALUE = new Symbol("MockContext: DEFAULT_VALUE");

      protected override object SymbolNotFoundGetValue(Symbol Symbol)
      {
         return DEFAULT_VALUE;
      }
   }
}
