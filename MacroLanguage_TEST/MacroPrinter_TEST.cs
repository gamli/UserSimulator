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
         filledBlock.Items.Add(new LeftClick());
         var filledBlockProgram =
            Program(filledBlock);
         AssertOutput(
            filledBlockProgram,
@"PROGRAM
{
   {
      LEFT_CLICK()
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
   LEFT_CLICK()
}");
      }

      [TestMethod]
      public void ForLoop_TEST()
      {
         var emptyForLoopProgram =
            Program(new ForLoop { RepetitionCount = new Constant(4711), Body = new Block() });
         AssertOutput(
            emptyForLoopProgram,
@"PROGRAM
{
   FOR(4711)
      {
      }
}");
         var forLoopWithBlockProgram =
            Program(new ForLoop { RepetitionCount = new Constant(4711), Body = new Block { } });
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
               new If
               {
                  Expression = 
                     new Windowshot 
                        { 
                           PositionX = new Constant(4711), 
                           PositionY = new Constant(-4711), 
                           ImageUrl = new Constant("nonExisting\"TestImage")
                        },
                  Body = new Block()
               });
         AssertOutput(
            windowshotProgram,
@"PROGRAM
{
   IF(WINDOWSHOT(4711, -4711, ""nonExisting\""TestImage""))
      {
      }
}");
         windowshotProgram =
            Program(
               new If
               {
                  Expression = 
                     new Windowshot 
                        { 
                           PositionX = new Constant(4711), 
                           PositionY = new Constant(-4711), 
                           ImageUrl = new Constant(null)
                        },
                  Body = new Block()
               });
         AssertOutput(
            windowshotProgram,
@"PROGRAM
{
   IF(WINDOWSHOT(4711, -4711, null))
      {
      }
}");
         windowshotProgram =
            Program(
               new If
               {
                  Expression =
                     new Windowshot
                     {
                        PositionX = new Constant(4711),
                        PositionY = new Constant(-4711),
                        ImageUrl = new Constant(null)
                     },
                  Body = new Block()
               });
         AssertOutput(
            windowshotProgram,
@"PROGRAM
{
   IF(WINDOWSHOT(4711, -4711, null))
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
                  TranslationX = new Constant(4711), 
                  TranslationY = new Constant(-4711) });
         AssertOutput(
            moveProgram,
@"PROGRAM
{
   MOVE(4711, -4711)
}");
      }

      [TestMethod]
      public void Pause_TEST()
      {
         var pauseProgram =
            Program(new Pause { Duration = new Constant(4711) });
         AssertOutput(
            pauseProgram,
@"PROGRAM
{
   PAUSE(4711)
}");
      }

      [TestMethod]
      public void Position_TEST()
      {
         var positionProgram = 
            Program(new Position { X = new Constant(4711), Y = new Constant(-4711) });
         AssertOutput(
            positionProgram,
@"PROGRAM
{
   POSITION(4711, -4711)
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
      public void If_TEST()
      {
         var ifProgram =
            Program(new If { Expression = new Constant(true), Body = new Block() });
         AssertOutput(
            ifProgram,
@"PROGRAM
{
   IF(True)
      {
      }
}");
      }

      [TestMethod]
      public void VariableAssignment_TEST()
      {
         var ifProgram =
            Program(new VariableAssignment { Symbol = "variableName", Expression = new Constant(true) });
         AssertOutput(
            ifProgram,
@"PROGRAM
{
   variableName = True
}");
         ifProgram =
            Program(new VariableAssignment { Symbol = "variableName", Expression = new Constant("moooo") });
         AssertOutput(
            ifProgram,
@"PROGRAM
{
   variableName = ""moooo""
}");
         ifProgram =
            Program(new VariableAssignment { Symbol = "variableName", Expression = new Constant(-4711) });
         AssertOutput(
            ifProgram,
@"PROGRAM
{
   variableName = -4711
}");
         ifProgram =
            Program(new VariableAssignment { Symbol = "variableName", Expression = new Constant(-4711.5) });
         AssertOutput(
            ifProgram,
@"PROGRAM
{
   variableName = -4711.5
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
         Assert.AreEqual(Program, parsedProgram);
         Assert.AreEqual(output, ExpectedOutput);
      }

      private string Print(Program Program)
      {
         return new MacroPrinter(Program).Print();
      }
   }
}
