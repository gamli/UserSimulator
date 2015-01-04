﻿using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class If_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var ifStatement = new If { Condition = new Constant(true), Alternative = new Block() };
         var testVisitor = new MockVisitor();
         ifStatement.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 2);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Constant));
         Assert.AreEqual(testVisitor.Macros[1].Macro.GetType(), typeof(If));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         var ifStatement = new If { Condition = new Constant(true), Alternative = new Block() };
         block.Items.Add(ifStatement);
         var program = new Program { Body = block };
         var programClone = new MacroCloner(program).Clone();
         Assert.AreEqual(program, programClone);
         ifStatement.Condition = new Constant(false);
         Assert.AreNotEqual(program, programClone);
      }
   }
}
