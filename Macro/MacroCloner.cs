using System.Linq;

namespace Macro
{
   public class MacroCloner
   {
      public static TMacro Clone<TMacro>(TMacro Macro)
         where TMacro : MacroBase
      {
         var programCloneVisitor = new CloneVisitor();
         Macro.Accept(programCloneVisitor);
         return (TMacro)programCloneVisitor.ClonedMacro;
      }

      private class CloneVisitor : IVisitor
      {
         public MacroBase ClonedMacro { get; private set; }

         public void VisitConstant(Constant Constant)
         {
            ClonedMacro = new Constant(Constant.Value);
         }

         public void VisitList(List List)
         {
            ClonedMacro = new List(List.Expressions.Select<Expression, Expression>(Clone).ToArray());
         }

         public void VisitSymbol(Symbol Symbol)
         {
            ClonedMacro = new Symbol(Symbol.Value);
         }
      }
   }
}
