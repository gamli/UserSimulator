using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading;
using IO;
using Macro;

namespace MacroRuntime
{
   public class RuntimeContext : ContextBase
   {
      private readonly IntPtr _targetWindow;

      public RuntimeContext(IntPtr TargetWindow)
      {
         _targetWindow = TargetWindow;

         AddIntrinsicFunction("move", MouseMove);
         AddIntrinsicFunction("position", MousePosition);
         AddIntrinsicFunction("pause", Pause);
         AddIntrinsicFunction("click", LeftClick);
         AddIntrinsicFunction("windowshot", Windowshot);
      }

      private void AddIntrinsicFunction(string Symbol, Func<List<object>, object> Function)
      {
         DefineValue(new Symbol(Symbol), Function);
      }

      [ExcludeFromCodeCoverage]
      protected override object SymbolNotFoundGetValue(Symbol Symbol)
      {
         var exceptionMessage = "Symbol >>" + Symbol.Value + "<< is not defined (did you forget to 'define' first?)";
         throw new RuntimeException(exceptionMessage, Symbol, this);
      }

      private object MouseMove(List<object> Args)
      {
         CheckArgsCount(Args, 2, "move function expected {0} argument(s) but got {1}");

         Mouse.X += (int)Convert.ChangeType(Args[0], typeof(int));
         Mouse.Y += (int)Convert.ChangeType(Args[1], typeof(int));

         return true;
      }

      private object MousePosition(List<object> Args)
      {
         CheckArgsCount(Args, 2, "position function expected {0} argument(s) but got {1}");

         int screenX, screenY;
         Window.ClientToScreen(
            _targetWindow,
            (int)Args[0], (int)Args[1],
            out screenX, out screenY);
         Mouse.Position = new Mouse.MousePoint(screenX, screenY);

         return true;
      }

      private object Pause(List<object> Args)
      {
         CheckArgsCount(Args, 1, "pause function expected {0} argument(s) but got {1}");

         var milliseconds = (int)Args[0];
         Thread.Sleep(milliseconds);

         return milliseconds;
      }

      [ExcludeFromCodeCoverage]
      private object LeftClick(List<object> Args)
      {
         CheckArgsCount(Args, 0, "click function expected {0} argument(s) but got {1}");

         Mouse.LeftClick();

         return true;
      }
        
      [ExcludeFromCodeCoverage]
      private object Windowshot(List<object> Args)
      {
         CheckArgsCount(Args, 3, "windowshot function expected {0} argument(s) but got {1}");

         using (var image = new Bitmap((string)Args[2]))
         using (var windowContent = Window.Capture(_targetWindow))
         using (var clippedWindowContent = new Bitmap(image.Width, image.Height))
         using (var clippedWindowContentGraphics = Graphics.FromImage(clippedWindowContent))
         {
            clippedWindowContentGraphics.DrawImage(
               windowContent,
               0, 0,
               new Rectangle(
                  (int)Args[0] - image.Width / 2, (int)Args[1] - image.Width / 2,
                  image.Width, image.Height),
               GraphicsUnit.Pixel
               );

            for (var x = 0; x < image.Width; x++)
               for (var y = 0; y < image.Height; y++)
                  if (image.GetPixel(x, y) != clippedWindowContent.GetPixel(x, y))
                  {
                     return false;
                  }
         }
         return true;
      }

      private void CheckArgsCount(List<object> Args, int ExpectedArgsCount, string ErrorMessageFormat)
      {
         if (Args.Count != ExpectedArgsCount)
            throw new RuntimeException(string.Format(ErrorMessageFormat, ExpectedArgsCount, Args.Count), null, this);
      }
   }
}
