using System.IO;
using System.Windows;
using System.Windows.Input;

namespace UserSimulator
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow
   {
      private readonly UserSimulatorModel _model;

      public MainWindow()
      {
         InitializeComponent();

         _model = new UserSimulatorModel();
         var viewModel = new UserSimulatorVM(_model);
         DataContext = viewModel;

         _lispishView.Formatter = _model.Format;

         InitializeProgram();         
      }

      private void InitializeProgram()
      {
         if (File.Exists("data"))
            _model.ExpressionText = File.ReadAllText("data");
      }

      private void ButtonSaveClick(object Sender, RoutedEventArgs E)
      {
         File.WriteAllText("data", _model.ExpressionText);
      }
   }
}
