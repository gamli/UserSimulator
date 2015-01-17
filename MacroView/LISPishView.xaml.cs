using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
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
      private readonly LispishSyntaxHighlighter _syntaxHighlighter = new LispishSyntaxHighlighter();

      public LISPishView()
      {
         InitializeComponent();

         _editor.PreviewKeyUp += (Sender, Args) =>
            {
               if (Args.Key == Key.Enter && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
               {
                  Text = _editor.Text;
                  Args.Handled = true;
               }
            };
         _editor.LostFocus += (Sender, Args) =>
            {
               Text = _editor.Text;
            };

         _editor.TextArea.TextView.LineTransformers.Add(_syntaxHighlighter);
      }

      public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
         "Text", typeof(string), typeof(LISPishView),
         new FrameworkPropertyMetadata(
            default(string),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            (Sender, Args) =>
            {
               var editor = ((LISPishView)Sender)._editor;
               var newValue = (string)Args.NewValue;
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
               lispishView._syntaxHighlighter.ParserErrorPosition = lispishView.ParserErrorPosition;
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
               lispishView._syntaxHighlighter.ParserErrorMessage = lispishView.ParserErrorMessage;
               RepaintEditor(lispishView);
            }));
      public string ParserErrorMessage { get { return (string)GetValue(ParserErrorMessageProperty); } set { SetValue(ParserErrorMessageProperty, value); } }

      public static readonly DependencyProperty EvaluationErrorPositionProperty = DependencyProperty.Register(
         "EvaluationErrorPosition", typeof(int), typeof(LISPishView),
         new FrameworkPropertyMetadata(
            -1,
            (Sender, Args) =>
            {
               var lispishView = (LISPishView)Sender;
               lispishView._syntaxHighlighter.EvaluationErrorPosition = lispishView.EvaluationErrorPosition;
               RepaintEditor(lispishView);
            }));

      public int EvaluationErrorPosition { get { return (int)GetValue(EvaluationErrorPositionProperty); } set { SetValue(EvaluationErrorPositionProperty, value); } }

      public static readonly DependencyProperty EvaluationErrorMessageProperty = DependencyProperty.Register(
         "EvaluationErrorMessage", typeof(string), typeof(LISPishView),
         new FrameworkPropertyMetadata(
            default(string),
            (Sender, Args) =>
            {
               var lispishView = (LISPishView)Sender;
               lispishView._syntaxHighlighter.EvaluationErrorMessage = lispishView.EvaluationErrorMessage;
               RepaintEditor(lispishView);
            }));
      public string EvaluationErrorMessage { get { return (string)GetValue(EvaluationErrorMessageProperty); } set { SetValue(EvaluationErrorMessageProperty, value); } }

      private static void RepaintEditor(LISPishView LISPishView)
      {
         var editor = LISPishView._editor;
         var text = editor.Text;
         editor.Text = "";
         editor.Text = text;
      }

      private sealed class LispishSyntaxHighlighter : DocumentColorizingTransformer
      {
         public int ParserErrorPosition { private get; set; }
         public string ParserErrorMessage { private get; set; }
         public int EvaluationErrorPosition { private get; set; }
         public string EvaluationErrorMessage { private get; set; }

         private const string SEPARATOR = @"\(|\)|\s|""|'";
         private readonly Regex _separator = new Regex(SEPARATOR);
         private readonly Regex
            _procedureCall = new Regex(@"\(\s*([^\(\)\s""']*)\b"),

            _specialForms = new Regex(@"define|if|lambda|quote|\."),
            _quoteSyntax = new Regex(@"'"),
            _lambdaFormalArgs = new Regex(@"lambda\s*\(.*\)"),

            _specialFunctions = new Regex(@"eval|=|constant\?|list\?|symbol\?|<=|>=|<|>|or|and|\+|-|\*|/|abs|car|cdr|append|move|position|pause|click|windowshot|list|last|begin"),

            _booleanTrue = new Regex(@"true"),
            _booleanFalse = new Regex(@"false"),

            _number = new Regex(@"\d+(\.[0-9]+)?|\.[0-9]+"),

            _string = new Regex(@"""([^""]|\"")*"""),
            _escapedCharacter = new Regex(@"\\.");

         protected override void ColorizeLine(DocumentLine Line)
         {
            StyleRegex(_procedureCall, Line, true, FontStyles.Oblique, Brushes.Blue, Brushes.Transparent, 1);
            StyleRegex(_lambdaFormalArgs, Line, true, FontStyles.Normal, Brushes.BlueViolet);

            StyleRegex(_specialForms, Line, false, FontWeights.Bold, Brushes.MediumVioletRed);
            StyleRegex(_quoteSyntax, Line, true, FontWeights.Bold, Brushes.MediumVioletRed);

            StyleRegex(_specialFunctions, Line, false, FontStyles.Italic, FontWeights.Bold, Brushes.DarkOrange);

            StyleRegex(_booleanTrue, Line, false, FontStyles.Oblique, FontWeights.SemiBold, Brushes.Green);
            StyleRegex(_booleanFalse, Line, false, FontStyles.Oblique, FontWeights.SemiBold, Brushes.Red);

            StyleRegex(_number, Line, false, FontStyles.Oblique, Brushes.DimGray);

            StyleRegex(_string, Line, true, Brushes.DarkRed);
            StyleRegex(_escapedCharacter, Line, true, Brushes.DeepPink);

            var textLength = CurrentContext.Document.Text.Length;

            if (ParserErrorPosition >= textLength)
               ParserErrorPosition = -1;
            if (ParserErrorPosition > -1 && CurrentContext.Document.GetLineByOffset(ParserErrorPosition) == Line)
            {
               var shiftedPosition = Math.Max(ParserErrorPosition - 1, 0);
               var shiftedPositionInLine = shiftedPosition - Line.Offset;
               StyleLinePart(
                  shiftedPosition, 
                  Line.Length - shiftedPositionInLine, 
                  FontStyles.Italic, 
                  FontWeights.Bold, 
                  Brushes.Red,
                  Brushes.Yellow);
            }

            if (EvaluationErrorPosition >= textLength)
               EvaluationErrorPosition = -1;
            if (EvaluationErrorPosition > -1 && CurrentContext.Document.GetLineByOffset(EvaluationErrorPosition) == Line)
            {
               var shiftedPosition = Math.Max(EvaluationErrorPosition - 1, 0);
               var shiftedPositionInLine = shiftedPosition - Line.Offset;
               StyleLinePart(
                  shiftedPosition,
                  Line.Length - shiftedPositionInLine,
                  FontStyles.Italic,
                  FontWeights.Bold,
                  Brushes.Blue,
                  Brushes.DeepPink);
            }
         }

         private void StyleRegex(Regex Regex, DocumentLine Line, bool IgnoreSeparator,
            Brush Foreground, Brush Background = null, int MatchOffsetLeft = 0, int MatchOffsetRight = 0)
         {
            StyleRegex(Regex, Line, IgnoreSeparator, FontStyles.Normal, FontWeights.Normal, Foreground, Background, MatchOffsetLeft, MatchOffsetRight);
         }

         private void StyleRegex(Regex Regex, DocumentLine Line, bool IgnoreSeparator,
            FontWeight Weight, Brush Foreground, Brush Background = null, int MatchOffsetLeft = 0, int MatchOffsetRight = 0)
         {
            StyleRegex(Regex, Line, IgnoreSeparator, FontStyles.Normal, Weight, Foreground, Background, MatchOffsetLeft, MatchOffsetRight);
         }

         private void StyleRegex(Regex Regex, DocumentLine Line, bool IgnoreSeparator,
            FontStyle Style, Brush Foreground, Brush Background = null, int MatchOffsetLeft = 0, int MatchOffsetRight = 0)
         {
            StyleRegex(Regex, Line, IgnoreSeparator, Style, FontWeights.Normal, Foreground, Background, MatchOffsetLeft, MatchOffsetRight);
         }

         private void StyleRegex(Regex Regex, DocumentLine Line, bool IgnoreSeparator,
            FontStyle Style, FontWeight Weight, Brush Foreground, Brush Background = null, int MatchOffsetLeft = 0, int MatchOffsetRight = 0)
         {
            var lineText = CurrentContext.Document.GetText(Line);
            var matches = Regex.Matches(lineText);
            foreach (Match match in matches)
            {
               var beginIndex = match.Index + MatchOffsetLeft;
               var length = match.Length - MatchOffsetLeft + MatchOffsetRight;
               var endIndex = beginIndex + length;
               if (IgnoreSeparator || IsSeparator(lineText, beginIndex - 1) && IsSeparator(lineText, endIndex))
               {
                  var lineBeginOffset = Line.Offset + beginIndex;
                  StyleLinePart(
                     lineBeginOffset,
                     length,
                     Style, Weight, Foreground, Background);
               }
            }
         }

         private void StyleLinePart(int LineBeginOffset, int Length, FontStyle Style, FontWeight Weight, Brush Foreground, Brush Background = null)
         {
            ChangeLinePart(
               LineBeginOffset,
               LineBeginOffset + Length,
               Element => SetTextRunProperties(Style, Weight, Foreground, Background, Element));
         }

         private static void SetTextRunProperties(FontStyle Style, FontWeight Weight, Brush Foreground, Brush Background,
            VisualLineElement Element)
         {
            Element.TextRunProperties.SetForegroundBrush(Foreground);

            Element.TextRunProperties.SetBackgroundBrush(Background ?? Brushes.Transparent);

            var defaultTypeface = Element.TextRunProperties.Typeface;
            Element.TextRunProperties.SetTypeface(
               new Typeface(
                  defaultTypeface.FontFamily,
                  Style,
                  Weight,
                  defaultTypeface.Stretch));
         }

         private bool IsSeparator(string Text, int Index)
         {
            return
               Index < 0 ||
               Index >= Text.Length ||
               _separator.IsMatch(Text[Index].ToString());
         }
      }
   }
}
