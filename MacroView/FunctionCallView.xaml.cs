using System.Windows;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for FunctionCallView.xaml
   /// </summary>
   public partial class FunctionCallView
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
