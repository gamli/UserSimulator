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
      void BeginVisitProgram(Program Macro);
      void EndVisitProgram(Program Macro);

      void VisitNoOp(NoOp NoOp);
      void VisitForLoop(ForLoop ForLoop);

      void VisitMove(Move Move);
      void VisitPosition(Position Position);
      void VisitPause(Pause Pause);
      void VisitImageEqualsWindowContent(ImageEqualsWindowContent ImageEqualsWindowContentConditional);

      void VisitLeftClick(LeftClick LeftClick);
   }
}
