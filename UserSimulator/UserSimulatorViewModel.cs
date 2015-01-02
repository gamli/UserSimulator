using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using MacroViewModel;

namespace UserSimulator
{
   class UserSimulatorVM : ViewModelBase<UserSimulatorModel>
   {
      private NotifyingTransformedProperty<ImageSource> _lastWindowshotVM;
      public ImageSource LastWindowshotVM { get { return _lastWindowshotVM.Value; } }

      private NotifyingTransformedProperty<ProgramVM> _programVM;
      public ProgramVM ProgramVM { get { return _programVM.Value; } }

      public UserSimulatorVM(UserSimulatorModel Model)
         : base(Model)
      {
         _lastWindowshotVM =
            new NotifyingTransformedProperty<ImageSource>(
               new[] { "LastWindowshot" }, "LastWindowshotVM",
               Model, this,
               () => System.Windows.Application.Current.Dispatcher.Invoke(() => DrawingImage2WpfImageSource(Model.LastWindowshot)));

         _programVM =
            new NotifyingTransformedProperty<ProgramVM>(
               new[] { "Program" }, "ProgramVM",
               Model, this,
               () => new ProgramVM(Model.Program),
               ViewModel => ViewModel.Dispose());
      }

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
   }
}
