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
         block.Items.Add(new NoOp());
         block.Items.Add(new NoOp());
         var testVisitor = new MockVisitor();
         block.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Block));
         Assert.AreEqual(testVisitor.Macros[0].Children.Count, 2);
         Assert.AreEqual(testVisitor.Macros[0].Children[0].Macro.GetType(), typeof(NoOp));
         Assert.AreEqual(testVisitor.Macros[0].Children[1].Macro.GetType(), typeof(NoOp));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         block.Items.Add(new NoOp());
         block.Items.Add(new NoOp());
         var program = new Program { Body = block };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
      }

      [TestMethod]
      public void AddAndRemoveItem_TEST()
      {
         var block = new Block();
         var constIntValue = 0;
         block.MacroChanged += (Sender, Args) => constIntValue = Sender is ConstantExpression<int> ? ((ConstantExpression<int>)Sender).Value : 0;
         var constInt = ConstantExpressions.Create(-1);
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
