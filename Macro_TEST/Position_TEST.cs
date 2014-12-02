using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Position_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var position = new Position();
         var testVisitor = new TestVisitor();
         position.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Position));
      }
   }
}
