using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class MacroBase_TEST
   {
      [TestMethod]
      public void Equals_TEST()
      {
         var constant = new Constant("mooo");
         // ReSharper disable once EqualExpressionComparison - for reference equals. is this code even necessary in MacroBase???
         Assert.IsTrue(constant.Equals(constant));
         Assert.AreNotEqual(constant, new Symbol((string)constant.Value));
         Assert.IsFalse(constant.Equals(null));
      }
   }
}
