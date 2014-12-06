using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for FunctionCallView.xaml
   /// </summary>
   public partial class FunctionCallView : ItemsControl
   {
      public FunctionCallView()
      {
         InitializeComponent();
      }

      public string FunctionName
      {
         get { return (string)GetValue(FunctionNameProperty); }
         set { SetValue(FunctionNameProperty, value); }
      }
      public static readonly DependencyProperty FunctionNameProperty =
         DependencyProperty.Register("FunctionName", typeof(string), typeof(FunctionCallView), null);
   }

   public class FunctionCallParameter : FrameworkElement
   {
      public FunctionCallParameter()
      {
         Value = "asdf";
      }
      public object Value
      {
         get { return (string)GetValue(ValueProperty); }
         set { SetValue(ValueProperty, value); }
      }
      public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(object), typeof(FunctionCallParameter), null);
   }
}
