using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for LISPishView.xaml
   /// </summary>
   public partial class LISPishView
   {
      private readonly ParserErrorImage _parserErrorImage = new ParserErrorImage();

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

         _editor.TextArea.TextView.ElementGenerators.Add(_parserErrorImage);
      }

      public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
         "Text", typeof (string), typeof (LISPishView), 
         new FrameworkPropertyMetadata(
            default(string), 
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            (Sender, Args) =>
               {
                  var editor = ((LISPishView) Sender)._editor;
                  var newValue = (string) Args.NewValue;
                  if (!Equals(editor.Text, newValue))
                        editor.Text = newValue;
               }));
      public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }

      public static readonly DependencyProperty ParserErrorPositionProperty = DependencyProperty.Register(
         "ParserErrorPosition", typeof(int), typeof(LISPishView),
         new FrameworkPropertyMetadata(
            -1,
            (Sender, Args) =>
               {
                  var lispishView = (LISPishView)Sender;
                  lispishView._parserErrorImage.Position = lispishView.ParserErrorPosition;
                  RepaintEditor(lispishView);
               }));

      public int ParserErrorPosition { get { return (int)GetValue(ParserErrorPositionProperty); } set { SetValue(ParserErrorPositionProperty, value); } }

      public static readonly DependencyProperty ParserErrorMessageProperty = DependencyProperty.Register(
         "ParserErrorMessage", typeof(string), typeof(LISPishView),
         new FrameworkPropertyMetadata(
            default(string),
            (Sender, Args) =>
               {
                  var lispishView = (LISPishView)Sender;
                  lispishView._parserErrorImage.Message = lispishView.ParserErrorMessage;
                  RepaintEditor(lispishView);
               }));
      public string ParserErrorMessage { get { return (string)GetValue(ParserErrorMessageProperty); } set { SetValue(ParserErrorMessageProperty, value); } }

      private sealed class ParserErrorImage : VisualLineElementGenerator
      {
         public int Position { private get; set; }
         public string Message { private get; set; }

         public override int GetFirstInterestedOffset(int StartOffset)
         {
            return Position > StartOffset ? Position : -1;
         }

         public override VisualLineElement ConstructElement(int Offset)
         {
            var bitmap = new BitmapImage(new Uri("pack://application:,,,/MacroView;component/Resources/ParserErrorIcon.png"));
            var image = 
               new Image
                  {
                     Source = bitmap,
                     Width = bitmap.PixelWidth,
                     Height = bitmap.PixelHeight,
                     ToolTip = Message
                  };
            return new InlineObjectElement(0, image);
         }
      }

      private static void RepaintEditor(LISPishView LISPishView)
      {
         var editor = LISPishView._editor;
         var text = editor.Text;
         editor.Text = "";
         editor.Text = text;
      }
   }
}
