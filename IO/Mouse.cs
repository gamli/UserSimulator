using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace IO
{
   [ExcludeFromCodeCoverage]
   public static class Mouse
   {
      public static MousePoint Position
      {
         get
         {
            MousePoint currentMousePoint;
            if (!GetCursorPos(out currentMousePoint))
               currentMousePoint = new MousePoint(0, 0);
            return currentMousePoint;
         }
         set
         {
            SetCursorPos(value.X, value.Y);
         }
      }

      public static int X
      {
         get
         {
            return Position.X;
         }
         set
         {
            var position = Position;
            position.X = value;
            Position = position;
         }
      }

      public static int Y
      {
         get
         {
            return Position.Y;
         }
         set
         {
            var position = Position;
            position.Y = value;
            Position = position;
         }
      }

      public static void LeftClick(int X, int Y)
      {
         Position = new MousePoint(X, Y);
         LeftClick();
      }

      public static void LeftClick()
      {
         RaiseEvent(MouseEventFlags.LeftDown | MouseEventFlags.LeftUp);
      }

      public static void RaiseEvent(MouseEventFlags MouseEventFlags)
      {
         var position = Position;
         mouse_event(
            (int)MouseEventFlags,
            position.X,
            position.Y,
            0,
            0);
      }

      [DllImport("user32.dll")]
      private static extern bool SetCursorPos(int X, int Y);

      [DllImport("user32.dll")]
      private static extern bool GetCursorPos(out MousePoint lpMousePoint);

      [DllImport("user32.dll")]
      private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

      [StructLayout(LayoutKind.Sequential)]
      public struct MousePoint
      {
         public int X;
         public int Y;

         public MousePoint(int x, int y)
         {
            X = x;
            Y = y;
         }
      }

      [Flags]
      public enum MouseEventFlags
      {
         LeftDown = 0x00000002,
         LeftUp = 0x00000004,
         MiddleDown = 0x00000020,
         MiddleUp = 0x00000040,
         Move = 0x00000001,
         Absolute = 0x00008000,
         RightDown = 0x00000008,
         RightUp = 0x00000010
      }
   }
}
