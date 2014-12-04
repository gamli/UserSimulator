using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class NoOp_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var noOp = new NoOp();
         var testVisitor = new MockVisitor();
         noOp.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(NoOp));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         var noOp = new NoOp();
         block.Items.Add(noOp);
         var program = new Program { Body = block };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
      }

      [TestMethod]
      public void BaseClass_Equals_TEST()
      {
         var noOp = new NoOp();
         Assert.IsFalse(noOp.Equals(null));
         Assert.IsFalse(noOp.Equals(new StringBuilder()));
      }
   }
}
