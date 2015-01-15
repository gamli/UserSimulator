using Macro;
using MacroLanguage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroLanguage_TEST
{
   [TestClass]
   public class MacroPrinter_TEST
   {
      [TestMethod]
      public void Print_TEST()
      {
         var testCase = MacroPrintParseTestCase.TestCase();

         AssertOutput(testCase.Item2, testCase.Item1, false);
      }

      [TestMethod]
      public void PrintWithLinebreaks_TEST()
      {
         var testCase = new MacroParser().Parse("(if True (lambda (x y z) (z x y)) (eq True False))");

         AssertOutput(testCase, "(if True\n\t(lambda (x y\n\t\tz)\n\t\t(z x\n\t\t\ty))\n\t(eq True\n\t\tFalse))", true);
      }

      private void AssertOutput(MacroBase Macro, string ExpectedOutput, bool Linebreaks)
      {
         var output = Print(Macro, Linebreaks);
         Assert.AreEqual(output, ExpectedOutput);
         var parser = new MacroParser();
         var parsedProgram = parser.Parse(output);
         Assert.AreEqual(Macro, parsedProgram);
      }

      private string Print(MacroBase Macro, bool Linebreaks)
      {
         return MacroPrinter.Print(Macro, Linebreaks);
      }
   }
}
