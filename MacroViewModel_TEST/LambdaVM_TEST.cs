using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class LambdaVM_TEST
   {
      [TestMethod]
      public void ArgumentSymbolsVM_Property_TEST()
      {
         var argumentSymbols = new SymbolList();
         argumentSymbols.Symbols.Add(new Symbol("x"));
         argumentSymbols.Symbols.Add(new Symbol("y"));
         var lambda = new Lambda{ ArgumentSymbols = argumentSymbols, Body = new Constant(true)};
         using (var lambdaVM = new LambdaVM(lambda))
         {
            Assert.AreEqual(lambda.ArgumentSymbols, lambdaVM.ArgumentSymbolsVM.Model);
            argumentSymbols.Symbols.Add(new Symbol("x"));
            Assert.AreEqual(lambda.ArgumentSymbols, lambdaVM.ArgumentSymbolsVM.Model);
            argumentSymbols.Symbols.Add(new Symbol("y"));
            Assert.AreEqual(lambda.ArgumentSymbols, lambdaVM.ArgumentSymbolsVM.Model);
            argumentSymbols.Symbols.Remove(new Symbol("x"));
            Assert.AreEqual(lambda.ArgumentSymbols, lambdaVM.ArgumentSymbolsVM.Model);
         }
      }

      [TestMethod]
      public void BodyVM_Property_TEST()
      {
         var lambda = new Lambda { ArgumentSymbols = new SymbolList(), Body = new Constant(true) };
         using (var lambdaVM = new LambdaVM(lambda))
         {
            Assert.AreEqual(lambda.Body, lambdaVM.BodyVM.Model);
            lambda.Body = new Constant(false);
            Assert.AreEqual(lambda.Body, lambdaVM.BodyVM.Model);
         }
      }
   }
}
