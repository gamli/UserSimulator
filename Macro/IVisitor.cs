namespace Macro
{
   public interface IVisitor
   {
      void VisitConstant(Constant Constant);

      void VisitList(List List);

      void VisitSymbol(Symbol Symbol);

      void VisitProcedure(ProcedureBase Procedure);
   }
}
