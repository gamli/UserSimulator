using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Loop_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var forLoop = new Loop { Body = new Block() };
         var testVisitor = new MockVisitor();
         forLoop.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Loop));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         var forLoop = new Loop { Body = new Constant(4711), Body = new Block() };
         block.Items.Add(forLoop);
         var program = new Program { Body = block };
         var programClone = new MacroCloner(program).Clone();
         Assert.AreEqual(program, programClone);
         forLoop.Body = new Constant(-4711);
         Assert.AreNotEqual(program, programClone);
      }
   }
}
