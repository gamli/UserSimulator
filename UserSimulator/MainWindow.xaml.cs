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
using MacroViewModel;

namespace UserSimulator
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : System.Windows.Window
   {
      private Program _model;
      private ObservableCollection<ProgramVM> _treeViewModel;

      public MainWindow()
      {
         InitializeComponent();

         var block = new Block();
         _model = new Program { Body = block };

         _treeViewModel = new ObservableCollection<ProgramVM> { new ProgramVM(_model) };
         DataContext = _treeViewModel;
      }

      private void ButtonFormatClick(object sender, RoutedEventArgs e)
      {
         Parse();
         Print();
      }

      private void ButtonPrintClick(object sender, RoutedEventArgs e)
      {
         Print();
      }

      private void Print()
      {

         _code.Text = new ProgramPrinter(_model).Print();
      }

      private void ButtonParseClick(object sender, RoutedEventArgs e)
      {
         Parse();
      }

      private void Parse()
      {
         try
         {
            _model = new ProgramParser().Parse(_code.Text);
         }
         catch (Exception e)
         {
            MessageBox.Show("Unknown error while parsing: " + e.Message);
         }
         
         _treeViewModel[0].Dispose();
         _treeViewModel.Clear();
         _treeViewModel.Add(new ProgramVM(_model));
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         new ProgramInterpreter(_model, IntPtr.Zero).Start();
      }

      private void _ast_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
      {
         Console.WriteLine();
      }
   }
}
