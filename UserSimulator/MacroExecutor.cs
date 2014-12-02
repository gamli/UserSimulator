using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IO;
using Macro;

namespace UserSimulator
{
   class MacroExecutor : IVisitor
   {
      private readonly Program _program;
      private readonly ScreenshotModel _screenshotModel;

      public MacroExecutor(Program Program, ScreenshotModel ScreenshotModel)
      {
         _program = Program;
         _screenshotModel = ScreenshotModel;
      }

      public void Execute()
      {
         _program.Accept(this);
      }

      public void BeginVisitProgram(Program Program)
      {
         // nothing to do
      }

      public void EndVisitProgram(Program Program)
      {
         // nothing to do
      }

      public void VisitNoOp(NoOp NoOp)
      {
         // nothing to do
      }

      public void VisitForLoop(ForLoop ForLoop)
      {
         for (var i = 0; i < ForLoop.RepetitionCount; i++)
            ForLoop.Body.Accept(this);
      }
      public void VisitImageEqualsWindowContent(ImageEqualsWindowContent ImageEqualsWindowContentConditional)
      {
         using (var image = new Bitmap(ImageEqualsWindowContentConditional.Image))
         using (var windowContent = Window.Capture(_screenshotModel.LastScreenshotWindow))
         using (var clippedWindowContent = new Bitmap(image.Width, image.Height))
         using (var clippedWindowContentGraphics = Graphics.FromImage(clippedWindowContent))
         {
            clippedWindowContentGraphics.DrawImage(
               windowContent,
               0, 0,
               new Rectangle(ImageEqualsWindowContentConditional.PositionX, ImageEqualsWindowContentConditional.PositionY, image.Width, image.Height),
               GraphicsUnit.Pixel
               );
            for (var x = 0; x < image.Width; x++)
               for (var y = 0; y < image.Height; y++)
                  if (image.GetPixel(x, y) != clippedWindowContent.GetPixel(x, y))
                     return;
         }
         ImageEqualsWindowContentConditional.Body.Accept(this);
      }

      public void VisitMove(Move Move)
      {
         Mouse.X += Move.TranslationX;
         Mouse.Y += Move.TranslationY;
      }

      public void VisitPosition(Position Position)
      {
         Mouse.Position = new Mouse.MousePoint(Position.X, Position.Y);
      }
      public void VisitLeftClick(LeftClick LeftClick)
      {
         Mouse.LeftClick();
      }

      public void VisitPause(Pause Pause)
      {
         Thread.Sleep(Pause.Duration);
      }
   }
}
