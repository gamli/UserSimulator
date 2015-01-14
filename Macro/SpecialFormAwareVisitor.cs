using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public abstract class SpecialFormAwareVisitor : IVisitor
   {
      public abstract void VisitConstant(Constant Constant);

      public void VisitList(List List)
      {
         if (List.Expressions.Count > 0)
         {
            var first = List.Expressions.First();

            var symbol = first as Symbol;
            if (symbol != null)
            {
               switch (symbol.Value)
               {
                  case "define":
                     VisitDefinition(List);
                     break;
                  case "if":
                     VisitIf(List);
                     break;
                  case "lambda":
                     VisitLambda(List);
                     break;
                  case "quote":
                     VisitQuote(List);
                     break;
                  case "loop":
                     VisitLoop(List);
                     break;
                  default:
                     VisitProcedureCall(List);
                     break;
               }
            }
            else
               VisitProcedureCall(List);
         }
         else
            VisitNil(List);
      }

      public abstract void VisitNil(List Nil);
      public abstract void VisitDefinition(List Definition);
      public abstract void VisitIf(List If);
      public abstract void VisitLambda(List Lambda);
      public abstract void VisitProcedureCall(List ProcedureCall);
      public abstract void VisitQuote(List Quote);
      public abstract void VisitLoop(List Loop);

      public abstract void VisitSymbol(Symbol Symbol);
   }
}
