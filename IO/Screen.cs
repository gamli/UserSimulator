using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ScreenShotDemo;

namespace IO
{
   [ExcludeFromCodeCoverage]
   public static class Screen
   {
      public static Image Capture()
      {
         return new ScreenCapture().CaptureScreen();
      }

      public static int PrimaryWidth
      {
         get
         {
            return Screen.PrimaryWidth;
         }
      }

      public static int PrimaryHeight
      {
         get
         {
            return Screen.PrimaryHeight;
         }
      }
   }
}
