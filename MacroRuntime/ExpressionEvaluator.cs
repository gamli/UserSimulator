using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO;
using Macro;

namespace MacroRuntime
{
   public class ExpressionEvaluator
   {
      private readonly ExpressionBase _expression;
      private ExpressionVisitor _expressionVisitor;
      private ContextBase _context;

      public ExpressionEvaluator(ContextBase Context = null)
      {

         _expression = Expression;
         _expressionVisitor = new ExpressionVisitor(TargetWindow, Context);
      }

      public object Evaluate(ExpressionBase Expression)
      {
         _expression.Accept(_expressionVisitor);
         return _expressionVisitor.Value;
      }

      private class ExpressionVisitor : IVisitor
      {
         public object Value { get; private set; }

         private readonly IntPtr _targetWindow;
         private ContextBase _context;

         public ExpressionVisitor(IntPtr TargetWindow, ContextBase Context)
         {
            _targetWindow = TargetWindow;
            _context = Context;
         }

         public void VisitProgram(Program Program)
         {
            throw new Exception("Malformed program: Program-Statement is not an expression");
         }

         public void VisitBlock(Block Block)
         {
            throw new Exception("Malformed program: Block-Statement is not an expression");
         }

         public void VisitLoop(Loop ForLoop)
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

         public void VisitConstant(Constant Constant)
         {
            Value = Constant.Value;
         }

         public void VisitSymbol(Symbol Symbol)
         {
            Value = _context.GetValue(Symbol);
         }

         public void VisitList(List List)
         {
            foreach (var expression in List.Expressions.Take(List.Expressions.Count - 1))
               expression.Accept(this);
            Value = new ExpressionEvaluator(List.Expressions.Last(), _targetWindow, _contexts).Evaluate();
         }

         public void VisitFunctionCall(FunctionCall FunctionCall)
         {
            throw new NotImplementedException();
         }

         public void VisitIf(If IfStatement)
         {
            throw new Exception("Malformed program: If-Statement is not an expression");
         }

         public void VisitDefinition(Definition VariableAssignment)
         {
            throw new Exception("Malformed program: VariableAssignment-Statement is not an expression");
         }

         private TValue EvaluateExpression<TValue>(ExpressionBase Expression)
         {
            return (TValue)new ExpressionEvaluator(Expression, _targetWindow, _contexts).Evaluate();
         }
      }
   }
}
