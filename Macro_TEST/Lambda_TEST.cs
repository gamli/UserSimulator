using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Lambda_TEST : List_TEST_Base<ExpressionBase>
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var argumentSymbolsX = new SymbolList();
         argumentSymbolsX.Expressions.Add(new Symbol("x"));
         var lambda = new Lambda { ArgumentSymbols = argumentSymbolsX, Body = new ProcedureCall { Procedure = new Symbol("x") } };
         var clone = MacroCloner.Clone(lambda);
         Assert.AreEqual(lambda, clone);

         lambda.Body = new ProcedureCall { Procedure = new Symbol("y") };
         AssertListsAreNotEqual(lambda, clone);
         lambda.Body = new ProcedureCall { Procedure = new Symbol("x") };
         AssertListsAreEqual(lambda, clone);

         var argumentSymbolsY = new SymbolList();
         argumentSymbolsY.Expressions.Add(new Symbol("y"));
         lambda.ArgumentSymbols = argumentSymbolsY;
         AssertListsAreNotEqual(lambda, clone);
         lambda.ArgumentSymbols = MacroCloner.Clone(argumentSymbolsX);
         AssertListsAreEqual(lambda, clone);
      }
   }
}
