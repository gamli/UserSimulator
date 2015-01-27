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

         WindowshotSnippetFormat = "(= {0} (windowshot {1} {2} {3} {4}))";
      }

      private readonly NotifyingTransformedProperty<ImageSource> _lastWindowshotVM;
      private int _codeEditorCursorPosition;
      private string _windowshotSnippetFormat;

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

      public string WindowshotSnippetFormat { get { return _windowshotSnippetFormat; } set { SetPropertyValue(ref _windowshotSnippetFormat, value); } }

      public void InsertWindowshotSnippetIntoCodeEditor(Rectangle WindowshotRect)
      {
         // empty selection
         if(WindowshotRect.Width <= 0 || WindowshotRect.Height <= 0)
            return;

         using (var lastWindowhotPart = new Bitmap(WindowshotRect.Width, WindowshotRect.Height))
         using (var lastWindowshotPartGraphics = Graphics.FromImage(lastWindowhotPart))
         {
            lastWindowshotPartGraphics.DrawImage(
               Model.LastWindowshot,
               new Rectangle(0, 0, lastWindowhotPart.Width, lastWindowhotPart.Height),
               WindowshotRect,
               GraphicsUnit.Pixel);


            var imageExpression = "\"" + Imaging.Image2PngHexString(lastWindowhotPart) + "\"";
            var windowshotSnippet = 
               string.Format(
                  WindowshotSnippetFormat, 
                  imageExpression, 
                  WindowshotRect.Left, WindowshotRect.Top, WindowshotRect.Width, WindowshotRect.Height);

            var codeEditorCursorPosition = CodeEditorCursorPosition;
            Model.ExpressionText = Model.ExpressionText.Insert(codeEditorCursorPosition, windowshotSnippet);
            CodeEditorCursorPosition = codeEditorCursorPosition + windowshotSnippet.Length;
         }
      }
   }
}
