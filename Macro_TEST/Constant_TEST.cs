using System.Collections.Generic;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Constant_TEST
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var constant = new Constant("moo");
         var clone = MacroCloner.Clone(constant);
         Assert.AreEqual(constant, clone);

         constant.Value = "foo";
         Assert.AreNotEqual(constant, clone);

         constant.Value = "moo";
         Assert.AreEqual(constant, clone);

         constant.Value = null;
         Assert.AreNotEqual(constant, clone);

         clone.Value = null;
         Assert.AreEqual(constant, clone);
      }

      [TestMethod]
      public void MacroGetHashCode_TEST()
      {
         var constant1 = new Constant("moo");
         var constant2 = new Constant("moo");
         var constant3 = new Constant("booo");
         var constant4 = new Constant(null);

         var set = new HashSet<Constant> { constant1 };
         Assert.IsTrue(set.Contains(constant1));
         Assert.IsTrue(set.Contains(constant2));
         Assert.IsFalse(set.Contains(constant3));
         Assert.IsFalse(set.Contains(constant4));

         set.Add(constant2);
         Assert.AreEqual(set.Count, 1);

         set.Add(constant3);
         Assert.IsTrue(set.Contains(constant1));
         Assert.IsTrue(set.Contains(constant2));
         Assert.IsTrue(set.Contains(constant3));
         Assert.IsFalse(set.Contains(constant4));

         set.Add(constant4);
         Assert.IsTrue(set.Contains(constant1));
         Assert.IsTrue(set.Contains(constant2));
         Assert.IsTrue(set.Contains(constant3));
         Assert.IsTrue(set.Contains(constant4));
      }

      [TestMethod]
      public void MacroChangedEvent_TEST()
      {
         var constant = new Constant("moo");
         var test = false;
         constant.MacroChanged += (Sender, Args) => test = true;

         constant.Value = "x";
         Assert.IsTrue(test);
         test = false;

         constant.Value = null;
         Assert.IsTrue(test);
         test = false;

         constant.Value = false;
         Assert.IsTrue(test);
      }
   }
}
