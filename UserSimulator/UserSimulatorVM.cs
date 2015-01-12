using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Macro;
using MacroViewModel;

namespace UserSimulator
{
   class UserSimulatorVM : ViewModelBase<UserSimulatorModel>
   {
      private readonly NotifyingTransformedProperty<ImageSource> _lastWindowshotVM;
      public ImageSource LastWindowshotVM { get { return _lastWindowshotVM.Value; } }

      private readonly NotifyingTransformedProperty<ExpressionVM> _expressionVM;
      public ExpressionVM ExpressionVM { get { return _expressionVM.Value; } }

      private readonly NotifyingTransformedProperty<ExpressionVM> _evaluatedExpressionVM;
      public ExpressionVM EvaluatedExpressionVM { get { return _evaluatedExpressionVM.Value; } }

      public UserSimulatorVM(UserSimulatorModel Model)
         : base(Model)
      {
         _lastWindowshotVM =
            new NotifyingTransformedProperty<ImageSource>(
               new[] { "LastWindowshot" }, "LastWindowshotVM",
               Model, this,
               () => Application.Current.Dispatcher.Invoke(() => DrawingImage2WpfImageSource(Model.LastWindowshot)));

         _expressionVM =
            new NotifyingTransformedProperty<ExpressionVM>(
               new[] { "Expression" }, "ExpressionVM",
               Model, this,
               () => (ExpressionVM)MacroViewModelFactory.Instance.Create(Model.Expression),
               ViewModel => ViewModel.Dispose());

         _evaluatedExpressionVM =
            new NotifyingTransformedProperty<ExpressionVM>(
               new[] { "EvaluatedExpression" }, "EvaluatedExpressionVM",
               Model, this,
               () => (ExpressionVM)MacroViewModelFactory.Instance.Create(Model.EvaluatedExpression),
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
