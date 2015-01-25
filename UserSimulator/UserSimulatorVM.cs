using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;

namespace UserSimulator
{
   class UserSimulatorVM : ViewModelBase<UserSimulatorModel>
   {
      public UserSimulatorVM(UserSimulatorModel Model)
         : base(Model)
      {
         _lastWindowshotVM =
            new NotifyingTransformedProperty<ImageSource>(
               new[] { "LastWindowshot" }, "LastWindowshotVM",
               Model, this,
               () => Application.Current.Dispatcher.Invoke(() => DrawingImage2WpfImageSource(Model.LastWindowshot)));
      }

      private readonly NotifyingTransformedProperty<ImageSource> _lastWindowshotVM;
      private int _codeEditorCursorPosition;

      public ImageSource LastWindowshotVM { get { return _lastWindowshotVM.Value; } }
      private static ImageSource DrawingImage2WpfImageSource(Image Image)
      {
         // TODO dispose???
         var stream = new MemoryStream();
         Image.Save(stream, ImageFormat.Bmp);
         stream.Position = 0;
         var imageSource = new BitmapImage();
         imageSource.BeginInit();
         imageSource.StreamSource = stream;
         imageSource.CacheOption = BitmapCacheOption.OnLoad;
         imageSource.EndInit();
         stream.Position = 0;
         return imageSource;
      }

      public int CodeEditorCursorPosition { get { return _codeEditorCursorPosition; } set { SetPropertyValue(ref _codeEditorCursorPosition, value); } }

      public void InsertWindowshotIntoCodeEditor(Rectangle WindowshotRect) // TODO width/height zero - what to do?
      {
         using (var lastWindowhotPart = new Bitmap(WindowshotRect.Width, WindowshotRect.Height))
         using (var lastWindowshotPartGraphics = Graphics.FromImage(lastWindowhotPart))
         {
            lastWindowshotPartGraphics.DrawImage(
               Model.LastWindowshot,
               new Rectangle(0, 0, lastWindowhotPart.Width, lastWindowhotPart.Height),
               WindowshotRect,
               GraphicsUnit.Pixel);


            var imageExpression = "\"" + Imaging.Image2PngHexString(lastWindowhotPart) + "\"";
            var windowshotExpression =
               string.Format(
                  "(windowshot {0} {1} {2} {3})",
                  WindowshotRect.Left, WindowshotRect.Top, WindowshotRect.Width, WindowshotRect.Height);
            var equalsExpression = "(= " + imageExpression + " " + windowshotExpression + ")";

            var codeEditorCursorPosition = CodeEditorCursorPosition;
            Model.ExpressionText = Model.ExpressionText.Insert(codeEditorCursorPosition, equalsExpression);
            CodeEditorCursorPosition = codeEditorCursorPosition + equalsExpression.Length;
         }
      }
   }
}
