using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using IO;
using Macro;
using MacroViewModel;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for WindowshotView.xaml
   /// </summary>
   public partial class WindowshotView : UserControl
   {
      const int WINDOWSHOT_MACRO_WIDTH = 16;
      const int WINDOWSHOT_MACRO_HEIGHT = 16;

      public WindowshotView()
      {
         InitializeComponent();
         var timer =
            new DispatcherTimer
               {
                  Interval = TimeSpan.FromMilliseconds(100)
               };
         timer.Tick +=
            (Sender, Args) =>
            {
               if (DataContext is WindowshotVM)
               {
                  var windowshotMacro = (Windowshot)((WindowshotVM)DataContext).Model;
                  var windowshotImageSource = ((BitmapImage)_windowshot.Source);
                  if (windowshotImageSource != null)
                  {
                     if (windowshotMacro.PositionX is Constant)
                        _muh.X =
                           (int)((Constant)windowshotMacro.PositionX).Value
                           * (_windowshot.ActualWidth / windowshotImageSource.PixelWidth)
                           - _windowshot.ActualWidth / 2;
                     else
                        _muh.X = 0;

                     if (windowshotMacro.PositionY is Constant)
                        _muh.Y =
                           (int)((Constant)windowshotMacro.PositionY).Value
                           * (_windowshot.ActualHeight / windowshotImageSource.PixelHeight)
                           - _windowshot.ActualHeight / 2;
                     else
                        _muh.Y = 0;
                  }
               }
               if (!IO.Keyboard.IsControlKeyDown())
                  return;
               var mousePosition = IO.Mouse.Position;
               var windowHandle = Desktop.WindowHandle(mousePosition.X, mousePosition.Y);
               if (_windowshotImage != null)
                  _windowshotImage.Dispose();
               _windowshotImage = IO.Window.Capture(windowHandle);
               using (var stream = new MemoryStream())
               {
                  _windowshotImage.Save(stream, ImageFormat.Bmp);
                  stream.Position = 0;
                  var imageSource = new BitmapImage();
                  imageSource.BeginInit();
                  imageSource.StreamSource = stream;
                  imageSource.CacheOption = BitmapCacheOption.OnLoad;
                  imageSource.EndInit();
                  stream.Position = 0;
                  _windowshot.Source = imageSource;
               }
               GC.Collect();
            };
         timer.Start();
      }

      private void WindowshotMouseUp(object sender, MouseButtonEventArgs e)
      {
         var mousePosition = e.GetPosition(_windowshot);
         var windowshotImageSource = ((BitmapImage)_windowshot.Source);
         var mousePositionPixelX = (int)((mousePosition.X * windowshotImageSource.PixelWidth) / _windowshot.ActualWidth);
         var mousePositionPixelY = (int)((mousePosition.Y * windowshotImageSource.PixelHeight) / _windowshot.ActualHeight);

         var windowshotMacro = (Windowshot)((WindowshotVM)DataContext).Model;
         var windowshotMacroImageUrl = System.IO.Path.GetTempFileName();
         using (var windowshotMacroImage = new System.Drawing.Bitmap(WINDOWSHOT_MACRO_WIDTH, WINDOWSHOT_MACRO_HEIGHT))
         using (var graphics = Graphics.FromImage(windowshotMacroImage))
         {
            graphics.DrawImage(
               _windowshotImage,
               0, 0,
               new System.Drawing.Rectangle(
                  mousePositionPixelX - WINDOWSHOT_MACRO_WIDTH / 2, 
                  mousePositionPixelY - WINDOWSHOT_MACRO_HEIGHT / 2, 
                  WINDOWSHOT_MACRO_WIDTH, 
                  WINDOWSHOT_MACRO_HEIGHT),
               GraphicsUnit.Pixel
               );
            using (var stream = File.OpenWrite(windowshotMacroImageUrl))
               windowshotMacroImage.Save(stream, ImageFormat.Bmp);
            windowshotMacro.ImageUrl = new Constant(windowshotMacroImageUrl);
         }
         windowshotMacro.PositionX = new Constant(mousePositionPixelX);
         windowshotMacro.PositionY = new Constant(mousePositionPixelY);
         GC.Collect();
      }

      private System.Drawing.Image _windowshotImage;
   }
}
