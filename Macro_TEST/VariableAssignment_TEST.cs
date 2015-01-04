using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class VariableAssignment_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var variableAssignment = new VariableAssignment { Symbol = "varName", Expression = new Constant(true) };
         var block = new Block();
         block.Items.Add(variableAssignment);
         var testVisitor = new MockVisitor();
         variableAssignment.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 2);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Constant));
         Assert.AreEqual(testVisitor.Macros[1].Macro.GetType(), typeof(VariableAssignment));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         var variableAssignment = new VariableAssignment { Symbol = "varName", Expression = new Constant(true) };
         block.Items.Add(variableAssignment);
         var program = new Program { Body = block };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
         variableAssignment.Expression = new Constant(false);
         Assert.AreNotEqual(program, programClone);
         variableAssignment.Symbol = "otherVarName";
         Assert.AreNotEqual(program, programClone);
      }
   }
}
