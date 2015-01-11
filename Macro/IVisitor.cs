namespace Macro
{
   public interface IVisitor
   {
      void VisitConstant(Constant Constant);

      void VisitDefinition(Definition Definition);

      void VisitExpressionList(ExpressionList ExpressionList);

      void VisitIf(If If);

      void VisitLambda(Lambda Lambda);

      void VisitLoop(Loop Loop);

      void VisitProcedureCall(ProcedureCall ProcedureCall);

      void VisitQuote(Quote Quote);

      void VisitSymbol(Symbol Symbol);

      void VisitSymbolList(SymbolList SymbolList);
   }
}
