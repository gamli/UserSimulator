using System;
using Macro;
using MacroLanguage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroLanguage_TEST
{
   [TestClass]
   public class MacroPrinter_TEST
   {
      [TestMethod]
      public void Block_TEST()
      {
         var emptyBlockProgram =
            Program(new Block());
         AssertOutput(
            emptyBlockProgram,
@"PROGRAM
{
   {
   }
}");
         var filledBlock = new Block();
         filledBlock.Items.Add(new NoOp());
         filledBlock.Items.Add(new LeftClick());
         var filledBlockProgram =
            Program(filledBlock);
         AssertOutput(
            filledBlockProgram,
@"PROGRAM
{
   {
      ;
      LEFT_CLICK();
   }
}");
      }

      [TestMethod]
      public void LeftClick_TEST()
      {
         var leftClickProgram =
            Program(new LeftClick());
         AssertOutput(
            leftClickProgram,
@"PROGRAM
{
   LEFT_CLICK();
}");
      }

      [TestMethod]
      public void ForLoop_TEST()
      {
         var emptyForLoopProgram =
            Program(new ForLoop { RepetitionCount = ConstantExpressions.Create(4711), Body = new NoOp() });
         AssertOutput(
            emptyForLoopProgram,
@"PROGRAM
{
   FOR(4711)
      ;
}");
         var forLoopWithBlockProgram =
            Program(new ForLoop { RepetitionCount = ConstantExpressions.Create(4711), Body = new Block { } });
         AssertOutput(
            forLoopWithBlockProgram,
@"PROGRAM
{
   FOR(4711)
      {
      }
}");
      }

      [TestMethod]
      public void Windowshot_TEST()
      {
         var windowshotProgram =
            Program(
               new Windowshot 
               { 
                  PositionX = ConstantExpressions.Create(4711), 
                  PositionY = ConstantExpressions.Create(-4711), 
                  ImageUrl = ConstantExpressions.Create("nonExistingTestImage"), Body = new NoOp() });
         AssertOutput(
            windowshotProgram,
@"PROGRAM
{
   IF_WINDOWSHOT(4711, -4711, ""nonExistingTestImage"")
      ;
}");
         windowshotProgram =
            Program(
               new Windowshot
               {
                  PositionX = ConstantExpressions.Create(4711),
                  PositionY = ConstantExpressions.Create(-4711),
                  ImageUrl = ConstantExpressions.Create<string>(null),
                  Body = new NoOp()
               });
         AssertOutput(
            windowshotProgram,
@"PROGRAM
{
   IF_WINDOWSHOT(4711, -4711, null)
      ;
}");

         var windowshotWithBlockProgram =
            Program(
               new Windowshot 
               { 
                  PositionX = ConstantExpressions.Create(4711), 
                  PositionY = ConstantExpressions.Create(-4711), Body = new Block { } });
         AssertOutput(
            windowshotWithBlockProgram,
@"PROGRAM
{
   IF_WINDOWSHOT(4711, -4711, null)
      {
      }
}");
      }

      [TestMethod]
      public void Move_TEST()
      {
         var moveProgram =
            Program(
               new Move 
               { 
                  TranslationX = ConstantExpressions.Create(4711), 
                  TranslationY = ConstantExpressions.Create(-4711) });
         AssertOutput(
            moveProgram,
@"PROGRAM
{
   MOVE(4711, -4711);
}");
      }

      [TestMethod]
      public void NoOp_TEST()
      {
         var noOpProgram =
            Program(new NoOp());
         AssertOutput(
            noOpProgram,
@"PROGRAM
{
   ;
}");
      }

      [TestMethod]
      public void Pause_TEST()
      {
         var pauseProgram =
            Program(new Pause { Duration = ConstantExpressions.Create(4711) });
         AssertOutput(
            pauseProgram,
@"PROGRAM
{
   PAUSE(4711);
}");
      }

      [TestMethod]
      public void Position_TEST()
      {
         var positionProgram = 
            Program(new Position { X = ConstantExpressions.Create(4711), Y = ConstantExpressions.Create(-4711) });
         AssertOutput(
            positionProgram,
@"PROGRAM
{
   POSITION(4711, -4711);
}");
      }

      [TestMethod]
      public void Program_TEST()
      {
         var emptyProgram = Program();
         AssertOutput(
            emptyProgram,
@"PROGRAM
{
}");
      }

      [TestMethod]
      public void IfStatement_TEST()
      {
         var ifStatementProgram =
            Program(new IfStatement { Expression = ConstantExpressions.Create(true), Body = new NoOp() });
         AssertOutput(
            ifStatementProgram,
@"PROGRAM
{
   IF(True)
      ;
}");
      }

      private Program Program(MacroBase Macro)
      {
         var program = Program();
         var block = new Block();
         block.Items.Add(Macro);
         program.Body = block;
         return program;
      }

      private Program Program()
      {
         return new Program { Body = new Block() };
      }

      private void AssertOutput(Program Program, string ExpectedOutput)
      {
         var output = Print(Program);
         var parser = new ProgramParser();
         var parsedProgram = parser.Parse(output);
         Assert.AreEqual(output, ExpectedOutput);
      }

      private string Print(Program Program)
      {
         return new MacroPrinter(Program).Print();
      }
   }
}
