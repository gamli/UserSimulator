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
         var programCloneVisitor = new CloneVisitor();
         _program.Accept(programCloneVisitor);
         return (Program)programCloneVisitor.Clone;
      }

      public ProgramCloner(Program Program)
      {
         _program = Program;
      }

      Program _program;

      private class CloneVisitor : IVisitor
      {
         public MacroBase Clone { get; private set; }

         public void VisitProgram(Program Program)
         {
            WithClone(new Program(), () => Program.Body.Accept(this));            
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
                  ImageUrl = Windowshot.ImageUrl,
                  PositionX = Windowshot.PositionX,
                  PositionY = Windowshot.PositionY
               },
               () => Windowshot.Body.Accept(this));
         }

         public void VisitLeftClick(LeftClick LeftClick)
         {
            WithClone(new LeftClick());
         }
         
         public void VisitConstantExpression<T>(ConstantExpression<T> ConstantExpression)
         {
            WithClone(new ConstantExpression<T> { Value = ConstantExpression.Value });
         }

         public void VisitIfStatement(IfStatement IfStatement)
         {
            WithClone(new IfStatement { Expression = CloneExpression(IfStatement.Expression) }, () => IfStatement.Body.Accept(this));
         }

         private void WithClone(MacroBase MacroClone, Action Action = null)
         {
            if (Clone == null)
               Clone = MacroClone;

            if (_macroStack.Count > 0)
            {
               var topLevelMacro = _macroStack.Peek();
               if (topLevelMacro is MacroWithBodyBase)
               {
                  var macroWithBody = (MacroWithBodyBase)topLevelMacro;
                  macroWithBody.Body = MacroClone;
               }
               else
               {
                  var block = (Block)topLevelMacro;
                  block.Items.Add(MacroClone);
               }
            }
            var putOnStack = MacroClone is MacroWithBodyBase || MacroClone is Block;
            if (putOnStack)
               _macroStack.Push(MacroClone);
            if(Action != null)
               Action();
            if (putOnStack)
               _macroStack.Pop();
         }
         private Stack<MacroBase> _macroStack = new Stack<MacroBase>();

         private ExpressionBase<T> CloneExpression<T>(ExpressionBase<T> Expression)
         {
            var cloneVisitor = new CloneVisitor();
            Expression.Accept(cloneVisitor);
            return (ExpressionBase<T>)cloneVisitor.Clone;
         }
      }
   }
}
