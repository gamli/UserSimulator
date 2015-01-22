using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;
using Point = System.Windows.Point;

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

      bool _windowshotImageMouseDown;
      Point _windowshotImageLastMouseDownPosition;
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
         _windowshotImageMouseDown = true;
         _windowshotImageLastMouseDownPosition = Args.GetPosition(_windowshotImage);
         _windowshotImage.CaptureMouse();

         Canvas.SetLeft(_windowshotImageSelectionBox, _windowshotImageLastMouseDownPosition.X);
         Canvas.SetTop(_windowshotImageSelectionBox, _windowshotImageLastMouseDownPosition.Y);
         _windowshotImageSelectionBox.Width = 0;
         _windowshotImageSelectionBox.Height = 0;

         _windowshotImageSelectionBox.Visibility = Visibility.Visible;
      }

      private void WindowshotImage_MouseUp(object Sender, MouseButtonEventArgs Args)
      {
         _windowshotImageMouseDown = false;
         _windowshotImage.ReleaseMouseCapture();

         _windowshotImageSelectionBox.Visibility = Visibility.Collapsed;

         var windowhotImageMouseUp = Args.GetPosition(_windowshotImage);

         var x = (int) _windowshotImageLastMouseDownPosition.X;
         var y = (int) _windowshotImageLastMouseDownPosition.Y;
         var width = (int) windowhotImageMouseUp.X - x;
         var height = (int) windowhotImageMouseUp.Y - y;
         _viewModel.InsertWindowshotIntoCodeEditor(Geometry.NormalizedRectangle(x, y, width, height));

         // TODO: 
         //
         // The mouse has been released, check to see if any of the items 
         // in the other canvas are contained within mouseDownPos and 
         // mouseUpPos, for any that are, select them!
         //
      }

      private void WindowshotImage_MouseMove(object Sender, MouseEventArgs Args)
      {
         if (!_windowshotImageMouseDown) 
            return;

         var mousePos = Args.GetPosition(_windowshotImage);

         if (_windowshotImageLastMouseDownPosition.X < mousePos.X)
         {
            Canvas.SetLeft(_windowshotImageSelectionBox, _windowshotImageLastMouseDownPosition.X);
            _windowshotImageSelectionBox.Width = mousePos.X - _windowshotImageLastMouseDownPosition.X;
         }
         else
         {
            Canvas.SetLeft(_windowshotImageSelectionBox, mousePos.X);
            _windowshotImageSelectionBox.Width = _windowshotImageLastMouseDownPosition.X - mousePos.X;
         }

         if (_windowshotImageLastMouseDownPosition.Y < mousePos.Y)
         {
            Canvas.SetTop(_windowshotImageSelectionBox, _windowshotImageLastMouseDownPosition.Y);
            _windowshotImageSelectionBox.Height = mousePos.Y - _windowshotImageLastMouseDownPosition.Y;
         }
         else
         {
            Canvas.SetTop(_windowshotImageSelectionBox, mousePos.Y);
            _windowshotImageSelectionBox.Height = _windowshotImageLastMouseDownPosition.Y - mousePos.Y;
         }
      }

      #endregion
   }
}
