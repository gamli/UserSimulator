using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;

namespace UserSimulator
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow
   {
      public MainWindow()
      {
         InitializeComponent();

         _model = new UserSimulatorModel();
         _viewModel = new UserSimulatorVM(_model);
         DataContext = _viewModel;

         _lispishView.Formatter = _model.Format;

         InitializeProgram();
      }

      bool _windowshotMouseDown;
      Point _windowshotLastMousePos;
      private readonly UserSimulatorModel _model;
      private readonly UserSimulatorVM _viewModel;

      private void InitializeProgram()
      {
         if (File.Exists("data"))
            _model.ExpressionText = File.ReadAllText("data");
      }

      private void ButtonSaveClick(object Sender, RoutedEventArgs E)
      {
         File.WriteAllText("data", _model.ExpressionText);
      }

      #region selction box within windowshot image

      private void WindowshotImage_MouseDown(object Sender, MouseButtonEventArgs Args)
      {
         _windowshotMouseDown = true;
         _windowshotLastMousePos = Args.GetPosition(_windowshotImage);
         _windowshotImage.CaptureMouse();

         Canvas.SetLeft(_windowshotSelectionBox, _windowshotLastMousePos.X);
         Canvas.SetTop(_windowshotSelectionBox, _windowshotLastMousePos.Y);
         _windowshotSelectionBox.Width = 0;
         _windowshotSelectionBox.Height = 0;

         _windowshotSelectionBox.Visibility = Visibility.Visible;
      }

      private void WindowshotImage_MouseUp(object Sender, MouseButtonEventArgs Args)
      {
         _windowshotMouseDown = false;
         _windowshotImage.ReleaseMouseCapture();

         _windowshotSelectionBox.Visibility = Visibility.Collapsed;

         var windowhotMouseUp = Args.GetPosition(_windowshotImage);

         var x = (int) _windowshotLastMousePos.X;
         var y = (int) _windowshotLastMousePos.Y;
         var width = (int) windowhotMouseUp.X - x;
         var height = (int) windowhotMouseUp.Y - y;
         _viewModel.InsertWindowshotSnippetIntoCodeEditor(Geometry.NormalizedRectangle(x, y, width, height));
      }

      private void WindowshotImage_MouseMove(object Sender, MouseEventArgs Args)
      {
         if (!_windowshotMouseDown) 
            return;

         var mousePos = Args.GetPosition(_windowshotImage);

         if (_windowshotLastMousePos.X < mousePos.X)
         {
            Canvas.SetLeft(_windowshotSelectionBox, _windowshotLastMousePos.X);
            _windowshotSelectionBox.Width = mousePos.X - _windowshotLastMousePos.X;
         }
         else
         {
            Canvas.SetLeft(_windowshotSelectionBox, mousePos.X);
            _windowshotSelectionBox.Width = _windowshotLastMousePos.X - mousePos.X;
         }

         if (_windowshotLastMousePos.Y < mousePos.Y)
         {
            Canvas.SetTop(_windowshotSelectionBox, _windowshotLastMousePos.Y);
            _windowshotSelectionBox.Height = mousePos.Y - _windowshotLastMousePos.Y;
         }
         else
         {
            Canvas.SetTop(_windowshotSelectionBox, mousePos.Y);
            _windowshotSelectionBox.Height = _windowshotLastMousePos.Y - mousePos.Y;
         }
      }

      #endregion
   }
}
