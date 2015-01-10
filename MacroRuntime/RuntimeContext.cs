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

         AddIntrinsicFunction("move", MouseMove, _mouseMoveDeltaX, _mouseMoveDeltaY);
         AddIntrinsicFunction("position", MousePosition, _mousePositionX, _mousePositionY);
         AddIntrinsicFunction("pause", Pause, _pauseDuration);
         AddIntrinsicFunction("click", LeftClick);
         AddIntrinsicFunction("windowshot", Windowshot, _windowshotX, _windowshotY, _windowshotImageUrl);
      }

      private void AddIntrinsicFunction(string Symbol, Func<ContextBase, ExpressionBase> Function, params Symbol[] Arguments)
      {
         var argumentSymbols = new SymbolList();
         foreach (var argument in Arguments)
            argumentSymbols.Symbols.Add(argument);
         DefineValue(new Symbol(Symbol), new IntrinsicProcedure { DefiningContext = this, ArgumentSymbols = argumentSymbols, Function = Function});
      }

      [ExcludeFromCodeCoverage]
      protected override void SymbolNotFoundSetValue(Symbol Symbol, ExpressionBase Value)
      {
         var exceptionMessage = "SetValue: Symbol >>" + Symbol.Value + "<< is not defined (did you forget to 'define' first?)";
         throw new RuntimeException(exceptionMessage, Symbol, this);
      }

      [ExcludeFromCodeCoverage]
      protected override ExpressionBase SymbolNotFoundGetValue(Symbol Symbol)
      {
         var exceptionMessage = "GetValue: Symbol >>" + Symbol.Value + "<< is not defined (did you forget to 'define' first?)";
         throw new RuntimeException(exceptionMessage, Symbol, this);
      }

      private readonly Symbol _mouseMoveDeltaX = new Symbol("DeltaX");
      private readonly Symbol _mouseMoveDeltaY = new Symbol("DeltaY");
      private Constant MouseMove(ContextBase Context)
      {
         Mouse.X += GetConstantValue<int>(Context, _mouseMoveDeltaX);
         Mouse.Y += GetConstantValue<int>(Context, _mouseMoveDeltaY);

         return new Constant(true);
      }


      private readonly Symbol _mousePositionX = new Symbol("X");
      private readonly Symbol _mousePositionY = new Symbol("Y");
      private Constant MousePosition(ContextBase Context)
      {
         int screenX, screenY;
         Window.ClientToScreen(
            _targetWindow,
            GetConstantValue<int>(Context, _mousePositionX), GetConstantValue<int>(Context, _mousePositionY),
            out screenX, out screenY);
         Mouse.Position = new Mouse.MousePoint(screenX, screenY);

         return new Constant(true);
      }


      private readonly Symbol _pauseDuration = new Symbol("Duration");
      private Constant Pause(ContextBase Context)
      {
         var milliseconds = GetConstantValue<int>(Context, _pauseDuration);
         Thread.Sleep(milliseconds);

         return new Constant(milliseconds);
      }

      [ExcludeFromCodeCoverage]
      private Constant LeftClick(ContextBase Context)
      {
         Mouse.LeftClick();

         return new Constant(true);
      }


      private readonly Symbol _windowshotX = new Symbol("X");
      private readonly Symbol _windowshotY = new Symbol("Y");
      private readonly Symbol _windowshotImageUrl = new Symbol("ImageUrl");
      [ExcludeFromCodeCoverage]
      private Constant Windowshot(ContextBase Context)
      {
         using (var image = new Bitmap(GetConstantValue<string>(Context, _windowshotImageUrl)))
         using (var windowContent = Window.Capture(_targetWindow))
         using (var clippedWindowContent = new Bitmap(image.Width, image.Height))
         using (var clippedWindowContentGraphics = Graphics.FromImage(clippedWindowContent))
         {
            var windowshotX = GetConstantValue<int>(Context, _windowshotX);
            var windowshotY = GetConstantValue<int>(Context, _windowshotY);

            clippedWindowContentGraphics.DrawImage(
               windowContent,
               0, 0,
               new Rectangle(
                  windowshotX - image.Width / 2, windowshotY - image.Width / 2,
                  image.Width, image.Height),
               GraphicsUnit.Pixel
               );

            for (var x = 0; x < image.Width; x++)
               for (var y = 0; y < image.Height; y++)
                  if (image.GetPixel(x, y) != clippedWindowContent.GetPixel(x, y))
                  {
                     return new Constant(false);
                  }
         }
         return new Constant(true);
      }

      private T GetConstantValue<T>(ContextBase Context, Symbol Symbol)
      {
         try
         {
            var constantValue = ((Constant)Context.GetValue(Symbol)).Value;
            try
            {
               return (T) constantValue;
            }
            catch (InvalidCastException e)
            {
               throw new RuntimeException(
                  string.Join("Symbol {0}: expected type was {1} but got {2}", Symbol, typeof(T), constantValue.GetType()), 
                  Symbol, 
                  Context, 
                  e);
            }
         }
         catch (InvalidCastException e)
         {
            throw new RuntimeException(string.Join("Symbol {0}: expected constant", Symbol), Symbol, Context, e);
         }
      }
   }
}
