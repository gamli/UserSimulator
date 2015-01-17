namespace Macro
{
   public class MacroCloner
   {
      public static TMacro Clone<TMacro>(TMacro Macro)
         where TMacro : MacroBase
      {
         var programCloneVisitor = new CloneVisitor();
         Macro.Accept(programCloneVisitor);
         return (TMacro)programCloneVisitor.Clone;
      }

      private class CloneVisitor : IVisitor
      {
         public MacroBase Clone { get; private set; }

         public void VisitConstant(Constant Constant)
         {
            Clone = new Constant(Constant.Value);
         }

         public void VisitList(List List)
         {
            var clone = new List();
            foreach (var expression in List.Expressions)
               clone.Expressions.Add(Clone(expression));
            Clone = clone;
         }

         public void VisitSymbol(Symbol Symbol)
         {
            Clone = new Symbol(Symbol.Value);
         }
      }
   }
}
