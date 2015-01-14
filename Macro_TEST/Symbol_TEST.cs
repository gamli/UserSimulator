using System.Collections.Generic;
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
      public void MacroGetHashCode_TEST()
      {
         var symbol1 = new Symbol("moo");
         var symbol2 = new Symbol("moo");
         var symbol3 = new Symbol("booo");
         var symbol4 = new Symbol(null);

         var set = new HashSet<Symbol> { symbol1 };
         Assert.IsTrue(set.Contains(symbol1));
         Assert.IsTrue(set.Contains(symbol2));
         Assert.IsFalse(set.Contains(symbol3));
         Assert.IsFalse(set.Contains(symbol4));

         set.Add(symbol2);
         Assert.AreEqual(set.Count, 1);

         set.Add(symbol3);
         Assert.IsTrue(set.Contains(symbol1));
         Assert.IsTrue(set.Contains(symbol2));
         Assert.IsTrue(set.Contains(symbol3));
         Assert.IsFalse(set.Contains(symbol4));

         set.Add(symbol4);
         Assert.IsTrue(set.Contains(symbol1));
         Assert.IsTrue(set.Contains(symbol2));
         Assert.IsTrue(set.Contains(symbol3));
         Assert.IsTrue(set.Contains(symbol4));
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
