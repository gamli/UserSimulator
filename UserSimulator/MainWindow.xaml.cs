using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IO;
using Macro;
using MacroLanguage;
using MacroRuntime;
using MacroViewModel;

namespace UserSimulator
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : System.Windows.Window
   {
      private readonly UserSimulatorModel _model;
      private readonly UserSimulatorVM _viewModel;

      public MainWindow()
      {
         InitializeComponent();

         _model = new UserSimulatorModel();
         _model.PropertyChanged += (Sender, Args) => {
            if (Args.PropertyName == "ExpressionText")
               _codeEditor.Document.Text = _model.ExpressionText;
         };
         _viewModel = new UserSimulatorVM(_model);
         DataContext = _viewModel;

         InitializeProgram();         
      }

      private void CodeEditorLostFocus(object sender, RoutedEventArgs e)
      {
         _model.ExpressionText = _codeEditor.Text;
      }

      private void InitializeProgram()
      {
         if (File.Exists("data"))
            _model.ExpressionText = File.ReadAllText("data");
         _codeEditor.Document.Text = _model.ExpressionText;
      }

      private void ButtonExecuteClick(object sender, RoutedEventArgs e)
      {
         _moo.DataContext = new ExpressionEvaluator(new RuntimeContext(_model.LastWindow)).Evaluate(_model.Expression);
      }

      private void ButtonSaveClick(object sender, RoutedEventArgs e)
      {
         File.WriteAllText("data", _model.ExpressionText);
      }
   }
}
