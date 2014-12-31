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
      public WindowshotView()
      {
         InitializeComponent();
         var timer =
            new DispatcherTimer
               {
                  Interval = TimeSpan.FromMilliseconds(1000)
               };
         timer.Tick +=
            (Sender, Args) =>
            {
               if (DataContext is WindowshotVM)
               {
                  var windowshotMacro = ((WindowshotVM)DataContext).Model;
                  var windowshotImageSource = ((BitmapImage)_windowshot.Source);
                  if (windowshotImageSource != null)
                  {
                     if (windowshotMacro.PositionX is ConstantExpression<int>)
                        _muh.X =
                           ((ConstantExpression<int>)windowshotMacro.PositionX).Value * (_windowshot.ActualWidth / windowshotImageSource.PixelWidth)
                           - _windowshot.ActualWidth / 2 + _kuh.BorderThickness.Left;
                     else
                        _muh.X = 0;
                     if (windowshotMacro.PositionY is ConstantExpression<int>)
                        _muh.Y =
                           ((ConstantExpression<int>)windowshotMacro.PositionY).Value * (_windowshot.ActualHeight / windowshotImageSource.PixelHeight)
                           - _windowshot.ActualHeight / 2 + _kuh.BorderThickness.Top;
                     else
                        _muh.Y = 0;
                     var windowshotMacroWidth = 16;
                     var windowshotMacroHeight = 16;
                     _kuh.Width = windowshotMacroWidth * (_windowshot.ActualWidth / _windowshotImage.Width) + _kuh.BorderThickness.Left + _kuh.BorderThickness.Right;
                     _kuh.Height = windowshotMacroHeight * (_windowshot.ActualHeight / _windowshotImage.Height) + _kuh.BorderThickness.Top + _kuh.BorderThickness.Bottom;
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
            };
         timer.Start();
      }

      private void _windowshot_MouseUp(object sender, MouseButtonEventArgs e)
      {
         var mousePosition = e.GetPosition(_windowshot);
         _muh.X = mousePosition.X - _windowshot.ActualWidth / 2;
         _muh.Y = mousePosition.Y - _windowshot.ActualHeight / 2;
         var windowshotImageSource = ((BitmapImage)_windowshot.Source);
         var mousePositionPixelX = (int)(mousePosition.X * windowshotImageSource.PixelWidth / _windowshot.ActualWidth);
         var mousePositionPixelY = (int)(mousePosition.Y * windowshotImageSource.PixelHeight / _windowshot.ActualHeight);
         var windowshotMacroWidth = 16;
         var windowshotMacroHeight = 16;
         var windowshotMacroPositionX = Math.Max(0, mousePositionPixelX - windowshotMacroWidth / 2);
         var windowshotMacroPositionY = Math.Max(0, mousePositionPixelY - windowshotMacroHeight / 2);
         _kuh.Width = windowshotMacroWidth * (_windowshot.ActualWidth / _windowshotImage.Width) + _kuh.BorderThickness.Left + _kuh.BorderThickness.Right;
         _kuh.Height = windowshotMacroHeight * (_windowshot.ActualHeight / _windowshotImage.Height) + _kuh.BorderThickness.Top + _kuh.BorderThickness.Bottom;

         var windowshotMacro = ((WindowshotVM)DataContext).Model;
         var windowshotMacroImageUrl = System.IO.Path.GetTempFileName();
         using (var windowshotMacroImage = new System.Drawing.Bitmap(windowshotMacroWidth, windowshotMacroHeight))
         using (var graphics = Graphics.FromImage(windowshotMacroImage))
         {
            graphics.DrawImage(
               _windowshotImage,
               0, 0,
               new System.Drawing.Rectangle(windowshotMacroPositionX, windowshotMacroPositionY, windowshotMacroWidth, windowshotMacroHeight),
               GraphicsUnit.Pixel
               );
            using (var stream = File.OpenWrite(windowshotMacroImageUrl))
               windowshotMacroImage.Save(stream, ImageFormat.Bmp);
            windowshotMacro.ImageUrl = ConstantExpressions.Create(windowshotMacroImageUrl);
         }
         windowshotMacro.PositionX = ConstantExpressions.Create(windowshotMacroPositionX);
         windowshotMacro.PositionY = ConstantExpressions.Create(windowshotMacroPositionY);
      }

      private System.Drawing.Image _windowshotImage;

      private void _windowshot_LayoutUpdated(object sender, EventArgs e)
      {

      }
   }
}
