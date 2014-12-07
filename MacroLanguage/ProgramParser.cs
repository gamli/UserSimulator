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
         return (Program)_parser.Parse(ProgramText);
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
            .Or.By(Windowshot(Config, statement))
            .Or.By(Move(Config))
            .Or.By(NoOp(Config))
            .Or.By(Pause(Config))
            .Or.By(Position(Config));
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
                     if(O.Items != null)
                        foreach(var bockItem in O.Items)
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
            .Followed.By(IntegerLiteral(Config)).As("RepetitionCount")
            .Followed.By(EndParameterList())
            .Followed.By(Statement).As("Body")
            .WhenFound(O => new ForLoop { RepetitionCount = O.RepetitionCount, Body = O.Body });

         return forLoop;
      }

      private static IRule Windowshot(IFluentParserConfigurator Config, IRule Statement)
      {
         var windowshot = Config.Rule();
         windowshot
            .IsMadeUp.By("IF_WINDOWSHOT")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerLiteral(Config)).As("PositionX")
            .Followed.By(ParameterSeperator())
            .Followed.By(IntegerLiteral(Config)).As("PositionY")
            .Followed.By(ParameterSeperator())
            .Followed.By(StringLiteral(Config)).As("ImageUrl")
            .Followed.By(EndParameterList())
            .Followed.By(Statement).As("Body")
            .WhenFound(O => new Windowshot { PositionX = O.PositionX, PositionY = O.PositionY, ImageUrl = O.ImageUrl, Body = O.Body });

         return windowshot;
      }

      private static IRule Move(IFluentParserConfigurator Config)
      {
         var move = Config.Rule();
         move
            .IsMadeUp.By("MOVE")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerLiteral(Config)).As("TranslationX")
            .Followed.By(ParameterSeperator())
            .Followed.By(IntegerLiteral(Config)).As("TranslationY")
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
            .Followed.By(IntegerLiteral(Config)).As("Duration")
            .Followed.By(EndParameterList())
            .Followed.By(StatementEnd())
            .WhenFound(O => new Pause { Duration = TimeSpan.FromMilliseconds(O.Duration) });

         return pause;
      }

      private static IRule Position(IFluentParserConfigurator Config)
      {
         var position = Config.Rule();
         position
            .IsMadeUp.By("POSITION")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerLiteral(Config)).As("X")
            .Followed.By(ParameterSeperator())
            .Followed.By(IntegerLiteral(Config)).As("Y")
            .Followed.By(EndParameterList())
            .Followed.By(StatementEnd())
            .WhenFound(O => new Position { X = O.X, Y = O.Y });

         return position;
      }

      private static IRule IntegerLiteral(IFluentParserConfigurator Config)
      {
         var integerConstant = Config.Rule();
         integerConstant
            .IsMadeUp.By<int>().As("Value")
            .WhenFound(O => O.Value)
            .Or.By("-").Followed.By<int>().As("Value")
            .WhenFound(O => -O.Value);
         return integerConstant;
      }

      private static IRule StringLiteral(IFluentParserConfigurator Config)
      {
         var stringConstant = Config.Rule();
         stringConstant
            .IsMadeUp.By(Config.QuotedString).As("Value")
            .WhenFound(O => O.Value)
            .Or.By("null")
            .WhenFound(O => null);
         return stringConstant;
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
}
