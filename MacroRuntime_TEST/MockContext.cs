using Macro;
using MacroRuntime;

namespace MacroRuntime_TEST
{
   class MockContext : ContextBase
   {
      protected override void SymbolNotFoundSetValue(Symbol Symbol, Expression Value)
      {
         SymbolNotFundSetValueValue = Value;
      }

      public Expression SymbolNotFundSetValueValue { get; private set; }

      protected override Expression SymbolNotFoundGetValue(Symbol Symbol)
      {
         return DefautlValue;
      }

      public static readonly Expression DefautlValue = new Symbol("MockContext: DefautlValue");
   }
}
