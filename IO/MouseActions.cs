using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IO
{
   static class MouseActions
   {
      public static Action Move(int X, int Y)
      {
         return () => Mouse.Position = new Mouse.MousePoint(X, Y);
      }

      public static Action LeftClick()
      {
         return () => Mouse.LeftClick();
      }

      public static Action Pause(int MilliSeconds = 1000)
      {
         return () => Thread.Sleep(MilliSeconds);
      }

      public static Action Batch(params Action[] MouseActions)
      {
         return InternalBatch(MouseActions);
      }

      public static Action Batch(IEnumerable<Action> MouseActions)
      {
         return InternalBatch(MouseActions);
      }

      private static Action InternalBatch(IEnumerable<Action> MouseActions)
      {
         return
            () =>
            {
               if (MouseActions == null)
                  return;
               foreach (var mouseAction in MouseActions)
                  mouseAction();
            };
      }
   }
}
