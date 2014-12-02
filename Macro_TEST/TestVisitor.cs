using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace Macro_TEST
{
   class TestVisitor : IVisitor
   {
      public List<MacroContainer> Macros { get; private set; }

      public TestVisitor()
      {
         Macros = new List<MacroContainer>();
      }

      public void BeginVisitProgram(Program Macro)
      {
         BeginVisit(Macro);
      }

      public void EndVisitProgram(Program Macro)
      {
         EndVisit(Macro);
      }

      public void VisitNoOp(NoOp NoOp)
      {
         Visit(NoOp);
      }

      public void VisitForLoop(ForLoop ForLoop)
      {
         Visit(ForLoop);
      }

      public void VisitMove(Move Move)
      {
         Visit(Move);
      }

      public void VisitPosition(Position Position)
      {
         Visit(Position);
      }

      public void VisitPause(Pause Pause)
      {
         Visit(Pause);
      }

      public void VisitImageEqualsWindowContent(ImageEqualsWindowContent ImageEqualsWindowContentConditional)
      {
         Visit(ImageEqualsWindowContentConditional);
      }

      public void VisitLeftClick(LeftClick LeftClick)
      {
         Visit(LeftClick);
      }

      private void BeginVisit(Macro.Base MacroBase)
      {
         var macroContainer = CreateAndAddMacroContainer(MacroBase);
         _macroContainerStack.Push(macroContainer);
      }
      private void Visit(Macro.Base MacroBase)
      {
         CreateAndAddMacroContainer(MacroBase);
      }
      private void EndVisit(Macro.Base MacroBase)
      {
         _macroContainerStack.Pop();
      }
      private MacroContainer CreateAndAddMacroContainer(Macro.Base MacroBase)
      {
         var macroContainer = new MacroContainer(MacroBase);
         if (_macroContainerStack.Count > 0)
            _macroContainerStack.Peek().Children.Add(macroContainer);
         else
            Macros.Add(macroContainer);
         return macroContainer;
      }

      private Stack<MacroContainer> _macroContainerStack = new Stack<MacroContainer>();

      public class MacroContainer
      {
         public List<MacroContainer> Children { get; private set; }

         public Macro.Base Macro { get; private set; }
         public MacroContainer(Macro.Base MacroBase)
         {
            this.Macro = MacroBase;
            Children = new List<MacroContainer>();
         }
      }
   }
}
