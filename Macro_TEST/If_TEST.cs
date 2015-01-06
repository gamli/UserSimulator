using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class If_TEST
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var ifExpr = new If { Condition = new Constant(true), Consequent = new Constant(true), Alternative = new Constant(false) };
         var clone = MacroCloner.Clone(ifExpr);
         Assert.AreEqual(ifExpr, clone);

         ifExpr.Alternative = new Constant(true);
         Assert.AreNotEqual(ifExpr, clone);
         ifExpr.Alternative = new Constant(false);
         Assert.AreEqual(ifExpr, clone);

         ifExpr.Consequent = new Constant(false);
         Assert.AreNotEqual(ifExpr, clone);
         ifExpr.Consequent = new Constant(true);
         Assert.AreEqual(ifExpr, clone);

         ifExpr.Condition = new Constant(false);
         Assert.AreNotEqual(ifExpr, clone);
         ifExpr.Condition = new Constant(true);
         Assert.AreEqual(ifExpr, clone);
      }
   }
}
