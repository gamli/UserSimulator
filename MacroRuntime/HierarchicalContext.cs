using Macro;

namespace MacroRuntime
{
   public class HierarchicalContext : ContextBase
   {
      private readonly ContextBase _parentContext;

      public HierarchicalContext(ContextBase ParentContext)
      {
         _parentContext = ParentContext;
      }

      protected override void SymbolNotFoundSetValue(Symbol Symbol, ExpressionBase Value)
      {
         _parentContext.SetValue(Symbol, Value);
      }

      protected override ExpressionBase SymbolNotFoundGetValue(Symbol Symbol)
      {
         return _parentContext.GetValue(Symbol);
      }
   }
}
