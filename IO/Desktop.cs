using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace IO
{
   [ExcludeFromCodeCoverage]
   public static class Desktop
   {
      [DllImport("user32.dll")]
      private static extern IntPtr WindowFromPoint(Point Point);
      [StructLayout(LayoutKind.Sequential)]
      private struct Point
      {
         public int X, Y;
      }

      public static IntPtr WindowHandle(int X, int Y)
      {
         return WindowFromPoint(new Point { X = X, Y = Y });
      }
   }
}
