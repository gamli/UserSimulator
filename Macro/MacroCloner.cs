using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class MacroCloner
   {
      private Stack<MacroBase> _macroStack = new Stack<MacroBase>();

      public MacroBase Clone()
      {
         var programCloneVisitor = new CloneVisitor();
         _macro.Accept(programCloneVisitor);
         return (MacroBase)programCloneVisitor.Clone;
      }

      public MacroCloner(MacroBase Macro)
      {
         _macro = Macro;
      }

      MacroBase _macro;

      private class CloneVisitor : IVisitor
      {
         public MacroBase Clone { get; private set; }

         public void VisitLoop(Loop ForLoop)
         {
            WithClone(new Loop { Condition = ForLoop.Condition }, () => ForLoop.Body.Accept(this));
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

         public void VisitSymbol(Symbol Symbol)
         {
            WithClone(new Symbol(Symbol.Value));
         }

         public void VisitList(List List)
         {
            var listClone = new List();
            CloneListExpressions(List, listClone);
            WithClone(listClone);
         }

         public void VisitFunctionCall(FunctionCall FunctionCall)
         {
            var functionCallClone = new FunctionCall();
            CloneListExpressions(FunctionCall, functionCallClone);
            WithClone(functionCallClone);
         }

         private void CloneListExpressions(List List, Macro.List listClone)
         {
            foreach (var expression in List.Expressions)
               listClone.Expressions.Add(CloneExpression(expression));
         }

         public void VisitIf(If If)
         {
            WithClone(new If { Condition = CloneExpression(If.Condition) }, () => If.Consequent.Accept(this));
         }

         public void VisitDefinition(Definition VariableAssignment)
         {
            WithClone(new Definition { Symbol = VariableAssignment.Symbol, Expression = CloneExpression(VariableAssignment.Expression) });
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
