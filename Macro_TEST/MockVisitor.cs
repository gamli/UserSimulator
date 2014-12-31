using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace Macro_TEST
{
   class MockVisitor : IVisitor
   {
      public List<MacroContainer> Macros { get; private set; }

      public MockVisitor()
      {
         Macros = new List<MacroContainer>();
      }

      public void VisitProgram(Program Program)
      {
         VisitMacroWithBody(Program);
      }

      public void VisitBlock(Block Block)
      {
         BeginVisit(Block);
         foreach (var item in Block.Items)
            item.Accept(this);
         EndVisit(Block);
      }

      public void VisitNoOp(NoOp NoOp)
      {
         Visit(NoOp);
      }

      public void VisitForLoop(ForLoop ForLoop)
      {
         VisitMacroWithBody(ForLoop);
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

      public void VisitWindowshot(Windowshot Windowshot)
      {
         Visit(Windowshot);
      }

      public void VisitLeftClick(LeftClick LeftClick)
      {
         Visit(LeftClick);
      }

      public void VisitConstantExpression<T>(ConstantExpression<T> ConstantExpression)
      {
         Visit(ConstantExpression);
      }

      public void VisitIfStatement(IfStatement IfStatement)
      {
         IfStatement.Expression.Accept(this);
         VisitMacroWithBody(IfStatement);
      }

      private void VisitMacroWithBody(MacroWithBodyBase MacroWithBody)
      {
         BeginVisit(MacroWithBody);
         MacroWithBody.Body.Accept(this);
         EndVisit(MacroWithBody);
      }

      private void BeginVisit(MacroBase MacroBase)
      {
         var macroContainer = CreateAndAddMacroContainer(MacroBase);
         _macroContainerStack.Push(macroContainer);
      }
      private void Visit(MacroBase MacroBase)
      {
         CreateAndAddMacroContainer(MacroBase);
      }
      private void EndVisit(MacroBase MacroBase)
      {
         _macroContainerStack.Pop();
      }
      private MacroContainer CreateAndAddMacroContainer(MacroBase MacroBase)
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

         public Macro.MacroBase Macro { get; private set; }
         public MacroContainer(Macro.MacroBase MacroBase)
         {
            this.Macro = MacroBase;
            Children = new List<MacroContainer>();
         }
      }
   }
}
