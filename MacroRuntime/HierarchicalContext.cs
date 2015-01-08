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

      protected override object SymbolNotFoundGetValue(Symbol Symbol)
      {
         return _parentContext.GetValue(Symbol);
      }
   }
}
