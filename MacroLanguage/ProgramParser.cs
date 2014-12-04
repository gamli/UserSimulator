using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;
using Piglet.Parser;
using Piglet.Parser.Configuration;
using Piglet.Parser.Configuration.Fluent;

namespace UserSimulator
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
            .Followed.By(Block(config)).As("Block")
            .WhenFound(O => new Program { Block = O.Block });

         return config.CreateParser();
      }

      private static IRule Block(IFluentParserConfigurator Config)
      {
         var item = Config.Rule();
         item
            .IsMadeUp.By(LeftClick(Config));
         throw new NotImplementedException();

         var block = Config.Rule();
         block
            .IsMadeUp.By("{")
            .Followed.ByListOf(item).As("Items")
            .Followed.By("}")
            .WhenFound(
               O => 
                  {
                     var bock = new Block();
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
            .Followed.By(BeginParameterList())
            .Followed.By(EndParameterList())
            .Followed.By(StatementEnd())
            .WhenFound(O => new LeftClick());

         return leftClick;
      }

      private static IRule ForLoop(IFluentParserConfigurator Config)
      {
         var forLoop = Config.Rule();
         forLoop
            .IsMadeUp.By("FOR")
            .Followed.By("(")
            .Followed.By(IntegerConstant(Config)).As("RepetitionCount")
            .Followed.By(")")
            .Followed.By(Block(Config)).As("Body")
            .WhenFound(O => new ForLoop { RepetitionCount = O.RepetitionCount, Body = O.Body });

         return forLoop;
      }

      private static IRule ImageEqualsWindowContent(IFluentParserConfigurator Config)
      {
         throw new NotImplementedException();
      }

      private static IRule Move(IFluentParserConfigurator Config)
      {
         var move = Config.Rule();
         move
            .IsMadeUp.By("MOVE")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerConstant(Config)).As("TranslationX")
            .Followed.By(ParameterSeperator())
            .Followed.By(IntegerConstant(Config)).As("TranslationY")
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
            .IsMadeUp.By("LEFT_CLICK")
            .Followed.By(BeginParameterList())
            .Followed.By(IntegerConstant(Config)).As("Duration")
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
            .Followed.By(IntegerConstant(Config)).As("X")
            .Followed.By(ParameterSeperator())
            .Followed.By(IntegerConstant(Config)).As("Y")
            .Followed.By(EndParameterList())
            .Followed.By(StatementEnd())
            .WhenFound(O => new Position { X = O.X, Y = O.Y });

         return position;
      }

      private static IRule IntegerConstant(IFluentParserConfigurator Config)
      {
         var integerConstant = Config.Rule();
         integerConstant
            .IsMadeUp.By<int>().As("Value")
            .WhenFound(O => O.Value);
         return integerConstant;
      }

      private static IFluentParserConfigurator ParserConfigurator()
      {
         return ParserFactory.Fluent();
      }

      private static string StatementEnd()
      {
         return ";";
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
