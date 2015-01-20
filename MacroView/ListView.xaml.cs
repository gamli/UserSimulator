using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Macro;
using MacroViewModel;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for BlockView.xaml
   /// </summary>
   public partial class ListView
   {
      public ListView()
      {
         InitializeComponent();
      }
   }
   public class ListViewTemplateSelector : DataTemplateSelector
   {
      public override DataTemplate SelectTemplate(object ListVM, DependencyObject Container)
      {
         var dataTemplateResourceKey = "defaultTemplate";

         var element = Container as FrameworkElement;
         if (element != null && ListVM is ListVM)
         {
            var expressions = ((List)((ExpressionVM)ListVM).Model).Expressions;

            if (expressions.Count > 0)
            {
               var firstSymbol = expressions[0] as Symbol;
               if (firstSymbol != null)
               {
                  switch (firstSymbol.Value)
                  {
                     case "define":
                        dataTemplateResourceKey = "defineTemplate";
                        break;
                     case "if":
                        dataTemplateResourceKey = "ifTemplate";
                        break;
                     case "lambda":
                        dataTemplateResourceKey = "lambdaTemplate";
                        break;
                     case "quote":
                        dataTemplateResourceKey = "quoteTemplate";
                        break;
                     case "loop":
                        dataTemplateResourceKey = "loopTemplate";
                        break;
                     case "begin":
                        dataTemplateResourceKey = "beginTemplate";
                        break;
                     default:
                        dataTemplateResourceKey = "procedureCallTemplate";
                        break;
                  }
               }
            }
            else
               dataTemplateResourceKey = "nilTemplate";
         }

         return FindDataTemplate(element, dataTemplateResourceKey);
      }

      private static DataTemplate FindDataTemplate(FrameworkElement Element, string DataTemplateResourceKey)
      {
         return Element.FindResource(DataTemplateResourceKey) as DataTemplate;
      }
   }
}
