using Macro;
using MacroRuntime;

namespace MacroRuntime_TEST
{
   class MockContext : ContextBase
   {
      protected override void SymbolNotFoundSetValue(Symbol Symbol, ExpressionBase Value)
      {
         SymbolNotFundSetValueValue = Value;
      }

      public ExpressionBase SymbolNotFundSetValueValue { get; private set; }

      protected override ExpressionBase SymbolNotFoundGetValue(Symbol Symbol)
      {
         return DefautlValue;
      }

      public static readonly ExpressionBase DefautlValue = new Symbol("MockContext: DefautlValue");
   }
}
