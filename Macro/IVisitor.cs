using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Common;

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
   }
}
