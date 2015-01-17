using Irony;
using Irony.Parsing;

namespace MacroLanguage
{
   public class IronyLispishGrammar : Grammar
   {
      public const string EXTRA_SYMBOL_CHARACTERS = "._+-*/=<>:?!" + Strings.DecimalDigits;

      public IronyLispishGrammar()
      {
         var expression = new NonTerminal("expression");

         var constant = new NonTerminal("constant") { Rule = ToTerm("nil") | ToTerm("null") };

         var booleanConstant = new NonTerminal("boolean") { Rule = ToTerm("true", "true") | ToTerm("false", "false") };

         var stringConstant = TerminalFactory.CreateCSharpString("string");

         var numberConstant = TerminalFactory.CreateCSharpNumber("number");
         numberConstant.Options |= NumberOptions.AllowSign;
         numberConstant.Priority = 1; // to solve conflict with symbol

         var sequence = new NonTerminal("sequence");
         sequence.Rule = MakeStarRule(sequence, expression);
         var list = new NonTerminal("list") { Rule = ToTerm("(") + sequence + ToTerm(")") };

         var symbol = new IdentifierTerminal("symbol", EXTRA_SYMBOL_CHARACTERS, EXTRA_SYMBOL_CHARACTERS);

         var quotedExpressionAlias = new NonTerminal("quoted-expression-alias") { Rule = ToTerm("'") + expression };

         expression.Rule = constant | booleanConstant | stringConstant | numberConstant | list | symbol | quotedExpressionAlias;

         Root = expression;
      }
   }
}
