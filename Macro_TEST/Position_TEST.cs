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
         var testVisitor = new MockVisitor();
         position.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Position));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         var position = new Position { X = ConstantExpressions.Create(4711), Y = ConstantExpressions.Create(-4711) };
         block.Items.Add(position);
         var program = new Program { Body = block };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
         position.X = ConstantExpressions.Create(-4711);
         Assert.AreNotEqual(program, programClone);
      }
   }
}
