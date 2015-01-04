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
      void VisitProgram(Program Macro);

      void VisitBlock(Block Block);

      void VisitForLoop(ForLoop ForLoop);

      void VisitMove(Move Move);

      void VisitPosition(Position Position);

      void VisitPause(Pause Pause);

      void VisitWindowshot(Windowshot Windowshot);

      void VisitLeftClick(LeftClick LeftClick);

      void VisitConstant(Constant Constant);

      void VisitIf(If If);

      void VisitVariableAssignment(VariableAssignment VariableAssignment);
   }
}
