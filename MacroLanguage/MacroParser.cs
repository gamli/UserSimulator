using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Irony;
using Irony.Parsing;
using Macro;
using Numerics;

namespace MacroLanguage
{
   public class MacroParser
   {
      public const string TEXTLINE_DATA = "TextLine";
      public const string TEXTCOLUMN_DATA = "TextColumn";
      public const string TEXTPOSITION_DATA = "TextPosition";
      public const string TEXTLENGTH_DATA = "TextLength";
      private readonly Parser _parser;

      public MacroParser()
      {
         _parser = CreateParser();
      }

      private static Parser CreateParser()
      {
         var ironyGrammar = new IronyLispishGrammar();
         var ironyParser =
            new Parser(ironyGrammar)
            {
               Context = { Mode = ParseMode.CommandLine }
            };
         return ironyParser;
      }

      public MacroBase Parse(string Text)
      {
         var ast = _parser.Parse(Text);
         
         if (ast.HasErrors())
         {
            var firstMessage = ast.ParserMessages.First();
            throw
               new ParseException(string.Join("\r\n", ast.ParserMessages.Select(ParserMessage => GenerateParserMessage(ParserMessage, Text))))
                  {
                     Line = firstMessage.Location.Line,
                     Column = firstMessage.Location.Column,
                     Position = firstMessage.Location.Position
                  };
         }

         if (_parser.Context.Status == ParserStatus.AcceptedPartial)
            return null;

         var macro = BuildMacroFromAst(ast.Root);
         return macro;
      }

      private static string GenerateParserMessage(LogMessage ParserMessage, string Text)
      {
         var position = Math.Max(0, ParserMessage.Location.Position - 20);
         var length = Math.Min(Text.Length - position, 40);
         var textContext = Text.Substring(position, length);
         return ParserMessage.Message + " At ..." + textContext + "...";
      }

      [ExcludeFromCodeCoverage]
      private static MacroBase BuildMacroFromAst(ParseTreeNode AstNode)
      {
         MacroBase macro;
         switch (AstNode.Term.Name)
         {
            case "expression":
               return BuildMacroFromAst(AstNode.ChildNodes.Single());
            case "constant":
               switch (AstNode.ChildNodes[0].Token.Text)
               {
                  case "nil":
                     macro = new List();
                     break;
                  case "null":
                     macro = new Constant(null);
                     break;
                  default:
                     throw new ParseException("Unknown constant  >> " + AstNode.Token.Text + " <<");
               }
               break;
            case "boolean":
               macro = new Constant(AstNode.ChildNodes.Single().Token.Text == "true");
               break;
            case "string":
               macro = new Constant(AstNode.Token.Value);
               break;
            case "decimal-number":
               macro = new Constant(new BigRational(decimal.Parse((string) AstNode.Token.Value, CultureInfo.InvariantCulture)));
               break;
            case "rational-number":
               var numeratorAndDenominator = ((string) AstNode.Token.Value).Split('/');
               var numerator = BigInteger.Parse(numeratorAndDenominator[0]);
               var denominator = BigInteger.Parse(numeratorAndDenominator[1]);
               macro = new Constant(new BigRational(numerator, denominator));
               break;
            case "list":
               macro = new List(AstNode.ChildNodes[1].ChildNodes.Select(BuildMacroFromAst).Cast<Expression>().ToArray());
               break;
            case "symbol":
               macro = new Symbol(AstNode.Token.Text);
               break;
            case "quoted-expression-alias":
               macro = SpecialForms.Quote((Expression)BuildMacroFromAst(AstNode.ChildNodes[1]));
               break;
            default:
               throw new ParseException("Unknown construct in ast node >> " + AstNode + " <<");
         }

         AddMacroData(macro, TEXTLINE_DATA, AstNode.Span.Location.Line);
         AddMacroData(macro, TEXTCOLUMN_DATA, AstNode.Span.Location.Column);
         AddMacroData(macro, TEXTPOSITION_DATA, AstNode.Span.Location.Position);
         AddMacroData(macro, TEXTLENGTH_DATA, AstNode.Span.Length);

         return macro;
      }

      private static void AddMacroData(MacroBase Macro, string DataKey, object DataValue)
      {
         Macro.Data.Add(DataKey, DataValue);
      }
   }

   [ExcludeFromCodeCoverage]
   public class ParseException : Exception
   {
      public int Line { get; set; }
      public int Column { get; set; }
      public int Position { get; set; }

      public ParseException(string Message, Exception InnerException = null)
         : base(Message, InnerException)
      {
         // nothing to do
      }

      public string DisplayMessage()
      {
         return "(LINE: " + (Line + 1) + ", COLOUMN: " + (Column + 1) + ") " + Message;
      }
   }
}
