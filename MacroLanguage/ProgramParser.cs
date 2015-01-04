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

namespace MacroLanguage
{
   public class ProgramParser
   {
      public ProgramParser()
      {
         _parser = CreateParser();
      }

      public Program Parse(string ProgramText)
      {
         try
         {
            var program = _parser.Parse(ProgramText); 
            return (Program)program;
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

      private static IParser<object> CreateParser()
      {
         var config = ParserConfigurator();


         var program = config.CreateNonTerminal();
         program.DebugName = "PROGRAM-Stmt";
         var statement = config.CreateNonTerminal();
         statement.DebugName = "stmt";
         var expression = config.CreateNonTerminal();
         expression.DebugName = "expr";

         program
            .AddProduction("PROGRAM", statement)
            .SetReduceFunction(ReduceProgram);
         ConfigureStatement(config, expression, statement);   
         ConfigureExpression(config, expression, statement);

         return config.CreateParser();
      }

      private static Program ReduceProgram(object[] ParseResults)
      {
         return new Program { Body = (StatementBase)ParseResults[1] };
      }

      private static void ConfigureExpression(IParserConfigurator<object> Config, INonTerminal<object> Expression, INonTerminal<object> Statement)
      {
         ReduceToSelf(Expression.AddProduction(ConstantBoolean(Config)));
         ReduceToSelf(Expression.AddProduction(ConstantString(Config)));
         ReduceToSelf(Expression.AddProduction(ConstantInteger(Config)));
         ReduceToSelf(Expression.AddProduction(ConstantDouble(Config)));
         ReduceToSelf(Expression.AddProduction(Windowshot(Config, Expression)));
         //Expression.AddProduction(Statement);
      }

      private static void ConfigureStatement(IParserConfigurator<object> Config, INonTerminal<object> Expression, INonTerminal<object> Statement)
      {

         ReduceToSelf(Statement.AddProduction(Windowshot(Config, Expression)));

         var variableSymbol = VariableSymbol(Config);
         ReduceToSelf(Statement.AddProduction(Block(Config, Statement)));
         ReduceToSelf(Statement.AddProduction(LeftClick(Config)));
         ReduceToSelf(Statement.AddProduction(ForLoop(Config, Expression, Statement)));
         ReduceToSelf(Statement.AddProduction(Move(Config, Expression)));
         ReduceToSelf(Statement.AddProduction(Pause(Config, Expression)));
         ReduceToSelf(Statement.AddProduction(Position(Config, Expression)));
         ReduceToSelf(Statement.AddProduction(If(Config, Expression, Statement)));
         ReduceToSelf(Statement.AddProduction(VariableAssignment(Config, Expression, variableSymbol)));
      }

      private static INonTerminal<object> Block(IParserConfigurator<object> Config, INonTerminal<object> Statement)
      {
         var statements = Config.CreateNonTerminal();
         statements.DebugName = "block-stmts";
         statements
            .AddProduction(statements, Statement)
            .SetReduceFunction(ReduceBlockStatementList);
         statements
            .AddProduction()
            .SetReduceFunction(ReduceEmptyBlockStatementList);

         var block = Config.CreateNonTerminal();
         block.DebugName = "block-stmt";
         block
            .AddProduction(BlockBegin(), statements, BlockEnd())
            .SetReduceFunction(ReduceBlock);

         return block;
      }

      private static string BlockBegin()
      {
         return "{";
      }

      private static string BlockEnd()
      {
         return "}";
      }

      private static object ReduceBlock(object[] ParseResults)
      {
         var block = new Block();
         foreach (var bockItem in (IEnumerable<StatementBase>)ParseResults[1])
            block.Items.Add(bockItem);
         return block;
      }

      private static object ReduceEmptyBlockStatementList(object[] ParseResults)
      {
         return new List<StatementBase>();
      }

      private static object ReduceBlockStatementList(object[] ParseResults)
      {
         return ((IEnumerable<StatementBase>)ParseResults[0]).Concat(new[] { (StatementBase)ParseResults[1] });
      }

      private static INonTerminal<object> LeftClick(IParserConfigurator<object> Config)
      {
         var leftClick = Config.CreateNonTerminal();
         leftClick.DebugName = "LEFT_CLICK-stmt";
         leftClick
            .AddProduction("LEFT_CLICK", BeginParameterList(), EndParameterList())
            .SetReduceFunction(ReduceLeftClick);

         return leftClick;
      }

      private static LeftClick ReduceLeftClick(object[] ParseResults)
      {
         return new LeftClick();
      }

      private static INonTerminal<object> ForLoop(IParserConfigurator<object> Config, INonTerminal<object> Expression, INonTerminal<object> Statement)
      {
         var forLoop = Config.CreateNonTerminal();
         forLoop.DebugName = "FOR-stmt";
         forLoop
            .AddProduction("FOR", BeginParameterList(), Expression, EndParameterList(), Statement)
            .SetReduceFunction(ReduceForLoop);

         return forLoop;
      }

      private static Loop ReduceForLoop(object[] ParseResults)
      {
         return new Loop { Body = (ExpressionBase)ParseResults[2], Body = (StatementBase)ParseResults[4] };
      }

      private static INonTerminal<object> Move(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         var move = Config.CreateNonTerminal();
         move.DebugName = "MOVE-stmt";
         move
            .AddProduction("MOVE", BeginParameterList(), Expression, ParameterSeperator(), Expression, EndParameterList())
            .SetReduceFunction(ReduceMove);

         return move;
      }

      private static Move ReduceMove(object[] ParseResults)
      {
         return new Move { TranslationX = (ExpressionBase)ParseResults[2], TranslationY = (ExpressionBase)ParseResults[4] };
      }

      private static INonTerminal<object> Pause(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         var pause = Config.CreateNonTerminal();
         pause.DebugName = "PAUSE-stmt";
         pause
            .AddProduction("PAUSE", BeginParameterList(), Expression, EndParameterList())
            .SetReduceFunction(ReducePause);

         return pause;
      }

      private static Pause ReducePause(object[] ParseResults)
      {
         return new Pause { Duration = (ExpressionBase)ParseResults[2] };
      }

      private static INonTerminal<object> Position(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         var position = Config.CreateNonTerminal();
         position.DebugName = "POSITION-stmt";
         position
            .AddProduction("POSITION", BeginParameterList(), Expression, ParameterSeperator(), Expression, EndParameterList())
            .SetReduceFunction(ReducePosition);

         return position;
      }

      private static Position ReducePosition(object[] ParseResults)
      {
         return new Position { X = (ExpressionBase)ParseResults[2], Y = (ExpressionBase)ParseResults[4] };
      }

      private static INonTerminal<object> VariableAssignment(IParserConfigurator<object> Config, INonTerminal<object> Expression, ITerminal<object> VariableSymbol)
      {
         var variableAssignment = Config.CreateNonTerminal();
         variableAssignment.DebugName = "variable assignment-stmt";
         variableAssignment
            .AddProduction(VariableSymbol, AssignmentSymbol(), Expression)
            .SetReduceFunction(ReduceVariableAssignment);

         return variableAssignment;
      }

      private static Definition ReduceVariableAssignment(object[] ParseResults)
      {
         return new Definition { Symbol = (string)ParseResults[0], Expression = (ExpressionBase)ParseResults[2] };
      }

      private static string AssignmentSymbol()
      {
         return "=";
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

      private static INonTerminal<object> Windowshot(IParserConfigurator<object> Config, INonTerminal<object> Expression)
      {
         var windowshot = Config.CreateNonTerminal();
         windowshot.DebugName = "WINDOWSHOT-expr";
         windowshot
            .AddProduction("WINDOWSHOT",
               BeginParameterList(), 
               Expression, ParameterSeperator(), 
               Expression, ParameterSeperator(), 
               Expression, 
               EndParameterList())
            .SetReduceFunction(ReduceWindowshot);


         return windowshot;
      }

      private static Windowshot ReduceWindowshot(object[] ParseResults)
      {
         return 
            new Windowshot
               {
                  PositionX = (ExpressionBase)ParseResults[2],
                  PositionY = (ExpressionBase)ParseResults[4],
                  ImageUrl = (ExpressionBase)ParseResults[6]
               };
      }

      private static ITerminal<object> VariableSymbol(IParserConfigurator<object> Config)
      {
         var constant = Config.CreateTerminal(@"[a-zA-Z][a-zA-Z0-9]*", ReduceVariableSymbol);
         constant.DebugName = "variable-smbl";
         return constant;
      }

      private static string ReduceVariableSymbol(string ParseResults)
      {
         return ParseResults;
      }

      private static INonTerminal<object> If(IParserConfigurator<object> Config, INonTerminal<object> Expression, INonTerminal<object> Statement)
      {
         var ifStatement = Config.CreateNonTerminal();
         ifStatement.DebugName = "if-stmt";
         ifStatement
            .AddProduction("IF", BeginParameterList(), Expression, EndParameterList(), Statement)
            .SetReduceFunction(ReduceIf);

         return ifStatement;
      }

      private static If ReduceIf(object[] ParseResults)
      {
         return new If { Condition = (ExpressionBase)ParseResults[2], Alternative = (StatementBase)ParseResults[4] };
      }

      private static void ReduceToSelf(IProduction<object> Production)
      {
         Production.SetReduceFunction(ReduceToSelf);
      }

      private static object ReduceToSelf(object[] ParseResults)
      {
         return ParseResults[0];
      }

      private static IParserConfigurator<object> ParserConfigurator()
      {
         return ParserFactory.Configure<object>();
      }

      private static string BeginParameterList()
      {
         return "(";
      }

      private static string ParameterSeperator()
      {
         return ",";
      }

      private static string EndParameterList()
      {
         return ")";
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
