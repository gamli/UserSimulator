﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Irony;
using Irony.Parsing;
using Irony.Parsing.Construction;

namespace MacroLanguage
{
   class IronyLispishGrammar : Grammar
   {
      public IronyLispishGrammar()
      {
         var expression = new NonTerminal("expression");

         var constant = new NonTerminal("constant") { Rule = ToTerm("nil") | ToTerm("null") };

         var booleanConstant = new NonTerminal("boolean") { Rule = ToTerm("true", "true") | ToTerm("false", "false") };

         var stringConstant = TerminalFactory.CreateCSharpString("string");

         var numberConstant = TerminalFactory.CreateCSharpNumber("number");
         numberConstant.Options |= NumberOptions.AllowSign;

         var sequence = new NonTerminal("sequence");
         sequence.Rule = MakeStarRule(sequence, expression);
         var list = new NonTerminal("list") { Rule = ToTerm("(") + sequence + ToTerm(")") };

         const string EXTRA_SYMBOL_CHARACTERS = "._+*/=<>:?!";
         var symbol = new IdentifierTerminal("symbol", EXTRA_SYMBOL_CHARACTERS + Strings.DecimalDigits + "-", EXTRA_SYMBOL_CHARACTERS);

         var quotedExpressionAlias = new NonTerminal("quoted-expression-alias", (Sender, Args) => Console.Write("oO")) { Rule = ToTerm("'") + expression };

         expression.Rule = constant | booleanConstant | stringConstant | numberConstant | list | symbol | quotedExpressionAlias;

         Root = expression;
      }
   }
}