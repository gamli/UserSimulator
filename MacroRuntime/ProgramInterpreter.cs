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
   public class ProgramInterpreter : IVisitor
   {
      private readonly Program _program;
      private readonly IntPtr _targetWindow;

      public ProgramInterpreter(Program Program, IntPtr TargetWindow)
      {
         _program = Program;
         _targetWindow = TargetWindow;
      }

      public void Start()
      {
         _program.Accept(this);
      }

      public void VisitProgram(Program Program)
      {
         Program.Body.Accept(this);
      }

      public void VisitBlock(Block Block)
      {
         foreach (var item in Block.Items)
            item.Accept(this);
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
      public void VisitWindowshot(Windowshot Windowshot)
      {
         using (var image = new Bitmap(Windowshot.ImageUrl))
         using (var windowContent = Window.Capture(_targetWindow))
         using (var clippedWindowContent = new Bitmap(image.Width, image.Height))
         using (var clippedWindowContentGraphics = Graphics.FromImage(clippedWindowContent))
         {
            clippedWindowContentGraphics.DrawImage(
               windowContent,
               0, 0,
               new Rectangle(Windowshot.PositionX, Windowshot.PositionY, image.Width, image.Height),
               GraphicsUnit.Pixel
               );

            for (var x = 0; x < image.Width; x++)
               for (var y = 0; y < image.Height; y++)
                  if (image.GetPixel(x, y) != clippedWindowContent.GetPixel(x, y))
                     return;
         }
         Windowshot.Body.Accept(this);
      }

      public void VisitMove(Move Move)
      {
         Mouse.X += Move.TranslationX;
         Mouse.Y += Move.TranslationY;
      }

      public void VisitPosition(Position Position)
      {
         int screenX, screenY;
         Window.ClientToScreen(_targetWindow, Position.X, Position.Y, out screenX, out screenY);
         Mouse.Position = new Mouse.MousePoint(screenX, screenY);
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
