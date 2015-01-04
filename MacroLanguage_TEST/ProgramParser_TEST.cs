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
         var programWithLeftClick = _parser.Parse("PROGRAM LEFT_CLICK()");
         Assert.AreEqual(programWithLeftClick.Body.GetType(), typeof(LeftClick));
      }

      [TestMethod]
      public void ForLoop_TEST()
      {
         var programWithForLoop = _parser.Parse("PROGRAM FOR(4711){}");
         var forLoop = programWithForLoop.Body;
         Assert.AreEqual(forLoop.GetType(), typeof(ForLoop));
         Assert.AreEqual(((ForLoop)forLoop).RepetitionCount, new Constant(4711));
         var forLoopBody = ((ForLoop)forLoop).Body;
         Assert.AreEqual(forLoopBody.GetType(), typeof(Block));
      }

      [TestMethod]
      public void Windowshot_TEST()
      {
         var programWithIfWindowshot = _parser.Parse("PROGRAM IF(WINDOWSHOT(4711, -4711, \"nonExistingTestImage\")){}");
         var ifStatetment = programWithIfWindowshot.Body;
         Assert.AreEqual(ifStatetment.GetType(), typeof(If));
         var windowshot = ((If)ifStatetment).Expression;
         Assert.AreEqual(windowshot.GetType(), typeof(Windowshot));
         Assert.AreEqual(
            windowshot, 
            new Windowshot 
               {
                  PositionX = new Constant(4711),
                  PositionY = new Constant(-4711),
                  ImageUrl = new Constant("nonExistingTestImage") 
               });
      }

      [TestMethod]
      public void Move_TEST()
      {
         var programWithMove = _parser.Parse("PROGRAM MOVE( 4711, -4711)");
         var move = programWithMove.Body;
         Assert.AreEqual(move.GetType(), typeof(Move));
         Assert.AreEqual(((Move)move).TranslationX, new Constant(4711));
         Assert.AreEqual(((Move)move).TranslationY, new Constant(-4711));
      }

      [TestMethod]
      public void Pause_TEST()
      {
         var programWithPause = _parser.Parse("PROGRAM PAUSE(4711 )");
         var pause = programWithPause.Body;
         Assert.AreEqual(pause.GetType(), typeof(Pause));
         Assert.AreEqual(((Pause)pause).Duration, new Constant(4711));
      }

      [TestMethod]
      public void Position_TEST()
      {
         var programWithPosition = 
            _parser.Parse(
               @"PROGRAM
                  {
            POSITION(4711,-4711)}");
         var block = programWithPosition.Body;
         Assert.IsTrue(block is Block);
         var position = ((Block)block).Items[0];
         Assert.AreEqual(position.GetType(), typeof(Position));
         Assert.AreEqual(((Position)position).X, new Constant(4711));
         Assert.AreEqual(((Position)position).Y, new Constant(-4711));
      }

      [TestMethod]
      public void If_TEST()
      {
         var programWithIf =
            _parser.Parse(
               @"PROGRAM
                  {
            IF(True){}}");
         var block = programWithIf.Body;
         Assert.IsTrue(block is Block);
         var ifStatement = ((Block)block).Items[0];
         Assert.AreEqual(ifStatement.GetType(), typeof(If));
         Assert.AreEqual(((If)ifStatement).Expression, new Constant(true));
         programWithIf =
            _parser.Parse(
               @"PROGRAM
                  {
            IF(False){}}");
         Assert.AreEqual(((If)((Block)programWithIf.Body).Items[0]).Expression, new Constant(false));
      }

      [TestMethod]
      public void VariableAssignment_TEST()
      {
         VariableAssignment_TEST(Tuple.Create("True", true), Tuple.Create("False", false));
         VariableAssignment_TEST(Tuple.Create("\"4711\"", "4711"), Tuple.Create("\"-4711\"", "-4711"));
         VariableAssignment_TEST(
            Tuple.Create("\"The cow says: \\\"moooo\\\"\"", "The cow says: \"moooo\""),
            Tuple.Create("\"The dog says: \\\"meeeowwww\\\"\"", "The dog says: \"meeeowwww\""));
         VariableAssignment_TEST(Tuple.Create("4711", 4711), Tuple.Create("-4711", -4711));
         VariableAssignment_TEST(Tuple.Create("4711.5", 4711.5), Tuple.Create("-4711.5", -4711.5));
      }

      private void VariableAssignment_TEST<T>(params Tuple<string, T>[] AssignmentExpressionsAndExpectedValues)
      {
         for (var i = 0; i < AssignmentExpressionsAndExpectedValues.Length; i++)
            VariableAssignment_TEST(AssignmentExpressionsAndExpectedValues[i]);
      }
      private void VariableAssignment_TEST<T>(Tuple<string, T> AssignmentExpressionAndExpectedValue)
      {
         var programWithVariableAssignment =
            _parser.Parse(
                  @"PROGRAM
                     {variableName123 = " + AssignmentExpressionAndExpectedValue.Item1 + " }");
         var block = programWithVariableAssignment.Body;
         Assert.IsTrue(block is Block);
         Assert.AreEqual(((Block)block).Items.Count, 1);
         var variableAssignment = ((Block)block).Items[0];
         Assert.AreEqual(variableAssignment.GetType(), typeof(VariableAssignment));
         Assert.AreEqual(((VariableAssignment)variableAssignment).Symbol, "variableName123");
         Assert.AreEqual(((VariableAssignment)variableAssignment).Expression, new Constant(AssignmentExpressionAndExpectedValue.Item2));
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
