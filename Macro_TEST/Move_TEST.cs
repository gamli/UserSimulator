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
         var testVisitor = new MockVisitor();
         move.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Move));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         var move = new Move { TranslationX = new Constant(4711), TranslationY = new Constant(-4711) };
         block.Items.Add(move);
         var program = new Program { Body = block };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
         move.TranslationX = new Constant(-4711);
         Assert.AreNotEqual(program, programClone);
      }
   }
}
