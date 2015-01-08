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
            SetCursorPos(value.MouseX, value.MouseY);
         }
      }

      public static int X
      {
         get
         {
            return Position.MouseX;
         }
         set
         {
            var position = Position;
            position.MouseX = value;
            Position = position;
         }
      }

      public static int Y
      {
         get
         {
            return Position.MouseY;
         }
         set
         {
            var position = Position;
            position.MouseY = value;
            Position = position;
         }
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
            position.MouseX,
            position.MouseY,
            0,
            0);
      }

      [DllImport("user32.dll")]
      private static extern bool SetCursorPos(int X, int Y);

      [DllImport("user32.dll")]
      private static extern bool GetCursorPos(out MousePoint LpMousePoint);

      [DllImport("user32.dll")]
      private static extern void mouse_event(int DwFlags, int Dx, int Dy, int DwData, int DwExtraInfo);

      [StructLayout(LayoutKind.Sequential)]
      public struct MousePoint
      {
         public int MouseX;
         public int MouseY;

         public MousePoint(int MouseX, int MouseY)
         {
            this.MouseX = MouseX;
            this.MouseY = MouseY;
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
