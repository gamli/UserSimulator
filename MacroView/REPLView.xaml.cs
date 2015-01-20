using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MacroRuntime;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for REPLView.xaml
   /// </summary>
   public partial class REPLView
   {
      public REPLView()
      {
         InitializeComponent();
      }

      private void REPLInput_OnKeyDown(object Sender, KeyEventArgs E)
      {
         if (E.Key == Key.Enter)
         {
            ((REPL) DataContext).ConsumeInput(_replInput.Text);
            _replInput.Text = "";
            _replOutput.ScrollToBottom();
         }
      }

      private void REPLView_OnGotFocus(object Sender, RoutedEventArgs E)
      {
         _replInput.Focus();
      }

      private void ResetREPL_OnClick(object Sender, RoutedEventArgs E)
      {
         ((REPL)DataContext).Reset();
      }
   }
   public class REPLOutputViewTemplateSelector : DataTemplateSelector
   {
      public override DataTemplate SelectTemplate(object REPLOutput, DependencyObject Container)
      {
         string dataTemplateResourceKey = null;

         var element = Container as FrameworkElement;
         if (element != null && REPLOutput is REPLOutput)
         {

            switch (((REPLOutput)REPLOutput).Type)
            {
               case REPLOutputType.InputEcho:
                  dataTemplateResourceKey = "inputEchoTemplate";
                  break;
               case REPLOutputType.EvaluatedExpression:
                  dataTemplateResourceKey = "evaluatedExpressionTemplate";
                  break;
               case REPLOutputType.ParseError:
                  dataTemplateResourceKey = "parseErrorTemplate";
                  break;
               case REPLOutputType.RuntimeError:
                  dataTemplateResourceKey = "runtimeErrorTemplate";
                  break;
               default:
                  throw new ArgumentOutOfRangeException();
            }
         }

         return FindDataTemplate(element, dataTemplateResourceKey);
      }

      private static DataTemplate FindDataTemplate(FrameworkElement Element, string DataTemplateResourceKey)
      {
         return Element.FindResource(DataTemplateResourceKey) as DataTemplate;
      }
   }
}
