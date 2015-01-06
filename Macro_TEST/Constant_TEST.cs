using System;
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
      }
   }
}
