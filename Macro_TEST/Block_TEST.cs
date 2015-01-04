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
         var testVisitor = new MockVisitor();
         block.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Block));
      }

      [TestMethod]
      public void Accept_TEST()
      {
         var block = new Block();
         block.Items.Add(new Block());
         block.Items.Add(new Block());
         var testVisitor = new MockVisitor();
         block.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Block));
         Assert.AreEqual(testVisitor.Macros[0].Children.Count, 2);
         Assert.AreEqual(testVisitor.Macros[0].Children[0].Macro.GetType(), typeof(Block));
         Assert.AreEqual(testVisitor.Macros[0].Children[1].Macro.GetType(), typeof(Block));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         block.Items.Add(new Block());
         block.Items.Add(new Block());
         var program = new Program { Body = block };
         var programClone = new MacroCloner(program).Clone();
         Assert.AreEqual(program, programClone);
      }

      [TestMethod]
      public void AddAndRemoveItem_TEST()
      {
         var block = new Block();
         var constIntValue = 0;
         block.MacroChanged += (Sender, Args) => constIntValue = Sender is Constant && ((Constant)Sender).Value is int ? (int)((Constant)Sender).Value : 0;
         var constInt = new Constant(-1);
         block.Items.Add(constInt);
         constInt.Value = 0;
         Assert.AreEqual(constIntValue, 0);
         constInt.Value = -4711;
         Assert.AreEqual(constIntValue, -4711);
         block.Items.Remove(constInt);
         constInt.Value = 4711;
         Assert.AreEqual(constIntValue, 0);
      }
   }
}
