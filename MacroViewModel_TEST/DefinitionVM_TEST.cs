using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class DefinitionVM_TEST
   {
      [TestMethod]
      public void SymbolVM_Property_TEST()
      {
         var definition = new Definition { Symbol = new Symbol("var"), Expression = new Constant(-4711) };
         using (var definitionVM = new DefinitionVM(definition))
         {
            Assert.AreEqual(definition.Symbol, definitionVM.SymbolVM.Model);
            Assert.AreEqual(definition.Expression, definitionVM.ExpressionVM.Model);
            definition.Symbol = new Symbol("rav");
            definition.Expression = new Constant(-1147);
            Assert.AreEqual(definition.Symbol, definitionVM.SymbolVM.Model);
            Assert.AreEqual(definition.Expression, definitionVM.ExpressionVM.Model);
         }
      }
   }
}
