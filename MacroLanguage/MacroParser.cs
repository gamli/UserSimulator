using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Irony;
using Irony.Parsing;
using Macro;

namespace MacroLanguage
{
   public class MacroParser
   {
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

      private string GenerateParserMessage(LogMessage ParserMessage, string Text)
      {
         var position = Math.Max(0, ParserMessage.Location.Position - 20);
         var length = Math.Min(Text.Length - position, 40);
         var textContext = Text.Substring(position, length);
         return ParserMessage.Message + " At ..." + textContext + "...";
      }

      [ExcludeFromCodeCoverage]
      private MacroBase BuildMacroFromAst(ParseTreeNode AstNode)
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
            case "number":
               macro = new Constant(AstNode.Token.Value);
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

         macro.Data.Add("TextLocation", AstNode.Span.Location.Position);
         macro.Data.Add("TextLength", AstNode.Span.Length);

         return macro;
      }
   }

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
