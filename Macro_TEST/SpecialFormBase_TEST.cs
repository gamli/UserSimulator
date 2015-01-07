using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class SpecialFormBase_TEST : List_TEST_Base
   {
      [TestMethod]
      public void SetSpecialFormSymbol_TEST()
      {
         var quote = new Quote { Expression = new Constant(4711) };
         Assert.AreEqual("quote", quote.SpecialFormSymbol.Value);
         quote.Expressions[0] = new Symbol("mooo");
         Assert.AreEqual("mooo", quote.SpecialFormSymbol.Value);
         quote.SpecialFormSymbol = new Symbol("boom");
         Assert.AreEqual(typeof(Symbol), quote.Expressions[0].GetType());
         Assert.AreEqual("boom", ((Symbol)quote.Expressions[0]).Value);
      }
   }
}
