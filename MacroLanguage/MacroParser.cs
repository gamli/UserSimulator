using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Irony.Parsing;
using Macro;

namespace MacroLanguage
{
   public class MacroParser
   {
      public MacroParser()
      {
         _parser = CreateParser();
      }

      public MacroBase Parse(string Text)
      {
         var ast = _parser.Parse(Text);
         if (ast.Status != ParseTreeStatus.Parsed)
         {
            var firstMessage = ast.ParserMessages.First();
            throw
               new ParseException(string.Join("\r\n", ast.ParserMessages.Select(ParserMessage => ParserMessage.Message)))
                  {
                     Line = firstMessage.Location.Line,
                     Column = firstMessage.Location.Column,
                     Position = firstMessage.Location.Position
                  };
         }
         var macro = BuildMacroFromAst(ast.Root);
         return macro;
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

      private readonly Parser _parser;

      private Parser CreateParser()
      {
         var ironyGrammar = new IronyLispishGrammar();
         return new Parser(ironyGrammar);
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
   }
}
