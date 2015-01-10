using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class SymbolList_TEST : List_TEST_Base<Symbol>
   {
      [TestMethod]
      public void Constructor_TEST()
      {
         var list = new SymbolList();
         Assert.IsNotNull(list.Expressions);
      }

      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var list = new SymbolList();
         list.Expressions.Add(new Symbol("sym1"));
         list.Expressions.Add(new Symbol("sym2"));
         list.Expressions.Add(new Symbol("sym3"));
         var clone = MacroCloner.Clone(list);
         Assert.AreEqual(list, clone);
      }

      [TestMethod]
      public void AddAndRemoveItem_TEST()
      {
         var symbolList = new SymbolList();
         var symbolValue = "";
         symbolList.MacroChanged +=
            (Sender, Args) =>
               {
                  var sym = Sender as Symbol;
                  symbolValue = sym != null ? sym.Value : null;
               };         
         var symbol = new Symbol(null);

         symbolList.Expressions.Add(symbol);
         Assert.AreEqual(symbolValue, null);
         symbol.Value = "sym1";
         Assert.AreEqual(symbolValue, "sym1");

         symbol.Value = "sym2";
         Assert.AreEqual(symbolValue, "sym2");

         symbolList.Expressions.Remove(symbol);
         Assert.AreEqual(symbolValue, null);
         symbol.Value = "sym3";
         Assert.AreEqual(symbolValue, null);
      }
   }
}
