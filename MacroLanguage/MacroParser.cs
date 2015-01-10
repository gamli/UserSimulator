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
            var program = _parser.Parse(Text);
            return (MacroBase)program;
         }
         catch(Piglet.Parser.ParseException e)
         {
            _parser = CreateParser();
            throw new ParseException(e.Message, e) { LineNumber = e.LexerState.CurrentLineNumber };
         }
         catch (LexerException e)
         {
            _parser = CreateParser();
            throw new ParseException(e.Message, e) { LineNumber = e.LineNumber};
         }
      }

      private IParser<object> _parser;

      private static IParser<object> CreateParser()
      {
         var config = ParserConfigurator();

         var expression = config.CreateNonTerminal();
         expression.DebugName = "expr";

         ReduceToSelf(expression.AddProduction(ConstantBoolean(config)));

         ReduceToSelf(expression.AddProduction(ConstantString(config)));

         ReduceToSelf(expression.AddProduction(ConstantInteger(config)));

         ReduceToSelf(expression.AddProduction(ConstantDouble(config)));

         var symbol = Symbol(config);
         ReduceToSelf(expression.AddProduction(symbol));

         ReduceToSelf(expression.AddProduction(Definition(config, symbol, expression)));

         ReduceToSelf(expression.AddProduction(If(config, expression)));

         var symbolList = SymbolList(config, symbol);

         ReduceToSelf(expression.AddProduction(Lambda(config, expression, symbolList)));

         ReduceToSelf(expression.AddProduction(Loop(config, expression)));

         var expressions = config.CreateNonTerminal();
         expressions.
            AddProduction(expressions, expression)
            .SetReduceFunction(ParseResults => ((IEnumerable<object>)ParseResults[0]).Concat(new[] { ParseResults[1] }));
         expressions.
            AddProduction()
            .SetReduceFunction(ParseResults => new List<object>());

         ReduceToSelf(expression.AddProduction(ProcedureCall(config, expression, expressions)));

         ReduceToSelf(expression.AddProduction(Quote(config, expression)));

         return config.CreateParser();
      }

      private class PrecedenceGroup : IPrecedenceGroup
      {

         public AssociativityDirection Associativity { get; private set; }

         public int Precedence { get; private set; }

         public PrecedenceGroup(int Precedence, AssociativityDirection Associativity = AssociativityDirection.Left)
         {
            this.Precedence = Precedence;
            this.Associativity = Associativity;
         }
      }

      private static IParserConfigurator<object> ParserConfigurator()
      {
         var config = ParserFactory.Configure<object>();
         return config;
      }

      private static ITerminal<object> ConstantBoolean(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal("(True|False)", ReduceConstantBoolean, true);
         constant.DebugName = "const bool-expr";
         return constant;
      }

      private static Constant ReduceConstantBoolean(string ParseResults)
      {
         return new Constant(bool.Parse(ParseResults));
      }

      private static ITerminal<object> ConstantString(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal("\"([^\"]|\\\\\")*\"|null", ReduceConstantString, true);
         constant.DebugName = "const string-expr";
         return constant;
      }

      private static Constant ReduceConstantString(string ParseResults)
      {
         if (ParseResults.Equals("null"))
            return new Constant(null);
         return new Constant(ParseResults.Substring(1, ParseResults.Length - 2).Replace("\\\"", "\""));
      }

      private static ITerminal<object> ConstantInteger(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal(@"-?\d+", ReduceConstantInteger, true);
         constant.DebugName = "const int-expr";
         return constant;
      }

      private static Constant ReduceConstantInteger(string ParseResults)
      {
         return new Constant(int.Parse(ParseResults));
      }

      private static ITerminal<object> ConstantDouble(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal(@"-?\d*\.\d+", ReduceConstantDouble, true);
         constant.DebugName = "const double-expr";
         return constant;
      }

      private static Constant ReduceConstantDouble(string ParseResults)
      {
         return new Constant(double.Parse(ParseResults, CultureInfo.InvariantCulture));
      }

      private static INonTerminal<object> Definition(IParserConfigurator<object> Config, ITerminal<object> Symbol, INonTerminal<object> Expression)
      {
         return ListGeneric(Config, "define-expr", ReduceDefinition, "define", Symbol, Expression);
      }

      private static Definition ReduceDefinition(object[] ParseResults)
      {
         return new Definition { Symbol = (Symbol)ParseResults[2], Expression = (ExpressionBase)ParseResults[3] };
      }

      private static INonTerminal<object> If(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         return ListGeneric(Config, "if-expr", ReduceIf, "if", Expression, Expression, Expression);
      }

      private static If ReduceIf(object[] ParseResults)
      {
         var ifExpression = 
            new If 
               { 
                  Condition = (ExpressionBase)ParseResults[2], 
                  Consequent = (ExpressionBase)ParseResults[3], 
                  Alternative = (ExpressionBase)ParseResults[4] 
               };
         return ifExpression;
      }
      private static INonTerminal<object> Lambda(IParserConfigurator<object> Config, INonTerminal<object> Expression, INonTerminal<object> SymbolList)
      {
         return ListGeneric(Config, "lambda-expr", ReduceLambda, "lambda", SymbolList, Expression);
      }

      private static Lambda ReduceLambda(object[] ParseResults)
      {
         var lambda =
            new Lambda
               {
                  ArgumentSymbols = (SymbolList) ParseResults[2],
                  Body = (ExpressionBase)ParseResults[3]
               };
         return lambda;
      }

      private static INonTerminal<object> Loop(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         return ListGeneric(Config, "loop-expr", ReduceLoop, "loop", Expression, Expression);
      }

      private static Loop ReduceLoop(object[] ParseResults)
      {
         return new Loop { Condition = (ExpressionBase)ParseResults[2], Body = (ExpressionBase)ParseResults[3] };
      }

      private static INonTerminal<object> ProcedureCall(IParserConfigurator<object> Config, INonTerminal<object> Expression, INonTerminal<object> Expressions)
      {
         return ListGeneric(Config, "function-call-expr", ReduceProcedureCall, Expression, Expressions);
      }

      private static ProcedureCall ReduceProcedureCall(object[] ParseResults)
      {
         var procedureCall = new ProcedureCall { Procedure = (ExpressionBase)ParseResults[1] };
         foreach (var arg in (IEnumerable<object>)ParseResults[2])
            procedureCall.Expressions.Add((ExpressionBase)arg);
         return procedureCall;
      }

      private static INonTerminal<object> Quote(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         return ListGeneric(Config, "quote-expr", ReduceQuote, "quote", Expression);
      }

      private static Quote ReduceQuote(object[] ParseResults)
      {
         return new Quote { Expression = (ExpressionBase)ParseResults[2] };
      }

      private static ITerminal<object> Symbol(IParserConfigurator<object> Config)
      {
         var symbol = Config.CreateTerminal(@"[a-zA-Z]([a-zA-Z0-9])*", ReduceSymbol);
         symbol.DebugName = "smybol";
         return symbol;
      }

      private static Symbol ReduceSymbol(string ParseResults)
      {
         return new Symbol(ParseResults);
      }

      private static INonTerminal<object> SymbolList(IParserConfigurator<object> Config, ITerminal<object> Symbol)
      {
         var symbols = Config.CreateNonTerminal();
         symbols.
            AddProduction(symbols, Symbol)
            .SetReduceFunction(ParseResults => ((IEnumerable<object>)ParseResults[0]).Concat(new[] { ParseResults[1] }));
         symbols.
            AddProduction()
            .SetReduceFunction(ParseResults => new List<object>());

         return ListGeneric(Config, "symbol-list", ReduceSymbolList, symbols);
      }

      private static SymbolList ReduceSymbolList(object[] ParseResults)
      {
         var symbolList = new SymbolList();
         foreach (Symbol symbol in (IEnumerable)ParseResults[1])
            symbolList.Symbols.Add(symbol);
         return symbolList;
      }

      private static INonTerminal<object> ListGeneric(
         IParserConfigurator<object> Config,
         string DebugName,
         Func<object[], object> ReduceFunction,
         params object[] ListElements)
      {
         var list = Config.CreateNonTerminal();
         list.DebugName = DebugName;
         var production = new object[ListElements.Length + 2];
         production[0] = "(";
         Array.Copy(ListElements, 0, production, 1, ListElements.Length);
         production[production.Length - 1] = ")";
         list
            .AddProduction(production)
            .SetReduceFunction(ReduceFunction);

         return list;
      }

      private static void ReduceToSelf(IProduction<object> Production)
      {
         Production.SetReduceFunction(ReduceToSelf);
      }

      private static object ReduceToSelf(object[] ParseResults)
      {
         return ParseResults[0];
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
