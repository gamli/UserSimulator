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
               });
         }

         public void VisitLeftClick(LeftClick LeftClick)
         {
            WithClone(new LeftClick());
         }
         
         public void VisitConstant(Constant ConstantExpression)
         {
            WithClone(new Constant(ConstantExpression.Value));
         }

         public void VisitIf(If If)
         {
            WithClone(new If { Expression = CloneExpression(If.Expression) }, () => If.Body.Accept(this));
         }

         public void VisitVariableAssignment(VariableAssignment VariableAssignment)
         {
            WithClone(new VariableAssignment { Symbol = VariableAssignment.Symbol, Expression = CloneExpression(VariableAssignment.Expression) });
         }

         private void WithClone(StatementBase MacroClone, Action Action = null)
         {
            WithClone((MacroBase)MacroClone);

            if (_macroStack.Count > 0)
            {
               var topLevelMacro = _macroStack.Peek();
               if (topLevelMacro is StatementWithBodyBase)
               {
                  var statementWithBody = (StatementWithBodyBase)topLevelMacro;
                  statementWithBody.Body = MacroClone;
               }
               else
               {
                  var block = (Block)topLevelMacro;
                  block.Items.Add(MacroClone);
               }
            }
            var putOnStack = MacroClone is StatementWithBodyBase || MacroClone is Block;
            if (putOnStack)
               _macroStack.Push(MacroClone);
            if(Action != null)
               Action();
            if (putOnStack)
               _macroStack.Pop();
         }

         private void WithClone(MacroBase MacroClone)
         {
            if (Clone == null)
               Clone = MacroClone;
         }
         private Stack<MacroBase> _macroStack = new Stack<MacroBase>();

         private ExpressionBase CloneExpression(ExpressionBase Expression)
         {
            var cloneVisitor = new CloneVisitor();
            Expression.Accept(cloneVisitor);
            return (ExpressionBase)cloneVisitor.Clone;
         }
      }
   }
}
