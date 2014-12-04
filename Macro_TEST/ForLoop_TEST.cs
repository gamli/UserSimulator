using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class ForLoop_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var forLoop = new ForLoop { Body = new NoOp() };
         var testVisitor = new MockVisitor();
         forLoop.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(ForLoop));
      }
   }
}
