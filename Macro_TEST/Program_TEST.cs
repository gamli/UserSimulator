using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Program_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var program = new Program();
         program.Block.Items.Add(new NoOp());
         var testVisitor = new TestVisitor();
         program.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Program));
         Assert.AreEqual(testVisitor.Macros[0].Children.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Children[0].Macro.GetType(), typeof(NoOp));
      }
   }
}
