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
