using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Macro;
using Piglet.Lexer;
using Piglet.Parser;
using Piglet.Parser.Configuration;
using Piglet.Parser.Construction;

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
         try
         {
            var macro = _parser.Parse(Text);
            return (MacroBase)macro;
         }
         catch (Piglet.Parser.ParseException e)
         {
            _parser = CreateParser();
            throw new ParseException(e.Message, e) { LineNumber = e.LexerState.CurrentLineNumber };
         }
      }

      private IParser<object> _parser;

      private static IParser<object> CreateParser()
      {
         var config = ParserConfigurator();

         var expression = config.CreateNonTerminal();
         expression.DebugName = "expr";

         expression.AddProduction(ConstantNull(config)).SetReduceToFirst();

         expression.AddProduction(ConstantBoolean(config)).SetReduceToFirst();

         expression.AddProduction(ConstantString(config)).SetReduceToFirst();

         expression.AddProduction(ConstantInteger(config)).SetReduceToFirst();

         expression.AddProduction(ConstantDouble(config)).SetReduceToFirst();

         expression.AddProduction(Symbol(config)).SetReduceToFirst();

         expression.AddProduction(List(config, expression)).SetReduceToFirst();

         expression.AddProduction(QuoteSyntacticSugar(config, expression)).SetReduceToFirst();

         return config.CreateParser();
      }

      private static IParserConfigurator<object> ParserConfigurator()
      {
         var config = ParserFactory.Configure<object>();
         return config;
      }

      private static ITerminal<object> ConstantNull(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal("null", ParseResults => new Constant(null), true);
         constant.DebugName = "const null-expr";
         return constant;
      }

      private static ITerminal<object> ConstantBoolean(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal("(True|False)", ParseResults => new Constant(bool.Parse(ParseResults)), true);
         constant.DebugName = "const bool-expr";
         return constant;
      }

      private static ITerminal<object> ConstantString(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal("\"([^\"]|\\\\\")*\"", ReduceConstantString, true);
         constant.DebugName = "const string-expr";
         return constant;
      }

      private static Constant ReduceConstantString(string ParseResults)
      {
         return new Constant(ParseResults.Substring(1, ParseResults.Length - 2).Replace("\\\"", "\""));
      }

      private static ITerminal<object> ConstantInteger(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal(@"-?\d+", ParseResults => new Constant(int.Parse(ParseResults)), true);
         constant.DebugName = "const int-expr";
         return constant;
      }

      private static ITerminal<object> ConstantDouble(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal(@"-?\d*\.\d+", ParseResults => new Constant(double.Parse(ParseResults, CultureInfo.InvariantCulture)), true);
         constant.DebugName = "const double-expr";
         return constant;
      }

      private static ITerminal<object> Symbol(IParserConfigurator<object> Config)
      {
         var symbol = Config.CreateTerminal(@"[^'\(\)\s]+", ParseResults => new Symbol(ParseResults));
         symbol.DebugName = "smybol";
         return symbol;
      }

      private static INonTerminal<object> List(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         var list = Config.CreateNonTerminal();
         list
            .AddProduction("nil")
            .SetReduceFunction(ParseResults => new List());

         var expressionSequence = Config.CreateNonTerminal();
         expressionSequence.DebugName = "expression-sequence";
         expressionSequence.
            AddProduction(expressionSequence, Expression)
            .SetReduceFunction(
               ParseResults =>
                  {
                     ((List)ParseResults[0]).Expressions.Add((Expression)ParseResults[1]);
                     return ParseResults[0];
                  });
         expressionSequence
            .AddProduction()
            .SetReduceFunction(ParseResults => new List());

         list
            .AddProduction("(", expressionSequence, ")")
            .SetReduceFunction(ParseResults => ParseResults[1]);

         return list;
      }

      private static INonTerminal<object> QuoteSyntacticSugar(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         var quoteSyntacticSugar = Config.CreateNonTerminal();
         quoteSyntacticSugar
            .AddProduction("'", Expression)
            .SetReduceFunction(ParseResults => SpecialForms.Quote((Expression) ParseResults[1]));

         return quoteSyntacticSugar;
      }
   }

   public class ParseException : Exception
   {
      public int LineNumber { get; set; }

      public ParseException(string Message, Exception InnerException)
         : base(Message, InnerException)
      {
         // nothing to do
      }
   }
}
