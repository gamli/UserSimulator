using System.IO;
using System.Windows;
using MacroRuntime;

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
         _model.PropertyChanged += 
            (Sender, Args) => 
               {
                  if (Args.PropertyName == "ExpressionText")
                     _codeEditor.Document.Text = _model.ExpressionText;
               };
         var viewModel = new UserSimulatorVM(_model);
         DataContext = viewModel;

         InitializeProgram();         
      }

      private void CodeEditorLostFocus(object Sender, RoutedEventArgs E)
      {
         _model.ExpressionText = _codeEditor.Text;
      }

      private void InitializeProgram()
      {
         if (File.Exists("data"))
            _model.ExpressionText = File.ReadAllText("data");
         _codeEditor.Document.Text = _model.ExpressionText;
      }

      private void ButtonExecuteClick(object Sender, RoutedEventArgs E)
      {
         _moo.DataContext = new ExpressionEvaluator(new RuntimeContext(_model.LastWindow)).Evaluate(_model.Expression);
      }

      private void ButtonSaveClick(object Sender, RoutedEventArgs E)
      {
         File.WriteAllText("data", _model.ExpressionText);
      }
   }
}
