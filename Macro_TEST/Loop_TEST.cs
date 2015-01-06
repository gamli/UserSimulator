using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Loop_TEST
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var forLoop = new Loop { Condition = new Constant(true), Body = new Constant(4711) };
         var clone = MacroCloner.Clone(forLoop);
         Assert.AreEqual(forLoop, clone);


         forLoop.Body = new Constant(-4711);
         Assert.AreNotEqual(forLoop, clone);
         forLoop.Body = new Constant(4711);
         Assert.AreEqual(forLoop, clone);
         
         forLoop.Condition = new Constant(false);
         Assert.AreNotEqual(forLoop, clone);
         forLoop.Condition = new Constant(true);
         Assert.AreEqual(forLoop, clone);
      }
   }
}
