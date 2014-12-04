using System;
using Macro;
using MacroLanguage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroLanguage_TEST
{
   [TestClass]
   public class ProgramParser_TEST
   {
      [TestInitialize]
      public void TEST_INITIALIZE()
      {
         _parser = new ProgramParser();
      }

      private ProgramParser _parser;

      [TestMethod]
      public void Block_TEST()
      {
         var programWithBlock = _parser.Parse("PROGRAM{{}}");
         Assert.AreEqual(programWithBlock.Block.Items.Count, 1);
         Assert.AreEqual(programWithBlock.Block.Items[0].GetType(), typeof(Block));
      }

      [TestMethod]
      public void LeftClick_TEST()
      {
         var programWithLeftClick = _parser.Parse("PROGRAM{LEFT_CLICK();}");
         Assert.AreEqual(programWithLeftClick.Block.Items.Count, 1);
         Assert.AreEqual(programWithLeftClick.Block.Items[0].GetType(), typeof(LeftClick));
      }

      [TestMethod]
      public void ForLoop_TEST()
      {
         var programWithForLoop = _parser.Parse("PROGRAM{FOR(4711){}}");
         Assert.AreEqual(programWithForLoop.Block.Items.Count, 1);
         var forLoop = programWithForLoop.Block.Items[0];
         Assert.AreEqual(forLoop.GetType(), typeof(ForLoop));
         Assert.AreEqual(((ForLoop)forLoop).RepetitionCount, 4711);
         var forLoopBody = ((ForLoop)forLoop).Body;
         Assert.AreEqual(forLoopBody.GetType(), typeof(Block));
      }

      [TestMethod]
      public void Move_TEST()
      {
         var programWithMove = _parser.Parse("PROGRAM{MOVE( 4711, 8);}");
         Assert.AreEqual(programWithMove.Block.Items.Count, 1);
         var move = programWithMove.Block.Items[0];
         Assert.AreEqual(move.GetType(), typeof(Move));
         Assert.AreEqual(((Move)move).TranslationX, 4711);
         Assert.AreEqual(((Move)move).TranslationY, 8);
      }

      [TestMethod]
      public void NoOp_TEST()
      {
         var programWithBlock = _parser.Parse("PROGRAM{;}");
         Assert.AreEqual(programWithBlock.Block.Items.Count, 1);
         Assert.AreEqual(programWithBlock.Block.Items[0].GetType(), typeof(NoOp));
      }

      [TestMethod]
      public void Pause_TEST()
      {
         var programWithPause = _parser.Parse("PROGRAM{PAUSE(4711 );}");
         Assert.AreEqual(programWithPause.Block.Items.Count, 1);
         var pause = programWithPause.Block.Items[0];
         Assert.AreEqual(pause.GetType(), typeof(Pause));
         Assert.AreEqual(((Pause)pause).Duration.TotalMilliseconds, 4711);
      }

      [TestMethod]
      public void Position_TEST()
      {
         var programWithPosition = 
            _parser.Parse(
               @"PROGRAM
                  {
            POSITION(4711,8);}");
         Assert.AreEqual(programWithPosition.Block.Items.Count, 1);
         var position = programWithPosition.Block.Items[0];
         Assert.AreEqual(position.GetType(), typeof(Position));
         Assert.AreEqual(((Position)position).X, 4711);
         Assert.AreEqual(((Position)position).Y, 8);
      }  

      [TestMethod]
      public void Program_TEST()
      {
         var emptyProgram = _parser.Parse("PROGRAM{}");
         Assert.AreEqual(emptyProgram.Block.Items.Count, 0);
      }
   }
}
