using System;
using System.Collections.Generic;
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
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for LISPishView.xaml
   /// </summary>
   public partial class LISPishView
   {
      public LISPishView()
      {
         InitializeComponent();

         _editor.PreviewKeyUp += (Sender, Args) =>
            {
               if(Args.Key == Key.Enter)
                  Text = _editor.Text;
            };
         _editor.LostFocus += (Sender, Args) =>
            {
               Text = _editor.Text;
            };

         var syntaxDefinitionResource = Application.GetResourceStream(
            new Uri("pack://application:,,,/MacroView;component/Resources/LispishSyntaxDefinition.xshd"));
         if (syntaxDefinitionResource != null)
            using (var syntaxDefinitionStream = syntaxDefinitionResource.Stream)
            {
               using (var syntaxDefinitionXmlReader = XmlReader.Create(syntaxDefinitionStream))
               {
                  _editor.SyntaxHighlighting =
                     HighlightingLoader.Load(syntaxDefinitionXmlReader, HighlightingManager.Instance);  
               }
            }
      }

      public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
         "Text", typeof (string), typeof (LISPishView), 
         new FrameworkPropertyMetadata(
            default(string), 
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            (Sender, Args) =>
               {
                  if (!Equals(((LISPishView) Sender)._editor.Text, (string) Args.NewValue))
                  {
                     //if(((LISPishView)Sender)._editor.)
                     //   ((LISPishView)Sender)._editor.EndChange();
                     ((LISPishView)Sender)._editor.Text = (string)Args.NewValue;
                  }
               }));
      public string Text { get { return (string) GetValue(TextProperty); } set { SetValue(TextProperty, value); } }
   }
}
