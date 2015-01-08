using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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
            return (int) SystemParameters.PrimaryScreenWidth;
         }
      }

      public static int PrimaryHeight
      {
         get
         {
            return (int)SystemParameters.PrimaryScreenHeight;
         }
      }
   }
}
