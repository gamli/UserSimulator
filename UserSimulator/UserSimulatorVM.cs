﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using MacroViewModel;

namespace UserSimulator
{
   class UserSimulatorVM : ViewModelBase<UserSimulatorModel>
   {
      private readonly NotifyingTransformedProperty<ImageSource> _lastWindowshotVM;
      public ImageSource LastWindowshotVM { get { return _lastWindowshotVM.Value; } }

      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _expressionVM;
      public ExpressionBaseVM ExpressionVM { get { return _expressionVM.Value; } }

      public UserSimulatorVM(UserSimulatorModel Model)
         : base(Model)
      {
         _lastWindowshotVM =
            new NotifyingTransformedProperty<ImageSource>(
               new[] { "LastWindowshot" }, "LastWindowshotVM",
               Model, this,
               () => Application.Current.Dispatcher.Invoke(() => DrawingImage2WpfImageSource(Model.LastWindowshot)));

         _expressionVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Expression" }, "ExpressionVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Expression),
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