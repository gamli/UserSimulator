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
         var programWithBlock = _parser.Parse("PROGRAM{}");
         Assert.AreEqual(programWithBlock.Body.GetType(), typeof(Block));
         Assert.AreEqual(((Block)programWithBlock.Body).Items.Count, 0);
      }

      [TestMethod]
      public void LeftClick_TEST()
      {
         var programWithLeftClick = _parser.Parse("PROGRAM LEFT_CLICK();");
         Assert.AreEqual(programWithLeftClick.Body.GetType(), typeof(LeftClick));
      }

      [TestMethod]
      public void ForLoop_TEST()
      {
         var programWithForLoop = _parser.Parse("PROGRAM FOR(4711){}");
         var forLoop = programWithForLoop.Body;
         Assert.AreEqual(forLoop.GetType(), typeof(ForLoop));
         Assert.AreEqual(((ForLoop)forLoop).RepetitionCount, 4711);
         var forLoopBody = ((ForLoop)forLoop).Body;
         Assert.AreEqual(forLoopBody.GetType(), typeof(Block));
      }

      [TestMethod]
      public void Move_TEST()
      {
         var programWithMove = _parser.Parse("PROGRAM MOVE( 4711, 8);");
         var move = programWithMove.Body;
         Assert.AreEqual(move.GetType(), typeof(Move));
         Assert.AreEqual(((Move)move).TranslationX, 4711);
         Assert.AreEqual(((Move)move).TranslationY, 8);
      }

      [TestMethod]
      public void NoOp_TEST()
      {
         var programWithBlock = _parser.Parse("PROGRAM;");
         Assert.AreEqual(programWithBlock.Body.GetType(), typeof(NoOp));
      }

      [TestMethod]
      public void Pause_TEST()
      {
         var programWithPause = _parser.Parse("PROGRAMPAUSE(4711 );");
         var pause = programWithPause.Body;
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
         var block = programWithPosition.Body;
         Assert.IsTrue(block is Block);
         var position = ((Block)block).Items[0];
         Assert.AreEqual(position.GetType(), typeof(Position));
         Assert.AreEqual(((Position)position).X, 4711);
         Assert.AreEqual(((Position)position).Y, 8);
      }  

      [TestMethod]
      public void Program_TEST()
      {
         var emptyProgram = _parser.Parse("PROGRAM{}");
         Assert.AreEqual(emptyProgram.Body.GetType(), typeof(Block));
      }
   }
}
