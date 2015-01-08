using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
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
      static extern int GetWindowText(IntPtr HWnd, StringBuilder Text, int Count);
      public static string Text(IntPtr WindowHandle)
      {
         const int MAX_LENGTH = 4096;
         var textBuilder = new StringBuilder(MAX_LENGTH);
         if (GetWindowText(WindowHandle, textBuilder, MAX_LENGTH) > 0)
            return textBuilder.ToString();
         return "UNKNOWN";
      }

      [DllImport("user32.dll")]
      private static extern bool ScreenToClient(IntPtr HWnd, ref Point LpPoint);
      public static void ScreenToClient(IntPtr WindowHandle, int ScreenX, int ScreenY, out int ClientX, out int ClientY)
      {
         var point = new Point { X = ScreenX, Y = ScreenY };
         ScreenToClient(WindowHandle, ref point);
         ClientX = point.X;
         ClientY = point.Y;
      }
      [DllImport("user32.dll")]
      private static extern bool ClientToScreen(IntPtr HWnd, ref Point LpPoint);
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
