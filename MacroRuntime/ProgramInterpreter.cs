using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IO;
using Macro;

namespace UserSimulator
{
   public class ProgramInterpreter
   {
      private readonly Program _program;
      private readonly StatementVisitor _statementVisitor;

      public ProgramInterpreter(Program Program, IntPtr TargetWindow)
      {
         _program = Program;
         _statementVisitor = new StatementVisitor(TargetWindow);
      }

      public void Start()
      {
         _program.Accept(_statementVisitor);
      }

      private class StatementVisitor : IVisitor
      {
         private readonly IntPtr _targetWindow;
         private readonly Stack<Dictionary<string, object>> _contexts = new Stack<Dictionary<string, object>>();

         public StatementVisitor(IntPtr TargetWindow)
         {
            _targetWindow = TargetWindow;
         }

         public void VisitProgram(Program Program)
         {
            Program.Body.Accept(this);
         }

         public void VisitBlock(Block Block)
         {
            _contexts.Push(new Dictionary<string, object>());
            foreach (var item in Block.Items)
               item.Accept(this);
         }

         public void VisitNoOp(NoOp NoOp)
         {
            // nothing to do
         }

         public void VisitForLoop(ForLoop ForLoop)
         {
            for (var i = 0; i < EvaluateExpression<int>(ForLoop.RepetitionCount); i++)
               ForLoop.Body.Accept(this);
         }
         public void VisitWindowshot(Windowshot Windowshot)
         {
            throw new Exception("Malformed program: Windowshot expression is not a statement");
         }

         public void VisitMove(Move Move)
         {
            Mouse.X += EvaluateExpression<int>(Move.TranslationX);
            Mouse.Y += EvaluateExpression<int>(Move.TranslationY);
         }

         public void VisitPosition(Position Position)
         {
            int screenX, screenY;
            Window.ClientToScreen(
               _targetWindow, 
               EvaluateExpression<int>(Position.X), EvaluateExpression<int>(Position.Y), 
               out screenX, out screenY);
            Mouse.Position = new Mouse.MousePoint(screenX, screenY);
         }
         public void VisitLeftClick(LeftClick LeftClick)
         {
            Mouse.LeftClick();
         }

         public void VisitPause(Pause Pause)
         {
            Thread.Sleep(EvaluateExpression<int>(Pause.Duration));
         }

         public void VisitConstantExpression<T>(ConstantExpression<T> ConstantExpression)
         {
            throw new Exception("Malformed program: Constant expression is not a statement");
         }

         public void VisitIf(If If)
         {
            if (EvaluateExpression<bool>(If.Expression))
               If.Body.Accept(this);
         }
         
         public void VisitVariableAssignment<T>(VariableAssignment<T> VariableAssignment)
         {
            var context = _contexts.Peek();
            if(context != null)
               context[VariableAssignment.Symbol] = EvaluateExpression<T>(VariableAssignment.Expression);
         }

         private T EvaluateExpression<T>(ExpressionBase<T> Expression)
         {
            return new ExpressionEvaluator<T>(Expression, _targetWindow).Evaluate();
         }
      }
   }

   public class ExpressionEvaluator<TExpressionValue>
   {
      private readonly ExpressionBase<TExpressionValue> _expression;
      private ExpressionVisitor _expressionVisitor;

      public ExpressionEvaluator(ExpressionBase<TExpressionValue> Expression, IntPtr TargetWindow)
      {
         _expression = Expression;
         _expressionVisitor = new ExpressionVisitor(TargetWindow);
      }

      public TExpressionValue Evaluate()
      {
         _expression.Accept(_expressionVisitor);
         if (_expressionVisitor.Value is TExpressionValue)
            return (TExpressionValue)_expressionVisitor.Value;
         else
            throw new Exception(
               string.Format(
                  "Malformed program: Expected expression of type {0} and got expression of type {1}",
                  typeof(TExpressionValue),
                  _expressionVisitor.Value.GetType()));
      }

      private class ExpressionVisitor : IVisitor
      {
         public object Value { get; private set; }

         private readonly IntPtr _targetWindow;

         public ExpressionVisitor(IntPtr TargetWindow)
         {
            _targetWindow = TargetWindow;
         }

         public void VisitProgram(Program Program)
         {
            throw new Exception("Malformed program: Program-Statement is not an expression");
         }

         public void VisitBlock(Block Block)
         {
            throw new Exception("Malformed program: Block-Statement is not an expression");
         }

         public void VisitNoOp(NoOp NoOp)
         {
            throw new Exception("Malformed program: NoOp-Statement is not an expression");
         }

         public void VisitForLoop(ForLoop ForLoop)
         {
            throw new Exception("Malformed program: For-Statement is not an expression");
         }
         public void VisitWindowshot(Windowshot Windowshot)
         {
            using (var image = new Bitmap(EvaluateExpression<string>(Windowshot.ImageUrl)))
            using (var windowContent = Window.Capture(_targetWindow))
            using (var clippedWindowContent = new Bitmap(image.Width, image.Height))
            using (var clippedWindowContentGraphics = Graphics.FromImage(clippedWindowContent))
            {
               clippedWindowContentGraphics.DrawImage(
                  windowContent,
                  0, 0,
                  new Rectangle(
                     EvaluateExpression<int>(Windowshot.PositionX) - image.Width / 2, EvaluateExpression<int>(Windowshot.PositionY) - image.Width / 2,
                     image.Width, image.Height),
                  GraphicsUnit.Pixel
                  );

               for (var x = 0; x < image.Width; x++)
                  for (var y = 0; y < image.Height; y++)
                     if (image.GetPixel(x, y) != clippedWindowContent.GetPixel(x, y))
                     {
                        Value = false;
                        return;
                     }
            }
            Value = true;
         }

         public void VisitMove(Move Move)
         {
            throw new Exception("Malformed program: Move-Statement is not an expression");
         }

         public void VisitPosition(Position Position)
         {
            throw new Exception("Malformed program: Position-Statement is not an expression");
         }
         public void VisitLeftClick(LeftClick LeftClick)
         {
            throw new Exception("Malformed program: LeftClick-Statement is not an expression");
         }

         public void VisitPause(Pause Pause)
         {
            throw new Exception("Malformed program: Pause-Statement is not an expression");
         }

         public void VisitConstantExpression<T>(ConstantExpression<T> ConstantExpression)
         {
            Value = ConstantExpression.Value;
         }

         public void VisitIf(If IfStatement)
         {
            throw new Exception("Malformed program: If-Statement is not an expression");
         }

         public void VisitVariableAssignment<T>(VariableAssignment<T> VariableAssignment)
         {
            throw new Exception("Malformed program: VariableAssignment-Statement is not an expression");
         }

         private T EvaluateExpression<T>(ExpressionBase<T> Expression)
         {
            return new ExpressionEvaluator<T>(Expression, _targetWindow).Evaluate();
         }
      }
   }
}
