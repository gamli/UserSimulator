using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ScreenShotDemo;

namespace IO
{
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
   }
}
