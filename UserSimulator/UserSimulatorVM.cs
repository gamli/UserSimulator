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

            using (var lastWindowshotPartStream = new MemoryStream())
            {
               lastWindowhotPart.Save(lastWindowshotPartStream, ImageFormat.Png);
               var binaryValue = lastWindowshotPartStream.ToArray();
               var hexValue = BitConverter.ToString(binaryValue).Replace("-", string.Empty).ToUpper();

               var codeEditorCursorPosition = CodeEditorCursorPosition;
               Model.ExpressionText = Model.ExpressionText.Insert(codeEditorCursorPosition, "\"" + hexValue + "\"");
               CodeEditorCursorPosition = codeEditorCursorPosition + hexValue.Length + 2;
            }
         }
      }
   }
}
