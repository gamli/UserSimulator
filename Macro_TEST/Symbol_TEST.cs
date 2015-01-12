using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Symbol_TEST
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var symbol = new Symbol("moo");
         var clone = MacroCloner.Clone(symbol);
         Assert.AreEqual(symbol, clone);

         symbol.Value = "foo";
         Assert.AreNotEqual(symbol, clone);

         symbol.Value = "moo";
         Assert.AreEqual(symbol, clone);
      }

      [TestMethod]
      public void MacroChangedEvent_TEST()
      {
         var symbol = new Symbol("moo");
         var test = false;
         symbol.MacroChanged += (Sender, Args) => test = true;
         
         symbol.Value = "x";
         Assert.IsTrue(test);
         test = false;

         symbol.Value = null;
         Assert.IsTrue(test);
         test = false;

         symbol.Value = "y";
         Assert.IsTrue(test);
      }
   }
}
