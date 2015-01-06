using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class If_TEST : List_TEST_Base
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var ifExpr = new If { Condition = new Constant(true), Consequent = new Constant(true), Alternative = new Constant(false) };
         var clone = MacroCloner.Clone(ifExpr);
         Assert.AreEqual(ifExpr, clone);

         ifExpr.Alternative = new Constant(true);
         AssertListsAreNotEqual(ifExpr, clone);
         ifExpr.Alternative = new Constant(false);
         AssertListsAreEqual(ifExpr, clone);

         ifExpr.Consequent = new Constant(false);
         AssertListsAreNotEqual(ifExpr, clone);
         ifExpr.Consequent = new Constant(true);
         AssertListsAreEqual(ifExpr, clone);

         ifExpr.Condition = new Constant(false);
         AssertListsAreNotEqual(ifExpr, clone);
         ifExpr.Condition = new Constant(true);
         AssertListsAreEqual(ifExpr, clone);
      }
   }
}
