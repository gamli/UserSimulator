using System;
using System.Diagnostics.CodeAnalysis;
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
         Assert.AreEqual(((ForLoop)forLoop).RepetitionCount, new ConstantExpression<int> { Value = 4711 });
         var forLoopBody = ((ForLoop)forLoop).Body;
         Assert.AreEqual(forLoopBody.GetType(), typeof(Block));
      }

      [TestMethod]
      public void WindowshotExpression_TEST()
      {
         var programWithIfWindowshot = _parser.Parse("PROGRAM IF(WINDOWSHOT(4711, -4711, \"nonExistingTestImage\")){}");
         var ifStatetment = programWithIfWindowshot.Body;
         Assert.AreEqual(ifStatetment.GetType(), typeof(IfStatement));
         var windowshot = ((IfStatement)ifStatetment).Expression;
         Assert.AreEqual(windowshot.GetType(), typeof(WindowshotExpression));
         Assert.AreEqual(
            windowshot, 
            new WindowshotExpression 
               {
                  PositionX = ConstantExpressions.Create(4711), 
                  PositionY = ConstantExpressions.Create(-4711),
                  ImageUrl = ConstantExpressions.Create("nonExistingTestImage") 
               });
      }

      [TestMethod]
      public void Move_TEST()
      {
         var programWithMove = _parser.Parse("PROGRAM MOVE( 4711, -4711);");
         var move = programWithMove.Body;
         Assert.AreEqual(move.GetType(), typeof(Move));
         Assert.AreEqual(((Move)move).TranslationX, ConstantExpressions.Create(4711));
         Assert.AreEqual(((Move)move).TranslationY, ConstantExpressions.Create(-4711));
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
         var programWithPause = _parser.Parse("PROGRAM PAUSE(4711 );");
         var pause = programWithPause.Body;
         Assert.AreEqual(pause.GetType(), typeof(Pause));
         Assert.AreEqual(((Pause)pause).Duration, ConstantExpressions.Create(4711));
      }

      [TestMethod]
      public void Position_TEST()
      {
         var programWithPosition = 
            _parser.Parse(
               @"PROGRAM
                  {
            POSITION(4711,-4711);}");
         var block = programWithPosition.Body;
         Assert.IsTrue(block is Block);
         var position = ((Block)block).Items[0];
         Assert.AreEqual(position.GetType(), typeof(Position));
         Assert.AreEqual(((Position)position).X, ConstantExpressions.Create(4711));
         Assert.AreEqual(((Position)position).Y, ConstantExpressions.Create(-4711));
      }

      [TestMethod]
      public void IfStatement_TEST()
      {
         var programWithPosition =
            _parser.Parse(
               @"PROGRAM
                  {
            IF(True);}");
         var block = programWithPosition.Body;
         Assert.IsTrue(block is Block);
         var ifStatement = ((Block)block).Items[0];
         Assert.AreEqual(ifStatement.GetType(), typeof(IfStatement));
         Assert.AreEqual(((IfStatement)ifStatement).Expression, ConstantExpressions.Create(true));
         programWithPosition =
            _parser.Parse(
               @"PROGRAM
                  {
            IF(False);}");
         Assert.AreEqual(((IfStatement)((Block)programWithPosition.Body).Items[0]).Expression, ConstantExpressions.Create(false));
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void Program_TEST()
      {
         var emptyProgram = _parser.Parse("PROGRAM{}");
         Assert.AreEqual(emptyProgram.Body.GetType(), typeof(Block));

         try
         {
            _parser.Parse("");
            Assert.Fail();
         }catch(ParseException)
         {
            // everything ok
         }
      }
   }
}
