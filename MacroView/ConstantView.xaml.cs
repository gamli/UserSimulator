using System;
using System.Collections.Generic;
using System.Globalization;
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
using Macro;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for ConstantExpressionView.xaml
   /// </summary>
   public partial class ConstantView : UserControl
   {
      public ConstantView()
      {
         InitializeComponent();
      }
   }
   public class ConstantViewTemplateSelector : DataTemplateSelector
   {
      public override DataTemplate SelectTemplate(object Constant, DependencyObject Container)
      {
         FrameworkElement element = Container as FrameworkElement;
         if (element != null && Constant != null && Constant is Constant)
         {
            var constantValueType = ((Constant)Constant).Value.GetType();
            if (constantValueType == typeof(bool))
               return element.FindResource("booleanTemplate") as DataTemplate;
            if (constantValueType == typeof(string))
               return element.FindResource("stringTemplate") as DataTemplate;
            if (constantValueType == typeof(int))
               return element.FindResource("integerTemplate") as DataTemplate;
            if (constantValueType == typeof(double))
               return element.FindResource("doubleTemplate") as DataTemplate;
         }
         return null;
      }
   }

   [ValueConversion(typeof(object), typeof(string))]
   public class NumericConverter : IValueConverter
   {
      public object Convert(object NumericValue, Type TargetType, object Parameter, CultureInfo Culture)
      {
         return NumericValue;
      }

      public object ConvertBack(object NumericValue, Type TargetType, object Parameter, CultureInfo Culture)
      {
         int intValue;
         if (int.TryParse((string)NumericValue, out intValue))
            return intValue;
         double doubleValue;
         if (double.TryParse((string)NumericValue, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleValue))
            return doubleValue;
         return null;
      }
   }
}
