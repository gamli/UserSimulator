using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Macro;
using MacroViewModel;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for ConstantExpressionView.xaml
   /// </summary>
   public partial class ConstantView
   {
      public ConstantView()
      {
         InitializeComponent();
      }
   }
   public class ConstantViewTemplateSelector : DataTemplateSelector
   {
      public override DataTemplate SelectTemplate(object ConstantVM, DependencyObject Container)
      {
         var dataTemplateResourceKey = "defaultTemplate";

         var element = Container as FrameworkElement;
         if (element != null && ConstantVM is ConstantVM)
         {
            var constantValueType = ((Constant)((ExpressionVM)ConstantVM).Model).Value.GetType();

            if (constantValueType == typeof(bool))
               dataTemplateResourceKey = "booleanTemplate";
            else if (constantValueType == typeof(string))
               dataTemplateResourceKey = "stringTemplate";
            else if (constantValueType == typeof (decimal))
               dataTemplateResourceKey = "decimalTemplate";
         }

         return FindDataTemplate(element, dataTemplateResourceKey);
      }

      private static DataTemplate FindDataTemplate(FrameworkElement Element, string DataTemplateResourceKey)
      {
         return Element.FindResource(DataTemplateResourceKey) as DataTemplate;
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
