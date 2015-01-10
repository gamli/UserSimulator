using System.Linq.Expressions;

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

         public void VisitDefinition(Definition Definition)
         {
            VisitListGeneric(Definition);
         }

         public void VisitIf(If If)
         {
            VisitListGeneric(If);
         }

         public void VisitLambda(Lambda Lambda)
         {
            VisitListGeneric(Lambda);
         }

         public void VisitLoop(Loop Loop)
         {
            VisitListGeneric(Loop);
         }

         public void VisitProcedureCall(ProcedureCall ProcedureCall)
         {
            VisitListGeneric(ProcedureCall);
         }

         public void VisitQuote(Quote Quote)
         {
            VisitListGeneric(Quote);
         }

         public void VisitSymbol(Symbol Symbol)
         {
            Clone = new Symbol(Symbol.Value);
         }

         public void VisitSymbolList(SymbolList SymbolList)
         {
            VisitListGeneric<SymbolList, Symbol>(SymbolList);
         }

         private void VisitListGeneric<TList>(TList List)
            where TList : ListExpressionBase<ExpressionBase>, new()
         {
            VisitListGeneric<TList, ExpressionBase>(List);
         }

         private void VisitListGeneric<TList, TExpression>(TList List)
            where TList : ListExpressionBase<TExpression>, new() 
            where TExpression : ExpressionBase
         {
            var clone = new TList();
            CloneListsExpressions(List, clone);
            Clone = clone;
         }

         private static void CloneListsExpressions<TExpression>(ListExpressionBase<TExpression> List, ListExpressionBase<TExpression> Clone)
            where TExpression : ExpressionBase
         {
            while (Clone.Expressions.Count < List.Expressions.Count)
               Clone.Expressions.Add(null);
            var index = 0;
            foreach (var expression in List.Expressions)
            {
               Clone.Expressions[index] = MacroCloner.Clone(expression);
               index++;
            }
         }
      }
   }
}
