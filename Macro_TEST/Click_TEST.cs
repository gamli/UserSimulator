using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Click_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var click = new LeftClick();
         var testVisitor = new MockVisitor();
         click.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(LeftClick));
      }
   }
}
