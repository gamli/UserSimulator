using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Quote_TEST : List_TEST_Base
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var quote = new Quote { Expression = new Constant(4711) };
         var clone = MacroCloner.Clone(quote);
         AssertListsAreEqual(quote, clone);


         quote.Expression = new Constant(-4711);
         AssertListsAreNotEqual(quote, clone);
         quote.Expression = new Constant(4711);
         AssertListsAreEqual(quote, clone);
      }
   }
}
