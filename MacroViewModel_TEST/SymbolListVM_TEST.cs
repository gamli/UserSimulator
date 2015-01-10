using System.Linq;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class SymbolListVM_TEST
   {
      [TestMethod]
      public void Symbols_Property_TEST()
      {
         var symbolList = new SymbolList();
         foreach(var childExpression in Enumerable.Repeat(0, 8).Select(ConstantValue => new Symbol(ConstantValue.ToString())))
            symbolList.Symbols.Add(childExpression);
         symbolList.Symbols.RemoveAt(3);
         using(var listVM = new SymbolListVM(symbolList))
         {
            Assert.AreEqual(symbolList.Symbols.Count, listVM.ExpressionsVM.Count);
            for (var i = 0; i < symbolList.Symbols.Count; i++)
               Assert.AreSame(symbolList.Symbols[i], listVM.ExpressionsVM[i].Model);
         }
      }
   }
}
