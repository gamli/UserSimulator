using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Macro;
using Piglet.Parser;
using Piglet.Parser.Configuration;
using Piglet.Parser.Configuration.Fluent;
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
         catch(Piglet.Parser.ParseException E)
         {
            _parser = CreateParser();
            throw new ParseException(E.Message, E) { LineNumber = E.LexerState.CurrentLineNumber };
         }
         catch (Piglet.Lexer.LexerException E)
         {
            _parser = CreateParser();
            throw new ParseException(E.Message, E) { LineNumber = E.LineNumber};
         }
      }

      private IParser<object> _parser;

      private static PrecedenceGroup CONSTANT_PRECEDENCE = new PrecedenceGroup(1000);
      private static PrecedenceGroup DEFINITION_PRECEDENCE = new PrecedenceGroup(900);
      private static PrecedenceGroup FUNCTION_CALL_PRECEDENCE = new PrecedenceGroup(500);
      private static PrecedenceGroup IF_PRECEDENCE = new PrecedenceGroup(900);
      private static PrecedenceGroup IF_WITH_ALTERNATIVE_PRECEDENCE = new PrecedenceGroup(100);
      private static PrecedenceGroup IF_WITHOUT_PRECEDENCE = new PrecedenceGroup(200);
      private static PrecedenceGroup LIST_PRECEDENCE = new PrecedenceGroup(100);
      private static PrecedenceGroup LOOP_PRECEDENCE = new PrecedenceGroup(900);
      private static PrecedenceGroup QUOTE_PRECEDENCE = new PrecedenceGroup(900);
      private static PrecedenceGroup SYMBOL_PRECEDENCE = new PrecedenceGroup(800);

      private static IParser<object> CreateParser()
      {
         var config = ParserConfigurator();

         var expression = config.CreateNonTerminal();
         expression.DebugName = "expr";

         var constantBoolean = expression.AddProduction(ConstantBoolean(config));
         constantBoolean.SetPrecedence(CONSTANT_PRECEDENCE);
         ReduceToSelf(constantBoolean);

         var constantString = expression.AddProduction(ConstantString(config));
         constantString.SetPrecedence(CONSTANT_PRECEDENCE);
         ReduceToSelf(constantString);

         var constantInteger = expression.AddProduction(ConstantInteger(config));
         constantInteger.SetPrecedence(CONSTANT_PRECEDENCE);
         ReduceToSelf(constantInteger);

         var constantDouble = expression.AddProduction(ConstantDouble(config));
         constantDouble.SetPrecedence(CONSTANT_PRECEDENCE);
         ReduceToSelf(constantDouble);

         var symbolNonTerminal = Symbol(config);
         var symbol = expression.AddProduction(symbolNonTerminal);
         symbol.SetPrecedence(SYMBOL_PRECEDENCE);
         ReduceToSelf(symbol);

         var definition = expression.AddProduction(Definition(config, symbolNonTerminal, expression));
         definition.SetPrecedence(DEFINITION_PRECEDENCE);
         ReduceToSelf(definition);

         var expressions = config.CreateNonTerminal();
         expressions.
            AddProduction(expressions, expression)
            .SetReduceFunction(ParseResults => ((IEnumerable<object>)ParseResults[0]).Concat(new[] { ParseResults[1] }));
         expressions.
            AddProduction()
            .SetReduceFunction(ParseResults => new List<object>());

         var functionCall = expression.AddProduction(FunctionCall(config, expression, expressions));
         functionCall.SetPrecedence(FUNCTION_CALL_PRECEDENCE);
         ReduceToSelf(functionCall);

         var ifExpression = expression.AddProduction(If(config, expression));
         ifExpression.SetPrecedence(IF_PRECEDENCE);
         ReduceToSelf(ifExpression);

         var loop = expression.AddProduction(Loop(config, expression));
         loop.SetPrecedence(LOOP_PRECEDENCE);
         ReduceToSelf(loop);

         var quote = expression.AddProduction(Quote(config, expression));
         quote.SetPrecedence(QUOTE_PRECEDENCE);
         ReduceToSelf(quote);

         return config.CreateParser();
      }

      private class PrecedenceGroup : IPrecedenceGroup
      {

         public AssociativityDirection Associativity { get; set; }

         public int Precedence { get; set; }

         public PrecedenceGroup(int Precedence)
         {
            this.Precedence = Precedence;
         }
      }

      private static IParserConfigurator<object> ParserConfigurator()
      {
         return ParserFactory.Configure<object>();
      }

      private static ITerminal<object> ConstantBoolean(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal("(True|False)", O => ReduceConstantBoolean(O), true);
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

      private static INonTerminal<object> FunctionCall(IParserConfigurator<object> Config, INonTerminal<object> Expression, INonTerminal<object> Expressions)
      {
         return ListGeneric(Config, "function-call-expr", ReduceFunctionCall, Expression, Expressions);
      }

      private static FunctionCall ReduceFunctionCall(object[] ParseResults)
      {
         var functionCall = new FunctionCall { Function = (ExpressionBase)ParseResults[1] };
         foreach (var arg in (IEnumerable<object>)ParseResults[2])
            functionCall.Expressions.Add((ExpressionBase)arg);
         return functionCall;
      }

      private static INonTerminal<object> If(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         var ifExpression = Config.CreateNonTerminal();
         ifExpression.DebugName = "if-expr";
         
         var ifWithAlternative = 
            ifExpression.AddProduction(ListGeneric(Config, "if-with-alternative-expr", ReduceIf, "if", Expression, Expression, Expression));
         ifWithAlternative.SetPrecedence(IF_WITH_ALTERNATIVE_PRECEDENCE);
         ReduceToSelf(ifWithAlternative);

         var ifWithoutAlternative =
            ifExpression.AddProduction(ListGeneric(Config, "if-without-alternative-expr", ReduceIf, "if", Expression, Expression));
         ifWithoutAlternative.SetPrecedence(IF_WITHOUT_PRECEDENCE);
         ReduceToSelf(ifWithoutAlternative);

         return ifExpression;
      }

      private static If ReduceIf(object[] ParseResults)
      {
         var ifExpression = new If { Condition = (ExpressionBase)ParseResults[2], Consequent = (ExpressionBase)ParseResults[3] };
         if(ParseResults.Length > 5)
            ifExpression.Alternative = (ExpressionBase)ParseResults[4];
         return ifExpression;
      }

      private static INonTerminal<object> List(IParserConfigurator<object> Config, INonTerminal<object> Expressions)
      {
         return ListGeneric(Config, "list-expr", ReduceList, Expressions);
      }

      private static List ReduceList(object[] ParseResults)
      {
         var list = new List();
         foreach (var listElement in (IEnumerable<object>)ParseResults[1])
            list.Expressions.Add((ExpressionBase)listElement);
         return list;
      }

      private static INonTerminal<object> Loop(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         return ListGeneric(Config, "loop-expr", ReduceLoop, "loop", Expression, Expression);
      }

      private static Loop ReduceLoop(object[] ParseResults)
      {
         return new Loop { Condition = (ExpressionBase)ParseResults[2], Body = (ExpressionBase)ParseResults[3] };
      }

      private static INonTerminal<object> Quote(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         return ListGeneric(Config, "quote-expr", ReduceQuote, "quote", Expression);
      }

      private static Quote ReduceQuote(object[] ParseResults)
      {
         return new Quote { Expression = (ExpressionBase)ParseResults[2] };
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

      private static ITerminal<object> Symbol(IParserConfigurator<object> Config)
      {
         var symbol = Config.CreateTerminal(@"[a-zA-Z][a-zA-Z0-9]*", ReduceSymbol);
         symbol.DebugName = "smybol";
         return symbol;
      }

      private static Symbol ReduceSymbol(string ParseResults)
      {
         return new Symbol(ParseResults);
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
