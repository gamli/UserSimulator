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
         var block = new Block();
         block.Items.Add(new Block());
         var program = new Program { Body = block };
         var testVisitor = new MockVisitor();
         program.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Program));
         Assert.AreEqual(testVisitor.Macros[0].Children.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Children[0].Macro.GetType(), typeof(Block));
         Assert.AreEqual(testVisitor.Macros[0].Children[0].Children.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Children[0].Children[0].Macro.GetType(), typeof(Block));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var program = new Program { Body = new Block() };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
      }
   }
}
