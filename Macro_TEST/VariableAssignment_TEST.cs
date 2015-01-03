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
         var variableAssignment = new VariableAssignment<bool> { Symbol = "varName", Expression = ConstantExpressions.Create(true) };
         var block = new Block();
         block.Items.Add(variableAssignment);
         var testVisitor = new MockVisitor();
         variableAssignment.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 2);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(ConstantExpression<bool>));
         Assert.AreEqual(testVisitor.Macros[1].Macro.GetType(), typeof(VariableAssignment<bool>));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         var variableAssignment = new VariableAssignment<bool> { Symbol = "varName", Expression = ConstantExpressions.Create(true) };
         block.Items.Add(variableAssignment);
         var program = new Program { Body = block };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
         variableAssignment.Expression = ConstantExpressions.Create(false);
         Assert.AreNotEqual(program, programClone);
         variableAssignment.Symbol = "otherVarName";
         Assert.AreNotEqual(program, programClone);
      }
   }
}
