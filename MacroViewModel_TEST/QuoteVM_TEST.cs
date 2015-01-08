using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class QuoteVM_TEST
   {
      [TestMethod]
      public void ExpressionVM_Property_TEST()
      {
         var quote = new Quote { Expression = new Constant(4711) };
         using (var quoteVM = new QuoteVM(quote))
         {
            Assert.AreEqual(quote.Expression, quoteVM.ExpressionVM.Model);
            quote.Expression = new Constant(1147);
            Assert.AreEqual(quote.Expression, quoteVM.ExpressionVM.Model);
         }
      }
   }
}
