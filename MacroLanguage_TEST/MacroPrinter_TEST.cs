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

         AssertOutput(testCase.Item2, testCase.Item1);
      }

      private void AssertOutput(MacroBase Macro, string ExpectedOutput)
      {
         var output = Print(Macro);
         var parser = new MacroParser();
         var parsedProgram = parser.Parse(output);
         Assert.AreEqual(Macro, parsedProgram);
         Assert.AreEqual(output, ExpectedOutput);
      }

      private string Print(MacroBase Macro)
      {
         return MacroPrinter.Print(Macro);
      }
   }
}
