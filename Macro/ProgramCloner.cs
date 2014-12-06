using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class ProgramCloner
   {
      private Stack<MacroBase> _macroStack = new Stack<MacroBase>();

      public Program Clone()
      {
         var programCloneVisitor = new ProgramCloneVisitor();
         _program.Accept(programCloneVisitor);
         return programCloneVisitor.Clone;
      }

      public ProgramCloner(Program Program)
      {
         _program = Program;
      }

      Program _program;

      private class ProgramCloneVisitor : IVisitor
      {
         public Program Clone { get; private set; }

         public void VisitProgram(Program Program)
         {
            Clone = new Program();
            WithClone(Clone, () => Program.Body.Accept(this));            
         }

         public void VisitBlock(Block Block)
         {
            WithClone(
               new Block(), 
               () => 
                  { 
                     foreach (var item in Block.Items) 
                        item.Accept(this); 
                  });
         }

         public void VisitNoOp(NoOp NoOp)
         {
            WithClone(new NoOp());
         }

         public void VisitForLoop(ForLoop ForLoop)
         {
            WithClone(new ForLoop { RepetitionCount = ForLoop.RepetitionCount }, () => ForLoop.Body.Accept(this));
         }

         public void VisitMove(Move Move)
         {
            WithClone(new Move { TranslationX = Move.TranslationX, TranslationY = Move.TranslationY });
         }

         public void VisitPosition(Position Position)
         {
            WithClone(new Position { X = Position.X, Y = Position.Y });
         }

         public void VisitPause(Pause Pause)
         {
            WithClone(new Pause { Duration = Pause.Duration });
         }

         public void VisitWindowshot(Windowshot Windowshot)
         {
            WithClone(
               new Windowshot
               {
                  Window = Windowshot.Window,
                  Image = Windowshot.Image,
                  PositionX = Windowshot.PositionX,
                  PositionY = Windowshot.PositionY
               },
               () => Windowshot.Body.Accept(this));
         }

         public void VisitLeftClick(LeftClick LeftClick)
         {
            WithClone(new LeftClick());
         }

         private void WithClone(MacroBase Clone, Action Action = null)
         {
            if (_macroStack.Count > 0)
            {
               var topLevelMacro = _macroStack.Peek();
               if (topLevelMacro is MacroWithBodyBase)
               {
                  var macroWithBody = (MacroWithBodyBase)topLevelMacro;
                  macroWithBody.Body = Clone;
               }
               else
               {
                  var block = (Block)topLevelMacro;
                  block.Items.Add(Clone);
               }
            }
            var putOnStack = Clone is MacroWithBodyBase || Clone is Block;
            if (putOnStack)
               _macroStack.Push(Clone);
            if(Action != null)
               Action();
            if (putOnStack)
               _macroStack.Pop();
         }
         private Stack<MacroBase> _macroStack = new Stack<MacroBase>();
      }
   }
}
