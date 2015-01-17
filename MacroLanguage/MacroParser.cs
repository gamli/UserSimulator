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
         switch (AstNode.Term.Name)
         {
            case "expression":
               return BuildMacroFromAst(AstNode.ChildNodes.Single());
            case "constant":
               switch (AstNode.ChildNodes[0].Token.Text)
               {
                  case "nil":
                     return new List();
                  case "null":
                     return new Constant(null);
                  default:
                     throw new ParseException("Unknown constant  >> " + AstNode.Token.Text + " <<");
               }
            case "boolean":
               return new Constant(AstNode.ChildNodes.Single().Token.Text == "true");
            case "string":
            case "number":
               return new Constant(AstNode.Token.Value);
            case "list":
               return new List(AstNode.ChildNodes[1].ChildNodes.Select(BuildMacroFromAst).Cast<Expression>().ToArray());
            case "symbol":
               return new Symbol(AstNode.Token.Text);
            case "quoted-expression-alias":
               return SpecialForms.Quote((Expression) BuildMacroFromAst(AstNode.ChildNodes[1]));
            default:
               throw new ParseException("Unknown construct in ast node >> " + AstNode + " <<");
         }
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
