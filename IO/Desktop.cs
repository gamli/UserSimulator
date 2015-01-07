using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IO
{
   [ExcludeFromCodeCoverage]
   public static class Desktop
   {
      [DllImport("user32.dll")]
      private static extern IntPtr WindowFromPoint(Point pnt);
      [StructLayout(LayoutKind.Sequential)]
      private struct Point
      {
         public int X { get; set; }
         public int Y { get; set; }
      }

      public static IntPtr WindowHandle(int X, int Y)
      {
         return WindowFromPoint(new Point { X = X, Y = Y });
      }
   }
}
