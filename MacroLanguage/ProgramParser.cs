using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return (Program)_parser.Parse(ProgramText);
         }
         catch(Piglet.Parser.ParseException E)
         {
            _parser = CreateParser();
            throw new ParseException(E.Message, E) { LineNumber = E.LexerState.CurrentLineNumber };
         }
      }

      private IParser<object> _parser;

      private static IParser<object> CreateParser()
      {
         var config = ParserConfigurator();

         config.Rule()
            .IsMadeUp.By("PROGRAM")
            .Followed.By(Statement(config)).As("Body")
            .WhenFound(O => new Program { Body = O.Body });

         return config.CreateParser();
      }

      private static IRule Statement(IFluentParserConfigurator Config)
      {
         var statement = Config.Rule();
         statement
            .IsMadeUp.By(Block(Config, statement))
            .Or.By(LeftClick(Config))
            .Or.By(ForLoop(Config, statement))
            .Or.By(Move(Config))
            .Or.By(NoOp(Config))
            .Or.By(Pause(Config))
            .Or.By(Position(Config))
            .Or.By(IfStatement(Config, statement));
         return statement;
      }

      private static IRule Block(IFluentParserConfigurator Config, IRule Statement)
      {

         var block = Config.Rule();
         block
            .IsMadeUp.By("{")
            .Followed.ByListOf(Statement).As("Items").ThatIs.Optional
            .Followed.By("}")
            .WhenFound(
               O =>
               {
                  var bock = new Block();
                  if (O.Items != null)
                     foreach (var bockItem in O.Items)
                        bock.Items.Add(bockItem);
                  return bock;
               });

         return block;
      }

      private static IRule LeftClick(IFluentParserConfigurator Config)
      {
         var leftClick = Config.Rule();
         leftClick
            .IsMadeUp.By("LEFT_CLICK")
            .Followed.By(EmptyParameterList())
            .Followed.By(StatementEnd())
            .WhenFound(O => new LeftClick());

         return leftClick;
      }

      private static IRule ForLoop(IFluentParserConfigurator Config, IRule Statement)
      {
         var forLoop = Config.Rule();
         forLoop
            .IsMadeUp.By("FOR")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerExpression(Config)).As("RepetitionCount")
            .Followed.By(EndParameterList())
            .Followed.By(Statement).As("Body")
            .WhenFound(O => new ForLoop { RepetitionCount = O.RepetitionCount, Body = O.Body });

         return forLoop;
      }

      private static IRule Move(IFluentParserConfigurator Config)
      {
         var move = Config.Rule();
         move
            .IsMadeUp.By("MOVE")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerExpression(Config)).As("TranslationX")
            .Followed.By(ParameterSeperator())
            .Followed.By(IntegerExpression(Config)).As("TranslationY")
            .Followed.By(EndParameterList())
            .Followed.By(StatementEnd())
            .WhenFound(O => new Move { TranslationX = O.TranslationX, TranslationY = O.TranslationY });

         return move;
      }

      private static IRule NoOp(IFluentParserConfigurator Config)
      {
         var noOp = Config.Rule();
         noOp
            .IsMadeUp.By(StatementEnd())
            .WhenFound(O => new NoOp());

         return noOp;
      }

      private static IRule Pause(IFluentParserConfigurator Config)
      {
         var pause = Config.Rule();
         pause
            .IsMadeUp.By("PAUSE")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerExpression(Config)).As("Duration")
            .Followed.By(EndParameterList())
            .Followed.By(StatementEnd())
            .WhenFound(O => new Pause { Duration = O.Duration });

         return pause;
      }

      private static IRule Position(IFluentParserConfigurator Config)
      {
         var position = Config.Rule();
         position
            .IsMadeUp.By("POSITION")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerExpression(Config)).As("X")
            .Followed.By(ParameterSeperator())
            .Followed.By(IntegerExpression(Config)).As("Y")
            .Followed.By(EndParameterList())
            .Followed.By(StatementEnd())
            .WhenFound(O => new Position { X = O.X, Y = O.Y });

         return position;
      }

      /*private static IRule Expression(IFluentParserConfigurator Config)
      {
         var expression = Config.Rule();
         expression
            .IsMadeUp.By(BooleanExpression(Config))
            .Or.By(StringExpression(Config))
            .Or.By(IntegerExpression(Config));

         return expression;
      }*/

      private static IRule WindowshotExpression(IFluentParserConfigurator Config)
      {
         var windowshot = Config.Rule();
         windowshot
            .IsMadeUp.By("WINDOWSHOT")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerExpression(Config)).As("PositionX")
            .Followed.By(ParameterSeperator())
            .Followed.By(IntegerExpression(Config)).As("PositionY")
            .Followed.By(ParameterSeperator())
            .Followed.By(StringExpression(Config)).As("ImageUrl")
            .Followed.By(EndParameterList())
            .WhenFound(O => new WindowshotExpression { PositionX = O.PositionX, PositionY = O.PositionY, ImageUrl = O.ImageUrl });
         return windowshot;
      }
      private static IRule BooleanExpression(IFluentParserConfigurator Config)
      {
         var booleanExpression = Config.Rule();
         booleanExpression
            .IsMadeUp.By(ConstantBooleanExpression(Config))
            .Or.By(WindowshotExpression(Config));
         return booleanExpression;
      }
      private static IRule ConstantBooleanExpression(IFluentParserConfigurator Config)
      {
         var boolExpression = Config.Rule();
         boolExpression
            .IsMadeUp.By("True").WhenFound(_ => new ConstantExpression<bool> { Value = true })
            .Or.By("False").WhenFound(_ => new ConstantExpression<bool> { Value = false });
         return boolExpression;
      }

      private static IRule StringExpression(IFluentParserConfigurator Config)
      {
         var stringExpression = Config.Rule();
         stringExpression
            .IsMadeUp.By(ConstantStringExpression(Config));
         return stringExpression;
      }
      private static IRule ConstantStringExpression(IFluentParserConfigurator Config)
      {
         var stringConstant = Config.Rule();
         stringConstant
            .IsMadeUp.By(Config.QuotedString).As("Value")
            .WhenFound(O => new ConstantExpression<string> { Value = O.Value })
            .Or.By("null")
            .WhenFound(O => new ConstantExpression<string> { Value = null });
         return stringConstant;
      }

      private static IRule IntegerExpression(IFluentParserConfigurator Config)
      {
         var integerExpression = Config.Rule();
         integerExpression
            .IsMadeUp.By(ConstantIntegerExpression(Config));
         return integerExpression;
      }
      private static IRule ConstantIntegerExpression(IFluentParserConfigurator Config)
      {
         var integerConstant = Config.Rule();
         integerConstant
            .IsMadeUp.By<int>().As("Value")
            .WhenFound(O => new ConstantExpression<int> { Value = O.Value })
            .Or.By("-").Followed.By<int>().As("Value")
            .WhenFound(O => new ConstantExpression<int> { Value = -O.Value });
         return integerConstant;
      }

      private static IRule IfStatement(IFluentParserConfigurator Config, IRule Statement)
      {
         var ifStatement = Config.Rule();
         ifStatement
            .IsMadeUp.By("IF")
            .Followed.By(BeginParameterList())
            .Followed.By(BooleanExpression(Config)).As("Expression")
            .Followed.By(EndParameterList())
            .Followed.By(Statement).As("Body")
            .WhenFound(O =>
               new IfStatement { Expression = O.Expression, Body = O.Body });
         return ifStatement;
      }

      private static IFluentParserConfigurator ParserConfigurator()
      {
         return ParserFactory.Fluent();
      }

      private static string StatementEnd()
      {
         return ";";
      }

      private static string EmptyParameterList()
      {
         return BeginParameterList() + EndParameterList();
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
