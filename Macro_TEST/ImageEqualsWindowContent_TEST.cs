using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class ImageEqualsWindowContent_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var imageEqualsWindowContent = new ImageEqualsWindowContent();
         var testVisitor = new MockVisitor();
         imageEqualsWindowContent.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(ImageEqualsWindowContent));
      }
   }
}
