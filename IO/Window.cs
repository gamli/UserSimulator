using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ScreenShotDemo;

namespace IO
{
   [ExcludeFromCodeCoverage]
   public static class Window
   {
      public static Image Capture(IntPtr WindowHandle)
      {
         return new ScreenCapture().CaptureWindow(WindowHandle);
      }


      [DllImport("user32.dll")]
      static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
      public static string Text(IntPtr WindowHandle)
      {
         const int MAX_LENGTH = 4096;
         var textBuilder = new StringBuilder(MAX_LENGTH);
         if (GetWindowText(WindowHandle, textBuilder, MAX_LENGTH) > 0)
            return textBuilder.ToString();
         return "UNKNOWN";
      }

      [DllImport("user32.dll")]
      private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);
      public static void ScreenToClient(IntPtr WindowHandle, int ScreenX, int ScreenY, out int ClientX, out int ClientY)
      {
         var point = new Point { X = ScreenX, Y = ScreenY };
         ScreenToClient(WindowHandle, ref point);
         ClientX = point.X;
         ClientY = point.Y;
      }
      [DllImport("user32.dll")]
      private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
      public static void ClientToScreen(IntPtr WindowHandle, int ClientX, int ClientY, out int ScreenX, out int ScreenY)
      {
         var point = new Point { X = ClientX, Y = ClientY };
         ClientToScreen(WindowHandle, ref point);
         ScreenX = point.X;
         ScreenY = point.Y;
      }
      [StructLayout(LayoutKind.Sequential)]
      private struct Point
      {
         public int X { get; set; }
         public int Y { get; set; }
      }
   }
}
