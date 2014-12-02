using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Move_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var move = new Move();
         var testVisitor = new TestVisitor();
         move.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Move));
      }
   }
}
