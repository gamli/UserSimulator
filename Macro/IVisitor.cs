namespace Macro
{
   public interface IVisitor
   {
      void VisitDefinition(Definition Definition);

      void VisitSymbol(Symbol Symbol);

      void VisitConstant(Constant Constant);

      void VisitList(List List);

      void VisitFunctionCall(FunctionCall FunctionCall);

      void VisitLoop(Loop Loop);

      void VisitIf(If If);

      void VisitQuote(Quote Quote);
   }
}
