using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IO;
using Macro;

namespace MacroRuntime
{
   public class RuntimeContext : ContextBase
   {
      private static readonly Symbol NIL_SYMBOL = new Symbol("nil");

      private IntPtr _targetWindow;

      public RuntimeContext(IntPtr TargetWindow)
      {
         SetValue(NIL_SYMBOL, new List());
         _targetWindow = TargetWindow;
      }

      protected override object SymbolNotFoundGetValue(Symbol Symbol)
      {
         throw new Exception("Symbol >>" + Symbol.Value + "<< is not defined");
      }

      private object MouseMove(object TranslationX, object TranslationY)
      {
         return
            Try(
               () =>
                  {
                     Mouse.X += (int)Convert.ChangeType(TranslationX, typeof(int));
                     Mouse.Y += (int)Convert.ChangeType(TranslationY, typeof(int));
                  },
               string.Format("Could not move the mouse by ({0}, {1})", TranslationX, TranslationY));
      }

      private object MousePosition(object X, object Y)
      {
         return
            Try(
               () =>
                  {
                     int screenX, screenY;
                     Window.ClientToScreen(
                        _targetWindow,
                        (int)X, (int)Y,
                        out screenX, out screenY);
                     Mouse.Position = new Mouse.MousePoint(screenX, screenY);
                  },
               string.Format("Could not position the mouse to ({0}, {1})", X, Y));
      }

      private object Pause(object Milliseconds)
      {
         return Try(() => Thread.Sleep((int)Milliseconds), string.Format("Could not sleep for {0} milliseconds", Milliseconds));
      }
      private object VisitLeftClick()
      {
         Mouse.LeftClick();
      }

      private object Windowshot(object X, object Y, object ImageUrl)
      {
         Thread.Sleep((int)Milliseconds);
         return GetValue(NIL_SYMBOL);
      }

      private object Try(Action Action, string ExceptionMessage, object ReturnValue = null)
      {
         if (ReturnValue == null)
            ReturnValue = GetValue(NIL_SYMBOL);
         return Try(() => { Action(); return ReturnValue }, ExceptionMessage);
      }

      private object Try(Func<object> Function, string ExceptionMessage)
      {         
         try
         {
            return Function();
         }
         catch (Exception E)
         {
            var descriptionSeparator = new string('=', ExceptionMessage.Length);
            var messageSeparator = new string('-', E.Message.Length);
            Logger.Instance.Log(string.Format("{0}\n\n{1}\n{2}", ExceptionMessage, descriptionSeparator, E.Message, messageSeparator, E.StackTrace));
            throw;
         }
      }
   }
}
