using Macro;
using MacroRuntime;

namespace MacroRuntime_TEST
{
   class MockContext : ContextBase
   {
      public static readonly object DefautlValue = new Symbol("MockContext: DefautlValue");

      protected override object SymbolNotFoundGetValue(Symbol Symbol)
      {
         return DefautlValue;
      }
   }
}
