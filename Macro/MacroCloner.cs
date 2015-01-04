using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

         public void VisitDefinition(Definition Definition)
         {
            VisitListGeneric(Definition);
         }

         public void VisitSymbol(Symbol Symbol)
         {
            Clone = new Symbol { Value = Symbol.Value };
         }

         public void VisitConstant(Constant Constant)
         {
            Clone = new Constant(Constant.Value);
         }

         public void VisitList(List List)
         {
            VisitListGeneric(List);
         }

         private void VisitListGeneric<TList>(TList List)
            where TList : List, new()
         {
            var clone = new TList();
            CloneListsExpressions(List, clone);
            Clone = clone;
         }

         private static void CloneListsExpressions(List List, List Clone)
         {
            foreach (var expression in List.Expressions)
               Clone.Expressions.Add(MacroCloner.Clone(expression));
         }

         public void VisitFunctionCall(FunctionCall FunctionCall)
         {
            VisitListGeneric(FunctionCall);
         }

         public void VisitLoop(Loop Loop)
         {
            VisitListGeneric(Loop);
         }

         public void VisitIf(If If)
         {
            VisitListGeneric(If);
         }
      }
   }
}
