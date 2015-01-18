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

      protected override bool SymbolNotFoundIsValueDefined(Symbol Symbol)
      {
         return _parentContext.IsValueDefined(Symbol);
      }

      protected override void SymbolNotFoundSetValue(Symbol Symbol, Expression Value)
      {
         _parentContext.SetValue(Symbol, Value);
      }

      protected override Expression SymbolNotFoundGetValue(Symbol Symbol)
      {
         return _parentContext.GetValue(Symbol);
      }
   }
}
