using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace MacroView
{
   public class LispishSyntaxHighlighter : DocumentColorizingTransformer
   {
      public int LastErrorPosition { private get; set; }
      public int LastErrorLength { private get; set; }

      private const string SEPARATOR = @"\(|\)|\s|""|'";
      private readonly Regex _separator = new Regex(SEPARATOR, RegexOptions.Compiled);
      private readonly Regex
         _procedureCall = new Regex(@"\(\s*([^\(\)\s""']*)\b", RegexOptions.Compiled),

         _specialForms = new Regex(@"define|if|lambda|quote|\.", RegexOptions.Compiled),
         _quoteSyntax = new Regex(@"'", RegexOptions.Compiled),
         _lambdaFormalArgs = new Regex(@"lambda\s*\([^\(\)""']*\)", RegexOptions.Compiled),

         _specialFunctions =
            new Regex(
               @"eval|=|constant\?|list\?|symbol\?|<=|>=|<|>|or|and|\+|-|\*|/|abs|car|cdr|append|move|position|pause|click|windowshot|list|last|begin",
               RegexOptions.Compiled),

         _booleanTrue = new Regex(@"true", RegexOptions.Compiled),
         _booleanFalse = new Regex(@"false", RegexOptions.Compiled),

         _number = new Regex(@"\d+(\.[0-9]+)?|\.[0-9]+", RegexOptions.Compiled),

         _string = new Regex(@"""([^""]|\"")*""", RegexOptions.Compiled),
         _escapedCharacter = new Regex(@"\\.", RegexOptions.Compiled);

      protected override void ColorizeLine(DocumentLine Line)
      {
         StyleRegex(_procedureCall, Line, true, FontStyles.Oblique, Brushes.Blue, Brushes.Transparent, 1);
         StyleRegex(_lambdaFormalArgs, Line, true, FontStyles.Normal, Brushes.BlueViolet);

         StyleRegex(_specialForms, Line, false, FontWeights.Bold, Brushes.MediumVioletRed);
         StyleRegex(_quoteSyntax, Line, true, FontWeights.Bold, Brushes.MediumVioletRed);

         StyleRegex(_specialFunctions, Line, false, FontStyles.Italic, FontWeights.Bold, Brushes.DarkOrange);

         StyleRegex(_booleanTrue, Line, false, FontStyles.Oblique, FontWeights.SemiBold, Brushes.Green);
         StyleRegex(_booleanFalse, Line, false, FontStyles.Oblique, FontWeights.SemiBold, Brushes.Red);

         StyleRegex(_number, Line, false, Brushes.DimGray);

         StyleRegex(_string, Line, true, Brushes.DarkRed);
         StyleRegex(_escapedCharacter, Line, true, Brushes.DeepPink);

         StyleError(Line);
      }

      private void StyleError(DocumentLine Line)
      {
         var lastErrorDisplayPosition = LastErrorPosition == 0 ? 0 : LastErrorPosition - 1;
         
         // text has been deleted or changed in some other way - so the error is not longer valid - TODO handle this somewhere else?
         if(CurrentContext.Document.TextLength <= lastErrorDisplayPosition)
            return;

         if (lastErrorDisplayPosition > -1 && CurrentContext.Document.GetLineByOffset(lastErrorDisplayPosition) == Line)
         {
            var lastErrorLength = LastErrorLength <= 0 ? 5 : LastErrorLength;
            
            var positionInLine = lastErrorDisplayPosition - Line.Offset;
            Contract.Assert(positionInLine >= 0);

            var displayLength = Math.Min(Line.Length - positionInLine, lastErrorLength);
            Contract.Assert(positionInLine + displayLength <= Line.Length);

            StyleLinePart(
               lastErrorDisplayPosition,
               displayLength,
               FontStyles.Italic,
               FontWeights.Bold,
               Brushes.Red,
               Brushes.Yellow);
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

   public class ImageConstant : VisualLineElementGenerator
   {
      readonly static Regex IMAGE_REGEX = new Regex(@"""([0-9A-F]*)""");

      Match FindMatch(int StartOffset)
      {
         var endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
         var document = CurrentContext.Document;
         var relevantText = document.GetText(StartOffset, endOffset - StartOffset);
         return IMAGE_REGEX.Match(relevantText);
      }

      public override int GetFirstInterestedOffset(int StartOffset)
      {
         var match = FindMatch(StartOffset);
         return match.Success ? (StartOffset + match.Index) + 1 : -1;
      }

      public override VisualLineElement ConstructElement(int Offset)
      {
         var match = FindMatch(Offset - 1);
         if (match.Success && match.Index == 0)
         {
            var hexValue = match.Groups[1].Value;
            var bitmap = LoadBitmapFromHexValue(hexValue);
            if (bitmap != null)
            {
               var image =
                  new Image
                  {
                     Source = bitmap,
                     MaxWidth = 18,
                     MaxHeight = 18,
                     ToolTip =
                        new Image { Source = bitmap }
                  };
               return new InlineObjectElement(match.Length - 2, image);
            }
         }
         return null;
      }

      static BitmapImage LoadBitmapFromHexValue(string HexValue)
      {
         try
         {
            var binaryValue = new byte[HexValue.Length / 2];
            for (var i = 0; i < HexValue.Length; i += 2)
               binaryValue[i / 2] = byte.Parse(HexValue.Substring(i, 2), NumberStyles.AllowHexSpecifier);

            using (var stream = new MemoryStream())
            {
               stream.Write(binaryValue, 0, binaryValue.Length);
               var bitmap = new BitmapImage();
               bitmap.BeginInit();
               bitmap.CacheOption = BitmapCacheOption.OnLoad;
               bitmap.StreamSource = stream;
               bitmap.EndInit();
               bitmap.Freeze();
               return bitmap;
            }
         }
         catch (Exception) // don't care
         {
            return null;
         }
      }
   }
}
