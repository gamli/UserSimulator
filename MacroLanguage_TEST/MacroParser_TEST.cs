using System.Diagnostics.CodeAnalysis;
using Macro;
using MacroLanguage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroLanguage_TEST
{
   [TestClass]
   public class MacroParser_TEST
   {
      [TestInitialize]
      public void TEST_INITIALIZE()
      {
         _parser = new MacroParser();
      }

      private MacroParser _parser;
      
      [TestMethod]
      public void Parse_TEST()
      {
         var testCase = MacroPrintParseTestCase.TestCase();

         AssertParseResult(testCase.Item1, testCase.Item2);
      }

      [TestMethod]
      public void NilAlias_TEST()
      {
         AssertParseResult("nil", new List());
         AssertParseResult("()", new List());
      }

      [TestMethod]
      public void QuoteSyntacticSugar_TEST()
      {
         AssertParseResult("(quote (fun))", SpecialForms.Quote(new List(new Symbol("fun"))));
         AssertParseResult("'(fun)", SpecialForms.Quote(new List(new Symbol("fun"))));

         AssertParseResult("(quote fun)", SpecialForms.Quote(new Symbol("fun")));
         AssertParseResult("'fun", SpecialForms.Quote(new Symbol("fun")));

         AssertParseResult("(quote \"fun\")", SpecialForms.Quote(new Constant("fun")));
         AssertParseResult("'\"fun\"", SpecialForms.Quote(new Constant("fun")));
      }

      private void AssertParseResult(string Text, MacroBase ExpectedMacro)
      {
         var macro = _parser.Parse(Text);
         Assert.AreEqual(ExpectedMacro, macro);
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void Fail_TEST()
      {
         Assert.IsNull(_parser.Parse("("));

         try
         {
            _parser.Parse(")");
            Assert.Fail();
         }
         catch (ParseException)
         {
            // everything ok
         }
      }
   }
}
