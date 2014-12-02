using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Block_TEST
   {
      [TestMethod]
      public void Constructor_TEST()
      {
         var block = new Block();
         Assert.IsNotNull(block.Items);
         var testVisitor = new TestVisitor();
         block.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 0);
      }

      [TestMethod]
      public void Accept_TEST()
      {
         var block = new Block();
         block.Items.Add(new NoOp());
         block.Items.Add(new NoOp());
         var testVisitor = new TestVisitor();
         block.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 2);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(NoOp));
         Assert.AreEqual(testVisitor.Macros[1].Macro.GetType(), typeof(NoOp));
      }
   }
}
